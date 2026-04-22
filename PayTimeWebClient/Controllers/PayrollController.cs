using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PayTimeWebClient.Infrastructure;
using OfficeOpenXml;
using System.Configuration;

using System.Text.RegularExpressions;
using System.Data.OleDb;
namespace PayTimeWebClient.Controllers
{

    public class PayrollController : Controller
    {
        //
        // GET: /Payroll/
        static readonly MasterDataRestClient RestClient = new MasterDataRestClient();
        static readonly UtilitiesDataRestClient RestClient1 = new UtilitiesDataRestClient();
        static readonly payrollDataRestClient RestClientpayroll = new payrollDataRestClient();
        static readonly EncryptionHelper EncryptionHelperr = new EncryptionHelper();

        private static readonly HttpClient _httpClient = new HttpClient();

        public ActionResult Index()
        {
            return View();
        }
        //public ActionResult PayrollFeature()
        //{
        //    return View("~/Views/PayRoll/Feature/PayrollFeature.cshtml");
        //}
        //public ActionResult PayrollManagementforFastEmployeeOnboarding()
        //{
        //    return View("~/Views/PayRoll/Feature/PayrollManagementforFastEmployeeOnboarding.cshtml");
        //}
        //public ActionResult PayrollManagementSystemforPowerfulAdministration()
        //{
        //    return View("~/Views/PayRoll/Feature/PayrollManagementSystemforPowerfulAdministration.cshtml");
        //}
        //public ActionResult PayrollSystemMakesEffortlessPayrollProcessing()
        //{
        //    return View("~/Views/PayRoll/Feature/PayrollSystemMakesEffortlessPayrollProcessing.cshtml");
        //}
        //public ActionResult SecuredEmployeeSelfServicePortal()
        //{
        //    return View("~/Views/PayRoll/Feature/SecuredEmployeeSelfServicePortal.cshtml");
        //}
        //public ActionResult HRPayrollEnsureComplianceandCustomeReports()
        //{
        //    return View("~/Views/PayRoll/Feature/HRPayrollEnsureComplianceandCustomeReports.cshtml");
        //}


