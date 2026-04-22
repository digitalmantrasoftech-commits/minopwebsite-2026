using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Models;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.IO;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using Newtonsoft.Json;
namespace PayTimeWebClient.Controllers
{
    public class DealerController : Controller
    {

        static readonly IDealerdataclient DealerAcco = new Dealerdataclient();
        static readonly Dealerdataclient DealerClient = new Dealerdataclient();
        Paymentintegrationclient payclint = new Paymentintegrationclient();
        MRespo resp = new MRespo();
        Resp pm = new Resp();

        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Registration(DealerRegisterModel obj)
        {

            if (Session["UserId"] != null)
            {
                obj.Actmanagerid = Convert.ToInt32(Session["UserId"]);
            }
            resp = DealerAcco.Register(obj);
            bool success = false;
            if (resp.MegSts == "ok" || resp.MegSts == "Ok")
            {
                success = true;
            }

            return Json(new { Resp = resp, IsSuccess = success }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(DealerLogin model, string rememberme)
        {
            Resp resp = new Resp();
            try
            {

                if (ModelState.IsValid)
                {
                    resp = DealerAcco.Login(model);
                    if (resp.MsgSts == "jwt_token")
                    {
                        //FormsAuthentication.SetAuthCookie(model.UserEmail, true);
                        Session["tokan"] = resp.Msg;
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

                        if (Convert.ToInt32(Session["RoleId"]) != 21 && Convert.ToInt32(Session["RoleId"]) != 31) //For Dealers 
                        {
                            Session["cmpname"] = jObj["cmpname"].ToString();
                            Session["UserEmail"] = jObj["name"].ToString();
                            Session["photo"] = jObj["photo"].ToString();
                            Session["UserId"] = jObj["UserId"].ToString();
                            string strkey = jObj["keystr"].ToString();
                            string systemWizarFlag = jObj["IsSetting"].ToString();
                            Session["cmpcode"] = strkey.Split('|')[1].ToString();
                            Session["ClientCompanyId"] = jObj["ClientCompanyId"].ToString();
                            Session["DealerId"] = jObj["compId"].ToString();
                            Session.Timeout = 10;
                            Session["IsSetting"] = jObj["IsSetting"].ToString();
                            Session["FirstEmpId"] = jObj["FirstEmpId"].ToString();
                            Session["EmpId"] = Convert.ToInt32(jObj["EmpId"]);
                            Session["domain"] = "Dealer";
                            return RedirectToAction("Dashboard", "Dealer");

                        }
                        else if (Convert.ToInt32(Session["RoleId"]) == 21 || Convert.ToInt32(Session["RoleId"]) == 31) //For Account manager
                        {
                            string strkey = jObj["keystr"].ToString();
                            Session["UserEmail"] = jObj["name"].ToString();
                            Session["UserId"] = jObj["UserId"].ToString();
                            Session["RoleId"] = Convert.ToInt32(jObj["RoleId"]);
                            Session["photo"] = jObj["photo"].ToString();
                            Session["IsSetting"] = true;
                            Session["EmpId"] = 0;
                            Session["domain"] = "Dealer";
                            Session.Timeout = 10;
                            return RedirectToAction("Managerdashboard", "Dealer");
                        }
                        else
                        {
                            return RedirectToAction("Login", "Dealer");
                        }
                    }
                    else
                    {

                        if (resp.Msg != "")
                        {
                            TempData["meg"] = resp.Msg;
                            TempData["MegSts"] = resp.MsgSts;
                        }
                        else
                        {
                            TempData["meg"] = "something wrong please try again";
                        }
                        return RedirectToAction("Login", "Dealer");
                    }

                }
                else
                {
                    if (resp.Msg != "")
                    {
                        TempData["meg"] = resp.Msg;
                    }
                    else
                    {
                        TempData["meg"] = "something wrong please try again";
                    }
                    return RedirectToAction("Login", "Dealer");
                }
            }
            catch (Exception Ex)
            {

                TempData["meg"] = Ex.InnerException;
                return RedirectToAction("Login", "Dealer");
            }
        }

        public ActionResult NewNumberVerification(NewMobileNumberVerification obj)
        {


            bool Success = false;
            pm = DealerAcco.NewNumberVerification(obj);

            if (pm.MsgSts == "ok" || pm.MsgSts == "Ok")
            {
                Success = true;
            }
            return Json(new { isSuccess = Success, pm }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult OptwithNumberVerification(string mobileNo)
        {

            bool Success = false;
            pm = DealerAcco.OptwithNumberVerification(mobileNo);

            if (pm.MsgSts == "ok" || pm.MsgSts == "Ok")
            {
                Success = true;
            }
            return Json(new { isSuccess = Success, pm }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ActiveDealer()
        {
            string ActivationCode = Request.QueryString["ed"].ToString();
            if (!string.IsNullOrEmpty(ActivationCode))
            {
                resp = DealerAcco.DealerActiveUser(ActivationCode);
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
                    ViewBag.Meg = "Dealer Already Activated.";
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

        public ActionResult AddManager(RegisterAccountmanager obj)
        {
            return View();
        }


        [CustAuthFilter]
        [HttpGet]
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Planpricing()
        {
            DelaerPlan pl = new DelaerPlan();
            Getpara gp = new Getpara();
            int isfirst = 0;
            int cmpid = 0;
            int noofuser = 0;
            var cookieisfpay = Request.Cookies["isfpay"];
            var cookiecmp = Request.Cookies["cmpid"];
            var cookieuser = Request.Cookies["noofuser"];
            if (cookieisfpay != null)
            {
                isfirst = Convert.ToInt32(cookieisfpay.Value);
            }
            if (cookiecmp != null)
            {
                cmpid = Convert.ToInt32(cookiecmp.Value);
            }
            if (cookieuser != null)
            {
                noofuser = Convert.ToInt32(cookieuser.Value);
            }

            try
            {
                gp.pkid = 0;
                gp.iscount = 0;
                gp.para = isfirst;
                if (Session["DealerId"] != null)
                {
                    gp.pkid = Convert.ToInt32(Session["DealerId"]);
                }

                ViewBag.Paidcmpid = cmpid;
                ViewBag.cmpuser = noofuser;

                pl.PlanList = DealerClient.PlanGetAll(gp);
                return View(pl);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "Dealer");
            }
        }

        public string action1 = string.Empty;
        public string hash1 = string.Empty;
        public string txnid1 = string.Empty;

        [HttpPost]
        public ActionResult Planpricing(DelaerPlan objplan)
        {
            Paymenttrans objpay = new Paymenttrans();
            var planlist = Request.Form["lstcmpaddonpack"];
            List<companyAddon> m = JsonConvert.DeserializeObject<List<companyAddon>>(planlist);

          
            
            //planlist = planlist.Replace("[", "").Replace("{", "").Replace("\"", "").Replace("}", "|").Replace("]", "").Replace("|,", "|");

            ReqUpdateAdminInfo cmpobj = new ReqUpdateAdminInfo();
            //objplan.PlanList = JsonConvert.DeserializeObject<IEnumerable<DelaerPlan>>(Request.Form["udf3"]);
            DataTable dtamount = new DataTable();
            string strForm = string.Empty;
            try
            {
                //dtamount = DealerClient.Getplantotalamount(objplan.DPlanId);

                int Dealeruserid = Convert.ToInt32(Session["UserId"]);
                cmpobj.admininfolist = DealerClient.GetDealerInfoById(Dealeruserid);
                objpay.TotalAmount = objplan.TotalAmount;
                objpay.CGST = objplan.CGST;
                objpay.SGST = objplan.SGST;
                objpay.Companyid = objplan.Companyid;
                objpay.DisccountAmt = objplan.Disccount;
                objpay.DisccountPer = objplan.DisccountPer;
                objpay.Dplanid = objplan.PlanId;
                objpay.Duration = objplan.Duration;
                objpay.Noofuser = objplan.Quantity;
                objpay.InvoiceData = objplan.Taxofpyament;
                objpay.IGST = 0.00;
                //if (dtamount != null && dtamount.Rows.Count > 0)
                //{
                //    objpay.TotalAmount = Convert.ToDouble(dtamount.Rows[0]["Totalamount"]);
                //    objpay.CGST = Convert.ToDouble(dtamount.Rows[0]["amt"].ToString().Split(',')[0].Split(':')[1].ToString());
                //    objpay.SGST = Convert.ToDouble(dtamount.Rows[0]["amt"].ToString().Split(',')[1].Split(':')[1].ToString());
                //    objpay.IGST = Convert.ToDouble(dtamount.Rows[0]["amt"].ToString().Split(',')[2].Split(':')[1].ToString());
                //}

                objpay.Amount = objplan.Price;
                objpay.DealerId = Convert.ToInt32(Session["DealerId"]);
                string paymentID = "";
                string _tId = DateTime.Now.ToString("yyyyMMddHHmmssFF");
                _tId = _tId.PadRight(16, '0');

                
                paymentID = SaveOnlinePayment(objpay);


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
                        lstcomAddon.Add(new companyAddon{
                      
                       AddonPackgeid= item.AddonPackgeid,
                        Addonid = item.Addonid,
                        PaymentId = paymentID,
                        Packagestartdate = startDate,
                        Packagetotalamt = item.Packagetotalamt,
                        Packageunitrate = item.Packageunitrate,
                        Unitquantity = item.Unitquantity,
                        Createby = Convert.ToInt32(Session["userid"]),
                        Companyid = objplan.Companyid,
                        Packageenddate = Convert.ToDateTime(endDate)
                       
                        });
                    
                    }


                 resp = Savecompanyaddon(lstcomAddon);

                }
                string username = Session["cmpname"].ToString();
                string email = Session["UserEmail"].ToString();
                string ContactNo = cmpobj.admininfolist.Select(x => x.MobileNo).FirstOrDefault();


                if (!string.IsNullOrEmpty(paymentID))
                {
                    strForm = Pay(_tId, paymentID, objplan.PlanId.ToString(), objpay.TotalAmount.ToString(), username, email, ContactNo);
                }
            
            }
            catch (Exception ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("PaymentExceptionLog", ex.Message);
                return RedirectToAction("ErrorPage", "PayTime");
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
        }


        public Resp Savecompanyaddon(List<companyAddon> objAddon)
        {

            Resp resp = new Resp();

            resp = DealerClient.Savecompanyaddon(objAddon);

            if (resp.Msg == "ok")
            {
                return resp;
            }

            return resp;

        }

        private string Pay(string tId, string paymentID, string Qty, string amnt, string UserName, string Email, string ContactNo)
        {
            string strForm = string.Empty;

            string MERCHANT_KEY = string.Empty;
            string SALT = string.Empty;
            string PAYU_BASE_URL = string.Empty;
            string action = string.Empty;
            string hashSequence = string.Empty;
            string RET_URL = string.Empty;
            try
            {
                int cmpid = Convert.ToInt32(Session["CompanyId"]);
                DataTable dt = DealerAcco.Fillgatewaypara(cmpid);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
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
                            RET_URL = dt.Rows[i]["Paravalue"].ToString().Replace("PayTime", "Dealer");
                        }
                    }

                }
                else
                {
                    GetDeviceDetails.ProcessLogLogFileWrite("PaymentLog", "Gatwway paramer not found");
                }

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
                                hash_string = hash_string + Qty;
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
                    data.Add("udf2", Qty);
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
                //obj.Companyid = Convert.ToInt32((Session["CompanyId"] != null) ? Session["CompanyId"] : 0);
                //obj.Companycode = Session["cmpcode"].ToString();
                res = DealerClient.InitPayment(obj);
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

        public ActionResult PaymentStatus()
        {
            Paymentres payres = new Paymentres();
            ResMsg res = new ResMsg();
            payres.PaymentID = Request.Form["udf1"];
            payres.txnid = Request.Form["txnid"];
            payres.Uniqhash = Request.Form["hash"];
            payres.Paymentstatus = Request.Form["status"];
            payres.ResDate = DateTime.Now;

            string Message = Request.Form["udf1"] + "|" + Request.Form["txnid"] + "|" + Request.Form["hash"] + "|" + Request.Form["status"];
            GetDeviceDetails.ProcessLogLogFileWrite("PaymentResponseLog", Message);

            if (Request.Form["status"] == "success")
            {
                payres.IsActive = "1";
                TempData["Msgstatus"] = "1";
                TempData["Msg"] = "<div class='successpay'><p><h1> <i class='fa fa-check-circle' style='color:green'></i> Thank you,</h1></p><p class='psuccess'>Your payment was successfully.</p><p>Amount : " + Request.Form["amount"] + "</p> <p>TransactionID : " + Request.Form["txnid"].ToString() + "</p> <p> Payment RefNo :" + Request.Form["udf1"] + "</p><p> Status : <b color='green'>" + Request.Form["status"] + "<b></p></div>";
                //var planlist = Request.Form["udf3"];
                //if (!string.IsNullOrEmpty(planlist))
                //{
                //    planlist = planlist.Remove(planlist.Length - 1, 1);
                //    var planlistarr = planlist.Split('|');
                //    List<partnerlicence> lstp = new List<partnerlicence>();
                //    partnerlicence pls = new partnerlicence();
                //    for (int i = 0; i < planlistarr.Length; i++)
                //    {
                //        partnerlicence pl = new partnerlicence();
                //        pl.partnerid = Convert.ToInt32(Session["DealerId"]);
                //        pl.Paymentid = Request.Form["udf1"];
                //        pl.planid = Convert.ToInt32(planlistarr[i].Split(',')[0].Split(':')[1]);
                //        pl.Quantity = Convert.ToInt32(planlistarr[i].Split(',')[2].Split(':')[1]);
                //        lstp.Add(pl);
                //    }
                //    pls.lstplicence = lstp;
                //    DealerClient.Savepartnerlicencedetails(pls);
                //}







            }
            else
            {
                payres.IsActive = "0";
                TempData["Msgstatus"] = "0";
                TempData["Msg"] = "<div class='failpay'><p><h1><i class='fa fa-times-circle' style='color:#721c24'></i> Oops!,</h1></p><p class='pfaild'>Your payment was Faild.</p><p>Amount : " + Request.Form["amount"] + "</p> <p>TransactionID : " + Request.Form["txnid"].ToString() + "</p> <p> Payment RefNo :" + Request.Form["udf1"] + "</p><p> Status : <b color='red'>" + Request.Form["status"] + "<b></p></div>";
            }




            res = DealerClient.Savepaymentres(payres);

            //res = payclint.Savepaymentres(payres);

            return RedirectToAction("Dashboard");
        }


        [HttpPost]
        public ActionResult RegistrationOtpVarification(RegistrationOtpVerification obj)
        {
            bool Success = false;
            pm = DealerAcco.RegistrationOtpVarification(obj);

            if (pm.MsgSts == "ok" || pm.MsgSts == "Ok")
            {
                Success = true;
            }
            return Json(new { isSuccess = Success, pm }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ResetDealerPassword(DealerOtpVerification model)
        {

            try
            {
                pm = DealerAcco.ResetPasswordDealer(model);
                if (pm.MsgSts == "ok")
                {
                    TempData["meg"] = pm.Msg;
                    TempData["MegSts"] = pm.MsgSts;

                }
                else
                {
                    TempData["meg"] = pm.Msg;
                    TempData["MegSts"] = pm.MsgSts;
                }
            }
            catch (Exception ex)
            {
                TempData["meg"] = ex.InnerException;
                return RedirectToAction("Login", "Dealer");
            }
            return View("ForgotPassword");
        }


        public ActionResult Managerdashboard()
        {
            return View();
        }



        [CustAuthFilter]
        public ActionResult EditProfile()
        {
            return View();
        }
        [CustAuthFilter]
        [HttpPost]
        public ActionResult EditProfile(ReqUpdateAdminInfo ad, HttpPostedFileBase EmpPhotoFile)
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
                    var path = Path.Combine(Server.MapPath("~/DealerLogo"), newfilename);
                    EmpPhotoFile.SaveAs(path);
                    empphoto = newfilename;
                    ad.Photo = empphoto;
                    Session["photo"] = ad.Photo;
                }
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(ad);

                var i = DealerAcco.UpdateDealerUser(ad);
                if (i == true)
                {
                    ViewBag.msg = "Profile updated successfully.";
                }
                else
                {
                    ViewBag.error = "Error in profile update";
                }
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Dealer");
            }
        }

        [HttpGet]
        [CustAuthFilter]
        public JsonResult GetDealerInfoById(int id)
        {
            JsonResult result;
            try
            {
                ReqUpdateAdminInfo admin = new ReqUpdateAdminInfo();
                admin.admininfolist = DealerAcco.GetDealerInfoById(id);
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
        public ActionResult AdminChangePassword(string Password, string NewPassword, string UserEmail, string UserId)
        {
            try
            {
                string concatstring = "";
                //logic for encrypt password
                string EncryptionKey = ConfigurationManager.AppSettings.Get("jwtKey");
                byte[] clearBytes = Encoding.Unicode.GetBytes(UserId + "$" + UserEmail + "$" + Password + "$" + NewPassword);
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
                    pm = DealerAcco.DealerResetPassword(concatstring);
                    if (pm.MsgSts.ToLower() == "ok")
                    {
                        TempData["msg"] = pm.Msg;
                        TempData.Keep("msg");
                    }
                    else
                    {
                        TempData["error"] = pm.Msg;
                        TempData.Keep("error");
                        //  ViewBag.error = resp.Meg;
                        TempData["flag"] = "1";
                        TempData.Keep("flag");
                        //  ViewBag.flag = "1";
                    }
                    return RedirectToAction("EditProfile", "Dealer");
                    //return View();
                }
                return RedirectToAction("EditProfile", "Dealer");
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Dealer");
            }
        }
        public ActionResult Logoff()
        {
            Session.RemoveAll();
            Session.Abandon();
            Session["tokan"] = null;
            return RedirectToAction("Login", "Dealer", false);
        }


        public ActionResult Mytransactions()
        {
            return View();
        }



        public ActionResult TaxMaster()
        {

            return View();

        }

        public ActionResult AddOn()
        {


            return View();
        }
        public ActionResult FinancialSetting() {


            return View();
        }




    }
}