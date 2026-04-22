using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Security.Claims;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Helper;
using System.Net;
using DevExpress.XtraReports.Web.WebDocumentViewer;

namespace PayTimeWebClient
{
    public class MvcApplication : System.Web.HttpApplication
    {

        //public  string tokan = string.Empty;
        //public  string CompanyCode = string.Empty;
        //public  string UserEmail = string.Empty;
        //public  int UserId = 0;
        //public  int CompanyId = 0;
        //public  int RoleId = 0;
        //public  string CompanyName = string.Empty;
        public static  string webapiuri = ConfigurationManager.AppSettings["webapipaytime"].ToString();
        //public static string webapiuri = "http://localhost:2159/api";


      

        protected void Application_Start()
        {
            //DevExpress.XtraReports.Web.WebDocumentViewer.Native.WebDocumentViewerBootstrapper.SessionState = System.Web.SessionState.SessionStateBehavior.Default;
            DefaultWebDocumentViewerContainer.UseFileDocumentStorage(Server.MapPath("~/App_Data/ReportPreviewCache"));
            var dueTime = TimeSpan.FromSeconds(30);
            var period = TimeSpan.FromSeconds(30);
            var reportTimeTolive = TimeSpan.FromMinutes(15);
            var documentTimeToLive = TimeSpan.FromMinutes(15);
            var exporteddocumentTimeToLive = TimeSpan.FromMinutes(15);
            DefaultWebDocumentViewerContainer.RegisterSingleton<StorageCleanerSettings>(new StorageCleanerSettings(dueTime, period, reportTimeTolive, documentTimeToLive, exporteddocumentTimeToLive));
            DefaultWebDocumentViewerContainer.RegisterSingleton<CacheCleanerSettings>(new CacheCleanerSettings(dueTime, period, reportTimeTolive, documentTimeToLive));
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = false;
            MvcHandler.DisableMvcResponseHeader = true;
        }
        protected void Application_Error(object sender, EventArgs e)
        {


            bool isResponseAvailable = true;
            try
            {
                //TODO: am p1. Write full error handling. Do special processing for unathorized requests (((HttpException)ex).ErrorCode == -2147467259).
                var ex = Server.GetLastError();
                //GetDeviceDetails.LogFileWrite(ex.Message+"--"+ex.StackTrace);

                try
                {
                    Response.Clear();
                    //Session.Clear();
                    //Session.Abandon();
                }
                catch
                {
                    //Errors that happen outside user requests will have no defined Response object,
                    //but checking it for null also causes an exception. Suppress.
                    //TODO: am p3. Find a safe way to to determine when we are not in user request.
                    isResponseAvailable = false;
                }

                //Check if this is not a real error but an unauthorized access attempt.
                //Forms security should automatically redirect to whatever URL is specified
                //in web.config, but sometimes these exceptions fall through to here.
                //Unauthorized exceptions seem to have a very specific error code, 
                //although it is possible that some other errors types may result 
                //in the same error code.
                //TODO: am p2. Research if this error code check is sufficient.
                //TODO: am p3. We can create our own class of non-fatal exceptions and redirect users as needed here.
                var isUnauthorizedAccessAttempt = ex != null && ex is HttpException && ((HttpException)ex).ErrorCode == -2147467259; //OxFFFFFFFF80004005

                long uniqueErrorCode = DateTime.UtcNow.Ticks;

                
                //Log in either case to help research if unauthorized access attempts reach this place.
                 //Logger.Log<Global>(isUnauthorizedAccessAttempt ? Level.Warn : Level.Error,
                 //   Category.UnhandledErrors, "Global application error.",
                 //   string.Format("UniqueErrorCode={0};IsUnauthorized={1}",
                 //   uniqueErrorCode, isUnauthorizedAccessAttempt),
                 //   null, ex);





                Server.ClearError();

                if (isResponseAvailable)
                {
                    //if (!isUnauthorizedAccessAttempt)
                    //{
                        //Try to redirect to some user friendly page.
                        string strexception = "Errorcode:" + uniqueErrorCode.ToString() + " MyHandler caught : " + ex.Message + "Method:" + ex.StackTrace;
                        GetDeviceDetails.ProcessLogLogFileWrite(isUnauthorizedAccessAttempt ? "Unauthorized" : "Fatal", strexception);
                        var routeData = new RouteData();
                        routeData.Values.Add("controller", "Error");
                        routeData.Values.Add("action",
                            isUnauthorizedAccessAttempt ? "Unauthorized" : "Fatal");
                        routeData.Values.Add("uniqueErrorCode", uniqueErrorCode.ToString());
                        IController errorController = new Controllers.ErrorController();
                        errorController.Execute(new RequestContext(
                            new HttpContextWrapper(Context), routeData));
                    //}

                }

                System.Diagnostics.Debug.WriteLine(ex);
            }
            catch (Exception eFatal)
            {
                string strexception = "Errorcode=UnhandledErrors :"+ eFatal.Message + "Method:" + eFatal.StackTrace;
                GetDeviceDetails.ProcessLogLogFileWrite(isResponseAvailable ? "Unauthorized" : "Fatal", strexception);
                var routeData = new RouteData();
                routeData.Values.Add("controller", "Error");
                routeData.Values.Add("action", "Fatal");
                routeData.Values.Add("uniqueErrorCode", eFatal.HResult);
                IController errorController = new Controllers.ErrorController();
                errorController.Execute(new RequestContext(
                    new HttpContextWrapper(Context), routeData));
                //Logger.Log<Global>(Level.Fatal,
                //    Category.UnhandledErrors, "Global application error.",
                //    string.Format("IsResponseAvailable={0}", isResponseAvailable),
                //    null, eFatal);

                //This is the worst case scenario. Recovery failed and the user 
                //is probably looking at a blank screen now. Not much else we can do.
            }
        }
        protected void Application_PreSendRequestHeaders()
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Response.Headers.Remove("Server");
                HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");
            }
        }



        //======================== url 301  =============================================================
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            string lowercaseUrl = HttpContext.Current.Request.Url.ToString().ToLower();

            if (lowercaseUrl == "https://app.minopcloud.com/feature/mobile-app-for-employees")
            {
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", lowercaseUrl.Replace(lowercaseUrl, "https://app.minopcloud.com/Feature/Mobile-App-For-Attendance"));
                Response.End();
            }
            else if (lowercaseUrl == "https://app.minopcloud.com/download")
            {
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", lowercaseUrl.Replace(lowercaseUrl, "https://app.minopcloud.com/Download-App"));
                Response.End();
            }
            else if (lowercaseUrl == "https://app.minopcloud.com/support")
            {
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", lowercaseUrl.Replace(lowercaseUrl, "https://app.minopcloud.com/Support-Request"));
                Response.End();
            }


            else if (lowercaseUrl == "https://app.minopcloud.com/feature/overtime-management")
            {
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", lowercaseUrl.Replace(lowercaseUrl, "https://app.minopcloud.com/Attendance-Feature/Overtime-Management"));
                Response.End();
            }
            else if (lowercaseUrl == "https://app.minopcloud.com/feature/mobile-app-for-attendance")
            {
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", lowercaseUrl.Replace(lowercaseUrl, "https://app.minopcloud.com/Attendance-Feature/Mobile-App-For-Attendance"));
                Response.End();
            }
            else if (lowercaseUrl == "https://app.minopcloud.com/feature/leave-management")
            {
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", lowercaseUrl.Replace(lowercaseUrl, "https://app.minopcloud.com/Attendance-Feature/Leave-Management"));
                Response.End();
            }
            else if (lowercaseUrl == "https://app.minopcloud.com/feature/employee-self-service")
            {
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", lowercaseUrl.Replace(lowercaseUrl, "https://app.minopcloud.com/Attendance-Feature/Employee-Self-Service"));
                Response.End();
            }
            else if (lowercaseUrl == "https://app.minopcloud.com/feature/attendance-management")
            {
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", lowercaseUrl.Replace(lowercaseUrl, "https://app.minopcloud.com/Attendance-Feature/Attendance-Management"));
                Response.End();
            }
            else if (lowercaseUrl == "https://app.minopcloud.com/feature")
            {
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", lowercaseUrl.Replace(lowercaseUrl, "https://app.minopcloud.com/Attendance-Feature"));
                Response.End();
            }
            else if (lowercaseUrl == "https://app.minopcloud.com/payroll")
            {
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", lowercaseUrl.Replace(lowercaseUrl, "https://app.minopcloud.com/Payroll-Software"));
                Response.End();
            }


        }
        public class HyphenatedRouteHandler : MvcRouteHandler
        {
            protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                requestContext.RouteData.Values["controller"] = requestContext.RouteData.Values["controller"].ToString().Replace("-", "");
                requestContext.RouteData.Values["action"] = requestContext.RouteData.Values["action"].ToString().Replace("-", "");
                return base.GetHttpHandler(requestContext);
            }
        }
        //======================== url 301  =============================================================

        
    }
}
