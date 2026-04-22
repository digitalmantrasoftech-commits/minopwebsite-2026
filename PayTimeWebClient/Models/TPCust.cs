using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class TPCust
    {
    
    }
    public class MedicalDocumentModel
    {
        public bool bloodtest { get; set; }
        public bool urinetest { get; set; }
        public bool ecgreport { get; set; }
        public bool xrayreport { get; set; }
        public bool visionreport { get; set; }
        public bool doctorfitness { get; set; }
        public string MedicalFiles { get; set; }
    }

    public class MedicalDocumentResponse
    {
        public int responseCode { get; set; }
        public string message { get; set; }
    }

}