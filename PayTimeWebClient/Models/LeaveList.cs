using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class LeaveList
    {
        public int LeaveId { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ApplyReason { get; set; }
        public int LeaveStatus { get; set; }
        public string LeaveStatusName { get; set; }
        public string ApprovalReason { get; set; }
        public string ActionBy { get; set; }

        public string Created { get; set; }
        public int LeavePaid { get; set; }
        public bool IsHalfLeave { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDates { get; set; }
        public int ModifyBy { get; set; }
        public DateTime ModifyDate { get; set; }
        public string EmpCode { get; set; }
        public string EmpEmail { get; set; }
        public string ReportingEmail { get; set; }
        public string CompanyEmail { get; set; }
        public string ApprovalDate { get; set; }
        public string ReportingName { get; set; }
        public string LeaveDoc { get; set; }
        public string Location { get; set; }
        public string LocAddress { get; set; }
        public int LeaveTypeDuration { get; set; }
        public IEnumerable<LeaveList> LeaveLists { get; set; }

        public string EmpPhoto { get; set; }


    }
    public class LeaveResponse
    {
        public string Message { get; set; }
        public int LeaveId { get; set; }
        public string Meg { get; set; }
    }

    public class AttedanceResponse
    {
        public string Message { get; set; }
        public string Meg { get; set; }
        public int OnCorrectionID { get; set; }
    }
}