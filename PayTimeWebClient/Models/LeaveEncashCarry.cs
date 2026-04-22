using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class LeaveEncashCarry
    {
        public Int64 LeaveEncashCarryId { get; set; }
        public Int64 EmpCode { get; set; }
        public int LeaveType { get; set; }
        public decimal LeaveBalance { get; set; }
        public int TransactionYearCode { get; set; }
        public int EncashCarryFlg { get; set; }

        public  string EmpName { get; set; }
        public  string LeaveTypeName { get; set; }
        public  string TransactionYearName { get; set; }
        public  string EncashCarry { get; set; }
        public bool IsActive { get; set; }
    }
    public class LeaveEncashEmp
    {
        public int[] EmpArr { get; set; }
        public string[] EmpArr1 { get; set; }
        public string Emps { get; set; }
    }
}