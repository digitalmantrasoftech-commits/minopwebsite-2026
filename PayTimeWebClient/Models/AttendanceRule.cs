using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class AttendanceRule
    {
        public int AttendanceRuleId { get; set; }
        public  int EmpId { get; set; }
        public int AllErrorCase { get; set; }
        public int ErrorCaseWH { get; set; }
        public int HdLate { get; set; }
        public int HdEd { get; set; }
        public int HdLateorEd { get; set; }
        public int ALate { get; set; }
        public int AED { get; set; }
        public int ALateorED { get; set; }
        public int TotalHrafterStatus { get; set; }
        public int TotalHrbeforeStatus { get; set; }
        public int LateInMin { get; set; }
        public int EDMin { get; set; }
        public int[] empidlst { get; set; }
        public bool IsActive { get; set; }

        public string AttCorrectionId { get; set; }        
        public DateTime attn_dt { get; set; }
        public string InPunchTime { get; set; }
        public string OutPunchTime { get; set; }
        public string ApplyReason { get; set; }
        public string ApprovalReason { get; set; }
        public string Status { get; set; }
        public IEnumerable<AttendanceRule> attndRules { get; set; }
    }
    public class UpdateAttendance
    {
        public string empids { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
    }

    public class AttendanceCorrectionreq    
    {
        public int AttCorrectionId { get; set; }
        public int EmpId { get; set; }
        public string attn_dt { get; set; }
        public string InPunchTime { get; set; }
        public string OutPunchTime { get; set; }
        public string ApplyReason { get; set; }
        public string ApprovalReason { get; set; }
        public int Status { get; set; }
        public int PunchID { get; set; }

    }

    public class AttanList
    {
        //string RoleID, string loginemployeeid, string Month, string Year, string status, string Employee
        #region AttanList
        public string BUId { get; set; }
        public string RoleID { get; set; }
        public string loginemployeeid { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string status { get; set; }
        public string Employee { get; set; }

        public string Date { get; set; }

        public string brid { get; set; }
        public string cmpid { get; set; }
        public string deptId { get; set; }
        public string desgId { get; set; }
        public string shiftId { get; set; }
        public int SelectAll { get; set; } = 0;
        public string SelectAllSearchTerm { get; set; } = string.Empty;

        #endregion
    }

    #region For FastTrackDataCorrection List
    public class AttendanceRecord
    {
        public int EmpID { get; set; }
        public string EmpName { get; set; }
        public string CardNo { get; set; }
        public string Att_dt { get; set; }
        public string Shift { get; set; }
        public string Status { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string TotHour { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool isNight { get; set; }
        public string ChangeReq { get; set; }
        public string Day { get; set; }
        public string ApprovedBy { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmployeeType { get; set; }
        public string RFIDCard { get; set; }
        public string IDCardNumber { get; set; }

        public string EmpCode { get; set; }
        public string EmpPunchID { get; set; }

    }
    #endregion
}