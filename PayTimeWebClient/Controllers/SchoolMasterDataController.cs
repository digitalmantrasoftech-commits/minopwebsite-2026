using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class SchoolMasterDataController : Controller
    {
        //
        // GET: /SchoolMasterData/
        MasterDataRestClient RestClient = new MasterDataRestClient();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult School()
        {
            return View();
        }
        public ActionResult StaffType()
        {
            return View();
        }
        public ActionResult Staff()
        {
            return View();
        }
        public ActionResult Class()
        {
            return View();
        }
        public ActionResult Division()
        {
            return View();
        }
        public ActionResult Parent()
        {
            return View();
        }
        public ActionResult Student()
        {
            ViewBag.HasSMS = Convert.ToBoolean(RestClient.GetSystemSettingById(1).HasSMS.ToString() != "" ? RestClient.GetSystemSettingById(1).HasSMS.ToString() : "0");
            return View();
        }
        public ActionResult Device()
        {
            Devices dvs = new Devices();
            var compcode = Session["cmpcode"].ToString();
            var cmpid=Convert.ToInt32(Session["ClientCompanyId"]);
            int[] ids = new List<string>(ConfigurationManager.AppSettings["lst_devicetypeid"].Split(',')).ConvertAll<int>(s => int.Parse(s)).ToArray();
            if (Session["RoleId"].ToString() == "6805")
            {
                dvs.DeviceList = RestClient.DeviceGetAllGrid(compcode).Where(x=>x.CompanyId==cmpid);
            }
            else
            {
                dvs.DeviceList = RestClient.DeviceGetAllGrid(compcode);
            }
            
            ViewBag.DeviceTypelst = RestClient.DeviceTypeGetAll().Where(item => ids.Contains(item.DeviceTypeCode));
            dvs.DeviceTypeList = RestClient.DeviceTypeGetAll();
            //ViewBag.CompanyList = FillCompany();
            return View(dvs);
        }
        [HttpPost]
        public ActionResult UploadParentphoto(string ParentPhoto, HttpPostedFileBase ParentPhotoFile)
        {
            string res = string.Empty;
            if (ParentPhotoFile != null)
            {

                var path = Path.Combine(Server.MapPath("~/UploadParentPhoto"), ParentPhoto);
                ParentPhotoFile.SaveAs(path);
            }
            return RedirectToAction("Parent");
        }

        [HttpPost]
        public ActionResult UploadDriverphoto(string DriverPhoto, HttpPostedFileBase DriverPhotoFile)
        {
            string res = string.Empty;
            if (DriverPhotoFile != null)
            {
                var path = Path.Combine(Server.MapPath("~/UploadDriverPhoto"), DriverPhoto);
                DriverPhotoFile.SaveAs(path);
            }
            return RedirectToAction("Driver");
        }

        [HttpPost]
        public ActionResult UploadStudentphoto(string StudentPhoto, HttpPostedFileBase StudentPhotoFile)
        {
            string res = string.Empty;
            if (StudentPhotoFile != null)
            {
                var path = Path.Combine(Server.MapPath("~/UploadEmpPhoto"), StudentPhoto);
                StudentPhotoFile.SaveAs(path);
            }
            return RedirectToAction("Student");
        }

        public ActionResult Trasactiondata()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Device(Devices dv)
        {
            //if (ModelState.IsValid)
            //{
            var compcode = Session["cmpcode"].ToString();
            var cmpid = dv.CompanyId!=0 ? dv.CompanyId : Convert.ToInt32(Session["ClientCompanyId"]);

            var branchgetall = RestClient.BranchGetAll();
            var branch = branchgetall.Where(x => x.CompanyID == cmpid).Select(x => x.BranchId).First();

            try
            {
                ViewBag.msg = "";
                ViewBag.error = "";
                int n;
                dv.IsSchool = true;
                bool isNumeric = int.TryParse(dv.DeviceCode, out n);
                if (isNumeric)
                {

                    dv.BranchId = branch;
                    if (dv.DeviceId > 0)
                    {
                        dv.IsActive = true;
                        dv.IsPushData = 1;
                        dv.RegCompanyCode = Session["cmpcode"].ToString();
                        var jsonSerialiser = new JavaScriptSerializer();
                        var json = jsonSerialiser.Serialize(dv);
                        ErrorMsg ms = new ErrorMsg();
                        ms = RestClient.DeviceUpdate(dv.DeviceId,dv);
                        if (ms.MegSts.ToLower() == "ok")
                        {
                            ViewBag.msg = ms.Meg;
                        }
                        else
                        {
                            ViewBag.error = ms.Meg;
                        }
                    }
                    else
                    {
                        //dv.DeviceList = RestClient.DeviceGetAll().Where(c => c.DeviceCode == dv.DeviceCode);
                        //if (dv.DeviceList.Count() > 0)
                        //{
                        //    ViewBag.error = dv.DeviceCode + " Device Code already exists.";
                        //}
                        //else
                        //{
                            //if (IsDeviceSrNo == "False")
                            //{
                            //    dv.DeviceSrNo = "Test" + Session["cmpcode"] + Guid.NewGuid().ToString().Substring(0, 4);
                            //}
                            dv.RegCompanyCode = Session["cmpcode"].ToString();
                            ErrorMsg ms = new ErrorMsg();
                            dv.IsPushData = 1;
                            ms = RestClient.DevicesAdd(dv);
                            if (ms.MegSts.ToLower() == "ok")
                            {
                                ViewBag.msg = ms.Meg;
                            }
                            else
                            {
                                ViewBag.error = ms.Meg;
                            }
                        //}
                    }
                }
                else
                {
                    ViewBag.error = dv.DeviceCode + " is not valid device code, Please enter numeric values.";
                }
                //}
                //else
                //{
                //    ViewBag.error = "Not Valid Entry";
                //}
                //var ip = GetDeviceDetails.GetIPAddress();
                //var systemid = GetDeviceDetails.GetSystemID();
                //var ServiceDetails = UtilitiesRestClient.GetDeviceServiceDetails(compcode);
                //bool whitelisted = ServiceDetails.Where(x => x.IpAddress == ip & x.SystemCode == systemid).Select(x => x.Iswhitelist).FirstOrDefault();
                //ViewBag.whitelisted = whitelisted;
                if (Session["RoleId"].ToString() == "6805")
                {
                    dv.DeviceList = RestClient.DeviceGetAllGrid(compcode).Where(x => x.CompanyId == cmpid);
                }
                else
                {
                    dv.DeviceList = RestClient.DeviceGetAllGrid(compcode);
                }
                dv.DeviceTypeList = RestClient.DeviceTypeGetAll();
                //int[] ids = { 1, 13, 21, 25, 26, 27, 28, 29, 31 };
                int[] ids = new List<string>(ConfigurationManager.AppSettings["lst_devicetypeid"].Split(',')).ConvertAll<int>(s => int.Parse(s)).ToArray();
                ViewBag.DeviceTypelst = RestClient.DeviceTypeGetAll().Where(item => ids.Contains(item.DeviceTypeCode));
                //ViewBag.CompanyList = FillCompany();
                // ViewBag.DeviceMasterList = RestClient.DeviceGetAll();
                return View(dv);

            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }



        }

        [HttpGet]
        public JsonResult GetDeviceSrCode(string DeviceSrNo, string DeviceCode)
        {
            if (DeviceSrNo == "")
            {
                DeviceSrNo = string.Empty;
            }
            if (DeviceCode == "")
            {
                DeviceCode = string.Empty;
            }
            JsonResult result;
            ResMsg emg = new ResMsg();
            emg = RestClient.SchoolCheckDeviceInfo(DeviceSrNo, DeviceCode);

            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(emg);
            result = Json(json);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        public List<SelectListItem> FillCompany()
        {
            Companys cm = new Companys();
            cm.Companyslist = RestClient.CompanyGetAll();
            List<SelectListItem> list = new List<SelectListItem>();
            //list.Add(new SelectListItem() { Text = "Select Company", Value = "0" });
            foreach (var i in cm.Companyslist)
            {
                list.Add(new SelectListItem() { Text = i.CompanyName, Value = Convert.ToString(i.CompanyID) });
            }
            return list;
        }

        public ActionResult Shift()
        {
            return View();
        }

        [HttpGet]
        public ActionResult HRPolicy()
        {
            ViewBag.msg=TempData["msg"];
            ViewBag.success = TempData["success"];
            ViewBag.error = TempData["error"];
            ViewBag.CompanyList = FillCompany();
            ViewBag.PolicyGetAll = RestClient.HRPolicyGetAll();
            return View();
        }

        [HttpPost]
        public ActionResult HRPolicy(HRPolicy hr)
        {
            ErrorMsg err = new ErrorMsg();
            if (hr.emptempweekoff != null)
            {
                hr.EmpSecondWeekOffRule = string.Join(",", hr.emptempweekoff);
            }
            else
            {
                hr.EmpSecondWeekOffRule = "0";
            }


            if (hr.emphalfdayoff != null)
            {
                hr.EmpHalfDayRule = string.Join(",", hr.emphalfdayoff);
            }
            else
            {
                hr.EmpHalfDayRule = "0";
            }
            TempData["msg"] = "";
            TempData["success"] = "";
            TempData["error"] = "";
            hr.CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            hr.CreatedBy = 1;
            hr.ModifyDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            hr.IsActive = 1;
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(hr);
            if (hr.PolicyId > 0)
            {
                hr.IsActive = 1;
                if (!RestClient.HRPolicyUpdate(hr.PolicyId, hr))
                {
                    ViewBag.msg = "Policy not updated due to service issue.";
                }
                else
                {
                    ViewBag.success = "true";
                    ViewBag.msg = "Policy is updated";
                }
            }
            else
            {
                hr.HRPolicylist = RestClient.HRPolicyGetAll().Where(c => c.PolicyName == hr.PolicyName);
                if (hr.HRPolicylist.Count() > 0)
                {
                    ViewBag.error = hr.PolicyName + " Policy already exists.";
                }
                else
                {

                    err = RestClient.HRPolicyAdd(hr);
                    if (err.MegSts.ToUpper() == "OK")
                    {
                        ViewBag.success = "true";
                        ViewBag.msg = "Policy Added Successfully.";
                    }
                    else
                    {
                        ViewBag.error = "Please Enter Proper Value.";
                    }

                }
            }
            ViewBag.CompanyList = FillCompany();
            ViewBag.PolicyGetAll = RestClient.HRPolicyGetAll();
            //return RedirectToAction("HRPolicy");
            return View();
        }

        /// <summary>
        /// Get Vehicle Tracking details on map
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Tracker()
        {
            return View();
        }
        /// <summary>
        /// Get Vehicle Details and register new vehicle.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Vehicle()
        {
            return View();
        }
        /// <summary>
        /// Get Driver Details and register new Driver.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Driver()
        {
            return View();
        }
        /// <summary>
        /// Get Vehicle Route and add Route.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult VehicleRoute()
        {
            return View();
        }
        /// <summary>
        /// Get Gate Details and register new Gate
        /// </summary>
        /// <returns></returns>
        public ActionResult Gate()
        {
            return View();
        }

        [HttpGet]
        public ActionResult VehicleRoutes()
        {
            return View();
        }

        [HttpGet]
        public ActionResult StudentTransfer()
        {
            return View();
        }

    }
}