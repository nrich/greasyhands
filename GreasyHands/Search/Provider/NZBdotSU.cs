using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreasyHands.Search.Provider
{
    // ReSharper disable InconsistentNaming
    public class NZBdotSU : ISearchProvider
    // ReSharper restore InconsistentNaming
    {
        private const int COMICS_CATEGORY = 7030;

        public string ApiKey { get; set; }

        private string GetSearchURL(string searchString)
        {
            return String.Format(
                "/api?apikey={0}&t=search&q={1}&cat={2}",
                ApiKey,
                HttpUtility.UrlPathEncode(searchString),
                COMICS_CATEGORY
            );
        }

        public List<Result> Search(Query query)
        {
            var searchString = String.Format("{0} {1}", query.Title, query.Num);

            var url = GetSearchURL(searchString);
            var feed = GetRssFeed.GetRssFeed.Get("https://nzb.su", url);

            Console.WriteLine(searchString);
            Console.WriteLine(url);

            if (feed == null)
                return new List<Result>();

            if (feed.Channels == null)
                return new List<Result>();

            if (feed.Channels[0].Items == null)
                return new List<Result>();

            return feed.Channels[0].Items.Select(item => new Result
                                                             {
                                                                 Link = new Uri(item.Link), Published = item.PubDate, Title = item.Title, Description = item.Description, IssueID = query.IssueID
                                                             }).ToList();
        }
    }
}
