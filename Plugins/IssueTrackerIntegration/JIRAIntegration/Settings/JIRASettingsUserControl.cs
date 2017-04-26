using System.ComponentModel.Composition;
using System.Windows.Forms;
using GitCommands.Settings;
using GitUIPluginInterfaces;
using GitUIPluginInterfaces.IssueTrackerIntegration;
using ResourceManager;

namespace JIRAIntegration.Settings
{
    [Export(typeof(IIssueTrackerSettingsUserControl))]
    [IssueTrackerSettingsUserControlMetadata("JIRA")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class JIRASettingsUserControl : GitExtensionsControl, IIssueTrackerSettingsUserControl
    {
        private string _defaultProjectName;

        public JIRASettingsUserControl()
        {
            InitializeComponent();
            Translate();

            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        }

        public void Initialize(string defaultProjectName)
        {
            _defaultProjectName = defaultProjectName;
        }

        public void LoadSettings(ISettingsSource IssueTrackerConfig)
        {
            if (IssueTrackerConfig != null)
            {
                JIRAServerUrl.Text = IssueTrackerConfig.GetString("ServerUrl", string.Empty);
                JIRAUsername.Text = IssueTrackerConfig.GetString("Username", string.Empty);
                JIRAPassword.Text = IssueTrackerConfig.GetString("Password", string.Empty);
            }
        }

        public void SaveSettings(ISettingsSource IssueTrackerConfig)
        {
            IssueTrackerConfig.SetString("ServerUrl", JIRAServerUrl.Text);
            IssueTrackerConfig.SetString("Username", JIRAUsername.Text);
            IssueTrackerConfig.SetString("Password", JIRAPassword.Text);
        }
    }
}
