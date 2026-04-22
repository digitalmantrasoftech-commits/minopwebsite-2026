using PayTimeWebClient.Helper;
using PayTimeWebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Razorpay.Api;
using System.Net;
using System.Text;
using System.Configuration;
namespace PayTimeWebClient.Controllers
{
    public class DevicesController : Controller
    {
        
        // GET: /Devices/
        static readonly MasterDataRestClient RestClient = new MasterDataRestClient();
        Paymentintegrationclient payclint = new Paymentintegrationclient();
        string RAZOR_KEY = ConfigurationManager.AppSettings["rzp_key"]; //"rzp_test_BA3EbIZsj3BLxG";
        string RAZOR_SECRET = ConfigurationManager.AppSettings["rzp_secret"];//"gbLWGSxPO0wyACZp0mF69VoQ";

        string returnurl = ConfigurationManager.AppSettings["rzp_returnurl"];

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult FaceAttendanceMachinembioFM01()
        {
            return View();
        }
        public ActionResult FaceBiometricMachineBionicF5()
        {
            return View();
        }
        public ActionResult FaceReaderAttendanceMachineBionicF7()
        {
            return View();
        }
        public ActionResult FaceRecognitionTimeAttendanceMachinebioNICFX9()
        {
            return View();
        }

        public ActionResult Cart()
        {
            LoginModel model = new LoginModel();
            model.IsCart = 1;
            ViewBag.meg = TempData["meg"];
            ViewBag.receiptno = TempData["receipt_no"];
            return View();
        }

        [HttpPost]
        public ActionResult Cart(productcartreq obj)
        {
           

            return View();
        }

        public ActionResult AddressInformation()
        {

            //AddressInformation obj = new AddressInformation();

            //if (string.IsNullOrEmpty(obj.receiptno))
            //{

            //    if (TempData["receipt_no"] != null)
            //    {
            //        ViewBag.receiptno = TempData["receipt_no"].ToString();
            //        obj.receiptno = TempData["receipt_no"].ToString();
            //    }
            //    else
            //    {
            //        //return RedirectToAction("cart", "Devices");
            //    }
            //}
            //else
            //{
            //    ViewBag.receiptno = obj.receiptno;
            //}

            return View();
        }

        public ActionResult EditCart(string receipt_no)
        {
            TempData["receipt_no"] = receipt_no;
            return RedirectToAction("cart", "Devices");
        }

