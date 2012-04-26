using System;
using FluentNHibernate.Mapping;

namespace GreasyHands.DAL.Container
{
    public class IssueHistory
    {
// ReSharper disable UnusedAutoPropertyAccessor.Local
        public virtual int Id { get; private set; }
        public virtual Issue Issue { get; private set; }
// ReSharper restore UnusedAutoPropertyAccessor.Local

        public virtual DateTime Created { get; set; }
        public virtual IssueStatus StateChange { get; set; }
        

        public override string ToString()
        {
            return String.Format("<IssueHistory: {0} {1} {2}>", Id, Issue, StateChange);
        }
    }


    public sealed class IssueHistoryMap : ClassMap<IssueHistory>
    {
        IssueHistoryMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Created).Not.Nullable();
            Map(x => x.StateChange).Not.Nullable();

            References(x => x.Issue).Not.Nullable();
        }
    }



    public class ReleaseHistory
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        public virtual int Id { get; private set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        public virtual DateTime Created { get; set; }
        public virtual Release Release { get; set; }


        public override string ToString()
        {
            return String.Format("<ReleaseHistory: {0} {1} {2}>", Id, Release, Created);
        }
    }


    public sealed class ReleaseHistoryMap : ClassMap<ReleaseHistory>
    {
        ReleaseHistoryMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Created).Not.Nullable();

            References(x => x.Release).Not.Nullable();
        }
    }
}
