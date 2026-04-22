using Newtonsoft.Json;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Converters;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class ConferenceRoomController : Controller
    {
        #region Declaration
        static readonly MasterDataRestClient RestClient = new MasterDataRestClient();
        #endregion
        // GET: RoomBook
        #region MasterPages
        [HttpGet]
        public ActionResult Amenity()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Room()
        {
            return View();
        }
        #endregion

        [HttpGet]
        public ActionResult Booking()
        {
            return View();
        }


    }
}