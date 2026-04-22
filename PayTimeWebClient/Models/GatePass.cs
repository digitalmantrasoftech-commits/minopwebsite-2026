using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class GatePass
    {
        public int GatePassIssueID { get; set; }
        public string OutTime { get; set; }
        public int EmpId { get; set; }
        public string EmployeeName { get; set; }
        public string EmpCode { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string PunchID { get; set; }
        public string CurrentShiftName { get; set; }
        public string INTime { get; set; }
        public DateTime? AppliedDate { get; set; }
        public string AppliedTime { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public string SrNo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ApprovedByName { get; set; }
    }

    public class GatePassAppliedPolicy
    {
        public string EmployeeName { get; set; }
        public string EmpCode { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string PolicyName { get; set; }
        public int PolicyCount { get; set; }
        public int GatePassIssueCount { get; set; }
        public int ApproveCount { get; set; }
        public int RejectCount { get; set; }
        public DateTime? AppliedDate { get; set; }
    }

    public class FastTrackDailyInOutPunchingReport
    {
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string DeviceSrNo { get; set; }
        public string DeviceName { get; set; }
        public string MachineCode { get; set; }  
        public string PunchTime { get; set; }
        public string PunchDifference { get; set; }
        public int TotalPunch { get; set; }
        public int PunchID { get; set; }
        public DateTime? AttnDt { get; set; }
    }

    public class FastTrackMonthlyAttendanceReport
    {
    }

    public class FastTrackEmployeePunchComparisonReport
    {
        public DateTime AttnDt { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string EmpCode { get; set; }
        public string RFIDCard { get; set; }
        public string IDCardNumber { get; set; }
        public string EmpName { get; set; }
        public string EmployeeType { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Shift { get; set; }
        public string DeviceCode { get; set; }
        public int DeviceId { get; set; }
        public string InTime { get; set; }
        public string InTime1 { get; set; }
        public string PunchDifference { get; set; }
        public string OutTime { get; set; }
        public string OutTime1 { get; set; }
        public string TotalHours { get; set; }
        public string LateIn { get; set; }
        public string EarlyOut { get; set; }
        public string Status { get; set; }
    }

    public class AttendanceCorrectionReport
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string EmpPunchID { get; set; }
        public DateTime AttnDate { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string ShiftName { get; set; }
        public string FinalStatus { get; set; }
        public string TotHour { get; set; }
        public string OTHr { get; set; }
        public string CorrectionBy { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }

    }

}