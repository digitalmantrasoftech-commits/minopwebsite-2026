using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class HRPolicy
    {
        #region Properties
        public int PolicyId { get; set; }
        public string PolicyName { get; set; }
        public int EmpWeekOff { get; set; }
        public int EmpSecondWeekOff { get; set; }
        public string EmpSecondWeekOffRule { get; set; }
        public string[] emptempweekoff { get; set; }
        public int EmpHalfDay { get; set; }

        public string EmpHalfDayRule { get; set; }
        public string[] emphalfdayoff { get; set;}

        public int EmpAllowOT { get; set; }
        public int MaxOutDur { get; set; }
        public int TimeBetPunch { get; set; }
        public int GraceLateIN { get; set; }
        public int GraceEarlyOUT { get; set; }
        public string ApplicationMode { get; set; }
        public int MonthlyRptStaDay { get; set; }
        public int WeekOffOTHour { get; set; }
        public int HolidayOffOTHour { get; set; }
        public int OTMinHour { get; set; }
        public int OTFormula { get; set; }
        public int OTAllowWO { get; set; }
        public int OTAllowHO { get; set; }
        public int NotPresentInMnth { get; set; }
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
        public int IsActive { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int ModifyBy { get; set; }
        public string ModifyDate { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int roleid { get; set; }
        public int Issedwrule { get; set; }
        public int IssedwruleType { get; set; }

        public int EarlyInoutMin { get; set; }
        public int EarlyInoutday { get; set; }
        //  public int EmpID { get; set; }
        //public string CompanyName { get; set; }
        //public string BranchName { get; set; }
        // public HRPolicy[] hrarray { get; set; }
        public IEnumerable<HRPolicy> HRPolicylist { get; set; }
        #endregion
    }

    public class Leads
    {
        #region Properties
        public string SourceUrl { get; set; }
        public string SourceTitle { get; set; }
        public string SourceType { get; set; }
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Comments { get; set; }
        public string Subject { get; set; }
        public string _captch { get; set; }
        #endregion
    }
    public class Cfc
    {
        public int frmid { get; set; }
        public string frmname { get; set; }
        public string frmdisplayname { get; set; }
        public string frmdata { get; set; }
        public int frmoperationid { get; set; }
        public int frmoperationparentid { get; set; }
        public string frmtblname { get; set; }
        public string frmcreatedate { get; set; }
        public string frmCustomField { get; set; }
    }
    public class Mnuoperation
    {
        public int OperationId { get; set; }
        public string OperationName { get; set; }
        public string OperationIds { get; set; }
        
    }
    public class Editcustomform
    {
        public int frmid { get; set; }
        public string frmdisplayname { get; set; }
        public string frmdata { get; set; }
        public int frmoperationparentid { get; set; }
        public string tblname { get; set; }
    }
    public class Getformdatareq
    {
        public string frmtblname { get; set; }
        
    }
    public class Autocomplreq
    {
        public string searchfrom { get; set; }
        public string searchval { get; set; }
        public string tblname { get; set; }
        public string selectedBU { get; set; }


    }
    public class Genriclist
    {
        public string label { get; set; }
        public string value { get; set; }
    }
    public class Genriclists
    {
        public string label { get; set; }
        public int value { get; set; }
    }

    public class AttendanceDetails
    {
        public int EmpID { get; set; }
        
    }
    public class PayrollProcess
    {
        public int EmpID { get; set; }
        public string Empcode { get; set; }
        public string EmpName { get; set; }
        public string IsActive { get; set; }
    }

}