using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayTimeWebClient.Controllers
{
    public class FeatureController : Controller
    {
        //
        // GET: /Feature/
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //Attendance Features------------------------------------------------------------------------------
        public ActionResult AttendanceFeature()
        {
            return View("~/Views/Feature/AttendanceFeature/AttendanceFeature.cshtml");
        }
        public ActionResult LeaveManagement()
        {
            return View("~/Views/Feature/AttendanceFeature/LeaveManagement.cshtml");
        }
        public ActionResult AttendanceManagement()
        {
            return View("~/Views/Feature/AttendanceFeature/AttendanceManagement.cshtml");
        }
        public ActionResult EmployeeSelfService()
        {
            return View("~/Views/Feature/AttendanceFeature/EmployeeSelfService.cshtml");
        }
        public ActionResult MobileAppForAttendance()
        {
            return View("~/Views/Feature/AttendanceFeature/MobileAppForAttendance.cshtml");
        }
        public ActionResult OvertimeManagement()
        {
            return View("~/Views/Feature/AttendanceFeature/OvertimeManagement.cshtml");
        }

        //Payroll Features------------------------------------------------------------------------------
        public ActionResult PayrollFeature()
        {
            return View("~/Views/Feature/PayrollFeature/PayrollFeature.cshtml");
        }
        public ActionResult PayrollManagementforFastEmployeeOnboarding()
        {
            return View("~/Views/Feature/PayrollFeature/PayrollManagementforFastEmployeeOnboarding.cshtml");
        }
        public ActionResult PayrollManagementSystemforPowerfulAdministration()
        {
            return View("~/Views/Feature/PayrollFeature/PayrollManagementSystemforPowerfulAdministration.cshtml");
        }
        public ActionResult PayrollSystemMakesEffortlessPayrollProcessing()
        {
            return View("~/Views/Feature/PayrollFeature/PayrollSystemMakesEffortlessPayrollProcessing.cshtml");
        }
        public ActionResult SecuredEmployeeSelfServicePortal()
        {
            return View("~/Views/Feature/PayrollFeature/SecuredEmployeeSelfServicePortal.cshtml");
        }
        public ActionResult HRPayrollEnsureComplianceandCustomeReports()
        {
            return View("~/Views/Feature/PayrollFeature/HRPayrollEnsureComplianceandCustomeReports.cshtml");
        }

        //Edtech Features------------------------------------------------------------------------------
        public ActionResult EdtechFeature()
        {
            return View("~/Views/Feature/EdtechFeature/EdtechFeature.cshtml");
        }

        //Attendance-API Features------------------------------------------------------------------------------

    }
}