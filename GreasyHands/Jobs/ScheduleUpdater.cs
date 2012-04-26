using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using GreasyHands.Schedule;

namespace GreasyHands.Jobs
{
    public class ScheduleUpdater
    {
        private readonly ScheduleParser parser;
        private readonly static Queue<string> UpdateURLs = new Queue<string>();

        public ScheduleUpdater(ScheduleParser parser)
        {
            this.parser = parser;
        }

        private static string PastRelease(int weeksAgo)
        {
            var start = DateTime.Now;

            start = 
                start.DayOfWeek > DayOfWeek.Wednesday ? 
                    start.AddDays(-((int) start.DayOfWeek - (int) DayOfWeek.Wednesday)) : 
                    start.AddDays(-(7 + (int)start.DayOfWeek - (int)DayOfWeek.Wednesday));

            var wednesday = start.AddDays(-7 * weeksAgo);

            return wednesday.ToString("MMddyy");
        }

        public static void AddReleaseURL(int weeks)
        {
            for(int i = 0; i < weeks; i++)
            {
                var past = PastRelease(i + 1);

                var url = string.Format("http://www.previewsworld.com/Archive/GetFile/1/1/71/994/{0}.txt", past);
                Console.WriteLine(url);
                UpdateURLs.Enqueue(url);
            }
        }

        public static void AddReleaseURL(Uri url)
        {
            UpdateURLs.Enqueue(url.ToString());
        }

        // ReSharper disable FunctionNeverReturns
        public void Run()
        {
            Thread.CurrentThread.Name = "Schedule Updater";

            while (true)
            {
                if (UpdateURLs.Count > 0)
                {

                    var url = UpdateURLs.Dequeue();

                    Console.WriteLine(url);

                    var request = WebRequest.Create(url);

                    try
                    {
                        var response = (HttpWebResponse)request.GetResponse();

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var stream = response.GetResponseStream();

                            if (stream != null)
                            {
                                var reader = new StreamReader(stream);
                                parser.Parse(reader);
                            }
                        }
                        else
                        {
                            Console.WriteLine(response.StatusCode.ToString());
                        }

                        response.Close();
                    }
                    catch (WebException we)
                    {

                        Console.WriteLine(we.ToString());
                    }
                }

                Thread.Sleep(1000);
            }

        }
        // ReSharper restore FunctionNeverReturns
    }
}
