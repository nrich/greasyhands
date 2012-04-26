using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text.RegularExpressions;
using Commons.Collections;
using NVelocity;
using NVelocity.App;
using Nancy;
using Nancy.Responses;
using Nancy.ViewEngines;

namespace GreasyHandsWebApp
{
    public class NVelocityViewEngine : IViewEngine 
    {
        private readonly VelocityEngine velocityEngine;

        class Helper
        {
// ReSharper disable UnusedMember.Local
            public string ReplaceMatch(string input, string tomatch, string replace)
            {
                var regex = new Regex(tomatch, RegexOptions.IgnoreCase);
                var match = regex.Match(input);
                var result = input;

                if (match.Success)
                {
                    replace = replace.Replace(@"%1", match.Value);
                    result = result.Replace(match.Value, replace);
                }

                return result;                
            }

            public int Percent(int a, int b)
            {
                if (a == 0 || b == 0)
                    return 0;

                float pc = a/(float)b;

                return (int) (pc*100);
            }
        }
        // ReSharper restore UnusedMember.Local

        public NVelocityViewEngine()
        {
            velocityEngine = new VelocityEngine();
            var props = new ExtendedProperties();
            velocityEngine.Init(props);

        }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
            //viewEngineStartupContext.ViewLocationResults
        }

        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            var viewFile = viewLocationResult.Location + "/" + viewLocationResult.Name + "." +
                           viewLocationResult.Extension;

            var response = new HtmlResponse
                               {
                                   Contents = stream =>
                                                  {
                                                      var viewModel = model ?? new ExpandoObject();
                                                      var context = new VelocityContext();
                                                      context.Put("Model", viewModel);
                                                      context.Put("Helper", new Helper());
                                                      var writer = new StreamWriter(stream);
                                                      var template = velocityEngine.GetTemplate(viewFile);
                                                      template.Merge(context, writer);
                                                      writer.Flush();
                                                  }
                               };

            return response;
        }

        public IEnumerable<string> Extensions
        {
            get { yield return "nvt"; }
        }
    }
}
