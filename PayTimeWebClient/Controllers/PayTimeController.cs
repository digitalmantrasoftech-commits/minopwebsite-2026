using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using PayTimeWebClient.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using CaptchaMvc.HtmlHelpers;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.UI;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using System.Net.Http;
using System.Threading.Tasks;
// using Razorpay.Api; // Replaced with HDFC SmartGateway
using System.Net.Http.Headers;
using CaptchaMvc;
using System.Drawing;
using System.Drawing.Imaging;
//using iTextSharp.text.pdf;
//using HiQPdf;


namespace PayTimeWebClient.Controllers
{
    //[CustAuthFilter]
    public class PayTimeController : MyBaseController
    {
        #region Declaration
        static readonly IAccountRestClint RestAccout = new AccountRestClint();
        static readonly MasterDataRestClient RestClient = new MasterDataRestClient();
        static readonly TransactionDataRestClient TransactionRestClient = new TransactionDataRestClient();
        static readonly AlertDataRestClient accrestclient = new AlertDataRestClient();
        Paymentintegrationclient payclint = new Paymentintegrationclient();
        MRespo resp = new MRespo();

        // Session cache keyed by companyId (tenantId) - no collision as each tenant is unique
        // Stores Dictionary of session variables to restore full session after HDFC callback
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Dictionary<string, string>> _sessionCache =
            new System.Collections.Concurrent.ConcurrentDictionary<string, Dictionary<string, string>>();

        // SECURITY: Cache for expected payment amounts - used to verify in PaymentStatus callback
        // Key: PaymentID (internal), Value: Expected amount
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, double> _expectedAmountCache =
            new System.Collections.Concurrent.ConcurrentDictionary<string, double>();

        /// <summary>
        /// Stores expected amount for a payment to verify later in callback
        /// </summary>
        private void StoreExpectedAmount(string paymentId, double expectedAmount)
        {
            if (!string.IsNullOrEmpty(paymentId))
            {
                _expectedAmountCache[paymentId] = expectedAmount;
                GetDeviceDetails.ProcessLogLogFileWrite("ExpectedAmountStored",
                    $"PaymentId: {paymentId}, Amount: {expectedAmount}");
            }
        }

        /// <summary>
        /// Retrieves and removes expected amount for verification (one-time use)
        /// </summary>
        private double? GetAndRemoveExpectedAmount(string paymentId)
        {
            if (!string.IsNullOrEmpty(paymentId) && _expectedAmountCache.TryRemove(paymentId, out double amount))
            {
                return amount;
            }
            return null;
        }

        /// <summary>
        /// Creates simple session key for udf1 (short, no encryption needed)
        /// Format: paymentId_userId_companyId (underscore separator - HDFC strips dashes)
        /// </summary>
        private string CreateSessionKey(string paymentId, int userId, int companyId)
        {
            // Use underscore as separator - HDFC strips dashes from udf1
            return $"{paymentId}_{userId}_{companyId}";
        }

        /// <summary>
        /// Stores ALL session variables in cache using companyId as key
        /// Called at login time when all session variables are populated
        /// </summary>
        private void StoreSessionForCompany(string companyId)
        {
            if (!string.IsNullOrEmpty(companyId) && Session != null)
            {
                var sessionData = new Dictionary<string, string>();
                foreach (string key in Session.Keys)
                {
                    if (Session[key] != null)
                    {
                        sessionData[key] = Session[key].ToString();
                    }
                }
                _sessionCache[companyId] = sessionData;
                GetDeviceDetails.ProcessLogLogFileWrite("SessionStored", "Stored " + sessionData.Count + " session variables for CompanyId: " + companyId);
            }
        }

        /// <summary>
        /// Retrieves session data from cache using companyId
        /// Uses TryGetValue (not TryRemove) to keep session for subsequent payments
        /// </summary>
        private Dictionary<string, string> GetSessionForCompany(string companyId)
        {
            if (!string.IsNullOrEmpty(companyId) && _sessionCache.TryGetValue(companyId, out Dictionary<string, string> sessionData))
            {
                return sessionData;
            }
            return null;
        }

        /// <summary>
        /// Clears session cache for a company (call on logout)
        /// Public static so it can be called from AccountController and DealerController
        /// </summary>
        public static void ClearSessionCacheForCompany(string companyId)
        {
            if (!string.IsNullOrEmpty(companyId))
            {
                _sessionCache.TryRemove(companyId, out _);
            }
        }

        /// <summary>
        /// Parses session key from udf1
        /// </summary>
        private bool ParseSessionKey(string sessionKey, out string paymentId, out int userId, out int companyId)
        {
            paymentId = ""; userId = 0; companyId = 0;
            try
            {
                if (string.IsNullOrEmpty(sessionKey)) return false;

                // Use underscore as separator (HDFC strips dashes)
                string[] parts = sessionKey.Split('_');
                if (parts.Length >= 3)
                {
                    paymentId = parts[0];
                    userId = int.TryParse(parts[1], out int uid) ? uid : 0;
                    companyId = int.TryParse(parts[2], out int cid) ? cid : 0;
                    return userId > 0 && companyId > 0;
                }
            }
            catch (Exception ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("ParseSessionKeyError", ex.Message);
            }
            return false;
        }

        List<string> sqlinjectionsearchList = new List<string>();



        private static readonly string _leadurl = ConfigurationManager.AppSettings["LeadUrl"];
        //private static readonly string _locattchments = ConfigurationManager.AppSettings["freshdeslocattchments"];
        //private static readonly string _freshdeskapiurl = ConfigurationManager.AppSettings["freshdeskapiurl"];
        private static readonly string _Zohodeskapiurl = ConfigurationManager.AppSettings["zohodeskapiurl"];
        private static readonly string _zohodeskdepartmentId = ConfigurationManager.AppSettings["zohodeskdepartmentId"];
        private static readonly string _zohoorgId = ConfigurationManager.AppSettings["zohoorgId"];


        private static readonly string _zohodeskapiurlrefreshtoken = ConfigurationManager.AppSettings["zohodeskapiurlrefreshtoken"];
        private static readonly string _refresh_token = ConfigurationManager.AppSettings["zohorefresh_token"];
        private static readonly string _client_id = ConfigurationManager.AppSettings["zohoclient_id"];
        private static readonly string _client_secret = ConfigurationManager.AppSettings["zohoclient_secret"];
        private static readonly string _scope = ConfigurationManager.AppSettings["zohoscope"];
        private static readonly string _redirect_uri = ConfigurationManager.AppSettings["zohoredirect_uri"];
        private static readonly string _grant_type = ConfigurationManager.AppSettings["zohogrant_type"];
        private static readonly string _access_type = ConfigurationManager.AppSettings["zohoaccess_type"];
        private static string _zohoaccess_token = string.Empty;

        //ZohoCRM integration 06042023
        private static string _zohocrmaccess_token = string.Empty;
        private static readonly string _zohocrmapiurlrefreshtoken = ConfigurationManager.AppSettings["zohodeskapiurlrefreshtoken"];
        private static readonly string _zohocrmrefresh_token = ConfigurationManager.AppSettings["zohocrmrefresh_token"];
        private static readonly string _zohocrmclient_id = ConfigurationManager.AppSettings["zohocrmclient_id"];
        private static readonly string _zohocrmclient_secret = ConfigurationManager.AppSettings["zohocrmclient_secret"];
        private static readonly string _zohocrmscope = ConfigurationManager.AppSettings["zohocrmscope"];
        private static readonly string _zohocrmredirect_uri = ConfigurationManager.AppSettings["zohocrmredirect_uri"];
        private static readonly string _zohocrmgrant_type = ConfigurationManager.AppSettings["zohocrmgrant_type"];
        private static readonly string _zohocrmaccess_type = ConfigurationManager.AppSettings["zohocrmaccess_type"];
        private static readonly string _zohocrmapiurl = ConfigurationManager.AppSettings["zohocrmapiurl"];


        // HDFC SmartGateway Configuration
        string HDFC_GATEWAY_URL = ConfigurationManager.AppSettings["hdfc_gateway_url"];
        string HDFC_MERCHANT_ID = ConfigurationManager.AppSettings["hdfc_merchant_id"];
        string HDFC_AUTH_TOKEN = ConfigurationManager.AppSettings["hdfc_auth_token"];
        string HDFC_PAYMENT_PAGE_CLIENT_ID = ConfigurationManager.AppSettings["hdfc_payment_page_client_id"];
        string HDFC_RETURN_URL = ConfigurationManager.AppSettings["hdfc_return_url"];
        #endregion
        #region ActiveUser
        public ActionResult ActiveUser()
        {
            try
            {
                var ss = Request.QueryString["ed"].ToString();
                string ActivationCode = Request.QueryString["ed"].ToString();
                if (!string.IsNullOrEmpty(ActivationCode))
                {
                    resp = AccountRestClint.ActivateUser(ActivationCode);
                    string domain = resp.Meg.Split('-')[1];
                    string message = resp.Meg.Split('-')[0];
                    ViewBag.MegSts = resp.MegSts;
                    if (resp.MegSts == "ok")
                    {
                        ViewBag.Meg = message;
                        ViewBag.Url = domain;
                    }
                    else if (resp.MegSts == "activated")
                    {
                        ViewBag.Meg = "User Already Activated.";
                        ViewBag.Url = domain;
                    }
                    else
                    {
                        ViewBag.Meg = "Error in activation, please contact admin";
                        ViewBag.Url = domain;
                    }

                }
                else
                {
                    ViewBag.MegSts = "Error";

                }
            }
            catch (Exception Ex)
            {

                TempData["meg"] = Ex.InnerException;

            }
            return View();
        }
        #endregion

        public ActionResult Timeout()
        {
            //return View("Index");
            return Json(new
            {
                redirectUrl = "/PayTime/Index",
                isRedirect = true
            }, JsonRequestBehavior.AllowGet);

        }

        #region Index
        [PayTimeWebClient.App_Start.TenantActionFilter]
        public ActionResult Index()
        {
            var domain = this.RouteData.Values["tenant"];
            //var domain = "https://app.minopcloud.com/";
            GetDeviceDetails.ProcessLogLogFileWrite("domain", domain.ToString());
            if (domain.ToString().ToLower() != "app")
            {

                if (domain.ToString() != "Default")
                {
                    if (domain.ToString() == "developers")
                    {
                        return View("DevelopersAccount");
                    }
                    if (domain.ToString().ToLower() == "dealer")
                    {
                        return RedirectToAction("Login", "Dealer");
                    }
                    if (domain.ToString().ToLower() == "app")
                    {
                        return RedirectToAction("Loginpage", "Paytime");
                    }
                    //find the domain in registered companies if you get domain redirect it to custom view page
                    //var res = RestClient.CheckDomain(domain.ToString());
                    //if (res.Count() > 0)
                    //{
                    //    ViewBag.CompanyName = res.First().CompanyName;
                    //    ViewBag.CompanyCode = res.First().CompanyCode;
                    //    TempData["CompanyCode"] = res.First().CompanyCode;
                    //    return View("CustomIndex");
                    //}
                    LoginModel model = new LoginModel();
                    if (Request.Cookies["Login"] != null)
                    {
                        ViewBag.UserEmail = Request.Cookies["Login"].Values["UserEmail"];
                        ViewBag.Password = Request.Cookies["Login"].Values["Password"];
                        ViewBag.rememberme = "true";
                    }
                    else if (Request.Cookies["EmpLogin"] != null)
                    {
                        ViewBag.companycode = Request.Cookies["EmpLogin"].Values["CompanyCode"];
                        ViewBag.empUserEmail = Request.Cookies["EmpLogin"].Values["EmpUserEmail"];
                        ViewBag.empPassword = Request.Cookies["EmpLogin"].Values["EmpPassword"];
                        ViewBag.emprememberme = "true";
                    }
                }
            }
            else
            {
                return RedirectToAction("Loginpage", "Paytime");
            }


            return View();
        }
        [HttpPost]
        public ActionResult Index(LoginModel model)
        {
            ModelState.Remove("CompanyCode");

            if (ModelState.IsValid)
            {
                resp = RestAccout.Register(model);
                return View("LoginPage");

            }
            else
            {
                ModelState.AddModelError(resp.MegSts, resp.Meg);
            }
            return View("LoginPage");
        }
        [HttpPost]
        public JsonResult ChangeLanguage(string lang)
        {
            JsonResult result;
            new SiteLanguages().SetLanguage(lang);
            //return RedirectToAction("Index", "Paytime");
            //return View();
            result = Json(new SelectList("", "0"));
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        #endregion

        #region Login
        [HttpPost]
        public ActionResult Login(LoginModel model, string rememberme)
        {

            ModelState.Remove("CompanyName");
            ModelState.Remove("CompanyCode");
            try
            {
                if (ModelState.IsValid)
                {
                    resp = RestAccout.Login(model);
                    if (resp.MegSts == "jwt_token")
                    {
                        //FormsAuthentication.SetAuthCookie(model.UserEmail, true);
                        Session["tokan"] = resp.Meg;
                        string payload = AuthHandler.getPayload(Session["tokan"].ToString(), ConfigurationManager.AppSettings["jwtKey"].ToString(), false);
                        JObject jObj = JObject.Parse(payload);
                        Session["RoleId"] = Convert.ToInt32(jObj["RoleId"]);

                        if (rememberme == "true")
                        {
                            HttpCookie cookie = new HttpCookie("Login");
                            cookie.Values.Add("UserEmail", model.UserEmail);
                            cookie.Values.Add("Password", model.Password);
                            cookie.Expires = DateTime.Now.AddDays(15);
                            Response.Cookies.Add(cookie);
                        }
                        else
                        {
                            if (Request.Cookies["Login"] != null)
                            {
                                var c = new HttpCookie("Login");
                                c.Expires = DateTime.Now.AddDays(-1);
                                Response.Cookies.Add(c);
                            }
                        }

                        if (Convert.ToInt32(Session["RoleId"]) != 15 && Convert.ToInt32(Session["RoleId"]) != 16) //21
                        {
                            Session["cmpname"] = jObj["cmpname"].ToString();
                            Session["UserEmail"] = jObj["name"].ToString();
                            Session["photo"] = jObj["photo"].ToString();
                            Session["UserId"] = jObj["UserId"].ToString();
                            string strkey = jObj["keystr"].ToString();
                            string systemWizarFlag = jObj["IsSetting"].ToString();
                            Session["cmpcode"] = strkey.Split('|')[1].ToString();
                            Session["ClientCompanyId"] = jObj["ClientCompanyId"].ToString();
                            Session["CompanyId"] = jObj["compId"].ToString();
                            Session.Timeout = 10;
                            Session["IsSetting"] = jObj["IsSetting"].ToString();
                            Session["FirstEmpId"] = jObj["FirstEmpId"].ToString();
                            Session["EmpId"] = Convert.ToInt32(jObj["EmpId"]);
                            Session["domain"] = strkey.Split('|')[3].ToString();
                            Session["DbName"] = strkey.Split('|')[4].ToString();
                            Session["IsSchool"] = jObj["IsSchool"].ToString();
                            Session["IsOffice"] = jObj["IsOffice"].ToString();
                            Session["IsDateFormat"] = jObj["IsDateFormat"].ToString();
                            Session["IsApprovalForWebpunch"] = jObj["IsApprovalForWebpunch"].ToString();
                            Session["IsDeviceTimeZone"] = jObj["IsDeviceTimeZone"].ToString();
                            Session["CountryCode"] = jObj["CountryCode"].ToString();
                            Session["AreaCode"] = jObj["AreaCode"].ToString();
                            Session["IsOTP"] = jObj["IsOTP"].ToString();
                            //Session["IsMobileOTP"] = jObj["IsMobileOTP"].ToString();
                            Session["IsMobileOTP"] = "0";
                            Session["lockAttendanceDay"] = jObj["lockAttendanceDay"].ToString();
                            Session["PlanExpDate"] = jObj["PlanExpDate"].ToString();
                            Session["AttendanceCorrection"] = jObj["AttendanceCorrection"].ToString();
                            int mcmpid = Convert.ToInt32(Session["CompanyId"].ToString());
                            string _clientDb = Session["DbName"].ToString();
                            Session["IsClientDBMatch"] = false;
                            Session["DaysLimitForAttendanceApproval"] = jObj["DaysLimitForAttendanceApproval"].ToString();
                            Session["OptHolidayLimit"] = jObj["OptHolidayLimit"].ToString();
                            Session["Hastourcompleted"] = Convert.ToBoolean(jObj["Hastourcompleted"]);
                            Session["admintour"] = Convert.ToBoolean(jObj["admintour"]);
                            Session["WidgetId"] = jObj["WidgetId"].ToString();

                            Session["DataGridThresold"] = Convert.ToInt64(jObj["DataGridThreShold"]);
                            Session["DataDropDownThreShold"] = Convert.ToInt64(jObj["DataDropDownThreShold"]);
                            Session["DataEmpListThreShold"] = Convert.ToInt64(jObj["DataEmpListThreShold"]);
                            //Feedback last Login logic //Feedback last Login logic ---  Commete stop feedback call -- 08 04 2024 -- Jayesh/Samit
                            //var i = RestClient.GetLastfeedback(Convert.ToString(Session["cmpcode"]));

                            //JArray jsonVal = JArray.Parse(i) as JArray;
                            //dynamic albums = jsonVal;
                            //int _fbd = 1;
                            //foreach (dynamic album in albums)
                            //{
                            //    _fbd = Convert.ToInt32(album.flag.Value);

                            //}
                            //Session["Feedbackvalid"] = Convert.ToInt32(_fbd);
                            //Feedback last Login logic
                            planRestrictions(mcmpid);

                            // Store all session variables in cache for HDFC payment redirect recovery
                            StoreSessionForCompany(Session["CompanyId"]?.ToString());

                            //if (Convert.ToInt32(Session["IsSchool"]) == 1 && Convert.ToInt32(Session["IsOffice"]) == 1)
                            //{
                            //    return RedirectToAction("ModuleSelection", "PayTime");
                            //}
                            if (systemWizarFlag.ToLowerInvariant() == "false" && model.IsCart == 0)
                            {
                                //if (Convert.ToInt32(Session["IsSchool"]) == 1 && Convert.ToInt32(Session["IsOffice"]) == 0)
                                //{
                                //    return RedirectToAction("AdminDashboard", "School");
                                //}

                                //else
                                //{
                                //    return RedirectToAction("SystemSettingWizard", "PayTime");
                                //}
                                return RedirectToAction("SystemSettingWizard", "PayTime");
                            }
                            else
                            {
                                //encrypt the ticket and add it to a cookie
                                //HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                                //Response.Cookies.Add(cookie);
                                //FormsAuthentication.RedirectFromLoginPage(loginUser.Username, false);
                                //if (Convert.ToInt32(Session["IsSchool"]) == 1 && Convert.ToInt32(Session["IsOffice"]) == 0)
                                //{
                                //    return RedirectToAction("AdminDashboard", "School");
                                //}
                                //else
                                //{
                                //    return RedirectToAction("AdminDashboard", "PayTime");
                                //}

                                if (Convert.ToInt32(Session["isPlanExp"]) == 1 &&
                                    Convert.ToInt32(Session["isFplan"]) == 0)
                                {
                                    return RedirectToAction("Planpricing", "PayTime");
                                }
                                else
                                {
                                    if (model.IsCart == 1)
                                    {
                                        TempData["meg"] = "Saved";
                                        return RedirectToAction("cart", "Devices");
                                    }
                                    else if (Convert.ToInt32(Session["IsSchool"]) == 1)
                                    {
                                        return RedirectToAction("AdminDashboard", "School");
                                    }
                                    else if (Convert.ToInt32(Session["IsOffice"]) == 1)
                                    {
                                        if (Convert.ToInt32(Session["IsOTP"]) == 1 && (Convert.ToInt32(Session["planId"]) == 6 || Convert.ToInt32(Session["planId"]) == 5 || Convert.ToInt32(Session["planId"]) == 3))
                                        {
                                            TempData["ShowLoginOTP"] = true;
                                            return RedirectToAction("LoginPage", "PayTime");
                                        }
                                        else if (Convert.ToInt32(Session["IsMobileOTP"]) == 1)
                                        {
                                            TempData["ShowLoginOTP"] = true;
                                            return RedirectToAction("LoginPage", "PayTime");
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(_clientDb))
                                            {
                                                bool isMatch = Helper.ClientDBNameCheck.IsDbNameMatch(_clientDb);
                                                if (isMatch)
                                                {
                                                    Session["IsClientDBMatch"] = isMatch;
                                                    return RedirectToAction("AdminDashboard", "Admin");
                                                }
                                            }
                                            return RedirectToAction("AdminDashboard", "Dashboard");
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(_clientDb))
                                        {
                                            bool isMatch = Helper.ClientDBNameCheck.IsDbNameMatch(_clientDb);
                                            if (isMatch)
                                            {
                                                Session["IsClientDBMatch"] = isMatch;
                                                return RedirectToAction("AdminDashboard", "Admin");
                                            }
                                        }
                                        return RedirectToAction("AdminDashboard", "Dashboard");
                                    }
                                }

                            }
                        }
                        else if (Convert.ToInt32(Session["RoleId"]) == 15) // 21
                        {
                            string strkey = jObj["keystr"].ToString();
                            Session["UserEmail"] = jObj["name"].ToString();
                            Session["RoleId"] = Convert.ToInt32(jObj["RoleId"]);
                            Session.Timeout = 10;
                            Session["DbName"] = "";
                            //return RedirectToAction("SuperAdminDashboard", "PayTime");
                            return RedirectToAction("SuperAdminCompanyDetails", "PayTime");
                        }
                        else
                        {
                            string strkey = jObj["keystr"].ToString();
                            Session["UserEmail"] = jObj["name"].ToString();
                            Session["RoleId"] = Convert.ToInt32(jObj["RoleId"]);
                            Session.Timeout = 10;
                            Session["DbName"] = "";
                            return RedirectToAction("DevicesList", "PayTime");
                        }
                    }
                    else
                    {

                        if (resp.Meg != "")
                        {
                            TempData["meg"] = resp.Meg;
                            TempData["MegSts"] = resp.MegSts;
                        }
                        else
                        {
                            TempData["meg"] = "something wrong please try again";
                        }
                        //ModelState.AddModelError(resp.MegSts, resp.Meg);
                        if (model.IsCart == 1)
                        {
                            return RedirectToAction("cart", "Devices");
                        }
                        return RedirectToAction("LoginPage", "PayTime");
                    }

                }
                else
                {

                    if (resp.Meg != "")
                    {
                        TempData["meg"] = resp.Meg;
                    }
                    else
                    {
                        TempData["meg"] = "something wrong please try again";
                    }

                    if (model.IsCart == 1)
                    {
                        if (string.IsNullOrEmpty(resp.Meg))
                        {
                            TempData["meg"] = "please enter valid details.";
                        }
                        return RedirectToAction("cart", "Devices");
                    }
                    //Response.Write("<script>alert('UserEmail and Password is not valid please try again.')</script>");
                    //ModelState.AddModelError(resp.MegSts, resp.Meg);
                    return RedirectToAction("LoginPage", "PayTime");
                }
            }
            catch (Exception Ex)
            {

                TempData["meg"] = Ex.InnerException;

                if (model.IsCart == 1)
                {
                    return RedirectToAction("cart", "Devices");
                }
                return RedirectToAction("LoginPage", "PayTime");
            }

            //return View(model);
            //return RedirectToAction("SystemSettingWizard", "PayTime");
        }

        private void planRestrictions(int mcmpid)
        {
            Getpara gp = new Getpara();
            gp.pkid = mcmpid;
            gp.para = 0;
            gp.iscount = 0;
            gp.companycode = Convert.ToString(Session["cmpcode"]);
            int nouser = 0;
            int isflg = 0;
            int noemp = 0;
            int isFace = 0;
            int isEss = 0;
            int isExp = 0;
            int isFplan = 0;
            int planid = 0;
            int rmDay = 0;
            int pDursn = 0;
            int isRecurring = 0;
            string rpcustid = "";
            string rptokenid = "";
            string Isman = "";
            int isStandard = 0;
            int iscurrencycode = 0;
            DataTable dtsub = RestClient.Restrictionasperplan(gp);
            if (dtsub != null && dtsub.Rows.Count > 0)
            {
                nouser = Convert.ToInt32(dtsub.Rows[0]["UseCount"]);
                isflg = Convert.ToInt32(dtsub.Rows[0]["islimit"]);
                noemp = Convert.ToInt32(dtsub.Rows[0]["empctn"]);
                isFace = Convert.ToInt32(dtsub.Rows[0]["isFace"]);
                isEss = Convert.ToInt32(dtsub.Rows[0]["isEss"]);
                isExp = Convert.ToInt32(dtsub.Rows[0]["Substs"]);
                isFplan = Convert.ToInt32(dtsub.Rows[0]["isFplan"]);
                planid = Convert.ToInt32(dtsub.Rows[0]["Planid"]);
                rmDay = Convert.ToInt32(dtsub.Rows[0]["Rmnday"]);
                pDursn = Convert.ToInt32(dtsub.Rows[0]["Pdrsn"]);
                rpcustid = dtsub.Rows[0]["rpcustid"].ToString();
                rptokenid = dtsub.Rows[0]["rptokenid"].ToString();
                isRecurring = Convert.ToInt32(dtsub.Rows[0]["IsRec"]);
                Isman = dtsub.Rows[0]["Isman"].ToString();
                isStandard = Convert.ToInt32(dtsub.Rows[0]["IsStandard"]);
                iscurrencycode = Convert.ToInt32(dtsub.Rows[0]["IScurrencycode"]);
            }

            Session["UseCount"] = nouser;
            Session["IsUserlimit"] = isflg;
            Session["Noofemp"] = noemp;
            Session["isPlanFace"] = isFace;
            Session["isPlanEss"] = isEss;
            Session["isPlanExp"] = isExp;
            Session["isFplan"] = isFplan;
            Session["planId"] = planid;
            Session["rmDay"] = rmDay;
            Session["Pdrsn"] = pDursn;
            Session["rzpcustid"] = rpcustid;
            Session["rzptokenid"] = rptokenid;
            Session["isRecurring"] = isRecurring;
            Session["isMan"] = Isman;
            Session["isStandard"] = isStandard;
            Session["IScurrencycode"] = iscurrencycode;
        }

