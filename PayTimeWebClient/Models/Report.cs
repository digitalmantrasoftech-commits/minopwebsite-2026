using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class Report
    {

        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<Dictionary<string, object>> data { get; set; }

        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public object details { get; set; }

    }
}