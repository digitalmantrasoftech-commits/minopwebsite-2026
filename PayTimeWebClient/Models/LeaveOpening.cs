using PayTimeWebClient.Models.DataGridModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class LeaveOpening
    {
        public Int64 LeaveOpeningId { get; set; }
        public string EmpCode { get; set; }
        public int LeaveType { get; set; }
        [Required(ErrorMessage = "Leave Balance Required")]
        public Decimal LeaveBalance { get; set; }
        public string BalanceDate { get; set; }
        public string BalanceDateStr { get; set; }
        public string[] empArr { get; set; }
        public Int16 flg { get; set; }
        public string LeaveTypeName { get; set; }
        public string EmpName { get; set; }
        public bool IsActive { get; set; }
        public int LeaveBalTransYearid { get; set; }
        public int IsCarray { get; set; }
        public string ECode { get; set; }
    }

    public class BranchParameter
    {
        public string CompanyID { get; set; }
        public string BranchID { get; set; }
        public string DepartmentID { get; set; }
        public int SelectAllFlage { get; set; }
    }
    public class LeaveOpeningGetAll
    {
        public string BranchID { get; set; }
        public int cmpid { get; set; }
        public int Brchid { get; set; }
        public int RoleId { get; set; }
        public string Empid { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public Int64 DataGridthresold { get; set; }

        public string EmpIds { get; set; }

        public string searchTerm = "";
        public string sortColumn = "";
        public string sortOrder = "";
        public int SelectAllBranch { get; set; } = 0;
        public string SelectAllBranchSearchTerm { get; set; } = string.Empty;
        public int SelectAllEmp { get; set; } = 0;
        public string SelectAllEmpSearchTerm { get; set; } = string.Empty;
        public int ExportFlage { get; set; }
        public int IsActive { get; set; }
    }


    public class LevaeSanctionUploadModel
    {
        public string EmpCode { get; set; }
        public string LeaveType { get; set; }
        public string LeaveBalance { get; set; }
        public string BalanceDate { get; set; }
    }
    public class ImportResponseLeaveSanction
    {
        public int statusCode { get; set; }
        public bool status { get; set; }
        public List<LevaeSanctionUploadModel> LevaeSanctionList { get; set; }
    }

    public class ImportLeaveSanctionfaillog
    {
        public string EmpCode { get; set; }
        public string LeaveType { get; set; }
        public string LeaveBalance { get; set; }
        public string BalanceDate { get; set; }
        public string Reason { get; set; }
    }


    public class ImportLeaveSanctionfaillogList
    {
        public int ImpDataId { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string Startdate { get; set; }
        public string CreatedDtFlag { get; set; }
        public string Err { get; set; }
        public IEnumerable<ImportLeaveSanctionfaillog> importleaveFailloglist { get; set; }
    }
	
	public class LeaveOpeningRequest
    {
        public LeaveOpening Des { get; set; }
        public DataTableForEmployeeDirectoryGridData Emp { get; set; }
    }
    
    public class LeaveOpeningSanctionResponse
    {
        public List<TotalCountModel> Table1 { get; set; }
        public List<LeaveOpening> Table2 { get; set; }
    }
    public class TotalCountModel
    {
        public int TotalCount { get; set; }
    }


}