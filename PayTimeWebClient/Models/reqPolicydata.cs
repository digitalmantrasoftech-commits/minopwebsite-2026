using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class reqPolicydata
    {
        public string comidlst { get; set; }
        public string branchidlst { get; set; }
        public string desigidlst { get; set; }
        public string deptidlst { get; set; }
        public string roleidlst { get; set; }
        public string empidlst { get; set; }
        public int policyid { get; set; }
    }
    public class UpdatePolicyEmp
    {
        public string empidlst { get; set; }
        public int policyid { get; set; }

    }
    public class resPolicyData
    {
        public int policyid { get; set; }
        public string PolicyName { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        public int BranchId { get; set; }
        public string BranchName { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public int DesignationId { get; set; }
        public string DesignationName { get; set; }

        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public int EmpId { get; set; }
        public string EmpName { get; set; }

    }
}