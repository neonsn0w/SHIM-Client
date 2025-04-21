using System.Windows.Forms;

namespace ClientGUI_MultipleClientsChatTest
{
    public partial class UsernameInput : Form
    {
        public UsernameInput()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            Program.username = textBox1.Text;
            this.Close();
        }
    }
}