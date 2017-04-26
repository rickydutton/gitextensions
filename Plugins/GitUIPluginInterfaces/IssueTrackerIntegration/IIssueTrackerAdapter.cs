using System;
using System.Reactive.Concurrency;

namespace GitUIPluginInterfaces.IssueTrackerIntegration
{
    public interface IIssueTrackerAdapter : IDisposable
    {
        void Initialize(IIssueTrackerWatcher issueTrackerWatcher, ISettingsSource config);

        /// <summary>
        /// Gets a unique key which identifies this issue tracker.
        /// </summary>
        string UniqueKey { get; }

        IObservable<Issue> GetActiveIssues(IScheduler scheduler);
    }
}