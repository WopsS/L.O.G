using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOG.Launcher
{
    public partial class AddServer : Form
    {
        public AddServer()
        {
            InitializeComponent();
        }

        private void AddServerButton_Click(object sender, EventArgs e)
        {
            if (ServerIPText.TextLength <= 0 && ServerPortText.TextLength <= 0)
                this.Close();

            if (Program.main.LoadingServersWorker.IsBusy == true)
                Program.main.LoadingServersWorker.CancelAsync();

            File.AppendAllText(Program.main.ServerListPath, string.Format("{0}:{1}", ServerIPText.Text, ServerPortText.Text) + Environment.NewLine);

            ServerInfo.ServerDetail.Clear();
            Program.main.LoadingServersWorker.RunWorkerAsync();

            this.Close();
        }

        private void CancelButtonForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ServerPortText_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}
