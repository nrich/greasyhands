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
        public Program(ScheduleUpdater updater, WantedSearch search, ScheduleGrabber grabber)
        {
            new Thread(updater.Run).Start();
            new Thread(search.Run).Start();
            new Thread(grabber.Run).Start();
        }

        // ReSharper disable FunctionNeverReturns
        // ReSharper disable UnusedParameter.Local
        static void Main(string[] args)
        // ReSharper restore UnusedParameter.Local
        {
            var builder = new ContainerBuilder();

            string host = "localhost";
            string port = "12345";

            if (args.Length == 2)
            {
                host = args[0];
                port = args[1];
            }
            else if (args.Length == 1)
            {
                host = args[0];
            }

            var url = String.Format("http://{0}:{1}", host, port);

            Console.WriteLine("Listening on {0}", url);

            //string dbFile = ConfigurationManager.AppSettings["DBFile"];

            builder.RegisterType<SearchMatcher>().As<ISearchMatcher>();
            builder.RegisterType<SQLIteSession>().As<ISession>();
            builder.RegisterType<TitleInfo>();
            builder.RegisterType<TitleParser>();
            builder.RegisterType<ScheduleParser>();
            builder.RegisterType<ScheduleUpdater>();
            builder.RegisterType<ScheduleGrabber>();
            builder.RegisterType<WantedSearch>();

            builder.RegisterType<Program>();



            var nancyHost = new NancyHost(new Uri(url));
            

            using (var container = builder.Build())
            {
                container.Resolve<Program>();
                nancyHost.Start(); // start hosting
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
