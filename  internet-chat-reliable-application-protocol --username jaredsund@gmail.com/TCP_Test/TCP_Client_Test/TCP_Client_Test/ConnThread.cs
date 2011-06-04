using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TCP_Client_Test
{
    class ConnThread
    {
        private BackgroundWorker worker;

        private System.Windows.Forms.ListBox listbox;
        
        private string server;
        private Int32 port;
        private string userName;

        private xmlChannelRequestGen xCR;

        private TcpClient client;
        private NetworkStream stream;

        public bool connOpen;

        public delegate void SetTextCallback(String text);

        public ConnThread(ref System.Windows.Forms.ListBox listbox, string server, Int32 port, string userName)
        {
            this.listbox = listbox;
            this.server = server;
            this.port = port;
            this.userName = userName;

            xCR = new xmlChannelRequestGen(this.userName);
            
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                client = new TcpClient(server, port);
                connOpen = true;
                stream = client.GetStream();
                send(xCR.genMessage("AcceptClient", userName, ""));

                Byte[] data = new Byte[1024];
                String responseData = "";
                while (true)
                {
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    xmlChannelParser xCP = new xmlChannelParser(responseData);
                    worker.ReportProgress(0, string.Format(xCP.data ));
                }

            }
            catch (Exception e2)
            {
                if (stream != null)
                {
                    stream.Close();
                }
                if (client != null)
                {
                    client.Close();
                }
                connOpen = false;
                //worker.ReportProgress(99, string.Format("Exception: {0}", e2));
                worker.CancelAsync();
            } 
        }

        private void send(string message)
        {
            try
            {
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message); 
                stream.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                worker.ReportProgress(99, String.Format("Exception: {0}", e));
            }
        }

        public void sendMessage(string message)
        {
            send(xCR.postMessage(message));
        }

        public void closeConn()
        {
            send(xCR.genMessage("CloseConn", userName, ""));
            stream.Close();
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage )
            {
                case 0:
                    SetText(String.Format(e.UserState.ToString ()));
                    break;
                case 99:
                    SetText(String.Format("Error: {0}", e.UserState.ToString()));
                    //closeConn();
                    break;
            }//end switch statement

        }

       
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(stream != null)
            stream.Close();
            if(client != null)
            client.Close();
            connOpen = false;
           // SetText("done");
        }

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.listbox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                listbox.Invoke(d, new object[] { text });
            }
            else
            {
               // this.listbox.Items.Insert(0, text);
                listbox.Items.Add(text);
                listbox.Items.Add("");
                listbox.TopIndex = listbox.Items.Count - 1;
            }
        }

        
    }
}
