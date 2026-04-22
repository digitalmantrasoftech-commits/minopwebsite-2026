using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using PayTimeWebClient.Models.DataGridModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;


namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class DevelopersController : Controller
    {
        #region Declaration
        static readonly MasterDataRestClient RestClient = new MasterDataRestClient();
        static readonly UtilitiesDataRestClient UtilitiesRestClient = new UtilitiesDataRestClient();
        static readonly TransactionDataRestClient TransactionRestClient = new TransactionDataRestClient();
        #endregion

        #region Developers Dashboard
        [HttpGet]
        public ActionResult DevelopersDashboard()
        {
            ViewBag.AllCounter = RestClient.GetDeveloperDashBoardCount();
            ViewBag.EndPointlst = (EndPoints)TransactionRestClient.GetDeveloperEndPoint().Where(x => x.Endpointid == 1).ToList().FirstOrDefault();
            return View();
        }
        [HttpPost]
        public ActionResult DevelopersDashboard(EndPoints Ep)
        {
            try
            {
                ViewBag.msg = "";
                ViewBag.error = "";
                if (Ep.Endpointid != 1)
                {
                    var result = TransactionRestClient.EndPointsAdd(Ep);
                    if (result.MegSts == "ok")
                    {
                        ViewBag.msg = "End point created successfully.";
                    }
                    else
                    {
                        ViewBag.error = "Error in creating endpoint.";
                    }
                }
                else
                {
                    bool Isupdate = TransactionRestClient.EndPointsupdate(Ep.Endpointid, Ep);
                    if (Isupdate)
                    {
                        ViewBag.msg = "EndPoint Updated successfully.";
                    }
                    else
                    {
                        ViewBag.error = "Error in updating Endpoint";
                    }
                }
                ViewBag.AllCounter = RestClient.GetDeveloperDashBoardCount();
                ViewBag.EndPointlst = (EndPoints)TransactionRestClient.GetDeveloperEndPoint().Where(x => x.Endpointid == 1).ToList().FirstOrDefault();
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Developers");
            }
        }
        #endregion

        #region Device
        [HttpGet]
        public ActionResult Device()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            var compcode = Session["cmpcode"].ToString();
            Devices dvs = new Devices();
            try
            {
                dvs.DeviceList = RestClient.GetAllDeviceGridDeveloper(compcode);
                dvs.DeviceTypeList = RestClient.DeviceTypeGetAll();
                int[] ids = new List<string>(ConfigurationManager.AppSettings["lst_devicetypeid"].Split(',')).ConvertAll<int>(s => int.Parse(s)).ToArray();
                ViewBag.DeviceTypelst = RestClient.DeviceTypeGetAll().Where(item => ids.Contains(item.DeviceTypeCode));
                return View(dvs);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Developers");
            }
        }
        [HttpPost]
        public ActionResult Device(Devices dv, string IsDeviceSrNo)
        {
            var compcode = Session["cmpcode"].ToString();
            try
            {
                ViewBag.msg = "";
                ViewBag.error = "";
                int n;
                bool isNumeric = int.TryParse(dv.DeviceCode, out n);
                if (isNumeric)
                {
                    dv.Mode = "Default";
                    if (dv.DeviceId > 0)
                    {
                        dv.IsActive = true;
                        dv.IsPushData = 1;
                        dv.RegCompanyCode = Session["cmpcode"].ToString();
                        var jsonSerialiser = new JavaScriptSerializer();
                        var json = jsonSerialiser.Serialize(dv);
                        ErrorMsg ms = new ErrorMsg();
                        ms = RestClient.DevelopersDeviceUpdate(dv.DeviceId, dv);
                        if (ms.MegSts.ToLower() == "ok")
                        {
                            ViewBag.msg = ms.Meg;
                        }
                        else
                        {
                            ViewBag.error = ms.Meg;
                        }
                    }
                    else
                    {
                        dv.DeviceList = RestClient.DeviceGetAll().Where(c => c.DeviceCode == dv.DeviceCode);
                        if (dv.DeviceList.Count() > 0)
                        {
                            ViewBag.error = dv.DeviceCode + " Device code already exists.";
                        }
                        else
                        {
                            dv.RegCompanyCode = Session["cmpcode"].ToString();
                            ErrorMsg ms = new ErrorMsg();
                            dv.IsPushData = 1;
                            ms = RestClient.DevelopersdevicesAdd(dv);
                            if (ms.MegSts.ToLower() == "ok")
                            {
                                ViewBag.msg = ms.Meg;
                            }
                            else
                            {
                                ViewBag.error = ms.Meg;
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.error = dv.DeviceCode + " is not valid device code, Please enter numeric values.";
                }
                dv.DeviceList = RestClient.GetAllDeviceGridDeveloper(compcode);
                dv.DeviceTypeList = RestClient.DeviceTypeGetAll();
                int[] ids = new List<string>(ConfigurationManager.AppSettings["lst_devicetypeid"].Split(',')).ConvertAll<int>(s => int.Parse(s)).ToArray();
                ViewBag.DeviceTypelst = RestClient.DeviceTypeGetAll().Where(item => ids.Contains(item.DeviceTypeCode));
                return View(dv);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Developers");
            }
        }

        [HttpGet]
        public JsonResult GetDeviceSrCode(string DeviceSrNo, string DeviceCode)
        {
            if (DeviceSrNo == "")
            {
                DeviceSrNo = string.Empty;
            }
            if (DeviceCode == "")
            {
                DeviceCode = string.Empty;
            }
            JsonResult result;
            ErrorMsg emg = new ErrorMsg();
            emg = RestClient.CheckDeviceInfodeveloper(DeviceSrNo, DeviceCode);

            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(emg);
            result = Json(json);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        #endregion

        #region Transaction Data
        public ActionResult TransactionsData()
        {
            return View();
        }

        //[HttpPost]
        //public JsonResult TransactionsDataGrid(string jsondata, DataTableAjaxPostModel model)
        //{
        //    string[] List = jsondata.Split(',' , ':' , '{' , '}', '"' , ' '); 
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    dynamic item = serializer.Deserialize<object>(jsondata);
        //    int branchID = 1;
        //    model.branchid = branchID;
        //    model.FilterFromDate = List[5];
        //    model.FilterToDate = List[11];
        //    var AllData = TransactionRestClient.DeveloperTransactionDataGetAll(model).Select(i => new { i.txnId,i.punchId, i.dvcId, i.txnDateTime,i.isSync,i.LastActivity });
        //    DatatableCounts DataCount = TransactionRestClient.DeveloperTransactionDataGetAllCount(model);
        //    var result = new List<TransactionData>(AllData.Count());
        //    foreach (var data in AllData)
        //    {
        //        // simple remapping adding extra info to found dataset
        //        result.Add(new TransactionData
        //        {
        //            txnId = data.txnId,
        //            punchId = data.punchId,
        //            dvcId = data.dvcId,
        //            txnDateTime = data.txnDateTime,
        //            isSync = data.isSync,
        //            LastActivity = data.LastActivity

        //        });
        //    };

        //    return Json(new
        //    {
        //        // this is what datatables wants sending back
        //        draw = model.draw,
        //        recordsTotal = DataCount.totalResultsCount,
        //        recordsFiltered = DataCount.filteredResultsCount,
        //        data = result
        //    });
        //}

        [HttpPost]
        public JsonResult TransactionsDataGrid_16062023(string jsondata, DataTableAjaxPostModel model)
        {
            string[] List = jsondata.Split(',', ':', '{', '}', '"', ' ');
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic item = serializer.Deserialize<object>(jsondata);
            int branchID = 1;
            model.branchid = branchID;
            model.FilterFromDate = List[5];
            model.FilterToDate = List[11];
            model.PunchID = List[17];
            var AllData = TransactionRestClient.DeveloperTransactionDataGetAll(model).Select(i => new { i.txnId, i.punchId, i.dvcId, i.txnDateTime, i.isSync, i.LastActivity, i.EntryDate, i.Remarks, i.RemarksDate });
            DatatableCounts DataCount = TransactionRestClient.DeveloperTransactionDataGetAllCount(model);
            var result = new List<TransactionData>(AllData.Count());
            foreach (var data in AllData)
            {
                // simple remapping adding extra info to found dataset
                result.Add(new TransactionData
                {
                    txnId = data.txnId,
                    punchId = data.punchId,
                    dvcId = data.dvcId,
                    txnDateTime = data.txnDateTime,
                    isSync = data.isSync,
                    LastActivity = data.LastActivity,
                    EntryDate = data.EntryDate,
                    Remarks = data.Remarks,
                    RemarksDate = data.RemarksDate

                });
            };

            return Json(new
            {
                // this is what datatables wants sending back
                draw = model.draw,
                recordsTotal = DataCount.totalResultsCount,
                recordsFiltered = DataCount.filteredResultsCount,
                data = result
            });
        }

        [HttpPost]
        public JsonResult TransactionsDataGrid(string jsondata, DataTableAjaxPostModel model)
        {
            string[] List = jsondata.Split(',', ':', '{', '}', '"', ' ');
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic item = serializer.Deserialize<object>(jsondata);
            int branchID = 1;
            model.branchid = branchID;
            model.FilterFromDate = List[5];
            model.FilterToDate = List[11];
            model.PunchID = List[17];
            model.DeviceCode = List[23];
            if (string.IsNullOrEmpty(model.DeviceCode))
            {
                model.DeviceCode = "0";
            }
            var AllData = TransactionRestClient.DeveloperTransactionDataGetAll(model).Select(i => new { i.txnId, i.punchId, i.dvcId, i.txnDateTime, i.isSync, i.LastActivity, i.EntryDate, i.Remarks, i.RemarksDate });


            return Json(new
            {
                data = AllData
            });
        }



        #endregion

        #region Download Sample
        public ActionResult Download()
        {
            return View();
        }
        #endregion

        public ActionResult ErrorPage()
        {
            return View();
        }


        #region PlanRenewal
        public ActionResult PlanRenewal()
        {
            return View();
        }
        #endregion

        #region ImportEmployees
        [HttpGet]
        public ActionResult ImportEmployees()
        {
            return View();
        }
        [CustAuthFilter]
        [HttpPost]
        public ActionResult ImportEmployees(HttpPostedFileBase fileUpload)
        {
            try
            {
                if (fileUpload == null)
                {
                    ViewBag.error = "Please Select File.";
                    return View();
                }
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(fileUpload.FileName);
                    if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".csv")
                    {
                        string fileLocation = Server.MapPath("~/Uploads/") + fileUpload.FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }
                        fileUpload.SaveAs(fileLocation);
                        string excelConnectionString = string.Empty;

                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        else if (fileExtension == ".xlsx")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        else if (fileExtension == ".csv")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        // Create Connection to Excel workbook and add the OleDb namespace
                        using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
                        {
                            excelConnection.Open();
                            DataTable dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                            if (dt == null)
                            {
                                return null;
                            }

                            // Get the first sheet name
                            string sheetName = dt.Rows[0]["TABLE_NAME"].ToString();

                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
                            {
                                DataSet ds = new DataSet();
                                dataAdapter.Fill(ds);
                                #region Add Excel data to a list
                                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                {
                                    DataTable dataTable = ds.Tables[0];
                                    List<ImportDevEmployee> items = new List<ImportDevEmployee>();
                                    bool hasErrors = false;
                                    List<ImportDevEmployeefaillog> errorList = new List<ImportDevEmployeefaillog>();
                                    ImportDevEmployeefaillog faillog = new ImportDevEmployeefaillog();
                                    if (dataTable.Rows.Count > 0)
                                    {
                                        try
                                        {

                                            foreach (DataRow row in dataTable.Rows)
                                            {
                                                int empPunchID;
                                                string empPunchIDStr = row["EmpPunchID"].ToString().Trim();
                                                if (string.IsNullOrEmpty(empPunchIDStr))
                                                {
                                                    errorList.Add(new ImportDevEmployeefaillog
                                                    {
                                                        ImportedFailId = dataTable.Rows.IndexOf(row) + 1,
                                                        Reason = "EmpPunchID must not be be blank.",
                                                        EmpPunchID = row["EmpPunchID"].ToString().Trim(),
                                                        EmpName = row["EmpName"].ToString().Trim()
                                                    });
                                                    hasErrors = true;
                                                    continue;
                                                }
                                                if (empPunchIDStr.Length > 10) // Adjust length limits as needed
                                                {
                                                    errorList.Add(new ImportDevEmployeefaillog
                                                    {
                                                        ImportedFailId = dataTable.Rows.IndexOf(row) + 1,
                                                        Reason = "EmpPunchID must not exceed 10 digits  : " + empPunchIDStr,
                                                        EmpPunchID = row["EmpPunchID"].ToString().Trim(),
                                                        EmpName = row["EmpName"].ToString().Trim()
                                                    });
                                                    hasErrors = true;
                                                    continue;
                                                }
                                                if (!int.TryParse(empPunchIDStr, out empPunchID))
                                                {
                                                    errorList.Add(new ImportDevEmployeefaillog
                                                    {
                                                        ImportedFailId = dataTable.Rows.IndexOf(row) + 1,
                                                        Reason = "EmpPunchID must be numeric: " + empPunchIDStr,
                                                        EmpPunchID = row["EmpPunchID"].ToString().Trim(),
                                                        EmpName = row["EmpName"].ToString().Trim()
                                                    });
                                                    hasErrors = true;
                                                    continue;
                                                }
                                                string empName = row["EmpName"].ToString();
                                                if (string.IsNullOrEmpty(empName))
                                                {
                                                    errorList.Add(new ImportDevEmployeefaillog
                                                    {
                                                        ImportedFailId = dataTable.Rows.IndexOf(row) + 1,
                                                        Reason = "EmpName must not be blank.",
                                                        EmpName = empName,
                                                        EmpPunchID = empPunchIDStr
                                                    });
                                                    hasErrors = true;
                                                    continue;
                                                }
                                                if (empName.Length > 50)
                                                {
                                                    errorList.Add(new ImportDevEmployeefaillog
                                                    {
                                                        ImportedFailId = dataTable.Rows.IndexOf(row) + 1,
                                                        Reason = "EmpName must not exceed 50 digits  : " + empPunchIDStr,
                                                        EmpPunchID = row["EmpPunchID"].ToString().Trim(),
                                                        EmpName = row["EmpName"].ToString().Trim()
                                                    });
                                                    hasErrors = true;
                                                    continue;
                                                }
                                                if (!System.Text.RegularExpressions.Regex.IsMatch(empName, @"^[a-zA-Z\s]+$"))
                                                {
                                                    errorList.Add(new ImportDevEmployeefaillog
                                                    {
                                                        ImportedFailId = dataTable.Rows.IndexOf(row) + 1,
                                                        Reason = "EmpName Not Valid: " + empName,
                                                        EmpName = empName,
                                                        EmpPunchID = row["EmpPunchID"].ToString().Trim(),
                                                    });
                                                    hasErrors = true;
                                                    continue;
                                                }
                                                items.Add(new ImportDevEmployee
                                                {
                                                    EmpPunchID = empPunchID,
                                                    EmpName = empName
                                                });

                                            }
                                            if (hasErrors)
                                            {
                                                ViewBag.Importfailloglist = errorList;
                                                ViewBag.error = "An error occurred while importing employees.";
                                                TempData["IsValid"] = false;
                                                return View();
                                            }
                                            else
                                            {
                                                faillog.Importfailloglist = RestClient.ImportDevEmployees(items);
                                                var importSuccess = !faillog.Importfailloglist.Any();
                                                if (importSuccess)
                                                {
                                                    ViewBag.msg = "Employees imported successfully.";
                                                    TempData["IsValid"] = true;
                                                }
                                                else
                                                {
                                                    ViewBag.error = "An error occurred while saving employees.";
                                                    TempData["IsValid"] = false;
                                                }
                                                ViewBag.Importfailloglist = faillog.Importfailloglist;

                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ViewBag.error = "Invalid file. Please import proper format file.";
                                            return View("ImportEmployees");
                                        }
                                    }
                                }
                                #endregion
                            }

                        }
                    }
                    else
                    {
                        ViewBag.error = "Please Upload Files in .xls, .xlsx or .csv format";
                    }
                }
                return View();
            }
            catch (Exception Ex)
            {
                ViewBag.error = "The imported file is in a bad format: " + Ex.Message;
                return View();
            }
        }
        #endregion


        #region Enroll User
        public ActionResult EnrollUser()
        {
            return View();
        }
        #endregion


        #region For TransctionGridDataConvertServerClientSide
        public async Task<JsonResult> GetPaginatedTransctionGridData(DataTableAjaxPostModelForTransctionData request)
        {
            // Initialize variables
            string searchTerm = string.Empty;
            string sortColumn = "EmpPunchID"; // Default sort column
            string sortOrder = "asc";     // Default sort order
            int pageSize = 10;            // Default page size
            int page = 1;                 // Default page number            
            Int64 DataGridValues = 0;
            // Check if request is not null
            if (request != null)
            {
                // Check if search object is present and has value

                if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                {
                    searchTerm = request.search.value;
                }
                // Check if order array is present and has elements
                if (request.order != null && request.length > 0)
                {
                    sortColumn = request.columns[request.order[0].column].data;
                    sortOrder = request.order[0].dir;
                }
                if (request.DatagridThresold > 0)
                {
                    DataGridValues = request.DatagridThresold;
                }
                // Set pageSize and page if length and start are set
                if (request.length > 0)
                {
                    pageSize = request.length;
                }
                if (request.start >= 0)
                {
                    page = (request.start / pageSize) + 1;
                }
            }
            DataTableForEnrollUserGridDataRequest dataRequest = new DataTableForEnrollUserGridDataRequest
            {
                page = page,
                pageSize = pageSize,
                searchTerm = searchTerm,
                sortOrder = sortOrder,
                sortColumn = sortColumn,
                DataGridValues = DataGridValues,
                BranchId = request.branchid,
                FilterFromDate = request.FilterFromDate,
                FilterToDate = request.FilterToDate,
                DeviceCode = request.DeviceCode
            };
            return Json("", JsonRequestBehavior.AllowGet);
            //var paginatedResult = "";// RestClient.GetPaginatedEnrollUserAsync(dataRequest);
            //return Json(new
            //{
            //    draw = request.draw,
            //    recordsTotal = paginatedResult.Result.TotalItems,
            //    recordsFiltered = paginatedResult.Result.TotalItems,
            //    data = paginatedResult.Result
            //});
        }
        #endregion


        #region MySubscription
        public ActionResult MySubscription()
        {
            return View();
        }
        #endregion

    }
}