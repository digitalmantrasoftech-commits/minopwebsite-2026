using System;
using System.Collections.Generic;

namespace PayTimeWebClient.Models
{
    public class DeviceService
    {
        public int Deviceserid { get; set; }
        public string AccountCode { get; set; }
        public string IpAddress { get; set; }
        public string SystemCode { get; set; }
        public bool Iswhitelist { get; set; }
        public DateTime CreatedDate { get; set; }
        public IEnumerable<DeviceService> DeviceServicelist { get; set; }
    }
}