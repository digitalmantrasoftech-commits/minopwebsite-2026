using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using PayTimeWebClient.Models.Notifications_Module;


namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class NotificationAlertController : Controller
    {

        #region Declaration       

        // Static dictionary to track export statuses
        //private static readonly ConcurrentDictionary<string, ExportStatus> ExportStatuses = new ConcurrentDictionary<string, ExportStatus>();

        static readonly DataGridOptimizeRestClient RestClient = new DataGridOptimizeRestClient();
        #endregion



        #region For NotificationRuleMaster
        public ActionResult NotificationRuleMaster()
        {
            return View();
        }

        #endregion
        // GET: NotificationAlert
        public ActionResult Index()
        {
            return View();
        }

        #region For NotificationTemplateMaster
        //public ActionResult TemplateMaster()
        //{
        //    var model = new NotificationTemplateModel
        //    {
        //        LanguageOptions = new List<SelectListItem>
        //        {
        //            new SelectListItem { Text = "English", Value = "en" },
        //            new SelectListItem { Text = "Spanish", Value = "es" },
        //            new SelectListItem { Text = "French", Value = "fr" }
        //        }
        //    };
        //    return View(model);
        //}

        [CustAuthFilter]
        [HttpGet]
        public ActionResult SampleNotificationDetails(int id)
        {
            try
            {
                //var model;
                NotificationDetailsViewModel model = new NotificationDetailsViewModel();
                var result = RestClient.GetAllNotificationRulesDetails(id); // Assume returns a tuple or model

                model.Table1 = result.Table1;
                model.Table2 = result.Table2;
                model.Table3 = result.Table3;
                return PartialView("SampleNotificationDetails", model);
                //return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        #endregion

        public ActionResult NotificationEventLogData()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        public ActionResult Notificationruleconfig()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }           
        }

        public ActionResult ChatBoatInex()
        {
            return View();
        }
    }
}