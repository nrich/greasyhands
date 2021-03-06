﻿using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using GreasyHands.DAL.Container;
using GreasyHands.DAL.Session;

namespace GreasyHands.Schedule
{
    public class ScheduleParser
    {
        private readonly TitleParser parser;
        private readonly ISession session;

        public ScheduleParser(ISession session, TitleParser parser)
        {
            this.session = session;
            this.parser = parser;
        }

        public DateTime ShippingDate(string line)
        {
            var shippingDatePattern = new Regex(@"^(?:Shipping(?:\s+\w+\s+Week:)?\s+)(.+)$", RegexOptions.IgnoreCase);

            if (shippingDatePattern.IsMatch(line))
            {
                var m = shippingDatePattern.Match(line);

                var shippingDate = DateTime.Parse(m.Groups[1].Value, new CultureInfo("en-US"));

                return shippingDate;
            }

            var shippingDatePatternRelease = new Regex(@"^New Releases For (.+)$", RegexOptions.IgnoreCase);
            if (shippingDatePatternRelease.IsMatch(line))
            {
                var m = shippingDatePatternRelease.Match(line);

                var shippingDate = DateTime.Parse(m.Groups[1].Value, new CultureInfo("en-US"));

                return shippingDate;
            }

            return DateTime.MinValue;
        }

        public void FixExistingTitles()
        {
            using (var db = session.SessionFactory("test"))
            {
                using (var s = db.OpenSession())
                {
                    foreach (var title in s.QueryOver<Title>().List<Title>())
                    {
                        var titleName = title.Name;
                        var fixedTitle = parser.FixTitle(titleName);                        

                        if (titleName != fixedTitle)
                        {
                            var titleList = s.QueryOver<Title>().Where(t => t.Name == titleName).List<Title>();

                            foreach (var t in titleList)
                            {
                                using (var transaction = s.BeginTransaction())
                                {
                                    t.Name = fixedTitle;

                                    if (t.SearchTitle == titleName)
                                    {
                                        t.SearchTitle = fixedTitle;
                                    }

                                    s.SaveOrUpdate(t);
                                    transaction.Commit();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Parse(TextReader reader)
        {

            string publisher = null;
            bool startParsing = false;
            string line;

            DateTime shippingDate = DateTime.MinValue;

            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();

                if (line.Length == 0)
                    continue;

                if (line.Contains("PREMIER PUBLISHERS"))
                {
                    startParsing = true;
                    continue;
                }

                var headerPattern = new Regex(@"ITEM\s+CODE\s+TITLE\s+PRICE", RegexOptions.IgnoreCase);


                if (!startParsing && shippingDate == DateTime.MinValue)
                {
                    shippingDate = ShippingDate(line);

                    if (shippingDate == DateTime.MinValue)
                        continue;

                    using (var db = session.SessionFactory("test"))
                    {
                        using (var s = db.OpenSession())
                        {
                            using (var transaction = s.BeginTransaction())
                            {
                                var date = shippingDate;
                                var release = s.QueryOver<Release>().Where(r => r.Date == date).SingleOrDefault<Release>();
                                if (release == null)
                                {
                                    release = new Release { Created = DateTime.Now, Date = shippingDate };
                                    s.SaveOrUpdate(release);
                                    transaction.Commit();
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                    continue;
                }

                if (headerPattern.IsMatch(line))
                {
                    startParsing = true;
                    continue;
                }

                if (!startParsing && Publisher.IsPublisher(line))
                {
                    publisher = Publisher.PrettyName(line);
                    startParsing = true;
                }

                if (!startParsing)
                    continue;

                if (Publisher.IsPublisher(line))
                {
                    publisher = Publisher.PrettyName(line);
                    continue;
                }

                var titleInfo = parser.BuildTitleInfo(line, publisher, shippingDate);

                if (titleInfo == null) 
                    continue;

                titleInfo.Insert(session);
            }
        }
    }
}
