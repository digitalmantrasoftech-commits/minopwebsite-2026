using DevExpress.Xpo;
using DevExpress.XtraReports.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using PayTimeWebClient.Models;
using PayTimeWebClient.Models.ReportsModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PayTimeWebClient.Models.DataGridModel;
using PayTimeWebClient.Models.Notifications_Module;


namespace PayTimeWebClient.Helper
{
    public class DataGridOptimizeRestClient
    {

        private static readonly string _url = ConfigurationManager.AppSettings["webapipaytime"];
        private readonly string _Payrollurl = ConfigurationManager.AppSettings["webapipaytimePayroll"];


        #region For EmployeeHighLevelGridData
        public EmployeeGridResult GetPaginatedEmployeesAsync(DataTableForEmployeeGridData objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/GetEmployeeGridData";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;
                EmployeeGridResult _HighLevelGridDataobj = new EmployeeGridResult();
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;
                        _HighLevelGridDataobj = JsonConvert.DeserializeObject<EmployeeGridResult>(jsonResponse);
                        return _HighLevelGridDataobj;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedEmployeesAsync Error", ex.Message);
                        return _HighLevelGridDataobj;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve EmployeeHighLevel employee data. Status code: " + response.StatusCode);
                }
            }
        }
        #endregion

        #region for EmployeeHighLevelDropDownData
        public EmployeeItemDropDownResult GetPaginatedEmployeesDropDownDataAsync(SerachBranchObjectParam objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/GetEmployeeDropDownData";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;
                EmployeeItemDropDownResult _HighLevelDropDownDataObj = new EmployeeItemDropDownResult();
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;
                        _HighLevelDropDownDataObj = JsonConvert.DeserializeObject<EmployeeItemDropDownResult>(jsonResponse);
                        return _HighLevelDropDownDataObj;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedEmployeesDropDownDataAsync Error", ex.Message);
                        return _HighLevelDropDownDataObj;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve EmployeesDropDownData data. Status code: " + response.StatusCode);
                }
            }
        }
        #endregion

        #region for BranchHighLevelDropDownData
        public EmployeeItemDropDownResult GetPaginatedBranchDropDownDataAsync(SerachBranchObjectParam objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/GetBranchDropDownData";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;
                EmployeeItemDropDownResult _HighLevelDropDownDataObj = new EmployeeItemDropDownResult();
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;
                        _HighLevelDropDownDataObj = JsonConvert.DeserializeObject<EmployeeItemDropDownResult>(jsonResponse);
                        return _HighLevelDropDownDataObj;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedBranchDropDownDataAsync Error", ex.Message);
                        return _HighLevelDropDownDataObj;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve BranchDropDownData data. Status code: " + response.StatusCode);
                }
            }
        }
        #endregion

        #region for BranchHighLevelDropDownData
        public EmployeeItemDropDownResult GetPaginatedDepartmentDropDownDataAsync(SerachBranchObjectParam objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/GetDepartmentDropDownData";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;
                EmployeeItemDropDownResult _HighLevelDropDownDataObj = new EmployeeItemDropDownResult();
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;
                        _HighLevelDropDownDataObj = JsonConvert.DeserializeObject<EmployeeItemDropDownResult>(jsonResponse);
                        return _HighLevelDropDownDataObj;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedDepartmentDropDownDataAsync Error", ex.Message);
                        return _HighLevelDropDownDataObj;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve BranchDropDownData data. Status code: " + response.StatusCode);
                }
            }
        }
        #endregion


        public DataTable GetEmployeeExcelGridData(DataTableForEmployeeGridData objData)
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                string uri = _url + "DataGridOptimize/GetEmployeeGridExportData";
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, objData);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;
                    dt = JsonConvert.DeserializeObject<DataTable>(jsonString);
                }
                else
                {
                    throw new Exception("Failed to retrieve data from the API. Status code: " + result.StatusCode);
                }
            }
            return dt;
        }
        public DataTable GetReportExcelGridData(reqReports objData)
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                string uri = _url + "Report/GetReportExcelGridData";
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, objData);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;
                    dt = JsonConvert.DeserializeObject<DataTable>(jsonString);
                }
                else
                {
                    throw new Exception("Failed to retrieve data from the API. Status code: " + result.StatusCode);
                }
            }
            return dt;
        }
        public DataTable GetDirectoryExcelGridData(DataTableForEmployeeDirectoryGridData objData)
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                string uri = _url + "DataGridOptimize/GetDirectoryGridExportData";
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, objData);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;
                    dt = JsonConvert.DeserializeObject<DataTable>(jsonString);
                }
                else
                {
                    throw new Exception("Failed to retrieve data from the API. Status code: " + result.StatusCode);
                }
            }
            return dt;
        }


        #region For BranchHighLevelGridData
        public BranchGridResult GetPaginatedBranchesAsync(DataTableForBranchGridData objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/GetBranchGridData";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;
                BranchGridResult _HighLevelGridDataobj = new BranchGridResult();
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;
                        _HighLevelGridDataobj = JsonConvert.DeserializeObject<BranchGridResult>(jsonResponse);
                        return _HighLevelGridDataobj;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedBranchesAsync Error", ex.Message);
                        return _HighLevelGridDataobj;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve EmployeeHighLevel employee data. Status code: " + response.StatusCode);
                }
            }
        }

        public UserGridResult GetPaginatedEnrollUserAsync(DataTableForEnrollUserGridData objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/EmployeelstEnrollUserPaginatList";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;
                UserGridResult _HighLevelGridDataobj = new UserGridResult();
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;
                        _HighLevelGridDataobj = JsonConvert.DeserializeObject<UserGridResult>(jsonResponse);
                        return _HighLevelGridDataobj;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedEnrollUserAsync Error", ex.Message);
                        return _HighLevelGridDataobj;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve EmployeeHighLevel employee data. Status code: " + response.StatusCode);
                }
            }
        }
        public string GetPaginatedEmployeeAsync(DataTableForEmployeeDirectoryGridData objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/EmployeeDirectoryPaginatList";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        return jsonResponse;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedEnrollUserAsync Error", ex.Message);
                        return null;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve EmployeeHighLevel employee data. Status code: " + response.StatusCode);
                }
            }
        }
        public DataTable GetBranchExcelGridData(DataTableForBranchGridData objData)
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                string uri = _url + "DataGridOptimize/GetBranchGridExportData";
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, objData);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;
                    dt = JsonConvert.DeserializeObject<DataTable>(jsonString);
                }
                else
                {
                    throw new Exception("Failed to retrieve data from the API. Status code: " + result.StatusCode);
                }
            }
            return dt;
        }
        #endregion

        #region For OnBoardingHighLevelGridData
        public string GetPaginatedOnBoardingEmployeeAsync(DataTableForEmployeeOnBoardingGridData objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/EmployeeOnBoardingPaginatList";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        return jsonResponse;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedOnBoardingEmployeeAsync Error", ex.Message);
                        return null;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve OnBoarding EmployeeHighLevel data Status code: " + response.StatusCode);
                }
            }
        }

        public DataTable GetOnBoardingExcelGridData(EmployeeOnBoardingDataTablesRequest objData)
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                string uri = _url + "DataGridOptimize/GetOnBoardingGridExportData";
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, objData);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;
                    dt = JsonConvert.DeserializeObject<DataTable>(jsonString);
                }
                else
                {
                    throw new Exception("Failed to retrieve data from the API. Status code: " + result.StatusCode);
                }
            }
            return dt;
        }
        #endregion

        #region For CompanyHighLevelGridData
        public string GetPaginatedCompanyAsync(DataTableForEmployeeOnBoardingGridData objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/GetCompanyGridData";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        return jsonResponse;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedCompanyAsync Error", ex.Message);
                        return null;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve Company data Status code: " + response.StatusCode);
                }
            }
        }

        public DataTable GetCompanyExcelGridData(EmployeeOnBoardingDataTablesRequest objData)
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                string uri = _url + "DataGridOptimize/GetCompanyGridExportData";
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, objData);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;
                    dt = JsonConvert.DeserializeObject<DataTable>(jsonString);
                }
                else
                {
                    throw new Exception("Failed to retrieve data from the API. Status code: " + result.StatusCode);
                }
            }
            return dt;
        }
        #endregion

        #region For DesignationHighLevelGridData
        public string GetPaginatedDesignationAsync(DataTableForEmployeeOnBoardingGridData objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/GetDesignationGridData";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        return jsonResponse;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedDesignationAsync Error", ex.Message);
                        return null;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve Designation data Status code: " + response.StatusCode);
                }
            }
        }

        public DataTable GetDesignationExcelGridData(EmployeeOnBoardingDataTablesRequest objData)
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                string uri = _url + "DataGridOptimize/GetDesignationGridExportData";
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, objData);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;
                    dt = JsonConvert.DeserializeObject<DataTable>(jsonString);
                }
                else
                {
                    throw new Exception("Failed to retrieve data from the API. Status code: " + result.StatusCode);
                }
            }
            return dt;
        }
        #endregion

        #region For DepartmentHighLevelGridData
        public string GetPaginatedDepartmentAsync(DataTableForEmployeeOnBoardingGridData objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/GetDepartmentGridData";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        return jsonResponse;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedDesignationAsync Error", ex.Message);
                        return null;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve Designation data Status code: " + response.StatusCode);
                }
            }
        }

        public DataTable GetDepartmentExcelGridData(EmployeeOnBoardingDataTablesRequest objData)
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                string uri = _url + "DataGridOptimize/GetDepartmentGridExportData";
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, objData);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;
                    dt = JsonConvert.DeserializeObject<DataTable>(jsonString);
                }
                else
                {
                    throw new Exception("Failed to retrieve data from the API. Status code: " + result.StatusCode);
                }
            }
            return dt;
        }
        #endregion

        #region For ShiftHighLevelGridData
        public string GetPaginatedShiftAsync(DataTableForEmployeeOnBoardingGridData objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/GetShiftGridData";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        return jsonResponse;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedDesignationAsync Error", ex.Message);
                        return null;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve Designation data Status code: " + response.StatusCode);
                }
            }
        }

        public DataTable GetShiftExcelGridData(EmployeeOnBoardingDataTablesRequest objData)
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                string uri = _url + "DataGridOptimize/GetShiftGridExportData";
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, objData);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;
                    dt = JsonConvert.DeserializeObject<DataTable>(jsonString);
                }
                else
                {
                    throw new Exception("Failed to retrieve data from the API. Status code: " + result.StatusCode);
                }
            }
            return dt;
        }
        #endregion

        #region For HolidayHighLevelGridData
        public string GetPaginatedHolidayAsync(DataTableForEmployeeOnBoardingGridData objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/GetHolidayGridData";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        return jsonResponse;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedHolidayAsync Error", ex.Message);
                        return null;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve Designation data Status code: " + response.StatusCode);
                }
            }
        }

        public DataTable GetHolidayExcelGridData(EmployeeOnBoardingDataTablesRequest objData)
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                string uri = _url + "DataGridOptimize/GetHolidayGridExportData";
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, objData);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;
                    dt = JsonConvert.DeserializeObject<DataTable>(jsonString);
                }
                else
                {
                    throw new Exception("Failed to retrieve data from the API. Status code: " + result.StatusCode);
                }
            }
            return dt;
        }
        #endregion

        #region For ReligionHighLevelGridData
        public string GetPaginatedReligionAsync(DataTableForEmployeeOnBoardingGridData objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/GetReligionGridData";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        return jsonResponse;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedHolidayAsync Error", ex.Message);
                        return null;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve Designation data Status code: " + response.StatusCode);
                }
            }
        }

        public DataTable GetReligionExcelGridData(EmployeeOnBoardingDataTablesRequest objData)
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                string uri = _url + "DataGridOptimize/GetReligionGridExportData";
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, objData);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;
                    dt = JsonConvert.DeserializeObject<DataTable>(jsonString);
                }
                else
                {
                    throw new Exception("Failed to retrieve data from the API. Status code: " + result.StatusCode);
                }
            }
            return dt;
        }
        #endregion


        public string GetEmployeeLoadLazyLoading(DataTableForEmployeeDirectoryGridData objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/EmployeeGetDataWithLazyLoading";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        return jsonResponse;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetEmployeeLoadLazyLoading Error", ex.Message);
                        return null;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve EmployeeHighLevel employee data. Status code: " + response.StatusCode);
                }
            }
        }
        public string GetEmployeeLoadLazyLoadingLeaveSanction(DataTableForEmployeeDirectoryGridData objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/EmployeeGetDataWithLazyLoadingForLeaveSanction";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        return jsonResponse;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetEmployeeLoadLazyLoading Error", ex.Message);
                        return null;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve EmployeeHighLevel employee data. Status code: " + response.StatusCode);
                }
            }
        }
        #region Device Master
        public string GetPaginatedDeviceMasterAsync(DeviceMasterGridReq objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "DataGridOptimize/DeviceMasterPaginatList";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        return jsonResponse;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedDeviceMasterAsync Error", ex.Message);
                        return null;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve Device Master data. Status code: " + response.StatusCode);
                }
            }
        }

        public DataTable GetDeviceMasterExcelGridData(DeviceMasterGridReq objData)
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                string uri = _url + "DataGridOptimize/GetDeviceGridExportData";
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, objData);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;
                    dt = JsonConvert.DeserializeObject<DataTable>(jsonString);
                }
                else
                {
                    throw new Exception("Failed to retrieve data from the API. Status code: " + result.StatusCode);
                }
            }
            return dt;
        }
        #endregion

        #region AdminDashBoard
        public string GetPaginatedAdminDashBoardAsync(AdminDashBoardReq objData)
        {

            string uri = _url + "DataGridOptimize/AdminDashBoardPaginatList";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        return jsonResponse;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedAdminDashBoardAsync Error", ex.Message);
                        return null;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve EmployeeHighLevel employee data. Status code: " + response.StatusCode);
                }
            }
        }

        public DataTable GetAdminDashBoardExcelGridData(AdminDashBoardReq objData)
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                string uri = _url + "DataGridOptimize/GetAdminDashBoardGridExportData";
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, objData);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;
                    dt = JsonConvert.DeserializeObject<DataTable>(jsonString);
                }
                else
                {
                    throw new Exception("Failed to retrieve data from the API. Status code: " + result.StatusCode);
                }
            }
            return dt;
        }
        #endregion

        #region AttendanceSummary
        public string GetPaginatedAttendanceSummaryAsync(AttanSummaryDto objData)
        {
            
            string uri = _url + "DataGridOptimize/AttendaceSummaryPaginatList";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                // Make the HTTP POST request synchronously
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        return jsonResponse;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedAttendanceSummaryAsync Error", ex.Message);
                        return null;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve Attendance Summary data. Status code: " + response.StatusCode);
                }
            }
        }

        public DataTable GetAttendanceSummaryExcelGridData(AttanSummaryDto objData)
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                string uri = _url + "DataGridOptimize/AttendaceSummaryExcelData";
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, objData);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;
                    dt = JsonConvert.DeserializeObject<DataTable>(jsonString);
                }
                else
                {
                    throw new Exception("Failed to retrieve data from the API. Status code: " + result.StatusCode);
                }
            }
            return dt;
        }

        #endregion

        #region BU Attendance Summary APIs

        public string GetPaginatedAttendanceSummaryBUAsync(AttanSummaryDto objData)
        {
            string uri = _url + "BU/AttendaceSummaryBUPaginatList";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;
                        return jsonResponse;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedAttendanceSummaryBUAsync Error", ex.Message);
                        return null;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve BU Attendance Summary data. Status code: " + response.StatusCode);
                }
            }
        }

        public DataTable GetAttendanceSummaryBUExcelGridData(AttanSummaryDto objData)
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                string uri = _url + "BU/AttendaceSummaryBUExcelData";
                var postTask = client.PostAsJsonAsync(uri, objData);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;
                    dt = JsonConvert.DeserializeObject<DataTable>(jsonString);
                }
                else
                {
                    throw new Exception("Failed to retrieve BU Excel data from the API. Status code: " + result.StatusCode);
                }
            }
            return dt;
        }

        #endregion
		 #region For Tracstion Moniter Data
        public TransctionDataGridResponse GetPaginatedFordveloperTrasctionData(TransctionDataTablesDeveloper objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());

            string uri = _url + "Master/developertmpDmpTerminalGetAllCounts_New";

            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                client.Timeout = TimeSpan.FromMinutes(30);               
                HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;
                TransctionDataGridResponse _HighLevelGridDataobj = new TransctionDataGridResponse();
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;
                        _HighLevelGridDataobj = JsonConvert.DeserializeObject<TransctionDataGridResponse>(jsonResponse);
                        return _HighLevelGridDataobj;
                    }
                    catch (Exception ex)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("GetPaginatedFordveloperTrasctionData Error", ex.Message);
                        return _HighLevelGridDataobj;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve EmployeeHighLevel employee data. Status code: " + response.StatusCode);
                }
            }
        }

        public DataTable GetPaginatedForDeveloperTransactionDataExport(TransctionDataTablesDeveloper objData)
        {
            DataTable dt = new DataTable();
            string uri = _url + "Master/DevelopertmpDmpTerminalGetAllExcel";

            using (HttpClient client = new HttpClient())
            {
                // Add authorization token from session if available
                var token = HttpContext.Current.Session["tokan"]?.ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", token);
                }
                client.Timeout = TimeSpan.FromMinutes(30);
                try
                {
                    HttpResponseMessage result = client.PostAsJsonAsync(uri, objData).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        string jsonString = result.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrWhiteSpace(jsonString))
                        {
                            JObject jsonObj = JObject.Parse(jsonString);
                            if (jsonObj["Table2"] is JArray table2Array && table2Array.HasValues)
                            {
                                dt = table2Array.ToObject<DataTable>();
                            }
                            else
                            {
                                dt = new DataTable();
                            }
                        }
                        else
                        {
                            throw new Exception("Failed to retrieve data. Status code: " + result.StatusCode);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Wrap the original exception with a custom message
                    throw new Exception("Error during API call: " + ex.Message, ex);
                }
            }
            return dt;
        }
        #endregion


        #region For leave Sanction Datagrid Bind Client & Server Side
        public LeaveOpeningSanctionResponse GetPaginatedForDeveloperLeaveOpeningGetAll(LeaveOpeningGetAll objleave)
        {            
            string uri = _url + "Transaction/LeaveOpeningGetAll";
            string _DbName = "";
            LeaveOpeningSanctionResponse _HighLevelGridDataobj = new LeaveOpeningSanctionResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(30);

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    HttpResponseMessage response = client.PostAsJsonAsync(uri, objleave).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData =  response.Content.ReadAsStringAsync().Result;
                        _HighLevelGridDataobj = JsonConvert.DeserializeObject<LeaveOpeningSanctionResponse>(responseData);
                        return _HighLevelGridDataobj;
                    }
                    else if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        string errorMessage =  response.Content.ReadAsStringAsync().Result;
                        throw new Exception(errorMessage); // Method exits here
                    }
                    else
                    {
                        int statusCodeValue = (int)response.StatusCode;
                        string statusCodemessage = response.ReasonPhrase;
                        throw new HttpRequestException($"HTTP Error: {statusCodeValue} - {statusCodemessage}"); // Method exits here
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveOpeningGetAll), _DbName, uri);
                return null;
            }
        }
        #endregion
        #region Analytics Dashboard
        public Report tmptableforReports(reqReports objReport)
        {
            string uri = _url + "Report/tmptableforReports";
            string _DbName = "";
            Report _HighLevelGridDataobj = new Report();
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(30);

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    HttpResponseMessage response = client.PostAsJsonAsync(uri, objReport).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = response.Content.ReadAsStringAsync().Result;
                        _HighLevelGridDataobj = JsonConvert.DeserializeObject<Report>(responseData);
                        return _HighLevelGridDataobj;
                    }
                    else if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        //string errorMessage = response.Content.ReadAsStringAsync().Result;
                        //throw new Exception(errorMessage); // Method exits here
                        return new Report();
                    }
                    else
                    {
                        //int statusCodeValue = (int)response.StatusCode;
                        //string statusCodemessage = response.ReasonPhrase;
                        //throw new HttpRequestException($"HTTP Error: {statusCodeValue} - {statusCodemessage}"); // Method exits here
                        return new Report();
                    }
                }
            }
            catch (Exception ex)
            {
                //ErrorHandler.HandleException(ex, nameof(reqReports), _DbName, uri);
                return new Report();
            }
        }
        #endregion

        #region For Notification Module Datagrid Sample Data Bind
        public NotificationDetailsViewModel GetAllNotificationRulesDetails(int nID)
        {
            var resultSet = new NotificationDetailsViewModel();

            //string uri = _url + "Alert/GetAllNotificationRulesDetailsData/" + nID;
            string uri = _url + $"Alert/GetAllNotificationRulesDetailsData?NotifyID={nID}";

            //string uri = _url + "Alert/GetAllNotificationRulesDetailsData?NotifyID=" + nID;

            using (HttpClient client = new HttpClient())
            {
                // Add token if exists
                var token = HttpContext.Current.Session["tokan"]?.ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", token);
                }

                client.Timeout = TimeSpan.FromMinutes(30);

                try
                {
                    // Send POST with NotifyID as body
                    //var response = client.PostAsJsonAsync(uri, nID).Result;
                    var response = client.PostAsync(uri, null).Result;


                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;

                        if (!string.IsNullOrWhiteSpace(jsonString))
                        {
                            JObject jsonObj = JObject.Parse(jsonString);

                            if (jsonObj["Table"] is JArray tableArray && tableArray.HasValues)
                                resultSet.Table1 = tableArray.ToObject<List<NotificationRule>>();

                            if (jsonObj["Table1"] is JArray table1Array && table1Array.HasValues)
                                resultSet.Table2 = table1Array.ToObject<List<NotificationConditionRule>>();

                            if (jsonObj["Table2"] is JArray table2Array && table2Array.HasValues)
                                resultSet.Table3 = table2Array.ToObject<List<NotificationEscalationRule>>();
                        }
                        else
                        {
                            throw new Exception("Empty response received from API.");
                        }
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve data. Status code: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error during API call: " + ex.Message, ex);
                }
            }

            return resultSet;
        }
        #endregion
    }
}