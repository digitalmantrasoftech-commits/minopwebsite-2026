using DocumentFormat.OpenXml.EMMA;
using Newtonsoft.Json;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using static PayTimeWebClient.Helper.AccountRestClint;
using PayTimeWebClient.Infrastructure;

namespace PayTimeWebClient.Controllers
{
    //[CustAuthFilter]
    public class TPController : Controller
    {
        static readonly IAccountRestClint RestAccout = new AccountRestClint();
        // GET: TP
        public ActionResult TPonboarding()
        {
            return View();
        }

        public ActionResult Voluntarydiscontinuation()
        {
            return View();
        }

        public ActionResult Exitflow()
        {
            return View();
        }

        public ActionResult TPApplicationForm()
        {
            return View("~/Views/TP/TPForm/TPApplicationForm.cshtml");
        }
        public ActionResult TPOnboardingForm()
        {
            return View("~/Views/TP/TPForm/TPOnboardingForm.cshtml");
        }
        public ActionResult TPDownloadDocument()
        {
            return View("~/Views/TP/TPForm/TPDownloadDocument.cshtml");
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult TPVerifyOTP([FromUri] string id, int form = 1)
        {
            var resp = RestAccout.GetEmailPassword();
            EmailCredentials creds = JsonConvert.DeserializeObject<EmailCredentials>(resp.Meg);
            LoginModel model = new LoginModel
            {
                CompanyName = "",                 
                CompanyCode = "",            
                UserEmail = creds.Email,
                Password = creds.Password.Replace("\0", ""),
                PlanId = 0,
                IsSchool = 0,
                IsOffice = 0,
                IsCart = 0
            };
            var response = RestAccout.Login(model);
            if (response.MegSts == "jwt_token")
            {
                Session["tokan"] = response.Meg;
            }
            ViewBag.TraineeEmail = id;
            ViewBag.FormFlag = form;
            return View();
        }

        [System.Web.Http.HttpPost]
        public JsonResult UploadEmployeeDocuments()
        {
            string aadharPath = "", aadharName = "";
            string panPath = "", panName = "";
            string photoPath = "", photoName = "";

            if (Request.Files.Count > 0)
            {
                foreach (string key in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[key];

                    if (file != null && file.ContentLength > 0)
                    {
                        if (file.ContentLength > 2 * 1024 * 1024)
                        {
                            return Json(key + " file size must not exceed 2 MB");
                        }
                        string originalFileName = Path.GetFileName(file.FileName);
                        string extension = Path.GetExtension(originalFileName).ToLower();

                        if (extension != ".jpg" && extension != ".jpeg" &&
                            extension != ".png" && extension != ".pdf")
                        {
                            return Json("Invalid file format");
                        }

                        string folderPath = ""; 

                        if (key == "AadharFile")
                            folderPath = "~/UploadTPDoc/Aadhar";
                        else if (key == "PanFile")
                            folderPath = "~/UploadTPDoc/PAN";
                        else if (key == "PhotoFile")
                            folderPath = "~/UploadTPDoc/Photo";
                        else
                            continue; 

                        string serverPath = Server.MapPath(folderPath);
                        if (!Directory.Exists(serverPath))
                        {
                            Directory.CreateDirectory(serverPath);
                        }

                        string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string newFileName = key + "_" + timeStamp + extension;
                        string fullPath = Path.Combine(serverPath, newFileName);

                        file.SaveAs(fullPath);

                        string relativePath = folderPath.Replace("~", "") + "/" + newFileName;

                        if (key == "AadharFile")
                        {
                            aadharPath = relativePath;
                            aadharName = originalFileName;
                        }
                        else if (key == "PanFile")
                        {
                            panPath = relativePath;
                            panName = originalFileName;
                        }
                        else if (key == "PhotoFile")
                        {
                            photoPath = relativePath;
                            photoName = originalFileName;
                        }
                    }
                }
                var result = new
                {
                    AadharPath = aadharPath,
                    AadharName = aadharName,
                    PanPath = panPath,
                    PanName = panName,
                    PhotoPath = photoPath,
                    PhotoName = photoName
                };

                return Json(result);
            }

            return Json("No files uploaded");
        }

        public ActionResult TPmedicalform()
        {
            return View();
        }
        [System.Web.Http.HttpPost]
        public JsonResult MedicalDocuments()
        {
            var uploadedFiles = new List<object>();

            if (Request.Files.Count > 0)
            {
                string folderPath = "~/UploadTPDoc/Medical";
                string serverPath = Server.MapPath(folderPath);

                if (!Directory.Exists(serverPath))
                    Directory.CreateDirectory(serverPath);
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        string originalFileName = Path.GetFileName(file.FileName);
                        string extension = Path.GetExtension(originalFileName).ToLower();

                        if (extension != ".jpg" && extension != ".jpeg" &&
                            extension != ".png" && extension != ".pdf" && extension != ".zip")
                        {
                            return Json("Invalid file format: " + originalFileName);
                        }

                        string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        string newFileName =
                            Path.GetFileNameWithoutExtension(originalFileName)
                            + "_" + timeStamp + extension;

                        string fullPath = Path.Combine(serverPath, newFileName);

                        file.SaveAs(fullPath);

                        string relativePath = folderPath.Replace("~", "") + "/" + newFileName;

                        uploadedFiles.Add(new
                        {
                            OriginalName = originalFileName,
                            SavedPath = relativePath
                        });
                    }
                }

                return Json(new
                {
                    UploadedFiles = uploadedFiles
                });
            }

