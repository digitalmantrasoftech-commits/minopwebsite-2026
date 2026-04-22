using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayTimeWebClient.Models;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.IO;
using Path = System.IO.Path;

namespace PayTimeWebClient.Controllers
{
    public class DashboardController : Controller
    {
        #region Declaration
      
        static readonly MasterDataRestClient RestClient = new MasterDataRestClient();
        static readonly TransactionDataRestClient TransactionRestClient = new TransactionDataRestClient();
        Paymentintegrationclient payclint = new Paymentintegrationclient();
        MRespo resp = new MRespo();
        #endregion
        //
        // GET: /Dashboard/
        public ActionResult Index()
        {
            return View();
        }
        #region For New admin Dashbord  
        [HttpGet]
        [CustAuthFilter]
        public ActionResult AdminDashboard()
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
        #endregion

        #region annoucements
        [HttpGet]
        [CustAuthFilter]
        public ActionResult Announcements()
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
        public ActionResult UploadPhoto(HttpPostedFileBase EmpPhotoFile)
        {
            if (EmpPhotoFile != null && EmpPhotoFile.ContentLength > 0)
            {
               
                string extension = Path.GetExtension(EmpPhotoFile.FileName);

                // Generate unique name
                string uniqueId = Guid.NewGuid().ToString(); // 36 characters

                // Calculate max length for original name part
                int maxNameLength = 100 - uniqueId.Length - extension.Length - 1; // -1 for underscore

                // Get original file name (without extension) and trim if needed
                string originalName = Path.GetFileNameWithoutExtension(EmpPhotoFile.FileName);
                if (originalName.Length > maxNameLength)
                {
                    originalName = originalName.Substring(0, maxNameLength);
                }

                // Final file name: original + _ + guid + extension
                string fileName = $"{originalName}_{uniqueId}{extension}";

                // Define save path
                string folderPath = Server.MapPath("~/assets/images/Annoucement");

                // Ensure folder exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Save the file
                string fullPath = Path.Combine(folderPath, fileName);
                EmpPhotoFile.SaveAs(fullPath);

                // Return relative path
                string relativePath = Url.Content("~/assets/images/Annoucement/" + fileName);
                return Json(new { imagePath = relativePath }, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(400, "No file uploaded.");
        }


        #endregion

        #region employeedashboard
        public List<SelectListItem> FillLeaveType()
        {
            LeaveTypeMaster lv = new LeaveTypeMaster();
            IEnumerable<LeaveTypeMaster> lvList;
            lvList = RestClient.LeaveTypeGetAll();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var i in lvList)
            {
                list.Add(new SelectListItem() { Text = i.LeaveTypeName, Value = Convert.ToString(i.LeaveTypeId) });
            }
            return list;
        }

        [HttpGet]
        [CustAuthFilter]
        public ActionResult EmployeeDashboard()
        {
            int empid = Convert.ToInt32(Session["EmpId"]);
            int Ishierchy = Convert.ToInt32(Session["Ishierchy"]);
            try
            {
                Employees emp = new Employees();
                var empgetall = RestClient.EmployeeGetAlls(Ishierchy);
                if (Convert.ToInt32(Session["RoleId"]) != 1 && Convert.ToInt32(Session["RoleId"]) != 6805 && Convert.ToInt32(Session["RoleId"]) != 6806)
                {
                    emp.EmployeeList = RestClient.EmployeeGet(Convert.ToInt32(Session["EmpId"]));
                    var policyId = emp.EmployeeList.Select(x => x.PolicyId).SingleOrDefault();
                    var policy = RestClient.HrPolicyGetbyID(policyId);
                    ViewBag.PolicyName = policy != null ? policy.PolicyName : "";
                }
                else if (Convert.ToInt32(Session["RoleId"]) == 1 && Convert.ToInt32(Session["EmpId"]) != 0)
                {
                    emp.EmployeeList = RestClient.EmployeeGet(Convert.ToInt32(Session["EmpId"]));
                    var policyId = emp.EmployeeList.Select(x => x.PolicyId).SingleOrDefault();
                    var policy = RestClient.HrPolicyGetbyID(policyId);
                    ViewBag.PolicyName = policy != null ? policy.PolicyName : "";
                }
                else if (Convert.ToInt32(Session["RoleId"]) == 6805 || Convert.ToInt32(Session["RoleId"]) == 6806)
                {
                    if (empgetall != null)
                    {
                        var firstemp = empgetall.FirstOrDefault();
                        emp.EmployeeList = new[] { firstemp };
                    }
                    else
                    {
                        emp.EmployeeList = RestClient.EmployeeGet(0);
                    }
                }
                else
                {
                    emp.EmployeeList = RestClient.EmployeeGet(Convert.ToInt32(Session["FirstEmpId"]));
                }
                if (empid > 0)
                {
                    ViewBag.allemp = TransactionRestClient.LeaveListbyEmployee(empid);
                }
                ViewBag.LeaveTypeList = FillLeaveType();
                ViewBag.Getsystemsettingdateformatupdated = TransactionRestClient.CompanySettingGetAll();
                var empdata = empgetall.Select(e => new { EmpName = e.EmpName, EmpId = e.EmpId });
                var jsonData = JsonConvert.SerializeObject(empdata);
                byte[] compressedData = CompressionUtils.Compress(jsonData);
                ViewBag.empdata = Convert.ToBase64String(compressedData);
                return View(emp);
            }
            catch (Exception ex)
            {
                string x = ex.ToString();
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        #endregion

        #region tpfeedback
        public ActionResult Feedbacks()
        {
            return View();
        }
        #endregion
    }
}