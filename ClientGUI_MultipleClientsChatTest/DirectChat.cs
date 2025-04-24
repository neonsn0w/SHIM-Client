using System.Net.Sockets;
using System.Text;
using System;
using System.Windows.Forms;
using System.Drawing;

namespace ClientGUI_MultipleClientsChatTest
{
    public partial class DirectChat : Form
    {
        private string publickey;
        private string username;

        public DirectChat(string publickey, string username)
        {
            InitializeComponent();
            this.publickey = publickey;
            this.username = username;
        }

        private void DirectChat_Load(object sender, System.EventArgs e)
        {
            label1.Text = username;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string userMessage = textBox1.Text.Trim().Replace("§", "ss");  // Get the text from TextBox1  

            if (!string.IsNullOrEmpty(userMessage))
            {
                byte[] messageSent = Encoding.UTF8.GetBytes($"dm {publickey}§{Convert.ToBase64String(CryptoTools.Encrypt(userMessage, publickey))}");

                int byteSent = Program.senderSocket.Send(messageSent);
                textBox1.Clear();  // Clear the TextBox after sending the message  

                richTextBox1.AppendText(Program.username + ": ", Color.Blue);
                richTextBox1.AppendText(userMessage + "\n");

                SoundTools.playIMSendSound();

                /* if (userMessage.ToLower() == "exit")
                {
                    Program.senderSocket.Shutdown(SocketShutdown.Both);
                    Program.senderSocket.Close();
                    this.Close();
                } */
            }
            else
            {
                MessageBox.Show("Please enter a message.");
            }
        }
    }
}