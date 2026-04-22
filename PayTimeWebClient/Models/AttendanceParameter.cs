using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class AttendanceParameter
    {
        public  int AttendanceParameterId { get; set; }
        public  int CompanyId { get; set; }
        public  int BranchId { get; set; }
        public  int MaxOutDur { get; set; }
        public  int TimeBetPunch { get; set; }
        public  int GraceLateIN { get; set; }
        public  int GraceEarlyOUT { get; set; }
        public  string ApplicationMode { get; set; }
        public  string MonthlyRptStaDay { get; set; }
        public  string MfsPopTime { get; set; }
        public  int MfsAuthMode { get; set; }
        public  int WeekOffOTHour { get; set; }
        public  int HolidayOffOTHour { get; set; }
        public  int OTMinHour { get; set; }
        public  int OTFormula { get; set; }
        public  int OTAllowWO { get; set; }
        public  int OTAllowHO { get; set; }
        public  int NotPresentInMnth { get; set; }
        public  bool Wbackup { get; set; }
        public  bool Dbackup { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifyBy { get; set; }
        public DateTime ModifyDate { get; set; }

        public IEnumerable<AttendanceParameter> AttendanceParaList { get; set; }
        public class MRespo
        {
            public string MegSts { get; set; }
            public string Meg { get; set; }
        }
    }
}