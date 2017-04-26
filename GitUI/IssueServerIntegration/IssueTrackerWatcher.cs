using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GitCommands;
using GitCommands.Config;
using GitUI.AutoCompletion;
using GitUI.HelperDialogs;
using GitUI.RevisionGridClasses;
using GitUIPluginInterfaces;
using GitUIPluginInterfaces.IssueTrackerIntegration;
using IIssueTrackerWatcher = GitUIPluginInterfaces.IssueTrackerIntegration.IIssueTrackerWatcher;

namespace GitUI.IssueTrackerIntegration
{
    public class IssueTrackerWatcher : IIssueTrackerWatcher, IDisposable
    {
        private readonly RevisionGrid revisionGrid;
        private readonly DvcsGraph revisions;
        private GitModule Module { get { return revisionGrid.Module; } }

        public int IssueStatusImageColumnIndex { get; private set; }
        public int IssueStatusMessageColumnIndex { get; private set; }

        private IDisposable IssueStatusCancellationToken;
        private IIssueTrackerAdapter IssueTrackerAdapter;

        private readonly object IssueTrackerCredentialsLock = new object();

        public IssueTrackerWatcher(RevisionGrid revisionGrid, DvcsGraph revisions)
        {
            this.revisionGrid = revisionGrid;
            this.revisions = revisions;
            IssueStatusImageColumnIndex = -1;
            IssueStatusMessageColumnIndex = -1;
        }

        public void LaunchIssueTrackerInfoFetchOperation()
        {
            CancelIssueTrackerFetchOperation();

            DisposeIssueTrackerAdapter();
            
            IssueTrackerAdapter = GetIssueTrackerAdapter();

            UpdateUI();

            if (IssueTrackerAdapter == null)
                return;

            var scheduler = NewThreadScheduler.Default;

            var runningIssuesObservable = IssueTrackerAdapter.GetActiveIssues(scheduler);

            var cancellationToken = new CompositeDisposable
                {
                    runningIssuesObservable.OnErrorResumeNext(Observable.Empty<Issue>()
                                                                        .DelaySubscription(TimeSpan.FromSeconds(10)))
                                           .Retry()
                                           .Repeat()
                                           .ObserveOn(SynchronizationContext.Current)
                                           .Subscribe(OnIssueInfoUpdate)
                };

            IssueStatusCancellationToken = cancellationToken;
        }

        

