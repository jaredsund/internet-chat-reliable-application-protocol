using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ICRAP_Server
{
    class ChannelThread
    {
        private BackgroundWorker worker;
        private System.Windows.Forms.ListBox  listbox;
        public Int32 port;
        public string ChannelName;

        public ChannelThread(ref System.Windows.Forms.ListBox listbox, string ChannelName)
        {
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

        public void killThread()
        {
            worker.CancelAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker object that raised this event.
            System.ComponentModel.BackgroundWorker worker2;
            worker2 = (System.ComponentModel.BackgroundWorker)sender;

            Channel myChannel = new Channel();
            myChannel.startChannel(worker2, e, ChannelName );
            
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Channel.CurrentState state = (Channel.CurrentState)e.UserState;

            port = state.port;

            listbox.Items.Add(String.Format("{0}, {1}: {2}", port.ToString(), state.channelName, state.message));

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            listbox.Items.Add("done");
        }

    }
}
