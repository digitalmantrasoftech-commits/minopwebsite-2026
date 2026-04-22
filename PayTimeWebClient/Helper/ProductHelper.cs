using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PayTimeWebClient.Models;

namespace PayTimeWebClient.Helper
{
    /// <summary>
    /// Helper class for product-related operations including URL-based product detection
    /// </summary>
    public static class ProductHelper
    {
        /// <summary>
        /// Gets product from session cache (API data)
        /// Primary data source - returns null if cache is empty (fallback to hardcoded)
        /// </summary>
        /// <param name="productId">Product ID to look up</param>
        /// <returns>Product from cache or null if not found</returns>
        private static Product GetProductFromCache(int productId)
        {
            try
            {
                if (HttpContext.Current?.Session?["CachedProductList"] != null)
                {
                    var cachedProducts = HttpContext.Current.Session["CachedProductList"] as List<Product>;
                    if (cachedProducts != null)
                    {
                        return cachedProducts.FirstOrDefault(p => p.ProductId == productId);
                    }
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Gets the current product ID based on URL or session
        /// Supports multi-tab Zoho-style product detection
        /// Priority: Query string ?p=code, then ?productId=# (backward compatibility), then URL detection, then Session
        /// </summary>
        /// <returns>Product ID (1-7)</returns>
        public static int GetCurrentProductId()
        {
            try
            {
                // PRIORITY 1: Check for ?p=code parameter (e.g., ?p=payroll)
                // This hides product IDs from browser URL
                if (HttpContext.Current?.Request?.QueryString["p"] != null)
                {
                    string productCode = HttpContext.Current.Request.QueryString["p"];
                    int productIdFromCode = GetProductIdFromCode(productCode);

                    if (productIdFromCode > 0)  // Removed upper limit - data-driven
                    {
                        // Sync session to match query string (for consistency)
                        if (HttpContext.Current?.Session != null)
                        {
                            HttpContext.Current.Session["SelectedProductId"] = productIdFromCode;
                        }
                        return productIdFromCode;
                    }
                }

                // PRIORITY 2: Check old ?productId=# parameter for backward compatibility
                // This allows existing bookmarks and links to continue working
                if (HttpContext.Current?.Request?.QueryString["productId"] != null)
                {
                    int queryProductId;
                    if (int.TryParse(HttpContext.Current.Request.QueryString["productId"], out queryProductId))
                    {
                        if (queryProductId > 0)  // Removed upper limit - data-driven
                        {
                            // Sync session to match query string (for consistency)
                            if (HttpContext.Current?.Session != null)
                            {
                                HttpContext.Current.Session["SelectedProductId"] = queryProductId;
                            }
                            return queryProductId;
                        }
                    }
                }

                // PRIORITY 3: Check if current URL is product-specific (dashboard or product folder)
                string currentUrl = HttpContext.Current?.Request?.Url?.PathAndQuery?.ToLower() ?? "";

                bool isProductSpecificUrl = currentUrl.Contains("/minopproduct/") ||
                                           currentUrl.Contains("/payroll/") ||
                                           currentUrl.Contains("/viewsems/") ||
                                           currentUrl.Contains("/ems/") ||
                                           currentUrl.Contains("/okr/") ||
                                           currentUrl.Contains("/pms/") ||
                                           currentUrl.Contains("/fieldsens/") ||
                                           currentUrl.Contains("/fieldtracking/") ||
                                           currentUrl.Contains("/cms/") ||
                                           currentUrl.Contains("/canteen/") ||
                                           currentUrl.Contains("/vms/") ||
                                           currentUrl.Contains("/visitor/");

                // For product-specific URLs (dashboards, product folders), use URL detection
                // This enables multi-tab support where each tab can have a different product
                if (isProductSpecificUrl)
                {
                    int productIdFromUrl = DetectProductIdFromUrl();
                    if (productIdFromUrl > 1) // Only return if it detected a specific product (not default TNA)
                        return productIdFromUrl;
                }

                // PRIORITY 4: For common pages (MasterData, Transactions, Reports, etc.),
                // use Session to maintain product context across navigation
                if (HttpContext.Current?.Session["SelectedProductId"] != null)
                {
                    int sessionProductId = Convert.ToInt32(HttpContext.Current.Session["SelectedProductId"]);
                    if (sessionProductId > 0)
                        return sessionProductId;
                }

                // Default to TNA if no product context found
                return 1;
            }
            catch
            {
                return 1; // Default to TNA on error
            }
        }

        /// <summary>
        /// Detects product ID from the current URL path
        /// Enables multi-tab support where different tabs can show different products
        /// </summary>
        /// <returns>Product ID (1-6) or 1 if not detected</returns>
        public static int DetectProductIdFromUrl()
        {
            try
            {
                if (HttpContext.Current?.Request?.Url == null)
                    return 1;

                string currentUrl = HttpContext.Current.Request.Url.PathAndQuery.ToLower();

                // Product 2: Payroll
                if (currentUrl.Contains("/minopproduct/payrolldashboard") ||
                    currentUrl.Contains("/payroll/"))
                {
                    return 2;
                }

                // Product 3: EMS
                if (currentUrl.Contains("/minopproduct/emsdashboard") ||
                    currentUrl.Contains("/viewsems/") ||
                    currentUrl.Contains("/ems/"))
                {
                    return 3;
                }

                // Product 4: OKR
                if (currentUrl.Contains("/minopproduct/okrdashboard") ||
                    currentUrl.Contains("/okr/"))
                {
                    return 8;
                }

                // Product 5: PMS
                if (currentUrl.Contains("/minopproduct/pmsdashboard") ||
                    currentUrl.Contains("/pms/"))
                {
                    return 4;
                }

                // Product 5: FieldSens
                if (currentUrl.Contains("/minopproduct/fieldsens") ||
                    currentUrl.Contains("/fieldsens/") ||
                    currentUrl.Contains("/fieldtracking/"))
                {
                    return 5;
                }

                // Product 6: CMS (Canteen Management System)
                if (currentUrl.Contains("/minopproduct/cmsdashboard") ||
                    currentUrl.Contains("/cms/") ||
                    currentUrl.Contains("/canteen/"))
                {
                    return 6;
                }

                // Product 7: VMS (Visitor Management System)
                if (currentUrl.Contains("/minopproduct/vmsdashboard") ||
                    currentUrl.Contains("/vms/") ||
                    currentUrl.Contains("/visitor/"))
                {
                    return 7;
                }

                // Product 1: TNA (Default)
                // Includes: /Dashboard/, /MasterData/, /Transactions/, /Report/, etc.
                return 1;
            }
            catch
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the product name for a given product ID
        /// Data-driven: Uses session cache first, then falls back to hardcoded values
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <returns>Product name</returns>
        public static string GetProductName(int productId)
        {
            // Try cache first (API data)
            var product = GetProductFromCache(productId);
            if (product != null)
                return product.ProductName;

            // Fallback to hardcoded
            switch (productId)
            {
                case 1:
                    return "Time & Attendance";
                case 2:
                    return "Payroll";
                case 3:
                    return "EMS";
                case 4:
                    return "PMS";
                case 5:
                    return "FieldSens";
                case 6:
                    return "Canteen Management";
                case 7:
                    return "Visitor Management";
                case 8:
                    return "OKR";
                default:
                    return "Unknown Product";
            }
        }

        /// <summary>
        /// Gets the product code for a given product ID
        /// Data-driven: Uses session cache first, then falls back to hardcoded values
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <returns>Product code</returns>
        public static string GetProductCode(int productId)
        {
            // Try cache first (API data)
            var product = GetProductFromCache(productId);
            if (product != null)
                return product.ProductCode;

            // Fallback to hardcoded
            switch (productId)
            {
                case 1:
                    return "TNA";
                case 2:
                    return "PAYROLL";
                case 3:
                    return "EMS";
                case 4:
                    return "PMS";
                case 5:
                    return "FIELDSENS";
                case 6:
                    return "CMS";
                case 7:
                    return "VMS";
                case 8:
                    return "OKR";
                default:
                    return "UNKNOWN";
            }
        }

        /// <summary>
        /// Encodes a plain text string to Base64
        /// Used for hiding product codes in URLs
        /// </summary>
        /// <param name="plainText">Plain text to encode</param>
        /// <returns>Base64 encoded string</returns>
        private static string EncodeToBase64(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            try
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(bytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Decodes a Base64 string to plain text
        /// Used for retrieving product codes from URLs
        /// </summary>
        /// <param name="base64Text">Base64 encoded text</param>
        /// <returns>Decoded plain text string</returns>
        private static string DecodeFromBase64(string base64Text)
        {
            if (string.IsNullOrEmpty(base64Text))
                return string.Empty;

            try
            {
                byte[] bytes = Convert.FromBase64String(base64Text);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets URL-friendly product code (Base64 encoded) for a given product ID
        /// Used for hiding product information in browser URLs (e.g., ?p=cGF5cm9sbA== instead of ?p=payroll)
        /// Data-driven: Uses session cache first, then falls back to hardcoded values
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <returns>Base64 encoded product code</returns>
        public static string GetProductCodeForUrl(int productId)
        {
            // Try cache first - use ProductCode from API
            var product = GetProductFromCache(productId);
            if (product != null && !string.IsNullOrEmpty(product.ProductCode))
            {
                return EncodeToBase64(product.ProductCode.ToLower());
            }

            // Fallback to hardcoded
            string plainCode;
            switch (productId)
            {
                case 1:
                    plainCode = "tna";
                    break;
                case 2:
                    plainCode = "payroll";
                    break;
                case 3:
                    plainCode = "ems";
                    break;
                case 4:
                    plainCode = "pms";
                    break;
                case 5:
                    plainCode = "fieldsens";
                    break;
                case 6:
                    plainCode = "cms";
                    break;
                case 7:
                    plainCode = "vms";
                    break;
                case 8:
                    plainCode = "okr";
                    break;
                default:
                    plainCode = "tna";
                    break;
            }

            // Return Base64 encoded product code to hide product names from URL
            return EncodeToBase64(plainCode);
        }

        /// <summary>
        /// Gets product ID from URL-friendly product code (Base64 encoded or plain text)
        /// Used for decoding product from URL parameter (e.g., ?p=cGF5cm9sbA== -> 2)
        /// Supports both Base64 encoded and plain text codes for backward compatibility
        /// Data-driven: Uses session cache first, then falls back to hardcoded values
        /// </summary>
        /// <param name="productCode">URL-friendly product code (Base64 encoded or plain text)</param>
        /// <returns>Product ID, defaults to 1 if invalid code</returns>
        public static int GetProductIdFromCode(string productCode)
        {
            if (string.IsNullOrEmpty(productCode))
                return 1;

            // First, try to decode from Base64
            string decodedCode = DecodeFromBase64(productCode);

            // If Base64 decoding failed or returned empty, use the original value (backward compatibility)
            if (string.IsNullOrEmpty(decodedCode))
                decodedCode = productCode;

            string normalizedCode = decodedCode.ToLower().Trim();

            // Try cache first - search by ProductCode
            try
            {
                if (HttpContext.Current?.Session?["CachedProductList"] != null)
                {
                    var cachedProducts = HttpContext.Current.Session["CachedProductList"] as List<Product>;
                    if (cachedProducts != null)
                    {
                        var product = cachedProducts.FirstOrDefault(p =>
                            p.ProductCode.ToLower() == normalizedCode);
                        if (product != null)
                            return product.ProductId;
                    }
                }
            }
            catch { }

            // Fallback to hardcoded
            switch (normalizedCode)
            {
                case "tna":
                    return 1;
                case "payroll":
                    return 2;
                case "ems":
                    return 3;
                case "pms":
                    return 4;
                case "fieldsens":
                    return 5;
                case "cms":
                    return 6;
                case "vms":
                    return 7;
                case "okr":
                    return 8;
                default:
                    return 1; // Default to TNA for invalid codes
            }
        }

        /// <summary>
        /// Checks if a URL belongs to a specific product
        /// </summary>
        /// <param name="url">URL to check</param>
        /// <param name="productId">Product ID to match</param>
        /// <returns>True if URL belongs to the product</returns>
        public static bool IsUrlForProduct(string url, int productId)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            url = url.ToLower();

            switch (productId)
            {
                case 1: // TNA
                    return !url.Contains("/minopproduct/") &&
                           !url.Contains("/payroll/") &&
                           !url.Contains("/viewsems/") &&
                           !url.Contains("/okr/") &&
                           !url.Contains("/pms/") &&
                           !url.Contains("/fieldsens/") &&
                           !url.Contains("/fieldtracking/") &&
                           !url.Contains("/cms/") &&
                           !url.Contains("/canteen/") &&
                           !url.Contains("/vms/") &&
                           !url.Contains("/visitor/");

                case 2: // Payroll
                    return url.Contains("/minopproduct/payrolldashboard") ||
                           url.Contains("/payroll/");

                case 3: // EMS
                    return url.Contains("/minopproduct/emsdashboard") ||
                           url.Contains("/viewsems/") ||
                           url.Contains("/ems/");

                case 4: // PMS
                    return url.Contains("/minopproduct/pmsdashboard") ||
                           url.Contains("/pms/");

                case 5: // FieldSens
                    return url.Contains("/minopproduct/fieldsens") ||
                           url.Contains("/fieldsens/") ||
                           url.Contains("/fieldtracking/");

                case 6: // CMS
                    return url.Contains("/minopproduct/cmsdashboard") ||
                           url.Contains("/cms/") ||
                           url.Contains("/canteen/");

                case 7: // VMS
                    return url.Contains("/minopproduct/vmsdashboard") ||
                           url.Contains("/vms/") ||
                           url.Contains("/visitor/");
                case 8: // OKR
                    return url.Contains("/minopproduct/okrdashboard") ||
                           url.Contains("/okr/");

                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets all configured products
        /// Data-driven: Uses session cache first, then falls back to hardcoded values
        /// </summary>
        /// <returns>Dictionary of product ID to product name</returns>
        public static Dictionary<int, string> GetAllConfiguredProducts()
        {
            // Try cache first
            try
            {
                if (HttpContext.Current?.Session?["CachedProductList"] != null)
                {
                    var cachedProducts = HttpContext.Current.Session["CachedProductList"] as List<Product>;
                    if (cachedProducts != null && cachedProducts.Count > 0)
                    {
                        return cachedProducts.ToDictionary(p => p.ProductId, p => p.ProductName);
                    }
                }
            }
            catch { }

            // Fallback to hardcoded
            return new Dictionary<int, string>
            {
                { 1, "Time & Attendance" },
                { 2, "Payroll" },
                { 3, "EMS" },
                { 4, "PMS" },
                { 5, "FieldSens" },
                { 6, "Canteen Management" },
                { 7, "Visitor Management" },
                { 8, "OKR" },
            };
        }

        /// <summary>
        /// Gets the dashboard URL for a specific product ID (Centralized helper method)
        /// Checks session cache, then API, then falls back to hardcoded values
        /// </summary>
        /// <param name="productId">Product ID (1-7)</param>
        /// <returns>Dashboard URL for the product</returns>
        public static string GetProductDashboardUrl(int productId)
        {
            try
            {
                // STEP 1: Check session cache first (fastest)
                if (HttpContext.Current?.Session != null && HttpContext.Current.Session["CachedProductList"] != null)
                {
                    var cachedProducts = HttpContext.Current.Session["CachedProductList"] as List<Product>;
                    if (cachedProducts != null)
                    {
                        var product = cachedProducts.FirstOrDefault(p => p.ProductId == productId);
                        if (product != null && !string.IsNullOrEmpty(product.RedirectUrl))
                        {
                            return product.RedirectUrl;
                        }
                    }
                }

                // STEP 2: Get from API/hardcoded list via CompanyProductRestClient
                var allProducts = CompanyProductRestClient.GetAllProductDefinitions();
                if (allProducts != null && allProducts.Count > 0)
                {
                    var matchedProduct = allProducts.FirstOrDefault(p => p.ProductId == productId);
                    if (matchedProduct != null && !string.IsNullOrEmpty(matchedProduct.RedirectUrl))
                    {
                        return matchedProduct.RedirectUrl;
                    }
                }

                // STEP 3: Fallback to hardcoded values (fail-safe)
                return GetDefaultDashboardUrl(productId);
            }
            catch
            {
                // On any error, return hardcoded fallback
                return GetDefaultDashboardUrl(productId);
            }
        }

        /// <summary>
        /// Gets the default hardcoded dashboard URL for a product (fail-safe fallback)
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <returns>Default dashboard URL</returns>
        private static string GetDefaultDashboardUrl(int productId)
        {
            switch (productId)
            {
                case 1:
                    return "/Dashboard/AdminDashboard";
                case 2:
                    return "/MinopProduct/PayrollDashboard";
                case 3:
                    return "/MinopProduct/EMSDashboard";              
                case 4:
                    return "/MinopProduct/PMSDashboard";
                case 5:
                    return "/MinopProduct/FieldSensDashboard";
                case 6:
                    return "/MinopProduct/CMSDashboard";
                case 7:
                    return "/MinopProduct/VMSDashboard";
                case 8:
                    return "/MinopProduct/OKRDashboard";
                default:
                    return "/Dashboard/AdminDashboard";
            }
        }
    }
}
