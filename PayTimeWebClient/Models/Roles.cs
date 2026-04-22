using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class Roles
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool IsActive { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public IEnumerable<Roles> RoleList { get; set; }
    }
}