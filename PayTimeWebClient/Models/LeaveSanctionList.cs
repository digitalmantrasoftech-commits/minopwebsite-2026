using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class LeaveSanctionList
    {
        public string Companyid;
        public string Branchid;
        public string Departmentid;
        public string Employeeid; 
        public IEnumerable<LeaveSanctionList> LeaveSanList { get; set; }
    }
    public class LeaveAutomaticSanctionlist
    {
        public int EmpId;
        public string EmpName;
        public string CompanyName;
        public string BranchName;
        public string DepartmentName;
        
    }
}