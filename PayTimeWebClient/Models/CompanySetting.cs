using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class CompanySetting
    {
        public int SettingId { get; set; }
        public string SettingName { get; set; }
        public int CompanyId { get; set; }
        public bool HasMultipleCompany { get; set; }
        public bool HasMultipleBranch { get; set; }
        public bool HasDepartment { get; set; }
        public bool HasDesignation { get; set; }
        public bool HasCatagory { get; set; }
        public bool HasType { get; set; }
        public bool HasGrade { get; set; }
        public bool HasShift { get; set; }
        public bool HasShiftGroup { get; set; }
        public bool HasHoliday { get; set; }
        public bool HasLeave { get; set; }
        public bool HasReligion { get; set; }
        public bool HasSMS { get; set; }
        public int MonthlyRptStaDay { get; set; }
        public int ServiceStartTime { get; set; }
        public int SchedularStartTime { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<CompanySetting> CompanySettinglist { get; set; }
        public bool IsLocation { get; set; }
        public bool IsEss { get; set; }
        public bool IsMorxAutoSync { get; set; }
        public bool IsFace { get; set; }
        public bool IsBeacon { get; set; }
        public bool IsBranchGeoFence { get; set; }
        public string ISDateFormat { get; set; }
        public bool IsApprovalForWebpunch { get; set; }
        public bool IsDeviceTimeZone { get; set; }
        public string CountryCode { get; set; }
        public string AreaCode { get; set; }
        public bool IsVerification { get; set; }
        public bool IsFlexi { get; set; }
        public bool IsOTP { get; set; }
        public string lockAttendanceDay { get; set; }
        public string AttendanceCorrection { get; set; }
        public int CurrancyType { get; set; }
        public int OptHolidayLimit { get; set; }
        public string  DaysLimitForAttendanceApproval { get; set; }

        public bool IsGatePassActivate { get; set; }

    }
}