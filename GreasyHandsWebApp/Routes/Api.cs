using GreasyHands.DAL.Container;
using GreasyHands.DAL.Session;
using Nancy;

namespace GreasyHandsWebApp.Routes
{
    class Api : NancyModule
    {
        Api(ISession session) : base("/api")
        {
            Post["/downloaded"] = parameters =>
            {
                if (!Request.Form.ApiKey.HasValue)
                {
                    return new Response {StatusCode = HttpStatusCode.Unauthorized};
                }

                if (!Request.Form.Title.HasValue)
                {
                    return new Response { StatusCode = HttpStatusCode.ExpectationFailed };
                }

                string apiKey = Request.Form.ApiKey.Value;
                //string title = Request.Form.Title.Value;

                using (var db = session.SessionFactory(""))
                {
                    using (var s = db.OpenSession())
                    {
                        using (s.BeginTransaction())
                        {
                            var userSettings = s.QueryOver<UserSettings>().SingleOrDefault<UserSettings>();

                            if (apiKey != userSettings.ApiKey)
                            {
                                return new Response { StatusCode = HttpStatusCode.Forbidden };
                            }

                            return new Response { StatusCode = HttpStatusCode.OK };
                        }
                    }
                }
            };
        }
    }
}
