using System;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Configuration;

namespace GreasyHandsUpdater
{
    class Program
    {
        [Serializable]
        class Downloaded
        {
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
            public int IssueID { get; set; }

            public int Limited { get; set; }
            public int Num { get; set; }

            public string Title { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Local
// ReSharper restore UnusedMember.Local
        }

        static void Main(string[] args)
        {
            if (args.Length < 1)
                Usage();

            var path = args[0];

            if (String.IsNullOrEmpty(path))
                Usage();

            var file = GetName(path);

            Console.WriteLine(file);

            var url = String.Format("http://{0}:{1}/api/downloaded?title={2}", ConfigurationManager.AppSettings["GreasyHandsHost"], ConfigurationManager.AppSettings["GreasyHandsPort"], file);

            Console.WriteLine(url);

            try
            {
                var request = WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var stream = response.GetResponseStream();

                    if (stream != null)
                    {
                        var reader = new StreamReader(stream);

                        string jsonstr = reader.ReadToEnd();
                        var serializer = new JavaScriptSerializer();

                        var d = serializer.Deserialize<Downloaded>(jsonstr);

                        if (d.Limited > 0)
                        {
                            Console.WriteLine("Marked {0} #{1} (of {2}) as downloaded", d.Title, d.Num, d.Limited);
                        }
                        else
                        {
                            Console.WriteLine("Marked {0} #{1} as downloaded", d.Title, d.Num);
                        }
                    }
                } 
                else
                {
                    Console.WriteLine("Error: {0}", response.StatusCode);    
                }

            }
            catch (WebException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        static private string GetName(string path)
        {
            string file;

            if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
            {
                Console.WriteLine("Is a dir");

                var a = path.Split(Path.DirectorySeparatorChar);

                Array.Reverse(a);

                file = String.IsNullOrEmpty(a[0]) ? a[1] : a[0];
            }
            else
            {
                file = Path.GetFileName(path);
            }

            return file;
        }

        static private void Usage()
        {
            Console.WriteLine("Usage: GreasyHandsUpdater <file>\n");
            Environment.Exit(255);
        }
    }
}
