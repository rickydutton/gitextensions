namespace GitUI.CommandsDialogs.SettingsDialog.Pages
{
    partial class IssueTrackerIntegrationSettingsPage
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
            this.issueTrackerSettingsPanel = new System.Windows.Forms.Panel();
            this.issueTrackerType = new System.Windows.Forms.ComboBox();
            this.labelIssueTrackerType = new System.Windows.Forms.Label();
            this.checkBoxEnableIssueTrackerIntegration = new System.Windows.Forms.CheckBox();
            this.labelIssueTrackerSettingsInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // issueTrackerSettingsPanel
            // 
            this.issueTrackerSettingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.issueTrackerSettingsPanel.Location = new System.Drawing.Point(16, 144);
            this.issueTrackerSettingsPanel.MinimumSize = new System.Drawing.Size(400, 227);
            this.issueTrackerSettingsPanel.Name = "issueTrackerSettingsPanel";
            this.issueTrackerSettingsPanel.Size = new System.Drawing.Size(828, 227);
            this.issueTrackerSettingsPanel.TabIndex = 5;
            // 
            // issueTrackerType
            // 
            this.issueTrackerType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.issueTrackerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.issueTrackerType.Enabled = false;
            this.issueTrackerType.FormattingEnabled = true;
            this.issueTrackerType.Location = new System.Drawing.Point(122, 89);
            this.issueTrackerType.Name = "issueTrackerType";
            this.issueTrackerType.Size = new System.Drawing.Size(722, 23);
            this.issueTrackerType.TabIndex = 4;
            this.issueTrackerType.SelectedIndexChanged += new System.EventHandler(this.IssueTrackerType_SelectedIndexChanged);
            // 
            // labelIssueTrackerType
            // 
            this.labelIssueTrackerType.AutoSize = true;
            this.labelIssueTrackerType.Location = new System.Drawing.Point(13, 92);
            this.labelIssueTrackerType.Name = "labelIssueTrackerType";
            this.labelIssueTrackerType.Size = new System.Drawing.Size(98, 15);
            this.labelIssueTrackerType.TabIndex = 3;
            this.labelIssueTrackerType.Text = "Issue tracker type";
            // 
            // checkBoxEnableIssueTrackerIntegration
            // 
            this.checkBoxEnableIssueTrackerIntegration.AutoSize = true;
            this.checkBoxEnableIssueTrackerIntegration.Enabled = false;
            this.checkBoxEnableIssueTrackerIntegration.Location = new System.Drawing.Point(13, 65);
            this.checkBoxEnableIssueTrackerIntegration.Name = "checkBoxEnableIssueTrackerIntegration";
            this.checkBoxEnableIssueTrackerIntegration.Size = new System.Drawing.Size(190, 19);
            this.checkBoxEnableIssueTrackerIntegration.TabIndex = 1;
            this.checkBoxEnableIssueTrackerIntegration.Text = "Enable issue tracker integration";
            this.checkBoxEnableIssueTrackerIntegration.ThreeState = true;
            this.checkBoxEnableIssueTrackerIntegration.UseVisualStyleBackColor = true;
            // 
            // labelIssueTrackerSettingsInfo
            // 
            this.labelIssueTrackerSettingsInfo.AutoSize = true;
            this.labelIssueTrackerSettingsInfo.Location = new System.Drawing.Point(10, 18);
            this.labelIssueTrackerSettingsInfo.Name = "labelIssueTrackerSettingsInfo";
            this.labelIssueTrackerSettingsInfo.Size = new System.Drawing.Size(561, 15);
            this.labelIssueTrackerSettingsInfo.TabIndex = 0;
            this.labelIssueTrackerSettingsInfo.Text = "Git Extensions can integrate with build servers to supply per-commit Continuous I" +
    "ntegration information.";
            // 
            // IssueTrackerIntegrationSettingsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelIssueTrackerSettingsInfo);
            this.Controls.Add(this.checkBoxEnableIssueTrackerIntegration);
            this.Controls.Add(this.issueTrackerSettingsPanel);
            this.Controls.Add(this.issueTrackerType);
            this.Controls.Add(this.labelIssueTrackerType);
            this.MinimumSize = new System.Drawing.Size(530, 330);
            this.Name = "IssueTrackerIntegrationSettingsPage";
            this.Size = new System.Drawing.Size(530, 1024);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel issueTrackerSettingsPanel;
        private System.Windows.Forms.ComboBox issueTrackerType;
        private System.Windows.Forms.Label labelIssueTrackerType;
        private System.Windows.Forms.CheckBox checkBoxEnableIssueTrackerIntegration;
        private System.Windows.Forms.Label labelIssueTrackerSettingsInfo;

    }
}