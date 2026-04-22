using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayTimeWebClient.Controllers
{
    public class AttendanceController : Controller
    {
        //
        // GET: /Attendance/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SignIn()
        {
            TempData["LoginType"] = "SignIn";
            return RedirectToAction("LoginPage", "PayTime");
            //return View();
        }
        public ActionResult Signup()
        {
            TempData["LoginType"] = "SignUP";
            return RedirectToAction("LoginPage", "PayTime");
        }
	}
}