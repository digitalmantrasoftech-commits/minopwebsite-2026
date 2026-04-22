using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayTimeWebClient.Controllers
{
    public class VMSController : Controller
    {
        // GET: VMS
        public ActionResult VMSDashboard()
        {
            return View();
        }

        public ActionResult Visitors()
        {
            return View();
        }

        public ActionResult Scheduled()
        {
            return View();
        }

        public ActionResult Visitortype()
        {
            return View();
        }

        public ActionResult location()
        {
            return View("~/Views/VMS/location.cshtml");
            //return View();
        }
    }
}