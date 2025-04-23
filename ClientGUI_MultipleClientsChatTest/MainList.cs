using System;
using System.Threading;
using System.Windows.Forms;

namespace ClientGUI_MultipleClientsChatTest
{
    public partial class MainList : Form
    {
        private string selectedUser;

        public void updateListBox()
        {
            if (Program.connectedUsers.Count != 0)
            {
                listBox1.Items.Clear();
                foreach (var user in Program.connectedUsers)
                {
                    // Mi vergogno dei miei peccati...
                    listBox1.Items.Add(user.Value + "\t\t\t\t\t" + user.Key);
                }
            }
        }

        private void updateConnectedUserDict()
        {
            Program.askUserListUpdate();
        }

        public MainList()
        {
            InitializeComponent();
            // updateConnectedUserDict();
        }

        private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            
        }

        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                // MessageBox.Show(listBox1.SelectedItem.ToString());
                selectedUser = listBox1.SelectedItem.ToString().Replace("\t\t\t\t\t", "§");
                string[] data = selectedUser.Split('§');

                DirectChat directChat = new DirectChat(data[1], data[0]);
                Program.DMs.TryAdd(data[0], directChat);

                Thread directChatThread = new Thread(() => directChat.ShowDialog());
                Program.directChatThreads.TryAdd(data[0], directChatThread);
                directChatThread.Start();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updateConnectedUserDict();
        }
    }
}