using System;
using System.ComponentModel.Composition;

namespace GitUIPluginInterfaces.IssueTrackerIntegration
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IssueTrackerAdapterMetadataAttribute : ExportAttribute
    {
        public IssueTrackerAdapterMetadataAttribute(string issueTrackerType)
            : base(typeof(IIssueTrackerTypeMetadata))
        {
            if (string.IsNullOrEmpty(issueTrackerType))
                throw new ArgumentException();
            
            IssueTrackerType = issueTrackerType;
        }

        public string IssueTrackerType { get; private set; }

        public virtual string CanBeLoaded
        {
            get
            {
                return null;
            }
        }
    }
}