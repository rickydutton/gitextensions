using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using GitUIPluginInterfaces.IssueTrackerIntegration;

namespace JIRAIntegration
{
    public class SqlCeIssueTrackerCache : IItemCache<long, Issue>
    {
        private SqlCeConnection GetIssueDbConnection()
        {
            var cn = new SqlCeConnection(GetIssueDbConnectionString());
            if (cn.State == ConnectionState.Closed)
                cn.Open();
            return cn;
        }

        private string GetIssueDbConnectionString()
        {
            return $"DataSource=\"{_issueDbFileName}\"; Password='{_issueDbPassword}'";
        }

        private int _refreshIntervalMins = 5;
        private const string _issueDbFileName = "jira_issues.sdf";
        private const string _issueDbPassword = "jira1234";

        private ConcurrentDictionary<long, Issue> _items = new ConcurrentDictionary<long, Issue>();
        public IDictionary<long, Issue> Items { get { return _items; } set { _items = value as ConcurrentDictionary<long, Issue>; } }
        public void Initialize(int refreshIntervalMins)
        {
            _refreshIntervalMins = refreshIntervalMins;

            if (!File.Exists(_issueDbFileName))
            {
                var db = new SqlCeEngine(GetIssueDbConnectionString());
                db.CreateDatabase();

                using (var cn = GetIssueDbConnection())
                {
                    string tableSql = "create table IssueCache("
                                      + "IssueId bigint not null,"
                                      + "IssueKey nvarchar(20),"
                                      + "Status nvarchar(50),"
                                      + "LastUpdated datetime)";

                    string hashTableSql = "create table IssueCommitCache(" +
                                          "IssueId bigint not null," +
                                          "CommitHash nvarchar(50) not null);";

                    var createTableCmd = new SqlCeCommand(tableSql, cn);
                    var hashTableCmd = new SqlCeCommand(hashTableSql, cn);

                    createTableCmd.ExecuteNonQuery();
                    hashTableCmd.ExecuteNonQuery();
                }
            }
            using (var cn = GetIssueDbConnection())
            {
                var getIssuesCmd = new SqlCeCommand("select * from IssueCache", cn);
                var rdr = getIssuesCmd.ExecuteReader();
                while (rdr.Read())
                {
                    var issueId = (long)rdr[0];
                    var issue = new Issue
                    {
                        id = issueId,
                        key = (string)rdr[1],
                        status = (string)rdr[2],
                        lastupdated = (DateTime)rdr[3]
                    };
                    using (var cn2 = GetIssueDbConnection())
                    {
                        var getHashCmd = new SqlCeCommand("select * from IssueCommitCache where IssueId = @issueid", cn2);
                        getHashCmd.Parameters.AddWithValue("@issueid", issue.id);
                        var hashRdr = getHashCmd.ExecuteReader();
                        while (hashRdr.Read())
                        {
                            issue.commithashes.Add((string)hashRdr[1]);
                        }
                    }
                    _items.AddOrUpdate(issueId, i => issue, (k, i) => issue);
                }
            }
        }

        public void AddOrUpdateItem(long issueId, Issue issue)
        {
            if (issue.id < 1)
                return;

            using (var cn = GetIssueDbConnection())
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();



                var cmd = new SqlCeCommand("delete from IssueCache where IssueId=@issueid", cn);
                cmd.Parameters.AddWithValue("@issueid", issue.id);
                cmd.ExecuteNonQuery();

                cmd = new SqlCeCommand("insert into IssueCache (IssueId, IssueKey, LastUpdated, Status) values(@issueid, @issuekey, @lastupdated, @status)", cn);
                cmd.Parameters.AddWithValue("@lastupdated", issue.lastupdated);
                cmd.Parameters.AddWithValue("@issueid", issue.id);
                cmd.Parameters.AddWithValue("@issuekey", issue.key);
                cmd.Parameters.AddWithValue("@status", issue.status);
                cmd.ExecuteNonQuery();

                cmd = new SqlCeCommand("delete from IssueCommitCache where IssueId=@issueid;", cn);
                cmd.Parameters.AddWithValue("@issueid", issue.id);
                cmd.ExecuteNonQuery();

                var insertCommitSql = "insert into IssueCommitCache(IssueId, CommitHash) values(@issueid, @commithash)";

                foreach (var hash in issue.commithashes)
                {
                    using (var cn2 = GetIssueDbConnection())
                    {
                        if (cn2.State == ConnectionState.Closed)
                            cn2.Open();

                        cmd = new SqlCeCommand(insertCommitSql, cn2);
                        cmd.Parameters.AddWithValue("@issueid", issue.id);
                        cmd.Parameters.AddWithValue("@commithash", hash);
                        cmd.ExecuteNonQuery();
                    }
                }
                _items.AddOrUpdate(issue.id, i => issue, (k, i) => issue);
            }
        }
        public IEnumerable<KeyValuePair<long, Issue>> GetValidItems()
        {
            return _items.Where(i => i.Value.lastupdated.AddMinutes(_refreshIntervalMins).CompareTo(DateTime.Now) <= 0);
        }
        public IEnumerable<KeyValuePair<long, Issue>> GetExpiredItems()
        {
            return _items.Where(i => i.Value.lastupdated.AddMinutes(_refreshIntervalMins).CompareTo(DateTime.Now) > 0);
        }
    }
}