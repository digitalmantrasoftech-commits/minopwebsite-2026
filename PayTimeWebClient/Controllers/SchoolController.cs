using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class SchoolController : Controller
    {
        static readonly MasterDataRestClient RestClient = new MasterDataRestClient();
        //
        // GET: /School/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AdminDashboard()
        {
            if (Session["RoleId"].ToString() != "6805")
            {
                CompanySetting cms = new CompanySetting();
                cms = RestClient.GetSystemSettingById(Convert.ToInt32(Session["ClientCompanyId"]));
                if (cms.IsActive == true)
                {
                    Session["IsSetting"] = cms.IsActive;
                }

                ViewBag.Companyslist = RestClient.CompanyGetAll();
            }
            else if (Session["RoleId"].ToString() == "6805")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                ViewBag.Companyslist = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
            }
            //string cmpid = "0";
            //string branchId = "0";
            //string fdate = DateTime.Now.ToString("yyyy-MM-dd");
            //ViewBag.AllCounter = RestClient.GetallAdminCounter(cmpid, branchId, fdate);
            
            return View();
        }
    }
}