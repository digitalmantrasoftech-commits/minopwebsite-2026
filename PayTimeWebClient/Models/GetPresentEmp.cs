using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class GetPresentEmp
    {
       // [{"CountStatus":16,"FinalStatus":"Absent"},{"CountStatus":4,"FinalStatus":"Error"},{"CountStatus":2,"FinalStatus":"Holiday"},{"CountStatus":5,"FinalStatus":"Present"},{"CountStatus":4,"FinalStatus":"Weekly Off"}]
        public int CountStatus { get; set; }
        public string FinalStatus { get; set; }
        //public int Error { get; set; }
        //public int Weekoff { get; set; }
        //public int Leave { get; set; }
    }
}