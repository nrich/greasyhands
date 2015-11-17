using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreasyHands.Search.Provider
{
    // ReSharper disable InconsistentNaming
    public class NUIP : ISearchProvider
    // ReSharper restore InconsistentNaming
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public bool SSL { get; set; }

        private string GetSearchURL(string searchString)
        {
            return String.Format(
                "/search?search={0}",
                HttpUtility.UrlPathEncode(searchString)
            );
        }

        private string GetSearchURL(string searchString, string optional)
        {
            return String.Format(
                "/search?search={0}&optional={1}",
                HttpUtility.UrlPathEncode(searchString),
                HttpUtility.UrlPathEncode(optional)
            );
        }

        private string GetHostResource()
        {
            return String.Format(
                "{0}://{1}:{2}",
                SSL ? "https" : "http",
                Host,
                Port
            );
        }

        public List<Result> Search(Query query)
        {
            var searchString = String.Format(!string.IsNullOrWhiteSpace(query.Year) ? "{0} {1}" : "{0}", query.Title, query.Year);
            var optionalString = String.Format("{0:000} {0:00}", query.Num);

            var url = GetSearchURL(searchString, optionalString);
            var feed = GetRssFeed.GetRssFeed.Get(GetHostResource(), url);

            Console.WriteLine(searchString);
            Console.WriteLine(url);

            if (feed == null)
                return new List<Result>();

            if (feed.Channels == null)
                return new List<Result>();

            if (feed.Channels[0].Items == null || feed.Channels[0].Items.Count == 0)
                return new List<Result>();

            Console.WriteLine(feed.Channels[0].Items[0].Link);

            return feed.Channels[0].Items.Select(item => new Result {
                                                                Link = new Uri(item.Link), 
                                                                Published = item.PubDate, 
                                                                Title = item.Title, 
                                                                Description = item.Description, 
                                                                IssueID = query.IssueID
                                                             }).ToList();
        }
    }
}
