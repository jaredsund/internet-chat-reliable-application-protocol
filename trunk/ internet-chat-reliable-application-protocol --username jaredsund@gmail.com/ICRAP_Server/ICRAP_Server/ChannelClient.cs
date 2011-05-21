using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;

namespace ICRAP_Server
{
    class ChannelClient
    {
        private TcpClient client;
        private NetworkStream stream;
        private BackgroundWorker worker;

        private System.ComponentModel.BackgroundWorker workerChannel;
        private System.ComponentModel.DoWorkEventArgs eChannel;

        public ChannelClient(TcpClient passedClient, ref System.ComponentModel.BackgroundWorker workerChannel, ref System.ComponentModel.DoWorkEventArgs eChannel)
        {
            this.workerChannel = workerChannel;
            this.eChannel = eChannel;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            worker.RunWorkerAsync();

            client = passedClient;
            stream = client.GetStream();
        }

        public void killThread()
        {
            worker.CancelAsync();
        }

        public void sendMessage(string message)
        {
            Byte[] data = new Byte[256];
            data = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            worker.ReportProgress(0, String.Format("Sent: {0}", message));
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Loop to receive all the data sent by the client.
            Byte[] data = new Byte[256];

            // String to store the response ASCII representation.
            String responseData = String.Empty;

            while (true)
            {
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                worker.ReportProgress(0, string.Format("Data Recieved: {0}", responseData));
                worker.ReportProgress(1, string.Format("{0}", responseData));
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            workerChannel.ReportProgress(e.ProgressPercentage, e.UserState.ToString());
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }


    }
}
