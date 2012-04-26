using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace GreasyHands.DAL.Container
{
    public class Release
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        public virtual int Id { get; private set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        public virtual DateTime Created { get; set; }
        public virtual DateTime Date { get; set; }

        public virtual IList<Issue> Issues { get; private set; }

        public Release()
        {
            Issues = new List<Issue>();
        }

        public override string ToString()
        {
            return String.Format("<Release: {0} {1}>", Id, Date);
        }
    }


    public sealed class ReleaseMap : ClassMap<Release>
    {
        ReleaseMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Created).Not.Nullable();
            Map(x => x.Date).Unique().Not.Nullable();

            HasMany(x => x.Issues).Inverse().Cascade.All();
        }
    }
}
