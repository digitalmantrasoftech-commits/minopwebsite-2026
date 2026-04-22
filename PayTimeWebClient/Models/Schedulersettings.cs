using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PayTimeWebClient.Models
{
    public class Schedulersettings
    {
        public int tssId { get; set; }
        public string tssName { get; set; }
        public string tssDescription { get; set; }
        public int tssTriggerid { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime tssStartdate { get; set; }
        public string tssStarttime { get; set; }
        public int tssIntervalday { get; set; } //every _ day
        public int tssIntervalweek { get; set; }  //every _week
        public string tssmonths { get; set; }  //every _month defalt set 1
        public string tssWeekday { get; set; } //comma seprated 1 to 7
        public string tssMothdayno { get; set; }  //1 to 31
        public string tssMonthday { get; set; } //1 to 7
        public string tssMonthdayrule { get; set; } //1 to 5
        public int tssActionid { get; set; }
        public string tssTrigger { get; set; }
        public string tssAction { get; set; }
        //public int tssLastStatus { get; set; } //0 running,1 completed,2 error
        public bool tssIsActive { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime tssCreatedate { get; set;}
        public int tssCreateby { get; set; }
        public DateTime tssModifydate { get; set; }
        public int tssModifyby { get; set; }
        public IEnumerable<Schedulersettings> Schedulersettingslist { get; set; }
        public string[] tssWeekdayarr { get; set; }
        public string[] tssmonthdayarr { get; set; }
        public string[] tssMothdaynoarr { get; set; }
        public string[] tssMonthsarr { get; set; }
        public string[] tssMonthsdatearr { get; set; }
        //public string tssMonths { get; set; }
    }

    public class ScheduleAction
    {
        public int saId { get; set; }
        public string saActionname { get; set; }
        public int saCompanyId { get; set; }
        public int saBranchId { get; set;}
        public string saDepartmentId { get; set; }
        public int saLastdays { get; set; }
        public int saReportId { get; set; }
        public string saCompanyName { get; set; }
        public IEnumerable<ScheduleAction> ScheduleActionlist { get; set; }
    }

    public class Schedulertasklog
    {
        public int stId { get; set; }
        public int stTssId { get; set; }
        public DateTime stLastexecute { get; set; }
        public int stStatus { get; set; }
        public string stDescription { get; set; }
    }
}