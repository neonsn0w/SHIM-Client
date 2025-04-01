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
        private Socket senderSocket;

        public Chat()
        {
            InitializeComponent();
        }

        private void readMessages(Socket reader)
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while (true)
                {
                    bytesRead = reader.Receive(buffer);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine(message);
                    this.richTextBox1.Invoke(new Action(() => this.richTextBox1.AppendText(message + Environment.NewLine)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        private void addTrailingTextToTextBox(string text)
        {
            richTextBox1.Text = richTextBox1.Text + text + "\n";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

                senderSocket = new Socket(ipAddr.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    senderSocket.Connect(localEndPoint);

                    Thread clientThread = new Thread(() => readMessages(senderSocket));
                    clientThread.Start();

                    addTrailingTextToTextBox("Socket connected to -> " + senderSocket.RemoteEndPoint.ToString());

                } catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string userMessage = textBox1.Text;  // Get the text from TextBox1

            if (!string.IsNullOrEmpty(userMessage))
            {
                byte[] messageSent = Encoding.UTF8.GetBytes(userMessage + "<EOF>");
                int byteSent = senderSocket.Send(messageSent);
                textBox1.Clear();  // Clear the TextBox after sending the message

                // Optionally, display the sent message in the richTextBox
                addTrailingTextToTextBox("You: " + userMessage);

                // If user types "exit", close the application
                if (userMessage.ToLower() == "exit")
                {
                    senderSocket.Shutdown(SocketShutdown.Both);
                    senderSocket.Close();
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
