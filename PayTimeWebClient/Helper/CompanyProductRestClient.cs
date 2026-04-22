using Newtonsoft.Json;
using PayTimeWebClient.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace PayTimeWebClient.Helper
{
    /// <summary>
    /// REST Client for Company-Product API operations
    /// </summary>
    public class CompanyProductRestClient
    {
        private static readonly string _baseurl = ConfigurationManager.AppSettings["webapipaytime"];

        /// <summary>
        /// Gets the list of product IDs that a company has purchased/access to
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <returns>List of product IDs</returns>
        public static List<int> GetCompanyPurchasedProducts(int companyId)
        {
            try
            {
                // Fallback: If companyId is 0 or invalid, return all products
                if (companyId <= 0)
                {
                    return GetAllProductIds();
                }

                using (var client = new HttpClient())
                {
                    string uri = _baseurl + $"CompanyProduct/GetCompanyProducts?companyId={companyId}";

                    var getTask = client.GetAsync(uri);
                    getTask.Wait();

                    var result = getTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var jsonResult = result.Content.ReadAsStringAsync().Result;
                        var response = JsonConvert.DeserializeObject<GetCompanyProductsResponse>(jsonResult);

                        if (response != null && response.Success && response.ProductIds != null && response.ProductIds.Count > 0)
                        {
                            return response.ProductIds;
                        }
                    }
                }

                // Fallback: Return all products if API fails or returns no data
                return GetAllProductIds();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetCompanyPurchasedProducts: {ex.Message}");
                // Fail-safe: Return all products on error
                return GetAllProductIds();
            }
        }

        /// <summary>
        /// Checks if a company has access to a specific product
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="productId">Product ID</param>
        /// <returns>True if company has access, false otherwise</returns>
        //public static bool CompanyHasProduct(int companyId, int productId)
        //{
        //    var purchasedProducts = GetCompanyPurchasedProducts(companyId);
        //    return purchasedProducts.Contains(productId);
        //}

        /// <summary>
        /// Adds a product to a company (purchases a product)
        /// </summary>
        /// <param name="request">Add product request</param>
        /// <returns>API response</returns>
        //public static CompanyProductApiResponse AddProductToCompany(AddProductToCompanyRequest request)
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            string uri = _baseurl + "CompanyProduct/AddProductToCompany";

        //            var postTask = client.PostAsJsonAsync(uri, request);
        //            postTask.Wait();

        //            var result = postTask.Result;

        //            if (result.IsSuccessStatusCode)
        //            {
        //                var jsonResult = result.Content.ReadAsStringAsync().Result;
        //                return JsonConvert.DeserializeObject<CompanyProductApiResponse>(jsonResult);
        //            }
        //            else
        //            {
        //                return new CompanyProductApiResponse
        //                {
        //                    Success = false,
        //                    Message = $"API Error: {result.StatusCode}"
        //                };
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error in AddProductToCompany: {ex.Message}");
        //        return new CompanyProductApiResponse
        //        {
        //            Success = false,
        //            Message = $"Exception: {ex.Message}"
        //        };
        //    }
        //}

        /// <summary>
        /// Removes a product from a company (deactivates access)
        /// </summary>
        /// <param name="request">Remove product request</param>
        /// <returns>API response</returns>
        //public static CompanyProductApiResponse RemoveProductFromCompany(RemoveProductFromCompanyRequest request)
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            string uri = _baseurl + "CompanyProduct/RemoveProductFromCompany";

        //            var postTask = client.PostAsJsonAsync(uri, request);
        //            postTask.Wait();

        //            var result = postTask.Result;

        //            if (result.IsSuccessStatusCode)
        //            {
        //                var jsonResult = result.Content.ReadAsStringAsync().Result;
        //                return JsonConvert.DeserializeObject<CompanyProductApiResponse>(jsonResult);
        //            }
        //            else
        //            {
        //                return new CompanyProductApiResponse
        //                {
        //                    Success = false,
        //                    Message = $"API Error: {result.StatusCode}"
        //                };
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error in RemoveProductFromCompany: {ex.Message}");
        //        return new CompanyProductApiResponse
        //        {
        //            Success = false,
        //            Message = $"Exception: {ex.Message}"
        //        };
        //    }
        //}

        /// <summary>
        /// Gets detailed company-product mapping information
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <returns>API response with product mappings</returns>
        //public static GetCompanyProductsResponse GetCompanyProductDetails(int companyId)
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            string uri = _baseurl + $"CompanyProduct/GetCompanyProductDetails?companyId={companyId}";

        //            var getTask = client.GetAsync(uri);
        //            getTask.Wait();

        //            var result = getTask.Result;

        //            if (result.IsSuccessStatusCode)
        //            {
        //                var jsonResult = result.Content.ReadAsStringAsync().Result;
        //                return JsonConvert.DeserializeObject<GetCompanyProductsResponse>(jsonResult);
        //            }
        //            else
        //            {
        //                return new GetCompanyProductsResponse
        //                {
        //                    Success = false,
        //                    Message = $"API Error: {result.StatusCode}",
        //                    ProductMappings = new List<CompanyProductMapping>()
        //                };
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error in GetCompanyProductDetails: {ex.Message}");
        //        return new GetCompanyProductsResponse
        //        {
        //            Success = false,
        //            Message = $"Exception: {ex.Message}",
        //            ProductMappings = new List<CompanyProductMapping>()
        //        };
        //    }
        //}

        /// <summary>
        /// Gets list of companies accessible to a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of companies</returns>
        public static GetUserCompaniesResponse GetUserCompanies(int userId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string uri = _baseurl + $"Company/GetUserCompanies?userId={userId}";

                    var getTask = client.GetAsync(uri);
                    getTask.Wait();

                    var result = getTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var jsonResult = result.Content.ReadAsStringAsync().Result;
                        return JsonConvert.DeserializeObject<GetUserCompaniesResponse>(jsonResult);
                    }
                    else
                    {
                        return new GetUserCompaniesResponse
                        {
                            Success = false,
                            Message = $"API Error: {result.StatusCode}",
                            Companies = new List<CompanyInfo>()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetUserCompanies: {ex.Message}");
                return new GetUserCompaniesResponse
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}",
                    Companies = new List<CompanyInfo>()
                };
            }
        }

        /// <summary>
        /// Gets all product master data from backend API (dynamic list)
        /// </summary>
        /// <returns>List of all products</returns>
        public static List<Product> GetAllProductDefinitions()
        {        
         
            string uri = _baseurl + "ProductSeperationMaster/GetAll";            
            string _DbName = "";

            try
            {               
                using (HttpClient httpClient = new HttpClient())
                {                    
                    httpClient.Timeout = TimeSpan.FromMinutes(30);                 
                    try
                    {                     
                        if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["DbName"] != null)
                        {                           
                            if (!string.IsNullOrEmpty(HttpContext.Current.Session["DbName"].ToString()))
                            {
                                _DbName = HttpContext.Current.Session["DbName"].ToString();
                            }
                        }
                        if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["tokan"] != null)
                        {
                            string token = HttpContext.Current.Session["tokan"].ToString();
                            if (!string.IsNullOrEmpty(token))
                            {
                                httpClient.DefaultRequestHeaders.Add("Authorization", token);                               
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("[GetAllProductDefinitions] ⚠️ No auth token available in session");
                        }
                    }
                    catch (Exception tokenEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"[GetAllProductDefinitions] ⚠️ Token error (continuing anyway): {tokenEx.Message}");
                    }

                    
                    Task<HttpResponseMessage> response = httpClient.GetAsync(uri);
                    System.Diagnostics.Debug.WriteLine("[GetAllProductDefinitions] ✓ GetAsync() called, task created");
                    var result = response.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var jsonResult = result.Content.ReadAsStringAsync().Result;
                        var res = JsonConvert.DeserializeObject<ProductMasterApiResponse>(jsonResult);
                        if (response != null && res.Success && res.Products != null && res.Products.Count > 0)
                        {                           
                            return res.Products;
                        }
                        else
                        {
                            if (response != null)
                            {                            
                                if (res.Products != null)
                                {
                                    System.Diagnostics.Debug.WriteLine($"[GetAllProductDefinitions] Response.Products count: {res.Products.Count}");
                                }
                            }
                        }
                    }
                    else
                    {
                        var errorContent = result.Content.ReadAsStringAsync().Result;                      
                    }
                }
                // Fallback: Return hardcoded products if API fails
                System.Diagnostics.Debug.WriteLine("[GetAllProductDefinitions] 🔄 FALLBACK: Using hardcoded products");
                return GetHardcodedProducts();
            }
            catch (TaskCanceledException timeoutEx)
            {                
                return GetHardcodedProducts();
            }
            catch (HttpRequestException httpEx)
            {       
                return GetHardcodedProducts();
            }
            catch (AggregateException aggEx)
            {
                System.Diagnostics.Debug.WriteLine($"[GetAllProductDefinitions] ⚠️ AGGREGATE EXCEPTION:");
                foreach (var innerEx in aggEx.InnerExceptions)
                {
                    System.Diagnostics.Debug.WriteLine($"  - {innerEx.GetType().Name}: {innerEx.Message}");
                }
                return GetHardcodedProducts();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetAllProductDefinitions] ⚠️ GENERAL EXCEPTION: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"[GetAllProductDefinitions] Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[GetAllProductDefinitions] Stack: {ex.StackTrace}");

                // Fail-safe: Return hardcoded products on error
                System.Diagnostics.Debug.WriteLine("[GetAllProductDefinitions] 🔄 FALLBACK: Using hardcoded products (exception)");
                return GetHardcodedProducts();
            }
        }

        /// <summary>
        /// Test method to check if backend API is reachable
        /// </summary>
        /// <returns>Diagnostic information about API connectivity</returns>
        //public static ApiConnectivityTestResult TestBackendConnectivity()
        //{
        //    var result = new ApiConnectivityTestResult
        //    {
        //        TestTimestamp = DateTime.Now,
        //        BaseUrl = _baseurl,
        //        Endpoint = "ProductSeparationMaster/GetAll"
        //    };

        //    try
        //    {
        //        if (string.IsNullOrEmpty(_baseurl))
        //        {
        //            result.IsSuccess = false;
        //            result.ErrorMessage = "Base URL is not configured in Web.config (webapipaytime key)";
        //            return result;
        //        }

        //        string uri = _baseurl + result.Endpoint;
        //        result.FullUrl = uri;

        //        using (HttpClient httpClient = new HttpClient())
        //        {
        //            httpClient.Timeout = TimeSpan.FromSeconds(10);

        //            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        //            var getTask = httpClient.GetAsync(uri);

        //            if (!getTask.Wait(TimeSpan.FromSeconds(10)))
        //            {
        //                result.IsSuccess = false;
        //                result.ErrorMessage = "Request timed out after 10 seconds";
        //                result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
        //                return result;
        //            }

        //            var response = getTask.Result;
        //            stopwatch.Stop();

        //            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
        //            result.StatusCode = (int)response.StatusCode;
        //            result.StatusCodeDescription = response.StatusCode.ToString();
        //            result.IsSuccess = response.IsSuccessStatusCode;

        //            if (!response.IsSuccessStatusCode)
        //            {
        //                result.ErrorMessage = $"API returned {response.StatusCode}: {response.ReasonPhrase}";
        //                try
        //                {
        //                    result.ResponseContent = response.Content.ReadAsStringAsync().Result;
        //                }
        //                catch { }
        //            }
        //            else
        //            {
        //                result.ResponseContent = response.Content.ReadAsStringAsync().Result;
        //            }
        //        }
        //    }
        //    catch (TaskCanceledException)
        //    {
        //        result.IsSuccess = false;
        //        result.ErrorMessage = "Request timed out";
        //    }
        //    catch (HttpRequestException httpEx)
        //    {
        //        result.IsSuccess = false;
        //        result.ErrorMessage = $"Cannot connect to backend: {httpEx.Message}";
        //        if (httpEx.InnerException != null)
        //        {
        //            result.ErrorMessage += $" | Inner: {httpEx.InnerException.Message}";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.IsSuccess = false;
        //        result.ErrorMessage = $"{ex.GetType().Name}: {ex.Message}";
        //    }

        //    return result;
        //}

        /// <summary>
        /// Returns all product IDs (for backward compatibility or when no mapping exists)
        /// </summary>
        /// <returns>List of all product IDs</returns>
        private static List<int> GetAllProductIds()
        {
            // All product IDs: TNA(1), Payroll(2), EMS(3), OKR(4), PMS(5), FieldSens(6), CMS(7), VMS(8)
            return new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
        }

        /// <summary>
        /// Hardcoded product list (fallback if API fails)
        /// </summary>
        /// <returns>List of hardcoded products</returns>
        private static List<Product> GetHardcodedProducts()
        {
            var products = new List<Product>
            {
                // Product 1: Time & Attendance (TNA) - Main Product
                new Product
                {
                    ProductId = 1,
                    ProductName = "Time & Attendance",
                    ProductCode = "TNA",
                    Description = "Time and attendance tracking system",
                    ApiUrl = ConfigurationManager.AppSettings["webapipaytime"],
                    IconClass = "fa-solid fa-clock",
                    IsMainProduct = true,
                    IsActive = true,
                    DisplayOrder = 1,
                    RedirectUrl = "/Dashboard/AdminDashboard",
                    Category = "Core HR",
                    CategoryIcon = "fa-solid fa-briefcase"
                },

                // Product 2: Payroll
                new Product
                {
                    ProductId = 2,
                    ProductName = "Payroll",
                    ProductCode = "PAYROLL",
                    Description = "Payroll processing and management",
                    ApiUrl = ConfigurationManager.AppSettings["webapipaytimePayroll"],
                    IconClass = "fa-solid fa-money-bill-wave",
                    IsMainProduct = false,
                    IsActive = true,
                    DisplayOrder = 2,
                    RedirectUrl = "/MinopProduct/PayrollDashboard",
                    Category = "Finance & Payroll",
                    CategoryIcon = "fa-solid fa-money-bill-wave"
                },

                // Product 3: EMS (Expense Management System)
                new Product
                {
                    ProductId = 3,
                    ProductName = "EMS",
                    ProductCode = "EMS",
                    Description = "Expense Management System",
                    ApiUrl = ConfigurationManager.AppSettings["WebApiEMS"],
                    IconClass = "fa-solid fa-receipt",
                    IsMainProduct = false,
                    IsActive = true,
                    DisplayOrder = 3,
                    RedirectUrl = "/MinopProduct/EMSDashboard",
                    Category = "Finance & Payroll",
                    CategoryIcon = "fa-solid fa-money-bill-wave"
                },

                // Product 4: OKR (Objectives and Key Results)
                new Product
                {
                    ProductId = 8,
                    ProductName = "OKR",
                    ProductCode = "OKR",
                    Description = "Objectives and Key Results",
                    ApiUrl = ConfigurationManager.AppSettings["webapiurlokr"],
                    IconClass = "fa-solid fa-bullseye",
                    IsMainProduct = false,
                    IsActive = true,
                    DisplayOrder = 8,
                    RedirectUrl = "/MinopProduct/OKRDashboard",
                    Category = "Performance Management",
                    CategoryIcon = "fa-solid fa-chart-line"
                },

                // Product 5: PMS (Performance Management System) - Currently Hidden
                new Product
                {
                    ProductId = 4,
                    ProductName = "PMS",
                    ProductCode = "PMS",
                    Description = "Performance Management System",
                    ApiUrl = ConfigurationManager.AppSettings["webapiPMS"],
                    IconClass = "fa-solid fa-trophy",
                    IsMainProduct = false,
                    IsActive = false,  // Hidden from modal
                    DisplayOrder = 4,
                    RedirectUrl = "/MinopProduct/PMSDashboard",
                    Category = "Performance Management",
                    CategoryIcon = "fa-solid fa-chart-line"
                },

                // Product 6: FieldSens (Field Service Management)
                new Product
                {
                    ProductId = 5,
                    ProductName = "FieldSens",
                    ProductCode = "FIELDSENS",
                    Description = "Field Service Management System",
                    ApiUrl = ConfigurationManager.AppSettings["webapipaytime"],  // Uses TNA API
                    IconClass = "fa-solid fa-location-dot",
                    IsMainProduct = false,
                    IsActive = true,
                    DisplayOrder = 5,
                    RedirectUrl = "/MinopProduct/FieldSensDashboard",
                    Category = "Field Service",
                    CategoryIcon = "fa-solid fa-location-dot"
                },

                // Product 7: CMS (Canteen Management System)
                new Product
                {
                    ProductId = 6,
                    ProductName = "Canteen Management",
                    ProductCode = "CMS",
                    Description = "Canteen Management System",
                    ApiUrl = ConfigurationManager.AppSettings["webapipaytime"],  // Uses TNA API
                    IconClass = "fa-solid fa-utensils",
                    IsMainProduct = false,
                    IsActive = true,
                    DisplayOrder = 6,
                    RedirectUrl = "/MinopProduct/CMSDashboard",
                    Category = "Facility Management",
                    CategoryIcon = "fa-solid fa-building"
                },

                // Product 8: VMS (Visitor Management System)
                new Product
                {
                    ProductId = 7,
                    ProductName = "Visitor Management",
                    ProductCode = "VMS",
                    Description = "Visitor Management System",
                    ApiUrl = ConfigurationManager.AppSettings["webapipaytime"],  // Uses TNA API
                    IconClass = "fa-solid fa-id-card-clip",
                    IsMainProduct = false,
                    IsActive = true,
                    DisplayOrder = 7,
                    RedirectUrl = "/MinopProduct/VMSDashboard",
                    Category = "Facility Management",
                    CategoryIcon = "fa-solid fa-building"
                }
            };

            return products;
        }
    }
}
