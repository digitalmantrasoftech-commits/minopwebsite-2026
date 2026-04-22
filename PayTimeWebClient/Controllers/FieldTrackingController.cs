using System;
using System.Collections.Generic;
using System.Web.Mvc;
using PayTimeWebClient.Models;
using PayTimeWebClient.Helper;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Configuration;
using System.Web;


namespace PayTimeWebClient.Controllers
{
    public class FieldTrackingController : Controller
    {
        
        // GET: FieldTracking/WorkPolicy
        public ActionResult WorkPolicy()
        {
            // Simple view return - all data loading is done via AJAX
            if (Session["UserId"] == null)
            {
                return RedirectToAction("LoginPage", "PayTime");
            }

            return View();
        }

        public ActionResult ViewWorkPolicy()
        {
            // Simple view return - all data loading is done via AJAX
            if (Session["UserId"] == null)
            {
                return RedirectToAction("LoginPage", "PayTime");
            }

            return View();
        }

        public ActionResult WorkPolicyAllocation()
        {
            // Simple view return - all data loading is done via AJAX
            if (Session["UserId"] == null)
            {
                return RedirectToAction("LoginPage", "PayTime");
            }

            return View();
        }

        public ActionResult Clients()
        {
            // Simple view return - all data loading is done via AJAX
            if (Session["UserId"] == null)
            {
                return RedirectToAction("LoginPage", "PayTime");
            }

            return View();
        }

        public ActionResult TaskManagement()
        {
            // Simple view return - all data loading is done via AJAX
            if (Session["UserId"] == null)
            {
                return RedirectToAction("LoginPage", "PayTime");
            }

            return View();
        }

        public ActionResult EmployeeJourney()
        {
            // Simple view return - all data loading is done via AJAX
            if (Session["UserId"] == null)
            {
                return RedirectToAction("LoginPage", "PayTime");
            }

            return View();
        }

        public ActionResult Overview()
        {
            // Simple view return - all data loading is done via AJAX
            if (Session["UserId"] == null)
            {
                return RedirectToAction("LoginPage", "PayTime");
            }

            return View();
        }

        public ActionResult Reports()
        {
            // Simple view return - all data loading is done via AJAX
            if (Session["UserId"] == null)
            {
                return RedirectToAction("LoginPage", "PayTime");
            }

            return View();
        }

        public ActionResult EmployeeDetails()
        {
            // Simple view return - all data loading is done via AJAX
            if (Session["UserId"] == null)
            {
                return RedirectToAction("LoginPage", "PayTime");
            }

            return View();
        }

        // Upload task photos
        [HttpPost]
        public ActionResult UploadTaskPhotos(int taskId)
        {
            try
            {
                if (Request.Files.Count == 0)
                {
                    return Json(new { success = false, message = "No files uploaded" });
                }

                var uploadedFiles = new List<string>();
                string folderPath = Server.MapPath("~/assets/images/FieldTracking/Photos");

                // Ensure folder exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];

                    if (file != null && file.ContentLength > 0)
                    {
                        string extension = Path.GetExtension(file.FileName);
                        string timestamp = DateTime.Now.Ticks.ToString();
                        string fileName = $"Task_{taskId}_Photo_{i + 1}_{timestamp}{extension}";

                        string fullPath = Path.Combine(folderPath, fileName);
                        file.SaveAs(fullPath);

                        uploadedFiles.Add(fileName);
                    }
                }

                return Json(new { success = true, filenames = uploadedFiles });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading photos: " + ex.Message });
            }
        }

        // Upload task signature
        [HttpPost]
        public ActionResult UploadTaskSignature(int taskId)
        {
            try
            {
                if (Request.Files.Count == 0 || Request.Files[0] == null)
                {
                    return Json(new { success = false, message = "No signature file uploaded" });
                }

                var signatureFile = Request.Files[0];

                if (signatureFile.ContentLength == 0)
                {
                    return Json(new { success = false, message = "Signature file is empty" });
                }

                string folderPath = Server.MapPath("~/assets/images/FieldTracking/Signature");

                // Ensure folder exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string timestamp = DateTime.Now.Ticks.ToString();
                string fileName = $"Task_{taskId}_Signature_{timestamp}.png";

                string fullPath = Path.Combine(folderPath, fileName);
                signatureFile.SaveAs(fullPath);

                return Json(new { success = true, filename = fileName });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading signature: " + ex.Message });
            }
        }

