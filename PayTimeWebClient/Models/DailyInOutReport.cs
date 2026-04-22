using System;
namespace PayTimeWebClient.Models
{
    public class DailyInOutReport
    {
        public virtual int BranchId { get; set; }
        public virtual int CompanyID { get; set; }
        public virtual string BranchName { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual int EmpID { get; set; }
        public virtual string Name { get; set; }
        public virtual string EmpName { get; set; }
        public virtual string CardNo { get; set; }
        public virtual DateTime Attn_dt { get; set; }
        public virtual string DepartmentName { get; set; }
        public virtual string DesignationName { get; set; }
        public virtual string ShiftName { get; set; }
        public virtual string ShiftStartTime { get; set; }
        public virtual string ShiftEndTime { get; set; }
        public virtual string FinalStatus { get; set; }
        public virtual string TotHour { get; set; }
        public virtual string LessHr { get; set; }
        public virtual string LateHr { get; set; }
        public virtual string EarlyHr { get; set; }
        public virtual string OTHr { get; set; }
        public virtual string InTime { get; set; }
        public virtual string OutTime { get; set; }
        public virtual string LateOUT { get; set; }
        public virtual string EarlyIN { get; set; }
        public virtual DateTime InTimeFull { get; set; }
        public virtual DateTime OutTimeFull { get; set; }
        public virtual int HalfLeavePresent { get; set; }
        public virtual string Shift { get; set; }
        public virtual string In_Device { get; set; }
        public virtual string Out_Device { get; set; }
        public virtual DateTime Late_Since { get; set; }
        public virtual string Late_Days { get; set; }
        public virtual DateTime Late_Hours { get; set; }
        public virtual DateTime Early_Since { get; set; }
        public virtual string Early_Days { get; set; }
        public virtual DateTime Early_Hours { get; set; }
        public virtual DateTime Absent_Since { get; set; }
        public virtual string Absent_Days { get; set; }
        public virtual string Punch { get; set; }
        public virtual int Punchid { get; set; }
        public virtual DateTime Attndt { get; set; }
        public virtual DateTime AttnDate { get; set; }
        public virtual string Location { get; set; }
        public virtual string Grpname { get; set; }
        public virtual string Empcode { get; set; }
        public virtual string InsertedBy { get; set; }
        public virtual string DeviceCode { get; set; }
        public virtual string DeviceName { get; set; }
        public virtual string DeviceIP { get; set; }
        public virtual int DeviceId { get; set; }
        public virtual string CompanyAddress { get; set; }
        public string PFromDate { get; set; }
        public string PTodate { get; set; }
        public string SumHrs { get; set; }
        public string Week { get; set; }
        public string WeekSumHrs { get; set; }
        public string Weekhrs { get; set; }
        public string Month { get; set; }
        public string TotalSumHours { get; set; }
        public string OTHrs { get; set; }
        public string WorkingHours { get; set; }
        public string Status { get; set; }
        public string EntryMode { get; set; }
        public string Leavestatus { get; set; }
        public string Workhours { get; set; }
        public string EmpMNo { get; set; }

    }
    public class DailyInOutReportCustom
    {
        public string Empcode { get; set; }
        public string EmpName { get; set; }
        public string CardNo { get; set; }
        public string BranchName { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string ShiftName { get; set; }
        public string ShiftStartTime { get; set; }
        public string ShiftEndTime { get; set; }
        public string Attn_dt { get; set; }
        public string FinalStatus { get; set; }
        public string TotHour { get; set; }
        public string LessHr { get; set; }
        public string LateHr { get; set; }
        public string EarlyHr { get; set; }
        public string OTHr { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string LateOUT { get; set; }
        public string EarlyIN { get; set; }
        public int HalfLeavePresent { get; set; }
        public DateTime InTimeFull { get; set; }
        public DateTime OutTimeFull { get; set; }
    }

    public class WeeklyInOutReport
    {
        public virtual int BranchId { get; set; }
        public virtual int CompanyID { get; set; }
        public virtual string BranchName { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual int EmpID { get; set; }
        public virtual string Name { get; set; }
        public virtual string EmpName { get; set; }
        public virtual string CardNo { get; set; }
        public virtual DateTime Attn_dt { get; set; }
        public virtual string DepartmentName { get; set; }
        public virtual string DesignationName { get; set; }
        public virtual string ShiftName { get; set; }
        public virtual string ShiftStartTime { get; set; }
        public virtual string ShiftEndTime { get; set; }
        public virtual string FinalStatus { get; set; }
        public virtual string TotHour { get; set; }
        public virtual string LessHr { get; set; }
        public virtual string LateHr { get; set; }
        public virtual string EarlyHr { get; set; }
        public virtual string OTHr { get; set; }
        public virtual string InTime { get; set; }
        public virtual string OutTime { get; set; }
        public virtual string LateOUT { get; set; }
        public virtual string EarlyIN { get; set; }
        public virtual DateTime InTimeFull { get; set; }
        public virtual DateTime OutTimeFull { get; set; }
        public virtual int HalfLeavePresent { get; set; }
        public virtual string Shift { get; set; }
        public virtual string In_Device { get; set; }
        public virtual string Out_Device { get; set; }
        public virtual DateTime Late_Since { get; set; }
        public virtual string Late_Days { get; set; }
        public virtual DateTime Late_Hours { get; set; }
        public virtual DateTime Early_Since { get; set; }
        public virtual string Early_Days { get; set; }
        public virtual DateTime Early_Hours { get; set; }
        public virtual DateTime Absent_Since { get; set; }
        public virtual string Absent_Days { get; set; }
        public virtual string Punch { get; set; }
        public virtual int Punchid { get; set; }
        public virtual DateTime Attndt { get; set; }
        public virtual DateTime AttnDate { get; set; }
        public virtual string Location { get; set; }
        public virtual string Grpname { get; set; }
        public virtual string Empcode { get; set; }
        public virtual string InsertedBy { get; set; }
        public virtual string DeviceCode { get; set; }
        public virtual string DeviceName { get; set; }
        public virtual string DeviceIP { get; set; }
        public virtual int DeviceId { get; set; }
        public virtual string CompanyAddress { get; set; }
        public string PFromDate { get; set; }
        public string PTodate { get; set; }
        public string SumHrs { get; set; }
    }
    public class Reporttemplate
    {
        #region Properties
        public int ReportId { get; set; }
        public string ReportType { get; set; }
        public string ReportName { get; set; }
        public string ReportHeaderName { get; set; }
        public int Rqportcreatedby { get; set; }
        public DateTime Reportcreateddate { get; set; }
        public int Rqportmodifyby { get; set; }
        public DateTime Reportmodifydate { get; set; }
        public string fieldList { get; set; }
        #endregion
    }

    public class Summaryreport
    {
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public DateTime Attn_dt { get; set; }
        public string TotalEmployeeCount { get; set; }
        public string Present { get; set; }
        public string AttAbsent { get; set; }
        public string AttLeave { get; set; }
        public string WeeklyOff { get; set; }
        public string Holiday { get; set; }
        public string AttError { get; set; }
        public string EarlyInHrs { get; set; }
        public string EarlyInCount { get; set; }
        public virtual string CompanyAddress { get; set; }
        public string PFromDate { get; set; }
        public string PTodate { get; set; }

    }

    public class GetFacePunchdata
    {
        public string Empcode { get; set; }
        public string EmpName { get; set; }
        public string BranchName { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public DateTime PunchDate { get; set; }
        public DateTime PunchTime { get; set; }
        public string Location { get; set; }
        public string EmpPhoto { get; set; }
        public string EntryMode { get; set; }
        public string Flglocation { get; set; }
    }
    public class GetFaceFilodetaildata
    {
        public string Empcode { get; set; }
        public string EmailID { get; set; }
        public string EmpName { get; set; }
        public string BranchName { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public DateTime PunchDate { get; set; }
        public string INTime { get; set; }
        public string OutTime { get; set; }
        public string Login_hours { get; set; }
        public string In_location { get; set; }
        public string Out_Location { get; set; }
        public string Flglocation { get; set; }
        public string Instatus { get; set; }
        public string Outstatus { get; set; }
        public string IN_letlog { get; set; }
        public string Out_letlog { get; set; }
        public string EmpPhoto { get; set; }
        public string EmpMNo { get; set; }
    }

    public class GetPunchwisereport
    {
        public string Empid { get; set; }
        public string FromDate { get; set; }
        public string Todate { get; set; }

        public string Empcode { get; set; }
        public string EmpName { get; set; }
        public string DepartmentName { get; set; }
        public string Punchid { get; set; }
        public string Attn_Dt { get; set; }


        public string Punch_IN_1 { get; set; }
        public string Punch_OUT_1 { get; set; }

        public string Punch_IN_2 { get; set; }
        public string Punch_OUT_2 { get; set; }
        public string Punch_IN_3 { get; set; }
        public string Punch_OUT_3 { get; set; }
        public string Punch_IN_4 { get; set; }
        public string Punch_OUT_4 { get; set; }
        public string Punch_IN_5 { get; set; }
        public string Punch_OUT_5 { get; set; }
        public string Punch_IN_6 { get; set; }
        public string Punch_OUT_6 { get; set; }
        public string Punch_IN_7 { get; set; }
        public string Punch_OUT_7 { get; set; }
        public string Punch_IN_8 { get; set; }
        public string Punch_OUT_8 { get; set; }
        public string Punch_IN_9 { get; set; }
        public string Punch_OUT_9 { get; set; }
        public string Punch_IN_10 { get; set; }
        public string Punch_OUT_10 { get; set; }
        public string Punch_IN_11 { get; set; }
        public string Punch_OUT_11 { get; set; }
        public string Punch_IN_12 { get; set; }
        public string Punch_OUT_12 { get; set; }
        public string Punch_IN_13 { get; set; }
        public string Punch_OUT_13 { get; set; }
        public string Punch_IN_14 { get; set; }
        public string Punch_OUT_14 { get; set; }
        public string Punch_IN_15 { get; set; }
        public string Punch_OUT_15 { get; set; }
        public string Punch_IN_16 { get; set; }
        public string Punch_OUT_16 { get; set; }
        public string Punch_IN_17 { get; set; }
        public string Punch_OUT_17 { get; set; }
        public string Punch_IN_18 { get; set; }
        public string Punch_OUT_18 { get; set; }
        public string Punch_IN_19 { get; set; }
        public string Punch_OUT_19 { get; set; }
        public string Punch_IN_20 { get; set; }
        public string Punch_OUT_20 { get; set; }
    }

    public class GetMissPunchReport
    {
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string Attn_dt { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string TotHour { get; set; }
        public string FinalStatus { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string EmpPunchID { get; set; }
    }


    public class GetjoinrejoinReport
    {
        public string EmpID { get; set; }
        public string Company { get; set; }
        public string Punch_ID { get; set; }
        public string Name { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Type { get; set; }
        public string Package { get; set; }
        public string Status { get; set; }
        public string StatusName { get; set; }
        public string JoiningDate { get; set; }
    }

    public class LeaveBalanceSummaryReport
    {
        public string Empname { get; set; }
        public string Empcode { get; set; }
        public decimal LeaveBalance { get; set; }
        public decimal ConsumeDays { get; set; }
        public decimal Total { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }

        public string DesignationName { get; set; }

        public string leaveType { get; set; }
      }
    //public class DailyInOutReportDeviceName
    //{

    //    public virtual int BranchId { get; set; }
    //    public virtual int CompanyID { get; set; }
    //    public virtual string BranchName { get; set; }
    //    public virtual string CompanyName { get; set; }
    //    public virtual int EmpID { get; set; }
    //    public virtual string Name { get; set; }
    //    public virtual string EmpName { get; set; }
    //    public virtual string CardNo { get; set; }
    //    public virtual DateTime Attn_dt { get; set; }
    //    public virtual string DepartmentName { get; set; }
    //    public virtual string DesignationName { get; set; }
    //    public virtual string ShiftName { get; set; }
    //    public virtual string ShiftStartTime { get; set; }
    //    public virtual string FinalStatus { get; set; }
    //    public virtual string TotHour { get; set; }
    //    public virtual string LessHr { get; set; }
    //    public virtual string LateHr { get; set; }
    //    public virtual string EarlyHr { get; set; }
    //    public virtual string OTHr { get; set; }
    //    public virtual string InTime { get; set; }
    //    public virtual string OutTime { get; set; }
    //    public virtual string LateOUT { get; set; }
    //    public virtual string EarlyIN { get; set; }
    //    public virtual DateTime InTimeFull { get; set; }
    //    public virtual DateTime OutTimeFull { get; set; }
    //    public virtual int HalfLeavePresent { get; set; }
    //    public virtual string Shift { get; set; }
    //    public virtual string In_Device { get; set; }
    //    public virtual string Out_Device { get; set; }
    //    public virtual string CompanyAddress { get; set; }
    //    public string PFromDate { get; set; }
    //    public string PTodate { get; set; }

    //}

    public class reqDailyInOutReport
    {
        public string FromDate { get; set; }
        public string Todate { get; set; }
        public string EmpID { get; set; }
        public string ReportName { get; set; }
        public string CompanyID { get; set; }
        public int RptID { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string OperationName { get; set; }
        public string ParentMenuName { get; set; }
        public string ImgClassName { get; set; }
        public int SeqNo { get; set; }
        public int IsFilo { get; set; }
        public string DepId { get; set; }
        public string BranchId { get; set; }
        public int crptid { get; set; }
        public int ReportID { get; set; }
        public int ActiveEmployee { get; set; }
        public string SearchCriteria { get; set; }

        public int CustRptID { get; set; }
        public string CustReportName { get; set; }

        public int Getfacedetail { get; set; }
    }
    public class reqWeeklyInOutReport
    {
        public string FromDate { get; set; }
        public string Todate { get; set; }
        public string EmpID { get; set; }
        public string ReportName { get; set; }
        public string CompanyID { get; set; }
        public int RptID { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string OperationName { get; set; }
        public string ParentMenuName { get; set; }
        public string ImgClassName { get; set; }
        public int SeqNo { get; set; }
        public int IsFilo { get; set; }
        public string DepId { get; set; }
        public string BranchId { get; set; }
        public int crptid { get; set; }
        public int ReportID { get; set; }
        public int ActiveEmployee { get; set; }
        public string SearchCriteria { get; set; }

        public int CustRptID { get; set; }
        public string CustReportName { get; set; }

    }
    public class CustreportList
    {
        public int CustRptID { get; set; }
        public string ReportName { get; set; }
        public string ReportHeaderName { get; set; }
    }
    public class LeaveBalanceReport
    {
        public int EmpID { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveType { get; set; }
        public decimal Sanction { get; set; }
        public decimal Consume { get; set; }
        public decimal Balance { get; set; }
        public string BalanceDate { get; set; }
        public string CompanyAddress { get; set; }
        public string PFromDate { get; set; }
        public string PTodate { get; set; }
    }

    public class Transactionmonitor
    {
        public string EmpId { get; set; }
        public string Punchid { get; set; }
        public string EmpName { get; set; }
        public string PunchTime { get; set; }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceIP { get; set; }
        public string DeviceType { get; set; }
        public string empcode { get; set; }
        public string Entrydate { get; set; }
    }
    public class DynamicShowReporttemplate
    {
        #region Properties
        public int ReportID { get; set; }
        public string SearchCriteria { get; set; }
        public string ReportHead { get; set; }
        public string Reportflage { get; set; }
        public string ReportmailID { get; set; }
        #endregion
    }
    public class DynamicReportExample
    {
        #region Properties
        public int ReportID { get; set; }
        public string SearchCriteria { get; set; }
        public string ReportHead { get; set; }
        public string Reportflage { get; set; }
        #endregion
    }

    public class SchoolReports
    {

    }
    public class CustomerReportField
    {
        #region Properties
        public int ReportID { get; set; }
        public string SearchCriteria { get; set; }
        public string ReportHead { get; set; }
        public string Reportflage { get; set; }
        public string ReportmailID { get; set; }

        public string FromDate { get; set; }
        public string Todate { get; set; }
        public string EmpID { get; set; }
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string OperationName { get; set; }
        public string ParentMenuName { get; set; }
        public string ImgClassName { get; set; }
        public int SeqNo { get; set; }
        public int IsFilo { get; set; }
        public string DepId { get; set; }
        public string BranchId { get; set; }
        public int crptid { get; set; }

        public int CustRptID { get; set; }
        public string CustReportName { get; set; }
        #endregion
    }
    public class MonthlyFlexiInOutReport
    {
        #region Properties
        public DateTime Attn_Dt { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string TotHour { get; set; }
        public string Name { get; set; }
        public string Week { get; set; }
        public string WeekSumHrs { get; set; }
        public string Weekhrs { get; set; }
        public string FinalStatus { get; set; }
        public string Month { get; set; }
        public string MonthSumHrs { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string WorkingHours { get; set; }
        public string OTHrs { get; set; }

        #endregion
    }

    public class WeeklyFlexiInOutReport
    {
        #region Properties
        public DateTime Attn_Dt { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string TotHour { get; set; }
        public string Name { get; set; }
        public string Week { get; set; }
        public string WeekSumHrs { get; set; }
        public string Weekhrs { get; set; }
        public string FinalStatus { get; set; }
        public string Month { get; set; }
        public string MonthSumHrs { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string WorkingHours { get; set; }
        public string TotalSumHours { get; set; }
        public string DepartmentName { get; set; }
        public string OTHrs { get; set; }



        #endregion
    }


    public class LeaveBalanceStatusReport
    {
        #region Properties
        public string EmpName { get; set; }
        public string EmpCode { get; set; }
        public string LeaveTypeName { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public decimal LeaveSanction { get; set; }
        public decimal ConsumedLeave { get; set; }
        public decimal RemainingLeave { get; set; }
      
        public string LeaveStartDate { get; set; }
        public string LeaveEndDate { get; set; }
        public string ApplicationDate { get; set; }
        public string LeaveStatus { get; set; }
        public string LeavePaid {  get; set; }
        public string Reason { get; set;}
        public string Comments { get; set;}
        #endregion
    }


    public class DailySummaryReport
    {
        #region Properties
        public string Empcode { get; set; }
        public string CompanyName { get; set; }
        public string Email_id { get; set; }
        public string DepartmentName { get; set; }
        public string BranchName { get; set; }
        public string DesignationName { get; set; }
        public string EmpName { get; set; }
        public string Punch_Date { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string Login_hours { get; set; }
        public string IN_letlog { get; set; }
        public string Out_letlog { get; set; }
        public string First_In_Location { get; set; }
        public string Last_Out_Location { get; set; }
        public string KM { get; set; }
        public string Total_Punches { get; set; }
        public string Empphoto { get; set; }
        #endregion
    }


    public class InstallationStatusReport
    {
        #region Properties
        public string Empcode { get; set; }
        public string CompanyName { get; set; }
        public string Email_id { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string BranchName { get; set; }
        public string EmpName { get; set; }
        public string Punch_Date { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string Login_hours { get; set; }
        public string IN_Status_Flag { get; set; }
        public string Out_status_flag { get; set; }
        public string IN_letlog { get; set; }
        public string Out_letlog { get; set; }
        public string In_Location { get; set; }
        public string Out_Location { get; set; }
        public string Empphoto { get; set; }  
        #endregion
    }


    public class FaceWebPunchDetailReport
    {
        #region Properties
        public string Empcode { get; set; }
        public string CompanyName { get; set; }
        public string Email_id { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string BranchName { get; set; }
        public string EmpName { get; set; }
        public string Punch_Date { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string Login_hours { get; set; }
        public string In_status { get; set; }
        public string Out_status { get; set; }
        public string IN_letlog { get; set; }
        public string Out_letlog { get; set; }
        public string In_Location { get; set; }
        public string Out_Location { get; set; }
        public string KM { get; set; }
        public string Punch_Count { get; set; }
        public string EmpMNo { get; set;}
        public string Empphoto { get; set; }


        #endregion
    }


    public class DailyWorkingHoursReport
    {
        public string Attn_Dt { get; set;}
        public int Punchid { get; set;}
        public string In_Time { get; set; }
        public string Out_Time { get; set; }
        public string Duration_HHMMSS { get; set;}
        public string IsTolal { get; set; }
        public string TotHour { get; set;}
        public string finalstatus { get; set;}
        public string Empname { get; set;}
        public string empcode { get; set; }
        public string Emppunchid { get; set; }
        public string DepartmentName { get; set;}

    }

    public class DailyWorkingHoursINOUTReport
    {
        public int EmpID { get; set;}   
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string DepartmentName { get; set; }
        public string BranchName { get; set;}
        public string FromDate {  get; set; }   
        public string ToDate { get; set; }  
        public string WorkingDays { get; set; } 
        public string AllocatedHrs { get; set; }    
        public string WorkingHrs { get; set;}
        public string Difference { get; set; }
        public string ODleavedays { get; set; }
        public string ODHours { get; set; }

        public string WHdays { get; set; }
        public string WHours { get; set; }
        public string HOdays { get; set; }
        public string HOHours { get; set; }
        public string ReportingTo { get; set; }

    }


    public class AbsentReport
    {
        public string Empcode { get; set; }
        public string EmpName { get; set; }
        public string EmppunchID { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string Attn_dt { get; set; }
        public string FinalStatus { get; set; }
        public string EmpMNo { get; set; }
    }

}