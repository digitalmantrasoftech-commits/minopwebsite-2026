using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PayTimeWebClient
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //-Common-URL----------------------------------------------------------------------

            routes.MapRoute("AboutUs", "About-Us", new { controller = "PayTime", action = "AboutUs" });
            routes.MapRoute("ContactUs", "Contact-Us", new { controller = "PayTime", action = "ContactUs" });

            routes.MapRoute("DownloadApp", "Download-App", new { controller = "PayTime", action = "DownloadApp" });
            routes.MapRoute("SupportRequest", "Support-Request", new { controller = "PayTime", action = "SupportRequest" });
            routes.MapRoute("UserGuide", "User-Guide", new { controller = "PayTime", action = "UserGuide" });
            routes.MapRoute("FAQ", "FAQ", new { controller = "PayTime", action = "FAQ" });
            routes.MapRoute("TroubleshootingVideo", "TroubleshootingVideo", new { controller = "PayTime", action = "TroubleshootingVideo" });

            routes.MapRoute("TermsofUse", "Terms-of-Use", new { controller = "PayTime", action = "TermsofUse" });
            routes.MapRoute("PrivacyPolicy", "Privacy-Policy", new { controller = "PayTime", action = "PrivacyPolicy" });
            routes.MapRoute("Sitemap", "Sitemap", new { controller = "PayTime", action = "Sitemap" });

            routes.MapRoute("CancellationandRefundPolicy", "Cancellation-and-Refund-Policy", new { controller = "PayTime", action = "CancellationandRefundPolicy" });

            //-End-Common-URL----------------------------------------------------------------------



            //-Attendance----------------------------------------------------------------------

            routes.MapRoute("Attendance", "Attendance", new { controller = "Attendance", action = "Index" });

            //-End-Edtech----------------------------------------------------------------------

            routes.MapRoute("Edtechstudentattendancemanagementsystem", "Edtech-student-attendance-management-system", new { controller = "Edtechstudentattendancemanagementsystem", action = "Index" });

            //-End-API----------------------------------------------------------------------

            routes.MapRoute("AttendanceAPI", "Attendance-API", new { controller = "AttendanceAPI", action = "Index" });

            //-End-API----------------------------------------------------------------------

            routes.MapRoute("Payroll-Software", "Payroll-Software", new { controller = "PayRoll", action = "Index" });

            //-Features----------------------------------------------------------------------

            routes.MapRoute("AttendanceFeature", "Attendance-Feature", new { controller = "Feature", action = "AttendanceFeature" });
            routes.MapRoute("LeaveManagement", "Attendance-Feature/Leave-Management", new { controller = "Feature", action = "LeaveManagement" });
            routes.MapRoute("AttendanceManagement", "Attendance-Feature/Attendance-Management", new { controller = "Feature", action = "AttendanceManagement" });
            routes.MapRoute("EmployeeSelfService", "Attendance-Feature/Employee-Self-Service", new { controller = "Feature", action = "EmployeeSelfService" });
            routes.MapRoute("MobileAppForAttendance", "Attendance-Feature/Mobile-App-For-Attendance", new { controller = "Feature", action = "MobileAppForAttendance" });
            routes.MapRoute("OvertimeManagement", "Attendance-Feature/Overtime-Management", new { controller = "Feature", action = "OvertimeManagement" });

            routes.MapRoute("PayrollFeature", "Payroll-Feature", new { controller = "Feature", action = "PayrollFeature" });
            routes.MapRoute("PayrollManagementforFastEmployeeOnboarding", "Payroll-Feature/Payroll-Management-for-Fast-Employee-Onboarding", new { controller = "Feature", action = "PayrollManagementforFastEmployeeOnboarding" });
            routes.MapRoute("PayrollManagementSystemforPowerfulAdministration", "Payroll-Feature/Payroll-Administration", new { controller = "Feature", action = "PayrollManagementSystemforPowerfulAdministration" });
            routes.MapRoute("PayrollSystemMakesEffortlessPayrollProcessing", "Payroll-Feature/Payroll-Processing", new { controller = "Feature", action = "PayrollSystemMakesEffortlessPayrollProcessing" });
            routes.MapRoute("SecuredEmployeeSelfServicePortal", "Payroll-Feature/Employee-Self-Service-Payroll", new { controller = "Feature", action = "SecuredEmployeeSelfServicePortal" });
            routes.MapRoute("HRPayrollEnsureComplianceandCustomeReports", "Payroll-Feature/HR-Payroll-Software", new { controller = "Feature", action = "HRPayrollEnsureComplianceandCustomeReports" });

            routes.MapRoute("EdtechFeature", "Edtech-Feature", new { controller = "Feature", action = "EdtechFeature" });

            //-Pricing----------------------------------------------------------------------

            routes.MapRoute("Pricing", "Pricing", new { controller = "PayTime", action = "Pricing" });

            //-Devices----------------------------------------------------------------------

            routes.MapRoute("Devices", "Devices", new { controller = "Devices", action = "Index" });
            routes.MapRoute("FaceAttendanceMachinembioFM01", "Devices/Face-Attendance-Machine-mbio-FM01", new { controller = "Devices", action = "FaceAttendanceMachinembioFM01" });
            routes.MapRoute("FaceBiometricMachineBionicF5", "Devices/Face-Biometric-Machine-Bionic-F5", new { controller = "Devices", action = "FaceBiometricMachineBionicF5" });
            routes.MapRoute("FaceReaderAttendanceMachineBionicF7", "Devices/Face-Reader-Attendance-Machine-Bionic-F7", new { controller = "Devices", action = "FaceReaderAttendanceMachineBionicF7" });
            routes.MapRoute("FaceRecognitionTimeAttendanceMachinebioNICFX9", "Devices/Face-Recognition-Time-Attendance-Machine-BioNIC-FX9", new { controller = "Devices", action = "FaceRecognitionTimeAttendanceMachinebioNICFX9" });

            routes.MapRoute("Cart", "Devices/Cart", new { controller = "Devices", action = "Cart" });
            routes.MapRoute("AddressInformation", "Devices/Address-Information", new { controller = "Devices", action = "AddressInformation" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "PayTime", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
