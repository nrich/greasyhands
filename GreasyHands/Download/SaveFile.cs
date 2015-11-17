using System;
using System.IO;
using System.Net;
using GreasyHands.Search;
using System.IO.Path;

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
                    
                    using (var stream = res.GetResponseStream())
                    {
                        string fileName;

                        if (res.Headers["Content-Disposition"] != null) {
                            fileName = res.Headers["Content-Disposition"].Replace("Attachment; filename=", "").Replace("\"", "");
                        } else {
                            fileName = System.IO.Path.GetFileName(result.Link.AbsolutePath);
                        }

                        var streamReader = new StreamReader(stream);
                        var content = streamReader.ReadToEnd();
                        
                        var fullFileName = string.Format("{0}/{1}", Path, fileName);

                        var streamWriter = new StreamWriter(new FileStream(fullFileName, FileMode.Create));
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
