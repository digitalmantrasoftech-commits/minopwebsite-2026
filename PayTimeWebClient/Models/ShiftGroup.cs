using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class ShiftGroup
    {
        public int ShiftGroupId { get; set; }
        public string ShiftGroupName { get; set; }
        public string ShiftGroupShortName { get; set; }
        public bool IsActive { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }

        public IEnumerable<ShiftGroup> ShiftGrouplist { get; set; }
    }
}