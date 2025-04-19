using System;
using System.Windows.Forms;

namespace ClientGUI_MultipleClientsChatTest
{
    public partial class MainList : Form
    {
        private string selectedUser;

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
                MessageBox.Show(listBox1.SelectedItem.ToString());
            }
        }
    }
}