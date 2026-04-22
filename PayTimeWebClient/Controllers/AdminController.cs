using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayTimeWebClient.Models;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;

namespace PayTimeWebClient.Controllers
{
    public class AdminController : Controller
    {
        #region Declaration
        //static readonly IAccountRestClint RestAccout = new AccountRestClint();
        static readonly MasterDataRestClient RestClient = new MasterDataRestClient();
        //static readonly AlertDataRestClient accrestclient = new AlertDataRestClient();
        Paymentintegrationclient payclint = new Paymentintegrationclient();
        MRespo resp = new MRespo();
        #endregion
        //
        // GET: /AdminDashBoard/
        public ActionResult Index()
        {
            return View();
        }

        #region For New admin Dashbord Page added to Custimized changes FastTrack
        [HttpGet]
        [CustAuthFilter]
        public ActionResult AdminDashBoard()
        {
            try
            {

                if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                {
                    CompanySetting cms = new CompanySetting();
                    cms = RestClient.GetSystemSettingById(Convert.ToInt32(Session["ClientCompanyId"]));
                    if (Convert.ToInt32(Session["RoleId"].ToString()) > 1)
                    {
                        cms = RestClient.GetSystemSettingById(1);
                    }
                    if (cms.IsActive == true)
                    {
                        Session["IsSetting"] = cms.IsActive;
                        Session["HasGrade"] = cms.HasGrade; //For Bionic F7 device inclued
                    }

                    ViewBag.Companyslist = RestClient.CompanyGetAll();
                }
                else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                {
                    CompanySetting cms = new CompanySetting();
                    cms = RestClient.GetSystemSettingById(1);

                    if (cms.IsActive == true)
                    {
                        Session["HasGrade"] = cms.HasGrade;//For Bionic F7 device inclued
                    }
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    ViewBag.Companyslist = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                }
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }


        [HttpPost]
        [CustAuthFilter]
        public JsonResult AdminDashboardDetails_FastTrack(int type, string cmpId, string branchId, string fdate, string tdate, string DepartId, string DesigId, string ShiftId)
        {
            JsonResult result;
            try
            {
                if (cmpId == "")
                {
                    cmpId = "0";
                }
                if (branchId == "" || branchId == "null")
                {
                    branchId = "0";
                }
                if (DepartId == "" || DepartId == "null")
                {
                    DepartId = "0";
                }
                if (DesigId == "" || DesigId == "null")
                {
                    DesigId = "0";
                }
                if (ShiftId == "" || ShiftId == "null")
                {
                    ShiftId = "0";
                }
                var i = RestClient.GetallAdminDetails_FastTrack(type, cmpId, branchId, fdate, tdate, DepartId, DesigId, ShiftId);
                result = Json(i);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.MaxJsonLength = Int32.MaxValue;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        #endregion

        #region For MissPunch
        [HttpPost]
        [CustAuthFilter]
        public JsonResult AdminDashboardDetails_FastTrack_MissPunch(int type, string cmpId, string branchId, string fdate, string tdate, string DepartId, string DesigId, string ShiftId)
        {
            JsonResult result;
            try
            {
                if (cmpId == "")
                {
                    cmpId = "0";
                }
                if (branchId == "" || branchId == "null")
                {
                    branchId = "0";
                }
                if (DepartId == "" || DepartId == "null")
                {
                    DepartId = "0";
                }
                if (DesigId == "" || DesigId == "null")
                {
                    DesigId = "0";
                }
                if (ShiftId == "" || ShiftId == "null")
                {
                    ShiftId = "0";
                }
                var i = RestClient.GetallAdminDetails_FastTrack_MissPunch(type, cmpId, branchId, fdate, tdate, DepartId, DesigId, ShiftId);
                result = Json(i);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.MaxJsonLength = Int32.MaxValue;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        #endregion
    }
}