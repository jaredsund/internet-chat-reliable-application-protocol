﻿using System;
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

        ~ConnThread()
        {
            myClientConn.Dispose();
        }

        public ConnThread(ref System.Windows.Forms.ListBox listbox, string server, Int32 port)
        {
            this.listbox = listbox;
            this.server = server;
            this.port = port;
            
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
            myClientConn = new ClientConn(ref worker, ref e, server, port);
            myClientConn.Connect();
        }

        public void sendMessage(string message)
        {
            myClientConn.sendMessage(message);
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ClientConn.CurrentState state = (ClientConn.CurrentState)e.UserState;
            
            port = state.port;

            listbox.Items.Add(String.Format("{0}, {1}", port.ToString(), state.message));

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            listbox.Items.Add("done");
        }
    }
}