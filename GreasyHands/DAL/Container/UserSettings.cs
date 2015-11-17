using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Mapping;
using GreasyHands.Download;
using GreasyHands.Search.Provider;

namespace GreasyHands.DAL.Container
{
    // ReSharper disable InconsistentNaming
    public enum SearchProviders
    {
        NZBdotSU,
        NZBINDEXdotNL,
        NewzNab,
        NUIP
    }
    // ReSharper restore InconsistentNaming

    public enum DownloadProviders
    {
        SABnzbd,
        SaveFile
    }

    public enum C2CPreference
    {
        Ignore,
        None,
        Only
    }

    public class UserSettings
    {
        public virtual int Id { get; private set; }
        public virtual bool Enabled { get; set; }
        public virtual string ApiKey { get; set; }
        public virtual C2CPreference C2CPreference { get; set; }

        public virtual int SearchInterval { get; set; }

        public virtual IList<DownloadProviderSettings> DownloadProviderSettings { get; private set; }
        public virtual IList<SearchProviderSettings> SearchProviderSettings { get; private set; }

        public UserSettings()
        {
            Id = 1;
            DownloadProviderSettings = new List<DownloadProviderSettings>();
            SearchProviderSettings = new List<SearchProviderSettings>();
        }

        public virtual bool IsConfigured()
        {
            if (!Enabled)
                return false;

            bool search = SearchProviderSettings.Any(searchProviderSettings => searchProviderSettings.Enabled);

            bool download = DownloadProviderSettings.Any(downloadProviderSetting => downloadProviderSetting.Enabled);

            return search && download;
        }
        
        protected internal virtual IList<ISearchProvider> GetSearchProviders()
        {
            var searchProviders = from searchProviderSettings in SearchProviderSettings
                                  where searchProviderSettings.Enabled
                                  select searchProviderSettings;

            ISearchProvider nzbdotsu = null;
            ISearchProvider nzbindexdotnl = null;
            ISearchProvider newznab = null;
            ISearchProvider nuip = null;

            var list = new List<ISearchProvider>();


            foreach (var searchProvider in searchProviders)
            {
                switch (searchProvider.Type)
                {
                    case SearchProviders.NewzNab:
                        newznab = searchProvider.GetProvider();
                        break;
                    case SearchProviders.NUIP:
                        nuip = searchProvider.GetProvider();
                        break;
                    case SearchProviders.NZBdotSU:
                        nzbdotsu = searchProvider.GetProvider();
                        break;
                    case SearchProviders.NZBINDEXdotNL:
                        nzbindexdotnl = searchProvider.GetProvider();
                        break;
                }
            }

            if (newznab != null)
                list.Add(newznab);

            if (nuip != null)
                list.Add(nuip);

            if (nzbdotsu != null)
                list.Add(nzbdotsu);

            if (nzbindexdotnl != null)
                list.Add(nzbindexdotnl);

            return list;
        }

        protected internal virtual IDownloadProvider GetBestDownloadProvider()
        {
            return (from downloadProviderSetting in DownloadProviderSettings where downloadProviderSetting.Enabled select downloadProviderSetting.GetProvider()).FirstOrDefault();
        }
    }

    public sealed class UserSettingsMap : ClassMap<UserSettings>
    {
        UserSettingsMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.Enabled).Not.Nullable();
            Map(x => x.ApiKey).Length(40);
            Map(x => x.C2CPreference).Not.Nullable().Default(C2CPreference.None.ToString());
            Map(x => x.SearchInterval).Not.Nullable().Default("1");

            HasMany(x => x.DownloadProviderSettings).Cascade.All();
            HasMany(x => x.SearchProviderSettings).Cascade.All();
        }
    }

    public class DownloadProviderSettings
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        public virtual int Id { get; private set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        public virtual DownloadProviders Type { get; set; }
        public virtual bool Enabled { get; set; }
        public virtual string Host { get; set; }
        public virtual string Port { get; set; }
        public virtual string Path { get; set; }
        public virtual string ApiKey { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual string Category { get; set; }

        public virtual UserSettings UserSettings { get; set; }

        protected internal virtual IDownloadProvider GetProvider()
        {
            IDownloadProvider provider = null;

            if (Type == DownloadProviders.SABnzbd)
            {
                var sabnzbd = new SABnzbd {ApiKey = ApiKey, Category = Category, Host = Host, Path = Path, Port = Port};

                provider = sabnzbd;
            }
            else if (Type == DownloadProviders.SaveFile)
            {
                var savefile = new SaveFile {Path = Path};
                provider = savefile;
            }

            return provider;
        }
    }

    public sealed class DownloadProviderSettingsMap : ClassMap<DownloadProviderSettings>
    {
        DownloadProviderSettingsMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Type).Not.Nullable().Unique();
            Map(x => x.Enabled).Not.Nullable();
            Map(x => x.Host).Length(254);
            Map(x => x.Port).Length(5);
            Map(x => x.Path).Length(254);
            Map(x => x.ApiKey).Length(40);
            Map(x => x.Username).Length(254);
            Map(x => x.Password).Length(32);
            Map(x => x.Category).Length(16);

            References(x => x.UserSettings).Not.Nullable();
        }
    }

    public class SearchProviderSettings
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        public virtual int Id { get; private set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        public virtual SearchProviders Type { get; set; }
        public virtual bool Enabled { get; set; }
        public virtual string ApiKey { get; set; }
        public virtual string Host { get; set; }
        public virtual string Port { get; set; }
        public virtual string Path { get; set; }
        public virtual bool SSL { get; set; }

        public virtual UserSettings UserSettings { get; set; }

        protected internal virtual ISearchProvider GetProvider()
        {
            ISearchProvider provider = null;

            switch (Type)
            {
                case SearchProviders.NewzNab:
                    {
                        var newznab = new NewzNab { ApiKey = ApiKey, Host = Host, Path = Path, Port = Port, SSL = SSL};
                        provider = newznab;
                    }
                    break;
                case SearchProviders.NUIP:
                    {
                        var nuip = new NUIP { Host = Host, Port = Port, SSL = SSL};
                        provider = nuip;
                    }
                    break;
                case SearchProviders.NZBdotSU:
                    {
                        var nzbdotsu = new NZBdotSU {ApiKey = ApiKey};
                        provider = nzbdotsu;
                    }
                    break;
                case SearchProviders.NZBINDEXdotNL:
                    {
                        var nzindexdotnl = new NZBINDEXdotNL();
                        provider = nzindexdotnl;
                    }
                    break;
            }

            return provider;
        }
    }

    public sealed class SearchProviderSettingsMap : ClassMap<SearchProviderSettings>
    {
        private SearchProviderSettingsMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Type).Not.Nullable().Length(254).Unique();
            Map(x => x.Enabled).Not.Nullable();
            Map(x => x.ApiKey).Length(40);
            Map(x => x.Host).Length(254);
            Map(x => x.Port).Length(5);
            Map(x => x.Path).Length(254);
            Map(x => x.SSL).Not.Nullable();

            References(x => x.UserSettings).Not.Nullable();
        }
    }
}
