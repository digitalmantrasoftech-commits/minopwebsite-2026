using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayTimeWebClient.Controllers
{
    /// <summary>
    /// Controller for handling force majeure situations.
    /// </summary>
    public class ErrorController : Controller
    {
        /// <summary>
        /// Unrecoverable error action handler.
        /// </summary>
        /// <returns></returns>
        public ActionResult Fatal(string uniqueErrorCode)
        {
            var hasErrorCode = uniqueErrorCode != null;

            ViewBag.HasErrorCode = hasErrorCode;
            if (hasErrorCode)
                ViewBag.UniqueErrorCode = uniqueErrorCode;

            return View();
        }

        /// <summary>
        /// Unauthorized error action handler.
        /// </summary>
        /// <returns></returns>
        public ActionResult Unauthorized(string uniqueErrorCode)
        {
            //This can be result of a programming error sending user
            //to a page their role does not allow, or it can be result
            //of a user manually typing in a URL they are not entitled to.

            //TODO: am p3. Check for referring URL. If exists, log so that programmers could research.
            var hasErrorCode = uniqueErrorCode != null;

            ViewBag.HasErrorCode = hasErrorCode;
            if (hasErrorCode)
                ViewBag.UniqueErrorCode = uniqueErrorCode;
            return View();
        }
    }
}
