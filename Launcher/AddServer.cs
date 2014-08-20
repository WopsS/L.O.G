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

namespace Launcher
{
    public partial class AddServer : Form
    {
        public AddServer()
        {
            InitializeComponent();
        }

        private void AddServerButton_Click(object sender, EventArgs e)
        {
            if(ServerIPText.TextLength <= 0 && ServerPortText.TextLength <= 0)
                this.Close();

            if (!File.Exists(Program.main.ServerListPath))
                File.Create(Program.main.ServerListPath);

            File.AppendAllText(Program.main.ServerListPath, string.Format("{0}:{1}", ServerIPText.Text, ServerPortText.Text) + Environment.NewLine);

            if (Program.main.LoadingServersWorker.IsBusy == false)
            {
                ServerInfo.ServerDetail.Clear();
                Program.main.ListOfServers.Rows.Clear();

                if (new FileInfo(Program.main.ServerListPath).Length != 0)
                    Program.main.LoadingServersWorker.RunWorkerAsync();
            }

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
