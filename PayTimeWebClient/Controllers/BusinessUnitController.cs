using Newtonsoft.Json;
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
    public class BusinessUnitController : Controller
    {
        static readonly TransactionDataRestClient RestClient = new TransactionDataRestClient();
        static readonly MasterDataRestClient MasterRestClient = new MasterDataRestClient();
        static readonly ESSRestClient ESSRestClient = new ESSRestClient();
        static readonly UtilitiesDataRestClient UtilitiesRestClient = new UtilitiesDataRestClient();

        // GET: BusinessUnit
        public ActionResult BUAssign()
        {
            if (Session["RoleId"] == null || Session["tokan"] == null)
            {
                return RedirectToAction("LoginPage", "PayTime");
            }
            return View();
        }

        public ActionResult BUApproval()
        {
            if (Session["RoleId"] == null || Session["tokan"] == null)
            {
                return RedirectToAction("LoginPage", "PayTime");
            }
            return View();
        }

        [HttpGet]
        public PartialViewResult BULeaveApproval()
        {
            return PartialView("BULeaveApproval");
        }

        [HttpGet]
        public PartialViewResult BUAttendanceApproval()
        {
            AttendanceCorrection obj = new AttendanceCorrection();
            obj.roleid = Convert.ToInt32(Session["RoleId"]);
            obj.EmpId = Convert.ToInt32(Session["EmpId"]);
            obj.cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
            obj.branchid = Session["BranchId"] == null ? 0 : Convert.ToInt32(Session["BranchId"].ToString());
            var data = ESSRestClient.GetAllAttendanceCorrection(obj);
            dynamic jsonResponse = JsonConvert.DeserializeObject(data);
            ViewBag.getallattnd = jsonResponse;
            return PartialView("BUAttendanceApproval");
        }

        [HttpGet]
        public PartialViewResult BUWebPunchApproval()
        {
            int roleid = Convert.ToInt32(Session["RoleId"]);
            ViewBag.Getsystemsettingdateformatupdated = RestClient.CompanySettingGetAll();
            return PartialView("BUWebPunchApproval");
        }

        public PartialViewResult BUFacePunchApproval()
        {
            return PartialView("BUFacePunchApproval");
        }

        public ActionResult BUEmployees()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            Employees emp = new Employees();
            try
            {
                if (Session["RoleId"].ToString() != "1")
                {
                    var cmpanyid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    ViewBag.Companyslist = MasterRestClient.CompanyGetAll().OrderBy(x => x.CompanyName).Where(x => x.CompanyID == cmpanyid);
                }
                else
                {
                    ViewBag.Companyslist = MasterRestClient.CompanyGetAll().OrderBy(x => x.CompanyName);
                }

                return View(emp);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
            finally
            {
                emp = null;
            }
        }

        public ActionResult BUAttendanceDetails()
        {
            ViewBag.LeaveTypeList = FillLeaveType();
            return View();
        }

        [HttpPost]
        public JsonResult GetAttendanceDetailsBU(string BUId, string loginemployeeid, string Month, string Year, string status, string Employee, int selectAll = 0, string selectAllSearchTerm = "")
        {
            JsonResult result;
            AttanList obj = new AttanList();
            obj.BUId = BUId;
            obj.loginemployeeid = loginemployeeid;
            obj.Month = Month;
            obj.Year = Year;
            obj.status = status;
            obj.Employee = Employee;
            obj.SelectAll = selectAll;
            obj.SelectAllSearchTerm = selectAllSearchTerm;
            var AttanList = UtilitiesRestClient.GetAttendanceDetailsBU(obj);
            result = Json(AttanList);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.MaxJsonLength = Int32.MaxValue;
            return result;
        }

        public List<SelectListItem> FillLeaveType()
        {
            LeaveTypeMaster lv = new LeaveTypeMaster();
            IEnumerable<LeaveTypeMaster> lvList;
            lvList = MasterRestClient.LeaveTypeGetAll();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var i in lvList)
            {
                list.Add(new SelectListItem() { Text = i.LeaveTypeName, Value = Convert.ToString(i.LeaveTypeId) });
            }
            return list;
        }
    }
}
