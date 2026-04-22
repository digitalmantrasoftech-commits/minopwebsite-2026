using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class BirthdayEmp
    {
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string EmpMNo { get; set; }
        public string Gender { get; set; }
        public string EmpJoinDate { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public IEnumerable<Employees> EmployeeList { get; set; }
    }
}