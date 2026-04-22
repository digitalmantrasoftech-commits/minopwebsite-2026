using Newtonsoft.Json;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Converters;
using System.Web;
using System.IO;
using System.Web.UI.WebControls;
using System.Data;
using PayTimeWebClient.Models.DataGridModel;

namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class TransactionsController : Controller
    {
        #region Declaration
        TransactionDataRestClient RestClient = new TransactionDataRestClient();
        MasterDataRestClient MasterRestClient = new MasterDataRestClient();
        SchoolMasterDataRestClient Schoolmaster = new SchoolMasterDataRestClient();
        static readonly UtilitiesDataRestClient UtilitiesRestClient = new UtilitiesDataRestClient();
        #endregion

        #region "Leave Opening"
        public ActionResult LeaveSanction()
        {
            try
            {
                ViewBag.CompanyList = FillCompany();
                ViewBag.LeaveTypeList = FillLeaveType();
                ViewBag.LeaveOpeniglist = RestClient.LeaveOpeningGetAll();
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [HttpPost]
        public string LeaveOpening(LeaveOpening LeaveOpening, DataTableForEmployeeDirectoryGridData mainObj)
        {
            string ad = "";
            ViewBag.msg = "";
            ViewBag.error = "";

            int curyear = DateTime.Now.Year;
            DateTime curdate = DateTime.Now;
            int curyearid = MasterRestClient.TransactionYearGetAll().Where(c => c.IsActive == true).OrderByDescending(c => c.FromYear).Select(c => c.TransactionYearId).SingleOrDefault();
            if (curyearid > 0)
            {
                var dynamicDateFormat = Session["IsDateFormat"].ToString();
                var isdateFormat = " ";
                if (dynamicDateFormat == "dd-M-yyyy" || dynamicDateFormat == "M-dd-yyyy" || dynamicDateFormat == "yyyy-M-dd")
                {
                    isdateFormat = Session["IsDateFormat"].ToString().Replace("M", "MMM");
                }
                else
                {
                    isdateFormat = Session["IsDateFormat"].ToString().Replace("mm", "MM");
                }
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = isdateFormat.ToString() };
                //LeaveOpening objLeaveOpening = JsonConvert.DeserializeObject<LeaveOpening>(LeaveOpening, dateTimeConverter);
                LeaveOpening obj = LeaveOpening;
      
                obj.LeaveBalTransYearid = curyearid;
                obj.IsCarray = 0;
                obj.IsActive = true;

                if (obj.flg == 1)
                {
                    obj.LeaveBalance = obj.LeaveBalance * 1;
                }
                else
                {
                    obj.LeaveBalance = obj.LeaveBalance * -1;
                }
                ad = RestClient.LeaveOpeningAdd(obj, mainObj);
            }
            else
            {
                ad = "Error in Leave balance : Please Add TransactionYear First";
            }

            return ad;
        }
        [HttpPost]
        public JsonResult GetAllLeaveOpening(string Empid, string BranchID)
        {
            JsonResult result;
            try
            {
                //if (BranchID > 0)
                //{
                //    var AllEmp = MasterRestClient.EmployeeGetAll().Where(x => x.BranchId == BranchID).Select(x => x.EmpId).Distinct();
                //    var AllData = RestClient.LeaveOpeningGetAll().Where(x => AllEmp.Contains(Convert.ToInt32(x.EmpCode))).Select(i => new { i.LeaveOpeningId, i.LeaveBalance, i.LeaveTypeName, i.BalanceDateStr, i.EmpName, i.ECode });
                //    return Json(AllData, JsonRequestBehavior.AllowGet);
                //}

                //switch (Convert.ToInt32(Session["RoleId"]))
                //{
                //    case 6805:
                //        {
                //            var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                //            var AllEmp = MasterRestClient.EmployeeGetAll().Where(x => x.CompanyID == cmpid).Select(x => x.EmpId).Distinct();
                //            var AllData = RestClient.LeaveOpeningGetAll().Where(x => AllEmp.Contains(Convert.ToInt32(x.EmpCode))).Select(i => new { i.LeaveOpeningId, i.LeaveBalance, i.LeaveTypeName, i.BalanceDateStr, i.EmpName, i.ECode });
                //            return Json(AllData, JsonRequestBehavior.AllowGet);
                //        }
                //    case 6806:
                //        {
                //            var Brchid = Convert.ToInt32(Session["BranchId"].ToString());
                //            var AllEmp = MasterRestClient.EmployeeGetAll().Where(x => x.BranchId == Brchid).Select(x => x.EmpId).Distinct();
                //            var AllData = RestClient.LeaveOpeningGetAll().Where(x => AllEmp.Contains(Convert.ToInt32(x.EmpCode))).Select(i => new { i.LeaveOpeningId, i.LeaveBalance, i.LeaveTypeName, i.BalanceDateStr, i.EmpName, i.ECode });
                //            return Json(AllData, JsonRequestBehavior.AllowGet);
                //        }
                //    default:
                //        {
                //            var AllData = RestClient.LeaveOpeningGetAll();
                //            return Json(AllData, JsonRequestBehavior.AllowGet);
                //        }
                //}

                var cmpid = 0;
                var Brchid = 0;
                if (Convert.ToInt32(Session["RoleId"]) == 6805)
                {
                    cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                }
                if (Convert.ToInt32(Session["RoleId"]) == 6806)
                {
                    Brchid = Convert.ToInt32(Session["BranchId"].ToString());
                }
                var AllData = RestClient.LeaveOpeningGetAll(BranchID, cmpid, Brchid, Convert.ToInt32(Session["RoleId"].ToString()), Empid);

                // Serialize to JSON
                var jsonData = JsonConvert.SerializeObject(AllData);

                // Compress the data
                byte[] compressedData = CompressionUtils.Compress(jsonData);

                return Json(Convert.ToBase64String(compressedData), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                result = Json(ex.ToString());
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        [HttpPost]
        public JsonResult LeaveSanctionUpload(HttpPostedFileBase FileUpload, string compcode, int roleid, int Companyid, int branchid)
        {
            try
            {
                if (FileUpload == null || FileUpload.ContentLength == 0)
                {
                    return Json(new { success = false, message = "Please select a file!" });
                }
                string errorMessage;
                DataTable dt = ProcessUploadedFile(FileUpload, out errorMessage);
                ImportResponseLeaveSanction response = new ImportResponseLeaveSanction();
                ImportLeaveSanctionfaillogList failLog = new ImportLeaveSanctionfaillogList();

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return Json(new { success = false, message = errorMessage });
                }

                response = DataTableConverterLeaveSanction.ConvertToImportedDataLeaveSanctionList(dt, this.HttpContext);

                if (response.status && response.statusCode == 101)
                {
                    if (response.LevaeSanctionList != null && response.LevaeSanctionList.Count > 0)
                    {
                        try
                        {
                            failLog.importleaveFailloglist = UtilitiesRestClient.ImportLeaveSanctionDetail(response.LevaeSanctionList, compcode, roleid, Companyid, branchid);
                            if (!failLog.importleaveFailloglist.Any())
                            {
                                return Json(new { success = true, message = "File imported successfully." });
                            }
                            else
                            {
                                return Json(new { success = false, message = "Some records failed to import." });
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(new { success = false, message = "Error while importing file: " + ex.Message });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Your Excel file contains no data." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = response.statusCode == 102 ? "Please import a file with the proper format." : "Your Excel file contains no data." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }

        public static class DataTableConverterLeaveSanction
        {
            public static ImportResponseLeaveSanction ConvertToImportedDataLeaveSanctionList(DataTable dt, HttpContextBase httpContext)
            {
                ImportResponseLeaveSanction res = new ImportResponseLeaveSanction();

                if (dt == null || dt.Rows.Count == 0)
                {
                    res.status = false;
                    res.statusCode = 103;
                    return res;
                }
                try
                {
                    var filteredRows = dt.AsEnumerable()
                        .Where(row => !row.ItemArray.All(f => f is DBNull || string.IsNullOrWhiteSpace(f.ToString())));
                    if (!filteredRows.Any())
                    {
                        res.status = false;
                        res.statusCode = 103;
                        return res;
                    }
                    DataTable dt1 = filteredRows.CopyToDataTable();

                    string[] requiredColumns = { "Empcode", "LeaveType", "LeaveBalance", "BalanceDate" };
                    bool allColumnsExist = requiredColumns.All(col => dt1.Columns.Contains(col));

                    if (!allColumnsExist)
                    {
                        res.status = false;
                        res.statusCode = 102;
                        return res;
                    }

                    res.LevaeSanctionList = dt1.AsEnumerable().Select(row => new LevaeSanctionUploadModel
                    {
                        EmpCode = row["Empcode"].ToString(),
                        LeaveType = row["LeaveType"].ToString(),
                        LeaveBalance = row["LeaveBalance"].ToString(),
                        BalanceDate = row["BalanceDate"].ToString(),
                    }).ToList();

                    res.status = true;
                    res.statusCode = 101;
                }
                catch (Exception ex)
                {
                    res.status = false;
                    res.statusCode = 103;
                }

                return res;
            }
        }
        [HttpGet]
        public JsonResult DownloadLeaveSanctionErrList(string DtFlag)
        {
            try
            {
                var faillogList = UtilitiesRestClient.ImportLeaveSanctionerror(DtFlag);
                List<ImportLeaveSanctionfaillog> LeaveSanctionFaillogList = faillogList
               .Select(InsuranceFail => new ImportLeaveSanctionfaillog
               {
                   EmpCode = InsuranceFail.EmpCode,
                   LeaveType = InsuranceFail.LeaveType,
                   LeaveBalance = InsuranceFail.LeaveBalance,
                   BalanceDate = InsuranceFail.BalanceDate,
                   Reason = InsuranceFail.Reason
               })
                .ToList();
                JsonResult result = Json(LeaveSanctionFaillogList);
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

        #region ShiftAllocation
        [CustAuthFilter]
        public ActionResult ShiftAllocation()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            try
            {
                //ViewBag.ShiftAllocationlist = RestClientTrans.ShiftAllocationGetAll();
                DateTime baseDate = DateTime.Today;
                var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                ViewBag.baseDate = baseDate.ToString("MM/dd/yyyy");
                ViewBag.weekStart = thisWeekStart.ToString("MM/dd/yyyy");
                ViewBag.weekEnd = thisWeekEnd.ToString("MM/dd/yyyy"); ;
                ViewBag.monthStart = thisMonthStart.ToString("MM/dd/yyyy"); ;
                ViewBag.monthEnd = thisMonthEnd.ToString("MM/dd/yyyy"); ;
                ViewBag.CompanyList = FillCompany();
                ViewBag.ShiftList = FillShift();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [HttpPost]
        public string ShiftAllocation(string ShiftAlloc)
        {
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //ShiftAlloc ObjShift = JsonConvert.DeserializeObject<ShiftAlloc>(ShiftAlloc);
            var dynamicDateFormat = Session["IsDateFormat"].ToString();
            var dateFormat = "";
            if (dynamicDateFormat == "dd-M-yyyy" || dynamicDateFormat == "M-dd-yyyy" || dynamicDateFormat == "yyyy-M-dd")
            {
                dateFormat = Session["IsDateFormat"].ToString().Replace("M", "MMM");
            }
            else
            {
                dateFormat = Session["IsDateFormat"].ToString().Replace("mm", "MM");
            }
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = dateFormat.ToString() };
            ShiftAlloc ObjShift = JsonConvert.DeserializeObject<ShiftAlloc>(ShiftAlloc, dateTimeConverter);
            string ad = "";
            ShiftAllocation model = new ShiftAllocation();
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(ObjShift.FromDt)))
                {
                    ViewBag.msg = "Select from date.";
                }
                else if (string.IsNullOrEmpty(Convert.ToString(ObjShift.ToDt)))
                {
                    ViewBag.msg = "Select to date.";
                }
                else if (ObjShift.ShiftID == 0)
                {
                    ViewBag.msg = "Select shift.";
                }
                if (ObjShift.SelectAllFlage != 1 && ObjShift.EmpArr == null)
                {
                    ViewBag.msg = "Select employee.";
                }
                else
                {
                    ad = RestClient.ShiftAllocationAdd(ObjShift);
                }
            }
            catch (Exception Ex)
            {
                ViewBag.error = Ex.Message;
            }
            return ad;
            //return View(r);
        }
        #region ImportShiftAllocation
        [HttpPost]
        public JsonResult ShiftAllocationUpload(HttpPostedFileBase FileUpload, string compcode, int roleid, int Companyid, int branchid, int empid)
        {
            try
            {
                if (FileUpload == null || FileUpload.ContentLength == 0)
                {
                    return Json(new { success = false, message = "Please select a file!" });
                }

                string errorMessage;
                DataTable dt = ProcessUploadedFile(FileUpload, out errorMessage);
                ImportResponseShiftAllocation response = new ImportResponseShiftAllocation();
                ImportShiftAllocationfaillogList failLog = new ImportShiftAllocationfaillogList();

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return Json(new { success = false, message = errorMessage });
                }

                response = DataTableConverter.ConvertToImportedDataShiftAllocationList(dt, this.HttpContext);

                if (response.status && response.statusCode == 101)
                {
                    if (response.ShiftAllocationList != null && response.ShiftAllocationList.Count > 0)
                    {
                        try
                        {
                            failLog.importshiftFailloglist = UtilitiesRestClient.ImportShiftAllocationDetail(response.ShiftAllocationList, compcode, roleid, Companyid, branchid, empid);
                            if (!failLog.importshiftFailloglist.Any())
                            {
                                return Json(new { success = true, message = "File imported successfully." });
                            }
                            else
                            {
                                return Json(new { success = false, message = "Some records failed to import." });
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(new { success = false, message = "Error while importing file: " + ex.Message });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Your Excel file contains no data." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = response.statusCode == 102 ? "Please import a file with the proper format." : "Your Excel file contains no data." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }

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
            public static ImportResponseShiftAllocation ConvertToImportedDataShiftAllocationList(DataTable dt, HttpContextBase httpContext)
            {
                ImportResponseShiftAllocation res = new ImportResponseShiftAllocation();

                if (dt == null || dt.Rows.Count == 0)
                {
                    res.status = false;
                    res.statusCode = 103;
                    return res;
                }

                try
                {
                    var filteredRows = dt.AsEnumerable()
                        .Where(row => !row.ItemArray.All(f => f is DBNull || string.IsNullOrWhiteSpace(f.ToString())));

                    if (!filteredRows.Any())
                    {
                        res.status = false;
                        res.statusCode = 103;
                        return res;
                    }

                    DataTable dt1 = filteredRows.CopyToDataTable();

                    string[] requiredColumns = { "EmpCode", "Shift", "FromDate", "ToDate" };
                    bool allColumnsExist = requiredColumns.All(col => dt1.Columns.Contains(col));

                    if (!allColumnsExist)
                    {
                        res.status = false;
                        res.statusCode = 102;
                        return res;
                    }

                    res.ShiftAllocationList = dt1.AsEnumerable().Select(row => new ShiftAllocationUploadModel
                    {
                        EmpCode = row["EmpCode"].ToString(),
                        ShiftName = row["Shift"].ToString(),
                        Fromdate = row["FromDate"].ToString(),
                        Todate = row["ToDate"].ToString(),
                    }).ToList();

                    res.status = true;
                    res.statusCode = 101;
                }
                catch (Exception ex)
                {
                    res.status = false;
                    res.statusCode = 103;
                }

                return res;
            }
        }

        [HttpGet]
        public JsonResult DownloadShiftAllocationErrList(string DtFlag)
        {
            try
            {
                var faillogList = UtilitiesRestClient.ImportShiftAllocationerror(DtFlag);
                List<ImportShiftAllocationfaillog> shiftallocationFaillogList = faillogList
               .Select(InsuranceFail => new ImportShiftAllocationfaillog
               {
                   EmpCode = InsuranceFail.EmpCode,
                   ShiftName = InsuranceFail.ShiftName,
                   Fromdate = InsuranceFail.Fromdate,
                   Todate = InsuranceFail.Todate,
                   Reason = InsuranceFail.Reason
               })
                .ToList();
                JsonResult result = Json(shiftallocationFaillogList);
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
        [HttpPost]
        public JsonResult GetAllShiftAllocation(DataTableAjaxPostModelShiftallocation model)
        {
            try
            {
                int cmpid = 0, Brchid = 0, empid = 0;

                if (Convert.ToInt32(Session["RoleId"]) == 6805)
                    cmpid = Convert.ToInt32(Session["ClientCompanyId"]);
                if (Convert.ToInt32(Session["RoleId"]) == 6806)
                    Brchid = Convert.ToInt32(Session["BranchId"]);
                if (Convert.ToInt32(Session["RoleId"]) > 1)
                    empid = Convert.ToInt32(Session["EmpId"]);

                var allData = RestClient.ShiftAllocationGetAll(
                    model.BranchID, model.fromdt, model.todt, cmpid, Brchid,
                    Session["RoleId"].ToString(), empid
                );

                // Search
                if (!string.IsNullOrEmpty(model.search?.value))
                {
                    string searchValue = model.search.value.ToLower();
                    allData = allData.Where(x =>
                        (x.EmpName != null && x.EmpName.ToLower().Contains(searchValue)) ||
                        x.EmpPunchId.ToString().ToLower().Contains(searchValue) ||
                        (x.ShiftName != null && x.ShiftName.ToLower().Contains(searchValue)) ||
                        (x.ShiftDateStr != null && x.ShiftDateStr.ToLower().Contains(searchValue))
                    ).ToList();
                }

                // Sorting
                var sortColumn = model.columns[model.order[0].column].data;
                var sortDirection = model.order[0].dir;

                if (!string.IsNullOrEmpty(sortColumn))
                {
                    switch (sortColumn)
                    {
                        case "EmpName":
                            allData = sortDirection == "asc"
                                ? allData.OrderBy(x => x.EmpName).ToList()
                                : allData.OrderByDescending(x => x.EmpName).ToList();
                            break;
                        case "EmpPunchId":
                            allData = sortDirection == "asc"
                                ? allData.OrderBy(x => x.EmpPunchId).ToList()
                                : allData.OrderByDescending(x => x.EmpPunchId).ToList();
                            break;
                        case "ShiftName":
                            allData = sortDirection == "asc"
                                ? allData.OrderBy(x => x.ShiftName).ToList()
                                : allData.OrderByDescending(x => x.ShiftName).ToList();
                            break;
                        case "ShiftDateStr":
                            allData = sortDirection == "asc"
                                ? allData.OrderBy(x => x.ShiftDateStr).ToList()
                                : allData.OrderByDescending(x => x.ShiftDateStr).ToList();
                            break;
                    }
                }

                var filteredData = allData.Skip(model.start).Take(model.length).ToList();
                var totalRecords = allData.Count();

                return Json(new
                {
                    draw = model.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords,
                    data = filteredData
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DatewiseShiftAllocation(string fromDate, string toDate)
        {
            JsonResult result;
            try
            {
                var datewicedata = RestClient.ShiftAllocationDatewice(fromDate, toDate);
                return Json(datewicedata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result = Json(ex.ToString());
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }



        [HttpPost]
        public JsonResult GetShiftAllocationDateWice(string fromDate, string toDate)
        {
            DateTime sdate = DateTime.Parse(fromDate);
            DateTime edate = DateTime.Parse(toDate);

            JsonResult result;
            try
            {
                var allEmp = MasterRestClient.EmployeeGetAll();
                IEnumerable<Holidays> allholidays = MasterRestClient.HolidaysGetAll();
                String[][] jaggedArray = new String[allEmp.Count()][];
                int i = 0;
                foreach (Employees e in allEmp)
                {
                    int j = 0;
                    //IEnumerable<Leave> empLeave = RestClient.OnDutyLeaveGetAll().Where(d => d.EmpId == e.EmpId && (d.LeaveStatus == 1 || d.LeaveStatus == 0));
                    //IEnumerable<ShiftAllocation> shiftalloc = RestClient.ShiftAllocationGetAll().Where(d => d.EmpCode == e.EmpId && d.WO == 0 && d.HO == 0 && d.ShiftDate.Date >= sdate.Date && d.ShiftDate.Date <= edate.Date).OrderBy(d => d.ShiftDate);
                    IEnumerable<Leave> empLeave = RestClient.OnDutyLeaveGetAll(e.EmpId);
                    IEnumerable<ShiftAllocation> shiftalloc = RestClient.ShiftAllocationGetAll(e.EmpId, sdate.Date, edate.Date);

                    int c = ((empLeave.ToList().Count() + shiftalloc.ToList().Count()) * 2) + 25;
                    jaggedArray[i] = new String[c];
                    jaggedArray[i][j++] = e.EmpId.ToString();
                    jaggedArray[i][j++] = e.EmpName;
                    jaggedArray[i][j++] = e.EmpPhoto;
                    jaggedArray[i][j++] = e.ShiftId.ToString();
                    jaggedArray[i][j++] = e.ShiftName;
                    jaggedArray[i][j++] = e.ShiftGroupId.ToString();
                    jaggedArray[i][j++] = e.ShiftGroupName;

                    //HRPolicy empPolicy = MasterRestClient.HrPolicyGetbyID(e.PolicyId);
                    jaggedArray[i][j++] = e.EmpWeekOff.ToString();
                    jaggedArray[i][j++] = e.EmpSecondWeekOff.ToString();
                    jaggedArray[i][j++] = e.EmpSecondWeekOffRule.ToString();
                    jaggedArray[i][j++] = e.EmpHalfDay.ToString();
                    jaggedArray[i][j++] = e.EmpHalfDayRule.ToString();




                    foreach (Leave l in empLeave)
                    {
                        jaggedArray[i][j++] = l.FromDate1;
                        jaggedArray[i][j++] = l.ToDate1;
                    }
                    jaggedArray[i][j++] = "Holidays";
                    foreach (Holidays h in allholidays)
                    {
                        jaggedArray[i][j++] = Convert.ToString(h.HolidayDate);
                    }
                    jaggedArray[i][j++] = "ShiftAllocation";
                    foreach (ShiftAllocation sa in shiftalloc)
                    {
                        jaggedArray[i][j++] = sa.ShiftDate.ToString();
                        jaggedArray[i][j++] = sa.ShiftName;

                    }
                    i++;
                }
                // var datewicedata = RestClient.ShiftAllocationDatewice(fromDate, toDate);
                return Json(jaggedArray, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result = Json(ex.ToString());
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

       
        #endregion

        #region PalmVerify
        public ActionResult PalmVerify()
        {
            return View();
        }
        #endregion

        #region DutyLeaveModule
        public ActionResult DutyLeaveModule()
        {
            return View();
        }
        #endregion

        #region "OnDuty/Leave"

        public ActionResult OnDutyLeave()
        {
            try
            {
                ViewBag.CompanyList = FillCompany();
                ViewBag.LeaveTypeList = FillLeaveType();
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        [HttpPost]
        public string OnDutyLeave(string Leave)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            Leave objOnDutyLeave = js.Deserialize<Leave>(Leave);

            string ad = "";
            ViewBag.msg = "";
            ViewBag.error = "";

            if (Convert.ToString(objOnDutyLeave.FromDate) == "" || Convert.ToString(objOnDutyLeave.ToDate) == "")
            {
                return "Please enter date.";
            }
            if (objOnDutyLeave.FromDate > objOnDutyLeave.ToDate)
            {
                return "Please enter valid date.";
            }


            string resp = "no";
            resp = MasterRestClient.LeaveTypeGetAll().Where(x => x.LeaveTypeId == objOnDutyLeave.LeaveTypeId).Select(d => d.HalfLeave).SingleOrDefault().ToString();
            if (objOnDutyLeave.IsHalfLeave == true && resp != null && resp == "False")
            {
                return "HalfLeave doesn't applicable for this leave type.";
            }
            if (objOnDutyLeave.LeaveId > 0)
            {
                var res = RestClient.OnDutyLeaveGetAllCheck(Convert.ToString(objOnDutyLeave.FromDate), Convert.ToString(objOnDutyLeave.ToDate), objOnDutyLeave.EmpId, objOnDutyLeave.IsHalfLeave, objOnDutyLeave.LeaveTypeId, objOnDutyLeave.LeavePaid);
                if (res.MegSts == "OK")
                {
                    if (!RestClient.OnDutyLeaveUpdate(objOnDutyLeave.LeaveId, objOnDutyLeave))
                    {
                        ad = "Leave not updated due to service issue.";
                    }
                    else
                    {
                        //ad = "OK";
                        if (ad == "OK")
                        {
                            ad = "OK";
                        }
                    }
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
                if (res.MegSts == "OK")
                {
                    objOnDutyLeave.LeaveStatus = 1;
                    objOnDutyLeave.ApprovalReason = "Approved by admin";
                    ad = RestClient.OnDutyLeaveAdd(objOnDutyLeave);
                    if (ad == "OK")
                    {
                        ad = "OK";
                    }

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
            return ad;
        }
        [HttpPost]
        public JsonResult GetAllOnDutyLeave(string Empid, int BranchID = 0, int selectAll = 0, string selectAllSearchTerm = "")
        {
            JsonResult result;
            try
            {
                //if (BranchID > 0)
                //{
                //    var AllEmp = MasterRestClient.EmployeeGetAll().Where(x => x.BranchId == BranchID).Select(x => x.EmpId).Distinct();
                //    var AllData = RestClient.OnDutyLeaveGetAll().Where(x => AllEmp.Contains(Convert.ToInt32(x.EmpId)));
                //    return Json(AllData, JsonRequestBehavior.AllowGet);
                //}
                //switch (Convert.ToInt32(Session["RoleId"]))
                //{
                //    case 6805:
                //    {
                //        var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                //        var AllEmp = MasterRestClient.EmployeeGetAll().Where(x => x.CompanyID == cmpid).Select(x => x.EmpId).Distinct();
                //        var AllData = RestClient.OnDutyLeaveGetAll().Where(x => AllEmp.Contains(Convert.ToInt32(x.EmpId)));
                //        return Json(AllData, JsonRequestBehavior.AllowGet);
                //    }
                //    case 6806:
                //    {
                //        var Brchid = Convert.ToInt32(Session["BranchId"].ToString());
                //        var AllEmp = MasterRestClient.EmployeeGetAll().Where(x => x.BranchId == Brchid).Select(x => x.EmpId).Distinct();
                //        var AllData = RestClient.OnDutyLeaveGetAll().Where(x => AllEmp.Contains(Convert.ToInt32(x.EmpId)));
                //        return Json(AllData, JsonRequestBehavior.AllowGet);
                //    }
                //    default:
                //    {
                //        var AllData = RestClient.OnDutyLeaveGetAll();
                //        return Json(AllData, JsonRequestBehavior.AllowGet);
                //    }
                //}

                var cmpid = 0;
                var Brchid = 0;
                string _empId = string.Empty;
                if (Convert.ToInt32(Session["RoleId"]) == 6805)
                {
                    cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                }
                if (Convert.ToInt32(Session["RoleId"]) == 6806)
                {
                    Brchid = Convert.ToInt32(Session["BranchId"].ToString());
                }

                _empId = Convert.ToString(Session["EmpId"]);
                var AllData = RestClient.OnDutyLeaveGetAll(BranchID, cmpid, Brchid, Session["RoleId"].ToString(), _empId, selectAll, selectAllSearchTerm);
                var jsonData = JsonConvert.SerializeObject(AllData);




                // Compress the data
                byte[] compressedData = CompressionUtils.Compress(jsonData);

                return Json(Convert.ToBase64String(compressedData), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                result = Json(ex.ToString());
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        public string DeleteOnDutyLeave(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.OnDutyLeaveDelete(Convert.ToInt64(id)))
                {
                    sts = "OK";
                }
            }
            return sts;
        }
        [HttpPost]
        public JsonResult GetBalanceByLeaveType(string OnDutyLeave)
        {
            JsonResult result;
            try
            {
                var AllData = RestClient.GetBalanceByLeaveType(OnDutyLeave);
                return Json(AllData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result = Json(ex.ToString());
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        [HttpGet]
        public string HalfLeaveValidate(int LeaveType)
        {
            string res = "no";
            res = MasterRestClient.LeaveTypeGetAll().Where(x => x.LeaveTypeId == LeaveType).Select(d => d.HalfLeave).SingleOrDefault().ToString();
            if (res != null && res == "True")
            {
                res = "yes";
            }
            else
            {
                res = "no";
            }
            return res;
        }
        [HttpGet]
        public JsonResult DocumentRequireForLeave(int LeaveType)
        {
            JsonResult result;
            try
            {
                var AllData = MasterRestClient.LeaveTypeGetAll().Where(x => x.LeaveTypeId == LeaveType);
                return Json(AllData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result = Json(ex.ToString());
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
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
            return RedirectToAction("OnDutyLeave");
        }

        #endregion

        #region "LeaveEncashCarry"
        public ActionResult LeaveCarryForward()
        {
            try
            {
                ViewBag.CompanyList = FillCompany();
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [HttpPost]
        public string LeaveEncashCarry(string EmpArr)
        {
            string ad = "";
            ViewBag.msg = "";
            ViewBag.error = "";
            LeaveEncashEmp ObjDeptAr = new LeaveEncashEmp();
            ObjDeptAr.Emps = EmpArr;

            ad = RestClient.LeaveEncashCarryAdd(ObjDeptAr);


            return ad;
        }
        [HttpGet]
        public JsonResult GetLeaveEncashCarryEmp(string DeptArr)
        {
            JsonResult result;
            try
            {
                var AllData = RestClient.LeaveEncashCarryGetAllEmp(DeptArr);
                return Json(AllData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                result = null;
                return result;
            }
        }
        [HttpPost]
        public JsonResult GetAllLeaveEncashCarry(int BranchID = 0)
        {
            JsonResult result;
            try
            {
                if (BranchID > 0)
                {
                    var AllEmp = MasterRestClient.EmployeeGetAll().Where(x => x.BranchId == BranchID).Select(x => x.EmpId).Distinct();
                    var AllData = RestClient.LeaveEncashCarryGetAll().Where(x => AllEmp.Contains(Convert.ToInt32(x.EmpCode))).Select(i => new { i.LeaveEncashCarryId, i.LeaveBalance, i.LeaveTypeName, i.TransactionYearName, i.EmpName, i.EncashCarry });

                    return Json(AllData, JsonRequestBehavior.AllowGet);
                }

                switch (Convert.ToInt32(Session["RoleId"]))
                {
                    case 6805:
                        {
                            var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                            var AllEmp = MasterRestClient.EmployeeGetAll().Where(x => x.CompanyID == cmpid).Select(x => x.EmpId).Distinct();
                            var AllData = RestClient.LeaveEncashCarryGetAll().Where(x => AllEmp.Contains(Convert.ToInt32(x.EmpCode))).Select(i => new { i.LeaveEncashCarryId, i.LeaveBalance, i.LeaveTypeName, i.TransactionYearName, i.EmpName, i.EncashCarry });

                            return Json(AllData, JsonRequestBehavior.AllowGet);
                        }
                    case 6806:
                        {
                            var Brchid = Convert.ToInt32(Session["BranchId"].ToString());
                            var AllEmp = MasterRestClient.EmployeeGetAll().Where(x => x.BranchId == Brchid).Select(x => x.EmpId).Distinct();
                            var AllData = RestClient.LeaveEncashCarryGetAll().Where(x => AllEmp.Contains(Convert.ToInt32(x.EmpCode))).Select(i => new { i.LeaveEncashCarryId, i.LeaveBalance, i.LeaveTypeName, i.TransactionYearName, i.EmpName, i.EncashCarry });
                            return Json(AllData, JsonRequestBehavior.AllowGet);
                        }
                    default:
                        {
                            var AllData = RestClient.LeaveEncashCarryGetAll();
                            return Json(AllData, JsonRequestBehavior.AllowGet);
                        }
                }
            }
            catch (Exception ex)
            {
                result = Json(ex.ToString());
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        #endregion

        #region "LeaveEncashCarryRollback"
        public ActionResult LeaveCarryForwardRollback()
        {
            try
            {
                ViewBag.CompanyList = FillCompany();
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [HttpGet]
        public JsonResult GetLeaveEncashCarryEmpRollback(string DeptArr)
        {
            JsonResult result;
            try
            {
                if (DeptArr != "")
                {
                    var AllData = RestClient.LeaveEncashCarryGetAllEmpRollback(DeptArr);
                    return Json(AllData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                result = Json(ex.ToString());
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        [HttpPost]
        public string Rollback(string EmpArr)
        {
            string sts = "";
            if (EmpArr != "")
            {
                if (RestClient.Rollback(EmpArr))
                {
                    sts = "OK";
                }
                else
                {
                    sts = "EmpID not found.";
                }
            }
            return sts;
        }
        #endregion

        #region ManualPunch
        public ActionResult ManualPunching()
        {
            try
            {
                ViewBag.CompanyList = FillCompany();
                ViewBag.FillDevice = FillDevice();
                ViewBag.FillIPList = MasterRestClient.DeviceGetAll();
                ViewBag.FillDeviceType = MasterRestClient.DeviceTypeGetAll();
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        [HttpPost]
        public JsonResult GetAllManualPunch(string jsondata, DataTableAjaxPostModel model)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic item = serializer.Deserialize<object>(jsondata);
            int branchID = Convert.ToInt32(item["BranchID"]);
            string fromdate = Convert.ToString(item["fromdate"]);
            string todate = Convert.ToString(item["todate"]);
            model.branchid = branchID;
            model.fromdate = fromdate;
            model.todate = todate;
            //  JsonResult jsonresult = item;

            //var AllEmp = MasterRestClient.EmployeeGetAll().Where(x => x.BranchId == branchID).Select(x => x.EmpId).Distinct();

            var AllData = RestClient.ManualPunchGetAll(model).Select(i => new { i.Empcode, i.EmpName, i.In_Out_Time, i.mode });
            DatatableCounts DataCount = RestClient.ManualPunchGetAllCount(model);
            var result = new List<ManualPunch>(AllData.Count());
            foreach (var data in AllData)
            {
                // simple remapping adding extra info to found dataset
                result.Add(new ManualPunch
                {
                    Empcode = data.Empcode,
                    EmpName = data.EmpName,
                    In_Out_Time = data.In_Out_Time,
                    mode = data.mode

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


            //try
            //{
            //    var AllEmp = MasterRestClient.EmployeeGetAll().Where(x => x.BranchId == branchID).Select(x => x.EmpId).Distinct();
            //    if (AllEmp.Count() != 0)
            //    {
            //        var AllData = RestClient.ManualPunchGetAll().Select(i => new { i.Empcode, i.EmpName, i.In_Out_Time, i.mode });
            //        return Json(AllData, JsonRequestBehavior.AllowGet);
            //    }
            //    else
            //    {
            //        var AllData = "";
            //        return Json(AllData, JsonRequestBehavior.AllowGet);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    result = Json(ex.ToString());
            //    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //    return result;
            //}
        }
        [HttpPost]
        public JsonResult ManualPunch(string ManualPunch)
        {
            JsonResult result;
            //JavaScriptSerializer js = new JavaScriptSerializer();
            var dynamicDateFormat = Session["IsDateFormat"].ToString();
            var isDateFormat = "";
            if (dynamicDateFormat == "dd-M-yyyy" || dynamicDateFormat == "M-dd-yyyy" || dynamicDateFormat == "yyyy-M-dd")
            {
                isDateFormat = Session["IsDateFormat"].ToString().Replace("M", "MMM");
            }
            else
            {
                isDateFormat = Session["IsDateFormat"].ToString().Replace("mm", "MM");
            }
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = isDateFormat.ToString() };
            ReqManualPunch objManualPunching = JsonConvert.DeserializeObject<ReqManualPunch>(ManualPunch, dateTimeConverter);
            string ad = "";
            ViewBag.msg = "";
            ViewBag.error = "";
            ReqManualPunch obj = objManualPunching;
            obj.EntryMode = 0;
            obj.Punchid = 1;
            obj.ShiftCode = 0;
            ad = RestClient.ManualPunchAdd(obj);
            result = Json(new { ad = ad });
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        [HttpPost]
        public JsonResult ClearData(UpdateAttendance obj)
        {
            JsonResult result;
            string res = string.Empty;
            var Isdateformat = Session["IsDateFormat"].ToString();
            //if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            //{
            //    var fromDate = obj.fromdate.Split('-');
            //    obj.fromdate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1] + " 00:00:00";
            //    var toDate = obj.todate.Split('-');
            //    obj.todate = toDate[2] + "-" + toDate[0] + "-" + toDate[1] + " 00:00:00";
            //}
            if (Isdateformat == "dd-M-yyyy" || Isdateformat == "M-dd-yyyy" || Isdateformat == "yyyy-M-dd")
            {
                obj.fromdate = Convert.ToDateTime(obj.fromdate).ToString("yyyy-MM-dd") + " 00:00:00";
                obj.todate = Convert.ToDateTime(obj.todate).ToString("yyyy-MM-dd") + " 00:00:00";
            }
            else
            {
                obj.fromdate = Convert.ToDateTime(obj.fromdate).ToString("yyyy-MM-dd") + " 00:00:00";
                obj.todate = Convert.ToDateTime(obj.todate).ToString("yyyy-MM-dd") + " 00:00:00";
            }
            //obj.fromdate = obj.fromdate + " 00:00:00";
            //obj.todate = obj.todate + " 00:00:00";
            //DateTime frmdate = Convert.ToDateTime(fromdate);
            //DateTime tdate = Convert.ToDateTime(todate);
            //string emp = string.Join(",", empids.Select(item => "'" + item + "'"));
            res = RestClient.cleardata(obj);
            result = Json(new { ad = res.ToLower() });
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        #endregion


        #region Device Transaction Monitor Data
        public ActionResult Transactionmonitor()
        {
            //try
            //{
            //    //ViewBag.LeaveTypeList = FillLeaveType();
            //    //if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            //    //{
            //    //    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
            //    //    ViewBag.Companyslist = MasterRestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid).ToList();
            //    //}
            //    //else if (Convert.ToInt32(Session["RoleId"].ToString()) > 1 && Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
            //    //{
            //    //    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
            //    //    ViewBag.Companyslist = MasterRestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid).ToList();
            //    //}
            //    //else
            //    //{
            //    //    ViewBag.Companyslist = MasterRestClient.CompanyGetAll();
            //    //}

            //    return View();
            //}
            //catch (Exception)
            //{
            //    return RedirectToAction("ErrorPage", "PayTime");
            //}
            ViewBag.LeaveTypeList = FillLeaveType();
            if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                ViewBag.Companyslist = MasterRestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid).ToList();
            }
            else if (Convert.ToInt32(Session["RoleId"].ToString()) > 1 && Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                ViewBag.Companyslist = MasterRestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid).ToList();
            }
            else
            {
                ViewBag.Companyslist = MasterRestClient.CompanyGetAll();
            }
            return View();
        }

        [HttpPost]
        public JsonResult Transactionmonitor(string jsondata, DataTableTransactionMonitorPostModel model)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic item = serializer.Deserialize<object>(jsondata);
            int Deviceid = Convert.ToInt32(item["DeviceID"]);
            model.deviceid = Deviceid;
            //  JsonResult jsonresult = item;

            //var AllEmp = MasterRestClient.EmployeeGetAll().Where(x => x.BranchId == branchID).Select(x => x.EmpId).Distinct();

            var AllData = RestClient.TransactionmonitorGetAll(model).Select(i => new { i.EmpId, i.Punchid, i.EmpName, i.PunchTime, i.DeviceId, i.DeviceName, i.DeviceIP, i.DeviceType });
            DatatableCounts DataCount = RestClient.TransactionmonitorGetAllCounts(model);
            var result = new List<Transactionmonitor>(AllData.Count());
            foreach (var data in AllData)
            {
                // simple remapping adding extra info to found dataset
                result.Add(new Transactionmonitor
                {
                    EmpId = data.EmpId,
                    Punchid = data.Punchid,
                    EmpName = data.EmpName,
                    PunchTime = data.PunchTime,
                    DeviceId = data.DeviceId,
                    DeviceName = data.DeviceName,
                    DeviceIP = data.DeviceIP,
                    DeviceType = data.DeviceType
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
        /// <summary>
        /// TransactionmonitorFilter is used for the filter the data of employee and from and to date wise.
        /// </summary>
        [HttpPost]
        public JsonResult TransactionmonitorFilter(string jsondata, DataTableTransactionMonitorFilterPostModel model)
        {

            dynamic item = JsonConvert.DeserializeObject(jsondata);
            //model.deviceid = Convert.ToInt32(item["DeviceID"]);
            model.deviceid = item["DeviceID"];
            model.fromdate = item["FromDate"];
            model.todate = item["ToDate"];
            model.empid = item["EmpId"];
            model.flag = item["flag"];
            model.companyid = item["CompanyId"];
            try
            {
                var AllData = RestClient.TransactionmonitorGetAll(model)
                    .Select(i => new
                    {
                        i.EmpId,
                        i.Punchid,
                        i.EmpName,
                        i.PunchTime,
                        i.DeviceId,
                        i.DeviceName,
                        i.DeviceIP,
                        i.DeviceType,
                        i.Entrydate,
                    }).ToList();


                if (model.flag == "excel")
                {

                    return new JsonResult
                    {
                        Data = new
                        {
                            draw = model.draw,
                            recordsTotal = AllData.Count(),
                            recordsFiltered = AllData.Count(),
                            data = AllData
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = Int32.MaxValue
                    };
                }


                DatatableCounts DataCount = RestClient.TransactionmonitorGetAllCounts(model);
                var resultList = AllData.Select(data => new Transactionmonitor
                {
                    EmpId = data.EmpId,
                    Punchid = data.Punchid,
                    EmpName = data.EmpName,
                    PunchTime = data.PunchTime,
                    DeviceId = data.DeviceId,
                    DeviceName = data.DeviceName,
                    DeviceIP = data.DeviceIP,
                    DeviceType = data.DeviceType,
                    Entrydate = data.Entrydate
                }).ToList();

                return Json(new
                {
                    draw = model.draw,
                    recordsTotal = DataCount.totalResultsCount,
                    recordsFiltered = DataCount.filteredResultsCount,
                    data = resultList
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = "An error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        //public JsonResult TransactionmonitorFilter(string jsondata, DataTableTransactionMonitorFilterPostModel model)
        //{
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    dynamic item = serializer.Deserialize<object>(jsondata);
        //    int Deviceid = Convert.ToInt32(item["DeviceID"]);
        //    model.deviceid = Deviceid;
        //    string FromDate = item["FromDate"];
        //    model.fromdate = FromDate;
        //    string ToDate = item["ToDate"];
        //    model.todate = ToDate;
        //    string empId = item["EmpId"];
        //    model.empid = empId;
        //    string flag = item["flag"];
        //    model.flag = flag;

        //    var AllData = RestClient.TransactionmonitorGetAll(model)
        //        .Select(i => new {
        //            i.EmpId,
        //            i.Punchid,
        //            i.EmpName,
        //            i.PunchTime,
        //            i.DeviceId,
        //            i.DeviceName,
        //            i.DeviceIP,
        //            i.DeviceType
        //        });


        //    if (model.flag == "excel")
        //    {
        //        return Json(new
        //        {

        //            draw = model.draw,
        //            recordsTotal = AllData.Count(),
        //            recordsFiltered = AllData.Count(),
        //            data = AllData
        //        }, JsonRequestBehavior.AllowGet);
        //    }

        //    // If flag is not "excel", apply pagination and get counts
        //    DatatableCounts DataCount = RestClient.TransactionmonitorGetAllCounts(model);

        //    var result = new List<Transactionmonitor>(AllData.Count());
        //    foreach (var data in AllData)
        //    {
        //        result.Add(new Transactionmonitor
        //        {
        //            EmpId = data.EmpId,
        //            Punchid = data.Punchid,
        //            EmpName = data.EmpName,
        //            PunchTime = data.PunchTime,
        //            DeviceId = data.DeviceId,
        //            DeviceName = data.DeviceName,
        //            DeviceIP = data.DeviceIP,
        //            DeviceType = data.DeviceType
        //        });
        //    };

        //    return Json(new
        //    {
        //        // This is for DataTables when pagination is needed
        //        draw = model.draw,
        //        recordsTotal = DataCount.totalResultsCount,
        //        recordsFiltered = DataCount.filteredResultsCount,
        //        data = result
        //    }, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult Transactionmonitorpartial()
        {
            try
            {
                var model = RestClient.TransactionmonitorGetAll();
                return PartialView("Transactionmonitorpartial", model);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [HttpGet]
        public JsonResult GetEmployeeByDeviceId(string DeviceId, string fromdate, string ToDate, string branchid, string companyid)
        {
            JsonResult result;
            try
            {
                var EmployeeGetByDeviceId = RestClient.GetEmployeeByDeviceId(DeviceId, fromdate, ToDate, branchid, companyid);
                result = Json(EmployeeGetByDeviceId);
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


        #region DataRcv
        [HttpPost]
        public string DataReceiveProcess(string EmpArr)
        {
            string ad = "";
            ViewBag.msg = "";
            ViewBag.error = "";
            //int[] deptLst = EmpArr.Split(',').Select(Int32.Parse).ToArray();
            DataReceive ObjDeptAr = new DataReceive();
            ObjDeptAr.Emps = EmpArr;
            ad = RestClient.DataReceiveProcessADD(ObjDeptAr);
            //ad = "OK";

            return ad;
        }
        #endregion

        public ActionResult DataReceive()
        {
            try
            {
                ViewBag.CompanyList = FillCompany();
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }



        #region ComboCommon
        public List<SelectListItem> FillCompany()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            Companys com = new Companys();
            IEnumerable<Companys> comList;
            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) == 1)
            {
                comList = MasterRestClient.CompanyGetAll();
            }
            else if (Convert.ToInt32(Session["RoleId"].ToString()) > 1 && Session["RoleId"].ToString() != "6806" && Session["RoleId"].ToString() != "6805")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                comList = MasterRestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
            }
            else
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                comList = MasterRestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
            }

            foreach (var i in comList)
            {
                list.Add(new SelectListItem() { Text = i.CompanyName, Value = Convert.ToString(i.CompanyID) });
            }
            return list;
        }
        [HttpPost]
        public JsonResult FillBranch(int id)
        {
            Branches b = new Branches();
            List<Branches> bList = new List<Branches>();
            if (Session["RoleId"].ToString() == "6806")
            {
                var brachid = Convert.ToInt32(Session["BranchId"].ToString());
                bList = MasterRestClient.BranchGetAll().Where(x => x.CompanyID == id && x.BranchId == brachid).ToList();
            }
            else if (Convert.ToInt32(Session["RoleId"].ToString()) > 1 && Session["RoleId"].ToString() != "6806" && Session["RoleId"].ToString() != "6805")
            {
                var brachid = Convert.ToInt32(Session["BranchId"].ToString());
                bList = MasterRestClient.BranchGetAll().Where(x => x.CompanyID == id && x.BranchId == brachid).ToList();
            }
            else
            {
                bList = MasterRestClient.BranchGetAll().Where(x => x.CompanyID == id).ToList();
            }

            return Json(bList, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult FillBranchForOnDutyLeave(int id)
        {
            //if (Session["UserEmail"].ToString() != "")
            //{
            Branches b = new Branches();
            List<Branches> bList = new List<Branches>();
            if (Session["RoleId"].ToString() == "6806")
            {
                var brachid = Convert.ToInt32(Session["BranchId"].ToString());
                bList = MasterRestClient.BranchGetAll().Where(x => x.CompanyID == id && x.BranchId == brachid).ToList();
            }
            else if (Session["RoleId"].ToString() != "6806" && Session["RoleId"].ToString() != "6805" && Convert.ToInt32(Session["RoleId"].ToString()) > 1)
            {
                var brachid = Convert.ToInt32(Session["BranchId"].ToString());
                bList = MasterRestClient.BranchGetAll().Where(x => x.CompanyID == id && x.BranchId == brachid).ToList();
            }
            else
            {
                bList = MasterRestClient.BranchGetAll().Where(x => x.CompanyID == id).ToList();
            }
            return Json(new { data = bList, status = true, JsonRequestBehavior.AllowGet });
            //}
            //else
            //{
            //    return Json(new { data = "", status = false });
            //}

        }


        [HttpPost]
        public JsonResult GetDeptbyBranch(int BranchId = 0)
        {
            JsonResult result;
            try
            {
                var AllDeptCode = MasterRestClient.EmployeeGetAll();
                IEnumerable<int> BranchWiseDeptCode;
                if (BranchId == 0)
                {
                    BranchWiseDeptCode = AllDeptCode.Select(x => x.DepartmentId).Distinct();
                }
                else
                {
                    BranchWiseDeptCode = AllDeptCode.Where(x => x.BranchId == BranchId).Select(x => x.DepartmentId).Distinct();
                }

                var BranchWiseDept = MasterRestClient.DepartmentGetAll().Where(x => BranchWiseDeptCode.Contains(x.DepartmentId));
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
        public JsonResult GetDivisionbyclass(int Class = 0)
        {
            JsonResult result;
            try
            {
                var AllDeptCode = Schoolmaster.GetDivisionbyclass(Class);
                result = Json(new SelectList(AllDeptCode, "DepartmentId", "DepartmentName"));
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
        [HttpPost]
        public JsonResult GetEmpByDept(string empArr)
        {
            JsonResult result;
            try
            {
                string[] passedVal = empArr.Split(';');
                int hierarchyState = Session["Ishierchy"] != null ? Convert.ToInt32(Session["Ishierchy"]) : 0;
                IEnumerable<Employees> otherObjects;
                string branchId = string.Empty;
                if (Convert.ToString(passedVal[1]) != "null")
                {
                    branchId = passedVal[1];
                }
                var DeptList = passedVal[0].Split(',').Select(Int32.Parse).ToList();
                var BranchList = passedVal[1].Split(',').Select(Int32.Parse).ToList();
                if (string.IsNullOrEmpty(branchId))
                {
                    otherObjects = MasterRestClient.EmployeeGetAll().OrderBy(x => x.EmpName).Where(x => (DeptList.Contains(x.DepartmentId)) && (x.IsActive = true));
                }
                else
                {
                    //--- Ishierchy not use mobile app apply mobile develpoment the use this
                    //if (Session["Ishierchy"] == null) //kashyap - need to check when ishierarchy is live 
                    //{
                    //    otherObjects = MasterRestClient.EmployeeGetAll(hierarchyState).OrderBy(x => x.EmpName).Where(x => (x.IsActive == true));
                    //}
                    //else
                    //{
                    otherObjects = MasterRestClient.EmployeeGetAlls(hierarchyState).OrderBy(x => x.EmpName).Where(x => (DeptList.Contains(x.DepartmentId)) && (BranchList.Contains(x.BranchId)) && (x.IsActive == true));
                    //}
                }
                result = Json(otherObjects);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.MaxJsonLength = int.MaxValue;
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
        public JsonResult GetEmpByDesig(string empArr)
        {
            JsonResult result;
            try
            {
                string[] passedVal = empArr.Split(';');
                //if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Session["RoleId"].ToString() != "1")
                //{
                //    int Ishierchy = Convert.ToInt32(Session["Ishierchy"]);
                //}
                int hierarchyState = Session["Ishierchy"] != null ? Convert.ToInt32(Session["Ishierchy"]) : 0;
                IEnumerable<Employees> otherObjects;
                string branchId = string.Empty;
                if (Convert.ToString(passedVal[1]) != "null")
                {
                    branchId = passedVal[1];
                }
                var DesigList = passedVal[0].Split(',').Select(Int32.Parse).ToList();
                var BranchList = passedVal[1].Split(',').Select(Int32.Parse).ToList();
                if (string.IsNullOrEmpty(branchId))
                {
                    otherObjects = MasterRestClient.EmployeeGetAll().OrderBy(x => x.EmpName).Where(x => (DesigList.Contains(x.DesignationId)) && (x.IsActive = true));
                }
                else
                {
                    otherObjects = MasterRestClient.EmployeeGetAll(hierarchyState).OrderBy(x => x.EmpName).Where(x => (DesigList.Contains(x.DesignationId)) && (BranchList.Contains(x.BranchId)) && (x.IsActive == true));

                }
                result = Json(otherObjects);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.MaxJsonLength = int.MaxValue;
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
        public JsonResult GetEnrollEmpByDept(string departmentid, string branchid)
        {
            JsonResult result;
            try
            {
                var otherObjects = MasterRestClient.GetEnrollEmpByDept(departmentid, branchid);
                result = Json(otherObjects);
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
        public JsonResult GetEmpByEmpBluckchanges(EmpFilter filter)
        {
            JsonResult result;
            try
            {
                string branchStr = filter.BranchStr;
                string deptStr = filter.DeptStr;
                string desgStr = filter.DesgStr;

                int hierarchyState = Session["Ishierchy"] != null ? Convert.ToInt32(Session["Ishierchy"]) : 0;

                var branchIds = branchStr.Split(',').Select(int.Parse).ToList();
                var deptIds = !string.IsNullOrEmpty(deptStr) ? deptStr.Split(',').Select(int.Parse).ToList() : new List<int>();
                var desgIds = !string.IsNullOrEmpty(desgStr) ? desgStr.Split(',').Select(int.Parse).ToList() : new List<int>();
                var allEmployees = MasterRestClient.EmployeeGetAlls(hierarchyState).OrderBy(e => e.EmpName);
                var filteredEmployees = allEmployees.Where(emp =>
                    emp.IsActive == true &&
                    branchIds.Contains(emp.BranchId) &&
                    (deptIds.Count == 0 || deptIds.Contains(emp.DepartmentId)) &&
                    (desgIds.Count == 0 || desgIds.Contains(emp.DesignationId))
                );

                result = Json(filteredEmployees, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = int.MaxValue;
                return result;
            }
            catch
            {
                result = Json(new { error = "An error occurred while retrieving employees." }, JsonRequestBehavior.AllowGet);
                return result;
            }
        }

        public List<SelectListItem> FillDevice()
        {
            Devices ds = new Devices();
            ds.DeviceList = MasterRestClient.DeviceGetAll();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var i in ds.DeviceList)
            {
                list.Add(new SelectListItem() { Text = i.DeviceName, Value = Convert.ToString(i.DeviceCode) });
            }
            return list;
        }
        public List<SelectListItem> FillIP()
        {
            Devices ds = new Devices();
            ds.DeviceList = MasterRestClient.DeviceGetAll();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var i in ds.DeviceList)
            {
                list.Add(new SelectListItem() { Text = i.DeviceIP, Value = Convert.ToString(i.DeviceIP) });
            }
            return list;
        }

        public List<SelectListItem> FillShift()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            Companys com = new Companys();
            IEnumerable<Shifts> shiftList;
            list.Add(new SelectListItem() { Text = "Default", Value = "-3" });
            list.Add(new SelectListItem() { Text = "Holidays Off", Value = "-2" });
            list.Add(new SelectListItem() { Text = "Weekly Off", Value = "-1" });
            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
            {
                shiftList = MasterRestClient.ShiftGetAll();
                foreach (var i in shiftList)
                {
                    list.Add(new SelectListItem() { Text = i.ShiftName, Value = Convert.ToString(i.ShiftId) });
                }
            }
            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                if (Session["RoleId"].ToString() == "6806")
                {
                    shiftList = MasterRestClient.ShiftGetAll().Where(x =>
                        (x.GraceBefore == cmpid && (x.GraceAfter == BranchID || x.GraceAfter == 0)) ||
                        (x.GraceBefore == 0));
                }
                else
                {
                    shiftList = MasterRestClient.ShiftGetAll()
                        .Where(x => x.GraceBefore == cmpid || x.GraceBefore == 0);
                }
                foreach (var i in shiftList.OrderBy(x => x.ShiftName))
                {
                    list.Add(new SelectListItem() { Text = i.ShiftName, Value = Convert.ToString(i.ShiftId) });
                }
            }
            return list;
        }


        [HttpPost]
        public JsonResult FillDeviceForEmp()
        {
            Devices d = new Devices();
            List<Devices> dList = new List<Devices>();
            dList = MasterRestClient.DeviceGetAll().ToList();
            return Json(dList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Update Attendance

        [HttpPost]
        public JsonResult UpdateAttendance(UpdateAttendance obj)
        {
            JsonResult result;
            if (obj.empids != "")
            {
                obj.empids = obj.empids.Replace("[", "").Replace("]", "").Replace("\"", "");
            }
            var Isdateformat = Session["IsDateFormat"].ToString();

            if (Isdateformat == "dd-M-yyyy" || Isdateformat == "M-dd-yyyy" || Isdateformat == "yyyy-M-dd")
            {
                obj.fromdate = Convert.ToDateTime(obj.fromdate).ToString("yyyy-MM-dd") + " 00:00:00";
                obj.todate = Convert.ToDateTime(obj.todate).ToString("yyyy-MM-dd") + " 00:00:00";
            }
            else
            {
                obj.fromdate = Convert.ToDateTime(obj.fromdate).ToString("yyyy-MM-dd") + " 00:00:00";
                obj.todate = Convert.ToDateTime(obj.todate).ToString("yyyy-MM-dd") + " 00:00:00";
            }
            string res = string.Empty;
            //obj.fromdate = obj.fromdate + " 00:00:00";
            //obj.todate = obj.todate + " 00:00:00";
            res = RestClient.UpdateAttendance(obj);
            result = Json(new { ad = res.ToLower() });
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        [HttpPost]
        public JsonResult AttendanceProcess(UpdateAttendance obj)
        {
            JsonResult result;
            if (obj.empids != "")
            {
                obj.empids = obj.empids.Replace("[", "").Replace("]", "").Replace("\"", "");
            }
            string res = string.Empty;
            var Isdateformat = Session["IsDateFormat"].ToString();

            if (Isdateformat == "dd-M-yyyy" || Isdateformat == "M-dd-yyyy" || Isdateformat == "yyyy-M-dd")
            {
                obj.fromdate = Convert.ToDateTime(obj.fromdate).ToString("yyyy-MM-dd") + " 00:00:00";
                obj.todate = Convert.ToDateTime(obj.todate).ToString("yyyy-MM-dd") + " 00:00:00";
            }
            else
            {
                obj.fromdate = Convert.ToDateTime(obj.fromdate).ToString("yyyy-MM-dd") + " 00:00:00";
                obj.todate = Convert.ToDateTime(obj.todate).ToString("yyyy-MM-dd") + " 00:00:00";
            }
            res = RestClient.AttendanceProcess(obj);
            result = Json(new { ad = res.ToLower() });
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        #endregion

        #region LeaveAutomaticSanction
        public ActionResult LeaveAutomaticSanction()
        {
            ViewBag.LeaveTypeList = FillLeaveType();
            if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                ViewBag.Companyslist = MasterRestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid).ToList();
            }
            else if (Convert.ToInt32(Session["RoleId"].ToString()) > 1 && Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                ViewBag.Companyslist = MasterRestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid).ToList();
            }
            else
            {
                ViewBag.Companyslist = MasterRestClient.CompanyGetAll();
            }
            //ViewBag.DesignationList = FillDesignation();
            //ViewBag.DesignationList = "";

            return View();
        }
        public List<SelectListItem> FillDesignation()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            Designations ds = new Designations();
            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
            {
                ds.Designationlist = MasterRestClient.DesignationsGetAll();
            }
            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                if (Session["RoleId"].ToString() == "6806")
                {
                    ds.Designationlist = MasterRestClient.DesignationsGetAll().Where(x => (x.CompanyID == cmpid && (x.BranchID == BranchID || x.BranchID == 0)) || (x.CompanyID == 0)).ToList();
                }
                else
                {
                    ds.Designationlist = MasterRestClient.DesignationsGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0).ToList();
                }
            }
            foreach (var i in ds.Designationlist)
            {
                list.Add(new SelectListItem() { Text = i.DesignationName, Value = Convert.ToString(i.DesignationId) });
            }
            return list;
        }
        [HttpPost]
        public JsonResult LeaveSanction(string Employeeid, string leavetype, string companyid)
        {
            JsonResult result;
            try
            {
                LeaveSanction obj = new LeaveSanction();
                obj.Companyid = companyid;
                obj.Employeeid = Employeeid;
                obj.leavetype = leavetype;
                var LeaveSanction = RestClient.InsertLeaveSanction(obj);

                var resp = JsonConvert.DeserializeObject<MRespo>(LeaveSanction);
                if (resp.MegSts.ToLower() == "ok")
                {
                    TempData["Msg"] = resp.Meg;
                }
                else
                {
                    TempData["error"] = resp.Meg;
                }
                result = Json(resp);
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
        public ActionResult LeaveSanctionList(string Companyid, string Branchid, string Departmentid, string Employeeid)
        {
            JsonResult result;
            try
            {
                LeaveSanctionList obj = new LeaveSanctionList();
                obj.Companyid = Companyid;
                obj.Branchid = Branchid;
                obj.Departmentid = Departmentid;
                obj.Employeeid = Employeeid;
                var LeaveSanctionList = RestClient.LeaveSanctionListGetAll(obj);

                var jsonSerialiser = new JavaScriptSerializer();
                result = Json(LeaveSanctionList);
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

        [HttpGet]
        public ActionResult ShiftAllocationApprove()
        {
            return View();
        }

    }
}