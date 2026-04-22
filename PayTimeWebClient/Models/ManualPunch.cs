using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class ManualPunch
    {
        public int tmpDmpid { get; set; }
        public int cmpid { get; set; }
        public int branchid { get; set; }
        public int[] empidlst { get; set; }
        public string mode { get; set; }
        public string In_Out_Time { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public int devicecode { get; set; }
        public string IPaddress { get; set; }
        public int EntryMode { get; set; }
        public int Punchid { get; set; }
       // public int tmpDmpid { get; set; }
        public Int64 Empid { get; set; }
        public string Empcode { get; set; }
        public string EmpName { get; set; }
       public int ShiftCode { get; set; }
       public string Attn_Dt { get; set; }
       public bool IsActive { get; set; }
       public IEnumerable<ManualPunch> PunchList { get; set; }
    }
    public class ReqManualPunch
    {
        public int tmpDmpid { get; set; }
        public int cmpid { get; set; }
        public int branchid { get; set; }
        public int[] empidlst { get; set; }
        public string[] mode { get; set; }
        public string[] In_Out_Time { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public int devicecode { get; set; }
        public string IPaddress { get; set; }
        public int EntryMode { get; set; }
        public int Punchid { get; set; }
       // public int tmpDmpid { get; set; }
        public Int64 Empid { get; set; }
        public string Empcode { get; set; }
        public string EmpName { get; set; }
       public int ShiftCode { get; set; }
       public string Attn_Dt { get; set; }
       public bool IsActive { get; set; }
       public string token { get; set; }
       public string Location { get; set; }
       public IEnumerable<ManualPunch> PunchList { get; set; }
    

    }
}