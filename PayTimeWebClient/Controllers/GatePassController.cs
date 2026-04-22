using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class GatePassController : Controller
    {

        #region Declaration
        static readonly GatePassRequestExpiryDateRestClient RestClient = new GatePassRequestExpiryDateRestClient();     
        #endregion

        [HttpGet]
        public ActionResult Policy()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GatepassApproval()
        {
            var _Compid = "";
            // Get the current date
            DateTime currentDate = DateTime.Now;
            DateTime previousDate = currentDate.AddDays(-1);
            // Calculate the difference in days
            TimeSpan difference = currentDate.Date - previousDate.Date;
            if (difference.TotalDays == 1)
            {
               
            }
            var LL = RestClient.GatePassRequestexpiryStatusUpdate(_Compid.ToString());
            return View();
        }
        [HttpGet]
        public ActionResult IssuePass()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GatePassRequest()
        {
            var _Compid = "0";

            // Get the current date
            DateTime currentDate = DateTime.Now;
            DateTime previousDate = currentDate.AddDays(-1);
            // Calculate the difference in days
            TimeSpan difference = currentDate.Date - previousDate.Date;
            if (difference.TotalDays == 1)
            {
                
            }
            if (!string.IsNullOrEmpty(Session["EmpId"].ToString()))
            {
                _Compid = Session["EmpId"].ToString();
            }
            var LL = RestClient.GatePassRequestexpiryStatusUpdate(_Compid.ToString());
            return View();
        }
    }
}