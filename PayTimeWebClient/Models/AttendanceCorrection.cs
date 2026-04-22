
namespace PayTimeWebClient.Models
{
    public class AttendanceCorrection
    {
        public int AttCorrectionId { get; set; }
        public int EmpId { get; set; }
        public string empname { get; set; }
        public string InPunchTime { get; set; }
        public string OutPunchTime { get; set; }
        public string ApplyReason { get; set; }
        public int Status { get; set; }
        public string ApprovalReason { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Location { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int ModifyBy { get; set; }
        public string ModifyDate { get; set; }
        public string attn_dt { get; set; }
        public int roleid { get; set; }
        public int cmpid { get; set; }
        public int branchid { get; set; }
        public string FilterFromDate { get; set; }
        public string FilterToDate { get; set; }
        public int _IsStatus { get; set; }
        public int AttendanceId { get; set; }        
        public int PunchID { get; set; }

    }
}