using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models.DataGridModel
{
    public class CommonDataGrid
    {
    }

    public class DataTablesRequest
    {
        public int draw { get; set; }
        public int ExportFlage { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Search search { get; set; }
        public Order[] order { get; set; }
        public Column[] columns { get; set; }

        public CustomSearchFilter[] CustFilter { get; set; }

        public CommonMasterFilter EmployeeFilter { get; set; }

        public RoleWiseFilter RoleWiseFilter { get; set; }

        public Int64 DatagridThresold { get; set; }

        public int LoginId { get; set; }
        public int RoleId { get; set; }

    }
    public class DataTablesRequestExtended
    {
        public int draw { get; set; }
        public int ExportFlage { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Search search { get; set; }
        public Order[] order { get; set; }
        public Column[] columns { get; set; }

        public CustomSearchFilter[] CustFilter { get; set; }

        public EnrollCommonMasterFilter EmployeeFilter { get; set; }

        public RoleWiseFilter RoleWiseFilter { get; set; }

        public Int64 DatagridThresold { get; set; }

        public int LoginId { get; set; }
        public int RoleId { get; set; }

    }
    public class EmployeeGridResult
    {
        public Result Result { get; set; }
        public int Id { get; set; }
        public int Status { get; set; }
        public object Exception { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsCompleted { get; set; }
        public int CreationOptions { get; set; }
        public object AsyncState { get; set; }
        public bool IsFaulted { get; set; }
    }

    public class Result
    {
        public List<EmployeeItem> Items { get; set; }
        public int totalRecords { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }

        public List<EmployeeItemDropDown> EmployeeDropDownData { get; set; }

        public List<BranchItemDropDown> BranchDropDownData { get; set; }
        public List<DepartmentItemDropDown> DepartmentDropDownData { get; set; }
    }

    public class EmployeeItem
    {
        public int EmpId { get; set; }
        public int UserId { get; set; }
        public string EmpName { get; set; }
        public string Empcode { get; set; }
        public int? RoleId { get; set; }
        public int? ReligionId { get; set; }
        public bool IsSMS { get; set; }
        public int Gender { get; set; }
        public bool EmpMarried { get; set; }
        public string EmpJoinDate { get; set; }
        public int EmpPunchID { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int DesignationId { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public int ShiftId { get; set; }
        public int? ReportingTo { get; set; }
        public string ShiftName { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string EmpDOB { get; set; }
        public string EmpAddress { get; set; }
        public string EmpPhNo { get; set; }
        public string EmpMNo { get; set; }
        public string EmpPhoto { get; set; }
        public string MobNoSMS { get; set; }
        public string EmpResignDate { get; set; }
        public int? TypeId { get; set; }
        public string EmpTypeName { get; set; }
        public int GradeId { get; set; }
        public int? ShiftGroupId { get; set; }
        public string ShiftGroupName { get; set; }
        public int? ContractorId { get; set; }
        public string ContractorName { get; set; }
        public int CategoryId { get; set; }
        public string Categoryname { get; set; }
        public string ReportingName { get; set; }
        public int PolicyId { get; set; }
        public string PolicyName { get; set; }
        public string EmpWeekOff { get; set; }
        public string EmpSecondWeekOff { get; set; }
        public string EmpSecondWeekOffRule { get; set; }
        public string EmpHalfDay { get; set; }
        public string EmpHalfDayRule { get; set; }
        public string ShiftShortName { get; set; }
        public string ShiftGroupShortName { get; set; }
        public bool isActive { get; set; }
        public string Status { get; set; }
        public string Married { get; set; }
        public string EmpGender { get; set; }
        public string RoleName { get; set; }
        public string TagId { get; set; }
        public string worf { get; set; }
        public string geoenable { get; set; }
        public string CountryCode { get; set; }
        public string BranchGeolocation { get; set; } // Fix typo from "BranchGeolocat ion" to "BranchGeolocation"

        public DateTime? JoinDate { get; set; }

        public DateTime? BirthDate { get; set; }

        public string BUName { get; set; }
        public string SubBUName { get; set; }
        public int BusinessUnit { get; set; }
        public int SubBusinessUnit { get; set; }

        
    }


    public class DataTableForEmployeeGridData
    {
        public string searchTerm = "";
        public string sortColumn = "";
        public string sortOrder = "";
        public int pageSize;
        public int page;
        public int ExportFlage;
        public Int64 DatagridThresold;
        public int StatusId;
        public List<CustomSearchFilter> CustomFilters { get; set; }
        public EnrollCommonMasterFilter EmployeeFilter { get; set; }

        public RoleWiseFilter RoleWiseFilter { get; set; }
    }

    public class CommonMasterFilter
    {
        public string[] CompanyIDs { get; set; }
        public string[] BranchIDs { get; set; }
        public string[] DepartmentIDs { get; set; }
        public string[] DesignationIDs { get; set; }
    }

    public class EnrollCommonMasterFilter
    {
        public string CompanyIDs { get; set; }
        public string BranchIDs { get; set; }
        public string DepartmentIDs { get; set; }
        public string DesignationIDs { get; set; }
    }

  
    public class RoleWiseFilter
    {
        public string CompanyID { get; set; }
        public string BranchID { get; set; }
        public string EmpID { get; set; }
    }

    public class CustomSearchFilter
    {
        public string Field { get; set; }    // The field to filter on (e.g., "Name", "Code", etc.)
        public string Value { get; set; }    // The value to search for in the specified field
        public string Operator { get; set; } // Optional: Specify operators like "Contains", "Equals", etc.
    }

    public class EmployeeItemDropDown
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class EmployeeItemDropDownResult
    {
        public Result Result { get; set; }
        public int Id { get; set; }
        public int Status { get; set; }
        public object Exception { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsCompleted { get; set; }
        public int CreationOptions { get; set; }
        public object AsyncState { get; set; }
        public bool IsFaulted { get; set; }
    }

    #region For Branch Filter ParamClass
    public class SerachBranchObjectParam
    {
        public string searchTerm { get; set; }
        public string sortOrder { get; set; }
        public string sortColumn { get; set; }
        public int pageSize { get; set; }
        public int page { get; set; }
        public int CompanyID { get; set; }
        public string BranchID { get; set; }
        public string DepartID { get; set; }
        public int RoleID { get; set; }

    }

    public class BranchItemDropDown
    {
        public int BranchId { get; set; }
        public int CompanyId { get; set; }
        public string BranchName { get; set; }
    }
    #endregion   
    public class DepartmentItemDropDown
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int CompanyId { get; set; }
        public int BranchID { get; set; }
    }


    #region FOr BranchMaster DataGrid
    public class BranchItem
    {
        #region Properties
        public int Srno { get; set; }
        [Required(ErrorMessage = "Please Enter BranchName.")]
        [StringLength(40, ErrorMessage = "BranchName cannot exceed 40 characters.")]
        public string BranchName { get; set; }
        public string ReportingBranchName { get; set; }
        public int CityID { get; set; }
        public int BranchId { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public int ReportingBranchId { get; set; }
        public int BranchLevel { get; set; }
        public string CompanyName { get; set; }
        public string CityName { get; set; }
        [Required(ErrorMessage = "Please Enter Company.")]
        public int CompanyID { get; set; }
        public bool IsActive { get; set; }
        public string CompanyCode { get; set; }
        [Required(ErrorMessage = "Please Enter BranchAddress.")]
        public string BranchAddress { get; set; }
        public string BranchLatitude { get; set; }
        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$", ErrorMessage = "Invalid email format.")]
        public string BranchHeadEmail { get; set; }
        public string BranchHeadPassword { get; set; }
        public string BranchRadius { get; set; }
        public string BranchBeaconMac { get; set; }
        public string BeaconUuid { get; set; }
        //public string Customfield { get; set; }
        //public string Customvalues { get; set; }
        [Required(ErrorMessage = "Please Enter Country TimeZone.")]
        public string CountryTimeZoneID { get; set; }

        //public int DepartmentId { get; set; }        
        //public int CreatedBy { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public int ModifyBy { get; set; }
        //public DateTime ModifyDate { get; set; }
        //public int RoleID { get; set; }
        //public int Isverification { get; set; }
        #endregion
    }
    public class BranchGridResult
    {
        public ResultForBranch Result { get; set; }
        public int Id { get; set; }
        public int Status { get; set; }
        public object Exception { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsCompleted { get; set; }
        public int CreationOptions { get; set; }
        public object AsyncState { get; set; }
        public bool IsFaulted { get; set; }
    }

    public class ResultForBranch
    {
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public List<BranchItem> ItemsForBranch { get; set; }
    }

    public class DataTableForBranchGridData
    {
        public string searchTerm = "";
        public string sortColumn = "";
        public string sortOrder = "";
        public int pageSize;
        public int page;
        public int Export_flg;
        public Int64 DatagridThresold;
        public int RoleId { get; set; }
        public int LoginId { get; set; }
        public List<CustomSearchFilter> CustomFilters { get; set; }

    }
    #endregion

    public class DataTableForEnrollUserGridData
    {
        public string searchTerm = "";
        public string sortColumn = "";
        public string sortOrder = "";
        public int pageSize;
        public int page;
        public int StatusId { get; set; }
        public Int64 DataGridValues { get; set; }
        public string BranchId { get; set; }
        public string CmpId { get; set; }
        public string DptId { get; set; }
        public int SelectAll { get; set; } = 0;
        public string SelectAllSearchTerm { get; set; } = string.Empty;

        //public CommonMasterFilter EmployeeFilter { get; set; }

        //public RoleWiseFilter RoleWiseFilter { get; set; }
    }
    public class UserEnrollDataTablesRequest
    {
        #region Properties
        public int draw { get; set; }
        public int ExportFlage { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Search search { get; set; }
        public Order[] order { get; set; }
        public Column[] columns { get; set; }
        public int StatusId { get; set; }
        public Int64 DatagridThresold { get; set; }

        public EnrollCommonMasterFilter EmployeeFilter { get; set; }

        public CustomSearchFilter[] CustFilter { get; set; }

        public int SelectAll { get; set; } = 0;
        public string SelectAllSearchTerm { get; set; } = string.Empty;


        #endregion
    }
    public class UserGridResult
    {
        #region Properties

        public ResultForEnrollUser Result { get; set; }


        #endregion
    }
    public class ResultForEnrollUser
    {
        #region Properties

        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public List<enrollUserItem> ItemsForEnrollUser { get; set; }

        #endregion
    }
    public class enrollUserItem
    {
        #region Properties
        public int EmpPunchID { get; set; }

        public string EmpName { get; set; }
        public string EmpPhoto { get; set; }
        public string Email { get; set; }
        #endregion
    }

    public class DataTableForEmployeeDirectoryGridData
    {
        public string searchTerm = "";
        public string sortColumn = "";
        public string sortOrder = "";
        public int pageSize;
        public int page;
        public Int64 DataGridValues { get; set; }
        public int OnBoardingID { get; set; }
        public int StrengthID { get; set; }
        public int RoleId { get; set; }
        public int LoginEmpId { get; set; }
        public int CompanyId { get; set; }
        public string BranchId { get; set; }
        public string DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public int DeactivateID { get; set; }

        public List<CustomSearchFilter> CustomFilters { get; set; }

        //public CommonMasterFilter EmployeeFilter { get; set; }

        //public RoleWiseFilter RoleWiseFilter { get; set; }
        public string CompanyIds { get; set; }
        public int SelectAll { get; set; } = 0;
        public string SelectAllSearchTerm { get; set; } = string.Empty;

        public int SelectAllBranch { get; set; } = 0;
        public string SelectAllBranchSearchTerm { get; set; } = string.Empty;

        public int SelectAllEmp { get; set; } = 0;
        public string SelectAllEmpSearchTerm { get; set; } = string.Empty;
    }

    public class EmployeeDirectoryDataTablesRequest
    {
        #region Properties
        public int draw { get; set; }
        public int ExportFlage { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Search search { get; set; }
        public Order[] order { get; set; }
        public Column[] columns { get; set; }
        public Int64 DatagridThresold { get; set; }
        public int OnBoardingID { get; set; }
        public int StrengthID { get; set; }
        public int RoleId { get; set; }
        public int LoginEmpId { get; set; }
        public int CompanyId { get; set; }
        public string BranchId { get; set; }
        public string DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public int DeactivateID { get; set; }
        public CommonMasterFilter EmployeeFilter { get; set; }

        public CustomSearchFilter[] CustFilter { get; set; }
        public string CompanyIds { get; set; }
        public int SelectAllDept { get; set; } = 0;
        public string SelectAllSearchTermDept { get; set; } = string.Empty;

        public int SelectAll { get; set; } = 0;
        public string SelectAllSearchTerm { get; set; } = string.Empty;

        public int SelectAllBranch { get; set; } = 0;
        public string SelectAllBranchSearchTerm { get; set; } = string.Empty;
        public int SelectAllEmp { get; set; } = 0;
        public string SelectAllEmpSearchTerm { get; set; } = string.Empty;

        #endregion
    }

    public class DataTableForEmployeeOnBoardingGridData
    {
        public string searchTerm = "";
        public string sortColumn = "";
        public string sortOrder = "";
        public int pageSize;
        public int page;
        public Int64 DataGridValues { get; set; }
        public int OnBoardingID { get; set; }
        public int StrengthID { get; set; }
        public int RoleId { get; set; }
        public int LoginEmpId { get; set; }
        public int CompanyID { get; set; }
        public string BranchID { get; set; }
        public string DeaprtmentID { get; set; }
        public int DesignationID { get; set; }
        public int DeactivateID { get; set; }
        public int ExportFlag { get; set; }

        public List<CustomSearchFilter> CustomFilters { get; set; }

        //public CommonMasterFilter EmployeeFilter { get; set; }

        //public RoleWiseFilter RoleWiseFilter { get; set; }
    }

    public class EmployeeOnBoardingDataTablesRequest
    {
        #region Properties

        public string searchTerm = "";
        public string sortColumn = "";
        public string sortOrder = "";
        public int pageSize;
        public int page;
        public Int64 DataGridValues { get; set; }
        public int draw { get; set; }
        public int ExportFlage { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Search search { get; set; }
        public Order[] order { get; set; }
        public Column[] columns { get; set; }
        public Int64 DatagridThresold { get; set; }
        public int OnBoardingID { get; set; }
        public int StrengthID { get; set; }
        public int RoleId { get; set; }
        public int LoginEmpId { get; set; }
        public int CompanyID { get; set; }
        public string BranchID { get; set; }
        public string DeaprtmentID { get; set; }
        public int DesignationID { get; set; }

        //public int DeactivateID { get; set; }
        public CommonMasterFilter EmployeeFilter { get; set; }

        public CustomSearchFilter[] CustFilter { get; set; }

        public List<CustomSearchFilter> CustomFilters { get; set; }


        #endregion
    }

    public class DeviceMasterGridReqDto
    {
        #region Properties
        public int draw { get; set; }
        public int ExportFlage { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Search search { get; set; }
        public Order[] order { get; set; }
        public Column[] columns { get; set; }
        public CustomSearchFilter[] CustFilter { get; set; }
        public Int64 DatagridThresold { get; set; }
        public int RoleId { get; set; }
        public int CompanyId { get; set; }
        public string BranchId { get; set; }
        public string CompanyCode { get; set; }

        #endregion
    }

    public class DeviceMasterGridReq
    {
        public string searchTerm = "";
        public string sortColumn = "";
        public string sortOrder = "";
        public int pageSize;
        public int page;
        public Int64 DataGridValues { get; set; }

        public string CompanyCode { get; set; }
        public int RoleId { get; set; }

        public int CompanyId { get; set; }
        public string BranchId { get; set; }
        public List<CustomSearchFilter> CustomFilters { get; set; }
        public int Export_flg;

    }
    public class AdminDashBoardDto
    {
        #region Properties
        public int draw { get; set; }
        public int ExportFlage { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Search search { get; set; }
        public Order[] order { get; set; }
        public Column[] columns { get; set; }
        public Int64 DatagridThresold { get; set; }
        public int CompanyId { get; set; }
        public string BranchId { get; set; }
        
        public int ReqID { get; set; }
        #endregion
    }
    public class AdminDashBoardReq
    {
        #region Properties
        public string searchTerm = "";
        public string sortColumn = "";
        public string sortOrder = "";
        public int pageSize;
        public int page;
        public Int64 DataGridValues { get; set; }
        public int ReqID { get; set; }
        public int CompanyId { get; set; }
        public string BranchId { get; set; }

        public int Export_flg;
        #endregion
    }

    public class AttanSummaryGridReq : AttanList
    {
        public int draw { get; set; }
        public int ExportFlage { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Search search { get; set; }
        public Order[] order { get; set; }
        public Column[] columns { get; set; }
        public CustomSearchFilter[] CustFilter { get; set; }
        public Int64 DatagridThresold { get; set; }

    }

    public class AttanSummaryDto : AttanList
    {
        public string searchTerm = "";
        public string sortColumn = "";
        public string sortOrder = "";
        public int pageSize;
        public int page;
        public Int64 DataGridValues { get; set; }
        public List<CustomSearchFilter> CustomFilters { get; set; }
        public int Export_flg;

    }
	#region For TractionDatatableRequser
    public class TransctionDataTablesDeveloper
    {
        #region Properties

        public string searchTerm = "";
        public string sortColumn = "";
        public string sortOrder = "";
        public int draw { get; set; }
        public int ExportFlage { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Search search { get; set; }
        public Order[] order { get; set; }
        public Column[] columns { get; set; }        
        public Int64 DatagridThresold { get; set; }

        public string FilterFromDate { get; set; }
        public string FilterToDate { get; set; }
        public string PunchID { get; set; }
        public string DeviceCode { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }       

        public CustomSearchFilter[] CustFilter { get; set; }

        #endregion
    }

    public class TransctionDataTablesDeveloperRequest
    {
        #region Properties

       
        public int draw { get; set; }
        public int ExportFlage { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Search search { get; set; }
        public Order[] order { get; set; }
        public Column[] columns { get; set; }
        public Int64 DatagridThresold { get; set; }

        public string FilterFromDate { get; set; }
        public string FilterToDate { get; set; }
        public string PunchID { get; set; }
        public string DeviceCode { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public CustomSearchFilter[] CustFilter { get; set; }

        public string EmpIds { get; set; }
        public string BranchID { get; set; }
        public int CompanyID { get; set; }
        public int SelectAllBranch { get; set; } = 0;
        public string SelectAllBranchSearchTerm { get; set; } = string.Empty;
        public int SelectAllEmp { get; set; } = 0;
        public string SelectAllEmpSearchTerm { get; set; } = string.Empty;
        public int IsActive { get; set; }
        #endregion
    }
    public class Table1Item
    {
        public int TotalCount { get; set; }
    }

    public class Table2Item
    {
        public long txnId { get; set; }
        public int punchId { get; set; }
        public int dvcId { get; set; }
        public string txnDateTime { get; set; }
        public bool isSync { get; set; }
        public string LastActivity { get; set; }
        public string EntryDate { get; set; }
        public string Remarks { get; set; }
        public string RemarksDate { get; set; }
    }

    public class TransctionDataGridResponse
    {
        public List<Table1Item> Table1 { get; set; }
        public List<Table2Item> Table2 { get; set; }
    }
    #endregion

    #region Analytics report
    public class reqReports
    {
        public string FromDate { get; set; }
        public string Todate { get; set; }
        public string EmpID { get; set; }
        public string reportName { get; set; }
        public Search search { get; set; }
        //public int CompanyID { get; set; }
        public string CompanyID { get; set; }
        public int RptID { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string OperationName { get; set; }
        public string ParentMenuName { get; set; }
        public string ImgClassName { get; set; }
        public int SeqNo { get; set; }
        public CustomSearchFilter[] columnFilters { get; set; }
        public List<CustomSearchFilter> CustomFilters { get; set; }
        public int IsFilo { get; set; }
        public string DepId { get; set; }
        public string BranchId { get; set; }
        public int ExportFlag { get; set; }
        
        public int page { get; set; }
        public int pageSize { get; set; }
        public string searchTerm { get; set; }
        public string sortColumn { get; set; }
        public string sortOrder { get; set; }
        public int Isactive { get; set; }
        public int Userid { get; set; }
        public int draw { get; set; }
       

    }
    #endregion
}