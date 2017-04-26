using System;
using System.ComponentModel.Composition;
using GitCommands.Utils;
using GitUIPluginInterfaces.IssueTrackerIntegration;

namespace JIRAIntegration
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class JiraIntegrationMetadata : IssueTrackerAdapterMetadataAttribute
    {
        public JiraIntegrationMetadata(string issueTrackerType)
            : base(issueTrackerType)
        {
        }

        public override string CanBeLoaded
        {
            get
            {
                if (EnvUtils.IsNet4FullOrHigher())
                    return null;
                else
                    return ".Net 4 full framework required";
            }
        }
    }
}