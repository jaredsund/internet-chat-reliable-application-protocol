using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

using System.IO;
using System.Net;
using System.Net.Sockets;

using System.Xml;


namespace ICRAP_Server
{
    class ControllerThread
    {
        private BackgroundWorker worker;
        private System.Windows.Forms.ListBox listbox;
        private System.Windows.Forms.ListBox listbox2;
        private ArrayList channels;
        private Int32 _port;

        private int maxChannels;
        private int maxClients;
      
        //constructor
        public ControllerThread(ref  System.Windows.Forms.ListBox listbox, ref  System.Windows.Forms.ListBox listbox2,Int32 port)
        {
            maxChannels = 5;
            maxClients = 5;
            
            this.listbox = listbox;
            this.listbox2 = listbox2;
            this._port = port;

            channels = new ArrayList();

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }

        public Int32 port
        {
            get { return _port; }
        }

         public void killThread()
        {
            worker.CancelAsync();
        }

         private void Commands(ref string data)
         {
             xmlResponseGen xRG = new xmlResponseGen();
             xmlCommandParser xCP = new xmlCommandParser(data);

             switch (xCP.command)
             {
                 case "EnumChan":
                     data = channels.Count > 0 ? xRG.responseEnumChannels(ref channels) :
                         xRG.eResponse( "No Channels Exist");
                     break;
                 case "CreateChan":
                     if (channels.Count < maxChannels)
                     {
                         channels.Add(new ChannelThread(ref listbox2, xCP.data, maxClients));
                         data = xRG.sResponse();
                     }
                     else
                     {
                         data = xRG.eResponse("Max Channel Error:  No new channels can be created");
                     }//end if
                     break;
                 case "DestChan":
                     bool killedChannel = false;
                     foreach (ChannelThread cT in channels)
                     {
                         if (cT.PortNo == int.Parse(xCP.data))
                         {
                             cT.killThread();
                             data = xRG.sResponse();
                             killedChannel = true;
                             break;
                         }
                     }
                     if(killedChannel == false )
                        data = xRG.eResponse(String.Format("Could not kill channel with port {0}", xCP.data));
                     break;
                 case "KillClient":
                     foreach (ChannelThread cT in channels)
                     {
                         cT.killClient(xCP.data);
                     }
                     xRG.sResponse();
                     break;
                 case "SetMaxChan":
                     data = int.TryParse(xCP.data, out maxChannels) ? xRG.sResponse() :
                         xRG.eResponse("Unable to parse value as number");
                     break;
                 case "SetMaxClients":
                     if (int.TryParse(xCP.data, out maxClients))
                     {
                         foreach (ChannelThread cT in channels)
                         {
                             cT.maxClients = maxClients;
                         }//end foreach loop
                         data = xRG.sResponse();
                     }
                     else
                     {
                         data = xRG.eResponse("Unable to parse value as number");
                     }//end iff
                     break;
                 case "SysMessage":
                     foreach (ChannelThread cT in channels)
                     {
                         cT.broadCastMessage(xCP.data);
                     }
                     data = xRG.sResponse();
                     break;
                 case "Version":
                     data = xRG.sResponse(Assembly.GetExecutingAssembly().GetName().Version.ToString());
                     break;
                 default:
                     data = xRG.eResponse(String.Format ("{0}: Command not understood", xCP.command));
                     break;
             }//end switch
            
         }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            xmlResponseGen xG = new xmlResponseGen();
            TcpListener server = null;

            //add default channels
            channels.Add(new ChannelThread(ref listbox2, "The Question Room", maxClients));
            channels.Add(new ChannelThread(ref listbox2, "The Answer Room", maxClients));
            channels.Add(new ChannelThread(ref listbox2, "The New Room", maxClients));

            try
            {
                server = new TcpListener(Dns.GetHostAddresses("localhost")[0], _port);
    
                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[1024];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    worker.ReportProgress(0, "Waiting for connection ...");
                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
  
                    worker.ReportProgress(0, "Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i=0;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        worker.ReportProgress (0, String.Format ("Received: {0}", data));

                        Commands(ref data); //processes and runs the commands

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        worker.ReportProgress(0, String.Format("Sent: {0}", data));

                    }//end while stream.Read

                    // Shutdown and end connection
                    client.Close();
                }//end while (true)
            }
            catch (SocketException e2)
            {
                worker.ReportProgress(99, String.Format("SocketException: {0}", e2));
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 0:
                    listbox.Items.Insert(0, String.Format("{2} - {0}: {1}", "Controller", e.UserState.ToString(), DateTime.Now.ToString()));
                    break;
                case 99:
                    listbox.Items.Insert(0, String.Format("{2} - Error: {0}, {1}", "Controller", e.UserState.ToString(), DateTime.Now.ToString()));
                    break;

            }//end switch case   
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }
    }
}
