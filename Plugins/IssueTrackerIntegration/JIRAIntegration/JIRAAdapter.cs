using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitCommands.Settings;
using GitUIPluginInterfaces;
using GitUIPluginInterfaces.IssueTrackerIntegration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JIRAIntegration
{
    [Export(typeof(IIssueTrackerAdapter))]
    [JiraIntegrationMetadata("JIRA")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class JiraAdapter : IIssueTrackerAdapter
    {
        private const int REFRESH_INTERVAL_MINS = 10;

        private IIssueTrackerWatcher _issueTrackerWatcher;
        private HttpClient _httpClient;
        private IItemCache<long, Issue> _issueCache;


        private Regex IssueIdFilter { get; set; }

        public void Initialize(IIssueTrackerWatcher issueTrackerWatcher, ISettingsSource config)
        {
            if (this._issueTrackerWatcher != null)
                throw new InvalidOperationException("Already initialized");

            this._issueTrackerWatcher = issueTrackerWatcher;

            _issueCache = new SqlCeIssueTrackerCache();
            _issueCache.Initialize(REFRESH_INTERVAL_MINS);

            var hostName = config.GetString("ServerUrl", null);
            var user = config.GetString("Username", null);
            var password = config.GetString("Password", null);

            if (!string.IsNullOrEmpty(hostName))
            {
                _httpClient = new HttpClient
                    {
                        Timeout = TimeSpan.FromMinutes(2),
                        BaseAddress = hostName.Contains("://")
                                          ? new Uri(hostName, UriKind.Absolute)
                                          : new Uri(string.Format("{0}://{1}", Uri.UriSchemeHttp, hostName), UriKind.Absolute)
                    };
                
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                IIssueTrackerCredentials issueTrackerCredentials = new IssueTrackerCredentials { Username = user, Password = password };

                if (issueTrackerCredentials.Username == null || issueTrackerCredentials.Password == null)
                    issueTrackerCredentials = issueTrackerWatcher.GetIssueTrackerCredentials(this, true);

               
                UpdateHttpClientOptions(issueTrackerCredentials, CancellationToken.None);

            }
        }

        /// <summary>
        /// Gets a unique key which identifies this Issue server.
        /// </summary>
        public string UniqueKey
        {
            get { return _httpClient.BaseAddress.Host; }
        }

        public IObservable<Issue> GetFinishedIssuesSince(IScheduler scheduler, DateTime? sinceDate = null)
        {
            return GetIssues(scheduler, sinceDate, false);
        }

        public IObservable<Issue> GetActiveIssues(IScheduler scheduler)
        {
            return GetIssues(scheduler, null, true);
        }

        public IObservable<Issue> GetIssues(IScheduler scheduler, DateTime? sinceDate = null, bool? running = null)
        {
            if (_httpClient == null || _httpClient.BaseAddress == null)
            {
                return Observable.Empty<Issue>(scheduler);
            }

            return Observable.Create<Issue>((observer, cancellationToken) =>
                Task<IDisposable>.Factory.StartNew(
                    () => scheduler.Schedule(() => ObserveIssues(observer, cancellationToken))));
        }

        private void ObserveIssues(IObserver<Issue> observer, CancellationToken cancellationToken)
        {
            try
            {
                var localObserver = observer;

                var issueIdTasks = new List<Task<JObject>> { GetActiveIssuesJsonResponseAsync(cancellationToken) }.ToArray();
                    
                Task.Factory
                    .ContinueWhenAll<JObject>(
                        issueIdTasks,
                        completedTasks =>
                            {
                                var issueIds = completedTasks.Where(task => task.Status == TaskStatus.RanToCompletion)
                                                             .Select(t=> JsonConvert.DeserializeObject<IssueContainer>(t.Result.ToString()))
                                                             .SelectMany(f=>f.Issues)
                                                             .Select(i=>i.id)
                                                             .ToArray();

                                NotifyObserverOfIssues(issueIds, observer, cancellationToken);
                            },
                        cancellationToken,
                        TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously,
                        TaskScheduler.Current)
                    .ContinueWith(
                        task => localObserver.OnError(task.Exception),
                        CancellationToken.None,
                        TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted,
                        TaskScheduler.Current);
            }
            catch (OperationCanceledException)
            {
                // Do nothing, the observer is already stopped
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
            }
        }

        private void NotifyObserverOfIssues(long[] issueIds, IObserver<Issue> observer, CancellationToken cancellationToken)
        {
            var cacheTasks = new List<Task>(_issueCache.Items.Count);

            foreach (var issue in _issueCache.Items)
            {
                cacheTasks.Add(Task.Run(() =>
                {
                    observer.OnNext(issue.Value);
                }, cancellationToken));
            }

            
            Task.WaitAll(cacheTasks.ToArray(), cancellationToken);

            var tasks = new List<Task>(8);
            var issuesToUpdate = issueIds.Except(_issueCache.GetValidItems().Select(c => c.Key)).OrderByDescending(k => k);
            var issuesLeft = issuesToUpdate.Count();
            

            foreach (var issueId in issuesToUpdate)
            {
                var issueItem = new Issue();
                var getIssueDetailsTask =
                    GetIssueFromIdJsonResponseAsync(issueId.ToString(), cancellationToken)
                        .ContinueWith(
                            issueDetailsTask =>
                            {
                                if (issueDetailsTask.Status == TaskStatus.RanToCompletion)
                                {
                                    dynamic issueResult = ExpandoJson(issueDetailsTask.Result);
                                    issueItem = new Issue()
                                    {
                                        id = issueId,
                                        lastupdated = DateTime.Now,
                                        status = issueResult.fields.status.name.ToString(),
                                        key = issueResult.key.ToString()
                                    };

                                }
                            },
                            cancellationToken,
                            TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously,
                            TaskScheduler.Current
                            );

                    var getIssueRepoDetailsTask = GetDevStatusFromIdJsonResponseAsync(issueId.ToString(), cancellationToken)
                        .ContinueWith(
                        issueRepoDetailsTask =>
                        {
                            if (issueRepoDetailsTask.Status == TaskStatus.RanToCompletion)
                            {
                                issueItem.commithashes = new ConcurrentBag<string>();
                                var jsonCommitHashes = issueRepoDetailsTask.Result.SelectTokens("$.detail[0].repositories[0].commits[*].id",  false);
                                foreach (var hash in jsonCommitHashes)
                                {
                                    issueItem.commithashes.Add(hash.Value<string>());
                                }
                            }
                        },
                        cancellationToken,
                        TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously,
                        TaskScheduler.Current);


                    var notifyObserverTask = Task.WhenAll(getIssueDetailsTask, getIssueRepoDetailsTask).ContinueWith(apiRequests =>
                    {

                        if (apiRequests.Status == TaskStatus.RanToCompletion)
                        {
                            _issueCache.AddOrUpdateItem(issueItem.id, issueItem);
                            observer.OnNext(issueItem);
                        }
                    }, cancellationToken,
                        TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously,
                        TaskScheduler.Current);
                
                tasks.Add(notifyObserverTask);
                --issuesLeft;

                if (tasks.Count == tasks.Capacity || issuesLeft == 0)
                {
                    var batchTasks = tasks.ToArray();
                    tasks.Clear();

                    try
                    {
                        Task.WaitAll(batchTasks, cancellationToken);
                    }
                    catch (Exception e)
                    {
                        observer.OnError(e);
                        return;
                    }
                }
            }

            observer.OnCompleted();
        }

        private static bool PropagateTaskAnomalyToObserver(Task task, IObserver<IssueContainer> observer)
        {
            if (task.IsCanceled)
            {
                observer.OnCompleted();
                return true;
            }

            if (task.IsFaulted)
            {
                Debug.Assert(task.Exception != null);

                observer.OnError(task.Exception);
                return true;
            }

            return false;
        }

        private static ExpandoObject ExpandoJson(JObject json)
        {
            var converter = new ExpandoObjectConverter();
            return JsonConvert.DeserializeObject<ExpandoObject>(json.ToString(), converter);
        }

        private static AuthenticationHeaderValue CreateBasicHeader(string username, string password)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

       
        private Uri GetApiPath(JIRAApiType apiType)
        {
            switch (apiType)
            {
                case JIRAApiType.Repository:
                    return new Uri("/rest/dev-status/1.0/", UriKind.Relative);
                case JIRAApiType.Session:
                    return new Uri("/rest/auth/1/", UriKind.Relative);
                case JIRAApiType.Standard:
                default:
                    return  new Uri("/rest/api/2/", UriKind.Relative);
            }
        }
            
        private Task<Stream> GetStreamAsync(string restServicePath, JIRAApiType apiType, CancellationToken cancellationToken)
        {
            var path = GetApiPath(apiType) + restServicePath;

            cancellationToken.ThrowIfCancellationRequested();

            return _httpClient.GetAsync(path, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                             .ContinueWith(
                                 task => GetStreamFromHttpResponseAsync(task, path, apiType, cancellationToken),
                                 cancellationToken,
                                 TaskContinuationOptions.AttachedToParent,
                                 TaskScheduler.Current)
                             .Unwrap();
        }

        private Task<Stream> PostStreamAsync (string restServicePath, JIRAApiType apiType, HttpContent content, CancellationToken cancellationToken)
        {
            string path = restServicePath;

            if (!restServicePath.StartsWith(GetApiPath(apiType).ToString()))
                path = GetApiPath(apiType) + restServicePath;
            cancellationToken.ThrowIfCancellationRequested();

            return _httpClient.PostAsync(path, content, cancellationToken)
                             .ContinueWith(
                                 task => GetStreamFromHttpResponseAsync(task, path, apiType, cancellationToken),
                                 cancellationToken,
                                 TaskContinuationOptions.AttachedToParent,
                                 TaskScheduler.Current)
                             .Unwrap();
        }


        private Task<Stream> GetStreamFromHttpResponseAsync(Task<HttpResponseMessage> task, string restServicePath, JIRAApiType apiType, CancellationToken cancellationToken)
        {
#if !__MonoCS__
            bool retry = task.IsCanceled && !cancellationToken.IsCancellationRequested;
            bool unauthorized = task.Status == TaskStatus.RanToCompletion &&
                                (task.Result.StatusCode == HttpStatusCode.Unauthorized || task.Result.StatusCode == HttpStatusCode.BadRequest);

            if (!retry)
            {
                if (task.Result.IsSuccessStatusCode)
                {
                    var httpContent = task.Result.Content;

                    if (httpContent.Headers.ContentType.MediaType == "text/html")
                    {
                        // JIRA responds with an HTML login page when guest access is denied.
                        unauthorized = true;
                    }
                    else
                    {
                        return httpContent.ReadAsStreamAsync();
                    }
                }
            }

            if (retry)
            {
                return GetStreamAsync(restServicePath, apiType, cancellationToken);
            }

            if (unauthorized)
            {
                
                var issueTrackerCredentials = _issueTrackerWatcher.GetIssueTrackerCredentials(this, true);
                
                if (issueTrackerCredentials != null)
                {
                    UpdateHttpClientOptions(issueTrackerCredentials, cancellationToken);
                    return GetStreamAsync(restServicePath, apiType, cancellationToken);
                }

                throw new OperationCanceledException(task.Result.ReasonPhrase);
            }

            throw new HttpRequestException(task.Result.ReasonPhrase);
#else
            return null;
#endif
        }

        private void UpdateHttpClientOptions(IIssueTrackerCredentials issueTrackerCredentials, CancellationToken cancellationToken)
        {
            if (_httpClient.DefaultRequestHeaders.Contains("cookie"))
                _httpClient.DefaultRequestHeaders.Remove("cookie");
            
            var sessionRequest = new StringContent(JsonConvert.SerializeObject(issueTrackerCredentials, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }), Encoding.UTF8, "application/json");
            
            var getSessionTask = PostJsonResponseAsync("session", JIRAApiType.Session, sessionRequest, cancellationToken);
            getSessionTask.ContinueWith(task =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    var sessionId = task.Result.SelectToken("$.session.value").ToString(Formatting.None).Replace("\"", "");
                    _httpClient.DefaultRequestHeaders.Add("cookie", $"JSESSIONID={sessionId}");
                }
            }, cancellationToken);
            
            
        }


        private Task<JObject> PostJsonResponseAsync(string actionPath, JIRAApiType apiType, HttpContent content, CancellationToken cancellationToken)
        {
            var postStreamTask = PostStreamAsync(actionPath, apiType, content, cancellationToken);

            return postStreamTask.ContinueWith(
                task =>
                {
                    using (var responseStream = task.Result)
                    {
                        var jsonStreamReader = new StreamReader(responseStream);
                        var jsonTextReader = (TextReader)jsonStreamReader;
                        var jDoc = new JsonTextReader(jsonTextReader);

                        var jObj = JObject.Load(jDoc);

                        return jObj;
                    }
                },
                cancellationToken,
                TaskContinuationOptions.AttachedToParent,
                TaskScheduler.Current);
        }

        private Task<JObject> GetJsonResponseAsync(string actionPath, JIRAApiType apiType, CancellationToken cancellationToken)
        {
            var getStreamTask = GetStreamAsync(actionPath, apiType,  cancellationToken);

            return getStreamTask.ContinueWith(
                task =>
                    {
                        using (var responseStream = task.Result)
                        {
                            var jsonStreamReader = new StreamReader(responseStream);
                            var jsonTextReader = (TextReader)jsonStreamReader;
                            var jDoc = new JsonTextReader(jsonTextReader);

                            var jObj = JObject.Load(jDoc);

                            return jObj;
                        }
                    },
                cancellationToken, 
                TaskContinuationOptions.AttachedToParent, 
                TaskScheduler.Current);
        }
        
        private Task<JObject> GetIssueFromIdJsonResponseAsync(string issueId, CancellationToken cancellationToken)
        {
            return GetJsonResponseAsync($"issue/{issueId}", JIRAApiType.Standard, cancellationToken);
        }
        private Task<JObject> GetDevStatusFromIdJsonResponseAsync(string issueId, CancellationToken cancellationToken)
        {
            return GetJsonResponseAsync($"issue/detail?issueId={issueId}&applicationType=github&dataType=repository", JIRAApiType.Repository, cancellationToken);
        }

        private Task<JObject> GetActiveIssuesJsonResponseAsync(CancellationToken cancellationToken)
        {
           	var url = "search?jql=project%3DBAC%20ORDER%20BY%20updated%20DESC&expand=issues&fields=id&maxResults=1000"; /*%26(status%3D%22In%20Progress%22%7Cstatus%3D%22To%20do%22%7Cstatus%3D%22Ready%20For%20QA%22)*/
            var filteredIssuesJsonResponseTask = GetJsonResponseAsync(url, JIRAApiType.Standard, cancellationToken);
            return filteredIssuesJsonResponseTask;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (_httpClient != null)
            {
                _httpClient.Dispose();
            }
        }
    }
}