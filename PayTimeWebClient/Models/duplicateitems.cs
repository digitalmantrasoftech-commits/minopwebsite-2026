using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class duplicateitems
    {
        public int HeadId { get; set; }

        public int HeadType { get; set; }

        public string HeadTitle { get; set; }

        public string CustomFormula { get; set; }

        public string NameInSalarySlip { get; set; }

        public int is_adjust_amount { get; set; }

        public int IsActive { get; set; }
    }
}