using LOG.API;
using LOG.API.Networking.Messages;
using LOG.MasterServer.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LOG.Launcher
{
    public partial class Main : Form
    {
        private Network m_network;
        private string m_serverIP = String.Empty, m_serverPort = String.Empty, m_KSPDirectory = String.Empty;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.m_loadingServersWorker.DoWork += m_loadingServersWorker_DoWork;

            this.m_KSPDirectory = Application.StartupPath;

            if (Properties.Settings.Default.UpgradeRequired == true) // It is in settings.
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            this.m_userNameTextBox.Text = Properties.Settings.Default.Username;

            this.m_network = new Network();

            this.m_network.RequestServersList();
            this.m_network.CalculatePing();

            this.m_loadingServersWorker.RunWorkerAsync();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.m_network.Dispose();
        }

        private void m_playButton_Click(object sender, EventArgs e)
        {
            if (this.m_userNameTextBox.Text.Length == 0)
            {
                MessageBox.Show("Username can't be empty!");
                return;
            }

            Properties.Settings.Default.Username = this.m_userNameTextBox.Text;
            Properties.Settings.Default.Save();

            m_serverIP = "localhost"; // TODO: Get server IP from table.
            m_serverPort = APIMain.ServerPort.ToString(); // TODO: Get server port from table.

            if (m_serverIP.Length > 0 && m_serverPort.Length > 0)
            {
                Process.Start(Path.Combine(this.m_KSPDirectory, "KSP.exe"), String.Format("-IP={0} -Port={1} -Username={2}", m_serverIP, m_serverPort, this.m_userNameTextBox.Text));
            }
        }

        private void m_refreshButton_Click(object sender, EventArgs e)
        {
            if (this.m_loadingServersWorker.IsBusy == false)
            {
                this.m_loadingServersWorker.RunWorkerAsync();
            }
        }

        private void m_loadingServersWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.m_network.RequestServersList();
            this.m_network.CalculatePing();

            Thread.Sleep(100); // Sleep for a while, client need to get servers infomartions.

            this.FillServersList();
        }

        private void FillServersList()
        {
            if (this.m_serversList.RowCount > 0)
                this.m_serversList.Invoke(new MethodInvoker(() => this.m_serversList.Rows.Clear()));

            Dictionary<long, ServerMessage> serverInfoList = this.m_network.GetServersList;

            foreach (KeyValuePair<long, ServerMessage> serverInfo in serverInfoList)
            {
                this.m_serversList.Invoke(new MethodInvoker(() => this.m_serversList.Rows.Add(serverInfo.Value.Hostname, serverInfo.Value.Players + " / " + serverInfo.Value.MaximumPlayers, 
                    serverInfo.Value.Ping)));
            }
        }
    }
}
