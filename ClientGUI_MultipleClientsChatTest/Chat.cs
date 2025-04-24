using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ClientGUI_MultipleClientsChatTest
{
    public partial class Chat : Form
    {
        public MainList mainList;

        public Chat()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mainList = new MainList();
            mainList.Show();
        }

        private void Form1_Close(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string userMessage = textBox1.Text.Trim().Replace("§", "ss");  // Get the text from TextBox1  

            if (!string.IsNullOrEmpty(userMessage))
            {
                byte[] messageSent = Encoding.UTF8.GetBytes("broadcast " + userMessage);
                int byteSent = Program.senderSocket.Send(messageSent);
                textBox1.Clear();  // Clear the TextBox after sending the message  

                richTextBox1.AppendText(Program.username + ": ", Color.Blue);
                richTextBox1.AppendText(userMessage + "\n");

                if (userMessage.ToLower() == "exit")
                {
                    Program.senderSocket.Shutdown(SocketShutdown.Both);
                    Program.senderSocket.Close();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Please enter a message.");
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            // Automatically scroll to the bottom  
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
    }
}
