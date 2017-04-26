namespace GitUIPluginInterfaces.IssueTrackerIntegration
{
    public class IssueTrackerCredentials : IIssueTrackerCredentials
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}