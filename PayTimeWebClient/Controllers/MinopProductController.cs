using Microsoft.CSharp.RuntimeBinder;
using PayTimeWebClient.Infrastructure;
using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace PayTimeWebClient.Controllers
{
    public class MinopProductController : Controller
    {
        // GET: MinopProduct
        public ActionResult PayrollDashboard()
        {
            ActionResult result;
            try
            {
                bool flag = base.Session["tokan"] == null;
                if (flag)
                {
                    result = base.RedirectToAction("LoginPage", "PayTime");
                }
                else
                {
                    base.Session["SelectedProductId"] = 2;
                    base.Session["SelectedProductName"] = "Payroll";
                    base.Session["SelectedProductCode"] = "PAYROLL";
                    base.Session["CurrentApiUrl"] = ConfigurationManager.AppSettings["webapipaytimePayroll"];                  
                    result = base.View();
                }
            }
            catch (Exception)
            {
                result = base.RedirectToAction("ErrorPage", "PayTime");
            }
            return result;
        }

        public ActionResult EMSDashboard()
        {
            ActionResult result;
            try
            {
                bool flag = base.Session["tokan"] == null;
                if (flag)
                {
                    result = base.RedirectToAction("LoginPage", "PayTime");
                }
                else
                {
                    base.Session["SelectedProductId"] = 3;
                    base.Session["SelectedProductName"] = "EMS";
                    base.Session["SelectedProductCode"] = "EMS";
                    base.Session["CurrentApiUrl"] = ConfigurationManager.AppSettings["WebApiEMS"];
                    result = base.View();
                }
            }
            catch (Exception)
            {
                result = base.RedirectToAction("ErrorPage", "PayTime");
            }
            return result;
        }

        public ActionResult OKRDashboard()
        {
            ActionResult result;
            try
            {
                bool flag = base.Session["tokan"] == null;
                if (flag)
                {
                    result = base.RedirectToAction("LoginPage", "PayTime");
                }
                else
                {
                    base.Session["SelectedProductId"] = 8;
                    base.Session["SelectedProductName"] = "OKR";
                    base.Session["SelectedProductCode"] = "OKR";
                    base.Session["CurrentApiUrl"] = ConfigurationManager.AppSettings["webapiurlokr"];                
                    result = base.View();
                }
            }
            catch (Exception)
            {
                result = base.RedirectToAction("ErrorPage", "PayTime");
            }
            return result;
        }

        public ActionResult PMSDashboard()
        {
            ActionResult result;
            try
            {
                bool flag = base.Session["tokan"] == null;
                if (flag)
                {
                    result = base.RedirectToAction("LoginPage", "PayTime");
                }
                else
                {
                    base.Session["SelectedProductId"] = 4;
                    base.Session["SelectedProductName"] = "PMS";
                    base.Session["SelectedProductCode"] = "PMS";
                    base.Session["CurrentApiUrl"] = ConfigurationManager.AppSettings["webapiPMS"];                  
                    result = base.View();
                }
            }
            catch (Exception)
            {
                result = base.RedirectToAction("ErrorPage", "PayTime");
            }
            return result;
        }

        public ActionResult FieldSensDashboard()
        {
            ActionResult result;
            try
            {
                bool flag = base.Session["tokan"] == null;
                if (flag)
                {
                    result = base.RedirectToAction("LoginPage", "PayTime");
                }
                else
                {
                    base.Session["SelectedProductId"] = 5;
                    base.Session["SelectedProductName"] = "FieldSens";
                    base.Session["SelectedProductCode"] = "FIELDSENS";
                    base.Session["CurrentApiUrl"] = ConfigurationManager.AppSettings["webapiFieldSens"];                  
                    result = base.View();
                }
            }
            catch (Exception)
            {
                result = base.RedirectToAction("ErrorPage", "PayTime");
            }
            return result;
        }

        public ActionResult CMSDashboard()
        {
            ActionResult result;
            try
            {
                bool flag = base.Session["tokan"] == null;
                if (flag)
                {
                    result = base.RedirectToAction("LoginPage", "PayTime");
                }
                else
                {
                    base.Session["SelectedProductId"] = 6;
                    base.Session["SelectedProductName"] = "Canteen Management System";
                    base.Session["SelectedProductCode"] = "CMS";
                    base.Session["CurrentApiUrl"] = ConfigurationManager.AppSettings["webapipaytime"];
                    result = base.View();
                }
            }
            catch (Exception)
            {
                result = base.RedirectToAction("ErrorPage", "PayTime");
            }
            return result;
        }

        public ActionResult VMSDashboard()
        {
            ActionResult result;
            try
            {
                bool flag = base.Session["tokan"] == null;
                if (flag)
                {
                    result = base.RedirectToAction("LoginPage", "PayTime");
                }
                else
                {
                    base.Session["SelectedProductId"] = 7;
                    base.Session["SelectedProductName"] = "Visitor Management";
                    base.Session["SelectedProductCode"] = "VMS";
                    base.Session["CurrentApiUrl"] = ConfigurationManager.AppSettings["webapipaytime"];
                    result = base.View();
                }
            }
            catch (Exception)
            {
                result = base.RedirectToAction("ErrorPage", "PayTime");
            }
            return result;
        }
    }
}