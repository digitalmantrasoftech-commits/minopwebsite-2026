using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class OnDutyLeave
    {
        #region Properties
        public Int64 OnDutyLeaveId { get; set; }
        public int EmpCode { get; set; }
        public int LeaveType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string FromDateStr { get; set; }
        public string ToDateStr { get; set; }
        public string Reason { get; set; }
        public Int16 LeavePaid { get; set; }
        public Int16 IsHalfLeave { get; set; }
        public string EmpName { get; set; }
        public string LeaveTypeName { get; set; }
        public int LeaveDays { get; set; }
        #endregion
    }
    public class ResOndutyLeave
    {
        public decimal LeaveBalance { get; set; }
        public decimal EncashLeaveBalance { get; set; }
        public decimal Total { get; set; }
        public bool IsActive { get; set; }

        public string Msg { get; set; }
    }
    public class Leave
    {
        public int LeaveId { get; set; }
        public int EmpId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ApplyReason { get; set; }
        public int LeaveStatus { get; set; }
        public string ApprovalReason { get; set; }
        public string EmpName { get; set; }
        public string EmpCode { get; set; }
        public string EmpPhoto { get; set; }
        public string LeaveTypeName { get; set; }
        public string Reason { get; set; }
        public string FromDate1 { get; set; }
        public string ToDate1 { get; set; }
        public int LeavePaid { get; set; }
        public Boolean IsHalfLeave { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifyBy { get; set; }
        public DateTime ModifyDate { get; set; }
        public string LeaveDoc { get; set; }
        public string Location { get; set; }
        public string LocAddress { get; set; }

        public int LeaveTypeDuration { get; set; }
    }
    public class GetLeaveListAll
    {
        public int BranchID { get; set; }
        public int cmpid { get; set; }
        public int Brchid { get; set; }
        public string RoleId { get; set; }
        public string Empid { get; set; }
        public int selectAll { get; set; }
        public string selectAllSearchTerm { get; set; }
    }
}