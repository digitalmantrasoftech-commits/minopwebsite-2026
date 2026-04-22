using PayTimeWebClient.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class SchoolSystemSettingController : Controller
    {
        //
        // GET: /SchoolSystemSetting/
        public ActionResult SmsAccount()
        {
            return View();
        }
    }
}