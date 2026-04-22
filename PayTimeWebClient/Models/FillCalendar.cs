using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class FillCalendar
    {
        public int id { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string color { get; set; }
      
        public IEnumerable<FillCalendar> FillCalendarList { get; set; }
    }
}