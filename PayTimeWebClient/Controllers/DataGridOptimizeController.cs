using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PayTimeWebClient.Models.DataGridModel;
using System.Data;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using OfficeOpenXml;
using Newtonsoft.Json.Linq;

namespace PayTimeWebClient.Controllers
{
    public class DataGridOptimizeController : Controller
    {
        #region Declaration
        static readonly DataGridOptimizeRestClient RestClient = new DataGridOptimizeRestClient();
        static readonly EncryptionHelper EncryptionHelperr = new EncryptionHelper();
        static readonly UtilitiesDataRestClient UtilitiesRestClient = new UtilitiesDataRestClient();

        // Static dictionary to track export statuses
        //private static readonly ConcurrentDictionary<string, ExportStatus> ExportStatuses = new ConcurrentDictionary<string, ExportStatus>();
        #endregion

        #region For CompanyHighLevelGridData
        public async Task<ActionResult> GetPaginatedCompany(EmployeeOnBoardingDataTablesRequest request)
        {
            try
            {
                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = ""; // Default sort column
                string sortOrder = "";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 DataGridValues = request.DatagridThresold;
                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value

                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }                   
                    else
                    {
                        sortColumn = "CompanyName";
                        sortOrder = "asc";
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }

                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }

                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];
                // Prepare parameters for RestClient or any service you're using
                DataTableForEmployeeOnBoardingGridData dataRequest = new DataTableForEmployeeOnBoardingGridData
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    OnBoardingID = request.OnBoardingID,
                    RoleId = request.RoleId,
                    LoginEmpId = request.LoginEmpId,
                    ExportFlag = 0,
                    CustomFilters = customFilters.ToList()
                };

                var Result = RestClient.GetPaginatedCompanyAsync(dataRequest);

                var paginatedResult = JsonConvert.DeserializeObject<DataSet>(Result);

                // Ensure the DataSet has necessary tables
                if (paginatedResult.Tables.Count < 2)
                {
                    throw new Exception("Invalid dataset structure. Tables missing.");
                }

                var dataTable = paginatedResult.Tables["Table"];    // Actual data
                var metadataTable = paginatedResult.Tables["Table1"]; // Metadata table

                var totalRecords = Convert.ToInt32(metadataTable.Rows[0]["TotalRecords"]);
                var filteredRecords = totalRecords;  // Adjust filtered records if necessary

                var response = new
                {
                    draw = request.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = dataTable
                };

                // Manually serialize the response using Json.NET
                var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    MaxDepth = int.MaxValue,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                // Return the serialized JSON response
                return Content(jsonResponse, "application/json");
            }
            catch (ArgumentNullException ex)
            {
                // Handle null argument errors
                return Json(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Handle argument-related issues
                return Json(new { error = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                // Handle issues with the HTTP client or REST API
                return Json(new { error = "There was an issue fetching data from the server. Please try again later." });
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                // Example: Logger.LogError(ex, "Error in GetPaginatedEmployeeDirectory");
                return Json(new { error = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpPost]
        public JsonResult GetAllCompanyExcelData(EmployeeOnBoardingDataTablesRequest request)
        {
            try
            {
                string searchTerm = string.Empty;
                string sortColumn = "";
                string sortOrder = "";
                int pageSize = 10;
                int page = 1;
                Int64 DataGridValues = 0;
                // Check if request is not null
                if (request != null)
                {
                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements
                    if (request.order != null && request.order.Length > 0)
                    {
                        //sortColumn = request.columns[request.order[0].column].data == "all" ? "Tf.OnboardingID" : request.columns[request.order[0].column].data;
                        //sortOrder = request.order[0].dir;
                        sortColumn = request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    else
                    {
                        sortColumn = "CompanyName";
                        sortOrder = "asc";
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];

                EmployeeOnBoardingDataTablesRequest dataRequest = new EmployeeOnBoardingDataTablesRequest
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    OnBoardingID = request.OnBoardingID,
                    RoleId = request.RoleId,
                    CompanyID = request.CompanyID,
                    BranchID = request.BranchID,
                    LoginEmpId = request.LoginEmpId,
                    DeaprtmentID = request.DeaprtmentID,
                    DesignationID = request.DesignationID,
                    CustomFilters = customFilters.ToList()
                };

                DataTable dataTableComp = RestClient.GetCompanyExcelGridData(dataRequest);
                var result = new ExportExcelFileMaster().SaveExcelToServer(dataTableComp, "CompanyMasterData", "CompanyMasterData_");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while processing your request. Please try again later." });
            }
        }
        #endregion

        #region For BranchHighLevelGridData
        public async Task<JsonResult> GetPaginatedBranches(DataTablesRequest request)
        {
            // Initialize variables
            string searchTerm = string.Empty;
            string sortColumn = "BranchId"; // Default sort column
            string sortOrder = "asc";     // Default sort order
            int pageSize = 10;            // Default page size
            int page = 1;                 // Default page number       
            int _ExportFlage = 0;
            Int64 _DatagridThresold = 0;
            int _loginID = 0;
            int _roleID = 0;

            // Check if request is not null
            if (request != null)
            {
                // Check if search object is present and has value

                if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                {
                    searchTerm = request.search.value;
                }
                // Check if order array is present and has elements
                if (request.order != null && request.order.Length > 0)
                {
                    sortColumn = request.columns[request.order[0].column].data;
                    sortOrder = request.order[0].dir;
                }

                // Set pageSize and page if length and start are set
                if (request.length > 0)
                {
                    pageSize = request.length;
                }
                if (request.start >= 0)
                {
                    page = (request.start / pageSize) + 1;
                }
                if (request.DatagridThresold > 0)
                {
                    _DatagridThresold = request.DatagridThresold;
                }
                if (request.ExportFlage > 0)
                {
                    _ExportFlage = request.ExportFlage;
                }
                if (request.LoginId > 0)
                {
                    _loginID = request.LoginId;
                }
                if (request.RoleId > 0)
                {
                    _roleID = request.RoleId;
                }
            }
            var customFilters = request.CustFilter ?? new CustomSearchFilter[0];
            // Prepare parameters for RestClient or any service you're using
            DataTableForBranchGridData dataRequest = new DataTableForBranchGridData
            {
                page = page,
                pageSize = pageSize,
                searchTerm = searchTerm,
                sortOrder = sortOrder,
                sortColumn = sortColumn,
                CustomFilters = customFilters.ToList(),
                DatagridThresold = _DatagridThresold,
                Export_flg = _ExportFlage,
                LoginId = _loginID,
                RoleId = _roleID
            };
            var paginatedResult = RestClient.GetPaginatedBranchesAsync(dataRequest);
            return Json(new
            {
                draw = request.draw,
                recordsTotal = paginatedResult.Result.TotalItems,
                recordsFiltered = paginatedResult.Result.TotalItems,
                data = paginatedResult.Result
            });
        }

        public JsonResult GetAllBranchExcelData(DataTablesRequest request)
        {
            try
            {
                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = "bm.branchname"; // Default sort column
                string sortOrder = "asc";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 _DatagridThresold = 0;
                int _loginID = 0;
                int _roleID = 0;


                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value

                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements
                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }

                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                    if (request.DatagridThresold > 0)
                    {
                        _DatagridThresold = request.DatagridThresold;
                    }
                    if (request.LoginId > 0)
                    {
                        _loginID = request.LoginId;
                    }
                    if (request.RoleId > 0)
                    {
                        _roleID = request.RoleId;
                    }
                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];

                // Prepare parameters for RestClient or any service you're using
                DataTableForBranchGridData dataRequest1 = new DataTableForBranchGridData
                {
                    page = page, // Export all data on a single page
                    pageSize = pageSize, // Export all rows
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DatagridThresold = _DatagridThresold,
                    Export_flg = 1,
                    CustomFilters = customFilters.ToList(),
                    LoginId = _loginID,
                    RoleId = _roleID
                };
                // Generate Excel content
                DataTable _dataTablebranch = RestClient.GetBranchExcelGridData(dataRequest1);
                var result = new ExportExcelFileMaster().SaveExcelToServer(_dataTablebranch, "BranchData", "BranchData_");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while processing your request. Please try again later." });
            }
        }
        #endregion
        

        #region For DesignationHighLevelGridData
        public async Task<ActionResult> GetPaginatedDesignation(EmployeeOnBoardingDataTablesRequest request)
        {
            try
            {
                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = ""; // Default sort column
                string sortOrder = "";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 DataGridValues = request.DatagridThresold;
                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value

                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }                  
                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    else
                    {
                        sortColumn = "DesignationName";
                        sortOrder = "asc";
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }

                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];
                // Prepare parameters for RestClient or any service you're using
                DataTableForEmployeeOnBoardingGridData dataRequest = new DataTableForEmployeeOnBoardingGridData
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    OnBoardingID = request.OnBoardingID,
                    RoleId = request.RoleId,
                    LoginEmpId = request.LoginEmpId,
                    ExportFlag = 0,
                    CustomFilters = customFilters.ToList()
                };

                var Result = RestClient.GetPaginatedDesignationAsync(dataRequest);

                var paginatedResult = JsonConvert.DeserializeObject<DataSet>(Result);

                // Ensure the DataSet has necessary tables
                if (paginatedResult.Tables.Count < 2)
                {
                    throw new Exception("Invalid dataset structure. Tables missing.");
                }

                var dataTable = paginatedResult.Tables["Table"];    // Actual data
                var metadataTable = paginatedResult.Tables["Table1"]; // Metadata table

                var totalRecords = Convert.ToInt32(metadataTable.Rows[0]["TotalRecords"]);
                var filteredRecords = totalRecords;  // Adjust filtered records if necessary

                var response = new
                {
                    draw = request.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = dataTable
                };

                // Manually serialize the response using Json.NET
                var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    MaxDepth = int.MaxValue,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                // Return the serialized JSON response
                return Content(jsonResponse, "application/json");
            }
            catch (ArgumentNullException ex)
            {
                // Handle null argument errors
                return Json(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Handle argument-related issues
                return Json(new { error = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                // Handle issues with the HTTP client or REST API
                return Json(new { error = "There was an issue fetching data from the server. Please try again later." });
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                // Example: Logger.LogError(ex, "Error in GetPaginatedEmployeeDirectory");
                return Json(new { error = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpPost]
        public JsonResult GetAllDesignationExcelData(EmployeeOnBoardingDataTablesRequest request)
        {
            try
            {
                string searchTerm = string.Empty;
                string sortColumn = "";
                string sortOrder = "";
                int pageSize = 10;
                int page = 1;
                Int64 DataGridValues = 0;
                // Check if request is not null
                if (request != null)
                {
                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements
                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    else
                    {
                        sortColumn = "DesignationName";
                        sortOrder = "asc";
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];

                // Prepare parameters for RestClient or any service you're using
                EmployeeOnBoardingDataTablesRequest dataRequest = new EmployeeOnBoardingDataTablesRequest
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    OnBoardingID = request.OnBoardingID,
                    RoleId = request.RoleId,
                    CompanyID = request.CompanyID,
                    BranchID = request.BranchID,
                    LoginEmpId = request.LoginEmpId,
                    DeaprtmentID = request.DeaprtmentID,
                    DesignationID = request.DesignationID,
                    CustomFilters = customFilters.ToList()
                };
                // Generate Excel content
                DataTable dataTableDesmaster = RestClient.GetDesignationExcelGridData(dataRequest);                
                var result = new ExportExcelFileMaster().SaveExcelToServer(dataTableDesmaster, "DesignationMasterData", "DesignationMasterData_");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while processing your request. Please try again later." });
            }
        }
        #endregion

        #region For DepartmentHighLevelGridData
        public async Task<ActionResult> GetPaginatedDepartment(EmployeeOnBoardingDataTablesRequest request)
        {
            try
            {
                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = ""; // Default sort column
                string sortOrder = "";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 DataGridValues = request.DatagridThresold;
                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value

                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements

                    if (request.order != null && request.order.Length > 0)
                    {                        
                        sortColumn = request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    else
                    {
                        sortColumn = "DepartmentId";
                        sortOrder = "asc";
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                }

                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];                
                DataTableForEmployeeOnBoardingGridData dataRequest = new DataTableForEmployeeOnBoardingGridData
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    OnBoardingID = request.OnBoardingID,
                    RoleId = request.RoleId,
                    LoginEmpId = request.LoginEmpId,
                    ExportFlag = 0,
                    CustomFilters = customFilters.ToList()
                };

                var Result = RestClient.GetPaginatedDepartmentAsync(dataRequest);

                var paginatedResult = JsonConvert.DeserializeObject<DataSet>(Result);

                // Ensure the DataSet has necessary tables
                if (paginatedResult.Tables.Count < 2)
                {
                    throw new Exception("Invalid dataset structure. Tables missing.");
                }

                var dataTable = paginatedResult.Tables["Table"];    // Actual data
                var metadataTable = paginatedResult.Tables["Table1"]; // Metadata table

                var totalRecords = Convert.ToInt32(metadataTable.Rows[0]["TotalRecords"]);
                var filteredRecords = totalRecords;  // Adjust filtered records if necessary

                var response = new
                {
                    draw = request.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = dataTable
                };

                // Manually serialize the response using Json.NET
                var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    MaxDepth = int.MaxValue,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                // Return the serialized JSON response
                return Content(jsonResponse, "application/json");
            }
            catch (ArgumentNullException ex)
            {
                // Handle null argument errors
                return Json(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Handle argument-related issues
                return Json(new { error = ex.Message });
            }
            catch (HttpRequestException)
            {
                // Handle issues with the HTTP client or REST API
                return Json(new { error = "There was an issue fetching data from the server. Please try again later." });
            }
            catch (Exception)
            {
                // Log exception for debugging
                // Example: Logger.LogError(ex, "Error in GetPaginatedEmployeeDirectory");
                return Json(new { error = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpPost]
        public JsonResult GetAllDepartmentExcelData(EmployeeOnBoardingDataTablesRequest request)
        {
            try
            {
                string searchTerm = string.Empty;
                string sortColumn = "";
                string sortOrder = "";
                int pageSize = 10;
                int page = 1;
                Int64 DataGridValues = 0;
                // Check if request is not null
                if (request != null)
                {
                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements

                    if (request.order != null && request.order.Length > 0)
                    {                      
                        sortColumn = request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    else
                    {
                        sortColumn = "DepartmentId";
                        sortOrder = "asc";
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }                    
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];                
                EmployeeOnBoardingDataTablesRequest dataRequest = new EmployeeOnBoardingDataTablesRequest
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    OnBoardingID = request.OnBoardingID,
                    RoleId = request.RoleId,
                    CompanyID = request.CompanyID,
                    BranchID = request.BranchID,
                    LoginEmpId = request.LoginEmpId,
                    DeaprtmentID = request.DeaprtmentID,
                    DesignationID = request.DesignationID,
                    CustomFilters = customFilters.ToList()
                };
                DataTable _datatabledepartment = RestClient.GetDepartmentExcelGridData(dataRequest);
                var result = new ExportExcelFileMaster().SaveExcelToServer(_datatabledepartment, "DepartmentMasterData", "DepartmentMasterData_");
                return Json(result, JsonRequestBehavior.AllowGet);            
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while processing your request. Please try again later." });
            }
        }
        #endregion

        #region For EmployeeHighLevelGridData
        public async Task<JsonResult> GetPaginatedEmployees([System.Web.Http.FromBody] DataTablesRequestExtended request)
        {
            try
            {


                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = "EmpID"; // Default sort column
                string sortOrder = "asc";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 _DatagridThresold = 0;
                int _ExportFlage = 0;

                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value

                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements
                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }

                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                    if (request.DatagridThresold > 0)
                    {
                        _DatagridThresold = request.DatagridThresold;
                    }
                    if (request.ExportFlage > 0)
                    {
                        _ExportFlage = request.ExportFlage;
                    }
                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];
                // Create EmployeeFilter object

                EnrollCommonMasterFilter employeeFilter = null;
                if (request.EmployeeFilter != null)
                {
                    employeeFilter = new EnrollCommonMasterFilter
                    {
                        CompanyIDs = request.EmployeeFilter.CompanyIDs,
                        BranchIDs = request.EmployeeFilter.BranchIDs,
                        DepartmentIDs = request.EmployeeFilter.DepartmentIDs,
                        DesignationIDs = request.EmployeeFilter.DesignationIDs
                    };
                }

                var RoleFilter = new RoleWiseFilter
                {
                    CompanyID = request.RoleWiseFilter.CompanyID,
                    BranchID = request.RoleWiseFilter.BranchID,
                    EmpID = request.RoleWiseFilter.EmpID
                };

                // Prepare parameters for RestClient or any service you're using
                DataTableForEmployeeGridData dataRequest = new DataTableForEmployeeGridData
                {
                    //page = page,
                    //pageSize = pageSize,
                    page = page, // Export all data on a single page
                    pageSize = pageSize, // Export all rows
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DatagridThresold = _DatagridThresold,
                    ExportFlage = _ExportFlage,
                    CustomFilters = customFilters.ToList(),
                    EmployeeFilter = employeeFilter != null ? employeeFilter : new EnrollCommonMasterFilter(),
                    RoleWiseFilter = RoleFilter != null ? RoleFilter : new RoleWiseFilter()
                };

                var paginatedResult = RestClient.GetPaginatedEmployeesAsync(dataRequest);
                return Json(new
                {
                    draw = request.draw,
                    recordsTotal = paginatedResult.Result.totalRecords,
                    recordsFiltered = paginatedResult.Result.totalRecords,
                    data = paginatedResult.Result
                });
            }
            catch (ArgumentNullException ex)
            {
                // Handle null argument errors
                return Json(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Handle argument-related issues
                return Json(new { error = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                // Handle issues with the HTTP client or REST API
                return Json(new { error = "There was an issue fetching data from the server. Please try again later." });
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                // Example: Logger.LogError(ex, "Error in GetPaginatedEmployeeDirectory");
                return Json(new { error = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpPost]
        public JsonResult GetAllEmployeeExcelData(DataTablesRequestExtended request)
        {
            try
            {
                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = "EmpID"; // Default sort column
                string sortOrder = "asc";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 _DatagridThresold = 0;


                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value

                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements
                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }

                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                    if (request.DatagridThresold > 0)
                    {
                        _DatagridThresold = request.DatagridThresold;
                    }
                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];
                EnrollCommonMasterFilter employeeFilter;
                RoleWiseFilter RoleFilter;

                // Create EmployeeFilter object
                if (request.EmployeeFilter != null)
                {
                    employeeFilter = new EnrollCommonMasterFilter
                    {
                        CompanyIDs = request.EmployeeFilter.CompanyIDs,
                        BranchIDs = request.EmployeeFilter.BranchIDs,
                        DepartmentIDs = request.EmployeeFilter.DepartmentIDs,
                        DesignationIDs = request.EmployeeFilter.DesignationIDs
                    };
                }
                else
                {
                    employeeFilter = new EnrollCommonMasterFilter(); // Create an empty filter object
                }

                if (request.RoleWiseFilter != null)
                {
                    RoleFilter = new RoleWiseFilter
                    {
                        CompanyID = request.RoleWiseFilter.CompanyID,
                        BranchID = request.RoleWiseFilter.BranchID,
                        EmpID = request.RoleWiseFilter.EmpID
                    };
                }
                else
                {
                    RoleFilter = new RoleWiseFilter(); // Create an empty filter object
                }
                // Prepare parameters for RestClient or any service you're using
                DataTableForEmployeeGridData dataRequest1 = new DataTableForEmployeeGridData
                {
                    //page = page,
                    //pageSize = pageSize,
                    page = page, // Export all data on a single page
                    pageSize = pageSize, // Export all rows
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DatagridThresold = _DatagridThresold,
                    ExportFlage = 1,
                    CustomFilters = customFilters.ToList(),
                    EmployeeFilter = employeeFilter != null ? employeeFilter : new EnrollCommonMasterFilter(),
                    RoleWiseFilter = RoleFilter != null ? RoleFilter : new RoleWiseFilter()
                };
                // Generate Excel content
                DataTable _dataTableEmployee = RestClient.GetEmployeeExcelGridData(dataRequest1);
                var result = new ExportExcelFileMaster().SaveExcelToServer(_dataTableEmployee, "EmployeesData", "EmployeesData_");
                return Json(result, JsonRequestBehavior.AllowGet);

                //var byteArray = GenerateExcelContent(_dataTableEmployee);
                //// Get the application root directory                
                //var appRootPath = Server.MapPath("~/");
                //var folderName = "ExportsExcelData";
                //var fileDirectory = Path.Combine(appRootPath, folderName);

                //// Ensure the directory exists (create it if it doesn't)                
                //if (!Directory.Exists(fileDirectory))
                //{
                //    Directory.CreateDirectory(fileDirectory);
                //}

                //// Delete only files starting with "EmployeesData_" in the folder
                //var existingFiles = Directory.GetFiles(fileDirectory, "EmployeesData_*");
                //foreach (var file in existingFiles)
                //{
                //    System.IO.File.Delete(file);
                //}

                //var fileName = $"EmployeesData_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                //var filePath = Path.Combine(fileDirectory, fileName);

                //// Save the file to the directory on the server
                //System.IO.File.WriteAllBytes(filePath, byteArray);

                //// Return the URL for downloading the file
                //var downloadUrl = "/ExportsExcelData/" + fileName;
                //return Json(new { success = true, filePath = downloadUrl });

            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while processing your request. Please try again later." });
            }
        }

        #endregion

        #region For ShiftHighLevelGridData
        public async Task<ActionResult> GetPaginatedShift(EmployeeOnBoardingDataTablesRequest request)
        {
            try
            {
                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = ""; // Default sort column
                string sortOrder = "";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 DataGridValues = request.DatagridThresold;
                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value

                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements
                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    else
                    {
                        sortColumn = "sm.ShiftId";
                        sortOrder = "asc";
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }

                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }

                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];
                // Prepare parameters for RestClient or any service you're using
                DataTableForEmployeeOnBoardingGridData dataRequest = new DataTableForEmployeeOnBoardingGridData
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    OnBoardingID = request.OnBoardingID,
                    RoleId = request.RoleId,
                    LoginEmpId = request.LoginEmpId,
                    CompanyID = request.CompanyID,
                    BranchID = request.BranchID,
                    ExportFlag = 0,
                    CustomFilters = customFilters.ToList()
                };

                var Result = RestClient.GetPaginatedShiftAsync(dataRequest);

                var paginatedResult = JsonConvert.DeserializeObject<DataSet>(Result);

                // Ensure the DataSet has necessary tables
                if (paginatedResult.Tables.Count < 2)
                {
                    throw new Exception("Invalid dataset structure. Tables missing.");
                }

                var dataTable = paginatedResult.Tables["Table"];    // Actual data
                var metadataTable = paginatedResult.Tables["Table1"]; // Metadata table

                var totalRecords = Convert.ToInt32(metadataTable.Rows[0]["TotalRecords"]);
                var filteredRecords = totalRecords;  // Adjust filtered records if necessary

                var response = new
                {
                    draw = request.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = dataTable
                };

                // Manually serialize the response using Json.NET
                var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    MaxDepth = int.MaxValue,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                // Return the serialized JSON response
                return Content(jsonResponse, "application/json");
            }
            catch (ArgumentNullException ex)
            {
                // Handle null argument errors
                return Json(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Handle argument-related issues
                return Json(new { error = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                // Handle issues with the HTTP client or REST API
                return Json(new { error = "There was an issue fetching data from the server. Please try again later." });
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                // Example: Logger.LogError(ex, "Error in GetPaginatedEmployeeDirectory");
                return Json(new { error = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpPost]
        public JsonResult GetAllShiftExcelData(EmployeeOnBoardingDataTablesRequest request)
        {
            try
            {
                string searchTerm = string.Empty;
                string sortColumn = "";
                string sortOrder = "";
                int pageSize = 10;
                int page = 1;
                Int64 DataGridValues = 0;
                // Check if request is not null
                if (request != null)
                {
                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements

                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    else
                    {
                        sortColumn = "sm.ShiftId";
                        sortOrder = "asc";
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];

                // Prepare parameters for RestClient or any service you're using
                EmployeeOnBoardingDataTablesRequest dataRequest = new EmployeeOnBoardingDataTablesRequest
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    OnBoardingID = request.OnBoardingID,
                    RoleId = request.RoleId,
                    LoginEmpId = request.LoginEmpId,
                    CompanyID = request.CompanyID,
                    BranchID = request.BranchID,
                    ExportFlage = 1,
                    CustomFilters = customFilters.ToList()
                };
                DataTable _dataTableShift = RestClient.GetShiftExcelGridData(dataRequest);                
                var result = new ExportExcelFileMaster().SaveExcelToServer(_dataTableShift, "ShiftMasterData", "ShiftMasterData_");
                return Json(result, JsonRequestBehavior.AllowGet);
               
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while processing your request. Please try again later." });
            }
        }
        #endregion

        #region For HolidayHighLevelGridData
        public async Task<ActionResult> GetPaginatedHoliday(EmployeeOnBoardingDataTablesRequest request)
        {
            try
            {
                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = ""; // Default sort column
                string sortOrder = "";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 DataGridValues = request.DatagridThresold;
                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value

                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements
                    if (request.order != null && request.order.Length > 0)
                    {                    
                        sortColumn = request.columns[request.order[0].column].name;
                        sortOrder = request.order[0].dir;
                    }
                    else
                    {
                        sortColumn = "HolidayId";
                        sortOrder = "asc";
                    }

                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }

                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }

                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];
                // Prepare parameters for RestClient or any service you're using
                DataTableForEmployeeOnBoardingGridData dataRequest = new DataTableForEmployeeOnBoardingGridData
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    OnBoardingID = request.OnBoardingID,
                    RoleId = request.RoleId,
                    LoginEmpId = request.LoginEmpId,
                    CompanyID = request.CompanyID,
                    BranchID = request.BranchID,
                    ExportFlag = 0,
                    CustomFilters = customFilters.ToList()
                };

                var Result = RestClient.GetPaginatedHolidayAsync(dataRequest);

                var paginatedResult = JsonConvert.DeserializeObject<DataSet>(Result);

                // Ensure the DataSet has necessary tables
                if (paginatedResult.Tables.Count < 2)
                {
                    throw new Exception("Invalid dataset structure. Tables missing.");
                }

                var dataTable = paginatedResult.Tables["Table"];    // Actual data
                var metadataTable = paginatedResult.Tables["Table1"]; // Metadata table

                var totalRecords = Convert.ToInt32(metadataTable.Rows[0]["TotalRecords"]);
                var filteredRecords = totalRecords;  // Adjust filtered records if necessary

                var response = new
                {
                    draw = request.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = dataTable
                };

                // Manually serialize the response using Json.NET
                var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    MaxDepth = int.MaxValue,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                // Return the serialized JSON response
                return Content(jsonResponse, "application/json");
            }
            catch (ArgumentNullException ex)
            {
                // Handle null argument errors
                return Json(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Handle argument-related issues
                return Json(new { error = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                // Handle issues with the HTTP client or REST API
                return Json(new { error = "There was an issue fetching data from the server. Please try again later." });
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                // Example: Logger.LogError(ex, "Error in GetPaginatedEmployeeDirectory");
                return Json(new { error = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpPost]
        public JsonResult GetAllHolidayExcelData(EmployeeOnBoardingDataTablesRequest request)
        {
            try
            {
                string searchTerm = string.Empty;
                string sortColumn = "";
                string sortOrder = "";
                int pageSize = 10;
                int page = 1;
                Int64 DataGridValues = 0;
                // Check if request is not null
                if (request != null)
                {
                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements
                    if (request.order != null && request.order.Length > 0)
                    {
                        //sortColumn = request.columns[request.order[0].column].data == "all" ? "Tf.OnboardingID" : request.columns[request.order[0].column].data;
                        //sortOrder = request.order[0].dir;
                        sortColumn = request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    else
                    {
                        sortColumn = "HolidayId";
                        sortOrder = "asc";
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];

                // Prepare parameters for RestClient or any service you're using
                EmployeeOnBoardingDataTablesRequest dataRequest = new EmployeeOnBoardingDataTablesRequest
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    OnBoardingID = request.OnBoardingID,
                    RoleId = request.RoleId,
                    LoginEmpId = request.LoginEmpId,
                    CompanyID = request.CompanyID,
                    BranchID = request.BranchID,
                    ExportFlage = 1,
                    CustomFilters = customFilters.ToList()
                };
                // Generate Excel content
                DataTable _dataTableHoliday = RestClient.GetHolidayExcelGridData(dataRequest);
                var result = new ExportExcelFileMaster().SaveExcelToServer(_dataTableHoliday, "HolidayMasterData", "HolidayMasterData_");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while processing your request. Please try again later." });
            }
        }
        #endregion

        #region For ReligionHighLevelGridData
        public async Task<ActionResult> GetPaginatedReligion(EmployeeOnBoardingDataTablesRequest request)
        {
            try
            {
                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = ""; // Default sort column
                string sortOrder = "";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 DataGridValues = request.DatagridThresold;
                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value

                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements
                    if (request.order != null && request.order.Length > 0)
                    {
                        //sortColumn = request.columns[request.order[0].column].data == "all" ? "Tf.OnboardingID" : request.columns[request.order[0].column].data;
                        //sortOrder = request.order[0].dir;
                        sortColumn = request.columns[request.order[0].column].name;
                        sortOrder = request.order[0].dir;
                    }
                    else
                    {
                        sortColumn = "ReligionName";
                        sortOrder = "asc";
                    }

                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }

                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }

                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];
                // Prepare parameters for RestClient or any service you're using
                DataTableForEmployeeOnBoardingGridData dataRequest = new DataTableForEmployeeOnBoardingGridData
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    OnBoardingID = request.OnBoardingID,
                    RoleId = request.RoleId,
                    LoginEmpId = request.LoginEmpId,
                    CompanyID = request.CompanyID,
                    BranchID = request.BranchID,
                    ExportFlag = 0,
                    CustomFilters = customFilters.ToList()
                };

                var Result = RestClient.GetPaginatedReligionAsync(dataRequest);

                var paginatedResult = JsonConvert.DeserializeObject<DataSet>(Result);

                // Ensure the DataSet has necessary tables
                if (paginatedResult.Tables.Count < 2)
                {
                    throw new Exception("Invalid dataset structure. Tables missing.");
                }

                var dataTable = paginatedResult.Tables["Table"];    // Actual data
                var metadataTable = paginatedResult.Tables["Table1"]; // Metadata table

                var totalRecords = Convert.ToInt32(metadataTable.Rows[0]["TotalRecords"]);
                var filteredRecords = totalRecords;  // Adjust filtered records if necessary

                var response = new
                {
                    draw = request.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = dataTable
                };

                // Manually serialize the response using Json.NET
                var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    MaxDepth = int.MaxValue,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                // Return the serialized JSON response
                return Content(jsonResponse, "application/json");
            }
            catch (ArgumentNullException ex)
            {
                // Handle null argument errors
                return Json(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Handle argument-related issues
                return Json(new { error = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                // Handle issues with the HTTP client or REST API
                return Json(new { error = "There was an issue fetching data from the server. Please try again later." });
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                // Example: Logger.LogError(ex, "Error in GetPaginatedEmployeeDirectory");
                return Json(new { error = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpPost]
        public JsonResult GetAllReligionExcelData(EmployeeOnBoardingDataTablesRequest request)
        {
            try
            {
                string searchTerm = string.Empty;
                string sortColumn = "";
                string sortOrder = "";
                int pageSize = 10;
                int page = 1;
                Int64 DataGridValues = 0;
                // Check if request is not null
                if (request != null)
                {
                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements
                    if (request.order != null && request.order.Length > 0)
                    {
                        //sortColumn = request.columns[request.order[0].column].data == "all" ? "Tf.OnboardingID" : request.columns[request.order[0].column].data;
                        //sortOrder = request.order[0].dir;
                        sortColumn = request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    else
                    {
                        sortColumn = "ReligionName";
                        sortOrder = "asc";
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];

                // Prepare parameters for RestClient or any service you're using
                EmployeeOnBoardingDataTablesRequest dataRequest = new EmployeeOnBoardingDataTablesRequest
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    OnBoardingID = request.OnBoardingID,
                    RoleId = request.RoleId,
                    LoginEmpId = request.LoginEmpId,
                    CompanyID = request.CompanyID,
                    BranchID = request.BranchID,
                    ExportFlage = 1,
                    CustomFilters = customFilters.ToList()
                };
                // Generate Excel File content  
                DataTable _dataTableReligion = RestClient.GetReligionExcelGridData(dataRequest);                
                var result = new ExportExcelFileMaster().SaveExcelToServer(_dataTableReligion, "Religion", "Religion_");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while processing your request. Please try again later." });
            }
        }
        #endregion

        #region Device Master
        [HttpPost]
        public async Task<ActionResult> GetPaginatedDeviceMaster(DeviceMasterGridReqDto request)
        {
            try
            {
                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = "Branch Name"; // Default sort column
                string sortOrder = "asc";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 DataGridValues = 0;
                var customFilters = new CustomSearchFilter[0];
                int _ExportFlage = 0;
                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value

                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data == null ? "Branch Name" : request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                    if (request.search.value != null)
                    {
                        searchTerm = request.search.value.Trim();
                    }
                    if (request.ExportFlage > 0)
                    {
                        _ExportFlage = request.ExportFlage;
                    }
                    if (request.BranchId == null)
                    {
                        request.BranchId = "";
                    }
                    if (request.CompanyId == null)
                    {
                        request.CompanyId = 0;
                    }
                    if (request.CompanyCode == null)
                    {
                        request.CompanyCode = "";
                    }
                    customFilters = request.CustFilter ?? new CustomSearchFilter[0];
                }

                // Prepare parameters for RestClient or any service you're using
                DeviceMasterGridReq dataRequest = new DeviceMasterGridReq
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    RoleId = request.RoleId,
                    CompanyId = request.CompanyId,
                    BranchId = request.BranchId,
                    CompanyCode = request.CompanyCode,
                    CustomFilters = customFilters.ToList(),
                    Export_flg = _ExportFlage,
                };



                var Result = RestClient.GetPaginatedDeviceMasterAsync(dataRequest);
                var paginatedResult = JsonConvert.DeserializeObject<DataSet>(Result);

                // Ensure the DataSet has necessary tables
                if (paginatedResult.Tables.Count < 2)
                {
                    throw new Exception("Invalid dataset structure. Tables missing.");
                }

                var dataTable = paginatedResult.Tables["Table"];    // Actual data
                var metadataTable = paginatedResult.Tables["Table1"]; // Metadata table

                var totalRecords = Convert.ToInt32(metadataTable.Rows[0]["TotalRecords"]);
                var filteredRecords = totalRecords;  // Adjust filtered records if necessary

                var response = new
                {
                    draw = request.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = dataTable
                };

                // Manually serialize the response using Json.NET
                var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    MaxDepth = int.MaxValue,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                // Return the serialized JSON response
                return Content(jsonResponse, "application/json");
            }
            catch (ArgumentNullException ex)
            {
                // Handle null argument errors
                return Json(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Handle argument-related issues
                return Json(new { error = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                // Handle issues with the HTTP client or REST API
                return Json(new { error = "There was an issue fetching data from the server. Please try again later." });
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                // Example: Logger.LogError(ex, "Error in GetPaginatedEmployeeDirectory");
                return Json(new { error = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpPost]
        public JsonResult GetAllDeviceExcelData(DeviceMasterGridReqDto request)
        {
            try
            {
                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = "Branch Name"; // Default sort column
                string sortOrder = "asc";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 DataGridValues = 0;
                var customFilters = new CustomSearchFilter[0];
                int _ExportFlage = 1;
                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value

                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data == null ? "Branch Name" : request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                    if (request.search != null && request.search.value != null)
                    {
                        searchTerm = request.search.value.Trim();
                    }
                    if (request.ExportFlage > 0)
                    {
                        _ExportFlage = request.ExportFlage;
                    }
                    if (request.BranchId == null)
                    {
                        request.BranchId = "";
                    }
                    if (request.CompanyId == null)
                    {
                        request.CompanyId = 0;
                    }
                    if (request.CompanyCode == null)
                    {
                        request.CompanyCode = "";
                    }
                    customFilters = request.CustFilter ?? new CustomSearchFilter[0];
                }

                // Prepare parameters for RestClient or any service you're using
                DeviceMasterGridReq dataRequest = new DeviceMasterGridReq
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    RoleId = request.RoleId,
                    CompanyId = request.CompanyId,
                    BranchId = request.BranchId,
                    CompanyCode = request.CompanyCode,
                    CustomFilters = customFilters.ToList(),
                    Export_flg = _ExportFlage,
                };
                // Generate Excel File content
                DataTable _dataTableDevice = RestClient.GetDeviceMasterExcelGridData(dataRequest);                                 
                var result = new ExportExcelFileMaster().SaveExcelToServer(_dataTableDevice, "DeviceMasterData", "DeviceMasterData_");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while processing your request. Please try again later." });
            }
        }
        #endregion

        #region For EnrollUserHighLevelGridData
        public async Task<JsonResult> GetPaginatedEnrollUser(UserEnrollDataTablesRequest request)
        {
            // Initialize variables
            string searchTerm = string.Empty;
            string sortColumn = "EmpPunchID"; // Default sort column
            string sortOrder = "asc";     // Default sort order
            int pageSize = 10;            // Default page size
            int page = 1;                 // Default page number            
            Int64 DataGridValues = 0;
            // Check if request is not null
            if (request != null)
            {
                // Check if search object is present and has value

                if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                {
                    searchTerm = request.search.value;
                }
                // Check if order array is present and has elements
                if (request.order != null && request.order.Length > 0)
                {
                    sortColumn = request.columns[request.order[0].column].data;
                    sortOrder = request.order[0].dir;
                }
                if (request.DatagridThresold > 0)
                {
                    DataGridValues = request.DatagridThresold;
                }
                // Set pageSize and page if length and start are set
                if (request.length > 0)
                {
                    pageSize = request.length;
                }
                if (request.start >= 0)
                {
                    page = (request.start / pageSize) + 1;
                }
            }
            var customFilters = request.CustFilter ?? new CustomSearchFilter[0];
            // Prepare parameters for RestClient or any service you're using
            DataTableForEnrollUserGridData dataRequest = new DataTableForEnrollUserGridData
            {
                page = page,
                pageSize = pageSize,
                searchTerm = searchTerm,
                sortOrder = sortOrder,
                sortColumn = sortColumn,
                DataGridValues = DataGridValues,
                StatusId = request.StatusId,
                BranchId = request.EmployeeFilter.BranchIDs,
                CmpId = request.EmployeeFilter.CompanyIDs,
                DptId = request.EmployeeFilter.DepartmentIDs,
                SelectAll = request.SelectAll,
                SelectAllSearchTerm = request.SelectAllSearchTerm
            };
            var paginatedResult = RestClient.GetPaginatedEnrollUserAsync(dataRequest);
            return Json(new
            {
                draw = request.draw,
                recordsTotal = paginatedResult.Result.TotalItems,
                recordsFiltered = paginatedResult.Result.TotalItems,
                data = paginatedResult.Result
            });
        }
        #endregion

        #region For DirectoryDirectoryHighLevelGridData
        public async Task<ActionResult> GetPaginatedEmployeeDirectory(EmployeeDirectoryDataTablesRequest request)
        {
            try
            {
                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = "OnboardingID"; // Default sort column
                string sortOrder = "asc";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 DataGridValues = 0;
                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value

                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements
                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data == "all" ? "OnboardingID" : request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }

                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];
                // Prepare parameters for RestClient or any service you're using
                DataTableForEmployeeDirectoryGridData dataRequest = new DataTableForEmployeeDirectoryGridData
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    OnBoardingID = request.OnBoardingID,
                    RoleId = request.RoleId,
                    CompanyId = request.CompanyId,
                    BranchId = request.BranchId,
                    LoginEmpId = request.LoginEmpId,
                    DepartmentId = request.DepartmentId,
                    DesignationId = request.DesignationId,
                    DeactivateID = request.DeactivateID,
                    CustomFilters = customFilters.ToList()
                };


                var Result = RestClient.GetPaginatedEmployeeAsync(dataRequest);

                var paginatedResult = JsonConvert.DeserializeObject<DataSet>(Result);

                // Ensure the DataSet has necessary tables
                if (paginatedResult.Tables.Count < 2)
                {
                    throw new Exception("Invalid dataset structure. Tables missing.");
                }

                var dataTable = paginatedResult.Tables["Table"];    // Actual data
                var metadataTable = paginatedResult.Tables["Table1"]; // Metadata table

                var totalRecords = Convert.ToInt32(metadataTable.Rows[0]["TotalRecords"]);
                var filteredRecords = totalRecords;  // Adjust filtered records if necessary

                var response = new
                {
                    draw = request.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = dataTable
                };

                // Manually serialize the response using Json.NET
                var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    MaxDepth = int.MaxValue,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                // Return the serialized JSON response
                return Content(jsonResponse, "application/json");
            }
            catch (ArgumentNullException ex)
            {
                // Handle null argument errors
                return Json(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Handle argument-related issues
                return Json(new { error = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                // Handle issues with the HTTP client or REST API
                return Json(new { error = "There was an issue fetching data from the server. Please try again later." });
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                // Example: Logger.LogError(ex, "Error in GetPaginatedEmployeeDirectory");
                return Json(new { error = "An unexpected error occurred. Please try again later." });
            }
        }
        [HttpPost]
        public JsonResult GetAllDirectoryExcelData(EmployeeDirectoryDataTablesRequest request)
        {
            try
            {
                string searchTerm = string.Empty;
                string sortColumn = "OnboardingID"; // Default sort column
                string sortOrder = "asc";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 DataGridValues = 0;
                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value

                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements
                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data == "all" ? "OnboardingID" : request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }

                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];
                // Prepare parameters for RestClient or any service you're using
                DataTableForEmployeeDirectoryGridData dataRequest = new DataTableForEmployeeDirectoryGridData
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    OnBoardingID = request.OnBoardingID,
                    RoleId = request.RoleId,
                    CompanyId = request.CompanyId,
                    BranchId = request.BranchId,
                    LoginEmpId = request.LoginEmpId,
                    DepartmentId = request.DepartmentId,
                    DesignationId = request.DesignationId,
                    DeactivateID = request.DeactivateID,
                    CustomFilters = customFilters.ToList()
                };

                DataTable dataTable = RestClient.GetDirectoryExcelGridData(dataRequest);


                var result = new ExportExcelFileMaster().SaveExcelToServer(dataTable, "DirectoryData", "DirectoryData_");
                return Json(result, JsonRequestBehavior.AllowGet);

                //// Generate CSV content
                //var csvContent = GenerateCsvContent(dataTable);
                //var byteArray = System.Text.Encoding.UTF8.GetBytes(csvContent);

                //// Get the application root directory
                //var appRootPath = Server.MapPath("~/"); // Get the root directory of your app
                //var fileName = $"EmployeesData_{DateTime.Now:yyyyMMddHHmmss}.csv";

                //// Combine with a folder name (make sure the folder exists or create it)
                //var fileDirectory = Path.Combine(appRootPath, "ExportsExcelData");  // "Exports" folder in the root directory

                //// Ensure the directory exists (create it if it doesn't)
                //if (!Directory.Exists(fileDirectory))
                //{
                //    Directory.CreateDirectory(fileDirectory);
                //}

                //// Combine the directory and file name
                //var filePath = Path.Combine(fileDirectory, fileName);

                //// Save the file to the directory on the server
                //System.IO.File.WriteAllBytes(filePath, byteArray);

                //// Return the URL for downloading the file
                //var downloadUrl = "/ExportsExcelData/" + fileName;

                //// Return the file path or a URL for download
                //return Json(new { success = true, filePath = downloadUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while processing your request. Please try again later." });
            }
        }

        [HttpPost]

        #endregion

        #region For OnBoardingHighLevelGridData
        public async Task<ActionResult> GetPaginatedEmployeeOnBoarding(EmployeeOnBoardingDataTablesRequest request)
        {
            try
            {
                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = ""; // Default sort column
                string sortOrder = "";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 DataGridValues = request.DatagridThresold;
                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value

                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements
                    if (request.order != null && request.order.Length > 0)
                    {
                        //sortColumn = request.columns[request.order[0].column].data == "all" ? "Tf.OnboardingID" : request.columns[request.order[0].column].data;
                        //sortOrder = request.order[0].dir;
                        sortColumn = request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    else
                    {
                        sortColumn = "Tf.OnBordingID";
                        sortOrder = "asc";
                    }

                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }

                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }

                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];
                // Prepare parameters for RestClient or any service you're using
                DataTableForEmployeeOnBoardingGridData dataRequest = new DataTableForEmployeeOnBoardingGridData
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    OnBoardingID = request.OnBoardingID,
                    RoleId = request.RoleId,
                    CompanyID = request.CompanyID,
                    BranchID = request.BranchID,
                    LoginEmpId = request.LoginEmpId,
                    DeaprtmentID = request.DeaprtmentID,
                    DesignationID = request.DesignationID,
                    ExportFlag = 0,
                    CustomFilters = customFilters.ToList()
                };

                var Result = RestClient.GetPaginatedOnBoardingEmployeeAsync(dataRequest);

                var paginatedResult = JsonConvert.DeserializeObject<DataSet>(Result);

                // Ensure the DataSet has necessary tables
                if (paginatedResult.Tables.Count < 2)
                {
                    throw new Exception("Invalid dataset structure. Tables missing.");
                }

                var dataTable = paginatedResult.Tables["Table"];    // Actual data
                var dataTable1 = paginatedResult.Tables["Table1"];
                var dataTable2 = paginatedResult.Tables["Table2"];
                var metadataTable = paginatedResult.Tables["Table3"]; // Metadata table

                var totalRecords = Convert.ToInt32(metadataTable.Rows[0]["TotalRecords"]);
                var filteredRecords = totalRecords;  // Adjust filtered records if necessary

                // Return the result in the expected DataTable format
                // Create the response object
                var response = new
                {
                    draw = request.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = new
                    {
                        Table = dataTable,
                        Table1 = dataTable1,
                        Table2 = dataTable2
                    }
                };

                // Manually serialize the response using Json.NET
                var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    MaxDepth = int.MaxValue,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                // Return the serialized JSON response
                return Content(jsonResponse, "application/json");
            }
            catch (ArgumentNullException ex)
            {
                // Handle null argument errors
                return Json(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Handle argument-related issues
                return Json(new { error = ex.Message });
            }
            catch (HttpRequestException)
            {
                // Handle issues with the HTTP client or REST API
                return Json(new { error = "There was an issue fetching data from the server. Please try again later." });
            }
            catch (Exception)
            {
                // Log exception for debugging
                // Example: Logger.LogError(ex, "Error in GetPaginatedEmployeeOnBoarding");
                return Json(new { error = "An unexpected error occurred. Please try again later." });
            }
        }


        [HttpPost]
        public JsonResult GetAllOnBoardingExcelData(EmployeeOnBoardingDataTablesRequest request)
        {
            try
            {
                string searchTerm = string.Empty;
                string sortColumn = "";
                string sortOrder = "";
                int pageSize = 10;
                int page = 1;
                Int64 DataGridValues = 0;
                // Check if request is not null
                if (request != null)
                {
                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                    // Check if order array is present and has elements
                    if (request.order != null && request.order.Length > 0)
                    {
                        //sortColumn = request.columns[request.order[0].column].data == "all" ? "Tf.OnboardingID" : request.columns[request.order[0].column].data;
                        //sortOrder = request.order[0].dir;
                        sortColumn = request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    else
                    {
                        sortColumn = "Tf.OnBordingID";
                        sortOrder = "asc";
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                }
                var customFilters = request.CustFilter ?? new CustomSearchFilter[0];

                // Prepare parameters for RestClient or any service you're using
                EmployeeOnBoardingDataTablesRequest dataRequest = new EmployeeOnBoardingDataTablesRequest
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    OnBoardingID = request.OnBoardingID,
                    RoleId = request.RoleId,
                    CompanyID = request.CompanyID,
                    BranchID = request.BranchID,
                    LoginEmpId = request.LoginEmpId,
                    DeaprtmentID = request.DeaprtmentID,
                    DesignationID = request.DesignationID,
                    CustomFilters = customFilters.ToList()
                };
                DataTable dataTable = RestClient.GetOnBoardingExcelGridData(dataRequest);

                var result = new ExportExcelFileMaster().SaveExcelToServer(dataTable, "OnBoardingData", "OnBoardingData_");
                return Json(result, JsonRequestBehavior.AllowGet);

                //// Generate CSV content
                //var csvContent = GenerateCsvContent(dataTable);
                //var byteArray = System.Text.Encoding.UTF8.GetBytes(csvContent);

                //// Get the application root directory
                //var appRootPath = Server.MapPath("~/"); // Get the root directory of your app
                //var fileName = $"OnBoardingData_{DateTime.Now:yyyyMMddHHmmss}.csv";

                //// Combine with a folder name (make sure the folder exists or create it)
                //var fileDirectory = Path.Combine(appRootPath, "ExportsExcelData");  // "Exports" folder in the root directory

                //// Ensure the directory exists (create it if it doesn't)
                //if (!Directory.Exists(fileDirectory))
                //{
                //    Directory.CreateDirectory(fileDirectory);
                //}

                //// Combine the directory and file name
                //var filePath = Path.Combine(fileDirectory, fileName);

                //// Save the file to the directory on the server
                //System.IO.File.WriteAllBytes(filePath, byteArray);

                //// Return the URL for downloading the file
                //var downloadUrl = "/ExportsExcelData/" + fileName;

                //// Return the file path or a URL for download
                //return Json(new { success = true, filePath = downloadUrl });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while processing your request. Please try again later." });
            }
        }
        #endregion

        #region AdminDashBoard
        [HttpPost]
        public async Task<ActionResult> GetPaginatedAdminDashBoard(AdminDashBoardDto request)
        {
            try
            {
                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = ""; // Default sort column
                string sortOrder = "";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 DataGridValues = 0;
                var customFilters = new CustomSearchFilter[0];
                int _ExportFlage = 0;
                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value

                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data == null ? "" : request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                    if (request.search.value != null)
                    {
                        searchTerm = request.search.value.Trim();
                    }
                    if (request.ExportFlage > 0)
                    {
                        _ExportFlage = request.ExportFlage;
                    }
                    if (request.BranchId == null)
                    {
                        request.BranchId = "";
                    }
                    if (request.CompanyId == null)
                    {
                        request.CompanyId = 0;
                    }
                    
                   
                }

                // Prepare parameters for RestClient or any service you're using
                AdminDashBoardReq dataRequest = new AdminDashBoardReq
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    
                    CompanyId = request.CompanyId,
                    BranchId = request.BranchId,
                    ReqID = request.ReqID,
                    Export_flg = _ExportFlage,
                };



                var Result = RestClient.GetPaginatedAdminDashBoardAsync(dataRequest);
                var paginatedResult = JsonConvert.DeserializeObject<DataSet>(Result);

                // Ensure the DataSet has necessary tables
                if (paginatedResult.Tables.Count < 2)
                {
                    throw new Exception("Invalid dataset structure. Tables missing.");
                }

                var dataTable = paginatedResult.Tables["Table"];    // Actual data
                var metadataTable = paginatedResult.Tables["Table1"]; // Metadata table

                var totalRecords = Convert.ToInt32(metadataTable.Rows[0]["TotalRecords"]);
                var filteredRecords = totalRecords;  // Adjust filtered records if necessary

                var response = new
                {
                    draw = request.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = dataTable
                };

                // Manually serialize the response using Json.NET
                var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    MaxDepth = int.MaxValue,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                // Return the serialized JSON response
                return Content(jsonResponse, "application/json");
            }
            catch (ArgumentNullException ex)
            {
                // Handle null argument errors
                return Json(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Handle argument-related issues
                return Json(new { error = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                // Handle issues with the HTTP client or REST API
                return Json(new { error = "There was an issue fetching data from the server. Please try again later." });
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                // Example: Logger.LogError(ex, "Error in GetPaginatedEmployeeDirectory");
                return Json(new { error = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpPost]
        public JsonResult GetAllAdminDashBoardExcelData(AdminDashBoardDto request)
        {
            try
            {
                // Initialize default values
                string searchTerm = string.Empty;
                string sortColumn = "";
                string sortOrder = "";
                int pageSize = 10;
                int page = 1;
                long dataGridValues = 0;
                string fileName = "EmployeeDashboardData";
                int exportFlag = 1;

                if (request != null)
                {
                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data ?? "";
                        sortOrder = request.order[0].dir;
                    }

                    if (request.DatagridThresold > 0)
                        dataGridValues = request.DatagridThresold;

                    if (request.length > 0)
                        pageSize = request.length;

                    if (request.start >= 0)
                        page = (request.start / pageSize) + 1;

                    if (request.search?.value != null)
                        searchTerm = request.search.value.Trim();

                    if (request.ExportFlage > 0)
                        exportFlag = request.ExportFlage;

                    if (request.BranchId == null)
                        request.BranchId = "";

                    if (request.CompanyId == null)
                        request.CompanyId = 0;
                }

                var dataRequest = new AdminDashBoardReq
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = dataGridValues,
                    CompanyId = request.CompanyId,
                    BranchId = request.BranchId,
                    ReqID = request.ReqID,
                    Export_flg = exportFlag,
                };

                DataTable dataTable = RestClient.GetAdminDashBoardExcelGridData(dataRequest);

                // Adjust file name based on ReqID
                switch (request.ReqID)
                {
                    case 1: fileName = "CheckedIn"; break;
                    case 3: fileName = "LateClockIn"; break;
                    case 4: fileName = "EarlyClockOut"; break;
                    case 6: fileName = "NotCheckedIn"; break;
                }

                // Generate Excel file content
                var excelBytes = GenerateExcelContent(dataTable, fileName);

                var appRootPath = Server.MapPath("~/");
                var folderName = "ExportsExcelData";
                var fileDirectory = Path.Combine(appRootPath, folderName);

                if (!Directory.Exists(fileDirectory))
                    Directory.CreateDirectory(fileDirectory);

                // Delete existing files with the same pattern
                var existingFiles = Directory.GetFiles(fileDirectory, $"{fileName}_Data*");
                foreach (var file in existingFiles)
                    System.IO.File.Delete(file);

                var fullFileName = $"{fileName}_Data_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var fullFilePath = Path.Combine(fileDirectory, fullFileName);

                System.IO.File.WriteAllBytes(fullFilePath, excelBytes);

                var downloadUrl = $"/{folderName}/{fullFileName}";

                return Json(new { success = true, filePath = downloadUrl });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while processing your request. Please try again later." });
            }
        }
        #endregion

        #region AttendanceSummary
        [HttpPost]
        public async Task<ActionResult> GetPaginatedAttendanceSummary(AttanSummaryGridReq request)
        {
            try
            {
                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = "Name"; // Default sort column
                string sortOrder = "asc";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 DataGridValues = 0;
                var customFilters = new CustomSearchFilter[0];
                int _ExportFlage = 0;
                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value
                    if (request.Employee == null) request.Employee = "";
                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data == null ? "Name" : request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                    if (request.search.value != null)
                    {
                        searchTerm = request.search.value.Trim();
                    }
                    if (request.ExportFlage > 0)
                    {
                        _ExportFlage = request.ExportFlage;
                    }
                    
                    customFilters = request.CustFilter ?? new CustomSearchFilter[0];
                }

                // Prepare parameters for RestClient or any service you're using
                AttanSummaryDto dataRequest = new AttanSummaryDto
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    Month = request.Month,
                    Year = request.Year,
                    loginemployeeid= request.loginemployeeid,
                    RoleID = request.RoleID,
                    SelectAll = request.SelectAll,
                    SelectAllSearchTerm = request.SelectAllSearchTerm,
                    status = request.status,
                    Employee = request.Employee,
                    CustomFilters = customFilters.ToList(),
                    Export_flg = _ExportFlage,
                };



                var Result = RestClient.GetPaginatedAttendanceSummaryAsync(dataRequest);
                var paginatedResult = JsonConvert.DeserializeObject<DataSet>(Result);

                // Ensure the DataSet has necessary tables
                if (paginatedResult.Tables.Count < 2)
                {
                    throw new Exception("Invalid dataset structure. Tables missing.");
                }

                var dataTable = paginatedResult.Tables["Table"];    // Actual data
                var metadataTable = paginatedResult.Tables["Table1"]; // Metadata table

                var totalRecords = Convert.ToInt32(metadataTable.Rows[0]["TotalRecords"]);
                var filteredRecords = totalRecords;  // Adjust filtered records if necessary

                var response = new
                {
                    draw = request.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = dataTable
                };

                // Manually serialize the response using Json.NET
                var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    MaxDepth = int.MaxValue,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                // Return the serialized JSON response
                return Content(jsonResponse, "application/json");
            }
            catch (ArgumentNullException ex)
            {
                // Handle null argument errors
                return Json(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Handle argument-related issues
                return Json(new { error = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                // Handle issues with the HTTP client or REST API
                return Json(new { error = "There was an issue fetching data from the server. Please try again later." });
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                // Example: Logger.LogError(ex, "Error in GetPaginatedEmployeeDirectory");
                return Json(new { error = "An unexpected error occurred. Please try again later." });
            }
        }
        [HttpPost]
        public JsonResult GetAllDeviceAttendanceSummaryExcelData(AttanSummaryGridReq request)
        {
            try
            {
                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = "Name"; // Default sort column
                string sortOrder = "asc";     // Default sort order
                int pageSize = 10;            // Default page size
                int page = 1;                 // Default page number            
                Int64 DataGridValues = 0;
                var customFilters = new CustomSearchFilter[0];
                int _ExportFlage = 1;
                // Check if request is not null
                if (request != null)
                {
                    // Check if search object is present and has value

                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data == null ? "Branch Name" : request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    // Set pageSize and page if length and start are set
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                    if (request.search != null && request.search.value != null)
                    {
                        searchTerm = request.search.value.Trim();
                    }
                    if (request.ExportFlage > 0)
                    {
                        _ExportFlage = request.ExportFlage;
                    }
                    if(request.Employee == null)
                    {
                        request.Employee = "";
                    }
                    
                    customFilters = request.CustFilter ?? new CustomSearchFilter[0];
                }

                // Prepare parameters for RestClient or any service you're using
                AttanSummaryDto dataRequest = new AttanSummaryDto
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    Month = request.Month,
                    Year = request.Year,
                    loginemployeeid = request.loginemployeeid,
                    RoleID = request.RoleID,
                    SelectAll = request.SelectAll,
                    SelectAllSearchTerm = request.SelectAllSearchTerm,
                    status = request.status,
                    Employee = request.Employee,
                    CustomFilters = customFilters.ToList(),
                    Export_flg = _ExportFlage,
                };
                // Generate Excel File content
                DataTable _dataTableDevice = RestClient.GetAttendanceSummaryExcelGridData(dataRequest);
                var result = new ExportExcelFileMaster().SaveExcelToServer(_dataTableDevice, "AttendanceSummaryData", "AttendanceSummaryData_");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while processing your request. Please try again later." });
            }
        }
        #endregion

        #region BU AttendanceSummary
        [HttpPost]
        public async Task<ActionResult> GetPaginatedAttendanceSummaryBU(AttanSummaryGridReq request)
        {
            try
            {
                string searchTerm = string.Empty;
                string sortColumn = "Name";
                string sortOrder = "asc";
                int pageSize = 10;
                int page = 1;
                Int64 DataGridValues = 0;
                var customFilters = new CustomSearchFilter[0];
                int _ExportFlage = 0;
                if (request != null)
                {
                    if (request.Employee == null) request.Employee = "";
                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data == null ? "Name" : request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                    if (request.search.value != null)
                    {
                        searchTerm = request.search.value.Trim();
                    }
                    if (request.ExportFlage > 0)
                    {
                        _ExportFlage = request.ExportFlage;
                    }
                    customFilters = request.CustFilter ?? new CustomSearchFilter[0];
                }

                AttanSummaryDto dataRequest = new AttanSummaryDto
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    Month = request.Month,
                    Year = request.Year,
                    loginemployeeid = request.loginemployeeid,
                    BUId = request.BUId,
                    SelectAll = request.SelectAll,
                    SelectAllSearchTerm = request.SelectAllSearchTerm,
                    status = request.status,
                    Employee = request.Employee,
                    CustomFilters = customFilters.ToList(),
                    Export_flg = _ExportFlage,
                };

                var Result = RestClient.GetPaginatedAttendanceSummaryBUAsync(dataRequest);
                var paginatedResult = JsonConvert.DeserializeObject<DataSet>(Result);

                if (paginatedResult.Tables.Count < 2)
                {
                    throw new Exception("Invalid dataset structure. Tables missing.");
                }

                var dataTable = paginatedResult.Tables["Table"];
                var metadataTable = paginatedResult.Tables["Table1"];

                var totalRecords = Convert.ToInt32(metadataTable.Rows[0]["TotalRecords"]);
                var filteredRecords = totalRecords;

                var response = new
                {
                    draw = request.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = dataTable
                };

                var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    MaxDepth = int.MaxValue,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                return Content(jsonResponse, "application/json");
            }
            catch (ArgumentNullException ex)
            {
                return Json(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return Json(new { error = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                return Json(new { error = "There was an issue fetching data from the server. Please try again later." });
            }
            catch (Exception ex)
            {
                return Json(new { error = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpPost]
        public JsonResult GetAllDeviceAttendanceSummaryBUExcelData(AttanSummaryGridReq request)
        {
            try
            {
                string searchTerm = string.Empty;
                string sortColumn = "Name";
                string sortOrder = "asc";
                int pageSize = 10;
                int page = 1;
                Int64 DataGridValues = 0;
                var customFilters = new CustomSearchFilter[0];
                int _ExportFlage = 1;
                if (request != null)
                {
                    if (request.order != null && request.order.Length > 0)
                    {
                        sortColumn = request.columns[request.order[0].column].data == null ? "Branch Name" : request.columns[request.order[0].column].data;
                        sortOrder = request.order[0].dir;
                    }
                    if (request.DatagridThresold > 0)
                    {
                        DataGridValues = request.DatagridThresold;
                    }
                    if (request.length > 0)
                    {
                        pageSize = request.length;
                    }
                    if (request.start >= 0)
                    {
                        page = (request.start / pageSize) + 1;
                    }
                    if (request.search != null && request.search.value != null)
                    {
                        searchTerm = request.search.value.Trim();
                    }
                    if (request.ExportFlage > 0)
                    {
                        _ExportFlage = request.ExportFlage;
                    }
                    if (request.Employee == null)
                    {
                        request.Employee = "";
                    }
                    customFilters = request.CustFilter ?? new CustomSearchFilter[0];
                }

                AttanSummaryDto dataRequest = new AttanSummaryDto
                {
                    page = page,
                    pageSize = pageSize,
                    searchTerm = searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = sortColumn,
                    DataGridValues = DataGridValues,
                    Month = request.Month,
                    Year = request.Year,
                    loginemployeeid = request.loginemployeeid,
                    BUId = request.BUId,
                    SelectAll = request.SelectAll,
                    SelectAllSearchTerm = request.SelectAllSearchTerm,
                    status = request.status,
                    Employee = request.Employee,
                    CustomFilters = customFilters.ToList(),
                    Export_flg = _ExportFlage,
                };
                DataTable _dataTableDevice = RestClient.GetAttendanceSummaryBUExcelGridData(dataRequest);
                var result = new ExportExcelFileMaster().SaveExcelToServer(_dataTableDevice, "BUAttendanceSummaryData", "BUAttendanceSummaryData_");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while processing your request. Please try again later." });
            }
        }
        #endregion

        #region For DataGridLazyloding
        public async Task<ActionResult> GetLazyLoadingGetEmployee(EmployeeDirectoryDataTablesRequest request)
        {
            // Initialize variables
            string searchTerm = string.Empty;
            string sortColumn = "EmpId"; // Default sort column
            string sortOrder = "asc";     // Default sort order
            int pageSize = 50;            // Default page size
            int page = 1;                 // Default page number            
            Int64 DataGridValues = 0;
            // Check if request is not null
            if (request != null)
            {
                // Check if search object is present and has value

                if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                {
                    searchTerm = request.search.value;
                }
                // Check if order array is present and has elements
                if (request.order != null && request.order.Length > 0)
                {
                    sortColumn = request.columns[request.order[0].column].data;
                    sortOrder = request.order[0].dir;
                }
                if (request.DatagridThresold > 0)
                {
                    DataGridValues = request.DatagridThresold;
                }
                // Set pageSize and page if length and start are set
                if (request.length > 0)
                {
                    pageSize = request.length;
                }
                if (request.start >= 0)
                {
                    page = (request.start / pageSize) + 1;
                }
                if (string.IsNullOrEmpty(request.SelectAllSearchTermDept)) request.SelectAllSearchTermDept = string.Empty;
            }
            
            //var customFilters = request.CustFilter ?? new CustomSearchFilter[0];
            // Prepare parameters for RestClient or any service you're using
            DataTableForEmployeeDirectoryGridData dataRequest = new DataTableForEmployeeDirectoryGridData
            {
                page = page,
                pageSize = pageSize,
                searchTerm = searchTerm,
                sortOrder = sortOrder,
                sortColumn = sortColumn,
                DataGridValues = DataGridValues,
                BranchId = request.BranchId,
                CompanyIds = request.CompanyIds,
                DepartmentId = request.DepartmentId,
                RoleId = request.RoleId,
                LoginEmpId = request.LoginEmpId,
                SelectAll = request.SelectAllDept,
                SelectAllSearchTerm = request.SelectAllSearchTermDept,
            };


            var Result = RestClient.GetEmployeeLoadLazyLoading(dataRequest);
            var paginatedResult = JsonConvert.DeserializeObject<dynamic>(Result);


            // Extract TotalRecords and Data
            int totalRecords = paginatedResult.TotalRecords;
            var dataTable = paginatedResult.Data.ToObject<List<dynamic>>();

            var response = new
            {
                draw = request.draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,  // Adjust if needed
                data = dataTable
            };

            // Manually serialize the response using Json.NET
            var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                MaxDepth = int.MaxValue,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            // Return the serialized JSON response
            return Content(jsonResponse, "application/json");
        }
		
		 public async Task<ActionResult> GetLazyLoadingGetEmployeeLeaveSanction(EmployeeDirectoryDataTablesRequest request)
        {
            // Initialize variables
            string searchTerm = string.Empty;
            string sortColumn = "EmpId"; // Default sort column
            string sortOrder = "asc";     // Default sort order
            int pageSize = 50;            // Default page size
            int page = 1;                 // Default page number            
            Int64 DataGridValues = 0;
            // Check if request is not null
            if (request != null)
            {
                // Check if search object is present and has value

                if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                {
                    searchTerm = request.search.value;
                }
                // Check if order array is present and has elements
                if (request.order != null && request.order.Length > 0)
                {
                    sortColumn = request.columns[request.order[0].column].data;
                    sortOrder = request.order[0].dir;
                }
                if (request.DatagridThresold > 0)
                {
                    DataGridValues = request.DatagridThresold;
                }
                // Set pageSize and page if length and start are set
                if (request.length > 0)
                {
                    pageSize = request.length;
                }
                if (request.start >= 0)
                {
                    page = (request.start / pageSize) + 1;
                }
            }
            //var customFilters = request.CustFilter ?? new CustomSearchFilter[0];
            // Prepare parameters for RestClient or any service you're using
            DataTableForEmployeeDirectoryGridData dataRequest = new DataTableForEmployeeDirectoryGridData
            {
                page = page,
                pageSize = pageSize,
                searchTerm = searchTerm,
                sortOrder = sortOrder,
                sortColumn = sortColumn,
                DataGridValues = DataGridValues,
                BranchId = request.BranchId,
                CompanyIds = request.CompanyIds,
                DepartmentId = request.DepartmentId,
                RoleId = request.RoleId,
                LoginEmpId = request.LoginEmpId,
                SelectAll = request.SelectAll,
                SelectAllSearchTerm = request.SelectAllSearchTerm,
                SelectAllBranch = request.SelectAllBranch,
                SelectAllBranchSearchTerm = request.SelectAllBranchSearchTerm,
                SelectAllEmp = request.SelectAllEmp,
                SelectAllEmpSearchTerm = request.SelectAllEmpSearchTerm
            };


            var Result = RestClient.GetEmployeeLoadLazyLoadingLeaveSanction(dataRequest);
            var paginatedResult = JsonConvert.DeserializeObject<dynamic>(Result);


            // Extract TotalRecords and Data
            int totalRecords = paginatedResult.TotalRecords;
            var dataTable = paginatedResult.Data.ToObject<List<dynamic>>();

            var response = new
            {
                draw = request.draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,  // Adjust if needed
                data = dataTable
            };

            // Manually serialize the response using Json.NET
            var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                MaxDepth = int.MaxValue,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            // Return the serialized JSON response
            return Content(jsonResponse, "application/json");
        }
        #endregion


        #region For Transction Monitor Data in Developer Account
        public async Task<JsonResult> GetPaginatedTransctionDeveloeprGridData(TransctionDataTablesDeveloperRequest request)
        {
            string searchTerm = string.Empty;
            string sortColumn = "txnDateTime";
            string sortOrder = "asc";
            int pageSize = 10;
            int page = 1;
            Int64 DataGridValues = 0;

            if (request != null)
            {
                // Check if search object is present and has value

                if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                {
                    searchTerm = request.search.value;
                }
                // Check if order array is present and has elements
                if (request.order != null && request.order.Length > 0)
                {
                    sortColumn = request.columns[request.order[0].column].data;
                    sortOrder = request.order[0].dir;
                }
                if (request.DatagridThresold > 0)
                {
                    DataGridValues = request.DatagridThresold;
                }
                // Set pageSize and page if length and start are set
                if (request.length > 0)
                {
                    pageSize = request.length;
                }
                if (request.start >= 0)
                {
                    page = (request.start / pageSize) + 1;
                }
            }
            var customFilters = request.CustFilter ?? new CustomSearchFilter[0];
            // Prepare parameters for RestClient or any service you're using
            TransctionDataTablesDeveloper dataRequest = new TransctionDataTablesDeveloper
            {
                Page = page,
                PageSize = pageSize,
                searchTerm = searchTerm,
                sortOrder = sortOrder,
                sortColumn = sortColumn,
                DatagridThresold = request.DatagridThresold,
                FilterFromDate= request.FilterFromDate,
                FilterToDate = request.FilterToDate,
                DeviceCode=request.DeviceCode,
                PunchID=request.PunchID,
                ExportFlage = request.ExportFlage
            };
            var paginatedResult = RestClient.GetPaginatedFordveloperTrasctionData(dataRequest);          

            return Json(new
            {
                draw = request.draw,
                recordsTotal = paginatedResult.Table1.FirstOrDefault()?.TotalCount ?? 0,
                recordsFiltered = paginatedResult.Table1.FirstOrDefault()?.TotalCount ?? 0,
                data = paginatedResult.Table2
            });
        }
        #endregion


        #region For Excel Sheeet Download in dyanamics Data
        public async Task<JsonResult> GetPaginatedTransctionDeveloeprGridDataExport(TransctionDataTablesDeveloperRequest request)
        {
            string searchTerm = string.Empty;
            string sortColumn = "txnDateTime";
            string sortOrder = "asc";
            int pageSize = 10;
            int page = 1;
            Int64 DataGridValues = 0;

            if (request != null)
            {
                // Check if search object is present and has value

                if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                {
                    searchTerm = request.search.value;
                }
                // Check if order array is present and has elements
                if (request.order != null && request.order.Length > 0)
                {
                    sortColumn = request.columns[request.order[0].column].data;
                    sortOrder = request.order[0].dir;
                }
                if (request.DatagridThresold > 0)
                {
                    DataGridValues = request.DatagridThresold;
                }
                // Set pageSize and page if length and start are set
                if (request.length > 0)
                {
                    pageSize = request.length;
                }
                if (request.start >= 0)
                {
                    page = (request.start / pageSize) + 1;
                }
            }
            var customFilters = request.CustFilter ?? new CustomSearchFilter[0];
            // Prepare parameters for RestClient or any service you're using
            TransctionDataTablesDeveloper dataRequest = new TransctionDataTablesDeveloper
            {
                Page = page,
                PageSize = pageSize,
                searchTerm = searchTerm,
                sortOrder = sortOrder,
                sortColumn = sortColumn,
                DatagridThresold = request.DatagridThresold,
                FilterFromDate = request.FilterFromDate,
                FilterToDate = request.FilterToDate,
                DeviceCode = request.DeviceCode,
                PunchID = request.PunchID,
                ExportFlage = request.ExportFlage
            };            
            DataTable _dataTableReligion = RestClient.GetPaginatedForDeveloperTransactionDataExport(dataRequest);


            //// Update issync column values from 0/1 to true/false
            //foreach (DataRow row in _dataTableReligion.Rows)
            //{
            //    if (row["isSync"] != DBNull.Value)
            //    {

            //        row["isSync"] = Convert.ToInt32(row["isSync"]) == 1 ? true : false;
            //    }
            //    _dataTableReligion.AcceptChanges();
            //}

            // Convert the column type if needed
            if (_dataTableReligion.Columns.Contains("isSync"))
            {
                DataColumn col = _dataTableReligion.Columns["isSync"];
                if (col.DataType != typeof(bool))
                {
                    // Create a temporary bool column
                    DataColumn boolCol = new DataColumn("isSync_bool", typeof(bool));

                    _dataTableReligion.Columns.Add(boolCol);

                    // Copy and convert values
                    foreach (DataRow row in _dataTableReligion.Rows)
                    {
                        boolCol.ReadOnly = false;
                        row["isSync_bool"] = Convert.ToInt32(row["isSync"]) == 1;
                    }

                    // Remove the old column and rename new one
                    int colIndex = _dataTableReligion.Columns.IndexOf(col);
                    _dataTableReligion.Columns.Remove("isSync");
                    boolCol.ColumnName = "isSync";
                    _dataTableReligion.Columns["isSync"].SetOrdinal(colIndex);
                }
            }


            var result = new ExportExcelFileMaster().SaveExcelToServerLargeFile(_dataTableReligion, "TransctionGridData", "TransctionGridData_");
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region For Excel Sheeet Download in dyanamics Data
        // POST: Start export

        public ActionResult DownloadFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                return File(filePath, "application/octet-stream", Path.GetFileName(filePath));
            }
            return HttpNotFound("File not found.");
        }
        private string GenerateCsvContent(DataTable dataTable)
        {
            var csvContent = new StringBuilder();

            // Generate CSV header
            var columnNames = dataTable.Columns.Cast<DataColumn>()
                .Select(column => column.ColumnName);
            csvContent.AppendLine(string.Join(",", columnNames));

            // Generate CSV rows
            foreach (DataRow row in dataTable.Rows)
            {
                var fields = row.ItemArray.Select(field => field?.ToString()?.Replace(",", " "));
                csvContent.AppendLine(string.Join(",", fields));
            }

            return csvContent.ToString();
        }
        // Method to generate XLSX content using EPPlus
        private byte[] GenerateExcelContent(DataTable dataTable, string fileName)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                    int totalColumns = dataTable.Columns.Count;

                    // Title row (merged and styled)
                    var titleCell = worksheet.Cells[1, 1, 1, totalColumns];
                    titleCell.Merge = true;
                    titleCell.Value = fileName;
                    titleCell.Style.Font.Bold = true;
                    titleCell.Style.Font.Size = 14;
                    titleCell.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    titleCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    titleCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2F75B5"));
                    titleCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    titleCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Row(1).Height = 25;

                    // Column headers
                    for (int i = 0; i < totalColumns; i++)
                    {
                        var cell = worksheet.Cells[2, i + 1];
                        cell.Value = dataTable.Columns[i].ColumnName;
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    // Data rows
                    for (int row = 0; row < dataTable.Rows.Count; row++)
                    {
                        for (int col = 0; col < totalColumns; col++)
                        {
                            var cell = worksheet.Cells[row + 3, col + 1];
                            var value = dataTable.Rows[row][col];
                            string columnName = dataTable.Columns[col].ColumnName.ToLower();

                            // Check if value is DateTime and format accordingly
                            if (value is DateTime dtValue &&
                                (columnName == "currentdate"))
                            {
                                cell.Value = dtValue.ToString("dd-MM-yyyy");
                            }
                            else
                            {
                                cell.Value = value;
                            }

                            cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }
                    }

                    worksheet.Cells.AutoFitColumns();

                    return package.GetAsByteArray();
                }
            }
            catch (Exception)
            {
                using (var emptyPackage = new ExcelPackage())
                {
                    var emptySheet = emptyPackage.Workbook.Worksheets.Add("Error");
                    emptySheet.Cells[1, 1].Value = "An error occurred while generating Excel.";
                    return emptyPackage.GetAsByteArray();
                }
            }
        }




        #endregion


        #region For Transction Monitor Data in Developer Account
        public async Task<JsonResult> GetPaginatedLeaveSanctionprGridData(TransctionDataTablesDeveloperRequest request)
        {
            string searchTerm = string.Empty;
            string sortColumn = "EmpName";
            string sortOrder = "asc";
            int pageSize = 10;
            int page = 1;
            Int64 DataGridValues = 0;

            if (request != null)
            {
                // Check if search object is present and has value

                if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                {
                    searchTerm = request.search.value;
                }
                // Check if order array is present and has elements
                if (request.order != null && request.order.Length > 0)
                {
                    sortColumn = request.columns[request.order[0].column].data;
                    sortOrder = request.order[0].dir;
                }
                if (request.DatagridThresold > 0)
                {
                    DataGridValues = request.DatagridThresold;
                }
                // Set pageSize and page if length and start are set
                if (request.length > 0)
                {
                    pageSize = request.length;
                }
                if (request.start >= 0)
                {
                    page = (request.start / pageSize) + 1;
                }
            }      
            var customFilters = request.CustFilter ?? new CustomSearchFilter[0];

           
            var Brchid = 0;
            var _RoleID = 0;
            var _EmpID = "0";
            if (Convert.ToInt32(Session["RoleId"]) == 6805)
            {
                
            }
            if (Convert.ToInt32(Session["RoleId"]) == 6806)
            {
                Brchid = Convert.ToInt32(Session["BranchId"].ToString());
                _EmpID = Session["EmpId"].ToString();
            }
            if (Session["RoleId"] !=null)
            {
                _RoleID = Convert.ToInt32(Session["RoleId"].ToString()); 
            }
            LeaveOpeningGetAll dataRequest = new LeaveOpeningGetAll
            {
                Page = page,
                PageSize = pageSize,
                searchTerm = searchTerm,
                sortOrder = sortOrder,
                sortColumn = sortColumn,
                DataGridthresold = request.DatagridThresold,                
                EmpIds = request.EmpIds,
                BranchID=request.BranchID,
                ExportFlage = request.ExportFlage,
                cmpid = request.CompanyID,
                Brchid = Brchid,
                RoleId = _RoleID,
                Empid = _EmpID,
                SelectAllBranch = request.SelectAllBranch,
                SelectAllBranchSearchTerm = request.SelectAllBranchSearchTerm,
                SelectAllEmp = request.SelectAllEmp,
                SelectAllEmpSearchTerm = request.SelectAllEmpSearchTerm,
                IsActive = request.IsActive
            };

            var paginatedResult = RestClient.GetPaginatedForDeveloperLeaveOpeningGetAll(dataRequest);           

            return Json(new
            {
                draw = request.draw,
                recordsTotal = paginatedResult.Table1.FirstOrDefault()?.TotalCount ?? 0,
                recordsFiltered = paginatedResult.Table1.FirstOrDefault()?.TotalCount ?? 0,
                data = paginatedResult.Table2
            });
        }
        #endregion

        #region Analaytics Dashboard
        public async Task<JsonResult> tmptableforReports(reqReports request)
        {
            try
            {
                string searchTerm = string.Empty;
                string sortColumn = "EmpName";
                string sortOrder = "asc";
                int pageSize = 10;
                int page = 1;
                Int64 DataGridValues = 0;

                if (request != null)
                {
                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                }
                var customFilters = request.columnFilters ?? new CustomSearchFilter[0];
                reqReports dataRequest = new reqReports
                {
                    page = request.page,
                    pageSize = request.pageSize,
                    searchTerm = request.searchTerm,
                    sortOrder = request.sortOrder,
                    sortColumn = request.sortColumn,
                    EmpID = request.EmpID,
                    BranchId = request.BranchId,
                    Todate = request.Todate,
                    FromDate = request.FromDate,
                    RptID = request.RptID,
                    IsFilo = request.IsFilo,
                    DepId = request.DepId,
                    Isactive = request.Isactive,
                    CompanyID = request.CompanyID,
                    draw = request.draw,
                    CustomFilters = customFilters.ToList()

                };

                var paginatedResult = RestClient.tmptableforReports(dataRequest);
                if (paginatedResult == null || paginatedResult.data == null || !paginatedResult.data.Any())
                {
                    return Json(new
                    {
                        draw = request.draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = new List<Dictionary<string, object>>()
                    });
                }                

                return Json(new
                {
                    draw = request.draw,
                    recordsTotal = paginatedResult.recordsTotal,
                    recordsFiltered = paginatedResult.recordsFiltered,
                    data = paginatedResult.data
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    draw = request.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<Dictionary<string, object>>(),
                    error = ex.Message
                });
            }
        }
        public async Task<ActionResult> tmptableforReportstable(reqReports request)
        {
            try
            {
                string searchTerm = string.Empty;
                string sortColumn = "EmpName";
                string sortOrder = "asc";
                int pageSize = 10;
                int page = 1;
                Int64 DataGridValues = 0;

                if (request != null)
                {
                    if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                    {
                        searchTerm = request.search.value;
                    }
                }
                var customFilters = request.columnFilters ?? new CustomSearchFilter[0];
                reqReports dataRequest = new reqReports
                {
                    page = request.page,
                    pageSize = request.pageSize,
                    searchTerm = request.searchTerm,
                    sortOrder = sortOrder,
                    sortColumn = request.sortColumn,
                    EmpID = request.EmpID,
                    BranchId = request.BranchId,
                    Todate = request.Todate,
                    FromDate = request.FromDate,
                    RptID = request.RptID,
                    IsFilo = request.IsFilo,
                    DepId = request.DepId,
                    Isactive = request.Isactive,
                    CompanyID = request.CompanyID,
                    draw = request.draw,
                    CustomFilters = customFilters.ToList()

                };

                var paginatedResult1 = RestClient.tmptableforReports(dataRequest);

                string jsonString = JsonConvert.SerializeObject(paginatedResult1);

                JObject jo = JObject.FromObject(paginatedResult1);

                var dataToken = jo["data"];
                var detailsToken = jo["details"]; 

                var totalRecordsToken = jo["recordsTotal"] ?? jo["RecordsTotal"] ?? jo["totalRecords"];
                int totalRecords = totalRecordsToken?.ToObject<int>() ?? 0;

                var filteredRecords = totalRecords;

                var response = new
                {
                    draw = request.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = dataToken,       
                    details = detailsToken  
                };

                var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    MaxDepth = int.MaxValue,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                return Content(jsonResponse, "application/json");
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    draw = request.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<Dictionary<string, object>>(),
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult GetAllReportExcelData(reqReports request)
        {
            try
            {
                int Flag = 0;

                if (request.RptID != null && request.RptID.ToString() == "199")
                {
                    Flag = 199;
                    request.RptID = 19;   // Convert 199 to 19
                }
                if (request.RptID != null && request.RptID.ToString() == "200")
                {
                    Flag = 200;
                    request.RptID = 20;   // Convert 200 to 20
                }


                // Initialize variables
                string searchTerm = string.Empty;
                string sortColumn = "EmpID";
                string sortOrder = "asc";
                int pageSize = 10;
                int page = 1;
                var customFilters = request.columnFilters ?? new CustomSearchFilter[0];
                reqReports dataRequest = new reqReports
                {
                    page = request.page,
                    pageSize = request.pageSize,
                    searchTerm = request.searchTerm,
                    sortOrder = request.sortOrder,
                    sortColumn = request.sortColumn,
                    EmpID = request.EmpID,
                    BranchId = request.BranchId,
                    Todate = request.Todate,
                    FromDate = request.FromDate,
                    RptID = request.RptID,
                    IsFilo = request.IsFilo,
                    DepId = request.DepId,
                    Isactive = request.Isactive,
                    CompanyID = request.CompanyID,
                    draw = request.draw,
                    CustomFilters = customFilters.ToList()
                };

                ExportExcelFileMaster.ExportResult result;

                DataTable _dataTableEmployee = RestClient.GetReportExcelGridData(dataRequest);

                string prefix = request.reportName.Split('_')[0];

                if (request.ExportFlag == 0)
                {
                    // If RptID = 19 → Muster Report For Old Report Design
                    if (request.RptID == 19 && Flag == 199) //fastreck
                    {
                        result = new ExportExcelFileMaster()
                            .SaveExcelToServerrpt(_dataTableEmployee, request.reportName, prefix + "_");
                    }
                    // If RptID = 20 → Muster Report For Old Report Design
                    else if (request.RptID == 20 && Flag == 200) //Working Duration
                    {
                        result = new ExportExcelFileMaster()
                            .SaveExcelToServerrpt(_dataTableEmployee, request.reportName, prefix + "_");
                    }
                    // If RptID = 19 → Muster Report For New Report Design
                    else if (request.RptID == 19)
                    {
                        result = new ExportExcelFileMaster()
                            .SaveExcelToServerrptMuster(_dataTableEmployee, request.reportName, prefix + "_");
                    }
                    // If RptID = 19 → Muster Report For New Report Design
                    else if (request.RptID == 20)
                    {
                        result = new ExportExcelFileMaster()
                            .SaveExcelToServerrptDuraction(_dataTableEmployee, request.reportName, prefix + "_");
                    }
                    else if (request.RptID == 201)
                    {
                        result = new ExportExcelFileMaster()
                            .SaveExcelToServerrptDuractionAttendance(_dataTableEmployee, request.reportName, prefix + "_");
                    }
                    else
                    {
                        result = new ExportExcelFileMaster()
                            .SaveExcelToServerrpt(_dataTableEmployee, request.reportName, prefix + "_");
                    }
                }
                else
                {
                    result = new ExportExcelFileMaster()
                        .SavePdfToServer(_dataTableEmployee, request.reportName, prefix + "_");
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while processing your request. Please try again later."
                });
            }
        }

        [HttpGet]
        public FileResult DownloadReportFile(string fileName)
        {
            var filePath = Server.MapPath("~/ExportsExcelData/" + fileName);

            string contentType;
            if (fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                contentType = "application/pdf";
            else if (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            else
                contentType = "application/octet-stream";

            return File(System.IO.File.ReadAllBytes(filePath), contentType, fileName); // forces download
        }

        #endregion
    }
}