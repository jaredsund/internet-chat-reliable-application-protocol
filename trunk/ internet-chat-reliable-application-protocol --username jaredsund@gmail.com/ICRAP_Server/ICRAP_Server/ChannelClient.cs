using System;
using System.Collections;
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

        private string _id;
        private string _userName;

        private xmlResponseGen xRG;

        //constructor
        public ChannelClient(TcpClient passedClient, ref System.ComponentModel.BackgroundWorker workerChannel, ref System.ComponentModel.DoWorkEventArgs eChannel)
        {
            _id = Guid.NewGuid().ToString();
            _userName = "username";
            this.workerChannel = workerChannel;
            this.eChannel = eChannel;
            xRG = new xmlResponseGen();

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            worker.RunWorkerAsync();

            client = passedClient;
            stream = client.GetStream();
        }//end of constructor

        //destructor
         ~ChannelClient()
        {
            if (client.Connected)
            {
                client.Close();
            }//end if
             stream.Dispose();
             worker.Dispose();
        }//end of destructor

        public string id
        {
            get { return _id; }
        }

        public string username
        {
            get { return _userName; }
        }

        public void killThread()
        {
            try
            {
                sendMessage("Connection closed by Server");
                worker.CancelAsync();
            }
            finally
            {
                worker.CancelAsync();
            }
        }

        public void sendMessage(string message)
        {
            try
            {
                if (stream.CanWrite)
                {
                    Byte[] data = new Byte[1024];
                    data = System.Text.Encoding.ASCII.GetBytes(xRG.clientResponse ("AcceptMessage",message));
                    stream.Write(data, 0, data.Length);
                    worker.ReportProgress(0, String.Format("Sent: {0}", message));
                }
                else
                {
                    worker.CancelAsync();
                }
            }
            catch (IOException e1)
            {
                worker.ReportProgress(99, String.Format("IOException: {0}:ID={1}", e1, _id));
                worker.CancelAsync();
            }
            catch (SocketException e2)
            {
                worker.ReportProgress(99, String.Format("SocketException: {0}:ID={1}", e2, _id));
                worker.CancelAsync();
            }
            catch (ObjectDisposedException e3)
            {
                worker.ReportProgress(99, String.Format("ObjectDisposedException: {0}:ID={1}", e3, _id));
                worker.CancelAsync();
            }
            finally
            {
                worker.CancelAsync();
            }
        }

        private void commands(string data)
        {
            xmlCommandParser xCP = new xmlCommandParser(data);
            xmlResponseGen xRG = new xmlResponseGen();

            switch (xCP.command)
            {
                case "AcceptClient":
                    
                    break;
                case "CloseConn":
                    worker.ReportProgress(4, String.Format("Closing client: {0}:ID={1}", _userName, _id));
                    break;
                case "EnumClients":
                    worker.ReportProgress(2, String.Format("ID={0}, Username={1}", _id,_userName ));
                    break;
                case "PostMessage":
                    worker.ReportProgress(0, string.Format("Data Recieved: {0}", xCP.data ));
                    worker.ReportProgress(1, string.Format("{0}", xCP.data ));
                    break;
                case "killClient":
                    break;
                case "SetMaxClients":
                    worker.ReportProgress (3,xCP.data );
                    sendMessage (xRG.sResponse ());
                    break;
                case "CloseChan":
                    worker.ReportProgress(1, String.Format("Closing Channel by client client: {0}:{1}", _userName, _id));
                    workerChannel.CancelAsync();
                    break;
                default://send message to the client, stating that the command was not understood.
                    sendMessage(xRG.eResponse(String.Format ("Command not understood: {0}", xCP.command )));
                    break;
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Loop to receive all the data sent by the client.
                Byte[] data = new Byte[1024];

                while (true)//stream.DataAvailable
                {
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    //blocking, waiting for new string from the client
                    commands(System.Text.Encoding.ASCII.GetString(data, 0, bytes));
                }//end while loop
            }
            catch (IOException e1)
            {
                worker.ReportProgress(99, String.Format("IOException: {0}:ID={1}", e1, _id));
                worker.CancelAsync();
            }
            catch (SocketException e2)
            {
                worker.ReportProgress(99, String.Format("SocketException: {0}:ID={1}", e2, _id));
                worker.CancelAsync();
            }
            catch (ObjectDisposedException e3)
            {
                worker.ReportProgress(99, String.Format("ObjectDisposedException: {0}:ID={1}", e3, _id));
                worker.CancelAsync();   
            }
            finally
            {
                worker.CancelAsync();
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            workerChannel.ReportProgress(e.ProgressPercentage, e.UserState.ToString());
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            workerChannel.ReportProgress(100, String.Format("User ID={0}", _id));
        }
    }
}
