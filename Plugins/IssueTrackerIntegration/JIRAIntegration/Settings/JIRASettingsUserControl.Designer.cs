namespace JIRAIntegration.Settings
{
    partial class JIRASettingsUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label13;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label labelBuildFilter;
            this.JIRAServerUrl = new System.Windows.Forms.TextBox();
            this.JIRAUsername = new System.Windows.Forms.TextBox();
            this.JIRAPassword = new System.Windows.Forms.TextBox();
            label13 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            labelBuildFilter = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // JIRAServerUrl
            // 
            this.JIRAServerUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.JIRAServerUrl.Location = new System.Drawing.Point(136, 8);
            this.JIRAServerUrl.Name = "JIRAServerUrl";
            this.JIRAServerUrl.Size = new System.Drawing.Size(318, 23);
            this.JIRAServerUrl.TabIndex = 1;
            // 
            // JIRAUsername
            // 
            this.JIRAUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.JIRAUsername.Location = new System.Drawing.Point(136, 37);
            this.JIRAUsername.Name = "JIRAUsername";
            this.JIRAUsername.Size = new System.Drawing.Size(318, 23);
            this.JIRAUsername.TabIndex = 3;
            // 
            // JIRAPassword
            // 
            this.JIRAPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.JIRAPassword.Location = new System.Drawing.Point(136, 67);
            this.JIRAPassword.Name = "JIRAPassword";
            this.JIRAPassword.Size = new System.Drawing.Size(318, 23);
            this.JIRAPassword.TabIndex = 5;
            this.JIRAPassword.UseSystemPasswordChar = true;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(34, 12);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(88, 15);
            label13.TabIndex = 0;
            label13.Text = "JIRA Server URL";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(58, 41);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(60, 15);
            label1.TabIndex = 2;
            label1.Text = "Username";
            // 
            // labelBuildFilter
            // 
            labelBuildFilter.AutoSize = true;
            labelBuildFilter.Location = new System.Drawing.Point(65, 70);
            labelBuildFilter.Name = "labelBuildFilter";
            labelBuildFilter.Size = new System.Drawing.Size(57, 15);
            labelBuildFilter.TabIndex = 4;
            labelBuildFilter.Text = "Password";
            // 
            // JIRASettingsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(labelBuildFilter);
            this.Controls.Add(this.JIRAPassword);
            this.Controls.Add(label1);
            this.Controls.Add(label13);
            this.Controls.Add(this.JIRAUsername);
            this.Controls.Add(this.JIRAServerUrl);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "JIRASettingsUserControl";
            this.Size = new System.Drawing.Size(467, 136);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox JIRAServerUrl;
        private System.Windows.Forms.TextBox JIRAUsername;
        private System.Windows.Forms.TextBox JIRAPassword;
    }
}
