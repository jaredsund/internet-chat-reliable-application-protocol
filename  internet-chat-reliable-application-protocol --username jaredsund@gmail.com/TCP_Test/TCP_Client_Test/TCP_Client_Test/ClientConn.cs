using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TCP_Client_Test
{
    class ClientConn
    {
        public class CurrentState
        {
            public string message;
            public Int32 port;
            public string server;
            public string ID;


        }
        
        ArrayList clients = new ArrayList();

        CurrentState state = new CurrentState();
        TcpClient client;
        NetworkStream stream;

        System.ComponentModel.BackgroundWorker worker;
        System.ComponentModel.DoWorkEventArgs e;

        public ClientConn(ref System.ComponentModel.BackgroundWorker worker, ref System.ComponentModel.DoWorkEventArgs e, String server, Int32 port)
        {
            
            this.worker = worker;
            this.e = e;
            state.port = port;
            state.server = server;
            state.ID = Guid.NewGuid().ToString();

        }

        public void Dispose()
        {
            // Close everything.
            stream.Close();
            client.Close();

        }

        public void Connect()
        {
            try
            {
                client = new TcpClient(state.server, state.port);
                stream = client.GetStream();
                recieveMessage();


            }
            catch (ArgumentNullException e2)
            {
                state.message = string.Format("ArgumentNullException: {0}", e2);
                worker.ReportProgress(0, state);
            }
            catch (SocketException e3)
            {
                state.message = string.Format("SocketException: {0}", e3);
                worker.ReportProgress(0, state);
            }


        }

        public void sendMessage(string message)
        {
            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            // Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length);

            state.message = String.Format("Sent: {0}", message);
            worker.ReportProgress(0, state);
        }

        public void recieveMessage()
        {
            try
            {
                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                Byte[] data = new Byte[1024];

                // String to store the response ASCII representation.
                String responseData = String.Empty;
                while (true)
                {
                    // Read the first batch of the TcpServer response bytes.
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    state.message = string.Format("Received: {0}", responseData);
                    worker.ReportProgress(0, state);
                }
            }
            catch (IOException e1)
            {
                worker.ReportProgress(99, String.Format("IOException: {0}:ID={1}", e1, state.ID));
                worker.CancelAsync();
            }
            catch (SocketException e2)
            {
                worker.ReportProgress(99, String.Format("SocketException: {0}:ID={1}", e2, state.ID));
                worker.CancelAsync();
            }
            catch (ObjectDisposedException e3)
            {
                worker.ReportProgress(99, String.Format("ObjectDisposedException: {0}:ID={1}", e3, state.ID));
                worker.CancelAsync();
            }
            finally
            {
                worker.CancelAsync();
            }
        }
    }
}