            return Json("No files uploaded");
        }


        private int GetUploadedFileCount()
        {
            // DB count logic
            return 0;
        }
        [System.Web.Http.HttpPost]
        public JsonResult UploadTPSDIDocument()
        {
            if (Request.Files.Count == 0)
                return Json("No file uploaded");

            if (Request.Files.Count > 1)
                return Json("Only one file is allowed");

            HttpPostedFileBase file = Request.Files[0];

            if (file.ContentLength > 2 * 1024 * 1024)
                return Json("File size must be less than 2 MB");

            string originalFileName = Path.GetFileName(file.FileName);
            string extension = Path.GetExtension(originalFileName).ToLower();

            if (extension != ".jpg" && extension != ".jpeg" &&
                extension != ".png" && extension != ".pdf")
            {
                return Json("Invalid file format");
            }

            string folderPath = "~/UploadTPDoc/TPSDI";
            string serverPath = Server.MapPath(folderPath);

            if (!Directory.Exists(serverPath))
                Directory.CreateDirectory(serverPath);

            originalFileName = originalFileName.Replace(" ", "_");

            string fullPath = Path.Combine(serverPath, originalFileName);

            file.SaveAs(fullPath);

            return Json(new
            {
                FilePath = folderPath.Replace("~", "") + "/" + originalFileName,
                FileName = originalFileName
            });
        }

