using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace GreasyHands.Search.Provider
{
// ReSharper disable InconsistentNaming
    public class NZBINDEXdotNL : ISearchProvider
// ReSharper restore InconsistentNaming
    {
        private string GetSearchURL(string searchString)
        {
            return String.Format(
                "/rss/?q={0}&g[]=41&g[]=779&sort=agedesc&max=250&more=1",
                HttpUtility.UrlPathEncode(searchString)
            );
        }

        public List<Result> Search(Query query)
        {
            var searchString = String.Format(!string.IsNullOrWhiteSpace(query.Year) ? "{0} {1} {2}" : "{0} {1}", query.Title, query.Num, query.Year);

            var url = GetSearchURL(searchString);
            Console.WriteLine(url);
            var feed = GetRssFeed.GetRssFeed.Get("http://www.nzbindex.nl", url);

            if (feed.Channels == null)
                return new List<Result>();

            if (feed.Channels[0].Items == null)
                return new List<Result>();

            return (from item in feed.Channels[0].Items
                    let title = CleanTitle(item.Title)
                    let link = CleanLink(new Uri(item.Link))
                    select new Result
                               {
                                   Link = link, Published = item.PubDate, Title = title, Description = item.Description, IssueID = query.IssueID
                               }).ToList();
        }

        private string CleanTitle(string title)
        {
            title = title.Replace('_', ' ');

            var regex = new Regex("\"(.+?)\"");
            var match = regex.Match(title);

            if (match.Success)
            {
                title = match.Groups[1].Value;
            }

            return title;
        }

        private Uri CleanLink(Uri uri)
        {
            var link = uri.ToString();

            link = link.Replace("http://www.nzbindex.nl/release", "http://www.nzbindex.nl/download");

            return new Uri(link);
        }
    }
}
