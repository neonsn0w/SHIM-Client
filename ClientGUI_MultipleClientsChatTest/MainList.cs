using System;
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
                    listBox1.Items.Add(user.Value);
                }
            }
        }

        private void updateConnectedUserDict()
        {
            Program.askUserListUpdate();
            updateListBox();
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
                MessageBox.Show(listBox1.SelectedItem.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updateConnectedUserDict();
        }
    }
}