using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Models;
using System.Web.Script.Serialization;
using System.Data;
using PayTimeWebClient.Infrastructure;
using System.IO;
using System.Collections;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Preview;
using DevExpress.Web.Mvc;
using System.Drawing;

namespace PayTimeWebClient.Controllers
{
    public class ExtraReportController : Controller
    {
        #region Declaration
        static readonly MasterDataRestClient RestClient = new MasterDataRestClient();
        static readonly UtilitiesDataRestClient UtilitiesRestClient = new UtilitiesDataRestClient();
        static readonly TransactionDataRestClient TransactionRestClient = new TransactionDataRestClient();
        #endregion
        public ActionResult DemoReport()
        {
            //  XtraReport3 report3 = new XtraReport3();
            // var i = RestClient.DesignationsGetAll();
            // ArrayList lst = new ArrayList();
            // foreach (var j in i)
            // {
            //     lst.Add(j);
            // }
            // report3.DataSource = lst;
            // //return WebDocumentViewerExtension.GetJsonModelScript(report,);
            //return View(report3);
            return View();
        }

        public ActionResult ImgDemo()
        {
            ImageDemo ID = new ImageDemo();
            var i = RestClient.EmployeeGetAll();
            ArrayList lst = new ArrayList();
            var k = 1;
            foreach (var j in i)
            {
              
                //if (j.EmpPhoto == "")
                //{
                //    j.EmpPhoto = "avtar.png";
                //}
                if (j.EmpPhoto != "")
                {
                    j.EmpPhoto = "~/UploadEmpPhoto/" + j.EmpPhoto;
                }
                else
                {
                    j.EmpPhoto = "~/UploadEmpPhoto/avtar.png";
                }
                lst.Add(j);
                k = k + 1;
            }
           ID.DataSource = lst;
            //return WebDocumentViewerExtension.GetJsonModelScript(report,);
            return View(ID);
          
        }

        public ActionResult EmployeeReport()
        {
            DesignDemo report3 = new DesignDemo();
            //XRPictureBox xrp = new XRPictureBox();
            var i = RestClient.EmployeeGetAll();
            ArrayList lst = new ArrayList();
            var k = 1;
            foreach (var j in i)
            {
                j.EmpId = k;
                j.EmpDOB = Convert.ToDateTime(j.EmpDOB).ToShortDateString();
                if(j.EmpPhoto == "")
                {
                    j.EmpPhoto = "avtar.png";
                }
                //if (j.EmpPhoto != "")
                //{
                //    j.EmpPhoto = "~/UploadEmpPhoto/" + j.EmpPhoto;
                //}
                //else
                //{
                //    j.EmpPhoto = "~/UploadEmpPhoto/avtar.png";
                //}
                lst.Add(j);
                k = k + 1;
            }
            report3.DataSource = lst;
            //return WebDocumentViewerExtension.GetJsonModelScript(report,);
            return View(report3);

        }
    }
}