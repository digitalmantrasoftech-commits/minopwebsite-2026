using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class OnDuty
    {
        public Int64 OnDutyId { get; set; }
        public Int64 EmpCode { get; set; }
        public int LeaveType { get; set; }
        public int ShiftCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Reason { get; set; }
        public Int16 LeavePaid { get; set; }
        public Int16 IsHalfLeave { get; set; }
        public bool IsActive { get; set; }
    }
}