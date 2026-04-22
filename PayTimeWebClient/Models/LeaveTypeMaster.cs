using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class LeaveTypeMaster
    {
        public int LeaveTypeId { get; set; }
        [Display(Name = "Leave Type")]
        [Required(ErrorMessage = "Leave Type Required")]
        public string LeaveTypeName { get; set; }
        [Required(ErrorMessage = "Description Required")]
        public string Description { get; set; }
        public bool Sync { get; set; }
        public bool IsCarrayForwrd { get; set; }
        public bool IsEncashable { get; set; }
        [Display(Name = "CarrayForword Leave")]
        public int CarrayForwrdLeave { get; set; }
        [Display(Name = "Encashable Leave")]
        public int EncashableLeave { get; set; }
        [Display(Name = "Is half leave applicable?")]
        public bool HalfLeave { get; set; }
        public bool IsActive { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public bool IsDocRequire { get; set; }
        public int LimitDays { get; set; }
        public int MinleaveLimit { get; set; }
        public bool IsLeaveApprove { get; set; }
        public int IsLocationEnabled { get; set; }
        public IEnumerable<LeaveTypeMaster> LeaveTypelist { get; set; }
    }
}