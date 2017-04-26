namespace GitUIPluginInterfaces.IssueTrackerIntegration
{
    public interface IIssueTrackerWatcher
    {
        IIssueTrackerCredentials GetIssueTrackerCredentials(IIssueTrackerAdapter issueTrackerAdapter, bool useStoredCredentialsIfExisting);

        void LaunchIssueTrackerInfoFetchOperation();

        void CancelIssueTrackerFetchOperation();
    }
}