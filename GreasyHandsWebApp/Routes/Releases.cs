using System;
using System.Collections;
using System.Configuration;
using System.Linq;
using GreasyHands;
using GreasyHands.DAL.Container;
using GreasyHands.DAL.Session;
using GreasyHands.Jobs;
using Nancy;

namespace GreasyHandsWebApp.Routes
{
    public class Releases : NancyModule
    {
        public Releases(ISession session) : base("/releases")
        {
            string dbFile = ConfigurationManager.AppSettings["DBFile"];

            Get["/"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        var issues = s.QueryOver<Issue>().Where(i => i.Status == IssueStatus.Wanted).OrderBy(i => i.Release).Asc.List<Issue>();
                        IList list = new ArrayList();

                        foreach (var issue in issues)
                        {
                            list.Add(new {issue.Id, issue.Num, Released = issue.Release.Date.NiceDate(), Title = new {issue.Title.Id, issue.Title.Name}, Publisher = new {issue.Title.Publisher.Name, issue.Title.Publisher.Id}});
                        }

                        return View["Releases/Index", new {Pending = list}];
                    }
                }
            };

            Get["/history"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        var releases = s.QueryOver<Release>().OrderBy(r => r.Date).Desc.List<Release>();
                        IList list = new ArrayList();

                        foreach (var release in releases)
                        {
                            list.Add(new { release.Id, Grabbed = release.Created.NiceDate(), Released = release.Date.NiceDate(), Issues = release.Issues.Count });
                        }

                        return View["Releases/History", new { Releases = list }];
                    }
                }
            };

            Get["/upload"] = parameters =>
            {
                return View["Releases/Upload"];
            };

            Post["/upload"] = parameters =>
            {
                string weeks = Request.Form.Weeks.Value;
                string url = Request.Form.Url.Value;

                if (weeks.Length > 0)
                {
                    ScheduleUpdater.AddReleaseURL(int.Parse(weeks));
                }

                if (url.Length > 0)
                {
                    ScheduleUpdater.AddReleaseURL(new Uri(url));
                }

                return View["Releases/Upload"];
            };

            Get["/single/{id}"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        int id = int.Parse(parameters.id);
                        //var issues = s.QueryOver<Issue>().Where(i => i.Release.Id == id).OrderBy(i => i.Title.Name).Asc.List<Issue>();
                        var release = s.QueryOver<Release>().Where(i => i.Id == id).SingleOrDefault<Release>();
                        IList list = new ArrayList();

                        var issues =
                            release.Issues.OrderBy(i => i.Title.Name).ThenBy(i => i.Num).ThenBy(i => i.Variant.Length);

                        foreach (var issue in issues)
                        {
                            list.Add(new { issue.Id, issue.Num, issue.Status, issue.Variant, issue.Code, Title = new { issue.Title.Id, issue.Title.Name, Publisher = new { issue.Title.Publisher.Name, issue.Title.Publisher.Id } } });
                        }

                        var model = new { Issues = list, Release = release };

                        return View["Releases/Single", model];
                        
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
                            string statusStr = Request.Form.Status;
                            string titleid = Request.Form.titleid;
                            IssueStatus status;

                            if (!Enum.TryParse(statusStr, false, out status))
                            {
                                throw new ArgumentException("Unknown issue status {0}", statusStr);
                            }

                            string issues = Request.Form.issue;

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

                            transaction.Commit();

                            return Response.AsRedirect(string.Format("/titles/single/{0}", titleid)); 
                        }
                    }
                }
            };
        }

    }
}
