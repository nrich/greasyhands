using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using GreasyHands.Schedule;

namespace GreasyHands.Jobs
{
    public class ScheduleGrabber
    {
        public ScheduleGrabber()
        {
        }

        // ReSharper disable FunctionNeverReturns
        public void Run()
        {
            Thread.CurrentThread.Name = "Schedule Grabber";

            while (true)
            {
                ScheduleUpdater.AddReleaseURL(new Uri("http://www.previewsworld.com/shipping/newreleases.txt"));
                Thread.Sleep(3600 * 1000);
            }

        }
        // ReSharper restore FunctionNeverReturns
    }
}
