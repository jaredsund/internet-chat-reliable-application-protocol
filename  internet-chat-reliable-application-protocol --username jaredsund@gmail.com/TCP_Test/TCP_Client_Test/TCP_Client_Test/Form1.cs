using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TCP_Client_Test
{
    public partial class Form1 : Form
    {
        ConnThread myThread;
        ConnThread myThread2;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox2.Text != "")
             myThread = new ConnThread(ref listBox1, "localhost", Int32.Parse(textBox2.Text));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            myThread.sendMessage(textBox1.Text);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != "")
                myThread2 = new ConnThread(ref listBox1, "localhost", Int32.Parse(textBox3.Text));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            myThread2.sendMessage(textBox1.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            xmlRequestGen xRG = new xmlRequestGen();
            string message = xRG.CreatChan(textBox4.Text);

            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.

                TcpClient client = new TcpClient("localhost", 13000);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                listBox1.Items.Add (String.Format ("Sent: {0}", message));

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                listBox1.Items.Add (String.Format("Received: {0}", responseData));

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e2)
            {
                listBox1.Items.Add ( string.Format("ArgumentNullException: {0}", e2));
            }
            catch (SocketException e3)
            {
                listBox1.Items.Add ( string.Format("SocketException: {0}", e3));
            }
        }

        

        

       

        
    }
}
