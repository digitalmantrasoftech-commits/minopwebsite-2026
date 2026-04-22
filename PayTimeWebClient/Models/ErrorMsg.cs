using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class ErrorMsg
    {
        public string MegSts { get; set; }
        public string Meg { get; set; }
        public IEnumerable<ErrorMsg> MsgList { get; set; }
    }
    public class ResMsg
    {
        public string MsgSts { get; set; }
        public string Msg { get; set; }
    }

}