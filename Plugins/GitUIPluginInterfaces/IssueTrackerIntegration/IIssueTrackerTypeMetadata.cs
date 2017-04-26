using System;

namespace GitUIPluginInterfaces.IssueTrackerIntegration
{
    public interface IIssueTrackerTypeMetadata
    {
        string IssueTrackerType { get; }

        /// <summary>
        /// returns null if can be loaded, the reason if can't
        /// </summary>
        string CanBeLoaded { get; }
    }
}