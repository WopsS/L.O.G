namespace LOG.Launcher
{
    partial class AddServer
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
            this.label1 = new System.Windows.Forms.Label();
            this.ServerIPText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ServerPortText = new System.Windows.Forms.TextBox();
            this.AddServerButton = new System.Windows.Forms.Button();
            this.CancelButtonForm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(235, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Add below IP address for server and description:";
            // 
            // ServerIPText
            // 
            this.ServerIPText.Location = new System.Drawing.Point(149, 51);
            this.ServerIPText.Name = "ServerIPText";
            this.ServerIPText.Size = new System.Drawing.Size(118, 20);
            this.ServerIPText.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(89, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Server IP:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(81, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Server Port:";
            // 
            // ServerPortText
            // 
            this.ServerPortText.Location = new System.Drawing.Point(149, 86);
            this.ServerPortText.Name = "ServerPortText";
            this.ServerPortText.Size = new System.Drawing.Size(118, 20);
            this.ServerPortText.TabIndex = 4;
            this.ServerPortText.Text = "4198";
            this.ServerPortText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ServerPortText_KeyPress);
            // 
            // AddServerButton
            // 
            this.AddServerButton.Location = new System.Drawing.Point(112, 127);
            this.AddServerButton.Name = "AddServerButton";
            this.AddServerButton.Size = new System.Drawing.Size(75, 23);
            this.AddServerButton.TabIndex = 5;
            this.AddServerButton.Text = "OK";
            this.AddServerButton.UseVisualStyleBackColor = true;
            this.AddServerButton.Click += new System.EventHandler(this.AddServerButton_Click);
            // 
            // CancelButtonForm
            // 
            this.CancelButtonForm.Location = new System.Drawing.Point(208, 127);
            this.CancelButtonForm.Name = "CancelButtonForm";
            this.CancelButtonForm.Size = new System.Drawing.Size(75, 23);
            this.CancelButtonForm.TabIndex = 6;
            this.CancelButtonForm.Text = "Cancel";
            this.CancelButtonForm.UseVisualStyleBackColor = true;
            this.CancelButtonForm.Click += new System.EventHandler(this.CancelButtonForm_Click);
            // 
            // AddServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 162);
            this.Controls.Add(this.CancelButtonForm);
            this.Controls.Add(this.AddServerButton);
            this.Controls.Add(this.ServerPortText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ServerIPText);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 200);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "AddServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ServerIPText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ServerPortText;
        private System.Windows.Forms.Button AddServerButton;
        private System.Windows.Forms.Button CancelButtonForm;
    }
}