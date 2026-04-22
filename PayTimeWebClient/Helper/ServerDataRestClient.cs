using DevExpress.Xpo;
using DevExpress.XtraReports.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Models;
using PayTimeWebClient.Models.DataGridModel;
using PayTimeWebClient.Models.ReportsModel;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static PayTimeWebClient.Helper.MasterDataRestClient;

namespace PayTimeWebClient.Helper
{


    public class AccountRestClint : IAccountRestClint
    {
        private static readonly string _baseurl = ConfigurationManager.AppSettings["webapipaytime"];
        private static readonly string _baseurlMS = ConfigurationManager.AppSettings["webapipaytimeMS"];
        private static readonly string _updAttnUrl = ConfigurationManager.AppSettings["updatesvc"];
        static MRespo mp = new MRespo();


        #region Register
        public MRespo Register(LoginModel model)
        {
            using (var client = new HttpClient())
            {
                string uri = _baseurl + "Registration/Register";
                //HTTP POST
                var postTask = client.PostAsJsonAsync<LoginModel>(uri, model);
                postTask.Wait();
                var result = postTask.Result;
                mp.MegSts = JObject.Parse(result.Content.ReadAsStringAsync().Result).First.First.ToString();
                mp.Meg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();
            }
            return mp;
        }
        public static MRespo ActivateUser(string activateCode)
        {
            using (var client = new HttpClient())
            {
                string Param = "?activateCode=" + activateCode;
                string uri = _baseurl + "Registration/ActivateUser" + Param;
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, activateCode);
                postTask.Wait();
                var result = postTask.Result;
                mp.MegSts = JObject.Parse(result.Content.ReadAsStringAsync().Result).First.First.ToString();
                mp.Meg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();
                return mp;
            }
        }
        public static MRespo DeveloperActiveUser(string activateCode)
        {
            using (var client = new HttpClient())
            {
                string Param = "?activateCode=" + activateCode;
                string uri = _baseurl + "Registration/DeveloperActiveUser" + Param;
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, activateCode);
                postTask.Wait();
                var result = postTask.Result;
                mp.MegSts = JObject.Parse(result.Content.ReadAsStringAsync().Result).First.First.ToString();
                mp.Meg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();
                return mp;
            }
        }
        public static MRespo ForgotPassword(string email, string CompanyCode)
        {
            using (var client = new HttpClient())
            {
                string Param = "?email=" + email + "&CompanyCode=" + CompanyCode;
                string uri = _baseurl + "Registration/ForgotPassword" + Param;
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, email);
                postTask.Wait();
                var result = postTask.Result;
                mp.MegSts = JObject.Parse(result.Content.ReadAsStringAsync().Result).First.First.ToString();
                mp.Meg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();
                return mp;
            }
        }
        public static MRespo DevelopersPassword(string email)
        {
            using (var client = new HttpClient())
            {
                string Param = "?email=" + email;
                string uri = _baseurl + "Registration/DevelopersPassword" + Param;
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, email);
                postTask.Wait();
                var result = postTask.Result;
                mp.MegSts = JObject.Parse(result.Content.ReadAsStringAsync().Result).First.First.ToString();
                mp.Meg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();
                return mp;
            }
        }
        public static MRespo ResetPassword(string otp, string enotp, string password, string CompanyCode)
        {
            using (var client = new HttpClient())
            {
                string Param = "?otp=" + otp + "&enotp=" + enotp + "&userpass=" + password + "&CompanyCode=" + CompanyCode;
                string uri = _baseurl + "Registration/ResetPassword" + Param;
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, otp);
                postTask.Wait();
                var result = postTask.Result;
                mp.MegSts = JObject.Parse(result.Content.ReadAsStringAsync().Result).First.First.ToString();
                mp.Meg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();
                return mp;
            }
        }
        public static MRespo ResetPasswordDeveloper(string otp, string enotp, string password)
        {
            using (var client = new HttpClient())
            {
                string Param = "?otp=" + otp + "&enotp=" + enotp + "&userpass=" + password;
                string uri = _baseurl + "Registration/ResetPasswordDeveloper" + Param;
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, otp);
                postTask.Wait();
                var result = postTask.Result;
                mp.MegSts = JObject.Parse(result.Content.ReadAsStringAsync().Result).First.First.ToString();
                mp.Meg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();
                return mp;
            }
        }
        public static MRespo EmployeeResetPassword(string Password, string NewPassword, string UserEmail, int EmpId, int RoleId)
        {
            Models.EmployeeResetPassword pwdr = new Models.EmployeeResetPassword();
            pwdr.EmpId = EmpId;
            pwdr.UserEmail = UserEmail;
            pwdr.Password = Password;
            pwdr.NewPassword = NewPassword;
            pwdr.RoleId = RoleId;

            //string Param = "?enotp=" + param;
            //string uri = _baseurl + "Registration/ResetPasswordEmp" + Param;
            string uri = _baseurl + "Registration/ResetPasswordEmp";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                //HTTP POST
                var postTask = client.PostAsJsonAsync<EmployeeResetPassword>(uri, pwdr);
                //var postTask = client.PostAsJsonAsync(uri, param);
                postTask.Wait();
                var result = postTask.Result;
                mp.MegSts = JObject.Parse(result.Content.ReadAsStringAsync().Result).First.First.ToString();
                mp.Meg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();
                return mp;
            }
        }
        public static MRespo AdminResetPassword(string param)
        {
            string Param = "?enotp=" + param;
            string uri = _baseurl + "Registration/ResetPasswordAdmin" + Param;
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, param);
                postTask.Wait();
                var result = postTask.Result;
                mp.MegSts = JObject.Parse(result.Content.ReadAsStringAsync().Result).First.First.ToString();
                mp.Meg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();
                return mp;
            }
        }

        #endregion

        #region Login
        public MRespo Login(LoginModel model)
        {
            using (var client = new HttpClient())
            {
                string uri = _baseurl + "Registration/AdminLogin";
                //HTTP POST
                var postTask = client.PostAsJsonAsync<LoginModel>(uri, model);
                //GetDeviceDetails.ProcessLogLogFileWrite("Login_uri", uri);
                try
                {
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        mp.MegSts = "jwt_token";
                        mp.Meg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();
                        //GetDeviceDetails.ProcessLogLogFileWrite("Login_uri_Sucess", mp.MegSts + "--------" + result.IsSuccessStatusCode);

                    }
                    else
                    {

                        var resp = JsonConvert.DeserializeObject<MRespo>(result.Content.ReadAsStringAsync().Result);
                        mp.MegSts = resp.MegSts;
                        mp.Meg = resp.Meg;
                        //GetDeviceDetails.ProcessLogLogFileWrite("Login_uri_fail", mp.MegSts + "--------" + mp.Meg);
                    }
                }
                catch (Exception ex)
                {
                    GetDeviceDetails.ProcessLogLogFileWrite("Login_uri_Exception", ex.ToString());
                    mp.MegSts = "error";
                    mp.Meg = "Error in login";
                }

            }
            return mp;
        }
        public MRespo DeveloperLogin(LoginModel model)
        {
            using (var client = new HttpClient())
            {
                string uri = _baseurl + "Registration/DeveloperLogin";
                //HTTP POST
                var postTask = client.PostAsJsonAsync<LoginModel>(uri, model);
                try
                {
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        mp.MegSts = "jwt_token";
                        mp.Meg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();

                    }
                    else
                    {

                        var resp = JsonConvert.DeserializeObject<MRespo>(result.Content.ReadAsStringAsync().Result);
                        mp.MegSts = resp.MegSts;
                        mp.Meg = resp.Meg;

                    }
                }
                catch (Exception)
                {
                    mp.MegSts = "error";
                    mp.Meg = "Error in login";
                }

            }
            return mp;
        }
        #endregion

        #region EmployeeLogin
        public MRespo EmpLogin(LoginModel model)
        {
            using (var client = new HttpClient())
            {
                string uri = _baseurl + "Registration/Login";
                //HTTP POST
                var postTask = client.PostAsJsonAsync<LoginModel>(uri, model);
                try
                {
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        mp.MegSts = "jwt_token";
                        mp.Meg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();

                    }
                    else
                    {
                        var resp = JsonConvert.DeserializeObject<MRespo>(result.Content.ReadAsStringAsync().Result);
                        mp.MegSts = resp.MegSts;
                        mp.Meg = resp.Meg;
                    }
                }
                catch (Exception)
                {
                    mp.MegSts = "error";
                    mp.Meg = "Error in login";
                }

            }
            return mp;
        }
        #endregion

        #region
        public MRespo SubDomainLogin(LoginModel model)
        {
            using (var client = new HttpClient())
            {
                string uri = _baseurl + "Registration/LoginSubDomain";
                //HTTP POST
                var postTask = client.PostAsJsonAsync<LoginModel>(uri, model);
                try
                {
                    postTask.Wait();
                    var result = postTask.Result;
                    //GetDeviceDetails.ProcessLogLogFileWrite("SubDomainLogin_Sucess", uri + "--------" + result.IsSuccessStatusCode);
                    if (result.IsSuccessStatusCode)
                    {
                        mp.MegSts = "jwt_token";
                        mp.Meg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();
                        //GetDeviceDetails.ProcessLogLogFileWrite("SubDomainLogin_Sucess", mp.MegSts);

                    }
                    else
                    {
                        var resp = JsonConvert.DeserializeObject<MRespo>(result.Content.ReadAsStringAsync().Result);
                        mp.MegSts = resp.MegSts;
                        mp.Meg = resp.Meg;
                        //GetDeviceDetails.ProcessLogLogFileWrite("SubDomainLogin_fail", mp.MegSts);
                    }
                }
                catch (Exception ex)
                {
                    GetDeviceDetails.ProcessLogLogFileWrite("SubDomainLogin_Exception", ex.Message.ToString());
                    //GetDeviceDetails.ProcessLogLogFileWrite("SubDomainLogin_Exception", ex.Message);
                    mp.MegSts = "error";
                    mp.Meg = "Error in login";
                }

            }
            return mp;
        }
        #endregion

        #region OTPVerify
        public static MRespo OTPCheck(int OTP, string CompanyCode, string Email, int RoleId)
        {
            using (var client = new HttpClient())
            {
                string Param = "?OTP=" + OTP + "&CompanyCode=" + CompanyCode + "&Email=" + Email + "&RoleId=" + RoleId;
                string uri = _baseurl + "Registration/OTPCheck" + Param;
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, OTP);
                postTask.Wait();
                var result = postTask.Result;
                mp.MegSts = JObject.Parse(result.Content.ReadAsStringAsync().Result).First.First.ToString();
                mp.Meg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();
                return mp;
            }
        }
        #endregion

        public class EmailCredentials
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public MRespo GetEmailPassword()
        {
            string uri = _baseurl + "TPOnboarding/GetEmailPassword"; // Ensure this matches your API route
            MRespo mres = new MRespo();

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    // 1. Session Authorization (Matching your Reports method style)
                    if (HttpContext.Current.Session["tokan"] != null)
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }

                    httpClient.Timeout = TimeSpan.FromMinutes(15);

                    // 2. Call API Synchronously using .Result
                    Task<HttpResponseMessage> response = httpClient.GetAsync(uri);
                    var res = response.Result.Content.ReadAsStringAsync().Result;

                    // 3. Deserialize into your EmailCredentials class
                    var creds = JsonConvert.DeserializeObject<EmailCredentials>(res.ToString());

                    if (creds != null)
                    {
                        mres.MegSts = "Success";
                        // We use the 'Meg' field to store the data since you cannot change MRespo
                        // We serialize it back to a string or use specific logic in the controller
                        mres.Meg = res.ToString();
                    }
                    else
                    {
                        mres.MegSts = "Error";
                        mres.Meg = "Could not parse credentials";
                    }
                }
                catch (Exception ex)
                {
                    mres.MegSts = "Exception";
                    mres.Meg = ex.Message;
                }
            }
            return mres;
        }
    }
    public class ImportHeadfaillog
    {
        public string head_name { get; set; }
        public bool issuccess { get; set; }
        public bool isduplicate { get; set; }

    }
    public class ApiResponse
    {
        public string head_name { get; set; }
        public bool isduplicate { get; set; }
        public bool issuccess { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public object ValidationErrors { get; set; }
        public object Responsedata { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseInfo { get; set; }
        public List<ImportHeadfaillog> Importfailloglist { get; set; }

    }
    public class MasterDataRestClient
    {
        private static readonly string _url = ConfigurationManager.AppSettings["webapipaytime"];
        private readonly string _Payrollurl = ConfigurationManager.AppSettings["webapipaytimePayroll"];
        #region ImportData
        public ApiResponse ImportData(List<ImportedDataExcel> items)
        {
            ApiResponse objresutl = new ApiResponse();
            List<Importfaillog> objlstfailed = new List<Importfaillog>();
            string uri = _Payrollurl + "ImportExcel/ImportExcelData";
            using (HttpClient client = new HttpClient())
            {
                var timeout = client.Timeout;
                var addedvalue = timeout.Add(new TimeSpan(0, 30, 0));
                client.Timeout = addedvalue;
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync<List<ImportedDataExcel>>(uri, items);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(res))
                {
                    if (res.Length > 2)
                    {
                        objresutl = JsonConvert.DeserializeObjectAsync<ApiResponse>(res.ToString()).Result;

                    }
                }
                return objresutl;
            }
            #endregion

        }

        
        

        //public string CountryTimeZoneAll(int id)
        //{
        //    // bool resp = false;

        //    using (var client = new HttpClient())
        //    {

        //        if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
        //        {
        //            client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
        //        }
        //        string uri = _url + "Master/DepartmentDelete/" + id;
        //        //HTTP POST
        //        var postTask = client.PostAsJsonAsync(uri, id);
        //        postTask.Wait();

        //        var result = postTask.Result;

        //        return result.IsSuccessStatusCode;
        //    }
        //}
        public Resp Savecompanyaddon(List<companyAddon> objAddon)
        {

            Resp rsp = new Resp();
            using (var client = new HttpClient())
            {

                companyAddon model = new companyAddon();
                string uri = _url + "Master/Savecompanyaddon";
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, objAddon);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    rsp.MsgSts = JObject.Parse(result.Content.ReadAsStringAsync().Result).First.First.ToString();
                    rsp.Msg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();
                }
                return rsp;
            }
        }

        #region SuperAdminDynamicReport
        public DataTable DynamicReports(SuperAdminReportsFilter objreq)
        {
            string uri = _url + "DashBoard/SuperAdminReportsGrid";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<SuperAdminReportsFilter>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var AnalyticReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;

                AnalyticReport.TableName = "ReportData";
                return AnalyticReport;

            }
        }
        #endregion
        #region  CompanyMaster-done
        public IEnumerable<Companys> CompanyGetAll()
        {
            string _DbName = "";
            string uri = _url + "Master/CompanyGetAll";
            IEnumerable<Companys> cmp = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                        if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                        {
                            _DbName = HttpContext.Current.Session["DbName"].ToString();
                        }
                        Task<HttpResponseMessage> response = httpClient.GetAsync(uri);
                        var result = response.Result;
                        if (response.Result.IsSuccessStatusCode)
                        {
                            var res = response.Result.Content.ReadAsStringAsync().Result;
                            cmp = JsonConvert.DeserializeObjectAsync<List<Companys>>(res.ToString()).Result;

                            return cmp;
                        }
                        else if (result.StatusCode == HttpStatusCode.Conflict)
                        {
                            var res1 = response.Result.Content.ReadAsStringAsync().Result;
                            cmp = JsonConvert.DeserializeObjectAsync<List<Companys>>(res1.ToString()).Result;
                            return cmp;

                        }
                        else
                        {
                            int statusCodeValue = (int)response.Result.StatusCode;
                            string statusCodemessage = response.Result.ReasonPhrase;
                            string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                            throw new HttpRequestException(errorDetails);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CompanyGetAll), _DbName, uri);
            }
            return cmp;
        }
        public IEnumerable<BUMaster> BUGetAll()
        {
            string _DbName = "";
            string uri = _url + "Master/BUDropdownGetAll";
            IEnumerable<BUMaster> cmp = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                        if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                        {
                            _DbName = HttpContext.Current.Session["DbName"].ToString();
                        }
                        Task<HttpResponseMessage> response = httpClient.GetAsync(uri);
                        var result = response.Result;
                        if (response.Result.IsSuccessStatusCode)
                        {
                            var res = response.Result.Content.ReadAsStringAsync().Result;
                            cmp = JsonConvert.DeserializeObjectAsync<List<BUMaster>>(res.ToString()).Result;

                            return cmp;
                        }
                        else if (result.StatusCode == HttpStatusCode.Conflict)
                        {
                            var res1 = response.Result.Content.ReadAsStringAsync().Result;
                            cmp = JsonConvert.DeserializeObjectAsync<List<BUMaster>>(res1.ToString()).Result;
                            return cmp;

                        }
                        else
                        {
                            int statusCodeValue = (int)response.Result.StatusCode;
                            string statusCodemessage = response.Result.ReasonPhrase;
                            string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                            throw new HttpRequestException(errorDetails);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CompanyGetAll), _DbName, uri);
            }
            return cmp;
        }
        public IEnumerable<SBUMaster> SBUGetAll(string buId)
        {
            string _DbName = "";
            string uri = _url + "Master/SubBUDropdown?BUId=" + buId;
            IEnumerable<SBUMaster> cmp = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                        if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                        {
                            _DbName = HttpContext.Current.Session["DbName"].ToString();
                        }
                        Task<HttpResponseMessage> response = httpClient.GetAsync(uri);
                        var result = response.Result;
                        if (response.Result.IsSuccessStatusCode)
                        {
                            var res = response.Result.Content.ReadAsStringAsync().Result;
                            cmp = JsonConvert.DeserializeObjectAsync<List<SBUMaster>>(res.ToString()).Result;

                            return cmp;
                        }
                        else if (result.StatusCode == HttpStatusCode.Conflict)
                        {
                            var res1 = response.Result.Content.ReadAsStringAsync().Result;
                            cmp = JsonConvert.DeserializeObjectAsync<List<SBUMaster>>(res1.ToString()).Result;
                            return cmp;

                        }
                        else
                        {
                            int statusCodeValue = (int)response.Result.StatusCode;
                            string statusCodemessage = response.Result.ReasonPhrase;
                            string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                            throw new HttpRequestException(errorDetails);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CompanyGetAll), _DbName, uri);
            }
            return cmp;
        }

        public IEnumerable<Companys> CompanyGetAll(string Fieldname, string Fieldvalue)
        {
            string _DbName = "";
            var _param = Fieldname + "=" + Fieldvalue;
            string uri = _url + "Master/CompanyGetAll?" + _param;
            IEnumerable<Companys> cmp = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                        if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                        {
                            _DbName = HttpContext.Current.Session["DbName"].ToString();
                        }
                        Task<HttpResponseMessage> response = httpClient.GetAsync(uri);
                        var result = response.Result;
                        if (response.Result.IsSuccessStatusCode)
                        {

                            var res = response.Result.Content.ReadAsStringAsync().Result;
                            cmp = JsonConvert.DeserializeObjectAsync<List<Companys>>(res.ToString()).Result;
                            return cmp;
                        }
                        else if (result.StatusCode == HttpStatusCode.Conflict)
                        {

                            return cmp;


                        }
                        else
                        {
                            int statusCodeValue = (int)response.Result.StatusCode;
                            string statusCodemessage = response.Result.ReasonPhrase;
                            string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                            throw new HttpRequestException(errorDetails);
                        }
                    }


                }
            }
            catch
            (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CompanyGetAll), _DbName, uri);
            }
            return cmp;
        }
        public Companys CompanyAdd(Companys Des)
        {
            string _DbName = "";
            string uri = _url + "Master/CompanyCreate";
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                        if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                        {
                            _DbName = HttpContext.Current.Session["DbName"].ToString();
                        }


                        //HTTP POST
                        var postTask = client.PostAsJsonAsync<Companys>(uri, Des);
                        postTask.Wait();
                        var response = postTask.Result;
                        if (response.IsSuccessStatusCode)
                        {

                            return new Companys();
                        }
                        else if (response.StatusCode == HttpStatusCode.Conflict)
                        {
                            return new Companys();
                        }
                        else
                        {
                            int statusCodeValue = (int)response.StatusCode;
                            string statusCodemessage = response.ReasonPhrase;
                            string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                            throw new HttpRequestException(errorDetails);
                        }



                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CompanyAdd), _DbName, uri);
            }
            return new Companys();
        }
        public bool CompanyDelete(int id)
        {
            bool resp = false;
            string _DbName = "";
            string uri = _url + "Master/CompanyDelete/" + id;
            try
            {

                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                        if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                        {
                            _DbName = HttpContext.Current.Session["DbName"].ToString();
                        }


                        //HTTP POST
                        var postTask = client.PostAsJsonAsync(uri, id);
                        postTask.Wait();

                        var response = postTask.Result;
                        if (response.IsSuccessStatusCode)
                        {

                            resp = response.IsSuccessStatusCode;
                            return resp;
                        }
                        else if (response.StatusCode == HttpStatusCode.Conflict)
                        {
                            return false;
                        }
                        else
                        {
                            int statusCodeValue = (int)response.StatusCode;
                            string statusCodemessage = response.ReasonPhrase;
                            string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                            throw new HttpRequestException(errorDetails);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CompanyDelete), _DbName, uri);
            }
            return resp;
        }
        public bool CompanyUpdate(int id, Companys Des)
        {

            string uri = _url + "Master/CompanyUpdate/" + id;
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Companys>(uri, Des);
                    postTask.Wait();

                    var response = postTask.Result;
                    if (response.IsSuccessStatusCode)
                    {

                        return response.IsSuccessStatusCode;
                    }
                    else if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        return false;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.StatusCode;
                        string statusCodemessage = response.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CompanyUpdate), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region Department-done
        public IEnumerable<Departments> DepartmentGetAll()
        {
            string uri = _url + "Master/DepartmentGetAll";
            string _DbName = "";
            IEnumerable<Departments> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Departments>>(res.ToString()).Result;
                        return cmp;
                    }

                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DepartmentGetAll), _DbName, uri);
            }
            return cmp;

        }
        public IEnumerable<Departments> GetDeptbyBranch(int branchid)
        {
            string param = "?BranchId=" + branchid;
            string uri = _url + "Master/GetDeptbyBranch" + param;
            string _DbName = "";
            IEnumerable<Departments> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Departments>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetDeptbyBranch), _DbName, uri);
            }
            return cmp;
        }

        public IEnumerable<Departments> GetDeptbyBranchstr(string branchid)
        {
            string param = "?BranchId=" + branchid;
            string uri = _url + "Master/GetDeptbyBranchstr" + param;
            string _DbName = "";
            IEnumerable<Departments> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Departments>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetDeptbyBranchstr), _DbName, uri);
            }
            return cmp;

        }




        public async Task<IEnumerable<Departments>> GetDeptbyBranchstrnew(string branchid)
        {
            GetAllBranchstr objemp = new GetAllBranchstr();
            objemp.BranchId = branchid;
            string _DbName = "";
            IEnumerable<Departments> cmp = null;
            //string param = "?BranchId=" + branchid;
            string uri = _url + "Master/GetDeptbyBranchstrnew";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync(uri, objemp);
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        // Read the response content and deserialize it
                        string responseData = await result.Content.ReadAsStringAsync();
                        cmp = JsonConvert.DeserializeObject<List<Departments>>(responseData);
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)result.StatusCode;
                        string statusCodemessage = result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetDeptbyBranchstrnew), _DbName, uri);
            }
            return cmp;
        }

        public async Task<IEnumerable<Designations>> GetDesigbyDepartstrnew(string branchid, string deptId)
        {
            GetAllBranchDepartstr objemp = new GetAllBranchDepartstr();
            objemp.BranchId = branchid;
            objemp.DeptId = deptId;
            string _DbName = "";
            IEnumerable<Designations> cmp = null;
            string uri = _url + "Master/GetDesifbyDeptstrnew";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync(uri, objemp);
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        // Read the response content and deserialize it
                        string responseData = await result.Content.ReadAsStringAsync();
                        //cmp = JsonConvert.DeserializeObject<List<Departments>>(responseData);
                        cmp = JsonConvert.DeserializeObject<List<Designations>>(responseData);
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)result.StatusCode;
                        string statusCodemessage = result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetDeptbyBranchstrnew), _DbName, uri);
            }
            return cmp;
        }


        public async Task<IEnumerable<Getdevicealltype>> GetDevicenew(string branchid)
        {
            //   Getdevice objemp = new Getdevice();
            Getdevicealltype objemp = new Getdevicealltype();
            objemp.BranchId = branchid;
            string _DbName = "";
            IEnumerable<Getdevicealltype> cmp = null;

            //string param = "?BranchId=" + branchid;
            string uri = _url + "Master/DeviceGet";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync(uri, objemp);
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        // Read the response content and deserialize it
                        string responseData = await result.Content.ReadAsStringAsync();
                        cmp = JsonConvert.DeserializeObject<List<Getdevicealltype>>(responseData);

                        return cmp;
                        //DataTable dt = ConvertToDataTable(cmp);

                        //return dt;
                    }
                    else
                    {
                        int statusCodeValue = (int)result.StatusCode;
                        string statusCodemessage = result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetDevicenew), _DbName, uri);
            }
            return cmp;
        }
        public IEnumerable<Departments> GetDeptbyBranchstrOnboarding(string branchid)
        {
            string param = "?BranchId=" + branchid;
            string uri = _url + "Master/GetDeptbyBranchstrOnboarding" + param;
            string _DbName = "";
            IEnumerable<Departments> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Departments>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetDeptbyBranchstrOnboarding), _DbName, uri);
            }
            return cmp;

        }


        public IEnumerable<Departments> GetDeptbyBranchstrfltr(BranchGetbyDept obj)
        {
            string uri = _url + "Master/GetDeptbyBranchstrfltr";
            string _DbName = "";
            IEnumerable<Departments> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync(uri, obj);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Departments>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetDeptbyBranchstrfltr), _DbName, uri);
            }
            return cmp;

        }

        public Departments DepartmentAdd(Departments d)
        {
            //ModelState.Remove("DepartmentHead");
            string _DbName = "";
            string uri = _url + "Master/DepartmentCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Departments>(uri, d);
                    postTask.Wait();

                    var result = postTask.Result;
                    // System.Net.HttpStatusCode status = System.Net.HttpStatusCode.OK;
                    if (result.IsSuccessStatusCode)
                    {
                        return new Departments();
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        return new Departments();
                    }
                    else
                    {
                        int statusCodeValue = (int)result.StatusCode;
                        string statusCodemessage = result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DepartmentAdd), _DbName, uri);
            }
            return new Departments();
        }
        public bool DepartmentDelete(int id)
        {
            // bool resp = false;
            string _DbName = "";
            string uri = _url + "Master/DepartmentDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (postTask.Result.IsSuccessStatusCode)
                    {



                        return result.IsSuccessStatusCode;
                    }

                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DepartmentDelete), _DbName, uri);
            }
            return false;
        }
        public bool DepartmentUpdate(int id, Departments Dept)
        {
            string _DbName = "";
            string uri = _url + "Master/DepartmentUpdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Departments>(uri, Dept);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {

                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DepartmentUpdate), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region Designations-done
        public IEnumerable<Designations> DesignationsGetAll()
        {
            string uri = _url + "Master/DesignationGetAll";
            string _DbName = "";
            IEnumerable<Designations> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Designations>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DesignationsGetAll), _DbName, uri);
            }
            return cmp;
        }

        //public IEnumerable<Designations> DesignationsGetAll(string DesigName)
        //{
        //    var param = "?DesigName=" + DesigName;
        //    string uri = _url + "Master/DesignationGetAll" + param;
        //    using (HttpClient client = new HttpClient())
        //    {
        //        if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
        //        {
        //            client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
        //        }
        //        Task<HttpResponseMessage> response = client.GetAsync(uri);
        //        var res = response.Result.Content.ReadAsStringAsync().Result;
        //        var cmp = JsonConvert.DeserializeObjectAsync<List<Designations>>(res.ToString()).Result;
        //        return cmp;
        //    }
        //}

        public Designations DesignationsAdd(Designations Desg)
        {
            string _DbName = "";
            string uri = _url + "Master/DesignationCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Designations>(uri, Desg);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (postTask.Result.IsSuccessStatusCode)
                    {

                        return new Designations();

                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        return new Designations();
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DesignationsAdd), _DbName, uri);
            }
            return new Designations();
        }
        public bool DesignationsDelete(int id)
        {
            // bool resp = false;
            string _DbName = "";
            string uri = _url + "Master/DesignationDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DesignationsDelete), _DbName, uri);
            }
            return false;
        }
        public bool DesignationsUpdate(int id, Designations Desg)
        {
            string _DbName = "";
            string uri = _url + "Master/DesignationUpdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Designations>(uri, Desg);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DesignationsUpdate), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region Holidays-done
        public IEnumerable<Holidays> HolidaysGetAll()
        {
            string uri = _url + "Master/HolidayMasterGetAll";
            string _DbName = "";
            IEnumerable<Holidays> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Holidays>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(HolidaysGetAll), _DbName, uri);
            }
            return cmp;

        }
        public ErrorMsg HolidaysAdd(Holidays hd)
        {
            ErrorMsg err = new ErrorMsg();
            hd.RoleId = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            hd.LoginEmpId = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {
                var holidaytodate = hd.HolidayToDate.Split('-');
                var holidaydate = hd.HolidayDate.Split('-');
                hd.HolidayDate = holidaydate[2] + "-" + holidaydate[1] + "-" + holidaydate[0];
                hd.HolidayToDate = holidaytodate[2] + "-" + holidaytodate[1] + "-" + holidaytodate[0];
            }
            else if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var holidaytodate = hd.HolidayToDate.Split('-');
                var holidaydate = hd.HolidayDate.Split('-');
                hd.HolidayDate = holidaydate[2] + "-" + holidaydate[0] + "-" + holidaydate[1];
                hd.HolidayToDate = holidaytodate[2] + "-" + holidaytodate[0] + "-" + holidaytodate[1];
            }
            else if (Isdateformat.ToString().Trim() == "yyyy-mm-dd")
            {
                var holidaytodate = hd.HolidayToDate.Split('-');
                var holidaydate = hd.HolidayDate.Split('-');
                hd.HolidayDate = holidaydate[0] + "-" + holidaydate[1] + "-" + holidaydate[2];
                hd.HolidayToDate = holidaytodate[0] + "-" + holidaytodate[1] + "-" + holidaytodate[2];
            }
            string _DbName = "";
            string uri = _url + "Master/HolidayMasterCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Holidays>(uri, hd);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (postTask.Result.IsSuccessStatusCode)
                    {

                        var res = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        var res = result.Content.ReadAsStringAsync().Result;

                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        return err;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(HolidaysAdd), _DbName, uri);
            }
            return err;
        }
        public bool HolidaysDelete(int id)
        {
            // bool resp = false;
            string _DbName = "";
            string uri = _url + "Master/HolidayMasterDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(HolidaysDelete), _DbName, uri);
            }
            return false;
        }
        public MRespo HolidaysUpdate(int id, Holidays hd)
        {

            hd.RoleId = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            hd.LoginEmpId = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {
                var holidaytodate = hd.HolidayToDate.Split('-');
                var holidaydate = hd.HolidayDate.Split('-');
                hd.HolidayDate = holidaydate[2] + "-" + holidaydate[1] + "-" + holidaydate[0];
                hd.HolidayToDate = holidaytodate[2] + "-" + holidaytodate[1] + "-" + holidaytodate[0];
            }
            else if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var holidaytodate = hd.HolidayToDate.Split('-');
                var holidaydate = hd.HolidayDate.Split('-');
                hd.HolidayDate = holidaydate[2] + "-" + holidaydate[0] + "-" + holidaydate[1];
                hd.HolidayToDate = holidaytodate[2] + "-" + holidaytodate[0] + "-" + holidaytodate[1];
            }
            else if (Isdateformat.ToString().Trim() == "yyyy-mm-dd")
            {
                var holidaytodate = hd.HolidayToDate.Split('-');
                var holidaydate = hd.HolidayDate.Split('-');
                hd.HolidayDate = holidaydate[0] + "-" + holidaydate[1] + "-" + holidaydate[2];
                hd.HolidayToDate = holidaytodate[0] + "-" + holidaytodate[1] + "-" + holidaytodate[2];
            }
            string _DbName = "";
            string uri = _url + "Master/HolidayMasterUpdate";
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Holidays>(uri, hd);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp1;


                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                    // return result.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(HolidaysUpdate), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<MRespo>(res);
        }
        #endregion

        #region ShiftGroup-done
        public IEnumerable<ShiftGroup> ShiftGroupGetAll()
        {
            string uri = _url + "Master/ShiftGroupMasterGetAll";
            string _DbName = "";
            IEnumerable<ShiftGroup> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<ShiftGroup>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftGroupGetAll), _DbName, uri);
            }
            return cmp;
        }

        public IEnumerable<ShiftGroup> ShiftGroupGetAll(string RoleId, int cmpid, int BranchID)
        {
            var param = "?RoleId=" + RoleId + "&cmpid=" + cmpid + "&BranchID=" + BranchID;
            string uri = _url + "Master/ShiftGroupMasterGetAll" + param;
            string _DbName = "";
            IEnumerable<ShiftGroup> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<ShiftGroup>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftGroupGetAll), _DbName, uri);
            }
            return cmp;
        }

        public IEnumerable<ShiftGroup> ShiftGroupGetAll(string shftgropname)
        {
            var param = "?ShiftGroupShortName=" + shftgropname;
            string uri = _url + "Master/ShiftGroupMasterGetAll" + param;
            string _DbName = "";
            IEnumerable<ShiftGroup> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<ShiftGroup>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }


                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftGroupGetAll), _DbName, uri);

            }
            return cmp;
        }

        public ShiftGroup ShiftGroupAdd(ShiftGroup sg)
        {
            string _DbName = "";
            string uri = _url + "Master/ShiftGroupMasterCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<ShiftGroup>(uri, sg);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (postTask.Result.IsSuccessStatusCode)
                    {

                        return new ShiftGroup();
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        return new ShiftGroup();


                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftGroupAdd), _DbName, uri);
            }
            return new ShiftGroup();
        }
        public bool ShiftGroupDelete(int id)
        {
            // bool resp = false;
            string _DbName = "";
            string uri = _url + "Master/ShiftGroupMasterDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftGroupDelete), _DbName, uri);
            }
            return false;
        }
        public bool ShiftGroupUpdate(int id, ShiftGroup sg)
        {
            string _DbName = "";
            string uri = _url + "Master/ShiftGroupMasterUpdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<ShiftGroup>(uri, sg);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftGroupUpdate), _DbName, uri);
            }
            return false;
        }

        public string CheckShiftGroupShortName(string shiftgroupshortname)
        {
            string Param = "?shiftgroupshortname=" + shiftgroupshortname;
            string uri = _url + "Master/ShiftGroupShortNameCheck" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CheckShiftGroupShortName), _DbName, uri);
            }
            return res;
        }
        #endregion

        #region Shift-done
        public IEnumerable<Shifts> ShiftGetAll()
        {
            string _DbName = "";
            string uri = _url + "Master/ShiftMasterGetAll";
            IEnumerable<Shifts> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Shifts>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftGetAll), _DbName, uri);
            }
            return cmp;
        }
        public IEnumerable<Shifts> ShiftGetAll(string RoleId, int cmpid, int BranchID)
        {
            var param = "?RoleId=" + RoleId + "&cmpid=" + cmpid + "&BranchID=" + BranchID;
            string uri = _url + "Master/ShiftMasterGetAll" + param;
            string _DbName = "";
            IEnumerable<Shifts> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Shifts>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftGetAll), _DbName, uri);
            }
            return cmp;
        }

        public IEnumerable<Shifts> ShiftGetAll(string shiftname, string fieldType)
        {
            string Param = "?shiftname=" + shiftname + "&fieldType='" + fieldType + "'";
            string uri = _url + "Master/ShiftMasterGetAll" + Param;
            string _DbName = "";
            IEnumerable<Shifts> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Shifts>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftGetAll), _DbName, uri);
            }
            return cmp;
        }

        public ErrorMsg ShiftAdd(Shifts s)
        {
            string _DbName = "";
            ErrorMsg err = new ErrorMsg();
            string uri = _url + "Master/ShiftMasterCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Shifts>(uri, s);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        var res = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        return err;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result = postTask.Result;
                        var res1 = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res1.ToString()).Result;
                        return err;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftAdd), _DbName, uri);
            }
            return err;
        }
        public bool ShiftDelete(int id)
        {
            string _DbName = "";
            string uri = _url + "Master/ShiftMasterDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftDelete), _DbName, uri);
            }
            return false;
        }
        public MRespo ShiftUpdate(int id, Shifts s)
        {
            string _DbName = "";
            string uri = _url + "Master/ShiftMasterUpdate/" + id;
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Shifts>(uri, s);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp1;


                    }
                    else if (result.StatusCode == HttpStatusCode.BadRequest)
                    {

                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp1;

                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftUpdate), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<MRespo>(res);
        }
        public string CheckShiftShortName(string shiftshortname)
        {
            string Param = "?shiftshortname=" + shiftshortname;
            string uri = _url + "Master/ShiftShortNameCheck" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CheckShiftShortName), _DbName, uri);
            }
            return res;

        }
        public string CheckShiftName(string shiftname)
        {
            string Param = "?shiftname=" + shiftname;
            string uri = _url + "Master/CheckShiftName" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }


                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CheckShiftName), _DbName, uri);
            }
            return res;
        }
        #endregion

        #region Employee-done

        public IEnumerable<Employees> EmployeeGetAll()
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());

            if (UserRoleid == 6805 || UserRoleid == 6806)
            {
                UserRoleid = 1;
            }
            string _DbName = "";
            IEnumerable<Employees> cmp = null;
            string uri = _url + "Master/EmployeeGetAll?roleid=" + UserRoleid + "&loginempid=" + UserEmpid + "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    var res = response.Result.Content.ReadAsStringAsync().Result;
                    cmp = Enumerable.Empty<Employees>();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        cmp = JsonConvert.DeserializeObjectAsync<List<Employees>>(res.ToString()).Result;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                    if (Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString()) == 6805)
                    {
                        int cmpid = Convert.ToInt32(HttpContext.Current.Session["ClientCompanyId"].ToString());
                        cmp = cmp.Where(x => x.CompanyID == cmpid && x.IsActive == true);
                    }

                    if (Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString()) == 6806)
                    {
                        int branchid = Convert.ToInt32(HttpContext.Current.Session["BranchId"].ToString());
                        cmp = cmp.Where(x => x.BranchId == branchid && x.IsActive == true);
                    }
                    return cmp;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmployeeGetAll), _DbName, uri);
            }
            return cmp;
        }

        public IEnumerable<Employees> EmployeeGetAll(int hierarchyState)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            if (UserRoleid == 6805 || UserRoleid == 6806 || UserRoleid == 6810)
            {
                UserRoleid = 1;
            }
            string _DbName = "";
            IEnumerable<Employees> cmp = null;
            string uri = _url + "Master/EmployeeGetAll?roleid=" + UserRoleid + "&loginempid=" + UserEmpid + "&hierarchyState=" + hierarchyState + "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    var res = response.Result.Content.ReadAsStringAsync().Result;
                    cmp = Enumerable.Empty<Employees>();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        cmp = JsonConvert.DeserializeObjectAsync<List<Employees>>(res.ToString()).Result;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                    if (Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString()) == 6805)
                    {
                        int cmpid = Convert.ToInt32(HttpContext.Current.Session["ClientCompanyId"].ToString());
                        cmp = cmp.Where(x => x.CompanyID == cmpid && x.IsActive == true);
                    }
                    if (Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString()) == 6806)
                    {
                        int branchid = Convert.ToInt32(HttpContext.Current.Session["BranchId"].ToString());
                        cmp = cmp.Where(x => x.BranchId == branchid && x.IsActive == true);
                    }
                    return cmp;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmployeeGetAll), _DbName, uri);
            }
            return cmp;
        }

        public IEnumerable<Employees> EmployeeGetAlls(int hierarchyState)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            if (UserRoleid == 6805 || UserRoleid == 6806 || UserRoleid == 6810)
            {
                UserRoleid = 1;
            }
            string _DbName = "";
            IEnumerable<Employees> cmp = null;
            string uri = _url + "Master/EmployeeGetAll?roleid=" + UserRoleid + "&loginempid=" + UserEmpid + "&hierarchyState=" + hierarchyState + "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    var res = response.Result.Content.ReadAsStringAsync().Result;
                    cmp = Enumerable.Empty<Employees>();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        cmp = JsonConvert.DeserializeObjectAsync<List<Employees>>(res.ToString()).Result;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                    if (Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString()) == 6805)
                    {
                        int cmpid = Convert.ToInt32(HttpContext.Current.Session["ClientCompanyId"].ToString());
                        cmp = cmp.Where(x => x.CompanyID == cmpid && x.IsActive == true);
                    }
                    if (Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString()) == 6806)
                    {
                        int branchid = Convert.ToInt32(HttpContext.Current.Session["BranchId"].ToString());
                        int empid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
                        if (empid != 0)
                        {
                            var branches = BranchlistGet(HttpContext.Current.Session["ClientCompanyId"].ToString(), empid)?.ToList();
                            List<int> branchIds = branches != null ? branches.Select(b => b.BranchId).ToList() : new List<int>();
                            cmp = cmp.Where(x => branchIds.Contains(x.BranchId) && x.IsActive == true);
                        }
                        else
                        {
                            cmp = cmp.Where(x => x.BranchId == branchid && x.IsActive == true);
                        }
                    }
                    return cmp;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmployeeGetAll), _DbName, uri);
            }
            return cmp;
        }

        public MRespo changeemphierarchy(int empid, int Ishierchy)
        {
            string uri = _url + "Master/GetHierarchyLevelState?empid=" + empid + "&Ishierchy=" + Ishierchy;
            string _DbName = "";
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    // Add the Authorization token from session if it exists
                    if (HttpContext.Current.Session["tokan"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    // Construct the URI for the API call


                    // Prepare the POST request (this should ideally be a GET request since you're passing parameters in the URL, but I'll retain POST based on your initial code)
                    var postTask = client.PostAsync(uri, null);  // null for body since you're using URL parameters
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        // Read the response
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                    // Deserialize the response to your MRespo object
                    var cmp = JsonConvert.DeserializeObject<MRespo>(res);

                    // Return the deserialized result
                    return cmp;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(changeemphierarchy), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<MRespo>(res);
        }
        public DataTable dtEmployeeGetAll()
        {
            DataTable dt = new DataTable();
            string uri = _url + "Master/dtEmployeeGetAll";
            string _DbName = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        dt = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                        dt.TableName = "Employeemaster";
                        return dt;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var res1 = response.Result.Content.ReadAsStringAsync().Result;
                        dt = JsonConvert.DeserializeObjectAsync<DataTable>(res1.ToString()).Result;
                        dt.TableName = "Employeemaster";
                        return dt;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(dtEmployeeGetAll), _DbName, uri);
            }
            DataTable emptyTable = new DataTable("Employeemaster");
            return emptyTable;

        }
        public DatatableCounts EmployeeGetAllCounts(DataTableEmployeePostModel model)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            int cmpid = 0;
            int branchid = 0;
            if (UserRoleid == 6805)
            {
                cmpid = Convert.ToInt32(HttpContext.Current.Session["ClientCompanyId"].ToString());
            }
            if (UserRoleid == 6806)
            {
                cmpid = Convert.ToInt32(HttpContext.Current.Session["ClientCompanyId"].ToString());
                branchid = Convert.ToInt32(HttpContext.Current.Session["BranchId"].ToString());
            }
            model.roleid = UserRoleid;
            model.loginempid = UserEmpid;
            model.cmpid = cmpid;
            model.branchid = branchid;
            string uri = _url + "Master/EmployeeGetAllCounts";
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    var postTask = client.PostAsJsonAsync<DataTableEmployeePostModel>(uri, model);
                    postTask.Wait();

                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var response = postTask.Result.Content.ReadAsStringAsync().Result;
                        DatatableCounts DataCounts = JsonConvert.DeserializeObjectAsync<DatatableCounts>(response.ToString()).Result;
                        return DataCounts;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var response1 = postTask.Result.Content.ReadAsStringAsync().Result;
                        DatatableCounts DataCounts = JsonConvert.DeserializeObjectAsync<DatatableCounts>(response1.ToString()).Result;
                        return DataCounts;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmployeeGetAllCounts), _DbName, uri);
            }
            return new DatatableCounts();
        }
        public IEnumerable<Employees> EmployeeGetAll(DataTableEmployeePostModel model)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            int cmpid = 0;
            int branchid = 0;
            //6808 for Payroll HR 
            if (UserRoleid == 6805)
            {
                cmpid = Convert.ToInt32(HttpContext.Current.Session["ClientCompanyId"].ToString());
            }
            if (UserRoleid == 6806)
            {
                cmpid = Convert.ToInt32(HttpContext.Current.Session["ClientCompanyId"].ToString());
                branchid = Convert.ToInt32(HttpContext.Current.Session["BranchId"].ToString());
            }
            if (UserRoleid > 1 && UserRoleid != 6805 && UserRoleid != 6806)
            {
                cmpid = Convert.ToInt32(HttpContext.Current.Session["ClientCompanyId"].ToString());
            }
            model.roleid = UserRoleid;
            model.loginempid = UserEmpid;
            model.cmpid = cmpid;
            model.branchid = branchid;
            string uri = _url + "Master/EmployeeGetAll";
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<DataTableEmployeePostModel>(uri, model);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {

                        var result = postTask.Result.Content.ReadAsStringAsync().Result;

                        var ManualPunch = JsonConvert.DeserializeObjectAsync<List<Employees>>(result.ToString()).Result;
                        return ManualPunch;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmployeeGetAll), _DbName, uri);
            }
            return Enumerable.Empty<Employees>();
        }

        public IEnumerable<Employees> EmployeeGetAllReporting()
        {
            string uri = _url + "Master/EmployeeGetAllReporting";
            string _DbName = "";
            IEnumerable<Employees> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    var res = response.Result.Content.ReadAsStringAsync().Result;
                    cmp = Enumerable.Empty<Employees>();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        cmp = JsonConvert.DeserializeObjectAsync<List<Employees>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmployeeGetAllReporting), _DbName, uri);
            }
            return cmp;
        }
        public IEnumerable<Employees> GetReportingHeadbyEmpID()
        {
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string Param = "?id=" + UserEmpid;
            string uri = _url + "Master/GetReportingHeadbyEmpID" + Param;
            string _DbName = "";
            IEnumerable<Employees> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    var res = response.Result.Content.ReadAsStringAsync().Result;
                    cmp = Enumerable.Empty<Employees>();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        cmp = JsonConvert.DeserializeObjectAsync<List<Employees>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetReportingHeadbyEmpID), _DbName, uri);
            }
            return cmp;
        }
        public bool EmployeeAdd1(Employees emp)
        {
            string _DbName = "";
            string uri = _url + "Master/EmployeeCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Employees>(uri, emp);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmployeeAdd1), _DbName, uri);
            }
            return false;
        }
        public MRespo EmployeeAdd(Employees emp)
        {
            string _DbName = "";
            string uri = _url + "Master/EmployeeCreate";
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Employees>(uri, emp);
                    postTask.Wait();
                    //var result = postTask.Result;
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmployeeAdd), _DbName, uri);

            }
            return JsonConvert.DeserializeObject<MRespo>(res);
        }
        public bool EmployeeDelete(int id)
        {
            string _DbName = "";
            string uri = _url + "Master/EmployeeDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmployeeDelete), _DbName, uri);

            }
            return false;
        }
        public MRespo EmployeeUpdate(int id, EmployeesExtended emp)
        {
            string _DbName = "";
            string uri = _url + "Master/EmployeeUpdate/" + id;
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Employees>(uri, emp);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmployeeUpdate), _DbName, uri);

            }
            return JsonConvert.DeserializeObject<MRespo>(res);
        }
        public IEnumerable<Employees> EmployeeGet(int id)
        {
            string _DbName = "";
            string uri = _url + "Master/EmployeeGetByIdlst?id=" + id;
            IEnumerable<Employees> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Employees>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmployeeGet), _DbName, uri);

            }
            return cmp;
        }
        public ErrorMsg CheckEmployeeEmail(string EmpEmail)
        {
            string Param = "?EmpEmail=" + EmpEmail;
            string uri = _url + "Master/CheckEmployeeEmail" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        return cmp1;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CheckEmployeeEmail), _DbName, uri);

            }
            return JsonConvert.DeserializeObject<ErrorMsg>(res);
        }
        public ErrorMsg CheckBeacon(string BeaconMac, int BranchId)
        {
            string Param = "?BeaconMac=" + BeaconMac + "&BranchId=" + BranchId;
            string uri = _url + "Master/CheckBeacon" + Param;
            var res = "";
            string _DbName = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        return cmp1;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CheckBeacon), _DbName, uri);

            }
            return JsonConvert.DeserializeObject<ErrorMsg>(res);
        }
        public string CheckEmployeeCode(string EmpCode)
        {
            string Param = "?EmpCode=" + EmpCode;
            string uri = _url + "Master/EmployeeCheckEmpCode" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CheckEmployeeCode), _DbName, uri);

            }
            return res;
        }
        public string CheckEmployeePunchId(string EmpPunchID)
        {
            string Param = "?punchId=" + EmpPunchID;
            string uri = _url + "Master/EmployeeCheckPunchId" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CheckEmployeePunchId), _DbName, uri);

            }
            return res;
        }
        public IEnumerable<Employees> GetEnrollEmpByDept(string departmentid, string branchid)
        {
            string Param = "?departmentid=" + departmentid + "&branchid=" + branchid;
            string uri = _url + "Master/GetEnrollEmpByDept" + Param;
            string _DbName = "";
            IEnumerable<Employees> cmp = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);

                    var res = response.Result.Content.ReadAsStringAsync().Result;
                    cmp = Enumerable.Empty<Employees>();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        cmp = JsonConvert.DeserializeObjectAsync<List<Employees>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }


                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetEnrollEmpByDept), _DbName, uri);

            }
            return cmp;
        }
        public async Task<IEnumerable<Employees>> GetEmpByDept(string departmentid, string branchid, string isActiveEmployee, string Empcode) //soumya 22-07-20225 added Empcode
        {

            //string Param = "?departmentid=" + departmentid + "&branchid=" + branchid + "&isActiveEmployee=" + isActiveEmployee;
            //string uri = _url + "Master/GetEmpByDept" + Param;
            //using (HttpClient client = new HttpClient())
            //{
            //    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
            //    {
            //        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
            //    }
            //    Task<HttpResponseMessage> response = client.GetAsync(uri);
            //    var res = response.Result.Content.ReadAsStringAsync().Result;
            //    var cmp = Enumerable.Empty<Employees>();
            //    if (response.Result.IsSuccessStatusCode)
            //    {
            //        cmp = JsonConvert.DeserializeObjectAsync<List<Employees>>(res.ToString()).Result;
            //    }
            //    return cmp;

            //}
            EmployeeGetAllByDept objemp = new EmployeeGetAllByDept();
            objemp.departmentid = departmentid;
            objemp.branchid = branchid;
            objemp.isActiveEmployee = isActiveEmployee;
            objemp.Empcode = Empcode; //soumya 22-07-20225 added Empcode
            string uri = _url + "Master/GetEmpByDept";
            var cmp = Enumerable.Empty<Employees>();
            string _DbName = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync(uri, objemp);
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        // Read the response content and deserialize it
                        string responseData = await result.Content.ReadAsStringAsync();
                        cmp = JsonConvert.DeserializeObject<List<Employees>>(responseData);
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)result.StatusCode;
                        string statusCodemessage = result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                    //else
                    //{
                    //    //// Handle error response
                    //    //string errorMessage = await result.Content.ReadAsStringAsync();
                    //    //// Handle or throw an exception based on the error message
                    //    //throw new Exception(errorMessage);

                    //}
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetEmpByDept), _DbName, uri);

            }
            return cmp;
        }
        #endregion

        #region GetSystemSettingById-done
        public CompanySetting GetSystemSettingById(int id)
        {
            string uri = _url + "Master/SystemSettingMasterGetById?id=" + id;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        res = res.Replace(@"[", "");
                        res = res.Replace(@"]", "");
                        var obj = JsonConvert.DeserializeObjectAsync<CompanySetting>(res.ToString()).Result;
                        return obj;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        res = res.Replace(@"[", "");
                        res = res.Replace(@"]", "");
                        var obj1 = JsonConvert.DeserializeObjectAsync<CompanySetting>(res.ToString()).Result;
                        return obj1;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetSystemSettingById), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<CompanySetting>(res);
        }

        #endregion

        #region Role-done
        public IEnumerable<Roles> RoleGetAll()
        {
            string _DbName = "";
            string uri = _url + "Master/RoleMasterGetAll";
            IEnumerable<Roles> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Roles>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(RoleGetAll), _DbName, uri);
            }
            return cmp;
        }
        public ErrorMsg RoleAdd(Roles role)
        {
            ErrorMsg err = new ErrorMsg();
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(role);
            string _DbName = "";
            string uri = _url + "Master/RoleMasterCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Roles>(uri, role);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        var res = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        return err;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result = postTask.Result;
                        var res1 = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res1.ToString()).Result;
                        return err;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(RoleAdd), _DbName, uri);
            }
            return err;
        }
        public bool RoleDelete(int id)
        {
            string _DbName = "";
            string uri = _url + "Master/RoleMasterDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(RoleDelete), _DbName, uri);
            }
            return false;
        }
        public bool RoleUpdate(int id, Roles role)
        {
            string _DbName = "";
            string uri = _url + "Master/RolemasterUpdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Roles>(uri, role);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(RoleUpdate), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region TransactionYear Master-done

        public IEnumerable<TransactionYear> TransactionYearGetAll()
        {
            string uri = _url + "Master/TransactionYearGetAll";
            string _DbName = "";
            IEnumerable<TransactionYear> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<TransactionYear>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(TransactionYearGetAll), _DbName, uri);


            }
            return cmp;

        }

        public TransactionYear TransactionYearCreate(TransactionYear transyear)
        {
            string _DbName = "";
            string uri = _url + "Master/TransactionYearCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<TransactionYear>(uri, transyear);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return new TransactionYear();
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        return new TransactionYear();
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(TransactionYearCreate), _DbName, uri);


            }
            return new TransactionYear();
        }
        public bool TransactionYearDelete(int id)
        {
            string _DbName = "";
            string uri = _url + "Master/TransactionYearDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(TransactionYearDelete), _DbName, uri);


            }
            return false;
        }
        public bool TransactionYearUpdate(int id, TransactionYear transyear)
        {
            string _DbName = "";
            string uri = _url + "Master/TransactionYearUpdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<TransactionYear>(uri, transyear);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(TransactionYearCreate), _DbName, uri);

            }
            return false;
        }

        #endregion

        #region operationmaster-done

        public IEnumerable<UserMenu> OperationMasterGetAll()
        {
            string uri = _url + "Master/OperationMasterGetAll";
            string _DbName = "";
            IEnumerable<UserMenu> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    //UserDetails obj = new UserDetails();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        UserMenu obj = new UserMenu();
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        //obj.EmployeeMenu = JsonConvert.DeserializeObjectAsync<List<UserMenu>>(res.ToString()).Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<UserMenu>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }

            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(OperationMasterGetAll), _DbName, uri);
            }
            return cmp;
        }

        #endregion

        #region LeaveTypeMaster-done
        public IEnumerable<LeaveTypeMaster> LeaveTypeGetAll()
        {
            string uri = _url + "Master/LeaveTypeMasterGetAll";
            string _DbName = "";
            IEnumerable<LeaveTypeMaster> OnDutyLeave = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        var leaveType = JsonConvert.DeserializeObjectAsync<List<LeaveTypeMaster>>(res.ToString()).Result;
                        return leaveType;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveTypeGetAll), _DbName, uri);
            }
            return OnDutyLeave;
        }

        public IEnumerable<LeaveTypeMaster> LeaveTypeGetAll(string LeaveTypeName, string LeaveTypeId)
        {
            var param = "?LeaveTypeName=" + LeaveTypeName + "&LeaveTypeId=" + LeaveTypeId;
            string uri = _url + "Master/LeaveTypeMasterGetAll" + param;
            string _DbName = "";
            IEnumerable<LeaveTypeMaster> leaveType = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        leaveType = JsonConvert.DeserializeObjectAsync<List<LeaveTypeMaster>>(res.ToString()).Result;
                        return leaveType;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveTypeGetAll), _DbName, uri);
            }
            return leaveType;
        }
        public LeaveTypeMaster LeaveTypeAdd(LeaveTypeMaster Des)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(Des);
            string _DbName = "";
            string uri = _url + "Master/LeaveTypeMasterCreate";
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<LeaveTypeMaster>(uri, Des);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        return new LeaveTypeMaster();
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {


                        return new LeaveTypeMaster();

                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveTypeAdd), _DbName, uri);
            }
            return new LeaveTypeMaster();
        }
        public bool LeaveTypeDelete(int id)
        {
            // bool resp = false;
            string _DbName = "";
            string uri = _url + "Master/LeaveTypeMasterDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveTypeDelete), _DbName, uri);
            }
            return false;
        }
        public bool LeaveTypeUpdate(int id, LeaveTypeMaster Des)
        {
            string uri = _url + "Master/LeaveTypeMasterUpdate/" + id;
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<LeaveTypeMaster>(uri, Des);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveTypeUpdate), _DbName, uri);
            }
            return false;

        }
        #endregion

        #region CompanyGetByid-done

        public Companys CompanyGetByid(int id)
        {
            string uri = _url + "Master/CompanyGetById/" + id;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        res = res.Replace(@"[", "");
                        res = res.Replace(@"]", "");
                        var cmp = JsonConvert.DeserializeObjectAsync<Companys>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        res = res.Replace(@"[", "");
                        res = res.Replace(@"]", "");
                        var cmp1 = JsonConvert.DeserializeObjectAsync<Companys>(res.ToString()).Result;
                        return cmp1;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CompanyGetByid), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<Companys>(res);
        }
        #endregion

        #region BranchMaster-done
        public IEnumerable<Branches> BranchGetAll()
        {
            string _DbName = "";
            string uri = _url + "Master/BranchGetAll";
            IEnumerable<Branches> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);

                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Branches>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(BranchGetAll), _DbName, uri);
            }
            return cmp;

        }
        public ErrorMsg BranchAdd(Branches Des)
        {
            string _DbName = "";
            ErrorMsg err = new ErrorMsg();
            string uri = _url + "Master/BranchCreate";
            try
            {


                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Branches>(uri, Des);
                    postTask.Wait();
                    var response = postTask.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var res = response.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        return err;
                    }
                    else if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        var res1 = response.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res1.ToString()).Result;
                        return err;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.StatusCode;
                        string statusCodemessage = response.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(BranchAdd), _DbName, uri);
            }
            return err;

        }
        public bool BranchDelete(int id)
        {
            // bool resp = false;
            string _DbName = "";
            string uri = _url + "Master/BranchDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();

                    var response = postTask.Result;
                    if (response.IsSuccessStatusCode)
                    {

                        return response.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.StatusCode;
                        string statusCodemessage = response.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(BranchDelete), _DbName, uri);
            }
            return false;

        }
        public bool BranchUpdate(int id, Branches Des)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(Des);
            string _DbName = "";
            string uri = _url + "Master/BranchUpdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Branches>(uri, Des);
                    postTask.Wait();

                    var response = postTask.Result;
                    if (response.IsSuccessStatusCode)
                    {

                        return response.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.StatusCode;
                        string statusCodemessage = response.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(BranchUpdate), _DbName, uri);
            }
            return false;
        }
        public IEnumerable<CountryTimeZone> CountryTimeZoneAll()
        {
            string uri = _url + "Master/CountryTimeZoneAll";
            string _DbName = "";
            IEnumerable<CountryTimeZone> cmp = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);

                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<CountryTimeZone>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CountryTimeZoneAll), _DbName, uri);
            }
            return cmp;
        }
        public IEnumerable<Branches> BranchlistGet(string companyid, int empid)
        {
            string uri = _url + "Master/GetBranchlist?companyid=" + companyid + "&empid=" + empid;
            string _DbName = "";
            IEnumerable<Branches> cmp = null;
            try
            {

                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Branches>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(BranchlistGet), _DbName, uri);
            }
            return cmp;

        }


        //public IEnumerable<CountryTimeZone> CountryTimeZoneAll()
        //{
        //    string uri = _url + "Master/CountryTimeZoneAll";
        //    using (HttpClient client = new HttpClient())
        //    {
        //        if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
        //        {
        //            client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
        //        }
        //        Task<HttpResponseMessage> response = client.GetAsync(uri);
        //        var res = response.Result.Content.ReadAsStringAsync().Result;
        //        var cmp = JsonConvert.DeserializeObjectAsync<List<CountryTimeZone>>(res.ToString()).Result;
        //        return cmp;
        //    }

        //}
        #endregion

        #region CityMaster
        public IEnumerable<CityMaster> CityGetAll()
        {

            string uri = _url + "Master/CityMasterGetAll";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<CityMaster>>(res.ToString()).Result;
                return cmp;
            }

        }
        public IEnumerable<CityMaster> CityMasterGetByStateId(int state_id)
        {
            string param = "?Stateid=" + state_id;
            string uri = _url + "Master/CityMasterGetByStateId" + param + "";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<CityMaster>>(res.ToString()).Result;
                return cmp;
            }

        }
        public CityMaster CityAdd(CityMaster Des)
        {

            using (var client = new HttpClient())
            {

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                string uri = _url + "Master/CityMasterCreate";
                //HTTP POST
                var postTask = client.PostAsJsonAsync<CityMaster>(uri, Des);
                postTask.Wait();

                var result = postTask.Result;

                if (result.IsSuccessStatusCode)
                {

                }
            }
            return new CityMaster();
        }
        public bool CityDelete(int id)
        {
            // bool resp = false;

            using (var client = new HttpClient())
            {

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                string uri = _url + "Master/CityMasterDelete/" + id;
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, id);
                postTask.Wait();

                var result = postTask.Result;

                return result.IsSuccessStatusCode;
            }

        }
        public bool CityUpdate(int id, CityMaster Des)
        {
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                string uri = _url + "Master/CityMasterUpdate/" + id;
                //HTTP POST
                var postTask = client.PostAsJsonAsync<CityMaster>(uri, Des);
                postTask.Wait();

                var result = postTask.Result;

                return result.IsSuccessStatusCode;
            }
        }
        #endregion

        #region StateMaster
        public IEnumerable<StateMaster> StateGetAll()
        {

            string uri = _url + "Master/StateMasterGetAll";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<StateMaster>>(res.ToString()).Result;
                return cmp;
            }

        }
        public IEnumerable<StateMaster> StateMasterGetByStateId(int CountryId)
        {
            string param = "?CountryId=" + CountryId;
            string uri = _url + "Master/StateMasterGetByCountryId" + param + "";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<StateMaster>>(res.ToString()).Result;
                return cmp;
            }

        }
        #endregion

        #region CountryMaster-done
        public IEnumerable<CountryMaster> CountryGetAll()
        {
            string _DbName = "";
            IEnumerable<CountryMaster> cmp = null;
            string uri = _url + "Master/CountryMasterGetAll";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<CountryMaster>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CountryGetAll), _DbName, uri);
            }
            return cmp;

        }
        #endregion

        #region rolerights-done
        public IEnumerable<RoleRightsMapping> RoleRightsGetAll(int roleId)   // changes not made here because there is some issue occured so we remove that code.
        {
            string _DbName = "";
            // IEnumerable<RoleRightsMapping> cmp = null;
            string param = "?roleID=" + roleId;
            //string _url = "http://localhost:2159/api/";
            string uri = _url + "Master/RoleRightsGetAll" + param;

            using (HttpClient client = new HttpClient())
            {

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                RoleRightsMapping obj = new RoleRightsMapping();
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<RoleRightsMapping>>(res.ToString()).Result;
                return cmp;

            }
        }
        public IEnumerable<RoleRightsMapping> GetMenuRoleWise(int roleId)
        {
            string param = "?roleID=" + roleId;

            string uri = _url + "Master/GetMenuRoleWise" + param;
            string _DbName = "";
            IEnumerable<RoleRightsMapping> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    RoleRightsMapping obj = new RoleRightsMapping();
                    var res = response.Result.Content.ReadAsStringAsync().Result;
                    //obj.EmployeeMenu = JsonConvert.DeserializeObjectAsync<List<RoleRightsMapping>>(res.ToString()).Result;
                    if (response.Result.IsSuccessStatusCode)
                    {
                        cmp = JsonConvert.DeserializeObjectAsync<List<RoleRightsMapping>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetMenuRoleWise), _DbName, uri);
            }
            return cmp;

        }
        public IEnumerable<RoleRightsMapping> GetMenuRoleWiseDeveloper(int roleId)
        {
            string param = "?roleID=" + roleId;
            string _DbName = "";
            string uri = _url + "Master/GetMenuRoleWiseDeveloper" + param;
            IEnumerable<RoleRightsMapping> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    RoleRightsMapping obj = new RoleRightsMapping();
                    var res = response.Result.Content.ReadAsStringAsync().Result;
                    //obj.EmployeeMenu = JsonConvert.DeserializeObjectAsync<List<RoleRightsMapping>>(res.ToString()).Result;
                    if (response.Result.IsSuccessStatusCode)
                    {
                        cmp = JsonConvert.DeserializeObjectAsync<List<RoleRightsMapping>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetMenuRoleWiseDeveloper), _DbName, uri);
            }
            return cmp;

        }

        public IEnumerable<reqDailyInOutReport> GetReportDashBoardRoleWise(int roleId)
        {
            string param = "?roleID=" + roleId;
            IEnumerable<reqDailyInOutReport> cmp = null;
            string _DbName = "";
            string uri = _url + "Master/GetReportDashBoardRoleWise" + param;
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        reqDailyInOutReport obj = new reqDailyInOutReport();
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<reqDailyInOutReport>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(reqDailyInOutReport), _DbName, uri);
            }
            return cmp;
        }

        public IEnumerable<CustreportList> GetCustomerReportList(int roleId)
        {
            string param = "?roleID=" + roleId;
            string _DbName = "";
            string uri = _url + "Master/GetCustomerReportList";
            IEnumerable<CustreportList> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        CustreportList obj = new CustreportList();
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<CustreportList>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetCustomerReportList), _DbName, uri);
            }
            return cmp;
        }
        public RoleRightsMapping RoleRightsAdd(RoleRightsMapping obj)
        {
            string _DbName = "";
            string uri = _url + "Master/RoleRightsCreate";

            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                {
                    _DbName = HttpContext.Current.Session["DbName"].ToString();
                }
                //string _url = "http://localhost:2159/api/";

                //HTTP POST
                var postTask = client.PostAsJsonAsync<RoleRightsMapping>(uri, obj);
                postTask.Wait();
                if (postTask.Result.IsSuccessStatusCode)
                {
                    var result = postTask.Result;
                    return new RoleRightsMapping();
                }
                else
                {
                    return new RoleRightsMapping();
                }

            }



        }


        public bool RoleRightsDelete(int id)
        {
            string _DbName = "";
            string uri = _url + "Master/RoleRightsDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    //string _url = "http://localhost:2159/api/";
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(RoleRightsDelete), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region SystemSetting-done
        public IEnumerable<CompanySetting> CompanySettingGetAll()
        {
            string _DbName = "";
            IEnumerable<CompanySetting> cmp = null;
            string uri = _url + "Master/SystemSettingMasterGetAll";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<CompanySetting>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(changeemphierarchy), _DbName, uri);
            }
            return cmp;
        }
        public CompanySetting CompanySettingAdd(CompanySetting cs)
        {
            string _DbName = "";
            string uri = _url + "Master/SystemSettingMasterCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<CompanySetting>(uri, cs);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return new CompanySetting();
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        return new CompanySetting();
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CompanySettingAdd), _DbName, uri);
            }
            return new CompanySetting();
        }
        public bool CompanySettingUpdate(int id, CompanySetting cs)
        {
            string _DbName = "";
            string uri = _url + "Master/SystemSettingMasterUpdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<CompanySetting>(uri, cs);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CompanySettingUpdate), _DbName, uri);
            }
            return false;
        }

        public bool EmployeemasterGeoFenceupdate(int BranchGeoFenceid)
        {
            // bool resp = false;
            string _DbName = "";
            string uri = _url + "Master/EmployeemasterGeoFenceupdate?BranchGeoFenceid=" + BranchGeoFenceid;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, BranchGeoFenceid);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmployeemasterGeoFenceupdate), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region BranchSetting-done
        public IEnumerable<BranchSetting> BranchSettingGetAll()
        {
            string _DbName = "";
            IEnumerable<BranchSetting> cmp = null;

            string uri = _url + "Master/BranchSettingsGetAll";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<BranchSetting>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(changeemphierarchy), _DbName, uri);
            }
            return cmp;
        }

        public BranchSetting BranchSettingAdd(BranchSetting bs)
        {
            string _DbName = "";
            string uri = _url + "Master/BranchSettingsCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<BranchSetting>(uri, bs);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return new BranchSetting();
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        return new BranchSetting();
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(BranchSettingAdd), _DbName, uri);
            }
            return new BranchSetting();
        }

        public bool BranchSettingUpdate(int id, BranchSetting bs)
        {
            string uri = _url + "Master/BranchSettingsUpdate/" + id;
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<BranchSetting>(uri, bs);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(BranchSettingUpdate), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region Device-done
        public IEnumerable<Devices> DeviceGetAll()
        {
            string uri = _url + "Master/DeviceGetAll";
            string _DbName = "";
            IEnumerable<Devices> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Devices>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DeviceGetAll), _DbName, uri);

            }
            return cmp;
        }

        public IEnumerable<Devices> DeviceGetAll(string DeviceCode)
        {
            string uri = _url + "Master/DeviceGetAll?DeviceCode= " + DeviceCode;
            string _DbName = "";

            IEnumerable<Devices> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Devices>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DeviceGetAll), _DbName, uri);

            }
            return cmp;
        }

        public IEnumerable<Devices> DeviceGetAllGrid(string compCode)
        {
            string uri = _url + "Master/DeviceGetAllGrid?compCode='" + compCode + "'";
            string _DbName = "";
            IEnumerable<Devices> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);

                    var res = response.Result.Content.ReadAsStringAsync().Result;
                    cmp = Enumerable.Empty<Devices>();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        cmp = JsonConvert.DeserializeObjectAsync<List<Devices>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DeviceGetAllGrid), _DbName, uri);

            }
            return cmp;
        }

        public IEnumerable<Devices> DeviceGetAllGrid(string compCode, string RoleId, int cmpid, int BranchID)
        {
            string uri = _url + "Master/DeviceGetAllGrid?compCode='" + compCode + "'&RoleId=" + RoleId + "&cmpid=" + cmpid + "&BranchID=" + BranchID;
            IEnumerable<Devices> cmp = null;
            string _DbName = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);

                    var res = response.Result.Content.ReadAsStringAsync().Result;
                    cmp = Enumerable.Empty<Devices>();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        cmp = JsonConvert.DeserializeObjectAsync<List<Devices>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DeviceGetAllGrid), _DbName, uri);

            }
            return cmp;
        }

        public IEnumerable<Devices> GetAllDeviceGridDeveloper(string companyCode)
        {
            string uri = _url + "Master/GetAllDeviceGridDeveloper?companyCode='" + companyCode + "'";
            string _DbName = "";
            IEnumerable<Devices> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    var res = response.Result.Content.ReadAsStringAsync().Result;
                    cmp = Enumerable.Empty<Devices>();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        cmp = JsonConvert.DeserializeObjectAsync<List<Devices>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAllDeviceGridDeveloper), _DbName, uri);

            }
            return cmp;
        }

        public IEnumerable<DeviceTypes> DeviceTypeGetAll()
        {
            string uri = _url + "Master/DeviceTypeGetAll";
            string _DbName = "";
            IEnumerable<DeviceTypes> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<DeviceTypes>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DeviceTypeGetAll), _DbName, uri);

            }
            return cmp;
        }

        public IEnumerable<Devices> DeviceGetByid(int id)
        {
            string uri = _url + "Master/DeviceGetById?id=" + id;
            string _DbName = "";
            IEnumerable<Devices> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Devices>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DeviceGetByid), _DbName, uri);

            }
            return cmp;
        }

        public ErrorMsg DevicesAdd(Devices dv)
        {


            ErrorMsg err = new ErrorMsg();
            string _DbName = "";
            string uri = _url + "Master/DeviceCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    var postTask = client.PostAsJsonAsync<Devices>(uri, dv);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        var res = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        return err;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result = postTask.Result;
                        var res1 = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res1.ToString()).Result;
                        return err;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DevicesAdd), _DbName, uri);

            }
            return err;

        }
        public ErrorMsg DevelopersdevicesAdd(Devices dv)
        {
            ErrorMsg err = new ErrorMsg();
            string _DbName = "";
            string uri = _url + "Master/DeviceCreateDeveloper";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    var postTask = client.PostAsJsonAsync<Devices>(uri, dv);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        var res = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        return err;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result = postTask.Result;
                        var res1 = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res1.ToString()).Result;
                        return err;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DevelopersdevicesAdd), _DbName, uri);

            }
            return err;
        }

        public bool DeviceDelete(int id)
        {
            // bool resp = false;
            string _DbName = "";
            string uri = _url + "Master/DeviceDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DeviceDelete), _DbName, uri);

            }
            return false;
        }

        public bool DeviceDeActiveActive(AcDevice obj)
        {
            // bool resp = false;
            string _DbName = "";
            string uri = _url + "Master/DeviceDeActiveActive";
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, obj);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DeviceDeActiveActive), _DbName, uri);

            }
            return false;
        }

        public ErrorMsg DeviceUpdate(int id, Devices dv)
        {
            ErrorMsg err = new ErrorMsg();
            string _DbName = "";
            string uri = _url + "Master/DeviceUpdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }


                    var postTask = client.PostAsJsonAsync<Devices>(uri, dv);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        var res = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        return err;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result = postTask.Result;
                        var res1 = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res1.ToString()).Result;
                        return err;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DeviceUpdate), _DbName, uri);

            }
            return err;
        }

        public ErrorMsg DevelopersDeviceUpdate(int id, Devices dv)
        {
            ErrorMsg err = new ErrorMsg();
            string _DbName = "";
            string uri = _url + "Master/DeviceUpdateDeveloper/" + id;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }


                    var postTask = client.PostAsJsonAsync<Devices>(uri, dv);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        var res = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        return err;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result = postTask.Result;
                        var res1 = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res1.ToString()).Result;
                        return err;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DevelopersdevicesAdd), _DbName, uri);

            }
            return err;
        }

        public ErrorMsg CheckDeviceInfo(string DeviceSrNo, string DeviceCode)
        {
            string Param = "?deviceSerialNo=" + DeviceSrNo + "&deviceCode=" + DeviceCode;
            string uri = _url + "Master/CheckDeviceInfo" + Param;
            string _DbName = "";
            ErrorMsg err = new ErrorMsg()
; try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        return err;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var res1 = response.Result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res1.ToString()).Result;
                        return err;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CheckDeviceInfo), _DbName, uri);

            }
            return err;
        }

        public ErrorMsg CheckDeviceInfodeveloper(string DeviceSrNo, string DeviceCode)
        {
            string Param = "?deviceSerialNo=" + DeviceSrNo + "&deviceCode=" + DeviceCode;
            string uri = _url + "Master/CheckDeviceInfodeveloper" + Param;
            string _DbName = "";
            ErrorMsg err = new ErrorMsg();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        return err;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var res1 = response.Result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res1.ToString()).Result;
                        return err;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CheckDeviceInfodeveloper), _DbName, uri);

            }
            return err;
        }

        public string DeviceDownloadData(int deviceid)
        {

            string Param = "?deviceid=" + deviceid;
            string _DbName = "";
            string uri = _url + "DeviceManagement/DownloadData" + Param;
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<GetPresentEmp>>(res.ToString()).Result;
                        //return obj;
                        return res;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;

                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DeviceDownloadData), _DbName, uri);

            }
            return res;

        }


        public IEnumerable<Devices> SchoolDeviceGetAllGrid(string compCode)
        {
            string uri = _url + "SchoolMaster/DeviceGetAll?compCode='" + compCode + "'";
            string _DbName = "";
            IEnumerable<Devices> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    Task<HttpResponseMessage> response = client.GetAsync(uri);

                    var res = response.Result.Content.ReadAsStringAsync().Result;
                    cmp = Enumerable.Empty<Devices>();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        cmp = JsonConvert.DeserializeObjectAsync<List<Devices>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SchoolDeviceGetAllGrid), _DbName, uri);

            }
            return cmp;

        }
        public ResMsg SchoolDevicesAdd(Devices dv)
        {

            string _DbName = "";
            ResMsg err = new ResMsg();
            string uri = _url + "SchoolMaster/DeviceCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    var postTask = client.PostAsJsonAsync<Devices>(uri, dv);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        var res = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ResMsg>(res.ToString()).Result;
                        return err;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result = postTask.Result;
                        var res1 = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ResMsg>(res1.ToString()).Result;
                        return err;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SchoolDevicesAdd), _DbName, uri);

            }
            return err;
        }
        public ResMsg SchoolDeviceUpdate(Devices dv)
        {
            ResMsg err = new ResMsg();
            string _DbName = "";
            string uri = _url + "SchoolMaster/DeviceUpdate/";
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    var postTask = client.PostAsJsonAsync<Devices>(uri, dv);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        var res = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ResMsg>(res.ToString()).Result;
                        return err;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result = postTask.Result;
                        var res1 = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ResMsg>(res1.ToString()).Result;
                        return err;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SchoolDeviceUpdate), _DbName, uri);

            }
            return err;
        }
        public ResMsg SchoolCheckDeviceInfo(string DeviceSrNo, string DeviceCode)
        {
            string Param = "?deviceSerialNo=" + DeviceSrNo + "&deviceCode=" + DeviceCode;
            string uri = _url + "SchoolMaster/CheckDeviceInfo" + Param;
            string _DbName = "";
            ResMsg err = new ResMsg();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ResMsg>(res.ToString()).Result;
                        return err;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var res1 = response.Result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ResMsg>(res1.ToString()).Result;
                        return err;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SchoolCheckDeviceInfo), _DbName, uri);

            }
            return err;
        }

        #region Device_Selling
        public bool AddCartInfo(productcartreq obj)
        {
            string _DbName = "";
            string uri = _url + "Devicecart/Saveproducttocart";
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, obj);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(AddCartInfo), _DbName, uri);

            }
            return false;
        }

        public bool AddressInformation(AddressInformation obj)
        {
            string uri = _url + "Devicecart/SaveDeviceAddressinfo";
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, obj);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(AddressInformation), _DbName, uri);

            }
            return false;
        }
        #endregion

        #endregion

        #region Admin Dashboard-done


        public IEnumerable<AdminDashboardCount> GetallAdminCounter(string cmpid, string branchId, string fdate)
        {
            string Param = "?cmpid=" + cmpid + "&branchId=" + branchId + "&fdate=" + fdate;
            string uri = _url + "DashBoard/GetAdminDashBoardCount" + Param;
            string _DbName = "";
            IEnumerable<AdminDashboardCount> obj = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        obj = JsonConvert.DeserializeObjectAsync<List<AdminDashboardCount>>(res.ToString()).Result;
                        return obj;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetallAdminCounter), _DbName, uri);
            }
            return obj;
        }

        public IEnumerable<DevDashboardCount> GetDeveloperDashBoardCount()
        {
            string uri = _url + "DashBoard/GetDeveloperDashBoardCount";
            string _DbName = "";
            IEnumerable<DevDashboardCount> obj = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        obj = JsonConvert.DeserializeObjectAsync<List<DevDashboardCount>>(res.ToString()).Result;
                        return obj;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetDeveloperDashBoardCount), _DbName, uri);
            }
            return obj;
        }

        public string GetallAdminDetails(int type, string cmpid, string branchId, string fdate, string tdate)
        {
            string Param = "?type=" + type + "&cmpid=" + cmpid + "&branchId=" + branchId + "&fdate=" + fdate + "&tdate=" + tdate;
            string uri = _url + "DashBoard/GetAdminDashBoardDetails" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<AdminDashboardCount>>(res.ToString()).Result;
                        // return obj;
                        return res;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<AdminDashboardCount>>(res.ToString()).Result;
                        // return obj;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetallAdminDetails), _DbName, uri);
            }
            return res;
        }

        public string GetPunchInfo(int empid)
        {
            string uri = _url + "DashBoard/GetPunchTimeDetails?empid=" + empid;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<AdminDashboardCount>>(res.ToString()).Result;
                        // return obj;
                        return res;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<AdminDashboardCount>>(res.ToString()).Result;
                        // return obj;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetPunchInfo), _DbName, uri);
            }
            return res;
        }
        public string GetAttendanceSummarycount(string empid, string fromdate, string todate, int statusflg, string finalStatus, string searchName, string searchBy)
        {
            var empids = Convert.ToInt32(empid);
            string _DbName = "";
            string Param = "?empid=" + empids + "&fromdate=" + fromdate + "&todate=" + todate + "&statusflg=" + statusflg + "&finalStatus=" + finalStatus + "&searchName=" + searchName + "&searchBy=" + searchBy;
            string uri = _url + "DashBoard/GetEmpAttendanceStatusSummary" + Param;
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<GetPresentEmp>>(res.ToString()).Result;
                        //return obj;
                        return res;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<GetPresentEmp>>(res.ToString()).Result;
                        //return obj;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAttendanceSummarycount), _DbName, uri);
            }
            return res;
        }

        public string GetAdminAttendanceSummarycount(string fromdate, string todate, int statusflg, string finalStatus, int cmpId, int branchId)
        {

            string Param = "?fromdate=" + fromdate + "&todate=" + todate + "&statusflg=" + statusflg + "&finalStatus=" + finalStatus + "&cmpId=" + cmpId + "&branchId=" + branchId;
            string uri = _url + "DashBoard/GetAdminAttendanceStatusSummary" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<GetPresentEmp>>(res.ToString()).Result;
                        //return obj;
                        return res;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<GetPresentEmp>>(res.ToString()).Result;
                        //return obj;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAdminAttendanceSummarycount), _DbName, uri);
            }
            return res;
        }

        public string GetEmployeeLeavecount(string leavedate, string cmpId, string branchId)
        {

            string Param = "?leavedate=" + leavedate + "&cmpId=" + cmpId + "&branchId=" + branchId;
            string uri = _url + "DashBoard/GetLeaveDetails" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<GetPresentEmp>>(res.ToString()).Result;
                        //return obj;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetEmployeeLeavecount), _DbName, uri);
            }
            return res;
        }

        public string GetEmployeeLeaveDetails(int LeaveDaysType, string leavedate, int cmpId, int branchId)
        {

            string Param = "?LeaveDaysType=" + LeaveDaysType + "&leavedate=" + leavedate + "&cmpId=" + cmpId + "&branchId=" + branchId;
            string uri = _url + "DashBoard/GetEmpLeaveDetails" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<GetPresentEmp>>(res.ToString()).Result;
                        //return obj;
                        return res;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;

                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetEmployeeLeaveDetails), _DbName, uri);
            }
            return res;
        }


        public string GetEmployeeHoursSummary(string cmpId, string branchId, string fdate)
        {

            string Param = "?cmpId=" + cmpId + "&branchId=" + branchId + "&fdate=" + fdate;
            string uri = _url + "DashBoard/GetHoursSummary" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<GetPresentEmp>>(res.ToString()).Result;
                        //return obj;
                        return res;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<GetPresentEmp>>(res.ToString()).Result;
                        //return obj;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetEmployeeHoursSummary), _DbName, uri);
            }
            return res;
        }


        #region forCalendarFill
        public IEnumerable<FillCalendar> GetFullcalendarDetails(string empid, string fromdate, string todate, int statusflg, string finalStatus, string searchName, string searchBy)
        {

            var empids = Convert.ToInt32(empid);
            if (empids == 0)
            {
                empids = Convert.ToInt32(HttpContext.Current.Session["FirstEmpId"]);
            }
            string Param = "?empid=" + empids + "&fromdate=" + fromdate + "&todate=" + todate + "&statusflg=" + statusflg + "&finalStatus=" + finalStatus + "&searchName=" + searchName + "&searchBy=" + searchBy;
            string uri = _url + "DashBoard/GetEmpAttendanceStatusSummary" + Param;
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObjectAsync<List<FillCalendar>>(res.ToString()).Result;
                return obj;
                //return res;
            }
        }
        #endregion

        #region Last check feedbackdetails
        public string GetLastfeedback(string cmpcode)
        {
            string Param = "?CompanyCode=" + cmpcode;
            string uri = _url + "DashBoard/GetLastfeedback" + Param;
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                return res;
            }
        }
        #endregion
        #endregion

        #region SuperAdminDashboard-done
        public IEnumerable<SuperAdminDetails> GetSuperAdminDashboard()
        {
            string uri = _url + "DashBoard/SuperAdminDashBoard";
            string _DbName = "";
            IEnumerable<SuperAdminDetails> cmp = null;
            try
            {

                using (HttpClient client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<SuperAdminDetails>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }

            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetSuperAdminDashboard), _DbName, uri);
            }
            return cmp;

        }
        public IEnumerable<SuperAdminDetails> GetActiveCompanies(int id)
        {
            //for Registered Companies
            string uri = _url + "DashBoard/ActiveCompany?countorDetails=" + id;
            IEnumerable<SuperAdminDetails> obj = null;
            string _DbName = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        obj = JsonConvert.DeserializeObjectAsync<List<SuperAdminDetails>>(res.ToString()).Result;
                        return obj;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetActiveCompanies), _DbName, uri);
            }
            return obj;
        }

        public IEnumerable<SuperAdminDashboardModal> GetallCounter()
        {
            string uri = _url + "DashBoard/GetSuperAdminDashBoardCount";
            string _DbName = "";
            IEnumerable<SuperAdminDashboardModal> obj = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        obj = JsonConvert.DeserializeObjectAsync<List<SuperAdminDashboardModal>>(res.ToString()).Result;
                        return obj;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetallCounter), _DbName, uri);
            }
            return obj;
        }

        public ErrorMsg DropDatabase(string DbName, string CompanyId)
        {
            ErrorMsg em = new ErrorMsg();
            string _DbName = "";
            string Param = "?DbName=" + DbName + "&CompanyId=" + CompanyId;
            string uri = _url + "DashBoard/DropDatabases" + Param;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<string>(uri, Param);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        em.MegSts = JObject.Parse(result.Content.ReadAsStringAsync().Result).First.First.ToString();
                        em.Meg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();
                        return em;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result = postTask.Result;
                        em.MegSts = JObject.Parse(result.Content.ReadAsStringAsync().Result).First.First.ToString();
                        em.Meg = JObject.Parse(result.Content.ReadAsStringAsync().Result).Last.First.ToString();
                        return em;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DropDatabase), _DbName, uri);
            }
            return em;
        }

        public IEnumerable<ActiveDevicesDetails> GetActiveDevicesDetails()
        {
            string uri = _url + "DashBoard/ActiveDevicesDetails";
            string _DbName = "";
            IEnumerable<ActiveDevicesDetails> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<ActiveDevicesDetails>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetActiveDevicesDetails), _DbName, uri);
            }
            return cmp;

        }
        public IEnumerable<AnalyticsDashboardDetails> GetAnalyticsData(string fname)
        {
            string Param = "?fname=" + fname;
            string uri = _url + "DashBoard/SuperAdminAnalyticsData" + Param;
            string _DbName = "";
            IEnumerable<AnalyticsDashboardDetails> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync(uri, Param);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<AnalyticsDashboardDetails>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAnalyticsData), _DbName, uri);
            }
            return cmp;

        }


        public IEnumerable<Analyticsdata> Analyticsdata()
        {
            string uri = _url + "DashBoard/SuperAdminAnalyticsDataFilterOperation";
            string _DbName = "";
            IEnumerable<Analyticsdata> obj = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        obj = JsonConvert.DeserializeObjectAsync<List<Analyticsdata>>(res.ToString()).Result;
                        return obj;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(Analyticsdata), _DbName, uri);
            }
            return obj;

        }

        public IEnumerable<Analyticsdata> GetallSuperAdminCounter(srchdata srch)
        {
            string Param = "?fdate=" + srch.frmdate + "&tdate=" + srch.todate;
            string uri = _url + "DashBoard/SuperAdminAnalyticsDataFilterWise" + Param;
            string _DbName = "";
            IEnumerable<Analyticsdata> obj = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync(uri, Param);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        obj = JsonConvert.DeserializeObjectAsync<List<Analyticsdata>>(res.ToString()).Result;
                        return obj;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetallSuperAdminCounter), _DbName, uri);
            }
            return obj;
        }



        public IEnumerable<AnalyticsDashboardDetails> GetallSuperAdminfilter(srchfilter srchfil)
        {
            string Param = "?fname=" + srchfil.Titles + "&FromDate=" + srchfil.frmdate + "&ToDate=" + srchfil.todate;
            string uri = _url + "DashBoard/SuperAdminAnalyticsDataDetailsFilterWise" + Param;
            string _DbName = "";
            IEnumerable<AnalyticsDashboardDetails> obj = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync(uri, Param);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        obj = JsonConvert.DeserializeObjectAsync<List<AnalyticsDashboardDetails>>(res.ToString()).Result;
                        return obj;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetallSuperAdminfilter), _DbName, uri);
            }
            return obj;
        }

        #endregion

        #region SystemConfiguration-done
        public IEnumerable<ConfigurationSystemMaster> SystemConfigurationGetall()
        {
            string uri = _url + "DashBoard/SystemConfigurationGetAll";
            string _DbName = "";
            IEnumerable<ConfigurationSystemMaster> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<ConfigurationSystemMaster>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SystemConfigurationGetall), _DbName, uri);
            }
            return cmp;
        }
        public ConfigurationSystemMaster SystemConfigurationAdd(ConfigurationSystemMaster Desg)
        {
            string _DbName = "";
            string uri = _url + "DashBoard/SystemConfigurationCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<ConfigurationSystemMaster>(uri, Desg);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return new ConfigurationSystemMaster();
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        return new ConfigurationSystemMaster();
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SystemConfigurationAdd), _DbName, uri);
            }
            return new ConfigurationSystemMaster();
        }
        public bool SystemConfigurationDelete(int id)
        {
            // bool resp = false;
            string _DbName = "";
            string uri = _url + "DashBoard/SystemConfigurationDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SystemConfigurationDelete), _DbName, uri);
            }
            return false;
        }
        public bool SystemConfigurationUpdate(int id, ConfigurationSystemMaster Desg)
        {
            string uri = _url + "DashBoard/SystemConfigurationUpdate/" + id;
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<ConfigurationSystemMaster>(uri, Desg);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SystemConfigurationUpdate), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region Plans-done
        public IEnumerable<Plan> PlanGetAll(Getpara obj)
        {
            //string Param = "?cmpid=" + companyid;
            string _DbName = "";
            IEnumerable<Plan> cmp = null;
            string uri = _url + "Dashboard/PlanGetAll";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync<Getpara>(uri, obj);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Plan>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(PlanGetAll), _DbName, uri);
            }
            return cmp;
        }
        public bool PlanAdd(Plan pl)
        {
            string _DbName = "";
            string uri = _url + "Dashboard/PlanCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Plan>(uri, pl);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(PlanAdd), _DbName, uri);
            }
            return false;
        }
        public bool PlanDelete(int id)
        {
            string _DbName = "";
            string uri = _url + "Dashboard/PlanDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(PlanGetAll), _DbName, uri);
            }
            return false;
        }
        public bool PlanUpdate(int id, Plan pl)
        {
            string _DbName = "";
            string uri = _url + "Dashboard/PlanUpdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Plan>(uri, pl);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(PlanUpdate), _DbName, uri);
            }
            return false;
        }
        public IEnumerable<Plan> PlanGetbyID(int id)
        {
            string uri = _url + "Dashboard/PlanGetById?id=" + id;
            string _DbName = "";
            IEnumerable<Plan> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Plan>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(PlanGetbyID), _DbName, uri);
            }
            return cmp;
        }
        public DataTable Getmysubscription(int cmpid, int para)
        {
            DataTable dt = new DataTable();

            string uri = _url + "Master/tblMyScubsriptionGetAll?companyid=" + cmpid + "&para=" + para;
            string _DbName = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        dt = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var res1 = response.Result.Content.ReadAsStringAsync().Result;
                        dt = JsonConvert.DeserializeObjectAsync<DataTable>(res1.ToString()).Result;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(Getmysubscription), _DbName, uri);
            }
            return dt;
        }

        public DataTable Restrictionasperplan(Getpara obj)
        {
            DataTable dt = new DataTable();
            string _DbName = "";
            string uri = _url + "Master/Restrictionasperplan";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync<Getpara>(uri, obj);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        dt = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var res1 = response.Result.Content.ReadAsStringAsync().Result;
                        dt = JsonConvert.DeserializeObjectAsync<DataTable>(res1.ToString()).Result;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(Restrictionasperplan), _DbName, uri);
            }
            return dt;
        }

        #endregion

        #region Templates-done
        public IEnumerable<Templates> TemplateGetAll()
        {
            string uri = _url + "Utility/TemplatesGetAll";
            string _DbName = "";
            IEnumerable<Templates> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Templates>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(TemplateGetAll), _DbName, uri);
            }
            return cmp;
        }
        public bool TemplateAdd(Templates tmp)
        {
            string _DbName = "";
            string uri = _url + "Utility/TemplatesCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Templates>(uri, tmp);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(TemplateAdd), _DbName, uri);
            }
            return false;
        }
        public bool TemplateDelete(int id)
        {
            string _DbName = "";
            string uri = _url + "Utility/TemplatesDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(TemplateDelete), _DbName, uri);
            }
            return false;
        }
        public bool TemplateUpdate(int id, Templates tmp)
        {
            string _DbName = "";
            string uri = _url + "Utility/TemplatesUpdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<Templates>(uri, tmp);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(TemplateUpdate), _DbName, uri);
            }
            return false;
        }
        public IEnumerable<Templates> TemplateGetByID(int id)
        {
            string uri = _url + "Utility/TemplatesGetById?id=" + id;
            IEnumerable<Templates> cmp = null;
            string _DbName = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Templates>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(TemplateGetByID), _DbName, uri);
            }
            return cmp;
        }
        #endregion

        #region Religion-done
        public IEnumerable<Religion> ReligionGetAll()
        {
            string uri = _url + "Master/ReligionGetAll";
            string _DbName = "";
            IEnumerable<Religion> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Religion>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ReligionGetAll), _DbName, uri);
            }
            return cmp;
        }
        public Religion ReligionAdd(Religion relg)
        {
            string _DbName = "";
            string uri = _url + "Master/ReligionCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Religion>(uri, relg);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (postTask.Result.IsSuccessStatusCode)
                    {

                        return new Religion();
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        return new Religion();


                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ReligionAdd), _DbName, uri);
            }
            return new Religion();
        }
        public bool ReligionDelete(int id)
        {
            // bool resp = false;
            string _DbName = "";
            string uri = _url + "Master/ReligionDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ReligionDelete), _DbName, uri);
            }
            return false;
        }
        public bool ReligionUpdate(int id, Religion relg)
        {
            string _DbName = "";
            string uri = _url + "Master/ReligionUpdate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Religion>(uri, relg);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ReligionUpdate), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region Policies-done
        public IEnumerable<HRPolicy> HRPolicyGetAll()
        {
            string uri = _url + "Master/PolicyGetAll";
            string _DbName = "";
            IEnumerable<HRPolicy> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<HRPolicy>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(HRPolicyGetAll), _DbName, uri);
            }
            return cmp;
        }
        public ErrorMsg HRPolicyAdd(HRPolicy hr)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(hr);
            string _DbName = "";
            ErrorMsg err = new ErrorMsg();
            string uri = _url + "Master/PolicyCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<HRPolicy>(uri, hr);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        var res = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result = postTask.Result;
                        var res1 = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res1.ToString()).Result;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }


                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(HRPolicyAdd), _DbName, uri);
            }
            return err;
        }
        public bool HRPolicyUpdate(int id, HRPolicy hr)
        {
            string _DbName = "";
            string uri = _url + "Master/PolicyUpdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<HRPolicy>(uri, hr);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(HRPolicyUpdate), _DbName, uri);
            }
            return false;
        }
        public HRPolicy HrPolicyGetbyID(int id)
        {
            string uri = _url + "Master/PolicyGetById?id=" + id;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<List<HRPolicy>>(res.ToString()).Result;
                        if (cmp != null && cmp.Count > 0) // Check if the list is not empty
                        {
                            return cmp[0];
                        }
                        return null;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<List<HRPolicy>>(res.ToString()).Result;
                        if (cmp1 != null && cmp1.Count > 0) // Check if the list is not empty
                        {
                            return cmp1[0];
                        }
                        return null;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(HrPolicyGetbyID), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<HRPolicy>(res);
        }


        public IEnumerable<HRPolicy> SampleHrPolicyGetbyID(int id)
        {
            string uri = _url + "Master/PolicyGetById?id=" + id;
            string _DbName = "";
            IEnumerable<HRPolicy> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<HRPolicy>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(HRPolicyGetAll), _DbName, uri);
            }
            return cmp;
        }
        public IEnumerable<HrpolicyAudit> SampleHrPolicyAuditData(int id)
        {
            string uri = _url + "Master/PolicyGetAuditById?id=" + id;
            string _DbName = "";
            IEnumerable<HrpolicyAudit> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<HrpolicyAudit>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SampleHrPolicyAuditData), _DbName, uri);
            }
            return cmp;
        }

        public IEnumerable<resPolicyData> GetAllDataForPolicyAllocation(reqPolicydata rpd)
        {
            string _DbName = "";
            IEnumerable<resPolicyData> cmp = null;
            string uri = _url + "Master/GetFillDetailsOfCompany";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<reqPolicydata>(uri, rpd);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var res = postTask.Result.Content.ReadAsStringAsync().Result;

                        //  var result = postTask.Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<resPolicyData>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAllDataForPolicyAllocation), _DbName, uri);
            }
            return cmp;
        }

        public IEnumerable<resPolicyData> GetAllPolicyAllocation(reqPolicydata rpd)
        {
            string _DbName = "";
            IEnumerable<resPolicyData> cmp = null;
            string uri = _url + "Master/GetPolicyList";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<reqPolicydata>(uri, rpd);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var res = postTask.Result.Content.ReadAsStringAsync().Result;

                        //  var result = postTask.Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<resPolicyData>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(HRPolicyGetAll), _DbName, uri);
            }
            return cmp;
        }

        public bool HRPolicyAllocationUpdate(reqPolicydata em)
        {
            string _DbName = "";
            string uri = _url + "Master/UpdatePolicy";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<reqPolicydata>(uri, em);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(HRPolicyGetAll), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region Acountsettings-done
        public bool EmpInfoUpdate(EmpInfo objEmp)
        {
            objEmp.CountryCode = objEmp.CountryCode;
            string _DbName = "";
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {
                var empDob = objEmp.EmpDOB.Split('-');
                objEmp.EmpDOB = empDob[2] + "-" + empDob[1] + "-" + empDob[0];
            }
            else if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var empDob = objEmp.EmpDOB.Split('-');
                objEmp.EmpDOB = empDob[2] + "-" + empDob[0] + "-" + empDob[1];
            }
            else if (Isdateformat.ToString().Trim() == "yyyy-mm-dd")
            {
                var empDob = objEmp.EmpDOB.Split('-');
                objEmp.EmpDOB = empDob[0] + "-" + empDob[1] + "-" + empDob[2];
            }
            else
            {
                objEmp.EmpDOB = Convert.ToDateTime(objEmp.EmpDOB).ToString("yyyy-MM-dd");
            }
            string uri = _url + "Master/UpdateEmpInfo";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<EmpInfo>(uri, objEmp);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmpInfoUpdate), _DbName, uri);
            }
            return false;
        }

        public bool EmpImageUpdate(EmpImg empi)
        {
            string uri = _url + "Master/UpdateEmpPhotoInfo";
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<EmpImg>(uri, empi);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmpImageUpdate), _DbName, uri);
            }
            return false;
        }

        public bool AdminInfoUpdate(ReqUpdateAdminInfo objAdmin)
        {
            var dynamicDateFormat = HttpContext.Current.Session["IsDateFormat"];
            if (dynamicDateFormat.ToString().Trim() == "dd-mm-yyyy")
            {
                var dateOfBirth = objAdmin.DateOfBirth.Split('-');
                objAdmin.DateOfBirth = dateOfBirth[2] + "-" + dateOfBirth[1] + "-" + dateOfBirth[0];

            }
            string uri = _url + "Master/UpdateAdminUserInfo";
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<ReqUpdateAdminInfo>(uri, objAdmin);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(AdminInfoUpdate), _DbName, uri);
            }
            return false;
        }

        public IEnumerable<ReqUpdateAdminInfo> AdminGet(int id)
        {
            string uri = _url + "Master/GetAdminUserInfoById?userId=" + id;
            string _DbName = "";
            IEnumerable<ReqUpdateAdminInfo> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<ReqUpdateAdminInfo>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(AdminGet), _DbName, uri);
            }
            return cmp;
        }
        #endregion

        #region attendancesheet-done
        public string GetAttendanceSheetData(string empid, string companyid, string branchid, string departmentid, string fromDate, string toDate)
        {
            string Param = "?empid=" + empid + "&companyid=" + companyid + "&branchid=" + branchid + "&departmentid=" + departmentid + "&fromDate=" + fromDate + "&toDate=" + toDate;
            string uri = _url + "AttendanceSheet/GetAttendanceSheet" + Param;
            var res = "";
            string _DbName = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        //var obj = JsonConvert.DeserializeObjectAsync<List<GetPresentEmp>>(res.ToString()).Result;
                        //return obj;
                        return res;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        //var obj = JsonConvert.DeserializeObjectAsync<List<GetPresentEmp>>(res.ToString()).Result;
                        //return obj;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAttendanceSheetData), _DbName, uri);
            }
            return res;
        }
        #endregion

        #region Scheduler Setting-done
        public IEnumerable<Scheduler> SchedulersettingsGetAll()
        {
            string uri = _url + "Master/SchedulersettingsGetAll";
            string _DbName = "";
            IEnumerable<Scheduler> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Scheduler>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SchedulersettingsGetAll), _DbName, uri);
            }
            return cmp;
        }
        public Scheduler GetSchedulerDetails(int id)
        {
            string uri = _url + "Master/GetSchedulerDetails?id=" + id;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<List<Scheduler>>(res.ToString()).Result;
                        return cmp[0];
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<List<Scheduler>>(res.ToString()).Result;
                        return cmp1[0];
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetSchedulerDetails), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<Scheduler>(res);
        }
        public MRespo SchedulersettingsAdd(Scheduler hd)
        {
            string _DbName = "";
            string uri = _url + "Master/SchedulersettingsCreate";
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Scheduler>(uri, hd);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SchedulersettingsAdd), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<MRespo>(res);
        }

        public bool SchedulersettingsDelete(int id)
        {
            // bool resp = false;
            string _DbName = "";
            string uri = _url + "Master/SchedulersettingsDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SchedulersettingsDelete), _DbName, uri);
            }
            return false;
        }
        public bool SchedulersettingsActiveInactive(int id, bool Isactive)
        {
            // bool resp = false;
            string _DbName = "";
            string Param = "?id=" + id + "&IsActive=" + Isactive;
            string uri = _url + "Master/SchedulersettingsActiveInactive" + Param;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SchedulersettingsActiveInactive), _DbName, uri);
            }
            return false;
        }
        public bool SchedulersettingsUpdate(int id, Scheduler hd)
        {
            string _DbName = "";
            string uri = _url + "Master/SchedulersettingsUpdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Scheduler>(uri, hd);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SchedulersettingsUpdate), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region Scheduler Action-done
        public IEnumerable<Scheduler> ScheduleActionGetAll()
        {
            string _DbName = "";
            IEnumerable<Scheduler> cmp = null;
            string uri = _url + "Master/ScheduleActionGetAll";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Scheduler>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ScheduleActionGetAll), _DbName, uri);
            }
            return cmp;
        }
        public Scheduler ScheduleActionAdd(Scheduler hd)
        {
            string _DbName = "";
            string uri = _url + "Master/SScheduleActionCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Scheduler>(uri, hd);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return new Scheduler();
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        return new Scheduler();
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ScheduleActionAdd), _DbName, uri);
            }
            return new Scheduler();
        }
        public bool ScheduleActionDelete(int id)
        {
            // bool resp = false;
            string _DbName = "";
            string uri = _url + "Master/ScheduleActionDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ScheduleActionDelete), _DbName, uri);
            }
            return false;
        }
        public bool ScheduleActionUpdate(int id, Scheduler hd)
        {
            string _DbName = "";
            string uri = _url + "Master/ScheduleActionUpdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Scheduler>(uri, hd);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ScheduleActionUpdate), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region SubDomain-done
        //public IEnumerable<Companys> CheckDomain(string domain)
        //{
        //    //GetDeviceDetails.ProcessLogLogFileWrite("PaymentResponsesaveExceptionlog", "  Check subdomain :" + domain);

        //    string Param = "?domainName='" + domain + "'";
        //    string uri = _url + "Registration/CheckDomain" + Param;
        //    string _DbName = "";
        //    IEnumerable<Companys> obj = null;
        //    try
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {
        //            //if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
        //            //{
        //            //    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
        //            //}
        //            if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
        //            {
        //                _DbName = HttpContext.Current.Session["DbName"].ToString();
        //            }
        //            Task<HttpResponseMessage> response = client.GetAsync(uri);
        //            if (response.Result.IsSuccessStatusCode)
        //            {
        //                var res = response.Result.Content.ReadAsStringAsync().Result;
        //                 obj = JsonConvert.DeserializeObjectAsync<List<Companys>>(res.ToString()).Result;
        //                return obj;
        //            }
        //            else
        //            {
        //                int statusCodeValue = (int)response.Result.StatusCode;
        //                string statusCodemessage = response.Result.ReasonPhrase;
        //                string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
        //                throw new HttpRequestException(errorDetails);
        //            }
        //            //return res;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.HandleException(ex, nameof(CheckDomain), _DbName, uri);
        //    }
        //    return obj;
        //}

        public IEnumerable<Companys> CheckDomain(string domain)
        {
            //GetDeviceDetails.ProcessLogLogFileWrite("PaymentResponsesaveExceptionlog", "  Check subdomain :" + domain);

            string Param = "?domainName='" + domain + "'";
            string uri = _url + "Registration/CheckDomain" + Param;
            using (HttpClient client = new HttpClient())
            {
                //if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                //{
                //    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                //}
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObjectAsync<List<Companys>>(res.ToString()).Result;
                return obj;
                //return res;
            }
        }
        #endregion

        #region EmployeeDashboard-done
        public string Fillleavesummary(string type, string empid)
        {

            string Param = "?typeid=" + type + "&empid=" + empid;
            string uri = _url + "DashBoard/Fillleavesummary" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<GetPresentEmp>>(res.ToString()).Result;
                        //return obj;
                        return res;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<GetPresentEmp>>(res.ToString()).Result;
                        //return obj;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(Fillleavesummary), _DbName, uri);
            }
            return res;
        }
        #endregion       
        public bool SendFeedbackMail(MailDetails MailDetails)
        {
            string _DbName = "";
            string uri = _url + "Transaction/SendFeedbackMail/";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<MailDetails>(uri, MailDetails);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SendFeedbackMail), _DbName, uri);
            }
            return false;
        }

        public bool InquiryMail(MailDetails MailDetails)
        {
            string uri = _url + "Transaction/InquiryMail/";
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<MailDetails>(uri, MailDetails);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(InquiryMail), _DbName, uri);
            }
            return false;
        }

        public bool Subscriptionupdate(Companyplan clsobj)
        {
            string uri = _url + "Master/Subscriptionupdate";
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Companyplan>(uri, clsobj);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return true;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(Subscriptionupdate), _DbName, uri);
            }

            return false;
        }


        public EnrollDataSave SaveEnrollDeviceUsers(EnrollDataSave hd)
        {
            string uri = _url + "DeviceDemo/SaveEnrollDeviceUsers";
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<EnrollDataSave>(uri, hd);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return new EnrollDataSave();
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        return new EnrollDataSave();
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ScheduleActionDelete), _DbName, uri);
            }
            return new EnrollDataSave();
        }

        #region DeviceDemo-done
        public bool UpdateDeviceLicence(DeviceLienceUpdate dl)
        {
            string _DbName = "";
            string uri = _url + "DeviceDemo/UpdateDeviceLicence";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<DeviceLienceUpdate>(uri, dl);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CheckDomain), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region Audit-done
        public IEnumerable<Employee> GetEmployeeAuditData(Audit Aud)
        {
            string uri = _url + "Master/GetAuditData";
            string _DbName = "";
            IEnumerable<Employee> aud = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync(uri, Aud);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        aud = JsonConvert.DeserializeObjectAsync<List<Employee>>(res.ToString()).Result;
                        return aud;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetEmployeeAuditData), _DbName, uri);
            }
            return aud;
        }
        public IEnumerable<Device> GetAuditDeviceData(Audit Aud)
        {
            string uri = _url + "Master/GetAuditData";
            string _DbName = "";
            IEnumerable<Device> aud = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync(uri, Aud);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        aud = JsonConvert.DeserializeObjectAsync<List<Device>>(res.ToString()).Result;
                        return aud;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAuditDeviceData), _DbName, uri);
            }
            return aud;
        }
        public IEnumerable<Shift> GetAuditShiftData(Audit Aud)
        {
            string uri = _url + "Master/GetAuditData";
            string _DbName = "";
            IEnumerable<Shift> aud = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync(uri, Aud);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        aud = JsonConvert.DeserializeObjectAsync<List<Shift>>(res.ToString()).Result;
                        return aud;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAuditShiftData), _DbName, uri);
            }
            return aud;
        }
        public IEnumerable<Policy> GetPolicyAuditData(Audit Aud)
        {
            string _DbName = "";
            IEnumerable<Policy> aud = null;
            string uri = _url + "Master/GetAuditData";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync(uri, Aud);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        aud = JsonConvert.DeserializeObjectAsync<List<Policy>>(res.ToString()).Result;
                        return aud;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetPolicyAuditData), _DbName, uri);
            }
            return aud;

        }
        public IEnumerable<Attendance> GetAttendanceAuditdata(Audit Aud)
        {
            string uri = _url + "Master/GetAuditData";
            string _DbName = "";
            IEnumerable<Attendance> aud = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync(uri, Aud);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        aud = JsonConvert.DeserializeObjectAsync<List<Attendance>>(res.ToString()).Result;
                        return aud;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAttendanceAuditdata), _DbName, uri);
            }
            return aud;

        }
        public IEnumerable<branch> GetBranchAuditdata(Audit Aud)
        {
            string uri = _url + "Master/GetAuditData";
            string _DbName = "";
            IEnumerable<branch> aud = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync(uri, Aud);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        aud = JsonConvert.DeserializeObjectAsync<List<branch>>(res.ToString()).Result;
                        return aud;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetBranchAuditdata), _DbName, uri);
            }
            return aud;
        }
        #endregion

        public List<Column> Columns { get; set; }


        #region For CustomeChanges In FastTrackBranch-done
        public string GetallAdminDetails_FastTrack(int type, string cmpid, string branchId, string fdate, string tdate, string DeptId, string DesgId, string ShiftId)
        {
            string Param = "?type=" + type + "&cmpid=" + cmpid + "&branchId=" + branchId + "&fdate=" + fdate + "&tdate=" + tdate + "&DepartId=" + DeptId + "&DesigId=" + DesgId + "&ShiftId=" + ShiftId;
            string uri = _url + "DashBoardCustom/GetAdminDashBoardDetails_FastTrack" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (HttpContext.Current.Session != null && HttpContext.Current.Session["DbName"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<AdminDashboardCount>>(res.ToString()).Result;
                        // return obj;
                        return res;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        // var obj = JsonConvert.DeserializeObjectAsync<List<AdminDashboardCount>>(res.ToString()).Result;
                        // return obj;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetallAdminDetails_FastTrack), _DbName, uri);
            }
            return res;
        }


        public string GetallAdminDetails_FastTrack_MissPunch(int type, string cmpid, string branchId, string fdate, string tdate, string DeptId, string DesgId, string ShiftId)
        {
            string Param = "?type=" + type + "&cmpid=" + cmpid + "&branchId=" + branchId + "&fdate=" + fdate + "&tdate=" + tdate + "&DepartId=" + DeptId + "&DesigId=" + DesgId + "&ShiftId=" + ShiftId;
            string uri = _url + "DashBoardCustom/GetAdminDashBoardCount_FastTrack_MissPunch" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (HttpContext.Current.Session != null && HttpContext.Current.Session["DbName"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetallAdminDetails_FastTrack), _DbName, uri);
            }
            return res;
        }
        #endregion


        #region Onboarding-done

        public async Task<OnBordingEmployee> PrintOnboardingEmployee(int id)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "Employee/OnBordingDataGetById?OnBordingID=" + id + "&roleid=" + UserRoleid + "&loginempid=" + UserEmpid;
            string _DbName = "";
            try
            {

                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    HttpResponseMessage response = await client.GetAsync(uri);
                    OnBordingEmployee onboardingEmployees = new OnBordingEmployee();

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            string jsonResponse = await response.Content.ReadAsStringAsync();
                            OnBordingEmployee onboardingEmployee = JsonConvert.DeserializeObject<OnBordingEmployee>(jsonResponse);
                            return onboardingEmployee;
                        }
                        catch (Exception ex)
                        {
                            return onboardingEmployees;
                        }

                    }
                    else
                    {
                        int statusCodeValue = (int)response.StatusCode;
                        string statusCodemessage = response.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(PrintOnboardingEmployee), _DbName, uri);
                throw new Exception("Failed to retrieve onboarding employee data. Status code: " + ex);
            }
        }
        #region saveschedylerfastrack
        public MRespo SavecreateTaskFasttrack(SchedulerFasttrack obj)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = obj.FromDate.Split('-');
                obj.FromDate = fromDate[0] + "-" + fromDate[1] + "-" + fromDate[2];
                var toDate = obj.Todate.Split('-');
                obj.Todate = toDate[0] + "-" + toDate[1] + "-" + toDate[2];
            }
            else
            {
                obj.FromDate = Convert.ToDateTime(obj.FromDate).ToString("yyyy-MM-dd");
                obj.Todate = Convert.ToDateTime(obj.Todate).ToString("yyyy-MM-dd");
            }
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                string uri = _url + "Employee/SaveSchedulerTask";
                var postTask = client.PostAsJsonAsync<SchedulerFasttrack>(uri, obj);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                return cmp;
            }
        }
        #endregion
        #endregion

        #region for Payroll data-done
        public async Task<PayrollEmpDetails> PrintOnboardingPayrollEmployee(int paystructureID, string MonthlyCtc)
        {
            string uri = _Payrollurl + "PayStructure/GetPayrollCalculationByPayStructure?paystructureid=" + paystructureID + "&ctcamount=" + MonthlyCtc;
            string _DbName = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    HttpResponseMessage response = await client.GetAsync(uri);
                    PayrollEmpDetails objEmpPayDet = new PayrollEmpDetails(); // Declared outside the try block

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            string jsonResponse = await response.Content.ReadAsStringAsync();
                            objEmpPayDet = JsonConvert.DeserializeObject<PayrollEmpDetails>(jsonResponse); // No redeclaration here
                            return objEmpPayDet;
                        }
                        catch (Exception ex)
                        {
                            // Handle exception if needed
                            // Log the exception, return default value, or rethrow the exception
                            return objEmpPayDet; // Return the default value of objEmpPayDet
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        string jsonResponse1 = await response.Content.ReadAsStringAsync();
                        objEmpPayDet = JsonConvert.DeserializeObject<PayrollEmpDetails>(jsonResponse1);
                        return objEmpPayDet;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.StatusCode;
                        string statusCodemessage = response.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ReligionGetAll), _DbName, uri);
                throw new Exception("Failed to retrieve onboarding employee data. Status code: " + ex);
            }
        }
        #endregion

        #region For OnBordingDataGrid CompressData-done
        public OnBordingEmployee OnBordingDataGridCompressData(OnboardingEmployeeGetAllData objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "Employee/OnBordingDataGetAll";
            string _DbName = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    // Make the HTTP POST request synchronously
                    HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;

                    OnBordingEmployee onboardingEmployees = new OnBordingEmployee();

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            string jsonResponse = response.Content.ReadAsStringAsync().Result;
                            onboardingEmployees = JsonConvert.DeserializeObject<OnBordingEmployee>(jsonResponse);
                            return onboardingEmployees;
                        }
                        catch (Exception ex)
                        {
                            GetDeviceDetails.ProcessLogLogFileWrite("OnBordingDataGridCompressDataResponse Error", ex.Message);
                            return onboardingEmployees;
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        string jsonResponse1 = response.Content.ReadAsStringAsync().Result;
                        onboardingEmployees = JsonConvert.DeserializeObject<OnBordingEmployee>(jsonResponse1);
                        return onboardingEmployees;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.StatusCode;
                        string statusCodemessage = response.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                    throw new Exception("Failed to retrieve onboarding employee data. Status code: " + response.StatusCode);

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ReligionGetAll), _DbName, uri);
                throw new Exception("Failed to retrieve onboarding employee data. Status code: " + ex);
            }
        }
        #endregion


        #region Enroll Active Inctive Employee-done

        public IEnumerable<Employees> EmployeeGetAllEnroll()
        {
            string uri = _url + "Master/EmployeeGetAllEnroll";
            string _DbName = "";
            IEnumerable<Employees> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);

                    var res = response.Result.Content.ReadAsStringAsync().Result;
                    cmp = Enumerable.Empty<Employees>();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        cmp = JsonConvert.DeserializeObjectAsync<List<Employees>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmployeeGetAllEnroll), _DbName, uri);
            }
            return cmp;
        }
        #endregion


        #region Enroll Department Filter Wise-done
        public IEnumerable<Employees> EmployeeGetAllEnrollUserList(EnrollUserParameter obj)
        {
            string uri = _url + "Employee/EmployeeGetAllEnrollUserList";
            string _DbName = "";
            IEnumerable<Employees> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync(uri, obj);

                    var res = response.Result.Content.ReadAsStringAsync().Result;
                    cmp = Enumerable.Empty<Employees>();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        cmp = JsonConvert.DeserializeObjectAsync<List<Employees>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmployeeGetAllEnrollUserList), _DbName, uri);
            }
            return cmp;
        }
        #endregion


        #region Get All Employee Fasttrack-done
        public IEnumerable<Employees> EmployeeGetAllFasttrack()
        {
            string _DbName = "";
            string uri = _url + "Employee/EmployeeGetAllFasttrack";
            IEnumerable<Employees> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);

                    var res = response.Result.Content.ReadAsStringAsync().Result;
                    cmp = Enumerable.Empty<Employees>();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        cmp = JsonConvert.DeserializeObjectAsync<List<Employees>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmployeeGetAllFasttrack), _DbName, uri);
            }
            return cmp; ;
        }
        #endregion


        #region For DirectoryDataGrid CompressData-done
        public DirectoryDetailsViewModel DirectoryDataGridCompressData(DirectoryEmployeeGetAllData objData)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            string uri = _url + "Employee/DirectoryDataGetAll";
            string _DbName = "";
            try
            {

                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    // Make the HTTP POST request synchronously
                    HttpResponseMessage response = client.PostAsJsonAsync(uri, objData).Result;
                    DirectoryDetailsViewModel onboardingEmployees = new DirectoryDetailsViewModel();
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            string jsonResponse = response.Content.ReadAsStringAsync().Result;
                            onboardingEmployees = JsonConvert.DeserializeObject<DirectoryDetailsViewModel>(jsonResponse);
                            return onboardingEmployees;
                        }
                        catch (Exception ex)
                        {
                            GetDeviceDetails.ProcessLogLogFileWrite("DirectoryDataGridCompressData Error", ex.Message);
                            return onboardingEmployees;
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        string jsonResponse1 = response.Content.ReadAsStringAsync().Result;
                        onboardingEmployees = JsonConvert.DeserializeObject<DirectoryDetailsViewModel>(jsonResponse1);
                        return onboardingEmployees;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.StatusCode;
                        string statusCodemessage = response.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }


                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DirectoryDataGridCompressData), _DbName, uri);
                throw new Exception("Failed to retrieve Directory employee data. Status code: " + ex);
            }
        }
        #endregion

        #region Import Developer Employee-done
        public IEnumerable<ImportDevEmployeefaillog> ImportDevEmployees(List<ImportDevEmployee> items)
        {
            string uri = _url + "Utility/ImportDevEmployees";
            string _DbName = "";
            IEnumerable<ImportDevEmployeefaillog> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var timeout = client.Timeout;
                    var addedvalue = timeout.Add(new TimeSpan(0, 30, 0));
                    client.Timeout = addedvalue;
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    var postTask = client.PostAsJsonAsync<List<ImportDevEmployee>>(uri, items);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var res = postTask.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<ImportDevEmployeefaillog>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ImportDevEmployees), _DbName, uri);
            }
            return cmp;
        }
        #endregion

        #region FastTrack Report

        #region GatePass Issue Report
        public static IEnumerable<GatePass> ViewDailyReportQueryForGatePassIsuueGetAll(reqDailyInOutReport objreq)
        {

            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            //GetDeviceDetails.ProcessLogLogFileWrite("ViewDailyReportQueryForGatePassIsuueGetAll", Isdateformat);
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    //GetDeviceDetails.ProcessLogLogFileWrite("ViewDailyReportQueryForGatePassIsuueGetAll", objreq.FromDate);
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                    //GetDeviceDetails.ProcessLogLogFileWrite("ViewDailyReportQueryForGatePassIsuueGetAll", objreq.Todate);
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }

            //string uri = _url + "Report/ViewGatePassIssuereport?FromDate=" + objreq.FromDate + "&Todate=" + objreq.Todate + "&EmpID=" + objreq.EmpID + "&CmpID=" + objreq.CompanyID + "&RptID=" + objreq.RptID + "&IsFilo=" + objreq.IsFilo;
            string uri = _url + "Report/ViewGatePassIssuereport";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                if (response.Result.IsSuccessStatusCode)
                {
                    var GatePassIssueReport = JsonConvert.DeserializeObjectAsync<List<GatePass>>(res.ToString()).Result;
                    //var tbldata = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                    return GatePassIssueReport;
                }
                else
                {
                    var GatePassIssueReport = JsonConvert.DeserializeObjectAsync<List<GatePass>>(res.ToString()).Result;
                    return GatePassIssueReport;
                }
            }
        }
        #endregion

        #region GatePass Policy Assigned Report
        public static IEnumerable<GatePassAppliedPolicy> ViewGatePassPolicyAssignedGetAll(reqDailyInOutReport objreq)
        {

            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            //GetDeviceDetails.ProcessLogLogFileWrite("ViewGatePassPolicyAssignedGetAll", Isdateformat);
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    //GetDeviceDetails.ProcessLogLogFileWrite("ViewGatePassPolicyAssignedGetAll", objreq.FromDate);
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                    //GetDeviceDetails.ProcessLogLogFileWrite("ViewGatePassPolicyAssignedGetAll", objreq.Todate);
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }

            //string uri = _url + "Report/ViewGatePassPolicyAssignedreport?FromDate=" + objreq.FromDate + "&Todate=" + objreq.Todate + "&EmpID=" + objreq.EmpID + "&CmpID=" + objreq.CompanyID + "&RptID=" + objreq.RptID + "&IsFilo=" + objreq.IsFilo;
            string uri = _url + "Report/ViewGatePassPolicyAssignedreport";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                if (response.Result.IsSuccessStatusCode)
                {
                    var GatePassPolicyAssignedReport = JsonConvert.DeserializeObjectAsync<List<GatePassAppliedPolicy>>(res.ToString()).Result;
                    //var tbldata = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                    return GatePassPolicyAssignedReport;
                }
                else
                {
                    var GatePassPolicyAssignedReport = JsonConvert.DeserializeObjectAsync<List<GatePassAppliedPolicy>>(res.ToString()).Result;
                    return GatePassPolicyAssignedReport;
                }
            }
        }
        #endregion

        #region FastTrack DailyInOut Security Report
        public static IEnumerable<FastTrackDailyInOutPunchingReport> ViewDailyReportQueryForFastTrackSecurityGetAll(reqDailyInOutReport objreq)
        {

            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            //GetDeviceDetails.ProcessLogLogFileWrite("ViewDailyReportQueryForFastTrackSecurityGetAll", Isdateformat);
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    //GetDeviceDetails.ProcessLogLogFileWrite("ViewDailyReportQueryForFastTrackSecurityGetAll", objreq.FromDate);
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                    //GetDeviceDetails.ProcessLogLogFileWrite("ViewDailyReportQueryForFastTrackSecurityGetAll", objreq.Todate);
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }

            //string uri = _url + "Report/ViewFastTrackSecurityreport?FromDate=" + objreq.FromDate + "&Todate=" + objreq.Todate + "&EmpID=" + objreq.EmpID + "&CmpID=" + objreq.CompanyID + "&RptID=" + objreq.RptID + "&IsFilo=" + objreq.IsFilo;
            string uri = _url + "Report/ViewFastTrackSecurityreport";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                if (response.Result.IsSuccessStatusCode)
                {
                    var FastTrackDailyInOutSecurityReport = JsonConvert.DeserializeObjectAsync<List<FastTrackDailyInOutPunchingReport>>(res.ToString()).Result;
                    //var tbldata = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                    return FastTrackDailyInOutSecurityReport;
                }
                else
                {
                    var FastTrackDailyInOutSecurityReport = JsonConvert.DeserializeObjectAsync<List<FastTrackDailyInOutPunchingReport>>(res.ToString()).Result;
                    return FastTrackDailyInOutSecurityReport;
                }
            }
        }
        #endregion

        #region FastTrack Attendace Of Month Report
        public static IEnumerable<MonthlyMuster> ViewFastTrackMonthlyAttendanceReportGetAll(reqDailyInOutReport objreq)
        {

            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            //GetDeviceDetails.ProcessLogLogFileWrite("ViewFastTrackMonthlyAttendanceReportGetAll", Isdateformat);
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    //GetDeviceDetails.ProcessLogLogFileWrite("ViewFastTrackMonthlyAttendanceReportGetAll", objreq.FromDate);
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                    //GetDeviceDetails.ProcessLogLogFileWrite("ViewFastTrackMonthlyAttendanceReportGetAll", objreq.Todate);
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }

            //string uri = _url + "Report/ViewFastTrackMonthlyAttendacereport?FromDate=" + objreq.FromDate + "&Todate=" + objreq.Todate + "&EmpID=" + objreq.EmpID + "&CmpID=" + objreq.CompanyID + "&RptID=" + objreq.RptID + "&IsFilo=" + objreq.IsFilo;
            string uri = _url + "Report/ViewFastTrackMonthlyAttendacereport";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                if (response.Result.IsSuccessStatusCode)
                {
                    var FastTrackMonthlyAttendanceReport = JsonConvert.DeserializeObjectAsync<List<MonthlyMuster>>(res.ToString()).Result;
                    //var tbldata = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                    return FastTrackMonthlyAttendanceReport;
                }
                else
                {
                    var FastTrackMonthlyAttendanceReport = JsonConvert.DeserializeObjectAsync<List<MonthlyMuster>>(res.ToString()).Result;
                    return FastTrackMonthlyAttendanceReport;
                }
            }
        }
        #endregion

        #region FastTrack Employee Punch Comparison Report
        public static IEnumerable<FastTrackEmployeePunchComparisonReport> ViewFastTrackEmpPunchComparisonReportGetAll(reqDailyInOutReport objreq)
        {

            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            //GetDeviceDetails.ProcessLogLogFileWrite("ViewFastTrackEmpPunchComparisonReportGetAll", Isdateformat);
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    //GetDeviceDetails.ProcessLogLogFileWrite("ViewFastTrackEmpPunchComparisonReportGetAll", objreq.FromDate);
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                    //GetDeviceDetails.ProcessLogLogFileWrite("ViewFastTrackEmpPunchComparisonReportGetAll", objreq.Todate);
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }

            //string uri = _url + "Report/ViewFastTrackMonthlyAttendacereport?FromDate=" + objreq.FromDate + "&Todate=" + objreq.Todate + "&EmpID=" + objreq.EmpID + "&CmpID=" + objreq.CompanyID + "&RptID=" + objreq.RptID + "&IsFilo=" + objreq.IsFilo;
            string uri = _url + "Report/ViewFastTrackEmpPunchComparisonreport";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                if (response.Result.IsSuccessStatusCode)
                {
                    var FastTrackEmpPunchComparisonReport = JsonConvert.DeserializeObjectAsync<List<FastTrackEmployeePunchComparisonReport>>(res.ToString()).Result;
                    //var tbldata = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                    return FastTrackEmpPunchComparisonReport;
                }
                else
                {
                    var FastTrackEmpPunchComparisonReport = JsonConvert.DeserializeObjectAsync<List<FastTrackEmployeePunchComparisonReport>>(res.ToString()).Result;
                    return FastTrackEmpPunchComparisonReport;
                }
            }
        }
        #endregion

        #region FastTrack Employee Attendance Correction Report

        public static IEnumerable<AttendanceCorrectionReport> ViewFastTrackEmpAttendaceCorrectionGetAll(reqDailyInOutReport objreq)
        {

            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            //GetDeviceDetails.ProcessLogLogFileWrite("ViewDailyReportQueryForGatePassIsuueGetAll", Isdateformat);
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    //GetDeviceDetails.ProcessLogLogFileWrite("ViewDailyReportQueryForGatePassIsuueGetAll", objreq.FromDate);
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                    //GetDeviceDetails.ProcessLogLogFileWrite("ViewDailyReportQueryForGatePassIsuueGetAll", objreq.Todate);
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }

            //string uri = _url + "Report/ViewGatePassIssuereport?FromDate=" + objreq.FromDate + "&Todate=" + objreq.Todate + "&EmpID=" + objreq.EmpID + "&CmpID=" + objreq.CompanyID + "&RptID=" + objreq.RptID + "&IsFilo=" + objreq.IsFilo;
            string uri = _url + "Report/ViewFastTrackEmpAttendaceCorrectionreport";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                if (response.Result.IsSuccessStatusCode)
                {
                    var AttnCorrectionReport = JsonConvert.DeserializeObjectAsync<List<AttendanceCorrectionReport>>(res.ToString()).Result;
                    //var tbldata = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                    return AttnCorrectionReport;
                }
                else
                {
                    var AttnCorrectionReport = JsonConvert.DeserializeObjectAsync<List<AttendanceCorrectionReport>>(res.ToString()).Result;
                    return AttnCorrectionReport;
                }
            }
        }

        #endregion

        #endregion
    }
    public class TransactionDataRestClient
    {
        private readonly string _url = ConfigurationManager.AppSettings["webapipaytime"];
        private readonly string _urlMS = ConfigurationManager.AppSettings["webapipaytimeMS"];
        private readonly string _updAttnUrl = ConfigurationManager.AppSettings["updatesvc"];

        #region ManualPunching-done
        public IEnumerable<ManualPunch> ManualPunchGetAll()
        {
            //string uri = _url + "Master/tmpDmpTerminalGetAll";
            string uri = _urlMS + "Master/tmpDmpTerminalGetAll";
            string _DbName = "";
            IEnumerable<ManualPunch> ManualPunch = null;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        ManualPunch = JsonConvert.DeserializeObjectAsync<List<ManualPunch>>(res.ToString()).Result;
                        return ManualPunch;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ManualPunchGetAll), _DbName, uri);
            }
            return ManualPunch;
        }
        public DatatableCounts ManualPunchGetAllCount(DataTableAjaxPostModel model)
        {
            string _DbName = "";
            string uri = _url + "Master/tmpDmpTerminalGetAllCounts";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //string uri = _urlMS + "Master/tmpDmpTerminalGetAllCounts";
                    var postTask = client.PostAsJsonAsync<DataTableAjaxPostModel>(uri, model);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result.Content.ReadAsStringAsync().Result;

                        DatatableCounts DataCounts = JsonConvert.DeserializeObjectAsync<DatatableCounts>(result.ToString()).Result;
                        return DataCounts;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result1 = postTask.Result.Content.ReadAsStringAsync().Result;

                        DatatableCounts DataCounts = JsonConvert.DeserializeObjectAsync<DatatableCounts>(result1.ToString()).Result;
                        return DataCounts;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }


                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ManualPunchGetAllCount), _DbName, uri);
            }
            return new DatatableCounts();
        }
        public IEnumerable<ManualPunch> ManualPunchGetAll(DataTableAjaxPostModel model)
        {
            //var jsonSerialiser = new JavaScriptSerializer();
            //var json = jsonSerialiser.Serialize(model);
            // string msg = "";
            string _DbName = "";
            IEnumerable<ManualPunch> ManualPunch = null;
            string uri = _url + "Master/tmpDmpTerminalGetAll";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //string uri = _urlMS + "Master/tmpDmpTerminalGetAll";
                    var postTask = client.PostAsJsonAsync<DataTableAjaxPostModel>(uri, model);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result.Content.ReadAsStringAsync().Result;

                        ManualPunch = JsonConvert.DeserializeObjectAsync<List<ManualPunch>>(result.ToString()).Result;
                        return ManualPunch;


                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ManualPunchGetAll), _DbName, uri);
            }
            return Enumerable.Empty<ManualPunch>();
        }


        public string ManualPunchAdd(ReqManualPunch MP)
        {
            //var jsonSerialiser = new JavaScriptSerializer();   
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = MP.fromdate.Split('-');
                MP.fromdate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                var toDate = MP.todate.Split('-');
                MP.todate = toDate[2] + "-" + toDate[0] + "-" + toDate[1];
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {
                var fromDate = MP.fromdate.Split('-');
                MP.fromdate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                var toDate = MP.todate.Split('-');
                MP.todate = toDate[2] + "-" + toDate[1] + "-" + toDate[0];
            }
            else
            {
                MP.fromdate = Convert.ToDateTime(MP.fromdate).ToString("yyyy-MM-dd");
                MP.todate = Convert.ToDateTime(MP.todate).ToString("yyyy-MM-dd");
            }
            var jsonSettings = new JsonSerializerSettings();
            // jsonSettings.DateFormatString = "New Date (yyyy,MM,dd)";
            var json = JsonConvert.SerializeObject(MP);
            string msg = "";

            string uri = _urlMS + "Master/tmpDmpTerminalCreate";
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //string uri = _url + "Master/tmpDmpTerminalCreate";

                    client.Timeout = TimeSpan.FromMinutes(30);
                    var postTask = client.PostAsJsonAsync<ReqManualPunch>(uri, MP);

                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        msg = "OK";
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        msg = result.ReasonPhrase;


                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ManualPunchAdd), _DbName, uri);
            }
            return msg;
        }

        public string cleardata(UpdateAttendance obj)
        {
            string msg = "";
            //string Param = "?EmpCode=" + empids + "&StartDate='" + fromdate + "'&EndDate='" + todate + "'";
            //string uri = _url + "Transaction/FlushData";
            string uri = _urlMS + "Transaction/FlushData";
            string _DbName = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync<UpdateAttendance>(uri, obj);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        ErrorMsg err = new ErrorMsg();
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        msg = err.MegSts;
                        return msg;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(cleardata), _DbName, uri);
            }
            return msg;
        }

        public IEnumerable<ManualPunch> PunchDataGetbyempid(int empid)
        {
            string uri = _url + "Master/tmpDmpTerminaldataGetbyempid?empid=" + empid;
            string _DbName = "";
            IEnumerable<ManualPunch> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<ManualPunch>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(PunchDataGetbyempid), _DbName, uri);
            }
            return cmp;
        }
        #endregion

        #region LeaveOpening-done
        public IEnumerable<LeaveOpening> LeaveOpeningGetAll()
        {
            string uri = _url + "Transaction/LeaveOpeningGetAll";
            string _DbName = "";
            IEnumerable<LeaveOpening> LeaveOpening = null;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        LeaveOpening = JsonConvert.DeserializeObjectAsync<List<LeaveOpening>>(res.ToString()).Result;
                        return LeaveOpening;
                    }

                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveOpeningGetAll), _DbName, uri);


            }
            return LeaveOpening;
        }

        public async Task<IEnumerable<LeaveOpening>> LeaveOpeningGetAll(string BranchID, int cmpid, int Brchid, int RoleId, string Empid)
        {
            LeaveOpeningGetAll objleave = new LeaveOpeningGetAll();
            objleave.BranchID = BranchID;
            objleave.cmpid = cmpid;
            objleave.Brchid = Brchid;
            objleave.RoleId = RoleId;
            objleave.Empid = Empid;
            // string Param = "?BranchID=" + BranchID + "&cmpid=" + cmpid + "&Brchid=" + Brchid + "&RoleId=" + RoleId + "&Empid=" + Empid;
            string uri = _url + "Transaction/LeaveOpeningGetAll";
            IEnumerable<LeaveOpening> LeaveOpening = null;
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync(uri, objleave);

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        // Read the response content and deserialize it
                        string responseData = await result.Content.ReadAsStringAsync();
                        LeaveOpening = JsonConvert.DeserializeObject<List<LeaveOpening>>(responseData);
                        return LeaveOpening;
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        string errorMessage = await result.Content.ReadAsStringAsync();

                        throw new Exception(errorMessage);

                    }
                    else
                    {
                        int statusCodeValue = (int)result.StatusCode;
                        string statusCodemessage = result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveOpeningGetAll), _DbName, uri);


            }
            return LeaveOpening;
        }

        public string LeaveOpeningAdd(LeaveOpening Des, DataTableForEmployeeDirectoryGridData branchParameter)
        {

            string msg = "";
            string _DbName = "";
            string uri = _url + "Transaction/LeaveOpeningCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                        client.Timeout = TimeSpan.FromMinutes(30);
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    // Wrapping both objects into a single request model
                    var requestData = new LeaveOpeningRequest
                    {
                        Des = Des,
                        Emp = branchParameter
                    };
                    //HTTP POST
                    //var postTask = client.PostAsJsonAsync<LeaveOpening>(uri, requestData);
                    var postTask = client.PostAsJsonAsync(uri, requestData);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        msg = "OK";
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        var res = result.Content.ReadAsStringAsync().Result;
                        ErrorMsg err = new ErrorMsg();
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        msg = err.Meg;


                    }
                    else
                    {
                        int statusCodeValue = (int)result.StatusCode;
                        string statusCodemessage = result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                    //else
                    //{
                    //    var res = result.Content.ReadAsStringAsync().Result;
                    //    ErrorMsg err = new ErrorMsg();
                    //    err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                    //    msg = err.Meg;
                    //}
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveOpeningAdd), _DbName, uri);


            }
            return msg;
        }
        public bool LeaveOpeningDelete(Int64 id)
        {
            // bool resp = false;
            string _DbName = "";
            string uri = _url + "Transaction/LeaveOpeningDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveOpeningDelete), _DbName, uri);
            }
            return false;
        }
        public bool LeaveOpeningUpdate(Int64 id, LeaveOpening Des)
        {
            string _DbName = "";
            string uri = _url + "Transaction/LeaveOpeningUpdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<LeaveOpening>(uri, Des);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveOpeningUpdate), _DbName, uri);


            }
            return false;
        }


        #endregion

        #region OnDutyLeave-done
        public IEnumerable<Leave> OnDutyLeaveGetAll()
        {
            string uri = _url + "Transaction/GetLeaveListAll";
            string _DbName = "";
            IEnumerable<Leave> OnDutyLeave = null;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        OnDutyLeave = JsonConvert.DeserializeObjectAsync<List<Leave>>(res.ToString()).Result;
                        return OnDutyLeave;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(OnDutyLeaveGetAll), _DbName, uri);
                return OnDutyLeave;

            }
        }

        public IEnumerable<Leave> OnDutyLeaveGetAll(int EmpId)
        {
            var param = "?EmpId=" + EmpId;
            string uri = _url + "Transaction/GetLeaveListAll" + param;
            string _DbName = "";
            IEnumerable<Leave> OnDutyLeave = null;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }


                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        OnDutyLeave = JsonConvert.DeserializeObjectAsync<List<Leave>>(res.ToString()).Result;
                        return OnDutyLeave;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(OnDutyLeaveGetAll), _DbName, uri);
                return OnDutyLeave;

            }
        }

        public async Task<IEnumerable<Leave>> OnDutyLeaveGetAll(int BranchID, int cmpid, int Brchid, string RoleId, string Empid, int selectAll, string selectAllSearchTerm)
        {
            GetLeaveListAll objleave = new GetLeaveListAll();
            objleave.BranchID = BranchID;
            objleave.cmpid = cmpid;
            objleave.Brchid = Brchid;
            objleave.RoleId = RoleId;
            objleave.Empid = Empid;
            objleave.selectAll = selectAll;
            objleave.selectAllSearchTerm = selectAllSearchTerm;
            //string Param = "?BranchID=" + BranchID + "&cmpid=" + cmpid + "&Brchid=" + Brchid + "&RoleId=" + RoleId + "&Empid=" + Empid;
            string uri = _url + "Transaction/GetLeaveListAll";
            string _DbName = "";
            IEnumerable<Leave> OnDutyLeave = null;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    var postTask = client.PostAsJsonAsync(uri, objleave);
                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        // Read the response content and deserialize it
                        string responseData = await result.Content.ReadAsStringAsync();
                        OnDutyLeave = JsonConvert.DeserializeObject<List<Leave>>(responseData);
                        return OnDutyLeave;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        string responseData1 = await result.Content.ReadAsStringAsync();
                        OnDutyLeave = JsonConvert.DeserializeObject<List<Leave>>(responseData1);
                        return OnDutyLeave;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(OnDutyLeaveGetAll), _DbName, uri);


            }
            return OnDutyLeave;
        }

        public MRespo OnDutyLeaveGetAllCheck(string fromDate, string toDate, int EmpId, bool IsHalfLeave, int LeaveTypeId, int LeavePaid)
        {
            string Param = "?EmpId=" + EmpId + "&fromDate=" + fromDate + "&toDate=" + toDate + "&IsHalfLeave=" + IsHalfLeave + "&LeaveTypeId=" + LeaveTypeId + "&LeavePaid=" + LeavePaid;
            string uri = _url + "Transaction/OnDutyLeaveGetAllCheck" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        //var _res = res.Split(':');
                        //res = _res[1].Replace(@"}]", "");
                        return cmp;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        //var _res = res.Split(':');
                        //res = _res[1].Replace(@"}]", "");
                        return cmp1;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                    //var cmp = JsonConvert.DeserializeObjectAsync<Companys>(res.ToString()).Result;

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(OnDutyLeaveGetAllCheck), _DbName, uri);


            }
            return JsonConvert.DeserializeObject<MRespo>(res);
        }




        public string OnDutyLeaveAdd(Leave Des)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(Des);
            string msg = "";
            string _DbName = "";
            string uri = _url + "Transaction/LeaveCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }


                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Leave>(uri, Des);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        msg = "OK";
                        return msg;
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        var res = result.Content.ReadAsStringAsync().Result;
                        ErrorMsg err = new ErrorMsg();
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        msg = err.Meg;

                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(OnDutyLeaveAdd), _DbName, uri);
            }
            return msg;
        }
        public bool OnDutyLeaveDelete(Int64 id)
        {
            // bool resp = false;
            string _DbName = "";
            string uri = _url + "Transaction/OnDutyLeaveDelete/" + id;
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }


                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, id);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(OnDutyLeaveDelete), _DbName, uri);
            }
            return false;
        }
        public bool OnDutyLeaveUpdate(Int64 id, Leave Des)
        {
            string _DbName = "";
            string uri = _url + "Transaction/LeaveUpdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Leave>(uri, Des);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(OnDutyLeaveUpdate), _DbName, uri);
            }
            return false;
        }
        public ResOndutyLeave GetBalanceByLeaveType(string leave)
        {
            string uri = _url + "Transaction/GetBalanceByLeaveType?leave=" + leave;
            string _DbName = "";
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        var bal = JsonConvert.DeserializeObjectAsync<ResOndutyLeave>(res.ToString()).Result;
                        return bal;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        var bal1 = JsonConvert.DeserializeObjectAsync<ResOndutyLeave>(res.ToString()).Result;
                        return bal1;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetBalanceByLeaveType), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<ResOndutyLeave>(res);
        }


        public IEnumerable<LeaveList> LeaveRequestGetAll()
        {
            string uri = _url + "Transaction/LeaveGetAll";
            string _DbName = "";
            IEnumerable<LeaveList> OnDutyLeave = null;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        OnDutyLeave = JsonConvert.DeserializeObjectAsync<List<LeaveList>>(res.ToString()).Result;
                        return OnDutyLeave;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(OnDutyLeaveAdd), _DbName, uri);
            }
            return OnDutyLeave;
        }
        public IEnumerable<LeaveList> LeaveListbyEmployee(int empid)
        {
            string uri = _url + "Transaction/GetAppliedLeaveList?empid=" + empid;
            string _DbName = "";
            IEnumerable<LeaveList> LeaveList = null;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        LeaveList = JsonConvert.DeserializeObjectAsync<List<LeaveList>>(res.ToString()).Result;
                        return LeaveList;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveListbyEmployee), _DbName, uri);
            }
            return LeaveList;
        }
        public LeaveResponse LeaveRequestAdd(LeaveList lst)
        {

            lst.FromDate = Convert.ToDateTime(lst.FromDate).ToString("yyyy-MM-dd");
            lst.ToDate = Convert.ToDateTime(lst.ToDate).ToString("yyyy-MM-dd");
            string msg = "";
            string _DbName = "";
            string uri = _url + "Transaction/LeaveCreate";
            var response = new LeaveResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<LeaveList>(uri, lst);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        //msg = "OK";
                        //return msg;

                        var res = result.Content.ReadAsStringAsync().Result;
                        dynamic data = JsonConvert.DeserializeObject(res);
                        response.Message = data.MegSts ?? "ok";
                        response.Meg = data.Meg ?? "";
                        response.LeaveId = data.LeaveID ?? 0;
                        return response;
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {
                        //var res = result.Content.ReadAsStringAsync().Result;
                        //ErrorMsg err = new ErrorMsg();
                        //err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        //msg = err.Meg;

                        var res = result.Content.ReadAsStringAsync().Result;
                        ErrorMsg err = JsonConvert.DeserializeObject<ErrorMsg>(res);
                        response.Message = err.Meg;
                        response.Meg = err.Meg ?? "";
                        response.LeaveId = 0;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveRequestAdd), _DbName, uri);
                response.Message = "Error Occurred";
                response.LeaveId = 0;
            }
            return response;
        }
        #endregion

        #region LeaveEncashCarry-done
        public IEnumerable<LeaveEncashCarry> LeaveEncashCarryGetAll()
        {
            string _DbName = "";
            IEnumerable<LeaveEncashCarry> LeaveEC = null;
            string uri = _url + "Transaction/LeaveEncashCarryGetAll";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        LeaveEC = JsonConvert.DeserializeObjectAsync<List<LeaveEncashCarry>>(res.ToString()).Result;
                        return LeaveEC;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveEncashCarryGetAll), _DbName, uri);
            }
            return LeaveEC;
        }
        public string LeaveEncashCarryAdd(LeaveEncashEmp ArrEmp)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(ArrEmp);
            string msg = "";
            string _DbName = "";
            string uri = _url + "Transaction/LeaveEncashCarryCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<LeaveEncashEmp>(uri, ArrEmp);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        msg = "OK";
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        var res = result.Content.ReadAsStringAsync().Result;
                        ErrorMsg err = new ErrorMsg();
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        msg = err.Meg;


                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveEncashCarryAdd), _DbName, uri);
            }
            return msg;
        }
        public IEnumerable<Employees> LeaveEncashCarryGetAllEmp(string ArrEmp)
        {
            string uri = _url + "Transaction/GetAllEmployeeForEncashCarry?ArrEmp=" + ArrEmp;
            string _DbName = "";
            IEnumerable<Employees> Emp = null;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        Emp = JsonConvert.DeserializeObjectAsync<List<Employees>>(res.ToString()).Result;
                        return Emp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveEncashCarryGetAllEmp), _DbName, uri);
            }
            return Emp;
        }
        #endregion

        #region LeaveEncashCarryRollback-done
        public IEnumerable<Employees> LeaveEncashCarryGetAllEmpRollback(string ArrEmp)
        {
            string uri = _url + "Transaction/GetAllEmployeeForRollback?ArrEmp=" + ArrEmp;
            string _DbName = "";
            IEnumerable<Employees> Emp = null;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        Emp = JsonConvert.DeserializeObjectAsync<List<Employees>>(res.ToString()).Result;
                        return Emp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveEncashCarryGetAllEmpRollback), _DbName, uri);
            }
            return Emp;
        }
        public bool Rollback(string ArrEmp)
        {
            //string uri = _url + "Transaction/Rollback?ArrEmp=" + ArrEmp;
            string _DbName = "";
            string uri = _url + "Transaction/Rollback?ArrEmp=" + ArrEmp;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync(uri, ArrEmp);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(Rollback), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region ShiftAllocation-done
        public IEnumerable<ShiftAllocation> ShiftAllocationGetAll()
        {
            string uri = _url + "Transaction/ShiftAllocationGetAll";
            string _DbName = "";
            IEnumerable<ShiftAllocation> ShiftAllocation = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = httpClient.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        ShiftAllocation = JsonConvert.DeserializeObjectAsync<List<ShiftAllocation>>(res.ToString()).Result;
                        return ShiftAllocation;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftAllocationGetAll), _DbName, uri);
            }
            return ShiftAllocation;
        }

        public IEnumerable<ShiftAllocation> ShiftAllocationGetAll(int BranchID, string fromdt, string todt, int cmpid, int Brchid, string roleid, int empid)
        {
            string Param = "?BranchID=" + BranchID + "&fromdt=" + fromdt + "&todt=" + todt + "&cmpid=" + cmpid + "&Brchid=" + Brchid + "&roleid=" + roleid + "&empid=" + empid;
            string uri = _url + "Transaction/ShiftAllocationGetAll" + Param;
            string _DbName = "";
            IEnumerable<ShiftAllocation> ShiftAllocation = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = httpClient.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        ShiftAllocation = JsonConvert.DeserializeObjectAsync<List<ShiftAllocation>>(res.ToString()).Result;
                        return ShiftAllocation;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftAllocationGetAll), _DbName, uri);
            }
            return ShiftAllocation;
        }

        public IEnumerable<ShiftAllocation> ShiftAllocationGetAll(int EmpCode, string todaydate)
        {
            string Param = "?EmpCode=" + EmpCode + "&todaydate=" + todaydate;
            string uri = _url + "Transaction/ShiftAllocationGetAll" + Param;
            string _DbName = "";
            IEnumerable<ShiftAllocation> ShiftAllocation = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = httpClient.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        ShiftAllocation = JsonConvert.DeserializeObjectAsync<List<ShiftAllocation>>(res.ToString()).Result;
                        return ShiftAllocation;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftAllocationGetAll), _DbName, uri);
            }
            return ShiftAllocation;
        }

        public IEnumerable<ShiftAllocation> ShiftAllocationGetAll(int EmpId, DateTime FDate, DateTime TDate)
        {
            string Param = "?EmpId=" + EmpId + "&FDate=" + FDate + "&TDate=" + TDate;
            string uri = _url + "Transaction/ShiftAllocationGetAll" + Param;
            string _DbName = "";
            IEnumerable<ShiftAllocation> ShiftAllocation = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = httpClient.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        ShiftAllocation = JsonConvert.DeserializeObjectAsync<List<ShiftAllocation>>(res.ToString()).Result;
                        return ShiftAllocation;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftAllocationGetAll), _DbName, uri);
            }
            return ShiftAllocation;
        }
        //remaining code---not change for exception beacuse not in proper formate
        public string ShiftAllocationDatewice(string fromDate, string toDate)
        {
            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>("fromDate", fromDate));
            values.Add(new KeyValuePair<string, string>("toDate", toDate));
            var content = new FormUrlEncodedContent(values);
            string msg = "";
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                string Param = "?fromDate=" + fromDate + "&toDate=" + toDate;
                string uri = _url + "Transaction/GetShiftAllocationDateWice" + Param;

                var postTask = client.PostAsJsonAsync(uri, content);
                postTask.Wait();
                JsonSerializer ser = new JsonSerializer();
                string jsonresp = JsonConvert.SerializeObject(postTask);
            }
            return msg;
        }

        public string ShiftAllocationAdd(ShiftAlloc Des)
        {
            string _DbName = "";
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(Des);
            string msg = "";
            string uri = _url + "Transaction/ShiftAllocationCreate";
            try
            {
                using (var client = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<ShiftAlloc>(uri, Des);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        msg = "OK";
                        return msg;
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        var res = result.Content.ReadAsStringAsync().Result;
                        ErrorMsg err = new ErrorMsg();
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        msg = err.Meg;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ShiftAllocationAdd), _DbName, uri);
            }
            return msg;
        }
        #endregion

        #region DataReceive-done
        public string DataReceiveProcessADD(DataReceive ArrEmp)
        {
            string msg = "";
            string uri = _url + "Utility/AttendanceProcess";
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<DataReceive>(uri, ArrEmp);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        msg = "Ok";
                        return msg;
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        var res = result.Content.ReadAsStringAsync().Result;
                        ErrorMsg err = new ErrorMsg();
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        msg = err.Meg;


                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DataReceiveProcessADD), _DbName, uri);
            }
            return msg;
        }
        #endregion

        #region DailyInOutReport
        //public IEnumerable<DailyInOutReport> ViewDailyReportQueryGetAll()
        //{
        //    string uri = _url + "Report/ViewDailyReportQueryGetAll";
        //    using (HttpClient httpClient = new HttpClient())
        //    {
        //        if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
        //        {
        //            httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
        //        }
        //        Task<HttpResponseMessage> response = httpClient.GetAsync(uri);
        //        var res = response.Result.Content.ReadAsStringAsync().Result;
        //        var DailyInOutReport = JsonConvert.DeserializeObjectAsync<List<DailyInOutReport>>(res.ToString()).Result;
        //        return DailyInOutReport;
        //    }
        //}
        #endregion

        #region DeviceTransactionMonitor-done

        public DatatableCounts TransactionmonitorGetAllCounts(DataTableTransactionMonitorPostModel model)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            //int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            int cmpid = 0;
            int branchid = 0;
            string _DbName = "";

            if (UserRoleid == 6805)
            {
                cmpid = Convert.ToInt32(HttpContext.Current.Session["ClientCompanyId"].ToString());
            }
            if (UserRoleid == 6806)
            {
                cmpid = Convert.ToInt32(HttpContext.Current.Session["ClientCompanyId"].ToString());
                branchid = Convert.ToInt32(HttpContext.Current.Session["BranchId"].ToString());
            }
            model.roleid = UserRoleid;
            //model.loginempid = UserEmpid;
            model.cmpid = cmpid;
            model.branchid = branchid;
            string uri = _url + "Transaction/TransactionmonitorGetAllCounts";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<DataTableTransactionMonitorPostModel>(uri, model);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result.Content.ReadAsStringAsync().Result;

                        DatatableCounts DataCounts = JsonConvert.DeserializeObjectAsync<DatatableCounts>(result.ToString()).Result;
                        return DataCounts;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result1 = postTask.Result.Content.ReadAsStringAsync().Result;

                        DatatableCounts DataCounts1 = JsonConvert.DeserializeObjectAsync<DatatableCounts>(result1.ToString()).Result;
                        return DataCounts1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }



                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(TransactionmonitorGetAllCounts), _DbName, uri);
            }
            return new DatatableCounts();
        }
        public DatatableCounts TransactionmonitorGetAllCounts(DataTableTransactionMonitorFilterPostModel model)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            //int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            int cmpid = 0;
            int branchid = 0;
            if (UserRoleid == 6805)
            {
                cmpid = Convert.ToInt32(HttpContext.Current.Session["ClientCompanyId"].ToString());
            }
            if (UserRoleid == 6806)
            {
                cmpid = Convert.ToInt32(HttpContext.Current.Session["ClientCompanyId"].ToString());
                branchid = Convert.ToInt32(HttpContext.Current.Session["BranchId"].ToString());
            }
            model.roleid = UserRoleid;
            //model.loginempid = UserEmpid;
            model.cmpid = cmpid;
            model.branchid = branchid;
            string _DbName = "";
            string uri = _url + "Transaction/TransactionmonitorGetAllCountsFilter";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<DataTableTransactionMonitorFilterPostModel>(uri, model);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result.Content.ReadAsStringAsync().Result;

                        DatatableCounts DataCounts = JsonConvert.DeserializeObjectAsync<DatatableCounts>(result.ToString()).Result;
                        return DataCounts;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result1 = postTask.Result.Content.ReadAsStringAsync().Result;

                        DatatableCounts DataCounts1 = JsonConvert.DeserializeObjectAsync<DatatableCounts>(result1.ToString()).Result;
                        return DataCounts1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }



                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(TransactionmonitorGetAllCounts), _DbName, uri);
            }
            return new DatatableCounts();
        }
        public IEnumerable<Transactionmonitor> TransactionmonitorGetAll(DataTableTransactionMonitorFilterPostModel model)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            //int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            int cmpid = 0;
            int branchid = 0;
            if (UserRoleid == 6805)
            {
                cmpid = Convert.ToInt32(HttpContext.Current.Session["ClientCompanyId"].ToString());
            }
            if (UserRoleid == 6806)
            {
                cmpid = Convert.ToInt32(HttpContext.Current.Session["ClientCompanyId"].ToString());
                branchid = Convert.ToInt32(HttpContext.Current.Session["BranchId"].ToString());
            }
            model.roleid = UserRoleid;
            //model.loginempid = UserEmpid;
            model.cmpid = cmpid;
            model.branchid = branchid;
            string _DbName = "";
            string uri = _url + "Transaction/TransactionmonitorGetAllFilter";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<DataTableTransactionMonitorFilterPostModel>(uri, model);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result.Content.ReadAsStringAsync().Result;

                        var ManualPunch = JsonConvert.DeserializeObjectAsync<List<Transactionmonitor>>(result.ToString()).Result;
                        return ManualPunch;

                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }


                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(TransactionmonitorGetAll), _DbName, uri);
            }
            return Enumerable.Empty<Transactionmonitor>();
        }
        public IEnumerable<Transactionmonitor> TransactionmonitorGetAll(DataTableTransactionMonitorPostModel model)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            //int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            int cmpid = 0;
            int branchid = 0;
            if (UserRoleid == 6805)
            {
                cmpid = Convert.ToInt32(HttpContext.Current.Session["ClientCompanyId"].ToString());
            }
            if (UserRoleid == 6806)
            {
                cmpid = Convert.ToInt32(HttpContext.Current.Session["ClientCompanyId"].ToString());
                branchid = Convert.ToInt32(HttpContext.Current.Session["BranchId"].ToString());
            }
            model.roleid = UserRoleid;
            //model.loginempid = UserEmpid;
            model.cmpid = cmpid;
            model.branchid = branchid;
            string _DbName = "";
            IEnumerable<Transactionmonitor> ManualPunch = null;
            string uri = _url + "Transaction/TransactionmonitorGetAll";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<DataTableTransactionMonitorPostModel>(uri, model);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result.Content.ReadAsStringAsync().Result;

                        ManualPunch = JsonConvert.DeserializeObjectAsync<List<Transactionmonitor>>(result.ToString()).Result;
                        return ManualPunch;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }



                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(TransactionmonitorGetAll), _DbName, uri);
            }
            return Enumerable.Empty<Transactionmonitor>();
        }
        public IEnumerable<Transactionmonitor> TransactionmonitorGetAll()
        {
            IEnumerable<Transactionmonitor> Transactionmonitor = null;
            string uri = _url + "Transaction/TransactionmonitorGetAll";
            string _DbName = "";
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = httpClient.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        Transactionmonitor = JsonConvert.DeserializeObjectAsync<List<Transactionmonitor>>(res.ToString()).Result;
                        return Transactionmonitor;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(TransactionmonitorGetAll), _DbName, uri);
            }
            return Transactionmonitor;
        }
        public IEnumerable<Transactionmonitor> GetEmployeeByDeviceId(int DeviceId)
        {
            string param = "?DeviceId=" + DeviceId;
            string _DbName = "";
            IEnumerable<Transactionmonitor> cmp = null;

            string uri = _url + "Transaction/TransactionmonitorGetEmployeeBydevice" + param;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Transactionmonitor>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetEmployeeByDeviceId), _DbName, uri);
            }
            return cmp;


        }

        public IEnumerable<Transactionmonitor> GetEmployeeByDeviceId(string DeviceId, string fromdate, string ToDate, string branchid, string companyid)  //,string branchid
        {

            string param = "?DeviceId=" + DeviceId + "&fromdate=" + fromdate + "&ToDate=" + ToDate + "&branchid=" + branchid + "&companyid=" + companyid;  //+ "&branchid=" + branchid;
            string uri = _url + "Transaction/TransactionmonitorGetEmployeeBydevice" + param;
            string _DbName = "";
            IEnumerable<Transactionmonitor> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Transactionmonitor>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetEmployeeByDeviceId), _DbName, uri);
            }
            return cmp;
        }



        #endregion


        #region Update Attendance-done
        public string UpdateAttendance(UpdateAttendance obj)
        {
            string msg = "";
            //string Param = "?EmpCode=" + empids + "&StartDate=" + fromdate + "&EndDate=" + todate;
            //string uri = _url + "Transaction/UpdateAttendance";
            //string uri = _urlMS + "Transaction/UpdateAttendance";
            string uri = _updAttnUrl + "Transaction/UpdateAttendance_KD";

            string _DbName = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                        client.Timeout = TimeSpan.FromMinutes(15);
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //Task<HttpResponseMessage> response = client.PostAsync(uri);
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync<UpdateAttendance>(uri, obj);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        ErrorMsg err = new ErrorMsg();
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        msg = err.MegSts;
                        return msg;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var res1 = response.Result.Content.ReadAsStringAsync().Result;
                        ErrorMsg err = new ErrorMsg();
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res1.ToString()).Result;
                        msg = err.MegSts;
                        return msg;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(UpdateAttendance), _DbName, uri);
            }
            return msg;
        }
        public string AttendanceProcess(UpdateAttendance obj)
        {
            string msg = "";
            //string Param = "?EmpCode=" + empids + "&StartDate=" + fromdate + "&EndDate=" + todate;
            //string uri = _url + "Transaction/AttendanceProcess";
            //string uri = _urlMS + "Transaction/AttendanceProcess";
            string uri = _updAttnUrl + "Transaction/AttendanceProcess_KD";
            string _DbName = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                        client.Timeout = TimeSpan.FromMinutes(15);
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //Task<HttpResponseMessage> response = client.PostAsync(uri);
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync<UpdateAttendance>(uri, obj);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        ErrorMsg err = new ErrorMsg();
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        msg = err.MegSts;
                        return msg;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var res1 = response.Result.Content.ReadAsStringAsync().Result;
                        ErrorMsg err = new ErrorMsg();
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res1.ToString()).Result;
                        msg = err.MegSts;
                        return msg;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(AttendanceProcess), _DbName, uri);
            }
            return msg;
        }


        #endregion

        #region WebPunch-done
        public IEnumerable<Webpunch> WebPunchGetByEmpid(int empid)
        {
            string uri = _url + "Master/WebPunchGetByEmpid?Empid=" + empid;
            string _DbName = "";
            IEnumerable<Webpunch> cmp = null;

            try
            {
                //string uri = _urlMS + "Master/WebPunchGetByEmpid?Empid=" + empid;
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Webpunch>>(res.ToString()).Result;
                        return cmp;

                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(WebPunchGetByEmpid), _DbName, uri);
            }
            return cmp;

        }

        public string Getwebpunchmode(int empid)
        {
            //string uri = _url + "Master/Getwebpunchmode?Empid=" + empid;
            string uri = _urlMS + "Master/Getwebpunchmode?Empid=" + empid;
            string _DbName = "";
            var cmp = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<string>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var res1 = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<string>(res1.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(Getwebpunchmode), _DbName, uri);
            }
            return cmp;
        }

        //public string Getwebpunchmode(int empid, int IsAutoApprove)
        //{
        //    //string uri = _url + "Master/Getwebpunchmode?Empid=" + empid;
        //    string Param = "?Empid=" + empid + "&IsAutoApprove=" + IsAutoApprove;
        //    string uri = _urlMS + "Master/Getwebpunchmode" + Param;
        //    using (HttpClient client = new HttpClient())
        //    {
        //        if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
        //        {
        //            client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
        //        }
        //        Task<HttpResponseMessage> response = client.GetAsync(uri);
        //        var res = response.Result.Content.ReadAsStringAsync().Result;
        //        var cmp = JsonConvert.DeserializeObjectAsync<string>(res.ToString()).Result;
        //        return cmp;
        //    }
        //}

        public string WebPunchAdd(Webpunch MP)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(MP);
            string msg = "";
            string _DbName = "";

            string resultMessage = "";
            string uri = _urlMS + "Master/WebPunchAdd";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //string uri = _url + "Master/WebPunchAdd";

                    HttpContext.Current.Server.ScriptTimeout = 1000;

                    //httpClient.Timeout = TimeSpan.FromMinutes(30);
                    var postTask = client.PostAsJsonAsync<Webpunch>(uri, MP);
                    var result = postTask.Result;

                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        if (cmp.MegSts.ToLower() == "ok")
                        {
                            msg = "OK";
                        }
                        else
                        {
                            msg = cmp.Meg;
                        }
                        resultMessage = msg;
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        var res = result.Content.ReadAsStringAsync().Result;

                        var cmp1 = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        resultMessage = cmp1.Meg;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }



                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(WebPunchAdd), _DbName, uri);
            }
            return resultMessage;
        }

        public string GetWebpunchApprovelist(int roleid)
        {
            string Param = "?roleid=" + roleid + "";
            //string uri = _url + "Master/GetWebpunchApprovelist" + Param;
            string uri = _urlMS + "Master/GetWebpunchApprovelist" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        return res.ToString();
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        return res.ToString();
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetWebpunchApprovelist), _DbName, uri);
            }
            return res;
        }
        public IEnumerable<Webpunch> GetWebpunchApprovelistajax(DataTableWebPunchAjaxPostModel model)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            //int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            model.roleid = UserRoleid;
            //model.loginempid = UserEmpid;
            //string uri = _url + "Master/WebpunchApproveGetAll";
            string uri = _urlMS + "Master/WebpunchApproveGetAll";
            string _DbName = "";
            IEnumerable<Webpunch> webpunch = null;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    var postTask = client.PostAsJsonAsync<DataTableWebPunchAjaxPostModel>(uri, model);
                    postTask.Wait();
                    var result = postTask.Result.Content.ReadAsStringAsync().Result;
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        webpunch = JsonConvert.DeserializeObjectAsync<List<Webpunch>>(result.ToString()).Result;
                        return webpunch;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetWebpunchApprovelistajax), _DbName, uri);
            }
            return webpunch;
        }

        public IEnumerable<Webpunch> GetWebpunchApprovelistajaxFilter(DataTableWebPunchAjaxPostModel model, string FilterFromDate, string FilterToDate, int selectstatus, int Isheararchy)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            //int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());
            model.roleid = UserRoleid;
            model.FilterFromDate = FilterFromDate;
            model.FilterToDate = FilterToDate;
            model.selectstatus = selectstatus;
            model.Isheararchy = Isheararchy;
            string _DbName = "";
            IEnumerable<Webpunch> webpunch = null;
            //model.loginempid = UserEmpid;
            string uri = _url + "Master/WebpunchApproveGetAllFilter";
            //string uri = _urlMS + "Master/WebpunchApproveGetAllFilter";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<DataTableWebPunchAjaxPostModel>(uri, model);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result.Content.ReadAsStringAsync().Result;

                        webpunch = JsonConvert.DeserializeObjectAsync<List<Webpunch>>(result.ToString()).Result;
                        return webpunch;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetWebpunchApprovelistajaxFilter), _DbName, uri);
            }
            return webpunch;
        }

        public DatatableCounts GetWebpunchApprovelistCounts(DataTableWebPunchAjaxPostModel model)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            //int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());

            model.roleid = UserRoleid;
            string _DbName = "";
            string uri = _urlMS + "Master/WebpunchApproveGetAllCounts";
            //model.loginempid = UserEmpid;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    //string uri = _url + "Master/WebpunchApproveGetAllCounts";
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<DataTableWebPunchAjaxPostModel>(uri, model);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result.Content.ReadAsStringAsync().Result;

                        DatatableCounts DataCounts = JsonConvert.DeserializeObjectAsync<DatatableCounts>(result.ToString()).Result;
                        return DataCounts;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result1 = postTask.Result.Content.ReadAsStringAsync().Result;

                        DatatableCounts DataCounts1 = JsonConvert.DeserializeObjectAsync<DatatableCounts>(result1.ToString()).Result;
                        return DataCounts1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }



                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(WebPunchGetByEmpid), _DbName, uri);
            }
            return new DatatableCounts();
        }

        public DatatableCounts GetWebpunchApprovelistCountsFilter(DataTableWebPunchAjaxPostModel model, string FilterFromDate, string FilterToDate, int selectstatus, int Isheararchy)
        {
            int UserRoleid = Convert.ToInt32(HttpContext.Current.Session["RoleId"].ToString());
            //int UserEmpid = Convert.ToInt32(HttpContext.Current.Session["EmpId"].ToString());

            model.roleid = UserRoleid;
            model.FilterFromDate = FilterFromDate;
            model.FilterToDate = FilterToDate;
            model.selectstatus = selectstatus;
            model.Isheararchy = Isheararchy;
            //model.loginempid = UserEmpid;
            string _DbName = "";
            string uri = _url + "Master/WebpunchApproveGetAllCountsFilter";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //string uri = _urlMS + "Master/WebpunchApproveGetAllCountsFilter";
                    var postTask = client.PostAsJsonAsync<DataTableWebPunchAjaxPostModel>(uri, model);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result.Content.ReadAsStringAsync().Result;

                        DatatableCounts DataCounts = JsonConvert.DeserializeObjectAsync<DatatableCounts>(result.ToString()).Result;
                        return DataCounts;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result1 = postTask.Result.Content.ReadAsStringAsync().Result;

                        DatatableCounts DataCounts1 = JsonConvert.DeserializeObjectAsync<DatatableCounts>(result1.ToString()).Result;
                        return DataCounts1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetWebpunchApprovelistCountsFilter), _DbName, uri);
            }
            return new DatatableCounts();
        }
        public bool WebPunchApprove(WebpunchApprove obj)
        {
            bool msg = false;
            string _DbName = "";
            string uri = _urlMS + "Master/WebPunchApprove";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //string uri = _url + "Master/WebPunchApprove";

                    HttpContext.Current.Server.ScriptTimeout = 1000;
                    //httpClient.Timeout = TimeSpan.FromMinutes(30);
                    var postTask = client.PostAsJsonAsync(uri, obj);

                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        msg = true;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(WebPunchApprove), _DbName, uri);
            }
            return false; ;

        }

        // not change this one right now for exception because it is not expected formate.
        public string PublicIPAddress()
        {
            string uri = "http://checkip.dyndns.org/";
            string ip = String.Empty;

            using (var client = new HttpClient())
            {
                var result = client.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;

                ip = result.Split(':')[1].Split('<')[0];
            }

            return ip;
        }

        public string GettimeZoneInfo(int empid)
        {
            string uri = _url + "Master/GettimeZoneInfo?Empid=" + empid;
            //string uri = _urlMS + "Master/GettimeZoneInfo?Empid=" + empid;
            string _DbName = "";
            var cmp = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<string>(res.ToString()).Result;
                        return cmp;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GettimeZoneInfo), _DbName, uri);
            }
            return cmp;
        }


        //public string GettimeZoneInfo(int empid)
        //{
        //    string uri = _url + "Master/GettimeZoneInfo?Empid=" + empid;
        //    using (HttpClient client = new HttpClient())
        //    {
        //        if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
        //        {
        //            client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
        //        }
        //        Task<HttpResponseMessage> response = client.GetAsync(uri);
        //        var res = response.Result.Content.ReadAsStringAsync().Result;
        //        var cmp = JsonConvert.DeserializeObjectAsync<string>(res.ToString()).Result;
        //        return cmp;
        //    }
        //}

        #endregion

        #region Developer End Point-done
        public IEnumerable<EndPoints> GetDeveloperEndPoint()
        {
            string uri = _url + "Transaction/GetDeveloperEndPointClt";
            IEnumerable<EndPoints> EndPoints = null;
            string _DbName = "";
            try
            {

                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        EndPoints = JsonConvert.DeserializeObjectAsync<List<EndPoints>>(res.ToString()).Result;
                        return EndPoints;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetDeveloperEndPoint), _DbName, uri);
            }
            return EndPoints;
        }
        public ErrorMsg EndPointsAdd(EndPoints endPoints)
        {
            ErrorMsg err = new ErrorMsg();
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(endPoints);
            string _DbName = "";
            string uri = _url + "Transaction/EndPointCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<EndPoints>(uri, endPoints);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        var res = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res.ToString()).Result;
                        return err;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result = postTask.Result;

                        var res1 = result.Content.ReadAsStringAsync().Result;
                        err = JsonConvert.DeserializeObjectAsync<ErrorMsg>(res1.ToString()).Result;
                        return err;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EndPointsAdd), _DbName, uri);
            }
            return err;
        }
        public bool EndPointsupdate(Int32 id, EndPoints Des)
        {
            string _DbName = "";
            string uri = _url + "Transaction/EndPointupdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<EndPoints>(uri, Des);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EndPointsupdate), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region Developer Transaction Data-done

        public IEnumerable<TransactionData> DeveloperTransactionDataGetAll()
        {
            string uri = _url + "Master/developertmpDmpTerminalGetAll";
            string _DbName = "";
            IEnumerable<TransactionData> ManualPunch = null;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        ManualPunch = JsonConvert.DeserializeObjectAsync<List<TransactionData>>(res.ToString()).Result;
                        return ManualPunch;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DeveloperTransactionDataGetAll), _DbName, uri);
            }
            return ManualPunch;
        }

        public DatatableCounts DeveloperTransactionDataGetAllCount(DataTableAjaxPostModel model)
        {
            string uri = _url + "Master/developertmpDmpTerminalGetAllCounts";
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<DataTableAjaxPostModel>(uri, model);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result.Content.ReadAsStringAsync().Result;

                        DatatableCounts DataCounts = JsonConvert.DeserializeObjectAsync<DatatableCounts>(result.ToString()).Result;
                        return DataCounts;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result1 = postTask.Result.Content.ReadAsStringAsync().Result;

                        DatatableCounts DataCounts1 = JsonConvert.DeserializeObjectAsync<DatatableCounts>(result1.ToString()).Result;
                        return DataCounts1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }





                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DeveloperTransactionDataGetAllCount), _DbName, uri);
            }
            return new DatatableCounts();
        }

        public IEnumerable<TransactionData> DeveloperTransactionDataGetAll(DataTableAjaxPostModel model)
        {
            //var jsonSerialiser = new JavaScriptSerializer();
            //var json = jsonSerialiser.Serialize(model);
            // string msg = "";
            string _DbName = "";
            IEnumerable<TransactionData> ManualPunch = null;
            string uri = _url + "Master/developertmpDmpTerminalGetAll";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<DataTableAjaxPostModel>(uri, model);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result.Content.ReadAsStringAsync().Result;

                        ManualPunch = JsonConvert.DeserializeObjectAsync<List<TransactionData>>(result.ToString()).Result;
                        return ManualPunch;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }


                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(DeveloperTransactionDataGetAll), _DbName, uri);
            }
            return Enumerable.Empty<TransactionData>();
        }
        #endregion

        #region LeaveSanction-done
        public string InsertLeaveSanction(LeaveSanction ls)
        {
            string _DbName = "";
            string uri = _url + "Transaction/LeaveSanction";
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<LeaveSanction>(uri, ls);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        //var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return res;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        //var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(InsertLeaveSanction), _DbName, uri);
            }
            return res;

        }
        //public IEnumerable<LeaveSanctionList> LeaveSanctionListGetAll(LeaveSanctionList ls)
        //{
        //    string uri = _url + "Transaction/LeaveSanctionList";
        //    using (HttpClient client = new HttpClient())
        //    {
        //        if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
        //        {
        //            client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
        //        }
        //        Task<HttpResponseMessage> response = client.PostAsJsonAsync(uri, ls);
        //        var res = response.Result.Content.ReadAsStringAsync().Result;
        //        var LeaveSanctionAutomaticList = JsonConvert.DeserializeObjectAsync<List<LeaveSanctionList>>(res.ToString()).Result;
        //        return LeaveSanctionAutomaticList;
        //    }        
        //}

        public DataTable LeaveSanctionListGetAll(LeaveSanctionList objreq)
        {
            string _DbName = "";
            string uri = _url + "Transaction/LeaveSanctionList";
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    httpClient.Timeout = TimeSpan.FromMinutes(15);
                    Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<LeaveSanctionList>(uri, objreq);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;

                        var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                        return DailyInOutReport;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var res1 = response.Result.Content.ReadAsStringAsync().Result;

                        var DailyInOutReport1 = JsonConvert.DeserializeObjectAsync<DataTable>(res1.ToString()).Result;
                        return DailyInOutReport1;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveSanctionListGetAll), _DbName, uri);
            }
            return new DataTable();
        }

        //public DataTable LeaveSanctionListGetAll(LeaveSanctionList ls)
        //{
        //    DataTable dt = new DataTable();

        //    string uri = _url + "Master/LeaveSanctionList";
        //    using (HttpClient client = new HttpClient())
        //    {
        //        if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
        //        {
        //            client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
        //        }
        //        Task<HttpResponseMessage> response = client.PostAsJsonAsync(uri, ls);
        //        var res = response.Result.Content.ReadAsStringAsync().Result;
        //        dt = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
        //    }
        //    return dt;
        //}
        #endregion

        #region SystemSetting get the updated session-done
        public IEnumerable<CompanySetting> CompanySettingGetAll()
        {
            string _DbName = "";
            string uri = _url + "Master/SystemSettingMasterGetAll";

            IEnumerable<CompanySetting> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<CompanySetting>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CompanySettingGetAll), _DbName, uri);


            }
            return cmp;
        }
        #endregion
    }

    public class ESSRestClient
    {
        private readonly string _url = ConfigurationManager.AppSettings["webapipaytime"];
        private readonly string _urlMS = ConfigurationManager.AppSettings["webapipaytimeMS"];
        #region Leave Approve-done
        public IEnumerable<LeaveList> GetAllLeaveRequest(int roleid, int empId)
        {
            string Param = "?roleid=" + roleid + "&empId=" + empId;
            string uri = _url + "Transaction/GetLeaveList" + Param;
            string _DbName = "";
            IEnumerable<LeaveList> LeaveOpening = null;

            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        LeaveOpening = JsonConvert.DeserializeObjectAsync<List<LeaveList>>(res.ToString()).Result;
                        return LeaveOpening;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAllLeaveRequest), _DbName, uri);

            }
            return LeaveOpening;
        }

        public IEnumerable<LeaveList> GetAllLeaveRequestlistajax(DataTableLeaveAprrovejaxPostModel model, int hierarchyWise = 0)
        {
            string uri = _url + "Transaction/GetLeaveList?hierarchyWise=" + hierarchyWise;
            string _DbName = "";
            IEnumerable<LeaveList> LeaveList = null;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<DataTableLeaveAprrovejaxPostModel>(uri, model);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result.Content.ReadAsStringAsync().Result;

                        LeaveList = JsonConvert.DeserializeObjectAsync<List<LeaveList>>(result.ToString()).Result;
                        return LeaveList;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAllLeaveRequestlistajax), _DbName, uri);

            }
            return LeaveList;
        }

        public DatatableCounts GetAllLeaveRequestlistCounts(DataTableLeaveAprrovejaxPostModel model, int hierarchyWise = 0)
        {
            string _DbName = "";
            string uri = _url + "Transaction/GetLeaveListCounts?hierarchyWise=" + hierarchyWise;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    var postTask = client.PostAsJsonAsync<DataTableLeaveAprrovejaxPostModel>(uri, model);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result.Content.ReadAsStringAsync().Result;

                        DatatableCounts DataCounts = JsonConvert.DeserializeObjectAsync<DatatableCounts>(result.ToString()).Result;
                        return DataCounts;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result1 = postTask.Result.Content.ReadAsStringAsync().Result;

                        DatatableCounts DataCounts1 = JsonConvert.DeserializeObjectAsync<DatatableCounts>(result1.ToString()).Result;
                        return DataCounts1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }



                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAllLeaveRequestlistCounts), _DbName, uri);


            }
            return new DatatableCounts();
        }

        public bool LeaveApprovalUpdate(Int64 id, LeaveList lev)
        {
            string _DbName = "";
            string uri = _url + "Transaction/LeaveUpdate/" + id;
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<LeaveList>(uri, lev);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.BadRequest)
                    {

                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;


                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(LeaveApprovalUpdate), _DbName, uri);

            }
            return false;
        }
        #endregion

        #region AttendanceCorrection-done
        public string GetAllAttendanceCorrection(AttendanceCorrection AttendanceCorrection)
        {
            //string Param = "?roleid=" + roleid + "&empId=" + empId;
            //string uri = _url + "Transaction/GetAttendanceCorrectionList" + Param;
            string _DbName = "";
            string uri = _url + "Transaction/GetAttendanceCorrectionList";
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //Task<HttpResponseMessage> response = client.GetAsync(uri);
                    //var res = response.Result.Content.ReadAsStringAsync().Result;

                    var postTask = client.PostAsJsonAsync<AttendanceCorrection>(uri, AttendanceCorrection);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        //var attndcorr = JsonConvert.DeserializeObjectAsync<List<AttendanceCorrection>>(res.ToString()).Result;
                        //return attndcorr;
                        return res.ToString();
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;

                        return res.ToString();
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAllAttendanceCorrection), _DbName, uri);

            }
            return res.ToString();

        }

        public string GetAppliedAttendanceCorrectionList(int empId)
        {
            string Param = "?empId=" + empId;
            string uri = _url + "Transaction/GetAppliedAttendanceCorrectionList" + Param;
            string _DbName = "";
            var res = "";
            try
            {

                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        //var attndcorr = JsonConvert.DeserializeObjectAsync<List<AttendanceCorrection>>(res.ToString()).Result;
                        //return attndcorr;
                        return res.ToString();
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        //var attndcorr = JsonConvert.DeserializeObjectAsync<List<AttendanceCorrection>>(res.ToString()).Result;
                        //return attndcorr;
                        return res.ToString();
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAppliedAttendanceCorrectionList), _DbName, uri);

            }
            return res.ToString();
        }

        public AttedanceResponse AttendanceCorrectionCreate(AttendanceCorrection objCorrection)
        {
            objCorrection.InPunchTime = Convert.ToDateTime(objCorrection.InPunchTime).ToString("yyyy-MM-dd HH:mm");
            objCorrection.OutPunchTime = Convert.ToDateTime(objCorrection.OutPunchTime).ToString("yyyy-MM-dd HH:mm");
            //}
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(objCorrection);

            string msg = "";
            string uri = _url + "Transaction/AttendanceCorrectionCreate";
            string _DbName = "";
            var res = "";
            var response = new AttedanceResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<AttendanceCorrection>(uri, objCorrection);
                    postTask.Wait();
                    var result = postTask.Result;

                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = result.Content.ReadAsStringAsync().Result;
                        dynamic data = JsonConvert.DeserializeObject(res);
                        response.Message = data.MegSts ?? "ok";
                        response.Meg = data.Meg ?? "";
                        response.OnCorrectionID = data.OnCorrectionID ?? 0;
                        return response;

                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        res = result.Content.ReadAsStringAsync().Result;
                        ErrorMsg err = JsonConvert.DeserializeObject<ErrorMsg>(res);
                        response.Message = err.MegSts;
                        response.Meg = err.Meg ?? "";
                        response.OnCorrectionID = 0;
                        return response;

                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(AttendanceCorrectionCreate), _DbName, uri);
                response.Message = "Error Occurred";
                response.OnCorrectionID = 0;
            }
            return response;

        }

        public string AttendanceCorrectionUpdate(AttendanceCorrection objCorrection)
        {
            string msg = "";
            string _DbName = "";
            string uri = _urlMS + "Transaction/AttendanceCorrectionUpdate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    client.Timeout = TimeSpan.FromMinutes(30);
                    //string uri = _url + "Transaction/AttendanceCorrectionUpdate";

                    var postTask = client.PostAsJsonAsync<AttendanceCorrection>(uri, objCorrection);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        msg = "OK";
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        msg = result.ReasonPhrase;

                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(AttendanceCorrectionUpdate), _DbName, uri);

            }
            return msg;
        }
        #endregion

        #region SendLeaveMail-done
        public bool SendLeaveMail(MailDetails MailDetails)
        {
            string _DbName = "";
            string uri = _url + "Transaction/SendLeaveMail/";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<MailDetails>(uri, MailDetails);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SendLeaveMail), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region EmployeePhotoVerify-done
        public bool IsPhotoVerify(List<reqEmployeePhoto> lst)
        {
            string uri = _url + "Master/IsEmployeeVerifyData";
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync(uri, lst);
                    postTask.Wait();
                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        //var responseContent = result.Content.ReadAsStringAsync().Result;
                        //var response = JsonConvert.DeserializeObject<MRespo>(responseContent);
                        //return response;
                        return true;


                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(IsPhotoVerify), _DbName, uri);
            }
            throw new Exception("Failed to verify employee data.");
        }
        public string IsPhotoVerify(reqEmployeePhoto objEmpphoto)
        {
            string msg = "";
            string _DbName = "";
            string uri = _url + "Master/IsEmployeeVerifyData";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    if (objEmpphoto != null)
                    {
                        string jsonLog = Newtonsoft.Json.JsonConvert.SerializeObject(objEmpphoto);
                        GetDeviceDetails.ProcessLogLogFileWrite("IsPhotoVerify Request", jsonLog);
                    }
                    var postTask = client.PostAsJsonAsync<reqEmployeePhoto>(uri, objEmpphoto);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        msg = "OK";
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        msg = result.ReasonPhrase;

                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(IsPhotoVerify), _DbName, uri);
            }
            return msg;
        }
        #endregion
    }
    public class UtilitiesDataRestClient
    {
        private readonly string _url = ConfigurationManager.AppSettings["webapipaytime"];
        private readonly string _urlMS = ConfigurationManager.AppSettings["webapipaytimeMS"];
        private readonly string _Payrollurl = ConfigurationManager.AppSettings["webapipaytimePayroll"];

        #region ImportNewData-done
        //public IEnumerable<ImportedNewData> ImportDataGetAll()
        //{
        //    string uri = _Payrollurl + "ImportExcel/ImportExcelAllData";
        //    using (HttpClient client = new HttpClient())
        //    {
        //        Task<HttpResponseMessage> response = client.GetAsync(uri);
        //        var res = response.Result.Content.ReadAsStringAsync().Result;
        //        var cmp = JsonConvert.DeserializeObjectAsync<List<ImportedNewData>>(res.ToString()).Result;
        //        return cmp;
        //    }
        //}
        public ApiResponse ImportDataGetAll()
        {
            ApiResponse objresut2 = new ApiResponse();
            List<Importfaillog> objlstfailed = new List<Importfaillog>();
            string uri = _Payrollurl + "ImportExcel/ImportExcelAllData";
            string _DbName = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var timeout = client.Timeout;
                    var addedvalue = timeout.Add(new TimeSpan(0, 30, 0));
                    client.Timeout = addedvalue;
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrEmpty(res))
                        {
                            if (res.Length > 2)
                            {
                                objresut2 = JsonConvert.DeserializeObjectAsync<ApiResponse>(res.ToString()).Result;
                            }
                        }
                        return objresut2;
                    }

                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ImportDataGetAll), _DbName, uri);
            }
            return objresut2;
        }

        #endregion
        #region AttendanceRules-done
        public string AttendanceRuleAdd(AttendanceRule at)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(at);
            string uri = _url + "Utility/AttendanceRuleCreate";
            string msg = "";
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<AttendanceRule>(uri, at);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        msg = "OK";
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        msg = result.ReasonPhrase;


                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(AttendanceRuleAdd), _DbName, uri);
            }
            return msg;
        }
        public bool AttendanceRuleUpdate(Int64 id, AttendanceRule ar)
        {
            string uri = _url + "Utility/AttendanceRuleUpdate/" + id;
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<AttendanceRule>(uri, ar);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(AttendanceRuleUpdate), _DbName, uri);
            }
            return false;
        }
        public AttendanceRule GetAttendanceRuleIDbyEmpID(int EmpId)
        {
            string uri = _url + "Utility/AttendanceRuleGetByEmpId?EmpId=" + EmpId;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert.DeserializeObjectAsync<AttendanceRule>(res.ToString()).Result;
                        return data;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = response.Result.Content.ReadAsStringAsync().Result;
                        var data1 = JsonConvert.DeserializeObjectAsync<AttendanceRule>(res.ToString()).Result;
                        return data1;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAttendanceRuleIDbyEmpID), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<AttendanceRule>(res);
        }
        #endregion

        #region AttendanceParameter-done
        public string AttendanceParameterCreate(AttendanceParameter ap)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(ap);
            string msg = "";
            string uri = _url + "Utility/AttendanceParameterCreate";
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<AttendanceParameter>(uri, ap);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        msg = "OK";
                    }
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                    {

                        msg = result.ReasonPhrase;


                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(AttendanceParameterCreate), _DbName, uri);
            }
            return msg;
        }

        public bool AttendanceParameterUpdate(Int64 id, AttendanceParameter ap)
        {
            string uri = _url + "Utility/AttendanceParameterUpdate/" + id;
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<AttendanceParameter>(uri, ap);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {

                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(AttendanceParameterUpdate), _DbName, uri);
            }
            return false;
        }

        public AttendanceParameter GetAttendanceParaIDbyBranchID(int Branchid)
        {
            string uri = _url + "Utility/AttendanceParameterGetByBranchId?Branchid=" + Branchid;
            AttendanceParameter ap = new AttendanceParameter();
            string _DbName = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert.DeserializeObjectAsync<AttendanceParameter>(res.ToString()).Result;
                        return data;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var res1 = response.Result.Content.ReadAsStringAsync().Result;
                        var data1 = JsonConvert.DeserializeObjectAsync<AttendanceParameter>(res1.ToString()).Result;
                        return data1;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAttendanceParaIDbyBranchID), _DbName, uri);
            }
            return ap;
        }
        #endregion

        #region ImportEmpMaster- Not do right now beacause currently word do on this module by priyanshi so avoid conflicts.
        //public string ImportEmployees(List<ImportedDataEmp> items, int branchid, int companyid)
        //{
        //    //string uri = _url + "Utility/ImportEmployeeData";
        //    string Param = "?branchId=" + branchid + "&companyId=" + companyid;
        //    string uri = _url + "Utility/ImportEmployees" + Param;
        //    using (HttpClient client = new HttpClient())
        //    {
        //        if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
        //        {
        //            client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
        //        }
        //        var postTask = client.PostAsJsonAsync<List<ImportedDataEmp>>(uri, items);
        //        postTask.Wait();
        //        var res = postTask.Result.Content.ReadAsStringAsync().Result;
        //        //var cmp = JsonConvert.DeserializeObjectAsync(res.ToString()).Result;

        //        return res;
        //    }
        //}

        public IEnumerable<Importexcelfaillog> Importerroremp(string DtFlag, int classMasterValue)
        {
            //string uri = _url + "Utility/ImportEmployeeData";
            string Param = "?DtFlag=" + DtFlag + "&ClassMasterValue=" + classMasterValue;
            string uri = _url + "Utility/failexcelEmployees" + Param;
            using (HttpClient client = new HttpClient())
            {
                var timeout = client.Timeout;
                var addedvalue = timeout.Add(new TimeSpan(0, 30, 0));
                client.Timeout = addedvalue;
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                //var postTask = client.PostAsJsonAsync<List<ImportedDataEmp>>(uri);
                var postTask = client.PostAsJsonAsync(uri, new List<ImportedDataEmp>());
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                //var cmp = JsonConvert.DeserializeObjectAsync(res.ToString()).Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<Importexcelfaillog>>(res.ToString()).Result;

                return cmp;
            }
        }


        public IEnumerable<ImportCompfaillog> Importcomperror(string DtFlag, int classMasterValue)
        {
            string Param = "?DtFlag=" + DtFlag + "&ClassMasterValue=" + classMasterValue;
            string uri = _url + "Utility/failexcelEmployees" + Param;
            using (HttpClient client = new HttpClient())
            {
                var timeout = client.Timeout;
                var addedvalue = timeout.Add(new TimeSpan(0, 30, 0));
                client.Timeout = addedvalue;
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync(uri, new List<ImportCompfaillog>());
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportCompfaillog>>(res.ToString()).Result;
                return cmp;
            }
        }

        public IEnumerable<ImportBranchfaillog> ImportBranchError(string DtFlag, int classMasterValue)
        {
            string Param = "?DtFlag=" + DtFlag + "&ClassMasterValue=" + classMasterValue;
            string uri = _url + "Utility/failexcelEmployees" + Param;
            using (HttpClient client = new HttpClient())
            {
                var timeout = client.Timeout;
                var addedvalue = timeout.Add(new TimeSpan(0, 30, 0));
                client.Timeout = addedvalue;
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync(uri, new List<ImportBranchfaillog>());
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportBranchfaillog>>(res.ToString()).Result;
                return cmp;
            }
        }


        public IEnumerable<ImportDeptfaillog> ImportDepartmentError(string DtFlag, int classMasterValue)
        {
            string Param = "?DtFlag=" + DtFlag + "&ClassMasterValue=" + classMasterValue;
            string uri = _url + "Utility/failexcelEmployees" + Param;
            using (HttpClient client = new HttpClient())
            {
                var timeout = client.Timeout;
                var addedvalue = timeout.Add(new TimeSpan(0, 30, 0));
                client.Timeout = addedvalue;
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync(uri, new List<ImportDeptfaillog>());
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportDeptfaillog>>(res.ToString()).Result;
                return cmp;
            }
        }

        public IEnumerable<ImportDesgfaillog> ImportDesignationError(string DtFlag, int classMasterValue)
        {
            string Param = "?DtFlag=" + DtFlag + "&ClassMasterValue=" + classMasterValue;
            string uri = _url + "Utility/failexcelEmployees" + Param;
            using (HttpClient client = new HttpClient())
            {
                var timeout = client.Timeout;
                var addedvalue = timeout.Add(new TimeSpan(0, 30, 0));
                client.Timeout = addedvalue;
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync(uri, new List<ImportDesgfaillog>());
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportDesgfaillog>>(res.ToString()).Result;
                return cmp;
            }
        }

        public IEnumerable<Importfaillog> ImportEmployees(List<ImportedDataEmp> items, int branchid, int companyid, int planId, string compcode)
        {
            //string uri = _url + "Utility/ImportEmployeeData";
            string Param = "?branchId=" + branchid + "&companyId=" + companyid + "&planId=" + planId + "&compcode=" + compcode;
            string uri = _url + "Utility/ImportEmployees" + Param;
            using (HttpClient client = new HttpClient())
            {
                var timeout = client.Timeout;
                var addedvalue = timeout.Add(new TimeSpan(0, 30, 0));
                client.Timeout = addedvalue;
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync<List<ImportedDataEmp>>(uri, items);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                //var cmp = JsonConvert.DeserializeObjectAsync(res.ToString()).Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<Importfaillog>>(res.ToString()).Result;

                return cmp;
            }
        }

        public IEnumerable<ImportCompfaillog> ImportCompanys(List<Companys> items, string compcode)
        {
            //string uri = _url + "Utility/ImportEmployeeData";            
            string param = "?compcode=" + compcode;
            string uri = _url + "Utility/ImportCompanys" + param;
            using (HttpClient client = new HttpClient())
            {
                var timeout = client.Timeout;
                var addedvalue = timeout.Add(new TimeSpan(0, 30, 0));
                client.Timeout = addedvalue;
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync<List<Companys>>(uri, items);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                //var cmp = JsonConvert.DeserializeObjectAsync(res.ToString()).Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportCompfaillog>>(res.ToString()).Result;

                return cmp;
            }
        }

        public IEnumerable<ImportBranchfaillog> ImportBranches(List<Branches> items, string compcode)
        {
            //string uri = _url + "Utility/ImportEmployeeData";            
            string param = "?compcode=" + compcode;
            string uri = _url + "Utility/ImportBranches" + param;
            using (HttpClient client = new HttpClient())
            {
                var timeout = client.Timeout;
                var addedvalue = timeout.Add(new TimeSpan(0, 30, 0));
                client.Timeout = addedvalue;
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync<List<Branches>>(uri, items);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                //var cmp = JsonConvert.DeserializeObjectAsync(res.ToString()).Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportBranchfaillog>>(res.ToString()).Result;

                return cmp;
            }
        }

        public IEnumerable<ImportDeptfaillog> ImportDepartments(List<Departments> items, string compcode)
        {
            //string uri = _url + "Utility/ImportEmployeeData";            
            string param = "?compcode=" + compcode;
            string uri = _url + "Utility/ImportDepartments" + param;
            using (HttpClient client = new HttpClient())
            {
                var timeout = client.Timeout;
                var addedvalue = timeout.Add(new TimeSpan(0, 30, 0));
                client.Timeout = addedvalue;
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync<List<Departments>>(uri, items);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                //var cmp = JsonConvert.DeserializeObjectAsync(res.ToString()).Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportDeptfaillog>>(res.ToString()).Result;

                return cmp;
            }
        }

        public IEnumerable<ImportDesgfaillog> ImportDesignations(List<Designations> items, string compcode)
        {
            //string uri = _url + "Utility/ImportEmployeeData";            
            string param = "?compcode=" + compcode;
            string uri = _url + "Utility/ImportDesignations" + param;
            using (HttpClient client = new HttpClient())
            {
                var timeout = client.Timeout;
                var addedvalue = timeout.Add(new TimeSpan(0, 30, 0));
                client.Timeout = addedvalue;
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync<List<Designations>>(uri, items);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                //var cmp = JsonConvert.DeserializeObjectAsync(res.ToString()).Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportDesgfaillog>>(res.ToString()).Result;

                return cmp;
            }
        }

        public IEnumerable<ImportInsurancefaillog> ImportInsuranceDetail(List<InsuranceDetails> items)
        {
            string uri = _url + "Utility/ImportInsuranceDetail";

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(30);

                if (HttpContext.Current.Session["tokan"] != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }

                var postTask = client.PostAsJsonAsync(uri, items);
                postTask.Wait();

                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<ImportInsurancefaillog>>(res);
            }
        }

        public IEnumerable<ImportInsurancefaillog> ImportInsuranceerror(string DtFlag)
        {
            string Param = "?DtFlag=" + DtFlag;
            string uri = _url + "Utility/failinsuranceDeatils" + Param;
            using (HttpClient client = new HttpClient())
            {
                var timeout = client.Timeout;
                var addedvalue = timeout.Add(new TimeSpan(0, 30, 0));
                client.Timeout = addedvalue;
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync(uri, new List<ImportCompfaillog>());
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportInsurancefaillog>>(res.ToString()).Result;
                return cmp;
            }
        }

        public IEnumerable<ImportShiftAllocationfaillog> ImportShiftAllocationDetail(List<ShiftAllocationUploadModel> items, string Compcode, int roleid, int Companyid, int branchid, int empid)
        {
            string param = "?roleid=" + roleid + "&companyid=" + Companyid + "&branchid=" + branchid + "&compcode=" + Compcode + "&empid=" + empid;
            string uri = _url + "Transaction/ImportShiftAllocationDetail" + param;
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(30);

                if (HttpContext.Current.Session["tokan"] != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }

                var postTask = client.PostAsJsonAsync(uri, items);
                postTask.Wait();

                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<ImportShiftAllocationfaillog>>(res);
            }
        }

        public IEnumerable<ImportShiftAllocationfaillog> ImportShiftAllocationerror(string DtFlag)
        {
            string Param = "?DtFlag=" + DtFlag;
            string uri = _url + "Transaction/failShiftAllocationDeatils" + Param;
            using (HttpClient client = new HttpClient())
            {
                var timeout = client.Timeout;
                var addedvalue = timeout.Add(new TimeSpan(0, 30, 0));
                client.Timeout = addedvalue;
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync(uri, new List<ImportShiftAllocationfaillog>());
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportShiftAllocationfaillog>>(res.ToString()).Result;
                return cmp;
            }
        }
        public IEnumerable<ImportLeaveSanctionfaillog> ImportLeaveSanctionDetail(List<LevaeSanctionUploadModel> items, string Compcode, int roleid, int Companyid, int branchid)
        {
            string param = "?roleid=" + roleid + "&companyid=" + Companyid + "&branchid=" + branchid + "&compcode=" + Compcode;
            string uri = _url + "Transaction/ImportLeaveSanctionDetail" + param;
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(30);

                if (HttpContext.Current.Session["tokan"] != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }

                var postTask = client.PostAsJsonAsync(uri, items);
                postTask.Wait();

                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<ImportLeaveSanctionfaillog>>(res);
            }
        }

        public IEnumerable<ImportLeaveSanctionfaillog> ImportLeaveSanctionerror(string DtFlag)
        {
            string Param = "?DtFlag=" + DtFlag;
            string uri = _url + "Transaction/failLeaveSanctionDeatils" + Param;
            using (HttpClient client = new HttpClient())
            {
                var timeout = client.Timeout;
                var addedvalue = timeout.Add(new TimeSpan(0, 30, 0));
                client.Timeout = addedvalue;
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync(uri, new List<ImportLeaveSanctionfaillog>());
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportLeaveSanctionfaillog>>(res.ToString()).Result;
                return cmp;
            }
        }
        public IEnumerable<ImportDeHiringfaillog> ImportDeHiringDetail(List<DeHiring> items, string compcode)
        {
            string param = "?compcode=" + compcode;
            string uri = _url + "Transaction/ImportDeHiringDetail" + param;

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(30);

                if (HttpContext.Current.Session["tokan"] != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }

                var postTask = client.PostAsJsonAsync(uri, items);
                postTask.Wait();

                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<ImportDeHiringfaillog>>(res);
            }
        }

        #endregion

        #region Restore Database-done
        public MRespo RestoreDatabase(string filePath, string CompanyName)
        {
            string Param = "?FilePath=" + filePath + "&CompanyName=" + CompanyName;
            string uri = _url + "Utility/RestoreDatabase" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var timeout = client.Timeout;
                    var addedvalue = timeout.Add(new TimeSpan(0, 5, 0));
                    client.Timeout = addedvalue;
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<string>(uri, filePath);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(RestoreDatabase), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<MRespo>(res);
        }

        public MRespo ImportMasterData(string CompanyName)
        {
            string Param = "?CompanyName=" + CompanyName;
            string uri = _url + "Utility/ImportMasterData" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var timeout = client.Timeout;
                    var addedvalue = timeout.Add(new TimeSpan(0, 5, 0));
                    client.Timeout = addedvalue;
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<string>(uri, CompanyName);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ImportMasterData), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<MRespo>(res);
        }

        public MRespo ImportTransData(string CompanyName)
        {
            string Param = "?CompanyName=" + CompanyName;
            string uri = _url + "Utility/ImportTransData" + Param;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var timeout = client.Timeout;
                    var addedvalue = timeout.Add(new TimeSpan(0, 5, 0));
                    client.Timeout = addedvalue;
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<string>(uri, CompanyName);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(ImportTransData), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<MRespo>(res);
        }
        public MRespo MigrateData(string CompanyName)
        {
            string Param = "?CompanyName=" + CompanyName;
            string uri = _url + "Utility/MigrateData" + Param;
            HttpContext.Current.Server.ScriptTimeout = 1000;
            string _DbName = "";
            var res = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var timeout = client.Timeout;
                    var addedvalue = timeout.Add(new TimeSpan(0, 5, 0));
                    client.Timeout = addedvalue;
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<string>(uri, CompanyName);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(MigrateData), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<MRespo>(res);
        }
        #endregion

        #region Valid Employee-done
        public IEnumerable<ValidEmployee> CheckUser(int validuser)
        {
            string param = "?validuser=" + validuser;
            string uri = _url + "Utility/CheckUser" + param;
            string _DbName = "";
            IEnumerable<ValidEmployee> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<ValidEmployee>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(CheckUser), _DbName, uri);
            }
            return cmp;
        }
        #endregion

        #region Attendance Sheet-done
        public IEnumerable<AttendanceSheet> GetAttnSheet(string fromdt, string todt, int cid, int bid)
        {
            string param = "?fromdt=" + fromdt + "&todt=" + todt + "&cid=" + cid + "&bid=" + bid;
            string uri = _url + "Utility/GetAttnSheet" + param;
            string _DbName = "";
            IEnumerable<AttendanceSheet> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<AttendanceSheet>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAttnSheet), _DbName, uri);
            }
            return cmp;

        }
        #endregion
        #region Device Service-done
        public IEnumerable<DeviceService> GetDeviceServiceDetails(string AccountCode)
        {
            string param = "?Accountcode=" + AccountCode;
            string uri = _url + "Utility/GetDeviceServiceDetails" + param;
            string _DbName = "";
            IEnumerable<DeviceService> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<DeviceService>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }


                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetDeviceServiceDetails), _DbName, uri);
            }
            return cmp;
        }
        public MRespo Whitelistipaddress(DeviceService d)
        {
            string uri = _url + "Utility/Whitelistipaddress";
            //ModelState.Remove("DepartmentHead");
            string _DbName = "";
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<DeviceService>(uri, d);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(Whitelistipaddress), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<MRespo>(res);

        }
        public IEnumerable<Devicecommand> GetDevicecommand()
        {
            string uri = _urlMS + "Utility/GetDevicecommand";
            string _DbName = "";
            IEnumerable<Devicecommand> cmp = null;
            try
            {
                //string uri = _url + "Utility/GetDevicecommand";

                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Devicecommand>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetDevicecommand), _DbName, uri);
                throw ex;
            }

        }
        public MRespo InsertDevicecommand(Devicecommand d)
        {
            string _DbName = "";
            string uri = _url + "Utility/InsertDevicecommand";
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Devicecommand>(uri, d);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(InsertDevicecommand), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<MRespo>(res);
        }
        #endregion

        #region custom form-done
        public IEnumerable<Cfc> GetAllcustomforms()
        {
            string uri = _url + "Customform/customformgetall";
            string _DbName = "";
            IEnumerable<Cfc> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Cfc>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAllcustomforms), _DbName, uri);
            }
            return cmp;
        }
        public IEnumerable<Mnuoperation> GetAllmastermenu()
        {
            string uri = _url + "Customform/Getallmastermenu";
            string _DbName = "";
            IEnumerable<Mnuoperation> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Mnuoperation>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAllmastermenu), _DbName, uri);
            }
            return cmp;
        }
        public MRespo Savecustomformdata(Dictionary<string, string> obj)
        {
            string _DbName = "";
            string uri = _url + "Customform/Savecustomformdata";
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<Dictionary<string, string>>(uri, obj);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(Savecustomformdata), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<MRespo>(res);

        }
        public DataTable Getcustomformdata(string selectmenustr)
        {
            Getformdatareq objreq = new Getformdatareq();
            objreq.frmtblname = selectmenustr;
            string uri = _url + "Customform/Getcustomformdata";
            string _DbName = "";

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    var response = httpClient.PostAsJsonAsync<Getformdatareq>(uri, objreq);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        var tbldata = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                        tbldata.TableName = "Formdata";
                        return tbldata;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var res1 = response.Result.Content.ReadAsStringAsync().Result;
                        var tbldata1 = JsonConvert.DeserializeObjectAsync<DataTable>(res1.ToString()).Result;
                        tbldata1.TableName = "Formdata";
                        return tbldata1;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(Getcustomformdata), _DbName, uri);
            }
            DataTable emptyTable = new DataTable("Formdata");
            return emptyTable;
        }
        public IEnumerable<Mnuoperation> GetAddMasterfiledmenulist()
        {
            string uri = _url + "Customform/GetAddMasterfiledmenulist";
            string _DbName = "";
            IEnumerable<Mnuoperation> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<Mnuoperation>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAddMasterfiledmenulist), _DbName, uri);
            }
            return cmp;
        }

        #endregion

        #region AttendanceDetails-done

        public DataTable AttendanceDetails(int EmpID)
        {
            AttendanceDetails objreq = new AttendanceDetails();
            objreq.EmpID = EmpID;
            string _DbName = "";

            string uri = _url + "Utility/GetAttendanceDetails";
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var response = httpClient.PostAsJsonAsync<AttendanceDetails>(uri, objreq);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;

                        var tbldata = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;

                        tbldata.TableName = "Formdata";
                        return tbldata;
                    }
                    else if (response.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        var res1 = response.Result.Content.ReadAsStringAsync().Result;

                        var tbldata1 = JsonConvert.DeserializeObjectAsync<DataTable>(res1.ToString()).Result;

                        tbldata1.TableName = "Formdata";
                        return tbldata1;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(AttendanceDetails), _DbName, uri);


            }
            DataTable emptyTable = new DataTable("Formdata");
            return emptyTable;
        }

        public string GetAttendanceDetails(AttanList ls)
        {
            string _DbName = "";
            string uri = _url + "Utility/GetAttendanceDetails";
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<AttanList>(uri, ls);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                    //var tbldata = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;

                    //tbldata.TableName = "Formdata";
                    //return tbldata;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAttendanceDetails), _DbName, uri);


            }
            return res;

        }

        public string GetAttendencesummary(AttanList ls)
        {
            string _DbName = "";
            string uri = _url + "Utility/GetAttendencesummary";
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<AttanList>(uri, ls);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                    //var tbldata = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;

                    //tbldata.TableName = "Formdata";
                    //return tbldata;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAttendencesummary), _DbName, uri);


            }
            return res;
        }
        #region GetAttendanceDetailsFasttrack
        public string GetAttendanceDetailsFastrack(AttanList ls)
        {
            string _DbName = "";
            var res = "";
            string uri = _url + "Employee/GetAttendanceDetailsFasttrack";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<AttanList>(uri, ls);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAttendanceDetailsFastrack), _DbName, uri);


            }
            return res;
        }

        public string GetAttendanceDetailsFastrack_NewMissPunch(AttanList ls)
        {
            string _DbName = "";
            var res = "";
            string uri = _url + "Employee/GetAttendanceDetailsFasttrack_MissPunchDetails";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<AttanList>(uri, ls);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAttendanceDetailsFastrack), _DbName, uri);
            }
            return res;
        }
        #endregion
        #endregion

        #region BU Attendance APIs

        public string GetAssignedBUs(string loginemployeeid, string RoleID)
        {
            string _DbName = "";
            string uri = _url + "BU/GetAssignedBUs";
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postData = new { loginemployeeid = loginemployeeid, RoleID = RoleID };
                    var postTask = client.PostAsJsonAsync(uri, postData);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAssignedBUs), _DbName, uri);
            }
            return res;
        }

        public string GetAttendanceDetailsBU(AttanList ls)
        {
            string _DbName = "";
            string uri = _url + "BU/GetAttendanceDetailsBU";
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<AttanList>(uri, ls);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(GetAttendanceDetailsBU), _DbName, uri);
            }
            return res;
        }

        #endregion

    }
    public class ReportDataRestClient
    {
        private readonly string _url = ConfigurationManager.AppSettings["webapipaytime"];
        #region DailyInOutReport
        public IEnumerable<DailyInOutReport> ViewDailyReportQueryGetAll()
        {
            string uri = _url + "Report/ViewDailyReportQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var DailyInOutReport = JsonConvert.DeserializeObjectAsync<List<DailyInOutReport>>(res.ToString()).Result;
                return DailyInOutReport;
            }
        }
        public IEnumerable<DailyInOutReport> ViewDailyReportQueryGetAll(reqDailyInOutReport objreq)
        {

            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            //GetDeviceDetails.ProcessLogLogFileWrite("ViewDailyReportQueryGetAll", Isdateformat);
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    //GetDeviceDetails.ProcessLogLogFileWrite("ViewDailyReportQueryGetAll", objreq.FromDate);
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                    //GetDeviceDetails.ProcessLogLogFileWrite("ViewDailyReportQueryGetAll", objreq.Todate);
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }


            // DataTable dt = new DataTable();

            //string uri = _url + "Report/ViewDailyReportQueryGetAll?FromDate=" + objreq.FromDate + "&Todate=" + objreq.Todate + "&EmpID=" + objreq.EmpID + "&CmpID=" + objreq.CompanyID + "&RptID=" + objreq.RptID + "&IsFilo=" + objreq.IsFilo;
            string uri = _url + "Report/ViewDailyReportQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                //var ss = response.Result.IsSuccessStatusCode;
                //GetDeviceDetails.ProcessLogLogFileWrite("ViewDailyReportQueryGetAll", ss.ToString());
                if (response.Result.IsSuccessStatusCode)
                {

                    var DailyInOutReport = JsonConvert.DeserializeObjectAsync<List<DailyInOutReport>>(res.ToString()).Result;
                    if (objreq.ReportName == "ErrorCaseReport")
                    {
                        return DailyInOutReport.Where(x => x.FinalStatus == "E").ToList();
                    }
                    else if (objreq.ReportName == "AbsentReport")
                    {
                        return DailyInOutReport.Where(x => x.FinalStatus == "A").ToList();
                    }
                    else
                    {
                        return DailyInOutReport;
                    }

                }
                else
                {
                    var DailyInOutReport = new List<DailyInOutReport>();
                    return DailyInOutReport;
                }
                //var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                //dt = ToDataTable<DailyInOutReport>(DailyInOutReport);
            }
        }
        public IEnumerable<Summaryreport> ViewReportQueryGetAll(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            //GetDeviceDetails.ProcessLogLogFileWrite("ViewDailyReportQueryGetAll", Isdateformat);
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    //GetDeviceDetails.ProcessLogLogFileWrite("ViewDailyReportQueryGetAll", objreq.FromDate);
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                    //GetDeviceDetails.ProcessLogLogFileWrite("ViewDailyReportQueryGetAll", objreq.Todate);
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }
            // DataTable dt = new DataTable();
            //string uri = _url + "Report/ViewReportQueryGetAll?FromDate=" + objreq.FromDate + "&Todate=" + objreq.Todate + "&companyId=" + objreq.CompanyID + "&BranchId=" + objreq.BranchId + "&DepId=" + objreq.DepId + "&RptID=" + objreq.RptID;
            string uri = _url + "Report/ViewReportQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                if (response.Result.IsSuccessStatusCode)
                {
                    var DailyInOutReport = JsonConvert.DeserializeObjectAsync<List<Summaryreport>>(res.ToString()).Result;
                    return DailyInOutReport;
                }
                else
                {
                    var DailyInOutReport = new List<Summaryreport>();
                    return DailyInOutReport;
                }
                //var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                //dt = ToDataTable<DailyInOutReport>(DailyInOutReport);
            }
        }


        public IEnumerable<LeaveBalanceSummaryReport> ViewReportQueryGetLeaveBAlanceAll(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }

            // DataTable dt = new DataTable();
            //string uri = _url + "Report/ViewReportQueryGetAll?FromDate=" + objreq.FromDate + "&Todate=" + objreq.Todate + "&companyId=" + objreq.CompanyID + "&BranchId=" + objreq.BranchId + "&DepId=" + objreq.DepId + "&RptID=" + objreq.RptID;
            string uri = _url + "Report/ViewReportQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                if (response.Result.IsSuccessStatusCode)
                {
                    var DailyInOutReport = JsonConvert.DeserializeObjectAsync<List<LeaveBalanceSummaryReport>>(res.ToString()).Result;
                    return DailyInOutReport;
                }
                else
                {
                    var DailyInOutReport = new List<LeaveBalanceSummaryReport>();
                    return DailyInOutReport;
                }
                //var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                //dt = ToDataTable<DailyInOutReport>(DailyInOutReport);
            }
        }
        public IEnumerable<GetFacePunchdata> ViewReportQueryGetAllface(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }

            // DataTable dt = new DataTable();
            //string uri = _url + "Report/ViewReportQueryGetAll?FromDate=" + objreq.FromDate + "&Todate=" + objreq.Todate + "&companyId=" + objreq.CompanyID + "&BranchId=" + objreq.BranchId + "&DepId=" + objreq.DepId + "&RptID=" + objreq.RptID;
            string uri = _url + "Report/ViewDailyReportQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                if (response.Result.IsSuccessStatusCode)
                {
                    var GetFacePunchData = JsonConvert.DeserializeObjectAsync<List<GetFacePunchdata>>(res.ToString()).Result;
                    return GetFacePunchData;
                }
                else
                {
                    var GetFacePunchData = new List<GetFacePunchdata>();
                    return GetFacePunchData;
                }
                //var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                //dt = ToDataTable<DailyInOutReport>(DailyInOutReport);
            }
        }

        public IEnumerable<GetPunchwisereport> ViewReportGetPunchwisereport(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }

            // DataTable dt = new DataTable();
            //string uri = _url + "Report/ViewReportQueryGetAll?FromDate=" + objreq.FromDate + "&Todate=" + objreq.Todate + "&companyId=" + objreq.CompanyID + "&BranchId=" + objreq.BranchId + "&DepId=" + objreq.DepId + "&RptID=" + objreq.RptID;
            string uri = _url + "Report/ViewPunchwisereport";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var MonthlyFlexiInOutReport = JsonConvert.DeserializeObjectAsync<List<GetPunchwisereport>>(res.ToString()).Result;
                return MonthlyFlexiInOutReport;
                //var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                //dt = ToDataTable<DailyInOutReport>(DailyInOutReport);
            }
        }

        public IEnumerable<GetMissPunchReport> ViewReportmisspunchreport(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }

            string uri = _url + "Report/Viewmisspunchreport";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var MonthlyFlexiInOutReport = JsonConvert.DeserializeObjectAsync<List<GetMissPunchReport>>(res.ToString()).Result;
                return MonthlyFlexiInOutReport;
            }
        }

        public IEnumerable<GetjoinrejoinReport> ViewReportjoinrejoinreport(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }

            string uri = _url + "Report/Viewjoinrejoinreport";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var MonthlyFlexiInOutReport = JsonConvert.DeserializeObjectAsync<List<GetjoinrejoinReport>>(res.ToString()).Result;
                return MonthlyFlexiInOutReport;
            }
        }


        public IEnumerable<GetFaceFilodetaildata> ViewReportQueryGetFiloface(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }

            // DataTable dt = new DataTable();
            //string uri = _url + "Report/ViewReportQueryGetAll?FromDate=" + objreq.FromDate + "&Todate=" + objreq.Todate + "&companyId=" + objreq.CompanyID + "&BranchId=" + objreq.BranchId + "&DepId=" + objreq.DepId + "&RptID=" + objreq.RptID;
            string uri = _url + "Report/ViewDailyReportQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                if (response.Result.IsSuccessStatusCode)
                {
                    var GetFacePunchData = JsonConvert.DeserializeObjectAsync<List<GetFaceFilodetaildata>>(res.ToString()).Result;
                    return GetFacePunchData;
                }
                else
                {
                    var GetFacePunchData = new List<GetFaceFilodetaildata>();
                    return GetFacePunchData;
                }
                //var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                //dt = ToDataTable<DailyInOutReport>(DailyInOutReport);
            }
        }


        public IEnumerable<LeaveBalanceReport> GetLeaveBalanceReport(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }
            //string uri = _url + "Report/GetReportData?EmpID=" + objreq.EmpID + "&FromDate=" + objreq.FromDate + "&Todate=" + objreq.Todate + "&CmpID=" + objreq.CompanyID + "&BranchId=" + objreq.BranchId + "&DepId=" + objreq.DepId + "&RptID=" + objreq.RptID + "&IsFilo=" + objreq.IsFilo;
            string uri = _url + "Report/GetReportData";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var DailyInOutReport = JsonConvert.DeserializeObjectAsync<List<LeaveBalanceReport>>(res.ToString()).Result;
                return DailyInOutReport;
            }
        }


        public IEnumerable<LeaveBalanceStatusReport> LeaveBalanceStatusReport(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }
            //string uri = _url + "Report/GetReportData?EmpID=" + objreq.EmpID + "&FromDate=" + objreq.FromDate + "&Todate=" + objreq.Todate + "&CmpID=" + objreq.CompanyID + "&BranchId=" + objreq.BranchId + "&DepId=" + objreq.DepId + "&RptID=" + objreq.RptID + "&IsFilo=" + objreq.IsFilo;
            string uri = _url + "Report/GetReportData";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var LeaveBalanceSummaryReport = JsonConvert.DeserializeObjectAsync<List<LeaveBalanceStatusReport>>(res.ToString()).Result;
                return LeaveBalanceSummaryReport;
            }
        }


        public IEnumerable<DailySummaryReport> DailyOperationsSummaryReport(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }
            string uri = _url + "Report/ViewDailyReportQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var LeaveBalanceSummaryReport = JsonConvert.DeserializeObjectAsync<List<DailySummaryReport>>(res.ToString()).Result;
                return LeaveBalanceSummaryReport;
            }
        }


        public IEnumerable<InstallationStatusReport> InstallationStatusReport(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }
            string uri = _url + "Report/ViewDailyReportQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var LeaveBalanceSummaryReport = JsonConvert.DeserializeObjectAsync<List<InstallationStatusReport>>(res.ToString()).Result;
                return LeaveBalanceSummaryReport;
            }
        }


        public IEnumerable<FaceWebPunchDetailReport> FaceWebPunchDetailsReport(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }
            string uri = _url + "Report/ViewDailyReportQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var LeaveBalanceSummaryReport = JsonConvert.DeserializeObjectAsync<List<FaceWebPunchDetailReport>>(res.ToString()).Result;
                return LeaveBalanceSummaryReport;
            }
        }

        public IEnumerable<DailyWorkingHoursReport> DailyWorkingHoursReport(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }
            string uri = _url + "Report/ViewDailyReportQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var LeaveBalanceSummaryReport = JsonConvert.DeserializeObjectAsync<List<DailyWorkingHoursReport>>(res.ToString()).Result;
                return LeaveBalanceSummaryReport;
            }
        }


        public IEnumerable<DailyWorkingHoursINOUTReport> DailyWorkingHoursINOUTReport(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }
            string uri = _url + "Report/ViewDailyReportQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var LeaveBalanceSummaryReport = JsonConvert.DeserializeObjectAsync<List<DailyWorkingHoursINOUTReport>>(res.ToString()).Result;
                return LeaveBalanceSummaryReport;
            }
        }


        public IEnumerable<AbsentReport> AbsentReportDetails(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }
            string uri = _url + "Report/ViewDailyReportQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var LeaveBalanceSummaryReport = JsonConvert.DeserializeObjectAsync<List<AbsentReport>>(res.ToString()).Result;
                return LeaveBalanceSummaryReport;
            }
        }

        public IEnumerable<MonthlyFlexiInOutReport> MonthlyFlexiInOutReport(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }
            //string uri = _url + "Report/GetReportData?EmpID=" + objreq.EmpID + "&FromDate=" + objreq.FromDate + "&Todate=" + objreq.Todate + "&CmpID=" + objreq.CompanyID + "&BranchId=" + objreq.BranchId + "&DepId=" + objreq.DepId + "&RptID=" + objreq.RptID + "&IsFilo=" + objreq.IsFilo;
            string uri = _url + "Report/ViewDailyReportQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var MonthlyFlexiInOutReport = JsonConvert.DeserializeObjectAsync<List<MonthlyFlexiInOutReport>>(res.ToString()).Result;
                return MonthlyFlexiInOutReport;
            }
        }


        public IEnumerable<WeeklyFlexiInOutReport> WeeklyFlexiInOutReport(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }

            string uri = _url + "Report/ViewDailyReportQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var WeeklyFlexiInOutReport = JsonConvert.DeserializeObjectAsync<List<WeeklyFlexiInOutReport>>(res.ToString()).Result;
                return WeeklyFlexiInOutReport;
            }
        }

        public DataTable GetReportData(reqDailyInOutReport objreq)
        {
            //string uri = _url + "Report/GetReportData?EmpID=" + objreq.EmpID + "&FromDate=" + objreq.FromDate + "&Todate=" + objreq.Todate + "&CmpID=" + objreq.CompanyID + "&BranchId=" + objreq.BranchId + "&DepId=" + objreq.DepId + "&RptID=" + objreq.RptID + "&IsFilo=" + objreq.IsFilo;
            string uri = _url + "Report/GetReportData";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                return DailyInOutReport;
            }
        }

        public DataTable GetSchoolReportsData(reqDailyInOutReport objreq)
        {
            string uri = _url + "Report/SchoolReports";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                return DailyInOutReport;
            }
        }


        public DataTable GetbionicsevenData(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }

            string uri = _url + "Report/Bionicsevenreport";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                return DailyInOutReport;
            }
        }


        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }


        public IEnumerable<MonthlyMuster> ViewMonthlyMusterQueryGetAll(reqDailyInOutReport objreq)
        {

            // DataTable dt = new DataTable();

            //string uri = _url + "Report/ViewMonthlyMusterQueryGetAll?EmpID=" + objreq.EmpID + "&FromDate=" + objreq.FromDate + "&companyId=" + objreq.CompanyID + "&BranchId=" + objreq.BranchId + "&DepId=" + objreq.DepId + "&IsFilo=" + objreq.IsFilo + "";
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }
            string uri = _url + "Report/ViewMonthlyMusterQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq); ;
                var res = response.Result.Content.ReadAsStringAsync().Result;
                if (response.Result.IsSuccessStatusCode)
                {
                    var DailyInOutReport = JsonConvert.DeserializeObjectAsync<List<MonthlyMuster>>(res.ToString()).Result;

                    return DailyInOutReport;
                }
                else
                {
                    var DailyInOutReport = new List<MonthlyMuster>();
                    return DailyInOutReport;
                }
                //var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                //dt = ToDataTable<DailyInOutReport>(DailyInOutReport);
            }
        }

        //for excel data 
        public byte[] ViewMonthlyMusterQueryGetAllExcelData(reqDailyInOutReport objreq)
        {

            DataTable dataTable = new DataTable();
            byte[] exceldata = new byte[0];  // An empty byte array

            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }
            string uri = _url + "Report/ViewMonthlyMusterQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                if (response.Result.IsSuccessStatusCode)
                {
                    dataTable = JsonConvert.DeserializeObject<DataTable>(res);
                    List<string> columnsToRemove = new List<string>
                    {
                        "Empid","branchId","DepartmentId","DesignationId","CompanyId","MonthRepostartday","LastdayMonth","Empidlev","col_dateLev","Leavetype","Leavecnt","Levlst","col_date","col_daylev"
                    };
                    foreach (var column in columnsToRemove)
                    {
                        if (dataTable.Columns.Contains(column))
                        {
                            dataTable.Columns.Remove(column);
                        }
                    }
                    exceldata = ExportDataTableToExcel(dataTable);
                    return exceldata;
                }
                else
                {
                    return exceldata;
                }
            }
        }

        private byte[] ExportDataTableToExcel(DataTable dt)
        {
            // Set the license context for non-commercial use
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("MonthlyMuster");
                worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                return package.GetAsByteArray(); // Convert to byte array
            }
        }
        public IEnumerable<Workingduration> MonthlyWorkingDurationReport(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }

            // DataTable dt = new DataTable();

            //string uri = _url + "Report/MonthlyWorkingDurationReport?EmpID=" + objreq.EmpID + "&FromDate=" + objreq.FromDate + "&companyId=" + objreq.CompanyID + "&BranchId=" + objreq.BranchId + "&DepId=" + objreq.DepId + "&IsFilo=" + objreq.IsFilo + "";
            string uri = _url + "Report/MonthlyWorkingDurationReport";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                if (response.Result.IsSuccessStatusCode)
                {
                    var DailyInOutReport = JsonConvert.DeserializeObjectAsync<List<Workingduration>>(res.ToString()).Result;
                    return DailyInOutReport;
                }
                else
                {
                    var DailyInOutReport = new List<Workingduration>();
                    return DailyInOutReport;
                }
                //var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                //dt = ToDataTable<DailyInOutReport>(DailyInOutReport);
            }
        }

        public IEnumerable<Workingduration> MonthlyWorkingDurationCustom(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }

            // DataTable dt = new DataTable();

            //string uri = _url + "Report/MonthlyWorkingDurationReport?EmpID=" + objreq.EmpID + "&FromDate=" + objreq.FromDate + "&companyId=" + objreq.CompanyID + "&BranchId=" + objreq.BranchId + "&DepId=" + objreq.DepId + "&IsFilo=" + objreq.IsFilo + "";
            string uri = _url + "Report/MonthlyWorkingDurationReport_Custom";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                if (response.Result.IsSuccessStatusCode)
                {
                    var DailyInOutReport = JsonConvert.DeserializeObjectAsync<List<Workingduration>>(res.ToString()).Result;
                    return DailyInOutReport;
                }
                else
                {
                    var DailyInOutReport = new List<Workingduration>();
                    return DailyInOutReport;
                }
                //var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                //dt = ToDataTable<DailyInOutReport>(DailyInOutReport);
            }
        }

        public IEnumerable<MonthlyKmReport> ViewMonthlyKmReportQueryGetAll(reqDailyInOutReport objreq)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[0] + "-" + ToDate[1];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else if (Isdateformat.ToString().Trim() == "dd-mm-yyyy")
            {

                var fromDate = objreq.FromDate.Split('-');
                if (fromDate[0].Length != 4)
                {
                    objreq.FromDate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                    var ToDate = objreq.Todate.Split('-');
                    objreq.Todate = ToDate[2] + "-" + ToDate[1] + "-" + ToDate[0];
                }
                else
                {
                    objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                    objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                objreq.FromDate = Convert.ToDateTime(objreq.FromDate).ToString("yyyy-MM-dd");
                objreq.Todate = Convert.ToDateTime(objreq.Todate).ToString("yyyy-MM-dd");
            }
            string uri = _url + "Report/ViewMonthlyKMReportGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq); ;
                var res = response.Result.Content.ReadAsStringAsync().Result;
                if (response.Result.IsSuccessStatusCode)
                {
                    var DailyInOutReport = JsonConvert.DeserializeObjectAsync<List<MonthlyKmReport>>(res.ToString()).Result;

                    return DailyInOutReport;
                }
                else
                {
                    var DailyInOutReport = new List<MonthlyKmReport>();
                    return DailyInOutReport;
                }
            }
        }

        #endregion
        #region PrintReport
        public DataTable ViewDailyReportQueryDatatableGetAll(reqDailyInOutReport objreq)
        {

            // DataTable dt = new DataTable();

            string uri = _url + "Report/ViewDailyReportQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;

                if (objreq.ReportName == "ErrorCaseReport")
                {
                    DataTable dtError;
                    dtError = DailyInOutReport.Clone();
                    foreach (DataRow dr in DailyInOutReport.Select("FinalStatus = 'E'"))
                    {
                        dtError.ImportRow(dr);
                    }
                    dtError.TableName = "ReportData";
                    return dtError;
                }
                else if (objreq.ReportName == "AbsentReport")
                {
                    DataTable dtabsent;
                    dtabsent = DailyInOutReport.Clone();
                    foreach (DataRow dr in DailyInOutReport.Select("FinalStatus = 'A'"))
                    {
                        dtabsent.ImportRow(dr);
                    }
                    dtabsent.TableName = "ReportData";
                    return dtabsent;
                }
                else
                {
                    DailyInOutReport.TableName = "ReportData";
                    return DailyInOutReport;
                }




                //dt = ToDataTable<DailyInOutReport>(DailyInOutReport);
            }
        }
        public DataTable ViewMonthlyMusterQueryDatatableGetAll(reqDailyInOutReport objreq)
        {

            // DataTable dt = new DataTable();

            string uri = _url + "Report/ViewMonthlyMusterQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                DailyInOutReport.TableName = "ReportData";
                return DailyInOutReport;
            }
        }
        public DataTable MonthlyWorkingDurationReportDatatable(reqDailyInOutReport objreq)
        {

            string uri = _url + "Report/MonthlyWorkingDurationReport";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;

                var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                DailyInOutReport.TableName = "ReportData";
                return DailyInOutReport;
            }
        }
        public DataTable ViewReportQueryDatatableGetAll(reqDailyInOutReport objreq)
        {

            string uri = _url + "Report/ViewReportQueryGetAll";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                DailyInOutReport.TableName = "ReportData";
                return DailyInOutReport;
            }
        }

        //public XtraReport GetSchoolReport(int Rptid, int isfilo)
        //{

        //    XtraReport pt = new XtraReport();
        //    if (Rptid == 1)
        //    {
        //        if (isfilo == 1)
        //        {
        //            pt = new Daily_Attendance_Detail_ALL();
        //        }
        //        else
        //        {
        //            pt = new Daily_Attendance_Detail();
        //        }

        //    }
        //    else if (Rptid == 2)
        //    {
        //        pt = new Student_wise_Attendance_Percentage();
        //    }
        //    else if (Rptid == 3)
        //    {
        //        pt = new Student_Punch_Detail();
        //    }
        //    else if (Rptid == 4)
        //    {
        //        pt = new Student_Wise_Daily_Attendance_Summary();
        //    }
        //    else if (Rptid == 5)
        //    {
        //        pt = new Student_Absent_List();
        //    }
        //    else if (Rptid == 6)
        //    {
        //        pt = new Monthly_Attendance();
        //    }
        //    //else if (Rptid == 4)
        //    //{
        //    //    pt = new PrintDailyInReport();
        //    //}
        //    //else if (Rptid == 5)
        //    //{
        //    //    pt = new PrintErrorCaseReport();
        //    //}
        //    //else if (Rptid == 6)
        //    //{
        //    //    pt = new PrintAbsentReport();
        //    //}
        //    //else if (Rptid == 7)
        //    //{
        //    //    pt = new PrintLateINReport();
        //    //}
        //    //else if (Rptid == 8)
        //    //{
        //    //    pt = new PrintEarlyINReport();
        //    //}
        //    //else if (Rptid == 9)
        //    //{
        //    //    pt = new PrintEarlyOutReport();
        //    //}
        //    //else if (Rptid == 10)
        //    //{
        //    //    pt = new PrintLateOutReport();
        //    //}
        //    //else if (Rptid == 11)
        //    //{
        //    //    pt = new PrintOTReport();
        //    //}
        //    //else if (Rptid == 12)
        //    //{
        //    //    pt = new PrintContinuousLateArrivalReport();
        //    //}
        //    //else if (Rptid == 13)
        //    //{
        //    //    pt = new PrintContinuousEarlyDepartureReport();
        //    //}
        //    //else if (Rptid == 14)
        //    //{
        //    //    pt = new PrintContinuousAbsenteeismReport();
        //    //}
        //    //else if (Rptid == 15)
        //    //{
        //    //    pt = new PrintMachineRawTransactionReport();
        //    //}
        //    //else if (Rptid == 16)
        //    //{
        //    //    pt = new PrintManualPunchReport();
        //    //}
        //    //else if (Rptid == 17)
        //    //{
        //    //    pt = new PrintDepartmentSummaryReport();
        //    //}
        //    //else if (Rptid == 18)
        //    //{
        //    //    pt = new PrintEarlyInSummaryReport();
        //    //}
        //    //else if (Rptid == 19)
        //    //{
        //    //    pt = new PrintMonthlyMusterReport();
        //    //}
        //    //else if (Rptid == 20)
        //    //{
        //    //    pt = new PrintWorkingDurationReport();
        //    //}
        //    //else if (Rptid == 21)
        //    //{
        //    //    pt = new PrintLeaveBalanceReport();
        //    //}
        //    //else if (Rptid == 22)
        //    //{
        //    //    pt = new Printmonthlyreports();
        //    //}
        //    //else if (Rptid == 23)
        //    //{
        //    //    pt = new PrintWorkingDurationCustomReport();
        //    //}
        //    else
        //    {
        //        pt = new XtraReport1();
        //    }
        //    return pt;
        //}

        //public XtraReport GetReport(int Rptid)
        //{

        //    XtraReport pt = new XtraReport();
        //    if (Rptid == 1)
        //    {
        //        pt = new PrintDailyInOut();
        //    }
        //    else if (Rptid == 2)
        //    {
        //        pt = new PrintDailyInOutWithInOutDevice();
        //    }
        //    else if (Rptid == 3)
        //    {
        //        pt = new PrintFirstINLastOUTReport();
        //    }
        //    else if (Rptid == 4)
        //    {
        //        pt = new PrintDailyInReport();
        //    }
        //    else if (Rptid == 5)
        //    {
        //        pt = new PrintErrorCaseReport();
        //    }
        //    else if (Rptid == 6)
        //    {
        //        pt = new PrintAbsentReport();
        //    }
        //    else if (Rptid == 7)
        //    {
        //        pt = new PrintLateINReport();
        //    }
        //    else if (Rptid == 8)
        //    {
        //        pt = new PrintEarlyINReport();
        //    }
        //    else if (Rptid == 9)
        //    {
        //        pt = new PrintEarlyOutReport();
        //    }
        //    else if (Rptid == 10)
        //    {
        //        pt = new PrintLateOutReport();
        //    }
        //    else if (Rptid == 11)
        //    {
        //        pt = new PrintOTReport();
        //    }
        //    else if (Rptid == 12)
        //    {
        //        pt = new PrintContinuousLateArrivalReport();
        //    }
        //    else if (Rptid == 13)
        //    {
        //        pt = new PrintContinuousEarlyDepartureReport();
        //    }
        //    else if (Rptid == 14)
        //    {
        //        pt = new PrintContinuousAbsenteeismReport();
        //    }
        //    else if (Rptid == 15)
        //    {
        //        pt = new PrintMachineRawTransactionReport();
        //    }
        //    else if (Rptid == 16)
        //    {
        //        pt = new PrintManualPunchReport();
        //    }
        //    else if (Rptid == 17)
        //    {
        //        pt = new PrintDepartmentSummaryReport();
        //    }
        //    else if (Rptid == 18)
        //    {
        //        pt = new PrintEarlyInSummaryReport();
        //    }
        //    else if (Rptid == 19)
        //    {
        //        pt = new PrintMonthlyMusterReport();
        //    }
        //    else if (Rptid == 20)
        //    {
        //        pt = new PrintWorkingDurationReport();
        //    }
        //    else if (Rptid == 21)
        //    {
        //        pt = new PrintLeaveBalanceReport();
        //    }
        //    else if (Rptid == 22)
        //    {
        //        pt = new Printmonthlyreports();
        //    }
        //    else if (Rptid == 23)
        //    {
        //        pt = new PrintWorkingDurationCustomReport();
        //    }
        //    else if (Rptid == 24)
        //    {
        //        pt = new PrintDailyFlexiReport();
        //    }
        //    else if (Rptid == 25)
        //    {
        //        pt = new Printwithoutmaskreport();
        //    }
        //    else if (Rptid == 26)
        //    {
        //        pt = new Printwithoutmaskreport();
        //    }
        //    else if (Rptid == 30)
        //    {
        //        pt = new PrintDailyFlexiReport();
        //    }
        //    else
        //    {
        //        pt = new XtraReport1();
        //    }
        //    return pt;
        //}

        //public XtraReport GetSmallReport(int Rptid)
        //{
        //    XtraReport pt = new XtraReport();
        //    if (Rptid == 1)
        //    {
        //        pt = new PrintDailyInOutSmall();
        //    }
        //    else if (Rptid == 2)
        //    {
        //        pt = new PrintDailyInOutWithInOutDeviceSmall();
        //    }
        //    else if (Rptid == 3)
        //    {
        //        pt = new PrintFirstINLastOUTReportSmall();
        //    }
        //    else if (Rptid == 5)
        //    {
        //        pt = new PrintErrorCaseReportSmall();
        //    }
        //    else if (Rptid == 7)
        //    {
        //        pt = new PrintLateINReportSmall();
        //    }
        //    else if (Rptid == 8)
        //    {
        //        pt = new PrintEarlyINReportSmall();
        //    }
        //    else if (Rptid == 9)
        //    {
        //        pt = new PrintEarlyOutReportSmall();
        //    }
        //    else if (Rptid == 10)
        //    {
        //        pt = new PrintLateOutReportSmall();
        //    }
        //    else if (Rptid == 15)
        //    {
        //        pt = new PrintMachineRawTransactionReportSmall();
        //    }
        //    else if (Rptid == 17)
        //    {
        //        pt = new PrintDepartmentSummaryReportSmall();
        //    }
        //    else if (Rptid == 21)
        //    {
        //        pt = new PrintLeaveBalanceReportSmall();
        //    }
        //    return pt;
        //}
        //public XtraReport PrintReports(reqDailyInOutReport csreq, string Printby, DataTable model)
        //{
        //    var pt = new XtraReport();
        //    //var model = new DataTable();
        //    int IsSchool = Convert.ToInt32(HttpContext.Current.Session["IsSchool"]);
        //    if (csreq != null)
        //    {
        //        if (IsSchool == 1)
        //        {
        //            pt = GetSchoolReport(csreq.RptID, csreq.IsFilo);
        //            string frmdt = csreq.FromDate != null || csreq.FromDate != "" ? Convert.ToDateTime(csreq.FromDate).ToString("dd-MMM-yyyy") : "";
        //            string todt = csreq.Todate != null || csreq.Todate != "" ? Convert.ToDateTime(csreq.Todate).ToString("dd-MMM-yyyy") : "";
        //            string Strfrmdt = "For Period " + frmdt + " TO " + todt + "";
        //            if (csreq.RptID == 6)
        //            {
        //                Strfrmdt = "For Month " + Convert.ToDateTime(csreq.FromDate).ToString("MMM-yyyy");
        //            }
        //            pt.Parameters["FromDate"].Value = Strfrmdt;
        //            pt.Parameters["Todate"].Value = csreq.Todate != null || csreq.Todate != "" ? Convert.ToDateTime(csreq.Todate).ToString("dd-MMM-yyyy") : "";
        //        }
        //        else
        //        {

        //            TimeSpan ts = Convert.ToDateTime(csreq.Todate) - Convert.ToDateTime(csreq.FromDate);
        //            int days = ts.Days;

        //            if (days <= 5 && (csreq.RptID == 1 || csreq.RptID == 2 || csreq.RptID == 3 || csreq.RptID == 5 || csreq.RptID == 7 || csreq.RptID == 8 || csreq.RptID == 9 || csreq.RptID == 10 || csreq.RptID == 15 || csreq.RptID == 17 || csreq.RptID == 21))
        //            {
        //                pt = GetSmallReport(csreq.RptID);

        //            }
        //            else
        //            {
        //                pt = GetReport(csreq.RptID);

        //            }
        //            if (csreq.RptID == 1)
        //            {
        //                pt.Parameters["IsSchool"].Value = IsSchool;
        //            }
        //            if (csreq.RptID <= 16)
        //            {
        //                #region Daily Report
        //                pt.Parameters["FromDate"].Value = csreq.FromDate;
        //                pt.Parameters["Todate"].Value = csreq.Todate;
        //                #endregion
        //            }
        //            else if (csreq.RptID > 16)
        //            {
        //                if (csreq.RptID == 17 || csreq.RptID == 18)
        //                {

        //                    if (csreq.RptID == 18)
        //                    {
        //                        pt.Parameters["FromDate"].Value = csreq.FromDate != null || csreq.FromDate != "" ? Convert.ToDateTime(csreq.FromDate).ToString("dd-MMM-yyyy") : "";
        //                        pt.Parameters["Todate"].Value = csreq.Todate != null || csreq.Todate != "" ? Convert.ToDateTime(csreq.Todate).ToString("dd-MMM-yyyy") : "";
        //                    }
        //                }
        //                else if (csreq.RptID == 21 || csreq.RptID == 23 || csreq.RptID == 24 || csreq.RptID == 25 || csreq.RptID == 26)
        //                {
        //                    pt.Parameters["FromDate"].Value = csreq.FromDate != null || csreq.FromDate != "" ? Convert.ToDateTime(csreq.FromDate).ToString("dd-MMM-yyyy") : "";
        //                    pt.Parameters["Todate"].Value = csreq.Todate != null || csreq.Todate != "" ? Convert.ToDateTime(csreq.Todate).ToString("dd-MMM-yyyy") : "";
        //                    if (csreq.RptID == 25 || csreq.RptID == 26)
        //                    {
        //                        if (csreq.RptID == 25)
        //                        {
        //                            //pt.Parameters["Header"].Value = "Employee Mask Report";
        //                            pt.Parameters["Header"].Value = "Mask Report";
        //                        }
        //                        else
        //                        {
        //                            pt.Parameters["Header"].Value = "Employee Temperature(℃) Report";
        //                        }

        //                    }
        //                    //model = GetReportData(csreq);
        //                }
        //                else if (csreq.RptID == 22)
        //                {
        //                    pt.Parameters["FromDate"].Value = csreq.FromDate != null || csreq.FromDate != "" ? Convert.ToDateTime(csreq.FromDate).ToString("dd-MMM-yyyy") : "";
        //                }
        //            }
        //        }
        //    }
        //    pt.Parameters["printby"].Value = Printby;
        //    pt.DataSource = model;
        //    pt.DataMember = "ReportData";
        //    return pt;
        //}
        #endregion
        #region Custome Report templates
        public DataTable Getcustomreportdata(reqDailyInOutReport objreq)
        {
            string uri = _url + "Report/Getcustomreport";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<reqDailyInOutReport>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;

                DailyInOutReport.TableName = "ReportData";
                return DailyInOutReport;

            }
        }

        public IEnumerable<Reporttemplate> GetCustomreportlist()
        {
            string uri = _url + "Report/ReporttemplateGetall";
            using (HttpClient client = new HttpClient())
            {

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                Reporttemplate obj = new Reporttemplate();
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<Reporttemplate>>(res.ToString()).Result;
                return cmp;
            }
        }


        public string Getreportfields(int Rpt)
        {
            DataTable dt = new DataTable();
            string result = string.Empty;
            if (Rpt == 1)
            {
                var Report = new List<DailyInOutReportCustom>();
                dt = ToDataTable<DailyInOutReportCustom>(Report);
                DataTable dtcl = new DataTable();
                dtcl.Columns.Add("colid", typeof(int));
                dtcl.Columns.Add("colname", typeof(string));
                dtcl.Columns.Add("coldisplay", typeof(string));


                dtcl.Rows.Add(1, "Empcode", "Employee Code");
                dtcl.Rows.Add(2, "EmpName", "Employee Name");
                dtcl.Rows.Add(3, "CardNo", "Employee CardNo");
                dtcl.Rows.Add(4, "BranchName", "Branch Name");
                dtcl.Rows.Add(5, "CompanyName", "Company Name");
                dtcl.Rows.Add(6, "DepartmentName", "Department Name");
                dtcl.Rows.Add(7, "DesignationName", "Designation Name");
                dtcl.Rows.Add(8, "ShiftName", "Shift Name");
                dtcl.Rows.Add(9, "ShiftStartTime", "Shift StartTime");
                dtcl.Rows.Add(10, "ShiftEndTime", "Shift EndTime");
                dtcl.Rows.Add(11, "Attn_dt", "Attn Date");
                dtcl.Rows.Add(12, "FinalStatus", "FinalStatus");
                dtcl.Rows.Add(13, "TotHour", "Total Hours");
                dtcl.Rows.Add(14, "LessHr", "Less Hours");
                dtcl.Rows.Add(15, "LateHr", "Late Hours");
                dtcl.Rows.Add(16, "EarlyHr", "Early Hours");
                dtcl.Rows.Add(17, "OTHr", "Over Time Hours");
                dtcl.Rows.Add(18, "InTime", "In Time");
                dtcl.Rows.Add(19, "OutTime", "Out Time");
                dtcl.Rows.Add(20, "LateOUT", "Late Out Time");
                dtcl.Rows.Add(21, "EarlyIN", "Early In Time");

                result = JsonConvert.SerializeObject(dtcl);


                //result= GetPorertynames(dt);

            }
            else if (Rpt == 2)
            {
                var Report = new List<MonthlyMusterCustom>();
                dt = ToDataTable<MonthlyMusterCustom>(Report);
                result = GetPorertynames(dt);
            }
            else if (Rpt == 3)
            {
                var Report = new List<Summaryreport>();
                dt = ToDataTable<Summaryreport>(Report);
                result = GetPorertynames(dt);
            }
            return result;
        }

        public string GetPorertynames(DataTable dt)
        {
            DataTable dtcl = new DataTable();
            dtcl.Columns.Add("colid", typeof(int));
            dtcl.Columns.Add("colname", typeof(string));
            dtcl.Columns.Add("coldisplay", typeof(string));



            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dtcl.Rows.Add(i, dt.Columns[i].ColumnName.ToString());
            }
            return JsonConvert.SerializeObject(dtcl);

        }
        #endregion
        #region Dynamic Report templates
        public IEnumerable<Reporttemplate> GetDynamicreportlist()
        {
            string uri = _url + "Report/DynamicReporttemplateGetall";
            using (HttpClient client = new HttpClient())
            {

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                Reporttemplate obj = new Reporttemplate();
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<Reporttemplate>>(res.ToString()).Result;
                return cmp;
            }
        }

        public DataTable GetDynamicReportDeshBord(DynamicShowReporttemplate objreq)
        {
            string uri = _url + "Report/ShowDynamicReports";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<DynamicShowReporttemplate>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;

                DailyInOutReport.TableName = "ReportData";
                return DailyInOutReport;

            }
        }

        public DataTable GetDynamicCustomReportList(CustomerReportField objreq)
        {
            string uri = _url + "Report/ShowDynamicReports";
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.PostAsJsonAsync<CustomerReportField>(uri, objreq);

                var res = response.Result.Content.ReadAsStringAsync().Result;

                var DailyInOutReport = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;

                DailyInOutReport.TableName = "ReportData";
                return DailyInOutReport;

            }
        }
        #endregion
    }

    public class AlertDataRestClient
    {
        private readonly string _url = ConfigurationManager.AppSettings["webapipaytime"];

        #region EmailDomainDetail-done

        public IEnumerable<EmailDomainDetail> EmailDomainDetailGetAll()
        {
            string uri = _url + "Alert/EmailDomainDetailGetAll";
            string _DbName = "";
            IEnumerable<EmailDomainDetail> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<EmailDomainDetail>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmailDomainDetailGetAll), _DbName, uri);
            }
            return cmp;
        }
        #endregion

        #region EmailType-done
        public IEnumerable<EmailType> EmailTypeGetAll()
        {
            string uri = _url + "Alert/EmailTypeGetAll";
            string _DbName = "";
            IEnumerable<EmailType> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<EmailType>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmailTypeGetAll), _DbName, uri);
            }
            return cmp;
        }
        #endregion

        #region Emailconfiguration-done

        public IEnumerable<EmailConfiguration> EmailconfigurationGetAll()
        {
            string uri = _url + "Alert/EmailconfigurationGetAll";
            string _DbName = "";
            IEnumerable<EmailConfiguration> cmp = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    Task<HttpResponseMessage> response = client.GetAsync(uri);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        cmp = JsonConvert.DeserializeObjectAsync<List<EmailConfiguration>>(res.ToString()).Result;
                        return cmp;
                    }
                    else
                    {
                        int statusCodeValue = (int)response.Result.StatusCode;
                        string statusCodemessage = response.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmailconfigurationGetAll), _DbName, uri);
            }
            return cmp;
        }

        public EmailConfiguration EmailconfigurationAdd(EmailConfiguration Emailcfg)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            string _DbName = "";
            var json = jsonSerialiser.Serialize(Emailcfg);
            string uri = _url + "Alert/EmailconfigurationCreate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<EmailConfiguration>(uri, Emailcfg);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return new EmailConfiguration();
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        return new EmailConfiguration();
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }


                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmailconfigurationAdd), _DbName, uri);
            }
            return new EmailConfiguration();
        }

        public bool EmailconfigurationUpdate(int id, EmailConfiguration Emailcfg)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(Emailcfg);
            string uri = _url + "Alert/EmailconfigurationUpdate/" + id;
            string _DbName = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<EmailConfiguration>(uri, Emailcfg);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(EmailconfigurationUpdate), _DbName, uri);
            }
            return false;
        }

        public MRespo SavecreateTask(Taskscheduler obj)
        {
            var Isdateformat = HttpContext.Current.Session["IsDateFormat"].ToString();
            if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
            {
                var fromDate = obj.FromDate.Split('-');
                obj.FromDate = fromDate[0] + "-" + fromDate[1] + "-" + fromDate[2];
                var toDate = obj.Todate.Split('-');
                obj.Todate = toDate[0] + "-" + toDate[1] + "-" + toDate[2];
            }
            else
            {
                obj.FromDate = Convert.ToDateTime(obj.FromDate).ToString("yyyy-MM-dd");
                obj.Todate = Convert.ToDateTime(obj.Todate).ToString("yyyy-MM-dd");
            }
            string _DbName = "";
            var res = "";
            string uri = _url + "Alert/SavecreateTask";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<Taskscheduler>(uri, obj);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var cmp1 = JsonConvert.DeserializeObjectAsync<MRespo>(res.ToString()).Result;
                        return cmp1;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SavecreateTask), _DbName, uri);
            }
            return JsonConvert.DeserializeObject<MRespo>(res);
        }

        #endregion

        #region SendTestConnectionMail-done
        public bool SendTestConnectionMail(MailDetails MailDetails)
        {
            string _DbName = "";
            string uri = _url + "Alert/SendTestConnectionMail/";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<MailDetails>(uri, MailDetails);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SendTestConnectionMail), _DbName, uri);
            }
            return false;
        }
        #endregion

        #region SendReportMail-done
        public bool SendReportMail(MailDetails mailDetails)
        {
            string _DbName = "";
            string uri = _url + "Alert/SendReportMail/";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<MailDetails>(uri, mailDetails);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        var result = postTask.Result;
                        return result.IsSuccessStatusCode;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(SendReportMail), _DbName, uri);
            }
            return false;
        }
        #endregion

    }

    #region for Fastrackdevelopment New Block-done

    public class EmployeeDataRestClientFastrack
    {
        private readonly string _url = ConfigurationManager.AppSettings["webapipaytime"];
        private readonly string _urlMSNew = ConfigurationManager.AppSettings["webapipaytimeMS"];

        public string AttendanceCorrectionCreate(AttendanceCorrection objCorrection)
        {
            objCorrection.InPunchTime = Convert.ToDateTime(objCorrection.InPunchTime).ToString("yyyy-MM-dd HH:mm");
            objCorrection.OutPunchTime = Convert.ToDateTime(objCorrection.OutPunchTime).ToString("yyyy-MM-dd HH:mm");
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(objCorrection);
            string _DbName = "";
            string msg = "";
            string uri = _url + "Employee/AttendanceCorrectionCreate";
            var res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }
                    var postTask = client.PostAsJsonAsync<AttendanceCorrection>(uri, objCorrection);
                    postTask.Wait();
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        res = postTask.Result.Content.ReadAsStringAsync().Result;
                        return res;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(AttendanceCorrectionCreate), _DbName, uri);

            }
            return res;
        }
        public string AttendanceCorrectionUpdate(AttendanceCorrection objCorrection)
        {
            string msg = "";
            string _DbName = "";
            string uri = _urlMSNew + "Transaction/AttendanceCorrectionUpdate";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    var postTask = client.PostAsJsonAsync<AttendanceCorrection>(uri, objCorrection);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        msg = "OK";
                    }
                    else if (postTask.Result.StatusCode == HttpStatusCode.Conflict)
                    {
                        msg = result.ReasonPhrase;
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(AttendanceCorrectionUpdate), _DbName, uri);

            }
            return msg;
        }


        public string AttendanceCorrectionUpdateFastrack(AttendanceCorrection objCorrection)
        {
            string msg = "";
            string _DbName = "";
            string uri = _urlMSNew + "Transaction/AttendanceCorrectionUpdateFastrack_HD";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                    {
                        _DbName = HttpContext.Current.Session["DbName"].ToString();
                    }

                    var postTask = client.PostAsJsonAsync<AttendanceCorrection>(uri, objCorrection);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        msg = "OK";
                    }
                    else
                    {
                        int statusCodeValue = (int)postTask.Result.StatusCode;
                        string statusCodemessage = postTask.Result.ReasonPhrase;
                        string errorDetails = $"HTTP Error: {statusCodeValue} - {statusCodemessage}";
                        throw new HttpRequestException(errorDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, nameof(AttendanceCorrectionUpdate), _DbName, uri);

            }
            return msg;
        }
    }
    #endregion


    public class GatePassRequestExpiryDateRestClient
    {
        #region For GatePassRequestExpiry

        private readonly string _URLRegister = ConfigurationManager.AppSettings["webapipaytime"];

        public string GatePassRequestexpiryStatusUpdate(string CompID)
        {
            string result = string.Empty;
            string uri = _URLRegister + "GatePassPolicy/GatePassRequestexpiryStatusUpdate?CompanyID=" + CompID;
            using (HttpClient httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                httpClient.Timeout = TimeSpan.FromMinutes(15);
                Task<HttpResponseMessage> response = httpClient.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                result = res;
            }
            return result;
        }
        #endregion
    }

    public class SiteLanguages
    {
        public static List<Languages> AvailableLanguages = new List<Languages>
        {
             new Languages{ LangFullName = "English", LangCultureName = "en"},
             new Languages{ LangFullName = "Español", LangCultureName = "es"},
             new Languages{ LangFullName = "বাংলা", LangCultureName = "bn"},
             new Languages{ LangFullName = "हिन्दी", LangCultureName = "hi"},
             new Languages{ LangFullName = "ગુજરાતી", LangCultureName = "gu"},
             new Languages{ LangFullName = "العربية", LangCultureName = "ar"}
        };

        public static bool IsLanguageAvailable(string lang)
        {
            return AvailableLanguages.Where(a => a.LangCultureName.Equals(lang)).FirstOrDefault() != null ? true : false;
        }

        public static string GetDefaultLanguage()
        {
            return AvailableLanguages[0].LangCultureName;
        }

        public void SetLanguage(string lang)
        {
            try
            {
                if (!IsLanguageAvailable(lang))
                    lang = GetDefaultLanguage();
                var cultureInfo = new CultureInfo(lang);
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
                HttpCookie langCookie = new HttpCookie("culture", lang);
                langCookie.Expires = DateTime.Now.AddYears(1);
                HttpContext.Current.Response.Cookies.Add(langCookie);


            }
            catch (Exception ex)
            {

            }
        }




    }

    public class Languages
    {
        public string LangFullName { get; set; }
        public string LangCultureName { get; set; }
    }

    #region School Master
    public class SchoolMasterDataRestClient
    {
        private readonly string _url = ConfigurationManager.AppSettings["webapipaytime"];
        public IEnumerable<RoleRightsMapping> GetSchoolMenuRoleWise(int roleId)
        {
            string param = "?roleID=" + roleId;

            string uri = _url + "SchoolMaster/GetSchoolMenuRoleWise" + param;
            using (HttpClient client = new HttpClient())
            {

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                RoleRightsMapping obj = new RoleRightsMapping();
                var res = response.Result.Content.ReadAsStringAsync().Result;
                if (response.Result.IsSuccessStatusCode)
                {
                    var cmp = JsonConvert.DeserializeObjectAsync<List<RoleRightsMapping>>(res.ToString()).Result;
                    return cmp;
                }
                else
                {
                    return new List<RoleRightsMapping>();
                }
            }

        }
        public IEnumerable<Roles> SchoolRoleGetAll()
        {

            string uri = _url + "SchoolMaster/RoleGetAll";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<Roles>>(res.ToString()).Result;
                return cmp;
            }
        }
        public IEnumerable<RoleRightsMapping> RoleRightsGetAll(int roleId)
        {
            string param = "?roleID=" + roleId;
            string uri = _url + "SchoolMaster/RoleRightsGetAll" + param;
            using (HttpClient client = new HttpClient())
            {

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                RoleRightsMapping obj = new RoleRightsMapping();
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<RoleRightsMapping>>(res.ToString()).Result;
                return cmp;
            }

        }
        public bool RoleRightsDelete(int id)
        {
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                string uri = _url + "SchoolMaster/RoleRightsDelete/" + id;
                var postTask = client.PostAsJsonAsync(uri, id);
                postTask.Wait();

                var result = postTask.Result;

                return result.IsSuccessStatusCode;
            }
        }
        public RoleRightsMapping RoleRightsAdd(RoleRightsMapping obj)
        {
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                string uri = _url + "SchoolMaster/RoleRightsCreate";
                var postTask = client.PostAsJsonAsync<RoleRightsMapping>(uri, obj);
                postTask.Wait();
                var result = postTask.Result;
            }
            return new RoleRightsMapping();
        }
        public IEnumerable<ImportedFailLog> ImportedStudentfaillogGetAll(List<ImportedStudent> items)
        {
            string uri = _url + "SchoolMaster/ImportedStudentfaillogGetAll";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync<List<ImportedStudent>>(uri, items);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportedFailLog>>(res.ToString()).Result;

                return cmp;
            }
        }
        public IEnumerable<ImportedFailLog> ImportedGatefaillogGetAll(List<ImportedGate> items)
        {
            string uri = _url + "SchoolMaster/ImportedGatefaillogGetAll";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync<List<ImportedGate>>(uri, items);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportedFailLog>>(res.ToString()).Result;

                return cmp;
            }
        }
        public IEnumerable<ImportedFailLog> ImportedDevicefaillogGetAll(List<ImportedDevice> items)
        {
            string uri = _url + "SchoolMaster/ImportedDevicefaillogGetAll";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync<List<ImportedDevice>>(uri, items);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportedFailLog>>(res.ToString()).Result;

                return cmp;
            }
        }
        public IEnumerable<ImportedFailLog> ImportedStaffTypefaillogGetAll(List<ImportedStaffType> items)
        {
            string uri = _url + "SchoolMaster/ImportedStaffTypefaillogGetAll";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync<List<ImportedStaffType>>(uri, items);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportedFailLog>>(res.ToString()).Result;

                return cmp;
            }
        }
        public IEnumerable<ImportedFailLog> ImportedStafffaillogGetAll(List<ImportedStaff> items)
        {
            string uri = _url + "SchoolMaster/ImportedStafffaillogGetAll";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync<List<ImportedStaff>>(uri, items);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportedFailLog>>(res.ToString()).Result;

                return cmp;
            }
        }
        public IEnumerable<ImportedFailLog> ImportedDriverfaillogGetAll(List<ImportedDriver> items)
        {
            string uri = _url + "SchoolMaster/ImportedDriverfaillogGetAll";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync<List<ImportedDriver>>(uri, items);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportedFailLog>>(res.ToString()).Result;

                return cmp;
            }
        }
        public IEnumerable<ImportedFailLog> ImportedVehiclefaillogGetAll(List<ImportedVehicle> items)
        {
            string uri = _url + "SchoolMaster/ImportedVehiclefaillogGetAll";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync<List<ImportedVehicle>>(uri, items);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportedFailLog>>(res.ToString()).Result;

                return cmp;
            }
        }
        public IEnumerable<ImportedFailLog> ImportedShiftfaillogGetAll(List<ImportedShift> items)
        {
            string uri = _url + "SchoolMaster/ImportedShiftfaillogGetAll";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync<List<ImportedShift>>(uri, items);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportedFailLog>>(res.ToString()).Result;

                return cmp;
            }
        }
        public IEnumerable<ImportedFailLog> ImportedClassfaillogGetAll(List<ImportedClass> items)
        {
            string uri = _url + "SchoolMaster/ImportedClassfaillogGetAll";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync<List<ImportedClass>>(uri, items);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportedFailLog>>(res.ToString()).Result;

                return cmp;
            }
        }
        public IEnumerable<ImportedFailLog> ImportedDivisionfaillogGetAll(List<ImportedDivision> items)
        {
            string uri = _url + "SchoolMaster/ImportedDivisionfaillogGetAll";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                var postTask = client.PostAsJsonAsync<List<ImportedDivision>>(uri, items);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<ImportedFailLog>>(res.ToString()).Result;

                return cmp;
            }
        }
        public IEnumerable<Departments> GetDivisionbyclass(int Classid)
        {
            string param = "?Classid=" + Classid;
            string uri = _url + "SchoolMaster/Divbyclass" + param;
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<List<Departments>>(res.ToString()).Result;
                return cmp;
            }

        }

    }
    #endregion

    #region Payment Integration
    public class Paymentintegrationclient
    {
        private readonly string _url = ConfigurationManager.AppSettings["webapipaytime"];
        public ResMsg InitPayment(Paymenttrans obj)
        {
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                string uri = _url + "Master/SavePaymenttransaction";
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Paymenttrans>(uri, obj);
                postTask.Wait();
                var res = postTask.Result.Content.ReadAsStringAsync().Result;
                var cmp = JsonConvert.DeserializeObjectAsync<ResMsg>(res.ToString()).Result;
                return cmp;
            }
        }
        public ResMsg Savepaymentres(Paymentres obj,string authToken=null)
        {

            // string _urlpay = ConfigurationManager.AppSettings["paymentapi"];
            using (var client = new HttpClient())
            {
                //if (!string.IsNullOrEmpty(obj._token))
                var token = authToken;
                if (string.IsNullOrEmpty(token))
                {
                    token = HttpContext.Current?.Session?["token"]?.ToString();
                }
                string uri = _url + "Master/Savetransactionres";
                try
                {

                    //GetDeviceDetails.ProcessLogLogFileWrite("PaymentResponseLogUrl", uri + "  token :" + HttpContext.Current.Session["tokan"] != null ? HttpContext.Current.Session["tokan"].ToString() : "Session null");

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Paymentres>(uri, obj);
                    postTask.Wait();
                    var res = postTask.Result.Content.ReadAsStringAsync().Result;
                    var cmp = JsonConvert.DeserializeObjectAsync<ResMsg>(res.ToString()).Result;
                    return cmp;
                }
                catch (Exception ex)
                {
                    GetDeviceDetails.ProcessLogLogFileWrite("PaymentResponsesaveExceptionlog", uri + "  Exception :" + ex.Message + ex.StackTrace);
                    return null;
                }

            }
        }
        public ResMsg Savepaymentresnew(Paymentres obj)
        {

            // string _urlpay = ConfigurationManager.AppSettings["paymentapi"];
            using (var client = new HttpClient())
            {
                //if (!string.IsNullOrEmpty(obj._token))
                string uri = _url + "Registration/Savetransactionres";
                try
                {

                    //GetDeviceDetails.ProcessLogLogFileWrite("PaymentResponseLogUrl", uri + "  token :" + HttpContext.Current.Session["tokan"] != null ? HttpContext.Current.Session["tokan"].ToString() : "Session null");

                    if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                    }

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Paymentres>(uri, obj);
                    postTask.Wait();
                    var res = postTask.Result.Content.ReadAsStringAsync().Result;
                    var cmp = JsonConvert.DeserializeObjectAsync<ResMsg>(res.ToString()).Result;
                    return cmp;
                }
                catch (Exception ex)
                {
                    GetDeviceDetails.ProcessLogLogFileWrite("Newrespsaveexplog", uri + "Paymentid :" + obj.PaymentID + "  Exception :" + ex.Message + ex.StackTrace);
                    return null;
                }

            }
        }
        public DataTable Fillgatewaypara(int companyid)
        {
            string Param = "?companyid=" + companyid;
            string uri = _url + "Master/GetGatwayparameter" + Param;
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                return data;
            }
        }
        public DataTable Getplantotalamount(int planid)
        {
            string Param = "?planid=" + planid;
            string uri = _url + "Master/Getplantotalamount" + Param;
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                return data;

            }
        }

        public DataTable Cancelsubscription(Getpara obj)
        {

            string uri = _url + "Registration/StopRecuring";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.PostAsJsonAsync<Getpara>(uri, obj);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                return data;

            }
        }

        public DataTable CancelRecuringpaymentMail(string _email)
        {
            string Param = "?email=" + _email;
            string uri = _url + "Registration/CancelRecuringpaymentMail" + Param;
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                return data;
            }
        }



    }
    #endregion

    #region greythr_payroll
    public class payrollDataRestClient
    {
        private readonly string _url = ConfigurationManager.AppSettings["webapipaytime"];

        private static readonly string _baseurpayroll = ConfigurationManager.AppSettings["webapipaytimePayroll"];

        static readonly EncryptionHelper EncryptionHelperr = new EncryptionHelper();

        public DataTable GetpayrollgreythrEmpIDlst()
        {
            DataTable dt = new DataTable();
            //string uri = _url + "Master/dtEmployeeGetAll";
            string uri = _url + "payroll/GetpayrollgreythrEmpIDlst";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                dt = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                dt.TableName = "Employeemaster";
            }
            return dt;
        }


        public DataTable GetgreythrEmppayrollprocess(string Fromdate, string Todate)
        {
            DataTable dt = new DataTable();
            string Param = "?Fromdate=" + Fromdate + "&Todate=" + Todate + "";
            string uri = _url + "payroll/GetgreythrEmppayrollprocess" + Param;
            //string uri = _url + "payroll/GetgreythrEmppayrollprocess";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                dt = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                dt.TableName = "Employeemaster";
            }
            return dt;
        }

        public bool UpdategreythrEmpID(string _Empcode, string _employeeId, string graytyhrresponse)
        {

            using (var client = new HttpClient())
            {

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }

                string Param = "?Empcode=" + _Empcode + "&EmployeeId=" + _employeeId + "&graytyhrresponse=" + graytyhrresponse + "";
                string uri = _url + "payroll/UpdategreythrEmpID" + Param;
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, _employeeId);
                postTask.Wait();

                var result = postTask.Result;

                return result.IsSuccessStatusCode;
            }

        }

        public bool UpdateLOPProcessDetails(string Empcode, string greythrEmpID, int month, int year, string LOP, string EMpname, int InserUpdateFlag)
        {
            using (var client = new HttpClient())
            {

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                string Param = "?Empcode='" + Empcode + "'&GreythrEmpID='" + greythrEmpID + "'&month=" + month + "&year=" + year + "&LOP='" + LOP + "'&EMpname='" + EMpname + "'&InserUpdateFlag=" + InserUpdateFlag + "";
                string uri = _url + "payroll/UpdateLOPProcessDetails" + Param;
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, Param);
                postTask.Wait();
                var result = postTask.Result;
                return result.IsSuccessStatusCode;

            }

        }
        public DataTable GreythrEmppayrollProcessDetailCheck(string GraytHRID, int Month, int Year)
        {
            DataTable dt = new DataTable();
            string Param = "?GraytHRID=" + GraytHRID + "&Month=" + Month + "&Year=" + Year + "";
            string uri = _url + "payroll/GreythrEmppayrollProcessDetailCheck" + Param;
            //string uri = _url + "payroll/GetgreythrEmppayrollprocess";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                dt = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                dt.TableName = "tbllopprocessdetails";
            }
            return dt;
        }


        public DataTable Greythrselect(string cmpcode)
        {
            DataTable dt = new DataTable();

            string uri = _url + "payroll/Greythrselect?Cmpcode='" + cmpcode + "'";
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                Task<HttpResponseMessage> response = client.GetAsync(uri);
                var res = response.Result.Content.ReadAsStringAsync().Result;
                dt = JsonConvert.DeserializeObjectAsync<DataTable>(res.ToString()).Result;
                dt.TableName = "Employeemaster";
            }
            return dt;
        }

        //public void DecryptMonthlyCTC(DataTable dt)
        //{
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        string encryptedValue = row["monthly_ctc"].ToString();
        //        string decryptedValueString = EncryptionHelperr.Decrypt(encryptedValue);
        //        decimal decryptedValue;
        //        if (decimal.TryParse(decryptedValueString, out decryptedValue))
        //        {
        //            row["monthly_ctc"] = decryptedValue;
        //        }
        //        // If parsing fails, keep the original value
        //    }
        //}


        //public void DecryptMonthlyCTC(DataTable dt, int? decryptCount)
        //{
        //    //start - number of rows to skip before returning rows by limite
        //    //decryptCount - how many rows to return in dt
        //    int decryptCountVal = decryptCount.HasValue ? decryptCount.Value : dt.Rows.Count;
        //    for (int i = 0; i < decryptCountVal; i++)
        //    {
        //        DataRow row = dt.Rows[i];
        //        string encryptedValue = row["monthly_ctc"].ToString();
        //        string decryptedValueString = EncryptionHelperr.Decrypt(encryptedValue);
        //        decimal decryptedValue;

        //        if (decimal.TryParse(decryptedValueString, out decryptedValue))
        //        {
        //            row["monthly_ctc"] = decryptedValue;
        //        }
        //        // If parsing fails, keep the original value
        //    }
        //}

        //public object getemployeectcdataTable(DataTableFilterCTCAjaxPostModelandFilterData request)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
        //        {
        //            client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
        //        }

        //        string uri = _baseurpayroll + "EmployeeCTC/GetAllEmployeePayrollCTCDetails";

        //        try
        //        {
        //            HttpResponseMessage response = client.PostAsJsonAsync(uri, request).Result;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                var responseContent = response.Content.ReadAsStringAsync().Result;
        //                var jsonObject = JsonConvert.DeserializeObject<JObject>(responseContent);

        //                // Extract metadata
        //                var draw = jsonObject["draw"].Value<int>();
        //                var recordsTotal = jsonObject["recordsTotal"].Value<int>();
        //                var recordsFiltered = jsonObject["recordsFiltered"].Value<int>();

        //                // Extract and process the "data" array
        //                var jsonArray = jsonObject["data"] as JArray;
        //                if (jsonArray == null)
        //                {
        //                    throw new Exception("Data array is missing or invalid.");
        //                }
        //                DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonArray.ToString());
        //                DecryptMonthlyCTC(dt, request.length);

        //                dt.Columns.Add("NumericCTC", typeof(decimal));

        //                for (int i = 0; i < dt.Rows.Count; i++)
        //                {
        //                    DataRow row = dt.Rows[i];
        //                    decimal value;
        //                    if (decimal.TryParse(row["monthly_ctc"].ToString(), out value))
        //                    {
        //                        row["NumericCTC"] = value;
        //                    }
        //                }

        //                //#region Sorting of CTC
        //                //var orderColumn = request.order.FirstOrDefault();
        //                DataView dv = dt.DefaultView;
        //                if (request.order[0].dir == "asc")
        //                {
        //                    dv.Sort = "NumericCTC ASC"; // Sort by CTC in ascending order
        //                    dt = dv.ToTable();
        //                }
        //                else
        //                {
        //                    dv.Sort = "NumericCTC DESC"; // Sort by CTC in ascending order
        //                    dt = dv.ToTable();
        //                }
        //                //#endregion


        //                //#region returnig orignal format of Colunm
        //                // Converting the sorted decimal values back to string format
        //                for (int i = 0; i < dt.Rows.Count; i++)
        //                {
        //                    DataRow row = dt.Rows[i];
        //                    row["monthly_ctc"] = row["NumericCTC"].ToString();
        //                }
        //                //#endregion 

        //                //var dataList = JsonConvert.DeserializeObject<List<PayrollEmployeeCTCDetails>>(dt); 

        //                // Convert the DataTable to a list of PayrollEmployeeCTCDetails
        //                var dataList = dt.AsEnumerable()
        //                                 .Select(row => new PayrollEmployeeCTCDetails
        //                                 {
        //                                     //emp_id = row.Field<int>("emp_id"),
        //                                     emp_id = Convert.ToInt32(row["emp_id"]),
        //                                     emp_code = row.Field<string>("emp_code"),
        //                                     EmpName = row.Field<string>("EmpName"),
        //                                     EmpPhoto = row.Field<string>("EmpPhoto"),
        //                                     UserEmail = row.Field<string>("UserEmail"),
        //                                     monthly_ctc = row.Field<string>("monthly_ctc"),
        //                                     structure_name = row.Field<string>("structure_name")
        //                                 })
        //                                 .ToList();

        //                // Return the formatted result
        //                return new
        //                {
        //                    draw = draw,
        //                    recordsTotal = recordsTotal,
        //                    recordsFiltered = recordsFiltered,
        //                    data = dataList
        //                };
        //            }
        //            else
        //            {
        //                throw new Exception("Failed to retrieve data: " + response.ReasonPhrase);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("An error occurred while fetching data.", ex);
        //        }
        //    }
        //}


        public DataTable getemployeectcdataTable(DataTableFilterCTCAjaxPostModelandFilterData request)
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
                {
                    client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
                }
                //string Param = "?CompanyIds='" + CompanyIds + "' &BranchIds='" + BranchIds + "' &DepartmentId='" + DepartmentId + "' &DesignationId='" + DesignationId + "'";
                string uri = _baseurpayroll + "EmployeeCTC/GetAllEmployeePayrollCTCDetails";
                //HTTP POST
                var postTask = client.PostAsJsonAsync(uri, request);
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
    }
    #endregion

    public class ReCaptchaClass
    {
        private static readonly string _Captchakey = ConfigurationManager.AppSettings["Captchakey"];
        public static string Validate(string EncodedResponse)
        {
            var client = new System.Net.WebClient();

            string PrivateKey = _Captchakey;

            var GoogleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret=" + PrivateKey + "&response=" + EncodedResponse));

            var captchaResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ReCaptchaClass>(GoogleReply);

            return captchaResponse.Success.ToLower();
        }

        [JsonProperty("success")]
        public string Success
        {
            get { return m_Success; }
            set { m_Success = value; }
        }

        private string m_Success;
        [JsonProperty("error-codes")]
        public List<string> ErrorCodes
        {
            get { return m_ErrorCodes; }
            set { m_ErrorCodes = value; }
        }


        private List<string> m_ErrorCodes;

    }

}


