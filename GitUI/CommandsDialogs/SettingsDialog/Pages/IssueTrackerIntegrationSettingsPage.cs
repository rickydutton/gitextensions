using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitUIPluginInterfaces;
using GitUIPluginInterfaces.IssueTrackerIntegration;
using ResourceManager;

namespace GitUI.CommandsDialogs.SettingsDialog.Pages
{
    public partial class IssueTrackerIntegrationSettingsPage : RepoDistSettingsPage
    {
        private readonly TranslationString _noneItem =
            new TranslationString("None");
        private Task<object> _populateIssueTrackerTypeTask;

        public IssueTrackerIntegrationSettingsPage()
        {
            InitializeComponent();
            Text = "Issue tracker integration";
            Translate();
        }

        protected override void Init(ISettingsPageHost aPageHost)
        {
            base.Init(aPageHost);

            _populateIssueTrackerTypeTask =
                Task.Factory.StartNew(() =>
                        {
                            var exports = ManagedExtensibility.GetExports<IIssueTrackerAdapter, IIssueTrackerTypeMetadata>();
                            var issueTrackerTypes = exports.Select(export =>
                                {
                                    var canBeLoaded = export.Metadata.CanBeLoaded;
                                    return export.Metadata.IssueTrackerType.Combine(" - ", canBeLoaded);
                                }).ToArray();

                            return issueTrackerTypes;
                        })
                    .ContinueWith(
                        task =>
                            {
                                checkBoxEnableIssueTrackerIntegration.Enabled = true;
                                issueTrackerType.Enabled = true;

                                issueTrackerType.DataSource = new[] { _noneItem.Text }.Concat(task.Result).ToArray();
                                return issueTrackerType.DataSource;
                            },
                        TaskScheduler.FromCurrentSynchronizationContext());
        }

        public override bool IsInstantSavePage
        {
            get { return false; }
        }

        protected override void SettingsToPage()
        {
            _populateIssueTrackerTypeTask.ContinueWith(
                task =>
                {
                    checkBoxEnableIssueTrackerIntegration.SetNullableChecked(CurrentSettings.IssueTracker.EnableIntegration.Value);
                    issueTrackerType.SelectedItem = CurrentSettings.IssueTracker.Type.Value ?? _noneItem.Text;
                },
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected override void PageToSettings()
        {
            CurrentSettings.IssueTracker.EnableIntegration.Value = checkBoxEnableIssueTrackerIntegration.GetNullableChecked();
            
            var selectedIssueTrackerType = GetSelectedIssueTrackerType();

            CurrentSettings.IssueTracker.Type.Value = selectedIssueTrackerType;

            var control =
                issueTrackerSettingsPanel.Controls.OfType<IIssueTrackerSettingsUserControl>()
                                        .SingleOrDefault();
            if (control != null)
                control.SaveSettings(CurrentSettings.IssueTracker.TypeSettings);
        }

        private void ActivateIssueTrackerSettingsControl()
        {
            var controls = issueTrackerSettingsPanel.Controls.OfType<IIssueTrackerSettingsUserControl>().Cast<Control>();
            var previousControl = controls.SingleOrDefault();
            if (previousControl != null) previousControl.Dispose();

            var control = CreateIssueTrackerSettingsUserControl();

            issueTrackerSettingsPanel.Controls.Clear();

            if (control != null)
            {
                control.LoadSettings(CurrentSettings.IssueTracker.TypeSettings);

                issueTrackerSettingsPanel.Controls.Add((Control)control);
            }
        }

        private IIssueTrackerSettingsUserControl CreateIssueTrackerSettingsUserControl()
        {
            if (issueTrackerType.SelectedIndex == 0 || string.IsNullOrEmpty(Module.WorkingDir))
                return null;
            var defaultProjectName = Module.WorkingDir.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Last();

            var exports = ManagedExtensibility.GetExports<IIssueTrackerSettingsUserControl, IIssueTrackerTypeMetadata>();
            var selectedExport = exports.SingleOrDefault(export => export.Metadata.IssueTrackerType == GetSelectedIssueTrackerType());
            if (selectedExport != null)
            {
                var issueTrackerSettingsUserControl = selectedExport.Value;
                issueTrackerSettingsUserControl.Initialize(defaultProjectName);
                return issueTrackerSettingsUserControl;
            }

            return null;
        }

        private string GetSelectedIssueTrackerType()
        {
            if (issueTrackerType.SelectedIndex == 0)
                return null;
            return (string)issueTrackerType.SelectedItem;
        }

        private void IssueTrackerType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ActivateIssueTrackerSettingsControl();
        }
    }
}
