using System;
using System.Collections.Generic;



namespace PayTimeWebClient.Models
{
    public class Webpunch
    {
        #region Properties
        public int WebpunchId { get; set; }
        public DateTime? In_Out_Time { get; set; }
        public DateTime? Attn_Dt { get; set; }
        public string Mode { get; set; }
        public string IPaddress { get; set; }
        public int DeviceId { get; set; }
        public int EntryMode { get; set; }
        public int RecDet { get; set; }
        public string AttnDt { get; set; }
        public string InOutTime { get; set; }
        public string Punchid { get; set; }
        public int Empid { get; set; }
        public string Empcode { get; set; }
        public string Location { get; set; }
        public string LocAddress { get; set; }
        public int Createdby { get; set; }
        public string CreatedDate { get; set; }
        public int ModifyBy { get; set; }
        public string ModifyDate { get; set; }
        public int IsApprove { get; set; }
        public int ApprovalBy { get; set; }
        public string ApprovalDate { get; set; }
        public string EmpName { get; set; }
        public int AprrovalRoleid { get; set; }
        public int PunchType { get; set; }
        public string Reason { get; set; }
        public string ReportingName { get; set; }
        public string ActionBy { get; set; }

        public string EmpPhoto { get; set; }
        public IEnumerable<Webpunch> PunchList { get; set; }
        #endregion
    }

    public class WebpunchApprove
    {
        #region Properties
        public int WebpunchId { get; set; }
        public int ApprovalBy { get; set; }
        public int ApprovalbyRole { get; set; }
        public int IsApprove { get; set; }
        #endregion
    }

    public class punchlist
    {
        public int WebpunchId { get; set; }
        public DateTime In_Out_Time { get; set; }
        public DateTime Attn_Dt { get; set; }
        public string Mode { get; set; }
        public string Location { get; set; }
        public string LocAddress { get; set; }
       
        public int ApprovalBy { get; set; }
        public string ApprovalDate { get; set; }
    }

}