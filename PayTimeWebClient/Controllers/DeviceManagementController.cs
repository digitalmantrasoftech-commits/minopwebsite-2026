using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayTimeWebClient.Controllers
{
	[CustAuthFilter]
	public class DeviceManagementController : Controller
	{
		#region Declaration
		static readonly MasterDataRestClient RestClient = new MasterDataRestClient();
		#endregion
		public ActionResult Devices()
		{
			ViewBag.CompanyList =  FillCompany();
			return View();
		}
        public ActionResult DeviceCommand()
        {
            return View();
        }
		public List<SelectListItem> FillCompany()
		{
			Companys cm = new Companys();
			cm.Companyslist = RestClient.CompanyGetAll();
			List<SelectListItem> list = new List<SelectListItem>();
		
			foreach (var i in cm.Companyslist)
			{
				list.Add(new SelectListItem() { Text = i.CompanyName, Value = Convert.ToString(i.CompanyID) });
			}
			return list;
		}
	}
}