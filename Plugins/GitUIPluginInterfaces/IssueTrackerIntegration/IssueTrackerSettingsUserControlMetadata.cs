using System;
using System.ComponentModel.Composition;

namespace GitUIPluginInterfaces.IssueTrackerIntegration
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IssueTrackerSettingsUserControlMetadata : IssueTrackerAdapterMetadataAttribute
    {
        public IssueTrackerSettingsUserControlMetadata(string issueTrackerType)
            : base(issueTrackerType)
        {
        }
    }
}