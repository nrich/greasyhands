using System.Collections;
using System.Configuration;
using System.Linq;
using GreasyHands;
using GreasyHands.DAL.Container;
using GreasyHands.DAL.Session;
using Nancy;

namespace GreasyHandsWebApp.Routes
{
    public class Publishers : NancyModule
    {
        string dbFile = ConfigurationManager.AppSettings["DBFile"];

        public Publishers(ISession session)
            : base("/publishers")
        {
            Get["/"] = parameters =>
                           {
                               using (var db = session.SessionFactory(dbFile))
                               {
                                   using (var s = db.OpenSession())
                                   {
                                       var publishers =
                                           s.QueryOver<Publisher>().OrderBy(p => p.Name).Asc.List<Publisher>();
                                       IList list = new ArrayList();

                                       foreach (var publisher in publishers)
                                       {
                                           list.Add(new {publisher.Id, publisher.Name, Titles = publisher.Titles.Count});
                                       }

                                       return View["Publishers/Index", new {Publishers = list}];
                                   }
                               }
                           };

            Get["/titles"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        using (s.BeginTransaction())
                        {
                            var titles = s.QueryOver<Title>().Where(t => t.Subscribed == false).OrderBy(t => t.Publisher).Asc.ThenBy(t => t.Name).Asc.List<Title>();
                            IList list = new ArrayList();

                            foreach (var title in titles)
                            {
                                list.Add(new { LatestNum = title.LatestIssue().Num, Latest = title.LatestIssue().Release.Date.NiceDate(), title.Limited, title.Subscribed, title.Name, title.Id, Publisher = new { title.Publisher.Name, title.Publisher.Id }, });
                            }

                            var model = new { titles = list };
                            return View["Publishers/Titles", model];
                        }
                    }
                }
            };

            Post["/titles"] = parameters =>
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

            Get["/single/{id}"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        int id = int.Parse(parameters.id);
                        var publisher =
                            s.QueryOver<Publisher>().Where(p => p.Id == id).SingleOrDefault<Publisher>();
                        IList list = new ArrayList();

                        foreach (var title in publisher.Titles.OrderBy(t => t.Name))
                        {
                            list.Add(
                                new
                                    {
                                        title.Id,
                                        title.Name,
                                        title.Limited,
                                        Have = title.Have(),
                                        Total = title.Total(),
                                        title.Mature,
                                        title.OneShot,
                                        title.GraphicNovel,
                                        title.HardCover,
                                        Latest = title.LatestIssue().Release.Date.NiceDate(),
                                    });
                        }

                        var model = new
                                        {
                                            Titles = list,
                                            Publisher = new
                                            {
                                                publisher.Id,
                                                publisher.Name,
                                            }
                                        };

                        return View["Publishers/Single", model];

                    }
                }
            };
        }
    }
}
