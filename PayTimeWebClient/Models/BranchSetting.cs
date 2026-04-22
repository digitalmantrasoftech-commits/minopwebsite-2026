using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class BranchSetting
    {
        public int BranchSettingId { get; set; }
        //public string BranchSettingName { get; set; }
        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public int TrancationYearId { get; set; }
        public string TimeZone { get; set; }
        public string Language { get; set; }
        public string Currency { get; set; }
        public string DatetimeFormat { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<BranchSetting> BranchSettinglist { get; set; }
    }
}