using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class DeviceTypes
    {
        #region Properties
        public  int DeviceTypeCode { get; set; }
        public  string DeviceType { get; set; }
        public  int ServerPort { get; set; }
        public  int HDPPort { get; set; }
        public bool IsActive { get; set; }

        public IEnumerable<DeviceTypes> DeviceTypeList { get; set; }
        #endregion

    }
}