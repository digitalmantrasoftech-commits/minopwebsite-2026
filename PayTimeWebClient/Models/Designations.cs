using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.DataAccess.ObjectBinding;

namespace PayTimeWebClient.Models
{
    public class Designations
    {
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }
        public bool IsActive { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
     //   public string IsActive { get; set; }

        public IEnumerable<Designations> Designationlist { get; set; }
    }
}