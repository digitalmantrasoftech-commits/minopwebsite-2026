using System.Web.Mvc;

namespace PayTimeWebClient.App_Start
{
    public class TenantActionFilter : ActionFilterAttribute, IActionFilter
    {

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var fullAddress = filterContext.HttpContext.Request.Headers["Host"].Split('.');
            var tenantSubdomain = "";

            //for Add checkdomail at the time of Minopcloud.com --- 21 01 2022 ---- Jayesh/Samit

            if (fullAddress.Length == 3 && fullAddress[0].ToString().ToLower() != "www" && fullAddress[0].ToString().ToLower() == "app")
            {

                tenantSubdomain = fullAddress[0];
                // filterContext.Result = new HttpStatusCodeResult(404); //or redirect filterContext.Result = new RedirectToRouteResult(..);                
            }
            else
            {

                tenantSubdomain = "Default";
                // Lookup tenant id (preferably use a cache)
            }


            filterContext.RouteData.Values.Add("tenant", tenantSubdomain);
            base.OnActionExecuting(filterContext);
        }

        //public void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    var fullAddress = filterContext.HttpContext.Request.Headers["Host"].Split('.');
        //    var tenantSubdomain = "";
        //    if (fullAddress.Length < 2)
        //    {
        //        // filterContext.Result = new HttpStatusCodeResult(404); //or redirect filterContext.Result = new RedirectToRouteResult(..);
        //        tenantSubdomain = "Default";
        //    }
        //    else
        //    {
        //        tenantSubdomain = fullAddress[0];
        //        // Lookup tenant id (preferably use a cache)
        //    }


        //    filterContext.RouteData.Values.Add("tenant", tenantSubdomain);
        //    base.OnActionExecuting(filterContext);
        //}
    }
}