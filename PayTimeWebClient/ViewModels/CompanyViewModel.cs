using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.ViewModels
{
    public class CompanyViewModel
    {
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyUrl { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyContact { get; set; }
        public string CompanyPincode { get; set; }
        public string CompanyGstNO { get; set; }
        public string CompanyZipcode { get; set; }
        public IEnumerable<CompanyViewModel> Companyslist { get; set; }

        public bool HasMultipleBranch { get; set; }
        public bool HasDepartment { get; set; }
        public bool HasDesignation { get; set; }
        public bool HasShift { get; set; }
        public bool HasHoliday { get; set; }
        public bool HasLeave { get; set; }

       
        public string TimeZone { get; set; }
        public string Language { get; set; }
        public string Currency { get; set; }
        public string DatetimeFormat { get; set; }
        public int TrancationYearId { get; set; }
    }
}