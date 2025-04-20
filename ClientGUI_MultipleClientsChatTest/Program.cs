using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using System.IO;

namespace ClientGUI_MultipleClientsChatTest
{
    internal static class Program
    {
        public static Socket senderSocket;
        private static Chat chat;

        public static string publicKey;
        public static string privateKey;

        // This variable is used to check the keys have been just generated
        // and if it's necessary to send the public key to the server
        private static bool needSetup = false;

        [STAThread]
        static void Main()
        {
            cryptographySetup();

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
                    chat.richTextBox1.Invoke(new Action(() => chat.richTextBox1.AppendText("Socket connected to -> " + senderSocket.RemoteEndPoint.ToString() + Environment.NewLine)));

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

        public static void readMessage(Socket reader)
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                bytesRead = reader.Receive(buffer);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine(message);
                chat.richTextBox1.Invoke(new Action(() => chat.richTextBox1.AppendText(message + Environment.NewLine)));
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
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

        public static void cryptographySetup()
        {
            try
            {
                var x = CryptoTools.LoadKeys();
                publicKey = x.publicKey;
                privateKey = x.privateKey;
            }
            catch (FileNotFoundException fne)
            {
                CryptoTools.GenerateAndSaveKeys();
                var x = CryptoTools.LoadKeys();
                publicKey = x.publicKey;
                privateKey = x.privateKey;
                needSetup = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                MessageBox.Show("FATAL ERROR, SHUTTING DOWN\n\n" + e.Message, "SHIM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

        }
    }
}
