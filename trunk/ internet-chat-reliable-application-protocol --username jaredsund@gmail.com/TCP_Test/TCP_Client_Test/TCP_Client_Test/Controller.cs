using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;

using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TCP_Client_Test
{
    class Controller
    {
        private string _userName;
        private ListBox passedListbox;

        public Controller(ref ListBox passedListbox)
        {
            this.passedListbox = passedListbox;
        }
        
        public Controller(string userName, ref ListBox passedListbox)
        {
            this.passedListbox = passedListbox;
            this._userName = userName;
        }

        public string username
        {
            set { _userName = value; }
        }

        private string sendContollerMessage(string message)
        {
            try
            {
                TcpClient client = new TcpClient("localhost", 13000);

                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                NetworkStream stream = client.GetStream();

                stream.Write(data, 0, data.Length);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[1024];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                // Close everything.
                stream.Close();
                client.Close();

                return responseData;
            }
            catch (Exception  e2)
            {
                return string.Format("ERROR: ArgumentNullException: {0}", e2);
            }
        }

        public  void enumChannels()
        {
            DataTable dt = new DataTable();

            xmlControllerRequestGen xRG = new xmlControllerRequestGen(_userName);
            string message = xRG.EnumChan();
            string responseData = sendContollerMessage(message);

            if (responseData.Contains ("ERROR"))
            {
                MessageBox.Show(responseData);
                return ;
            }//end if

            xmlControllerParser xCP = new xmlControllerParser(responseData);

            xCP.channels(ref dt);

            passedListbox.DataSource = dt;
            passedListbox.DisplayMember = "displayname";
        }

        public  void createChannel(string channelName)
        {
            xmlControllerRequestGen xRG = new xmlControllerRequestGen(_userName);
            string message = xRG.CreatChan(channelName);
            string responseData = sendContollerMessage(message);
            enumChannels();
        }

        public void destChann(int port)
        {
            xmlControllerRequestGen xRG = new xmlControllerRequestGen(_userName);
            string message = xRG.DestChan(port);
            string response = sendContollerMessage(message);

        }
    }
}
