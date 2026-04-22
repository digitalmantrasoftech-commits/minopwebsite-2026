using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    // Start - JSon class sent from Datatables

    public class DataTableAjaxPostModel
    {
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public int branchid { get; set; }
        public List<Column> columns { get; set; }
        public Search search { get; set; }
        public List<Order> order { get; set; }
        public string FilterFromDate { get; set; }
        public string FilterToDate { get; set; }
        public string PunchID { get; set; }
        public string DeviceCode { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
    }


    public class DataTableEmployeePostModel
    {
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public int loginempid { get; set; }
        public int roleid { get; set; }
        public List<Column> columns { get; set; }
        public Search search { get; set; }
        public List<Order> order { get; set; }
        public int cmpid { get; set; }
        public int branchid { get; set; }
		public string departmentid { get; set; }
        public int selectstatus { get; set; }
    }

    public class DataTableTransactionMonitorPostModel
    {
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public int loginempid { get; set; }
        public int roleid { get; set; }
        public List<Column> columns { get; set; }
        public Search search { get; set; }
        public List<Order> order { get; set; }
        public int cmpid { get; set; }
        public int branchid { get; set; }
        public int deviceid { get; set; }
    }

    public class DataTableTransactionMonitorFilterPostModel
    {
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public int loginempid { get; set; }
        public int roleid { get; set; }
        public List<Column> columns { get; set; }
        public Search search { get; set; }
        public List<Order> order { get; set; }
        public int cmpid { get; set; }
        public int branchid { get; set; }
        public string deviceid { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string empid { get; set; }        
        public string flag { get; set; }
        public string companyid { get; set; }
    }
    public class DataTableWebPunchAjaxPostModel
    {
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public int branchid { get; set; }
        public List<Column> columns { get; set; }
        public Search search { get; set; }
        public List<Order> order { get; set; }
        public int roleid { get; set; }
        public int empid { get; set; }
        public int cmpid { get; set; }
        public string FilterFromDate { get; set; }
        public string FilterToDate { get; set; }
        public int selectstatus { get; set; }
        public int planid { get; set; }
        public int Isheararchy { get; set; }

    }

    public class DataTableLeaveAprrovejaxPostModel
    {
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public int branchid { get; set; }
        public List<Column> columns { get; set; }
        public Search search { get; set; }
        public List<Order> order { get; set; }
        public int roleid { get; set; }
        public int empid { get; set; }
        public int cmpid { get; set; }
        public string Employeeids { get; set; }
        public string filterFromDate { get; set; }
        public string filterToDate { get; set; }
        public int selectedStatus { get; set; }

        public int selectAll { get; set; } = 0;
        public string selectAllSearchTerm { get; set; } = string.Empty;
    }

    public class DataTableAttendanceAjaxPostModel
    {
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public int branchid { get; set; }
        public List<Column> columns { get; set; }
        public Search search { get; set; }
        public List<Order> order { get; set; }
        public int roleid { get; set; }
        public int empid { get; set; }
        public int cmpid { get; set; }
    }

    public class DataTableAjaxPostModelShiftallocation
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Search search { get; set; }
        public List<Column> columns { get; set; }
        public List<Order> order { get; set; }
        public int BranchID { get; set; }
        public string fromdt { get; set; }
        public string todt { get; set; }
        public string searchValue { get; set; }
    }

    public class Column
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public Search search { get; set; }
    }

    public class Search
    {
        public string value { get; set; }
        public string regex { get; set; }
    }

    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }
    }

    public class DatatableCounts
    {
        public int filteredResultsCount { get; set; }
        public int totalResultsCount { get; set; }
    }

    public class DatatableDealerCount
    {
        public int TotalNocompany { get; set; }
        public int TotalEmpCount { get;set;}
        public int TotalDeviceCount { get; set; }
    }


    /// End- JSon class sent from Datatables
    /// 

    //Add by Sh
    public class DataTableFilterCTCAjaxPostModelandFilterData
    {
        //public int? draw { get; set; }
        //public int? start { get; set; }
        //public int? length { get; set; }
        //public List<Column> columns { get; set; }
        //public Search search { get; set; }
        //public List<Order> order { get; set; }
        public string CompanyIds { get; set; }
        public string BranchIds { get; set; }
        public string DepartmentId { get; set; }   
        public string DesignationId { get; set; }
    }


    public class DataTableAjaxPostModelForTransctionData
    {
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public int branchid { get; set; }
        public List<Column> columns { get; set; }
        public Search search { get; set; }
        public List<Order> order { get; set; }
        public string FilterFromDate { get; set; }
        public string FilterToDate { get; set; }
        public string PunchID { get; set; }
        public string DeviceCode { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long DatagridThresold { get; set; }
    }

    public class DataTableForEnrollUserGridDataRequest
    {
        public string searchTerm = "";
        public string sortColumn = "";
        public string sortOrder = "";
        public int pageSize;
        public int page;        
        public Int64 DataGridValues { get; set; }
        public int BranchId { get; set; }
        public int CmpId { get; set; }
        
        public string FilterFromDate { get; set; }
        public string FilterToDate { get; set; }

        public string PunchID { get; set; }
        public string DeviceCode { get; set; }

    }
}