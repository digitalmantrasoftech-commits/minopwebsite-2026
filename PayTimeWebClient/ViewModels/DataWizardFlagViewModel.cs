using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.ViewModels
{
    public class DataWizardFlagViewModel
    {
        public bool HasMultipleBranch { get; set; }
        public bool HasDepartment { get; set; }
        public bool HasDesignation { get; set; }
        public bool HasShift { get; set; }
        public bool HasHoliday { get; set; }
        public bool HasLeave { get; set; }
       

    }
}