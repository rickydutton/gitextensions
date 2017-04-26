
namespace GitUIPluginInterfaces.IssueTrackerIntegration
{
    public interface IIssueTrackerSettingsUserControl
    {
        void Initialize(string projectName);

        void LoadSettings(ISettingsSource issueTrackerConfig);
        void SaveSettings(ISettingsSource issueTrackerConfig);
    }
}