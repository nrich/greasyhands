using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using System.Linq;


namespace GreasyHands.DAL.Container
{
    public enum MatchTitle
    {
        Exact,
        Partial
    }

    public class Title
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        public virtual int Id { get; private set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        public virtual string Name { get; set; }
        public virtual int Limited { get; set; }

        public virtual bool Subscribed { get; set; }

        public virtual bool GraphicNovel { get; set; }
        public virtual bool HardCover { get; set; }
        public virtual bool OneShot { get; set; }
        public virtual bool Mature { get; set; }
        public virtual bool Trade { get; set; }
        public virtual bool Hidden { get; set; }

        public virtual DateTime LastSearch { get; set; }
        public virtual MatchTitle MatchTitle { get; set; }

        public virtual string SearchTitle { get; set; }

        public virtual Publisher Publisher { get; set; }
        public virtual IList<Issue> Issues { get; private set; }

        public override string ToString()
        {
            return String.Format("<Title: {0} {1} {2}>", Id, Name, Publisher);
        }

        public Title()
        {
            Issues = new List<Issue>();
        }

        public virtual Issue LatestIssue()
        {
            return Issues.OrderBy(i => i.Release.Date).Reverse().First();
        }

        public virtual int Have()
        {
            return
                (from issues in Issues
                 where issues.Status != IssueStatus.Wanted && issues.Status != IssueStatus.New
                 select issues).Count();
        }

        public virtual int Total()
        {
            return
                (from issues in Issues
                 select issues).Count();
        }
    }

    public sealed class TitleMap : ClassMap<Title>
    {
        public TitleMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name).UniqueKey("TitlePublisherLimited").Length(254);
            Map(x => x.Limited).UniqueKey("TitlePublisherLimited");

            Map(x => x.Subscribed).Default("false");

            Map(x => x.GraphicNovel).Default("false");
            Map(x => x.HardCover).Default("false");
            Map(x => x.OneShot).Default("false");
            Map(x => x.Mature).Default("false");
            Map(x => x.Trade).Default("false");
            Map(x => x.Hidden).Default("false");

            Map(x => x.SearchTitle).Nullable();

            Map(x => x.LastSearch).Not.Nullable();
            Map(x => x.MatchTitle).Not.Nullable().Default(MatchTitle.Exact.ToString());

            References(x => x.Publisher).UniqueKey("TitlePublisherLimited").Not.Nullable();
            HasMany(x => x.Issues).Inverse().Cascade.All();
        }
    }
}
