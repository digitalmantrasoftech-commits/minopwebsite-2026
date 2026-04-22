using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class EmployeeController : Controller
    {
        #region Declaration
        static readonly MasterDataRestClient RestClient = new MasterDataRestClient();
        static readonly EncryptionHelper EncryptionHelperr = new EncryptionHelper();
        static readonly UtilitiesDataRestClient UtilitiesRestClient = new UtilitiesDataRestClient();
        static readonly EmployeeDataRestClientFastrack ESSRestClient = new EmployeeDataRestClientFastrack();
        #endregion

        #region Directory
        [HttpGet]
        public ActionResult Directory()
        {
            var getSysParam = RestClient.GetSystemSettingById(1);
            ViewBag.IsEss = Convert.ToBoolean(getSysParam.IsEss.ToString() != "" ? getSysParam.IsEss.ToString() : "0");
            ViewBag.HasSMS = Convert.ToBoolean(getSysParam.HasSMS.ToString() != "" ? getSysParam.HasSMS.ToString() : "0");
            return View();
        }
        #endregion

        #region OnBorading
        [HttpGet]
        public ActionResult OnBoarding()
        {
            var getSysParam = RestClient.GetSystemSettingById(1);
            ViewBag.IsEss = Convert.ToBoolean(getSysParam.IsEss.ToString() != "" ? getSysParam.IsEss.ToString() : "0");
            ViewBag.HasSMS = Convert.ToBoolean(getSysParam.HasSMS.ToString() != "" ? getSysParam.HasSMS.ToString() : "0");
            return View();
        }
        /// <summary>
        /// Upload Adhar card 
        /// </summary>
        /// <param name="EmpAadharCardPhoto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadEmployeeFilesAdharCard(IEnumerable<HttpPostedFileBase> EmpAadharCardPhoto, string EmpCode)
        {
            try
            {
                foreach (HttpPostedFileBase file in EmpAadharCardPhoto)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string uploadFolderPath = Server.MapPath("~/UploadOnboardingEmployeeDoc");

                        DirectoryInfo directory = new DirectoryInfo(uploadFolderPath);

                        if (!directory.Exists)
                        {
                            directory.Create();
                            string uploadFolderPathasEmpCode = Server.MapPath("~/UploadOnboardingEmployeeDoc/" + EmpCode + "");
                            DirectoryInfo directory1 = new DirectoryInfo(uploadFolderPathasEmpCode);

                            if (!directory1.Exists)
                            {
                                directory1.Create();
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                            else
                            {
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                        }
                        else
                        {
                            string uploadFolderPathasEmpCode = Server.MapPath("~/UploadOnboardingEmployeeDoc/" + EmpCode + "");
                            DirectoryInfo directory1 = new DirectoryInfo(uploadFolderPathasEmpCode);

                            if (!directory1.Exists)
                            {
                                directory1.Create();
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                            else
                            {
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                        }
                    }
                }

                return Json(new { success = true, message = "Files uploaded successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while uploading files: " + ex.Message });
            }
        }


        /// <summary>
        /// Upload Adhar card of employee
        /// </summary>
        /// <param name="EmpAadharCardPhoto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadAdharcard(IEnumerable<HttpPostedFileBase> Emp_adhar_photo, string EmpCode)
        {
            try
            {
                foreach (HttpPostedFileBase file in Emp_adhar_photo)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string uploadFolderPath = Server.MapPath("~/UploadOnboardingEmployeeDoc");

                        DirectoryInfo directory = new DirectoryInfo(uploadFolderPath);

                        if (!directory.Exists)
                        {
                            directory.Create();
                            string uploadFolderPathasEmpCode = Server.MapPath("~/UploadOnboardingEmployeeDoc/" + EmpCode + "");
                            DirectoryInfo directory1 = new DirectoryInfo(uploadFolderPathasEmpCode);

                            if (!directory1.Exists)
                            {
                                directory1.Create();
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                            else
                            {
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                        }
                        else
                        {
                            string uploadFolderPathasEmpCode = Server.MapPath("~/UploadOnboardingEmployeeDoc/" + EmpCode + "");
                            DirectoryInfo directory1 = new DirectoryInfo(uploadFolderPathasEmpCode);

                            if (!directory1.Exists)
                            {
                                directory1.Create();
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                            else
                            {
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                        }
                    }
                }

                return Json(new { success = true, message = "Files uploaded successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while uploading files: " + ex.Message });
            }
        }

        /// <summary>
        /// Upload Pan card 
        /// </summary>
        /// <param name="EmpPanCardPhoto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadEmployeeFilesPanCard(IEnumerable<HttpPostedFileBase> EmpPanCardPhoto, string EmpCode)
        {
            try
            {
                foreach (HttpPostedFileBase file in EmpPanCardPhoto)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string uploadFolderPath = Server.MapPath("~/UploadOnboardingEmployeeDoc");

                        DirectoryInfo directory = new DirectoryInfo(uploadFolderPath);

                        if (!directory.Exists)
                        {
                            directory.Create();
                            string uploadFolderPathasEmpCode = Server.MapPath("~/UploadOnboardingEmployeeDoc/" + EmpCode + "");
                            DirectoryInfo directory1 = new DirectoryInfo(uploadFolderPathasEmpCode);

                            if (!directory1.Exists)
                            {
                                directory1.Create();
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                            else
                            {
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                        }
                        else
                        {
                            string uploadFolderPathasEmpCode = Server.MapPath("~/UploadOnboardingEmployeeDoc/" + EmpCode + "");
                            DirectoryInfo directory1 = new DirectoryInfo(uploadFolderPathasEmpCode);

                            if (!directory1.Exists)
                            {
                                directory1.Create();
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                            else
                            {
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                        }
                    }
                }
                // Upload successful, return a success response
                return Json(new { success = true, message = "Files uploaded successfully." });
            }
            catch (Exception ex)
            {
                // Upload failed, return an error response
                return Json(new { success = false, message = "An error occurred while uploading files: " + ex.Message });
            }
        }


        /// <summary>
        /// Upload Pan card of employee
        /// </summary>
        /// <param name="EmpPanCardPhoto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadPanCard(IEnumerable<HttpPostedFileBase> emp_pan_Photo, string EmpCode)
        {
            try
            {
                foreach (HttpPostedFileBase file in emp_pan_Photo)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string uploadFolderPath = Server.MapPath("~/UploadOnboardingEmployeeDoc");

                        DirectoryInfo directory = new DirectoryInfo(uploadFolderPath);

                        if (!directory.Exists)
                        {
                            directory.Create();
                            string uploadFolderPathasEmpCode = Server.MapPath("~/UploadOnboardingEmployeeDoc/" + EmpCode + "");
                            DirectoryInfo directory1 = new DirectoryInfo(uploadFolderPathasEmpCode);

                            if (!directory1.Exists)
                            {
                                directory1.Create();
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                            else
                            {
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                        }
                        else
                        {
                            string uploadFolderPathasEmpCode = Server.MapPath("~/UploadOnboardingEmployeeDoc/" + EmpCode + "");
                            DirectoryInfo directory1 = new DirectoryInfo(uploadFolderPathasEmpCode);

                            if (!directory1.Exists)
                            {
                                directory1.Create();
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                            else
                            {
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                        }
                    }
                }
                // Upload successful, return a success response
                return Json(new { success = true, message = "Files uploaded successfully." });
            }
            catch (Exception ex)
            {
                // Upload failed, return an error response
                return Json(new { success = false, message = "An error occurred while uploading files: " + ex.Message });
            }
        }


        /// <summary>
        /// Upload AdharTWo card 
        /// </summary>
        /// <param name="EmpAadharCardPhotoTwo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadEmployeeFilesAdharCardTwo(IEnumerable<HttpPostedFileBase> EmpTwoAadharCardPhoto, string EmpCode)
        {
            try
            {
                foreach (HttpPostedFileBase file in EmpTwoAadharCardPhoto)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string uploadFolderPath = Server.MapPath("~/UploadOnboardingEmployeeDoc");

                        DirectoryInfo directory = new DirectoryInfo(uploadFolderPath);

                        if (!directory.Exists)
                        {
                            directory.Create();
                            string uploadFolderPathasEmpCode = Server.MapPath("~/UploadOnboardingEmployeeDoc/" + EmpCode + "");
                            DirectoryInfo directory1 = new DirectoryInfo(uploadFolderPathasEmpCode);

                            if (!directory1.Exists)
                            {
                                directory1.Create();
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                            else
                            {
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                        }
                        else
                        {
                            string uploadFolderPathasEmpCode = Server.MapPath("~/UploadOnboardingEmployeeDoc/" + EmpCode + "");
                            DirectoryInfo directory1 = new DirectoryInfo(uploadFolderPathasEmpCode);

                            if (!directory1.Exists)
                            {
                                directory1.Create();
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                            else
                            {
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                        }
                    }
                }
                // Upload successful, return a success response
                return Json(new { success = true, message = "Files uploaded successfully." });
            }
            catch (Exception ex)
            {
                // Upload failed, return an error response
                return Json(new { success = false, message = "An error occurred while uploading files: " + ex.Message });
            }
        }

        /// <summary>
        /// Upload PanTWo card 
        /// </summary>
        /// <param name="EmpPanCardPhotoTWo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadEmployeeFilesPanCardTwo(IEnumerable<HttpPostedFileBase> EmpTwoPanCardPhoto, string EmpCode)
        {
            try
            {
                foreach (HttpPostedFileBase file in EmpTwoPanCardPhoto)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string uploadFolderPath = Server.MapPath("~/UploadOnboardingEmployeeDoc");

                        DirectoryInfo directory = new DirectoryInfo(uploadFolderPath);

                        if (!directory.Exists)
                        {
                            directory.Create();
                            string uploadFolderPathasEmpCode = Server.MapPath("~/UploadOnboardingEmployeeDoc/" + EmpCode + "");
                            DirectoryInfo directory1 = new DirectoryInfo(uploadFolderPathasEmpCode);

                            if (!directory1.Exists)
                            {
                                directory1.Create();
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                            else
                            {
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                        }
                        else
                        {
                            string uploadFolderPathasEmpCode = Server.MapPath("~/UploadOnboardingEmployeeDoc/" + EmpCode + "");
                            DirectoryInfo directory1 = new DirectoryInfo(uploadFolderPathasEmpCode);

                            if (!directory1.Exists)
                            {
                                directory1.Create();
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                            else
                            {
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                        }
                    }
                }
                // Upload successful, return a success response
                return Json(new { success = true, message = "Files uploaded successfully." });
            }
            catch (Exception ex)
            {
                // Upload failed, return an error response
                return Json(new { success = false, message = "An error occurred while uploading files: " + ex.Message });
            }
        }

        /// <summary>
        /// Upload Passbook
        /// </summary>
        /// <param name="EmpPassbookPhoto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadEmployeeFilesPassBook(IEnumerable<HttpPostedFileBase> EmpPassbookPhoto, string EmpCode)
        {
            try
            {
                foreach (HttpPostedFileBase file in EmpPassbookPhoto)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string uploadFolderPath = Server.MapPath("~/UploadOnboardingEmployeeDoc");

                        DirectoryInfo directory = new DirectoryInfo(uploadFolderPath);

                        if (!directory.Exists)
                        {
                            directory.Create();
                            string uploadFolderPathasEmpCode = Server.MapPath("~/UploadOnboardingEmployeeDoc/" + EmpCode + "");
                            DirectoryInfo directory1 = new DirectoryInfo(uploadFolderPathasEmpCode);

                            if (!directory1.Exists)
                            {
                                directory1.Create();
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                            else
                            {
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                        }
                        else
                        {
                            string uploadFolderPathasEmpCode = Server.MapPath("~/UploadOnboardingEmployeeDoc/" + EmpCode + "");
                            DirectoryInfo directory1 = new DirectoryInfo(uploadFolderPathasEmpCode);

                            if (!directory1.Exists)
                            {
                                directory1.Create();
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                            else
                            {
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                        }
                    }
                }
                // Upload successful, return a success response
                return Json(new { success = true, message = "Files uploaded successfully." });
            }
            catch (Exception ex)
            {
                // Upload failed, return an error response
                return Json(new { success = false, message = "An error occurred while uploading files: " + ex.Message });
            }
        }

        /// <summary>
        /// Upload CheckBook
        /// </summary>
        /// <param name="EmpCheckbookPhoto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadEmpPhoto(IEnumerable<HttpPostedFileBase> EmpPhotoFile, string EmpCode)
        {
            try
            {
                string fileName = string.Empty;
                foreach (HttpPostedFileBase file in EmpPhotoFile)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var accountcode = Session["cmpcode"].ToString();
                        var FileName = Guid.NewGuid().ToString().Substring(0, 6);
                        var photoname = FileName + "_" + accountcode;
                        fileName = photoname + ".jpg";
                        string uploadFolderPath = Server.MapPath("~/UploadEmpPhoto");
                        string filePath = Path.Combine(uploadFolderPath, fileName);
                        // Save the file with the unique name
                        file.SaveAs(filePath);
                    }
                }
                // Upload successful, return the name of the photo
                return Json(new { success = true, photoName = fileName });
            }
            catch (Exception ex)
            {
                // Upload failed, return an error response
                return Json(new { success = false, message = "An error occurred while uploading files: " + ex.Message });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadEmployeeFilesCheckbook(IEnumerable<HttpPostedFileBase> EmpCheckbookPhoto, string EmpCode)
        {
            try
            {
                foreach (HttpPostedFileBase file in EmpCheckbookPhoto)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string uploadFolderPath = Server.MapPath("~/UploadOnboardingEmployeeDoc");

                        DirectoryInfo directory = new DirectoryInfo(uploadFolderPath);

                        if (!directory.Exists)
                        {
                            directory.Create();
                            string uploadFolderPathasEmpCode = Server.MapPath("~/UploadOnboardingEmployeeDoc/" + EmpCode + "");
                            DirectoryInfo directory1 = new DirectoryInfo(uploadFolderPathasEmpCode);

                            if (!directory1.Exists)
                            {
                                directory1.Create();
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                            else
                            {
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                        }
                        else
                        {
                            string uploadFolderPathasEmpCode = Server.MapPath("~/UploadOnboardingEmployeeDoc/" + EmpCode + "");
                            DirectoryInfo directory1 = new DirectoryInfo(uploadFolderPathasEmpCode);

                            if (!directory1.Exists)
                            {
                                directory1.Create();
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                            else
                            {
                                var path = Path.Combine(uploadFolderPathasEmpCode, fileName);
                                file.SaveAs(path);
                            }
                        }
                    }
                }
                // Upload successful, return a success response
                return Json(new { success = true, message = "Files uploaded successfully." });
            }
            catch (Exception ex)
            {
                // Upload failed, return an error response
                return Json(new { success = false, message = "An error occurred while uploading files: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UploadEmpPhotoForWebcamImage(EmployeePhoto data)
        {
            try
            {
                string EmpPhotoFile = data.EmpPhotoData;
                string fileName = string.Empty;

                if (!string.IsNullOrEmpty(EmpPhotoFile))
                {
                    var accountcode = Session["cmpcode"].ToString();
                    var FileName = Guid.NewGuid().ToString().Substring(0, 6);
                    var photoname = FileName + "_" + accountcode;
                    fileName = photoname + ".jpg";
                    string uploadFolderPath = Server.MapPath("~/UploadEmpPhoto");
                    string filePath = Path.Combine(uploadFolderPath, fileName);

                    EmpPhotoFile = EmpPhotoFile.PadRight(EmpPhotoFile.Length + (4 - EmpPhotoFile.Length % 4) % 4, '=');

                    // Convert base64 string to byte array and save as image file
                    byte[] imageBytes = Convert.FromBase64String(EmpPhotoFile);
                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                }
                // Upload successful, return the name of the photo
                return Json(new { success = true, photoName = fileName });
            }
            catch (Exception ex)
            {
                // Upload failed, return an error response
                return Json(new { success = false, message = "An error occurred while uploading files: " + ex.Message });
            }
        }

        #endregion

        #region Strength
        public ActionResult Strength()
        {
            return View();
        }
        #endregion

        #region Approveprint

        public async Task<ActionResult> Approveprint(string id)
        {
            try
            {
                // Assuming RestClient.PrintOnboardingEmployee is an async method returning Task<OnBordingEmployee>

                var OnBordingID = EncryptionHelperr.Decrypt(id);
                var ID = Convert.ToInt32(OnBordingID);
                OnBordingEmployee objEmp = await RestClient.PrintOnboardingEmployee(ID);
                ViewBag.OnboardingEmployee = objEmp;

                int paystructureID = objEmp.Table[0].PaystuctureID;
                string MonthlyCtc = objEmp.Table[0].MonthlyCTC;

                var EncryptCTC = EncryptionHelperr.Encrypt(MonthlyCtc);

                PayrollEmpDetails objpayroll = await RestClient.PrintOnboardingPayrollEmployee(paystructureID, EncryptCTC);
                ViewBag.PayrollEmpDetails = objpayroll;

                return View();
            }
            catch (Exception ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("Approveprint", ex.Message);
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        #endregion


        #region For Attandance Correction Fasttrack

        public ActionResult AttendanceCorrection(string EmpId, string AttnDate, string brid, string cmpid, string deptId, string desgId, string shiftId)
        {
            try
            {
                if (!string.IsNullOrEmpty(EmpId))
                {
                    Session["EmpIdnew"] = EmpId;
                }
                if (!string.IsNullOrEmpty(AttnDate))
                {
                    Session["AttnDatenew"] = AttnDate;
                }
                if (!string.IsNullOrEmpty(brid))
                {
                    Session["bridnew"] = brid;
                }
                if (!string.IsNullOrEmpty(cmpid))
                {
                    Session["cmpidnew"] = cmpid;
                }
                if (!string.IsNullOrEmpty(deptId))
                {
                    Session["deptIdnew"] = deptId;
                }
                if (!string.IsNullOrEmpty(desgId))
                {
                    Session["desgIdnew"] = desgId;
                }
                if (!string.IsNullOrEmpty(shiftId))
                {
                    Session["shiftIdnew"] = shiftId;
                }
            }
            catch (Exception ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("AttendanceCorrection", ex.Message);
                return RedirectToAction("ErrorPage", "PayTime");
            }
            return View();
        }
        #endregion


        #region For Check compresseOnBordingGridData
        public async Task<JsonResult> OnBordingGetCompressData(OnboardingEmployeeGetAllData objData)
        {
            JsonResult result;
            try
            {
                OnBordingEmployee emp = new OnBordingEmployee();
                var getEmpReporting = RestClient.OnBordingDataGridCompressData(objData);
                result = Json(getEmpReporting);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.MaxJsonLength = int.MaxValue;
                string jsonData = JsonConvert.SerializeObject(getEmpReporting);
                byte[] compressedData = CompressionUtils.Compress(jsonData);
                return Json(Convert.ToBase64String(compressedData), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        #endregion

        public class uploadWebCamData
        {
            public string EmpPhotoFile { get; set; }
        }
        public class EmployeePhoto
        {
            public string EmpPhotoData { get; set; }
        }
        [CustAuthFilter]
        [HttpGet]
        public JsonResult ApproveprintOnBordingIDEncrypt(int ID)
        {
            JsonResult result;
            var OnBordingID = EncryptionHelperr.Encrypt(ID.ToString());
            result = Json(new { OnBordingID = OnBordingID });
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }


        #region For Attandance Correction Data Bind
        [HttpPost]
        public JsonResult GetAttendanceDetailsForEmployeeWise(string RoleID, string loginemployeeid, string Month, string Year, string status, string Employee, int isAttaDate, string Pre_attnDate)
        {
            JsonResult result;
            string res = string.Empty;
            var data = "";
            AttanList obj = new AttanList();
            try
            {
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

                string jsonResponse = UtilitiesRestClient.GetAttendanceDetailsFastrack(obj);
                var attendanceList = JsonConvert.DeserializeObject<List<AttendanceRecord>>(jsonResponse);
                int loginEmpId;

                List<AttendanceRecord> filteredList = new List<AttendanceRecord>();

                if (int.TryParse(obj.loginemployeeid, out loginEmpId))
                {
                    var dynamicDateFormat = Session["IsDateFormat"].ToString();
                    var isDateformat = "";
                    if (dynamicDateFormat == "dd-M-yyyy" || dynamicDateFormat == "M-dd-yyyy" || dynamicDateFormat == "yyyy-M-dd")
                    {
                        isDateformat = Session["IsDateFormat"].ToString().Replace("M", "MMM");
                    }
                    else
                    {
                        isDateformat = Session["IsDateFormat"].ToString().Replace("mm", "MM");
                    }
                    if (isAttaDate == 0)
                    {
                        filteredList = attendanceList
                           .Where(a => a.Att_dt == Pre_attnDate && a.InTime != null) // Corrected the date format to "MM"
                           .OrderByDescending(a => a.Att_dt)
                           .ToList();
                    }
                    else
                    {
                        filteredList = attendanceList
                        .Where(a => a.OutTime != null).ToList();
                        //.OrderByDescending(a => a.Att_dt)
                        //.ToList();
                    }
                    // Filter the list based on the loginemployeeid
                    //var filteredList = attendanceList.Where(a => a.EmpID == loginEmpId).OrderByDescending(a => a.Att_dt).ToList();
                    // Serialize the filtered list to JSON string
                    string filteredJsonResponse = JsonConvert.SerializeObject(filteredList);
                    // Return the JSON string as part of a JSON result
                    result = Json(filteredJsonResponse);
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    result.MaxJsonLength = Int32.MaxValue;
                }
                else
                {
                    result = Json(new { error = "Invalid loginemployeeid" });
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                }
            }
            catch (Exception ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("GetAttendanceDetailsForEmployeeWise FastTrack", ex.Message);
                result = Json(new { success = false, message = "An error occurred while processing your request." });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            }
            return result;
        }
        #endregion
        #region For Attandance Correction Data Bind
        [HttpPost]
        public JsonResult GetAttendanceDetailsForEmployeeWise_Fastrack(string RoleID, string loginemployeeid, string Month, string Year, string status, string Employee, int isAttaDate, string Pre_attnDate, string brid, string cmpid, string deptId, string desgId, string shiftId)
        {
            JsonResult result;
            string res = string.Empty;
            var data = "";
            AttanList obj = new AttanList();
            try
            {
                obj.RoleID = RoleID;
                if (Pre_attnDate != "")
                {
                    obj.Date = Pre_attnDate.Split('T')[0];
                }
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
                obj.brid = brid;
                obj.cmpid = cmpid;
                obj.deptId = deptId;
                obj.desgId = desgId;
                obj.shiftId = shiftId;

                string jsonResponse = UtilitiesRestClient.GetAttendanceDetailsFastrack_NewMissPunch(obj);
                var attendanceList = JsonConvert.DeserializeObject<List<AttendanceRecord>>(jsonResponse);
                int loginEmpId;

                List<AttendanceRecord> filteredList = new List<AttendanceRecord>();

                if (int.TryParse(obj.loginemployeeid, out loginEmpId))
                {
                    var dynamicDateFormat = Session["IsDateFormat"].ToString();
                    var isDateformat = "";
                    if (dynamicDateFormat == "dd-M-yyyy" || dynamicDateFormat == "M-dd-yyyy" || dynamicDateFormat == "yyyy-M-dd")
                    {
                        isDateformat = Session["IsDateFormat"].ToString().Replace("M", "MMM");
                    }
                    else
                    {
                        isDateformat = Session["IsDateFormat"].ToString().Replace("mm", "MM");
                    }
                    if (isAttaDate == 0)
                    {
                        filteredList = attendanceList
                           .Where(a => a.Att_dt == Pre_attnDate) // Corrected the date format to "MM"                                                               
                           .OrderByDescending(a => a.Att_dt)
                           .ToList();
                    }
                    else
                    {
                        filteredList = attendanceList.Where(a => a.OutTime != null).ToList();
                    }
                    string filteredJsonResponse = JsonConvert.SerializeObject(filteredList);                    
                    result = Json(filteredJsonResponse);
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    result.MaxJsonLength = Int32.MaxValue;
                }
                else
                {
                    result = Json(new { error = "Invalid loginemployeeid" });
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                }
            }
            catch (Exception ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("GetAttendanceDetailsForEmployeeWise FastTrack", ex.Message);
                result = Json(new { success = false, message = "An error occurred while processing your request." });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            }
            return result;
        }
        [HttpPost]
        public JsonResult AttendanceCorrectionAdd(string CorrectionDetails)
        {
            JsonResult result;
            try
            {
                //JavaScriptSerializer js = new JavaScriptSerializer();
                var dynamicDateFormat = Session["IsDateFormat"].ToString();
                var isDateformat = "";
                if (dynamicDateFormat == "dd-M-yyyy" || dynamicDateFormat == "M-dd-yyyy" || dynamicDateFormat == "yyyy-M-dd")
                {
                    isDateformat = Session["IsDateFormat"].ToString().Replace("M", "MMM");
                }
                else
                {
                    isDateformat = Session["IsDateFormat"].ToString().Replace("mm", "MM");
                }
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = isDateformat.ToString() };
                AttendanceCorrection objCorrection = JsonConvert.DeserializeObject<AttendanceCorrection>(CorrectionDetails, dateTimeConverter);
                objCorrection.InPunchTime = objCorrection.InPunchTime;
                objCorrection.OutPunchTime = objCorrection.OutPunchTime;
                objCorrection.CreatedDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
                objCorrection.ModifyDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
                objCorrection.AttCorrectionId = objCorrection.AttendanceId;
                objCorrection.ModifyBy = objCorrection.EmpId;
                var i = ESSRestClient.AttendanceCorrectionCreate(objCorrection);
                var resp = JsonConvert.DeserializeObject<MRespo>(i);
                if (resp.MegSts.ToLower() == "ok")
                {
                    TempData["Msg"] = resp.Meg;
                }
                else
                {
                    TempData["error"] = resp.Meg;
                }
                result = Json(i);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            }
            catch (Exception ex)
            {
                GetDeviceDetails.ProcessLogLogFileWrite("AttendanceCorrectionAdd FastTrack", ex.Message);
                result = Json(new { success = false, message = "An error occurred while processing your request." });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            }
            return result;
        }
        #endregion


        #region SendReportMail
         [HttpGet]
        public ActionResult ReportEmailSender()
        {
            try
            {
                if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    ViewBag.Companyslist = RestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid).ToList();
                }
                else
                {
                    ViewBag.Companyslist = RestClient.CompanyGetAll();
                }

                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }



         [HttpPost]
         public JsonResult SaveSchedulerTask(int Taskid, string FromDate, string Todate, string EmpID, string ReportName, string CompanyID, string RptID,
             string CompanyName, string BranchName,string IsCompanyHead, string IsDepartmentHead, string IsEmployeeSendto,
             string LstEmpID, string[] selectedDep, string Triggerid, string Startdate, string Starttime
             , string Schedulermonth, string Schedulerdayon, string Schedulerdayonmonth, string Scheduleronday, string Scheduleronweek,
             string CustRptID, string CustReportName
             )
         {
             JsonResult result;
             SchedulerFasttrack obj = new SchedulerFasttrack();
             List<int> LstSltDepid = selectedDep.Select(i => int.Parse(i)).ToList();
             var _deplidt = "";
             for (int i = 0; i < LstSltDepid.Count; i++)
             {
                 if (_deplidt == "")
                 {
                     _deplidt = LstSltDepid[i].ToString();
                 }
                 else
                 {
                     _deplidt += "," + LstSltDepid[i].ToString();
                 }

             }

             obj.Taskid = Taskid;
             obj.FromDate = FromDate;
             obj.Todate = Todate;
             obj.EmpID = EmpID;
             obj.ReportName = ReportName;
             obj.CompanyID = CompanyID;
             obj.RptID = RptID;
             obj.CompanyName = CompanyName;
             obj.BranchName = BranchName;
             obj.IsCompanyHead = IsCompanyHead;
             obj.IsDepartmentHead = IsDepartmentHead;
             obj.IsEmployeeSendto = IsEmployeeSendto;
             obj.LstEmpID = LstEmpID;
             obj.selectedDep = _deplidt;
             obj.Triggerid = Triggerid;
             obj.Startdate = Startdate;
             obj.Starttime = Starttime;
             obj.Schedulermonth = Schedulermonth;
             obj.Schedulerdayon = Schedulerdayon;
             obj.Schedulerdayonmonth = Schedulerdayonmonth;
             obj.Scheduleronday = Scheduleronday;
             obj.Scheduleronweek = Scheduleronweek;

             obj.EntryBy = Convert.ToInt32(Session["CompanyId"]);
             if (CustRptID.ToString() == ",")
             {
                 obj.CustRptID = "";
             }
             else
             {
                 if (!string.IsNullOrEmpty(CustRptID))
                 {
                     obj.CustRptID = CustRptID;
                 }
             }
             if (CustReportName.ToString() == ",")
             {
                 obj.CustReportName = "";
             }
             else
             {
                 if (!string.IsNullOrEmpty(CustReportName))
                 {
                     obj.CustReportName = CustReportName;
                 }
             }
             /*For this  block are Check in Dublicate EmpId are Enter in
               Database Level So Single Values are Check and Enter in Database Level 
               this Issue are happend in Select all employee time.(19-07-2021)*/
             if (!string.IsNullOrEmpty(obj.EmpID))
             {
                 string[] str1;
                 str1 = obj.EmpID.Split(',');
                 var _EmpIdTest = "";
                 var hashSet = new HashSet<string>();
                 for (int i = 0; i < str1.Length; i++)
                 {
                     int No = 0;
                     if (!hashSet.Add(str1[i]))
                     {
                         No = 1;
                     }
                     if (_EmpIdTest == "")
                     {
                         _EmpIdTest = str1[i];
                     }
                     else
                     {
                         if (No == 0)
                         {
                             _EmpIdTest += "," + str1[i];
                         }
                         obj.EmpID = _EmpIdTest;
                     }
                 }
             }
             MRespo mres = RestClient.SavecreateTaskFasttrack(obj);
             if (mres.MegSts == "Ok")
             {
                 result = Json(new { message = "Scheduler added successfully.", MegSts = "Ok" });
             }
             else
             {
                 result = Json(new { message = "Scheduler on error.", MegSts = "error" });
             }

             result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
             return result;

         }
        #endregion



         #region For Check compresseDirectoryGridData
         public async Task<JsonResult> DirectoryGetCompressData(DirectoryEmployeeGetAllData objData)
         {
             JsonResult result;
             try
             {
                 DirectoryDetailsViewModel emp = new DirectoryDetailsViewModel();
                 var getEmpReporting = RestClient.DirectoryDataGridCompressData(objData);
                 result = Json(getEmpReporting);                 
                 result.MaxJsonLength = int.MaxValue;
                 string jsonData = JsonConvert.SerializeObject(getEmpReporting);
                 Console.WriteLine("Serialized JSON Data: " + jsonData);
                 byte[] compressedData = CompressionUtils.Compress(jsonData);
                 return Json(Convert.ToBase64String(compressedData), JsonRequestBehavior.AllowGet);
             }
             catch (Exception)
             {
                 result = Json(new SelectList("", "0"));
                 result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                 return result;
             }
         }
        #endregion

        [HttpGet]
        public ActionResult EmployeeHighLevelData()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EmployeeHighLevelDropDownData()
        {
            return View();
        }

        //#region For Attandance Correction Data Bind
        //[HttpPost]
        //public JsonResult GetAttendanceDetailsForEmployeeWise_Fastrack(string RoleID, string loginemployeeid, string Month, string Year, string status, string Employee, int isAttaDate, string Pre_attnDate)
        //{
        //    JsonResult result;
        //    string res = string.Empty;
        //    var data = "";
        //    AttanList obj = new AttanList();
        //    try
        //    {
        //        obj.RoleID = RoleID;
        //        if (RoleID == "6805")
        //        {
        //            obj.loginemployeeid = Session["ClientCompanyId"].ToString();
        //        }
        //        else if (RoleID == "6806")
        //        {
        //            obj.loginemployeeid = Session["BranchId"].ToString();
        //        }
        //        else
        //        {
        //            obj.loginemployeeid = loginemployeeid;
        //        }
        //        obj.Month = Month;
        //        obj.Year = Year;
        //        obj.status = status;
        //        obj.Employee = Employee;

        //        string jsonResponse = UtilitiesRestClient.GetAttendanceDetailsFastrack_NewMissPunch(obj);
        //        var attendanceList = JsonConvert.DeserializeObject<List<AttendanceRecord>>(jsonResponse);
        //        int loginEmpId;

        //        List<AttendanceRecord> filteredList = new List<AttendanceRecord>();

        //        if (int.TryParse(obj.loginemployeeid, out loginEmpId))
        //        {
        //            var dynamicDateFormat = Session["IsDateFormat"].ToString();
        //            var isDateformat = "";
        //            if (dynamicDateFormat == "dd-M-yyyy" || dynamicDateFormat == "M-dd-yyyy" || dynamicDateFormat == "yyyy-M-dd")
        //            {
        //                isDateformat = Session["IsDateFormat"].ToString().Replace("M", "MMM");
        //            }
        //            else
        //            {
        //                isDateformat = Session["IsDateFormat"].ToString().Replace("mm", "MM");
        //            }
        //            if (isAttaDate == 0)
        //            {
        //                filteredList = attendanceList
        //                   .Where(a => a.Att_dt == Pre_attnDate && a.InTime != null) // Corrected the date format to "MM"
        //                   .OrderByDescending(a => a.Att_dt)
        //                   .ToList();
        //            }
        //            else
        //            {
        //                filteredList = attendanceList
        //                .Where(a => a.OutTime != null).ToList();
        //            }
        //            string filteredJsonResponse = JsonConvert.SerializeObject(filteredList);
        //            result = Json(filteredJsonResponse);
        //            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //            result.MaxJsonLength = Int32.MaxValue;
        //        }
        //        else
        //        {
        //            result = Json(new { error = "Invalid loginemployeeid" });
        //            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GetDeviceDetails.ProcessLogLogFileWrite("GetAttendanceDetailsForEmployeeWise FastTrack", ex.Message);
        //        result = Json(new { success = false, message = "An error occurred while processing your request." });
        //        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //    }
        //    return result;
        //}

        //#endregion


        #region For AttendanceCorrectionUpdatereqFastrack

        [HttpPost]
        public JsonResult AttendanceCorrectionUpdatereqFastrack(List<AttendanceCorrectionreq> objCorrection)
        {
            JsonResult result;
            string res = string.Empty;
            var data = "";
            for (int i = 0; i < objCorrection.Count; i++)
            {
                AttendanceCorrection attndcrr = new AttendanceCorrection();
                attndcrr.AttCorrectionId = objCorrection[i].AttCorrectionId;
                attndcrr.EmpId = objCorrection[i].EmpId;
                attndcrr.attn_dt = objCorrection[i].attn_dt;
                attndcrr.InPunchTime = objCorrection[i].InPunchTime;
                attndcrr.OutPunchTime = objCorrection[i].OutPunchTime;
                attndcrr.ApplyReason = objCorrection[i].ApplyReason;
                attndcrr.ApprovalReason = string.IsNullOrEmpty(objCorrection[i].ApprovalReason) ? "" : objCorrection[i].ApprovalReason;

                attndcrr.Status = objCorrection[i].Status;
                attndcrr.ModifyDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
                attndcrr.ModifyBy = attndcrr.EmpId;
                attndcrr.PunchID = objCorrection[i].PunchID;
                data = ESSRestClient.AttendanceCorrectionUpdateFastrack(attndcrr);
            }
            //make Changes
            if (data == "OK")
            {
                res = "Ok";
                result = Json(new { ad = res.ToLower() });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            else
            {

                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        #region For Fastrack custom correction Page
        [HttpGet]
        public ActionResult AttendanceCorrectionDetails()
        {            
            return View();
        }
        #endregion
    }
}