using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class Religion
    {
        public int ReligionId { get; set; }
        public string ReligionName { get; set; }
        public bool IsActive { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }

        public IEnumerable<Religion> religionList { get; set; }
    }
}