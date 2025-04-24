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
            
            Program.chat.buddyList.updateListBox();
        }

        private void updateConnectedUserDict()
        {
            Program.askUserListUpdate();
        }

        public MainList()
        {
            InitializeComponent();
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

                if (Program.DMs.ContainsKey(data[1].Trim()))
                {
                    MessageBox.Show("This user is already in your DMs.");
                    return;
                }

                DirectChat directChat = new DirectChat(data[1], data[0]);
                Program.DMs.TryAdd(data[1].Trim(), directChat);

                Thread directChatThread = new Thread(() => directChat.ShowDialog());
                Program.directChatThreads.TryAdd(data[1], directChatThread);
                directChatThread.Start();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updateConnectedUserDict();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            selectedUser = listBox1.SelectedItem.ToString().Replace("\t\t\t\t\t", "§");
            string[] data = selectedUser.Split('§');

            if (Program.buddies.ContainsKey(data[1].Trim()))
            {
                MessageBox.Show("This user is already in your buddy list.");
                return;
            }

            Program.buddies.TryAdd(data[1].Trim(), data[0]);
            Program.chat.buddyList.updateListBox();
            Program.serializer.SaveToFile(Program.buddies, "buddies.json");
        }
    }
}