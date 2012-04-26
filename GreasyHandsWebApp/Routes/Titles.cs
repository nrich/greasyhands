using System;
using System.Collections;
using System.Configuration;
using GreasyHands;
using GreasyHands.DAL.Container;
using GreasyHands.DAL.Session;
using NHibernate.Criterion;
using Nancy;
using System.Linq;

namespace GreasyHandsWebApp.Routes
{
    public class Titles : NancyModule
    {
        public Titles(ISession session) : base("/titles")
        {
            string dbFile = ConfigurationManager.AppSettings["DBFile"];

            Get["/"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        using (s.BeginTransaction())
                        {
                            var titles = s.QueryOver<Title>().Where(t => t.Subscribed == false).OrderBy(t => t.Name).Asc.List<Title>();
                            IList list = new ArrayList();

                            foreach (var title in titles)
                            {
                                list.Add(new { LatestNum = title.LatestIssue().Num, Latest = title.LatestIssue().Release.Date.NiceDate(), title.Limited, title.Subscribed, title.Name, title.Id, Publisher = new { title.Publisher.Name, title.Publisher.Id }, });
                            }

                            var model = new { titles = list };
                            return View["Titles/Index", model];
                        }
                    }
                }
            };

            Post["/"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        using (var transaction = s.BeginTransaction())
                        {
                            string titles = Request.Form.title;

                            var titlelist = titles.Split(',');

                            foreach (var t in titlelist)
                            {
                                int titleid = int.Parse(t);

                                var title = s.QueryOver<Title>().Where(ti => ti.Id == titleid).SingleOrDefault<Title>();

                                if (title != null)
                                {
                                    title.Subscribed = true;

                                    foreach (var i in title.Issues)
                                    {
                                        if (i.Status == IssueStatus.New)
                                        {
                                            i.Status = IssueStatus.Wanted;
                                            s.SaveOrUpdate(i);
                                        }
                                    }
                                }

                                s.SaveOrUpdate(title);
                            }

                            transaction.Commit();
                            return Response.AsRedirect("/");
                        }
                    }
                }
            };

            Get["/search"] = parameters =>
            {
                return View["Titles/Search", new { Count = 0, Results = new ArrayList(), SearchTerm = "" }];
            };

            Post["/search"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {

                        string search = Request.Form.search;

                        var titles =
                            s.QueryOver<Title>().Where(t => t.Name.IsInsensitiveLike(search, MatchMode.Anywhere)).OrderBy(t => t.Name).Asc.List<Title>();
                        IList results = new ArrayList();

                        foreach (var title in titles)
                        {
                            results.Add(new { title.Limited, title.Subscribed, title.Name, title.Id, Publisher = new {title.Publisher.Name, title.Publisher.Id} });
                        }

                        return View["Titles/Search", new { Results = results, SearchTerm = search }];
                        
                    }
                }
            };

            Get["/single/{id}"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        int id = int.Parse(parameters.id);
                        var title = s.QueryOver<Title>().Where(t => t.Id == id).SingleOrDefault<Title>();
                        IList list = new ArrayList();

                        foreach (var issue in title.Issues.OrderBy(i => i.Num).Reverse())
                        {
                            list.Add(new { issue.Id, issue.Num, issue.Status, issue.Variant, issue.Code, Released = issue.Release.Date.NiceDate() });
                        }

                        var model = new
                                        {
                                            issues = list, 
                                            Title = new
                                                        {
                                                            title.Limited, 
                                                            title.Id, 
                                                            title.Name, 
                                                            Have = title.Have(), 
                                                            Total = title.Total(), 
                                                            title.Mature, 
                                                            title.OneShot,
                                                            title.GraphicNovel,
                                                            title.HardCover,
                                                            Latest = title.LatestIssue().Release.Date.NiceDate(), 
                                                            title.Subscribed,
                                                            MatchTitle = title.MatchTitle.ToString(),
                                                            Publisher = new
                                                                            {
                                                                                title.Publisher.Name, 
                                                                                title.Publisher.Id
                                                                            }
                                                        }
                                        };

                        return View["Titles/Single", model];
                        
                    }
                }
            };

            Post["/single/{id}"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        using (var transaction = s.BeginTransaction())
                        {
                            int titleid = int.Parse(Request.Form.titleid);

                            if (Request.Form.issue.HasValue && Request.Form.Status.HasValue)
                            {
                                string statusStr = Request.Form.Status;
                                string issues = Request.Form.issue;

                                IssueStatus status;

                                if (!Enum.TryParse(statusStr, false, out status))
                                {
                                    throw new ArgumentException("Unknown issue status {0}", statusStr);
                                }

                                var issuelist = issues.Split(',');

                                foreach (var idstr in issuelist)
                                {
                                    var id = int.Parse(idstr);

                                    var issue = s.QueryOver<Issue>().Where(i => i.Id == id).SingleOrDefault<Issue>();

                                    if (issue.Status != status)
                                    {
                                        issue.Status = status;
                                        s.SaveOrUpdate(issue);
                                    }
                                }
                            }

                            if (Request.Form.MatchTitle.HasValue)
                            {
                                string matchTitleStr = Request.Form.MatchTitle;
                                bool subscribed = Request.Form.Subscribed.HasValue;

                                MatchTitle matchTitle;

                                if (!Enum.TryParse(matchTitleStr, false, out matchTitle))
                                {
                                    throw new ArgumentException("Unknown title match {0}", matchTitleStr);
                                }

                                var title = s.QueryOver<Title>().Where(i => i.Id == titleid).SingleOrDefault<Title>();
                                title.Subscribed = subscribed;
                                title.MatchTitle = matchTitle;

                                s.SaveOrUpdate(title);
                            }

                            transaction.Commit();

                            return Response.AsRedirect(string.Format("/titles/single/{0}", titleid)); 
                        }
                    }
                }
            };
        }

    }
}
