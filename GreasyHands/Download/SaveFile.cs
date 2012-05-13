using System;
using System.IO;
using System.Net;
using GreasyHands.Search;

namespace GreasyHands.Download
{
    class SaveFile : IDownloadProvider
    {
        public string Path { get; set; }

        public bool QueueDownload(Result result)
        {
            try
            {
                var req = WebRequest.Create(result.Link);
                var res = (HttpWebResponse)req.GetResponse();

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    var stream = res.GetResponseStream();

                    if (stream != null)
                    {
                        var streamReader = new StreamReader(stream);
                        var content = streamReader.ReadToEnd();
                        var filename = string.Format("{0}/{1}", Path,
                                                     result.Link.Fragment[result.Link.Fragment.Length - 1]);

                        var streamWriter = new StreamWriter(new FileStream(filename, FileMode.Create));
                        streamWriter.Write(content);
                        streamWriter.Close();

                        return true;
                    }
                }

                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }
    }
}