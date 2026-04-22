using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class EmailType
    {
        public int EmailTypeID { get; set; }
        public string EmailTypeName { get; set; }
        public bool IsActive { get; set; }

        public IEnumerable<EmailType> EmailTypeList { get; set; }
    }
}