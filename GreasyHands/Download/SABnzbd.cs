using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using GreasyHands.Search;

namespace GreasyHands.Download
{
    class SABnzbd : IDownloadProvider
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Path { get; set; }
        public string ApiKey { get; set; }
        public string Category { get; set; }

        public bool QueueDownload(Result result)
        {
            var url = String.Format(
                "http://{0}:{1}/{2}/api?apikey={3}&mode=addurl&name={4}&cat={5}&nzbname={6}",
                Host,
                Port,
                Path,
                ApiKey,
                EmbeddedURL(result.Link),
                Category,
                result.Title.Replace("&", "And")
                );

            try
            {
                var req = WebRequest.Create(url);
                var res = (HttpWebResponse) req.GetResponse();

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    var stream = res.GetResponseStream();

                    if (stream != null)
                    {
                        var streamReader = new StreamReader(stream);
                        var content = streamReader.ReadToEnd();
                        return !content.Contains("error:");
                    }

                    return res.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            return false;
        }

        private static string EmbeddedURL(Uri uri)
        {
            string url = uri.ToString();

            url = HttpUtility.UrlEncode(url).Replace("+", "%20");            
            url = Regex.Replace(url, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());
            url = url.Replace("(", "%28").Replace(")", "%29").Replace("$", "%24").Replace("!", "%21").Replace("*", "%2A").Replace("'", "%27").Replace("&", "%26");

            return url;
        }

    }
}