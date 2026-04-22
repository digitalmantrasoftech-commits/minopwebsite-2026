using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PayTimeWebClient.Helper
{
    public class MyBaseController : Controller
    {
        // Here I have created this for execute each time any controller (inherit this) load 
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string lang = null;
            HttpCookie langCookie = Request.Cookies["culture"];
            if (langCookie != null)
            {
                lang = langCookie.Value;
            }
            else
            {
                var userLanguage = Request.UserLanguages;
                var userLang = userLanguage != null ? userLanguage[0] : "";
                if (userLang != "")
                {
                    lang = userLang;
                }
                else
                {
                    lang = SiteLanguages.GetDefaultLanguage();
                }
            }

            new SiteLanguages().SetLanguage(lang);

            return base.BeginExecuteCore(callback, state);
        }
    }


    //public class SubdomainRoute : Route
    //{
    //    public SubdomainRoute(string domain, string url, RouteValueDictionary defaults)
    //        : this(domain, url, defaults, new MvcRouteHandler())
    //    {
    //    }

    //    public SubdomainRoute(string domain, string url, object defaults)
    //        : this(domain, url, new RouteValueDictionary(defaults), new MvcRouteHandler())
    //    {
    //    }

    //    public SubdomainRoute(string domain, string url, object defaults, IRouteHandler routeHandler)
    //        : this(domain, url, new RouteValueDictionary(defaults), routeHandler)
    //    {
    //    }

    //    public SubdomainRoute(string domain, string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
    //        : base(url, defaults, routeHandler)
    //    {
    //        this.Domain = domain;
    //    }

    //    public string Domain { get; set; }

    //    public override RouteData GetRouteData(HttpContextBase httpContext)
    //    {
    //        var routeData = base.GetRouteData(httpContext);
    //        routeData.Values.Add("client", httpContext.Request.Url.Host.Split('.')[0].ToString());
    //        return routeData;
    //    }
    //}
}