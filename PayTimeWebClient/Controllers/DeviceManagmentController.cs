using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PayTimeWebClient.Controllers
{
    public class DeviceManagmentController : Controller
    {
        #region Declaration
        static readonly MasterDataRestClient RestClient = new MasterDataRestClient();
        #endregion

        public ActionResult DeviceManage()
        {
            return View();
        }

        [HttpGet]
        public JsonResult DownloadDataFromDevice(string id)
        {
            JsonResult result;
            int DeviceID = Convert.ToInt32(id);
            var str = RestClient.DeviceDownloadData(DeviceID);
            //var jsonSerialiser = new JavaScriptSerializer();
            //var json = jsonSerialiser.Serialize(str);
            result = Json(str);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }


    }
}