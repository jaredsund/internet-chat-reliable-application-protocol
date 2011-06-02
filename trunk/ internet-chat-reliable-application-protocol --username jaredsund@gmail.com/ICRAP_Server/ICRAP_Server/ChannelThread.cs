using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ICRAP_Server
{
    class ChannelThread
    {
        private BackgroundWorker worker;
        private System.Windows.Forms.ListBox  listbox;
        private Int32 port;
        private string ChannelName;
        //private string _id;
        private ArrayList clients;
        private int _maxClients;

        TcpListener server;

        private xmlResponseGen xRG;
        
        public delegate void SetTextCallback(String text);

        public ChannelThread(ref System.Windows.Forms.ListBox listbox, string ChannelName, int maxClients)
        {
            //_id = Guid.NewGuid().ToString();
            _maxClients = maxClients;
            clients = new ArrayList();
            this.listbox = listbox;
            this.ChannelName = ChannelName;
            xRG = new xmlResponseGen();

            
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }

        public string id
        {
            get { return this.port.ToString(); }
        }

        public string Name
        {
            get { return this.ChannelName; }
        }

        public Int32 PortNo
        {
            get { return this.port; }
        }

        public Int32 numUsers
        {
            get { return clients.Count; }
        }

        public int maxClients
        {
            set { _maxClients = value; }
            get { return _maxClients; }
        }

        public void killThread()
        {
            broadCastMessage("This Channel has been closed");
            foreach (ChannelClient CC in clients)
            {
                CC.killThread();
                  
            }
            server.Stop();
            worker.CancelAsync();
        }

        public void killClient(string clientID)
        {
            foreach (ChannelClient CC in clients)
            {
                if (CC.id == clientID)
                {
                    SetText(String.Format("{2} - {0}:User: {1} ({3}) has beed dropped", ChannelName, CC.username, DateTime.Now.ToString(), CC.id ));
                    clients.Remove(CC);
                    break;
                }//end if
            }//end foreach
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            server = null;
            try
            {
                //create a new tcp listener on port 0 (server assigned)
                server = new TcpListener(Dns.GetHostAddresses("localhost")[0], 0);

                // Start listening for client requests.
                server.Start();
                port = ((IPEndPoint)server.LocalEndpoint).Port;
                
                // Enter the listening loop.
                while (true)
                {
                    worker.ReportProgress(0, String.Format ("Waiting for a connection on port: {0}", port.ToString ()));
 
                    // Perform a blocking call to accept requests.
                    TcpClient client = server.AcceptTcpClient();
                    ChannelClient cc = new ChannelClient(client, ref worker, ref e);
                    clients.Add(cc);
                    worker.ReportProgress(0, String.Format ("Client Connected" ));
                }
            }
            catch (Exception e2)
            {
                worker.ReportProgress(99, String.Format("Exception: {0}", e2));
                server.Stop();
                worker.CancelAsync();
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
                worker.CancelAsync();
            }
            worker.ReportProgress(0, String.Format ("Server Stopped" ));
        }

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.listbox .InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                listbox.Invoke(d, new object[] { text });
            }
            else
            {
                this.listbox.Items.Insert (0, text);
            }
        }

        public void broadCastMessage(string message)
        {
            foreach (ChannelClient c in clients)
            {
                c.sendMessage(message);
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string id = "";
            string userName = "";

            switch (e.ProgressPercentage)
            {
                case 0:// general message
                    SetText(String.Format("{2} - {0}: {1}", ChannelName, e.UserState.ToString(), DateTime.Now.ToString()));
                    break;
                case 1://post message
                    broadCastMessage(e.UserState.ToString());
                    break;
                case 2://enum clients
                    id = e.UserState.ToString().Split(new string[] { "ID=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    foreach (ChannelClient CC in clients)
                    {
                        if (CC.id == id)
                        {
                            CC.sendMessage(xRG.responseEnumClients(ref clients ));
                            SetText(String.Format("{2} - {0}: Enumerated Clients: , {1}", ChannelName, e.UserState.ToString(), DateTime.Now.ToString()));
                            break;
                        }
                    }
                    break;
                case 3: //set maxClients for the channel
                    _maxClients = int.Parse(e.UserState.ToString());
                    SetText(String.Format("{2} - {0}: Max Clients Updated: , {1}", ChannelName, e.UserState.ToString(), DateTime.Now.ToString()));
                    break;
                case 4: //close client
                    id = e.UserState.ToString().Split(new string[] { "ID=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    foreach (ChannelClient CC in clients)
                    {
                        if (CC.id == id)
                        {
                            SetText(String.Format("{2} - {0}: {1}", ChannelName, e.UserState.ToString(), DateTime.Now.ToString()));
                            
                            clients.Remove(CC);
                            
                            break;
                        }
                    }
                    break;
                case 5: //accept user
                    bool userExists = false;
                        id = e.UserState.ToString().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        userName = e.UserState.ToString().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[0];

                    foreach (ChannelClient CC in clients)
                    {

                        if (CC.username == userName && CC.id != id)
                        {
                            CC.sendMessage("Username cannot be used, server closing connection");
                            clients.Remove(CC);
                            userExists = true;
                            break;
                        }
                    }
                    if (userExists == false)
                    {
                        broadCastMessage(String.Format("{0}: has joined", userName));
                    }
                    
                    break;
                case 99:
                    SetText(String.Format("{2} - {0}: Error: , {1}", ChannelName, e.UserState.ToString(), DateTime.Now.ToString()));
                    break;
                case 100:
                    id = e.UserState.ToString().Split(new string[] { "ID=" }, StringSplitOptions.RemoveEmptyEntries)[1];

                    foreach (ChannelClient CC in clients)
                    {
                        if (CC.id == id)
                        {
                            userName = CC.username;
                            clients.Remove(CC);
                            SetText(String.Format("{2} - {0}:User: {1} ({3}) has beed dropped", ChannelName, userName, DateTime.Now.ToString(), id));
                            break;
                        }//end if
                    }//end foreach
                    break;
            }//end switch
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            foreach (ChannelClient CC in clients)
            {
                SetText(String.Format("{2} - {0}:User: {1} ({3}) has beed dropped", ChannelName, CC.username, DateTime.Now.ToString(), CC.id));
                CC.killThread();
            }//end foreach

            SetText(String.Format("{0}: done", ChannelName));
        }

    }
}
