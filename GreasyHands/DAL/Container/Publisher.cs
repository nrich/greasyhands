using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace GreasyHands.DAL.Container
{
    public class Publisher
    {
        private static readonly Dictionary<string, string> Publishers = new Dictionary<string, string>
            {
                {"MARVEL", "Marvel Comics"},
                {"MARVEL COMICS", "Marvel Comics"},
                {"DC COMICS", "DC Comics"},
                {"DARK HORSE", "Dark Horse Comics"},
                {"DARK HORSE COMICS", "Dark Horse Comics"},
                {"IMAGE", "Image Comics"},
                {"IMAGE COMICS", "Image Comics"},
                {"WIZARD", "Wizard Entertainment"},
                {"WIZARD ENTERTAINMENT", "Wizard Entertainment"},
                {"COMICS", "Other/Independent"},
                {"MAGAZINES", "Magazines"},
                {"MERCHANDISE", "Merchandise"},
                {"RELATED MERCHANDISE", "Merchandise"},
                {"PREVIEWS PUBLICATIONS","Preview Publications"},
                {"IDW PUBLISHING", "IDW Publishing"},
                {"IDW", "IDW Publishing"},
                {"COMICS & GRAPHIC NOVELS", "Other/Independent"},
                {"BOOKS", "Other/Independent"},
                {"IMAGE COMICS/TOP COW PRODUCTIONS", "Image Comics"},
                {"IMAGE COMICS/MCFARLANE TOYS", "Merchandise"},
                {"DC COMICS/DC DIRECT", "Merchandise"},
            };

        public static bool IsPublisher(string line)
        {
            return Publishers.ContainsKey(line);
        }

        public override string ToString()
        {
            return String.Format("<Publisher: {0} {1}>", Id, Name);
        }

        public static string PrettyName(string line)
        {
            if (!IsPublisher(line))
                throw new ArgumentException("Name '{0}' not recognised as a publisher", line);

            return Publishers[line];
        }

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        public virtual int Id { get; private set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        public virtual string Name { get; set; }
        public virtual IList<Title> Titles { get; private set; }

        public Publisher()
        {
            Titles = new List<Title>();
        }
    }

    public sealed class PublisherMap : ClassMap<Publisher>
    {
        public PublisherMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name).Unique().Length(32);
            HasMany(x => x.Titles).Inverse().Cascade.All();
        }
    }
}
