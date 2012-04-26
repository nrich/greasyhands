using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using GreasyHands.Search;

namespace GreasyHands.DAL.Container
{
    public enum IssueStatus
    {
        New,
        Wanted,
        Grabbed,
        Owned,
        Read,
        Skipped,
        Failed
    }

    public class Issue : IComparable<Issue>
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        public virtual int Id { get; private set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        public virtual int Num { get; set; }
        public virtual string Code { get; set; }
        public virtual string Variant { get; set; }
        public virtual IssueStatus Status { get; set; }

        public virtual Title Title { get; set; }
        public virtual Release Release { get; set; }

        public virtual IList<IssueHistory> History { get; private set; }

        public Issue()
        {
            History = new List<IssueHistory>();
        }

        public virtual int CompareTo(Issue other)
        {
            if (other.Id == Id)
            {
                return Num.CompareTo(other.Num);
            }

            return String.CompareOrdinal(Title.Name, other.Title.Name);
        }

        public override string ToString()
        {
            return String.Format("<Issue: {0} {1} {2} '{3}' {4} {5}>", Id, Num, Code, Variant ?? "", Title, Release);
        }

        public virtual Query GenerateQuery()
        {
            return new Query
                       {
                           IssueID = Id,
                           Limited = Title.Limited,
                           Num = Num,
                           Title = Title.Name,
                       };
        }
    }

    public sealed class IssueMap : ClassMap<Issue>
    {
        public IssueMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Num).Not.Nullable();
            Map(x => x.Variant);
            Map(x => x.Code).Not.Nullable().Unique().Length(9);
            Map(x => x.Status).Not.Nullable().Default("New");

            References(x => x.Title).Not.Nullable();
            References(x => x.Release).Not.Nullable();

            HasMany(x => x.History).Cascade.All();
        }
    }
}
