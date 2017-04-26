using System;
using System.Windows.Forms;
using GitUIPluginInterfaces.IssueTrackerIntegration;

namespace GitUI.HelperDialogs
{
    public partial class FormIssueTrackerCredentials : Form
    {
        public FormIssueTrackerCredentials(string issueTrackerUniqueKey)
        {
            InitializeComponent();

            labelHeader.Text = string.Format(labelHeader.Text, issueTrackerUniqueKey);
        }

        public IIssueTrackerCredentials IssueTrackerCredentials { get; set; }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (IssueTrackerCredentials == null)
                IssueTrackerCredentials = new IssueTrackerCredentials();

            IssueTrackerCredentials.Username = textBoxUserName.Text;
            IssueTrackerCredentials.Password = textBoxPassword.Text;

            Close();
        }

        private void FormIssueTrackerCredentials_Load(object sender, EventArgs e)
        {
            if (IssueTrackerCredentials != null)
            {
                textBoxUserName.Text = IssueTrackerCredentials.Username;
                textBoxPassword.Text = IssueTrackerCredentials.Password;
            }
        }
    }
}
