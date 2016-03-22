﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArduinoSerialServer
{
    public partial class ServerForm : Form
    {
        public ServerForm()
        {
            InitializeComponent();
            SocketServer.StartListening(this, 8901);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AddMessageInternal(string message)
        {
            textBoxLog.AppendText(message + "\r\n");
        }
        private delegate void AddMessageDelegate(string message);

        public void AddMessage(string message)
        {
            this.BeginInvoke(new AddMessageDelegate(AddMessageInternal), new object[] { message });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            SocketServer.Shutdown();
            base.OnClosing(e);
        }
    }
}