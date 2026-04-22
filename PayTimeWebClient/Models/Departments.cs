using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class Departments
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentHead { get; set; }
        public string DeparmentEmailID { get; set; }
        public bool DeptIsEmail { get; set; }
        public string DepartmentHeadName { get; set; }
        public bool IsActive { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }

        public IEnumerable<Departments> Departmentslist { get; set; }
    }

    public class BranchGetbyDept
    {
        public string BranchId { get; set; }
    }


    public class Getdevicealltype
    {
        public string DeviceCode { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string DeviceIP { get; set; }
        public string DevicePort { get; set; }
        public string DeviceTypeCode { get; set; }
        public string BranchId { get; set; }
        public string Devicesrno { get; set; }
    }
}