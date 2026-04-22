using System;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
namespace PayTimeWebClient.Handler
{
    /// <summary>
    /// Summary description for sessionAlive
    /// </summary>
    public class SessionHeartbeatHttpHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            context.Session["Heartbeat"] = DateTime.Now;
        }
    }
}