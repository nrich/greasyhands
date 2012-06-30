using System;
using System.Configuration;
using System.Linq;
using GreasyHands;
using GreasyHands.DAL.Container;
using GreasyHands.DAL.Session;
using GreasyHands.Jobs;
using Nancy;

namespace GreasyHandsWebApp.Routes
{
    public class Settings : NancyModule
    {
        public Settings(ISession session) :base("/settings")
        {
            string dbFile = ConfigurationManager.AppSettings["DBFile"];

            Get["/"] = parameters =>
                           {
                               var args = Environment.GetCommandLineArgs();
                               var model = new
                                               {
                                                   AppPath = args[0],
                                                   Args = String.Join(",", args.Skip(1).Take(args.Count())),
                                                   OS = Environment.OSVersion.ToString(),
                                                   Runtime = Environment.Version.ToString()
                                               };

                               return View["Settings/Index", model];
                           };

            Get["/general"] = parameters =>
                {
                    using (var db = session.SessionFactory(dbFile))
                    {
                        using (var s = db.OpenSession())
                        {
                            var userSettings = s.QueryOver<UserSettings>().SingleOrDefault<UserSettings>();

                            return View["Settings/General", new { userSettings.ApiKey, userSettings.C2CPreference, userSettings.SearchInterval }];

                        }
                    }
                };
            
            Post["/general"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        using (var transaction = s.BeginTransaction())
                        {
                            var userSettings = s.QueryOver<UserSettings>().SingleOrDefault<UserSettings>();

                            string covertocoverStr = Request.Form.C2CPreference.Value;
                            C2CPreference covertocover;

                            if (!Enum.TryParse(covertocoverStr, false, out covertocover))
                            {
                                throw new ArgumentException("Unknown download provider {0}", covertocoverStr);
                            }

                            userSettings.ApiKey = Request.Form.ApiKey;
                            userSettings.Enabled = true;
                            userSettings.C2CPreference = covertocover;
                            userSettings.SearchInterval = int.Parse(Request.Form.SearchInterval);

                            s.SaveOrUpdate(userSettings);
                            transaction.Commit();

                            return Response.AsRedirect("/settings/general");
                        }
                    }
                }
            };

