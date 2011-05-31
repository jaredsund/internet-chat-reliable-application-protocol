using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TCP_Client_Test
{
    class ConnThread
    {
        private BackgroundWorker worker;
        private System.Windows.Forms.ListBox listbox;
        public string server;
        public Int32 port;
        ClientConn myClientConn;
        private string userName;

        xmlChannelRequestGen xCR;

        ~ConnThread()
        {
            myClientConn.Dispose();
        }

        public ConnThread(ref System.Windows.Forms.ListBox listbox, string server, Int32 port, string userName)
        {
            xCR = new xmlChannelRequestGen(userName );

            this.listbox = listbox;
            this.server = server;
            this.port = port;
            this.userName = userName;
            
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
            myClientConn = new ClientConn(ref worker, ref e, server, port, userName );
            myClientConn.Connect();
        }

        public void sendMessage(string message)
        {
            myClientConn.sendMessage(xCR.postMessage(message));
        }

        public void closeConn()
        {
            myClientConn.sendMessage(xCR.closeConn());
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage )
            {
                case 0:
                    ClientConn.CurrentState state = (ClientConn.CurrentState)e.UserState;
                    port = state.port;
                    displayMessage(String.Format("{0}, {1}", port.ToString(), state.message));
                    break;
                case 99:
                    displayMessage(String.Format("Error: {0}", e.UserState.ToString()));
                    break;
            }

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            displayMessage("done");
        }

        private void displayMessage(string message)
        {
            listbox.Items.Add(message);
            listbox.Items.Add("");
            listbox.TopIndex = listbox.Items.Count - 1;
        }
    }
}
