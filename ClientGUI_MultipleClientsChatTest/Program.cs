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

// I really didn't want to do this, but I need to use the InputBox...
// using Microsoft.VisualBasic;

// I ACTUALLY DONT NEED IT ANYMORE YES!!! GODOOOOOO!!!!!!!!

namespace ClientGUI_MultipleClientsChatTest
{
    internal static class Program
    {
        public static Socket senderSocket;
        private static Chat chat;
        private static UsernameInput usernameInput;

        public static string username = ""; // neonsn0w!

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
            usernameInput = new UsernameInput();

            Application.Run(usernameInput);

            if (username == "")
            {
                // If the user didn't enter a username, we need to hurt his feelings
                MessageBox.Show("This username is ass. Session terminated.", "SHIM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

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

                    // Send the public key to the server
                    byte[] messageSent = Encoding.UTF8.GetBytes("setpubkey " + publicKey);
                    int byteSent = Program.senderSocket.Send(messageSent);

                    readMessage(senderSocket);

                    // Send the nickname to the server
                    messageSent = Encoding.UTF8.GetBytes($"setnick {username}");
                    byteSent = Program.senderSocket.Send(messageSent); 

                    readMessage(senderSocket);

                    // update the database
                    messageSent = Encoding.UTF8.GetBytes("updatedb");
                    byteSent = Program.senderSocket.Send(messageSent);

                    readMessage(senderSocket);

                    Thread clientThread = new Thread(() => readMessages(senderSocket));
                    clientThread.Start();

                    // Invoke is required to avoid cross-thread operation exception
                    chat.richTextBox1.Invoke(new Action(() => chat.richTextBox1.AppendText("Connected to this server's public pool.\nMessages here are not encrypted")));

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
                //Console.WriteLine(message);
                // chat.richTextBox1.Invoke(new Action(() => chat.richTextBox1.AppendText(message + Environment.NewLine)));
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
