using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayTimeWebClient.Infrastructure;
using System.IO;

namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class ESSOKRController : Controller
    {
        [HttpGet]
        public ActionResult MyOKR()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadOKRFilesEmployee(IEnumerable<HttpPostedFileBase> Uploadempokrdoc)
        {
            foreach (HttpPostedFileBase file in Uploadempokrdoc)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/UploadOKREmpDOC"), fileName);
                    file.SaveAs(path);
                }
            }
            return RedirectToAction("MyOKR");
        }
	}
}