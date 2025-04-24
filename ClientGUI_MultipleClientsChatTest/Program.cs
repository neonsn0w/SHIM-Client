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
using System.Collections.Concurrent;
using System.Drawing;

namespace ClientGUI_MultipleClientsChatTest
{
    internal static class Program
    {
        public static Socket senderSocket;
        private static Chat chat;
        private static UsernameInput usernameInput;

        // Format: publickey + username  
        public static ConcurrentDictionary<string, string> connectedUsers = new ConcurrentDictionary<string, string>();
        public static ConcurrentDictionary<string, DirectChat> DMs = new ConcurrentDictionary<string, DirectChat>();

        // Thread pool for DirectChat threads  
        public static ConcurrentDictionary<string, Thread> directChatThreads = new ConcurrentDictionary<string, Thread>();

        public static string username = ""; // neonsn0w!  

        public static string publicKey;
        public static string privateKey;

        private static Thread clientThread;

        // TODO: Check if this is necessary  
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

            username = username.Replace("§", "ss");

            if (username.Trim() == "")
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

                    clientThread = new Thread(() => readMessages(senderSocket));
                    clientThread.Start();

                    // Invoke is required to avoid cross-thread operation exception  
                    chat.richTextBox1.Invoke(new Action(() => chat.richTextBox1.AppendText("Connected to this server's public pool.\nMessages here are not encrypted\n")));

                    askUserListUpdate();
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

        public static void askUserListUpdate()
        {
            byte[] messageSent = Encoding.UTF8.GetBytes("getusers");
            int byteSent = Program.senderSocket.Send(messageSent);
        }

        public static void updateUserList(string serverResponse)
        {
            string[] users = serverResponse.Split('\n');

            foreach (string user in users)
            {
                if (user.Trim() != "")
                {
                    // Format: "username§publickey"  
                    string[] userInfo = user.Split('§');

                    // If I uncomment this, I will https://learn.microsoft.com/en-us/windows-server/get-started/kms-client-activation-keys  
                    /*if (userInfo[1] == publicKey)  
                    {  
                        continue;  
                    }*/

                    // Format: publickey + username  
                    connectedUsers.TryAdd(userInfo[1], userInfo[0]);
                }
            }
            chat.mainList.Invoke(new Action(() => chat.mainList.updateListBox()));
        }

        public static string readMessage(Socket reader)
        {
            byte[] buffer = new byte[65536];
            int bytesRead;

            try
            {
                bytesRead = reader.Receive(buffer);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                // Console.WriteLine(message);  
                // chat.richTextBox1.Invoke(new Action(() => chat.richTextBox1.AppendText(message + Environment.NewLine)));  

                return message;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return e.Message;
            }
        }

        public static void readMessages(Socket reader)
        {
            byte[] buffer = new byte[65536];
            int bytesRead;

            try
            {
                while (true)
                {
                    bytesRead = reader.Receive(buffer);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine(message);
                    if (message.StartsWith("§§§USERLIST§§§"))
                    {
                        updateUserList(message.Substring(14));
                    }
                    else if (message.StartsWith("md "))
                    {
                        string[] userInfo = message.Substring(3).Split('§');
                        if (!DMs.ContainsKey(userInfo[0]))
                        {
                            DirectChat directChat = new DirectChat(userInfo[0], userInfo[2]);
                            DMs.TryAdd(userInfo[0], directChat);

                            // Create a thread for the DirectChat instance  
                            Thread directChatThread = new Thread(() => directChat.ShowDialog());
                            directChatThreads.TryAdd(userInfo[0], directChatThread);
                            directChatThread.Start();
                        }
                        Thread.Sleep(100);
                        DMs[userInfo[0]].Invoke(new Action(() => DMs[userInfo[0]].richTextBox1.AppendText(userInfo[2] + ": ", Color.Red)));
                        DMs[userInfo[0]].Invoke(new Action(() => DMs[userInfo[0]].richTextBox1.AppendText(CryptoTools.Decrypt(Convert.FromBase64String(userInfo[1]), privateKey) + Environment.NewLine)));

                        SoundTools.playIMRcvSound();

                    }
                    else if (message.StartsWith("brd "))
                    {
                        string[] data = message.Substring(4).Split('§');
                        chat.richTextBox1.Invoke(new Action(() => chat.richTextBox1.AppendText(data[1] + ": ", Color.Red)));
                        chat.richTextBox1.Invoke(new Action(() => chat.richTextBox1.AppendText(data[0] + Environment.NewLine)));

                        SoundTools.playIMRcvSound();
                    }

                    else
                    {
                        chat.richTextBox1.Invoke(new Action(() => chat.richTextBox1.AppendText(message + Environment.NewLine)));
                    }
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
