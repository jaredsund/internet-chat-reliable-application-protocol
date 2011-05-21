using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

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
        ArrayList channels;

        public ControllerThread(ref  System.Windows.Forms.ListBox listbox, ref  System.Windows.Forms.ListBox listbox2)
        {
            this.listbox = listbox;
            this.listbox2 = listbox2;
            channels = new ArrayList();

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }

         public void killThread()
        {
            worker.CancelAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
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

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        worker.ReportProgress (0, String.Format ("Received: {0}", data));


                        xmlCommandParser xC = new xmlCommandParser(data);

                        switch (xC.command)
                        {
                            case "EnumChan":
                                break;
                            case "CreateChan":
                                
                                channels.Add(new ChannelThread(ref listbox2, xC.data));
                                xmlResponseGen xG = new xmlResponseGen();
                                data = xG.response("Success");
                                break;
                            case "DestChan":
                                break;
                            case "AuthClient":
                                break;
                            case "KillClient":
                                break;
                            case "SetMaxChan":
                                break;
                            case "SetMaxClients":
                                break;
                            case "SysMessage":
                                break;
                            case "Version":
                                break;
                            default:
                                break;
                        }



                        // Process the data sent by the client.
                        

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        worker.ReportProgress(0, String.Format("Sent: {0}", data));
                    }

                    // Shutdown and end connection
                    client.Close();
                }
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
            if (e.ProgressPercentage == 0)
            {

                listbox.Items.Insert(0, String.Format("{2} - {0}: {1}", "Controller", e.UserState.ToString(), DateTime.Now.ToString()));
            }
            else if (e.ProgressPercentage == 99)
            {
                listbox.Items.Insert(0, String.Format("{2} - Error: {0}, {1}", "Controller", e.UserState.ToString(), DateTime.Now.ToString()));
            }
           
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }
    }
}