        [System.Web.Http.HttpPost]
        public JsonResult UploadEducationDocuments()
        {
            var uploadedDocs = new List<object>();
            string trackId = Request.Form["TrackId"];

            if (string.IsNullOrEmpty(trackId))
            {
                return Json(new { success = false, message = "TrackId is missing" });
            }

            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    string key = Request.Files.AllKeys[i];

                    if (file != null && file.ContentLength > 0)
                    {
                        if (file.ContentLength > 2 * 1024 * 1024)
                        {
                            return Json(new { success = false, message = file.FileName + " exceeds 2 MB" });
                        }

                        string originalFileName = Path.GetFileName(file.FileName);
                        string extension = Path.GetExtension(originalFileName).ToLower();
                        string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf", ".zip" };

                        if (!allowedExtensions.Contains(extension))
                        {
                            return Json(new { success = false, message = "Invalid format for " + originalFileName });
                        }
                        string folderPath = "~/UploadTPDoc/EducationDoc";
                        string serverPath = Server.MapPath(folderPath);

                        if (!Directory.Exists(serverPath))
                        {
                            Directory.CreateDirectory(serverPath);
                        }
                        string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        string newFileName = trackId + "_" + i + "_" + timeStamp + extension;
                        string fullPath = Path.Combine(serverPath, newFileName);

                        file.SaveAs(fullPath);

                        string relativePath = folderPath.Replace("~", "") + "/" + newFileName;

                        string docType = Request.Form["EduTypes[" + i + "]"] ?? "General";

                        uploadedDocs.Add(new
                        {
                            FileName = originalFileName,
                            FilePath = relativePath,
                            DocumentType = docType,
                            TrackId = trackId
                        });
                    }
                }
                return Json(new { success = true, Data = uploadedDocs });
            }

            return Json(new { success = false, message = "No files found" });
        }

        [System.Web.Http.HttpPost]
        public JsonResult UploadTrainneLetter()
        {
            if (Request.Files.Count == 0)
                return Json("No file uploaded");

            if (Request.Files.Count > 1)
                return Json("Only one file is allowed");

            HttpPostedFileBase file = Request.Files[0];

            if (file.ContentLength > 2 * 1024 * 1024)
                return Json("File size must be less than 2 MB");

            string originalFileName = Path.GetFileName(file.FileName);
            string extension = Path.GetExtension(originalFileName).ToLower();

            if (extension != ".jpg" && extension != ".jpeg" &&
                extension != ".png" && extension != ".pdf")
            {
                return Json("Invalid file format");
            }

            string folderPath = "~/UploadTPDoc/TrainneLetter";
            string serverPath = Server.MapPath(folderPath);

            if (!Directory.Exists(serverPath))
                Directory.CreateDirectory(serverPath);

            string fileNameOnly = Path.GetFileNameWithoutExtension(originalFileName).Replace(" ", "_");
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string newFileName = $"{fileNameOnly}_{timestamp}{extension}";
            string fullPath = Path.Combine(serverPath, newFileName);
            file.SaveAs(fullPath);

            return Json(new
            {
                FilePath = folderPath.Replace("~", "") + "/" + newFileName,
                FileName = newFileName
            });
        }

        [System.Web.Http.HttpPost]
        public JsonResult UploadTraineeDocuments()
        {
            if (Request.Files.Count == 0)
                return Json(new { status = false, message = "No files uploaded" });

            string letterPath = "", letterName = "";
            string signPath = "", signName = "";

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                string keyName = Request.Files.AllKeys[i];

                // Basic Validation
                if (file.ContentLength > 2 * 1024 * 1024)
                    return Json(new { status = false, message = "File size must be less than 2 MB" });

                string originalFileName = Path.GetFileName(file.FileName);
                string extension = Path.GetExtension(originalFileName).ToLower();

                string folderPath = (keyName == "SignatureFile") ? "~/UploadTPDoc/HRSign" : "~/UploadTPDoc/TrainneLetter";
                string serverPath = Server.MapPath(folderPath);

                if (!Directory.Exists(serverPath))
                    Directory.CreateDirectory(serverPath);

                string fileNameOnly = Path.GetFileNameWithoutExtension(originalFileName).Replace(" ", "_");
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string newFileName = $"{fileNameOnly}_{timestamp}{extension}";

                file.SaveAs(Path.Combine(serverPath, newFileName));

                if (keyName == "SignatureFile")
                {
                    signPath = folderPath.Replace("~", "") + "/" + newFileName;
                    signName = newFileName;
                }
                else
                {
                    letterPath = folderPath.Replace("~", "") + "/" + newFileName;
                    letterName = newFileName;
                }
            }

            return Json(new
            {
                status = true,
                LetterPath = letterPath,
                LetterName = letterName,
                SignaturePath = signPath,
                SignatureName = signName
            });
        }

        [System.Web.Http.HttpPost]
        public JsonResult UploadBankDocument()
        {
            if (Request.Files.Count == 0)
                return Json("No file uploaded");

            if (Request.Files.Count > 1)
                return Json("Only one file is allowed");

            HttpPostedFileBase file = Request.Files[0];

            if (file.ContentLength > 2 * 1024 * 1024)
                return Json("File size must be less than 2 MB");

            string originalFileName = Path.GetFileName(file.FileName);
            string extension = Path.GetExtension(originalFileName).ToLower();

            if (extension != ".jpg" && extension != ".jpeg" &&
                extension != ".png" && extension != ".pdf")
            {
                return Json("Invalid file format");
            }
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string originalNameOnly = Path.GetFileNameWithoutExtension(file.FileName).Replace(" ", "_");
            string newFileName = $"{originalNameOnly}_{timestamp}{extension}";

            string folderPath = "~/UploadTPDoc/PassBook";
            string serverPath = Server.MapPath(folderPath);

            if (!Directory.Exists(serverPath))
                Directory.CreateDirectory(serverPath);

            //originalFileName = originalFileName.Replace(" ", "_");

            string fullPath = Path.Combine(serverPath, newFileName);
            file.SaveAs(fullPath);

            return Json(new
            {
                FilePath = folderPath + "/" + newFileName,
                FileName = newFileName
            });
        }

        public ActionResult PreviousRewards()
        {
            return View();
        }

        public ActionResult RewardRequest()
        {
            return View();
        }

        public ActionResult HSEview()
        {
            return View();
        }

        public ActionResult ManagerApproval()
        {
            return View();
        }

        public ActionResult Headapproval()
        {
            return View();
        }
        public ActionResult BHRapproval()
        {
            return View();
        }

    }
}