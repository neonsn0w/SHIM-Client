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
        

        public Chat()
        {
            InitializeComponent();
        }

        public void addTrailingTextToTextBox(string text)
        {
            richTextBox1.Text = richTextBox1.Text + text + "\n";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string userMessage = textBox1.Text;  // Get the text from TextBox1

            if (!string.IsNullOrEmpty(userMessage))
            {
                byte[] messageSent = Encoding.UTF8.GetBytes(userMessage + "<EOF>");
                int byteSent = Program.senderSocket.Send(messageSent);
                textBox1.Clear();  // Clear the TextBox after sending the message

                // Optionally, display the sent message in the richTextBox
                addTrailingTextToTextBox("You: " + userMessage);

                // If user types "exit", close the application
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
    }
}
