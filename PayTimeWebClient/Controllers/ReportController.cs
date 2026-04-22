using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.XtraReports.UI;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using PayTimeWebClient.Models.ReportsModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using System.Xml.Linq;


namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class ReportController : Controller
    {
        #region Declaration

        MasterDataRestClient RestClient = new MasterDataRestClient();
        ReportDataRestClient RestClientReport = new ReportDataRestClient();
        reqDailyInOutReport reqmodel = new reqDailyInOutReport();
        reqDailyInOutReport csreq = new reqDailyInOutReport();

        #endregion

        #region Daily Reports

        [HttpPost]
        public ActionResult PayTimeWebReports(reqDailyInOutReport reqrep)
        {
            try
            {
                ViewBag.selectedreport = reqrep.ReportName;
                ViewBag.selectedreportid = reqrep.RptID;
                ViewBag.crptid = reqrep.crptid;
                ViewBag.crptheader = reqrep.ReportName;
                ViewBag.RptID = 0;
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                if (Session["RoleId"].ToString() == "6805" && Session["IsSchool"].ToString() == "1")
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                }
                else
                {
                    if (Session["RoleId"].ToString() == "6810")
                    {
                        ViewBag.Companyslist = RestClient.CompanyGetAll();
                    }
                    else if (Session["RoleId"].ToString() != "1")
                    {
                        ViewBag.Companyslist = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                    }
                    
                    else
                    {
                        ViewBag.Companyslist = RestClient.CompanyGetAll();
                    }
                }
                try
                {
                    Session["FilterData"] = reqrep != null ? reqrep : null;
                    Session["NFilterData"] = null;
                }
                catch (Exception Ex)
                {
                    Session["FilterData"] = null;
                    ViewBag.error = Ex.Message;
                    return RedirectToAction("ErrorPage", "PayTime");
                }
            }
            catch (Exception Ex)
            {
                Session["FilterData"] = null;
                ViewBag.error = Ex.Message;
                return RedirectToAction("ErrorPage", "PayTime");

            }

            return View("Gridview");

        }
        [HttpGet]
        public ActionResult PayTimeWebReports()
        {
            if (Session["NFilterData"] != null)
            {
                csreq = (reqDailyInOutReport)Session["NFilterData"];
                ViewBag.selectedreport = csreq.ReportName;
                ViewBag.selectedreportid = csreq.RptID;
                ViewBag.crptid = csreq.crptid;
                ViewBag.crptheader = csreq.ReportName;
                ViewBag.RptID = 0;

                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                if (Session["RoleId"].ToString() == "6805" && Session["IsSchool"].ToString() == "1")
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                }
                else
                {
                    if (Session["RoleId"].ToString() != "1")
                    {
                        ViewBag.Companyslist = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                    }
                    else
                    {
                        ViewBag.Companyslist = RestClient.CompanyGetAll();
                    }
                }

                Session["FilterData"] = Session["NFilterData"];
                return View("Gridview");
            }
            else
            {
                ViewBag.error = "Opps! an error has occurred. something went wrong. please try again later";
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [ValidateInput(false)]
        public ActionResult GridViewPartial()
        {

            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];

                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;

                    return PartialView("_GridViewPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_GridViewPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [ValidateInput(false)]
        public ActionResult DailyinoutDevicePartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_DailyInOutDeviceName", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_DailyInOutDeviceName", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [ValidateInput(false)]
        public ActionResult FirstINLastOUTPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_FirstINLastOUTPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_FirstINLastOUTPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }



        }
        [ValidateInput(false)]
        public ActionResult DailyInPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_DailyInPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_DailyInPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }



        }
        [ValidateInput(false)]
        public ActionResult ErrorCasePartial()
        {

            try
            {

                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_ErrorCasePartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_ErrorCasePartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [ValidateInput(false)]
        public ActionResult AbsentPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_AbsentPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_AbsentPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }
        [ValidateInput(false)]
        public ActionResult LateINPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_LateINPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_LateINPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [ValidateInput(false)]
        public ActionResult EarlyINPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_EarlyINPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_EarlyINPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [ValidateInput(false)]
        public ActionResult EarlyOutPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_EarlyOutPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_EarlyOutPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [ValidateInput(false)]
        public ActionResult LateOutPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_LateOutPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_LateOutPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }
        [ValidateInput(false)]
        public ActionResult OTPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {

                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_OTPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_OTPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }
        [ValidateInput(false)]
        public ActionResult LateArrivalPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_LateArrivalPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_LateArrivalPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }
        [ValidateInput(false)]
        public ActionResult ContinuousEarlyDeparturePartial()
        {

            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_ContinuousEarlyDeparturePartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_ContinuousEarlyDeparturePartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [ValidateInput(false)]
        public ActionResult ContinuousAbsenteeismPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_ContinuousAbsenteeismPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_ContinuousAbsenteeismPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [ValidateInput(false)]
        public ActionResult MachineRawTransactionPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_MachineRawTransactionPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_MachineRawTransactionPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }
        [ValidateInput(false)]
        public ActionResult ManualPunchPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_ManualPunchPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_ManualPunchPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [ValidateInput(false)]
        public ActionResult LeaveBalanceReportPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<LeaveBalanceReport> model = RestClientReport.GetLeaveBalanceReport(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<LeaveBalanceReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_LeaveBalanceReportPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_LeaveBalanceReportPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }
        public ActionResult ExportTo(string OutputFormat)
        {
            try
            {
                var model = Session["data"];
                //reqDailyInOutReport csreq = new reqDailyInOutReport();
                csreq = (reqDailyInOutReport)Session["NFilterData"];
                GetReportDetails(csreq);
                if (csreq.RptID == 27)
                {
                    model = Session["Datatable"];
                    OutputFormat = "XLSX";
                }
                if (csreq.RptID == 28)
                {
                    model = Session["Datatable"];
                    OutputFormat = "XLSX";
                }
                if (csreq.RptID == 29)
                {
                    model = Session["Datatable"];
                    OutputFormat = "XLSX";
                }
                if (csreq.RptID == 32)
                {
                    model = Session["Datatable"];
                    OutputFormat = "XLSX";
                }
                if (csreq.RptID == 33)
                {
                    model = Session["Datatable"];
                    OutputFormat = "XLSX";
                }
                if (csreq.RptID == 20)
                {
                    IEnumerable<Workingduration> modeldt = (IEnumerable<Workingduration>)Session["data"];
                    foreach (Workingduration md in modeldt)
                    {
                        ReportStartEndday.Fday = md.MonthRepostartday;
                        ReportStartEndday.Lday = md.LastdayMonth;
                        break;
                    }

                }
                if (csreq.RptID == 19)
                {
                    IEnumerable<MonthlyMuster> modeldt = (IEnumerable<MonthlyMuster>)Session["data"];
                    foreach (MonthlyMuster md in modeldt)
                    {
                        ReportStartEndday.Fday = md.MonthRepostartday;
                        ReportStartEndday.Lday = md.LastdayMonth;
                        break;
                    }
                }

                ViewBag.selectedreport = csreq.ReportName;
                ViewBag.selectedreportid = csreq.RptID;
                ViewBag.RptID = 0;
                switch (OutputFormat.ToUpper())
                {
                    case "CSV":
                        return GridViewExtension.ExportToCsv(GridViewHelper.ExportReportsettings, model);
                    case "PDF":
                        return GridViewExtension.ExportToPdf(GridViewHelper.ExportReportsettings, model);
                    case "RTF":
                        return GridViewExtension.ExportToRtf(GridViewHelper.ExportReportsettings, model);
                    case "XLS":
                        return GridViewExtension.ExportToXls(GridViewHelper.ExportReportsettings, model);
                    case "XLSX":
                        return GridViewExtension.ExportToXlsx(GridViewHelper.ExportReportsettings, model);
                    default:
                        return View("Gridview");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }




        }
        [ValidateInput(false)]
        public ActionResult SchoolReportsPartial()
        {

            try
            {

                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    SetReportname(csreq.RptID);
                    DataTable model = RestClientReport.GetSchoolReportsData(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["Datatable"] = null;
                    Session["Datatable"] = model;
                    return PartialView("_SchoolReportsPartial", model);
                }
                else
                {
                    csreq = (reqDailyInOutReport)Session["NFilterData"];
                    SetReportname(csreq.RptID);
                    var model = Session["Datatable"];
                    return PartialView("_SchoolReportsPartial", model);
                }

            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        public void SetReportname(int rptid)
        {
            ViewBag.Rptid = rptid;
            if (rptid == 1)
            {
                ViewBag.Gridheader = "Daily Attendance Details";
            }
            else if (rptid == 2)
            {
                ViewBag.Gridheader = "Attendance Percentage";
            }
            else if (rptid == 3)
            {
                ViewBag.Gridheader = "Punch Details";
            }
            else if (rptid == 4)
            {
                ViewBag.Gridheader = "Daily Attendance Summary";
            }
            else if (rptid == 5)
            {
                ViewBag.Gridheader = "Absent Student List";
            }
            else if (rptid == 6)
            {
                ViewBag.Gridheader = "Monthly Attendance";
            }
            else if (rptid == 25)
            {
                //ViewBag.Gridheader = "Attendance Without Mask";
                ViewBag.Gridheader = "Mask Report";
            }
            else if (rptid == 26)
            {
                ViewBag.Gridheader = "Temperature (&#8451;) Report";
            }
            else if (rptid == 27)
            {
                ViewBag.Gridheader = "Failed Status Report";
            }
        }
        [ValidateInput(false)]
        public ActionResult DailyFlexiInOutReport()
        {

            try
            {

                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyInOutReport> model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;

                    //csreq = (reqDailyInOutReport)Session["FilterData"];
                    //DataTable model = RestClientReport.Getcustomreportdata(csreq);                    
                    //Session["NFilterData"] = Session["FilterData"];
                    //Session["FilterData"] = null;
                    //Session["data"] = null;                    
                    //Session["Datatable"] = model;
                    //Session["data"] = model;

                    return PartialView("_DailyFlexiInOutReport", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_DailyFlexiInOutReport", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [ValidateInput(false)]
        public ActionResult bionicsevenreport()
        {
            try
            {

                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    DataTable model = RestClientReport.GetbionicsevenData(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["Datatable"] = null;
                    Session["Datatable"] = model;
                    SetReportname(csreq.RptID);
                    return PartialView("_Bionicsevenpartial", model);

                }
                else
                {
                    csreq = (reqDailyInOutReport)Session["NFilterData"];
                    SetReportname(csreq.RptID);
                    var model = Session["Datatable"];
                    return PartialView("_Bionicsevenpartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [ValidateInput(false)]
        public ActionResult FaceWebPunchReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<GetFacePunchdata> model = RestClientReport.ViewReportQueryGetAllface(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<GetFacePunchdata>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_FacePunchPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_FacePunchPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [ValidateInput(false)]
        public ActionResult FaceWebPunchFiloReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<GetFaceFilodetaildata> model = RestClientReport.ViewReportQueryGetFiloface(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<GetFaceFilodetaildata>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_FacePunchIsFilo", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_FacePunchIsFilo", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [ValidateInput(false)]
        public ActionResult MonthlyFlexiInOutReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<MonthlyFlexiInOutReport> model = RestClientReport.MonthlyFlexiInOutReport(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<MonthlyFlexiInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("MonthlyReport/_MonthlyFlexiInOutReport", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("MonthlyReport/_MonthlyFlexiInOutReport", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [ValidateInput(false)]
        public ActionResult WeeklyFlexiInOutReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<WeeklyFlexiInOutReport> model = RestClientReport.WeeklyFlexiInOutReport(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<WeeklyFlexiInOutReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("MonthlyReport/_WeeklyFlexiInOutReport", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("MonthlyReport/_WeeklyFlexiInOutReport", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [ValidateInput(false)]
        public ActionResult Punchwisereport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<GetPunchwisereport> model = RestClientReport.ViewReportGetPunchwisereport(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<GetPunchwisereport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_Punchwisereport", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_Punchwisereport", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [ValidateInput(false)]
        public ActionResult Misspunchreport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<GetMissPunchReport> model = RestClientReport.ViewReportmisspunchreport(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<GetMissPunchReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_Misspunchreport", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_Misspunchreport", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [ValidateInput(false)]
        public ActionResult JoinrejoinReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<GetjoinrejoinReport> model = RestClientReport.ViewReportjoinrejoinreport(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<GetjoinrejoinReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_JoinrejoinReport", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_JoinrejoinReport", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [ValidateInput(false)]
        public ActionResult LeaveBalanceStatusReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<LeaveBalanceStatusReport> model = RestClientReport.LeaveBalanceStatusReport(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<LeaveBalanceStatusReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_LeaveBalanceStatusReportPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_LeaveBalanceStatusReportPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }


        [ValidateInput(false)]
        public ActionResult DailyOperationsSummaryReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailySummaryReport> model = RestClientReport.DailyOperationsSummaryReport(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailySummaryReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_DailyOperationsSummaryReportPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_DailyOperationsSummaryReportPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [ValidateInput(false)]
        public ActionResult InstallationStatusReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<InstallationStatusReport> model = RestClientReport.InstallationStatusReport(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<InstallationStatusReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_InstallationReportPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_InstallationReportPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [ValidateInput(false)]
        public ActionResult FaceWebPunchDetailsReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<FaceWebPunchDetailReport> model = RestClientReport.FaceWebPunchDetailsReport(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<FaceWebPunchDetailReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_FaceWebPunchDeatilReportPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_FaceWebPunchDeatilReportPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [ValidateInput(false)]
        public ActionResult DailyWorkingHoursReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyWorkingHoursReport> model = RestClientReport.DailyWorkingHoursReport(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyWorkingHoursReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_DailyWorkingHoursReportPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_DailyWorkingHoursReportPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [ValidateInput(false)]
        public ActionResult DailyWorkingHoursINOUTReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<DailyWorkingHoursINOUTReport> model = RestClientReport.DailyWorkingHoursINOUTReport(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<DailyWorkingHoursINOUTReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_DailyWorkingHoursINOUTReportPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_DailyWorkingHoursINOUTReportPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [ValidateInput(false)]
        public ActionResult AbsentReportDetails()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<AbsentReport> model = RestClientReport.AbsentReportDetails(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<AbsentReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_AbsentReportPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_AbsentReportPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        #endregion

        #region Monthly Reports
        public ActionResult MonthlyMusterReport()
        {
            try
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                }
                else
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll();
                }

                Session["FilterData"] = null;

                return View("MonthlyReport/MonthlyReport");
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }
        [HttpPost, ValidateInput(false)]
        public ActionResult MonthlyReports(reqMonthlyReport reqrep)
        {
            try
            {
                Session["FilterData"] = reqrep != null ? reqrep : null;
            }
            catch (Exception Ex)
            {
                Session["FilterData"] = null;
                ViewBag.error = Ex.Message;
                return RedirectToAction("ErrorPage", "PayTime");

            }
            return View("MonthlyReport/GridviewMonthly");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MonthlyReportsExcel()
        {
            try
            {
                // Get the filter data from the session
                reqDailyInOutReport csreq = (reqDailyInOutReport)Session["NFilterData"];

                // Call the method to get Excel bytes from the service
                byte[] excelBytes = RestClientReport.ViewMonthlyMusterQueryGetAllExcelData(csreq);

                // Check if the byte array is not null or empty
                if (excelBytes == null || excelBytes.Length == 0)
                {
                    // Return a meaningful error response if no data is returned
                    return Json(new { success = false, message = "No data available for the report." });
                }

                // Return the Excel file as a download
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MonthlyMusterReport.xlsx");
            }
            catch (Exception ex)
            {
                // Log the error (optional) and return an error response
                ViewBag.error = ex.Message;
                return Json(new { success = false, message = "An error occurred while generating the report." });
            }
        }

        [ValidateInput(false)]
        public ActionResult MonthlyMusterViewPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<MonthlyMuster> model = RestClientReport.ViewMonthlyMusterQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<MonthlyMuster>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    foreach (MonthlyMuster b in model)
                    {
                        ViewBag.startday = b.MonthRepostartday;
                        ViewBag.lastday = b.LastdayMonth;

                        ViewBag.Leavetype = b.Leavetype;
                        ViewBag.Leavecnt = b.Leavecnt;
                        ViewBag.Levlst = b.Levlst;
                        break;
                    }
                    return PartialView("MonthlyReport/_MonthlyMusterViewPartial", model);
                }
                else
                {
                    csreq = (reqDailyInOutReport)Session["NFilterData"];
                    //IEnumerable<MonthlyMuster> model = (IEnumerable<MonthlyMuster>)Session["data"];
                    IEnumerable<MonthlyMuster> model = RestClientReport.ViewMonthlyMusterQueryGetAll(csreq);
                    foreach (MonthlyMuster b in model)
                    {
                        ViewBag.startday = b.MonthRepostartday;
                        ViewBag.lastday = b.LastdayMonth;

                        ViewBag.Leavetype = b.Leavetype;
                        ViewBag.Leavecnt = b.Leavecnt;
                        ViewBag.Levlst = b.Levlst;
                        //ExportTo("XLSX");
                        break;
                    }
                    return PartialView("MonthlyReport/_MonthlyMusterViewPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }



        }

        [ValidateInput(false)]
        public ActionResult MonthlyMusterCustom()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<MonthlyMuster> model = RestClientReport.ViewMonthlyMusterQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<MonthlyMuster>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    foreach (MonthlyMuster b in model)
                    {
                        ViewBag.startday = b.MonthRepostartday;
                        ViewBag.lastday = b.LastdayMonth;
                        break;
                    }
                    return PartialView("MonthlyReport/_MonthlyMusterCustom", model);
                }
                else
                {
                    IEnumerable<MonthlyMuster> model = (IEnumerable<MonthlyMuster>)Session["data"];
                    foreach (MonthlyMuster b in model)
                    {
                        ViewBag.startday = b.MonthRepostartday;
                        ViewBag.lastday = b.LastdayMonth;
                        break;
                    }
                    return PartialView("MonthlyReport/_MonthlyMusterCustom", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }



        }
        [ValidateInput(false)]
        public ActionResult MonthlyWorkingDurationReportPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {

                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<Workingduration> model = RestClientReport.MonthlyWorkingDurationReport(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<Workingduration>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    foreach (Workingduration b in model)
                    {
                        ViewBag.startday = b.MonthRepostartday;
                        ViewBag.lastday = b.LastdayMonth;
                        break;
                    }
                    return PartialView("MonthlyReport/_MonthlyWorkingDurationReportPartial", model);
                }
                else
                {

                    IEnumerable<Workingduration> model = (IEnumerable<Workingduration>)Session["data"];
                    foreach (Workingduration b in model)
                    {
                        ViewBag.startday = b.MonthRepostartday;
                        ViewBag.lastday = b.LastdayMonth;
                        break;
                    }
                    return PartialView("MonthlyReport/_MonthlyWorkingDurationReportPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }
        [ValidateInput(false)]
        public ActionResult MonthlyWorkingDurationCustom()
        {
            try
            {
                if (Session["FilterData"] != null)
                {

                    csreq = (reqDailyInOutReport)Session["FilterData"];//Status=="First IN" or Status=="Last Out"(new System.Collections.Generic.Mscorlib_CollectionDebugView<PayTimeWebClient.Models.ReportsModel.Workingduration>(model as System.Collections.Generic.List<PayTimeWebClient.Models.ReportsModel.Workingduration>)).Items[0].Status
                    IEnumerable<Workingduration> model = RestClientReport.MonthlyWorkingDurationCustom(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<Workingduration>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    foreach (Workingduration b in model)
                    {
                        ViewBag.startday = b.MonthRepostartday;
                        ViewBag.lastday = b.LastdayMonth;
                        ViewBag.ReportHead = Convert.ToDateTime(b.PFromDate).ToString("MMM-yyyy");
                        break;
                    }
                    return PartialView("MonthlyReport/_MonthlyWorkingDurationCustom", model);
                }
                else
                {

                    IEnumerable<Workingduration> model = (IEnumerable<Workingduration>)Session["data"];
                    foreach (Workingduration b in model)
                    {
                        ViewBag.startday = b.MonthRepostartday;
                        ViewBag.lastday = b.LastdayMonth;
                        ViewBag.ReportHead = Convert.ToDateTime(b.PFromDate).ToString("MMM-yyyy");
                        break;
                    }
                    return PartialView("MonthlyReport/_MonthlyWorkingDurationCustom", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }


        [ValidateInput(false)]
        public ActionResult MonthlyKMReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<MonthlyKmReport> model = RestClientReport.ViewMonthlyKmReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<MonthlyKmReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    foreach (MonthlyKmReport b in model)
                    {
                        ViewBag.startday = b.MonthRepostartday;
                        ViewBag.lastday = b.LastdayMonth;
                        break;
                    }
                    return PartialView("MonthlyReport/_MonthlyKMViewPartialReport", model);
                }
                else
                {
                    csreq = (reqDailyInOutReport)Session["NFilterData"];
                    IEnumerable<MonthlyKmReport> model = RestClientReport.ViewMonthlyKmReportQueryGetAll(csreq);
                    foreach (MonthlyKmReport b in model)
                    {
                        ViewBag.startday = b.MonthRepostartday;
                        ViewBag.lastday = b.LastdayMonth;
                        break;
                    }
                    return PartialView("MonthlyReport/_MonthlyKMViewPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }



        }
        public void GetReportDetails(reqDailyInOutReport csreq)
        {

            if (csreq.ReportName == "DailyInOutReport")
            {
                ReportDetails.Rptid = 1;
                ReportDetails.RptName = "DailyInOutReport";
                ReportDetails.RptTitle = "Daily In-Out Report";

            }
            if (csreq.ReportName == "DailyInOutDeviceNameReport")
            {
                ReportDetails.Rptid = 2;
                ReportDetails.RptName = "DailyInOutDeviceNameReport";
                ReportDetails.RptTitle = "Daily In-Out With Device Name Report";
            }
            if (csreq.ReportName == "FirstINLastOUTReport")
            {
                ReportDetails.Rptid = 3;
                ReportDetails.RptName = "FirstINLastOUTReport";
                ReportDetails.RptTitle = "First IN Last OUT Report";
            }
            if (csreq.ReportName == "DailyInReport")
            {
                ReportDetails.Rptid = 4;
                ReportDetails.RptName = "DailyInReport";
                ReportDetails.RptTitle = "Daily In Report";
            }
            if (csreq.ReportName == "ErrorCaseReport")
            {
                ReportDetails.Rptid = 5;
                ReportDetails.RptName = "ErrorCaseReport";
                ReportDetails.RptTitle = "Error Case Report";
            }
            if (csreq.ReportName == "AbsentReport")
            {
                ReportDetails.Rptid = 6;
                ReportDetails.RptName = "AbsentReport";
                ReportDetails.RptTitle = "Absent Report";
            }

            if (csreq.ReportName == "LateINReport")
            {

                ReportDetails.Rptid = 7;
                ReportDetails.RptName = "LateINReport";
                ReportDetails.RptTitle = "Late IN Report";
            }
            if (csreq.ReportName == "EarlyINReport")
            {
                ReportDetails.Rptid = 8;
                ReportDetails.RptName = "EarlyINReport";
                ReportDetails.RptTitle = "Early IN Report";
            }
            if (csreq.ReportName == "EarlyOutReport")
            {
                ReportDetails.Rptid = 9;
                ReportDetails.RptName = "EarlyOutReport";
                ReportDetails.RptTitle = "Early Out Report";
            }
            if (csreq.ReportName == "LateOutReport")
            {
                ReportDetails.Rptid = 10;
                ReportDetails.RptName = "LateOutReport";
                ReportDetails.RptTitle = "Late Out Report";
            }
            if (csreq.ReportName == "OTReport")
            {
                ReportDetails.Rptid = 11;
                ReportDetails.RptName = "OTReport";
                ReportDetails.RptTitle = "OT Report";
            }
            if (csreq.ReportName == "LateArrivalReport")
            {
                ReportDetails.Rptid = 12;
                ReportDetails.RptName = "LateArrivalReport";
                ReportDetails.RptTitle = "CONTINUOUS LATE ARRIVAL REPORT";
            }
            if (csreq.ReportName == "ContinuousEarlyDepartureReport")
            {
                ReportDetails.Rptid = 13;
                ReportDetails.RptName = "ContinuousEarlyDepartureReport";
                ReportDetails.RptTitle = "CONTINUOUS EARLY DEPARTURE REPORT";
            }
            if (csreq.ReportName == "ContinuousAbsenteeismReport")
            {
                ReportDetails.Rptid = 14;
                ReportDetails.RptName = "ContinuousAbsenteeismReport";
                ReportDetails.RptTitle = "CONTINUOUS ABSENTEEISM REPORT";
            }
            if (csreq.ReportName == "MachineRawTransactionReport")
            {
                ReportDetails.Rptid = 15;
                ReportDetails.RptName = "MachineRawTransactionReport";
                ReportDetails.RptTitle = "MACHINE RAW TRANSACTION REPORT";
            }
            if (csreq.ReportName == "ManualPunchReport")
            {
                ReportDetails.Rptid = 16;
                ReportDetails.RptName = "ManualPunchReport";
                ReportDetails.RptTitle = "Manual Punch Report";
            }
            if (csreq.ReportName == "DepartmentSummaryReport")
            {
                ReportDetails.Rptid = 17;
                ReportDetails.RptName = "DepartmentSummaryReport";
                ReportDetails.RptTitle = "Department Summary Report";
            }
            if (csreq.ReportName == "EarlyInSummaryReport")
            {
                ReportDetails.Rptid = 18;
                ReportDetails.RptName = "EarlyInSummaryReport";
                ReportDetails.RptTitle = "Early-In Summary Report";
            }
            if (csreq.ReportName == "MonthlyMusterReport")
            {
                ReportDetails.Rptid = 19;
                ReportDetails.RptName = "MonthlyMusterReport";
                ReportDetails.RptTitle = "Monthly Muster Report";

            }
            if (csreq.ReportName == "MonthlyWorkingDurationReport")
            {
                ReportDetails.Rptid = 20;
                ReportDetails.RptName = "MonthlyWorkingDurationReport";
                ReportDetails.RptTitle = "Monthly Working Duration Report";
            }
            if (csreq.ReportName == "Dailyhightempraturedetectedreport")
            {
                ReportDetails.Rptid = 27;
                ReportDetails.RptName = "FailedStatusReport";
                ReportDetails.RptTitle = "Failed status Report";
            }
            if (csreq.ReportName == "FaceWebPunchReport")
            {
                ReportDetails.Rptid = 28;
                ReportDetails.RptName = "FaceWebPunchReport";
                ReportDetails.RptTitle = "Face / Web Punch Report";
            }
            if (csreq.ReportName == "LeaveBalanceSummaryReport")
            {
                ReportDetails.Rptid = 29;
                ReportDetails.RptName = "LeaveBalanceSummaryReport";
                ReportDetails.RptTitle = "Leave Balance Summary Report";
            }
            if (csreq.ReportName == "FaceWebPunchFiloReport")
            {
                ReportDetails.Rptid = 32;
                ReportDetails.RptName = "FaceWebPunchFiloReport";
                ReportDetails.RptTitle = "Face WebPunch Details";
            }

            if (csreq.ReportName == "Punchwisereport")
            {
                ReportDetails.Rptid = 33;
                ReportDetails.RptName = "Punchwisereport";
                ReportDetails.RptTitle = "Punch wise report";
            }

            ReportDetails.Fdate = csreq.FromDate;
            ReportDetails.Tdate = csreq.Todate;
            ReportDetails.bname = csreq.BranchName;
            ReportDetails.cname = csreq.CompanyName;
        }

        public ActionResult AnalyticsDashboard()
        {
            try
            {
                if (Session["RoleId"].ToString() == "6810")
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll().OrderBy(x => x.CompanyName);
                }
                else if (Session["RoleId"].ToString() != "1")
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    ViewBag.Companyslist = RestClient.CompanyGetAll().OrderBy(x => x.CompanyName).Where(x => x.CompanyID == cmpid);
                }
                else
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll().OrderBy(x => x.CompanyName);
                }
                ViewBag.ShowReportButton = false;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        public ActionResult AnalyticsDashboardEMS()
        {
            try
            {
                if (Session["RoleId"].ToString() == "6810" || Session["RoleId"].ToString() == "6000")
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll().OrderBy(x => x.CompanyName);
                }
                else if (Session["RoleId"].ToString() != "1")
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    ViewBag.Companyslist = RestClient.CompanyGetAll().OrderBy(x => x.CompanyName).Where(x => x.CompanyID == cmpid);
                }
                else
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll().OrderBy(x => x.CompanyName);
                }
                ViewBag.ShowReportButton = false;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        public ActionResult PMSDashboard()
        {
            try
            {
                if (Session["RoleId"].ToString() == "6810" || Session["RoleId"].ToString() == "6000")
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll().OrderBy(x => x.CompanyName);
                }
                else if (Session["RoleId"].ToString() != "1")
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    ViewBag.Companyslist = RestClient.CompanyGetAll().OrderBy(x => x.CompanyName).Where(x => x.CompanyID == cmpid);
                }
                else
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll().OrderBy(x => x.CompanyName);
                }
                ViewBag.ShowReportButton = false;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        #endregion

        #region Summary Report
        public ActionResult DepartmentSummaryReport()
        {
            try
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                }
                else
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll();
                }

                return View("LocalReportExample", reqmodel);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [ValidateInput(false)]
        public ActionResult DepartmentSummaryPivotePartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<Summaryreport> model = RestClientReport.ViewReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<Summaryreport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;

                    return PartialView("_DepartmentSummaryPivotePartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_DepartmentSummaryPivotePartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }
        [ValidateInput(false)]
        public ActionResult EarlyInSummaryReportPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<Summaryreport> model = RestClientReport.ViewReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<Summaryreport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("_EarlyInSummaryReportPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_EarlyInSummaryReportPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [ValidateInput(false)]
        public ActionResult DepartmentSummaryChartPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    var model = RestClientReport.ViewReportQueryGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["data"] = model;
                    return PartialView("_DepartmentSummaryChartPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_DepartmentSummaryChartPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [ValidateInput(false)]
        public ActionResult LeaveBalanceSummaryReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    var model = RestClientReport.ViewReportQueryGetLeaveBAlanceAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<LeaveBalanceSummaryReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = null;
                    Session["data"] = model;

                    return PartialView("_LeaveBalanceSummaryPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_LeaveBalanceSummaryPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        #endregion

        #region Custom Report Template
        [ValidateInput(false)]
        public ActionResult CustomeReportPartial()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    DataTable model = RestClientReport.Getcustomreportdata(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["data"] = model;
                    return PartialView("_CustomReportPartial", model);
                }
                else
                {
                    var model = Session["data"];
                    return PartialView("_CustomReportPartial", model);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }


        [HttpGet]
        public ActionResult CustomReportTemplate()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Getreportfields(int rptid)
        {
            var v1 = RestClientReport.Getreportfields(rptid);
            return Json(v1, JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        //public ActionResult CustomReport()
        //{
        //    return View();
        //}

        #endregion
        #region BUwise Analyatic Dashboard
        [HttpGet]
        public ActionResult BUAnalyticsDashboard()
        {
            return View();
        }
            #endregion

            [HttpGet]
        public ActionResult ReportDashboard()
        {
            try
            {

                if (Session["RoleId"].ToString() == "6810")
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll().OrderBy(x => x.CompanyName);
                }
                else if (Session["RoleId"].ToString() != "1")
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    ViewBag.Companyslist = RestClient.CompanyGetAll().OrderBy(x => x.CompanyName).Where(x => x.CompanyID == cmpid);
                }
                else
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll().OrderBy(x => x.CompanyName);
                }
                ViewBag.ShowReportButton = false;
                return View("LocalReportExample");
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        public JsonResult ClearSelection()
        {
            JsonResult result;
            Session["FilterData"] = null;
            Session["NFilterData"] = null;
            result = Json(new { message = "True" });
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        //public ActionResult PrintReport()
        //{
        //    try
        //    {
        //        return View(RestClientReport.PrintReports((reqDailyInOutReport)Session["NFilterData"], Session["UserEmail"].ToString(), (DataTable)Session["Datatable"]));
        //    }
        //    catch (Exception)
        //    {
        //        return RedirectToAction("ErrorPage", "PayTime");
        //    }
        //    finally
        //    {
               
        //    }

        //}

        #region DynamicReportTemplate
        [HttpGet]
        public ActionResult DynamicReportTemplate()
        {

            return View();
        }

        [HttpPost]
        public ActionResult DynamicReportTemplate(DynamicShowReporttemplate objcls)
        {
            objcls.ReportmailID = Session["UserEmail"].ToString();
            DataTable model = RestClientReport.GetDynamicReportDeshBord(objcls);
            Session["NFilterData"] = Session["FilterData"];
            Session["FilterData"] = null;
            Session["data"] = null;
            Session["data"] = model;
            ViewBag.RptID = 1;
            ViewBag.ShowReportButton = false;
            ViewBag.selectedreportid = 0;
            ViewBag.selectedreport = "";
            ViewBag.crptid = 0;
            ViewBag.ReportHead = objcls.ReportHead;
            ViewBag.Title = objcls.ReportHead;
            ViewBag.Reportflage = objcls.Reportflage;
            ViewBag.ReportID = objcls.ReportID;
            return View("DynamicReportExample", model);
        }

        [HttpGet]
        public ActionResult DynamicReportExample()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DynamicReportExample(DynamicShowReporttemplate objcls)
        {

            objcls.ReportmailID = Session["UserEmail"].ToString();
            DataTable model = RestClientReport.GetDynamicReportDeshBord(objcls);

            Session["NFilterData"] = Session["FilterData"];
            Session["FilterData"] = null;
            Session["data"] = null;
            Session["data"] = model;
            ViewBag.RptID = 1;
            ViewBag.ShowReportButton = false;
            ViewBag.selectedreportid = 0;
            ViewBag.selectedreport = "";
            ViewBag.crptid = 0;
            ViewBag.ReportHead = objcls.ReportHead;
            ViewBag.Title = objcls.ReportHead;
            ViewBag.Reportflage = objcls.Reportflage;
            ViewBag.ReportID = objcls.ReportID;
            return View(model);
        }

        [HttpGet]
        public ActionResult DynamicReportDashboard()
        {
            try
            {
                if (Session["RoleId"].ToString() != "1")
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    ViewBag.Companyslist = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                }
                else
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll();
                }

                if (Session["FilterData"] != null)
                {
                    DynamicShowReporttemplate objcls = new DynamicShowReporttemplate();
                    var model = Session["data"];
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["data"] = model;
                    ViewBag.RptID = 1;
                    ViewBag.ShowReportButton = false;
                    ViewBag.selectedreportid = 0;
                    ViewBag.selectedreport = "";
                    ViewBag.crptid = 0;
                    ViewBag.Reportflage = objcls.Reportflage;
                    ViewBag.ReportHead = "";

                    return View();
                }
                else
                {
                    ViewBag.selectedreportid = 0;
                    var model = Session["data"];
                    return View();
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        [HttpPost]
        public ActionResult DynamicReportDashboard(DynamicShowReporttemplate objcls)
        {
            try
            {
                objcls.ReportmailID = Session["UserEmail"].ToString();
                DataTable model = RestClientReport.GetDynamicReportDeshBord(objcls);
                Session["NFilterData"] = Session["FilterData"];
                Session["FilterData"] = null;
                Session["data"] = null;
                Session["data"] = model;
                ViewBag.RptID = 1;
                ViewBag.ShowReportButton = false;
                ViewBag.selectedreportid = 0;
                ViewBag.selectedreport = "";
                ViewBag.crptid = 0;
                ViewBag.ReportHead = objcls.ReportHead;
                ViewBag.Title = objcls.ReportHead;
                ViewBag.Reportflage = objcls.Reportflage;
                ViewBag.ReportID = objcls.ReportID;
                return View("DynamicReportExample", model);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        #endregion

        #region FastTrack Report

        #region GatePass Issue Report
        [ValidateInput(false)]
        public ActionResult GatePassIssueReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<GatePass> model = MasterDataRestClient.ViewDailyReportQueryForGatePassIsuueGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<GatePass>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("GatePassReport/GatePassIssueReport", model);

                }
                else
                {
                    var model = Session["data"];
                    return PartialView("GatePassReport/GatePassIssueReport", model);
                }
            }
            catch (Exception Ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        #endregion

        #region GatePass Policy Assigned Report
        [ValidateInput(false)]
        public ActionResult GatePassPolicyAssignedReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<GatePassAppliedPolicy> model = MasterDataRestClient.ViewGatePassPolicyAssignedGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<GatePassAppliedPolicy>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("GatePassReport/GatePassPolicyReport", model);

                }
                else
                {
                    var model = Session["data"];
                    return PartialView("GatePassReport/GatePassPolicyReport", model);
                }
            }
            catch (Exception Ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        #endregion

        #region FastTrack DailyInOut Security Report
        [ValidateInput(false)]
        public ActionResult DailyInOutSecurityReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<FastTrackDailyInOutPunchingReport> model = MasterDataRestClient.ViewDailyReportQueryForFastTrackSecurityGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<FastTrackDailyInOutPunchingReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("GatePassReport/_DailyInOutSecurityReport", model);

                }
                else
                {
                    var model = Session["data"];
                    return PartialView("GatePassReport/_DailyInOutSecurityReport", model);
                }
            }
            catch (Exception Ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        #endregion

        #region FastTrack Attendace Of Month Report
        [ValidateInput(false)]
        public ActionResult MonthlyAttendanceReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<MonthlyMuster> model = MasterDataRestClient.ViewFastTrackMonthlyAttendanceReportGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<MonthlyMuster>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    //foreach (MonthlyMuster b in model)
                    //{
                    ViewBag.Attn_Dt = csreq.FromDate;
                    //break;
                    //}
                    return PartialView("GatePassReport/_MonthlyAttendanceReport", model);

                }
                else
                {
                    var model = Session["data"] as IEnumerable<MonthlyMuster>;
                    if (model != null)
                    {
                        foreach (MonthlyMuster b in model)
                        {
                            ViewBag.Attn_Dt = b.PFromDate;
                            break;
                        }
                    }
                    return PartialView("GatePassReport/_MonthlyAttendanceReport", model);
                }
            }
            catch (Exception Ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        #endregion

        #region FastTrack Employee Punch Comparison Report
        [ValidateInput(false)]
        public ActionResult EmployeePunchComparisonReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<FastTrackEmployeePunchComparisonReport> model = MasterDataRestClient.ViewFastTrackEmpPunchComparisonReportGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<FastTrackEmployeePunchComparisonReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;
                    return PartialView("GatePassReport/_EmployeePunchComparisonReport", model);

                }
                else
                {
                    var model = Session["data"];
                    return PartialView("GatePassReport/_EmployeePunchComparisonReport", model);
                }
            }
            catch (Exception Ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        #endregion

        #region FastTrack Employee Attendance Correction Report

        [ValidateInput(false)]
        public ActionResult AttendanceCorrectionReport()
        {
            try
            {
                if (Session["FilterData"] != null)
                {
                    csreq = (reqDailyInOutReport)Session["FilterData"];
                    IEnumerable<AttendanceCorrectionReport> model = MasterDataRestClient.ViewFastTrackEmpAttendaceCorrectionGetAll(csreq);
                    Session["NFilterData"] = Session["FilterData"];
                    Session["FilterData"] = null;
                    Session["data"] = null;
                    Session["Datatable"] = null;
                    DataTable dt = GridViewHelper.ToDataTable<AttendanceCorrectionReport>(model);
                    Session["Datatable"] = dt;
                    Session["data"] = model;                    
                    return PartialView("GatePassReport/AttendanceCorrectionReport", model);

                }
                else
                {
                    var model = Session["data"];
                    return PartialView("GatePassReport/AttendanceCorrectionReport", model);
                }
            }
            catch (Exception Ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        #endregion

        #endregion

    }
    public static class GridViewHelper
    {
        #region GridViewHelper
        private static GridViewSettings exportReportsettings;
        private static GridViewSettings customExportReportsettings;
        public static GridViewSettings ExportReportsettings
        {
            get
            {
                exportReportsettings = ExportReport();
                return exportReportsettings;
            }
        }
        public static GridViewSettings CustomExportReportsettings
        {
            get
            {
                customExportReportsettings = CustExportReport();
                return customExportReportsettings;
            }
        }
        private static GridViewSettings ExportReport()
        {
            GridViewSettings settings = new GridViewSettings();
            var isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (ReportDetails.Rptid > 16)
            {

                #region Summary Report
                if (ReportDetails.Rptid == 17)
                {
                    settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString(isdateformat) + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString(isdateformat) + ")";
                    settings.SettingsExport.PageHeader.Left = ReportDetails.cname.ToUpper();
                    settings.SettingsExport.PageHeader.Font.Size = 12;
                    settings.SettingsExport.PageFooter.Font.Size = 6;
                    settings.SettingsExport.Styles.Cell.Font.Size = 8;
                    settings.SettingsExport.Styles.Header.Font.Size = 10;
                    settings.SettingsExport.Styles.Header.Font.Bold = true;
                    settings.Name = ReportDetails.RptName;
                    if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                    {
                        settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString("" + isdateformat.Replace("mm", "MM") + " hh:mm");
                    }
                    else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                    {
                        settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString("" + isdateformat.Replace("M", "MMM") + " hh:mm");
                    }
                    settings.SettingsExport.PageFooter.Right = "www.mantratec.com";
                    //settings.CallbackRouteValues = new { Controller = "Report", Action = "DepartmentSummaryPivotePartial" };
                    //settings.SettingsBehavior.ConfirmDelete = true;
                    //settings.SettingsPager.Visible = true;
                    //settings.Settings.ShowGroupPanel = true;
                    //settings.Settings.ShowFilterRow = true;
                    //settings.SettingsBehavior.AllowSelectByRowClick = true;
                    //settings.SettingsBehavior.EnableCustomizationWindow = true;
                    //settings.SettingsAdaptivity.AdaptiveDetailColumnCount = 1;
                    //settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
                    //settings.SettingsAdaptivity.HideDataCellsAtWindowInnerWidth = 0;
                    //settings.SettingsBehavior.AllowEllipsisInText = true;
                    //settings.Settings.ShowTitlePanel = true;

                    settings.Columns.Add("BranchName", "Branch").GroupIndex = 0;
                    settings.Columns.Add("DepartmentName", "Department").GroupIndex = 1;
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "Attn_dt";
                        //column.GroupIndex = 2;
                        column.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                        if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                        {
                            column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("mm", "MM");
                        }
                        else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                        {
                            column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("M", "MMM");
                        }
                    });
                    settings.Columns.Add("TotalEmployeeCount", "Total Employee Count");
                    settings.Columns.Add("Present");
                    settings.Columns.Add("AttAbsent", "Absent");
                    settings.Columns.Add("AttLeave", "Leave");
                    settings.Columns.Add("WeeklyOff", "Weekly Off");
                    settings.Columns.Add("Holiday");
                    settings.Columns.Add("AttError", "Error");
                    settings.SettingsBehavior.AutoExpandAllGroups = true;
                    //settings.SettingsText.Title = "Department Summary Report";
                }
                else if (ReportDetails.Rptid == 18)
                {
                    settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString("dd-MM-yyyy") + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString("dd-MM-yyyy") + ")";
                    settings.SettingsExport.PageHeader.Left = ReportDetails.cname.ToUpper();
                    settings.SettingsExport.PageHeader.Font.Size = 12;
                    settings.SettingsExport.PageFooter.Font.Size = 6;
                    settings.SettingsExport.Styles.Cell.Font.Size = 8;
                    settings.SettingsExport.Styles.Header.Font.Size = 10;
                    settings.SettingsExport.Styles.Header.Font.Bold = true;
                    settings.Name = ReportDetails.RptName;
                    if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                    {
                        settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString("" + isdateformat.Replace("mm", "MM") + " hh:mm");
                    }
                    else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                    {
                        settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString("" + isdateformat.Replace("M", "MMM") + " hh:mm");
                    }
                    //settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString(""+isdateformat+" hh:mm");
                    settings.SettingsExport.PageFooter.Right = "www.mantratec.com";
                    //settings.SettingsBehavior.ConfirmDelete = true;
                    //settings.SettingsPager.Visible = true;
                    settings.Settings.ShowGroupPanel = true;
                    //settings.Settings.ShowFilterRow = true;
                    //settings.SettingsBehavior.AllowSelectByRowClick = true;
                    //settings.SettingsBehavior.EnableCustomizationWindow = true;
                    //settings.SettingsAdaptivity.AdaptiveDetailColumnCount = 1;
                    //settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
                    //settings.SettingsAdaptivity.HideDataCellsAtWindowInnerWidth = 0;
                    //settings.SettingsBehavior.AllowEllipsisInText = true;
                    //settings.Settings.ShowTitlePanel = true;
                    settings.Columns.Add("BranchName").GroupIndex = 0;
                    settings.Columns.Add("DepartmentName").GroupIndex = 1;
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "Attn_dt";
                        column.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                        if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                        {
                            column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("mm", "MM");
                        }
                        else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                        {
                            column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("M", "MMM");
                        }
                        //column.PropertiesEdit.DisplayFormatString = isdateformat;
                    });
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "EarlyInHrs";
                        column.Caption = "Early In Hrs";
                        column.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        column.Width = System.Web.UI.WebControls.Unit.Percentage(10);
                        column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    });
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "EarlyInCount";
                        column.Caption = "Early In Count";
                        column.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        column.Width = System.Web.UI.WebControls.Unit.Percentage(10);
                        column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    });
                    settings.SettingsBehavior.AutoExpandAllGroups = true;
                    //settings.SettingsText.Title = "Early In Summary Report";
                }
                else if (ReportDetails.Rptid == 19)
                {
                    settings.Settings.ShowTitlePanel = true;
                    settings.SettingsExport.PageHeader.Center = ReportDetails.RptTitle + "-" + ReportDetails.Fdate.Substring(5, 2) + "/" + ReportDetails.Fdate.Substring(0, 4);
                    settings.SettingsExport.PageHeader.Left = ReportDetails.cname.ToUpper();
                    settings.SettingsExport.PageHeader.Font.Size = 14;
                    settings.SettingsExport.Styles.Cell.Font.Size = 6;
                    settings.SettingsExport.Styles.Header.Font.Size = 7;
                    settings.SettingsExport.TopMargin = 10;
                    settings.SettingsExport.RightMargin = 05;
                    settings.SettingsExport.BottomMargin = 05;
                    settings.SettingsExport.LeftMargin = 05;
                    settings.Settings.ShowFooter = true;
                    settings.SettingsExport.PageFooter.Left = "Print Date: " + DateTime.Now.ToString("" + isdateformat + " hh:mm") + "  P=Total Present ,A=Total Absent,H=Total Holiday,WO=Total WeekOff, TH=Total Hours,PL=Total Paid Leave";
                    settings.SettingsExport.PageFooter.Right = "www.mantratec.com";
                    settings.SettingsExport.PageFooter.Font.Size = 7;
                    settings.Styles.Header.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    settings.Styles.Header.Font.Bold = true;
                    settings.Styles.Cell.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    settings.SettingsExport.Landscape = true;
                    settings.Name = ReportDetails.RptName;
                    settings.Columns.Add("BranchName", "Branch");
                    settings.Columns.Add("DepartmentName", "Department");
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "EmpCode";
                        column.Width = 80;
                        column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left;

                    });
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "EmpName";
                        column.Width = 300;
                        column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left;
                    });
                    int j = ReportStartEndday.Fday;
                    for (int i = 1; i < ReportStartEndday.Lday; i++)
                    {
                        string colname = string.Empty;
                        colname = "D" + j.ToString();
                        //if (j < 10)
                        //{
                        //    colname = "D0" + j.ToString();
                        //}
                        //else
                        //{
                        //    colname = "D" + j.ToString();
                        //}
                        settings.Columns.Add(colname, j.ToString()).Width = 40;
                        j++;
                        if (j == ReportStartEndday.Lday)
                        {
                            j = 1;
                        }
                    }
                    //settings.Columns.Add("D01", "1").Width = 40;
                    //settings.Columns.Add("D02", "2").Width = 40;
                    //settings.Columns.Add("D03", "3").Width = 40;
                    //settings.Columns.Add("D04", "4").Width = 40;
                    //settings.Columns.Add("D05", "5").Width = 40;
                    //settings.Columns.Add("D06", "6").Width = 40;
                    //settings.Columns.Add("D07", "7").Width = 40;
                    //settings.Columns.Add("D08", "8").Width = 40;
                    //settings.Columns.Add("D09", "9").Width = 40;
                    //settings.Columns.Add("D10", "10").Width = 40;
                    //settings.Columns.Add("D11", "11").Width = 40;
                    //settings.Columns.Add("D12", "12").Width = 40;
                    //settings.Columns.Add("D13", "13").Width = 40;
                    //settings.Columns.Add("D14", "14").Width = 40;
                    //settings.Columns.Add("D15", "15").Width = 40;
                    //settings.Columns.Add("D16", "16").Width = 40;
                    //settings.Columns.Add("D17", "17").Width = 40;
                    //settings.Columns.Add("D18", "18").Width = 40;
                    //settings.Columns.Add("D19", "19").Width = 40;
                    //settings.Columns.Add("D20", "20").Width = 40;
                    //settings.Columns.Add("D21", "21").Width = 40;
                    //settings.Columns.Add("D22", "22").Width = 40;
                    //settings.Columns.Add("D23", "23").Width = 40;
                    //settings.Columns.Add("D24", "24").Width = 40;
                    //settings.Columns.Add("D25", "25").Width = 40;
                    //settings.Columns.Add("D26", "26").Width = 40;
                    //settings.Columns.Add("D27", "27").Width = 40;
                    //settings.Columns.Add("D28", "28").Width = 40;
                    //settings.Columns.Add("D29", "29").Width = 40;
                    //settings.Columns.Add("D30", "30").Width = 40;
                    settings.Columns.Add("P").Width = 50;
                    settings.Columns.Add("A").Width = 50;
                    settings.Columns.Add("WO").Width = 50;
                    settings.Columns.Add("H").Width = 50;
                    settings.Columns.Add("PL").Width = 100;
                    settings.Columns.Add("TotHour", "TH").Width = 100;
                    //settings.SettingsText.Title = ReportDetails.RptTitle;
                }
                else if (ReportDetails.Rptid == 20)
                {


                    settings.Settings.ShowTitlePanel = true;
                    settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " - " + ReportDetails.Fdate.Substring(5, 2) + "/" + ReportDetails.Fdate.Substring(0, 4);
                    settings.SettingsExport.PageHeader.Left = "Company: " + ReportDetails.cname.ToUpper() + "   Branch: " + ReportDetails.bname.ToUpper();
                    settings.SettingsExport.PageHeader.Font.Bold = true;
                    settings.SettingsExport.PageHeader.Font.Size = 14;
                    settings.SettingsExport.Styles.Cell.Font.Size = 6;
                    settings.SettingsExport.Styles.Header.Font.Size = 7;
                    settings.SettingsExport.RightMargin = 05;
                    settings.SettingsExport.BottomMargin = 05;
                    settings.SettingsExport.LeftMargin = 05;
                    //settings.SettingsExport.TopMargin = 10;
                    //settings.SettingsExport.RightMargin = 05;
                    //settings.SettingsExport.BottomMargin = 05;
                    //settings.SettingsExport.LeftMargin = 05;
                    settings.Settings.ShowFooter = true;
                    if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                    {
                        settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString("" + isdateformat.Replace("mm", "MM") + " hh:mm");
                    }
                    else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                    {
                        settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString("" + isdateformat.Replace("M", "MMM") + " hh:mm");
                    }
                    //settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString(""+isdateformat+" hh:mm");
                    settings.SettingsExport.PageFooter.Right = "www.mantratec.com";
                    settings.SettingsExport.PageFooter.Font.Size = 7;
                    settings.Styles.Header.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    settings.Styles.Header.Font.Bold = true;
                    settings.Styles.Cell.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    settings.SettingsExport.Landscape = true;
                    settings.Name = ReportDetails.RptName;
                    //settings.Columns.Add("Department", "Department");
                    settings.Styles.Header.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    settings.Styles.Cell.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    //settings.Columns.Add("EmpID", "EmpID");
                    //settings.Columns.Add("EmpName", "EmpName");
                    //settings.Columns.Add("Designation", "Designation");
                    //settings.Columns.Add("Shift", "Shift");
                    settings.Columns.Add("EmpCode", "EmpCode").GroupIndex = 0;
                    settings.Columns.Add("EmpName", "Name").GroupIndex = 0;
                    settings.Columns.Add("Status", "Status");

                    int j = ReportStartEndday.Fday;
                    for (int i = 1; i < ReportStartEndday.Lday; i++)
                    {
                        settings.Columns.Add("col_" + j.ToString(), j.ToString()).Width = 60;
                        j++;
                        if (j == ReportStartEndday.Lday)
                        {
                            j = 1;
                        }
                    }
                    //settings.Columns.Add("col_1", "1").Width = 60;
                    //settings.Columns.Add("col_2", "2").Width = 60;
                    //settings.Columns.Add("col_3", "3").Width = 60;
                    //settings.Columns.Add("col_4", "4").Width = 60;
                    //settings.Columns.Add("col_5", "5").Width = 60;
                    //settings.Columns.Add("col_6", "6").Width = 60;
                    //settings.Columns.Add("col_7", "7").Width = 60;
                    //settings.Columns.Add("col_8", "8").Width = 60;
                    //settings.Columns.Add("col_9", "9").Width = 60;
                    //settings.Columns.Add("col_10", "10").Width = 60;
                    //settings.Columns.Add("col_11", "11").Width = 60;
                    //settings.Columns.Add("col_12", "12").Width = 60;
                    //settings.Columns.Add("col_13", "13").Width = 60;
                    //settings.Columns.Add("col_14", "14").Width = 60;
                    //settings.Columns.Add("col_15", "15").Width = 60;
                    //settings.Columns.Add("col_16", "16").Width = 60;
                    //settings.Columns.Add("col_17", "17").Width = 60;
                    //settings.Columns.Add("col_18", "18").Width = 60;
                    //settings.Columns.Add("col_19", "19").Width = 60;
                    //settings.Columns.Add("col_20", "20").Width = 60;
                    //settings.Columns.Add("col_21", "21").Width = 60;
                    //settings.Columns.Add("col_22", "22").Width = 60;
                    //settings.Columns.Add("col_23", "23").Width = 60;
                    //settings.Columns.Add("col_24", "24").Width = 60;
                    //settings.Columns.Add("col_25", "25").Width = 60;
                    //settings.Columns.Add("col_26", "26").Width = 60;
                    //settings.Columns.Add("col_27", "27").Width = 60;
                    //settings.Columns.Add("col_28", "28").Width = 60;
                    //settings.Columns.Add("col_29", "29").Width = 60;
                    //settings.Columns.Add("col_30", "30").Width = 60;
                    //settings.Columns.Add("col_31", "31").Width = 60;
                    settings.Columns.Add("TotalFinal", "Total").Width = 100;
                }
                else if (ReportDetails.Rptid == 27)
                {
                    if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                    {
                        settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString(isdateformat.Replace("mm", "MM")) + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString(isdateformat.Replace("mm", "MM")) + ")";
                    }
                    else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                    {
                        settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString(isdateformat.Replace("M", "MMM")) + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString(isdateformat.Replace("M", "MMM")) + ")";
                    }
                    //settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString(isdateformat) + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString(isdateformat) + ")";
                    settings.SettingsExport.PageHeader.Left = ReportDetails.cname.ToUpper();
                    settings.SettingsExport.PageHeader.Font.Size = 12;
                    settings.SettingsExport.PageFooter.Font.Size = 6;
                    settings.SettingsExport.Styles.Cell.Font.Size = 8;
                    settings.SettingsExport.Styles.Header.Font.Size = 10;
                    settings.SettingsExport.Styles.Header.Font.Bold = true;
                    settings.Name = ReportDetails.RptName;
                    if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                    {
                        settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString("" + isdateformat.Replace("mm", "MM") + " hh:mm");
                    }
                    else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                    {
                        settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString("" + isdateformat.Replace("M", "MMM") + " hh:mm");
                    }
                    //settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString(""+isdateformat+" hh:mm");
                    settings.SettingsExport.PageFooter.Right = "app.minopcloud.com";
                    settings.Columns.Add("EmpId").Visible = true;
                    settings.Columns.Add("Name").Visible = true;
                    settings.Columns.Add("EmpCode").Visible = true;
                    settings.Columns.Add("PunchID").Visible = true;
                    settings.Columns.Add("DepartmentName").Visible = true;
                    settings.Columns.Add("DesignationName").Visible = true;
                    settings.Columns.Add("Attn_dt").Visible = true;
                    settings.Columns.Add("PunchTime").Visible = true;
                    settings.Columns.Add("Temperature").Visible = true;
                    settings.Columns.Add("Status").Visible = true;
                    settings.Columns.Add("CompanyName").Visible = true;
                    settings.Columns.Add("CompanyAddress").Visible = true;
                    settings.Columns.Add("BranchName").Visible = true;
                }
                else if (ReportDetails.Rptid == 28)
                {
                    settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString("dd-MM-yyyy") + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString("dd-MM-yyyy") + ")";
                    settings.SettingsExport.PageHeader.Left = ReportDetails.cname.ToUpper();
                    settings.SettingsExport.PageHeader.Font.Size = 12;
                    settings.SettingsExport.PageFooter.Font.Size = 6;
                    settings.SettingsExport.Styles.Cell.Font.Size = 8;
                    settings.SettingsExport.Styles.Header.Font.Size = 10;
                    settings.SettingsExport.Styles.Header.Font.Bold = true;
                    settings.Name = ReportDetails.RptName;
                    settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString("" + isdateformat + " hh:mm");
                    settings.SettingsExport.PageFooter.Right = "app.minopcloud.com";
                    settings.Columns.Add("Empcode").Visible = true;
                    settings.Columns.Add("EmpName").Visible = true;
                    settings.Columns.Add("BranchName").Visible = true;
                    settings.Columns.Add("CompanyName").Visible = true;
                    settings.Columns.Add("DepartmentName").Visible = true;
                    settings.Columns.Add("DesignationName").Visible = true;
                    settings.Columns.Add("PunchDate").Visible = true;
                    settings.Columns.Add("PunchTime").Visible = true;
                    settings.Columns.Add("Location").Visible = true;
                }
                if (ReportDetails.Rptid == 29)
                {
                    if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                    {
                        settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString(isdateformat.Replace("mm", "MM")) + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString(isdateformat.Replace("mm", "MM")) + ")";
                    }
                    else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                    {
                        settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString(isdateformat.Replace("M", "MMM")) + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString(isdateformat.Replace("M", "MMM")) + ")";
                    }
                    //settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString(isdateformat) + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString(isdateformat) + ")";
                    settings.SettingsExport.PageHeader.Left = ReportDetails.cname.ToUpper();
                    settings.SettingsExport.PageHeader.Font.Size = 12;
                    settings.SettingsExport.PageFooter.Font.Size = 6;
                    settings.SettingsExport.Styles.Cell.Font.Size = 8;
                    settings.SettingsExport.Styles.Header.Font.Size = 10;
                    settings.SettingsExport.Styles.Header.Font.Bold = true;
                    settings.Name = ReportDetails.RptName;
                    if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                    {
                        settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString("" + isdateformat.Replace("mm", "MM") + " hh:mm");
                    }
                    else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                    {
                        settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString("" + isdateformat.Replace("M", "MMM") + " hh:mm");
                    }
                    //settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString(""+isdateformat+" hh:mm");
                    settings.SettingsExport.PageFooter.Right = "app.minopcloud.com";
                    settings.Columns.Add("Empname").Visible = true;
                    settings.Columns.Add("Empcode").Visible = true;
                    settings.Columns.Add("LeaveBalance").Visible = true;
                    settings.Columns.Add("ConsumeDays").Visible = true;
                    settings.Columns.Add("Total").Visible = true;
                }

                if (ReportDetails.Rptid == 32)
                {
                    settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString("dd-MM-yyyy") + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString("dd-MM-yyyy") + ")";
                    settings.SettingsExport.PageHeader.Left = ReportDetails.cname.ToUpper();
                    settings.SettingsExport.PageHeader.Font.Size = 12;
                    settings.SettingsExport.PageFooter.Font.Size = 6;
                    settings.SettingsExport.Styles.Cell.Font.Size = 8;
                    settings.SettingsExport.Styles.Header.Font.Size = 10;
                    settings.SettingsExport.Styles.Header.Font.Bold = true;
                    settings.Name = ReportDetails.RptName;
                    settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString("" + isdateformat + " hh:mm");
                    settings.SettingsExport.PageFooter.Right = "app.minopcloud.com";
                    settings.Columns.Add("Empcode").Visible = true;
                    settings.Columns.Add("EmpName").Visible = true;
                    settings.Columns.Add("BranchName").Visible = true;
                    settings.Columns.Add("CompanyName").Visible = true;
                    settings.Columns.Add("DepartmentName").Visible = true;
                    settings.Columns.Add("DesignationName").Visible = true;
                    settings.Columns.Add("PunchDate").Visible = true;
                    settings.Columns.Add("PunchTime").Visible = true;
                    settings.Columns.Add("Location").Visible = true;
                }

                if (ReportDetails.Rptid == 33)
                {
                    settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString("dd-MM-yyyy") + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString("dd-MM-yyyy") + ")";
                    settings.SettingsExport.PageHeader.Left = ReportDetails.cname.ToUpper();
                    settings.SettingsExport.PageHeader.Font.Size = 12;
                    settings.SettingsExport.PageFooter.Font.Size = 6;
                    settings.SettingsExport.Styles.Cell.Font.Size = 8;
                    settings.SettingsExport.Styles.Header.Font.Size = 10;
                    settings.SettingsExport.Styles.Header.Font.Bold = true;
                    settings.Name = ReportDetails.RptName;
                    settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString("" + isdateformat + " hh:mm");
                    settings.SettingsExport.PageFooter.Right = "app.minopcloud.com";
                    settings.Columns.Add("Empcode").Visible = true;
                    settings.Columns.Add("Punchid").Visible = true;
                    settings.Columns.Add("Attn_Dt").Visible = true;
                    settings.Columns.Add("DepartmentName").Visible = true;
                    settings.Columns.Add("Punch_IN_1").Visible = true;
                    settings.Columns.Add("Punch_OUT_1").Visible = true;
                    settings.Columns.Add("Punch_IN_2").Visible = true;
                    settings.Columns.Add("Punch_OUT_2").Visible = true;
                    settings.Columns.Add("Punch_IN_3").Visible = true;
                    settings.Columns.Add("Punch_OUT_3").Visible = true;
                    settings.Columns.Add("Punch_IN_4").Visible = true;
                    settings.Columns.Add("Punch_OUT_4").Visible = true;
                    settings.Columns.Add("Punch_IN_5").Visible = true;
                    settings.Columns.Add("Punch_OUT_5").Visible = true;
                    settings.Columns.Add("Punch_IN_6").Visible = true;
                    settings.Columns.Add("Punch_OUT_6").Visible = true;
                    settings.Columns.Add("Punch_IN_7").Visible = true;
                    settings.Columns.Add("Punch_OUT_7").Visible = true;
                    settings.Columns.Add("Punch_IN_8").Visible = true;
                    settings.Columns.Add("Punch_OUT_8").Visible = true;
                    settings.Columns.Add("Punch_IN_9").Visible = true;
                    settings.Columns.Add("Punch_OUT_9").Visible = true;
                    settings.Columns.Add("Punch_IN_10").Visible = true;
                    settings.Columns.Add("Punch_OUT_10").Visible = true;
                    settings.Columns.Add("Punch_IN_11").Visible = true;
                    settings.Columns.Add("Punch_OUT_11").Visible = true;
                    settings.Columns.Add("Punch_IN_12").Visible = true;
                    settings.Columns.Add("Punch_OUT_12").Visible = true;
                    settings.Columns.Add("Punch_IN_13").Visible = true;
                    settings.Columns.Add("Punch_OUT_13").Visible = true;
                    settings.Columns.Add("Punch_IN_14").Visible = true;
                    settings.Columns.Add("Punch_OUT_14").Visible = true;
                    settings.Columns.Add("Punch_IN_15").Visible = true;
                    settings.Columns.Add("Punch_OUT_15").Visible = true;
                    settings.Columns.Add("Punch_IN_16").Visible = true;
                    settings.Columns.Add("Punch_OUT_16").Visible = true;
                    settings.Columns.Add("Punch_IN_17").Visible = true;
                    settings.Columns.Add("Punch_OUT_17").Visible = true;
                    settings.Columns.Add("Punch_IN_18").Visible = true;
                    settings.Columns.Add("Punch_OUT_18").Visible = true;
                    settings.Columns.Add("Punch_IN_19").Visible = true;
                    settings.Columns.Add("Punch_OUT_19").Visible = true;
                    settings.Columns.Add("Punch_IN_20").Visible = true;
                    settings.Columns.Add("Punch_OUT_20").Visible = true;
                }

                #endregion
            }
            else
            {
                #region DailyReport
                settings.Name = ReportDetails.RptName;
                settings.KeyFieldName = "EmpID";
                if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                {
                    settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString(isdateformat.Replace("mm", "MM")) + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString(isdateformat.Replace("mm", "MM")) + ")";
                }
                else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                {
                    settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString(isdateformat.Replace("M", "MMM")) + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString(isdateformat.Replace("M", "MMM")) + ")";
                }
                //settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString(isdateformat) + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString(isdateformat) + ")";
                settings.SettingsExport.PageHeader.Left = ReportDetails.cname.ToUpper();
                settings.SettingsExport.PageHeader.Font.Size = 12;
                if (ReportDetails.Rptid == 9)
                {
                    settings.SettingsExport.LeftMargin = 50;
                    settings.SettingsExport.RightMargin = 50;
                }
                if (ReportDetails.Rptid == 4)
                {
                    settings.SettingsExport.Styles.Cell.Font.Size = 8;
                    settings.SettingsExport.Styles.Header.Font.Size = 9;
                }
                else
                {
                    settings.SettingsExport.Styles.Cell.Font.Size = 9;
                    settings.FormatConditions.AddColorScale("FinalStatus", GridConditionColorScaleFormat.GreenWhite);
                    settings.SettingsExport.Styles.Header.Font.Size = 10;
                }

                if (ReportDetails.Rptid != 4 && ReportDetails.Rptid != 5 && ReportDetails.Rptid != 6 || ReportDetails.Rptid != 12)
                    settings.SettingsExport.Landscape = true;



                if (ReportDetails.Rptid == 15)
                {
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "Punchid";
                        column.Caption = "Card No";
                        column.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
                        column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    });
                }
                if (ReportDetails.Rptid == 15 || ReportDetails.Rptid == 16)
                {
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "Empcode";
                        column.Caption = "Employee Code";
                        column.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
                        column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    });
                }
                else
                { settings.Columns.Add("Empcode").Visible = false; }

                if (ReportDetails.Rptid == 15)
                {
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "EmpName";
                        column.Caption = "Employee Name";
                    });
                }
                else
                {
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "Name";
                        column.Caption = "EmpCode Name";
                    });
                }

                settings.Columns.Add("CardNo").Visible = false;
                if (ReportDetails.Rptid == 6)
                {
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "Attn_dt";

                        //column.GroupIndex = 2;
                        column.Caption = "Attn Date";
                        column.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                        if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                        {
                            column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("mm", "MM");
                        }
                        else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                        {
                            column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("M", "MMM");
                        }
                        //column.PropertiesEdit.DisplayFormatString = isdateformat;
                    });
                }
                else
                {
                    if (ReportDetails.Rptid != 12 && ReportDetails.Rptid != 13 && ReportDetails.Rptid != 16 && ReportDetails.Rptid != 14 && ReportDetails.Rptid != 15)
                        settings.Columns.Add(column =>
                        {
                            column.FieldName = "Attn_dt";
                            column.Caption = "Attn Date";
                            column.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                            if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                            {
                                column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("mm", "MM");
                            }
                            else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                            {
                                column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("M", "MMM");
                            }
                            //column.PropertiesEdit.DisplayFormatString = isdateformat;
                        });
                }


                settings.Columns.Add("DepartmentName", "Department");


                if (ReportDetails.Rptid == 15)
                {
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "Attndt";
                        column.Caption = "Attn Date";
                        column.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                        if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                        {
                            column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("mm", "MM");
                        }
                        else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                        {
                            column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("M", "MMM");
                        }
                        //column.PropertiesEdit.DisplayFormatString = isdateformat;
                        column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    });
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "Punch";
                        column.Caption = "Punch";
                        column.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    });
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "DeviceCode";
                        column.Caption = "Device Code";
                        column.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    });
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "DeviceName";
                        column.Caption = "Device Name";
                        column.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    });
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "DeviceIP";
                        column.Caption = "DeviceIP";
                        column.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    });
                }
                if (ReportDetails.Rptid == 16)
                {
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "AttnDate";
                        column.Caption = "Attn Date";
                        column.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                        if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                        {
                            column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("mm", "MM");
                        }
                        else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                        {
                            column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("M", "MMM");
                        }
                        //column.PropertiesEdit.DisplayFormatString = isdateformat;
                    });
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "Punch";
                        column.Caption = "Punch Time";
                        column.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                        column.PropertiesEdit.DisplayFormatString = "HH:mm:ss";
                        column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    });
                    settings.Columns.Add(column =>
                    {
                        column.FieldName = "Location";
                        column.Caption = "Location";
                        column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                    });
                }

                if (ReportDetails.Rptid != 15 && ReportDetails.Rptid != 16)
                {
                    settings.Columns.Add("DesignationName", "Designation");
                }
                if (ReportDetails.Rptid != 6 && ReportDetails.Rptid != 12 && ReportDetails.Rptid != 13 && ReportDetails.Rptid != 14 && ReportDetails.Rptid != 15 && ReportDetails.Rptid != 16)
                    settings.Columns.Add("ShiftName", "Shift");

                if (ReportDetails.Rptid == 7 || ReportDetails.Rptid == 8)
                { settings.Columns.Add("ShiftStartTime", "Shift StartTime"); }
                else
                { settings.Columns.Add("ShiftStartTime").Visible = false; }

                if (ReportDetails.Rptid == 9 || ReportDetails.Rptid == 10)
                {
                    settings.Columns.Add("ShiftEndTime", "Shift EndTime");
                }

                settings.Columns.Add("LessHr").Visible = false;

                if (ReportDetails.Rptid != 5 && ReportDetails.Rptid != 6 && ReportDetails.Rptid != 9 && ReportDetails.Rptid != 10 && ReportDetails.Rptid != 12 && ReportDetails.Rptid != 13 && ReportDetails.Rptid != 14 && ReportDetails.Rptid != 15 && ReportDetails.Rptid != 16)
                    settings.Columns.Add("InTime", "In Time");
                if (ReportDetails.Rptid == 8)
                    settings.Columns.Add("EarlyIN", "Early In Hrs");
                else
                    settings.Columns.Add("EarlyHr").Visible = false;


                if (ReportDetails.Rptid == 7)
                    settings.Columns.Add("LateHr", "Late In Hrs");
                else
                    settings.Columns.Add("LateHr").Visible = false;
                if (ReportDetails.Rptid == 2)
                    settings.Columns.Add("In_Device", "In Device");
                if (ReportDetails.Rptid != 4 && ReportDetails.Rptid != 5 && ReportDetails.Rptid != 6 && ReportDetails.Rptid != 7 && ReportDetails.Rptid != 8 && ReportDetails.Rptid != 12 && ReportDetails.Rptid != 13 && ReportDetails.Rptid != 14 && ReportDetails.Rptid != 15 && ReportDetails.Rptid != 16)
                    settings.Columns.Add("OutTime");
                if (ReportDetails.Rptid == 9)
                {
                    settings.Columns.Add("EarlyHr", "Early Out Hrs");
                }
                if (ReportDetails.Rptid == 2)
                    settings.Columns.Add("Out_Device", "Out Device");
                if (ReportDetails.Rptid != 4 && ReportDetails.Rptid != 5 && ReportDetails.Rptid != 6 && ReportDetails.Rptid != 7 && ReportDetails.Rptid != 8 && ReportDetails.Rptid != 10 && ReportDetails.Rptid != 12 && ReportDetails.Rptid != 13 && ReportDetails.Rptid != 14 && ReportDetails.Rptid != 15 && ReportDetails.Rptid != 16)
                    settings.Columns.Add("TotHour", "Total Hrs");
                if (ReportDetails.Rptid != 4 && ReportDetails.Rptid != 5 && ReportDetails.Rptid != 6 && ReportDetails.Rptid != 7 && ReportDetails.Rptid != 8 && ReportDetails.Rptid != 10 && ReportDetails.Rptid != 12 && ReportDetails.Rptid != 13 && ReportDetails.Rptid != 14 && ReportDetails.Rptid != 15 && ReportDetails.Rptid != 16 && ReportDetails.Rptid != 9)
                    settings.Columns.Add("OTHr", "OS Hrs");
                if (ReportDetails.Rptid == 10)
                {
                    settings.Columns.Add("LateOUT", "Late Out Hrs");
                }
                if (ReportDetails.Rptid != 12 && ReportDetails.Rptid != 13 && ReportDetails.Rptid != 14 && ReportDetails.Rptid != 15 && ReportDetails.Rptid != 16)
                {
                    settings.Columns.Add("FinalStatus", "Status");
                    settings.Columns.Add("LateOUT").Visible = false;
                    settings.Columns.Add("EarlyIN").Visible = false;
                    settings.Columns.Add("HalfLeavePresent").Visible = false;
                    settings.Columns.Add("InTimeFull").Visible = false;
                    settings.Columns.Add("OutTimeFull").Visible = false;
                    settings.Columns.Add("In_Device").Visible = false;
                    settings.Columns.Add("Out_Device").Visible = false;
                    settings.Columns.Add("Leavestatus").Visible = false;
                    settings.Columns.Add("Workhours").Visible = false;
                }
                else
                {
                    if (ReportDetails.Rptid == 12)
                    {


                        //settings.Columns.Add(column =>
                        //{
                        //    column.FieldName = "Grpname";
                        //    column.Caption = "";
                        //    column.GroupIndex = 0;

                        //    column.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        //    column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        //    column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left;
                        //});

                        settings.Columns.Add(column =>
                        {
                            column.FieldName = "Late_Since";
                            column.Caption = "Late In Since";
                            column.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                            if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                            {
                                column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("mm", "MM");
                            }
                            else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                            {
                                column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("M", "MMM");
                            }
                            //column.PropertiesEdit.DisplayFormatString = isdateformat;
                            column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                            column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        });
                        settings.Columns.Add(column =>
                        {
                            column.FieldName = "Late_Days";
                            column.Caption = "Late In Days";
                            column.UnboundType = DevExpress.Data.UnboundColumnType.String;
                            column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                            column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        });
                        settings.Columns.Add(column =>
                        {
                            column.FieldName = "Late_Hours";
                            column.Caption = "Late In Hours";
                            column.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                            column.PropertiesEdit.DisplayFormatString = "HH:mm";
                            column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                            column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        });
                    }
                    else if (ReportDetails.Rptid == 13)
                    {
                        settings.Columns.Add(column =>
                        {
                            column.FieldName = "Early_Since";
                            column.Caption = "Early Out Since";
                            column.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                            if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                            {
                                column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("mm", "MM");
                            }
                            else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                            {
                                column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("M", "MMM");
                            }
                            //column.PropertiesEdit.DisplayFormatString = isdateformat;
                            column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                            column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        });
                        settings.Columns.Add(column =>
                        {
                            column.FieldName = "Early_Days";
                            column.Caption = "Early Out Days";
                            column.UnboundType = DevExpress.Data.UnboundColumnType.String;

                            column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                            column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        });
                        settings.Columns.Add(column =>
                        {
                            column.FieldName = "Early_Hours";
                            column.Caption = "Early Out Hours";
                            column.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                            column.PropertiesEdit.DisplayFormatString = "HH:mm";
                            column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                            column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        });
                    }
                    else if (ReportDetails.Rptid == 14)
                    {
                        settings.Columns.Add(column =>
                        {
                            column.FieldName = "Absent_Since";
                            column.Caption = "Absent Since";
                            column.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                            if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
                            {
                                column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("mm", "MM");
                            }
                            else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
                            {
                                column.PropertiesEdit.DisplayFormatString = isdateformat.Replace("M", "MMM");
                            }
                            //column.PropertiesEdit.DisplayFormatString =isdateformat;
                            column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                            column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        });
                        settings.Columns.Add(column =>
                        {
                            column.FieldName = "Absent_Days";
                            column.Caption = "Absent Days";
                            column.UnboundType = DevExpress.Data.UnboundColumnType.String;
                            column.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                            column.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                        });
                    }

                }

                settings.Columns.Add("BranchName", "Branch");

                //settings.Columns.Add("CompanyName");
                //settings.SettingsText.Title = ReportDetails.RptTitle;
                settings.Settings.ShowTitlePanel = true;
                settings.Settings.ShowFooter = true;
                settings.SettingsExport.PageFooter.Center = "W = Weekly Off / A = Absent / H = Holiday / LH = Less Hours / HD = Half Day / PW = Present On WeekOff Day / PH = Present On Holiday / PHW = Present On Holiday & WeekOff Day / XX = Not Applicable / CL = Casaul Leave / HCL = Half Casaul Leave";
                settings.SettingsExport.Styles.Footer.Font.Size = 4;
                //settings.SettingsExport.PageFooter.Left = DateTime.Now.ToString("dd-MM-yyyy hh:mm");
                //settings.SettingsExport.PageFooter.Right = "www.mantratec.com";




                #endregion
            }
            return settings;
        }

        private static GridViewSettings CustExportReport()
        {
            GridViewSettings settings = new GridViewSettings();
            var isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            #region CustReport
            settings.Name = ReportDetails.RptName;
            if (isdateformat == "dd-mm-yyyy" || isdateformat == "mm-dd-yyyy" || isdateformat == "yyyy-mm-dd")
            {
                settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString(isdateformat.Replace("mm", "MM")) + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString(isdateformat.Replace("mm", "MM")) + ")";
            }
            else if (isdateformat == "dd-M-yyyy" || isdateformat == "M-dd-yyyy" || isdateformat == "yyyy-M-dd")
            {
                settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString(isdateformat.Replace("M", "MMM")) + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString(isdateformat.Replace("M", "MMM")) + ")";
            }
            //settings.SettingsExport.PageHeader.Right = ReportDetails.RptTitle + " (" + Convert.ToDateTime(ReportDetails.Fdate).ToString("dd-MM-yyyy") + " to " + Convert.ToDateTime(ReportDetails.Tdate).ToString("dd-MM-yyyy") + ")";
            settings.SettingsExport.PageHeader.Left = ReportDetails.cname.ToUpper();
            settings.SettingsExport.PageHeader.Font.Size = 12;

            settings.Settings.ShowTitlePanel = true;
            settings.Settings.ShowFooter = true;
            settings.SettingsExport.PageFooter.Center = "W = Weekly Off / A = Absent / H = Holiday / LH = Less Hours / HD = Half Day / PW = Present On WeekOff Day / PH = Present On Holiday / PHW = Present On Holiday & WeekOff Day / XX = Not Applicable / CL = Casaul Leave / HCL = Half Casaul Leave";
            settings.SettingsExport.Styles.Footer.Font.Size = 4;
            #endregion
            return settings;
        }
        public static DataTable ToDataTable<T>(this IEnumerable<T> items)
        {
            // Create the result table, and gather all properties of a T
            DataTable table = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Add the properties as columns to the datatable
            foreach (var prop in props)
            {
                Type propType = prop.PropertyType;

                // Is it a nullable type? Get the underlying type
                if (propType.IsGenericType && propType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    propType = new NullableConverter(propType).UnderlyingType;

                table.Columns.Add(prop.Name, propType);
            }

            // Add the property values per T as rows to the datatable
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                    values[i] = props[i].GetValue(item, null);

                table.Rows.Add(values);
            }

            return table;
        }
        #endregion
    }
    public static class ReportDetails
    {
        public static int Rptid { get; set; }
        public static string RptName { get; set; }
        public static string RptTitle { get; set; }
        public static string Fdate { get; set; }
        public static string Tdate { get; set; }
        public static string cname { get; set; }
        public static string bname { get; set; }
    }
    public static class ReportStartEndday
    {
        public static int Fday { get; set; }
        public static int Lday { get; set; }

    }
}