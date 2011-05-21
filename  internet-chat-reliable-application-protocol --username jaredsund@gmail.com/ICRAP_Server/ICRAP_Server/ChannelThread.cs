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
        private ArrayList clients;

        public delegate void SetTextCallback(String text);



        public ChannelThread(ref System.Windows.Forms.ListBox listbox, string ChannelName)
        {
            clients = new ArrayList();
            this.listbox = listbox;
            this.ChannelName = ChannelName;
            
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
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

        public void killThread()
        {
            worker.CancelAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            TcpListener server = null;
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                //create a new tcp listener on port 0 (server assigned)
                server = new TcpListener(localAddr, 0);

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
            catch (SocketException e2)
            {
                worker.ReportProgress(99, String.Format("SocketException: {0}", e2));
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
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

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
           
            if (e.ProgressPercentage == 0)
            {
                SetText(String.Format("{2} - {0}: {1}", ChannelName, e.UserState.ToString(), DateTime.Now.ToString()));
                //listbox.Items.Insert(0, String.Format("{2} - {0}: {1}", ChannelName, e.UserState.ToString(), DateTime.Now.ToString ()));
            }
            else if (e.ProgressPercentage == 1)
            {
                foreach (ChannelClient c in clients)
                {
                    c.sendMessage(e.UserState.ToString());
                }
                //listbox.Items.Insert(0, String.Format("{2} - {0}: Sent: {1}", ChannelName, e.UserState.ToString(), DateTime.Now.ToString()));
            }
            else if (e.ProgressPercentage == 99)
            {
                listbox.Items.Insert(0, String.Format("{2} - Error: {0}, {1}", ChannelName, e.UserState.ToString(), DateTime.Now.ToString()));
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            listbox.Items.Add(String.Format ("{0}: done", ChannelName ));
        }

    }
}
