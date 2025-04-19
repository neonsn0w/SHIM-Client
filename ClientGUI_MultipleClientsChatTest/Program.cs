using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Text;

namespace ClientGUI_MultipleClientsChatTest
{
    internal static class Program
    {
        public static Socket senderSocket;
        private static Chat chat;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            chat = new Chat();

            Thread graphicsThread = new Thread(() => Application.Run(chat));
            graphicsThread.Start();

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

                    // Invoke is required to avoid cross-thread operation exception
                    chat.richTextBox1.Invoke(new Action(() => chat.richTextBox1.AppendText("Socket connected to -> " + senderSocket.RemoteEndPoint.ToString())));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MessageBox.Show("FATAL ERROR, SHUTTING DOWN\n\n" + ex.Message, "SHIM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void readMessages(Socket reader)
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
                    chat.richTextBox1.Invoke(new Action(() => chat.richTextBox1.AppendText(message + Environment.NewLine)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
    }
}
