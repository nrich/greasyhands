﻿using System;
using System.Threading;
using GreasyHands.DAL.Container;
using GreasyHands.DAL.Session;
using GreasyHands.Download;
using GreasyHands.Search.Matcher;
using GreasyHands.Search.Provider;

namespace GreasyHands.Jobs
{
    public class WantedSearch
    {
        private readonly ISession session;
        private readonly ISearchMatcher searchMatcher;

        public WantedSearch(ISession session, ISearchMatcher searchMatcher)
        {
            this.session = session;

            this.searchMatcher = searchMatcher;
        }

        // ReSharper disable FunctionNeverReturns
        public void Run()
        {
            Thread.CurrentThread.Name = "Wanted Search";

            while (true)
            {
                using (var db = session.SessionFactory(""))
                {
                    using (var s = db.OpenSession())
                    {
                        using (var transaction = s.BeginTransaction())
                        {
                            var userSettings = s.QueryOver<UserSettings>().SingleOrDefault<UserSettings>();

                            ISearchProvider searchProvider;
                            IDownloadProvider downloadProvider;



                            if (userSettings != null && userSettings.IsConfigured())
                            {
                                searchProvider = userSettings.GetBestSearchProvider();
                                downloadProvider = userSettings.GetBestDownloadProvider();
                            }
                            else
                            {
                                Console.WriteLine("Settings not configured, doing nothing");
                                continue;
                            }

                            foreach (var title in s.QueryOver<Title>().Where(t => t.Subscribed).List<Title>())
                            {

                                if (title.LastSearch.AddHours(userSettings.SearchInterval) > DateTime.Now)
                                    continue;

                                Console.WriteLine(title);
                                var localTitle = title;

                                var issues =
                                    s.QueryOver<Issue>().Where(i => i.Status == IssueStatus.Wanted && i.Title == localTitle).List<Issue>();

                                foreach (var issue in issues)
                                {
                                    Console.WriteLine(issue);

                                    var query = issue.GenerateQuery();
                                    var results = searchProvider.Search(query);

                                    foreach (var result in results)
                                    {
                                        if (searchMatcher.MatchFilename(query, result.Title, userSettings.C2CPreference, issue.Title.MatchTitle))
                                        {
                                            issue.Status = downloadProvider.QueueDownload(result) ? IssueStatus.Grabbed : IssueStatus.Failed;
                                            s.SaveOrUpdate(issue);

                                            break;
                                        }
                                    }
                                }

                                title.LastSearch = DateTime.Now;
                                s.SaveOrUpdate(title);
                            }

                            transaction.Commit();
                        }
                    }
                }

                Thread.Sleep(60000);
            }

        }
        // ReSharper restore FunctionNeverReturns
    }
}