        [HttpPost]
        public ActionResult EmpLogin(LoginModel model, string emprememberme)
        {
            TempData["IsEmp"] = "true";
            ModelState.Remove("CompanyName");
            try
            {
                if (ModelState.IsValid)
                {
                    resp = RestAccout.EmpLogin(model);
                    if (resp.MegSts == "jwt_token")
                    {
                        Session["tokan"] = resp.Meg;
                        string payload = AuthHandler.getPayload(Session["tokan"].ToString(), ConfigurationManager.AppSettings["jwtKey"].ToString(), false);
                        JObject jObj = JObject.Parse(payload);

                        if (emprememberme == "true")
                        {
                            HttpCookie empcookie = new HttpCookie("EmpLogin");
                            empcookie.Values.Add("CompanyCode", model.CompanyCode);
                            empcookie.Values.Add("EmpUserEmail", model.UserEmail);
                            empcookie.Values.Add("EmpPassword", model.Password);
                            empcookie.Expires = DateTime.Now.AddDays(15);
                            Response.Cookies.Add(empcookie);
                        }
                        else
                        {
                            if (Request.Cookies["EmpLogin"] != null)
                            {
                                var c = new HttpCookie("EmpLogin");
                                c.Expires = DateTime.Now.AddDays(-1);
                                Response.Cookies.Add(c);
                            }
                        }
                        Session["RoleId"] = Convert.ToInt32(jObj["RoleId"]);
                        Session["photo"] = jObj["photo"].ToString();
                        Session["cmpname"] = jObj["cmpname"].ToString();
                        Session["UserEmail"] = jObj["name"].ToString();
                        Session["EmpId"] = Convert.ToInt32(jObj["EmpId"]);
                        Session["DepartmentId"] = Convert.ToInt32(jObj["departmentId"]);
                        //Session["EmpName"] = jObj["EmpName"].ToString();
                        string strkey = jObj["keystr"].ToString();
                        Session["cmpcode"] = strkey.Split('|')[1].ToString();
                        Session["DbName"] = strkey.Split('|')[4].ToString();
                        Session["ClientCompanyId"] = jObj["ClientCompanyId"].ToString();
                        Session["IsSetting"] = jObj["IsSetting"].ToString();
                        Session.Timeout = 10;
                        Session["domain"] = strkey.Split('|')[3].ToString();
                        Session["IsSchool"] = jObj["IsSchool"].ToString();
                        Session["IsOffice"] = jObj["IsOffice"].ToString();
                        Session["BranchId"] = jObj["BranchId"].ToString();
                        Session["UserId"] = jObj["UserId"].ToString();
                        Session["CompanyId"] = jObj["compId"].ToString();
                        Session["IsApprovalForWebpunch"] = jObj["IsApprovalForWebpunch"].ToString();
                        Session["IsDeviceTimeZone"] = jObj["IsDeviceTimeZone"].ToString();
                        Session["IsDateFormat"] = jObj["IsDateFormat"].ToString();
                        Session["CountryCode"] = jObj["CountryCode"].ToString();
                        Session["AreaCode"] = jObj["AreaCode"].ToString();
                        Session["IsOTP"] = jObj["IsOTP"].ToString();
                        //Session["IsMobileOTP"] = jObj["IsMobileOTP"].ToString();
                        Session["IsMobileOTP"] = "0";
                        Session["lockAttendanceDay"] = jObj["lockAttendanceDay"].ToString();
                        Session["AttendanceCorrection"] = jObj["AttendanceCorrection"].ToString();
                        int mcmpid = Convert.ToInt32(Session["CompanyId"].ToString());
                        Session["worf"] = jObj["worf"].ToString();
                        Session["PlanExpDate"] = jObj["PlanExpDate"].ToString();
                        Session["EmpName"] = jObj["empname"].ToString();
                        Session["DaysLimitForAttendanceApproval"] = jObj["DaysLimitForAttendanceApproval"].ToString();
                        planRestrictions(mcmpid);
                        string _clientDb = Session["DbName"].ToString();
                        Session["IsClientDBMatch"] = false;
                        Session["OptHolidayLimit"] = jObj["OptHolidayLimit"].ToString();
                        Session["Hastourcompleted"] = Convert.ToBoolean(jObj["Hastourcompleted"]);
                        Session["admintour"] = Convert.ToBoolean(jObj["admintour"]);
                        Session["WidgetId"] = jObj["WidgetId"].ToString();
                        Session["Ishierchy"] = Convert.ToInt32(jObj["Ishierchy"]);

                        Session["DataGridThresold"] = Convert.ToInt64(jObj["DataGridThreShold"]);
                        Session["DataDropDownThreShold"] = Convert.ToInt64(jObj["DataDropDownThreShold"]);
                        Session["DataEmpListThreShold"] = Convert.ToInt64(jObj["DataEmpListThreShold"]);
                        Session["DesignationId"] = Convert.ToInt32(jObj["DesignationId"]);
                        //Feedback last Login logic ---  Commete stop feedback call -- 08 04 2024 -- Jayesh/Samit
                        //var i = RestClient.GetLastfeedback(Convert.ToString(Session["cmpcode"]));
                        //JArray jsonVal = JArray.Parse(i) as JArray;
                        //dynamic albums = jsonVal;
                        //int _fbd = 1;
                        //foreach (dynamic album in albums)
                        //{
                        //    _fbd = Convert.ToInt32(album.flag.Value);

                        //}
                        //Session["Feedbackvalid"] = Convert.ToInt32(_fbd);
                        //Feedback last Login logic
                        if ((Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806") && Session["IsSchool"].ToString() == "1")
                        {
                            return RedirectToAction("AdminDashboard", "School");
                        }
                        else if ((Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806") && Session["IsOffice"].ToString() == "1")
                        {
                            planRestrictions(0);


                            if (Convert.ToInt32(Session["isPlanExp"]) == 1 &&
                                Convert.ToInt32(Session["isFplan"]) == 0)
                            {
                                return RedirectToAction("Planpricing", "PayTime");
                            }
                            if (Session["UserEmail"].ToString() == model.Password.ToString())
                            {
                                return RedirectToAction("ResetPassword", "ESS");
                            }
                            else
                            {

                                if (Convert.ToInt32(Session["IsOTP"]) == 1 && (Convert.ToInt32(Session["planId"]) == 6 || Convert.ToInt32(Session["planId"]) == 5 || Convert.ToInt32(Session["planId"]) == 3))
                                {
                                    TempData["ShowLoginOTP"] = true;
                                    return RedirectToAction("LoginPage", "PayTime");
                                }
                                else if (Convert.ToInt32(Session["IsMobileOTP"]) == 1)
                                {
                                    TempData["ShowLoginOTP"] = true;
                                    return RedirectToAction("LoginPage", "PayTime");
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(_clientDb))
                                    {
                                        bool isMatch = Helper.ClientDBNameCheck.IsDbNameMatch(_clientDb);
                                        if (isMatch)
                                        {
                                            Session["IsClientDBMatch"] = isMatch;
                                            return RedirectToAction("AdminDashboard", "Admin");
                                        }
                                    }
                                    return RedirectToAction("AdminDashboard", "Dashboard");
                                }
                                //Feedback last Login logic
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(Session["isPlanExp"]) == 1 &&
                                Convert.ToInt32(Session["isFplan"]) == 0)
                            {
                                return RedirectToAction("Planpricing", "PayTime");
                            }
                            if (Session["UserEmail"].ToString() == model.Password.ToString())
                            {
                                return RedirectToAction("ResetPassword", "ESS");
                            }
                            else
                            {
                                if (Convert.ToInt32(Session["IsOTP"]) == 1 && (Convert.ToInt32(Session["planId"]) == 6 || Convert.ToInt32(Session["planId"]) == 5 || Convert.ToInt32(Session["planId"]) == 3))
                                {
                                    TempData["ShowLoginOTP"] = true;
                                    return RedirectToAction("LoginPage", "PayTime");
                                }
                                else if (Convert.ToInt32(Session["IsMobileOTP"]) == 1)
                                {
                                    TempData["ShowLoginOTP"] = true;
                                    return RedirectToAction("LoginPage", "PayTime");
                                }
                                else
                                {
                                    //return RedirectToAction("EmployeeDashboard", "PayTime");
                                    if (Session["RoleId"].ToString() == "6000")
                                    {
                                        // ----- Only PMS Admin login page call --- 24 10 2024
                                        return RedirectToAction("OKRGenerate", "OKR");
                                    }
                                    else if(Session["RoleId"].ToString() == "5811")
                                    {
                                        return RedirectToAction("orders", "cms");
                                    }
                                    else
                                    {
                                        return RedirectToAction("EmployeeDashboard", "Dashboard");
                                    }
                                }
                            }

                        }

                    }
                    else
                    {
                        if (resp.Meg != "")
                        {
                            TempData["meg"] = resp.Meg;
                        }
                        else
                        {
                            //TempData["meg"] = "Something Went Wrong. Please Try Again";
                        }
                        //ModelState.AddModelError(resp.MegSts, resp.Meg);

                        return RedirectToAction("LoginPage", "PayTime");
                    }
                }
                else
                {

                    if (resp.Meg != "")
                    {
                        TempData["meg"] = resp.Meg;
                    }
                    else
                    {
                        //TempData["meg"] = "Something Went Wrong. Please Try Again";
                    }
                    //Response.Write("<script>alert('UserEmail and Password is not valid please try again.')</script>");
                    //ModelState.AddModelError(resp.MegSts, resp.Meg);
                    return RedirectToAction("LoginPage", "PayTime");
                }
            }
            catch (Exception Ex)
            {

                TempData["meg"] = Ex.InnerException;


                return RedirectToAction("LoginPage", "PayTime");
            }

            //return View(model);
            //return RedirectToAction("SystemSettingWizard", "PayTime");
        }

        public JsonResult LiveDemoLogin(string UserEmail, string Password)
        {
            JsonResult result;
            string str = "";
            LoginModel model = new LoginModel();
            model.UserEmail = UserEmail;
            model.Password = Password;
            resp = RestAccout.Login(model);
            if (resp.MegSts == "jwt_token")
            {
                //FormsAuthentication.SetAuthCookie(model.UserEmail, true);

                Session["tokan"] = resp.Meg;
                string payload = AuthHandler.getPayload(Session["tokan"].ToString(), ConfigurationManager.AppSettings["jwtKey"].ToString(), false);
                JObject jObj = JObject.Parse(payload);
                Session["RoleId"] = Convert.ToInt32(jObj["RoleId"]);
                if (Convert.ToInt32(Session["RoleId"]) != 15)
                {
                    Session["cmpname"] = jObj["cmpname"].ToString();
                    Session["UserEmail"] = jObj["name"].ToString();
                    Session["photo"] = jObj["photo"].ToString();
                    Session["UserId"] = jObj["UserId"].ToString();
                    string strkey = jObj["keystr"].ToString();
                    string systemWizarFlag = jObj["IsSetting"].ToString();
                    Session["cmpcode"] = strkey.Split('|')[1].ToString();
                    Session["ClientCompanyId"] = jObj["ClientCompanyId"].ToString();
                    Session["CompanyId"] = jObj["compId"].ToString();
                    Session.Timeout = 10;
                    Session["domain"] = strkey.Split('|')[3].ToString();
                    Session["DbName"] = strkey.Split('|')[4].ToString();
                    Session["IsSetting"] = jObj["IsSetting"].ToString();
                    Session["IsSchool"] = jObj["IsSchool"].ToString();
                    Session["IsOffice"] = jObj["IsOffice"].ToString();
                    Session["FirstEmpId"] = jObj["FirstEmpId"].ToString();
                    Session["EmpId"] = Convert.ToInt32(jObj["EmpId"]);
                    Session["IsApprovalForWebpunch"] = jObj["IsApprovalForWebpunch"].ToString();
                    Session["IsDeviceTimeZone"] = jObj["IsDeviceTimeZone"].ToString();
                    Session["IsDateFormat"] = jObj["IsDateFormat"].ToString();
                    Session["CountryCode"] = jObj["CountryCode"].ToString();
                    Session["AreaCode"] = jObj["AreaCode"].ToString();

                    planRestrictions(0);
                    //if (systemWizarFlag == "False")
                    //{
                    //        str = "PayTime/SystemSettingWizard";
                    //         result = Json(str);
                    //         result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    //         return result;
                    //   // return RedirectToAction("SystemSettingWizard", "PayTime");
                    //}


                }
                str = "true";
                result = Json(str);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            else
            {
                str = "false";
                result = Json(str);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }

        }


        [HttpPost]
        public JsonResult LiveEmpDemoLogin(string CompanyCode, string UserEmail, string Password)
        {
            JsonResult result;
            string str = "";
            LoginModel model = new LoginModel();
            model.CompanyCode = CompanyCode;
            model.UserEmail = UserEmail;
            model.Password = Password;
            resp = RestAccout.EmpLogin(model);
            if (resp.MegSts == "jwt_token")
            {
                Session["DbName"] = "";
                Session["tokan"] = resp.Meg;
                string payload = AuthHandler.getPayload(Session["tokan"].ToString(), ConfigurationManager.AppSettings["jwtKey"].ToString(), false);
                JObject jObj = JObject.Parse(payload);
                Session["RoleId"] = Convert.ToInt32(jObj["RoleId"]);
                Session["photo"] = jObj["photo"].ToString();
                Session["cmpname"] = jObj["cmpname"].ToString();
                Session["UserEmail"] = jObj["name"].ToString();
                Session["EmpId"] = Convert.ToInt32(jObj["EmpId"]);
                //Session["EmpName"] = jObj["	EmpName"].ToString();
                string strkey = jObj["keystr"].ToString();
                Session["cmpcode"] = strkey.Split('|')[1].ToString();
                Session["ClientCompanyId"] = jObj["ClientCompanyId"].ToString();
                Session["IsSetting"] = jObj["IsSetting"].ToString();
                Session.Timeout = 10;
                //return RedirectToAction("EmployeeDashboard", "PayTime");
                str = "true";
                result = Json(str);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            else
            {
                str = "false";
                result = Json(str);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

        [HttpPost]
        public ActionResult SubDomainLogin(LoginModel model, string emprememberme)
        {
            ModelState.Remove("CompanyName");
            try
            {
                if (ModelState.IsValid)
                {
                    resp = RestAccout.SubDomainLogin(model);
                    if (resp.MegSts == "jwt_token")
                    {
                        Session["tokan"] = resp.Meg;
                        string payload = AuthHandler.getPayload(Session["tokan"].ToString(), ConfigurationManager.AppSettings["jwtKey"].ToString(), false);
                        JObject jObj = JObject.Parse(payload);

                        if (emprememberme == "true")
                        {
                            HttpCookie empcookie = new HttpCookie("EmpLogin");
                            empcookie.Values.Add("CompanyCode", model.CompanyCode);
                            empcookie.Values.Add("EmpUserEmail", model.UserEmail);
                            empcookie.Values.Add("EmpPassword", model.Password);
                            empcookie.Expires = DateTime.Now.AddDays(15);
                            Response.Cookies.Add(empcookie);
                        }
                        else
                        {
                            if (Request.Cookies["EmpLogin"] != null)
                            {
                                var c = new HttpCookie("EmpLogin");
                                c.Expires = DateTime.Now.AddDays(-1);
                                Response.Cookies.Add(c);
                            }
                        }
                        Session["DbName"] = "";
                        Session["RoleId"] = Convert.ToInt32(jObj["RoleId"]);
                        Session["photo"] = jObj["photo"].ToString();
                        Session["cmpname"] = jObj["cmpname"].ToString();
                        Session["UserId"] = jObj["UserId"].ToString();
                        Session["UserEmail"] = jObj["name"].ToString();
                        Session["EmpId"] = Convert.ToInt32(jObj["EmpId"]);
                        //Session["EmpName"] = jObj["	EmpName"].ToString();
                        string strkey = jObj["keystr"].ToString();
                        Session["cmpcode"] = strkey.Split('|')[1].ToString();
                        Session["ClientCompanyId"] = jObj["ClientCompanyId"].ToString();
                        Session["IsSetting"] = jObj["IsSetting"].ToString();
                        Session["FirstEmpId"] = jObj["FirstEmpId"].ToString();
                        string systemWizarFlag = jObj["IsSetting"].ToString();
                        Session["domain"] = strkey.Split('|')[3].ToString();
                        Session["CompanyId"] = jObj["compId"].ToString();
                        int _CompanyId = Convert.ToInt32(jObj["compId"].ToString());
                        planRestrictions(_CompanyId);
                        int flgtype = Convert.ToInt32(jObj["flgEmpType"]);
                        //if (flgtype == 1)
                        //{
                        //    Session["IsSchool"] = jObj["IsSchool"].ToString();
                        //    Session["IsOffice"] = jObj["IsOffice"].ToString();
                        //}
                        Session["IsSchool"] = jObj["IsSchool"].ToString();
                        Session["IsOffice"] = jObj["IsOffice"].ToString();
                        Session["BranchId"] = jObj["BranchId"].ToString();
                        Session.Timeout = 10;
                        Session["IsDateFormat"] = jObj["IsDateFormat"].ToString();
                        Session["IsApprovalForWebpunch"] = jObj["IsApprovalForWebpunch"].ToString();
                        Session["IsDeviceTimeZone"] = jObj["IsDeviceTimeZone"].ToString();
                        Session["CountryCode"] = jObj["CountryCode"].ToString();
                        Session["AreaCode"] = jObj["AreaCode"].ToString();
                        if (Convert.ToInt32(Session["IsSchool"]) == 1 && Convert.ToInt32(Session["IsOffice"]) == 1)
                        {
                            return RedirectToAction("ModuleSelection", "PayTime");
                        }
                        if (systemWizarFlag.ToLowerInvariant() == "false")
                        {
                            if (Convert.ToInt32(Session["IsSchool"]) == 1 && Convert.ToInt32(Session["IsOffice"]) == 0)
                            {
                                return RedirectToAction("AdminDashboard", "School");
                            }
                            else
                            {
                                return RedirectToAction("SystemSettingWizard", "PayTime");
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(Session["IsSchool"]) == 1 && Convert.ToInt32(Session["IsOffice"]) == 0)
                            {
                                return RedirectToAction("AdminDashboard", "School");
                            }
                            else
                            {
                                if (flgtype == 1)
                                {
                                    //planRestrictions(0);

                                    if (Convert.ToInt32(Session["isPlanExp"]) == 1 &&
                                        Convert.ToInt32(Session["isFplan"]) == 0)
                                    {
                                        return RedirectToAction("Planpricing", "PayTime");
                                    }
                                    else
                                    {
                                        return RedirectToAction("AdminDashboard", "Dashboard");
                                    }
                                }
                                else if (flgtype == 2)
                                {

                                    if (Convert.ToInt32(Session["isPlanExp"]) == 1 &&
                                        Convert.ToInt32(Session["isFplan"]) == 0)
                                    {
                                        return RedirectToAction("Planpricing", "PayTime");
                                    }
                                    else
                                    {

                                        if ((Session["RoleId"].ToString() == "6805" ||
                                             Session["RoleId"].ToString() == "6806") &&
                                            Session["IsSchool"].ToString() == "1")
                                        {
                                            return RedirectToAction("AdminDashboard", "School");
                                        }
                                        else if ((Session["RoleId"].ToString() == "6805" ||
                                                  Session["RoleId"].ToString() == "6806") &&
                                                 Session["IsOffice"].ToString() == "1")
                                        {
                                            planRestrictions(0);
                                            return RedirectToAction("AdminDashboard", "Dashboard");
                                        }
                                        else
                                        {
                                            return RedirectToAction("EmployeeDashboard", "Dashboard");
                                        }
                                    }
                                }
                                else
                                {
                                    return RedirectToAction("Index", "PayTime");
                                }
                            }
                        }

                    }
                    else
                    {
                        if (resp.Meg != "")
                        {
                            TempData["meg"] = resp.Meg;
                            TempData["MegSts"] = resp.MegSts;
                        }
                        else
                        {
                            //TempData["meg"] = "Something Went Wrong. Please Try Again";
                        }
                        //ModelState.AddModelError(resp.MegSts, resp.Meg);

                        return RedirectToAction("Index", "PayTime");
                    }
                }
                else
                {

                    if (resp.Meg != "")
                    {
                        TempData["meg"] = resp.Meg;
                    }
                    else
                    {
                        //TempData["meg"] = "Something Went Wrong. Please Try Again";
                    }
                    //Response.Write("<script>alert('UserEmail and Password is not valid please try again.')</script>");
                    //ModelState.AddModelError(resp.MegSts, resp.Meg);
                    return RedirectToAction("Index", "PayTime");
                }
            }
            catch (Exception Ex)
            {

                TempData["meg"] = Ex.InnerException;


                return RedirectToAction("Index", "PayTime");
            }

            //return View(model);
            //return RedirectToAction("SystemSettingWizard", "PayTime");
        }
        #endregion

        #region Dashboards
        //foradmin
        [CustAuthFilter]
        public ActionResult AdminDashboard()
        {
            try
            {

                if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                {
                    CompanySetting cms = new CompanySetting();
                    cms = RestClient.GetSystemSettingById(Convert.ToInt32(Session["ClientCompanyId"]));
                    if (Convert.ToInt32(Session["RoleId"].ToString()) > 1)
                    {
                        cms = RestClient.GetSystemSettingById(1);
                    }
                    if (cms.IsActive == true)
                    {
                        Session["IsSetting"] = cms.IsActive;
                        Session["HasGrade"] = cms.HasGrade; //For Bionic F7 device inclued
                    }

                    ViewBag.Companyslist = RestClient.CompanyGetAll();
                }
                else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                {
                    CompanySetting cms = new CompanySetting();
                    cms = RestClient.GetSystemSettingById(1);

                    if (cms.IsActive == true)
                    {
                        Session["HasGrade"] = cms.HasGrade;//For Bionic F7 device inclued
                    }
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    ViewBag.Companyslist = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                }
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [HttpPost]
        [CustAuthFilter]
        public ActionResult AdminDashboard(string CompanyID, string BranchId)
        {
            try
            {
                if (BranchId == "")
                {
                    BranchId = "0";
                }
                string fdate = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.AllCounter = RestClient.GetallAdminCounter(CompanyID, BranchId, fdate);
                ViewBag.Companyslist = RestClient.CompanyGetAll();
                ViewBag.cbid = CompanyID;
                ViewBag.bid = BranchId;
                return View();
            }
            catch
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        [HttpPost]
        [CustAuthFilter]
        public JsonResult AdminDashboardCounters(string cmpId, string branchId, string fdate)
        {
            JsonResult result;
            try
            {
                if (cmpId == "")
                {
                    cmpId = "0";
                }
                if (branchId == "" || branchId == "null")
                {
                    branchId = "0";
                }
                var i = RestClient.GetallAdminCounter(cmpId, branchId, fdate);
                result = Json(i);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }


        [HttpPost]
        [CustAuthFilter]
        public JsonResult AdminDashboardDetails(int type, string cmpId, string branchId, string fdate, string tdate)
        {
            JsonResult result;
            try
            {
                if (cmpId == "")
                {
                    cmpId = "0";
                }
                if (branchId == "" || branchId == "null")
                {
                    branchId = "0";
                }
                var i = RestClient.GetallAdminDetails(type, cmpId, branchId, fdate, tdate);
                result = Json(i);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.MaxJsonLength = Int32.MaxValue;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

        [HttpGet]
        [CustAuthFilter]
        public JsonResult FillChartAdminDashboard(string fromdate, string todate, int statusflg, string finalStatus, int cmpId, int branchId)
        {
            JsonResult result;
            try
            {
                var i = RestClient.GetAdminAttendanceSummarycount(fromdate, todate, statusflg, finalStatus, cmpId, branchId);
                result = Json(i);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }


        [HttpGet]
        [CustAuthFilter]
        public JsonResult FillLeaveChart(string leavedate, string cmpId, string branchId)
        {
            JsonResult result;
            try
            {
                //string leavedate = DateTime.Now.ToString("yyyy-MM-dd");
                var i = RestClient.GetEmployeeLeavecount(leavedate, cmpId, branchId);
                result = Json(i);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }



        [HttpGet]
        [CustAuthFilter]
        public JsonResult FillLeaveChartDetails(int LeaveDaysType, string leavedate, int cmpId, int branchId)
        {
            JsonResult result;
            try
            {

                var i = RestClient.GetEmployeeLeaveDetails(LeaveDaysType, leavedate, cmpId, branchId);
                result = Json(i);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }


        [HttpGet]
        [CustAuthFilter]
        public JsonResult FillHoursSummary(string cmpId, string branchId, string fdate)
        {
            JsonResult result;
            try
            {
                //string leavedate = DateTime.Now.ToString("yyyy-MM-dd");
                var i = RestClient.GetEmployeeHoursSummary(cmpId, branchId, fdate);
                result = Json(i);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }



        [HttpGet]
        [CustAuthFilter]
        public JsonResult FillAtworkAdminDashboard(string CompanyID, string BranchId, string fdate)
        {
            JsonResult result;
            try
            {
                if (BranchId == "")
                {
                    BranchId = "0";
                }
                if (CompanyID == "")
                {
                    CompanyID = "0";
                }
                var i = RestClient.GetallAdminCounter(CompanyID, BranchId, fdate);
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(i);
                result = Json(json);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }



        //foremp

        [HttpGet]
        [CustAuthFilter]
        public JsonResult FillCalenderDashboard(string empid, string fromdate, string todate, int statusflg, string finalStatus, string searchName, string searchBy)
        {
            JsonResult result;
            try
            {
                var i = RestClient.GetAttendanceSummarycount(empid, fromdate, todate, statusflg, finalStatus, searchName, searchBy);
                result = Json(i);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        //   [HttpPost]
        // [CustAuthFilter]



        public JsonResult ListEventsJson(string empid, string fromdate, string todate, int statusflg, string finalStatus, string searchName, string searchBy)
        {
            FillCalendar fc = new FillCalendar();
            fc.FillCalendarList = RestClient.GetFullcalendarDetails(empid, fromdate, todate, statusflg, finalStatus, searchName, searchBy);
            ArrayList arr = new ArrayList();
            foreach (var i in fc.FillCalendarList)
            {
                arr.Add(i);
            }
            var rows = arr.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        [CustAuthFilter]
        public JsonResult GetPunchInformation(string empid)
        {
            JsonResult result;
            try
            {
                int eid = Convert.ToInt32(empid);
                var i = RestClient.GetPunchInfo(eid);
                result = Json(i);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

        //[CustAuthFilter]
        //[HttpGet]
        //public JsonResult EmployeeBirthday()
        //{
        //    JsonResult result;
        //    try
        //    {
        //        var i = RestClient.GetBirthdayOfEmployees();
        //        result = Json(i);
        //        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        result = Json(new SelectList("", "0"));
        //        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //        return result;
        //    }

        //}
        public List<SelectListItem> FillLeaveType()
        {
            LeaveTypeMaster lv = new LeaveTypeMaster();
            IEnumerable<LeaveTypeMaster> lvList;
            lvList = RestClient.LeaveTypeGetAll();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var i in lvList)
            {
                list.Add(new SelectListItem() { Text = i.LeaveTypeName, Value = Convert.ToString(i.LeaveTypeId) });
            }
            return list;
        }
        [CustAuthFilter]
        public ActionResult EmployeeDashboard()
        {
            int empid = Convert.ToInt32(Session["EmpId"]);
            int Ishierchy = Convert.ToInt32(Session["Ishierchy"]);
            try
            {
                Employees emp = new Employees();
                var empgetall = RestClient.EmployeeGetAll(Ishierchy);
                if (Convert.ToInt32(Session["RoleId"]) != 1 && Convert.ToInt32(Session["RoleId"]) != 6805 && Convert.ToInt32(Session["RoleId"]) != 6806)
                {
                    emp.EmployeeList = RestClient.EmployeeGet(Convert.ToInt32(Session["EmpId"]));
                    var policyId = emp.EmployeeList.Select(x => x.PolicyId).SingleOrDefault();
                    var policy = RestClient.HrPolicyGetbyID(policyId);
                    ViewBag.PolicyName = policy != null ? policy.PolicyName : "";

                    //ViewBag.PolicyName = RestClient.HrPolicyGetbyID(emp.EmployeeList.Select(x => x.PolicyId).SingleOrDefault()).PolicyName;
                }
                else if (Convert.ToInt32(Session["RoleId"]) == 1 && Convert.ToInt32(Session["EmpId"]) != 0)
                {
                    emp.EmployeeList = RestClient.EmployeeGet(Convert.ToInt32(Session["EmpId"]));
                    //ViewBag.PolicyName = RestClient.HrPolicyGetbyID(emp.EmployeeList.Select(x => x.PolicyId).SingleOrDefault()).PolicyName;
                    var policyId = emp.EmployeeList.Select(x => x.PolicyId).SingleOrDefault();
                    var policy = RestClient.HrPolicyGetbyID(policyId);
                    ViewBag.PolicyName = policy != null ? policy.PolicyName : "";
                }
                else if (Convert.ToInt32(Session["RoleId"]) == 6805 || Convert.ToInt32(Session["RoleId"]) == 6806)
                {
                    if (empgetall != null)
                    {
                        var firstemp = empgetall.FirstOrDefault();
                        emp.EmployeeList = new[] { firstemp };
                    }
                    else
                    {
                        emp.EmployeeList = RestClient.EmployeeGet(0);
                    }
                }
                else
                {
                    emp.EmployeeList = RestClient.EmployeeGet(Convert.ToInt32(Session["FirstEmpId"]));
                }
                if (empid > 0)
                {
                    ViewBag.allemp = TransactionRestClient.LeaveListbyEmployee(empid);
                }
                ViewBag.LeaveTypeList = FillLeaveType();
                ViewBag.Getsystemsettingdateformatupdated = TransactionRestClient.CompanySettingGetAll();
                // ViewBag.empdata = empgetall.Select(e => e.EmpName);
                //ViewBag.empdata = empgetall.Select(e => new { EmpName = e.EmpName, EmpId = e.EmpId });
                var empdata = empgetall.Select(e => new { EmpName = e.EmpName, EmpId = e.EmpId });
                var jsonData = JsonConvert.SerializeObject(empdata);
                byte[] compressedData = CompressionUtils.Compress(jsonData);
                ViewBag.empdata = Convert.ToBase64String(compressedData);
                return View(emp);
            }
            catch (Exception ex)
            {
                string x = ex.ToString();
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [CustAuthFilter]
        public ActionResult GenrateDashboard(int? cmpid, int? branchid, string daterang)
        {
            if (cmpid > 0 && branchid > 0 && daterang != "")
            {
                var Emplist = RestClient.EmployeeGetAll();
                // ViewBag.TE=Emplist.Where(x=>x.EmpJoinDate<=daterang.Value)
                ViewBag.Emploayeelist = Emplist;

                ViewBag.TE = 0;//Total Strength
                ViewBag.EAW = 0;//Employee At Work
                ViewBag.TB = 0;//BirthDay
                ViewBag.TLI = 0;//Late IN Count
                ViewBag.TEO = 0;//Early OUT Count
                ViewBag.TD = 0;//Total Device
            }
            return RedirectToAction("AdminDashboard");
        }

        #endregion

        #region SuperAdmin
        [CustAuthFilter]
        [HttpGet]
        public ActionResult SuperAdminDashboard()
        {
            try
            {
                ViewBag.AllCounter = RestClient.GetallCounter();
                ViewBag.recentregistered = RestClient.GetSuperAdminDashboard().OrderByDescending(c => c.CompanyId).Take(8).ToList();
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [CustAuthFilter]
        public ActionResult SuperAdminCompanyDetails()
        {
            SuperAdminDetails obj = new SuperAdminDetails();
            try
            {
                obj.SuperAdminDetailslist = RestClient.GetSuperAdminDashboard();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
            return View(obj);
        }
        [CustAuthFilter]
        public ActionResult ActiveCompany()
        {
            try
            {
                SuperAdminDetails sa = new SuperAdminDetails();
                sa.SuperAdminDetailslist = RestClient.GetActiveCompanies(1);
                //sa.SuperAdminDetailslist = RestClient.GetSuperAdminDashboard().Where(c => c.IsActiveUser == true);
                return View(sa);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }

        public ActionResult RemoveCompanies()
        {
            try
            {
                SuperAdminDetails sa = new SuperAdminDetails();
                sa.SuperAdminDetailslist = RestClient.GetActiveCompanies(1);
                return View(sa);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }
        [HttpPost]
        public ActionResult RemoveCompanies(string DbName, string CompanyId)
        {
            try
            {
                ErrorMsg em = new ErrorMsg();
                em = RestClient.DropDatabase(DbName, CompanyId);

                SuperAdminDetails sa = new SuperAdminDetails();
                sa.SuperAdminDetailslist = RestClient.GetActiveCompanies(1);
                return View(sa);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");

            }



            //var dbhost =
            //    $dbhost = 'localhost:3036';
            //$dbuser = 'root';
            //$dbpass = 'rootpassword';
            //$conn = mysql_connect($dbhost, $dbuser, $dbpass);
            //if(! $conn ) {
            //   die('Could not connect: ' . mysql_error());
            //}
            //echo 'Connected successfully<br />';
            //$sql = 'DROP DATABASE TUTORIALS';
            //$retval = mysql_query( $sql, $conn );
            //if(! $retval ) {
            //   die('Could not delete database: ' . mysql_error());
            //}
            //echo "Database TUTORIALS deleted successfully\n";
            //mysql_close($conn);
        }


        public ActionResult ConfigurationSystem()
        {
            try
            {
                ConfigurationSystemMaster csm = new ConfigurationSystemMaster();
                csm.ConfigSysList = RestClient.SystemConfigurationGetall();
                return View(csm);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        [HttpPost]
        public ActionResult ConfigurationSystem(ConfigurationSystemMaster csm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ViewBag.msg = "";
                    ViewBag.error = "";
                    if (csm.ConfigId > 0)
                    {
                        if (!RestClient.SystemConfigurationUpdate(csm.ConfigId, csm))
                        {
                            ViewBag.msg = "System configuration not updated due to service issue.";
                        }
                        else
                        {
                            ViewBag.msg = "System configuration is updated.";
                        }
                    }
                    else
                    {
                        RestClient.SystemConfigurationAdd(csm);
                        ViewBag.msg = "System configuration added successfully.";
                    }
                }
                else
                {
                    ViewBag.error = "Not valid entry.";
                }
                ConfigurationSystemMaster cs = new ConfigurationSystemMaster();
                cs.ConfigSysList = RestClient.SystemConfigurationGetall();
                return View(cs);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");

            }


        }


        [HttpPost]
        public string DeleteSystemConfiguration(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.SystemConfigurationDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }


        [CustAuthFilter]
        public ActionResult RegisteredDevicesList()
        {
            ActiveDevicesDetails obj = new ActiveDevicesDetails();
            try
            {
                obj.ActiveDevicesDetailslist = RestClient.GetActiveDevicesDetails();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
            return View(obj);
        }

        [CustAuthFilter]
        public ActionResult ActiveCompanyPunch()
        {
            return View();
        }
        #endregion

        #region PaytimePlans
        [CustAuthFilter]
        public ActionResult Plans()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            Plan pl = new Plan();
            try
            {
                int companyid = Convert.ToInt32(Session["CompanyId"]);
                Getpara obj = new Getpara();
                obj.pkid = companyid;
                obj.para = 1;
                obj.iscount = 0;

                pl.PlanList = RestClient.PlanGetAll(obj);
                return View(pl);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [CustAuthFilter]
        [HttpPost]
        public ActionResult Plans(Plan pl)
        {
            Getpara obj = new Getpara();
            try
            {

                int companyid = Convert.ToInt32(Session["CompanyId"]);

                obj.pkid = companyid;
                obj.para = 1;
                obj.iscount = 0;


                if (ModelState.IsValid)
                {
                    ViewBag.msg = "";
                    ViewBag.error = "";
                    if (pl.PlanId > 0)
                    {
                        pl.IsActive = true;
                        if (!RestClient.PlanUpdate(pl.PlanId, pl))
                        {
                            ViewBag.msg = "Plan not updated due to service issue.";
                        }
                        else
                        {
                            ViewBag.msg = "Plan is updated.";
                        }
                    }
                    else
                    {

                        pl.PlanList = RestClient.PlanGetAll(obj).Where(c => c.Name == pl.Name);
                        if (pl.PlanList.Count() > 0)
                        {
                            ViewBag.error = pl.Name + " Plan already exists.";
                        }
                        else
                        {
                            RestClient.PlanAdd(pl);
                            ViewBag.msg = "Plan added successfully.";
                        }
                    }
                }
                else
                {
                    ViewBag.error = "Not valid entry.";
                }
                pl.PlanList = RestClient.PlanGetAll(obj);
                return View(pl);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [CustAuthFilter]
        [HttpPost]
        public string DeletePlan(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.PlanDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }
        #endregion

        #region Wizards

        [HttpGet]
        [CustAuthFilter]
        public ActionResult SystemSettingWizard()
        {
            CompanyViewModel vmCmp = new CompanyViewModel();
            try
            {
                var data = RestClient.CompanyGetByid(Convert.ToInt32(Session["ClientCompanyId"]));
                vmCmp.CompanyName = Session["cmpname"].ToString();
                vmCmp.CompanyEmail = Session["UserEmail"].ToString();
                ViewBag.gettransasctionYear = RestClient.TransactionYearGetAll();
                ViewBag.TimeZones = ShowTimeZone();
                ViewBag.Countrylist = RestClient.CountryGetAll();
                ViewBag.isStandard = Convert.ToInt32(Session["isStandard"]);
                string _clientDb = Session["DbName"].ToString();
                #region  for FastTrack attendance correcion add filed
                if (!string.IsNullOrEmpty(_clientDb))
                {
                    bool isMatch = Helper.ClientDBNameCheck.IsDbNameMatch(_clientDb);
                    ViewBag.IsMatch = isMatch;
                }
                #endregion
                return View(vmCmp);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [HttpGet]
        [CustAuthFilter]
        public ActionResult CompanySettingWizard()
        {
            CompanySetting cms = new CompanySetting();
            try
            {
                cms = RestClient.GetSystemSettingById(Convert.ToInt32(Session["ClientCompanyId"]));
                return View(cms);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }

        [CustAuthFilter]
        public ActionResult AdminInfo()
        {
            return View();
        }


        [CustAuthFilter]
        [HttpPost]
        public ActionResult AdminInfo(ReqUpdateAdminInfo ad, HttpPostedFileBase EmpPhotoFile)
        {
            try
            {
                string empphoto = "";
                ad.ModifyBy = ad.UserId;
                ad.ModifyDate = DateTime.Now.ToString();
                if (EmpPhotoFile != null)
                {
                    string newfilename = "";
                    var filename = Path.GetFileName(EmpPhotoFile.FileName);
                    string formatedTemplate = "{0}_{1}.{2}";
                    var fileNameWithoutExtension = filename.Split('.')[0];
                    var fileExtension = filename.Split('.')[1];
                    var MaxId = Guid.NewGuid().ToString().Substring(0, 6);
                    newfilename = String.Format(formatedTemplate, fileNameWithoutExtension, MaxId, fileExtension);
                    var path = Path.Combine(Server.MapPath("~/AdminProfile"), newfilename);
                    EmpPhotoFile.SaveAs(path);
                    empphoto = newfilename;
                    ad.Photo = empphoto;
                }
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(ad);
                var i = RestClient.AdminInfoUpdate(ad);
                return RedirectToAction("AdminInfo", "PayTime");
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        #endregion

        #region List-JsonMethods
        [CustAuthFilter]
        [HttpGet]
        public JsonResult EmployeeFilteration(string id, string searchselection)
        {
            JsonResult result;
            var hierarchy = Session["Ishierchy"];
            int hierarchyState = Convert.ToInt32(hierarchy);

            var json = "";
            var alldata = new List<Employees>();
            var jsonSerialiser = new JavaScriptSerializer();
            try
            {
                if (TempData["AllEmp"] == null)
                {
                    alldata = RestClient.EmployeeGetAlls(hierarchyState).ToList();
                }
                else
                {
                    alldata = TempData["AllEmp"] as List<Employees>;
                }

                Employees em = new Employees();
                if (searchselection == "PunchId")
                {
                    var data = alldata.Where(c => c.EmpPunchID == id).FirstOrDefault();
                    if (data == null)
                    {
                        json = jsonSerialiser.Serialize("No Data Found");
                    }
                    else
                    {
                        json = jsonSerialiser.Serialize(data);
                    }

                }
                else if (searchselection == "EmpID")
                {
                    var data1 = alldata.Where(c => c.Empcode.ToLower() == id.ToLower()).FirstOrDefault();
                    if (data1 == null)
                    {
                        json = jsonSerialiser.Serialize("No Data Found");
                    }
                    else
                    {
                        json = jsonSerialiser.Serialize(data1);
                    }

                }
                else
                {
                    var emp = alldata.Where(c => c.EmpName.ToLower().StartsWith(id.ToLower())).FirstOrDefault();
                    if (emp == null)
                    {
                        json = jsonSerialiser.Serialize("No Data Found");
                    }
                    else
                    {
                        json = jsonSerialiser.Serialize(emp);
                    }

                }

                result = Json(json);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

        [HttpGet]
        [CustAuthFilter]
        public JsonResult GetCompanyInfo(int id)
        {
            JsonResult result;
            Companys cs = new Companys();
            try
            {
                cs = RestClient.CompanyGetByid(id);
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(cs);
                result = Json(json);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        [CustAuthFilter]
        public List<SelectListItem> ShowTimeZone()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var timeZone in timeZones)
            {
                //  items.Add(new SelectListItem() { Text = "Test1", Value = "1", Selected = true });
                items.Add(new SelectListItem() { Text = timeZone.DisplayName, Value = timeZone.Id });
            }
            return items;
        }
        #endregion

        #region ForgotPassword
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            ResetPassword rp = new ResetPassword();
            if (TempData["CompanyCode"] != null)
                rp.CompanyCode = TempData["CompanyCode"].ToString();
            return View(rp);
        }
        [HttpPost]
        public ActionResult ForgotPassword(ResetPassword model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(model.Email))
                    {
                        sqlinjectionsearchList.Add("select ");
                        sqlinjectionsearchList.Add("update ");
                        sqlinjectionsearchList.Add("insert ");
                        sqlinjectionsearchList.Add("delete ");
                        sqlinjectionsearchList.Add("drop ");
                        sqlinjectionsearchList.Add("from ");
                        sqlinjectionsearchList.Add("1=1");
                        bool OTPString = sqlinjectionsearchList.Any(word =>
                            model.OTP != null && model.OTP.ToLower().Contains(word));
                        bool UserPassString = sqlinjectionsearchList.Any(word =>
                            model.UserPass != null && model.UserPass.ToLower().Contains(word));
                        bool ConfirmPasswordString = sqlinjectionsearchList.Any(word =>
                            model.ConfirmPassword != null && model.ConfirmPassword.ToLower().Contains(word));
                        if (!OTPString && !UserPassString && !ConfirmPasswordString)
                        {
                            if (!string.IsNullOrEmpty(model.OTP) && !string.IsNullOrEmpty(model.UserPass) &&
                                !string.IsNullOrEmpty(model.ConfirmPassword))
                            {
                                if (Session["guid"] != null)
                                {
                                    resp = AccountRestClint.ResetPassword(model.OTP, Session["guid"].ToString(),
                                    model.UserPass, model.CompanyCode);
                                    // Session["guid"] = null;
                                    ViewBag.MegSts = resp.MegSts;
                                    ViewBag.Meg = resp.Meg;
                                }
                                else
                                {
                                    return RedirectToAction("LoginPage", "PayTime");
                                }

                            }
                            else
                            {
                                resp = AccountRestClint.ForgotPassword(model.Email, model.CompanyCode);
                                if (resp.MegSts == "ok")
                                {
                                    Session["guid"] = resp.Meg.ToString();
                                    resp.Meg = "OTP sent on your registered email.";
                                    TempData["Otp"] = resp.Meg.ToString();
                                    ViewBag.Otp = resp.Meg.ToString();
                                }

                                ViewBag.MegSts = resp.MegSts;
                                ViewBag.Meg = resp.Meg;
                            }
                        }
                        else
                        {
                            ViewBag.MegSts = "Error";
                        }
                    }
                    else
                    {
                        ViewBag.MegSts = "Error";

                    }
                }
                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        // forgot password for employee
        [HttpGet]
        public ActionResult ForgotPasswordForEmployee()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPasswordForEmployee(ResetPassword model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(model.Email))
                    {
                        sqlinjectionsearchList.Add("select ");
                        sqlinjectionsearchList.Add("update ");
                        sqlinjectionsearchList.Add("insert ");
                        sqlinjectionsearchList.Add("delete ");
                        sqlinjectionsearchList.Add("drop ");
                        sqlinjectionsearchList.Add("from ");
                        sqlinjectionsearchList.Add("1=1");
                        bool OTPString = sqlinjectionsearchList.Any(word =>
                            model.OTP != null && model.OTP.ToLower().Contains(word));
                        bool UserPassString = sqlinjectionsearchList.Any(word =>
                            model.UserPass != null && model.UserPass.ToLower().Contains(word));
                        bool ConfirmPasswordString = sqlinjectionsearchList.Any(word =>
                            model.ConfirmPassword != null && model.ConfirmPassword.ToLower().Contains(word));
                        if (!OTPString && !UserPassString && !ConfirmPasswordString)
                        {
                            if (!string.IsNullOrEmpty(model.OTP) && !string.IsNullOrEmpty(model.UserPass) &&
                                !string.IsNullOrEmpty(model.ConfirmPassword))
                            {
                                if (Session["guid"] != null)
                                {
                                    resp = AccountRestClint.ResetPassword(model.OTP, Session["guid"].ToString(),
                                    model.UserPass, model.CompanyCode);
                                    // Session["guid"] = null;
                                    ViewBag.MegSts = resp.MegSts;
                                    ViewBag.Meg = resp.Meg;
                                }
                                else
                                {
                                    return RedirectToAction("LoginPage", "PayTime");
                                }
                            }
                            else
                            {
                                resp = AccountRestClint.ForgotPassword(model.Email, model.CompanyCode);
                                if (resp.MegSts == "ok")
                                {
                                    Session["guid"] = resp.Meg.ToString();
                                    resp.Meg = "OTP sent on your registered email.";
                                    TempData["Otp"] = resp.Meg.ToString();
                                    ViewBag.Otp = resp.Meg.ToString();
                                }

                                ViewBag.MegSts = resp.MegSts;
                                ViewBag.Meg = resp.Meg;
                            }
                        }
                        else
                        {
                            ViewBag.MegSts = "Error";
                        }
                    }
                    else
                    {
                        ViewBag.MegSts = "Error";

                    }
                }
                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        #endregion

        #region EmployeeEditProfile
        [CustAuthFilter]
        public ActionResult EmpChangePassword()
        {
            ViewBag.IsEss = Convert.ToBoolean(RestClient.GetSystemSettingById(1).IsEss.ToString() != "" ? RestClient.GetSystemSettingById(1).IsEss.ToString() : "0");
            ViewBag.Getsystemsettingdateformatupdated = RestClient.CompanySettingGetAll();
            return View();
        }


        [HttpPost]
        [CustAuthFilter]
        public ActionResult EmpChangePassword(string Password, string NewPassword, string UserEmail, int EmpId, int RoleId)
        {
            try
            {

                ViewBag.IsEss = Convert.ToBoolean(RestClient.GetSystemSettingById(1).IsEss.ToString() != "" ? RestClient.GetSystemSettingById(1).IsEss.ToString() : "0");
                ViewBag.Getsystemsettingdateformatupdated = RestClient.CompanySettingGetAll();
                resp = AccountRestClint.EmployeeResetPassword(Password, NewPassword, UserEmail, EmpId, RoleId);
                if (resp.MegSts.ToLower() == "ok")
                {
                    ViewBag.msg = resp.Meg;
                }
                else
                {
                    ViewBag.error = resp.Meg;
                    ViewBag.flag = "1";
                }


                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        [HttpGet]
        [CustAuthFilter]
        public JsonResult EmployeebyID(int id)
        {
            JsonResult result;
            try
            {
                Employees emp = new Employees();
                emp.EmployeeList = RestClient.EmployeeGet(id);
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(emp.EmployeeList);
                result = Json(json);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }


        [HttpPost]
        [CustAuthFilter]
        public JsonResult EmpChangeInfo(string EmpDetails)
        {
            JsonResult result;
            JavaScriptSerializer js = new JavaScriptSerializer();
            EmpInfo objEmp = js.Deserialize<EmpInfo>(EmpDetails);
            var i = RestClient.EmpInfoUpdate(objEmp);
            Session["EmpName"] = objEmp.EmpName;
            result = Json(i);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        [CustAuthFilter]
        public ActionResult EmpChangeProfilePhoto(int EmpId, string EmpPhoto, string EmpPhotoData, HttpPostedFileBase EmpPhotoFile)
        {
            try
            {
                EmpImg empi = new EmpImg();
                empi.EmpId = EmpId;
                empi.ModifyBy = EmpId;
                empi.ModifyDate = DateTime.Now.ToString();
                var accountcode = Session["cmpcode"].ToString();
                if (!string.IsNullOrEmpty(EmpPhoto))
                {
                    byte[] imageBytes = Convert.FromBase64String(EmpPhoto);
                    // Generate a unique filename
                    var FileName = Guid.NewGuid().ToString().Substring(0, 6);
                    var photoname = FileName + "_" + accountcode;
                    string empphotofilename = photoname + ".jpg";
                    string originalFilePath = Server.MapPath("~/UploadEmpPhoto/" + empphotofilename);

                    /// Save original byte array as an image file
                    System.IO.File.WriteAllBytes(originalFilePath, imageBytes);
                    empi.EmpPhoto = empphotofilename;
                    var i = RestClient.EmpImageUpdate(empi);
                    Session["photo"] = empi.EmpPhoto;
                }
                if (!string.IsNullOrEmpty(EmpPhotoData))
                {
                    // Convert base64 string to byte array
                    byte[] imageBytes = Convert.FromBase64String(EmpPhotoData);

                    // Generate a unique filename
                    var FileName = Guid.NewGuid().ToString().Substring(0, 6);
                    var photoname = FileName + "_" + accountcode;
                    string empphotofilename = photoname + ".jpg";
                    string originalFilePath = Server.MapPath("~/UploadEmpPhoto/" + empphotofilename);

                    // Save original byte array as an image file
                    System.IO.File.WriteAllBytes(originalFilePath, imageBytes);


                    empi.EmpPhoto = empphotofilename;
                    var i = RestClient.EmpImageUpdate(empi);
                    Session["photo"] = empi.EmpPhoto;

                }
                if (EmpPhotoFile != null)
                {
                    byte[] thePictureAsBytes = new byte[EmpPhotoFile.ContentLength];
                    using (BinaryReader theReader = new BinaryReader(EmpPhotoFile.InputStream))
                    {
                        thePictureAsBytes = theReader.ReadBytes(EmpPhotoFile.ContentLength);
                    }

                    var FileName = Guid.NewGuid().ToString().Substring(0, 6);
                    var photoname = FileName + "_" + accountcode;
                    string empphotofilename = photoname + ".jpg";
                    string originalFilePath = Server.MapPath("~/UploadEmpPhoto/" + empphotofilename);

                    // Save original byte array as an image file
                    System.IO.File.WriteAllBytes(originalFilePath, thePictureAsBytes);


                    empi.EmpPhoto = empphotofilename;
                    var i = RestClient.EmpImageUpdate(empi);
                    Session["photo"] = empi.EmpPhoto;
                }
                return RedirectToAction("EmpChangePassword", "PayTime");
            }
            catch (Exception)
            {

                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        #endregion

        #region AdminEditProfile
        [CustAuthFilter]
        public ActionResult AdminEditProfile()
        {
            return View();
        }
        [CustAuthFilter]
        [HttpPost]
        public ActionResult AdminEditProfile(ReqUpdateAdminInfo ad, HttpPostedFileBase EmpPhotoFile)
        {
            try
            {
                string empphoto = "";
                ad.ModifyBy = ad.UserId;
                ad.ModifyDate = DateTime.Now.ToString();
                //to remove the + on an input in amobile no filed 
                ad.CountryCode = ad.CountryCode;
                if (EmpPhotoFile != null)
                {
                    string newfilename = "";
                    var filename = Path.GetFileName(EmpPhotoFile.FileName);
                    string formatedTemplate = "{0}_{1}.{2}";
                    var fileNameWithoutExtension = filename.Split('.')[0];
                    var fileExtension = filename.Split('.')[1];
                    var MaxId = Guid.NewGuid().ToString().Substring(0, 6);
                    newfilename = String.Format(formatedTemplate, fileNameWithoutExtension, MaxId, fileExtension);
                    var path = Path.Combine(Server.MapPath("~/AdminProfile"), newfilename);
                    EmpPhotoFile.SaveAs(path);
                    empphoto = newfilename;
                    ad.Photo = empphoto;
                    Session["photo"] = ad.Photo;
                }
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(ad);

                var i = RestClient.AdminInfoUpdate(ad);
                if (i == true)
                {
                    ad.hdnDealerCode = ad.DealerCode;
                    ViewBag.msg = "Profile updated successfully.";
                }
                else
                {
                    ViewBag.error = "Error in profile update.";
                }

                //return RedirectToAction("AdminEditProfile", "PayTime");
                return RedirectToAction("AdminEditProfile");
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [HttpGet]
        [CustAuthFilter]
        public JsonResult AdminInfoByID(int id)
        {
            JsonResult result;
            try
            {
                ReqUpdateAdminInfo admin = new ReqUpdateAdminInfo();
                admin.admininfolist = RestClient.AdminGet(id);
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(admin.admininfolist);
                result = Json(json);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

        [HttpPost]
        [CustAuthFilter]
        public ActionResult AdminChangePassword(string Password, string NewPassword, string RePassword, string UserEmail, string UserId)
        {
            try
            {
                string concatstring = "";
                //logic for encrypt password
                string EncryptionKey = ConfigurationManager.AppSettings.Get("jwtKey");
                byte[] clearBytes = Encoding.Unicode.GetBytes(UserId + "$" + UserEmail + "$" + Password + "$" + NewPassword + "$" + RePassword);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        concatstring = Convert.ToBase64String(ms.ToArray());
                    }
                    resp = AccountRestClint.AdminResetPassword(concatstring);
                    if (resp.MegSts.ToLower() == "ok")
                    {
                        TempData["msg"] = resp.Meg;
                        TempData.Keep("msg");
                    }
                    else
                    {
                        TempData["error"] = resp.Meg;
                        TempData.Keep("error");
                        //  ViewBag.error = resp.Meg;
                        TempData["flag"] = "1";
                        TempData.Keep("flag");
                        //  ViewBag.flag = "1";
                    }
                    return RedirectToAction("AdminEditProfile", "PayTime");
                    //return View();
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        #endregion

        #region HR Policy
        [CustAuthFilter]
        [HttpGet]
        public ActionResult HRPolicy()
        {

            IEnumerable<HRPolicy> Hrpolicylst = null;
            //ViewBag.CompanyList = FillCompany();

            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
            {
                Hrpolicylst = RestClient.HRPolicyGetAll();
            }
            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                Hrpolicylst = RestClient.HRPolicyGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
                if (Session["RoleId"].ToString() == "6806")
                {
                    Hrpolicylst = RestClient.HRPolicyGetAll().Where(x => (x.CompanyID == cmpid && (x.BranchID == BranchID || x.BranchID == 0)) || (x.CompanyID == 0));
                }
            }

            ViewBag.PolicyGetAll = Hrpolicylst;

            return View();
        }
        [CustAuthFilter]
        [HttpPost]
        public ActionResult HRPolicy(HRPolicy hr)
        {
            ErrorMsg err = new ErrorMsg();

            int cmpid = 0;
            int BranchID = 0;

            if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                BranchID = Convert.ToInt32(Session["BranchId"].ToString());
            }

            if (hr.emptempweekoff != null)
            {
                hr.EmpSecondWeekOffRule = string.Join(",", hr.emptempweekoff);
            }
            else
            {
                hr.EmpSecondWeekOffRule = "0";
            }


            if (hr.emphalfdayoff != null)
            {
                hr.EmpHalfDayRule = string.Join(",", hr.emphalfdayoff);
            }
            else
            {
                hr.EmpHalfDayRule = "0";
            }

            hr.CompanyID = cmpid;
            hr.BranchID = BranchID;
            hr.CreatedDate = DateTime.Now.ToString();
            hr.CreatedBy = 1;
            hr.ModifyDate = DateTime.Now.ToString();
            hr.IsActive = 1;
            hr.roleid = Convert.ToInt32(Session["RoleId"]);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(hr);
            if (hr.PolicyId > 0)
            {
                hr.IsActive = 1;
                if (!RestClient.HRPolicyUpdate(hr.PolicyId, hr))
                {
                    ViewBag.msg = "Policy not updated due to service issue.";
                }
                else
                {
                    ViewBag.success = "true";
                    ViewBag.msg = "Policy updated successfully.";
                }
            }
            else
            {
                hr.HRPolicylist = RestClient.HRPolicyGetAll().Where(c => c.PolicyName == hr.PolicyName);
                if (hr.HRPolicylist.Count() > 0)
                {
                    ViewBag.error = hr.PolicyName + " Policy already exists.";
                }
                else
                {

                    err = RestClient.HRPolicyAdd(hr);
                    if (err.MegSts.ToUpper() == "OK")
                    {
                        ViewBag.success = "true";
                        ViewBag.msg = "Policy added successfully.";
                    }
                    else
                    {
                        ViewBag.error = "Please enter proper value.";
                    }

                }
            }

            IEnumerable<HRPolicy> Hrpolicylst = null;
            //ViewBag.CompanyList = FillCompany();

            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
            {
                Hrpolicylst = RestClient.HRPolicyGetAll();
            }
            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                Hrpolicylst = RestClient.HRPolicyGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
                if (Session["RoleId"].ToString() == "6806")
                {
                    Hrpolicylst = RestClient.HRPolicyGetAll().Where(x => x.CompanyID == cmpid && x.BranchID == BranchID || x.CompanyID == 0);
                }
            }

            ViewBag.PolicyGetAll = Hrpolicylst;
            return View();
        }
        [CustAuthFilter]
        public List<SelectListItem> FillCompany()
        {
            Companys com = new Companys();
            IEnumerable<Companys> comList;
            List<SelectListItem> list = new List<SelectListItem>();
            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
            {
                comList = RestClient.CompanyGetAll();
                foreach (var i in comList)
                {
                    list.Add(new SelectListItem() { Text = i.CompanyName, Value = Convert.ToString(i.CompanyID) });
                }
            }
            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                comList = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                foreach (var i in comList)
                {
                    list.Add(new SelectListItem() { Text = i.CompanyName, Value = Convert.ToString(i.CompanyID) });
                }
            }

            return list;
        }

        [HttpGet]
        [CustAuthFilter]
        public JsonResult FillPolicyListbyID(int id)
        {
            JsonResult result;
            try
            {
                var i = RestClient.HrPolicyGetbyID(id);
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(i);
                result = Json(json);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }

        }

        [HttpGet]
        [CustAuthFilter]
        public ActionResult HRPolicyAllocation()
        {
            reqPolicydata rpd = new reqPolicydata();
            rpd.comidlst = Convert.ToString(1);
            rpd.branchidlst = Convert.ToString(0);
            rpd.desigidlst = Convert.ToString(0);
            rpd.deptidlst = Convert.ToString(0);
            rpd.roleidlst = Convert.ToString(0);
            rpd.empidlst = Convert.ToString(0);
            rpd.policyid = 0;

            IEnumerable<HRPolicy> Hrpolicylst = null;
            IEnumerable<resPolicyData> HrpolicyAllocation = null;
            IEnumerable<Companys> cmplist = null;
            IEnumerable<Departments> departmentlist = null;
            IEnumerable<Designations> desclist = null;
            IEnumerable<Roles> Roleslist = null;
            //ViewBag.CompanyList = FillCompany();

            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) == 1)
            {
                Hrpolicylst = RestClient.HRPolicyGetAll();
                HrpolicyAllocation = RestClient.GetAllPolicyAllocation(rpd);
                cmplist = RestClient.CompanyGetAll();
                departmentlist = RestClient.DepartmentGetAll();
                desclist = RestClient.DesignationsGetAll();
                Roleslist = RestClient.RoleGetAll();

            }
            else if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && !(Convert.ToInt32(Session["RoleId"].ToString()) > 1))
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                Hrpolicylst = RestClient.HRPolicyGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
                HrpolicyAllocation = RestClient.GetAllPolicyAllocation(rpd).Where(x => x.CompanyId == cmpid);
                cmplist = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                departmentlist = RestClient.DepartmentGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
                desclist = RestClient.DesignationsGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
                Roleslist = RestClient.RoleGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
            }
            else if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) > 1)
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                Hrpolicylst = RestClient.HRPolicyGetAll().Where(x => x.CompanyID == cmpid && x.BranchID == BranchID || x.CompanyID == 0);
                HrpolicyAllocation = RestClient.GetAllPolicyAllocation(rpd).Where(x => x.CompanyId == cmpid && x.BranchId == BranchID);
                cmplist = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                departmentlist = RestClient.DepartmentGetAll().Where(x => x.CompanyID == cmpid && x.BranchID == BranchID || x.CompanyID == 0);
                desclist = RestClient.DesignationsGetAll().Where(x => x.CompanyID == cmpid && x.BranchID == BranchID || x.CompanyID == 0);
                Roleslist = RestClient.RoleGetAll().Where(x => x.CompanyID == cmpid && x.BranchID == BranchID || x.CompanyID == 0);
            }
            else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                Hrpolicylst = RestClient.HRPolicyGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
                HrpolicyAllocation = RestClient.GetAllPolicyAllocation(rpd).Where(x => x.CompanyId == cmpid);
                cmplist = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                departmentlist = RestClient.DepartmentGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
                desclist = RestClient.DesignationsGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
                Roleslist = RestClient.RoleGetAll().Where(x => x.CompanyID == cmpid || x.CompanyID == 0);
                if (Session["RoleId"].ToString() == "6806")
                {
                    Hrpolicylst = RestClient.HRPolicyGetAll().Where(x => x.CompanyID == cmpid && x.BranchID == BranchID || x.CompanyID == 0);
                    HrpolicyAllocation = RestClient.GetAllPolicyAllocation(rpd).Where(x => x.CompanyId == cmpid && x.BranchId == BranchID);
                    departmentlist = RestClient.DepartmentGetAll().Where(x => x.CompanyID == cmpid && x.BranchID == BranchID || x.CompanyID == 0);
                    desclist = RestClient.DesignationsGetAll().Where(x => x.CompanyID == cmpid && x.BranchID == BranchID || x.CompanyID == 0);
                    Roleslist = RestClient.RoleGetAll().Where(x => x.CompanyID == cmpid && x.BranchID == BranchID || x.CompanyID == 0);
                }
            }


            ViewBag.PolicyGetAll = HrpolicyAllocation;
            ViewBag.PolicyName = Hrpolicylst;
            ViewBag.Companyslist = cmplist;
            ViewBag.DepartmentList = departmentlist;
            ViewBag.DesignationList = desclist;
            ViewBag.RoleList = Roleslist;
            return View();
        }


        [HttpPost]
        [CustAuthFilter]
        public JsonResult ApplyPolicyToEmployee(string comidlst, string branchidlst, string desigidlst, string deptidlst, string roleidlst, string empidlst, string policyid)
        {
            JsonResult result;
            reqPolicydata upe = new reqPolicydata();
            upe.comidlst = comidlst;
            upe.branchidlst = branchidlst;
            upe.desigidlst = desigidlst;
            upe.deptidlst = deptidlst;
            upe.roleidlst = roleidlst;
            upe.empidlst = empidlst;
            upe.policyid = Convert.ToInt32(policyid);
            try
            {
                var i = RestClient.HRPolicyAllocationUpdate(upe);
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(i);
                result = Json(json);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                ViewBag.msg = "Policy applied successfully.";
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

        [CustAuthFilter]
        [HttpPost]
        public JsonResult ApplyPolicyView(string comidlst, string branchidlst, string desigidlst, string deptidlst, string roleidlst, string empidlst, string policyid)
        {
            if (comidlst != "" && branchidlst != "" && desigidlst != "" && deptidlst != "" && roleidlst != "" && empidlst != "")
            {
                reqPolicydata rpd = new reqPolicydata();
                rpd.comidlst = comidlst;
                rpd.branchidlst = branchidlst;
                rpd.desigidlst = desigidlst;
                rpd.deptidlst = deptidlst;
                rpd.roleidlst = roleidlst;
                rpd.empidlst = empidlst;
                rpd.policyid = Convert.ToInt32(policyid);
                var lst = RestClient.GetAllPolicyAllocation(rpd);
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        [CustAuthFilter]
        public ActionResult HRPolicyDownloadFormat()
        {
            return View();
        }

        //public byte[] GetPDF(string pHTML)
        //{
        //    byte[] bPDF = null;

        //    MemoryStream ms = new MemoryStream();
        //    TextReader txtReader = new StringReader(pHTML);

        //    // 1: create object of a itextsharp document class
        //    iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25, 25, 25, 25);

        //    // 2: we create a itextsharp pdfwriter that listens to the document and directs a XML-stream to a file
        //    PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);

        //    // 3: we create a worker parse the document
        //    iTextSharp.text.html.simpleparser.HTMLWorker htmlWorker = new iTextSharp.text.html.simpleparser.HTMLWorker(doc);

        //    // 4: we open document and start the worker on the document
        //    doc.Open();
        //    htmlWorker.StartDocument();

        //    // 5: parse the html into the document
        //    htmlWorker.Parse(txtReader);

        //    // 6: close the document and the worker
        //    htmlWorker.EndDocument();
        //    htmlWorker.Close();
        //    doc.Close();

        //    bPDF = ms.ToArray();

        //    return bPDF;
        //}
        //public void DownloadPDF()
        //{
        //    string HTMLContent = "Hello <b>World</b>";

        //    Response.Clear();
        //    Response.ContentType = "application/pdf";
        //    Response.AddHeader("content-disposition", "attachment;filename=" + "PDFfile.pdf");
        //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    Response.BinaryWrite(GetPDF(HTMLContent));
        //    Response.End();
        //}
        //[HttpPost]
        //public FileResult DownloadAttachment(int PolicyId, string DownloadType)
        //{
        //    var i = RestClient.HrPolicyGetbyID(PolicyId);
        //    var jsonSerialiser = new JavaScriptSerializer();
        //    var json = jsonSerialiser.Serialize(i);
        //    Utility obj = new Utility();
        //    DataTable dt = obj.JsonStringToDataTable(json);
        //    string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        //    string pathDownload = Path.Combine(pathUser, "Downloads");
        //    string path = pathDownload + "Policy_+" + PolicyId + "" + DateTime.Now.Date.ToString("yyyy-mm-dd") + ".xls";
        //    obj.CreateXlsFile(dt, path);

        //    return File(mem, "application/vnd.ms-excel", "WidgetData.xlsx");
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(path);
        //    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, path);

        //}

        [HttpPost]
        // [CustAuthFilter]
        public string DownloadPolicy(int PolicyId, string DownloadType)
        {

            // JsonResult result;
            try
            {


                var i = RestClient.HrPolicyGetbyID(PolicyId);
                i.EmpSecondWeekOffRule = i.EmpSecondWeekOffRule.Replace(',', '~');
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(i);

                if (DownloadType == "PDF")
                {
                    return "data";
                    //return Json(new { data = "", JsonRequestBehavior.AllowGet });
                }
                else if (DownloadType == "Excel")
                {
                    Utility obj = new Utility();
                    //JavaScriptSerializer js = new JavaScriptSerializer();
                    //HRPolicy objHrPolicy = js.Deserialize<HRPolicy>(json);
                    DataTable dt = obj.JsonStringToDataTable(json);
                    //  DataTable dt = obj.JsonStringToDataTable(objHrPolicy);
                    string path = Server.MapPath("~/Downloads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Downloads"));
                    }
                    string locationpath = "Policy_" + PolicyId + "_" + DateTime.Now.Date.ToString("yyyy-mm-dd") + ".xls";
                    string filename = path + locationpath;
                    obj.CreateXlsFile(dt, filename);
                    //obj.CreateCSVFile(dt, filename);
                    return locationpath;
                }
                else if (DownloadType == "CSV")
                {
                    Utility obj = new Utility();
                    DataTable dt = obj.JsonStringToDataTable(json);
                    string path = Server.MapPath("~/Downloads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Downloads"));
                    }
                    string locationpath = "Policy_" + PolicyId + "_" + DateTime.Now.Date.ToString("yyyy-mm-dd") + ".csv";
                    string filename = path + locationpath;
                    obj.CreateCSVFile(dt, filename);
                    //  obj.CreateXlsFile(dt, filename);
                    return locationpath;
                }
                else
                {
                    return "Please Select Proper Input";
                }
            }
            catch (Exception ex)
            {
                //string em = ex.ToString();
                //bool IsSend = false;
                //var SMTPDetails = accrestclient.EmailconfigurationGetAll().Where(x => x.IsActive == true).FirstOrDefault();
                //IsSend = ReportMailDetails(SMTPDetails, "charmi.patel@mantratec.com", em);
                return "/PayTime/ErrorPage";
            }
        }

        //public bool ReportMailDetails(EmailConfiguration SMTPDetails, string lstSendTo, string em)
        //{
        //    List<string> reports = new List<string>();
        //    string path = Server.MapPath("~/Downloads/");
        //    string locationpath ="Policy_1_2017-00-28.txt";
        //    string filename = path + locationpath;
        //    reports.Add(filename);
        //    bool IsmailSend = false;
        //    if (SMTPDetails != null && lstSendTo != null)
        //    {
        //            MailDetails mailDetails = new MailDetails();
        //            mailDetails.SMTPHost = SMTPDetails.SMTPIP;
        //            mailDetails.SMTPPort = SMTPDetails.SMTPPORT;
        //            mailDetails.CredentialEmailId = SMTPDetails.CredentialEmailID;
        //            mailDetails.CredentialPassword = SMTPDetails.CredentialEmailIDPassword;
        //            mailDetails.fromEmail = SMTPDetails.FromEmailID;
        //            mailDetails.Isattchement = true;
        //            mailDetails.AttachementPath = new List<string>();
        //           mailDetails.AttachementPath.AddRange(reports);
        //            mailDetails.MailBody = "<Html><Head></head><body><p>Exception Is: "+em+",</p></body></Html>";
        //            mailDetails.Subject = "Reports";
        //            mailDetails.ToEmail = lstSendTo;
        //            IsmailSend = accrestclient.SendReportMail(mailDetails);
        //        return IsmailSend;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public void DownloadTextPolicy(string PolicyId)
        {
            var i = RestClient.HrPolicyGetbyID(Convert.ToInt32(PolicyId));
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(i);

            string path = Server.MapPath("~/Downloads/");
            string locationpath = "Policy_" + PolicyId + "_" + DateTime.Now.Date.ToString("yyyy-mm-dd") + ".txt";
            string filename = path + locationpath;

            string sFileName = System.IO.Path.GetRandomFileName();
            string sGenName = "HRPolicy.txt";

            //YOu could omit these lines here as you may
            //not want to save the textfile to the server
            //I have just left them here to demonstrate that you could create the text file
            using (System.IO.StreamWriter SW = new System.IO.StreamWriter(
                   Server.MapPath("~/Downloads/" + locationpath)))
            {
                SW.WriteLine(json);
                SW.Close();
            }

            System.IO.FileStream fs = null;
            fs = System.IO.File.Open(Server.MapPath("~/Downloads/" + locationpath), System.IO.FileMode.Open);
            byte[] btFile = new byte[fs.Length];
            fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            Response.AddHeader("Content-disposition", "attachment; filename=" + sGenName);
            Response.ContentType = "application/octet-stream";
            Response.BinaryWrite(btFile);
            Response.End();
        }

        public void DownloadXMLPolicy(string PolicyId)
        {
            var i = RestClient.HrPolicyGetbyID(Convert.ToInt32(PolicyId));
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(i);

            string path = Server.MapPath("~/Downloads/");
            string locationpath = "Policy_" + PolicyId + "_" + DateTime.Now.Date.ToString("yyyy-mm-dd") + ".xml";
            string filename = path + locationpath;

            XmlDocument doc = JsonConvert.DeserializeXmlNode(json, "Policy");
            doc.Save(filename);

            string strFullPath = Server.MapPath("~/Downloads/" + locationpath);
            //string strFullPath = Server.MapPath(filename);
            string strContents = null;
            System.IO.StreamReader objReader = default(System.IO.StreamReader);
            objReader = new System.IO.StreamReader(strFullPath);
            strContents = objReader.ReadToEnd();
            objReader.Close();

            string attachment = "attachment; filename=HRPolicy.xml";
            Response.ClearContent();
            Response.ContentType = "application/xml";
            Response.AddHeader("content-disposition", attachment);
            Response.Write(strContents);
            Response.End();
        }




        //[HttpPost]
        //[ValidateInput(false)]
        //public FileResult Export(string GridHtml)
        //{
        //    using (MemoryStream stream = new System.IO.MemoryStream())
        //    {
        //        StringReader sr = new StringReader(GridHtml);
        //        Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
        //        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
        //        pdfDoc.Open();
        //        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
        //        pdfDoc.Close();
        //        return File(stream.ToArray(), "application/pdf", "StudentDetails.pdf");
        //    }
        //}


        #endregion

        public ActionResult ErrorPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaveHierarchyLevelState(int empid, int Ishierchy)
        {

            JsonResult result;
            try
            {

                var emphierarchy = RestClient.changeemphierarchy(empid, Ishierchy);
                Session["Ishierchy"] = Ishierchy;
                result = Json(emphierarchy);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            return View();

        }
        [CustAuthFilter]
        [HttpGet]
        public ActionResult SamplePolicy(int id)
        {
            try
            {
                HRPolicy hrp = new HRPolicy();
                ViewBag.policydata = RestClient.SampleHrPolicyGetbyID(id);
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        #region sample hrpolicyaudit
        [CustAuthFilter]
        [HttpGet]
        public ActionResult SamplePolicyAuditData(int id)
        {
            try
            {
                HrpolicyAudit hrp = new HrpolicyAudit();
                ViewBag.policydata = RestClient.SampleHrPolicyAuditData(id);
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        #endregion


        #region Templates
        [HttpGet]
        public ActionResult Templates()
        {
            try
            {
                ViewBag.templateGetall = RestClient.TemplateGetAll();
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [HttpGet]
        public ActionResult TemplateShift()
        {
            try
            {
                ViewBag.templateGetall = RestClient.TemplateGetAll();
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [HttpGet]
        public ActionResult TemplateRole()
        {
            try
            {
                ViewBag.templateGetall = RestClient.TemplateGetAll();
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [CustAuthFilter]
        [HttpPost]
        public JsonResult ApplyTemplate(int TemplateId)
        {
            ErrorMsg err = new ErrorMsg();
            string res = "";
            string policy = "";
            string shift = "";
            string role = "";
            string msg = "";
            try
            {
                if (TemplateId == 4)
                {
                    HRPolicy hrp = new HRPolicy();
                    hrp.PolicyName = "Basic Policy";
                    hrp.EmpWeekOff = 1;
                    hrp.EmpSecondWeekOff = 7;
                    hrp.EmpSecondWeekOffRule = "2";
                    hrp.EmpHalfDay = 0;
                    hrp.EmpHalfDayRule = "0";
                    hrp.EmpAllowOT = 1;
                    hrp.MaxOutDur = 1;
                    hrp.TimeBetPunch = 1;
                    hrp.GraceLateIN = 5;
                    hrp.GraceEarlyOUT = 5;
                    hrp.ApplicationMode = "AUTO";
                    hrp.MonthlyRptStaDay = 1;
                    hrp.WeekOffOTHour = 2;
                    hrp.HolidayOffOTHour = 2;
                    hrp.OTMinHour = 1;
                    hrp.OTFormula = 4;
                    hrp.OTAllowWO = 1;
                    hrp.OTAllowHO = 1;
                    hrp.NotPresentInMnth = 0;
                    hrp.AllErrorCase = 0;
                    hrp.ErrorCaseWH = 0;
                    hrp.HdLate = 0;
                    hrp.HdEd = 0;
                    hrp.HdLateorEd = 0;
                    hrp.ALate = 2;
                    hrp.AED = 0;
                    hrp.ALateorED = 0;
                    hrp.TotalHrafterStatus = 0;
                    hrp.TotalHrbeforeStatus = 0;
                    hrp.LateInMin = 1;
                    hrp.EDMin = 1;
                    hrp.IsActive = 1;
                    hrp.CreatedBy = 1;
                    hrp.CreatedDate = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                    hrp.ModifyBy = 0;
                    hrp.ModifyDate = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                    if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                    {
                        hrp.CompanyID = 0;
                        hrp.BranchID = 0;
                    }
                    else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                    {
                        var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                        var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                        hrp.CompanyID = cmpid;
                        hrp.BranchID = BranchID;
                    }
                    err = RestClient.HRPolicyAdd(hrp);
                    policy = err.Meg;


                    Shifts sh = new Shifts();
                    sh.ShiftName = "9to6:30 Shift";
                    sh.ShiftShortName = "9T6";
                    sh.ShiftGroupId = 0;
                    sh.StartTime = "09:00";
                    //DateTime.ParseExact("09:00:00", "hh:mm:ss");
                    sh.EndTime = "18:30";
                    if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                    {
                        sh.GraceBefore = 0;
                        sh.GraceAfter = 0;
                        sh.roleid = Convert.ToInt32(Session["RoleId"]);

                    }
                    else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                    {
                        var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                        var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                        sh.GraceBefore = cmpid;
                        sh.GraceAfter = BranchID;
                        sh.roleid = Convert.ToInt32(Session["RoleId"]);
                    }
                    sh.ShiftDur = "9:30";
                    sh.MinHrsFullDay = "8:30";
                    sh.MinHrsHalfDay = "4:00";
                    sh.RecessDur = "1:00";
                    sh.SmsScheduleTime = "2:00";
                    sh.IsActive = true;
                    err = RestClient.ShiftAdd(sh);
                    shift = err.Meg;
                    if (policy == "Policy Created Successfully." && shift == "Shift created Successfully.")
                    {
                        res = "Template Added Successfully";
                    }
                    else if (policy == "Policy Created Successfully." && shift == "Shift already exists.")
                    {
                        res = "Template Added Successfully";
                    }
                    else if (policy == "Policy Already exists." && shift == "Shift created Successfully.")
                    {
                        res = "Template Added Successfully";
                    }
                    else if (policy == "Policy Already exists." && shift == "Shift already exists.")
                    {
                        res = "Template Already Applied";
                    }
                    else
                    {
                        res = "Error on Template applied !";
                    }
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                if (TemplateId == 6)
                {
                    HRPolicy hrp = new HRPolicy();
                    hrp.PolicyName = "Advance Policy";
                    hrp.EmpWeekOff = 2;
                    hrp.EmpSecondWeekOff = 3;
                    hrp.EmpSecondWeekOffRule = "2";
                    hrp.EmpHalfDay = 0;
                    hrp.EmpHalfDayRule = "0";
                    hrp.EmpAllowOT = 1;
                    hrp.MaxOutDur = 12;
                    hrp.TimeBetPunch = 21;
                    hrp.GraceLateIN = 21;
                    hrp.GraceEarlyOUT = 21;
                    hrp.ApplicationMode = "AUTO";
                    hrp.MonthlyRptStaDay = 1;
                    hrp.WeekOffOTHour = 0;
                    hrp.HolidayOffOTHour = 56;
                    hrp.OTMinHour = 5;
                    hrp.OTFormula = 1;
                    hrp.OTAllowWO = 0;
                    hrp.OTAllowHO = 1;
                    hrp.NotPresentInMnth = 1;
                    hrp.AllErrorCase = 1;
                    hrp.ErrorCaseWH = 1;
                    hrp.HdLate = 0;
                    hrp.HdEd = 0;
                    hrp.HdLateorEd = 0;
                    hrp.ALate = 0;
                    hrp.AED = 0;
                    hrp.ALateorED = 0;
                    hrp.TotalHrafterStatus = 0;
                    hrp.TotalHrbeforeStatus = 0;
                    hrp.LateInMin = 0;
                    hrp.EDMin = 0;
                    hrp.IsActive = 1;
                    hrp.CreatedBy = 1;
                    hrp.CreatedDate = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                    hrp.ModifyBy = 0;
                    hrp.ModifyDate = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                    if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                    {
                        hrp.CompanyID = 0;
                        hrp.BranchID = 0;
                    }
                    else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                    {
                        var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                        var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                        hrp.CompanyID = cmpid;
                        hrp.BranchID = BranchID;
                    }
                    err = RestClient.HRPolicyAdd(hrp);
                    policy = err.Meg;

                    Shifts sh = new Shifts();
                    sh.ShiftName = "10to8 Shift";
                    sh.ShiftShortName = "1T8";
                    sh.ShiftGroupId = 0;
                    sh.StartTime = "10:00";
                    sh.EndTime = "19:00";
                    if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                    {
                        sh.GraceBefore = 0;
                        sh.GraceAfter = 0;
                        sh.roleid = Convert.ToInt32(Session["RoleId"]);
                    }
                    else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                    {
                        var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                        var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                        sh.GraceBefore = cmpid;
                        sh.GraceAfter = BranchID;
                        sh.roleid = Convert.ToInt32(Session["RoleId"]);
                    }
                    sh.ShiftDur = "10:00";
                    sh.MinHrsFullDay = "09:00";
                    sh.MinHrsHalfDay = "5:00";
                    sh.RecessDur = "1:00";
                    sh.SmsScheduleTime = "2:00";
                    sh.IsActive = true;
                    err = RestClient.ShiftAdd(sh);

                    shift = err.Meg;
                    if (policy == "Policy Created Successfully." && shift == "Shift created Successfully.")
                    {
                        res = "Template Added Successfully";
                    }
                    else if (policy == "Policy Created Successfully." && shift == "Shift already exists.")
                    {
                        res = "Template Added Successfully";
                    }
                    else if (policy == "Policy Already exists." && shift == "Shift created Successfully.")
                    {
                        res = "Template Added Successfully";
                    }
                    else if (policy == "Policy Already exists." && shift == "Shift already exists.")
                    {
                        res = "Template Already Applied";
                    }
                    else
                    {
                        res = "Error on Template applied !";
                    }

                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                if (TemplateId == 7)
                {
                    Shifts sh = new Shifts();
                    sh.ShiftName = "Morning Shift";
                    sh.ShiftShortName = "MNG";
                    sh.ShiftGroupId = 0;
                    sh.StartTime = "06:00";
                    //DateTime.ParseExact("09:00:00", "hh:mm:ss");
                    sh.EndTime = "15:00";
                    if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                    {
                        sh.GraceBefore = 0;
                        sh.GraceAfter = 0;
                        sh.roleid = Convert.ToInt32(Session["RoleId"]);
                    }
                    else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                    {
                        var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                        var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                        sh.GraceBefore = cmpid;
                        sh.GraceAfter = BranchID;
                        sh.roleid = Convert.ToInt32(Session["RoleId"]);
                    }
                    sh.ShiftDur = "09:00";
                    sh.MinHrsFullDay = "09:00";
                    sh.MinHrsHalfDay = "4:30";
                    sh.RecessDur = "1:00";
                    sh.SmsScheduleTime = "1:00";
                    sh.IsActive = true;
                    err = RestClient.ShiftAdd(sh);
                    shift = err.Meg;

                    if (shift == "Shift created Successfully.")
                    {
                        res = "Template Added Successfully";
                    }
                    else if (shift == "Shift already exists.")
                    {
                        res = "Template Already Applied";
                    }
                    else
                    {
                        res = "Error on Template applied !";
                    }
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                if (TemplateId == 8)
                {
                    Shifts sh = new Shifts();
                    sh.ShiftName = "Noon Shift";
                    sh.ShiftShortName = "Non";
                    sh.ShiftGroupId = 0;
                    sh.StartTime = "15:00";
                    //DateTime.ParseExact("09:00:00", "hh:mm:ss");
                    sh.EndTime = "23:00";
                    if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                    {
                        sh.GraceBefore = 0;
                        sh.GraceAfter = 0;
                        sh.roleid = Convert.ToInt32(Session["RoleId"]);
                    }
                    else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                    {
                        var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                        var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                        sh.GraceBefore = cmpid;
                        sh.GraceAfter = BranchID;
                        sh.roleid = Convert.ToInt32(Session["RoleId"]);
                    }

                    sh.ShiftDur = "09:00";
                    sh.MinHrsFullDay = "09:00";
                    sh.MinHrsHalfDay = "4:30";
                    sh.RecessDur = "1:00";
                    sh.SmsScheduleTime = "1:00";
                    sh.IsActive = true;
                    err = RestClient.ShiftAdd(sh);
                    shift = err.Meg;

                    if (shift == "Shift created Successfully.")
                    {
                        res = "Template Added Successfully";
                    }
                    else if (shift == "Shift already exists.")
                    {
                        res = "Template Already Applied";
                    }
                    else
                    {
                        res = "Error on Template applied !";
                    }
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                if (TemplateId == 9)
                {
                    Shifts sh = new Shifts();
                    sh.ShiftName = "Evening Shift";
                    sh.ShiftShortName = "Evg";
                    sh.ShiftGroupId = 0;
                    sh.StartTime = "16:00";
                    //DateTime.ParseExact("09:00:00", "hh:mm:ss");
                    sh.EndTime = "01:00";
                    if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                    {
                        sh.GraceBefore = 0;
                        sh.GraceAfter = 0;
                        sh.roleid = Convert.ToInt32(Session["RoleId"]);
                    }
                    else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                    {
                        var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                        var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                        sh.GraceBefore = cmpid;
                        sh.GraceAfter = BranchID;
                        sh.roleid = Convert.ToInt32(Session["RoleId"]);
                    }
                    sh.ShiftDur = "9:00";
                    sh.MinHrsFullDay = "9:00";
                    sh.MinHrsHalfDay = "4:30";
                    sh.RecessDur = "1:00";
                    sh.SmsScheduleTime = "1:00";
                    sh.IsActive = true;
                    err = RestClient.ShiftAdd(sh);
                    shift = err.Meg;

                    if (shift == "Shift created Successfully.")
                    {
                        res = "Template Added Successfully";
                    }
                    else if (shift == "Shift already exists.")
                    {
                        res = "Template Already Applied";
                    }
                    else
                    {
                        res = "Error on Template applied !";
                    }
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                if (TemplateId == 10)
                {
                    Shifts sh = new Shifts();
                    sh.ShiftName = "Night Shift";
                    sh.ShiftShortName = "Nig";
                    sh.ShiftGroupId = 0;
                    sh.StartTime = "22:00";
                    //DateTime.ParseExact("09:00:00", "hh:mm:ss");
                    sh.EndTime = "6:00";
                    if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                    {
                        sh.GraceBefore = 0;
                        sh.GraceAfter = 0;
                    }
                    else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                    {
                        var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                        var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                        sh.GraceBefore = cmpid;
                        sh.GraceAfter = BranchID;
                        sh.roleid = Convert.ToInt32(Session["RoleId"]);
                    }
                    sh.ShiftDur = "9:00";
                    sh.MinHrsFullDay = "9:00";
                    sh.MinHrsHalfDay = "4:30";
                    sh.RecessDur = "1:00";
                    sh.SmsScheduleTime = "1:00";
                    sh.IsActive = true;
                    err = RestClient.ShiftAdd(sh);
                    shift = err.Meg;

                    if (shift == "Shift created Successfully.")
                    {
                        res = "Template Added Successfully";
                    }
                    else if (shift == "Shift already exists.")
                    {
                        res = "Template Already Applied";
                    }
                    else
                    {
                        res = "Error on Template applied !";
                    }
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                if (TemplateId == 11)
                {
                    Roles r1 = new Roles();
                    r1.RoleName = "Employee";
                    r1.RoleDescription = "emp";
                    r1.IsActive = true;
                    err = RestClient.RoleAdd(r1);
                    role = err.Meg;
                    if (role == "Role name already exists." || role == "Role created successfully.")
                    {
                        RoleRightsMapping model = new RoleRightsMapping();
                        model.RoleId = 2;
                        if (model.RoleId > 0)
                        {
                            var RoleMenuData1 = RestClient.RoleRightsGetAll(model.RoleId);
                            var RoleMenuData = RoleMenuData1.Where(x => x.OperationId > 0 && x.ParentMenuId != 0);
                            int count = 0;
                            bool flg;
                            flg = RestClient.RoleRightsDelete(model.RoleId);
                            var roleid = "3,4,58,62,33,60,61,62,63,65,66,84";
                            var values = roleid.Split(',');
                            if (flg)
                            {
                                //foreach (var s in RoleMenuData)
                                //        {
                                for (int i = 0; i < values.Length; i++)
                                {
                                    model.OperationId = Convert.ToInt32(values[i].Trim());
                                    model.CanView = true;
                                    model.CanAdd = true;
                                    model.CanEdit = true;
                                    model.CanDelete = false;
                                    model.CanImport = true;
                                    model.CanExport = true;
                                    if (model.CanView == true || model.CanAdd == true || model.CanEdit == true || model.CanDelete == true || model.CanImport == true || model.CanExport == true)
                                    {
                                        RestClient.RoleRightsAdd(model);
                                    }
                                }
                                msg = "Rights Updated Successfully.";
                            }
                            else
                            {
                                msg = "Issue in Service.";
                            }

                        }
                        if (role == "Role created successfully." && msg == "Rights Updated Successfully.")
                        {
                            res = "Template Added Successfully";
                        }
                        else if (role == "Role name already exists." && msg == "Rights Updated Successfully.")
                        {
                            res = "Template Added Successfully";
                        }
                        else if (role == "Role name already exists." && msg == "Issue in Service.")
                        {
                            res = "Template Already Applied.";
                        }
                        else
                        {
                            res = "Error on Template applied !";
                        }
                    }
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                //if (TemplateId == 24)
                //{
                //    Roles r1 = new Roles();
                //    r1.RoleName = "CompanyAdmin";
                //    r1.RoleDescription = "Compadmin";
                //    r1.IsActive = true;
                //    err = RestClient.RoleAdd(r1);
                //    role = err.Meg;
                //    RoleRightsMapping model = new RoleRightsMapping();
                //    model.RoleId = 6805;
                //    if (model.RoleId > 0)
                //    {
                //        var RoleMenuData1 = RestClient.RoleRightsGetAll(model.RoleId);
                //        var RoleMenuData = RoleMenuData1.Where(x => x.OperationId > 0 && x.ParentMenuId != 0);
                //        int count = 0;
                //        bool flg;
                //        flg = RestClient.RoleRightsDelete(model.RoleId);
                //        var roleid = "2,3,8,9,10,11,12,13,14,78,16,17,18,20,21,22,24,26,31,70,139,32,50,68";
                //        var values = roleid.Split(',');
                //        if (flg)
                //        {
                //            //foreach (var s in RoleMenuData)
                //            //        {
                //            for (int i = 0; i < values.Length; i++)
                //            {
                //                model.OperationId = Convert.ToInt32(values[i].Trim());
                //                model.CanView = true;
                //                model.CanAdd = true;
                //                model.CanEdit = true;
                //                model.CanDelete = false;
                //                model.CanImport = true;
                //                model.CanExport = true;
                //                if (model.CanView == true || model.CanAdd == true || model.CanEdit == true || model.CanDelete == true || model.CanImport == true || model.CanExport == true)
                //                {
                //                    RestClient.RoleRightsAdd(model);
                //                }
                //            }
                //            msg = "Rights Updated Successfully.";
                //            //}


                //        }
                //        else
                //        {
                //            ViewBag.error = "Issue in Service.";
                //        }

                //    }


                //    if (role == "Role Created Successfully." && msg == "Rights Updated Successfully.")
                //    {
                //        res = "Template Added Successfully";
                //    }
                //    else if (role == "Role Name Already exists.")
                //    {
                //        res = "Template Already Applied";
                //    }
                //    else
                //    {
                //        res = "Error on Template applied !";
                //    }
                //    return Json(res, JsonRequestBehavior.AllowGet);
                //}

                else
                {
                    res = "Something wrong Try again";
                    return Json(res, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {
                res = "Error";
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        [CustAuthFilter]
        [HttpGet]
        public ActionResult TemplateAdd()
        {
            try
            {
                Templates tmp = new Templates();
                tmp.TemplateList = RestClient.TemplateGetAll();
                return View(tmp);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }

        [CustAuthFilter]
        [HttpPost]
        public ActionResult TemplateAdd(Templates tmp)
        {
            try
            {
                tmp.IsActive = true;
                tmp.CreatedDate = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                tmp.ModifyDate = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(tmp);

                ViewBag.msg = "";
                ViewBag.error = "";
                if (tmp.TemplateId > 0)
                {
                    tmp.IsActive = true;
                    if (!RestClient.TemplateUpdate(tmp.TemplateId, tmp))
                    {
                        ViewBag.msg = "Template not updated due to service issue.";
                    }
                    else
                    {
                        ViewBag.msg = "Template is updated.";
                    }
                }
                else
                {
                    RestClient.TemplateAdd(tmp);
                    ViewBag.msg = "Template added successfully.";
                }

                tmp.TemplateList = RestClient.TemplateGetAll();
                return View(tmp);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }


        }

        [CustAuthFilter]
        [HttpPost]
        public string DeleteTemplate(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.TemplateDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }
        #endregion

        #region fill leave summary
        public string Fillleavesummary1(int id)
        {
            var res1 = RestClient.Fillleavesummary("1", id.ToString());
            return res1;
        }
        public string Fillleavesummary2(int id)
        {
            var res1 = RestClient.Fillleavesummary("2", id.ToString());
            return res1;
        }
        #endregion

        #region AttendanceSheet
        [CustAuthFilter]
        [HttpGet]
        public ActionResult AttendanceSheet()
        {
            ViewBag.Companyslist = RestClient.CompanyGetAll();
            ViewBag.DepartmentList = RestClient.DepartmentGetAll();
            return View();
        }

        [HttpPost]
        public JsonResult AttendanceSheetData(string empid, string companyid, string branchid, string departmentid, string fromDate, string toDate)
        {
            JsonResult result;
            try
            {
                if (empid == null)
                {
                    empid = "";
                }
                if (branchid == null || branchid == "0")
                {
                    branchid = "";
                }
                if (departmentid == null || departmentid == "0")
                {
                    departmentid = "";
                }

                var i = RestClient.GetAttendanceSheetData(empid, companyid, branchid, departmentid, fromDate, toDate);
                //var jsonSerialiser = new JavaScriptSerializer();
                //var json = jsonSerialiser.Serialize(i);
                result = Json(i);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }

        }

        #endregion

        #region Feedback
        [HttpPost]
        public JsonResult Feedback(FormCollection collection)
        {
            JsonResult result;
            try
            {
                bool IsmailSend = false;
                string name = collection[0];
                string email = collection[1];
                string feedback = collection[2];
                bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                bool isname = Regex.IsMatch(name, "^[A-Za-z0-9-_\\s]*$");
                bool isfeedback = Regex.IsMatch(feedback, "^[A-Za-z0-9-_\\s]*$");
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(feedback))
                {
                    if (isEmail)
                    {
                        if (isname && isfeedback)
                        {
                            sqlinjectionsearchList.Add("select ");
                            sqlinjectionsearchList.Add("update ");
                            sqlinjectionsearchList.Add("insert ");
                            sqlinjectionsearchList.Add("delete ");
                            sqlinjectionsearchList.Add("drop ");
                            sqlinjectionsearchList.Add("from ");
                            sqlinjectionsearchList.Add("1=1");
                            bool nameString =
                                sqlinjectionsearchList.Any(word => name != null && name.ToLower().Contains(word));
                            bool emailString =
                                sqlinjectionsearchList.Any(word => email != null && email.ToLower().Contains(word));
                            bool feedbackString = sqlinjectionsearchList.Any(word =>
                                feedback != null && feedback.ToLower().Contains(word));
                            if (!nameString && !emailString && !feedbackString)
                            {
                                MailDetails mailDetails = new MailDetails();
                                mailDetails.Isattchement = false;
                                mailDetails.MailBody =
                                    "<Html><Head></Head><Body><p>Please find the feedback below:</p><table><tbody><tr><td>Name : </td><td>" +
                                    name + "</td></tr><tr><td>Email : </td><td>" + email +
                                    "</td></tr><tr><td>Feedback : </td><td>" + feedback +
                                    "</td></tr></tbody></table></Body></Html>";
                                mailDetails.Subject = "Feedback for Minop";
                                IsmailSend = RestClient.SendFeedbackMail(mailDetails);
                                if (IsmailSend)
                                {
                                    result = Json(
                                        new
                                        {
                                            isSend = true,
                                            msg = "Thank you for your feedback, It will help us improve."
                                        });
                                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                                    return result;
                                }
                                else
                                {
                                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                    result = Json(new
                                    {
                                        isSend = false,
                                        msg = "There was some error while submitting feedback, Please try again later."
                                    });
                                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                                    return result;
                                }
                            }
                            else
                            {
                                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                result = Json(new
                                {
                                    isSend = false,
                                    msg = "There was some error while submitting feedback, Please try again later."
                                });
                                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                                return result;
                            }
                        }
                        else
                        {
                            Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            result = Json(new
                            {
                                isSend = false,
                                msg = "Special characters are not allowed, Please remove special characters."
                            });
                            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                            return result;
                        }
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        result = Json(new
                        {
                            isSend = false,
                            msg = "Email is not valid, Please enter valid email."
                        });
                        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                        return result;
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    result = Json(new
                    {
                        isSend = false,
                        msg = "All fields are mandatory."
                    });
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    return result;
                }
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                result = Json(new { isSend = false, msg = "There was some error while submitting feedback, Please try again later." });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        #endregion

        #region Developer Account
        [HttpGet]
        public ActionResult DevelopersAccount()
        {
            if (Request.Cookies["DeveloperAccount"] != null)
            {
                ViewBag.UserEmail = Request.Cookies["DeveloperAccount"].Values["DeveloperEmail"];
                ViewBag.Password = Request.Cookies["DeveloperAccount"].Values["DeveloperPassword"];
                ViewBag.rememberme = "true";
            }
            return View();
        }

        [HttpPost]
        public ActionResult DeveloperLogin(LoginModel model, string rememberme)
        {
            ModelState.Remove("CompanyName");
            ModelState.Remove("CompanyCode");
            try
            {
                if (ModelState.IsValid)
                {
                    resp = RestAccout.DeveloperLogin(model);
                    if (resp.MegSts == "jwt_token")
                    {
                        //FormsAuthentication.SetAuthCookie(model.UserEmail, true);
                        Session["tokan"] = resp.Meg;
                        string payload = AuthHandler.getPayload(Session["tokan"].ToString(), ConfigurationManager.AppSettings["jwtKey"].ToString(), false);
                        JObject jObj = JObject.Parse(payload);
                        Session["RoleId"] = Convert.ToInt32(jObj["RoleId"]);

                        if (rememberme == "true")
                        {
                            HttpCookie cookie = new HttpCookie("DeveloperAccount");
                            cookie.Values.Add("DeveloperEmail", model.UserEmail);
                            cookie.Values.Add("DeveloperPassword", model.Password);
                            cookie.Expires = DateTime.Now.AddDays(15);
                            Response.Cookies.Add(cookie);
                        }
                        else
                        {
                            if (Request.Cookies["DeveloperAccount"] != null)
                            {
                                var c = new HttpCookie("DeveloperAccount");
                                c.Expires = DateTime.Now.AddDays(-1);
                                Response.Cookies.Add(c);
                            }
                        }

                        if (Convert.ToInt32(Session["RoleId"]) != 15)
                        {
                            Session["cmpname"] = jObj["cmpname"].ToString();
                            Session["UserEmail"] = jObj["name"].ToString();
                            Session["photo"] = jObj["photo"].ToString();
                            Session["UserId"] = jObj["UserId"].ToString();
                            string strkey = jObj["keystr"].ToString();
                            string systemWizarFlag = jObj["IsSetting"].ToString();
                            Session["cmpcode"] = strkey.Split('|')[1].ToString();
                            Session["ClientCompanyId"] = jObj["ClientCompanyId"].ToString();
                            Session["CompanyId"] = jObj["compId"].ToString();
                            Session.Timeout = 10;
                            Session["IsSetting"] = jObj["IsSetting"].ToString();
                            Session["FirstEmpId"] = jObj["FirstEmpId"].ToString();
                            Session["EmpId"] = Convert.ToInt32(jObj["EmpId"]);
                            string isExp = jObj["PlanExpDate"].ToString();
                            Session["validupto"] = jObj["lockAttendanceDay"].ToString();
                            Session["DbName"] = "";
                            Session["DataGridThresold"] = Convert.ToInt64(jObj["DataGridThreShold"]);
                            int ispaymentStatus = Convert.ToInt32(jObj["IsApprovalForWebpunch"]);
                            Session["IsDeviceLimit"] = Convert.ToInt32(jObj["IsDeviceLimit"]); ;

                            if (systemWizarFlag == "False")
                            {
                                return RedirectToAction("SystemSettingWizard", "PayTime");
                            }
                            else
                            {
                                return RedirectToAction("DevelopersDashboard", "Developers");
                            }

                        }
                        else
                        {
                            string strkey = jObj["keystr"].ToString();
                            Session["UserEmail"] = jObj["name"].ToString();
                            Session["RoleId"] = Convert.ToInt32(jObj["RoleId"]);
                            Session.Timeout = 10;
                            //return RedirectToAction("SuperAdminDashboard", "PayTime");
                            return RedirectToAction("SuperAdminCompanyDetails", "PayTime");
                        }
                    }
                    else
                    {

                        if (resp.Meg != "")
                        {
                            TempData["meg"] = resp.Meg;
                            TempData["MegSts"] = resp.MegSts;
                        }
                        else
                        {
                            TempData["meg"] = "something wrong please try again";
                        }
                        //ModelState.AddModelError(resp.MegSts, resp.Meg);

                        return RedirectToAction("DevelopersAccount", "PayTime");
                    }
                }
                else
                {

                    if (resp.Meg != "")
                    {
                        TempData["meg"] = resp.Meg;
                    }
                    else
                    {
                        TempData["meg"] = "something wrong please try again";
                    }
                    //Response.Write("<script>alert('UserEmail and Password is not valid please try again.')</script>");
                    //ModelState.AddModelError(resp.MegSts, resp.Meg);
                    return RedirectToAction("DevelopersAccount", "PayTime");
                }
            }
            catch (Exception Ex)
            {

                TempData["meg"] = Ex.InnerException;
                return RedirectToAction("DevelopersAccount", "PayTime");

            }
        }
        [HttpGet]
        public ActionResult DevelopersPassword()
        {
            return View();
        }
        [HttpPost]
        //[CustAuthFilter]
        public ActionResult DevelopersPassword(ResetPassword model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(model.Email))
                    {
                        if (!string.IsNullOrEmpty(model.OTP) && !string.IsNullOrEmpty(model.UserPass) && !string.IsNullOrEmpty(model.ConfirmPassword))
                        {

                            if (Session["guid"] != null)
                            {
                                resp = AccountRestClint.ResetPasswordDeveloper(model.OTP, Session["guid"].ToString(), model.UserPass);
                                // Session["guid"] = null;
                                ViewBag.MegSts = resp.MegSts;
                                ViewBag.Meg = resp.Meg;
                            }
                            else
                            {
                                return RedirectToAction("LoginPage", "PayTime");
                            }
                        }
                        else
                        {
                            resp = AccountRestClint.DevelopersPassword(model.Email);
                            if (resp.MegSts == "ok")
                            {
                                Session["guid"] = resp.Meg.ToString();
                                resp.Meg = "OTP sent on your registered email.";
                                ViewBag.Otp = resp.Meg.ToString();
                            }
                            ViewBag.MegSts = resp.MegSts;
                            ViewBag.Meg = resp.Meg;
                        }
                    }
                    else
                    {
                        ViewBag.MegSts = "Error";

                    }
                }
                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        public ActionResult DeveloperActiveUser()
        {
            string ActivationCode = Request.QueryString["ed"].ToString();
            if (!string.IsNullOrEmpty(ActivationCode))
            {
                resp = AccountRestClint.DeveloperActiveUser(ActivationCode);
                string domain = resp.Meg.Split('-')[1];
                string message = resp.Meg.Split('-')[0];
                ViewBag.MegSts = resp.MegSts;
                if (resp.MegSts == "ok")
                {
                    ViewBag.Meg = message;
                    ViewBag.Url = domain;
                }
                else if (resp.MegSts == "activated")
                {
                    ViewBag.Meg = "User Already Activated.";
                    ViewBag.Url = domain;
                }
                else
                {
                    ViewBag.Meg = "Error in activation, please contact admin";
                    ViewBag.Url = domain;
                }

            }
            else
            {
                ViewBag.MegSts = "Error";

            }
            return View();
        }

        [HttpPost]
        [CustAuthFilter]
        public JsonResult developerDashboardCounters()
        {
            JsonResult result;
            try
            {
                var i = RestClient.GetDeveloperDashBoardCount();
                result = Json(i);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

        #endregion

        #region DeviceDemo
        [CustAuthFilter]
        public ActionResult DevicesList()
        {
            return View();
        }
        [HttpPost]
        [CustAuthFilter]
        public ActionResult DevicesList(DeviceLienceUpdate DL, HttpPostedFileBase FileUpload)
        {
            try
            {
                string filePath = string.Empty;
                if (Request.Files["FileUpload"].ContentLength > 0)
                {
                    string extension = System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName).ToLower();
                    string[] validFileTypes = { ".lic" };
                    if (validFileTypes.Contains(extension))
                    {
                        Stream fs = FileUpload.InputStream;
                        BinaryReader br = new BinaryReader(fs);
                        byte[] bytes = br.ReadBytes((Int32)fs.Length);
                        DL.DeviceLicence = bytes;
                        var i = RestClient.UpdateDeviceLicence(DL);
                        if (i)
                        {
                            ViewBag.msg = "Licence uploaded sucessfully.";
                        }
                        else
                        {
                            ViewBag.error = "Error in uploading.";
                        }
                    }
                    else
                    {
                        ViewBag.error = "Please upload files in .Lic format";
                    }

                }
                return View();
            }
            catch (Exception)
            {
                ViewBag.error = "Error in uploading file.";
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        #endregion

        #region Subscription
        [CustAuthFilter]
        [HttpGet]
        public ActionResult Mysubscription()
        {
            planRestrictions(0);

            return View();
        }
        #endregion

        #region Module Selection
        [CustAuthFilter]
        public ActionResult ModuleSelection()
        {
            return View();
        }
        #endregion

        #region SMS Template
        [CustAuthFilter]
        public ActionResult SmsTemplate()
        {
            return View();
        }
        #endregion

        #region School SMS Template
        [CustAuthFilter]
        public ActionResult SchoolSmsTemplate()
        {
            return View();
        }
        #endregion


        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult Pricing()
        {
            return View();
        }

        public ActionResult TimeAttendanceManagementSystem()
        {
            return View();
        }

        public ActionResult VisitorManagementSystem()
        {
            return View();
        }

        public ActionResult EmployeeSelfService()
        {
            return View();
        }

        public ActionResult SchoolManagement()
        {
            return View();
        }

        public ActionResult DownloadApp()
        {
            return View();
        }
        public ActionResult SupportRequest()
        {
            return View();
        }
        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ContactUs(Leads reqobj)
        {
            var msg = "";
            ViewBag.msg = "";
            ViewBag.error = "";
            try
            {
                string EncodedResponse = Request.Form["_captch"];
                bool IsCaptchaValid = (ReCaptchaClass.Validate(EncodedResponse) == "true" ? true : false);

                if (IsCaptchaValid) // Captcha velidation
                {
                    //Valid Request
                    #region Send Lead
                    if (ModelState.IsValid)
                    {
                        #region bitrixlead 06-04-2023
                        //reqobj.SourceUrl = Request.UrlReferrer.PathAndQuery;
                        //reqobj.SourceTitle = "Minop Contact";
                        //reqobj.SourceType = "WEB";
                        //reqobj.ProductID = 6;
                        //reqobj.Comments = reqobj.Subject + " - " + reqobj.Comments;
                        //using (var client = new HttpClient())
                        //{
                        //    string uri = _leadurl + "Leads/Create";
                        //    var postTask = client.PostAsJsonAsync<Leads>(uri, reqobj);
                        //    postTask.Wait();
                        //    var res = postTask.Result.Content.ReadAsStringAsync().Result;
                        //    var resobj = JsonConvert.DeserializeObject<Resp>(res);
                        //    if (resobj.MsgSts.ToLower() == "ok")
                        //    {
                        //        msg = "Thank you for your Inquiry, We will get back to you.";
                        //        ViewBag.msg = msg;
                        //    }
                        //    else
                        //    {
                        //        msg = "Error: captcha is not valid.";
                        //        ViewBag.error = msg;
                        //    }
                        //}
                        #endregion

                        #region Zoho CRM Lead Generation Contact us
                        msg = Zohocrmleadgenerate(reqobj, msg);
                        ViewBag.msg = msg;
                        #endregion

                    }
                    else
                    {
                        msg = "Something went wrong, Please try again.";
                        ViewBag.error = msg;
                    }
                    #endregion

                    #region inqure Mail
                    bool IsmailSend = false;
                    string name = reqobj.Name;
                    string email = reqobj.Email;
                    string message = reqobj.MobileNo;
                    MailDetails mailDetails = new MailDetails();
                    mailDetails.Isattchement = false;
                    mailDetails.MailBody = "<Html><Head></Head><Body><p>Please find the inquiry below:</p><table><tbody><tr><td>Name : </td><td>" + name + "</td></tr><tr><td>Email : </td><td>" + email + "</td></tr><tr><td>Message : </td><td>" + message + "</td></tr></tbody></table><br/>Regards,<br/></br>MinopTeam</Body></Html>";
                    mailDetails.Subject = "Inquiry from MinopCloud";
                    IsmailSend = RestClient.InquiryMail(mailDetails);
                    if (IsmailSend)
                    {
                        msg = "Thank you for your inquiry, we will get back to you soon.";
                        ViewBag.msg = msg;

                    }
                    else
                    {
                        msg = "There was some error while submitting inquiry, Please try again later.";
                        ViewBag.error = msg;
                    }

                    #endregion
                }
                else
                {
                    ViewBag.ErrMessage = "Error: captcha is not valid.";
                }
            }
            catch (Exception)
            {
                msg = "There was some error while submitting inquiry, Please try again later.";
                ViewBag.error = msg;
            }

            return View();
        }

        public ActionResult LoginPage()
        {
            ViewBag.LoginType = TempData["LoginType"];
            return View();
        }
        public ActionResult TermsofUse()
        {
            return View();
        }
        public ActionResult PrivacyPolicy()
        {
            return View();
        }
        public ActionResult FAQ()
        {

            return View();
        }
        public ActionResult FAQNEW()
        {

            return View();
        }
        public ActionResult UserGuide()
        {
            return View();
        }
        public ActionResult TroubleshootingVideo()
        {
            return View();
        }
        public ActionResult Sitemap()
        {
            return View();
        }

        #region Zohodesk support page
        [HttpGet]
        public ActionResult CustomerSupport()
        {
            ViewBag.Ticketno = "";
            return View();
        }
        [HttpPost]
        public ActionResult CustomerSupport(Supportticketreq objreq, HttpPostedFileBase[] attechmentfiles)
        {
            objreq.email = Session["UserEmail"].ToString();
            objreq.priority = "2";
            objreq.status = "2";
            string uri = string.Empty;

            if (string.IsNullOrEmpty(_zohoaccess_token))
            {
                Getzohoaccesstoken();
            }


            try
            {


                Supportticketzohoreq objreqzoho = new Supportticketzohoreq();
                classcontact objctn = new classcontact();
                objctn.firstName = Session["cmpname"].ToString();
                objctn.lastName = Session["cmpcode"].ToString();
                objctn.email = objreq.email;
                objctn.phone = Session["cmpcode"].ToString();

                objreqzoho.departmentId = _zohodeskdepartmentId; ;
                objreqzoho.subject = objreq.subject;
                objreqzoho.description = objreq.description;
                objreqzoho.contact = objctn;

                //Attachments:
                List<string> _strfilename = new List<string>();

                try
                {
                    if (attechmentfiles != null)
                    {
                        using (var objclient = new HttpClient())
                        {
                            uri = _Zohodeskapiurl + "/api/v1/uploads";
                            ServicePointManager.Expect100Continue = true;
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            objclient.DefaultRequestHeaders.Add("orgId", _zohoorgId);
                            objclient.DefaultRequestHeaders.Add("Authorization", "Zoho-oauthtoken " + _zohoaccess_token);
                            var multiContent = new MultipartFormDataContent();
                            foreach (HttpPostedFileBase file in attechmentfiles)
                            {
                                if (file != null)
                                {
                                    var fileStreamContent = new StreamContent(file.InputStream);
                                    multiContent.Add(fileStreamContent, "file", file.FileName);
                                }
                            }

                            Task<HttpResponseMessage> response = objclient.PostAsync(uri, multiContent);
                            response.Wait();
                            if (response.Result.StatusCode == HttpStatusCode.OK)
                            {
                                var res = response.Result.Content.ReadAsStringAsync().Result;
                                GetDeviceDetails.ProcessLogLogFileWrite("zohodesk_success_Response", response.Result.StatusCode + "--" + res);
                                var jss = new JavaScriptSerializer();
                                var table = jss.Deserialize<dynamic>(res);
                                if (table.ContainsKey("id"))
                                {
                                    _strfilename.Add(table["id"]);
                                }

                            }
                            objreq.strfilename = _strfilename.ToArray();
                        }

                    }
                    else
                    {
                        objreq.strfilename = null;
                    }
                }
                catch (WebException webex)
                {
                    GetDeviceDetails.ProcessLogLogFileWrite("zohodesk_success_Response", webex.Message + "\n" + webex.StackTrace);
                }
                catch (Exception ex)
                {
                    GetDeviceDetails.ProcessLogLogFileWrite("CustomerSupport_exception", ex.InnerException.Message + "--" + ex.StackTrace);
                }




                //string uri = _freshdeskapiurl + "/api/Freshdesk/CreateTicket";

                uri = _Zohodeskapiurl + "/api/v1/tickets";
                Uri _url = new Uri(uri);
                var listOfStrings = new List<string>();
                var _upload = objreq.strfilename != null && objreq.strfilename.Length > 0 ? objreq.strfilename : listOfStrings.ToArray();

                using (HttpClient client = new HttpClient())
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    client.Timeout = TimeSpan.FromMinutes(15);
                    client.DefaultRequestHeaders.Add("orgId", _zohoorgId);
                    client.DefaultRequestHeaders.Add("Authorization", "Zoho-oauthtoken " + _zohoaccess_token);
                    objreqzoho.uploads = _upload;
                    //Task<HttpResponseMessage> response = client.PostAsJsonAsync<Supportticketreq>(uri, objreq);
                    Task<HttpResponseMessage> response = client.PostAsJsonAsync<Supportticketzohoreq>(uri, objreqzoho);
                    response.Wait();
                    if (response.Result.StatusCode == HttpStatusCode.OK)
                    {
                        var res = response.Result.Content.ReadAsStringAsync().Result;
                        GetDeviceDetails.ProcessLogLogFileWrite("zohodesk_success_Response", response.Result.StatusCode + "--" + res);
                        var jss = new JavaScriptSerializer();
                        var table = jss.Deserialize<dynamic>(res);
                        if (Convert.ToInt32(table["ticketNumber"]) > 0)
                        {
                            var ticketid = table["ticketNumber"];
                            ViewBag.msg = string.Format("Ticket created.Ticket No is: {0}", ticketid);
                        }
                        GetDeviceDetails.ProcessLogLogFileWrite("Freshdesk_Response", res);
                    }
                    else
                    {
                        if (response.Result.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            Getzohoaccesstoken();
                        }
                        ViewBag.error = "Error on ticket create.";
                        GetDeviceDetails.ProcessLogLogFileWrite("Freshdesk_Error_Response", response.Result.StatusCode + "--" + response.Result.Content.ReadAsStringAsync().Result);

                    }

                }
            }
            catch (WebException webex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("CustomerSupport_exception_web", webex.Message + "\n" + webex.StackTrace);
            }
            catch (Exception ex)
            {
                ViewBag.error = "Error on ticket create";
                GetDeviceDetails.ProcessLogLogFileWrite("CustomerSupport_exception", ex.InnerException.Message + "--" + ex.StackTrace);
            }

            return View();
        }
        private static void writeCRLF(Stream o)
        {
            byte[] crLf = Encoding.ASCII.GetBytes("\r\n");
            o.Write(crLf, 0, crLf.Length);
        }
        private static void writeBoundaryBytes(Stream o, string b, bool isFinalBoundary)
        {
            string boundary = isFinalBoundary == true ? "--" + b + "--" : "--" + b + "\r\n";
            byte[] d = Encoding.ASCII.GetBytes(boundary);
            o.Write(d, 0, d.Length);
        }
        private static void writeContentDispositionFileHeader(Stream o, string name, string fileName, string contentType)
        {
            string data = "Content-Disposition: form-data; name=\"" + name + "\"; filename=\"" + fileName + "\"\r\n";
            data += "Content-Type: " + contentType + "\r\n\r\n";
            byte[] b = Encoding.ASCII.GetBytes(data);
            o.Write(b, 0, b.Length);
        }
        private void Getzohoaccesstoken()
        {
            try
            {
                //var strPost = "refresh_token=1000.c251023395eabafefa933a8b878641aa.73886baebb0309ca77ee400190635c77&client_id=1000.XBUGANPNVOP15LBEMVFMI9Y9Z0V77G&client_secret=ad902b28c1a7090999ef9332a09cc157a7f641fe90&scope=Desk.tickets.ALL,Desk.basic.CREATE&redirect_uri=https://www.zylker.com/oauthgrant&grant_type=refresh_token&access_type =offline";
                var uri = _zohodeskapiurlrefreshtoken;
                var strPost = "refresh_token=" + _refresh_token + "&client_id=" + _client_id + "&client_secret=" + _client_secret + "&scope=" + _scope + "&redirect_uri=" + _redirect_uri + "&grant_type=" + _grant_type + "&access_type =" + _access_type + "";
                String result = "";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(uri);
                StreamWriter myWriter = null;
                objRequest.Method = "POST";
                objRequest.ContentLength = strPost.Length;
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
                using (StreamReader sr =
                   new StreamReader(objResponse.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                    var jss = new JavaScriptSerializer();
                    var table = jss.Deserialize<dynamic>(result);
                    if (table.ContainsKey("access_token"))
                    {
                        _zohoaccess_token = table["access_token"];
                    }
                    else
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("Zohodesk_Error_Response", "On Get access token :" + table["error"]);
                    }
                    // Close and clean up the StreamReader
                    sr.Close();
                }

            }
            catch (Exception Ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("Zohodesk_Error_Response", "On Get access token :" + Ex.Message + "--" + Ex.InnerException);
            }
        }

        private void Getzohocrmaccesstoken()
        {
            try
            {
                //var strPost = "refresh_token=1000.c251023395eabafefa933a8b878641aa.73886baebb0309ca77ee400190635c77&client_id=1000.XBUGANPNVOP15LBEMVFMI9Y9Z0V77G&client_secret=ad902b28c1a7090999ef9332a09cc157a7f641fe90&scope=Desk.tickets.ALL,Desk.basic.CREATE&redirect_uri=https://www.zylker.com/oauthgrant&grant_type=refresh_token&access_type =offline";
                var uri = _zohocrmapiurlrefreshtoken;
                //var strPost = "refresh_token=" + _zohocrmrefresh_token + "&client_id=" + _zohocrmclient_id + "&client_secret=" + _zohocrmclient_secret + "&scope=" + _zohocrmscope + "&redirect_uri=" + _zohocrmredirect_uri + "&grant_type=" + _zohocrmgrant_type + "&access_type =" + _zohocrmaccess_type + "";
                var strPost = "refresh_token=" + _zohocrmrefresh_token + "&client_id=" + _zohocrmclient_id + "&client_secret=" + _zohocrmclient_secret + "&grant_type=" + _zohocrmgrant_type + "";
                //var strPost = "refresh_token=1000.626f706b57b88cc60db469df64918c42.ee275d630d10bcb3a803b1956c59ac13&client_id=1000.960Y5CQR4S4FCTW2VQWQ6J76ZGCLYY&client_secret=9394ae4ddf0e2facf0f673d53631d4cf8a26e12cdc&grant_type=refresh_token";
                String result = "";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(uri);
                StreamWriter myWriter = null;
                objRequest.Method = "POST";
                objRequest.ContentLength = strPost.Length;
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
                using (StreamReader sr =
                   new StreamReader(objResponse.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                    var jss = new JavaScriptSerializer();
                    var table = jss.Deserialize<dynamic>(result);
                    if (table.ContainsKey("access_token"))
                    {
                        _zohocrmaccess_token = table["access_token"];
                    }
                    else
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("ZohoCRM_Error_Response", "On Get access token :" + table["error"]);
                    }
                    // Close and clean up the StreamReader
                    sr.Close();
                }

            }
            catch (Exception Ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("ZohoCRM_Error_Response", "On Get access token :" + Ex.Message + "--" + Ex.InnerException);
            }
        }
        private string Zohocrmleadgenerate(Leads reqobj, string msg)
        {
            #region Zoho CRM Lead Generation
            reqobj.SourceUrl = Request.UrlReferrer.PathAndQuery;
            reqobj.SourceTitle = "Minop Contact";
            reqobj.SourceType = "WEB";
            reqobj.ProductID = 6;
            reqobj.Comments = reqobj.SourceTitle + " - " + reqobj.Subject;

            reqobj.CompanyName = "MINOP";

            var data = new List<Dictionary<string, string>>
                        {
                            new Dictionary<string, string>
                            {
                                {"Last_Name", reqobj.Name},
                                {"First_Name", reqobj.Name},
                                {"Email", reqobj.Email},
                                {"Phone", reqobj.MobileNo},
                                {"Company", reqobj.CompanyName},
                                {"Lead_Source", reqobj.SourceType},
                                {"Description", reqobj.Comments},
                                {"Designation",reqobj.SourceTitle},
                                {"Website","app.minopcloud.com"},

                            }
                        };

            var json = JsonConvert.SerializeObject(new { data });

            Getzohocrmaccesstoken();

            string crmUrl = _zohocrmapiurl + "crm/v4/Leads";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(crmUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _zohocrmaccess_token);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = httpClient.PostAsync(crmUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {
                string responseContent = response.Content.ReadAsStringAsync().Result;
                msg = "Thank you for your Inquiry, We will get back to you.";
                //ViewBag.msg = msg;
            }
            else
            {
                msg = "Error: captcha is not valid.";
                //ViewBag.error = msg;
            }
            #endregion
            return msg;
        }

        #endregion


        #region Inquiry
        public JsonResult Inquiry(Leads reqobj, FormCollection collection)
        {
            JsonResult result;
            try
            {
                //if (this.IsCaptchaValid("Captcha is not valid"))
                //{
                #region Send Lead
                if (ModelState.IsValid)
                {
                    reqobj.SourceUrl = Request.UrlReferrer.PathAndQuery;
                    reqobj.SourceTitle = "Minop Contact";
                    reqobj.SourceType = "WEB";
                    reqobj.ProductID = 6;
                    reqobj.Comments = reqobj.Subject + " - " + reqobj.Comments;
                    using (var client = new HttpClient())
                    {
                        string uri = _leadurl + "Leads/Create";
                        var postTask = client.PostAsJsonAsync<Leads>(uri, reqobj);
                        postTask.Wait();
                        var res = postTask.Result.Content.ReadAsStringAsync().Result;
                        var resobj = JsonConvert.DeserializeObject<Resp>(res);
                        if (resobj.MsgSts.ToLower() == "ok")
                        {
                            result = Json(
                                new
                                {
                                    isSend = true,
                                    msg = "Thank you for your Inquiry, We will get back to you."
                                });
                        }
                        else
                        {
                            result = Json(
                                new
                                {
                                    isSend = false,
                                    msg = "Something went wrong, Please try again."
                                });
                        }
                    }
                }
                else
                {
                    result = Json(
                        new
                        {
                            isSend = false,
                            msg = "Something went wrong, Please try again."
                        });
                }
                #endregion

                #region inqure Mail
                bool IsmailSend = false;
                string name = collection[0];
                string email = collection[1];
                string message = collection[2];
                MailDetails mailDetails = new MailDetails();
                mailDetails.Isattchement = false;
                mailDetails.MailBody = "<Html><Head></Head><Body><p>Please find the inquiry below:</p><table><tbody><tr><td>Name : </td><td>" + name + "</td></tr><tr><td>Email : </td><td>" + email + "</td></tr><tr><td>Message : </td><td>" + message + "</td></tr></tbody></table><br/>Regards,<br/></br>MinopTeam</Body></Html>";
                mailDetails.Subject = "Inquiry from MinopCloud";
                IsmailSend = RestClient.InquiryMail(mailDetails);
                if (IsmailSend)
                {
                    result = Json(new { isSend = true, msg = "Thank you for your inquiry, we will get back to you soon." });
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    return result;
                }
                else
                {
                    result = Json(new { isSend = false, msg = "There was some error while submitting inquiry, Please try again later." });
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    return result;
                }
                //}
                //else
                //{
                //    result = Json(new { isSend = false, msg = "Captcha is not valid." });
                //    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                //    return result;
                //}

                #endregion
            }
            catch (Exception)
            {
                result = Json(new { isSend = false, msg = "There was some error while submitting inquiry, Please try again later." });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }

        }
        #endregion

        #region Device Notification
        [CustAuthFilter]
        public ActionResult DeviceNotification()
        {
            return View();
        }
        #endregion

        #region Payment Integration
        #region Payment variable
        // Variable for online payment
        public string action1 = string.Empty;
        public string hash1 = string.Empty;
        public string txnid1 = string.Empty;

        ////Live
        //public string MERCHANT_KEY = "u37Tcw";
        //public string SALT = "GBsexCv1";
        //public string PAYU_BASE_URL = "https://secure.payu.in";
        //public string action = "";
        //public string hashSequence = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";

        //Test
        //public string MERCHANT_KEY = "gtKFFx";
        //public string SALT = "eCwWELxi";
        //public string PAYU_BASE_URL = "https://test.payu.in";
        //public string action = "";
        //public string hashSequence = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";
        // Over


        #endregion

        [CustAuthFilter]
        public ActionResult Planpricing()
        {
            Plan pl = new Plan();
            Getpara obj = new Getpara();

            try
            {
                planRestrictions(0);
                int companyid = Convert.ToInt32(Session["CompanyId"]);
                obj.pkid = companyid;
                obj.para = 1;
                obj.iscount = 0;
                obj.IScurrencycode = Convert.ToInt32(Session["IScurrencycode"]);

                if (TempData["pl"] != null)
                {
                    pl = TempData["pl"] as Plan;
                    ViewBag.isAdduser = 1;
                }
                else
                {
                    ViewBag.isAdduser = 0;
                    pl.PlanList = RestClient.PlanGetAll(obj);
                }

                return View(pl);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [CustAuthFilter]
        public ActionResult AddPlanuser()
        {
            Plan pl = new Plan();
            Getpara obj = new Getpara();
            int subPlanid = Convert.ToInt32(Session["planId"]);

            try
            {
                int companyid = Convert.ToInt32(Session["CompanyId"]);
                obj.pkid = companyid;
                obj.para = 1;
                obj.iscount = 0;

                pl.NoOfEmployee = 0;
                pl.PlanList = RestClient.PlanGetAll(obj).Where(x => x.PlanId == subPlanid);
                TempData["pl"] = pl;
                TempData.Keep();
                return RedirectToAction("Planpricing");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [HttpPost]
        public ActionResult Planpricing(Plan objplan)
        {

            string curcode = "INR";

            if (objplan.IScurrencycode == 1)
            {
                curcode = "USD";
            }

            Paymenttrans objpay = new Paymenttrans();
            var planlist = Request.Form["lstcmpaddonpack"];
            List<companyAddon> m = JsonConvert.DeserializeObject<List<companyAddon>>(planlist);
            int companyid = Convert.ToInt32(Session["CompanyId"]);
            int isAdduser = Convert.ToInt32(Request.Form["isAdduser"]);

            Companys cmpobj = new Companys();
            DataTable dtamount = new DataTable();
            string strForm = string.Empty;

            string paymentID = "";
            var orderid = "";
            var custid = "";
            bool isReccuring = false;
            try
            {
                if (objplan.PlanId == 12)
                {
                    var StartDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var _EndDate = DateTime.Now.AddDays(364);
                    var EndDate = _EndDate.ToString("yyyy-MM-dd HH:mm:ss");
                    Companyplan clsobj = new Companyplan();
                    clsobj.CompanyId = companyid;
                    clsobj.PlanId = objplan.PlanId;
                    clsobj.StartDate = StartDate;
                    clsobj.EndDate = EndDate;
                    bool i = RestClient.Subscriptionupdate(clsobj);
                }
                else
                {
                    //dtamount = payclint.Getplantotalamount(objplan.PlanId);
                    int clicmpid = Convert.ToInt32(Session["ClientCompanyId"]);
                    cmpobj = RestClient.CompanyGetByid(clicmpid);

                    // ========== SECURITY FIX: Server-Side Amount Validation ==========
                    // Calculate MINIMUM expected amount server-side (plan + GST, without add-ons)
                    double serverCalculatedCGST, serverCalculatedSGST, serverCalculatedBasePrice;
                    double serverMinimumAmount = CalculateAmountServerSide(
                        objplan.PlanId,
                        objplan.Quantity,
                        objplan.Duration,
                        objplan.IScurrencycode,
                        out serverCalculatedCGST,
                        out serverCalculatedSGST,
                        out serverCalculatedBasePrice);

                    // Validate: Client amount must be >= server minimum (add-ons only increase amount)
                    // This prevents tampering to REDUCE amount while allowing add-ons
                    bool isAmountValid = ValidateAmount(objplan.TotalAmount, serverMinimumAmount, objplan.PlanId, companyid);

                    if (!isAmountValid)
                    {
                        // Amount tampering detected - client sent less than minimum
                        GetDeviceDetails.ProcessLogLogFileWrite("PAYMENT_REJECTED_TAMPERING",
                            $"CompanyId: {companyid}, PlanId: {objplan.PlanId}, ClientAmount: {objplan.TotalAmount}, " +
                            $"ServerMinimum: {serverMinimumAmount}, IP: {Request.UserHostAddress}");

                        TempData["rerrormsg"] = "Payment validation failed. Please try again.";
                        return RedirectToAction("Planpricing", "PayTime");
                    }

                    // Use CLIENT amount (validated to be >= minimum, includes add-ons)
                    objpay.TotalAmount = objplan.TotalAmount;
                    objpay.CGST = objplan.CGST;
                    objpay.SGST = objplan.SGST;
                    objpay.Amount = objplan.Price;
                    // ========== END SECURITY FIX ==========

                    objpay.Companyid = companyid;
                    objpay.DisccountAmt = objplan.Disccount;
                    objpay.DisccountPer = objplan.DisccountPer;
                    objpay.Dplanid = objplan.PlanId;
                    objpay.Duration = objplan.Duration;
                    objpay.Noofuser = objplan.Quantity;
                    objpay.InvoiceData = objplan.Taxofpyament;
                    objpay.IGST = 0.00;
                    objpay.IsApproved = isAdduser;
                    objpay.InvoiceNo = objplan.GSTNo;
                    objpay.ReceptNo = objplan.ReceptNo;
                    //if (dtamount != null && dtamount.Rows.Count > 0)
                    //{
                    //    //objpay.TotalAmount = Convert.ToDouble(dtamount.Rows[0]["Totalamount"]);
                    //    objpay.CGST = Convert.ToDouble(dtamount.Rows[0]["amt"].ToString().Split(',')[0].Split(':')[1].ToString());
                    //    objpay.SGST = Convert.ToDouble(dtamount.Rows[0]["amt"].ToString().Split(',')[1].Split(':')[1].ToString());
                    //    objpay.IGST = Convert.ToDouble(dtamount.Rows[0]["amt"].ToString().Split(',')[2].Split(':')[1].ToString());
                    //}
                    objpay.DealerId = 0;
                    //  int mcmpid = Convert.ToInt32(Session["CompanyId"].ToString());
                    var dtsubscrip = RestClient.Getmysubscription(companyid, 0);
                    if (dtsubscrip != null && dtsubscrip.Rows.Count > 0)
                    {
                        objpay.DealerId = Convert.ToInt32(dtsubscrip.Rows[0]["partnerid"]);
                    }

                    // Use validated client amount for payment gateway (includes add-ons)
                    string _amt = Math.Round(objplan.TotalAmount).ToString();


                    int _CredittAmt = Convert.ToInt32(_amt);


                    int IsRedeemcrAmt = 0;
                    if (objplan.Isrecuring == 1)
                    {
                        isReccuring = true;
                        IsRedeemcrAmt = 1;
                        _CredittAmt = 1;
                        objpay.TotalAmount = 1;
                        objpay.Amount = 1;
                    }


                    string _tId = DateTime.Now.ToString("yyyyMMddHHmmssFF");
                    _tId = _tId.PadRight(16, '0');



                    // HDFC SmartGateway - Generate customer ID
                    custid = "CUST_" + companyid + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");

                    string username = Session["cmpname"].ToString();
                    string email = Session["UserEmail"].ToString();
                    string ContactNo = cmpobj.CompanyContact;

                    // Use transaction ID as order ID
                    orderid = _tId;

                    objpay.RejectReason = orderid;
                    objpay.CredittAmt = _CredittAmt;
                    objpay.IsRedeemcrAmt = IsRedeemcrAmt;
                    objpay.IScurrencycode = objplan.IScurrencycode;

                    // Save payment first to get internal PaymentID
                    paymentID = SaveOnlinePayment(objpay);

                    // SECURITY: Store expected amount for verification in PaymentStatus callback
                    if (!string.IsNullOrEmpty(paymentID))
                    {
                        StoreExpectedAmount(paymentID, objpay.TotalAmount);
                    }

                    // Session is already stored at login using StoreSessionForCompany
                    // No need to store again here

                    // Create simple session key for udf1 (short, won't get corrupted)
                    int userId = Session["userid"] != null ? Convert.ToInt32(Session["userid"]) : 0;
                    string sessionKey = CreateSessionKey(paymentID, userId, companyid);
                    GetDeviceDetails.ProcessLogLogFileWrite("HDFCSessionKey", "PaymentID: " + paymentID + ", UserId: " + userId + ", CompanyId: " + companyid + ", Key: " + sessionKey);

                    // Create HDFC Session with session key in udf1
                    string paymentLink = CreateHDFCSession(_amt, _tId, custid, email, ContactNo, username, curcode, "Payment for Plan - " + objplan.PlanId, sessionKey);

                    var startDate = System.DateTime.Now;
                    var endDate = "";
                    if (objpay.Duration == 30)
                    {
                        endDate = Convert.ToString(startDate.AddDays(30));
                    }
                    else
                    {
                        endDate = Convert.ToString(startDate.AddDays(365));
                    }

                    List<companyAddon> lstcomAddon = new List<companyAddon>();
                    companyAddon comAddon = new companyAddon();
                    Resp resp = new Resp();
                    if (planlist != "")
                    {
                        foreach (var item in m)
                        {
                            lstcomAddon.Add(new companyAddon
                            {

                                AddonPackgeid = item.AddonPackgeid,
                                Addonid = item.Addonid,
                                PaymentId = paymentID,
                                Packagestartdate = startDate,
                                Packagetotalamt = item.Packagetotalamt,
                                Packageunitrate = item.Packageunitrate,
                                Unitquantity = item.Unitquantity,
                                Createby = Convert.ToInt32(Session["userid"]),
                                Companyid = companyid,
                                Packageenddate = Convert.ToDateTime(endDate)

                            });

                        }


                        resp = Savecompanyaddon(lstcomAddon);

                    }

                    //if (!string.IsNullOrEmpty(paymentID))
                    //{
                    //    strForm = Pay(paymentID, _tId, objplan.PlanId.ToString(), objpay.TotalAmount.ToString(), username, email, ContactNo);
                    //    //strForm = Pay(_tId, paymentID, objplan.PlanId.ToString(), objpay.TotalAmount.ToString(), username, email, ContactNo);
                    //}

                    // CreateOrder(objpay, paymentID);




                    // HDFC SmartGateway - Prepare data for redirect
                    System.Collections.Hashtable data = new System.Collections.Hashtable();
                    data.Add("amount", _amt);
                    data.Add("usename", username);
                    data.Add("email", email);
                    data.Add("contact", ContactNo);
                    data.Add("order_id", orderid);
                    data.Add("customer_id", custid);
                    data.Add("desc", paymentID);
                    data.Add("curcode", curcode);
                    data.Add("payment_link", paymentLink); // HDFC payment link
                    strForm = POSTForm(data, isReccuring);
                }
            }
            catch (Exception ex)
            {
                var reqstrmeg = string.Format("Cusid :{0},Orderid:{1},Paymentid :{2},isReccuring :{3} Error :{4}", custid, orderid, paymentID, isReccuring, ex.Message);
                GetDeviceDetails.ProcessLogLogFileWrite("PaymentExceptionLog", reqstrmeg);
                TempData["rerrormsg"] = ex.Message;
                return RedirectToAction("Planpricing", "PayTime");


            }
            finally
            {
                dtamount = null;
                objpay = null;
                cmpobj = null;
            }




            if (!string.IsNullOrEmpty(strForm))
            {
                return Content(strForm, System.Net.Mime.MediaTypeNames.Text.Html);
            }
            else
            {
                return RedirectToAction("Planpricing");
            }
            //return RedirectToAction("Planpricing");
        }

        //[HttpPost]
        #region Server-Side Amount Calculation (Security Fix for Amount Tampering)
        /// <summary>
        /// Calculates the total amount server-side to prevent amount tampering attacks.
        /// This method fetches plan details from database and recalculates the amount.
        /// </summary>
        /// <param name="planId">The plan ID selected by user</param>
        /// <param name="quantity">Number of users</param>
        /// <param name="duration">Duration in days (30 for monthly, 365 for yearly)</param>
        /// <param name="currencyCode">0 for INR, 1 for USD</param>
        /// <param name="calculatedCGST">Output: Calculated CGST amount</param>
        /// <param name="calculatedSGST">Output: Calculated SGST amount</param>
        /// <param name="calculatedBasePrice">Output: Calculated base price before tax</param>
        /// <returns>Total amount including taxes</returns>
        private double CalculateAmountServerSide(int planId, int quantity, int duration, int currencyCode,
            out double calculatedCGST, out double calculatedSGST, out double calculatedBasePrice)
        {
            calculatedCGST = 0;
            calculatedSGST = 0;
            calculatedBasePrice = 0;

            try
            {
                // Fetch plan details from database
                int companyid = Convert.ToInt32(Session["CompanyId"]);
                Getpara obj = new Getpara();
                obj.pkid = companyid;
                obj.para = 1;
                obj.iscount = 0;
                obj.IScurrencycode = currencyCode;

                var planList = RestClient.PlanGetAll(obj);
                var plan = planList?.FirstOrDefault(p => p.PlanId == planId);

                if (plan == null)
                {
                    GetDeviceDetails.ProcessLogLogFileWrite("AmountCalculationError",
                        $"Plan not found - PlanId: {planId}, CompanyId: {companyid}");
                    return 0;
                }

                // Determine price based on duration (monthly vs yearly)
                // Frontend calculates: pricePerDay * duration * quantity
                // Monthly: ppupm * 30 * qty
                // Yearly: ppupy * 365 * qty
                double pricePerUserPerDay = 0;
                if (duration <= 30) // Monthly
                {
                    pricePerUserPerDay = plan.ppupm; // Price per user per day (monthly rate)
                }
                else // Yearly (365 days)
                {
                    pricePerUserPerDay = plan.ppupy; // Price per user per day (yearly rate)
                }

                // Calculate base price: pricePerDay * duration * quantity
                calculatedBasePrice = pricePerUserPerDay * duration * quantity;

                // Parse tax rates from plan
                // TaxRate format: "CGST:9:0,SGST:9:0" where format is TaxName:Rate:Type
                // Type 0 = percentage, Type 1 = fixed amount
                double cgstRate = 0, sgstRate = 0;
                int cgstType = 0, sgstType = 0; // 0 = percentage, 1 = fixed

                if (!string.IsNullOrEmpty(plan.TaxRate))
                {
                    var taxParts = plan.TaxRate.Split(',');
                    foreach (var part in taxParts)
                    {
                        var keyValue = part.Split(':');
                        if (keyValue.Length >= 2)
                        {
                            string taxName = keyValue[0].Trim().ToUpper();
                            double taxValue = 0;
                            int taxType = 0;

                            double.TryParse(keyValue[1].Trim(), out taxValue);
                            if (keyValue.Length >= 3)
                                int.TryParse(keyValue[2].Trim(), out taxType);

                            if (taxName == "CGST")
                            {
                                cgstRate = taxValue;
                                cgstType = taxType;
                            }
                            else if (taxName == "SGST")
                            {
                                sgstRate = taxValue;
                                sgstType = taxType;
                            }
                        }
                    }
                }

                // Calculate taxes based on type (0 = percentage, 1 = fixed)
                if (cgstType == 1)
                    calculatedCGST = cgstRate; // Fixed amount
                else
                    calculatedCGST = calculatedBasePrice * cgstRate / 100; // Percentage

                if (sgstType == 1)
                    calculatedSGST = sgstRate; // Fixed amount
                else
                    calculatedSGST = calculatedBasePrice * sgstRate / 100; // Percentage

                // Calculate total
                double totalAmount = calculatedBasePrice + calculatedCGST + calculatedSGST;

                GetDeviceDetails.ProcessLogLogFileWrite("AmountCalculationSuccess",
                    $"PlanId: {planId}, Qty: {quantity}, Duration: {duration}, BasePrice: {calculatedBasePrice}, " +
                    $"CGST: {calculatedCGST}, SGST: {calculatedSGST}, Total: {totalAmount}");

                return totalAmount;
            }
            catch (Exception ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("AmountCalculationException", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Validates if the client-provided amount is not less than server-calculated minimum.
        /// This prevents amount tampering (reducing amount) while allowing add-ons (which only increase amount).
        /// </summary>
        /// <param name="clientAmount">Amount provided by client (may include add-ons)</param>
        /// <param name="serverMinimumAmount">Minimum amount calculated server-side (plan + GST, no add-ons)</param>
        /// <param name="planId">Plan ID for logging</param>
        /// <param name="companyId">Company ID for logging</param>
        /// <returns>True if client amount >= minimum, false if tampering detected (amount reduced)</returns>
        private bool ValidateAmount(double clientAmount, double serverMinimumAmount, int planId, int companyId)
        {
            // Allow small tolerance for rounding differences (1 rupee or 0.01%)
            double tolerance = Math.Max(1, serverMinimumAmount * 0.0001);

            // Client amount should be >= server minimum (add-ons only increase amount)
            if (clientAmount < (serverMinimumAmount - tolerance))
            {
                // TAMPERING DETECTED - Amount reduced below minimum
                GetDeviceDetails.ProcessLogLogFileWrite("SECURITY_ALERT_AMOUNT_TAMPERING",
                    $"CompanyId: {companyId}, PlanId: {planId}, ClientAmount: {clientAmount}, " +
                    $"ServerMinimum: {serverMinimumAmount}, Timestamp: {DateTime.Now}");
                return false;
            }

            GetDeviceDetails.ProcessLogLogFileWrite("AmountValidationSuccess",
                $"CompanyId: {companyId}, PlanId: {planId}, ClientAmount: {clientAmount}, " +
                $"ServerMinimum: {serverMinimumAmount}, Difference(AddOns): {clientAmount - serverMinimumAmount}");
            return true;
        }
        #endregion

        #region HDFC SmartGateway Methods
        /// <summary>
        /// Creates HDFC SmartGateway session and returns payment link
        /// </summary>
        public string CreateHDFCSession(string amount, string orderId, string customerId, string customerEmail, string customerPhone, string customerName, string curcode, string description, string udf1 = "")
        {
            string paymentLink = string.Empty;
            try
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;

                using (var httpClient = new HttpClient())
                {
                    // Set headers
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", HDFC_AUTH_TOKEN);
                    httpClient.DefaultRequestHeaders.Add("x-merchantid", HDFC_MERCHANT_ID);
                    httpClient.DefaultRequestHeaders.Add("x-customerid", customerId);

                    // Prepare request body with udf1 for encrypted session data
                    var requestData = new
                    {
                        order_id = orderId,
                        amount = amount,
                        customer_id = customerId,
                        customer_email = customerEmail,
                        customer_phone = customerPhone,
                        payment_page_client_id = HDFC_PAYMENT_PAGE_CLIENT_ID,
                        action = "paymentPage",
                        currency = curcode,
                        return_url = HDFC_RETURN_URL,
                        description = description,
                        first_name = customerName,
                        last_name = "",
                        udf1 = udf1  // Encrypted session data (token, paymentId, userId, etc.)
                    };

                    var jsonContent = JsonConvert.SerializeObject(requestData);
                    GetDeviceDetails.ProcessLogLogFileWrite("HDFCSessionRequest", jsonContent);

                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var response = httpClient.PostAsync(HDFC_GATEWAY_URL, content).Result;
                    var responseString = response.Content.ReadAsStringAsync().Result;

                    GetDeviceDetails.ProcessLogLogFileWrite("HDFCSessionResponse", responseString);

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);
                        paymentLink = jsonResponse.payment_links.web;
                        GetDeviceDetails.ProcessLogLogFileWrite("HDFCPaymentLink", paymentLink);
                    }
                    else
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("HDFCSessionError", "Status: " + response.StatusCode + " Response: " + responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("HDFCSessionException", ex.Message);
            }

            return paymentLink;
        }

        /// <summary>
        /// Gets order status from HDFC SmartGateway Order Status API
        /// </summary>
        public dynamic GetHDFCOrderStatus(string orderId, string customerId = "")
        {
            dynamic orderDetails = null;
            try
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;

                using (var httpClient = new HttpClient())
                {
                    // Set headers as per HDFC Order Status API
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", HDFC_AUTH_TOKEN);
                    httpClient.DefaultRequestHeaders.Add("x-merchantid", HDFC_MERCHANT_ID);
                    httpClient.DefaultRequestHeaders.Add("version", "2023-06-30");
                    if (!string.IsNullOrEmpty(customerId))
                    {
                        httpClient.DefaultRequestHeaders.Add("x-customerid", customerId);
                    }

                    // Order Status API endpoint: GET /orders/{order_id}
                    // Base URL: https://smartgateway.hdfcuat.bank.in/session -> https://smartgateway.hdfcuat.bank.in/orders/{order_id}
                    string baseUrl = HDFC_GATEWAY_URL.Replace("/session", "");
                    string orderStatusUrl = baseUrl + "/orders/" + orderId;

                    GetDeviceDetails.ProcessLogLogFileWrite("HDFCOrderStatusRequest", "URL: " + orderStatusUrl + " | OrderId: " + orderId);

                    var response = httpClient.GetAsync(orderStatusUrl).Result;
                    var responseString = response.Content.ReadAsStringAsync().Result;

                    GetDeviceDetails.ProcessLogLogFileWrite("HDFCOrderStatusResponse", responseString);

                    if (response.IsSuccessStatusCode)
                    {
                        orderDetails = JsonConvert.DeserializeObject(responseString);
                    }
                    else
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("HDFCOrderStatusError", "Status: " + response.StatusCode + " Response: " + responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("HDFCOrderStatusException", ex.Message + " | " + ex.StackTrace);
            }

            return orderDetails;
        }

        /// <summary>
        /// Kept for backward compatibility - redirects to CreateHDFCSession
        /// </summary>
        public string CreateOrder(string ammount, string Receptno, bool isReccuring, string method, string custmid, string curcode)
        {
            // Note: This method is kept for backward compatibility
            // The actual HDFC session creation is handled in Planpricing POST method
            return Receptno; // Return order ID for reference
        }

        /// <summary>
        /// CreateCustomer - Not needed for HDFC SmartGateway (customer created during session)
        /// </summary>
        public string CreateCustomer(string name, string contactno, string emailid)
        {
            // HDFC SmartGateway handles customer creation within the session
            // Return a generated customer ID for internal tracking
            return "CUST_" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
        #endregion
        /// <summary>
        /// Fetchtoken - Not used with HDFC SmartGateway
        /// HDFC handles token management differently
        /// </summary>
        public string Fetchtoken(string customerid)
        {
            // HDFC SmartGateway doesn't use the same token-based approach as Razorpay
            // Return empty string for backward compatibility
            return string.Empty;
        }
        public Resp Savecompanyaddon(List<companyAddon> objAddon)
        {

            Resp resp = new Resp();

            resp = RestClient.Savecompanyaddon(objAddon);

            if (resp.Msg == "ok")
            {
                return resp;
            }

            return resp;

        }

        public string Pay(string tId, string paymentID, string planid, string amnt, string UserName, string Email, string ContactNo)
        {
            string strForm = string.Empty;

            string MERCHANT_KEY = string.Empty;
            string SALT = string.Empty;
            string PAYU_BASE_URL = string.Empty;
            string action = string.Empty;
            string hashSequence = string.Empty;
            string RET_URL = string.Empty;

            //string RAZOR_BASE_URL = string.Empty;
            //string RAZOR_KEY = string.Empty;
            //string RAZOR_SECRET = string.Empty;
            //int Gatwayid = 1;
            try
            {
                int cmpid = Convert.ToInt32(Session["CompanyId"]);
                DataTable dt = payclint.Fillgatewaypara(cmpid);


                if (dt != null && dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //if (dt.Rows[i]["Gatewayid"].ToString() == "1")
                        //{
                        if (dt.Rows[i]["Paraname"].ToString() == "MERCHANT_KEY")
                        {
                            MERCHANT_KEY = dt.Rows[i]["Paravalue"].ToString();
                        }
                        if (dt.Rows[i]["Paraname"].ToString() == "SALT")
                        {
                            SALT = dt.Rows[i]["Paravalue"].ToString();
                        }
                        if (dt.Rows[i]["Paraname"].ToString() == "PAYU_BASE_URL")
                        {
                            PAYU_BASE_URL = dt.Rows[i]["Paravalue"].ToString();
                        }
                        if (dt.Rows[i]["Paraname"].ToString() == "action")
                        {
                            action = dt.Rows[i]["Paravalue"].ToString();
                        }
                        if (dt.Rows[i]["Paraname"].ToString() == "hashSequence")
                        {
                            hashSequence = dt.Rows[i]["Paravalue"].ToString();
                        }
                        if (dt.Rows[i]["Paraname"].ToString() == "RET_URL")
                        {
                            string weburl = string.Empty;
                            if (Request.Url != null)
                            {
                                string strPathAndQuery = Request.Url.PathAndQuery;
                                weburl = Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

                            }
                            else
                            {
                                weburl = ConfigurationManager.AppSettings["weburlminop"].ToString();
                            }
                            RET_URL = weburl + dt.Rows[i]["Paravalue"].ToString();
                            GetDeviceDetails.ProcessLogLogFileWrite("PaymentLogReturnurl", RET_URL);
                        }
                        if (dt.Rows[i]["Paraname"].ToString() == "RET_URL")
                        {
                            string weburl = string.Empty;
                            if (Request.Url != null)
                            {
                                string strPathAndQuery = Request.Url.PathAndQuery;
                                weburl = Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

                            }
                            else
                            {
                                weburl = ConfigurationManager.AppSettings["weburlminop"].ToString();
                            }
                            RET_URL = weburl + dt.Rows[i]["Paravalue"].ToString();
                            GetDeviceDetails.ProcessLogLogFileWrite("PaymentLogReturnurl", RET_URL);
                        }

                        //}
                        //else
                        //{

                        //    Gatwayid = 2;

                        //    if (dt.Rows[i]["Paraname"].ToString() == "RAZOR_BASE_URL")
                        //    {
                        //        RAZOR_BASE_URL = dt.Rows[i]["Paravalue"].ToString();
                        //    }
                        //    if (dt.Rows[i]["Paraname"].ToString() == "RAZOR_KEY")
                        //    {
                        //        RAZOR_KEY = dt.Rows[i]["Paravalue"].ToString();
                        //    }
                        //    if (dt.Rows[i]["Paraname"].ToString() == "RAZOR_SECRET")
                        //    {
                        //        RAZOR_SECRET = dt.Rows[i]["Paravalue"].ToString();
                        //    }

                        //}
                    }

                    //if (Gatwayid == 1)
                    //{
                    #region Payyou
                    string[] hashVarsSeq;
                    string hash_string = string.Empty;
                    string txtPayFor = "Minop Cloud";
                    txnid1 = tId;

                    if (string.IsNullOrEmpty(Request.Form["hash"])) // generating hash value
                    {
                        if (
                            //string.IsNullOrEmpty(ConfigurationManager.AppSettings["MERCHANT_KEY"]) ||
                            string.IsNullOrEmpty(MERCHANT_KEY) ||
                            string.IsNullOrEmpty(txnid1) ||
                            string.IsNullOrEmpty(amnt) ||
                            string.IsNullOrEmpty(UserName) ||
                            string.IsNullOrEmpty(Email) ||
                            string.IsNullOrEmpty(txtPayFor.Trim())
                            )
                        {
                            return string.Empty;
                        }

                        else
                        {
                            //frmError.Visible = false;
                            //hashVarsSeq = ConfigurationManager.AppSettings["hashSequence"].Split('|'); // spliting hash sequence from config
                            hashVarsSeq = hashSequence.Split('|');
                            hash_string = "";
                            foreach (string hash_var in hashVarsSeq)
                            {
                                if (hash_var == "key")
                                {
                                    //hash_string = hash_string + ConfigurationManager.AppSettings["MERCHANT_KEY"];
                                    hash_string = hash_string + MERCHANT_KEY;
                                    hash_string = hash_string + '|';
                                }
                                else if (hash_var == "txnid")
                                {
                                    hash_string = hash_string + txnid1;
                                    hash_string = hash_string + '|';
                                }
                                else if (hash_var == "amount")
                                {
                                    hash_string = hash_string + Convert.ToDecimal(amnt).ToString("g29");
                                    hash_string = hash_string + '|';
                                }
                                else if (hash_var == "productinfo")
                                {
                                    hash_string = hash_string + txtPayFor.Trim();
                                    hash_string = hash_string + '|';
                                }
                                else if (hash_var == "firstname")
                                {
                                    hash_string = hash_string + UserName;
                                    hash_string = hash_string + '|';
                                }
                                else if (hash_var == "email")
                                {
                                    hash_string = hash_string + Email;
                                    hash_string = hash_string + '|';
                                }
                                else if (hash_var == "udf1")
                                {
                                    hash_string = hash_string + paymentID;
                                    hash_string = hash_string + '|';
                                }
                                else if (hash_var == "udf2")
                                {
                                    hash_string = hash_string + planid;
                                    hash_string = hash_string + '|';
                                }
                                else
                                {
                                    hash_string = hash_string + (Request.Form[hash_var] != null ? Request.Form[hash_var] : "");// isset if else
                                    hash_string = hash_string + '|';
                                }
                            }

                            //hash_string += ConfigurationManager.AppSettings["SALT"];// appending SALT

                            hash_string += SALT;

                            hash1 = Generatehash512(hash_string).ToLower();         //generating hash
                            //action1 = ConfigurationManager.AppSettings["PAYU_BASE_URL"] + "/_payment";// setting URL
                            action1 = PAYU_BASE_URL + "/" + action;// setting URL
                        }
                    }

                    else if (!string.IsNullOrEmpty(Request.Form["hash"]))
                    {
                        hash1 = Request.Form["hash"];
                        //action1 = ConfigurationManager.AppSettings["PAYU_BASE_URL"] + "/_payment";
                        action1 = PAYU_BASE_URL + "/" + action;
                    }
                    if (!string.IsNullOrEmpty(hash1))
                    {
                        //hash.Value = hash1;
                        //txnid.Value = txnid1;

                        System.Collections.Hashtable data = new System.Collections.Hashtable(); // adding values in gash table for data post
                        data.Add("hash", hash1);
                        data.Add("txnid", txnid1);
                        data.Add("key", MERCHANT_KEY);
                        string AmountForm = Convert.ToDecimal(amnt).ToString("g29");// eliminating trailing zeros
                        data.Add("amount", AmountForm);
                        data.Add("firstname", UserName);
                        data.Add("email", Email);
                        data.Add("phone", ContactNo);
                        data.Add("productinfo", txtPayFor.Trim());
                        data.Add("surl", RET_URL);
                        data.Add("furl", RET_URL);
                        data.Add("lastname", "");
                        data.Add("curl", "");
                        data.Add("address1", "");
                        data.Add("address2", "");
                        data.Add("city", "");
                        data.Add("state", "");
                        data.Add("country", "");
                        data.Add("zipcode", "");
                        data.Add("udf1", paymentID);
                        data.Add("udf2", planid);
                        data.Add("udf3", "");
                        data.Add("udf4", "");
                        data.Add("udf5", "");
                        data.Add("pg", "");

                        strForm = PreparePOSTForm(action1, data);
                    }

                    else
                    {
                        //no hash
                    }
                    #endregion
                    //}
                    //else
                    //{
                    //    #region Razorpay
                    //    RazorpayClient client = new RazorpayClient(RAZOR_KEY, RAZOR_SECRET);
                    //    ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;

                    //    Dictionary<string, object> options = new Dictionary<string, object>();
                    //    options.Add("amount", Convert.ToDecimal(amnt).ToString("g29"));
                    //    options.Add("currency", "INR");
                    //    options.Add("receipt", tId);
                    //    Razorpay.Api.Order order = client.Order.Create(options);



                    //    //Payment paymentCaptured = payment.Capture(options);
                    //    #endregion
                    //}












                }
                else
                {
                    GetDeviceDetails.ProcessLogLogFileWrite("PaymentLog", "Gatwway paramer not found");
                }


            }
            catch (Exception ex)
            {
                //remove comment from here

                //saveError(tId, pId, ex.Message.ToString());
                //divError.InnerHtml = ex.Message.ToString();
                //DispMessage("e");
            }

            return strForm;
        }
        private string PreparePOSTForm(string url, System.Collections.Hashtable data)      // post form
        {
            //Set a name for the form
            string formID = "PostForm";
            //Build the form using the specified data to be posted.
            StringBuilder strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" +
                           formID + "\" action=\"" + url +
                           "\" method=\"POST\">");

            foreach (System.Collections.DictionaryEntry key in data)
            {

                strForm.Append("<input type=\"hidden\" name=\"" + key.Key +
                               "\" value=\"" + key.Value + "\">");
            }

            strForm.Append("</form>");
            //Build the JavaScript which will do the Posting operation.
            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("var v" + formID + " = document." +
                             formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");
            //Return the form and the script concatenated.
            //(The order is important, Form then JavaScript)
            return strForm.ToString() + strScript.ToString();
        }
        /// <summary>
        /// Generates redirect script to HDFC SmartGateway payment page
        /// </summary>
        private string POSTForm(System.Collections.Hashtable data, bool isrecuring)
        {
            StringBuilder strScript = new StringBuilder();
            string paymentLink = data["payment_link"]?.ToString() ?? "";

            if (!string.IsNullOrEmpty(paymentLink))
            {
                // Redirect to HDFC SmartGateway payment page
                strScript.Append("<!DOCTYPE html><html><head><title>Redirecting to Payment...</title></head>");
                strScript.Append("<body style='font-family: Arial; text-align: center; padding-top: 100px;'>");
                strScript.Append("<h2>Redirecting to HDFC Payment Gateway...</h2>");
                strScript.Append("<p>Please wait while we redirect you to the secure payment page.</p>");
                strScript.Append("<p>If you are not redirected automatically, <a href='" + paymentLink + "'>click here</a>.</p>");
                strScript.Append("<script language='javascript'>");
                strScript.Append("window.location.href = '" + paymentLink + "';");
                strScript.Append("</script>");
                strScript.Append("</body></html>");
            }
            else
            {
                strScript.Append("<!DOCTYPE html><html><head><title>Payment Error</title></head>");
                strScript.Append("<body style='font-family: Arial; text-align: center; padding-top: 100px;'>");
                strScript.Append("<h2>Payment Session Error</h2>");
                strScript.Append("<p>Unable to create payment session. Please try again.</p>");
                strScript.Append("<a href='/PayTime/Planpricing'>Go Back</a>");
                strScript.Append("</body></html>");
            }






            //Return the form and the script concatenated.
            //(The order is important, Form then JavaScript)
            return strScript.ToString();
        }
        public string Generatehash512(string text)
        {
            byte[] message = Encoding.UTF8.GetBytes(text);
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }
        private string SaveOnlinePayment(Paymenttrans obj)
        {
            ResMsg res = new ResMsg();
            try
            {
                obj.PaymentMode = 2;
                obj.Companyid = Convert.ToInt32((Session["CompanyId"] != null) ? Session["CompanyId"] : 0);
                obj.Companycode = Session["cmpcode"].ToString();
                res = payclint.InitPayment(obj);
                if (res.MsgSts == "ok")
                {
                    return res.Msg;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("PaymentExceptionLog", ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// HDFC SmartGateway Payment Status Handler
        /// Handles callback from HDFC after payment completion
        /// </summary>
        public ActionResult PaymentStatus()
        {
            Paymentres payres = new Paymentres();
            ResMsg res = new ResMsg();
            string cachedToken = null;
            string internalPaymentID = null;

            try
            {
                // Log all received parameters for debugging
                string allParams = "HDFC Response - ";
                foreach (string key in Request.Form.AllKeys)
                {
                    allParams += key + "=" + Request.Form[key] + " | ";
                }
                foreach (string key in Request.QueryString.AllKeys)
                {
                    allParams += key + "=" + Request.QueryString[key] + " | ";
                }

                // Also try to read JSON body if present
                string jsonBody = "";
                dynamic jsonResponse = null;
                try
                {
                    Request.InputStream.Position = 0;
                    using (var reader = new StreamReader(Request.InputStream))
                    {
                        jsonBody = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(jsonBody))
                        {
                            jsonResponse = JsonConvert.DeserializeObject(jsonBody);
                            allParams += " | JSON Body: " + jsonBody;
                        }
                    }
                }
                catch { }

                GetDeviceDetails.ProcessLogLogFileWrite("HDFCPaymentResponse", allParams);

                // HDFC SmartGateway response parameters - check Form, QueryString, and JSON body
                var _orderID = Request.Form["order_id"] ?? Request.QueryString["order_id"] ??
                               (jsonResponse?.order_id?.ToString()) ?? "";
                var _status = Request.Form["status"] ?? Request.QueryString["status"] ??
                              (jsonResponse?.status?.ToString()) ?? "";
                var _txnId = Request.Form["txn_id"] ?? Request.QueryString["txn_id"] ??
                             (jsonResponse?.txn_id?.ToString()) ?? (jsonResponse?.txn_uuid?.ToString()) ?? "";
                var _amount = Request.Form["amount"] ?? Request.QueryString["amount"] ??
                              (jsonResponse?.amount?.ToString()) ?? "0";
                var _signature = Request.Form["signature"] ?? Request.QueryString["signature"] ??
                                 (jsonResponse?.signature?.ToString()) ?? "";
                var _customerId = Request.Form["customer_id"] ?? Request.QueryString["customer_id"] ??
                                  (jsonResponse?.customer_id?.ToString()) ?? "";
                var _paymentMethod = Request.Form["payment_method"] ?? Request.QueryString["payment_method"] ??
                                     (jsonResponse?.payment_method?.ToString()) ?? "";
                var _errorMessage = Request.Form["error_message"] ?? Request.QueryString["error_message"] ??
                                    (jsonResponse?.error_message?.ToString()) ?? (jsonResponse?.bank_error_message?.ToString()) ?? "";

                // Call Order Status API to get complete payment details including udf1
                string _udf1 = Request.Form["udf1"] ?? Request.QueryString["udf1"] ?? (jsonResponse?.udf1?.ToString()) ?? "";

                if (!string.IsNullOrEmpty(_orderID))
                {
                    dynamic orderStatusResponse = GetHDFCOrderStatus(_orderID, _customerId);
                    if (orderStatusResponse != null)
                    {
                        // Override with Order Status API response (more reliable)
                        _status = orderStatusResponse.status?.ToString() ?? _status;
                        _txnId = orderStatusResponse.txn_id?.ToString() ?? orderStatusResponse.txn_uuid?.ToString() ?? _txnId;
                        _amount = orderStatusResponse.amount?.ToString() ?? _amount;
                        _customerId = orderStatusResponse.customer_id?.ToString() ?? _customerId;
                        _paymentMethod = orderStatusResponse.payment_method?.ToString() ?? _paymentMethod;
                        _errorMessage = orderStatusResponse.bank_error_message?.ToString() ?? _errorMessage;

                        // Get udf1 from Order Status API response (contains encrypted session data)
                        if (string.IsNullOrEmpty(_udf1))
                        {
                            _udf1 = orderStatusResponse.udf1?.ToString() ?? "";
                        }

                        GetDeviceDetails.ProcessLogLogFileWrite("HDFCOrderStatusUsed",
                            "Status: " + _status + " | TxnId: " + _txnId + " | Amount: " + _amount + " | UDF1: " + (!string.IsNullOrEmpty(_udf1) ? "Present" : "Empty"));
                    }
                }

                // Parse session key from udf1 (format: paymentId-userId-companyId)
                string parsedPaymentId = "";
                int parsedUserId = 0, parsedCompanyId = 0;

                if (!string.IsNullOrEmpty(_udf1))
                {
                    if (ParseSessionKey(_udf1, out parsedPaymentId, out parsedUserId, out parsedCompanyId))
                    {
                        internalPaymentID = parsedPaymentId;

                        // Get session data from cache using companyId (stored at login time)
                        var sessionData = GetSessionForCompany(parsedCompanyId.ToString());
                        if (sessionData != null)
                        {
                            // Restore ALL session variables (including empty ones) for user to stay logged in
                            foreach (var kvp in sessionData)
                            {
                                Session[kvp.Key] = kvp.Value ?? "";
                            }
                            cachedToken = sessionData.ContainsKey("tokan") ? sessionData["tokan"] : "";
                            GetDeviceDetails.ProcessLogLogFileWrite("HDFCSessionRestored",
                                "Restored " + sessionData.Count + " session variables for CompanyId: " + parsedCompanyId);
                        }
                        else
                        {
                            // Fallback: set basic session variables from udf1
                            Session["userid"] = parsedUserId;
                            Session["cmpid"] = parsedCompanyId;
                            GetDeviceDetails.ProcessLogLogFileWrite("HDFCSessionCacheMiss",
                                "No cached session found for CompanyId: " + parsedCompanyId + ", using udf1 values");
                        }

                        GetDeviceDetails.ProcessLogLogFileWrite("HDFCSessionKeyParsed",
                            "Session restored - UserId: " + parsedUserId + ", CompanyId: " + parsedCompanyId + ", PaymentID: " + parsedPaymentId +
                            ", Token: " + (!string.IsNullOrEmpty(cachedToken) ? "Found" : "NotFound") +
                            ", DbName: " + (Session["DbName"] != null ? "Set" : "NotSet") +
                            ", CmpCode: " + (Session["cmpcode"] != null ? "Set" : "NotSet") +
                            ", CompanyId: " + (Session["CompanyId"] != null ? "Set" : "NotSet"));
                    }
                    else
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("HDFCSessionKeyParseFailed", "Failed to parse udf1: " + _udf1 + " for order_id: " + _orderID);
                    }
                }
                else
                {
                    GetDeviceDetails.ProcessLogLogFileWrite("HDFCUDF1NotFound", "No udf1 data for order_id: " + _orderID);
                }

                // Determine payment status based on HDFC response
                // HDFC/Juspay common statuses: CHARGED, PENDING, FAILED, AUTHORIZATION_FAILED
                bool isSuccess = _status.ToUpper() == "CHARGED" || _status.ToUpper() == "SUCCESS" || _status.ToUpper() == "TXN_SUCCESS";

                // ========== SECURITY FIX: Verify Amount in Callback ==========
                // Check if paid amount matches expected amount (prevents tampering that bypassed first check)
                if (isSuccess && !string.IsNullOrEmpty(internalPaymentID))
                {
                    double? expectedAmount = GetAndRemoveExpectedAmount(internalPaymentID);
                    if (expectedAmount.HasValue)
                    {
                        double paidAmount = 0;
                        double.TryParse(_amount, out paidAmount);

                        // Allow small tolerance for rounding (1 rupee or 0.01%)
                        double tolerance = Math.Max(1, expectedAmount.Value * 0.0001);
                        double difference = Math.Abs(paidAmount - expectedAmount.Value);

                        if (difference > tolerance)
                        {
                            // AMOUNT MISMATCH DETECTED - Possible tampering or payment gateway issue
                            GetDeviceDetails.ProcessLogLogFileWrite("SECURITY_ALERT_CALLBACK_AMOUNT_MISMATCH",
                                $"PaymentId: {internalPaymentID}, ExpectedAmount: {expectedAmount.Value}, " +
                                $"PaidAmount: {paidAmount}, Difference: {difference}, OrderId: {_orderID}, " +
                                $"Status: {_status}, Timestamp: {DateTime.Now}");

                            // Mark as failed due to amount mismatch
                            isSuccess = false;
                            _errorMessage = "Amount verification failed. Expected: " + expectedAmount.Value + ", Received: " + paidAmount;
                        }
                        else
                        {
                            GetDeviceDetails.ProcessLogLogFileWrite("AmountVerificationSuccess",
                                $"PaymentId: {internalPaymentID}, ExpectedAmount: {expectedAmount.Value}, PaidAmount: {paidAmount}");
                        }
                    }
                    else
                    {
                        // No expected amount found - could be expired cache or direct callback
                        GetDeviceDetails.ProcessLogLogFileWrite("AmountVerificationSkipped",
                            $"No expected amount in cache for PaymentId: {internalPaymentID}");
                    }
                }
                // ========== END SECURITY FIX ==========

                // Use internal PaymentID (from SaveOnlinePayment) instead of order_id
                payres.PaymentID = !string.IsNullOrEmpty(internalPaymentID) ? internalPaymentID : _orderID;
                payres.txnid = _orderID; // Store our order_id (as per old Razorpay flow)
                payres.Paymentstatus = isSuccess ? "Success" : "Failed";
                payres.ResDate = DateTime.Now;
                payres.Uniqhash = _signature;
                payres.customerId = _customerId;
                payres.tokenid = "";
                payres.rzppaymentid = _txnId; // Store HDFC transaction ID (SG4000-xxx-1)

                if (isSuccess)
                {
                    payres.IsActive = "1";
                }
                else
                {
                    payres.IsActive = "0";
                }

                string Message = "OrderID:" + _orderID + "|InternalPaymentID:" + internalPaymentID + "|TxnId:" + _txnId + "|Status:" + _status + "|Amount:" + _amount;
                GetDeviceDetails.ProcessLogLogFileWrite("HDFCPaymentResponseLog", Message);

                // Pass token from cache to Savepaymentres
                res = payclint.Savepaymentres(payres, cachedToken);

                if (res != null && res.MsgSts == "ok")
                {
                    try
                    {
                        planRestrictions(0);
                    }
                    catch
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("PayressuccRestfail", Message);
                    }
                    GetDeviceDetails.ProcessLogLogFileWrite("HDFCPaymentResponsesavesuccessLog", Message);
                }
                else
                {
                    if (res == null)
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("HDFCPaymentResponsesavefailLog", "Exception on payment response save: " + Message);
                    }
                    else
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("HDFCPaymentResponsesavefailLog", res.Msg + " Req: " + Message);
                    }
                }

                // Set TempData for UI display
                if (isSuccess)
                {
                    TempData["Msgstatus"] = "1";
                    TempData["Msg"] = "<div class='successpay' style='background-color: #8fe285; padding: 5px 5px 5px 5px; padding-left: 15px;'><p><h1> <i class='fa fa-check-circle' style='color:green'></i> Thank you,</h1></p><p class='psuccess'>Your payment was successful.</p><p>Amount : " + _amount + "</p> <p>TransactionID : " + _txnId + "</p> <p> Order ID : " + _orderID + "</p><p> Status : <b style='color: green;'>Success</b></p></div>";
                }
                else
                {
                    TempData["Msgstatus"] = "0";
                    string errorDetail = !string.IsNullOrEmpty(_errorMessage) ? _errorMessage : _status;
                    TempData["Msg"] = "<div class='failpay' style='background-color: #f8d7da; padding: 5px 5px 5px 5px; padding-left: 15px;'><p><h1><i class='fa fa-times-circle' style='color:#721c24'></i> Oops!</h1></p><p class='pfaild'>Your payment failed.</p><p>Amount : " + _amount + "</p> <p>Order ID : " + _orderID + "</p><p> Status : <b style='color: red;'>" + errorDetail + "</b></p></div>";
                }
            }
            catch (Exception ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("HDFCPaymentStatusException", ex.Message);
                TempData["Msgstatus"] = "0";
                TempData["Msg"] = "<div class='failpay' style='background-color: #f8d7da; padding: 5px;'><p><h1><i class='fa fa-times-circle' style='color:#721c24'></i> Error</h1></p><p>An error occurred while processing payment response.</p></div>";
            }


            return RedirectToAction("Mysubscription");


        }

        [HttpPost]
        [CustAuthFilter]
        public ActionResult Cancelsubscription(Getpara obj)
        {
            DataTable dt = new DataTable();

            try
            {
                dt = payclint.Cancelsubscription(obj);
                if (dt.Rows.Count > 0)
                {
                    var companyname = dt.Rows[0]["companyName"].ToString();
                    var Noofuser = Convert.ToInt32(dt.Rows[0]["Emp_count"].ToString());

                    if (Convert.ToInt32(dt.Rows[0]["finaltotal"].ToString()) != 0)
                    {
                        var Orderid = string.Empty;
                        string _tId = DateTime.Now.ToString("yyyyMMddHHmmssFF");
                        _tId = _tId.PadRight(16, '0');
                        var totalamount = dt.Rows[0]["finaltotal"].ToString();
                        var custid = dt.Rows[0]["customer_id"].ToString();
                        var tokenid = dt.Rows[0]["token"].ToString();
                        var Email = dt.Rows[0]["email"].ToString();
                        var contact = dt.Rows[0]["contact"].ToString();

                        var companyid = Convert.ToInt32(dt.Rows[0]["Companyid"].ToString());
                        var Dealerid = Convert.ToInt32(dt.Rows[0]["DealerId"].ToString());
                        var planid = Convert.ToInt32(dt.Rows[0]["PlanId"].ToString());

                        var _Taxofpyament = "TaxName : GST TaxAmount: " + dt.Rows[0]["gst"].ToString() + " TaxPer: 18 TaxVtype :2";
                        var rcptno = "Receipt_" + Guid.NewGuid().ToString().Substring(0, 6);
                        var curruncy = dt.Rows[0]["curruncy"].ToString();
                        var isrecurring = dt.Rows[0]["isrecurring"].ToString();
                        var DayDuration = Convert.ToInt32(dt.Rows[0]["Dayduration"].ToString());
                        string currencycode = "INR";
                        if (obj.IScurrencycode == 1)
                        {
                            currencycode = "USD";
                        }
                        if (!string.IsNullOrEmpty(tokenid))
                        {
                            Orderid = CreateOrder(totalamount, rcptno, false, "", "", currencycode);

                            Paymenttrans objpay = new Paymenttrans();
                            objpay.TotalAmount = Convert.ToDouble(totalamount);
                            objpay.CGST = 0.00;
                            objpay.SGST = 0.00;
                            objpay.Companyid = companyid;
                            objpay.DisccountAmt = 0;
                            objpay.DisccountPer = 0;
                            objpay.Dplanid = planid;
                            objpay.Duration = DayDuration;
                            objpay.Noofuser = Noofuser;
                            objpay.InvoiceData = _Taxofpyament;
                            objpay.IGST = 0.00;
                            objpay.IsApproved = 0;
                            objpay.InvoiceNo = "CR";
                            objpay.ReceptNo = rcptno;
                            objpay.DealerId = Dealerid;
                            objpay.RejectReason = Orderid;
                            objpay.CredittAmt = Convert.ToDouble(totalamount);
                            objpay.IsRedeemcrAmt = 0;

                            var paymentid = SaveOnlinePayment(objpay);


                            if (!string.IsNullOrEmpty(paymentid))
                            {

                                if (!string.IsNullOrEmpty(Orderid))
                                {
                                    if (Makerecurin(Email, contact, curruncy, objpay.TotalAmount.ToString(), Orderid, custid, tokenid, isrecurring, paymentid, companyname))
                                    {
                                        return RedirectToAction("Planpricing");
                                    }
                                }

                            }
                            else
                            {

                                GetDeviceDetails.ProcessLogLogFileWrite("StoprecurringPaymentlog", "Paymenttrasaction not save properly");

                            }

                        }
                        else
                        {
                            GetDeviceDetails.ProcessLogLogFileWrite("StoprecurringPaymentlog", "token id not exist for customer");

                        }



                    }
                    else
                    {
                        GetDeviceDetails.ProcessLogLogFileWrite("StoprecurringPaymentlog", companyname + " have " + Noofuser + " user(s)  with zero Amount.");
                        return RedirectToAction("Planpricing");
                    }
                }

            }
            catch (Exception ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("StoprecurringPaymentlog", ex.Message);
            }

            return RedirectToAction("Mysubscription");
        }

        /// <summary>
        /// Makerecurin - Recurring payment processing
        /// Note: HDFC SmartGateway requires different implementation for recurring payments
        /// </summary>
        public bool Makerecurin(string _email, string _contact, string _currency, string _amount, string _orderid, string _custid, string _token, string recurring, string desc, string cname)
        {
            // TODO: Implement HDFC SmartGateway recurring payment if needed
            // HDFC has different API for recurring/subscription payments
            GetDeviceDetails.ProcessLogLogFileWrite("HDFCRecurringPayment",
                string.Format("Recurring payment requested - Email:{0}, Amount:{1}, OrderId:{2}, CustomerId:{3}",
                _email, _amount, _orderid, _custid));

            // Return false for now - implement HDFC recurring payment API when available
            return false;
        }


        #endregion
        [HttpPost]
        public ActionResult UploadPhoto(HttpPostedFileBase EmpPhotoFile)
        {
            if (EmpPhotoFile != null && EmpPhotoFile.ContentLength > 0)
            {
                string extension = Path.GetExtension(EmpPhotoFile.FileName);

                // Use shorter unique ID
                string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8); // 8 characters

                // Max total length = 30
                int maxTotalLength = 30;
                int fixedLength = uniqueId.Length + extension.Length + 1; // 1 for underscore

                // Max allowed length for the original name
                int maxNameLength = maxTotalLength - fixedLength;

                // Trim original name if too long
                string originalName = Path.GetFileNameWithoutExtension(EmpPhotoFile.FileName);
                if (originalName.Length > maxNameLength)
                {
                    originalName = originalName.Substring(0, maxNameLength);
                }

                // Final filename
                string fileName = $"{originalName}_{uniqueId}{extension}";

                // Save path
                string folderPath = Server.MapPath("~/UploadEmpPhoto");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fullPath = Path.Combine(folderPath, fileName);
                EmpPhotoFile.SaveAs(fullPath);

                // Return the filename only (you can prefix with path on client side)
                return Json(new { imagePath = fileName }, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(400, "No file uploaded.");
        }

        //[HttpPost]
        //public ActionResult UploadPhoto(HttpPostedFileBase EmpPhotoFile)
        //{
        //    if (EmpPhotoFile != null && EmpPhotoFile.ContentLength > 0)
        //    {

        //        string extension = Path.GetExtension(EmpPhotoFile.FileName);

        //        // Generate unique name
        //        string uniqueId = Guid.NewGuid().ToString(); // 36 characters

        //        // Calculate max length for original name part
        //        int maxNameLength = 100 - uniqueId.Length - extension.Length - 1; // -1 for underscore

        //        // Get original file name (without extension) and trim if needed
        //        string originalName = Path.GetFileNameWithoutExtension(EmpPhotoFile.FileName);
        //        if (originalName.Length > maxNameLength)
        //        {
        //            originalName = originalName.Substring(0, maxNameLength);
        //        }

        //        // Final file name: original + _ + guid + extension
        //        string fileName = $"{originalName}_{uniqueId}{extension}";

        //        // Define save path
        //        string folderPath = Server.MapPath("~/UploadEmpPhoto");

        //        // Ensure folder exists
        //        if (!Directory.Exists(folderPath))
        //        {
        //            Directory.CreateDirectory(folderPath);
        //        }

        //        // Save the file
        //        string fullPath = Path.Combine(folderPath, fileName);
        //        EmpPhotoFile.SaveAs(fullPath);

        //        // Return relative path
        //        string relativePath = Url.Content(fileName);
        //        return Json(new { imagePath = relativePath }, JsonRequestBehavior.AllowGet);
        //    }

        //    return new HttpStatusCodeResult(400, "No file uploaded.");
        //}

        public ActionResult CancellationandRefundPolicy()
        {
            return View();
        }

        #region RealesedNote
        [HttpGet]
        public ActionResult RealesedNote()
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

        #endregion

        #region SuperadminAnalyticsDashboard
        [CustAuthFilter]
        public ActionResult AnalyticsDashboard()
        {
            ViewBag.AnalyticsData = RestClient.Analyticsdata();
            return View();
        }
        [CustAuthFilter]
        public ActionResult SuperAdminAnalyticsDetails(string fname)
        {
            AnalyticsDashboardDetails obj = new AnalyticsDashboardDetails();
            try
            {
                obj.AnalyticsDashboardDetailslist = RestClient.GetAnalyticsData(fname);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
            return View(obj);
        }
        [HttpPost]
        [CustAuthFilter]
        public ActionResult AnalyticsDashboard(srchdata srch)
        {
            AnalyticsDashboardDetails obj = new AnalyticsDashboardDetails();
            try
            {
                ViewBag.AnalyticsData = RestClient.GetallSuperAdminCounter(srch);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
            return View();
        }
        [HttpPost]
        public ActionResult SuperAdminAnalyticsDetails(srchfilter srchfil)
        {
            AnalyticsDashboardDetails obj = new AnalyticsDashboardDetails();
            try
            {
                obj.AnalyticsDashboardDetailslist = RestClient.GetallSuperAdminfilter(srchfil);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
            return View(obj);
        }

        public ActionResult AnalyticsReports()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AnalyticsReports(SuperAdminReportsFilter obj)
        {
            DataTable model = RestClient.DynamicReports(obj);
            Session["data"] = model;
            return View("AnalyticsReports", model);
        }
        #endregion

        #region OTPCheck
        [HttpGet]
        public ActionResult OTPCheck()
        {
            OTPCheck OtpChk = new OTPCheck();
            if (Convert.ToString(Session["cmpcode"]) != null && Convert.ToInt32(Session["RoleId"]) != 0 && Convert.ToString(Session["UserEmail"]) != null)
            {
                OtpChk.CompanyCode = Session["cmpcode"].ToString();
                OtpChk.RoleId = Convert.ToInt32(Session["RoleId"].ToString());
                OtpChk.Email = Session["UserEmail"].ToString();
            }
            else
            {
                return RedirectToAction("LoginPage", "PayTime");
            }
            return View(OtpChk);
        }

        [HttpPost]
        public ActionResult OTPCheck(OTPCheck model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.OTP != 0 && !string.IsNullOrEmpty(model.CompanyCode) && !string.IsNullOrEmpty(model.Email) && model.RoleId != 0)
                    {
                        if (Convert.ToString(Session["cmpcode"]) != null && Convert.ToInt32(Session["RoleId"]) != 0 && Convert.ToString(Session["UserEmail"]) != null)
                        {

                            resp = AccountRestClint.OTPCheck(model.OTP, model.CompanyCode, model.Email, model.RoleId);
                            ViewBag.MegSts = resp.MegSts;
                            ViewBag.Meg = resp.Meg;
                            if (model.RoleId > 1 && model.RoleId != 6805 && model.RoleId != 6806)
                            {
                                if (ViewBag.MegSts == "Ok")
                                {
                                    return RedirectToAction("EmployeeDashboard", "Dashboard");
                                }
                            }
                            else
                            {
                                if (ViewBag.MegSts == "Ok")
                                {
                                    return RedirectToAction("AdminDashboard", "Dashboard");
                                }
                            }

                        }
                        else
                        {
                            return RedirectToAction("LoginPage", "PayTime");
                        }
                    }
                    else
                    {
                        ViewBag.MegSts = "Error";
                    }
                }
                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [HttpPost]
        public JsonResult VerifyLoginOTP(int OTP)
        {
            try
            {
                string companyCode = Convert.ToString(Session["cmpcode"]);
                int roleId = Convert.ToInt32(Session["RoleId"]);
                string email = Convert.ToString(Session["UserEmail"]);

                if (string.IsNullOrEmpty(companyCode) || roleId == 0 || string.IsNullOrEmpty(email))
                {
                    return Json(new { MegSts = "Error", Meg = "Session expired. Please login again.", RedirectUrl = "/PayTime/LoginPage" });
                }

                var resp = AccountRestClint.OTPCheck(OTP, companyCode, email, roleId);

                if (resp.MegSts == "Ok")
                {
                    string redirectUrl;
                    if (roleId > 1 && roleId != 6805 && roleId != 6806)
                    {
                        redirectUrl = Url.Action("EmployeeDashboard", "Dashboard");
                    }
                    else
                    {
                        redirectUrl = Url.Action("AdminDashboard", "Dashboard");
                    }
                    return Json(new { MegSts = resp.MegSts, Meg = resp.Meg, RedirectUrl = redirectUrl });
                }
                else
                {
                    return Json(new { MegSts = resp.MegSts, Meg = resp.Meg, RedirectUrl = "" });
                }
            }
            catch (Exception)
            {
                return Json(new { MegSts = "Error", Meg = "Something went wrong. Please try again.", RedirectUrl = "" });
            }
        }
        #endregion

        #region EmployeeChange Password
        [HttpGet]
        public ActionResult EmployeeChangePassword()
        {
            return View();
        }
        #endregion

    }
}