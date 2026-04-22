using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayTimeWebClient.Infrastructure;

namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class OKRController : Controller
    {
        #region OKRGenerate
        public ActionResult OKRGenerate()
        {
            return View();
        }
        #endregion 
       

        #region OKRAssign
        public ActionResult OKRAssign()
        {
            return View();
        }
        #endregion 


        #region OKRReview
        public ActionResult OKRReview()
        {
            return View();
        }
        #endregion 
    }
}