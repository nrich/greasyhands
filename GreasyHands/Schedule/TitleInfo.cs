using System;
using GreasyHands.DAL.Container;
using GreasyHands.DAL.Session;

namespace GreasyHands.Schedule
{
    public class TitleInfo
    {
        public string Code { get; set; }
        public string Publisher { get; set; }
        public bool GraphicNovel { get; set; }
        public bool HardCover { get; set; }
        public int Limited { get; set; }
        public bool Mature { get; set; }
        public string Name { get; set; }
        public int Num { get; set; }
        public bool OneShot { get; set; }
        public bool Trade { get; set; }
        public string Variant { get; set; }
        public DateTime Released { get; set; }

        public override string ToString()
        {
            return String.Format("{0} #{1}", Name, Num);
        }

        public bool Insert(ISession session)
        {
            using (var db = session.SessionFactory("test"))
            {
                using (var s = db.OpenSession())
                {
                    using (var transaction = s.BeginTransaction())
                    {
                        var publisher = s.QueryOver<Publisher>().Where(p => p.Name == Publisher).SingleOrDefault<Publisher>();
                        if (publisher == null)
                        {
                            publisher = new Publisher {Name = Publisher};
                            s.SaveOrUpdate(publisher);
                        }

                        var release = s.QueryOver<Release>().Where(r => r.Date == Released).SingleOrDefault<Release>();
                        if (release == null)
                        {
                            release = new Release {Created = DateTime.Now, Date = Released};
                            s.SaveOrUpdate(release);
                        }

                        var title =
                            s.QueryOver<Title>().Where(
                                t => t.Name == Name && t.Publisher == publisher && t.Limited == Limited).SingleOrDefault();

                        if (title == null)
                        {
                            title = new Title
                                        {
                                            Name = Name,
                                            SearchTitle = Name,
                                            GraphicNovel = GraphicNovel,
                                            HardCover = HardCover,
                                            Hidden = false,
                                            Limited = Limited,
                                            Mature = Mature,
                                            OneShot = OneShot,
                                            Publisher = publisher
                                        };
                            s.SaveOrUpdate(title);
                        }

                        var issue = s.QueryOver<Issue>().Where(i => i.Code == Code).SingleOrDefault<Issue>();

                        if (issue == null)
                        {
                            var status = IssueStatus.New;

                            if (title.Subscribed)
                            {
                                status = IssueStatus.Wanted;
                            }

                            issue = new Issue
                                            {
                                                Code = Code,
                                                Num = Num,
                                                Title = title,
                                                Release = release,
                                                Status = status,
                                                Variant = Variant
                                            };

                            s.SaveOrUpdate(issue);
                        }

                        transaction.Commit();

                    }
                }
            }

            return true;
        }
    }
}
