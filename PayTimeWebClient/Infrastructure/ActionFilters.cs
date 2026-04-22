using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PayTimeWebClient.Infrastructure
{
    public class CustAuthFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            string _ActionName = filterContext.ActionDescriptor.ActionName;
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                // For AJAX requests, return result as a simple string,
                // and inform calling JavaScript code that a user should be redirected.
                if (HttpContext.Current.Session["tokan"] == null && _ActionName != "Login"
                && _ActionName != "LogOff" && _ActionName != "ChangePasswordSuccess" )
                {
                    filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "Paytime", action = "Timeout" }));
                }
            }
            else
            {
                //HttpCookie tokanc = HttpContext.Current.Request.Cookies["jwt_token"];
                if (HttpContext.Current.Session["tokan"] == null && _ActionName != "Login"
                    && _ActionName != "LogOff" && _ActionName != "ChangePasswordSuccess" )
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Paytime", action = "Index" }));
                }
            }
        }
    }




}