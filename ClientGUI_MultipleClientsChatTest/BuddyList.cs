using System.Threading;
using System;
using System.Windows.Forms;

namespace ClientGUI_MultipleClientsChatTest
{
    public partial class BuddyList : Form
    {
        private string selectedUser;

        private void updateUsernames()
        {
            foreach (var buddy in Program.buddies)
            {
                if (Program.connectedUsers.ContainsKey(buddy.Key))
                {
                    Program.buddies.TryRemove(buddy.Key, out _);
                    Program.buddies.TryAdd(buddy.Key, Program.connectedUsers[buddy.Key]);
                }
            }
        }

        public void updateListBox()
        {
            updateUsernames();

            listBox1.Items.Clear();
            foreach (var user in Program.buddies)
            {
                // Mi vergogno dei miei peccati...
                listBox1.Items.Add(user.Value + "\t\t\t\t\t" + user.Key);
            }
        }

        public BuddyList()
        {
            InitializeComponent();
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

                if (!Program.connectedUsers.ContainsKey(data[1].Trim()))
                {
                    MessageBox.Show("This user is offline.");
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
            selectedUser = listBox1.SelectedItem.ToString().Replace("\t\t\t\t\t", "§");
            string[] data = selectedUser.Split('§');
            
            Program.buddies.TryRemove(data[1].Trim(), out _);
            Program.chat.buddyList.updateListBox();
            Program.serializer.SaveToFile(Program.buddies, "buddies.json");
        }

        // This makes the form only hide when the user clicks the X button
        private void BuddyList_Close(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }
}