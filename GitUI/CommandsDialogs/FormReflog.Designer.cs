namespace GitUI.CommandsDialogs
{
    partial class FormReflog
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
            this.components = new System.ComponentModel.Container();
            this.Branches = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.rebasePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.gridReflog = new System.Windows.Forms.DataGridView();
            this.Sha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Ref = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Action = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStripReflog = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createABranchOnThisCommitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetCurrentBranchOnThisCommitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.rebasePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridReflog)).BeginInit();
            this.contextMenuStripReflog.SuspendLayout();
            this.SuspendLayout();
            // 
            // Branches
            // 
            this.Branches.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Branches.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.Branches.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.Branches.FormattingEnabled = true;
            this.Branches.Location = new System.Drawing.Point(86, 4);
            this.Branches.Margin = new System.Windows.Forms.Padding(4);
            this.Branches.Name = "Branches";
            this.Branches.Size = new System.Drawing.Size(272, 24);
            this.Branches.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Reference:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 590F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(782, 555);
            this.tableLayoutPanel1.TabIndex = 19;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.rebasePanel, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.gridReflog, 0, 5);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 6;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(774, 547);
            this.tableLayoutPanel3.TabIndex = 32;
            // 
            // rebasePanel
            // 
            this.rebasePanel.AutoSize = true;
            this.rebasePanel.Controls.Add(this.label2);
            this.rebasePanel.Controls.Add(this.Branches);
            this.rebasePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rebasePanel.Location = new System.Drawing.Point(4, 4);
            this.rebasePanel.Margin = new System.Windows.Forms.Padding(4);
            this.rebasePanel.Name = "rebasePanel";
            this.rebasePanel.Size = new System.Drawing.Size(766, 32);
            this.rebasePanel.TabIndex = 32;
            // 
            // gridReflog
            // 
            this.gridReflog.AllowUserToAddRows = false;
            this.gridReflog.AllowUserToDeleteRows = false;
            this.gridReflog.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            this.gridReflog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Sha,
            this.Ref,
            this.Action,
            this.Message});
            this.gridReflog.ContextMenuStrip = this.contextMenuStripReflog;
            this.gridReflog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridReflog.Location = new System.Drawing.Point(3, 43);
            this.gridReflog.Name = "gridReflog";
            this.gridReflog.ReadOnly = true;
            this.gridReflog.RowTemplate.Height = 24;
            this.gridReflog.Size = new System.Drawing.Size(768, 501);
            this.gridReflog.TabIndex = 33;
            // 
            // Sha
            // 
            this.Sha.DataPropertyName = "Sha";
            this.Sha.HeaderText = "Sha-1";
            this.Sha.Name = "Sha";
            this.Sha.ReadOnly = true;
            this.Sha.Width = 5;
            // 
            // Ref
            // 
            this.Ref.DataPropertyName = "Ref";
            this.Ref.HeaderText = "Ref";
            this.Ref.Name = "Ref";
            this.Ref.ReadOnly = true;
            this.Ref.Width = 5;
            // 
            // Action
            // 
            this.Action.DataPropertyName = "Action";
            this.Action.HeaderText = "Action";
            this.Action.Name = "Action";
            this.Action.ReadOnly = true;
            this.Action.Width = 5;
            // 
            // Message
            // 
            this.Message.DataPropertyName = "Message";
            this.Message.HeaderText = "Message";
            this.Message.Name = "Message";
            this.Message.ReadOnly = true;
            this.Message.Width = 5;
            // 
            // contextMenuStripReflog
            // 
            this.contextMenuStripReflog.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripReflog.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createABranchOnThisCommitToolStripMenuItem,
            this.resetCurrentBranchOnThisCommitToolStripMenuItem});
            this.contextMenuStripReflog.Name = "contextMenuStripReflog";
            this.contextMenuStripReflog.Size = new System.Drawing.Size(323, 56);
            // 
            // createABranchOnThisCommitToolStripMenuItem
            // 
            this.createABranchOnThisCommitToolStripMenuItem.Name = "createABranchOnThisCommitToolStripMenuItem";
            this.createABranchOnThisCommitToolStripMenuItem.Size = new System.Drawing.Size(322, 26);
            this.createABranchOnThisCommitToolStripMenuItem.Text = "Create a branch on this commit";
            this.createABranchOnThisCommitToolStripMenuItem.Click += new System.EventHandler(this.createABranchOnThisCommitToolStripMenuItem_Click);
            // 
            // resetCurrentBranchOnThisCommitToolStripMenuItem
            // 
            this.resetCurrentBranchOnThisCommitToolStripMenuItem.Name = "resetCurrentBranchOnThisCommitToolStripMenuItem";
            this.resetCurrentBranchOnThisCommitToolStripMenuItem.Size = new System.Drawing.Size(322, 26);
            this.resetCurrentBranchOnThisCommitToolStripMenuItem.Text = "Reset current branch on this commit";
            this.resetCurrentBranchOnThisCommitToolStripMenuItem.Click += new System.EventHandler(this.resetCurrentBranchOnThisCommitToolStripMenuItem_Click);
            // 
            // FormReflog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(782, 555);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "FormReflog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Reflog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.rebasePanel.ResumeLayout(false);
            this.rebasePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridReflog)).EndInit();
            this.contextMenuStripReflog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox Branches;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel rebasePanel;
        private System.Windows.Forms.DataGridView gridReflog;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sha;
        private System.Windows.Forms.DataGridViewTextBoxColumn Ref;
        private System.Windows.Forms.DataGridViewTextBoxColumn Action;
        private System.Windows.Forms.DataGridViewTextBoxColumn Message;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripReflog;
        private System.Windows.Forms.ToolStripMenuItem createABranchOnThisCommitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetCurrentBranchOnThisCommitToolStripMenuItem;
    }
}