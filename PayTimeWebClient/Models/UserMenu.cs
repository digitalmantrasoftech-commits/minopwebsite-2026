using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PayTimeWebClient.Models
{
    public class UserMenu
    {
        public int OperationId { get; set; }
        public string OperationName { get; set; }
        public string ActionUrl { get; set; }
        public short ParentId { get; set; }
        public short ParentMenuId { get; set; }
        public string MenuController { get; set; }
        public string MenuAction { get; set; }
        public short SeqNo { get; set; }
        public string ParentMenuName { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanExport { get; set; }
        public bool CanImport { get; set; }
        public List<UserMenu> EmployeeMenu { get; set; }
        public bool IsActive { get; set; }
        
    }

    //public class UserDetails
    //{
    //    public List<UserMenu> EmployeeMenu { get; set; }
    //}
}