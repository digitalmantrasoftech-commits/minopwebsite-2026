using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class EmailDomainDetail
    {
        public int DomainId { get; set; }
        public string DomainName { get; set; }
        public string SMTPServer { get; set; }
        public int ServerPort { get; set; }
        public bool IsActive { get; set; }

        public IEnumerable<EmailDomainDetail> Emaildomaindetaillist { get; set; }
    }
}