        [HttpPost]
        public ActionResult AddressInformation(AddressInformation obj)
        {

            try
            {
                string strForm = string.Empty;

                try
                {
                    if (!string.IsNullOrEmpty(obj.receiptno))
                    {

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        productcartreq objcart = js.Deserialize<productcartreq>(obj.product);

                        if (RestClient.AddCartInfo(objcart))
                        {
                            if (RestClient.AddressInformation(obj))
                            {


                                string rztotalamt = "0";
                                string rzusername = "";
                                string rzemail = "";
                                string rzContactNo = "";
                                string rzorderid = "";
                                string rzcustid = "";
                                string rzpaymentID = "";
                                bool rzisrec = false;


                                string username = Session["cmpname"].ToString();
                                string email = Session["UserEmail"].ToString();
                                string ContactNo = obj.mobile_number_bd;


                                rzusername = username;
                                rzContactNo = ContactNo;
                                rzemail = email;
                                Paymenttrans objpay = new Paymenttrans();

                                string _tId = DateTime.Now.ToString("yyyyMMddHHmmssFF");
                                _tId = _tId.PadRight(16, '0');
                                var isrec = false;
                                objpay.IsRedeemcrAmt = 0;
                                var paymentmethod = objcart.Paymentmethod;
                                int companyid = Convert.ToInt32(Session["CompanyId"]);
                                if (obj.Duration != "-NA-")
                                {
                                    #region payment Transaction New customer

                                    var _duration = 0;
                                    if (obj.Duration == "Monthly")
                                    {
                                        //isrec = true;
                                        isrec = false;
                                        _duration = 30;
                                        //objpay.IsRedeemcrAmt = 1;
                                        objpay.IsRedeemcrAmt = 0;
                                    }
                                    if (obj.Duration == "Yearly")
                                    {
                                        _duration = 365;
                                        objpay.IsRedeemcrAmt = 0;
                                    }

                                    rzContactNo = ContactNo;

                                    var custid = Session["rzpcustid"].ToString();

                                    if (custid == "" && isrec)
                                    {
                                        custid = CreateCustomer(username, ContactNo, email);
                                    }

                                    objpay.Amount = Math.Round(Convert.ToDouble(obj.camt));
                                    objpay.TotalAmount = Math.Round(Convert.ToDouble(obj.Totalamt));

                                    var orderid = CreateOrder(objpay.TotalAmount.ToString(), _tId, isrec);
                                    //var orderid = CreateOrder(objpay.TotalAmount.ToString(), _tId, isrec, objcart.Paymentmethod, custid);
                                    


                                    objpay.CGST = 0;
                                    objpay.SGST = 0;
                                    objpay.Companyid = companyid;
                                    objpay.DisccountAmt = 0;
                                    objpay.DisccountPer = 0;
                                    objpay.Dplanid = Convert.ToInt32(obj.PlanId);
                                    objpay.Duration = _duration;
                                    objpay.Noofuser = Convert.ToInt32(obj.Quantity);
                                    objpay.InvoiceData = obj.Taxofpyament;
                                    objpay.IGST = 0.00;
                                    objpay.IsApproved = 0;
                                    objpay.InvoiceNo = obj.gstn_bd;
                                    objpay.RejectReason = orderid;
                                    objpay.CredittAmt = Math.Round(Convert.ToDouble(obj.Totalamt));
                                    objpay.ReceptNo = obj.receiptno;


                                    rztotalamt = obj.Totalamt;
                                    rzorderid = orderid;
                                    rzcustid = custid;
                                    rzpaymentID = SaveOnlinePayment(objpay);
                                    
                                    #endregion
                                }
                                else
                                {
                                    #region payment Transaction Existing customer

                                    rzContactNo = ContactNo;
                                    rztotalamt = obj.Totalamt;

                                    objpay.Amount = Math.Round(Convert.ToDouble(obj.camt));

                                    objpay.TotalAmount = Math.Round(Convert.ToDouble(obj.Totalamt));

                                    var orderid = CreateOrder(objpay.TotalAmount.ToString(), _tId, isrec);
                                    //var orderid = CreateOrder(objpay.TotalAmount.ToString(), _tId, isrec,"", "");

                                    objpay.CGST = 0;
                                    objpay.SGST = 0;
                                    objpay.Companyid = companyid;
                                    objpay.DisccountAmt = 0;
                                    objpay.DisccountPer = 0;
                                    objpay.Dplanid = 0;
                                    objpay.Duration = 0;
                                    objpay.Noofuser = 0;
                                    objpay.InvoiceData = obj.Taxofpyament;
                                    objpay.IGST = 0.00;
                                    objpay.IsApproved = 0;
                                    objpay.InvoiceNo = obj.gstn_bd;
                                    objpay.RejectReason = orderid;
                                    objpay.CredittAmt = Math.Round(Convert.ToDouble(obj.Totalamt));
                                    objpay.ReceptNo = obj.receiptno;

                                    rztotalamt = obj.Totalamt;
                                    rzorderid = orderid;
                                    rzpaymentID = SaveOnlinePayment(objpay);

                                    #endregion
                                }

                                rzisrec = isrec;

                                System.Collections.Hashtable data = new System.Collections.Hashtable();
                                data.Add("amount", rztotalamt + "00");
                                data.Add("usename", rzusername);
                                data.Add("email", rzemail);
                                data.Add("contact", rzContactNo);
                                data.Add("order_id", rzorderid);
                                data.Add("customer_id", rzcustid);
                                data.Add("desc", rzpaymentID);
                                strForm = POSTForm(data, rzisrec);

                            }
                        }



                    }


                }
                catch (Exception ex)
                {
                    if (ex.Source == "Razorpay")
                    {
                        TempData["rerrormsg"] = ex.Message;
                        return RedirectToAction("AddressInformation");
                    }
                    else
                    {
                        return RedirectToAction("AddressInformation");
                    }
                }

                if (!string.IsNullOrEmpty(strForm))
                {
                    return Content(strForm, System.Net.Mime.MediaTypeNames.Text.Html);
                }
                else
                {
                    TempData["receipt_no"] = obj.receiptno;
                    return RedirectToAction("AddressInformation");
                }
            }
            catch (Exception ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("PaymentExceptionLog", ex.Message);
                if (ex.Source == "Razorpay")
                {
                    TempData["receipt_no"] = obj.receiptno;
                    TempData["erromsg"] = ex.Message;
                }
                return RedirectToAction("AddressInformation");

            }

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
        //public string CreateOrder(string ammount, string Receptno, bool isReccuring, string method,string custmid)        
        //{
        //    #region Razorpay

        //    //string RAZOR_KEY = "rzp_test_BA3EbIZsj3BLxG";
        //    //string RAZOR_SECRET = "gbLWGSxPO0wyACZp0mF69VoQ";
        //    //JsonResult result;
        //    //string recept_no = Receptno;
        //    RazorpayClient client = new RazorpayClient(RAZOR_KEY, RAZOR_SECRET);
        //    ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
        //    Dictionary<string, object> options = new Dictionary<string, object>();
        //    Dictionary<string, object> tooptions = new Dictionary<string, object>();
        //    method = "netbanking";
        //    if (isReccuring)
        //    {
        //         if (method == "upi")
        //        {
                
        //            options.Add("method", method);
        //            options.Add("customer_id", custmid);
                    
        //        }
        //        else
        //        {
        //            tooptions.Add("auth_type", method);                    
        //            options.Add("method", "emandate");
        //            options.Add("customer_id", custmid);
        //            options.Add("token", tooptions);                    
        //        }
                
        //    }
            
        //        options.Add("amount", ammount + "00");
        //        options.Add("currency", "INR");
        //        options.Add("receipt", Receptno);
            
        //    Razorpay.Api.Order _order = client.Order.Create(options);

        //    var x = _order.Attributes["id"];
        //    //result = Json(JsonConvert.SerializeObject(x));
        //    //string orderid = result.Data[0]["id"]
        //    return x;
        //    #endregion
        //}
        public string CreateOrder(string ammount, string Receptno, bool isReccuring)
        {
            #region Razorpay

            //string RAZOR_KEY = "rzp_test_BA3EbIZsj3BLxG";
            //string RAZOR_SECRET = "gbLWGSxPO0wyACZp0mF69VoQ";
            //JsonResult result;
            //string recept_no = Receptno;
            RazorpayClient client = new RazorpayClient(RAZOR_KEY, RAZOR_SECRET);
            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", ammount + "00");
            options.Add("currency", "INR");
            options.Add("receipt", Receptno);
            Razorpay.Api.Order _order = client.Order.Create(options);

            var x = _order.Attributes["id"];
            //result = Json(JsonConvert.SerializeObject(x));
            //string orderid = result.Data[0]["id"]
            return x;
            #endregion
        }
        public string CreateCustomer(string name, string contactno, string emailid)
        {
            #region Razorpay
            RazorpayClient client = new RazorpayClient(RAZOR_KEY, RAZOR_SECRET);
            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("name", name);
            options.Add("email", emailid);
            options.Add("contact", contactno);
            options.Add("fail_existing", "0");
            Razorpay.Api.Customer _customer = client.Customer.Create(options);
            var x = _customer.Attributes["id"];
            return x;
            #endregion
        }

        private string POSTForm(System.Collections.Hashtable data, bool isrecuring)
        {

            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script src='https://checkout.razorpay.com/v1/checkout.js'></script>");
            if (isrecuring)
            {
                strScript.Append("<script language='javascript'>");
                strScript.Append("var options = { 'key': '" + RAZOR_KEY + "' ,");
                strScript.Append("'order_id': '" + data["order_id"] + "',");
                strScript.Append("'customer_id': '" + data["customer_id"] + "',");
                strScript.Append("'recurring': '1',");
                strScript.Append("'currency': 'INR', 'name': 'Minop Cloud','description': '" + data["desc"] + "',");
                strScript.Append("'image': 'https://app.minopcloud.com/AssetsNew/images/logo.png',  'callback_url': '" + returnurl + "',");
                //strScript.Append(" 'name': '" + data["usename"] + "','email': '" + data["email"] + "', 'order_id': '" + data["order_id"] + "','contact': '" + data["contact"] + "'},");
                strScript.Append("'notes': { 'address': 'Mantra Softech (India) Pvt. Ltd' },'theme': {'color': '#3399cc' }};");
                strScript.Append("var rzp1 = new Razorpay(options); rzp1.open(); e.preventDefault();");
                strScript.Append("</script>");
            }
            else
            {
                strScript.Append("<script language='javascript'>");
                strScript.Append("var options = { 'key': '" + RAZOR_KEY + "' ,");
                strScript.Append("'amount': '" + data["amount"] + "',");
                strScript.Append("'order_id': '" + data["order_id"] + "',");
                strScript.Append("'currency': 'INR', 'name': 'Minop Cloud','description': '" + data["desc"] + "',");
                strScript.Append("'image': 'https://app.minopcloud.com/AssetsNew/images/logo.png',  'callback_url': '" + returnurl + "','prefill': {");
                strScript.Append(" 'name': '" + data["usename"] + "','email': '" + data["email"] + "','contact': '" + data["contact"] + "'},");
                strScript.Append("'notes': { 'address': 'Mantra Softech (India) Pvt. Ltd' },'theme': {'color': '#3399cc' }};");
                strScript.Append("var rzp1 = new Razorpay(options); rzp1.open(); e.preventDefault();");
                strScript.Append("</script>");
            }

            //Return the form and the script concatenated.
            //(The order is important, Form then JavaScript)
            return strScript.ToString();
        }


        public ActionResult PaymentStatus()
        {
            Paymentres payres = new Paymentres();
            ResMsg res = new ResMsg();


            var _PaymentID = Request.Form["razorpay_payment_id"];
            var _orderID = Request.Form["razorpay_order_id"];
            var _signature = Request.Form["razorpay_signature"];
            var _orglogo = Request.Form["org_logo"];
            var _orgname = Request.Form["org_name"];
            var _chkoutlogo = Request.Form["checkout_logo"];
            var _custombra = Request.Form["custom_branding"];




            RazorpayClient client = new RazorpayClient(RAZOR_KEY, RAZOR_SECRET);
            Payment payment = client.Payment.Fetch(_PaymentID);


            payres.PaymentID = payment.Attributes["description"];
            payres.Paymentstatus = "success";
            payres.txnid = _orderID;



            payres.ResDate = DateTime.Now;
            payres.Uniqhash = _signature;
            payres.customerId = payment.Attributes["customer_id"];

            string Message = payment.Attributes["description"] + "|" + _orderID + "|" + _signature + "|" + _orglogo + "|" + _orgname + "|" + _chkoutlogo + "|" + _custombra;
            GetDeviceDetails.ProcessLogLogFileWrite("PaymentResponseLog", Message);

            if (payres.Paymentstatus == "success")
            {
                payres.IsActive = "1";
            }
            else
            {
                payres.IsActive = "0";
            }

            res = payclint.Savepaymentresnew(payres);

            if (res == null)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("PaymentResponsesavefailLog", " Exeption on payment response save with new method Req: " + Message);
            }
            else
            {
                GetDeviceDetails.ProcessLogLogFileWrite("PaymentResponsesavefailLog", res.Msg + " Req: " + Message);
            }


            if (payres.Paymentstatus == "success")
            {
                payres.IsActive = "1";
                TempData["Msgstatus"] = "1";
                var payamount = payment.Attributes["amount"] != null || payment.Attributes["amount"] != "" ? Convert.ToInt32(payment.Attributes["amount"]) / 100 : 0;
                //TempData["Msg"] = "<div class='successpay'><p><h1> <i class='fa fa-check-circle' style='color:green'></i> Thank you,</h1></p><p class='psuccess'>Your payment was successfully.</p><p>Amount : " + Request.Form["amount"] + "</p> <p>TransactionID : " + Request.Form["txnid"].ToString() + "</p> <p> Payment RefNo :" + Request.Form["udf1"] + "</p><p> Status : <b color='green'>" + Request.Form["status"] + "<b></p><p><b> Note :</b> An invoice will provide on your email within seven business days.</p></div>";
                //TempData["Msg"] = "<div class='successpay'><p><h1> <i class='fa fa-check-circle' style='color:green'></i> Thank you,</h1></p><p class='psuccess'>Your payment was successfully.</p><p>Amount : " + Request.Form["amount"] + "</p> <p>TransactionID : " + Request.Form["txnid"].ToString() + "</p> <p> Payment RefNo :" + Request.Form["udf1"] + "</p><p> Status : <b color='green'>" + Request.Form["status"] + "<b></p><p><b> Note :</b> For Invoice Generation kinldy process,</br>E- mail on servico@mantratec.com with below details,</br>Company Name, Contact Person Name, Contact No,</br>Company GST No, Company PAN No, Billing Address</p></div>";
                TempData["Msg"] = "<div class='successpay'><p><h1> <i class='fa fa-check-circle' style='color:green'></i> Thank you,</h1></p><p class='psuccess'>Your payment was successfully.</p><p>Amount : " + payamount + "</p> <p>TransactionID : " + payment.Attributes["id"] + "</p> <p> Payment RefNo :" + payment.Attributes["description"] + "</p><p> Status : <b color='green'>" + payres.Paymentstatus + "<b></p></div>";
            }
            else
            {
                payres.IsActive = "0";
                TempData["Msgstatus"] = "0";
                TempData["Msg"] = "<div class='failpay'><p><h1><i class='fa fa-times-circle' style='color:#721c24'></i> Oops!,</h1></p><p class='pfaild'>Your payment was Failed.</p><p>Amount : " + Request.Form["amount"] + "</p> <p>TransactionID : " + Request.Form["txnid"].ToString() + "</p> <p> Payment RefNo :" + Request.Form["udf1"] + "</p><p> Status : <b color='red'>" + Request.Form["status"] + "<b></p></div>";
            }


            return RedirectToAction("Mysubscription");


        }
    }
}