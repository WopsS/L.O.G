namespace LOG.Launcher
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.m_loadingServersWorker = new System.ComponentModel.BackgroundWorker();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.NameLabel = new System.Windows.Forms.Label();
            this.m_userNameTextBox = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.m_refreshButton = new System.Windows.Forms.Button();
            this.m_statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.Ping = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Players = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HostName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.m_playButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_serversList = new System.Windows.Forms.DataGridView();
            this.m_statusStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_serversList)).BeginInit();
            this.SuspendLayout();
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.panel6.Location = new System.Drawing.Point(0, 34);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(800, 1);
            this.panel6.TabIndex = 3;
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = global::LOG.Launcher.Properties.Resources.Icon;
            this.panel5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panel5.Location = new System.Drawing.Point(751, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(38, 35);
            this.panel5.TabIndex = 7;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.panel4.Location = new System.Drawing.Point(744, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1, 35);
            this.panel4.TabIndex = 3;
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(233, 11);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(38, 13);
            this.NameLabel.TabIndex = 6;
            this.NameLabel.Text = "Name:";
            // 
            // m_userNameTextBox
            // 
            this.m_userNameTextBox.Location = new System.Drawing.Point(274, 8);
            this.m_userNameTextBox.Name = "m_userNameTextBox";
            this.m_userNameTextBox.Size = new System.Drawing.Size(123, 20);
            this.m_userNameTextBox.TabIndex = 5;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.panel3.Location = new System.Drawing.Point(85, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1, 35);
            this.panel3.TabIndex = 2;
            // 
            // m_refreshButton
            // 
            this.m_refreshButton.BackgroundImage = global::LOG.Launcher.Properties.Resources.Refresh;
            this.m_refreshButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.m_refreshButton.Location = new System.Drawing.Point(49, 2);
            this.m_refreshButton.Name = "m_refreshButton";
            this.m_refreshButton.Size = new System.Drawing.Size(30, 30);
            this.m_refreshButton.TabIndex = 4;
            this.m_refreshButton.UseVisualStyleBackColor = true;
            this.m_refreshButton.Click += new System.EventHandler(this.m_refreshButton_Click);
            // 
            // m_statusStrip
            // 
            this.m_statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.m_statusStrip.Location = new System.Drawing.Point(0, 341);
            this.m_statusStrip.Name = "m_statusStrip";
            this.m_statusStrip.Size = new System.Drawing.Size(792, 22);
            this.m_statusStrip.SizingGrip = false;
            this.m_statusStrip.TabIndex = 6;
            this.m_statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(102, 17);
            this.toolStripStatusLabel1.Text = "L.O.G. Multiplayer";
            // 
            // Ping
            // 
            this.Ping.HeaderText = "Ping";
            this.Ping.Name = "Ping";
            this.Ping.ReadOnly = true;
            // 
            // Players
            // 
            this.Players.HeaderText = "Players";
            this.Players.Name = "Players";
            this.Players.ReadOnly = true;
            // 
            // HostName
            // 
            this.HostName.HeaderText = "HostName";
            this.HostName.Name = "HostName";
            this.HostName.ReadOnly = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.panel2.Location = new System.Drawing.Point(42, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1, 35);
            this.panel2.TabIndex = 1;
            // 
            // m_playButton
            // 
            this.m_playButton.BackgroundImage = global::LOG.Launcher.Properties.Resources.Play;
            this.m_playButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.m_playButton.Location = new System.Drawing.Point(6, 2);
            this.m_playButton.Name = "m_playButton";
            this.m_playButton.Size = new System.Drawing.Size(30, 30);
            this.m_playButton.TabIndex = 0;
            this.m_playButton.UseVisualStyleBackColor = true;
            this.m_playButton.Click += new System.EventHandler(this.m_playButton_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
            this.panel1.Controls.Add(this.panel6);
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.NameLabel);
            this.panel1.Controls.Add(this.m_userNameTextBox);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.m_refreshButton);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.m_playButton);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(792, 35);
            this.panel1.TabIndex = 4;
            // 
            // m_serversList
            // 
            this.m_serversList.AllowUserToAddRows = false;
            this.m_serversList.AllowUserToDeleteRows = false;
            this.m_serversList.AllowUserToResizeRows = false;
            this.m_serversList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.m_serversList.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(216)))), ((int)(((byte)(216)))));
            this.m_serversList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.m_serversList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_serversList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.HostName,
            this.Players,
            this.Ping});
            this.m_serversList.Location = new System.Drawing.Point(0, 34);
            this.m_serversList.MultiSelect = false;
            this.m_serversList.Name = "m_serversList";
            this.m_serversList.ReadOnly = true;
            this.m_serversList.RowHeadersVisible = false;
            this.m_serversList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.m_serversList.Size = new System.Drawing.Size(792, 307);
            this.m_serversList.TabIndex = 5;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 363);
            this.Controls.Add(this.m_statusStrip);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.m_serversList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "L.O.G. Multiplayer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.m_statusStrip.ResumeLayout(false);
            this.m_statusStrip.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_serversList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.ComponentModel.BackgroundWorker m_loadingServersWorker;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.TextBox m_userNameTextBox;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button m_refreshButton;
        private System.Windows.Forms.StatusStrip m_statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Ping;
        private System.Windows.Forms.DataGridViewTextBoxColumn Players;
        private System.Windows.Forms.DataGridViewTextBoxColumn HostName;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button m_playButton;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.DataGridView m_serversList;
    }
}