            Get["/search"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        var searchSettings = s.QueryOver<SearchProviderSettings>().List<SearchProviderSettings>();

                        if (searchSettings.Count == 0)
                        {
                            using (var transaction = s.BeginTransaction())
                            {

                                var userSettings = s.QueryOver<UserSettings>().SingleOrDefault<UserSettings>();



                                var searchSettingsDefault = new SearchProviderSettings
                                                                {
                                                                    ApiKey = "",
                                                                    Type = SearchProviders.NZBINDEXdotNL,
                                                                    Enabled = true,
                                                                    UserSettings = userSettings,
                                                                };

                                transaction.Commit();
                                s.SaveOrUpdate(searchSettingsDefault);
                                searchSettings.Add(searchSettingsDefault);
                            }
                        }

                        return View["Settings/Search",
                            new {
                                Search = searchSettings,
                            }];                            
                    }
                }
            };

            Post["/search"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        using (var transaction = s.BeginTransaction())
                        {
                            if (Request.Form.id.HasValue)
                            {
                                int id = int.Parse(Request.Form.id.Value);

                                var searchSettings =
                                    s.QueryOver<SearchProviderSettings>().Where(se => se.Id == id).SingleOrDefault
                                        <SearchProviderSettings>();

                                if (Request.Form.ApiKey.HasValue)
                                {
                                    searchSettings.ApiKey = Request.Form.ApiKey;                                    
                                }

                                if (Request.Form.Host.HasValue)
                                {
                                    searchSettings.Host = Request.Form.Host;
                                }

                                if (Request.Form.Port.HasValue)
                                {
                                    searchSettings.Port = Request.Form.Port;
                                }

                                if (Request.Form.Path.HasValue)
                                {
                                    searchSettings.Path = Request.Form.Path;
                                }

                                searchSettings.SSL = Request.Form.SSL.HasValue;
                                searchSettings.Enabled = Request.Form.Enabled.HasValue;

                                s.SaveOrUpdate(searchSettings); 
                            } 
                            else
                            {
                                var userSettings = s.QueryOver<UserSettings>().SingleOrDefault<UserSettings>();

                                string typeStr = Request.Form.Type.Value;
                                SearchProviders type;
                                bool enabled = Request.Form.Enabled.HasValue;
                                bool ssl = Request.Form.SSL.HasValue;
                                string host = Request.Form.Host.Value;
                                string port = Request.Form.Port.Value;
                                string path = Request.Form.Path.Value;

                                if (!Enum.TryParse(typeStr, false, out type))
                                {
                                    throw new ArgumentException("Unknown search provider {0}", typeStr);
                                }

                                var searchSettings = new SearchProviderSettings
                                                                {
                                                                    ApiKey = Request.Form.ApiKey,
                                                                    Type = type,
                                                                    Enabled = enabled,
                                                                    UserSettings = userSettings,
                                                                    Host = host,
                                                                    Port = port,
                                                                    Path = path,
                                                                    SSL = ssl,
                                                                };

                                s.SaveOrUpdate(searchSettings); 
                            }

                            transaction.Commit();

                            return Response.AsRedirect("/settings/search");
                        }
                    }
                }
            };

            Get["/download"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        using (var transaction = s.BeginTransaction())
                        {
                            var destinationSettings = s.QueryOver<DownloadProviderSettings>().List<DownloadProviderSettings>();

                            if (destinationSettings.Count == 0)
                            {
                                var userSettings = s.QueryOver<UserSettings>().SingleOrDefault<UserSettings>();

                                /*
                                var searchSettingsDefaultSave = new DownloadProviderSettings
                                {
                                    ApiKey = "",
                                    Host = "",
                                    Port = "",
                                    Type = DownloadProviders.SaveFile,
                                    Enabled = false,
                                    Path = "./",
                                    UserSettings = userSettings,
                                    Category = "",
                                    Username = "",
                                    Password = "",
                                };

                                s.SaveOrUpdate(searchSettingsDefaultSave);
                                destinationSettings.Add(searchSettingsDefaultSave);
                                */

                                var searchSettingsDefaultSABnzbd = new DownloadProviderSettings
                                {
                                    ApiKey = "FIXME",
                                    Host = "localhost",
                                    Port = "9090",
                                    Type = DownloadProviders.SABnzbd,
                                    Enabled = false,
                                    Path = "sabnzbd",
                                    UserSettings = userSettings,
                                    Category = "books",
                                    Username = "",
                                    Password = "",
                                };

                                s.SaveOrUpdate(searchSettingsDefaultSABnzbd);
                                destinationSettings.Add(searchSettingsDefaultSABnzbd);
                            }

                            transaction.Commit();

                            return View["Settings/Download",
                                new
                                {
                                    Destination = destinationSettings,
                                }];
                        }
                    }
                }
            };

            Post["/download"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        using (var transaction = s.BeginTransaction())
                        {
                            if (Request.Form.id.HasValue)
                            {
                                int id = int.Parse(Request.Form.id.Value);

                                var destinationSettings =
                                    s.QueryOver<DownloadProviderSettings>().Where(d => d.Id == id).SingleOrDefault
                                        <DownloadProviderSettings>();

                                if (Request.Form.ApiKey.HasValue)
                                {
                                    destinationSettings.ApiKey = Request.Form.ApiKey;
                                    destinationSettings.Host = Request.Form.Host;
                                    destinationSettings.Port = Request.Form.Port;
                                    destinationSettings.Enabled = Request.Form.Enabled.HasValue;
                                    destinationSettings.Path = Request.Form.Path;
                                    destinationSettings.Category = Request.Form.Category;
                                    destinationSettings.Username = Request.Form.Username;
                                    destinationSettings.Password = Request.Form.Password;
                                }
                            }
                            else
                            {
                                var userSettings = s.QueryOver<UserSettings>().SingleOrDefault<UserSettings>();

                                string typeStr = Request.Form.Type;
                                DownloadProviders type;

                                if (!Enum.TryParse(typeStr, false, out type))
                                {
                                    throw new ArgumentException("Unknown download provider {0}", typeStr);
                                }

                                var destinationSettings = new DownloadProviderSettings
                                {
                                    ApiKey = Request.Form.ApiKey.Value,
                                    Host = Request.Form.Host.Value,
                                    Port = Request.Form.Port.Value,
                                    Type = type,
                                    Enabled = Request.Form.Enabled.HasValue,
                                    Path = Request.Form.Path.Value,
                                    UserSettings = userSettings,
                                    Category = Request.Form.Category.Value,
                                    Username = Request.Form.Username.Value,
                                    Password = Request.Form.Password.Value,
                                };

                                s.SaveOrUpdate(destinationSettings);
                            }

                            transaction.Commit();

                            return Response.AsRedirect("/settings/download");
                        }
                    }
                }
            };



            Get["/wizard"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        using (var transaction = s.BeginTransaction())
                        {
                            var userSettings = s.QueryOver<UserSettings>().SingleOrDefault<UserSettings>() ??
                                               new UserSettings
                                                {
                                                    ApiKey = "hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh".Randomise(),
                                                    Enabled = false,
                                                };

                            s.SaveOrUpdate(userSettings);

                            /*
                            var destinationSettings = s.QueryOver<DownloadProviderSettings>().List<DownloadProviderSettings>();

                            if (destinationSettings.Count == 0)
                            {
                                var searchSettingsDefaultSABnzbd = new DownloadProviderSettings
                                {
                                    ApiKey = "FIXME",
                                    Host = "localhost",
                                    Port = "9090",
                                    Type = DownloadProviders.SABnzbd,
                                    Enabled = false,
                                    Path = "sabnzbd",
                                    UserSettings = userSettings,
                                    Category = "books",
                                    Username = "",
                                    Password = "",
                                };

                                s.SaveOrUpdate(searchSettingsDefaultSABnzbd);
                                destinationSettings.Add(searchSettingsDefaultSABnzbd);
                            }

                            var searchSettings = s.QueryOver<SearchProviderSettings>().List<SearchProviderSettings>();

                            if (searchSettings.Count == 0)
                            {
                                var searchSettingsDefault = new SearchProviderSettings
                                {
                                    ApiKey = "",
                                    Type = SearchProviders.NZBINDEXdotNL,
                                    Enabled = true,
                                    UserSettings = userSettings
                                };

                                s.SaveOrUpdate(searchSettingsDefault);

                                searchSettings.Add(searchSettingsDefault);
                            }
                            */

                            transaction.Commit();

                            return View["Settings/Wizard",
                                new
                                {
                                    //Search = searchSettings,
                                    //Destination = destinationSettings,
                                    userSettings.ApiKey,
                                }];
                        }
                    }
                }
            };

            Post["/wizard"] = parameters =>
            {
                using (var db = session.SessionFactory(dbFile))
                {
                    using (var s = db.OpenSession())
                    {
                        using (var transaction = s.BeginTransaction())
                        {

                            string covertocoverStr = Request.Form.C2CPreference.Value;
                            C2CPreference covertocover;

                            if (!Enum.TryParse(covertocoverStr, false, out covertocover))
                            {
                                throw new ArgumentException("Unknown download provider {0}", covertocoverStr);
                            }
  
                            var userSettings = s.QueryOver<UserSettings>().SingleOrDefault<UserSettings>();
                            userSettings.ApiKey = Request.Form.ApiKey;
                            userSettings.Enabled = true;
                            userSettings.C2CPreference = covertocover;
                            userSettings.SearchInterval = int.Parse(Request.Form.SearchInterval);
                            s.SaveOrUpdate(userSettings);

                            ScheduleUpdater.AddReleaseURL(int.Parse(Request.Form.Weeks));

                            string downloadTypeStr = Request.Form.DownloadType.Value;
                            DownloadProviders downloadType;

                            if (!Enum.TryParse(downloadTypeStr, false, out downloadType))
                            {
                                throw new ArgumentException("Unknown download provider {0}", downloadTypeStr);
                            }

                            var destinationSettings = new DownloadProviderSettings
                            {
                                ApiKey = Request.Form.DownloadApiKey,
                                Host = Request.Form.Host,
                                Port = Request.Form.Port,
                                Type = downloadType,
                                Enabled = Request.Form.DownloadEnabled.HasValue,
                                Path = Request.Form.Path,
                                UserSettings = userSettings,
                                Category = Request.Form.Category,
                                Username = Request.Form.Username,
                                Password = Request.Form.Password,
                            };

                            s.SaveOrUpdate(destinationSettings);

                            string searchTypeStr = Request.Form.SearchType.Value;
                            SearchProviders searchType;

                            if (!Enum.TryParse(searchTypeStr, false, out searchType))
                            {
                                throw new ArgumentException("Unknown search provider {0}", searchTypeStr);
                            }

                            var searchSettings = new SearchProviderSettings
                            {
                                ApiKey = Request.Form.SearchApiKey,
                                Type = searchType,
                                Enabled = Request.Form.SearchEnabled.HasValue,
                                UserSettings = userSettings
                            };

                            s.SaveOrUpdate(searchSettings);

                            transaction.Commit();

                            return Response.AsRedirect("/");
                        }
                    }
                }
            };
        }

    }
}