        // Export tasks to Excel
        [HttpPost]
        public async Task<JsonResult> ExportTask(string fromDate, string toDate, string assignedToEmpId,
            string priority, string status, string searchTerm)
        {
            try
            {
                // Get the backend API URL from Web.config
                string apiBaseUrl = ConfigurationManager.AppSettings["webapifieldtracking"];

                // Build query parameters
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(fromDate))
                    queryParams.Add($"fromDate={Uri.EscapeDataString(fromDate)}");

                if (!string.IsNullOrEmpty(toDate))
                    queryParams.Add($"toDate={Uri.EscapeDataString(toDate)}");

                if (!string.IsNullOrEmpty(assignedToEmpId) && assignedToEmpId != "0")
                    queryParams.Add($"assignedToEmpId={assignedToEmpId}");

                if (!string.IsNullOrEmpty(priority))
                    queryParams.Add($"priority={Uri.EscapeDataString(priority)}");

                if (!string.IsNullOrEmpty(status))
                    queryParams.Add($"status={Uri.EscapeDataString(status)}");

                if (!string.IsNullOrEmpty(searchTerm))
                    queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");

                // Build the full API URL
                string queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                string apiUrl = $"{apiBaseUrl}taskmanagement/export{queryString}";

                // Call the backend API
                using (var httpClient = new HttpClient())
                {
                    // Add authorization token from session if available
                    if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["tokan"].ToString()))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", System.Web.HttpContext.Current.Session["tokan"].ToString());
                    }

                    var response = await httpClient.GetAsync(apiUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        return Json(new { success = false, message = "Failed to fetch data from backend API" });
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    // Check if data exists
                    if (apiResult.data == null)
                    {
                        return Json(new { success = false, message = "No data returned from API" });
                    }

                    // Convert JSON array to DataTable manually
                    var dataTable = new System.Data.DataTable();
                    var dataArray = apiResult.data;

                    // Define columns based on the task structure
                    //dataTable.Columns.Add("Task ID", typeof(int));
                    dataTable.Columns.Add("Task Code", typeof(string));
                    dataTable.Columns.Add("Task Title", typeof(string));
                    dataTable.Columns.Add("Task Description", typeof(string));
                    dataTable.Columns.Add("Priority", typeof(string));
                    dataTable.Columns.Add("Status", typeof(string));
                    dataTable.Columns.Add("Assigned To", typeof(string));
                    dataTable.Columns.Add("Employee Code", typeof(string));
                    dataTable.Columns.Add("Client Name", typeof(string));
                    dataTable.Columns.Add("Location", typeof(string));
                    dataTable.Columns.Add("Task Date", typeof(string));
                    dataTable.Columns.Add("Expected Duration", typeof(string));
                    dataTable.Columns.Add("Actual Duration", typeof(string));
                    dataTable.Columns.Add("Start Time", typeof(string));
                    dataTable.Columns.Add("End Time", typeof(string));

                    // Populate rows with task data
                    foreach (var task in dataArray)
                    {
                        var row = dataTable.NewRow();
                        //row["Task ID"] = task.taskId ?? 0;
                        row["Task Code"] = task.taskCode ?? "";
                        row["Task Title"] = task.taskTitle ?? "";
                        row["Task Description"] = task.taskDescription ?? "";
                        row["Priority"] = task.priority ?? "";
                        row["Status"] = task.status ?? "";
                        row["Assigned To"] = task.assignedToEmpName ?? "";
                        row["Employee Code"] = task.assignedToEmpCode ?? "";
                        row["Client Name"] = task.clientName ?? "";
                        row["Location"] = task.locationAddress ?? "";
                        row["Task Date"] = task.taskDate?.ToString();
                        row["Expected Duration"] = task.expectedDurationDisplay ?? "";
                        row["Actual Duration"] = task.actualDurationDisplay ?? "";
                        row["Start Time"] = ConvertUtcToIst(task.startTime?.ToString());
                        row["End Time"] = ConvertUtcToIst(task.endTime?.ToString());
                        dataTable.Rows.Add(row);
                    }

                    // Generate Excel file using ExportExcelFileMaster
                    var result = new ExportExcelFileMaster().SaveExcelToServer(dataTable, "TasksData", "TasksData_");

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        // Helper method to convert UTC to IST
        private string ConvertUtcToIst(string utcDateTimeString)
        {
            if (string.IsNullOrEmpty(utcDateTimeString) || utcDateTimeString == "null")
                return "";

            try
            {
                DateTime utcDateTime = DateTime.Parse(utcDateTimeString);
                TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime istDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, istZone);
                return istDateTime.ToString("dd-MMM-yyyy hh:mm tt");
            }
            catch
            {
                return utcDateTimeString;
            }
        }
    }
}