        public void CancelIssueTrackerFetchOperation()
        {
            var cancellationToken = Interlocked.Exchange(ref IssueStatusCancellationToken, null);

            if (cancellationToken != null)
            {
                cancellationToken.Dispose();
            }
        }
        

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "http://stackoverflow.com/questions/1065168/does-disposing-streamreader-close-the-stream")]
        public IIssueTrackerCredentials GetIssueTrackerCredentials(IIssueTrackerAdapter IssueTrackerAdapter, bool useStoredCredentialsIfExisting)
        {
            lock (IssueTrackerCredentialsLock)
            {
                IIssueTrackerCredentials IssueTrackerCredentials = new IssueTrackerCredentials();

                const string CredentialsConfigName = "Credentials";

                const string UsernameKey = "Username";
                const string PasswordKey = "Password";
                using (var stream = GetIssueTrackerOptionsIsolatedStorageStream(IssueTrackerAdapter, FileAccess.Read, FileShare.Read))
                {
                    
                    if (stream.Position < stream.Length)
                    {
                        var protectedData = new byte[stream.Length];

                        stream.Read(protectedData, 0, (int)stream.Length);

                        byte[] unprotectedData = ProtectedData.Unprotect(protectedData, null, DataProtectionScope.CurrentUser);
                        using (var memoryStream = new MemoryStream(unprotectedData))
                        {
                            ConfigFile credentialsConfig = new ConfigFile("", false);

                            using (var textReader = new StreamReader(memoryStream, Encoding.UTF8))
                            {
                                credentialsConfig.LoadFromString(textReader.ReadToEnd());
                            }

                            ConfigSection section = credentialsConfig.FindConfigSection(CredentialsConfigName);

                            if (section != null)
                            {
                                IssueTrackerCredentials.Username = section.GetValue(UsernameKey);
                                IssueTrackerCredentials.Password = section.GetValue(PasswordKey);

                                if (useStoredCredentialsIfExisting)
                                {
                                    return IssueTrackerCredentials;
                                }
                            }
                        }
                    }
                }

                if (!useStoredCredentialsIfExisting)
                {
                    IssueTrackerCredentials = ShowIssueTrackerCredentialsForm(IssueTrackerAdapter.UniqueKey, IssueTrackerCredentials);

                    if (IssueTrackerCredentials != null)
                    {
                        ConfigFile credentialsConfig = new ConfigFile("", true);

                        ConfigSection section = credentialsConfig.FindOrCreateConfigSection(CredentialsConfigName);

                        section.SetValue(UsernameKey, IssueTrackerCredentials.Username);
                        section.SetValue(PasswordKey, IssueTrackerCredentials.Password);

                        using (var stream = GetIssueTrackerOptionsIsolatedStorageStream(IssueTrackerAdapter, FileAccess.Write, FileShare.None))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                using (var textWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                                {
                                    textWriter.Write(credentialsConfig.GetAsString());
                                }

                                var protectedData = ProtectedData.Protect(memoryStream.ToArray(), null, DataProtectionScope.CurrentUser);
                                stream.Write(protectedData, 0, protectedData.Length);
                            }
                        }

                        return IssueTrackerCredentials;
                    }
                }

                return null;
            }
        }

        private IIssueTrackerCredentials ShowIssueTrackerCredentialsForm(string IssueTrackerUniqueKey, IIssueTrackerCredentials IssueTrackerCredentials)
        {
            if (revisionGrid.InvokeRequired)
            {
                return (IIssueTrackerCredentials)revisionGrid.Invoke(new Func<IIssueTrackerCredentials>(() => ShowIssueTrackerCredentialsForm(IssueTrackerUniqueKey, IssueTrackerCredentials)));
            }

            using (var form = new FormIssueTrackerCredentials(IssueTrackerUniqueKey))
            {
                form.IssueTrackerCredentials = IssueTrackerCredentials;

                if (form.ShowDialog(revisionGrid) == DialogResult.OK)
                {
                    return IssueTrackerCredentials;
                }
            }

            return null;
        }

        private void AddIssueStatusColumns()
        {
           
            if (IssueStatusImageColumnIndex == -1)
            {
                var IssueStatusImageColumn = new DataGridViewImageColumn
                {
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = 16,
                    ReadOnly = true,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                IssueStatusImageColumnIndex = revisions.Columns.Add(IssueStatusImageColumn);
            }
            
            if (IssueStatusMessageColumnIndex == -1)
            {
                var IssueMessageTextBoxColumn = new DataGridViewTextBoxColumn
                {
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    ReadOnly = true,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };

                IssueStatusMessageColumnIndex = revisions.Columns.Add(IssueMessageTextBoxColumn);
            }
      
        }

        private void OnIssueInfoUpdate(Issue issueInfo)
        {

            if (IssueStatusCancellationToken == null)
                return;


                
                foreach (var commitHash in issueInfo.commithashes)
                {
                    int row = revisionGrid.TrySearchRevision(commitHash);
                    if (row >= 0)
                    {
                        var rowData = revisions.GetRowData(row);
                        if (rowData.IssueStatus == null)
                        {
                            rowData.IssueStatus = issueInfo.status;

                            if (IssueStatusImageColumnIndex != -1)
                                revisions.UpdateCellValue(IssueStatusImageColumnIndex, row);
                            if (IssueStatusMessageColumnIndex != -1)
                                revisions.UpdateCellValue(IssueStatusMessageColumnIndex, row);
                        }
                    }
                }
            
        }

        private IIssueTrackerAdapter GetIssueTrackerAdapter()
        {
            if (!Module.EffectiveSettings.IssueTracker.EnableIntegration.ValueOrDefault)
                return null;
            var IssueTrackerType = Module.EffectiveSettings.IssueTracker.Type.Value;
            if (string.IsNullOrEmpty(IssueTrackerType))
                return null;
            var exports = ManagedExtensibility.GetExports<IIssueTrackerAdapter, IIssueTrackerTypeMetadata>();
            var export = exports.SingleOrDefault(x => x.Metadata.IssueTrackerType == IssueTrackerType);

            if (export != null)
            {
                try
                {
                    var canBeLoaded = export.Metadata.CanBeLoaded;
                    if (!canBeLoaded.IsNullOrEmpty())
                    {
                        System.Diagnostics.Debug.Write(export.Metadata.IssueTrackerType + " adapter could not be loaded: " + canBeLoaded);
                        return null;
                    }
                    var IssueTrackerAdapter = export.Value;
                    IssueTrackerAdapter.Initialize(this, Module.EffectiveSettings.IssueTracker.TypeSettings);
                    return IssueTrackerAdapter;
                }
                catch (InvalidOperationException ex)
                {
                    Debug.Write(ex);
                    // Invalid arguments, do not return a Issue server adapter
                }
            }

            return null;
        }

        private void UpdateUI()
        {
           
            var columnsAreVisible = IssueTrackerAdapter != null;

            if (columnsAreVisible)
            {
                AddIssueStatusColumns();
            }

            if (IssueStatusImageColumnIndex != -1)
                revisions.Columns[IssueStatusImageColumnIndex].Visible = columnsAreVisible;

            if (IssueStatusMessageColumnIndex != -1)
                revisions.Columns[IssueStatusMessageColumnIndex].Visible = columnsAreVisible;
                    /* Todo: Put this back in? ... && Module.EffectiveSettings.IssueTracker.ShowIssueSummaryInGrid.ValueOrDefault; */
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                CancelIssueTrackerFetchOperation();

                DisposeIssueTrackerAdapter();
            }
        }

        private void DisposeIssueTrackerAdapter()
        {
            if (IssueTrackerAdapter != null)
            {
                IssueTrackerAdapter.Dispose();
                IssueTrackerAdapter = null;
            }
        }

        private static IsolatedStorageFileStream GetIssueTrackerOptionsIsolatedStorageStream(IIssueTrackerAdapter IssueTrackerAdapter, FileAccess fileAccess, FileShare fileShare)
        {
            var fileName = string.Format("IssueTracker-{0}.options", Convert.ToBase64String(Encoding.UTF8.GetBytes(IssueTrackerAdapter.UniqueKey)));
            return new IsolatedStorageFileStream(fileName, FileMode.OpenOrCreate, fileAccess, fileShare);
        }
    }
}