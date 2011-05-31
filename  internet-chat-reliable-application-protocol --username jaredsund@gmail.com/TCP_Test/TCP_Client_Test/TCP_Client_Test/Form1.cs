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
        ConnThread connectionThread;
        private string userName;
        private Controller myController;

        public Form1()
        {
            InitializeComponent();
            label3.Text = "";
            myController = new Controller(ref listBox2);
        }

        
        private void listBox2_DoubleClick(object sender, EventArgs e)//join a channel
        {
            DataRowView dr = (DataRowView)listBox2.SelectedItem;
            Int32 port = 0;

            if (dr == null)
            {
                return;
            }
            else if (Int32.TryParse(dr["port"].ToString(), out port))
            {
                if (connectionThread != null)
                    connectionThread.closeConn();

                connectionThread = new ConnThread(ref listBox1, "localhost", port, userName);
                label3.Text = dr["name"].ToString();
                myController.enumChannels();
                clearMessages();
            }
            else
            {
                MessageBox.Show("bad port :" + dr["port"].ToString() + " could not connect to channel");
            }//end if

        }


        private void displayMessage(string message)
        {
            listBox1.Items.Add(message);
            listBox1.TopIndex = listBox1.Items.Count - 1;
        }

        private void clearMessages()
        {
            listBox1.Items.Clear();
        }

        private void textBoxUserName_TextChanged(object sender, EventArgs e)
        {
            userName = textBoxUserName.Text;
            myController.username = userName;
        }

        private void buttonSendMessage_Click(object sender, EventArgs e)
        {
            connectionThread.sendMessage(textBox1.Text);
        }

        private void buttonEnumChannels_Click(object sender, EventArgs e)
        {
            myController.enumChannels();
        }

        private void buttonCreateChannel_Click(object sender, EventArgs e)
        {
            myController.createChannel(textBox4.Text );
        }

        

        

       

        
    }
}
