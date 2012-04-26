using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Script.Serialization;
using GreasyHands;
using GreasyHands.DAL.Container;
using GreasyHands.DAL.Session;
using Nancy;

namespace GreasyHandsWebApp.Routes
{
    public class Main : NancyModule
    {
        public Main(ISession session)
        {
            string dbFile = ConfigurationManager.AppSettings["DBFile"];

            Get["/"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        var userSettings = s.QueryOver<UserSettings>().SingleOrDefault<UserSettings>();

                        if (userSettings == null || !userSettings.IsConfigured())
                        {
                            return Response.AsRedirect("/settings/wizard");
                        }


                        var titles =
                            s.QueryOver<Title>().Where(t => t.Subscribed).OrderBy(t => t.Name).Asc.List<Title>();
                        IList list = new ArrayList();

                        foreach (var title in titles)
                        {
                            list.Add(new { Latest = title.LatestIssue().Release.Date.NiceDate(), title.Limited, title.Subscribed, title.Name, title.Id, Publisher = new {title.Publisher.Name, title.Publisher.Id }, Have = title.Have(), Total = title.Total()});
                        }

                        var model = new { titles = list };
                        return View["Index", model];
                       
                    }
                }
            };

            Get["/add"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        var userSettings = s.QueryOver<UserSettings>().SingleOrDefault<UserSettings>();

                        if (userSettings == null || !userSettings.IsConfigured())
                        {
                            return Response.AsRedirect("/settings/wizard");
                        }


                        var titles =
                            s.QueryOver<Title>().Where(t => t.Subscribed == false).OrderBy(t => t.Name).Asc.List<Title>();
                        var list = new List<string>();

                        foreach (var title in titles)
                        {
                            list.Add(title.Name);
                        }

                        var serialiser = new JavaScriptSerializer();
                        var model = new { titles = serialiser.Serialize(list) };
                        return View["Add", model];

                    }
                }
            };

            Post["/add"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        using (var transaction = s.BeginTransaction())
                        {
                            string titleName = Request.Form.TitleName;
                            string matchTitleStr = Request.Form.MatchTitle;
                            string issueStatusStr = Request.Form.IssueStatus;

                            MatchTitle matchTitle;
                            IssueStatus issueStatus;

                            if (!Enum.TryParse(matchTitleStr, false, out matchTitle))
                            {
                                throw new ArgumentException("Unknown match title {0}", matchTitleStr);
                            }

                            if (!Enum.TryParse(issueStatusStr, false, out issueStatus))
                            {
                                throw new ArgumentException("Unknown match title {0}", matchTitleStr);
                            }

                            var title = s.QueryOver<Title>().Where(ti => ti.Name == titleName).SingleOrDefault<Title>();

                            if (title != null)
                            {
                                title.Subscribed = true;
                                title.MatchTitle = matchTitle;

                                foreach (var i in title.Issues)
                                {
                                    if (i.Status == IssueStatus.New)
                                    {
                                        i.Status = issueStatus;
                                        s.SaveOrUpdate(i);
                                    }
                                }

                                s.SaveOrUpdate(title);

                                transaction.Commit();
                            }

                            return Response.AsRedirect("/");
                        }
                    }
                }
            };

        }

    }
}
