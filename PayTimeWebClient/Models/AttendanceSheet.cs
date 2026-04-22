using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class AttendanceSheet
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Shift { get; set; }
        public string Date { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string TotHour { get; set; }
        public string Status { get; set; }

    }
}