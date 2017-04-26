using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GitUIPluginInterfaces.IssueTrackerIntegration
{
    public class IssueContainer
    {
        public List<Issue> Issues { get; set; }
    }

    public class Issue
    {
        public long id { get; set; }

        public string key { get; set; }
        public string status { get; set; }
        public DateTime lastupdated { get; set; }

        public ConcurrentBag<string> commithashes { get; set; } = new ConcurrentBag<string>();
    }
}
    