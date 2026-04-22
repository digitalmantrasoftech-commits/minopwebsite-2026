using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;

namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class cmsController : Controller
    {
        // GET: cms
        public ActionResult canteens()
        {
            return View();
        }

        public ActionResult mealslot()
        {
            return View();
        }

        public ActionResult policy()
        {
            return View();
        }

        public ActionResult allocation()
        {
            return View();
        }

        public ActionResult meal()
        {
            return View();
        }

        public ActionResult category()
        {
            return View();
        }

        public ActionResult canteenmenu()
        {
            return View();
        }

        public ActionResult dailymenu()
        {
            return View();
        }

        public ActionResult alwaysavailable()
        {
            return View();
        }

        public ActionResult templates()
        {
            return View();
        }

        public ActionResult orders()
        {
            return View();
        }

        public ActionResult report()
        {
            return View();
        }

        public ActionResult guests()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ExportCanteensExcel(CmsCanteenExportRequest request)
        {
            try
            {
                string token = Session["tokan"]?.ToString();
                string cmsApiUrl = ConfigurationManager.AppSettings["webapicms"]?.ToString();

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(cmsApiUrl))
                {
                    return Json(new { success = false, message = "Session expired or API not configured." });
                }

                // Build query params for the CMS canteens export API
                var queryParams = new List<string>();

                if (request != null)
                {
                    if (!string.IsNullOrEmpty(request.searchTerm))
                        queryParams.Add("searchTerm=" + Uri.EscapeDataString(request.searchTerm));
                    if (!string.IsNullOrEmpty(request.isActive))
                        queryParams.Add("isActive=" + request.isActive);
                }

                string url = cmsApiUrl + "canteens/export" + (queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "");

                // Call CMS backend API
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.Timeout = TimeSpan.FromMinutes(5);

                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return Json(new { success = false, message = "Failed to fetch canteen data. Status: " + response.StatusCode });
                    }

                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    JObject parsed = JObject.Parse(jsonResponse);
                    JArray canteens = parsed["canteens"] as JArray;

                    if (canteens == null || canteens.Count == 0)
                    {
                        return Json(new { success = false, message = "No data available for export." });
                    }

                    // Build DataTable for Excel generation
                    DataTable dt = new DataTable("Canteens");
                    dt.Columns.Add("Canteen Name", typeof(string));
                    dt.Columns.Add("Opening Time", typeof(string));
                    dt.Columns.Add("Closing Time", typeof(string));
                    dt.Columns.Add("Head Email", typeof(string));
                    dt.Columns.Add("Status", typeof(string));

                    foreach (var item in canteens)
                    {
                        string openTime = item["openingTime"]?.ToString();
                        string closeTime = item["closingTime"]?.ToString();

                        dt.Rows.Add(
                            item["canteenName"]?.ToString() ?? "",
                            !string.IsNullOrEmpty(openTime) && openTime.Length >= 5 ? openTime.Substring(0, 5) : "",
                            !string.IsNullOrEmpty(closeTime) && closeTime.Length >= 5 ? closeTime.Substring(0, 5) : "",
                            item["canteenHeadEmail"]?.ToString() ?? "-",
                            item["isActive"]?.ToObject<bool>() == true ? "Active" : "Inactive"
                        );
                    }

                    var result = new ExportExcelFileMaster().SaveExcelToServer(dt, "CanteensData", "CanteensData_");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ExportPoliciesExcel(CmsPolicyExportRequest request)
        {
            try
            {
                string token = Session["tokan"]?.ToString();
                string cmsApiUrl = ConfigurationManager.AppSettings["webapicms"]?.ToString();

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(cmsApiUrl))
                {
                    return Json(new { success = false, message = "Session expired or API not configured." });
                }

                var queryParams = new List<string>();

                if (request != null)
                {
                    if (!string.IsNullOrEmpty(request.companyId))
                        queryParams.Add("companyId=" + Uri.EscapeDataString(request.companyId));
                    if (!string.IsNullOrEmpty(request.searchTerm))
                        queryParams.Add("searchTerm=" + Uri.EscapeDataString(request.searchTerm));
                    if (!string.IsNullOrEmpty(request.isActive))
                        queryParams.Add("isActive=" + request.isActive);
                }

                string url = cmsApiUrl + "policies/export" + (queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.Timeout = TimeSpan.FromMinutes(5);

                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return Json(new { success = false, message = "Failed to fetch policy data. Status: " + response.StatusCode });
                    }

                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    JObject parsed = JObject.Parse(jsonResponse);
                    JArray policies = parsed["policies"] as JArray;

                    if (policies == null || policies.Count == 0)
                    {
                        return Json(new { success = false, message = "No data available for export." });
                    }

                    DataTable dt = new DataTable("Policies");
                    dt.Columns.Add("Policy Name", typeof(string));
                    dt.Columns.Add("Breakfast", typeof(string));
                    dt.Columns.Add("Lunch", typeof(string));
                    dt.Columns.Add("Snacks", typeof(string));
                    dt.Columns.Add("Dinner", typeof(string));
                    dt.Columns.Add("Monthly Limit", typeof(string));
                    dt.Columns.Add("Warning", typeof(string));
                    dt.Columns.Add("Status", typeof(string));

                    var slotCategoryNames = new Dictionary<int, string>
                    {
                        { 1, "Breakfast" },
                        { 2, "Lunch" },
                        { 3, "Snacks" },
                        { 4, "Dinner" }
                    };

                    foreach (var item in policies)
                    {
                        var slotDetails = item["slotDetails"] as JArray;
                        var slotDisplays = new Dictionary<int, string>();

                        if (slotDetails != null)
                        {
                            foreach (var detail in slotDetails)
                            {
                                int slotCategoryId = detail["slotCategoryId"]?.ToObject<int>() ?? 0;
                                string display = detail["subsidyDisplay"]?.ToString() ?? "-";
                                slotDisplays[slotCategoryId] = display;
                            }
                        }

                        dt.Rows.Add(
                            item["policyName"]?.ToString() ?? "",
                            slotDisplays.ContainsKey(1) ? slotDisplays[1] : "-",
                            slotDisplays.ContainsKey(2) ? slotDisplays[2] : "-",
                            slotDisplays.ContainsKey(3) ? slotDisplays[3] : "-",
                            slotDisplays.ContainsKey(4) ? slotDisplays[4] : "-",
                            item["monthlyLimitDisplay"]?.ToString() ?? "-",
                            item["warningDisplay"]?.ToString() ?? "-",
                            item["isActive"]?.ToObject<bool>() == true ? "Active" : "Inactive"
                        );
                    }

                    var result = new ExportExcelFileMaster().SaveExcelToServer(dt, "PoliciesData", "PoliciesData_");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ExportAllocationsExcel(CmsAllocationExportRequest request)
        {
            try
            {
                string token = Session["tokan"]?.ToString();
                string cmsApiUrl = ConfigurationManager.AppSettings["webapicms"]?.ToString();

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(cmsApiUrl))
                {
                    return Json(new { success = false, message = "Session expired or API not configured." });
                }

                var queryParams = new List<string>();

                if (request != null)
                {
                    if (!string.IsNullOrEmpty(request.companyId))
                        queryParams.Add("companyId=" + Uri.EscapeDataString(request.companyId));
                    if (!string.IsNullOrEmpty(request.branchId))
                        queryParams.Add("branchId=" + Uri.EscapeDataString(request.branchId));
                    if (!string.IsNullOrEmpty(request.departmentId))
                        queryParams.Add("departmentId=" + Uri.EscapeDataString(request.departmentId));
                    if (!string.IsNullOrEmpty(request.policyId))
                        queryParams.Add("policyId=" + Uri.EscapeDataString(request.policyId));
                    if (!string.IsNullOrEmpty(request.searchTerm))
                        queryParams.Add("searchTerm=" + Uri.EscapeDataString(request.searchTerm));
                }

                string url = cmsApiUrl + "allocations/export" + (queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.Timeout = TimeSpan.FromMinutes(5);

                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return Json(new { success = false, message = "Failed to fetch allocation data. Status: " + response.StatusCode });
                    }

                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    JObject parsed = JObject.Parse(jsonResponse);
                    JArray allocations = parsed["allocations"] as JArray;

                    if (allocations == null || allocations.Count == 0)
                    {
                        return Json(new { success = false, message = "No data available for export." });
                    }

                    DataTable dt = new DataTable("Allocations");
                    dt.Columns.Add("Employee Name", typeof(string));
                    dt.Columns.Add("Employee Code", typeof(string));
                    dt.Columns.Add("Department", typeof(string));
                    dt.Columns.Add("Policy Name", typeof(string));
                    dt.Columns.Add("Canteens", typeof(string));
                    dt.Columns.Add("Breakfast", typeof(string));
                    dt.Columns.Add("Lunch", typeof(string));
                    dt.Columns.Add("Snacks", typeof(string));
                    dt.Columns.Add("Dinner", typeof(string));
                    dt.Columns.Add("Monthly Limit", typeof(string));

                    foreach (var item in allocations)
                    {
                        var canteens = item["canteens"] as JArray;
                        string canteenNames = "-";
                        if (canteens != null && canteens.Count > 0)
                        {
                            canteenNames = string.Join(", ", canteens.Select(c => c["canteenName"]?.ToString() ?? ""));
                        }

                        dt.Rows.Add(
                            item["empName"]?.ToString() ?? "",
                            item["empCode"]?.ToString() ?? "",
                            item["departmentName"]?.ToString() ?? "-",
                            item["policyName"]?.ToString() ?? "Not Assigned",
                            canteenNames,
                            item["breakfast"]?.ToString() ?? "-",
                            item["lunch"]?.ToString() ?? "-",
                            item["snacks"]?.ToString() ?? "-",
                            item["dinner"]?.ToString() ?? "-",
                            item["monthlyLimit"]?.ToString() ?? "-"
                        );
                    }

                    var result = new ExportExcelFileMaster().SaveExcelToServer(dt, "AllocationsData", "AllocationsData_");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ExportMealsExcel(CmsMealExportRequest request)
        {
            try
            {
                string token = Session["tokan"]?.ToString();
                string cmsApiUrl = ConfigurationManager.AppSettings["webapicms"]?.ToString();

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(cmsApiUrl))
                {
                    return Json(new { success = false, message = "Session expired or API not configured." });
                }

                var queryParams = new List<string>();

                if (request != null)
                {
                    if (!string.IsNullOrEmpty(request.canteenId))
                        queryParams.Add("canteenId=" + Uri.EscapeDataString(request.canteenId));
                    if (!string.IsNullOrEmpty(request.categoryId))
                        queryParams.Add("categoryId=" + Uri.EscapeDataString(request.categoryId));
                    if (!string.IsNullOrEmpty(request.dietaryType))
                        queryParams.Add("dietaryType=" + Uri.EscapeDataString(request.dietaryType));
                    if (!string.IsNullOrEmpty(request.isActive))
                        queryParams.Add("isActive=" + request.isActive);
                    if (!string.IsNullOrEmpty(request.searchTerm))
                        queryParams.Add("searchTerm=" + Uri.EscapeDataString(request.searchTerm));
                }

                string url = cmsApiUrl + "meals/export" + (queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.Timeout = TimeSpan.FromMinutes(5);

                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return Json(new { success = false, message = "Failed to fetch meal data. Status: " + response.StatusCode });
                    }

                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    JObject parsed = JObject.Parse(jsonResponse);
                    JArray meals = parsed["meals"] as JArray;

                    if (meals == null || meals.Count == 0)
                    {
                        return Json(new { success = false, message = "No data available for export." });
                    }

                    DataTable dt = new DataTable("Meals");
                    dt.Columns.Add("Meal Name", typeof(string));
                    dt.Columns.Add("Description", typeof(string));
                    dt.Columns.Add("Category", typeof(string));
                    dt.Columns.Add("Dietary Type", typeof(string));
                    dt.Columns.Add("Jain Friendly", typeof(string));
                    dt.Columns.Add("Price", typeof(string));
                    dt.Columns.Add("Tax %", typeof(string));
                    dt.Columns.Add("Final Price", typeof(string));
                    dt.Columns.Add("Status", typeof(string));

                    foreach (var item in meals)
                    {
                        dt.Rows.Add(
                            item["mealName"]?.ToString() ?? "",
                            item["mealDescription"]?.ToString() ?? "",
                            item["categoryName"]?.ToString() ?? "-",
                            item["dietaryTypeDisplay"]?.ToString() ?? "-",
                            item["isJainFriendly"]?.ToObject<bool>() == true ? "Yes" : "No",
                            item["price"]?.ToString() ?? "0",
                            item["taxPercentage"]?.ToString() ?? "0",
                            item["finalPrice"]?.ToString() ?? "0",
                            item["isActive"]?.ToObject<bool>() == true ? "Active" : "Inactive"
                        );
                    }

                    var result = new ExportExcelFileMaster().SaveExcelToServer(dt, "MealsData", "MealsData_");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ExportReportExcel(CmsReportExportRequest request)
        {
            try
            {
                string token = Session["tokan"]?.ToString();
                string cmsApiUrl = ConfigurationManager.AppSettings["webapicms"]?.ToString();

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(cmsApiUrl))
                {
                    return Json(new { success = false, message = "Session expired or API not configured." });
                }

                var queryParams = new List<string>();

                if (request != null)
                {
                    if (!string.IsNullOrEmpty(request.fromDate))
                        queryParams.Add("fromDate=" + Uri.EscapeDataString(request.fromDate));
                    if (!string.IsNullOrEmpty(request.toDate))
                        queryParams.Add("toDate=" + Uri.EscapeDataString(request.toDate));
                    if (!string.IsNullOrEmpty(request.orderType))
                        queryParams.Add("orderType=" + request.orderType);
                    if (!string.IsNullOrEmpty(request.canteenId))
                        queryParams.Add("canteenId=" + request.canteenId);
                    if (!string.IsNullOrEmpty(request.companyId))
                        queryParams.Add("companyId=" + request.companyId);

                    // Multi-select branch IDs
                    if (!string.IsNullOrEmpty(request.branchIds))
                    {
                        foreach (var id in request.branchIds.Split(','))
                        {
                            if (!string.IsNullOrWhiteSpace(id))
                                queryParams.Add("branchIds=" + id.Trim());
                        }
                    }

                    // Multi-select department IDs
                    if (!string.IsNullOrEmpty(request.departmentIds))
                    {
                        foreach (var id in request.departmentIds.Split(','))
                        {
                            if (!string.IsNullOrWhiteSpace(id))
                                queryParams.Add("departmentIds=" + id.Trim());
                        }
                    }

                    // Search text
                    if (!string.IsNullOrEmpty(request.search))
                        queryParams.Add("search=" + Uri.EscapeDataString(request.search));
                }

                string url = cmsApiUrl + "orders/export" + (queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.Timeout = TimeSpan.FromMinutes(5);

                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return Json(new { success = false, message = "Failed to fetch report data. Status: " + response.StatusCode });
                    }

                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    JObject parsed = JObject.Parse(jsonResponse);
                    JArray employees = parsed["employees"] as JArray;

                    if (employees == null || employees.Count == 0)
                    {
                        return Json(new { success = false, message = "No data available for export." });
                    }

                    int orderType = 1;
                    if (request != null && !string.IsNullOrEmpty(request.orderType))
                        int.TryParse(request.orderType, out orderType);

                    DataTable dt;

                    if (orderType == 2)
                    {
                        // Guest report
                        dt = new DataTable("GuestReport");
                        dt.Columns.Add("Sr No", typeof(int));
                        dt.Columns.Add("Guest Name", typeof(string));
                        dt.Columns.Add("Host Employee", typeof(string));
                        dt.Columns.Add("Host Emp Code", typeof(string));
                        dt.Columns.Add("Canteen", typeof(string));
                        dt.Columns.Add("Total Orders", typeof(int));
                        dt.Columns.Add("Gross Amount", typeof(decimal));
                        dt.Columns.Add("Net Amount", typeof(decimal));

                        int srNo = 1;
                        foreach (var item in employees)
                        {
                            dt.Rows.Add(
                                srNo++,
                                item["guestName"]?.ToString() ?? "",
                                item["hostEmpName"]?.ToString() ?? "",
                                item["hostEmpCode"]?.ToString() ?? "",
                                item["canteenName"]?.ToString() ?? "",
                                item["totalOrders"]?.ToObject<int>() ?? 0,
                                item["grossAmount"]?.ToObject<decimal>() ?? 0,
                                item["netAmount"]?.ToObject<decimal>() ?? 0
                            );
                        }
                    }
                    else
                    {
                        // Employee report
                        dt = new DataTable("EmployeeReport");
                        dt.Columns.Add("Sr No", typeof(int));
                        dt.Columns.Add("Emp Code", typeof(string));
                        dt.Columns.Add("Employee Name", typeof(string));
                        dt.Columns.Add("Department", typeof(string));
                        dt.Columns.Add("Branch", typeof(string));
                        dt.Columns.Add("Canteen", typeof(string));
                        dt.Columns.Add("Policy", typeof(string));
                        dt.Columns.Add("Monthly Limit", typeof(string));
                        dt.Columns.Add("Total Orders", typeof(int));
                        dt.Columns.Add("Gross Amount", typeof(decimal));
                        dt.Columns.Add("Subsidy Amount", typeof(decimal));
                        dt.Columns.Add("Net Amount (Deduction)", typeof(decimal));
                        dt.Columns.Add("Usage %", typeof(string));

                        int srNo = 1;
                        foreach (var item in employees)
                        {
                            var monthlyLimit = item["monthlyLimit"];
                            string limitStr = (monthlyLimit == null || monthlyLimit.Type == JTokenType.Null) ? "Unlimited" : monthlyLimit.ToObject<decimal>().ToString("N2");

                            var usagePct = item["usagePercentage"];
                            string usageStr = (usagePct == null || usagePct.Type == JTokenType.Null) ? "-" : usagePct.ToObject<decimal>().ToString("F1") + "%";

                            dt.Rows.Add(
                                srNo++,
                                item["empCode"]?.ToString() ?? "",
                                item["empName"]?.ToString() ?? "",
                                item["departmentName"]?.ToString() ?? "",
                                item["branchName"]?.ToString() ?? "",
                                item["canteenName"]?.ToString() ?? "",
                                item["policyName"]?.ToString() ?? "",
                                limitStr,
                                item["totalOrders"]?.ToObject<int>() ?? 0,
                                item["grossAmount"]?.ToObject<decimal>() ?? 0,
                                item["subsidyAmount"]?.ToObject<decimal>() ?? 0,
                                item["netAmount"]?.ToObject<decimal>() ?? 0,
                                usageStr
                            );
                        }
                    }

                    string filePrefix = orderType == 2 ? "GuestReport_" : "SalaryDeductionReport_";
                    var result = new ExportExcelFileMaster().SaveExcelToServer(dt, "ReportData", filePrefix);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UploadMealImage(HttpPostedFileBase MealImageFile)
        {
            try
            {
                if (MealImageFile == null || MealImageFile.ContentLength == 0)
                {
                    return new HttpStatusCodeResult(400, "No file uploaded.");
                }

                // Validate file size (max 1 MB)
                if (MealImageFile.ContentLength > 1024 * 1024)
                {
                    return Json(new { success = false, message = "File size must be less than 1 MB." });
                }

                // Validate file extension
                string extension = Path.GetExtension(MealImageFile.FileName).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp" };
                if (!allowedExtensions.Contains(extension))
                {
                    return Json(new { success = false, message = "Only .jpg, .jpeg, .png, .bmp files are allowed." });
                }

                // Generate unique filename - sanitize to remove special characters
                string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
                string originalName = Path.GetFileNameWithoutExtension(MealImageFile.FileName);
                originalName = System.Text.RegularExpressions.Regex.Replace(originalName, @"[^a-zA-Z0-9_\-]", "");
                if (string.IsNullOrEmpty(originalName)) originalName = "meal";
                if (originalName.Length > 20)
                {
                    originalName = originalName.Substring(0, 20);
                }
                string fileName = originalName + "_" + uniqueId + extension;

                // Save to ~/assets/MealImages folder
                string folderPath = Server.MapPath("~/assets/MealImages");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fullPath = Path.Combine(folderPath, fileName);
                MealImageFile.SaveAs(fullPath);

                return Json(new { success = true, imagePath = "/assets/MealImages/" + fileName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ExportMyOrdersExcel(CmsMyOrderExportRequest request)
        {
            try
            {
                string token = Session["tokan"]?.ToString();
                string cmsApiUrl = ConfigurationManager.AppSettings["webapicms"]?.ToString();

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(cmsApiUrl))
                {
                    return Json(new { success = false, message = "Session expired or API not configured." });
                }

                var queryParams = new List<string>();

                if (request != null)
                {
                    if (!string.IsNullOrEmpty(request.fromDate))
                        queryParams.Add("fromDate=" + Uri.EscapeDataString(request.fromDate));
                    if (!string.IsNullOrEmpty(request.toDate))
                        queryParams.Add("toDate=" + Uri.EscapeDataString(request.toDate));
                    if (!string.IsNullOrEmpty(request.status))
                        queryParams.Add("status=" + Uri.EscapeDataString(request.status));
                }

                string url = cmsApiUrl + "orders/my/export" + (queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.Timeout = TimeSpan.FromMinutes(5);

                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return Json(new { success = false, message = "Failed to fetch orders data. Status: " + response.StatusCode });
                    }

                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    JObject parsed = JObject.Parse(jsonResponse);
                    JArray orders = parsed["orders"] as JArray;

                    if (orders == null || orders.Count == 0)
                    {
                        return Json(new { success = false, message = "No data available for export." });
                    }

                    DataTable dt = new DataTable("MyOrders");
                    dt.Columns.Add("Order Code", typeof(string));
                    dt.Columns.Add("Date", typeof(string));
                    dt.Columns.Add("Canteen", typeof(string));
                    dt.Columns.Add("Slot", typeof(string));
                    dt.Columns.Add("Items", typeof(string));
                    dt.Columns.Add("Gross Amount", typeof(string));
                    dt.Columns.Add("Subsidy", typeof(string));
                    dt.Columns.Add("Net Amount", typeof(string));
                    dt.Columns.Add("Status", typeof(string));

                    foreach (var item in orders)
                    {
                        string orderDate = item["orderDate"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(orderDate) && orderDate.Length >= 10)
                        {
                            orderDate = orderDate.Substring(0, 10);
                        }

                        dt.Rows.Add(
                            item["orderCode"]?.ToString() ?? "",
                            orderDate,
                            item["canteenName"]?.ToString() ?? "-",
                            item["slotName"]?.ToString() ?? "-",
                            item["itemSummary"]?.ToString() ?? "-",
                            item["grossAmount"]?.ToString() ?? "0",
                            item["subsidyAmount"]?.ToString() ?? "0",
                            item["netAmount"]?.ToString() ?? "0",
                            item["orderStatusDisplay"]?.ToString() ?? "-"
                        );
                    }

                    var result = new ExportExcelFileMaster().SaveExcelToServer(dt, "MyOrdersData", "MyOrdersData_");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }

    public class CmsMyOrderExportRequest
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string status { get; set; }
    }

    public class CmsMealExportRequest
    {
        public string canteenId { get; set; }
        public string categoryId { get; set; }
        public string dietaryType { get; set; }
        public string isActive { get; set; }
        public string searchTerm { get; set; }
    }

    public class CmsAllocationExportRequest
    {
        public string companyId { get; set; }
        public string branchId { get; set; }
        public string departmentId { get; set; }
        public string policyId { get; set; }
        public string searchTerm { get; set; }
    }

    public class CmsPolicyExportRequest
    {
        public string companyId { get; set; }
        public string searchTerm { get; set; }
        public string isActive { get; set; }
    }

    public class CmsCanteenExportRequest
    {
        public string searchTerm { get; set; }
        public string isActive { get; set; }
    }

    public class CmsReportExportRequest
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string orderType { get; set; }
        public string canteenId { get; set; }
        public string companyId { get; set; }
        public string branchIds { get; set; }
        public string departmentIds { get; set; }
        public string search { get; set; }
    }
}