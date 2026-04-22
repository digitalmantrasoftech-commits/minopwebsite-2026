using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PayTimeWebClient.Models
{
    public class ShiftAllocation
    {
        [Required(ErrorMessage = "")]
        public string[] EmpCodeArray { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string CompanyName { get; set; }
        public int CompanyID { get; set; }
        public int EmpCode { get; set; }
        public int ShiftCode { get; set; }
        public int WO { get; set; }
        public int HO { get; set; }
        public DateTime ShiftDate { get; set; }
        public decimal Nshiftdate { get; set; }
        public IEnumerable<ShiftAllocation> ShiftAllocationlist { get; set; }
        public IEnumerable<Companys> Companyslist { get; set; }
        public IEnumerable<Branches> Brancheslist { get; set; }
        public int ShiftId { get; set; }

        public virtual Int64 ShiftAllocationId { get; set; }
        public virtual string EmpPunchId { get; set; }
        public virtual string EmpName { get; set; }
        public virtual string ShiftDateStr { get; set; }
        public virtual string ShiftName { get; set; }
        public bool IsActive { get; set; }
    }
    public class ShiftAlloc
    {
        public string[] EmpArr { get; set; }
        public int ShiftID { get; set; }
        public DateTime FromDt { get; set; }
        public DateTime ToDt { get; set; }
        public bool IsActive { get; set; }

        public int SelectAllFlage { get; set; }
        public string SelectAllSearchTerm { get; set; }
        public string SelectAllSearchTermDept { get; set; }
        public int SelectAllDept { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public string DepartmentID { get; set; }

    }

    public class ShiftAllocationUploadModel
    {
        public string EmpCode{ get; set; }
        public string ShiftName { get; set; }   
        public string Fromdate { get; set; }
        public string Todate { get; set; }
    }
    public class ImportResponseShiftAllocation    {
        public int statusCode { get; set; }
        public bool status { get; set; }
        public List<ShiftAllocationUploadModel> ShiftAllocationList { get; set; }
    }

    public class ImportShiftAllocationfaillog
    {
        public string EmpCode { get; set; }
        public string ShiftName { get; set; }
        public string Fromdate { get; set; }
        public string Todate { get; set; }
        public string Reason { get; set; }
    }


    public class ImportShiftAllocationfaillogList
    {
        public int ImpDataId { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string Startdate { get; set; }
        public string CreatedDtFlag { get; set; }
        public string Err { get; set; }
        public IEnumerable<ImportShiftAllocationfaillog> importshiftFailloglist { get; set; }
    }


}