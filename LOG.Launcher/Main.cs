using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using LOG.API.Networking.Messages;

namespace LOG.Launcher
{
    public partial class Main : Form
    {
        public string KSPDirectory, LOGDirectory, ServerListPath;
        public DiscoveryRequestMessage DiscoveryMessage;
       
        private string ServerIP = null, ServerPort = null;

        private static bool is64BitProcess = (IntPtr.Size == 8);
        private static bool is64BitOperatingSystem = is64BitProcess || InternalCheckIsWow64();

        #region Get Informations About Windows
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        public static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal))
                    {
                        return false;
                    }
                    return retVal;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        public Main()
        {
            InitializeComponent();
            
            KSPDirectory = Application.StartupPath;
            LOGDirectory = Path.Combine(KSPDirectory, "L.O.G");
            ServerListPath = Path.Combine(LOGDirectory, "ServerList.list");

            if(Properties.Settings.Default.UpgradeRequired == true)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(LOGDirectory))
                Directory.CreateDirectory(LOGDirectory);

            if (File.Exists(ServerListPath) == true && new FileInfo(ServerListPath).Length != 0)
            {
                Network.Initialize();
                LoadingServersWorker.RunWorkerAsync();
            }
        }

        public void PlayButton_Click(object sender, EventArgs e)
        {
            if (UserNameBox.Text.Length == 0)
            {
                MessageBox.Show("Username can't be empty!");
                return;
            }

            ServerIP = ServerInfo.ServerDetail[ListOfServers.CurrentCell.RowIndex].IPAddress;
            ServerPort = ServerInfo.ServerDetail[ListOfServers.CurrentCell.RowIndex].Port.ToString();

            if (ServerIP.Length > 0 && ServerPort.Length > 0)
            {
                if (is64BitOperatingSystem == true)
                    Process.Start(Path.Combine(KSPDirectory, "KSP_x64.exe"), string.Format("-IP={0} -Port={1} -Username={2}", ServerIP, ServerPort, UserNameBox.Text));
                else
                    Process.Start(Path.Combine(KSPDirectory, "KSP.exe"), string.Format("-IP={0} -Port={1} -Username={2}", ServerIP, ServerPort, UserNameBox.Text));
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (Program.addserver.IsDisposed == true)
                Program.addserver = new AddServer();

            Program.addserver.ShowDialog(this);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshServerInfo();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            string[] ServerListOld = File.ReadAllLines(ServerListPath);
            File.WriteAllLines(ServerListPath, ServerListOld.Where(line => !line.Contains(ServerInfo.ServerDetail[ListOfServers.CurrentCell.RowIndex].IPAddress)));

            ListOfServers.Rows.RemoveAt(ListOfServers.CurrentCell.RowIndex);
        }

        private void LoadingServersWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadServerList();
        }

        private void LoadingServersWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(ListOfServers.RowCount > 0)
                ListOfServers.Rows[0].Selected = true;
        }

        private void ListOfServers_Click(object sender, EventArgs e)
        {
            RefreshServerInfo();
        }

        public void LoadServerList()
        {
            if (ListOfServers.RowCount > 0)
                ListOfServers.Invoke(new MethodInvoker(() => ListOfServers.Rows.Clear()));

            string[] ServerList = File.ReadAllLines(ServerListPath), Server;

            for (int i = 0; i < ServerList.Length; i++)
            {
                Server = ServerList[i].Split(':');

                Network.DiscoverServers(Server[0], Convert.ToInt32(Server[1]));

                ServerInfo.ServerDetail.Add(new ServerInfo()
                {
                    IPAddress = Server[0],
                    Port = Convert.ToInt32(Server[1]),
                    Hostname = this.DiscoveryMessage.Hostname,
                    Players = this.DiscoveryMessage.Players,
                    MaximumPlayers = this.DiscoveryMessage.MaximumPlayers,
                    Ping = this.DiscoveryMessage.Ping
                });
            }

            foreach (ServerInfo serverinfo in ServerInfo.ServerDetail) // Do that because we don't want server to be added one by one.
                ListOfServers.Invoke(new MethodInvoker(() => ListOfServers.Rows.Add(serverinfo.Hostname, serverinfo.Players + " / " + serverinfo.MaximumPlayers, serverinfo.Ping)));
        }

        private void RefreshServerInfo()
        {
            if (ListOfServers.SelectedRows.Count > 0)
            {
                Network.DiscoverServers(ServerInfo.ServerDetail[ListOfServers.CurrentCell.RowIndex].IPAddress, Convert.ToInt32(ServerInfo.ServerDetail[ListOfServers.CurrentCell.RowIndex].Port));

                ServerInfo.ServerDetail[ListOfServers.CurrentCell.RowIndex].Hostname = this.DiscoveryMessage.Hostname;
                ServerInfo.ServerDetail[ListOfServers.CurrentCell.RowIndex].Players = this.DiscoveryMessage.Players;
                ServerInfo.ServerDetail[ListOfServers.CurrentCell.RowIndex].MaximumPlayers = this.DiscoveryMessage.MaximumPlayers;
                ServerInfo.ServerDetail[ListOfServers.CurrentCell.RowIndex].Ping = this.DiscoveryMessage.Ping;

                ListOfServers.SelectedRows[0].Cells[0].Value = ServerInfo.ServerDetail[ListOfServers.CurrentCell.RowIndex].Hostname;
                ListOfServers.SelectedRows[0].Cells[1].Value = ServerInfo.ServerDetail[ListOfServers.CurrentCell.RowIndex].Players + " / " + ServerInfo.ServerDetail[ListOfServers.CurrentCell.RowIndex].MaximumPlayers;
                ListOfServers.SelectedRows[0].Cells[2].Value = ServerInfo.ServerDetail[ListOfServers.CurrentCell.RowIndex].Ping;
            }
        }

        private void UserNameBox_MouseLeave(object sender, EventArgs e)
        {
            Properties.Settings.Default["Username"] = UserNameBox.Text;
            Properties.Settings.Default.Save();
        }
    }
}