        #region Payroll process
        [CustAuthFilter]
        [HttpGet]
        public ActionResult SyncProcess()
        {
            try
            {
                if (Session["tokan"] == null)
                {
                    Session.RemoveAll();
                    Session.Abandon();
                    Session["tokan"] = null;
                    return RedirectToAction("Index", "PayTime", false);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [CustAuthFilter]
        [HttpPost]
        public JsonResult SyncProcess(string obj, string Companycode, string greythrtoken, string greythrurl, string greythrdomain, string greythrapi)
        {
            JsonResult result;
            DataTable dt = new DataTable();
            int i = 0;
            string ad = "";
            if (obj != null)
            {
                var t = obj.Replace("[", "").Replace("]", "").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace("}", "").Replace("EmployeeId", "").Replace(":", "");
                var _EmployeeId = t.Split(',');
                dt.Columns.Add("EmpCode");
                foreach (string value in t.Split(','))
                {
                    dt.Rows.Add(value);
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    GetGreyHrtoken(greythrtoken, greythrurl);
                    if (!string.IsNullOrEmpty(Convert.ToString(Session["access_token"])))
                    {
                        i = GetProductAsync(Convert.ToString(Session["access_token"]), dt, greythrapi, greythrdomain);
                        ViewBag.msg = "Sucess";
                    }
                }
                else
                {
                    ViewBag.msg = "Failed";
                    ad = "Failed";
                }
                if (i == 1)
                {
                    ad = "OK";
                    ViewBag.msg = "Sucess";
                }
                else
                {
                    ViewBag.msg = "Failed";
                    ad = "Failed";
                }
            }
            else
            {
                ViewBag.msg = "Please select Employee.";
                ad = "Please select Employee.";
            }
            result = Json(new { ad = ad });
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        private void GetGreyHrtoken(string greythrtoken, string greythrurl)
        {
            try
            {
                //{"EmployeeId":"[\"3\"]","Companycode":"","greythrtoken":"","greythrurl":"","greythrdomain":"","greythrapi":""}
                var strPost = "";
                string _greythraccess_token = "";
                //var uri = "https://mantraminop.greythr.com/uas/v1/oauth2/client-token";
                var uri = greythrurl;
                String result = "";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(uri);
                //objRequest.Headers.Add("Authorization", "Basic QXBpVXNlcjo3MGI4NjJjZC04YmU0LTQ0YTItOTk5Ni0wM2Q2NmUwNzVjNTk=");
                objRequest.Headers.Add("Authorization", greythrtoken);
                StreamWriter myWriter = null;
                objRequest.Method = "POST";
                objRequest.ContentType = "application/x-www-form-urlencoded";

                try
                {
                    myWriter = new StreamWriter(objRequest.GetRequestStream());
                    myWriter.Write(strPost);
                }
                catch (Exception e)
                {
                    //e.Message;
                }
                finally
                {
                    myWriter.Close();
                }

                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                    var jss = new JavaScriptSerializer();
                    var table = jss.Deserialize<dynamic>(result);
                    if (table.ContainsKey("access_token"))
                    {
                        _greythraccess_token = table["access_token"];
                        Session["access_token"] = table["access_token"];
                        //GetProductAsync(_greythraccess_token, dt);
                    }
                    else
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("greythrdesk_Error_Response", "On Get access token :" + table["error"]);
                    }
                    // Close and clean up the StreamReader
                    sr.Close();
                }

            }
            catch (Exception Ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("greythrdesk_Error_Response", "On Get access token :" + Ex.Message + "--" + Ex.InnerException);
            }
        }

        public static int GetProductAsync(string _token, DataTable dt, string greythrapi, string greythrdomain)
        {
            int S = 0;
            string _employeeId = "";
            try
            {
                var accessToken = _token;
                var sts = "";
                var _Empcode = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var strPost = "";
                    string _greythraccess_token = "";
                    try
                    {
                        _Empcode = dt.Rows[i]["Empcode"].ToString();
                        //var uri = "https://api.greythr.com/employee/v2/employees/lookup?q=" + _Empcode + "";
                        var uri = greythrapi + "/employee/v2/employees/lookup?q=" + _Empcode + "";
                        String result = "";
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        WebRequest request = HttpWebRequest.Create(uri);
                        request.Headers.Add("ACCESS-TOKEN", _token);
                        //request.Headers.Add("x-greythr-domain", "mantraminop.greythr.com");
                        request.Headers.Add("x-greythr-domain", greythrdomain);
                        WebResponse response = request.GetResponse();
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        string responseText = reader.ReadToEnd();
                        var jss = new JavaScriptSerializer();
                        var table = jss.Deserialize<dynamic>(responseText);
                        var graytyhrresponse = "";
                        if (table.ContainsKey("employeeId"))
                        {
                            _employeeId = Convert.ToString(table["employeeId"]);
                            if (!string.IsNullOrEmpty(_employeeId))
                            {
                                graytyhrresponse = "Success";
                                if (RestClientpayroll.UpdategreythrEmpID(_Empcode, _employeeId, graytyhrresponse))
                                {
                                    S = 1;
                                    sts = "OK";
                                    _employeeId = "";
                                }
                            }
                        }
                        else
                        {
                            GetDeviceDetails.ProcessLogLogFileWrite("greythrdesk_Error_Response", "On Get access token :" + table["error"]);
                            graytyhrresponse = table["error"];
                            RestClientpayroll.UpdategreythrEmpID(_Empcode, _employeeId, graytyhrresponse);

                        }
                    }
                    catch (Exception Ex)
                    {
                        var graytyhrresponse = "";
                        GetDeviceDetails.ProcessLogLogFileWrite("GetProductAsync", "Sync Employee Time Excetion Empcode:" + _Empcode + ",Message" + Ex.Message);
                        graytyhrresponse = Ex.Message;
                        RestClientpayroll.UpdategreythrEmpID(_Empcode, _employeeId, graytyhrresponse);
                        continue;
                    }
                }
                return S;
            }
            catch (Exception)
            {
                return S;
            }
        }
        [CustAuthFilter]
        [HttpPost]
        public JsonResult Getpayroll(string objcorrection, string Companycode, string greythrtoken, string greythrurl, string greythrdomain, string greythrapi)
        {
            JsonResult result;
            var Emplsterror = "";
            var Emplstsucess = "";
            string ad = "";
            DataTable dt = new DataTable();
            try
            {
                dt = JsonConvert.DeserializeObject<DataTable>(objcorrection);
                //GetGreyHrtoken();
                GetGreyHrtoken(greythrtoken, greythrurl);
                string _token = Convert.ToString(Session["access_token"]);
                var accessToken = _token;
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var _LOP = dt.Rows[i]["LOP"].ToString();
                        var _greythrEmpID = dt.Rows[i]["greythrEmpID"].ToString();
                        var _Empcode = dt.Rows[i]["Empcode"].ToString();
                        var _Fromdate = dt.Rows[i]["Fromdate"].ToString();
                        var _EMPName = dt.Rows[i]["EMPName"].ToString();
                        int InserUpdateFlag = 0;

                        //var uri = "https://api.greythr.com/payroll/v2/employees/" + _greythrEmpID;
                        var uri = greythrapi + "/payroll/v2/employees/" + _greythrEmpID;
                        var handler = new HttpClientHandler();
                        handler.UseCookies = false;
                        int month = 0;
                        int year = 0;
                        if (!string.IsNullOrEmpty(dt.Rows[i]["Fromdate"].ToString()))
                        {
                            DateTime todaysDate = Convert.ToDateTime(dt.Rows[i]["Fromdate"].ToString());
                            month = todaysDate.Month;
                            year = todaysDate.Year;
                        }
                        else
                        {
                            DateTime todaysDate = DateTime.Now;
                            month = todaysDate.Month;
                            year = todaysDate.Year;
                        }
                        DataTable EmployeeCount = new DataTable();
                        using (var httpClient = new HttpClient(handler))
                        {
                            EmployeeCount = RestClientpayroll.GreythrEmppayrollProcessDetailCheck(_greythrEmpID, month, year);
                            int Cout = Convert.ToInt32(EmployeeCount.Rows[0][0]);
                            if (Cout >= 1)
                            {
                                using (var request = new HttpRequestMessage(new HttpMethod("PUT"), uri))
                                {
                                    request.Headers.TryAddWithoutValidation("ACCESS-TOKEN", _token);
                                    //request.Headers.TryAddWithoutValidation("x-greythr-domain", "mantraminop.greythr.com");
                                    request.Headers.TryAddWithoutValidation("x-greythr-domain", greythrdomain);
                                    request.Content = new StringContent("[\n{\n  \"value\":" + _LOP + ",\n  \"fromDate\": \"" + _Fromdate + "\",\n  \"item\":\"LOP\"\n}\n]");
                                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                                    var response = httpClient.SendAsync(request);
                                    var response1 = response.Result;
                                    var Resp = response1.IsSuccessStatusCode;
                                    InserUpdateFlag = 1;
                                    if (response1.IsSuccessStatusCode)
                                    {
                                        var res = response1.ToString();
                                        string text = response1.Content.ToString();
                                        Emplstsucess += _Empcode + ",";
                                        if (RestClientpayroll.UpdateLOPProcessDetails(_Empcode, _greythrEmpID, month, year, _LOP, _EMPName, InserUpdateFlag))
                                        {
                                            ViewBag.msg = "ok";
                                        }
                                    }
                                    else
                                    {
                                        var res = response1.ToString();
                                        string text = response1.Content.ToString();
                                        Emplsterror += _Empcode + ", ";
                                    }
                                }
                            }
                            else
                            {
                                using (var request = new HttpRequestMessage(new HttpMethod("POST"), uri))
                                {
                                    request.Headers.TryAddWithoutValidation("ACCESS-TOKEN", _token);
                                    //request.Headers.TryAddWithoutValidation("x-greythr-domain", "mantraminop.greythr.com");
                                    request.Headers.TryAddWithoutValidation("x-greythr-domain", greythrdomain);
                                    request.Content = new StringContent("[\n{\n  \"value\":" + _LOP + ",\n  \"fromDate\": \"" + _Fromdate + "\",\n  \"item\":\"LOP\"\n}\n]");
                                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                                    var response = httpClient.SendAsync(request);
                                    var response1 = response.Result;
                                    var Resp = response1.IsSuccessStatusCode;
                                    if (response1.IsSuccessStatusCode)
                                    {
                                        var res = response1.ToString();
                                        string text = response1.Content.ToString();
                                        Emplstsucess += _Empcode + ",";
                                        ViewBag.msg = "ok";
                                        if (RestClientpayroll.UpdateLOPProcessDetails(_Empcode, _greythrEmpID, month, year, _LOP, _EMPName, InserUpdateFlag))
                                        {
                                            ViewBag.msg = "ok";
                                        }
                                    }
                                    else
                                    {
                                        var res = response1.ToString();
                                        string text = response1.Content.ToString();
                                        Emplsterror += _Empcode + ", ";
                                    }
                                }
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Emplsterror))
                {
                    if (!string.IsNullOrEmpty(Emplstsucess))
                    {
                        //Emplstsucess = "Sucess Employee Code Payroll Process " + Emplstsucess + " <br/>  Failed  on Employee Payroll Process " + Emplsterror;
                        Emplstsucess = "Success Employee Code Payroll Process " + Emplstsucess + " <br/>";
                        if (!string.IsNullOrEmpty(Emplsterror))
                        {
                            Emplstsucess += "<span style='color:black;'>" + "Fail LOP Process For " + Emplsterror + "</span>";
                        }
                    }
                    else
                    {
                        Emplstsucess = "<span style='color:black;'>" + "Fail LOP Process For " + Emplsterror + "</span>";
                    }

                    //return Emplstsucess;
                }
                else
                {
                    Emplstsucess = "Success LOP process For " + Emplstsucess + " <br/>";
                }
                //ad = "Sucess Employee Code Payroll Process " + Emplstsucess + " <br/>  Failed  on Employee Payroll Process " + Emplsterror;
                result = Json(new { ad = Emplstsucess });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new { ad = Emplstsucess });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        public void Getzohoaccesstoken()
        {

            try
            {
                string _token = Convert.ToString(Session["access_token"]);

                var uri = "https://api.greythr.com/payroll/v2/employees/125";
                //string soap = "soap xml string";
                //params = JSON.parse('{"category": "_CA_campaign_by_union_14904", "start_date": "2016-11-02",  "end_date": "2016-11-02"}')
                var strPost = "value=10&fromDate='2021-01-28'&item='LOP'";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Headers.Add("ACCESS-TOKEN", _token);
                request.Headers.Add("x-greythr-domain", "mantraminop.greythr.com");
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";

                using (Stream stm = request.GetRequestStream())
                {
                    using (StreamWriter stmw = new StreamWriter(stm))
                    {
                        stmw.Write(strPost);
                    }
                }

                using (WebResponse webResponse = request.GetResponse())
                {
                }
                //    //var uri = "https://api.greythr.com/payroll/v2/employees/125";                
                //    //string _token = "sXQgwUXGtecZ3pijCfR7NypoqmQfWvmv6YVyUu-LqQw.gvzPX4TjtkQvGr1coEt1XkJ7gxcTM_mEOr_2AffzwX4";
                //    //var accessToken = _token;
                //    //var strPost = "value=5&fromDate='2020-08-01'&item='LOP'";
                //    //String result = "";                
                //    //HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(uri);
                //    //objRequest.Headers.Add("ACCESS-TOKEN", _token);
                //    //objRequest.Headers.Add("x-greythr-domain", "mantraminop.greythr.com");
                //    //StreamWriter myWriter = null;
                //    //objRequest.Method = "POST";
                //    //objRequest.ContentLength = strPost.Length;
                //    //objRequest.ContentType = "application/x-www-form-urlencoded";

                //    //try
                //    //{
                //    //    myWriter = new StreamWriter(objRequest.GetRequestStream());
                //    //    myWriter.Write(strPost);
                //    //}
                //    //catch (WebException webex)
                //    //{
                //    //    WebResponse errResp = webex.Response;
                //    //    using (Stream respStream = errResp.GetResponseStream())
                //    //    {
                //    //        StreamReader reader = new StreamReader(respStream);
                //    //        string text = reader.ReadToEnd();
                //    //    }
                //    //}
                //    //catch (Exception e)
                //    //{
                //    //    //e.Message;
                //    //}
                //    //finally
                //    //{
                //    //    myWriter.Close();
                //    //}

                //    //HttpWebResponse objResponse1 = (HttpWebResponse)objRequest.GetResponse();                                

            }
            catch (WebException webex)
            {
                WebResponse errResp = webex.Response;
                using (Stream respStream = errResp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(respStream);
                    string text = reader.ReadToEnd();
                }
            }
            catch (Exception Ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("Zohodesk_Error_Response", "On Get access token :" + Ex.Message + "--" + Ex.InnerException);
            }
        }
        #endregion

        #region Payroll Structure
        [CustAuthFilter]
        public ActionResult PayrollStructure()
        {
            ViewBag.Companyslist = RestClient.CompanyGetAll();
            ViewBag.DesignationList = FillDesignation();
            return View();
        }
        [CustAuthFilter]
        public ActionResult PfCalculator()
        {
            return View();
        }
        [CustAuthFilter]
        [HttpGet]
        public ActionResult Headslist()
        {
            return View();
        }

        [CustAuthFilter]
        public ActionResult PayrollDynamicColumn()
        {
            return View();
        }

        [CustAuthFilter]
        public ActionResult AttendanceFinalize()
        {
            ViewBag.Companyslist = RestClient.CompanyGetAll();
            ViewBag.DesignationList = FillDesignation();
            return View();
        }

        [CustAuthFilter]
        public ActionResult PayrollGenerate()
        {
            return View();
        }

        [CustAuthFilter]
        public ActionResult AccountPayroll()
        {
            ViewBag.Companyslist = RestClient.CompanyGetAll();
            ViewBag.DesignationList = FillDesignation();
            return View();
        }

        [CustAuthFilter]
        public ActionResult approval()
        {
            ViewBag.Companyslist = RestClient.CompanyGetAll();
            ViewBag.DesignationList = FillDesignation();
            return View();
        }

        [CustAuthFilter]
        public ActionResult exportsheetbank()
        {
            return View();
        }

        [CustAuthFilter]
        public ActionResult EmployeeCTC()
        {
            ViewBag.DesignationList = FillDesignation();
            return View();
        }

        [CustAuthFilter]
        [HttpGet]
        public JsonResult EmployeeCTCEncrypt(string EmployeeCTC)
        {
            JsonResult result;
            var EncryptCTC = EncryptionHelperr.Encrypt(EmployeeCTC);
            //return EncryptCTC;
            result = Json(new { EncryptCTC = EncryptCTC });
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        [CustAuthFilter]
        [HttpPost]


        public JsonResult EmployeeCTCDecrypt(string EmployeeCTC)
        {
            JsonResult result;
            var EncryptCTC = EncryptionHelperr.Decrypt(EmployeeCTC);
            result = Json(new { EncryptCTC = EncryptCTC });
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        [CustAuthFilter]
        public ActionResult NonRecurringIncome()
        {
            return View();
        }

        [CustAuthFilter]
        public ActionResult PaySlipView()
        {
            return View();
        }

        [CustAuthFilter]
        public ActionResult PayslipTemplate()
        {
            return View();
        }

        [CustAuthFilter]
        public ActionResult PayslipExport()
        {
            return View();
        }

        [CustAuthFilter]
        public ActionResult EsicMaster()
        {
            return View();
        }

        [CustAuthFilter]
        public ActionResult TaxMaster()
        {
            return View();
        }

        [CustAuthFilter]
        public ActionResult PayrollCycle()
        {
            return View();
        }

        //Fasttrack salarydistrubution
        [CustAuthFilter]
        public ActionResult PayrollFasttrackDisbursement()
        {
            ViewBag.DesignationList = FillDesignation();
            return View("Fasttrack/PayrollFasttrackDisbursement");
        }

        public List<SelectListItem> FillDesignation()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            Designations ds = new Designations();
            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
            {
                ds.Designationlist = RestClient.DesignationsGetAll();
            }
            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                if (Session["RoleId"].ToString() == "6806")
                {
                    ds.Designationlist = RestClient.DesignationsGetAll().Where(x => (x.CompanyID == cmpid && (x.BranchID == BranchID || x.BranchID == 0)) || (x.CompanyID == 0)).ToList();
                }
                else
                {
                    ds.Designationlist = RestClient.DesignationsGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0).ToList();
                }
            }
            foreach (var i in ds.Designationlist)
            {
                list.Add(new SelectListItem() { Text = i.DesignationName, Value = Convert.ToString(i.DesignationId) });
            }
            return list;
        }

        [CustAuthFilter]
        public ActionResult PayrollConfiguration()
        {
            RoleRightsMapping model = new RoleRightsMapping();
            var rolelist = RestClient.RoleGetAll();
            ViewBag.RoleList = rolelist.Where(x => x.RoleName.ToLower() != "admin");
            return View(model);
        }

        [CustAuthFilter]
        public ActionResult Removeprocessedpayrolldata()
        {
            return View();
        }

        #endregion

        //[HttpPost]
        //public DataTable GetEmployeeCTCDetails(DataTableWebPunchAjaxPostModel request, string CompanyIds, string BranchIds, string? DepartmentId, string? DesignationId) 
        //{    
        //     string _url = System.Configuration.ConfigurationManager.AppSettings["webapipaytime"];
        //     string uri = _url + "EmployeeCTCC/GetAllEmployeePayrollCTCDetails";
        //    using (HttpClient client = new HttpClient())
        //    {
        //        if (!string.IsNullOrEmpty(HttpContext.Current.Session["tokan"].ToString()))
        //        {
        //            client.DefaultRequestHeaders.Add("Authorization", HttpContext.Current.Session["tokan"].ToString());
        //        }
        //        JsonResult response = client.GetAsync(uri);
        //        var res = response.Result.Content.ReadAsStringAsync().Result;
        //        var cmp = JsonConvert.DeserializeObjectAsync<List<Holidays>>(res.ToString()).Result;
        //        return cmp;
        //    }


        //}



        //[HttpPost]
        //public JsonResult GetEmployeeCTCDetails(DataTableFilterCTCAjaxPostModelandFilterData1 request)
        //{
        //    try
        //    {
        //        // Call the repository method to get the data
        //        var response = RestClientpayroll.getemployeectcdataTable(request);

        //        // Cast the response to a dynamic object to access its properties
        //        var dynamicResponse = response as dynamic;

        //        // Prepare the response object for the AJAX DataTable
        //        var returnResponse = new
        //        {
        //            draw = dynamicResponse.draw,
        //            recordsTotal = dynamicResponse.recordsTotal,
        //            recordsFiltered = dynamicResponse.recordsFiltered,
        //            data = dynamicResponse.data,
        //            StatusCode = "200"
        //        };

        //        // Return the response as JsonResult
        //        return Json(returnResponse, JsonRequestBehavior.AllowGet);
        //        //return returnResponse;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Prepare an error response object
        //        var errorResponse = new
        //        {
        //            draw = request.draw,
        //            recordsTotal = 0,
        //            recordsFiltered = 0,
        //            data = new JArray(), // Empty array in case of error
        //            StatusCode = "505",
        //            message = ex.Message
        //        };

        //        // Return the error response as JsonResult
        //        return Json(errorResponse, JsonRequestBehavior.AllowGet);
        //    }
        //}




        //[HttpPost]
        //public JsonResult GetEmployeeCTCDetails(DataTableFilterCTCAjaxPostModelandFilterData1 request)
        //{
        //    var response = RestClientpayroll.getemployeectcdataTable(request);

        //    var responseContent = response;

        //    JsonResult data;
        //    data = Json(new { data = responseContent });
        //    data.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //    return data;
        //    //dt = JsonConvert.DeserializeObject<DataTable>(responseContent);
        //    //return responseContent;               

        //}

        [HttpPost]
        public JsonResult GetAllEmployeePayrollCTCDetails(DataTableFilterCTCAjaxPostModelandFilterData request)
        {
            // Fetch the DataTable from the API
            DataTable dataTable = RestClientpayroll.getemployeectcdataTable(request);

            // Convert DataTable to a list of dictionary objects for easier JSON serialization
            var data = dataTable.AsEnumerable().Select(row => dataTable.Columns
                .Cast<DataColumn>()
                .ToDictionary(column => column.ColumnName, column => row[column])
            ).ToList();

            // Return the data as a JSON response
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }


        #region ESIC Challan
        [CustAuthFilter]
        public ActionResult ESICChallan()
        {
            return View();
        }
        #endregion


        #region ImportExcel
        [CustAuthFilter]
        [HttpPost]
        public ActionResult Headslist(HttpPostedFileBase file)
        {
            try
            {
                var regexItem = new Regex("^[a-zA-Z0-9]*$");
                var regexItem1 = new Regex("^[0-9!@#$%^&*()-_=+\\[\\]{};:'\",.<>/?]*$");
                var regexItem2 = new Regex("[^a-zA-Z]");
                if (file == null)
                {
                    ViewBag.error = "Please Select File.";
                }
                if (file != null && file.ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(file.FileName);
                    if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".csv")
                    {
                        string fileLocation = Server.MapPath("~/Content/") + file.FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }
                        file.SaveAs(fileLocation);
                        string excelConnectionString = string.Empty;

                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        else if (fileExtension == ".xlsx")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        else if (fileExtension == ".csv")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }

                        // Create Connection to Excel workbook and add the OleDb namespace
                        using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
                        {
                            excelConnection.Open();
                            DataTable dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                            if (dt == null)
                            {
                                return null;
                            }

                            string[] excelSheets = new string[dt.Rows.Count];
                            int t = 0;

                            // Excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }

                            string query = string.Format("SELECT * FROM [{0}]", excelSheets[0]);
                            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
                            {
                                DataSet ds = new DataSet();
                                dataAdapter.Fill(ds);
                                if (ds.Tables.Count > 0)
                                {
                                    #region Add Excel data to a list
                                    DataTable dataTable = ds.Tables[0];
                                    HashSet<string> uniqueHeadTitles = new HashSet<string>();
                                    List<ImportedDataExcel> items = new List<ImportedDataExcel>();
                                    List<ImportedDataExcel> duplicateItems = new List<ImportedDataExcel>();
                                    Dictionary<int, int> frequencyMap = new Dictionary<int, int>();
                                    if (dataTable.Rows.Count > 0)
                                    {
                                        try
                                        {
                                            foreach (DataRow row in dataTable.Rows)
                                            {
                                                string headTitle = row["HeadTitle"].ToString();
                                                string NameInSalarySlip = row["NameInSalarySlip"].ToString();
                                                object DHeadTypeObject = row["HeadType"];
                                                if (DHeadTypeObject == null || DHeadTypeObject == DBNull.Value)
                                                {
                                                    ViewBag.error = "HeadType cannot be null.";
                                                    return View();
                                                }
                                                string DHeadTypeStr = row["HeadType"].ToString();
                                                if (!Regex.IsMatch(DHeadTypeStr, @"^\d+$"))
                                                {
                                                    ViewBag.error = "HeadType should not contain special characters";
                                                    return View();
                                                }

                                                int DHeadType = Convert.ToInt32(row["HeadType"]);
                                                if (DHeadType > 3 || DHeadType == 0)
                                                {
                                                    ViewBag.error = "HeadType Format does not exist.";
                                                    return View();
                                                }
                                                if (headTitle == "" || headTitle == null)
                                                {
                                                    ViewBag.error = "HeadTitle should not contain special characters, numbers or spaces.";
                                                    return View();
                                                }
                                                if (regexItem2.IsMatch(headTitle))
                                                {
                                                    if (regexItem1.IsMatch(headTitle) || headTitle == "")
                                                    {
                                                        ViewBag.error = "HeadTitle should not contain special characters, numbers or spaces.";
                                                        return View();
                                                    }
                                                }
                                                if (regexItem2.IsMatch(NameInSalarySlip))
                                                {
                                                    if (regexItem1.IsMatch(NameInSalarySlip) || headTitle == "")
                                                    {
                                                        ViewBag.error = "NameInSalarySlip should not contain special characters or spaces.";
                                                        return View();
                                                    }
                                                }
                                                if (NameInSalarySlip == "" || NameInSalarySlip == null)
                                                {
                                                    ViewBag.error = "NameInSalarySlip should not contain special characters or spaces.";
                                                    return View();
                                                }

                                                if (uniqueHeadTitles.Contains(headTitle))
                                                {
                                                    ImportedDataExcel duplicateItem = new ImportedDataExcel
                                                    {
                                                        HeadType = Convert.ToInt32(row["HeadType"]),
                                                        HeadTitle = headTitle,
                                                        NameInSalarySlip = row["NameInSalarySlip"].ToString(),
                                                        CustomFormula = "",
                                                    };

                                                    duplicateItems.Add(duplicateItem); // Add to the list of duplicates
                                                }
                                                else
                                                {
                                                    if (!Regex.IsMatch(DHeadTypeStr, @"^\d+$"))
                                                    {
                                                        ViewBag.error = "HeadType Format does not exist.";
                                                        return View();
                                                    }
                                                    if (DHeadTypeObject == null || DHeadTypeObject == DBNull.Value)
                                                    {
                                                        ViewBag.error = "HeadType cannot be null.";
                                                        return View();
                                                    }
                                                    int HeadType = Convert.ToInt32(row["HeadType"]);
                                                    if (HeadType > 3)
                                                    {
                                                        ViewBag.error = "HeadType Format does not exist";
                                                        return View();
                                                    }
                                                    if (headTitle == "" || headTitle == null)
                                                    {
                                                        ViewBag.error = "HeadTitle should not contain special characters, numbers or spaces.";
                                                        return View();
                                                    }
                                                    if (regexItem2.IsMatch(headTitle))
                                                    {
                                                        if (regexItem1.IsMatch(headTitle) || headTitle == "")
                                                        {
                                                            ViewBag.error = "HeadTitle should not contain special characters, numbers or spaces.";
                                                            return View();
                                                        }
                                                    }
                                                    if (regexItem2.IsMatch(NameInSalarySlip))
                                                    {
                                                        if (regexItem1.IsMatch(NameInSalarySlip) || headTitle == "")
                                                        {
                                                            ViewBag.error = "NameInSalarySlip should not contain special characters or spaces.";
                                                            return View();
                                                        }
                                                    }
                                                    if (NameInSalarySlip == "" || NameInSalarySlip == null)
                                                    {
                                                        ViewBag.error = "NameInSalarySlip should not contain special characters or spaces.";
                                                        return View();
                                                    }
                                                    uniqueHeadTitles.Add(headTitle); // Add to the set of unique HeadTitles
                                                    ImportedDataExcel itemModel = new ImportedDataExcel
                                                    {
                                                        HeadType = Convert.ToInt32(row["HeadType"]),
                                                        //HeadType = Convert.ToInt32(row["HeadType"]),
                                                        HeadTitle = headTitle,
                                                        NameInSalarySlip = row["NameInSalarySlip"].ToString(),
                                                        CustomFormula = "",
                                                    };

                                                    items.Add(itemModel); // Add to the main list
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ViewBag.error = "Invalid file. Please import proper format file.";
                                            //ViewBag.error = "Invalid data format: " + ex.Message;
                                            return View("Headslist");
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.error = "Imported Excel values cannot be null.";
                                        return View("Headslist");
                                    }
                                    #endregion
                                    Exceptionlog.LogFileWrite("Import Head items:" + items.Count);
                                    ApiResponse tempresult = RestClient.ImportData(items);
                                    #region Download excel of duplicate data
                                    if (tempresult.Importfailloglist.ToList().Count > 0)
                                    {
                                        // Create a list to store duplicate row data
                                        List<duplicateitems> duplicateRows = new List<duplicateitems>();
                                        foreach (var value in tempresult.Importfailloglist)
                                        {
                                            var ValidateSuccess = value.issuccess;
                                            var headname = value.head_name;

                                            if (!ValidateSuccess) // Only process failed rows for duplicates
                                            {
                                                if (tempresult.ResponseCode == 203)
                                                {
                                                    ViewBag.error = tempresult.Message;
                                                    return View("Headslist");
                                                }
                                                var originalItem = items.Find(item => item.HeadTitle == headname);
                                                if (originalItem != null)
                                                {
                                                    int DHeadType = originalItem.HeadType;
                                                    string headTitle = originalItem.HeadTitle;
                                                    string NameInSalarySlip = originalItem.NameInSalarySlip;
                                                    object DHeadTypeObject = originalItem.HeadType;
                                                    if (DHeadTypeObject == null || DHeadTypeObject == DBNull.Value)
                                                    {
                                                        ViewBag.error = "HeadType cannot be null.";
                                                        return View();
                                                    }
                                                    if (DHeadType > 3)
                                                    {
                                                        ViewBag.error = "HeadType Format does not exist.";
                                                        return View();
                                                    }
                                                    if (headTitle == "" || headTitle == null)
                                                    {
                                                        ViewBag.error = "HeadTitle should not contain special characters, numbers or spaces.";
                                                        return View();
                                                    }
                                                    if (regexItem2.IsMatch(headTitle))
                                                    {
                                                        if (regexItem1.IsMatch(headTitle) || headTitle == "")
                                                        {
                                                            ViewBag.error = "HeadTitle should not contain special characters, numbers or spaces.";
                                                            return View();
                                                        }
                                                    }
                                                    if (regexItem2.IsMatch(NameInSalarySlip))
                                                    {
                                                        if (regexItem1.IsMatch(NameInSalarySlip) || headTitle == "")
                                                        {
                                                            ViewBag.error = "NameInSalarySlip should not contain special characters or spaces.";
                                                            return View();
                                                        }
                                                    }
                                                    if (NameInSalarySlip == "" || NameInSalarySlip == null)
                                                    {
                                                        ViewBag.error = "NameInSalarySlip should not contain special characters or spaces.";
                                                        return View();
                                                    }
                                                    // Create a duplicate row and add it to the list
                                                    var duplicateRow = new duplicateitems
                                                    {
                                                        HeadType = originalItem.HeadType,
                                                        HeadTitle = originalItem.HeadTitle,
                                                        NameInSalarySlip = originalItem.NameInSalarySlip,
                                                    };
                                                    duplicateRows.Add(duplicateRow);
                                                }
                                                if (duplicateRows.Count > 0)
                                                {
                                                    ViewBag.error = "(" + duplicateRows.Count + " Duplicate) rows found . If you want to download that duplicates data, then click on Download Button.";
                                                    TempData["ErrorMsg"] = "(" + duplicateRows.Count + " Duplicate) rows found . If you want to download that duplicates data, then click on Download Button.";
                                                    TempData["DuplicateRows"] = duplicateRows;
                                                }
                                            }
                                            else
                                            {
                                                if (uniqueHeadTitles.Count > 0)
                                                {
                                                    ViewBag.msg = "File is Validated and Imported (" + uniqueHeadTitles.Count + " rows) Successfully";
                                                    TempData["SuccessMsg"] = "File is Validated and Imported (" + uniqueHeadTitles.Count + " rows) Successfully";
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                    //ViewBag.ExcelDataList = items;
                                }
                            }
                        }
                    }
                    else
                    {
                        ViewBag.error = "Please Upload Files in .xls, .xlsx or .csv format";
                    }
                }
                return View();
            }
            catch (Exception Ex)
            {
                ViewBag.error = "The imported file is in a bad format: " + Ex.Message;
                return View();
            }
        }

        [CustAuthFilter]
        [HttpGet]
        public ActionResult DownloadExcel()
        {
            try
            {
                List<duplicateitems> duplicateRows = TempData["DuplicateRows"] as List<duplicateitems>;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("DuplicateRows");
                    worksheet.Cells[1, 1].Value = "HeadType";
                    worksheet.Cells[1, 2].Value = "HeadTitle";
                    worksheet.Cells[1, 3].Value = "NameInSalarySlip";
                    int row = 2;
                    foreach (var row1 in duplicateRows)
                    {
                        worksheet.Cells[row, 1].Value = row1.HeadType;
                        worksheet.Cells[row, 2].Value = row1.HeadTitle;
                        worksheet.Cells[row, 3].Value = row1.NameInSalarySlip;
                        row++;
                    }
                    // Save the Excel package to a stream
                    MemoryStream stream = new MemoryStream();
                    package.SaveAs(stream);

                    // Provide the Excel file for download
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment; filename=duplicate_rows.xlsx");
                    Response.BinaryWrite(stream.ToArray());
                    return Content("Excel file downloaded successfully.");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception here, you can log it or provide a user-friendly error message.
                TempData["ExErrorMsg"] = "An error occurred: " + ex.Message;
                return RedirectToAction("Headslist", "PayRoll");
            }
        }
        #endregion

        #region PfMaster
        [CustAuthFilter]
        public ActionResult PfMaster()
        {
            return View();
        }
        #endregion

        #region Payroll Report
        [CustAuthFilter]
        public ActionResult PayrollReport()
        {
            ViewBag.Companyslist = RestClient.CompanyGetAll();
            ViewBag.DesignationList = FillDesignation();
            return View();
        }
        #endregion

        #region PF Challan
        [CustAuthFilter]
        public ActionResult PfChallan()
        {
            return View();
        }
        #endregion

        #region Approval
        [CustAuthFilter]
        public ActionResult PayrollApproval()
        {
            ViewBag.Companyslist = RestClient.CompanyGetAll();
            ViewBag.DesignationList = FillDesignation();
            return View();
        }
        #endregion

        #region Increment Planning
        [CustAuthFilter]
        public ActionResult IncrementPlanningPage()
        {
            ViewBag.DesignationList = FillDesignation();
            return View();
        }
        #endregion

        #region Increment Approval
        [CustAuthFilter]
        public ActionResult IncrementApproval()
        {
            ViewBag.DesignationList = FillDesignation();
            return View();
        }
        #endregion

        #region F&F Settlement
        [CustAuthFilter]
        public ActionResult WorkFlow()
        {
            ViewBag.DesignationList = FillDesignation();
            return View();
        }

        [CustAuthFilter]
        public ActionResult ClearanceRequest()
        {
            ViewBag.DesignationList = FillDesignation();
            return View();
        }

        [CustAuthFilter]
        public ActionResult ExitEmployeeRequest()
        {
            ViewBag.DesignationList = FillDesignation();
            return View();
        }
        #endregion

        [CustAuthFilter]
        public ActionResult AdvanceSalaryApproval()
        {
            ViewBag.DesignationList = FillDesignation();
            return View();
        }

        [CustAuthFilter]
        public ActionResult AdvanceSalaryPolicy()
        {
            ViewBag.DesignationList = FillDesignation();
            return View();
        }
    }
}
