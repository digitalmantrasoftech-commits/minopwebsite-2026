using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class Shifts
    {
        public int ShiftId { get; set; }
        public string ShiftName { get; set; }
        public string ShiftShortName { get; set; }
        public int ShiftGroupId { get; set; }
        public string ShiftGroupName { get; set; }
         //[DisplayFormat(DataFormatString = "{0:hh:mm:ss}")]
        public string StartTime { get; set; }
         //[DisplayFormat(DataFormatString = "{0:hh:mm:ss}")]
        public string EndTime { get; set; }
        public int GraceBefore { get; set; }
        public int GraceAfter { get; set; }
        public string ShiftDur { get; set; }
        public string MinHrsHalfDay { get; set; }
        public string MinHrsFullDay { get; set; }
        public string RecessDur { get; set; }
        public string SmsScheduleTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsFlexi { get; set; }
        public bool IsNightFlexi { get; set; }
        public int roleid { get; set; }
        public string FlexiShiftName { get; set; }
        public int ShiftType { get; set; }
        public DateTime ShiftEndTime { get; set; }
        public string TotalHoursFlexi { get; set; }
        public string MinHrs { get; set; }
        public string MaxHrs { get; set; }
        public string MinHrsForHalfDay { get; set; }
        public IEnumerable<Shifts> Shiftslist { get; set; }
        public IEnumerable<ShiftGroup> ShiftGrouplist { get; set; }

        public int IsCalculateHolidayHrs { get; set; }
        public int IsCalculatePaidLeaveHrs { get; set; }
        public int IsCalculateLeaveHrs { get; set; }
        public int IsCalculateWOHrs { get; set; }
    }
}