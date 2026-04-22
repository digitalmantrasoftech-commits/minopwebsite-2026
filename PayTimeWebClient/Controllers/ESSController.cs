using Newtonsoft.Json;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Converters;
using System.Threading;
using System.Web;
using System.IO;
using System.Data;
namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class ESSController : Controller
    {
        #region Declaration
        static readonly TransactionDataRestClient RestClient = new TransactionDataRestClient();
        static readonly MasterDataRestClient MasterRestClient = new MasterDataRestClient();
        static readonly ESSRestClient ESSRestClient = new ESSRestClient();
        static readonly AlertDataRestClient AlertRestClient = new AlertDataRestClient();
        static readonly UtilitiesDataRestClient UtilitiesRestClient = new UtilitiesDataRestClient();
        #endregion

        #region HolidayList
        [HttpGet]
        public ActionResult HolidayList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetHolidays()
        {
            int _EmpId = 0;
            string _CompanyId = "0";
            _CompanyId = Session["ClientCompanyId"].ToString();
            _EmpId = Convert.ToInt32(Session["EmpId"]);
            MasterDataRestClient ac = new MasterDataRestClient();
            bool HasReligion = ac.CompanySettingGetAll().Select(x => x.HasReligion).SingleOrDefault();
            int _religionid = ac.EmployeeGet(_EmpId).Select(x => x.ReligionId).SingleOrDefault();
            string _branchid = ac.EmployeeGet(_EmpId).Select(x => x.BranchId.ToString()).SingleOrDefault();
            int _departmentid = ac.EmployeeGet(_EmpId).Select(x => x.DepartmentId).SingleOrDefault();
            int _countryid = string.IsNullOrEmpty(_branchid) ? 0 : ac.BranchGetAll().Where(x => x.BranchId.ToString().Contains(_branchid)).Select(x => x.CountryID).FirstOrDefault();
            int _cityid = string.IsNullOrEmpty(_branchid) ? 0 : ac.BranchGetAll().Where(x => x.BranchId.ToString().Contains(_branchid)).Select(x => x.CityId).FirstOrDefault();
            int _stateid = string.IsNullOrEmpty(_branchid) ? 0 : ac.BranchGetAll().Where(x => x.BranchId.ToString().Contains(_branchid)).Select(x => x.StateID).FirstOrDefault();

            Holidays cmp = new Holidays();
            try
            {
                if (HasReligion)
                {
                    cmp.Holidayslist = MasterRestClient.HolidaysGetAll().Where(c => c.IsActive == true && (c.CompanyID.Contains(_CompanyId) || c.CompanyID == "0") && (c.BranchId.Contains(_branchid ?? "0") || c.BranchId == "0") && (c.CountryId == _countryid || c.CountryId == 0) && (c.StateId == _stateid || c.StateId == 0) && (c.CityId == _cityid || c.CityId == 0) && (c.ReligionId.Contains(Convert.ToString(_religionid)) || c.ReligionId == "0"));
                }
                else
                {
                    cmp.Holidayslist = MasterRestClient.HolidaysGetAll().Where(c => c.IsActive == true && (c.CompanyID.Contains(_CompanyId) || c.CompanyID == "0") && (c.BranchId.Contains(_branchid) || c.BranchId == "0") && (c.CountryId == _countryid || c.CountryId == 0) && (c.StateId == _stateid || c.StateId == 0) && (c.CityId == _cityid || c.CityId == 0));
                }

                return Json(cmp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred." },
                     JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region WebPunch
        [HttpGet]
        public ActionResult WebPunch()
        {
            try
            {
                int empid = Convert.ToInt32(Session["EmpId"]);

                //var _timeZoneInfo = RestClient.GettimeZoneInfo(Convert.ToInt32(empid));
                var _timeZoneInfo = "India Standard Time";
                TimeZoneInfo timeZoneInfo;
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_timeZoneInfo);
                DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                var _datezone = Convert.ToString(dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                var _Timezone = Convert.ToString(dateTime.ToString("HH:mm:ss"));
                ViewBag.Datezone = _datezone;
                var _Hourzone = Convert.ToString(dateTime.ToString("tt"));
                ViewBag.Hourzone = _Hourzone;
                Session["Timezone"] = _timeZoneInfo;

                Webpunch MP = new Webpunch();
                MP.PunchList = RestClient.WebPunchGetByEmpid(empid).Where(c => (Convert.ToDateTime(c.Attn_Dt).ToString("yyyy-MM-dd") == Convert.ToDateTime(_datezone).ToString("yyyy-MM-dd") && c.AttnDt != null)).OrderBy(c => c.In_Out_Time);

                //ViewBag.Timezone = MasterRestClient.CountryTimeZoneAll();
                //MP.PunchList = RestClient.WebPunchGetByEmpid(empid).Where(c => (Convert.ToDateTime(c.Attn_Dt).ToString("yyyy-MM-dd") == DateTime.Today.ToString("yyyy-MM-dd"))).OrderBy(c => c.In_Out_Time);

                var _DisplayName = timeZoneInfo.DisplayName.ToString().Split(')');
                ViewBag.Timezone = _DisplayName[0].Replace("(", "").Replace("UTC", "");
                ViewBag.GetPunchbyEmployee = MP.PunchList;
                ViewBag.Getsystemsettingdateformatupdated = RestClient.CompanySettingGetAll();
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }
        //[HttpPost]
        //public JsonResult WebPunch(string WebPunch)
        //{
        //    JsonResult result;
        //    try
        //    {
        //        var Empdata = MasterRestClient.EmployeeGet(Convert.ToInt32(Session["EmpId"])).Select(c => new { c.EmpPunchID, c.Empcode }).SingleOrDefault();

        //        JavaScriptSerializer js = new JavaScriptSerializer();
        //        Webpunch objWebPunch = js.Deserialize<Webpunch>(WebPunch);
        //        objWebPunch.Punchid = Empdata.EmpPunchID;
        //        objWebPunch.Empcode = Empdata.Empcode;
        //        //string IP = getIP();  //HttpContext.Request.Params["HTTP_CLIENT_IP"] ?? HttpContext.Request.ServerVariables.;
        //        objWebPunch.IPaddress = RestClient.PublicIPAddress();//System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(1).ToString();
        //        string ad = "";
        //        ViewBag.msg = "";
        //        ViewBag.error = "";
        //        //objWebPunch.PunchType = 0;
        //        if (Convert.ToInt32(Session["IsApprovalForWebpunch"]) == 1)
        //        {
        //            objWebPunch.PunchType = 1;
        //            //objWebPunch.IsApprove = 1;
        //        }
        //        else
        //        {
        //            objWebPunch.PunchType = 0;
        //        }
        //        //obj.ShiftCode = 0;
        //        ad = RestClient.WebPunchAdd(objWebPunch);
        //        result = Json(ad);
        //        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        result = Json(new SelectList("", "0"));
        //        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //        return result;
        //    }
        //}
        [HttpPost]
        public JsonResult WebPunch(string WebPunch)
        {
            JsonResult result;
            try
            {
                var Empdata = MasterRestClient.EmployeeGet(Convert.ToInt32(Session["EmpId"])).Select(c => new { c.EmpPunchID, c.Empcode }).SingleOrDefault();

                JavaScriptSerializer js = new JavaScriptSerializer();
                Webpunch objWebPunch = js.Deserialize<Webpunch>(WebPunch);
                objWebPunch.Punchid = Empdata.EmpPunchID;
                objWebPunch.Empcode = Empdata.Empcode;
                //string IP = getIP();  //HttpContext.Request.Params["HTTP_CLIENT_IP"] ?? HttpContext.Request.ServerVariables.;
                objWebPunch.IPaddress = RestClient.PublicIPAddress();//System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(1).ToString();
                string ad = "";
                ViewBag.msg = "";
                ViewBag.error = "";
                //objWebPunch.PunchType = 0;
                if (Convert.ToInt32(Session["IsApprovalForWebpunch"]) == 1)
                {
                    objWebPunch.PunchType = 1;
                    //objWebPunch.IsApprove = 1;
                }
                else
                {
                    objWebPunch.PunchType = 0;
                }
                //obj.ShiftCode = 0;
                ad = RestClient.WebPunchAdd(objWebPunch);
                result = Json(ad);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        [HttpGet]
        public JsonResult WebPunchTimezoneInfo()
        {
            JsonResult result;
            TimeZoneInfo timeZoneInfo;
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(Session["Timezone"].ToString());
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            var _datezone = Convert.ToString(dateTime);
            var _Timezone = Convert.ToString(dateTime.ToString("HH:mm:ss"));
            var _Hourzone = Convert.ToString(dateTime.ToString("tt"));
            ViewBag.Datezone = _datezone;
            ViewBag.TimeZone = _Timezone;
            ViewBag.Hourzone = _Hourzone;
            //var _dtzone = Convert.ToDateTime(_datezone).ToString("yyyy-MM-dd HH:mm:ss tt");
            var _dtzone = Convert.ToDateTime(_datezone).ToString("yyyy-MM-dd HH:mm:ss");
            result = Json(_dtzone);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
            //return _Timezone.ToString;
        }
        public string GetPunchMode(string id)
        {
            string str = "";
            try
            {

                //Webpunch mp = new Webpunch();
                var j = RestClient.Getwebpunchmode(Convert.ToInt32(id));
                str = j;
                //if (j.Count() != 0)
                //{
                //    // var i = RestClient.PunchDataGetbyempid(Convert.ToInt32(empid)).OrderBy(c => c.tmpDmpid).LastOrDefault();
                //    var todaydate = DateTime.Now.ToString("yyyy-MM-dd");
                //    var ii = RestClient.WebPunchGetByEmpid(Convert.ToInt32(id)).OrderBy(c => c.WebpunchId).Where(c => Convert.ToDateTime(c.In_Out_Time).ToString("yyyy-MM-dd") == todaydate).LastOrDefault();
                //    //customers.Where(c => SqlMethods.Like(c.Name, "%john%"));
                //    //.Where(a => Regex.IsMatch(a.YouColumn, ".*VALUE.*")).ToList();

                //    if (ii != null)
                //    {
                //        if (ii.RecDet > 0)
                //        {

                //             if (ii.EntryMode == -1)
                //             {
                //                 todaydate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
                //                 ii = RestClient.WebPunchGetByEmpid(Convert.ToInt32(id)).OrderBy(c => c.WebpunchId).Where(c => Convert.ToDateTime(c.In_Out_Time).ToString("yyyy-MM-dd") == todaydate).LastOrDefault();
                //             }
                //             else
                //             {

                //                 ii = RestClient.WebPunchGetByEmpid(Convert.ToInt32(id)).OrderBy(c => c.WebpunchId).Where(c => Convert.ToDateTime(c.In_Out_Time).ToString("yyyy-MM-dd") == todaydate).LastOrDefault();
                //             }
                //        }
                //        else
                //        {
                //            ii = RestClient.WebPunchGetByEmpid(Convert.ToInt32(id)).OrderBy(c => c.WebpunchId).Where(c => Convert.ToDateTime(c.In_Out_Time).ToString("yyyy-MM-dd") == todaydate).LastOrDefault();
                //        }

                //        str = ii.Mode;
                //    }
                //    else
                //    {
                //        str = "No data Found";
                //    }

                //}
                //else
                //{
                //    str = "No data Found";
                //}
                return str;

            }
            catch (Exception)
            {
                str = "/PayTime/ErrorPage";
                return str;
            }


        }
        [HttpGet]
        public ActionResult WebPunchApprove()
        {
            try
            {
                int roleid = Convert.ToInt32(Session["RoleId"]);
                ViewBag.Getsystemsettingdateformatupdated = RestClient.CompanySettingGetAll();
                //var data = RestClient.GetWebpunchApprovelist(roleid);
                //dynamic jsonResponse = JsonConvert.DeserializeObject(data);
                //ViewBag.GetWebPunchlist = jsonResponse;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [HttpPost]
        public JsonResult webpunchAjax(DataTableWebPunchAjaxPostModel model)
        {
            model.roleid = Convert.ToInt32(Session["RoleId"]);
            model.empid = Convert.ToInt32(Session["EmpId"]);
            if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                model.cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                model.branchid = Convert.ToInt32(Session["BranchId"].ToString());
            }
            var AllData = RestClient.GetWebpunchApprovelistajax(model).Select(i => new { i.WebpunchId, i.Empid, i.EmpName, i.Attn_Dt, i.In_Out_Time, i.Mode, i.LocAddress, i.IsApprove, i.Reason });
            DatatableCounts DataCount = RestClient.GetWebpunchApprovelistCounts(model);
            var result = new List<Webpunch>(AllData.Count());
            foreach (var data in AllData)
            {
                result.Add(new Webpunch
                {
                    WebpunchId = data.WebpunchId,
                    Empid = data.Empid,
                    EmpName = data.EmpName,
                    Attn_Dt = data.Attn_Dt,
                    In_Out_Time = data.In_Out_Time,
                    Mode = data.Mode,
                    LocAddress = data.LocAddress,
                    IsApprove = data.IsApprove,
                    Reason = data.Reason
                });
            }
            return Json(new
            {
                // this is what datatables wants sending back
                draw = model.draw,
                recordsTotal = DataCount.totalResultsCount,
                recordsFiltered = DataCount.filteredResultsCount,
                data = result
            });
        }
        [HttpPost]
        public JsonResult webpunchAjaxFilter(DataTableWebPunchAjaxPostModel model, string FilterFromDate, string FilterToDate, int selectstatus, int Isheararchy)
        {
            model.roleid = Convert.ToInt32(Session["RoleId"]);
            model.empid = Convert.ToInt32(Session["EmpId"]);
            if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                model.cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                model.branchid = Convert.ToInt32(Session["BranchId"].ToString());
            }
            var AllData = RestClient.GetWebpunchApprovelistajaxFilter(model, FilterFromDate, FilterToDate, selectstatus, Isheararchy).Select(i => new { i.WebpunchId, i.EmpPhoto, i.Empid, i.EmpName, i.Attn_Dt, i.In_Out_Time, i.Mode, i.LocAddress, i.ActionBy, i.IsApprove, i.Reason, i.Empcode, i.ApprovalDate, i.ReportingName });
            DatatableCounts DataCount = RestClient.GetWebpunchApprovelistCountsFilter(model, FilterFromDate, FilterToDate, selectstatus, Isheararchy);
            var result = new List<Webpunch>(AllData.Count());
            foreach (var data in AllData)
            {
                result.Add(new Webpunch
                {
                    WebpunchId = data.WebpunchId,
                    Empid = data.Empid,
                    EmpName = data.EmpName,
                    Attn_Dt = data.Attn_Dt,
                    In_Out_Time = data.In_Out_Time,
                    Mode = data.Mode,
                    LocAddress = data.LocAddress,
                    IsApprove = data.IsApprove,
                    Reason = data.Reason,
                    Empcode = data.Empcode,
                    ApprovalDate = data.ApprovalDate,
                    ReportingName = data.ReportingName,
                    ActionBy = data.ActionBy,
                    EmpPhoto = data.EmpPhoto
                });


            }
            return Json(new
            {
                // this is what datatables wants sending back
                draw = model.draw,
                recordsTotal = DataCount.totalResultsCount,
                recordsFiltered = DataCount.filteredResultsCount,
                data = result
            });
        }
        [HttpPost]
        public ActionResult WebPunchApprove(int Webpunchid, int IsApprove)
        {
            try
            {
                WebpunchApprove wa = new WebpunchApprove();
                wa.ApprovalbyRole = Convert.ToInt32(Session["RoleId"]);
                wa.WebpunchId = Webpunchid;
                wa.IsApprove = IsApprove;
                var data = RestClient.WebPunchApprove(wa);
                return RedirectToAction("WebPunchApprove");
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        public string checkSession()
        {
            string str = "";
            try
            {
                return "1";

            }
            catch (Exception)
            {
                str = "/PayTime/LoginPage";
                return str;
            }


        }

        #endregion

        #region Attendance
        public ActionResult Attendance()
        {
            try
            {
                AttendanceCorrection obj = new AttendanceCorrection();
                obj.roleid = Convert.ToInt32(Session["RoleId"]);
                obj.EmpId = Convert.ToInt32(Session["EmpId"]);
                obj.cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                obj.branchid = Session["BranchId"] == null ? 0 : Convert.ToInt32(Session["BranchId"].ToString());
                var data = ESSRestClient.GetAllAttendanceCorrection(obj);
                dynamic jsonResponse = JsonConvert.DeserializeObject(data);
                ViewBag.getallattnd = jsonResponse;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }

        [HttpPost]
        public ActionResult Attendance(AttendanceCorrection attndcrr)
        {
            try
            {
                // attndcrr.attn_dt = Convert.ToDateTime(attndcrr.InPunchTime).ToString("yyyy-MM-dd");
                attndcrr.ModifyDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
                attndcrr.ModifyBy = attndcrr.EmpId;
                var data = ESSRestClient.AttendanceCorrectionUpdate(attndcrr);
                return RedirectToAction("Attendance", "ESS");
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        [HttpPost]
        public JsonResult AttendanceFilter(string FilterFromDate, string FilterToDate, int _IsStatus)
        {
            AttendanceCorrection obj = new AttendanceCorrection();
            obj.roleid = Convert.ToInt32(Session["RoleId"]);
            obj.EmpId = Convert.ToInt32(Session["EmpId"]);
            obj.cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
            obj.branchid = Session["BranchId"] == null ? 0 : Convert.ToInt32(Session["BranchId"].ToString());
            obj.FilterFromDate = FilterFromDate;
            obj.FilterToDate = FilterToDate;
            obj._IsStatus = _IsStatus;
            var AllData = ESSRestClient.GetAllAttendanceCorrection(obj);
            var result = Json(AllData);
            return result;
        }

        [HttpPost]
        public JsonResult AttendanceCorrectionUpdatereq(List<AttendanceCorrectionreq> objCorrection)
        {
            JsonResult result;
            string res = string.Empty;
            var data = "";
            for (int i = 0; i < objCorrection.Count; i++)
            {
                AttendanceCorrection attndcrr = new AttendanceCorrection();
                attndcrr.AttCorrectionId = objCorrection[i].AttCorrectionId;
                attndcrr.EmpId = objCorrection[i].EmpId;
                attndcrr.attn_dt = objCorrection[i].attn_dt;
                attndcrr.InPunchTime = objCorrection[i].InPunchTime;
                attndcrr.OutPunchTime = objCorrection[i].OutPunchTime;
                attndcrr.ApplyReason = objCorrection[i].ApplyReason;
                attndcrr.ApprovalReason = string.IsNullOrEmpty(objCorrection[i].ApprovalReason) ? "" : objCorrection[i].ApprovalReason;

                attndcrr.Status = objCorrection[i].Status;
                attndcrr.ModifyDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
                attndcrr.ModifyBy = attndcrr.EmpId;
                data = ESSRestClient.AttendanceCorrectionUpdate(attndcrr);
            }
            //make Changes
            if (data == "OK")
            {
                res = "Ok";
                result = Json(new { ad = res.ToLower() });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            else
            {

                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult AttendanceCorrection()
        {
            try
            {
                int empid = Convert.ToInt32(Session["EmpId"]);
                var data = ESSRestClient.GetAppliedAttendanceCorrectionList(empid);
                dynamic jsonRes = JsonConvert.DeserializeObject(data);
                ViewBag.getallappliedattnd = jsonRes;
                ViewBag.msg = null;
                ViewBag.Getsystemsettingdateformatupdated = RestClient.CompanySettingGetAll();
                if (TempData["Msg"] != null)
                {
                    ViewBag.msg = TempData["Msg"].ToString();
                }
                if (TempData["error"] != null)
                {
                    ViewBag.error = TempData["error"].ToString();
                }
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }

        [HttpPost]
        public JsonResult AttendanceCorrectionAdd(string CorrectionDetails)
        {
            try
            {
                JsonResult result;
                string ad = "";
                string adMessage = "";
                string meg = "";
                int OnCorrectionID = 0;
                //JavaScriptSerializer js = new JavaScriptSerializer();
                var dynamicDateFormat = Session["IsDateFormat"].ToString();
                var isDateformat = "";
                if (dynamicDateFormat == "dd-M-yyyy" || dynamicDateFormat == "M-dd-yyyy" || dynamicDateFormat == "yyyy-M-dd")
                {
                    isDateformat = Session["IsDateFormat"].ToString().Replace("M", "MMM");
                }
                else
                {
                    isDateformat = Session["IsDateFormat"].ToString().Replace("mm", "MM");
                }
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = isDateformat.ToString() };
                AttendanceCorrection objCorrection = JsonConvert.DeserializeObject<AttendanceCorrection>(CorrectionDetails, dateTimeConverter);
                //var jsonSerialiser = new JavaScriptSerializer();
                //var crrcdetails = jsonSerialiser.Serialize(CorrectionDetails);
                objCorrection.InPunchTime = objCorrection.InPunchTime;
                objCorrection.OutPunchTime = objCorrection.OutPunchTime;
                objCorrection.CreatedDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
                objCorrection.ModifyDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
                objCorrection.AttCorrectionId = objCorrection.AttendanceId;
                objCorrection.ModifyBy = objCorrection.EmpId;
                AttedanceResponse adResponse = ESSRestClient.AttendanceCorrectionCreate(objCorrection);
                //var resp = JsonConvert.DeserializeObject<MRespo>(i);
                if (adResponse.Message == "ok")
                {
                    TempData["Msg"] = adResponse.Meg;//resp.Meg;
                    adMessage = adResponse.Message;//resp.Meg;
                    meg = adResponse.Meg;//resp.Meg;
                    OnCorrectionID = adResponse.OnCorrectionID;
                }
                else
                {
                    TempData["error"] = adResponse.Meg; //resp.Meg;
                    adMessage = adResponse.Message; //resp.Meg;
                    meg = adResponse.Meg;//resp.Meg;
                    OnCorrectionID = adResponse.OnCorrectionID;
                }
                return Json(new { message = adMessage, Meg = meg, OnCorrectionID = OnCorrectionID }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                string str = "/PayTime/ErrorPage";
                return Json(new { message = "Error", Meg = "", OnCorrectionID = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Leave
        [HttpGet]
        public ActionResult LeaveRequest()
        {
            int empid = Convert.ToInt32(Session["EmpId"]);
            try
            {
                if (empid > 0)
                {
                    ViewBag.allemp = RestClient.LeaveListbyEmployee(empid);
                }
                //else
                //{
                //    ViewBag.allemp = "No Leave Applied Yet.";
                //}
                ViewBag.LeaveTypeList = FillLeaveType();
                ViewBag.Getsystemsettingdateformatupdated = RestClient.CompanySettingGetAll();
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }
        [HttpGet]
        public ActionResult ApproveLeave()
        {
            try
            {

                int roleid = Convert.ToInt32(Session["RoleId"]);
                int empid = Convert.ToInt32(Session["EmpId"]);
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [HttpPost]
        public JsonResult ApproveLeaveAjax(DataTableLeaveAprrovejaxPostModel model, string filterFromDate, string filterToDate, int selectedStatus, string Employeeids, int hierarchyWise, int selectAll, string selectAllSearchTerm)
        {
            model.roleid = Convert.ToInt32(Session["RoleId"]);
            model.empid = Convert.ToInt32(Session["EmpId"]);
            model.filterFromDate = filterFromDate;
            model.filterToDate = filterToDate;
            model.selectedStatus = selectedStatus;
            model.Employeeids = Employeeids;
            model.selectAll = selectAll;
            model.selectAllSearchTerm = selectAllSearchTerm;

            // int hierarchyWise = Session["Ishierchy"] != null ? Convert.ToInt32(Session["Ishierchy"]) : 0;

            if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                model.cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                model.branchid = Convert.ToInt32(Session["BranchId"].ToString());
            }

            var AllData = ESSRestClient.GetAllLeaveRequestlistajax(model, hierarchyWise).Select(i => new { i.LeaveId, i.EmpId, i.EmpPhoto, i.EmpName, i.LeaveTypeId, i.FromDate, i.ToDate, i.ApplyReason, i.LeaveStatus, i.LeaveStatusName, i.ApprovalReason, i.ActionBy, i.Created, i.CreatedDates, i.LeavePaid, i.IsHalfLeave, i.LeaveTypeName, i.EmpCode, i.EmpEmail, i.ReportingEmail, i.CompanyEmail, i.ApprovalDate, i.ReportingName, i.LeaveDoc, i.LocAddress });
            DatatableCounts DataCount = ESSRestClient.GetAllLeaveRequestlistCounts(model, hierarchyWise);
            var result = new List<LeaveList>(AllData.Count());
            foreach (var data in AllData)
            {
                result.Add(new LeaveList
                {
                    LeaveId = data.LeaveId,
                    EmpId = data.EmpId,
                    EmpPhoto = data.EmpPhoto,
                    EmpName = data.EmpName,
                    FromDate = data.FromDate,
                    ToDate = data.ToDate,
                    LeaveTypeId = data.LeaveTypeId,
                    LeaveTypeName = data.LeaveTypeName,
                    ApplyReason = data.ApplyReason,
                    LeaveStatusName = data.LeaveStatusName,
                    ApprovalReason = data.ApprovalReason,
                    ActionBy = data.ActionBy,
                    Created = data.Created,
                    CreatedDates = data.CreatedDates,
                    LeavePaid = data.LeavePaid,
                    IsHalfLeave = data.IsHalfLeave,
                    LeaveStatus = data.LeaveStatus,
                    EmpCode = data.EmpCode,
                    EmpEmail = data.EmpEmail,
                    ReportingEmail = data.ReportingEmail,
                    CompanyEmail = data.CompanyEmail,
                    ApprovalDate = data.ApprovalDate,
                    ReportingName = data.ReportingName,
                    LeaveDoc = data.LeaveDoc,
                    LocAddress = data.LocAddress

                });
            }
            return Json(new
            {
                // this is what datatables wants sending back
                draw = model.draw,
                recordsTotal = DataCount.totalResultsCount,
                recordsFiltered = DataCount.filteredResultsCount,
                data = result
            });
        }
        [HttpPost]
        public JsonResult ApproveLeave(LeaveList lst)
        {
            string res = string.Empty;
            JsonResult result;
            try
            {
                int roleid = Convert.ToInt32(Session["RoleId"]);
                int empid = Convert.ToInt32(Session["EmpId"]);
                var LeaveStatusName = string.Empty;
                if (lst.LeaveStatus == 1)
                {
                    LeaveStatusName = "Approved";
                }
                if (lst.LeaveStatus == 2)
                {
                    LeaveStatusName = "Rejected";
                }
                if (lst.LeaveStatus == 4)
                {
                    LeaveStatusName = "Canceled";
                }
                string companyHead = lst.CompanyEmail;
                //string companyHead = MasterRestClient.CompanyGetAll().Where(x => x.CompanyCode == Convert.ToString(Session["cmpcode"])).Select(x => x.CompanyEmail).FirstOrDefault();
                //lst.LeaveTypeName = MasterRestClient.LeaveTypeGetAll().Where(c => c.LeaveTypeId == lst.LeaveTypeId).Select(x => x.LeaveTypeName).FirstOrDefault();
                //lst.EmpName = MasterRestClient.EmployeeGetAll().Where(c => c.EmpId == lst.EmpId).Select(x => x.EmpName).FirstOrDefault();
                if (lst.LeaveId > 0)
                {
                    if (!ESSRestClient.LeaveApprovalUpdate(lst.LeaveId, lst))
                    {
                        ViewBag.msg = "Leave approval not updated due to service issue.";
                    }
                    else
                    {
                        ViewBag.msg = "Leave approval is updated.";
                        ////var reportingToEmail = (from emp1 in MasterRestClient.EmployeeGetAll() join emp2 in MasterRestClient.EmployeeGetAll()
                        ////                        on emp1.ReportingTo equals emp2.EmpId where emp1.EmpId == lst.EmpId select emp2.Email).FirstOrDefault();
                        ////var empEmail = MasterRestClient.EmployeeGetAll().Where(x => x.EmpId == lst.EmpId).Select(c => c.Email).FirstOrDefault();

                        //var reportingToEmail = lst.ReportingEmail;
                        //var empEmail = lst.EmpEmail;
                        //MailDetails maildetails = new MailDetails();
                        //if (reportingToEmail != null)
                        //{
                        //    maildetails.ToEmail = empEmail + "," + companyHead + "," + reportingToEmail;
                        //}
                        //else
                        //{
                        //    maildetails.ToEmail = empEmail + "," + companyHead;
                        //}
                        //maildetails.MailBody = "<Html><Head></head><body><p>Hello " + lst.EmpName + ",</p><p>Leave applied is " + LeaveStatusName + ".</p><p><table style='width: 258px; height: 41px;' border='2'><tbody><tr style='height: 21px;'><td style='width: 128px; height: 21px;'>Leave Type</td><td style='width: 128px; height: 21px;'>" + lst.LeaveTypeName + "</td></tr><tr style='height: 21px;'><td style='width: 128px; height: 21px;'>Leave From</td><td style='width: 128px; height: 21px;'>" + lst.FromDate + "</td></tr><tr style='height: 21px;'><td style='width: 128px; height: 21px;'>Leave To</td><td style='width: 128px; height: 21px;'>" + lst.ToDate + "</td></tr><tr style='height: 21px;'><td style='width: 128px; height: 21px;'>Leave status</td><td style='width: 128px; height: 21px;'>" + LeaveStatusName + "</td></tr><tr style='height: 21px;'><td style='width: 128px; height: 21px;'>Reason</td><td style='width: 128px; height: 21px;'>" + lst.ApprovalReason + "</td></tr></tbody></table></p></br>Regards,</br>Mantra</body></Html>";
                        //maildetails.Subject = "Leave Request " + LeaveStatusName;
                        //ESSRestClient.SendLeaveMail(maildetails);
                    }
                }
                else
                {
                    ViewBag.msg = "Oops! Something wrong, Try Again...";
                }
                //ViewBag.Leaveapproval = ESSRestClient.GetAllLeaveRequest(roleid, empid);
                //Make Changes 
                res = "Ok";
                result = Json(new { ad = res.ToLower() });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
                //return View();
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
                //return RedirectToAction("ErrorPage", "PayTime");
            }


        }

        [HttpPost]
        public JsonResult ApproveLeaveUpdate(List<LeaveList> lst)
        {
            JsonResult result;
            string res = string.Empty;
            bool data = false;
            for (int i = 0; i < lst.Count; i++)
            {
                string companyHead = lst[i].CompanyEmail;
                var LeaveStatusName = string.Empty;
                if (lst[i].LeaveStatus == 1)
                {
                    LeaveStatusName = "Approved";
                }
                if (lst[i].LeaveStatus == 2)
                {
                    LeaveStatusName = "Rejected";
                }
                LeaveList objleave = new LeaveList();
                objleave.LeaveId = lst[i].LeaveId;
                objleave.EmpId = lst[i].EmpId;
                objleave.FromDate = lst[i].FromDate;
                objleave.ToDate = lst[i].ToDate;
                objleave.ApplyReason = lst[i].ApplyReason;
                objleave.LeaveTypeId = lst[i].LeaveTypeId;
                objleave.LeavePaid = lst[i].LeavePaid;
                objleave.LeaveStatus = lst[i].LeaveStatus;
                objleave.ApprovalReason = lst[i].ApprovalReason;
                objleave.IsHalfLeave = lst[i].IsHalfLeave;
                objleave.ModifyDate = Convert.ToDateTime(DateTime.Now.Date.ToString("yyyy-MM-dd"));
                objleave.ModifyBy = lst[i].EmpId;
                objleave.LeaveDoc = lst[i].LeaveDoc;
                data = ESSRestClient.LeaveApprovalUpdate(objleave.LeaveId, objleave);
                if (data == true)
                {
                    ViewBag.msg = "Leave approval is updated.";
                }
            }
            res = "Ok";
            result = Json(new { ad = res.ToLower() });
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        [HttpPost]
        public JsonResult LeaveCreate(string OnDutyLeave)
        {
            try
            {
                //JavaScriptSerializer js = new JavaScriptSerializer();
                var dynamicDateFormat = Session["IsDateFormat"].ToString();
                var dateformat = "";
                if (dynamicDateFormat == "dd-M-yyyy" || dynamicDateFormat == "M-dd-yyyy" || dynamicDateFormat == "yyyy-M-dd")
                {
                    dateformat = Session["IsDateFormat"].ToString().Replace("M", "MMM");
                }
                else
                {
                    dateformat = Session["IsDateFormat"].ToString().Replace("mm", "MM");
                }
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = dateformat.ToString() };
                LeaveList objOnDutyLeave = JsonConvert.DeserializeObject<LeaveList>(OnDutyLeave, dateTimeConverter);
                objOnDutyLeave.EmpName = MasterRestClient.EmployeeGetAll().Where(c => c.EmpId == objOnDutyLeave.EmpId).Select(x => x.EmpName).FirstOrDefault();
                objOnDutyLeave.LeaveTypeName = MasterRestClient.LeaveTypeGetAll().Where(c => c.LeaveTypeId == objOnDutyLeave.LeaveTypeId).Select(x => x.LeaveTypeName).FirstOrDefault();
                string companyHead = MasterRestClient.CompanyGetAll().Where(x => x.CompanyCode == Convert.ToString(Session["cmpcode"])).Select(x => x.CompanyEmail).FirstOrDefault();

                string ad = "";
                string adMessage = "";
                string meg = "";
                int leaveId = 0;

                string _weburlminop = ConfigurationManager.AppSettings["weburlminop"];
                ViewBag.msg = "";
                ViewBag.error = "";
                if (objOnDutyLeave.LeaveId > 0)
                {

                    var res = RestClient.OnDutyLeaveGetAllCheck(Convert.ToString(objOnDutyLeave.FromDate), Convert.ToString(objOnDutyLeave.ToDate), objOnDutyLeave.EmpId, objOnDutyLeave.IsHalfLeave, objOnDutyLeave.LeaveTypeId, objOnDutyLeave.LeavePaid);
                    if (res.MegSts == "OK")
                    {

                    }
                    else if (res.MegSts == "error")
                    {
                        ad = res.Meg;
                    }
                    else
                    {
                        ad = res.Meg;
                    }
                }
                else
                {
                    var res = RestClient.OnDutyLeaveGetAllCheck(Convert.ToString(objOnDutyLeave.FromDate), Convert.ToString(objOnDutyLeave.ToDate), objOnDutyLeave.EmpId, objOnDutyLeave.IsHalfLeave, objOnDutyLeave.LeaveTypeId, objOnDutyLeave.LeavePaid);
                    //if (AllData == null)
                    if (res.MegSts == "OK")
                    {
                        LeaveResponse adResponse = RestClient.LeaveRequestAdd(objOnDutyLeave);
                        string reportingToEmail = string.Empty;
                        if (adResponse.Message == "ok")
                        {
                            adMessage = adResponse.Message;
                            leaveId = adResponse.LeaveId;
                            meg = adResponse.Meg;//resp.Meg;
                        }
                        else
                        {
                            adMessage = res.Meg;
                            meg = adResponse.Meg;//resp.Meg;
                        }
                    }
                    else if (res.MegSts == "error")
                    {
                        adMessage = res.Meg;
                        meg = res.Meg;//resp.Meg;
                    }
                    else
                    {
                        adMessage = res.Meg;
                        meg = res.Meg;//resp.Meg;
                    }
                }
                return Json(new { message = adMessage, Meg = meg, leaveId = leaveId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                string str = "/PayTime/ErrorPage";
                return Json(new { message = "Error", Meg = "", leaveId = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadLeaveRequestDocument(HttpPostedFileBase UploadLeaveDoc)
        {
            string res = string.Empty;
            if (UploadLeaveDoc.ContentLength > 0)
            {
                string fileName = Path.GetFileName(UploadLeaveDoc.FileName);
                var path = Path.Combine(Server.MapPath("~/UploadLeaveDoc"), fileName);
                UploadLeaveDoc.SaveAs(path);
            }
            return RedirectToAction("LeaveRequest");
        }
        #endregion

        #region ResetPassword
        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        [CustAuthFilter]
        public ActionResult ResetPassword(string Password, string NewPassword, string UserEmail, int EmpId, int RoleId)
        {
            try
            {
                MRespo resp = new MRespo();
                resp = AccountRestClint.EmployeeResetPassword(Password, NewPassword, UserEmail, EmpId, RoleId);
                if (resp.MegSts.ToLower() == "ok")
                {
                    ViewBag.msg = resp.Meg;
                }
                else
                {
                    ViewBag.error = resp.Meg;
                }
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        #endregion

        #region Functions
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

        #endregion

        #region My Policy
        public ActionResult MyPolicy()
        {
            try
            {
                int empid = Convert.ToInt32(Session["EmpId"]);
                int Policyid = MasterRestClient.EmployeeGet(empid).Select(x => x.PolicyId).SingleOrDefault();
                HRPolicy hrp = new HRPolicy();
                ViewBag.policydata = MasterRestClient.SampleHrPolicyGetbyID(Policyid);
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        #endregion


        #region EmpPhotoVerify
        [HttpPost]
        public JsonResult ISPhotoVerifyData(List<reqEmployeePhoto> lst)
        {
            JsonResult result;
            string res = string.Empty;
            var data = "";
            for (int i = 0; i < lst.Count; i++)
            {
                reqEmployeePhoto objEmpphoto = new reqEmployeePhoto();
                objEmpphoto.EmpId = lst[i].EmpId;
                objEmpphoto.EmpPhoto = lst[i].EmpPhoto;
                //string filePath = Server.MapPath("~/UploadEmpPhoto/" + objEmpphoto.EmpPhoto);
                //byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
                //string base64String = Convert.ToBase64String(imageBytes);
                //objEmpphoto.EmpPhoto = base64String;
                objEmpphoto.IsApprove = lst[i].IsApprove;
                objEmpphoto.Email = lst[i].Email;
                objEmpphoto.EmpName = lst[i].EmpName;
                data = ESSRestClient.IsPhotoVerify(objEmpphoto);
            }

            res = "Ok";
            result = Json(new { ad = res.ToLower() });
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        #endregion
        [HttpGet]
        public ActionResult AttendanceRegularization()
        {
            return View();
        }

        [HttpGet]

        public PartialViewResult AttendanceApproval()
        {
            AttendanceCorrection obj = new AttendanceCorrection();
            obj.roleid = Convert.ToInt32(Session["RoleId"]);
            obj.EmpId = Convert.ToInt32(Session["EmpId"]);
            obj.cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
            obj.branchid = Session["BranchId"] == null ? 0 : Convert.ToInt32(Session["BranchId"].ToString());
            var data = ESSRestClient.GetAllAttendanceCorrection(obj);
            dynamic jsonResponse = JsonConvert.DeserializeObject(data);
            ViewBag.getallattnd = jsonResponse;
            return PartialView("AttendanceApproval");
        }
        [HttpGet]

        public PartialViewResult LeaveApproval()
        {
            return PartialView("LeaveApproval");
        }
        [HttpGet]

        public PartialViewResult WebPunchApproval()
        {
            int roleid = Convert.ToInt32(Session["RoleId"]);
            ViewBag.Getsystemsettingdateformatupdated = RestClient.CompanySettingGetAll();
            return PartialView("WebPunchApproval");
        }

        public PartialViewResult FacePunchApproval()
        {
            return PartialView("FacePunchApproval");
        }

        #region Employee Reference Guide
        public ActionResult EmployeeReferenceGuide()
        {
            return View();
        }
        public ActionResult PolicyCreation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadEmpPolicies(HttpPostedFileBase policy_file)
        {
            if (policy_file.ContentLength > 0 && policy_file != null)
            {
                var accountcode = Session["cmpcode"].ToString();
                if (!string.IsNullOrEmpty(accountcode))
                {
                    string fileName = Path.GetFileName(policy_file.FileName);
                    var directoryPath = Path.Combine(Server.MapPath("~/UploadEmployeePolicies"), accountcode);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    var filePath = Path.Combine(directoryPath, fileName);
                    policy_file.SaveAs(filePath);
                }
            }
            return RedirectToAction("PolicyCreation");
        }
        #endregion


        #region Insurance Deatails
        public ActionResult InsuranceCreate()
        {
            return View();
        }

        public ActionResult InsuranceDetails()
        {
            return View();
        }
        #endregion

        #region ImportInsuranceDetails
        [HttpGet]
        public ActionResult ImportInsuranceDetails()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ImportInsuranceDetails(HttpPostedFileBase fileUpload)
        {
            try
            {
                string errorMessage;
                DataTable dt = ProcessUploadedFile(fileUpload, out errorMessage);
                ImportResponseInsurance response = new ImportResponseInsurance();
                ImportFailLogInsurance failLog = new ImportFailLogInsurance();

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    ViewBag.Error = errorMessage;
                    return View();
                }

                response = DataTableConverter.ConvertToImportedDataInsuranceList(dt, this.HttpContext);

                if (response.status && response.statusCode == 101)
                {
                    if (response.InsuranceList != null && response.InsuranceList.Count > 0)
                    {
                        try
                        {
                            failLog.ImportInsurancefaillogList = UtilitiesRestClient.ImportInsuranceDetail(response.InsuranceList);
                            if (!failLog.ImportInsurancefaillogList.Any())
                            {
                                TempData["IsValid"] = true;
                                ViewBag.Msg = "File imported successfully.";
                            }
                            else
                            {
                                TempData["IsValid"] = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Error = "Error while importing file: " + ex.Message;
                            TempData["IsValid"] = false;
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Your Excel file contains no data.";
                        TempData["IsValid"] = false;
                    }
                }
                else
                {
                    ViewBag.Error = response.statusCode == 102 ? "Please import a file with the proper format." : "Your Excel file contains no data.";
                    TempData["IsValid"] = false;
                }

                dt.Clear();
                dt.Dispose();
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        //-- convert selected file to datatable for import Insurace details --//
        private DataTable ProcessUploadedFile(HttpPostedFileBase file, out string errorMessage)
        {
            DataTable dt = new DataTable();
            errorMessage = string.Empty;
            string extension = System.IO.Path.GetExtension(file.FileName).ToLower();
            string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
            string path = string.Format("{0}/{1}", Server.MapPath("~/Uploads"), file.FileName);

            try
            {
                if (!validFileTypes.Contains(extension))
                {
                    errorMessage = "Please Upload Files in .xls, .xlsx or .csv format";
                    return null;
                }

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                file.SaveAs(path);

                Utility utl = new Utility();
                string connString = "";

                if (extension == ".csv")
                {
                    dt = utl.ConvertCSVtoDataTable(path);
                }
                else if (extension == ".xls")
                {
                    connString = "Provider=Microsoft.Ace.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
                    dt = utl.ConvertXSLXtoDataTable(path, connString);
                }
                else if (extension == ".xlsx")
                {
                    connString = "Provider=Microsoft.Ace.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
                    dt = utl.ConvertXSLXtoDataTable(path, connString);
                }

                // Clean up
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                return dt;
            }
            catch (Exception ex)
            {
                errorMessage = "An error occurred while processing the file: " + ex.Message;
                return null;
            }
        }
        public static class DataTableConverter
        {
            public static ImportResponseInsurance ConvertToImportedDataInsuranceList(DataTable dt, HttpContextBase httpContext)
            {
                ImportResponseInsurance res = new ImportResponseInsurance();
                if (dt != null && dt.Rows.Count > 0)
                {
                    try
                    {
                        var filteredRows = dt.Rows.Cast<DataRow>()
                        .Where(row => !row.ItemArray.All(f => f is DBNull));

                        DataTable dt1;

                        if (!filteredRows.Any())
                        {
                            dt1 = dt.Clone();
                            res.status = false;
                            res.statusCode = 103;
                        }
                        else
                        {
                            dt1 = filteredRows.CopyToDataTable();
                            string[] requiredColumns = { "EMPLOYEEID", "INSURANCEOF", "TYPEOFPOLICY", "INSURANCECOMPANYNAME", "PolicyNUMBER" };
                            bool allColumnsExist = requiredColumns.All(col => dt1.Columns.Contains(col));

                            if (allColumnsExist)
                            {
                                res.InsuranceList = (from DataRow row in dt1.Rows
                                                     select new InsuranceDetails
                                                     {
                                                         employee_code = row["EMPLOYEEID"].ToString(),
                                                         insurance_of = row["INSURANCEOF"].ToString(),
                                                         dependent_name = row["DEPENDENTNAME"].ToString(),
                                                         dependent_age = row["DEPENDENTAGE"].ToString(),
                                                         dependent_gender = row["DEPENDENTGENDER"].ToString(),
                                                         dependent_birthdate = row["DEPENDENTDATEOFBIRTH"].ToString(),
                                                         policy_type = row["TYPEOFPOLICY"].ToString(),
                                                         insurance_company_name = row["INSURANCECOMPANYNAME"].ToString(),
                                                         policy_number = row["PolicyNumber"].ToString(),
                                                         registration_date = row["POLICYREGISTRATIONDATE"].ToString(),
                                                         due_date = row["DUEDATE"].ToString(),
                                                         expire_date = row["EXPIREDATE"].ToString(),
                                                         insurance_amount = row["INSURANCEAMOUNT"].ToString(),
                                                         annual_amount = row["INSURANCEANNUALAMOUNT"].ToString(),
                                                         monthly_premium = row["MONTHLYPREMIUM"].ToString(),
                                                         premium_deduct_salary = Convert.ToInt32(row["ISPREMIUMDEDUCTFROMSALARY"].ToString()),
                                                         Salary_deduct_date = row["PREMIUMDEDUCTIONEFFECTDATE"].ToString(),
                                                         is_verify = Convert.ToInt32(row["ISVERIFIEDORNOT"].ToString()),
                                                         link = row["Link"].ToString(),
                                                         UID = row["UID"].ToString()
                                                     }).ToList();

                                res.status = true;
                                res.statusCode = 101;
                            }
                            else
                            {
                                res.status = false;
                                res.statusCode = 102;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        res.status = false;
                        res.statusCode = 103;
                    }
                }
                else
                {
                    res.status = false;
                    res.statusCode = 103;
                }
                return res;
            }
        }

        [HttpGet]
        public JsonResult DownloadInsuranceErrList(string DtFlag)
        {
            try
            {

                var faillogList = UtilitiesRestClient.ImportInsuranceerror(DtFlag);
                List<ImportInsurancefaillog> compFaillogList = faillogList
               .Select(InsuranceFail => new ImportInsurancefaillog
               {
                   employee_code = InsuranceFail.employee_code,
                   insurance_of = InsuranceFail.insurance_of,
                   dependent_name = InsuranceFail.dependent_name,
                   dependent_age = InsuranceFail.dependent_age,
                   dependent_birthdate = InsuranceFail.dependent_birthdate,
                   dependent_gender = InsuranceFail.dependent_gender,
                   policy_type = InsuranceFail.policy_type,
                   insurance_company_name = InsuranceFail.insurance_company_name,
                   policy_number = InsuranceFail.policy_number,
                   registration_date = InsuranceFail.registration_date,
                   due_date = InsuranceFail.due_date,
                   expire_date = InsuranceFail.expire_date,
                   insurance_amount = InsuranceFail.insurance_amount,
                   annual_amount = InsuranceFail.annual_amount,
                   monthly_premium = InsuranceFail.monthly_premium,
                   premium_deduct_salary = InsuranceFail.premium_deduct_salary,
                   Salary_deduct_date = InsuranceFail.Salary_deduct_date,
                   is_verify = InsuranceFail.is_verify,
                   link = InsuranceFail.link,
                   UID = InsuranceFail.UID,
                   Reason = InsuranceFail.Reason,
               })
                .ToList();
                JsonResult result = Json(compFaillogList);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.MaxJsonLength = Int32.MaxValue;

                return result;

            }
            catch (Exception ex)
            {
                ViewBag.error = "Error while downloading file.";
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }
        #endregion

        public ActionResult AdvanceSalaryRequest()
        {
            return View();
        }

        public ActionResult DeviceVerificationEnrollUser()
        {
            return View();
        }
    }
}