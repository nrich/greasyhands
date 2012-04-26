using System;
using System.Threading;
using Autofac;
using GreasyHands.DAL.Session;
using GreasyHands.Jobs;
using GreasyHands.Schedule;
using GreasyHands.Search.Matcher;
using GreasyHands.Search.Provider;
using Nancy.Hosting.Self;

namespace GreasyHandsWebApp
{
    class Program
    {
        public Program(ScheduleUpdater updater, WantedSearch search)
        {
            ScheduleUpdater.AddReleaseURL(new Uri("http://www.previewsworld.com/shipping/newreleases.txt"));
            new Thread(updater.Run).Start();
            new Thread(search.Run).Start();
        }

        // ReSharper disable FunctionNeverReturns
        // ReSharper disable UnusedParameter.Local
        static void Main(string[] args)
        // ReSharper restore UnusedParameter.Local
        {
            var builder = new ContainerBuilder();
            var searchProvider = new NZBdotSU { ApiKey = "test" };
            //string dbFile = ConfigurationManager.AppSettings["DBFile"];

            builder.RegisterType<SearchMatcher>().As<ISearchMatcher>();
            builder.RegisterType<SQLIteSession>().As<ISession>();
            builder.RegisterInstance(searchProvider).As<ISearchProvider>();
            builder.RegisterType<TitleInfo>();
            builder.RegisterType<TitleParser>();
            builder.RegisterType<ScheduleParser>();
            builder.RegisterType<ScheduleUpdater>();
            builder.RegisterType<WantedSearch>();
            

            builder.RegisterType<Program>();

            var host = new NancyHost(new Uri("http://localhost:12345"));
            host.Start(); // start hosting

            using (var container = builder.Build())
            {
                container.Resolve<Program>();
                Console.ReadKey();
				
				while (true) 
				{
					Thread.Sleep(1000);
				}				
            }
        }
        // ReSharper restore FunctionNeverReturns
    }
}
