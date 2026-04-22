using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class ConfigurationSystemMaster
    {
        public int ConfigId { get; set; }
        public string Mastsqlconstr { get; set; }
        public string Mastmysqlconstr { get; set; }
        public string Mastoracconstr { get; set; }
        public string Dbtype { get; set; }
        public string DbLocation { get; set; }
        public string JWTKey { get; set; }

        public string ClientServer { get; set; }
        public string ClientDbUser { get; set; }
        public string ClientDbPass { get; set; }
        public string ClientDbType { get; set; }

        public string SmtpHost { get; set; }
        public string FromEmail { get; set; }
        public string FromPassword { get; set; }
        public string SmtpPort { get; set; }
        public string ActiveUserURL { get; set; }

        public IEnumerable<ConfigurationSystemMaster> ConfigSysList { get; set; }
    }
}