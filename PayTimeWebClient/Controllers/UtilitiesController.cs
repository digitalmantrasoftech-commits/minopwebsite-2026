using Newtonsoft.Json;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Net;
using DevExpress.Xpo;
using System.Threading.Tasks;

namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class UtilitiesController : Controller
    {
        #region Declaration
        static readonly MasterDataRestClient MasterRestClient = new MasterDataRestClient();
        static readonly UtilitiesDataRestClient UtilitiesRestClient = new UtilitiesDataRestClient();

        #endregion

        public ActionResult AttendanceParameter()
        {
            return View();
        }
        public ActionResult AttendanceRules()
        {
            try
            {
                ViewBag.CompanyList = FillCompany();
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        [HttpPost]
        public string AttendanceRulesAdd(string AttdRules)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            AttendanceRule objattdRule = js.Deserialize<AttendanceRule>(AttdRules);
            string ad = "";
            ViewBag.msg = "";
            ViewBag.error = "";
            AttendanceRule obj = objattdRule;
            ad = UtilitiesRestClient.AttendanceRuleAdd(obj);
            return ad;
        }

        [HttpGet]
        public JsonResult GetAttendanceRuleIDByEmployee(int EmpId)
        {
            JsonResult result;

            var data = UtilitiesRestClient.GetAttendanceRuleIDbyEmpID(EmpId);

            if (data == null)
            {
                AttendanceRule ar = new AttendanceRule();
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(ar.attndRules);
                result = Json(json);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            else
            {
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(data);
                result = Json(json);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }

        }

        public List<SelectListItem> FillCompany()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            Companys com = new Companys();
            IEnumerable<Companys> comList;
            if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) == 1)
            {
                comList = MasterRestClient.CompanyGetAll();
            }
            else if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806" && Convert.ToInt32(Session["RoleId"].ToString()) > 1)
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                comList = MasterRestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
            }
            else
            {
                var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                comList = MasterRestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
            }
            foreach (var i in comList)
            {
                list.Add(new SelectListItem() { Text = i.CompanyName, Value = Convert.ToString(i.CompanyID) });
            }
            return list;
        }
        [HttpPost]
        public JsonResult FillBranch(int id)
        {
            Branches b = new Branches();
            List<Branches> bList = new List<Branches>();
            if (Session["RoleId"].ToString() == "6806")
            {
                var brachid = Convert.ToInt32(Session["BranchId"].ToString());
                bList = MasterRestClient.BranchGetAll().Where(x => x.CompanyID == id && x.BranchId == brachid).ToList();
            }
            else
            {
                bList = MasterRestClient.BranchGetAll().Where(x => x.CompanyID == id).ToList();
            }
            return Json(bList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ImportEmployeeExcel(Importfaillog faillog)
        {
            try
            {
                ViewBag.CompanyList = FillCompany();
                //As per plan subscription  no of employee restriction
                int overctn = Convert.ToInt32(Session["IsUserlimit"]);
                if (overctn == 1)
                {
                    ViewBag.flgempover = "true";
                    ViewBag.error = "You are rich maximum users limit of this plan, Please update your plan to add more employees.";
                }
                else
                {
                    ViewBag.flgempover = "false";
                    ViewBag.error = "";
                }


                return View(faillog);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        //private bool Checksubscription(int emptoimport ,out int overctn)
        //{
        //    int mcmpid = Convert.ToInt32(Session["CompanyId"].ToString());
        //    var dtsubscrip = MasterRestClient.Getmysubscription(mcmpid,0);
        //    int Noofemp = 0;
        //    if (dtsubscrip != null && dtsubscrip.Rows.Count > 0)
        //    {
        //        Noofemp = Convert.ToInt32(dtsubscrip.Rows[0]["NoOfEmployee"]);
        //    }
        //    var emplctn = MasterRestClient.EmployeeGetAll().Count();
        //    var totalemp = emptoimport + emplctn;

        //    if (totalemp > Noofemp)
        //    {
        //        overctn = totalemp - Noofemp;
        //        return true;
        //    }
        //    else
        //    {
        //        overctn = 0;
        //        return false;
        //    }
        //}
        //[HttpPost]
        //public ActionResult ImportEmployeeExcel(HttpPostedFileBase FileUpload, int ddlBranch, int ddlCompany)
        //{
        //    try
        //    {
        //        string filePath = string.Empty;
        //        DataTable dt = new DataTable();
        //        Utility utl = new Utility();
        //        Importfaillog faillog = new Importfaillog();
        //        if (Request.Files["FileUpload"].ContentLength > 0)
        //        {
        //            string extension = System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName).ToLower();
        //            string query = null;
        //            string connString = "";
        //            string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
        //            string path1 = string.Format("{0}/{1}", Server.MapPath("~/Uploads"), Request.Files["FileUpload"].FileName);
        //            if (!Directory.Exists(path1))
        //            {
        //                Directory.CreateDirectory(Server.MapPath("~/Uploads"));
        //            }
        //            if (validFileTypes.Contains(extension))
        //            {
        //                if (System.IO.File.Exists(path1))
        //                {
        //                    System.IO.File.Delete(path1);
        //                }
        //                Request.Files["FileUpload"].SaveAs(path1);
        //                if (extension == ".csv")
        //                {
        //                    dt = utl.ConvertCSVtoDataTable(path1);
        //                    ViewBag.Data = dt;
        //                }
        //                //Connection String to Excel Workbook
        //                else if (extension.Trim() == ".xls")
        //                {
        //                    connString = "Provider=Microsoft.Ace.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
        //                    //connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
        //                    dt = utl.ConvertXSLXtoDataTable(path1, connString);
        //                    ViewBag.Data = dt;
        //                }
        //                else if (extension.Trim() == ".xlsx")
        //                {
        //                    connString = "Provider=Microsoft.Ace.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
        //                    //connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
        //                    dt = utl.ConvertXSLXtoDataTable(path1, connString);
        //                    ViewBag.Data = dt;
        //                }
        //                int dtcnt = 0;
        //                int overctn = 0;
        //                int nouser = Convert.ToInt32(Session["UseCount"]);
        //                int noemp = Convert.ToInt32(Session["Noofemp"]);
        //                dtcnt = dt.Rows.Count;
        //                int totuser = noemp + dtcnt;
        //                overctn = totuser - nouser;

        //                var isrecurrng = Convert.ToInt32(Session["isRecurring"]);
        //                if (totuser > nouser && isrecurrng != 1)
        //                {
        //                    ViewBag.flgempover = "false";
        //                    ViewBag.error = "You are importing more then maximum users limit of this plan, Please update your plan to import more employees or remove " + overctn + " recoreds from sheet.";
        //                }
        //                else
        //                {
        //                    ViewBag.flgempover = "false";
        //                    ViewBag.error = "";

        //                    //MemoryStream str = new MemoryStream();
        //                    //dt.WriteXml(str, true);
        //                    //str.Seek(0, SeekOrigin.Begin);
        //                    //StreamReader sr = new StreamReader(str);
        //                    //string xmlstr;
        //                    //xmlstr = sr.ReadToEnd();
        //                    //fill cmpid and branchid from drop down
        //                    int cmpid = ddlCompany;
        //                    int branchid = ddlBranch;
        //                    List<ImportedDataEmp> items = new List<ImportedDataEmp>();
        //                    if (dt.Rows.Count > 0)
        //                    {
        //                        try
        //                        {
        //                            items = (from DataRow row in dt.Rows

        //                                     select new ImportedDataEmp
        //                                     {
        //                                         Empcode = row["EmpCode"].ToString(),
        //                                         EmpName = row["EmpName"].ToString(),
        //                                         EmpPunchID = row["EmpPunchID"].ToString(),
        //                                         EmpMarried = row["EmpMarried"].ToString(),
        //                                         EmpJoinDate = row["EmpJoinDate"].ToString(),
        //                                         EmpBirthDate = row["EmpBirthDate"].ToString(),
        //                                         EmpDepartment = row["EmpDepartment"].ToString(),
        //                                         EmpDesignation = row["EmpDesignation"].ToString(),
        //                                         EmpShift = row["EmpShift"].ToString(),
        //                                         EmpAddress = row["EmpAddress"].ToString(),
        //                                         EmpPhone = row["EmpPhone"].ToString(),
        //                                         EmpMobile = row["EmpMobile"].ToString(),
        //                                         EmpEmail = row["EmpEmail"].ToString(),
        //                                         Rolename = row["Rolename"].ToString(),
        //                                         Gender = row["Gender"].ToString(),
        //                                         PolicyName = row["PolicyName"].ToString(),
        //                                         EmpShiftGroup = row["EmpShiftGroup"].ToString(),
        //                                         CountryCode = row["CountryCode"].ToString(),
        //                                         BranchName = row["BranchName"].ToString(),

        //                                     }).ToList();

        //                            int planId = Convert.ToInt32(Session["planId"]);

        //                            faillog.Importfailloglist = UtilitiesRestClient.ImportEmployees(items, branchid, cmpid, planId);
        //                            if (faillog.Importfailloglist.ToList().Count > 0)
        //                            {
        //                                TempData["IsValid"] = false;
        //                            }
        //                            else
        //                            {
        //                                TempData["IsValid"] = true;
        //                                ViewBag.msg = "File imported Successfully.";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {
        //                            ViewBag.error = "Please import proper format file.";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        ViewBag.error = "Please import proper format file.";
        //                    }
        //                    //JavaScriptSerializer oJS = new JavaScriptSerializer();
        //                    //ErrorMsg em = new ErrorMsg();
        //                    //em = oJS.Deserialize<ErrorMsg>(i);
        //                    //if (em.MegSts == "error")
        //                    //{
        //                    //    ViewBag.error = em.Meg;
        //                    //}
        //                    //else
        //                    //{
        //                    //    ViewBag.msg = em.Meg;
        //                    //}
        //                    FileInfo file = new FileInfo(path1);
        //                    if (file.Exists)
        //                    {
        //                        file.Delete();
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                ViewBag.error = "Please Upload Files in .xls, .xlsx or .csv format";

        //            }

        //        }
        //        ViewBag.CompanyList = FillCompany();
        //        return View(faillog);
        //    }
        //    catch (Exception)
        //    {
        //        return RedirectToAction("ErrorPage", "PayTime");
        //    }
        //}

        [HttpGet]
        public ActionResult DeviceManagment()
        {
            return View();
        }

        [HttpPost]
        public FileResult DeviceManagment(string str)
        {
            return File(Server.MapPath("~/Downloads/DeviceServiceSetup.msi"), "application/x-msi", "DeviceServiceSetup.msi");
        }

        #region UpdateAttendance
        public ActionResult ProcessMaster()
        {
            try
            {
                ViewBag.CompanyList = FillCompany();
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [HttpPost]
        public JsonResult GetAttnSheet(string fromdt, string todt, int cid, int bid)
        {

            JsonResult result;
            try
            {
                var AllData = UtilitiesRestClient.GetAttnSheet(fromdt, todt, cid, bid);
                return Json(AllData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result = Json(ex.ToString());
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }

        }
        #endregion

        #region Restore Client DB
        [HttpGet]
        public ActionResult RestoreDatabase()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RestoreDatabase(HttpPostedFileBase FileUpload)
        {
            try
            {
                string filePath = string.Empty;
                if (Request.Files["FileUpload"].ContentLength > 0)
                {
                    string extension = System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName).ToLower();
                    string[] validFileTypes = { ".bak" };
                    string directoryPath = ConfigurationManager.AppSettings["Dbloaction"].ToString();
                    string path1 = string.Format("{0}/{1}", directoryPath, Request.Files["FileUpload"].FileName);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    if (validFileTypes.Contains(extension))
                    {
                        if (System.IO.File.Exists(path1))
                        {
                            ViewBag.error = "File with same name exists, please change filename and upload again.";
                        }
                        else
                        {
                            Request.Files["FileUpload"].SaveAs(path1);
                            ViewBag.msg = "File uploaded sucessfully.";
                            ViewBag.isfileuploaded = "true";
                            ViewBag.FileName = Request.Files["FileUpload"].FileName.ToString();
                        }
                    }
                    else
                    {
                        ViewBag.error = "Please upload files in .BAK format.";
                    }

                }
                return View();
            }
            catch (Exception)
            {
                ViewBag.error = "Error in uploading file.";
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        public JsonResult CheckFile(string FileName)
        {
            JsonResult result;
            string directoryPath = ConfigurationManager.AppSettings["Dbloaction"].ToString();
            string path1 = string.Format("{0}/{1}", directoryPath, Path.GetFileName(FileName));
            if (System.IO.File.Exists(path1))
            {
                //ViewBag.error = "File with same name exists, please change filename and upload again.";
                result = Json(new { isExist = true, msg = "File with same name exists, please change filename and upload again." });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            else
            {
                result = Json(new { isExist = false, msg = "" });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        public JsonResult RestoreDatabaseProcess(string FileName)
        {
            string companyName = Session["cmpcode"].ToString();
            FileName = FileName.Substring(FileName.LastIndexOf(':') + 1);
            string directoryPath = ConfigurationManager.AppSettings["Dbloaction"].ToString();
            string path1 = string.Format("{0}/{1}", directoryPath, Path.GetFileName(FileName));
            var response = UtilitiesRestClient.RestoreDatabase(path1, companyName);
            if (System.IO.File.Exists(path1))
            {
                System.IO.File.Delete(path1);
            }
            JsonResult result;
            result = Json(response);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        public JsonResult ImportMasterData()
        {
            string companyName = Session["cmpcode"].ToString();
            var response = UtilitiesRestClient.ImportMasterData(companyName);
            JsonResult result;
            result = Json(response);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        public JsonResult ImportTransData()
        {
            string companyName = Session["cmpcode"].ToString();
            var response = UtilitiesRestClient.ImportTransData(companyName);
            JsonResult result;
            result = Json(response);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        public JsonResult MigrateData()
        {
            string companyName = Session["cmpcode"].ToString();
            var response = UtilitiesRestClient.MigrateData(companyName);
            JsonResult result;
            result = Json(response);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        #endregion

        #region Valid Employee
        public ActionResult ValidEmployee()
        {
            ViewBag.validuser = UtilitiesRestClient.CheckUser(0);
            ViewBag.Invaliduser = UtilitiesRestClient.CheckUser(1);
            ViewBag.InvalidPunch = UtilitiesRestClient.CheckUser(2);
            return View();
        }
        #endregion

        #region Device Service
        public ActionResult DeviceService()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            string AccountCode = Session["cmpcode"].ToString();
            ViewBag.DeviceServicelist = UtilitiesRestClient.GetDeviceServiceDetails(AccountCode);
            return View();
        }
        [HttpPost]
        public JsonResult Whitelistipaddress(string accountcode, int deviceserviceid, string ipaddress, string systemcode, bool iswhitelist)
        {
            JsonResult result;
            DeviceService ds = new DeviceService();
            ds.AccountCode = accountcode;
            ds.Deviceserid = deviceserviceid;
            ds.IpAddress = ipaddress;
            ds.SystemCode = systemcode;
            if (iswhitelist)
            {
                iswhitelist = false;
            }
            else
            {
                iswhitelist = true;
            }
            ds.Iswhitelist = iswhitelist;
            var data = UtilitiesRestClient.Whitelistipaddress(ds);
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(data);
            result = Json(json);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        public ActionResult DeviceCommand()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            ViewBag.DeviceCommandlist = UtilitiesRestClient.GetDevicecommand();
            return View();
        }

        [HttpPost]
        public JsonResult InsertDevicecommand(int deviceid, string DeviceCode, string Commandid, string Commandpara, string FromDate, string ToDate)
        {
            Commandpara = Commandpara.TrimEnd(',');
            JsonResult result;
            Devicecommand dc = new Devicecommand();
            dc.Deviceid = deviceid;
            dc.DeviceCode = DeviceCode;
            dc.Commandid = Commandid;
            dc.Commanddate = DateTime.Now.ToString();
            dc.Commandpara = Commandpara;
            dc.BranchId = 0;
            dc.BranchName = "";
            dc.FromDate = FromDate;
            dc.ToDate = ToDate;
            var jsonSerialiser = new JavaScriptSerializer();
            var data = UtilitiesRestClient.InsertDevicecommand(dc);
            var json = jsonSerialiser.Serialize(data);
            result = Json(json);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        #endregion

        #region Custom Field
        public ActionResult CustomField()
        {
            return View();
        }
        #endregion


        #region Custom Forms
        [HttpGet]
        public ActionResult CustomformBuilder()
        {
            ViewBag.Mastermenulist = UtilitiesRestClient.GetAllmastermenu();
            ViewBag.AddMasterfiledmenulist = UtilitiesRestClient.GetAddMasterfiledmenulist();
            TempData["frmid"] = 0;
            return View();
        }
        [HttpPost]
        public ActionResult CustomformBuilder(Editcustomform frmobj)
        {
            ViewBag.Mastermenulist = UtilitiesRestClient.GetAllmastermenu();
            ViewBag.AddMasterfiledmenulist = UtilitiesRestClient.GetAddMasterfiledmenulist();
            TempData["frmid"] = frmobj.frmid;
            TempData["frmdata"] = frmobj.frmdata;
            TempData["frmoperationparentid"] = frmobj.frmoperationparentid;
            TempData["frmdisplayname"] = frmobj.frmdisplayname;
            TempData["frmtblname"] = frmobj.tblname;
            return View();
        }
        [HttpGet]
        public ActionResult Customforms()
        {
            ViewBag.lstcustomform = UtilitiesRestClient.GetAllcustomforms();


            return View();
        }
        [HttpGet]
        public ActionResult ViewCustomforms()
        {

            if (Session["actname"] != null)
            {
                TempData["frmname"] = Session["actname"].ToString();
            }

            IEnumerable<Cfc> lstobj = UtilitiesRestClient.GetAllcustomforms();

            ViewBag.lstcustomform = JsonConvert.SerializeObject(lstobj);
            DataTable model = UtilitiesRestClient.Getcustomformdata(Session["actname"].ToString());
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveCustomforms(FormCollection formCollection)
        {

            try
            {

                Dictionary<string, string> dictionary = new Dictionary<string, string>();

                foreach (var key in formCollection.AllKeys)
                {
                    string colvalue = "";
                    string colname = "";
                    var _upload = key.IndexOf("hdnuploadfile"); // file upload logic 
                    if (key == "menurender")
                    {
                        TempData["frmname"] = formCollection[key];
                        colname = "'" + key + "'";
                        colvalue = "'" + formCollection[key] + "'";
                    }
                    else if (key == "selecttblname")
                    {
                        colname = "'" + key + "'";
                        colvalue = "`" + formCollection[key] + "`";
                    }
                    else
                    {
                        decimal filesize;

                        if (_upload != -1)
                        {
                            string path = Server.MapPath("~/FileuploadControlData/" + Session["cmpcode"] + "/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            var fileName = formCollection[key];
                            fileName = fileName.Replace("undefined", "");

                            if (!string.IsNullOrEmpty(fileName))
                            {
                                var file = Request.Files.Keys[0];
                                var Path1 = string.Format("{0}/{1}", Server.MapPath("~/FileuploadControlData/" + Session["cmpcode"]), fileName);
                                Request.Files["" + file + ""].SaveAs(Path1);
                                colname = "`" + file + "`";
                                colvalue = "'" + fileName + "'";
                            }
                        }
                        else
                        {
                            if (key.IndexOf("checkbox-group") != -1)
                            {
                                colname = "`" + key.Replace("[]", "") + "`";
                                colvalue = "'" + Regex.Replace(formCollection[key], "<.*?>", String.Empty) + "'";
                            }
                            else
                            {
                                if (key.IndexOf("number") == -1)
                                {
                                    colname = "`" + key + "`";
                                    colvalue = "'" + Regex.Replace(formCollection[key], "<.*?>", String.Empty) + "'";
                                }
                                else
                                {
                                    colname = "`" + key + "`";
                                    if (string.IsNullOrEmpty(formCollection[key]))
                                    {
                                        colvalue = "0";
                                    }
                                    else
                                    {
                                        colvalue = "'" + Regex.Replace(formCollection[key], "<.*?>", String.Empty) + "'";
                                    }
                                }
                            }
                        }
                    }

                    if (colname != "")
                    {
                        dictionary.Add(colname, colvalue);
                    }

                    var keyb = dictionary.Where(pair => pair.Key.Contains("button"))
                            .Select(pair => pair.Key)
                            .FirstOrDefault();

                    if (keyb != null)
                    {
                        dictionary.Remove(keyb);
                    }
                }
                MRespo mres = UtilitiesRestClient.Savecustomformdata(dictionary);
                if (mres.MegSts == "ok")
                {
                    TempData["smsg"] = mres.Meg;
                }
                else
                {
                    TempData["emsg"] = mres.Meg;
                }
                //IEnumerable<Cfc> lstobj = UtilitiesRestClient.GetAllcustomforms();
                //ViewBag.lstcustomform = JsonConvert.SerializeObject(lstobj);
                //}
            }
            catch (Exception ex)
            {
                var ss = ex.Message;
            }
            return RedirectToAction("ViewCustomforms");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Viewform()
        {
            string actname = Request.Form["selmnu"].ToString();
            TempData["frmname"] = actname.Trim();
            Session["actname"] = actname.Trim();
            return RedirectToAction("ViewCustomforms");
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Callautocomple(Autocomplreq objreq)
            {

            JsonResult res = new JsonResult();
            var varfrom = objreq.searchfrom;
            List<Genriclist> objgen = new List<Genriclist>();

            if (varfrom == "Company")
            {
                if (objreq.tblname == "All")
                {
                    var lstmodel = MasterRestClient.CompanyGetAll().Select(x => new Genriclist
                    {
                        label = x.CompanyName,
                        value = x.CompanyName
                    }).ToList();
                    res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var lstmodel = MasterRestClient.CompanyGetAll().Where(x => x.CompanyName.ToLower().Contains(objreq.searchval.ToLower())).Select(x => new Genriclist
                    {
                        label = x.CompanyName,
                        value = x.CompanyName
                    }).ToList();
                    res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

            }
            if (varfrom == "Branch")
            {
                if (objreq.tblname == "All")
                {
                    var lstmodel = MasterRestClient.BranchGetAll().Select(x => new Genriclist
                    {
                        label = x.BranchName,
                        value = x.BranchName

                    }).ToList();
                    res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var lstmodel = MasterRestClient.BranchGetAll().Where(x => x.BranchName.ToLower().Contains(objreq.searchval.ToLower())).Select(x => new Genriclist
                    {
                        label = x.BranchName,
                        value = x.BranchName

                    }).ToList();
                    res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

            }
            if (varfrom == "BU")
            {
                if (objreq.tblname == "All")
                {
                    var lstmodel = MasterRestClient.BUGetAll().Select(x => new Genriclists
                    {
                        label = x.BUName,
                        value = x.BUId

                    }).ToList();
                    res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var lstmodel = MasterRestClient.BUGetAll().Where(x => x.BUName.ToLower().Contains(objreq.searchval.ToLower())).Select(x => new Genriclists
                    {
                        label = x.BUName,
                        value = x.BUId

                    }).ToList();
                    res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

            }
            if (varfrom == "SBU")
            {
                if (!string.IsNullOrEmpty(objreq.selectedBU))
                {
                    if (objreq.tblname == "All")
                    {
                        var lstmodel = MasterRestClient.SBUGetAll(objreq.selectedBU).Select(x => new Genriclists
                        {
                            label = x.SubBUName,
                            value = x.SubBUId

                        }).ToList();
                        res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                    else
                    {
                        var lstmodel = MasterRestClient.SBUGetAll(objreq.selectedBU).Where(x => x.SubBUName.ToLower().Contains(objreq.selectedBU.ToLower())).Select(x => new Genriclists
                        {
                            label = x.SubBUName,
                            value = x.SubBUId

                        }).ToList();
                        res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                }

            }
            if (varfrom == "Policy")
            {
                if (objreq.tblname == "All")
                {
                    var lstmodel = MasterRestClient.HRPolicyGetAll().Select(x => new Genriclist
                    {

                        label = x.PolicyName,
                        value = x.PolicyName

                    }).ToList();
                    res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var lstmodel = MasterRestClient.HRPolicyGetAll().Where(x => x.PolicyName.ToLower().Contains(objreq.searchval.ToLower())).Select(x => new Genriclist
                    {

                        label = x.PolicyName,
                        value = x.PolicyName

                    }).ToList();
                    res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }

            }
            if (varfrom == "Department")
            {
                if (objreq.tblname == "All")
                {
                    var lstmodel = MasterRestClient.DepartmentGetAll().Select(x => new Genriclist
                    {

                        label = x.DepartmentName,
                        value = x.DepartmentName

                    }).ToList();
                    res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var lstmodel = MasterRestClient.DepartmentGetAll().Where(x => x.DepartmentName.ToLower().Contains(objreq.searchval.ToLower())).Select(x => new Genriclist
                    {

                        label = x.DepartmentName,
                        value = x.DepartmentName

                    }).ToList();
                    res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }

            }
            if (varfrom == "Designation")
            {
                if (objreq.tblname == "All")
                {
                    var lstmodel = MasterRestClient.DesignationsGetAll().Select(x => new Genriclist
                    {

                        label = x.DesignationName,
                        value = x.DesignationName

                    }).ToList();
                    res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var lstmodel = MasterRestClient.DesignationsGetAll().Where(x => x.DesignationName.ToLower().Contains(objreq.searchval.ToLower())).Select(x => new Genriclist
                    {

                        label = x.DesignationName,
                        value = x.DesignationName

                    }).ToList();
                    res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }
            }
            if (varfrom == "Employee")
            {
                if (objreq.tblname == "All")
                {
                    var lstmodel = MasterRestClient.EmployeeGetAll().Select(x => new Genriclist
                    {

                        label = x.EmpName,
                        value = x.EmpName

                    }).ToList();
                    res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var lstmodel = MasterRestClient.EmployeeGetAll().Where(x => x.EmpName.ToLower().Contains(objreq.searchval.ToLower())).Select(x => new Genriclist
                    {

                        label = x.EmpName,
                        value = x.EmpName

                    }).ToList();
                    res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }
            }
            if (varfrom == "Device")
            {
                if (objreq.tblname == "All")
                {
                    var lstmodel = MasterRestClient.DeviceGetAll().Select(x => new Genriclist
                    {

                        label = x.DeviceName,
                        value = x.DeviceName

                    }).ToList();
                    res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var lstmodel = MasterRestClient.DeviceGetAll().Where(x => x.DeviceName.ToLower().Contains(objreq.searchval.ToLower())).Select(x => new Genriclist
                    {

                        label = x.DeviceName,
                        value = x.DeviceName

                    }).ToList();
                    res = new JsonResult { Data = lstmodel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }
            }
            return res;
        }

        #endregion

        #region EnrollUser
        public ActionResult EnrollUser()
        {
            //var compcode = Session["cmpcode"].ToString();

            //var devicelist = MasterRestClient.DeviceGetAllGrid(compcode);
            //List<SelectListItem> list = new List<SelectListItem>();
            //foreach (var i in devicelist)
            //{
            //    list.Add(new SelectListItem() { Text = i.DeviceName, Value = Convert.ToString(i.DeviceId) });
            //}
            //ViewBag.DeviceList = list;
            //ViewBag.DeviceList = MasterRestClient.DeviceGetAllGrid(compcode);
            return View();
        }
        [HttpPost]
        public ActionResult EnrollUser(EnrollDataSave ad, HttpPostedFileBase EmpPhotoFile)
        {
            try
            {
                string empphoto = "";
                //ad.ModifyBy = ad.UserId;
                //ad.ModifyDate = DateTime.Now.ToString();
                if (EmpPhotoFile != null)
                {
                    string newfilename = "";
                    var filename = Path.GetFileName(EmpPhotoFile.FileName);
                    string formatedTemplate = "{0}_{1}.{2}";
                    var fileNameWithoutExtension = filename.Split('.')[0];
                    var fileExtension = filename.Split('.')[1];
                    var MaxId = Guid.NewGuid().ToString().Substring(0, 6);
                    newfilename = String.Format(formatedTemplate, fileNameWithoutExtension, MaxId, fileExtension);
                    var path = Path.Combine(Server.MapPath("~/EnrollUserProfile"), newfilename);
                    EmpPhotoFile.SaveAs(path);
                    //empphoto = newfilename;
                    Image img = Image.FromFile(path);

                    //ImageConverter Class convert Image object to Byte array.
                    byte[] bytes = (byte[])(new ImageConverter()).ConvertTo(img, typeof(byte[]));
                    ad.Photo = bytes;
                    ad.Photopath = path;
                }
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(ad);

                var i = MasterRestClient.SaveEnrollDeviceUsers(ad);
                ViewBag.msg = "added successfully.";

                //return RedirectToAction("AdminEditProfile", "PayTime");
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        #endregion

        #region EmployeeJoiningMaster
        [HttpGet]
        public ActionResult EmployeeJoiningMaster()
        {
            return View();
        }

        #endregion

        #region AttendanceDetails
        [HttpGet]
        public ActionResult AttendanceDetails()
        {
            ViewBag.LeaveTypeList = FillLeaveType();
            return View();
        }
        public List<SelectListItem> FillLeaveType()
        {
            LeaveTypeMaster lv = new LeaveTypeMaster();
            IEnumerable<LeaveTypeMaster> lvList;
            lvList = MasterRestClient.LeaveTypeGetAll();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var i in lvList)
            {
                list.Add(new SelectListItem() { Text = i.LeaveTypeName, Value = Convert.ToString(i.LeaveTypeId) });
            }
            return list;
        }
        [HttpPost]
        public JsonResult AttendanceDetails(string ID)
        {
            JsonResult result;
            int roleid = Convert.ToInt32(Session["RoleId"]);
            int empid = Convert.ToInt32(Session["EmpId"]);
            if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
            {
                int cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
            }

            //DataTable DataCount = UtilitiesRestClient.AttendanceDetails(empid);

            DataTable ad = UtilitiesRestClient.AttendanceDetails(empid);
            result = Json(ad);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        [HttpPost]
        public JsonResult GetAttendanceDetails(string RoleID, string loginemployeeid, string Month, string Year, string status, string Employee , int selectAll=0 ,string selectAllSearchTerm="")
        {
            JsonResult result;
            string res = string.Empty;
            var data = "";
            AttanList obj = new AttanList();
            obj.RoleID = RoleID;
            if (RoleID == "6805")
            {
                obj.loginemployeeid = Session["ClientCompanyId"].ToString();
            }
            else if (RoleID == "6806")
            {
                if(Convert.ToInt32(Session["EmpId"]) != 0)
                {
                    int empids = Convert.ToInt32(Session["EmpId"]);
                    var branches = MasterRestClient.BranchlistGet(Session["ClientCompanyId"].ToString(), empids)?.ToList();
                    if (branches != null && branches.Any())
                    {
                        obj.loginemployeeid = string.Join(",", branches.Select(b => b.BranchId));
                    }
                    else
                    {
                        obj.loginemployeeid = string.Empty;
                    }
                }
                else
                {
                    obj.loginemployeeid = Session["BranchId"].ToString();
                }
            }
            else
            {
                obj.loginemployeeid = loginemployeeid;
            }
            obj.Month = Month;
            obj.Year = Year;
            obj.status = status;
            obj.Employee = Employee;
            obj.SelectAll = selectAll;
            obj.SelectAllSearchTerm = selectAllSearchTerm;
            var AttanList = UtilitiesRestClient.GetAttendanceDetails(obj);
            result = Json(AttanList);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.MaxJsonLength = Int32.MaxValue;
            return result;

        }

        [HttpPost]
        public JsonResult GetAttendencesummary(string RoleID, string loginemployeeid, string Month, string Year, string status, string Employee,int SelectAll=0 , string SelectAllSearchTerm = "")
        {
            JsonResult result;
            string res = string.Empty;
            var data = "";
            AttanList obj = new AttanList();
            obj.RoleID = RoleID;
            if (RoleID == "6805")
            {
                obj.loginemployeeid = Session["ClientCompanyId"].ToString();
            }
            else if (RoleID == "6806")
            {
                obj.loginemployeeid = Session["BranchId"].ToString();
            }
            else
            {
                obj.loginemployeeid = loginemployeeid;
            }
            obj.Month = Month;
            obj.Year = Year;
            obj.status = status;
            obj.Employee = Employee;
            obj.SelectAll = SelectAll;
            obj.SelectAllSearchTerm =SelectAllSearchTerm;
            var AttanList = UtilitiesRestClient.GetAttendencesummary(obj);
            result = Json(AttanList);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.MaxJsonLength = Int32.MaxValue;
            return result;

        }
        #endregion


        #region for Import Class Master module wise
        [HttpPost]
        public ActionResult ImportEmployeeExcel(HttpPostedFileBase FileUpload, int ddlBranch, int ddlCompany, int ddlClassMaster, string compcode)
        {
            try
            {
                string errorMessage;
                DataTable dt = ProcessUploadedFile(FileUpload, out errorMessage);
                ImportResponse Response = new ImportResponse();


                if (!string.IsNullOrEmpty(errorMessage))
                {
                    ViewBag.error = errorMessage;
                    return View(new Importfaillog());
                }

                // Common import result handling
                bool importSuccess = false;
                Importfaillog faillog = new Importfaillog();

                // ddlClassMaster=1 for company
                if (ddlClassMaster == 1)
                {
                    if (dt.Rows.Count > 5000)
                    {
                        ViewBag.error = "The Excel file exceeds the maximum allowed row limit of 5000. Please reduce the number of rows.";
                        return View(faillog);
                    }
                    Response = DataTableConverter.ConvertToImportedDataCompList(dt, this.HttpContext);

                    if (Response.status && Response.statusCode == 101)
                    {
                        if (Response.companyList != null && Response.companyList.Count > 0)
                        {
                            try
                            {
                                faillog.ImportCompfailloglist = UtilitiesRestClient.ImportCompanys(Response.companyList, compcode);
                                importSuccess = !faillog.ImportCompfailloglist.Any();
                            }
                            catch (Exception Ex)
                            {
                                ViewBag.error = "Error while importing file.";
                                importSuccess = false;
                            }
                        }
                        else if (Response.status == false && Response.statusCode == 103)
                        {
                            ViewBag.error = "Your Excel file contains no data.";
                            importSuccess = false;
                        }
                    }
                    else if (Response.status == false && Response.statusCode == 102)
                    {
                        ViewBag.error = "Please import a file with the proper format.";
                        importSuccess = false;
                    }
                    else if (Response.status == false && Response.statusCode == 103)
                    {
                        ViewBag.error = "Your Excel file contains no data.";
                        importSuccess = false;
                    }
                }
                // ddlClassMaster=2 for branch
                else if (ddlClassMaster == 2)
                {
                    if (dt.Rows.Count > 5000)
                    {
                        ViewBag.error = "The Excel file exceeds the maximum allowed row limit of 5000. Please reduce the number of rows.";
                        return View(faillog);
                    }
                    Response = DataTableConverter.ConvertToImportedDataBranchList(dt);

                    if (Response.status && Response.statusCode == 101)
                    {
                        if (Response.branchList != null && Response.branchList.Count > 0)
                        {
                            try
                            {
                                faillog.ImportBranchfailloglist = UtilitiesRestClient.ImportBranches(Response.branchList, compcode);
                                importSuccess = !faillog.ImportBranchfailloglist.Any();
                            }
                            catch (Exception Ex)
                            {
                                ViewBag.error = "Error while importing file.";
                                importSuccess = false;
                            }
                        }
                        else if (Response.status == false && Response.statusCode == 103)
                        {
                            ViewBag.error = "Your Excel file contains no data.";
                            importSuccess = false;
                        }
                    }
                    else if (Response.status == false && Response.statusCode == 102)
                    {
                        ViewBag.error = "Please import a file with the proper format.";
                        importSuccess = false;
                    }
                    else if (Response.status == false && Response.statusCode == 103)
                    {
                        ViewBag.error = "Your Excel file contains no data.";
                        importSuccess = false;
                    }
                }
                // ddlClassMaster=3 for department
                else if (ddlClassMaster == 3)
                {
                    if (dt.Rows.Count > 5000)
                    {
                        ViewBag.error = "The Excel file exceeds the maximum allowed row limit of 5000. Please reduce the number of rows.";
                        return View(faillog);
                    }
                    Response = DataTableConverter.ConvertToImportedDataDeptList(dt, this.HttpContext);

                    if (Response.status && Response.statusCode == 101)
                    {
                        if (Response.departmentList != null && Response.departmentList.Count > 0)
                        {

                            try
                            {
                                faillog.ImportDeptfailloglist = UtilitiesRestClient.ImportDepartments(Response.departmentList, compcode);
                                importSuccess = !faillog.ImportDeptfailloglist.Any();
                            }
                            catch (Exception Ex)
                            {
                                ViewBag.error = "Error while importing file.";
                                importSuccess = false;
                            }
                        }
                        else if (Response.status == false && Response.statusCode == 103)
                        {
                            ViewBag.error = "Your Excel file contains no data.";
                            importSuccess = false;
                        }
                    }
                    else if (Response.status == false && Response.statusCode == 102)
                    {
                        ViewBag.error = "Please import a file with the proper format.";
                        importSuccess = false;
                    }
                    else if (Response.status == false && Response.statusCode == 103)
                    {
                        ViewBag.error = "Your Excel file contains no data.";
                        importSuccess = false;
                    }
                }
                // ddlClassMaster=4 for designation
                else if (ddlClassMaster == 4)
                {
                    if (dt.Rows.Count > 5000)
                    {
                        ViewBag.error = "The Excel file exceeds the maximum allowed row limit of 5000. Please reduce the number of rows.";
                        return View(faillog);
                    }
                    Response = DataTableConverter.ConvertToImportedDataDesgList(dt);

                    if (Response.status && Response.statusCode == 101)
                    {
                        if (Response.designationList != null && Response.designationList.Count > 0)
                        {
                            try
                            {
                                faillog.ImportDesgfailloglist = UtilitiesRestClient.ImportDesignations(Response.designationList, compcode);
                                importSuccess = !faillog.ImportDesgfailloglist.Any();
                            }
                            catch (Exception Ex)
                            {
                                ViewBag.error = "Error while importing file.";
                                importSuccess = false;
                            }
                        }
                        else if (Response.status == false && Response.statusCode == 103)
                        {
                            ViewBag.error = "Your Excel file contains no data.";
                            importSuccess = false;
                        }
                    }
                    else if (Response.status == false && Response.statusCode == 102)
                    {
                        ViewBag.error = "Please import a file with the proper format.";
                        importSuccess = false;
                    }
                    else if (Response.status == false && Response.statusCode == 103)
                    {
                        ViewBag.error = "Your Excel file contains no data.";
                        importSuccess = false;
                    }
                }
                // ddlClassMaster=4 for Employee
                else if (ddlClassMaster == 5)
                {
                    int dtcnt = dt.Rows.Count;
                    int nouser = Convert.ToInt32(Session["UseCount"]);
                    int noemp = Convert.ToInt32(Session["Noofemp"]);
                    int totuser = noemp + dtcnt;
                    int overctn = totuser - nouser;
                    var isrecurrng = Convert.ToInt32(Session["isRecurring"]);
                    if (dtcnt > 5000)
                    {
                        ViewBag.error = "The Excel file exceeds the maximum allowed row limit of 5000. Please reduce the number of rows.";
                        ViewBag.CompanyList = FillCompany();
                        return View(faillog);
                    }
                    if (totuser > nouser && isrecurrng != 1)
                    {
                        ViewBag.flgempover = "false";
                        ViewBag.error = "You are importing more than the maximum users limit of this plan. Please update your plan to import more employees or remove " + overctn + " records from the sheet.";
                        ViewBag.CompanyList = FillCompany();
                        return View(faillog);
                    }
                    if (!dt.Columns.Contains("AadharCardNumber"))
                    {
                        DataColumn column = new DataColumn("AadharCardNumber", typeof(string))
                        {
                            DefaultValue = string.Empty // Set default value as an empty string
                        };
                        dt.Columns.Add(column);
                    }
                    Response = DataTableConverter.ConvertToImportedDataEmpList(dt);

                    if (Response.status && Response.statusCode == 101)
                    {
                        if (Response.employeeList != null && Response.employeeList.Count > 0)
                        {
                            try
                            {
                                int planId = Convert.ToInt32(Session["planId"]);
                                faillog.Importfailloglist = UtilitiesRestClient.ImportEmployees(Response.employeeList, ddlBranch, ddlCompany, planId, compcode);
                                importSuccess = !faillog.Importfailloglist.Any();
                            }
                            catch (Exception Ex)
                            {
                                ViewBag.error = "Error while importing file.";
                                importSuccess = false;
                            }
                        }
                        else if (Response.status == false && Response.statusCode == 103)
                        {
                            ViewBag.error = "Your Excel file contains no data.";
                            importSuccess = false;
                        }
                    }
                    else if (Response.status == false && Response.statusCode == 102)
                    {
                        ViewBag.error = "Please import a file with the proper format.";
                        importSuccess = false;
                    }
                    else if (Response.status == false && Response.statusCode == 103)
                    {
                        ViewBag.error = "Your Excel file contains no data.";
                        importSuccess = false;
                    }
                }
                // Common result handling
                if (importSuccess)
                {
                    TempData["IsValid"] = true;
                    TempData["ddlClassMaster"] = ddlClassMaster;
                    ViewBag.msg = "File imported successfully.";
                    dt.Clear();
                    dt.Dispose();
                }
                else
                {
                    if (ViewBag.error == "" || ViewBag.error == null)
                    {
                        TempData["IsValid"] = false;
                        TempData["ddlClassMaster"] = ddlClassMaster;
                    }
                }

                ViewBag.CompanyList = FillCompany();
                Response = null;
                return View(faillog);
            }
            catch (Exception Ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        [HttpGet]
        public JsonResult DownloadErrorList(string DtFlag, int classMasterValue)
        {
            try
            {
                // Retrieve the list of import error logs
                var faillogList = UtilitiesRestClient.Importerroremp(DtFlag, classMasterValue);
                // If necessary, convert the list to the appropriate type
                List<Importexcelfaillog> excelfaillogList = faillogList
                .Select(fail => new Importexcelfaillog
                {
                    EmpCode = fail.EmpCode,
                    EmpName = fail.EmpName,
                    EmpPunchID = fail.EmpPunchID,
                    EmpMarried = fail.EmpMarried,
                    EmpJoinDate = fail.EmpJoinDate,
                    EmpBirthDate = fail.EmpBirthDate,
                    EmpDepartment = fail.EmpDepartment,
                    EmpDesignation = fail.EmpDesignation,
                    EmpShift = fail.EmpShift,
                    EmpAddress = fail.EmpAddress,
                    EmpPhone = fail.EmpPhone,
                    EmpMobile = fail.EmpMobile,
                    EmpEmail = fail.EmpEmail,
                    Rolename = fail.Rolename,
                    Gender = fail.Gender,
                    PolicyName = fail.PolicyName,
                    BranchName = fail.BranchName,
                    EmpShiftGroup = fail.EmpShiftGroup,
                    Reason = fail.Reason,
                })
               .ToList();

                JsonResult result = Json(excelfaillogList);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.MaxJsonLength = Int32.MaxValue;

                return result;
            }
            catch (Exception ex)
            {
                ViewBag.error = "Error while importing file.";
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }

        [HttpGet]
        public JsonResult DownloadCompErrList(string DtFlag, int classMasterValue)
        {
            try
            {

                var faillogList = UtilitiesRestClient.Importcomperror(DtFlag, classMasterValue);
                List<ImportCompfaillog> compFaillogList = faillogList
               .Select(compFail => new ImportCompfaillog
               {
                   CompanyName = compFail.CompanyName,
                   Email = compFail.Email,
                   Contact = compFail.Contact,
                   Address = compFail.Address,
                   CompanyWebsite = compFail.CompanyWebsite,
                   Reason = compFail.Reason
               })
                .ToList();
                JsonResult result = Json(compFaillogList);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.MaxJsonLength = Int32.MaxValue;

                return result;

            }
            catch (Exception ex)
            {
                ViewBag.error = "Error while downloading file.";
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }

        [HttpGet]
        public JsonResult DownloadBranchErrList(string DtFlag, int classMasterValue)
        {
            try
            {

                var faillogList = UtilitiesRestClient.ImportBranchError(DtFlag, classMasterValue);


                List<ImportBranchfaillog> branchFaillogList = faillogList
               .Select(BranchFail => new ImportBranchfaillog
               {
                   CompanyName = BranchFail.CompanyName,
                   BranchName = BranchFail.BranchName,
                   BranchHeadEmail = BranchFail.BranchHeadEmail,
                   BranchHeadPassword = BranchFail.BranchHeadPassword,
                   BranchAddress = BranchFail.BranchAddress,
                   ReportingBranchName = BranchFail.ReportingBranchName,
                   Reason = BranchFail.Reason
               })
                .ToList();


                JsonResult result = Json(branchFaillogList);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.MaxJsonLength = Int32.MaxValue;
                return result;
            }
            catch (Exception ex)
            {
                ViewBag.error = "Error while Downloading file.";
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }

        [HttpGet]
        public JsonResult DownloadDepartmentErrList(string DtFlag, int classMasterValue)
        {
            try
            {

                var faillogList = UtilitiesRestClient.ImportDepartmentError(DtFlag, classMasterValue);


                List<ImportDeptfaillog> departmentFaillogList = faillogList
               .Select(DeptFail => new ImportDeptfaillog
               {
                   DepartmentName = DeptFail.DepartmentName,
                   DepartmentHeadName = DeptFail.DepartmentHeadName,
                   DepartmentEmail = DeptFail.DepartmentEmail,
                   Reason = DeptFail.Reason
               })
                .ToList();


                JsonResult result = Json(departmentFaillogList);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.MaxJsonLength = Int32.MaxValue;
                return result;
            }
            catch (Exception ex)
            {
                ViewBag.error = "Error while Downloading file.";
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }


        [HttpGet]
        public JsonResult DownloadDesignationErrList(string DtFlag, int classMasterValue)
        {
            try
            {

                var faillogList = UtilitiesRestClient.ImportDesignationError(DtFlag, classMasterValue);


                List<ImportDesgfaillog> designationFaillogList = faillogList
               .Select(DesgFail => new ImportDesgfaillog
               {
                   DesignationName = DesgFail.DesignationName,
                   Reason = DesgFail.Reason
               })
                .ToList();


                JsonResult result = Json(designationFaillogList);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.MaxJsonLength = Int32.MaxValue;
                return result;
            }
            catch (Exception ex)
            {
                ViewBag.error = "Error while Downloading file.";
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }

        #region selected file to datatable for import Class Master module

        //-- convert selected file to datatable for import Class Master module --//
        private DataTable ProcessUploadedFile(HttpPostedFileBase file, out string errorMessage)
        {
            DataTable dt = new DataTable();
            errorMessage = string.Empty;
            string extension = System.IO.Path.GetExtension(file.FileName).ToLower();
            string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
            string path = string.Format("{0}/{1}", Server.MapPath("~/Uploads"), file.FileName);

            try
            {
                if (!validFileTypes.Contains(extension))
                {
                    errorMessage = "Please Upload Files in .xls, .xlsx or .csv format";
                    return null;
                }

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                file.SaveAs(path);

                Utility utl = new Utility();
                string connString = "";

                if (extension == ".csv")
                {
                    dt = utl.ConvertCSVtoDataTable(path);
                }
                else if (extension == ".xls")
                {
                    connString = "Provider=Microsoft.Ace.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
                    dt = utl.ConvertXSLXtoDataTable(path, connString);
                }
                else if (extension == ".xlsx")
                {
                    connString = "Provider=Microsoft.Ace.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
                    dt = utl.ConvertXSLXtoDataTable(path, connString);
                }

                // Clean up
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                return dt;
            }
            catch (Exception ex)
            {
                errorMessage = "An error occurred while processing the file: " + ex.Message;
                return null;
            }
        }
        #endregion

        #region For Convert DatatableToDataList
        //-- for employee datalist
        public static class DataTableConverter
        {
            private static int _cmdid;
            private static int _branchid;
            private static string _CompanyCode;

            public static void Initialize(HttpContextBase httpContext)
            {
                if (httpContext != null)
                {
                    if (httpContext.Session["CompanyId"] != null)
                    {
                        _cmdid = Convert.ToInt32(httpContext.Session["CompanyId"]);
                    }
                    if (httpContext.Session["BranchId"] != null)
                    {
                        _branchid = Convert.ToInt32(httpContext.Session["BranchId"]);
                    }
                    if (httpContext.Session["cmpcode"].ToString() != null || httpContext.Session["cmpcode"].ToString() != "")
                    {
                        _CompanyCode = httpContext.Session["cmpcode"].ToString();
                    }
                }
            }

            public static ImportResponse ConvertToImportedDataEmpList(DataTable dt)
            {
                //List<ImportedDataEmp> items = new List<ImportedDataEmp>();
                ImportResponse res = new ImportResponse();

                if (dt != null && dt.Rows.Count > 0)
                {
                    try
                    {
                        var filteredRows = dt.Rows.Cast<DataRow>()
                          .Where(row => !row.ItemArray.All(f => f is DBNull));

                        DataTable dt1;

                        if (!filteredRows.Any())
                        {
                            dt1 = dt.Clone();
                            res.status = false;
                            res.statusCode = 103;
                        }
                        else
                        {
                            dt1 = filteredRows.CopyToDataTable();

                            string[] requiredColumns = { "EmpCode", "EmpName", "EmpPunchID", "EmpJoinDate", "BranchName", "EmpShift", "EmpEmail", "RoleName", "PolicyName", "EmpShiftGroup", "AadharCardNumber" };
                            bool allColumnsExist = requiredColumns.All(col => dt1.Columns.Contains(col));

                            if (allColumnsExist)
                            {

                                res.employeeList = (from DataRow row in dt1.Rows
                                                    select new ImportedDataEmp
                                                    {
                                                        Empcode = row["EmpCode"].ToString(),
                                                        EmpName = row["EmpName"].ToString(),
                                                        EmpPunchID = row["EmpPunchID"].ToString(),
                                                        EmpMarried = row["EmpMarried"].ToString(),
                                                        EmpJoinDate = row["EmpJoinDate"].ToString(),
                                                        EmpBirthDate = row["EmpBirthDate"].ToString(),
                                                        EmpDepartment = row["EmpDepartment"].ToString(),
                                                        EmpDesignation = row["EmpDesignation"].ToString(),
                                                        EmpShift = row["EmpShift"].ToString(),
                                                        EmpAddress = row["EmpAddress"].ToString(),
                                                        EmpPhone = row["EmpPhone"].ToString(),
                                                        EmpMobile = row["EmpMobile"].ToString(),
                                                        EmpEmail = row["EmpEmail"].ToString(),
                                                        Rolename = row["Rolename"].ToString(),
                                                        Gender = row["Gender"].ToString(),
                                                        PolicyName = row["PolicyName"].ToString(),
                                                        EmpShiftGroup = row["EmpShiftGroup"].ToString(),
                                                        CountryCode = row["CountryCode"].ToString(),
                                                        BranchName = row["BranchName"].ToString(),
                                                        AdharCard = row["AadharCardNumber"].ToString()
                                                    }).ToList();

                                res.status = true;
                                res.statusCode = 101;
                            }
                            else
                            {
                                res.status = false;
                                res.statusCode = 102;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        res.status = false;
                        res.statusCode = 103;
                    }
                }
                else
                {
                    res.status = false;
                    res.statusCode = 103;
                }
                return res;
            }

            public static ImportResponse ConvertToImportedDataCompList(DataTable dt, HttpContextBase httpContext)
            {
                Initialize(httpContext);
                ImportResponse res = new ImportResponse();
                //List<Companys> items = new List<Companys>();

                if (dt != null && dt.Rows.Count > 0)
                {
                    try
                    {
                        var filteredRows = dt.Rows.Cast<DataRow>()
                          .Where(row => !row.ItemArray.All(f => f is DBNull));

                        DataTable dt1;

                        if (!filteredRows.Any())
                        {
                            dt1 = dt.Clone();
                            res.status = false;
                            res.statusCode = 103;
                        }
                        else
                        {
                            dt1 = filteredRows.CopyToDataTable();

                            string[] requiredColumns = { "CompanyName", "Email", "Contact", "Address" };
                            bool allColumnsExist = requiredColumns.All(col => dt1.Columns.Contains(col));

                            if (allColumnsExist)
                            {
                                res.companyList = (from DataRow row in dt1.Rows
                                                   select new Companys
                                                   {
                                                       CompanyName = row["CompanyName"].ToString(),
                                                       CompanyEmail = row["Email"].ToString(),
                                                       CompanyContact = row["Contact"].ToString(),
                                                       CompanyAddress = row["Address"].ToString(),
                                                       CompanyUrl = row["CompanyWebsite"].ToString(),
                                                       CompanyCode = _CompanyCode,
                                                   }).ToList();

                                res.status = true;
                                res.statusCode = 101;
                            }
                            else
                            {
                                res.status = false;
                                res.statusCode = 102;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        res.status = false;
                        res.statusCode = 103;
                    }
                }
                else
                {
                    res.status = false;
                    res.statusCode = 103;
                }
                return res;
            }

            public static ImportResponse ConvertToImportedDataBranchList(DataTable dt)
            {
                ImportResponse res = new ImportResponse();

                if (dt != null && dt.Rows.Count > 0)
                {
                    try
                    {
                        var filteredRows = dt.Rows.Cast<DataRow>()
                          .Where(row => !row.ItemArray.All(f => f is DBNull));

                        DataTable dt1;

                        if (!filteredRows.Any())
                        {
                            dt1 = dt.Clone();
                            res.status = false;
                            res.statusCode = 103;
                        }
                        else
                        {
                            dt1 = filteredRows.CopyToDataTable();

                            string[] requiredColumns = { "BranchName", "CompanyName", "BranchAddress" };
                            bool allColumnsExist = requiredColumns.All(col => dt1.Columns.Contains(col));

                            if (allColumnsExist)
                            {
                                res.branchList = (from DataRow row in dt1.Rows
                                                  select new Branches
                                                  {
                                                      BranchName = row["BranchName"].ToString(),
                                                      CompanyName = row["CompanyName"].ToString(),
                                                      BranchAddress = row["BranchAddress"].ToString(),
                                                      BranchHeadEmail = row["BranchHeadEmail"].ToString(),
                                                      BranchHeadPassword = row["BranchHeadPassword"].ToString(),
                                                      ReportingBranchName = row["ReportingBranchName"].ToString(),
                                                  }).ToList();

                                res.status = true;
                                res.statusCode = 101;
                            }
                            else
                            {
                                res.status = false;
                                res.statusCode = 102;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        res.status = false;
                        res.statusCode = 103;
                    }
                }
                else
                {
                    res.status = false;
                    res.statusCode = 103;
                }
                return res;
            }

            public static ImportResponse ConvertToImportedDataDeptList(DataTable dt, HttpContextBase httpContext)
            {
                Initialize(httpContext);
                ImportResponse res = new ImportResponse();

                if (dt != null && dt.Rows.Count > 0)
                {
                    try
                    {
                        var filteredRows = dt.Rows.Cast<DataRow>()
                          .Where(row => !row.ItemArray.All(f => f is DBNull));

                        DataTable dt1;

                        if (!filteredRows.Any())
                        {
                            dt1 = dt.Clone();
                            res.status = false;
                            res.statusCode = 103;
                        }
                        else
                        {
                            dt1 = filteredRows.CopyToDataTable();

                            string[] requiredColumns = { "DepartmentName" };
                            bool allColumnsExist = requiredColumns.All(col => dt1.Columns.Contains(col));

                            if (allColumnsExist)
                            {
                                res.departmentList = (from DataRow row in dt1.Rows
                                                      select new Departments
                                                      {
                                                          DepartmentName = row["DepartmentName"].ToString(),
                                                          DepartmentHeadName = row["DepartmentHead"].ToString(),
                                                          DeparmentEmailID = row["DepartmentEmail"].ToString(),
                                                          CompanyID = _cmdid,
                                                          BranchID = _branchid
                                                      }).ToList();

                                res.status = true;
                                res.statusCode = 101;
                            }
                            else
                            {
                                res.status = false;
                                res.statusCode = 102;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        res.status = false;
                        res.statusCode = 103;
                    }
                }
                else
                {
                    res.status = false;
                    res.statusCode = 103;
                }
                return res;
            }

            public static ImportResponse ConvertToImportedDataDesgList(DataTable dt)
            {
                ImportResponse res = new ImportResponse();

                if (dt != null && dt.Rows.Count > 0)
                {
                    try
                    {
                        var filteredRows = dt.Rows.Cast<DataRow>()
                          .Where(row => !row.ItemArray.All(f => f is DBNull));

                        DataTable dt1;

                        if (!filteredRows.Any())
                        {
                            dt1 = dt.Clone();
                            res.status = false;
                            res.statusCode = 103;
                        }
                        else
                        {
                            dt1 = filteredRows.CopyToDataTable();

                            string[] requiredColumns = { "DesignationName" };
                            bool allColumnsExist = requiredColumns.All(col => dt1.Columns.Contains(col));

                            if (allColumnsExist)
                            {
                                res.designationList = (from DataRow row in dt1.Rows
                                                       select new Designations
                                                       {
                                                           DesignationName = row["DesignationName"].ToString()
                                                       }).ToList();

                                res.status = true;
                                res.statusCode = 101;
                            }
                            else
                            {
                                res.status = false;
                                res.statusCode = 102;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        res.status = false;
                        res.statusCode = 103;
                    }
                }
                else
                {
                    res.status = false;
                    res.statusCode = 103;
                }
                return res;
            }

            public static ImportDeHiringResponse ConvertToImportedDataDeHiringList(DataTable dt, HttpContextBase httpContext)
            {
                Initialize(httpContext);
                ImportDeHiringResponse res = new ImportDeHiringResponse();
                //List<Companys> items = new List<Companys>();

                if (dt != null && dt.Rows.Count > 0)
                {
                    try
                    {
                        var filteredRows = dt.Rows.Cast<DataRow>()
                          .Where(row => !row.ItemArray.All(f => f is DBNull));

                        DataTable dt1;

                        if (!filteredRows.Any())
                        {
                            dt1 = dt.Clone();
                            res.status = false;
                            res.statusCode = 103;
                        }
                        else
                        {
                            dt1 = filteredRows.CopyToDataTable();

                            string[] requiredColumns = { "EmpPunchID", "EmpName", "Designation", "Department", "RehireStatus", "ReasonForLeaving", "Gender", "LastWorkingDate" };
                            bool allColumnsExist = requiredColumns.All(col => dt1.Columns.Contains(col));

                            if (allColumnsExist)
                            {
                                res.DeHiringList = (from DataRow row in dt1.Rows
                                                    select new DeHiring
                                                    {
                                                        EmpPunchID = row["EmpPunchID"].ToString(),
                                                        EmpName = row["EmpName"].ToString(),
                                                        Designation = row["Designation"].ToString(),
                                                        Department = row["Department"].ToString(),
                                                        Gender = row["Gender"].ToString(),
                                                        LastWorkingDate = row["LastWorkingDate"].ToString(),
                                                        ReasonForLeaving = row["ReasonForLeaving"].ToString(),
                                                        RehireStatus = row["RehireStatus"].ToString()

                                                    }).ToList();

                                res.status = true;
                                res.statusCode = 101;
                            }
                            else
                            {
                                res.status = false;
                                res.statusCode = 102;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        res.status = false;
                        res.statusCode = 103;
                    }
                }
                else
                {
                    res.status = false;
                    res.statusCode = 103;
                }
                return res;
            }
        }
        #endregion

        #region Download Error List

        #endregion

        #endregion for Import Class Master module wise

        #region deHiring
        public ActionResult DeHiring()
        {

            var employees = TempData["errEmployeesRecords"] as ImportDeHiringfaillog;
            ViewBag.error = TempData["error"];
            ViewBag.msg = TempData["msg"];
            return View(employees);
        }
        [HttpPost]
        public ActionResult DeHiring(HttpPostedFileBase FileUpload, string compcode)
        {
            try
            {
                string errorMessage;
                DataTable dt = ProcessUploadedFile(FileUpload, out errorMessage);
                ImportDeHiringResponse Response = new ImportDeHiringResponse();


                if (!string.IsNullOrEmpty(errorMessage))
                {
                    ViewBag.error = errorMessage;
                    return RedirectToAction("DeHiring");
                }

                // Common import result handling
                bool importSuccess = false;
                ImportDeHiringfaillog faillog = new ImportDeHiringfaillog();


                Response = DataTableConverter.ConvertToImportedDataDeHiringList(dt, this.HttpContext);

                if (Response.status && Response.statusCode == 101)
                {
                    if (Response.DeHiringList != null && Response.DeHiringList.Count > 0)
                    {
                        try
                        {
                            faillog.DehiringError = UtilitiesRestClient.ImportDeHiringDetail(Response.DeHiringList, compcode);
                            importSuccess = !faillog.DehiringError.Any();
                            if (faillog.DehiringError.Any())
                            {
                                TempData["msg"] = "Bulk dehiring partially completed! Some records were not updated due to invalid or missing information.";
                            }

                            TempData["errEmployeesRecords"] = faillog;

                        }
                        catch (Exception Ex)
                        {
                            TempData["error"] = "Error while importing file.";
                            importSuccess = false;
                        }
                    }
                    else if (Response.status == false && Response.statusCode == 103)
                    {
                        TempData["error"] = "Your Excel file contains no data.";
                        importSuccess = false;
                    }
                }
                else if (Response.status == false && Response.statusCode == 102)
                {
                    TempData["error"] = "Please import a file with the proper format.";
                    importSuccess = false;
                }
                else if (Response.status == false && Response.statusCode == 103)
                {
                    TempData["error"] = "Your Excel file contains no data.";
                    importSuccess = false;
                }

                if (importSuccess)
                {
                    TempData["msg"] = "Bulk dehiring completed successfully!";
                    dt.Clear();
                    dt.Dispose();
                }
                return RedirectToAction("DeHiring");
            }
            catch (Exception Ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        #endregion

    }
}