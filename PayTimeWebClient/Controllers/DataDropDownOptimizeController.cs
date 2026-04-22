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

namespace PayTimeWebClient.Controllers
{
    public class DataDropDownOptimizeController : Controller
    {
        // GET: DataDropDownOptimize

        #region Declaration
        static readonly DataGridOptimizeRestClient RestClient = new DataGridOptimizeRestClient();
        static readonly EncryptionHelper EncryptionHelperr = new EncryptionHelper();
        static readonly UtilitiesDataRestClient UtilitiesRestClient = new UtilitiesDataRestClient();

        // Static dictionary to track export statuses
        //private static readonly ConcurrentDictionary<string, ExportStatus> ExportStatuses = new ConcurrentDictionary<string, ExportStatus>();
        #endregion

        #region For NormalDropDown All Method

        #endregion

        #region For GetDropDownBranchDataList
        public async Task<JsonResult> GetEmployeeDropdownOptions(SerachBranchObjectParam dataRequestBranch)
        {
            string searchTermNew = string.Empty;
            if (dataRequestBranch.searchTerm != null)
            {
                searchTermNew = dataRequestBranch.searchTerm;
            }
            else
            {
                searchTermNew = "";
            }
            SerachBranchObjectParam _dataRequest = new SerachBranchObjectParam
            {
                page = dataRequestBranch.page,
                pageSize = dataRequestBranch.pageSize,
                sortOrder = dataRequestBranch.sortOrder,
                searchTerm = searchTermNew,
                CompanyID = dataRequestBranch.CompanyID,
                RoleID = dataRequestBranch.RoleID,
                BranchID = dataRequestBranch.BranchID,
                DepartID = dataRequestBranch.DepartID,
                sortColumn = searchTermNew
            };
            var employees = RestClient.GetPaginatedEmployeesDropDownDataAsync(_dataRequest);
            return Json(employees, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetBranchDropdownOptions(SerachBranchObjectParam dataRequestBranch)
        {
            string searchTermNew = string.Empty;
            if (dataRequestBranch.searchTerm != null)
            {
                searchTermNew = dataRequestBranch.searchTerm;
            }
            else
            {
                searchTermNew = "";
            }
            SerachBranchObjectParam _dataRequest = new SerachBranchObjectParam
            {
                page = dataRequestBranch.page,
                pageSize = dataRequestBranch.pageSize,
                sortOrder = dataRequestBranch.sortOrder,
                searchTerm = searchTermNew,
                CompanyID = dataRequestBranch.CompanyID,
                RoleID = dataRequestBranch.RoleID,
                sortColumn = searchTermNew
            };
            var employees = RestClient.GetPaginatedBranchDropDownDataAsync(_dataRequest);
            return Json(employees, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region For GetDropDownDepartmentDataList
        public async Task<JsonResult> GetDepartmentDropdownOptions(SerachBranchObjectParam dataRequestBranch)
        {
            string searchTermNew = string.Empty;
            if (dataRequestBranch.searchTerm != null)
            {
                searchTermNew = dataRequestBranch.searchTerm;
            }
            else
            {
                searchTermNew = "";
            }
            SerachBranchObjectParam _dataRequest = new SerachBranchObjectParam
            {
                page = dataRequestBranch.page,
                pageSize = dataRequestBranch.pageSize,
                sortOrder = dataRequestBranch.sortOrder,
                searchTerm = searchTermNew,
                CompanyID = dataRequestBranch.CompanyID,
                RoleID = dataRequestBranch.RoleID,
                sortColumn = searchTermNew
            };
            var employees = RestClient.GetPaginatedDepartmentDropDownDataAsync(_dataRequest);
            return Json(employees, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}