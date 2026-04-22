using Newtonsoft.Json;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Converters;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class MasterDataController : MyBaseController
    {
        #region Declaration
        static readonly MasterDataRestClient RestClient = new MasterDataRestClient();
        static readonly UtilitiesDataRestClient UtilitiesRestClient = new UtilitiesDataRestClient();
        static readonly TransactionDataRestClient TransactionRestClient = new TransactionDataRestClient();
        #endregion

        #region Company Master
        [HttpGet]
        public ActionResult CompanyMaster()
        {

            DataTable dtcustomformdata = new DataTable();

            try
            {

                ViewBag.planID = Session["planId"];
                Session["actname"] = "Company Master";

                if (Session["actname"] != null)
                {
                    TempData["frmname"] = Session["actname"].ToString();
                }

                IEnumerable<Cfc> lstobj = UtilitiesRestClient.GetAllcustomforms();
                ViewBag.lstcustomform = JsonConvert.SerializeObject(lstobj);
                dtcustomformdata = UtilitiesRestClient.Getcustomformdata(Session["actname"].ToString());

            }
            catch
            {
                dtcustomformdata = null;
                return RedirectToAction("ErrorPage", "PayTime");
            }
            finally
            {
                dtcustomformdata = null;
            }

            return View(dtcustomformdata);

            //return View();
        }


        [HttpGet]
        public ActionResult CompanyMasterGetbyid(string id)
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            Companys cmp = new Companys();
            try
            {
                cmp = RestClient.CompanyGetByid(Convert.ToInt32(id.Trim()));
                return View(cmp);
            }
            catch (Exception ex)
            {
                cmp = null;
                return RedirectToAction("ErrorPage", "PayTime");
            }
            finally
            {
                cmp = null;
            }

        }

        [HttpPost]
        public string DeleteCompany(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.CompanyDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }


        [HttpGet]
        public string GetCompaniesEmail(string emailid)
        {
            var emailList = RestClient.CompanyGetAll().Where(c => c.CompanyEmail == emailid).Select(c => c.CompanyEmail).FirstOrDefault();
            //var emailList = RestClient.CompanyGetAll("emailid", emailid);
            return emailList;
        }

        [HttpPost]
        public string uploaddoc()
        {
            string sts = "";
            var fileName = "";
            try
            {
                if (Request.Files.Count != 0)
                {

                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files[i];

                        fileName = Path.GetFileName(file.FileName);

                        var path = Path.Combine(Server.MapPath("~/FileuploadControlData/" + Session["cmpcode"] + "/"), fileName);
                        file.SaveAs(path);
                        sts = "OK";
                    }
                }

            }
            catch
            {
                sts = "";
                fileName = "";
                return new JavaScriptSerializer().Serialize(sts);
            }
            finally
            {
                sts = "";
                fileName = "";
            }

            return new JavaScriptSerializer().Serialize(sts);
        }

        #endregion

        #region Branch Master

        public ActionResult BranchMaster()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            ViewBag.planID = Session["planId"];
            Branches objc = new Branches();
            ViewBag.isStandard = Convert.ToInt32(Session["isStandard"]);
            try
            {
                CompanySetting objsys = RestClient.GetSystemSettingById(1);
                ViewBag.IsEss = Convert.ToBoolean(objsys.IsEss.ToString() != "" ? objsys.IsEss.ToString() : "0");
                ViewBag.IsBeacon = Convert.ToBoolean(objsys.IsBeacon.ToString() != "" ? objsys.IsBeacon.ToString() : "0");
                ViewBag.IsBranchGeoFence = Convert.ToBoolean(objsys.IsBranchGeoFence.ToString() != "" ? objsys.IsBranchGeoFence.ToString() : "0");
                ViewBag.IsVerification = Convert.ToInt32(RestClient.GetSystemSettingById(1).IsVerification);

                Session["actname"] = "Branch Master";

                if (Session["actname"] != null)
                {
                    TempData["frmname"] = Session["actname"].ToString();
                }

                IEnumerable<Cfc> lstobj = UtilitiesRestClient.GetAllcustomforms();

                ViewBag.lstcustomform = JsonConvert.SerializeObject(lstobj);
                DataTable model = UtilitiesRestClient.Getcustomformdata(Session["actname"].ToString());

                return View(objc);
            }
            catch (Exception ex)
            {
                objc.Brancheslist = null;
                ViewBag.error = ex.Message;
                return RedirectToAction("ErrorPage", "PayTime");
            }
            finally
            {
                objc.Brancheslist = null;
                objc = null;
            }

        }

        [HttpPost]
        public string DeleteBranch(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.BranchDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }


        //--- GetCountryTimeZone select branchwise timezone details
        [HttpGet]
        public JsonResult GetCountryTimeZone()
        {
            JsonResult result;
            try
            {
                var TimeZone = RestClient.CountryTimeZoneAll();
                var CountryTimeZone = TimeZone;
                result = Json(CountryTimeZone);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            finally
            {
                result = Json(new SelectList("", "0"));
            }
        }
        #endregion

        #region ShiftGroupMaster

        [HttpGet]
        public ActionResult ShiftGroupMaster()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            ShiftGroup cmp = new ShiftGroup();
            string RoleId = Session["RoleId"].ToString();
            int cmpid = 0;
            int BranchID = 0;
            try
            {
                //if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                //{
                //    cmp.ShiftGrouplist = RestClient.ShiftGroupGetAll();
                //}
                //else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                //{
                //    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                //    var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                //    if (Session["RoleId"].ToString() == "6806")
                //    {
                //        cmp.ShiftGrouplist = RestClient.ShiftGroupGetAll().Where(x =>(x.CompanyID == cmpid && (x.BranchID == BranchID || x.BranchID == 0)) ||
                //            (x.CompanyID == 0));
                //    }
                //    else
                //    {
                //        cmp.ShiftGrouplist = RestClient.ShiftGroupGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
                //    }

                //}

                if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                {
                    cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                }

                cmp.ShiftGrouplist = RestClient.ShiftGroupGetAll(RoleId, cmpid, BranchID);

                return View(cmp);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        [HttpPost]
        public ActionResult ShiftGroupMaster(ShiftGroup h)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ViewBag.msg = "";
                    ViewBag.error = "";
                    if (h.ShiftGroupId > 0)
                    {
                        h.IsActive = true;
                        h.ShiftGrouplist = RestClient.ShiftGroupGetAll().Where(c => c.ShiftGroupName.ToLower() == h.ShiftGroupName.ToLower() && c.ShiftGroupId != h.ShiftGroupId);
                        if (h.ShiftGrouplist.Any())
                        {
                            ViewBag.error = h.ShiftGroupName + " Shift group name already exists.";
                        }
                        else
                        {
                            if (!RestClient.ShiftGroupUpdate(h.ShiftGroupId, h))
                            {
                                ViewBag.error = "Shift group not updated.";
                            }
                            else
                            {
                                ViewBag.msg = "Shift group updated successfully.";
                            }
                        }
                    }
                    else
                    {
                        h.ShiftGrouplist = RestClient.ShiftGroupGetAll().Where(c => c.ShiftGroupName.ToLower() == h.ShiftGroupName.ToLower());
                        //h.ShiftGrouplist = RestClient.ShiftGroupGetAll(h.ShiftGroupName.ToLower());
                        if (h.ShiftGrouplist.Any())
                        {
                            ViewBag.error = h.ShiftGroupName + " Shift group name already exists.";
                        }
                        else
                        {
                            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                            {
                                h.CompanyID = 0;
                                h.BranchID = 0;
                            }
                            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                            {
                                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                                h.CompanyID = cmpid;
                                h.BranchID = BranchID;
                            }
                            RestClient.ShiftGroupAdd(h);
                            ViewBag.msg = "Shift group added successfully.";
                        }
                    }
                }
                else
                {
                    ViewBag.error = "Not valid entry.";
                }

                // ################## Replace Linq 13 06 2023
                string RoleId = Session["RoleId"].ToString();
                int _cmpid = 0;
                int _BranchID = 0;

                //if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                //{
                //    h.ShiftGrouplist = RestClient.ShiftGroupGetAll();
                //}
                //else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                //{
                //    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                //    var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                //    if (Session["RoleId"].ToString() == "6806")
                //    {
                //        h.ShiftGrouplist = RestClient.ShiftGroupGetAll().Where(x =>(x.CompanyID == cmpid && (x.BranchID == BranchID || x.BranchID == 0)) || (x.CompanyID == 0));
                //    }
                //    else
                //    {
                //        h.ShiftGrouplist = RestClient.ShiftGroupGetAll() .Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
                //    }

                //}

                if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                {
                    _cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    _BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                }

                h.ShiftGrouplist = RestClient.ShiftGroupGetAll(RoleId, _cmpid, _BranchID);


                // ################## Replace Linq 13 06 2023

                return View(h);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }

        [HttpPost]
        public string DeleteShiftGroup(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.ShiftGroupDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }


        [HttpGet]
        public JsonResult GetShiftGroupShortName(string shiftgroupshortname)
        {
            if (shiftgroupshortname == "")
            {
                shiftgroupshortname = string.Empty;
            }
            JsonResult result;
            //ErrorMsg emg = new ErrorMsg();
            string str = RestClient.CheckShiftGroupShortName(shiftgroupshortname);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(str);
            result = Json(json);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        #endregion

        #region ShiftMaster

        [HttpGet]
        public ActionResult ShiftMaster()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            Shifts cmp = new Shifts();
            try
            {

                CompanySetting objsys = RestClient.GetSystemSettingById(1);
                ViewBag.IsFlexi = Convert.ToInt32(RestClient.GetSystemSettingById(1).IsFlexi);
                var RoleId = Session["RoleId"].ToString();
                var cmpid = 0;
                var BranchID = 0;

                //if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                //{
                //    cmp.Shiftslist = RestClient.ShiftGetAll();
                //    cmp.ShiftGrouplist = RestClient.ShiftGroupGetAll();
                //}
                //else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                //{
                //    cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                //    BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                //    if (Session["RoleId"].ToString() == "6806")
                //    {
                //        cmp.Shiftslist = RestClient.ShiftGetAll().Where(x => (x.GraceBefore == cmpid && (x.GraceAfter == BranchID || x.GraceAfter == 0)) || (x.GraceBefore == 0));
                //        cmp.ShiftGrouplist = RestClient.ShiftGroupGetAll().Where(x => (x.CompanyID == cmpid && (x.BranchID == BranchID || x.BranchID == 0)) || (x.CompanyID == 0));
                //    }
                //    else
                //    {
                //        cmp.Shiftslist = RestClient.ShiftGetAll().Where(x => x.GraceBefore == cmpid || x.GraceBefore == 0);
                //        cmp.ShiftGrouplist = RestClient.ShiftGroupGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
                //    }

                //}

                if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                {
                    cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                }

                cmp.Shiftslist = RestClient.ShiftGetAll(RoleId, cmpid, BranchID);
                cmp.ShiftGrouplist = RestClient.ShiftGroupGetAll(RoleId, cmpid, BranchID);

                return View(cmp);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        [HttpPost]
        public ActionResult ShiftMaster(Shifts h)
        {
            try
            {
                Shifts cmp = new Shifts();
                ViewBag.fillShiftGroup = FillShiftGroup();
                h.roleid = Convert.ToInt32(Session["RoleId"]);
                ViewBag.msg = "";
                ViewBag.error = "";

                if (h.ShiftId > 0)
                {
                    h.IsActive = true;
                    var res = RestClient.ShiftUpdate(h.ShiftId, h);
                    if (res.MegSts == "ok")
                    {
                        ViewBag.msg = "Shift updated successfully.";
                    }
                    else
                    {
                        ViewBag.error = res.Meg;
                    }
                }
                else
                {
                    h.Shiftslist = RestClient.ShiftGetAll(h.ShiftName.ToLower(), "ShiftName");
                    //h.Shiftslist = RestClient.ShiftGetAll().Where(c => c.ShiftName.ToLower() == h.ShiftName.ToLower());
                    if (h.Shiftslist.Any())
                    {
                        ViewBag.error = h.ShiftName + " Shift name already exists.";
                    }
                    else
                    {
                        if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                        {
                            h.GraceBefore = 0;
                            h.GraceAfter = 0;
                        }
                        else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                        {
                            var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                            var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                            h.GraceBefore = cmpid;
                            h.GraceAfter = BranchID;
                        }
                        var res = RestClient.ShiftAdd(h);
                        if (res.MegSts == "ok")
                        {
                            ViewBag.msg = "Shift added successfully.";
                        }
                        else
                        {
                            ViewBag.error = res.Meg;
                        }

                    }
                }

                //if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                //{
                //    cmp.Shiftslist = RestClient.ShiftGetAll();
                //    cmp.ShiftGrouplist = RestClient.ShiftGroupGetAll();
                //}
                //else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                //{
                //    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                //    var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                //    if (Session["RoleId"].ToString() == "6806")
                //    {
                //        cmp.Shiftslist = RestClient.ShiftGetAll().Where(x => (x.GraceBefore == cmpid && (x.GraceAfter == BranchID || x.GraceAfter == 0)) || (x.GraceBefore == 0));
                //        cmp.ShiftGrouplist = RestClient.ShiftGroupGetAll().Where(x => (x.CompanyID == cmpid && (x.BranchID == BranchID || x.BranchID == 0)) || (x.CompanyID == 0));
                //    }
                //    else
                //    {
                //        cmp.Shiftslist = RestClient.ShiftGetAll().Where(x => x.GraceBefore == cmpid || x.GraceBefore == 0);
                //        cmp.ShiftGrouplist = RestClient.ShiftGroupGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
                //    }
                //}
                var RoleId = Session["RoleId"].ToString();
                var _cmpid = 0;
                var _BranchID = 0;

                if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                {
                    _cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    _BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                }

                cmp.Shiftslist = RestClient.ShiftGetAll(RoleId, _cmpid, _BranchID);
                cmp.ShiftGrouplist = RestClient.ShiftGroupGetAll(RoleId, _cmpid, _BranchID);

                return View(cmp);
            }
            catch (Exception)
            {

                return RedirectToAction("ErrorPage", "PayTime");
            }


        }
        [HttpPost]
        public string DeleteShift(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.ShiftDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }

        public JsonResult GetShiftDuration(int id, string checkintime, string mode)
        {
            JsonResult result;
            var list = new List<Tuple<int, int>>();
            var todaydate = DateTime.Now.ToString("yyyy:MM:dd");
            var shiftallocation = TransactionRestClient.ShiftAllocationGetAll().Where(c => c.EmpCode == id && c.ShiftDateStr == todaydate).Select(s => s.ShiftName).FirstOrDefault();
            //var shiftallocation = TransactionRestClient.ShiftAllocationGetAll(id, todaydate).Select(s => s.ShiftName).FirstOrDefault();
            if (shiftallocation == null)
            {
                Employees em = new Employees();
                em.EmployeeList = RestClient.EmployeeGet(id);
                int shiftid = 0;
                foreach (var sh in em.EmployeeList)
                {
                    shiftid = sh.ShiftId;
                    if (shiftid == 0)
                    {
                        shiftid = sh.ShiftGroupId;
                        //var gettime = RestClient.ShiftGetAll().Where(c => c.ShiftGroupId == shiftid).Select(j => new { StartTime = Convert.ToDateTime(j.StartTime).ToString("dd-MMM-yyyy HH:mm:ss"), j.ShiftId });
                        var gettime = RestClient.ShiftGetAll(Convert.ToString(shiftid), "ShiftGroupId").Select(j => new { StartTime = Convert.ToDateTime(j.StartTime).ToString("dd-MMM-yyyy HH:mm:ss"), j.ShiftId });
                        foreach (var data in gettime)
                        {
                            var d = Convert.ToDateTime(checkintime);
                            TimeSpan ts = new TimeSpan();
                            ts = d.Subtract(Convert.ToDateTime(data.StartTime));
                            int totalMinutesDiff = ((Math.Abs(ts.Hours)) * 60) + (Math.Abs(ts.Minutes));
                            list.Add(new Tuple<int, int>(totalMinutesDiff, data.ShiftId));
                        }
                        var jc = list.Min();
                        shiftid = jc.Item2;
                    }
                }
                //var i = RestClient.ShiftGetAll().Where(c => c.ShiftId == shiftid).Select(c => c.ShiftDur);
                var i = RestClient.ShiftGetAll(Convert.ToString(shiftid), "shiftid").Select(c => c.ShiftDur);
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(i);
                result = Json(json);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            }
            else
            {
                var ii = RestClient.ShiftGetAll(shiftallocation, "shiftallocation").Select(c => c.ShiftDur);
                //var ii = RestClient.ShiftGetAll().Where(c => c.ShiftName == shiftallocation).Select(c => c.ShiftDur);
                var jsonSerialiser1 = new JavaScriptSerializer();
                var json1 = jsonSerialiser1.Serialize(ii);
                result = Json(json1);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            }
            return result;
        }


        [HttpPost]
        public JsonResult GetShiftShortName(string shiftshortname)
        {
            if (shiftshortname == "")
            {
                shiftshortname = string.Empty;
            }
            JsonResult result;
            //ErrorMsg emg = new ErrorMsg();
            string str = RestClient.CheckShiftShortName(shiftshortname);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(str);
            result = Json(json);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        [HttpPost]
        public JsonResult GetShiftName(string shiftname)
        {
            if (shiftname == "")
            {
                shiftname = string.Empty;
            }
            JsonResult result;
            //ErrorMsg emg = new ErrorMsg();
            string str = RestClient.CheckShiftName(shiftname);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(str);
            result = Json(json);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        #endregion

        #region Department
        [HttpGet]
        public ActionResult DepartmentMaster()
        {
            //ViewBag.msg = "";
            //ViewBag.error = "";
            //Departments cmp = new Departments();
            try
            {
                // cmp.Departmentslist = RestClient.DepartmentGetAll();
                //ViewBag.DepartmentMasterList = RestClient.DepartmentGetAll();
                //ViewBag.fillEmployees = FillEmployee();
                //ViewBag.fillEmployees = RestClient.EmployeeGetAll();

                ViewBag.planID = Session["planId"];

                Session["actname"] = "Department Master";

                if (Session["actname"] != null)
                {
                    TempData["frmname"] = Session["actname"].ToString();
                }

                IEnumerable<Cfc> lstobj = UtilitiesRestClient.GetAllcustomforms();

                ViewBag.lstcustomform = JsonConvert.SerializeObject(lstobj);
                DataTable model = UtilitiesRestClient.Getcustomformdata(Session["actname"].ToString());
                return View(model);
                //return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        //[HttpPost]
        //public ActionResult DepartmentMaster(Departments d)
        //{

        //    try
        //    {

        //        if (d.DepartmentHead == 0)
        //        {
        //            ModelState.Remove("DepartmentHead");
        //        }
        //        if (ModelState.IsValid)
        //        {
        //            ViewBag.msg = "";
        //            ViewBag.error = "";
        //            if (d.DepartmentId > 0)
        //            {
        //                d.IsActive = true;
        //                if (!RestClient.DepartmentUpdate(d.DepartmentId, d))
        //                {
        //                    ViewBag.msg = "Department not updated due to service issue.";
        //                }
        //                else
        //                {
        //                    ViewBag.msg = "Department is updated";
        //                }
        //            }
        //            else
        //            {
        //                d.Departmentslist = RestClient.DepartmentGetAll().Where(c => c.DepartmentName == d.DepartmentName);
        //                if (d.Departmentslist.Count() > 0)
        //                {
        //                    ViewBag.error = d.DepartmentName + " Department already exists.";
        //                }
        //                else
        //                {

        //                    RestClient.DepartmentAdd(d);
        //                    ViewBag.msg = "Department added successfully.";
        //                }
        //            }
        //        }
        //        else
        //        {
        //            ViewBag.error = "Not Valid Entry";
        //        }
        //        d.Departmentslist = RestClient.DepartmentGetAll();
        //        ViewBag.fillEmployees = FillEmployee();
        //        //ViewBag.fillEmployees = RestClient.EmployeeGetAll();
        //        ViewBag.DepartmentMasterList = RestClient.DepartmentGetAll();
        //        return View(d);
        //    }
        //    catch (Exception)
        //    {
        //        return RedirectToAction("ErrorPage", "PayTime");
        //    }


        //}
        [HttpPost]
        public string DeleteDepartment(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.DepartmentDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }
        #endregion

        #region Designation

        [HttpGet]
        public ActionResult DesignationMaster()
        {
            //ViewBag.msg = "";
            //ViewBag.error = "";
            //Designations cmp = new Designations();
            ViewBag.planID = Session["planId"];
            try
            {
                //cmp.Designationlist = RestClient.DesignationsGetAll();
                Session["actname"] = "Designation Master";

                if (Session["actname"] != null)
                {
                    TempData["frmname"] = Session["actname"].ToString();
                }

                IEnumerable<Cfc> lstobj = UtilitiesRestClient.GetAllcustomforms();

                ViewBag.lstcustomform = JsonConvert.SerializeObject(lstobj);
                DataTable model = UtilitiesRestClient.Getcustomformdata(Session["actname"].ToString());
                return View();
                //return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        [HttpPost]
        public ActionResult DesignationMaster(Designations d)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ViewBag.msg = "";
                    ViewBag.error = "";
                    if (d.DesignationId > 0)
                    {
                        d.IsActive = true;
                        if (!RestClient.DesignationsUpdate(d.DesignationId, d))
                        {
                            ViewBag.msg = "Designation not updated due to service issue.";
                        }
                        else
                        {
                            ViewBag.msg = "Designation is updated.";
                        }
                    }
                    else
                    {
                        d.Designationlist = RestClient.DesignationsGetAll().Where(c => c.DesignationName.ToLower() == d.DesignationName.ToLower());
                        //d.Designationlist = RestClient.DesignationsGetAll(d.DesignationName.ToLower());
                        if (d.Designationlist.Count() > 0)
                        {
                            ViewBag.error = d.DesignationName + " Designation already exists.";
                        }
                        else
                        {

                            RestClient.DesignationsAdd(d);
                            ViewBag.msg = "Designation added successfully.";
                        }
                    }
                }
                else
                {
                    ViewBag.error = "Not valid entry.";
                }
                d.Designationlist = RestClient.DesignationsGetAll();
                return View(d);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [HttpPost]
        public string DeleteDesignation(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.DesignationsDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }
        #endregion

        #region Holiday

        [HttpGet]
        public ActionResult HolidayMaster()
        {
             return View();
        }
        [HttpPost]
        public ActionResult HolidayMaster(Holidays h)
        {
            try
            {
                Holidays holiday = new Holidays();

                string[] _BranchIds = h.BranchIds;
                var strBranchIds = String.Join(",", _BranchIds);
                h.BranchId = strBranchIds;

                string[] _CompanyIDs = h.CompanyIDs;
                var strCompanyIDs = String.Join(",", _CompanyIDs);
                h.CompanyID = strCompanyIDs;

                string[] _ReligionIds = h.ReligionIds;
                var strReligionIds = String.Join(",", _ReligionIds);
                h.ReligionId = strReligionIds;

            
                var dynamicDateFormat = Session["IsDateFormat"].ToString();
                var isDateFormat = " ";
                if (dynamicDateFormat == "dd-M-yyyy" || dynamicDateFormat == "M-dd-yyyy" || dynamicDateFormat == "yyyy-M-dd")
                {
                    isDateFormat = Session["IsDateFormat"].ToString().Replace("M", "MMM");
                }
                else
                {
                    isDateFormat = Session["IsDateFormat"].ToString().Replace("mm", "MM");
                }
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = isDateFormat.ToString() };
                var json = JsonConvert.SerializeObject(h, new IsoDateTimeConverter() { DateTimeFormat = dateTimeConverter.ToString() });
                if (string.IsNullOrEmpty(h.ReligionId))
                {
                    h.ReligionId = "0";
                }
                ViewBag.msg = "";
                ViewBag.error = "";
                if (Session["RoleId"].ToString() == "6805")
                {
                    var cmpid = Session["ClientCompanyId"].ToString();
                    h.CompanyID = cmpid.ToString();
                }
                if (Session["RoleId"].ToString() == "6806")
                {
                    var cmpid = Session["ClientCompanyId"].ToString();
                    var brachid = Session["BranchId"].ToString();
                    h.BranchId = brachid;
                    h.CompanyID = cmpid;
                }
                if (h.HolidayId > 0)
                {
                    h.IsActive = true;
                    var resp = RestClient.HolidaysUpdate(h.HolidayId, h);
                    if (resp.MegSts == "ok")
                    {
                        ViewBag.msg = "Holiday updated successfully.";
                    }
                    else
                    {
                        ViewBag.error = resp.Meg;
                    }
                }
                else
                {
                    h.IsActive = true;
                    var resp = RestClient.HolidaysAdd(h);
                    if (resp.MegSts == "ok")
                        ViewBag.msg = "Holiday added successfully.";
                    else
                    {
                        ViewBag.error = resp.Meg;
                    }

                }


                ViewBag.BranchFill = FillBranch();
                h.Countrylist = RestClient.CountryGetAll();
                if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                {
                    h.ReligionList = RestClient.ReligionGetAll();
                }
                else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                    if (Session["RoleId"].ToString() == "6806")
                    {
                        h.ReligionList = RestClient.ReligionGetAll().Where(x =>
                            (x.CompanyID == cmpid && (x.BranchID == BranchID || x.BranchID == 0)) ||
                            (x.CompanyID == 0));
                    }
                    else
                    {
                        h.ReligionList = RestClient.ReligionGetAll()
                            .Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
                    }
                }
                if (Session["RoleId"].ToString() == "6806")
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                    h.BranchList = RestClient.BranchGetAll().Where(x => x.CompanyID == cmpid && x.BranchId == BranchID);
                }
                else if (Session["RoleId"].ToString() == "6805")
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    h.BranchList = RestClient.BranchGetAll().Where(x => x.CompanyID == cmpid);
                }
                else if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) > 1)
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                    h.BranchList = RestClient.BranchGetAll().Where(x => x.CompanyID == cmpid && x.BranchId == BranchID);
                }
                else
                {
                    h.BranchList = RestClient.BranchGetAll();
                }

                if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) == 1)
                {
                    h.CompanyList = RestClient.CompanyGetAll();
                }
                else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    h.CompanyList = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                }
                else if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) > 1)
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    h.CompanyList = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                }

                ViewBag.isReligion = Convert.ToBoolean(RestClient.GetSystemSettingById(1).HasReligion.ToString() != "" ? RestClient.GetSystemSettingById(1).HasReligion.ToString() : "0");
                ViewBag.TimeZones = ShowTimeZone();
                if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) == 1)
                {
                    h.Holidayslist = RestClient.HolidaysGetAll();
                }
                else if (Session["RoleId"].ToString() == "6805")
                {
                    var cmpid = Session["ClientCompanyId"].ToString();
                    h.Holidayslist = RestClient.HolidaysGetAll().Where(x => x.CompanyID.Contains(cmpid) || x.CompanyID == "0");
                }
                else if (Session["RoleId"].ToString() == "6806")
                {
                    var brachid = Session["BranchId"].ToString();
                    h.Holidayslist = RestClient.HolidaysGetAll().Where(x => x.BranchId.Contains(brachid) || x.BranchId == "0");
                }
                else if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) > 1)
                {
                    var cmpid = Session["ClientCompanyId"].ToString();
                    h.Holidayslist = RestClient.HolidaysGetAll().Where(x => x.CompanyID.Contains(cmpid) || x.CompanyID == "0");
                }
                return View(h);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [HttpPost]
        public string DeleteHolidayMaster(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.HolidaysDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }
        public PartialViewResult HolidayMasterPartial()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            Holidays cmp = new Holidays();
            ViewBag.BranchFill = FillBranch();
            cmp.Countrylist = RestClient.CountryGetAll();
            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
            {
                cmp.ReligionList = RestClient.ReligionGetAll();
            }
            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                if (Session["RoleId"].ToString() == "6806")
                {
                    cmp.ReligionList = RestClient.ReligionGetAll().Where(x => (x.CompanyID == cmpid && (x.BranchID == BranchID || x.BranchID == 0)) || (x.CompanyID == 0));
                }
                else
                {
                    cmp.ReligionList = RestClient.ReligionGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
                }
            }
            if (Session["RoleId"].ToString() == "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                cmp.BranchList = RestClient.BranchGetAll().Where(x => x.CompanyID == cmpid && x.BranchId == BranchID);
            }
            else if (Session["RoleId"].ToString() == "6805")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                cmp.BranchList = RestClient.BranchGetAll().Where(x => x.CompanyID == cmpid);
            }
            else if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) > 1)
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                cmp.BranchList = RestClient.BranchGetAll().Where(x => x.CompanyID == cmpid && x.BranchId == BranchID);
            }
            else
            {
                cmp.BranchList = RestClient.BranchGetAll();
            }
            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) == 1)
            {
                cmp.CompanyList = RestClient.CompanyGetAll();
            }
            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                cmp.CompanyList = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
            }
            else if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) > 1)
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                cmp.CompanyList = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
            }
            ViewBag.isReligion = Convert.ToBoolean(RestClient.GetSystemSettingById(1).HasReligion.ToString() != "" ? RestClient.GetSystemSettingById(1).HasReligion.ToString() : "0");
            ViewBag.TimeZones = ShowTimeZone();
            try
            {
                if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) == 1)
                {
                    cmp.Holidayslist = RestClient.HolidaysGetAll();
                }
                else if (Session["RoleId"].ToString() == "6805")
                {
                    var cmpid = Session["ClientCompanyId"].ToString();
                    cmp.Holidayslist = RestClient.HolidaysGetAll().Where(x =>
                                        x.CompanyID.Split(',').Select(b => b.Trim()).Contains(cmpid) || x.CompanyID == "0"
                                        );
                }
                else if (Session["RoleId"].ToString() == "6806")
                {
                    var brachid = Session["BranchId"].ToString();
                    cmp.Holidayslist = RestClient.HolidaysGetAll().Where(x =>
                        x.BranchId.Split(',').Select(b => b.Trim()).Contains(brachid) || x.BranchId == "0"
                    );
                }
                else if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) > 1)
                {
                    var cmpid = Session["ClientCompanyId"].ToString();
                    cmp.Holidayslist = RestClient.HolidaysGetAll().Where(x =>
                    x.CompanyID.Split(',').Select(b => b.Trim()).Contains(cmpid) || x.CompanyID == "0"
                    );

                }
                return PartialView("_HolidayMasterPartial",cmp);

            }
            catch (Exception ex)
            {
                ViewBag.error = "An error occurred while loading holiday data.";
                return PartialView("_HolidayMasterPartial", cmp);
            }
        }
        public PartialViewResult OptionalHolidayPartial()
        {
            return PartialView("_OptionalHolidayPartial");
        }
        #endregion

        #region EmployeeMaster


        #region Stop pageload dropdown list 13 06 2023
        //[HttpGet]
        //public ActionResult EmployeeMaster()
        //{
        //    ViewBag.msg = "";
        //    ViewBag.error = "";
        //    ViewBag.planID = Session["planId"];

        //    int cmpid = 0;
        //    int BranchID = 0;
        //    Employees emp = new Employees();
        //    try
        //    {
        //        ViewBag.DepartmentList = FillDepartment();
        //        ViewBag.DesignationList = FillDesignation();
        //        ViewBag.ShiftList = FillShift();
        //        ViewBag.ShiftGroupList = FillShiftGroup();
        //        ViewBag.fillrole = FillRole();
        //        ViewBag.fillcompanys = FillCompany();
        //        ViewBag.fillReporting = FillEmployeeReporting();

        //        //ViewBag.PolicyName = RestClient.HRPolicyGetAll();
        //        if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
        //        {
        //            ViewBag.PolicyName = RestClient.HRPolicyGetAll().ToList();
        //        }
        //        else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
        //        {
        //            cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
        //            BranchID = Convert.ToInt32(Session["BranchId"].ToString());
        //            if (Session["RoleId"].ToString() == "6806")
        //            {
        //                ViewBag.PolicyName = RestClient.HRPolicyGetAll().Where(x => (x.CompanyID == cmpid && (x.BranchID == BranchID || x.BranchID == 0)) || (x.CompanyID == 0));
        //            }
        //            else
        //            {
        //                ViewBag.PolicyName = RestClient.HRPolicyGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0).ToList();
        //            }

        //        }
        //        ViewBag.ReligionList = RestClient.ReligionGetAll();
        //        ViewBag.isReligion = Convert.ToBoolean(RestClient.GetSystemSettingById(1).HasReligion.ToString() != "" ? RestClient.GetSystemSettingById(1).HasReligion.ToString() : "0");
        //        ViewBag.HasSMS = Convert.ToBoolean(RestClient.GetSystemSettingById(1).HasSMS.ToString() != "" ? RestClient.GetSystemSettingById(1).HasSMS.ToString() : "0");
        //        ViewBag.IsEss = Convert.ToBoolean(RestClient.GetSystemSettingById(1).IsEss.ToString() != "" ? RestClient.GetSystemSettingById(1).IsEss.ToString() : "0");
        //        ViewBag.IsVerification = Convert.ToInt32(RestClient.GetSystemSettingById(1).IsVerification);
        //        planRestrictions(0);
        //        ViewBag.Islimit = Convert.ToInt32(Session["IsUserlimit"]);
        //        emp.isPlanFace = Convert.ToInt32(Session["isPlanFace"]);
        //        Session["actname"] = "Employee Master";

        //        if (Session["actname"] != null)
        //        {
        //            TempData["frmname"] = Session["actname"].ToString();
        //        }

        //        IEnumerable<Cfc> lstobj = UtilitiesRestClient.GetAllcustomforms();

        //        ViewBag.lstcustomform = JsonConvert.SerializeObject(lstobj);
        //        //DataTable model = UtilitiesRestClient.Getcustomformdata(Session["actname"].ToString());
        //        //return View(model);
        //        return View(emp);
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("ErrorPage", "PayTime");
        //    }
        //    finally
        //    {
        //        cmpid = 0;
        //        BranchID = 0;
        //        emp = null;
        //    }
        //}
        #endregion

        [HttpGet]
        public ActionResult EmployeeMaster()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            ViewBag.planID = Session["planId"];

            int cmpid = 0;
            int BranchID = 0;
            Employees emp = new Employees();
            try
            {
                
                //ViewBag.DepartmentList = FillDepartment();
                //ViewBag.DesignationList = FillDesignation();
                //ViewBag.ShiftList = FillShift();
                //ViewBag.ShiftGroupList = FillShiftGroup();
                // ViewBag.fillrole = FillRole();
                //ViewBag.fillcompanys = FillCompany();
                //ViewBag.fillReporting = FillEmployeeReporting();

                //ViewBag.PolicyName = RestClient.HRPolicyGetAll();
                //if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                //{
                //    ViewBag.PolicyName = RestClient.HRPolicyGetAll().ToList();
                //}
                //else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                //{
                //    cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                //    BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                //    if (Session["RoleId"].ToString() == "6806")
                //    {
                //        ViewBag.PolicyName = RestClient.HRPolicyGetAll().Where(x => (x.CompanyID == cmpid && (x.BranchID == BranchID || x.BranchID == 0)) || (x.CompanyID == 0));
                //    }
                //    else
                //    {
                //        ViewBag.PolicyName = RestClient.HRPolicyGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0).ToList();
                //    }

                //}
                //ViewBag.ReligionList = RestClient.ReligionGetAll();
                var _dtsys = RestClient.GetSystemSettingById(1);
                ViewBag.isReligion = Convert.ToBoolean(_dtsys.HasReligion.ToString() != "" ? _dtsys.HasReligion.ToString() : "0");
                ViewBag.HasSMS = Convert.ToBoolean(_dtsys.HasSMS.ToString() != "" ? _dtsys.HasSMS.ToString() : "0");
                ViewBag.IsEss = Convert.ToBoolean(_dtsys.IsEss.ToString() != "" ? _dtsys.IsEss.ToString() : "0");
                ViewBag.IsVerification = Convert.ToInt32(_dtsys.IsVerification);
             
                planRestrictions(0);
                ViewBag.Islimit = Convert.ToInt32(Session["IsUserlimit"]);
                emp.isPlanFace = Convert.ToInt32(Session["isPlanFace"]);
                Session["actname"] = "Employee Master";

                if (Session["actname"] != null)
                {
                    TempData["frmname"] = Session["actname"].ToString();
                }

                IEnumerable<Cfc> lstobj = UtilitiesRestClient.GetAllcustomforms();

                ViewBag.lstcustomform = JsonConvert.SerializeObject(lstobj);


             
                if (Session["RoleId"].ToString() != "1")
                {
                    var cmpanyid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    ViewBag.Companyslist = RestClient.CompanyGetAll().OrderBy(x => x.CompanyName).Where(x => x.CompanyID == cmpanyid);
                }
                else
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll().OrderBy(x => x.CompanyName);
                }

                ViewBag.DesignationList = FillDesignation();

                return View(emp);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
            finally
            {
                cmpid = 0;
                BranchID = 0;
                emp = null;
            }
        }

        public List<SelectListItem> FillDesignation()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            Designations ds = new Designations();
            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
            {
                ds.Designationlist = RestClient.DesignationsGetAll();
            }
            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                if (Session["RoleId"].ToString() == "6806")
                {
                    ds.Designationlist = RestClient.DesignationsGetAll().Where(x => (x.CompanyID == cmpid && (x.BranchID == BranchID || x.BranchID == 0)) || (x.CompanyID == 0)).ToList();
                }
                else
                {
                    ds.Designationlist = RestClient.DesignationsGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0).ToList();
                }
            }
            foreach (var i in ds.Designationlist)
            {
                list.Add(new SelectListItem() { Text = i.DesignationName, Value = Convert.ToString(i.DesignationId) });
            }
            return list;
        }
        private void planRestrictions(int mcmpid)
        {
            Getpara gp = new Getpara();
            gp.pkid = mcmpid;
            gp.para = 0;
            gp.iscount = 0;
            gp.companycode = Convert.ToString(Session["cmpcode"]);

            int nouser = 0;
            int isflg = 0;
            int noemp = 0;
            int isFace = 0;
            int isEss = 0;
            int isExp = 0;
            int isFplan = 0;
            int planid = 0;
            int rmDay = 0;
            int pDursn = 0;
            DataTable dtsub = RestClient.Restrictionasperplan(gp);
            if (dtsub != null && dtsub.Rows.Count > 0)
            {
                nouser = Convert.ToInt32(dtsub.Rows[0]["UseCount"]);
                isflg = Convert.ToInt32(dtsub.Rows[0]["islimit"]);
                noemp = Convert.ToInt32(dtsub.Rows[0]["empctn"]);
                isFace = Convert.ToInt32(dtsub.Rows[0]["isFace"]);
                isEss = Convert.ToInt32(dtsub.Rows[0]["isEss"]);
                isExp = Convert.ToInt32(dtsub.Rows[0]["Substs"]);
                isFplan = Convert.ToInt32(dtsub.Rows[0]["isFplan"]);
                planid = Convert.ToInt32(dtsub.Rows[0]["Planid"]);
                rmDay = Convert.ToInt32(dtsub.Rows[0]["Rmnday"]);
                pDursn = Convert.ToInt32(dtsub.Rows[0]["Pdrsn"]);
            }


            Session["UseCount"] = nouser;
            Session["IsUserlimit"] = isflg;
            Session["Noofemp"] = noemp;
            Session["isPlanFace"] = isFace;
            Session["isPlanEss"] = isEss;
            Session["isPlanExp"] = isExp;
            Session["isFplan"] = isFplan;
            Session["planId"] = planid;
            Session["rmDay"] = rmDay;
            Session["Pdrsn"] = pDursn;
        }
        //public JsonResult Getrestriction()
        //{
        //    JsonResult result;
        //    ResMsg resp = new ResMsg();
        //    try
        //    {
        //        int mcmpid = Convert.ToInt32(Session["CompanyId"].ToString());
        //        int nouser = 0;
        //        bool otherObjects = Checksubscription(mcmpid, out nouser);
        //        if (otherObjects)
        //        {
        //            resp.Msg = "You are rich maximum users limit of this plan";
        //            resp.MsgSts = "error";
        //        }
        //        result = Json(resp);
        //        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //        result.MaxJsonLength = Int32.MaxValue;
        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        result = Json(new SelectList("", "0"));
        //        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //        return result;
        //    }
        //}


        //public static bool Checksubscription(int mcmpid, out int nouser)
        //{
        //    var dtsubscrip = RestClient.Getmysubscription(mcmpid, 0);
        //    int Noofemp = 0;
        //    var subedt=string.Empty;
        //    if (dtsubscrip != null && dtsubscrip.Rows.Count > 0)
        //    {
        //        Noofemp = Convert.ToInt32(dtsubscrip.Rows[0]["NoOfEmployee"]);
        //        subedt = dtsubscrip.Rows[0]["EndDate"].ToString();
        //    }
        //    var emplctn = RestClient.EmployeeGetAll().Count();
        //    nouser = emplctn;
        //    if (emplctn >= Noofemp)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        [HttpPost]
        public JsonResult EmployeeMasterAjax(DataTableEmployeePostModel model)
        {
            //  JsonResult jsonresult = item;
            //var AllEmp = MasterRestClient.EmployeeGetAll().Where(x => x.BranchId == branchID).Select(x => x.EmpId).Distinct();
            var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
            if (model.roleid == 6805)
            {
                //AllData = AllData.Where(x => x.CompanyID == cmpid);
                model.cmpid = cmpid;
            }

            var AllData = RestClient.EmployeeGetAll(model).Select(i => new { i.EmpId, i.EmpName, i.UserId, i.EmpAddress, i.EmpPhNo, i.EmpMNo, i.RoleId, i.RoleName, i.Gender, i.Email, i.Password, i.Empcode, i.EmpDOB, i.EmpMarried, i.EmpJoinDate, i.EmpResignDate, i.EmpPunchID, i.EmpPhoto, i.MobNoSMS, i.CompanyID, i.CompanyName, i.BranchId, i.BranchName, i.DepartmentId, i.DepartmentName, i.DesignationId, i.DesignationName, i.CategoryId, i.CategoryName, i.TypeId, i.EmpTypeName, i.GradeId, i.ShiftId, i.ShiftName, i.ShiftShortName, i.ShiftGroupId, i.ShiftGroupName, i.ShiftGroupShortName, i.ContractorId, i.ContractorName, i.ReportingTo, i.PolicyId, i.PolicyName, i.EmpWeekOff, i.EmpSecondWeekOff, i.EmpSecondWeekOffRule, i.EmpHalfDay, i.EmpHalfDayRule, i.ReligionId, i.IsSMS, i.ModifyBy, i.ModifyDate, i.IsActive, i.Status, i.BirthDate, i.JoinDate, i.Married, i.EmpGender, i.TagId, i.CustomFields, i.Customvalues, i.geoenable, i.worf, i.CountryCode, i.BranchGeolocation });
            //DatatableCounts DataCount = RestClient.EmployeeGetAllCounts(model);


            var result = new List<Employees>(AllData.Count());
            //foreach (var data in AllData)
            //{
            //    // simple remapping adding extra info to found dataset
            //    result.Add(new Employees
            //    {
            //        EmpId = data.EmpId,
            //        EmpName = data.EmpName,
            //        UserId = data.UserId,
            //        EmpAddress = data.EmpAddress,
            //        EmpPhNo = data.EmpPhNo,
            //        EmpMNo = data.EmpMNo,
            //        RoleId = data.RoleId,
            //        RoleName = data.RoleName,
            //        Gender = data.Gender,
            //        Email = data.Email,
            //        Password = data.Password,
            //        Empcode = data.Empcode,
            //        EmpDOB = data.EmpDOB,
            //        EmpMarried = data.EmpMarried,
            //        EmpJoinDate = data.EmpJoinDate,
            //        EmpResignDate = data.EmpResignDate,
            //        EmpPunchID = data.EmpPunchID,
            //        EmpPhoto = data.EmpPhoto,
            //        MobNoSMS = data.MobNoSMS,
            //        CompanyID = data.CompanyID,
            //        CompanyName = data.CompanyName,
            //        BranchId = data.BranchId,
            //        BranchName = data.BranchName,
            //        DepartmentId = data.DepartmentId,
            //        DepartmentName = data.DepartmentName,
            //        DesignationId = data.DesignationId,
            //        DesignationName = data.DesignationName,
            //        CategoryId = data.CategoryId,
            //        CategoryName = data.CategoryName,
            //        TypeId = data.TypeId,
            //        EmpTypeName = data.EmpTypeName,
            //        GradeId = data.GradeId,
            //        ShiftId = data.ShiftId,
            //        ShiftName = data.ShiftName,
            //        ShiftShortName = data.ShiftShortName,
            //        ShiftGroupId = data.ShiftGroupId,
            //        ShiftGroupName = data.ShiftGroupName,
            //        ShiftGroupShortName = data.ShiftGroupShortName,
            //        ContractorId = data.ContractorId,
            //        ContractorName = data.ContractorName,
            //        ReportingTo = data.ReportingTo,
            //        PolicyId = data.PolicyId,
            //        PolicyName = data.PolicyName,
            //        EmpWeekOff = data.EmpWeekOff,
            //        EmpSecondWeekOff = data.EmpSecondWeekOff,
            //        EmpSecondWeekOffRule = data.EmpSecondWeekOffRule,
            //        EmpHalfDay = data.EmpHalfDay,
            //        EmpHalfDayRule = data.EmpHalfDayRule,
            //        ReligionId = data.ReligionId,
            //        IsSMS = data.IsSMS,
            //        ModifyBy = data.ModifyBy,
            //        ModifyDate = data.ModifyDate,
            //        IsActive = data.IsActive,
            //        Status = data.Status,
            //        Married = data.Married,
            //        BirthDate = data.BirthDate,
            //        JoinDate = data.JoinDate,
            //        EmpGender = data.EmpGender,
            //        TagId = data.TagId,
            //        CustomFields = data.CustomFields,
            //        Customvalues = data.Customvalues,
            //        geoenable = data.geoenable,
            //        worf = data.worf,
            //        CountryCode = data.CountryCode,
            //        BranchGeolocation = data.BranchGeolocation
            //    });
            //};


            return Json(new
            {
                // this is what datatables wants sending back
                //draw = model.draw,
                //recordsTotal = DataCount.totalResultsCount,
                //recordsFiltered = DataCount.filteredResultsCount,
                data = AllData

            });

        }

        [HttpPost]
        public JsonResult EmployeeMasterAjaxwithfilter(DataTableEmployeePostModel model)
        {
            //for employee  enroll in mfstab
            var branchid = model.roleid;
            var cmpid = model.loginempid;
            DatatableCounts DataCount = new DatatableCounts();
            var AllData = RestClient.EmployeeGetAllReporting().Select(i => new { i.EmpId, i.EmpName, i.UserId, i.EmpAddress, i.EmpPhNo, i.EmpMNo, i.RoleId, i.RoleName, i.Gender, i.Email, i.Password, i.Empcode, i.EmpDOB, i.EmpMarried, i.EmpJoinDate, i.EmpResignDate, i.EmpPunchID, i.EmpPhoto, i.MobNoSMS, i.CompanyID, i.CompanyName, i.BranchId, i.BranchName, i.DepartmentId, i.DepartmentName, i.DesignationId, i.DesignationName, i.CategoryId, i.CategoryName, i.TypeId, i.EmpTypeName, i.GradeId, i.ShiftId, i.ShiftName, i.ShiftShortName, i.ShiftGroupId, i.ShiftGroupName, i.ShiftGroupShortName, i.ContractorId, i.ContractorName, i.ReportingTo, i.PolicyId, i.PolicyName, i.EmpWeekOff, i.EmpSecondWeekOff, i.EmpSecondWeekOffRule, i.EmpHalfDay, i.EmpHalfDayRule, i.ReligionId, i.IsSMS, i.ModifyBy, i.ModifyDate, i.IsActive, i.Status, i.BirthDate, i.JoinDate, i.Married, i.EmpGender, i.TagId, i.CustomFields, i.Customvalues });
            DataCount.totalResultsCount = AllData.Count();
            if (!string.IsNullOrEmpty(model.search.value))
            {
                DataCount.filteredResultsCount = AllData.Where(x => x.BranchId == (branchid != -1 ? branchid : x.BranchId) && x.CompanyID == (cmpid != -1 ? cmpid : x.CompanyID) && x.EmpName.ToLower().Contains(model.search.value.ToLower()) || x.EmpPunchID.ToLower().Contains(model.search.value.ToLower()) || x.Email.ToLower().Contains(model.search.value.ToLower())).ToList().Count();
                AllData = AllData.Where(x => x.BranchId == (branchid != -1 ? branchid : x.BranchId) && x.CompanyID == (cmpid != -1 ? cmpid : x.CompanyID) && x.EmpName.ToLower().Contains(model.search.value.ToLower()) || x.EmpPunchID.ToLower().Contains(model.search.value.ToLower()) || x.Email.ToLower().Contains(model.search.value.ToLower())).ToList().Skip(model.start).Take(model.length);
            }
            else
            {
                DataCount.filteredResultsCount = AllData.Where(x => x.BranchId == (branchid != -1 ? branchid : x.BranchId) && x.CompanyID == (cmpid != -1 ? cmpid : x.CompanyID)).ToList().Count();
                AllData = AllData.Where(x => x.BranchId == (branchid != -1 ? branchid : x.BranchId) && x.CompanyID == (cmpid != -1 ? cmpid : x.CompanyID)).ToList().Skip(model.start).Take(model.length);
            }

            var result = new List<Employees>(AllData.Count());

            foreach (var data in AllData)
            {
                // simple remapping adding extra info to found dataset
                result.Add(new Employees
                {
                    EmpId = data.EmpId,
                    EmpName = data.EmpName,
                    UserId = data.UserId,
                    EmpAddress = data.EmpAddress,
                    EmpPhNo = data.EmpPhNo,
                    EmpMNo = data.EmpMNo,
                    RoleId = data.RoleId,
                    RoleName = data.RoleName,
                    Gender = data.Gender,
                    Email = data.Email,
                    Password = data.Password,
                    Empcode = data.Empcode,
                    EmpDOB = data.EmpDOB,
                    EmpMarried = data.EmpMarried,
                    EmpJoinDate = data.EmpJoinDate,
                    EmpResignDate = data.EmpResignDate,
                    EmpPunchID = data.EmpPunchID,
                    EmpPhoto = data.EmpPhoto,
                    MobNoSMS = data.MobNoSMS,
                    CompanyID = data.CompanyID,
                    CompanyName = data.CompanyName,
                    BranchId = data.BranchId,
                    BranchName = data.BranchName,
                    DepartmentId = data.DepartmentId,
                    DepartmentName = data.DepartmentName,
                    DesignationId = data.DesignationId,
                    DesignationName = data.DesignationName,
                    CategoryId = data.CategoryId,
                    CategoryName = data.CategoryName,
                    TypeId = data.TypeId,
                    EmpTypeName = data.EmpTypeName,
                    GradeId = data.GradeId,
                    ShiftId = data.ShiftId,
                    ShiftName = data.ShiftName,
                    ShiftShortName = data.ShiftShortName,
                    ShiftGroupId = data.ShiftGroupId,
                    ShiftGroupName = data.ShiftGroupName,
                    ShiftGroupShortName = data.ShiftGroupShortName,
                    ContractorId = data.ContractorId,
                    ContractorName = data.ContractorName,
                    ReportingTo = data.ReportingTo,
                    PolicyId = data.PolicyId,
                    PolicyName = data.PolicyName,
                    EmpWeekOff = data.EmpWeekOff,
                    EmpSecondWeekOff = data.EmpSecondWeekOff,
                    EmpSecondWeekOffRule = data.EmpSecondWeekOffRule,
                    EmpHalfDay = data.EmpHalfDay,
                    EmpHalfDayRule = data.EmpHalfDayRule,
                    ReligionId = data.ReligionId,
                    IsSMS = data.IsSMS,
                    ModifyBy = data.ModifyBy,
                    ModifyDate = data.ModifyDate,
                    IsActive = data.IsActive,
                    Status = data.Status,
                    Married = data.Married,
                    BirthDate = data.BirthDate,
                    JoinDate = data.JoinDate,
                    EmpGender = data.EmpGender,
                    TagId = data.TagId,
                    CustomFields = data.CustomFields,
                    Customvalues = data.Customvalues


                });
            };

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
        public JsonResult EmployeelstEnroll(DataTableEmployeePostModel model)
        {
            //for employee  enroll in mfstab
            var branchid = model.roleid;
            var cmpid = model.loginempid;
            var statusid = Convert.ToBoolean(model.selectstatus);
            DatatableCounts DataCount = new DatatableCounts();
            var AllData = RestClient.EmployeeGetAllEnroll().Select(i => new { i.EmpId, i.EmpName, i.UserId, i.EmpAddress, i.EmpPhNo, i.EmpMNo, i.RoleId, i.RoleName, i.Gender, i.Email, i.Password, i.Empcode, i.EmpDOB, i.EmpMarried, i.EmpJoinDate, i.EmpResignDate, i.EmpPunchID, i.EmpPhoto, i.MobNoSMS, i.CompanyID, i.CompanyName, i.BranchId, i.BranchName, i.DepartmentId, i.DepartmentName, i.DesignationId, i.DesignationName, i.CategoryId, i.CategoryName, i.TypeId, i.EmpTypeName, i.GradeId, i.ShiftId, i.ShiftName, i.ShiftShortName, i.ShiftGroupId, i.ShiftGroupName, i.ShiftGroupShortName, i.ContractorId, i.ContractorName, i.ReportingTo, i.PolicyId, i.PolicyName, i.EmpWeekOff, i.EmpSecondWeekOff, i.EmpSecondWeekOffRule, i.EmpHalfDay, i.EmpHalfDayRule, i.ReligionId, i.IsSMS, i.ModifyBy, i.ModifyDate, i.IsActive, i.Status, i.BirthDate, i.JoinDate, i.Married, i.EmpGender, i.TagId, i.CustomFields, i.Customvalues }).Where(x => x.IsActive == statusid);
            DataCount.totalResultsCount = AllData.Count();
            if (!string.IsNullOrEmpty(model.search.value))
            {
                DataCount.filteredResultsCount = AllData.Where(x => x.BranchId == (branchid != -1 ? branchid : x.BranchId) && x.CompanyID == (cmpid != -1 ? cmpid : x.CompanyID) && x.IsActive == statusid && (x.EmpName.ToLower().Contains(model.search.value.ToLower()) || x.EmpPunchID.ToLower().Contains(model.search.value.ToLower()) || x.Email.ToLower().Contains(model.search.value.ToLower()))).Count();
                AllData = AllData.Where(x => x.BranchId == (branchid != -1 ? branchid : x.BranchId) && x.CompanyID == (cmpid != -1 ? cmpid : x.CompanyID) && x.IsActive == statusid && (x.EmpName.ToLower().Contains(model.search.value.ToLower()) || x.EmpPunchID.ToLower().Contains(model.search.value.ToLower()) || x.Email.ToLower().Contains(model.search.value.ToLower()))).Skip(model.start).Take(model.length).ToList();

                //DataCount.filteredResultsCount = AllData.Where(x => x.BranchId == (branchid != -1 ? branchid : x.BranchId) && x.CompanyID == (cmpid != -1 ? cmpid : x.CompanyID) && x.IsActive == statusid && x.EmpName.ToLower().Contains(model.search.value.ToLower()) || x.EmpPunchID.ToLower().Contains(model.search.value.ToLower()) || x.Email.ToLower().Contains(model.search.value.ToLower())).ToList().Count();
                //AllData = AllData.Where(x => x.BranchId == (branchid != -1 ? branchid : x.BranchId) && x.CompanyID == (cmpid != -1 ? cmpid : x.CompanyID) && x.IsActive == statusid && x.EmpName.ToLower().Contains(model.search.value.ToLower()) || x.EmpPunchID.ToLower().Contains(model.search.value.ToLower()) || x.Email.ToLower().Contains(model.search.value.ToLower())).ToList().Skip(model.start).Take(model.length);
            }
            else
            {
                DataCount.filteredResultsCount = AllData.Where(x => x.BranchId == (branchid != -1 ? branchid : x.BranchId) && x.CompanyID == (cmpid != -1 ? cmpid : x.CompanyID) && x.IsActive == statusid).ToList().Count();
                AllData = AllData.Where(x => x.BranchId == (branchid != -1 ? branchid : x.BranchId) && x.CompanyID == (cmpid != -1 ? cmpid : x.CompanyID) && x.IsActive == statusid).ToList().Skip(model.start).Take(model.length);
            }

            var result = new List<Employees>(AllData.Count());

            foreach (var data in AllData)
            {
                // simple remapping adding extra info to found dataset
                result.Add(new Employees
                {
                    EmpId = data.EmpId,
                    EmpName = data.EmpName,
                    UserId = data.UserId,
                    EmpAddress = data.EmpAddress,
                    EmpPhNo = data.EmpPhNo,
                    EmpMNo = data.EmpMNo,
                    RoleId = data.RoleId,
                    RoleName = data.RoleName,
                    Gender = data.Gender,
                    Email = data.Email,
                    Password = data.Password,
                    Empcode = data.Empcode,
                    EmpDOB = data.EmpDOB,
                    EmpMarried = data.EmpMarried,
                    EmpJoinDate = data.EmpJoinDate,
                    EmpResignDate = data.EmpResignDate,
                    EmpPunchID = data.EmpPunchID,
                    EmpPhoto = data.EmpPhoto,
                    MobNoSMS = data.MobNoSMS,
                    CompanyID = data.CompanyID,
                    CompanyName = data.CompanyName,
                    BranchId = data.BranchId,
                    BranchName = data.BranchName,
                    DepartmentId = data.DepartmentId,
                    DepartmentName = data.DepartmentName,
                    DesignationId = data.DesignationId,
                    DesignationName = data.DesignationName,
                    CategoryId = data.CategoryId,
                    CategoryName = data.CategoryName,
                    TypeId = data.TypeId,
                    EmpTypeName = data.EmpTypeName,
                    GradeId = data.GradeId,
                    ShiftId = data.ShiftId,
                    ShiftName = data.ShiftName,
                    ShiftShortName = data.ShiftShortName,
                    ShiftGroupId = data.ShiftGroupId,
                    ShiftGroupName = data.ShiftGroupName,
                    ShiftGroupShortName = data.ShiftGroupShortName,
                    ContractorId = data.ContractorId,
                    ContractorName = data.ContractorName,
                    ReportingTo = data.ReportingTo,
                    PolicyId = data.PolicyId,
                    PolicyName = data.PolicyName,
                    EmpWeekOff = data.EmpWeekOff,
                    EmpSecondWeekOff = data.EmpSecondWeekOff,
                    EmpSecondWeekOffRule = data.EmpSecondWeekOffRule,
                    EmpHalfDay = data.EmpHalfDay,
                    EmpHalfDayRule = data.EmpHalfDayRule,
                    ReligionId = data.ReligionId,
                    IsSMS = data.IsSMS,
                    ModifyBy = data.ModifyBy,
                    ModifyDate = data.ModifyDate,
                    IsActive = data.IsActive,
                    Status = data.Status,
                    Married = data.Married,
                    BirthDate = data.BirthDate,
                    JoinDate = data.JoinDate,
                    EmpGender = data.EmpGender,
                    TagId = data.TagId,
                    CustomFields = data.CustomFields,
                    Customvalues = data.Customvalues
                });
            };

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
        public ActionResult EmployeeMaster(EmployeesExtended emp, HttpPostedFileBase EmpPhotoFile)
        {
            try
            {
                if (emp.SelectAll == 1)
                {
                    emp.BranchGeolocation = string.Empty;                   
                }
                else
                {
                    string[] branchgeolocationids = emp.BranchGeoLocations;
                    var strBranchGeoIds = String.Join(",", branchgeolocationids).Trim();
                    emp.BranchGeolocation = strBranchGeoIds;
                }
                emp.CountryCode = emp.CountryCode;
                emp.isPlanFace = Convert.ToInt32(Session["isPlanFace"]);
                ModelState.Remove("CompanyID");
                ViewBag.msg = "";
                ViewBag.error = "";
                var accountcode = Session["cmpcode"].ToString();
                //emp.IsEditPhoto = 1;
                if (!string.IsNullOrEmpty(emp.EmpphotoBase))
                {
                    emp.EmpphotoData = emp.EmpphotoBase;
                    byte[] imageBytes = Convert.FromBase64String(emp.EmpphotoBase);

                    //Save the Byte Array as Image File.
                    var FileName = Guid.NewGuid().ToString().Substring(0, 6);
                    var photoname = FileName + "_" + accountcode;
                    string filePath = Server.MapPath("~/UploadEmpPhoto/" + photoname + ".jpg");
                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                    emp.EmpPhoto = photoname + ".jpg";
                }
                else if (!string.IsNullOrEmpty(emp.EmpphotoData))
                {
                    emp.EmpphotoData = emp.EmpphotoData;
                    byte[] imageBytes = Convert.FromBase64String(emp.EmpphotoData);

                    //Save the Byte Array as Image File.
                    var FileName = Guid.NewGuid().ToString().Substring(0, 6);
                    var photoname = FileName + "_" + accountcode;
                    string filePath = Server.MapPath("~/UploadEmpPhoto/" + photoname + ".jpg");
                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                    emp.EmpPhoto = photoname + ".jpg";
                }
                else if (EmpPhotoFile != null)
                {
                    byte[] thePictureAsBytes = new byte[EmpPhotoFile.ContentLength];
                    using (BinaryReader theReader = new BinaryReader(EmpPhotoFile.InputStream))
                    {
                        thePictureAsBytes = theReader.ReadBytes(EmpPhotoFile.ContentLength);
                    }
                    var FileName = Guid.NewGuid().ToString().Substring(0, 6);
                    var photoname = FileName + "_" + accountcode;
                    string filePath = Server.MapPath("~/UploadEmpPhoto/" + photoname + ".jpg");
                    System.IO.File.WriteAllBytes(filePath, thePictureAsBytes);
                    emp.EmpPhoto = photoname + ".jpg";
                    emp.EmpphotoData = Convert.ToBase64String(thePictureAsBytes);
                }
                if (!string.IsNullOrEmpty(emp.CustomFields))
                {
                    emp.Customvalues = emp.Customvalues.Replace("C:\\fakepath\\", "");
                    var _CustomFields = emp.CustomFields.Split(',');
                    var _Customvalues = emp.Customvalues.Split(',');
                    for (var k = 0; k < _CustomFields.Length; k++)
                    {
                        var _filename = _CustomFields[k].ToString();
                        var _fileval = _Customvalues[k].ToString();
                        if (_filename.IndexOf("file-") != -1)
                        {
                            string path = Server.MapPath("~/FileuploadControlData/" + Session["cmpcode"] + "/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            if (!string.IsNullOrEmpty(_fileval))
                            {
                                _fileval = _fileval.Replace("C:\\fakepath\\", "");
                                var filePath = Server.MapPath("~/FileuploadControlData/" + Session["cmpcode"] + "") + "\\" + _fileval;
                                Request.Files["" + _filename + ""].SaveAs(filePath);
                            }

                        }
                    }
                }

                var Isdateformat = Session["IsDateFormat"];
                if (emp.EmpDOB != null)
                {
                    //emp.EmpDOB = emp.EmpDOB;
                    if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
                    {
                        var empDob = emp.EmpDOB.Split('-');
                        emp.EmpDOB = empDob[2] + "-" + empDob[1] + "-" + empDob[0];
                    }
                    else if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
                    {
                        var empDob = emp.EmpDOB.Split('-');
                        emp.EmpDOB = empDob[2] + "-" + empDob[0] + "-" + empDob[1];
                    }
                    else if (Isdateformat.ToString().Trim() == "yyyy-mm-dd")
                    {
                        var empDob = emp.EmpDOB.Split('-');
                        emp.EmpDOB = empDob[0] + "-" + empDob[1] + "-" + empDob[2];
                    }
                    else
                    {
                        emp.EmpDOB = emp.EmpDOB;
                    }
                }

                if (emp.EmpJoinDate != null)
                {
                    //emp.EmpJoinDate = emp.EmpJoinDate;
                    if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
                    {
                        var empJoindt = emp.EmpJoinDate.Split('-');
                        emp.EmpJoinDate = empJoindt[2] + "-" + empJoindt[1] + "-" + empJoindt[0];
                    }
                    else if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
                    {
                        var empJoindt = emp.EmpJoinDate.Split('-');
                        emp.EmpJoinDate = empJoindt[2] + "-" + empJoindt[0] + "-" + empJoindt[1];
                    }
                    else if (Isdateformat.ToString().Trim() == "yyyy-mm-dd")
                    {
                        var empJoindt = emp.EmpJoinDate.Split('-');
                        emp.EmpJoinDate = empJoindt[0] + "-" + empJoindt[1] + "-" + empJoindt[2];
                    }
                    else
                    {
                        emp.EmpJoinDate = emp.EmpJoinDate;
                    }
                    //emp.EmpJoinDate = Convert.ToDateTime(emp.EmpJoinDate).ToString("yyyy-MM-dd");
                }

                if (emp.EmpId > 0)
                {
                    if (emp.EmpResignDate != null)
                    {
                        if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
                        {
                            var empResignDt = emp.EmpResignDate.Split('-');
                            emp.EmpResignDate = empResignDt[2] + "-" + empResignDt[1] + "-" + empResignDt[0];
                        }
                        else if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
                        {
                            var empResignDt = emp.EmpResignDate.Split('-');
                            emp.EmpResignDate = empResignDt[2] + "-" + empResignDt[0] + "-" + empResignDt[1];
                        }
                        else if (Isdateformat.ToString().Trim() == "yyyy-mm-dd")
                        {
                            var empResignDt = emp.EmpResignDate.Split('-');
                            emp.EmpResignDate = empResignDt[0] + "-" + empResignDt[1] + "-" + empResignDt[2];
                        }
                        else if (Isdateformat.ToString().Trim() == "yyyy-M-dd")
                        {
                            var empResignDt = emp.EmpResignDate.Split('-');
                            emp.EmpResignDate = empResignDt[0] + "-" + empResignDt[1] + "-" + empResignDt[2];
                        }
                        else if (Isdateformat.ToString().Trim() == "M-dd-yyyy")
                        {
                            var empResignDt = emp.EmpResignDate.Split('-');
                            emp.EmpResignDate = empResignDt[2] + "-" + empResignDt[0] + "-" + empResignDt[1];
                        }
                        else if (Isdateformat.ToString().Trim() == "dd-M-yyyy")
                        {
                            var empResignDt = emp.EmpResignDate.Split('-');
                            emp.EmpResignDate = empResignDt[0] + "-" + empResignDt[1] + "-" + empResignDt[2];
                        }
                        emp.IsActive = false;
                    }
                    else
                    {
                        if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
                        {
                            if (emp.EmpResignDate != null)
                            {
                                var empResignDt = emp.EmpResignDate.Split('-');
                                emp.EmpResignDate = empResignDt[2] + "-" + empResignDt[1] + "-" + empResignDt[0];
                            }
                            else
                            {
                                var empResignDt = emp.EmpResignDate;
                            }
                        }
                        else if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
                        {
                            if (emp.EmpResignDate != null)
                            {
                                var empResignDt = emp.EmpResignDate.Split('-');
                                emp.EmpResignDate = empResignDt[2] + "-" + empResignDt[0] + "-" + empResignDt[1];
                            }
                            else
                            {
                                var empResignDt = emp.EmpResignDate;
                            }
                        }
                        else if (Isdateformat.ToString().Trim() == "yyyy-mm-dd")
                        {
                            if (emp.EmpResignDate != null)
                            {
                                var empResignDt = emp.EmpResignDate.Split('-');
                                emp.EmpResignDate = empResignDt[0] + "-" + empResignDt[1] + "-" + empResignDt[2];
                            }
                            else
                            {
                                var empResignDt = emp.EmpResignDate;
                            }
                        }
                        else if (Isdateformat.ToString().Trim() == "yyyy-M-dd")
                        {
                            if (emp.EmpResignDate != null)
                            {
                                var empResignDt = emp.EmpResignDate.Split('-');
                                emp.EmpResignDate = empResignDt[0] + "-" + empResignDt[1] + "-" + empResignDt[2];
                            }
                            else
                            {
                                var empResignDt = emp.EmpResignDate;
                            }
                        }
                        else if (Isdateformat.ToString().Trim() == "dd-M-yyyy")
                        {
                            if (emp.EmpResignDate != null)
                            {
                                var empResignDt = emp.EmpResignDate.Split('-');
                                emp.EmpResignDate = empResignDt[0] + "-" + empResignDt[1] + "-" + empResignDt[2];
                            }
                            else
                            {
                                var empResignDt = emp.EmpResignDate;
                            }
                        }
                        else if (Isdateformat.ToString().Trim() == "M-dd-yyyy")
                        {
                            if (emp.EmpResignDate != null)
                            {
                                var empResignDt = emp.EmpResignDate.Split('-');
                                emp.EmpResignDate = empResignDt[0] + "-" + empResignDt[1] + "-" + empResignDt[2];
                            }
                            else
                            {
                                var empResignDt = emp.EmpResignDate;
                            }
                        }
                        else
                        {
                            var empResignDt = emp.EmpResignDate;
                        }
                        emp.IsActive = true;
                    }
                    var res = RestClient.EmployeeUpdate(emp.EmpId, emp);
                    if (res.MegSts == "Ok")
                    {
                        ViewBag.msg = "Employee updated successfully.";
                    }
                    else
                    {
                        ViewBag.error = res.Meg;
                    }
                }
                else
                {
                    var res = RestClient.EmployeeAdd(emp);
                    if (res.MegSts == "Ok")
                    {
                        ViewBag.msg = "Employee added successfully.";
                    }
                    else
                    {
                        ViewBag.error = res.Meg;
                    }
                }

                //ViewBag.DepartmentList = FillDepartment();
                //ViewBag.DesignationList = FillDesignation();
                //ViewBag.ShiftList = FillShift();
                //ViewBag.ShiftGroupList = FillShiftGroup();
                //ViewBag.fillrole = FillRole();
                //ViewBag.fillcompanys = FillCompany();
                //ViewBag.fillReporting = FillEmployee();
                //ViewBag.PolicyName = RestClient.HRPolicyGetAll();
                //ViewBag.empdata = RestClient.EmployeeGetAll();
                //ViewBag.ReligionList = RestClient.ReligionGetAll();

                var _dtsys = RestClient.GetSystemSettingById(1);
                ViewBag.isReligion = Convert.ToBoolean(_dtsys.HasReligion.ToString() != "" ? _dtsys.HasReligion.ToString() : "0");
                ViewBag.HasSMS = Convert.ToBoolean(_dtsys.HasSMS.ToString() != "" ? _dtsys.HasSMS.ToString() : "0");
                ViewBag.IsEss = Convert.ToBoolean(_dtsys.IsEss.ToString() != "" ? _dtsys.IsEss.ToString() : "0");
                ViewBag.IsVerification = Convert.ToInt32(_dtsys.IsVerification);



                //ViewBag.isReligion = Convert.ToBoolean(RestClient.GetSystemSettingById(1).HasReligion.ToString() != "" ? RestClient.GetSystemSettingById(1).HasReligion.ToString() : "0");
                //ViewBag.HasSMS = Convert.ToBoolean(RestClient.GetSystemSettingById(1).HasSMS.ToString() != "" ? RestClient.GetSystemSettingById(1).HasSMS.ToString() : "0");
                //ViewBag.IsEss = Convert.ToBoolean(RestClient.GetSystemSettingById(1).IsEss.ToString() != "" ? RestClient.GetSystemSettingById(1).IsEss.ToString() : "0");
                //ViewBag.IsVerification = Convert.ToInt32(RestClient.GetSystemSettingById(1).IsVerification);
                planRestrictions(0);
                ViewBag.Islimit = Convert.ToInt32(Session["IsUserlimit"]);


                Session["actname"] = "Employee Master";

                if (Session["actname"] != null)
                {
                    TempData["frmname"] = Session["actname"].ToString();
                }

                IEnumerable<Cfc> lstobj = UtilitiesRestClient.GetAllcustomforms();

                ViewBag.lstcustomform = JsonConvert.SerializeObject(lstobj);
                DataTable model = UtilitiesRestClient.Getcustomformdata(Session["actname"].ToString());
                if (Session["RoleId"].ToString() != "1")
                {
                    var cmpanyid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    ViewBag.Companyslist = RestClient.CompanyGetAll().OrderBy(x => x.CompanyName).Where(x => x.CompanyID == cmpanyid);
                }
                else
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll().OrderBy(x => x.CompanyName);
                }

                ViewBag.DesignationList = FillDesignation();
                return View();
                //return RedirectToAction("EmployeeMaster","MasterData");
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }
        /// <summary>
        /// For the compress image method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static void CompressImage(string inputPath, string outputPath, int quality)
        {
            using (Bitmap image = new Bitmap(inputPath))
            {
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

                image.Save(outputPath, jpgEncoder, encoderParameters);
            }
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }


        [HttpPost]
        public string DeleteEmployeeMaster(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.EmployeeDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }


        //public string ExportDatatoExcel()
        //{
        //    Employees ems = new Employees();
        //    ems.EmployeeList = RestClient.EmployeeGetAll();


        //}

        //[HttpGet]
        //public async Task<JsonResult> GetEmpByBranchDept(string departmentid, string branchid, string isActiveEmployee)
        //{
        //    JsonResult result;
        //    try
        //    {
        //        var otherObjects = await RestClient.GetEmpByDept(departmentid, branchid, isActiveEmployee);
        //        // Now you can use OrderBy because employeesTask is no longer a Task but the actual result.
        //        var orderedEmployees = otherObjects.OrderBy(i => i.EmpName);
        //        result = Json(orderedEmployees, JsonRequestBehavior.AllowGet);
        //        result.MaxJsonLength = Int32.MaxValue;
        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        result = Json(new SelectList("", "0"));
        //        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //        return result;
        //    }
        //}
        [HttpGet]
        public async Task<JsonResult> GetEmpByBranchDept(EmployeeGetAllByDept objAllByDept)
        {
            JsonResult result;
            try
            {
                var otherObjects = await RestClient.GetEmpByDept(objAllByDept.departmentid, objAllByDept.branchid, objAllByDept.isActiveEmployee, objAllByDept.Empcode); // soumya 22-07-20225 add Empcode);
                // Now you can use OrderBy because employeesTask is no longer a Task but the actual result.
                var orderedEmployees = otherObjects.OrderBy(i => i.EmpName);
                result = Json(orderedEmployees, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = Int32.MaxValue;
                return result;
            }
            catch (Exception ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("GetEmpByBranchDept", ex.Message);
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

        [HttpGet]
        public JsonResult GetEmployeeEmail(string EmpEmail)
        {
            if (EmpEmail == "")
            {
                EmpEmail = string.Empty;
            }
            JsonResult result;
            ErrorMsg emg = new ErrorMsg();
            emg = RestClient.CheckEmployeeEmail(EmpEmail);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(emg);
            result = Json(json);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        [HttpGet]
        public JsonResult GetBeacon(string BeaconMac, int BranchId = 0)
        {
            if (BeaconMac == "")
            {
                BeaconMac = string.Empty;
            }
            JsonResult result;
            ErrorMsg emg = new ErrorMsg();
            emg = RestClient.CheckBeacon(BeaconMac, BranchId);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(emg);
            result = Json(json);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        [HttpGet]
        public JsonResult GetEmployeeCode(string Empcode)
        {
            if (Empcode == "")
            {
                Empcode = string.Empty;
            }
            JsonResult result;
            //ErrorMsg emg = new ErrorMsg();
            string str = RestClient.CheckEmployeeCode(Empcode);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(str);
            result = Json(json);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        [HttpGet]
        public JsonResult GetEmployeePunchId(string EmpPunchID)
        {
            if (EmpPunchID == "")
            {
                EmpPunchID = string.Empty;
            }
            JsonResult result;
            //ErrorMsg emg = new ErrorMsg();
            string str = RestClient.CheckEmployeePunchId(EmpPunchID);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(str);
            result = Json(json);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        public JsonResult EmployeeGetReporting()
        {
            JsonResult result;
            try
            {
                Employee emp = new Employee();
                var getEmpReporting = RestClient.EmployeeGetAllReporting();
                //result = Json(getEmpReporting);
                //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                //return result;

                result = Json(getEmpReporting);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.MaxJsonLength = int.MaxValue;

                string jsonData = JsonConvert.SerializeObject(getEmpReporting);
                byte[] compressedData = CompressionUtils.Compress(jsonData);
                return Json(Convert.ToBase64String(compressedData), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        #endregion

        #region "Leave Type Master"

        [HttpGet]
        public ActionResult LeaveTypeMaster()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            LeaveTypeMaster leave = new LeaveTypeMaster();
            try
            {
                leave.LeaveTypelist = RestClient.LeaveTypeGetAll();
                return View(leave);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }

        [HttpPost]
        public ActionResult LeaveTypeMaster(LeaveTypeMaster r)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ViewBag.msg = "";
                    ViewBag.error = "";
                    r.LeaveTypeName = r.LeaveTypeName.ToUpper();
                    if (r.LeaveTypeId > 0)
                    {
                        //r.LeaveTypelist = RestClient.LeaveTypeGetAll().Where(d => d.LeaveTypeName.ToLower() == r.LeaveTypeName.ToLower() && d.LeaveTypeId != r.LeaveTypeId);
                        r.LeaveTypelist = RestClient.LeaveTypeGetAll(r.LeaveTypeName, Convert.ToString(r.LeaveTypeId));
                        if (r.LeaveTypelist.Count() > 0)
                        {
                            ViewBag.error = r.LeaveTypeName + " Leave type already exists.";
                        }
                        else
                        {
                            if (!RestClient.LeaveTypeUpdate(r.LeaveTypeId, r))
                            {
                                ViewBag.msg = "Leave type not updated due to service issue.";
                            }
                            else
                            {
                                ViewBag.msg = "Leave type updated successfully.";
                            }
                        }
                    }
                    else
                    {
                        r.LeaveTypelist = RestClient.LeaveTypeGetAll();
                        if (r.LeaveTypelist.Count() > 0)
                        {
                            //r.LeaveTypelist = RestClient.LeaveTypeGetAll().Where(d => d.LeaveTypeName.ToLower() == r.LeaveTypeName.ToLower());
                            r.LeaveTypelist = RestClient.LeaveTypeGetAll(r.LeaveTypeName, "0");
                            if (r.LeaveTypelist.Count() > 0)
                            {
                                ViewBag.error = r.LeaveTypeName + " Leave type already exists.";
                            }
                            else
                            {
                                if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                                {
                                    r.CompanyID = 0;
                                    r.BranchID = 0;
                                }
                                else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                                {
                                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                                    var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                                    r.CompanyID = cmpid;
                                    r.BranchID = BranchID;
                                }
                                RestClient.LeaveTypeAdd(r);
                                ViewBag.msg = "Leave type added successfully.";
                            }
                        }
                        else
                        {
                            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                            {
                                r.CompanyID = 0;
                                r.BranchID = 0;
                            }
                            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                            {
                                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                                r.CompanyID = cmpid;
                                r.BranchID = BranchID;
                            }

                            RestClient.LeaveTypeAdd(r);
                            ViewBag.msg = "Leave type added successfully.";
                        }
                    }

                }
                else
                {
                    ViewBag.error = "Not valid entry.";
                }

                r.LeaveTypelist = RestClient.LeaveTypeGetAll();
                return View(r);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [HttpPost]
        public string DeleteLeaveType(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.LeaveTypeDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }
        #endregion

        #region DeviceMaster

        public ActionResult DeviceMaster()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            var compcode = Session["cmpcode"].ToString();
            Devices dvs = new Devices();
            try
            {
                dvs.DeviceTypeList = RestClient.DeviceTypeGetAll();
                int[] ids = new List<string>(ConfigurationManager.AppSettings["lst_devicetypeid"].Split(',')).ConvertAll<int>(s => int.Parse(s)).ToArray();
                ViewBag.DeviceTypelst = dvs.DeviceTypeList.Where(item => ids.Contains(item.DeviceTypeCode));
                ViewBag.CompanyList = FillCompany();
                dvs.DeviceList = RestClient.DeviceGetAllGrid(compcode);
                var RoleId = Session["RoleId"].ToString();
                var cmpid = 0;
                var BranchID = 0;

                #region For FastTrack DealCom DeviceMaster Add Filed
                if (Session["DbName"] != null)
                {
                    string _clientDb = Session["DbName"].ToString();
                    if (!string.IsNullOrEmpty(_clientDb))
                    {
                        if (!string.IsNullOrEmpty(_clientDb))
                        {
                            bool isMatch = Helper.ClientDBNameCheck.IsDbNameMatch(_clientDb);
                            if (isMatch)
                            {
                                ViewBag.DevicePunchType = isMatch;
                            }
                            else
                            {
                                ViewBag.DevicePunchType = isMatch;
                            }
                        }
                    }
                }
                #endregion

                if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                {
                    //var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    //var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                    cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    BranchID = Convert.ToInt32(Session["BranchId"].ToString());

                    //if (Session["RoleId"].ToString() == "6805")
                    //{
                    //    dvs.DeviceList = RestClient.DeviceGetAllGrid(compcode).Where(x => x.CompanyId == cmpid);
                    //}
                    //else if (Session["RoleId"].ToString() == "6806")
                    //{
                    //    dvs.DeviceList = RestClient.DeviceGetAllGrid(compcode).Where(x => x.CompanyId == cmpid && x.BranchId == BranchID);
                    //}
                    dvs.DeviceList = RestClient.DeviceGetAllGrid(compcode, RoleId, cmpid, BranchID);
                }
                else if (Convert.ToInt32(Session["RoleId"].ToString()) > 1 && Session["RoleId"].ToString() != "6806" && Session["RoleId"].ToString() != "6805")
                {
                    //var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    //var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                    cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                    //dvs.DeviceList = RestClient.DeviceGetAllGrid(compcode).Where(x => x.CompanyId == cmpid && x.BranchId == BranchID);
                    dvs.DeviceList = RestClient.DeviceGetAllGrid(compcode, RoleId, cmpid, BranchID);
                }



                return View(dvs);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        [HttpPost]
        public ActionResult DeviceMaster(Devices dv, string IsDeviceSrNo)
        {
           
            var compcode = Session["cmpcode"].ToString();
            try
            {
                if (Convert.ToInt32(Session["IsSchool"]) == 1)
                {
                    dv.IsSchool = true;
                }
                ViewBag.msg = "";
                ViewBag.error = "";
                int n;
                bool isNumeric = int.TryParse(dv.DeviceCode, out n);
                if (isNumeric)
                {
                    if (dv.DeviceId > 0)
                    {
                        dv.IsActive = true;
                        dv.IsPushData = 1;
                        dv.RegCompanyCode = Session["cmpcode"].ToString();
                        var jsonSerialiser = new JavaScriptSerializer();
                        var json = jsonSerialiser.Serialize(dv);
                        ErrorMsg ms = new ErrorMsg();
                        ms = RestClient.DeviceUpdate(dv.DeviceId, dv);
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
                        dv.DeviceList = RestClient.DeviceGetAll(dv.DeviceCode);
                        if (dv.DeviceList.Count() > 0)
                        {
                            ViewBag.error = dv.DeviceCode + " Device code already exists.";
                        }
                        else
                        {
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
                        }
                    }
                }
                else
                {
                    ViewBag.error = dv.DeviceCode + " is not valid device code, Please enter numeric values.";
                }


                dv.DeviceTypeList = RestClient.DeviceTypeGetAll();
                int[] ids = new List<string>(ConfigurationManager.AppSettings["lst_devicetypeid"].Split(',')).ConvertAll<int>(s => int.Parse(s)).ToArray();
                ViewBag.DeviceTypelst = dv.DeviceTypeList.Where(item => ids.Contains(item.DeviceTypeCode));
                ViewBag.CompanyList = FillCompany();
                var RoleId = Session["RoleId"].ToString();
                if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                {
                    dv.DeviceList = RestClient.DeviceGetAllGrid(compcode);
                }
                else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    var BranchID = Convert.ToInt32(Session["BranchId"].ToString());


                    //dv.DeviceList = RestClient.DeviceGetAllGrid(compcode).Where(x => x.CompanyId == cmpid);

                    //if (Session["RoleId"].ToString() == "6806")
                    //{
                    //    dv.DeviceList = RestClient.DeviceGetAllGrid(compcode).Where(x => x.CompanyId == cmpid && x.BranchId == BranchID);
                    //}

                    dv.DeviceList = RestClient.DeviceGetAllGrid(compcode, RoleId, cmpid, BranchID);
                }

                return View(dv);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        [HttpPost]
        public string DeleteDeviceMaster(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.DeviceDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";

                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }

        [HttpPost]
        public string ActiveDeActiveDeviceMaster(AcDevice obj)
        {
            string sts = "";
            if (obj.id != null)
            {
                if (RestClient.DeviceDeActiveActive(obj))
                {
                    sts = "ok";
                }
                else
                {
                    sts = "error";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
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
            ErrorMsg emg = new ErrorMsg();
            emg = RestClient.CheckDeviceInfo(DeviceSrNo, DeviceCode);

            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(emg);
            result = Json(json);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }


        [HttpGet]
        public JsonResult GetDevicebyId(string id)
        {
            JsonResult result;
            int DeviceID = Convert.ToInt32(id);
            Devices dv = new Devices();
            dv.DeviceList = RestClient.DeviceGetByid(DeviceID);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(dv.DeviceList);
            result = Json(json);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        #endregion

        #region PalmEnrollment
        public ActionResult PalmEnrollment()
        {
            return View();
        }
        #endregion

        #region ScheduleMaster
        public ActionResult ScheduleMaster()
        {
            ViewBag.Companylist = RestClient.CompanyGetAll();
            ViewBag.BranchList = RestClient.BranchGetAll();
            Scheduler sa = new Scheduler();

            sa.ScheduleActionlist = RestClient.ScheduleActionGetAll();
            return View(sa);
        }
        #endregion

        #region TransactionYear Master

        [HttpGet]
        public ActionResult TransactionYearMaster()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            TransactionYear tl = new TransactionYear();

            try
            {
                tl.TransactionYearList = RestClient.TransactionYearGetAll();
                ViewBag.TransactionYearGetAll = RestClient.TransactionYearGetAll();
                ViewBag.LastTodate = RestClient.TransactionYearGetAll().Select(x => x.ToDate).LastOrDefault();
                return View(tl);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        [HttpPost]
        public ActionResult TransactionYearMaster(TransactionYear tl)
        {
            try
            {
                ViewBag.msg = "";
                ViewBag.error = "";
                if (tl.TransactionYearId > 0)
                {
                    tl.IsActive = true;
                    if (!RestClient.TransactionYearUpdate(tl.TransactionYearId, tl))
                    {

                        ViewBag.msg = "Transaction year not updated due to service issue.";
                    }
                    else
                    {

                        ViewBag.msg = "Transaction year is updated.";
                    }
                }
                else
                {
                    tl.FromYear = DateTime.Parse(tl.FromDate).Year;
                    tl.ToYear = DateTime.Parse(tl.ToDate).Year;
                    tl.TransactionYearList = RestClient.TransactionYearGetAll().Where(c => c.TransactionYearName.ToLower() == tl.TransactionYearName.ToLower());
                    if (tl.TransactionYearList.Count() > 0)
                    {

                        ViewBag.error = tl.TransactionYearName + " name already exists.";
                    }
                    else
                    {
                        RestClient.TransactionYearCreate(tl);
                        ViewBag.msg = "Transaction year added successfully.";
                    }
                }

                tl.TransactionYearList = RestClient.TransactionYearGetAll();
                ViewBag.TransactionYearGetAll = RestClient.TransactionYearGetAll();
                ViewBag.LastTodate = RestClient.TransactionYearGetAll().Select(x => x.ToDate).LastOrDefault();
                return View(tl);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }
        [HttpPost]
        public string DeleteTransactionYearMaster(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.TransactionYearDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }

        #endregion

        #region AttendanceParameterBranch

        [HttpPost]
        public string AttendanceParaCreate(string AttndParameter)
        {
            string ad = "";
            ViewBag.msg = "";
            ViewBag.error = "";
            JavaScriptSerializer js = new JavaScriptSerializer();
            AttendanceParameter objattpara = js.Deserialize<AttendanceParameter>(AttndParameter);
            if (objattpara.AttendanceParameterId > 0)
            {
                objattpara.IsActive = true;
                if (!UtilitiesRestClient.AttendanceParameterUpdate(objattpara.AttendanceParameterId, objattpara))
                {
                    ad = "Attendance Parameter not updated due to service issue.";
                }
                else
                {
                    ad = "Attendance Parameter is updated.";
                }
            }
            else
            {
                ad = UtilitiesRestClient.AttendanceParameterCreate(objattpara);
            }

            return ad;
            // return RedirectToAction("BranchMaster", "MasterData");
        }

        [HttpGet]
        public JsonResult GetAttendanceParameterIDByBranchID(int Branchid)
        {
            JsonResult result;
            var data = UtilitiesRestClient.GetAttendanceParaIDbyBranchID(Branchid);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(data);
            result = Json(json);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        #endregion

        #region SystemSetting/BranchSetting

        [HttpGet]
        public JsonResult GetSettingIDbyCompanyID(int comid)
        {
            JsonResult result;
            CompanySetting cs = new CompanySetting();
            try
            {
                cs.CompanySettinglist = RestClient.CompanySettingGetAll();
                List<CompanySetting> lst = new List<CompanySetting>();
                foreach (var i in cs.CompanySettinglist)
                {
                    if (i.CompanyId == comid)
                    {
                        lst.Add(i);
                    }
                }
                if (lst.Count == 0)
                {
                    CompanySetting cst = new CompanySetting();
                    cst.SettingId = 0;
                    cst.HasMultipleCompany = true;
                    cst.HasMultipleBranch = true;
                    cst.HasDepartment = true;
                    cst.HasDesignation = true;
                    cst.HasShift = true;
                    cst.HasHoliday = true;
                    cst.HasLeave = true;
                    cst.HasReligion = true;
                    cst.IsLocation = true;
                    cst.HasSMS = false;
                    cst.IsEss = true;
                    cst.IsMorxAutoSync = false;
                    cst.IsFace = false;
                    cst.IsBeacon = false;
                    cst.IsBranchGeoFence = false;
                    lst.Add(cst);
                }
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(lst);
                result = Json(json);
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

        [HttpPost]
        public JsonResult CompanySettingCreateUpdate(int SettingID, int CompanyID, string SettingName, bool HasMultipleCompany,
            bool HasDepartment, bool HasMultipleBranch, bool HasDesignation, bool HasShift, bool HasHoliday, bool HasLeave, bool HasReligion, int MonthlyRptStaDay,
            int ServiceStartTime, int SchedularStartTime, bool IsLocation, bool HasSMS, bool IsEss, bool IsMorxAutoSync, bool IsFace, bool IsBeacon, bool IsBranchGeoFence,
            string IsDateFormat, bool IsApprovalForWebpunch, bool IsDeviceTimeZone, string CountryCode, string AreaCode, bool IsVerification, bool IsFlexi, bool IsOTP,
            string lockAttendanceDay, string AttendanceCorrection, int CurrancyType, string approvalDayLimitForAttendance, bool IsGatePass)
        {
            JsonResult result;
            var data = "";
            try
            {
                CompanySetting cmp = new CompanySetting();
                cmp.SettingId = SettingID;
                cmp.CompanyId = CompanyID;
                cmp.SettingName = SettingName;
                cmp.HasMultipleCompany = HasMultipleCompany;
                cmp.HasDepartment = HasDepartment;
                cmp.HasMultipleBranch = HasMultipleBranch;
                cmp.HasDesignation = HasDesignation;
                cmp.HasShift = HasShift;
                cmp.HasHoliday = HasHoliday;
                cmp.HasLeave = HasLeave;
                cmp.HasReligion = HasReligion;
                cmp.ServiceStartTime = ServiceStartTime;
                cmp.MonthlyRptStaDay = MonthlyRptStaDay;
                cmp.SchedularStartTime = SchedularStartTime;
                cmp.IsActive = true;
                cmp.IsLocation = IsLocation;
                cmp.HasSMS = HasSMS;
                cmp.IsEss = IsEss;
                cmp.IsMorxAutoSync = IsMorxAutoSync;
                cmp.IsFace = IsFace;
                cmp.IsBeacon = IsBeacon;
                cmp.IsBranchGeoFence = IsBranchGeoFence;
                cmp.ISDateFormat = IsDateFormat;
                cmp.IsApprovalForWebpunch = IsApprovalForWebpunch;
                cmp.IsDeviceTimeZone = IsDeviceTimeZone;
                cmp.IsVerification = IsVerification;
                cmp.IsFlexi = IsFlexi;
                cmp.IsOTP = IsOTP;
                cmp.lockAttendanceDay = lockAttendanceDay;
                cmp.AttendanceCorrection = AttendanceCorrection;
                cmp.CurrancyType = CurrancyType;
                cmp.DaysLimitForAttendanceApproval = approvalDayLimitForAttendance;

                cmp.IsGatePassActivate = IsGatePass;

                if (IsDeviceTimeZone == true)
                {
                    Session["IsDeviceTimeZone"] = 1;
                }
                else
                {
                    Session["IsDeviceTimeZone"] = 0;
                }
                cmp.CountryCode = CountryCode;
                cmp.AreaCode = AreaCode;
                if (string.IsNullOrEmpty(CountryCode))
                {
                    Session["CountryCode"] = "91";
                }
                else
                {
                    Session["CountryCode"] = CountryCode;
                }
                if (string.IsNullOrEmpty(IsDateFormat))
                {
                    Session["IsDateFormat"] = "yyyy-mm-dd";
                }
                else
                {
                    Session["IsDateFormat"] = IsDateFormat;
                }
                Session["AreaCode"] = AreaCode;
                Session["lockAttendanceDay"] = lockAttendanceDay;
                if (SettingID > 0)
                {
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(cmp);
                    if (!RestClient.CompanySettingUpdate(cmp.SettingId, cmp))
                    {
                        //Response.Write("<script>alert('Company Setting not updated due to service issue.')</script>");
                        data = "Company Setting not updated due to service issue.";
                    }
                    else
                    {
                        //Response.Write("<script>alert('Company Setting is updated.')</script>");
                        data = "Company Setting is updated.";
                    }
                }
                else
                {
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(cmp);
                    RestClient.CompanySettingAdd(cmp);
                    RestClient.GetMenuRoleWise(Convert.ToInt32(Session["RoleId"]));
                    //Response.Write("<script>alert('Company Setting save Sucessfully.')</script>");
                    data = "Company Setting save Sucessfully.";
                }
                //error code of json data
                //data = "Company Setting added successfully.";
                result = Json(new { data = data });
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
        public JsonResult GetBranchSettingIDbyBranchID(int brid)
        {
            JsonResult result;
            BranchSetting bs = new BranchSetting();
            try
            {
                bs.BranchSettinglist = RestClient.BranchSettingGetAll();
                List<BranchSetting> lst = new List<BranchSetting>();
                foreach (var i in bs.BranchSettinglist)
                {
                    if (i.BranchId == brid)
                    {
                        lst.Add(i);
                    }
                }
                if (lst.Count == 0)
                {
                    BranchSetting cst = new BranchSetting();
                    cst.BranchSettingId = 0;
                    lst.Add(cst);
                }
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(lst);
                result = Json(json);
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

        [HttpPost]
        public JsonResult BranchSettingCreateUpdate(int BranchSettingID, string BranchSettingName, int CompanyId, int BranchId, string TimeZone, string Language,
            string Currency, string DatetimeFormat, int TrancationYearId)
        {

            JsonResult result;
            var data = "";
            try
            {
                BranchSetting brs = new BranchSetting();
                brs.BranchSettingId = BranchSettingID;
                //    brs.BranchSettingName = BranchSettingName;
                brs.CompanyId = CompanyId;
                brs.BranchId = BranchId;
                brs.TimeZone = TimeZone;
                brs.Language = Language;
                brs.Currency = Currency;
                brs.DatetimeFormat = DatetimeFormat;
                brs.TrancationYearId = TrancationYearId;
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(brs);
                if (BranchSettingID > 0)
                {
                    if (!RestClient.BranchSettingUpdate(brs.BranchSettingId, brs))
                    {
                        Response.Write("<script>alert('Branch Setting not updated due to service issue.')</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Branch Setting is updated.')</script>");
                    }
                }
                else
                {
                    RestClient.BranchSettingAdd(brs);
                    Response.Write("<script>alert('Branch Setting added successfully.')</script>");
                }
                //error code of json data
                data = "Branch Setting added successfully.";
                result = Json(data);
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


        [HttpPost]
        public string EmployeemasterGeoFenceupdate(string BranchGeoFenceid)
        {
            string sts = "";
            if (BranchGeoFenceid != null)
            {
                if (RestClient.EmployeemasterGeoFenceupdate(Convert.ToInt32(BranchGeoFenceid.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }

        #endregion

        #region ReligionMaster
        public ActionResult ReligionMaster()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            Religion cmp = new Religion();
            try
            {
                if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                {
                    cmp.religionList = RestClient.ReligionGetAll();
                }
                else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                    cmp.religionList = RestClient.ReligionGetAll();
                }

                return View(cmp);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }


        [HttpPost]
        public ActionResult ReligionMaster(Religion r)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ViewBag.msg = "";
                    ViewBag.error = "";
                    if (r.ReligionId > 0)
                    {
                        r.IsActive = true;
                        r.religionList = RestClient.ReligionGetAll().Where(c => c.ReligionName.ToLower() == r.ReligionName.ToLower() && c.ReligionId != r.ReligionId);
                        if (r.religionList.Count() > 0)
                        {
                            ViewBag.error = r.ReligionName + " Religion already exists.";
                        }
                        else
                        {
                            if (!RestClient.ReligionUpdate(r.ReligionId, r))
                            {
                                ViewBag.error = "Religion not updated.";
                            }
                            else
                            {
                                ViewBag.msg = "Religion updated successfully.";
                            }
                        }
                    }
                    else
                    {
                        r.religionList = RestClient.ReligionGetAll().Where(c => c.ReligionName.ToLower() == r.ReligionName.ToLower());
                        if (r.religionList.Count() > 0)
                        {
                            ViewBag.error = r.ReligionName + " Religion already exists.";
                        }
                        else
                        {

                            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                            {
                                r.CompanyID = 0;
                                r.BranchID = 0;
                            }
                            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                            {
                                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                                r.CompanyID = cmpid;
                                r.BranchID = BranchID;
                            }

                            RestClient.ReligionAdd(r);
                            ViewBag.msg = "Religion added successfully.";
                        }
                    }
                }
                else
                {
                    ViewBag.error = "Not valid entry.";
                }
                r.religionList = RestClient.ReligionGetAll();
                return View(r);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [HttpPost]
        public string DeleteReligion(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.ReligionDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }
        #endregion

        #region SMS Configuration
        [HttpGet]
        public ActionResult Smsconfiguration()
        {
            return View();
        }

        #endregion

        #region Scheduler Settings
        [HttpGet]
        public ActionResult Schedulersettings()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            //ViewBag.CompanyList = FillCompany();
            ViewBag.Companyslist = RestClient.CompanyGetAll();
            ViewBag.BranchFill = FillBranch();
            try
            {
                Scheduler ss = new Scheduler();

                ss.Schedulersettingslist = RestClient.SchedulersettingsGetAll();
                ss.Schedulersettingslist = ss.Schedulersettingslist.Where(x => x.tssUserId == Convert.ToInt32(Session["UserId"]));
                return View(ss);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        [HttpGet]
        [CustAuthFilter]
        public JsonResult FillSchedulerbyID(int id)
        {
            JsonResult result;
            try
            {
                var i = RestClient.GetSchedulerDetails(id);
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(i);
                result = Json(json);
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

        [HttpPost]
        public string DeleteScheduler(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.SchedulersettingsDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
          
        }

        [HttpPost]
        public string ActiveInactiveScheduler(string id, bool isactive)
        {
            string sts = "";
            if (id != null)
            {
                var h = RestClient.GetSchedulerDetails(Convert.ToInt32(id));
                if (h.tssWeekdayarr != null)
                {
                    h.tssWeekday = string.Join(",", h.tssWeekdayarr);
                }
                if (h.tssMothdaynoarr != null)
                {
                    h.tssMonthday = string.Join(",", h.tssMothdaynoarr);
                }
                if (h.tssmonthdayarr != null)
                {
                    h.tssMonthdayrule = string.Join(",", h.tssmonthdayarr);
                }
                if (h.tssMonthsarr != null)
                {
                    h.tssmonths = string.Join(",", h.tssMonthsarr);
                }
                if (h.tssMonthsdatearr != null)
                {
                    h.tssMothdayno = string.Join(",", h.tssMonthsdatearr);
                }
                if (h.DepartmentIds != null)
                {
                    h.DepartmentId = string.Join(",", h.DepartmentIds);
                }
                h.DepartmentId = Regex.Replace(h.DepartmentId, @"\s", "");
                h.saActionname = "Attendance Process";
                h.tssIsActive = isactive;
                //if (RestClient.SchedulersettingsActiveInactive(Convert.ToInt32(id.Trim()),isactive))
                if (!RestClient.SchedulersettingsUpdate(Convert.ToInt32(id), h))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
            //string ReturnURL = "/MasterData/Schedulersettings/";
            //string ReturnURL = Url.Action("Schedulersettings", "MasterData");
            //return Json(ReturnURL);
        }

        [HttpPost]
        public ActionResult Schedulersettings(Scheduler h)
        {
            try
            {
                if (h.tssWeekdayarr != null)
                {
                    h.tssWeekday = string.Join(",", h.tssWeekdayarr);
                }
                if (h.tssMothdaynoarr != null)
                {
                    h.tssMonthday = string.Join(",", h.tssMothdaynoarr);
                }
                if (h.tssmonthdayarr != null)
                {
                    h.tssMonthdayrule = string.Join(",", h.tssmonthdayarr);
                }
                if (h.tssMonthsarr != null)
                {
                    h.tssmonths = string.Join(",", h.tssMonthsarr);
                }
                if (h.tssMonthsdatearr != null)
                {
                    h.tssMothdayno = string.Join(",", h.tssMonthsdatearr);
                }
                if (h.DepartmentIds != null)
                {
                    h.DepartmentId = string.Join(",", h.DepartmentIds);
                }
                h.DepartmentId = Regex.Replace(h.DepartmentId, @"\s", "");
                h.saActionname = "Attendance Process";
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(h);
                ViewBag.msg = "";
                ViewBag.error = "";
                if (h.tssId > 0)
                {
                    h.tssModifyby = 0;
                    h.tssModifydate = Convert.ToDateTime("1899-12-30");
                    if (!RestClient.SchedulersettingsUpdate(h.tssId, h))
                    {
                        ViewBag.msg = "Scheduler settings not updated due to service issue.";
                    }
                    else
                    {
                        ViewBag.msg = "Scheduler setting  is updated.";
                    }
                }
                else
                {
                    var hls = RestClient.SchedulersettingsGetAll().Where(c => c.tssName.ToLower() == h.tssName.ToLower());
                    if (hls.Count() > 0)
                    {
                        ViewBag.error = h.tssName + " Scheduler setting already exists.";
                    }
                    else
                    {
                        h.tssIsActive = true;
                        var res = RestClient.SchedulersettingsAdd(h);
                        if (res.MegSts.ToLower() == "ok")
                        {
                            ViewBag.msg = "Scheduler setting created successfully.";
                        }
                        else
                        {
                            ViewBag.error = res.Meg;
                        }

                    }
                }

                ViewBag.Companyslist = RestClient.CompanyGetAll();
                ViewBag.BranchFill = FillBranch();
                h.Schedulersettingslist = RestClient.SchedulersettingsGetAll();
                h.Schedulersettingslist = h.Schedulersettingslist.Where(x => x.tssUserId == Convert.ToInt32(Session["UserId"]));
                return View(h);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [HttpGet]
        public JsonResult CheckIsSchedulerExist(string Name)
        {
            JsonResult result;
            bool IsExist = false;
            try
            {
                if (Name != string.Empty)
                {
                    var Schedulersettings = RestClient.SchedulersettingsGetAll().Where(c => c.tssName.ToLower() == Name.ToLower());
                    if (Schedulersettings.Count() > 0)
                    {
                        IsExist = true;
                    }
                    result = Json(new { IsExist = IsExist });
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    return result;
                }
                else
                {
                    result = Json(new { IsExist = false });
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    return result;
                }

            }
            catch (Exception)
            {
                result = Json(new { Isconfigured = true });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

        #endregion

        #region SelectList
        //public List<SelectListItem> FillDepartment()
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    Departments dp = new Departments();
        //    if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
        //    {
        //        dp.Departmentslist = RestClient.DepartmentGetAll();
        //    }
        //    else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
        //    {
        //        var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
        //        var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
        //        if (Session["RoleId"].ToString() == "6806")
        //        {
        //            dp.Departmentslist = RestClient.DepartmentGetAll().Where(x => (x.CompanyID == cmpid && (x.BranchID == BranchID || x.BranchID == 0)) || (x.CompanyID == 0)).ToList();
        //        }
        //        else
        //        {
        //            dp.Departmentslist = RestClient.DepartmentGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0).ToList();
        //        }
        //    }
        //    foreach (var i in dp.Departmentslist)
        //    {
        //        list.Add(new SelectListItem() { Text = i.DepartmentName, Value = Convert.ToString(i.DepartmentId) });
        //    }
        //    return list;
        //}
        //public List<SelectListItem> FillDesignation()
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    Designations ds = new Designations();
        //    if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
        //    {
        //        ds.Designationlist = RestClient.DesignationsGetAll();
        //    }
        //    else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
        //    {
        //        var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
        //        var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
        //        if (Session["RoleId"].ToString() == "6806")
        //        {
        //            ds.Designationlist = RestClient.DesignationsGetAll().Where(x => (x.CompanyID == cmpid && (x.BranchID == BranchID || x.BranchID == 0)) || (x.CompanyID == 0)).ToList();
        //        }
        //        else
        //        {
        //            ds.Designationlist = RestClient.DesignationsGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0).ToList();
        //        }
        //    }
        //    foreach (var i in ds.Designationlist)
        //    {
        //        list.Add(new SelectListItem() { Text = i.DesignationName, Value = Convert.ToString(i.DesignationId) });
        //    }
        //    return list;
        //}
        //public List<SelectListItem> FillShift()
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    Shifts sh = new Shifts();
        //    if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
        //    {
        //        sh.Shiftslist = RestClient.ShiftGetAll();
        //    }
        //    else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
        //    {
        //        var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
        //        var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
        //        if (Session["RoleId"].ToString() == "6806")
        //        {
        //            sh.Shiftslist = RestClient.ShiftGetAll().Where(x => (x.GraceBefore == cmpid && (x.GraceAfter == BranchID || x.GraceAfter == 0)) || (x.GraceBefore == 0)).ToList();
        //        }
        //        else
        //        {
        //            sh.Shiftslist = RestClient.ShiftGetAll().Where(x => x.GraceBefore == cmpid || x.GraceBefore == 0).ToList();
        //        }
        //    }
        //    foreach (var i in sh.Shiftslist)
        //    {
        //        list.Add(new SelectListItem() { Text = i.ShiftName, Value = Convert.ToString(i.ShiftId) });
        //    }
        //    return list;
        //}
        public List<SelectListItem> FillShiftGroup()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            ShiftGroup sh = new ShiftGroup();
            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
            {
                sh.ShiftGrouplist = RestClient.ShiftGroupGetAll();
            }
            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                if (Session["RoleId"].ToString() == "6806")
                {
                    sh.ShiftGrouplist = RestClient.ShiftGroupGetAll().Where(x => (x.CompanyID == cmpid && (x.BranchID == BranchID || x.BranchID == 0)) || (x.CompanyID == 0));
                }
                else
                {
                    sh.ShiftGrouplist = RestClient.ShiftGroupGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
                }
            }

            foreach (var i in sh.ShiftGrouplist)
            {
                list.Add(new SelectListItem() { Text = i.ShiftGroupName, Value = Convert.ToString(i.ShiftGroupId) });
            }
            return list;
        }
        //public List<SelectListItem> FillRole()
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    Roles rl = new Roles();
        //    if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
        //    {
        //        rl.RoleList = RestClient.RoleGetAll();
        //    }
        //    else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
        //    {
        //        if (Session["RoleId"].ToString() == "6806")
        //        {
        //            rl.RoleList = RestClient.RoleGetAll().Where(x => x.RoleId != 6806 && x.RoleId != 6805 && x.RoleId != 6807 && x.RoleId != 1);
        //        }
        //        else
        //        {
        //            rl.RoleList = RestClient.RoleGetAll().Where(x => x.RoleId != 6805 && x.RoleId != 6807 && x.RoleId != 1);
        //        }
        //    }

        //    foreach (var i in rl.RoleList)
        //    {
        //        list.Add(new SelectListItem() { Text = i.RoleName, Value = Convert.ToString(i.RoleId) });
        //    }
        //    return list;
        //}
        //public List<SelectListItem> FillEmployee()
        //{
        //    Employees em = new Employees();
        //    em.EmployeeList = RestClient.EmployeeGetAll();
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    foreach (var i in em.EmployeeList)
        //    {
        //        list.Add(new SelectListItem() { Text = i.EmpName, Value = Convert.ToString(i.EmpId) });
        //    }
        //    return list;
        //}
        //public List<SelectListItem> FillEmployeeReporting()
        //{
        //    Employees em = new Employees();
        //    em.EmployeeList = RestClient.EmployeeGetAllReporting();
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    foreach (var i in em.EmployeeList)
        //    {
        //        list.Add(new SelectListItem() { Text = i.EmpName + '(' + i.Empcode + ')', Value = Convert.ToString(i.EmpId) });
        //    }
        //    return list;
        //}
        public List<SelectListItem> FillBranch()
        {
            Branches br = new Branches();
            br.Brancheslist = RestClient.BranchGetAll();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var i in br.Brancheslist)
            {
                list.Add(new SelectListItem() { Text = i.BranchName, Value = Convert.ToString(i.BranchId) });
            }
            return list;
        }
        public List<SelectListItem> FillCompany()
        {

            List<SelectListItem> list = new List<SelectListItem>();
            Companys cm = new Companys();

            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) == 1)
            {

                cm.Companyslist = RestClient.CompanyGetAll();
                //list.Add(new SelectListItem() { Text = "Select Company", Value = "0" });
                foreach (var i in cm.Companyslist)
                {
                    list.Add(new SelectListItem() { Text = i.CompanyName, Value = Convert.ToString(i.CompanyID) });
                }
            }
            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                cm.Companyslist = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                foreach (var i in cm.Companyslist)
                {
                    list.Add(new SelectListItem() { Text = i.CompanyName, Value = Convert.ToString(i.CompanyID) });
                }
            }
            else if (Convert.ToInt32(Session["RoleId"].ToString()) > 1)
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                cm.Companyslist = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                foreach (var i in cm.Companyslist)
                {
                    list.Add(new SelectListItem() { Text = i.CompanyName, Value = Convert.ToString(i.CompanyID) });
                }
            }

            return list;
        }
        //public List<SelectListItem> FillTransactionYear()
        //{
        //    TransactionYear ty = new TransactionYear();
        //    ty.TransactionYearList = RestClient.TransactionYearGetAll();
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    foreach (var i in ty.TransactionYearList)
        //    {
        //        list.Add(new SelectListItem() { Text = i.TransactionYearName, Value = Convert.ToString(i.TransactionYearId) });
        //    }
        //    return list;
        //}

        [HttpPost]
        public JsonResult FillDesignationForEmp()
        {
            Designations d = new Designations();
            List<Designations> dList = new List<Designations>();
            dList = RestClient.DesignationsGetAll().ToList();
            return Json(dList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FillDepartmentForEmp()
        {
            Departments d = new Departments();
            List<Departments> dList = new List<Departments>();
            dList = RestClient.DepartmentGetAll().ToList();
            return Json(dList, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult FillRoleForEmp()
        {
            Roles d = new Roles();
            List<Roles> dList = new List<Roles>();
            dList = RestClient.RoleGetAll().ToList();
            return Json(dList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FillShiftGroupForEmp()
        {
            ShiftGroup d = new ShiftGroup();
            List<ShiftGroup> dList = new List<ShiftGroup>();
            dList = RestClient.ShiftGroupGetAll().ToList();
            return Json(dList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FillShiftForEmp()
        {
            Shifts d = new Shifts();
            List<Shifts> dList = new List<Shifts>();

            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
            {
                dList = RestClient.ShiftGetAll().ToList();
            }
            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                if (Session["RoleId"].ToString() == "6806")
                {
                    dList = RestClient.ShiftGetAll().Where(x => (x.GraceBefore == cmpid && (x.GraceAfter == BranchID || x.GraceAfter == 0)) || (x.GraceBefore == 0)).ToList();
                }
                else
                {
                    dList = RestClient.ShiftGetAll().Where(x => x.GraceBefore == cmpid || (x.GraceBefore == 0)).ToList();
                }
            }
            return Json(dList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult FillHRPolicyForEmp()
        {
            HRPolicy d = new HRPolicy();
            List<HRPolicy> dList = new List<HRPolicy>();
            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
            {
                dList = RestClient.HRPolicyGetAll().ToList();
            }
            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                if (Session["RoleId"].ToString() == "6806")
                {
                    dList = RestClient.HRPolicyGetAll().Where(x => (x.CompanyID == cmpid && (x.BranchID == BranchID || x.BranchID == 0)) || (x.CompanyID == 0)).ToList();
                }
                else
                {
                    dList = RestClient.HRPolicyGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0).ToList();
                }

            }

            return Json(dList, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult FillCompanyforEmp()
        //{
        //    Companys d = new Companys();
        //    List<Companys> dList = new List<Companys>();
        //    if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
        //    {
        //        dList = RestClient.CompanyGetAll().ToList();
        //    }
        //    else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
        //    {
        //        var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
        //        dList = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid).ToList();
        //    }

        //    return Json(dList, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult FillEventype()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "OnEveryPunch", Value = "2" });
            items.Add(new SelectListItem() { Text = "FirstInFirstOut", Value = "3" });
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public List<SelectListItem> ShowTimeZone()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var timeZone in timeZones)
            {
                //  items.Add(new SelectListItem() { Text = "Test1", Value = "1", Selected = true });
                items.Add(new SelectListItem() { Text = timeZone.DisplayName, Value = timeZone.Id });
            }
            return items;
        }


        //[HttpGet]
        //public JsonResult GetBranchById(int CompanyId = 0)
        //{
        //    JsonResult result;
        //    try
        //    {
        //        var Branchlist = RestClient.BranchGetByCompanyId(CompanyId);
        //        result = Json(new SelectList(Branchlist, "BranchId", "BranchName"));
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

        [HttpGet]
        public JsonResult GetBranchlist(string companyid, int empid)
        {
            JsonResult result;
            try
            {
                var AllBranch = RestClient.BranchlistGet(companyid, empid);

                var CompanywiseBranch = AllBranch.Where(x => companyid.Contains(x.CompanyID.ToString()))
                                                 .Select(i => new { i.BranchId, i.BranchName, i.BranchLevel })
                                                 .ToList();

                if (Session["RoleId"] != null && Session["RoleId"].ToString() == "6806")
                {
                    if (Session["BranchId"] != null)
                    {
                        var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                        CompanywiseBranch = AllBranch.Where(x => companyid.Contains(x.CompanyID.ToString()))
                                                     .Select(i => new { i.BranchId, i.BranchName, i.BranchLevel })
                                                     .ToList();
                    }
                }
                else if (Session["RoleId"] != null && Session["RoleId"].ToString() == "6808")
                {
                    // Implement logic for RoleId 6808 if needed
                }
                else if (Session["RoleId"] != null && Session["RoleId"].ToString() != "6806" && Session["RoleId"].ToString() != "6805" && Convert.ToInt32(Session["RoleId"].ToString()) > 1)
                {
                    if (Session["BranchIds"] != null)
                    {
                        string branchIdsString = Session["BranchIds"].ToString();
                        List<int> branchIds = branchIdsString.Split(',').Select(int.Parse).ToList();

                        CompanywiseBranch = AllBranch.Where(x => companyid.Contains(x.CompanyID.ToString()) && branchIds.Contains(x.BranchId))
                                                     .Select(i => new { i.BranchId, i.BranchName, i.BranchLevel })
                                                     .ToList();
                    }
                }

                result = Json(CompanywiseBranch, JsonRequestBehavior.AllowGet);
                return result;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine(ex.Message);

                result = Json(new SelectList("", "0"), JsonRequestBehavior.AllowGet);
                return result;
            }
        }

        [HttpGet]
        public JsonResult GetBranchbycomapny(int comapanyid = 0)
        {
            JsonResult result;
            try
            {
                var AllBranch = RestClient.BranchGetAll();
                var CompanywiseBranch = AllBranch.Where(x => x.CompanyID == comapanyid).Select(i => new { i.BranchId, i.BranchName, i.BranchLevel });
                if (Session["RoleId"].ToString() == "6806")
                {
                    var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                    CompanywiseBranch = AllBranch.Where(x => x.CompanyID == comapanyid && x.BranchId == BranchID).Select(i => new { i.BranchId, i.BranchName, i.BranchLevel });
                }
                else if (Session["RoleId"].ToString() != "6806" && Session["RoleId"].ToString() != "6805" && Convert.ToInt32(Session["RoleId"].ToString()) > 1)
                {
                    var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                    CompanywiseBranch = AllBranch.Where(x => x.CompanyID == comapanyid && x.BranchId == BranchID).Select(i => new { i.BranchId, i.BranchName, i.BranchLevel });
                }
                result = Json(CompanywiseBranch);
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
        public JsonResult GetBranchbycomapnyid(string companyid)
        {
            JsonResult result;
            try
            {
                var companyIds = companyid.Split(',').Select(id => int.Parse(id.Trim())).ToList();
                var allBranches = RestClient.BranchGetAll();
                var CompanywiseBranch = allBranches.Where(x => companyIds.Contains(x.CompanyID)).Select(i => new { i.BranchId, i.BranchName, i.BranchLevel }).ToList();
                if (Session["RoleId"].ToString() == "6806")
                {
                    var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                    CompanywiseBranch = allBranches.Where(x => companyIds.Contains(x.CompanyID) && x.BranchId == BranchID).Select(i => new { i.BranchId, i.BranchName, i.BranchLevel }).ToList();
                }
                //6000 is pms admin role.
                else if(Session["RoleId"].ToString() == "6000")
                {
                    CompanywiseBranch = allBranches.Where(x => companyIds.Contains(x.CompanyID)).Select(i => new { i.BranchId, i.BranchName, i.BranchLevel }).ToList();
                }
                //6808 for hr role for payroll 
                else if (Session["RoleId"].ToString() == "6808")
                {

                }
                else if (Session["RoleId"].ToString() != "6806" && Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6810" && Convert.ToInt32(Session["RoleId"].ToString()) > 1)
                {
                   var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                  CompanywiseBranch = allBranches.Where(x => companyid.Contains(x.CompanyID.ToString()) && x.BranchId == BranchID).Select(i => new { i.BranchId, i.BranchName, i.BranchLevel }).ToList();
                }
                result = Json(CompanywiseBranch);
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
        public JsonResult GetBranchbycomapnyholiday(int comapanyid = 0)
        {
            JsonResult result;
            try
            {
                var AllBranch = RestClient.BranchGetAll();
                var CompanywiseBranch = AllBranch.Select(i => new { i.BranchId, i.BranchName, i.BranchLevel });
                if (comapanyid != 0)
                {
                    CompanywiseBranch = AllBranch.Where(x => x.CompanyID == comapanyid).Select(i => new { i.BranchId, i.BranchName, i.BranchLevel });
                }

                if (Session["RoleId"].ToString() == "6806")
                {
                    var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                    if (comapanyid != 0)
                    {
                        CompanywiseBranch = AllBranch.Where(x => x.CompanyID == comapanyid && x.BranchId == BranchID)
                        .Select(i => new { i.BranchId, i.BranchName, i.BranchLevel });
                    }
                    else
                    {
                        CompanywiseBranch = AllBranch.Where(x => x.BranchId == BranchID)
                        .Select(i => new { i.BranchId, i.BranchName, i.BranchLevel });
                    }
                }

                result = Json(CompanywiseBranch);
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
        public JsonResult FillEmployeebyChange(string comidlst, string branchidlst, string desigidlst, string deptidlst, string roleidlst, string empidlst)
        {
            if (comidlst != "" && branchidlst != "" && desigidlst != "" && deptidlst != "" && roleidlst != "" && empidlst != "")
            {
                reqPolicydata rpd = new reqPolicydata();
                rpd.comidlst = comidlst;
                rpd.branchidlst = branchidlst;
                rpd.desigidlst = desigidlst;
                rpd.deptidlst = deptidlst;
                rpd.roleidlst = roleidlst;
                rpd.empidlst = empidlst;
                var lst = RestClient.GetAllDataForPolicyAllocation(rpd);
                //if (Convert.ToInt32(Session["RoleId"].ToString()) > 1)
                //{
                //    if (Session["RoleId"].ToString() != "6806" && Session["RoleId"].ToString() != "6805")
                //    {
                //        var employeeid = Convert.ToInt32(Session["EmpId"].ToString());
                //        lst = RestClient.GetAllDataForPolicyAllocation(rpd).Where(x => x.EmpId == employeeid);
                //    }

                //}
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult FillDepartmentsbyChange(string ArrDepartment)
        {
            JsonResult result;
            try
            {
                Departments d = new Departments();
                List<Departments> bList = new List<Departments>();
                List<Departments> lst = new List<Departments>();
                var AllDeptCode = RestClient.EmployeeGetAll();
                var query = from val in ArrDepartment.Split(',')
                            select int.Parse(val);
                foreach (int num in query)
                {
                    var BranchWiseDeptCode = AllDeptCode.Where(x => x.BranchId == num).Select(x => x.DepartmentId).Distinct();
                    bList = RestClient.DepartmentGetAll().Where(x => BranchWiseDeptCode.Contains(x.DepartmentId)).ToList();
                    lst.AddRange(bList);
                }
                result = Json(new SelectList(lst, "DepartmentId", "DepartmentName"));
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

        //[HttpGet]
        //public JsonResult FillBranchesbyCompanyChange(string comidlst)
        //{
        //    JsonResult result;
        //    try
        //    {
        //        Branches b = new Branches();
        //        List<Branches> bList = new List<Branches>();
        //        List<Branches> lst = new List<Branches>();
        //        if (comidlst != "")
        //        {
        //            var query = from val in comidlst.Split(',')
        //                        select int.Parse(val);
        //            foreach (int num in query)
        //            {
        //                if (Session["RoleId"].ToString() != "6805" || Session["RoleId"].ToString() == "6806" || Convert.ToInt32(Session["RoleId"].ToString()) > 1)
        //                {
        //                    var brachid = Convert.ToInt32(Session["BranchId"].ToString());
        //                    bList = RestClient.BranchGetAll().Where(x => x.CompanyID == num && x.BranchId == brachid).ToList();
        //                }
        //                else
        //                {
        //                    bList = RestClient.BranchGetAll().Where(x => x.CompanyID == num).ToList();
        //                }
        //                lst.AddRange(bList);
        //            }
        //            return Json(lst, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            return Json("", JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        result = Json(new SelectList("", "0"));
        //        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //        return result;
        //    }
        //}

        [HttpGet]
        public JsonResult FillBranchesbyCompanyChange(string comidlst)
        {
            JsonResult result;
            try
            {
                Branches b = new Branches();
                List<Branches> bList = new List<Branches>();
                List<Branches> lst = new List<Branches>();
                if (comidlst != "")
                {
                    var query = from val in comidlst.Split(',')
                                select int.Parse(val);
                    foreach (int num in query)
                    {
                        if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) > 1)
                        {
                            var brachid = Convert.ToInt32(Session["BranchId"].ToString());
                            bList = RestClient.BranchGetAll().Where(x => x.CompanyID == num && x.BranchId == brachid).ToList();
                        }
                        else
                        {
                            bList = RestClient.BranchGetAll().Where(x => x.CompanyID == num).ToList();
                        }
                        lst.AddRange(bList);
                    }
                    return Json(lst, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }


        public JsonResult FillBranchbyChange(int id)
        {
            Branches b = new Branches();
            List<Branches> bList = new List<Branches>();
            if (Session["RoleId"].ToString() == "6806")
            {
                var brachid = Convert.ToInt32(Session["BranchId"].ToString());
                bList = RestClient.BranchGetAll().Where(x => x.CompanyID == id && x.BranchId == brachid).ToList();
            }
            else
            {
                bList = RestClient.BranchGetAll().Where(x => x.CompanyID == id).ToList();
            }

            return Json(bList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FillCompanyMasterGetbyid(string id)
        {
            Companys cmp = new Companys();
            cmp = RestClient.CompanyGetByid(Convert.ToInt32(id.Trim()));
            return Json(cmp, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetDeptbyBranch(int BranchId = 0)
        {
            JsonResult result;
            try
            {
                //var AllDeptCode = RestClient.EmployeeGetAll();
                //var BranchWiseDeptCode = AllDeptCode.Where(x => x.BranchId == BranchId).Select(x => x.DepartmentId).Distinct();
                //var BranchWiseDept = RestClient.DepartmentGetAll().Where(x => x.DepartmentId.Contains(BranchWiseDeptCode));
                //var BranchWiseDept = RestClient.DepartmentGetAll().Where(x => BranchWiseDeptCode.Contains(x.DepartmentId));
                var BranchWiseDept = RestClient.GetDeptbyBranch(BranchId);
                result = Json(new SelectList(BranchWiseDept, "DepartmentId", "DepartmentName"));
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
        public JsonResult GetDeptbyBranchstr(string BranchId)
        {
            JsonResult result;
            try
            {
                //var AllDeptCode = RestClient.EmployeeGetAll();
                //var BranchWiseDeptCode = AllDeptCode.Where(x => x.BranchId == BranchId).Select(x => x.DepartmentId).Distinct();
                //var BranchWiseDept = RestClient.DepartmentGetAll().Where(x => x.DepartmentId.Contains(BranchWiseDeptCode));
                //var BranchWiseDept = RestClient.DepartmentGetAll().Where(x => BranchWiseDeptCode.Contains(x.DepartmentId));
                var BranchWiseDept = RestClient.GetDeptbyBranchstr(BranchId).OrderBy(x => x.DepartmentName);
                result = Json(new SelectList(BranchWiseDept, "DepartmentId", "DepartmentName"));
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

        [HttpPost]
        public async Task<JsonResult> GetDeptbyBranchstrnew(GetAllBranchstr branchstr)
        {
            JsonResult result;
            try
            {
                //var BranchWiseDept = RestClient.GetDeptbyBranchstr(branchstr.BranchId).OrderBy(x => x.DepartmentName);
                //result = Json(new SelectList(BranchWiseDept, "DepartmentId", "DepartmentName"));
                //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                //return result;
                var otherObjects = await RestClient.GetDeptbyBranchstrnew(branchstr.BranchId);
                // Now you can use OrderBy because employeesTask is no longer a Task but the actual result.
                result = Json(new SelectList(otherObjects, "DepartmentId", "DepartmentName"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
                //var orderedEmployees = otherObjects.OrderBy(i => i.EmpName);
                //result = Json(orderedEmployees, JsonRequestBehavior.AllowGet);
                //result.MaxJsonLength = Int32.MaxValue;
                //return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }


        [HttpPost]
        public async Task<JsonResult> DeviceGet(Getdevicealltype device)
        {
            JsonResult result;
            try
            {
                //var BranchWiseDept = RestClient.GetDeptbyBranchstr(branchstr.BranchId).OrderBy(x => x.DepartmentName);
                //result = Json(new SelectList(BranchWiseDept, "DepartmentId", "DepartmentName"));
                //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                //return result;
                var otherObjects = await RestClient.GetDevicenew(device.BranchId);
                // Now you can use OrderBy because employeesTask is no longer a Task but the actual result.
                //result = Json(new SelectList(otherObjects, "DeviceCode", "DeviceName"));
                //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                //return result;

                var deviceList = otherObjects.Select(o => new
                {
                    DeviceCode = o.DeviceCode,
                    DeviceName = o.DeviceName,
                    Devicesrno = o.Devicesrno
                }).ToList();

                // Return the list as JSON
                result = Json(deviceList, JsonRequestBehavior.AllowGet);
                return result;
                //var orderedEmployees = otherObjects.OrderBy(i => i.EmpName);
                //result = Json(orderedEmployees, JsonRequestBehavior.AllowGet);
                //result.MaxJsonLength = Int32.MaxValue;
                //return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        [HttpGet]
        public JsonResult GetDeptbyBranchstrOnboarding(string BranchId)
        {
            JsonResult result;
            try
            {
                var BranchWiseDept = RestClient.GetDeptbyBranchstrOnboarding(BranchId).OrderBy(x => x.DepartmentName);
                result = Json(new SelectList(BranchWiseDept, "DepartmentId", "DepartmentName"));
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
        public JsonResult GetDeptbyBranchstrfltr(BranchGetbyDept obj)
        {
            JsonResult result;
            try
            {
                var BranchWiseDept = RestClient.GetDeptbyBranchstrfltr(obj).OrderBy(x => x.DepartmentName);
                result = Json(new SelectList(BranchWiseDept, "DepartmentId", "DepartmentName"));
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
        public JsonResult GetEmpByDept(int DepartmentId = 0)
        {
            JsonResult result;
            try
            {
                var EmpList = RestClient.EmployeeGetAll().Select(m => new { m.EmpName, m.EmpId, m.EmpPunchID, m.DepartmentId }).Where(x => x.DepartmentId == DepartmentId);
                result = Json(EmpList);
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

        //[HttpGet]
        //public JsonResult GetEmpByreporting(int EmpId = 0)
        //{
        //    JsonResult result;
        //    try
        //    {
        //        if ((int)Session["EmpId"] == EmpId)
        //        {

        //        if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
        //        {
        //            var EmpList = RestClient.EmployeeGetAll().Select(m => new { m.EmpName, m.EmpId, m.Empcode, m.ReportingTo, m.IsActive }).Where(x => x.IsActive == true);
        //            result = Json(EmpList);
        //            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //        }
        //        else
        //        {
        //            //var EmpList = RestClient.EmployeeGetAll().Select(m => new { m.EmpName, m.EmpId, m.ReportingTo, m.IsActive }).Where(x => x.ReportingTo == EmpId || x.EmpId == EmpId && x.IsActive == true);
        //            var EmpList = RestClient.EmployeeGetAll().Select(m => new { m.EmpName, m.EmpId, m.ReportingTo, m.IsActive, m.Empcode });
        //            result = Json(EmpList);
        //            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //            }
        //        }
        //        else
        //        {
        //            result = null;
        //        }

        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        result = Json(new SelectList("", "0"));
        //        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //        return result;
        //    }
        //}

        [HttpGet]
        public JsonResult GetEmpByreporting(int hierarchyState = 0, int EmpId = 0) //GetEmpByreportinglevel
        {
            //int Ishierchy = Convert.ToInt32(Session["Ishierchy"]);
            JsonResult result;
            try
            {
                if ((int)Session["EmpId"] == EmpId)
                {

                    if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                    {
                        var EmpList = RestClient.EmployeeGetAll(hierarchyState).Select(m => new { m.EmpName, m.EmpId, m.Empcode, m.ReportingTo, m.IsActive }).Where(x => x.IsActive == true);
                        result = Json(EmpList);
                        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    }
                    else
                    {
                        //var EmpList = RestClient.EmployeeGetAll().Select(m => new { m.EmpName, m.EmpId, m.ReportingTo, m.IsActive }).Where(x => x.ReportingTo == EmpId || x.EmpId == EmpId && x.IsActive == true);
                        var EmpList = RestClient.EmployeeGetAll().Select(m => new { m.EmpName, m.EmpId, m.ReportingTo, m.IsActive, m.Empcode });
                        result = Json(EmpList);
                        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    }
                }
                else
                {
                    result = null;
                }

                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

        //[HttpGet]
        //public string GetEmployeePunchID(string punchid)
        //{
        //    var EmpList = RestClient.EmployeeGetAll().Where(c => c.EmpPunchID == punchid).Select(c => c.EmpPunchID).FirstOrDefault();
        //    return EmpList;
        //}

        #endregion

        #region Customer Master

        public ActionResult CustomerMaster()
        {
            ViewBag.msg = "";
            ViewBag.error = "";

            CustomerMaster objc = new CustomerMaster();

            try
            {
                CompanySetting objsys = RestClient.GetSystemSettingById(1);
                ViewBag.IsEss = Convert.ToBoolean(objsys.IsEss.ToString() != "" ? objsys.IsEss.ToString() : "0");
                ViewBag.IsBeacon = Convert.ToBoolean(objsys.IsBeacon.ToString() != "" ? objsys.IsBeacon.ToString() : "0");
                return View(objc);
            }
            catch (Exception ex)
            {
                objc.Customerlist = null;
                ViewBag.error = ex.Message;
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        //[HttpPost]
        //public string DeleteBranch(string id)
        //{
        //    string sts = "";
        //    if (id != null)
        //    {
        //        if (RestClient.BranchDelete(Convert.ToInt32(id.Trim())))
        //        {
        //            sts = "OK";
        //        }
        //    }
        //    return new JavaScriptSerializer().Serialize(sts);
        //}
        #endregion

        #region Audit
        public ActionResult Audit()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetEmployeeAuditData(int Type, int tblid, string FromDate, string ToDate, string UserEmail)
        {
            JsonResult result;
            try
            {
                Audit obj = new Audit();
                obj.Type = Type;
                obj.tblid = tblid;
                obj.FromDate = FromDate;
                obj.ToDate = ToDate;
                obj.UserEmail = UserEmail;
                var GetAuditData = RestClient.GetEmployeeAuditData(obj);
                result = Json(GetAuditData);
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
        [HttpPost]
        public JsonResult GetAuditDeviceData(int Type, int tblid, string FromDate, string ToDate, string UserEmail)
        {
            JsonResult result;
            try
            {
                Audit obj = new Audit();
                obj.Type = Type;
                obj.tblid = tblid;
                obj.FromDate = FromDate;
                obj.ToDate = ToDate;
                obj.UserEmail = UserEmail;
                var GetAuditData = RestClient.GetAuditDeviceData(obj);
                result = Json(GetAuditData);
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
        [HttpPost]
        public JsonResult GetShiftAuditData(int Type, int tblid, string FromDate, string ToDate, string UserEmail)
        {
            JsonResult result;
            try
            {
                Audit obj = new Audit();
                obj.Type = Type;
                obj.tblid = tblid;
                obj.FromDate = FromDate;
                obj.ToDate = ToDate;
                obj.UserEmail = UserEmail;
                var GetAuditData = RestClient.GetAuditShiftData(obj);
                result = Json(GetAuditData);
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
        [HttpPost]
        public JsonResult GetPolicyAuditData(int Type, int tblid, string FromDate, string ToDate, string UserEmail)
        {
            JsonResult result;
            try
            {
                Audit obj = new Audit();
                obj.Type = Type;
                obj.tblid = tblid;
                obj.FromDate = FromDate;
                obj.ToDate = ToDate;
                obj.UserEmail = UserEmail;

                var GetAuditData = RestClient.GetPolicyAuditData(obj);
                result = Json(GetAuditData);
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
        [HttpPost]
        public JsonResult GetAttendanceAuditdata(int Type, int tblid, string FromDate, string ToDate, string UserEmail)
        {
            JsonResult result;
            try
            {
                Audit obj = new Audit();
                obj.Type = Type;
                obj.tblid = tblid;
                obj.FromDate = FromDate;
                obj.ToDate = ToDate;
                obj.UserEmail = UserEmail;
                var GetAuditData = RestClient.GetAttendanceAuditdata(obj);
                result = Json(GetAuditData);
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
        [HttpPost]
        public JsonResult GetBranchAuditdata(int Type, int tblid, string FromDate, string ToDate, string UserEmail)
        {
            JsonResult result;
            try
            {
                Audit obj = new Audit();
                obj.Type = Type;
                obj.tblid = tblid;
                obj.FromDate = FromDate;
                obj.ToDate = ToDate;
                obj.UserEmail = UserEmail;
                var GetAuditData = RestClient.GetBranchAuditdata(obj);
                result = Json(GetAuditData);
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

        #endregion

        #region Process Data
        public ActionResult SummaryDetail()
        {
            return View();
        }

        public ActionResult TransactionDetail()
        {
            ViewBag.Companyslist = RestClient.CompanyGetAll();
            return View();
        }

        [HttpGet]
        public JsonResult GetEmployeeByComBranch(string BranchID, string CompanyId)
        {
            Employees emp = new Employees();
            JsonResult result;
            try
            {
                int[] branchid = new List<string>(BranchID.Split(',')).ConvertAll<int>(s => int.Parse(s)).ToArray();
                int[] companyid = new List<string>(CompanyId.Split(',')).ConvertAll<int>(s => int.Parse(s)).ToArray();
                var AllEmp = RestClient.EmployeeGetAll().Where(item => branchid.Contains(item.BranchId) && companyid.Contains(item.CompanyID));
                return Json(AllEmp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result = Json(ex.ToString());
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }

        }
        #endregion

        #region EmpHierchy
        public ActionResult MySuperior()
        {
            return View();
        }
        #endregion

        #region payrollauth
        public class payrollauth
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Empcode { get; set; }
            public string EmployeeId { get; set; }
        }
        public int empid { get; set; }
        #endregion


        #region IdentityVerification
        [HttpGet]
        public ActionResult IdentityVerification()
        {
            return View();
        }
        #endregion


        #region EmployeeBulkChanges
        [HttpGet]
        public ActionResult EmployeeBulkChanges()
        {
            return View();
        }
        #endregion


        #region GetEmployeefastrack
        [HttpGet]
        public JsonResult GetEmpByFastrackMisspunch()
        {
            JsonResult result;
            try
            {
                var EmpList = RestClient.EmployeeGetAllFasttrack().Select(m => new { m.EmpName, m.EmpId, m.Empcode, m.ReportingTo, m.IsActive }).Where(x => x.IsActive == true);
                result = Json(EmpList);
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

        #endregion

        #region Holiday Optional

        public ActionResult HolidayOptional()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            ViewBag.planID = Session["planId"];
            Holidays objc = new Holidays();
            ViewBag.isStandard = Convert.ToInt32(Session["isStandard"]);
            try
            {
                CompanySetting objsys = RestClient.GetSystemSettingById(1);
                ViewBag.IsEss = Convert.ToBoolean(objsys.IsEss.ToString() != "" ? objsys.IsEss.ToString() : "0");
                ViewBag.IsBeacon = Convert.ToBoolean(objsys.IsBeacon.ToString() != "" ? objsys.IsBeacon.ToString() : "0");
                ViewBag.IsBranchGeoFence = Convert.ToBoolean(objsys.IsBranchGeoFence.ToString() != "" ? objsys.IsBranchGeoFence.ToString() : "0");
                ViewBag.IsVerification = Convert.ToInt32(RestClient.GetSystemSettingById(1).IsVerification);

                Session["actname"] = "Holiday Optional";

                if (Session["actname"] != null)
                {
                    TempData["frmname"] = Session["actname"].ToString();
                }

                IEnumerable<Cfc> lstobj = UtilitiesRestClient.GetAllcustomforms();

                ViewBag.lstcustomform = JsonConvert.SerializeObject(lstobj);
                DataTable model = UtilitiesRestClient.Getcustomformdata(Session["actname"].ToString());

                return View(objc);
            }
            catch (Exception ex)
            {
                objc.Holidayslist = null;
                ViewBag.error = ex.Message;
                return RedirectToAction("ErrorPage", "PayTime");
            }
            finally
            {
                objc.Holidayslist = null;
                objc = null;
            }

        }
        #endregion

        #region EmployeelstEnrollUserList
        [HttpPost]
        public JsonResult EmployeelstEnrollUserList(DataTableEmployeePostModel model)
        {
            //for employee  enroll in mfstab
            var recordsTotalCount = 0;
            EnrollUserParameter objep = new EnrollUserParameter();

            objep = new EnrollUserParameter
            {
                BranchId = model.roleid,
                CmpId = model.loginempid,
                StatusId = model.selectstatus,
                DepartmentId = model.departmentid
            };
            DatatableCounts DataCount = new DatatableCounts();

            var AllData = RestClient.EmployeeGetAllEnrollUserList(objep);

            if (!string.IsNullOrEmpty(model.search.value))
            {
                AllData = AllData.Where((x => x.EmpName.ToLower().Contains(model.search.value.ToLower()) || x.EmpPunchID.ToLower().Contains(model.search.value.ToLower()) || x.Email.ToLower().Contains(model.search.value.ToLower()))).Skip(model.start).Take(model.length).ToList();
            }
            else
            {
                AllData = AllData.ToList();
            }
            var recordsFilteredCount = AllData.Count();
            AllData = AllData.Skip(model.start).Take(model.length).ToList();

            var result = new List<Employees>(AllData.Count());
            foreach (var data in AllData)
            {
                result.Add(new Employees
                {
                    EmpId = data.EmpId,
                    EmpName = data.EmpName,
                    UserId = data.UserId,
                    EmpAddress = data.EmpAddress,
                    EmpPhNo = data.EmpPhNo,
                    EmpMNo = data.EmpMNo,
                    RoleId = data.RoleId,
                    RoleName = data.RoleName,
                    Gender = data.Gender,
                    Email = data.Email,
                    Password = data.Password,
                    Empcode = data.Empcode,
                    EmpDOB = data.EmpDOB,
                    EmpMarried = data.EmpMarried,
                    EmpJoinDate = data.EmpJoinDate,
                    EmpResignDate = data.EmpResignDate,
                    EmpPunchID = data.EmpPunchID,
                    EmpPhoto = data.EmpPhoto,
                    MobNoSMS = data.MobNoSMS,
                    CompanyID = data.CompanyID,
                    CompanyName = data.CompanyName,
                    BranchId = data.BranchId,
                    BranchName = data.BranchName,
                    DepartmentId = data.DepartmentId,
                    DepartmentName = data.DepartmentName,
                    DesignationId = data.DesignationId,
                    DesignationName = data.DesignationName,
                    CategoryId = data.CategoryId,
                    CategoryName = data.CategoryName,
                    TypeId = data.TypeId,
                    EmpTypeName = data.EmpTypeName,
                    GradeId = data.GradeId,
                    ShiftId = data.ShiftId,
                    ShiftName = data.ShiftName,
                    ShiftShortName = data.ShiftShortName,
                    ShiftGroupId = data.ShiftGroupId,
                    ShiftGroupName = data.ShiftGroupName,
                    ShiftGroupShortName = data.ShiftGroupShortName,
                    ContractorId = data.ContractorId,
                    ContractorName = data.ContractorName,
                    ReportingTo = data.ReportingTo,
                    PolicyId = data.PolicyId,
                    PolicyName = data.PolicyName,
                    EmpWeekOff = data.EmpWeekOff,
                    EmpSecondWeekOff = data.EmpSecondWeekOff,
                    EmpSecondWeekOffRule = data.EmpSecondWeekOffRule,
                    EmpHalfDay = data.EmpHalfDay,
                    EmpHalfDayRule = data.EmpHalfDayRule,
                    ReligionId = data.ReligionId,
                    IsSMS = data.IsSMS,
                    ModifyBy = data.ModifyBy,
                    ModifyDate = data.ModifyDate,
                    IsActive = data.IsActive,
                    Status = data.Status,
                    Married = data.Married,
                    BirthDate = data.BirthDate,
                    JoinDate = data.JoinDate,
                    EmpGender = data.EmpGender,
                    TagId = data.TagId,
                    CustomFields = data.CustomFields,
                    Customvalues = data.Customvalues,
                    totalCount = data.totalCount
                });
            };

            if (AllData.Count() > 0)
            {
                recordsTotalCount = result[0].totalCount;
            }

            return Json(new
            {
                // this is what datatables wants sending back
                draw = model.draw,
                //recordsTotal = DataCount.totalResultsCount,
                recordsTotal = recordsTotalCount,
                recordsFiltered = recordsFilteredCount,
                data = result
            });
        }
        #endregion

        public ActionResult BusinessUnit()
        {
            return View();
        }

        public ActionResult SubBusinessUnit()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetDesigbyDepartstrnew(GetAllBranchDepartstr branchstr)
        {
            JsonResult result;
            try
            {

                var otherObjects = await RestClient.GetDesigbyDepartstrnew(branchstr.BranchId, branchstr.DeptId);
                result = Json(new SelectList(otherObjects, "DesignationId", "Designationname"));
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
    }
}
