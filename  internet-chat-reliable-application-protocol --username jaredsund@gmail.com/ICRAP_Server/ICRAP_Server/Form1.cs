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

namespace ICRAP_Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartThread();
        }

        private void StartThread()
        {
            ChannelThread mythread = new ChannelThread(ref listBox1, "Channel 1");
            ChannelThread mythread2 = new ChannelThread(ref listBox1, "Channel 2");

            
        }

       
        private void button2_Click(object sender, EventArgs e)
        {
            
            
        }

       
        
    }
}
