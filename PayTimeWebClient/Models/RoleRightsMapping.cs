using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PayTimeWebClient.Models
{
    public class RoleRightsMapping
    {
        #region Database Properties
        public int RoleRightsId { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public int OperationId { get; set; }
        public string OperationName { get; set; }
        public string ActionUrl { get; set; }
        public string ParentMenuName { get; set; }
        public virtual int ParentMenuId { get; set; }
        public int SeqNo { get; set; }
        public Boolean CanAdd { get; set; }
        public Boolean CanEdit { get; set; }
        public Boolean CanDelete { get; set; }
        public Boolean CanImport { get; set; }
        public Boolean CanExport { get; set; }
        public Boolean CanView { get; set; }
        public bool IsActive { get; set; }
        public string ImgClassName { get; set; }
        public int Devicecnt { get; set; }
        public string ProductId { get; set; }  // Changed from int? to string to support comma-separated values (e.g., "1,2,3")

        #endregion

        //public DataTable RoleMenuData { get; set; }
        //public List<RoleRightsMapping> EmployeeMenu { get; set; }
        public IEnumerable<RoleRightsMapping> RoleMenuData { get; set; }
        public DataTable RoleData { get; set; }
        //public IEnumerable<Roles> RoleList { get; set; }
    }
}