using PayTimeWebClient.Models;
using PayTimeWebClient.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace PayTimeWebClient.Controllers
{
    /// <summary>
    /// Controller for managing product switching and retrieval
    /// </summary>
    public class ProductController : Controller
    {
        /// <summary>
        /// Gets the list of available products for the current user
        /// </summary>
        /// <returns>JSON result with list of products</returns>
        private static readonly MasterDataRestClient RestClient = new MasterDataRestClient();

        [HttpGet]
        public JsonResult GetAvailableProducts()
        {
            try
            {
                // CHECK SESSION CACHE FIRST - Products locked to session until logout/login
                if (Session["CachedProductList"] != null)
                {
                    var cachedProducts = Session["CachedProductList"] as List<Product>;
                    return Json(new { success = true, data = cachedProducts }, JsonRequestBehavior.AllowGet);
                }

                // FETCH FROM API (only if not cached - first call after login)
                var products = GetProductsForUser(0, 0);

                // Filter by IsActive and ApiUrl
                var activeProducts = products.Where(p =>
                    !string.IsNullOrEmpty(p.ApiUrl) &&
                    p.IsActive
                ).ToList();

                // STORE IN SESSION CACHE - will be used for all subsequent requests
                Session["CachedProductList"] = activeProducts;

                return Json(new { success = true, data = activeProducts }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetAvailableProducts] Error: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while loading products. Please try again." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Switches the current product context for the user
        /// </summary>
        /// <param name="productId">ID of the product to switch to</param>
        /// <returns>JSON result with success status and redirect URL</returns>
        [HttpPost]
        public JsonResult SwitchProduct(int productId)
        {
            try
            {
                var product = GetProductsForUser(0, 0).FirstOrDefault(p => p.ProductId == productId);

                if (product == null)
                    return Json(new { success = false, message = "Product not found" });

                if (!product.IsActive)
                    return Json(new { success = false, message = "Product is not active" });

                // Update session with selected product
                Session["SelectedProductId"] = productId;
                Session["SelectedProductName"] = product.ProductName;
                Session["SelectedProductCode"] = product.ProductCode;
                Session["CurrentApiUrl"] = product.ApiUrl;

                return Json(new
                {
                    success = true,
                    message = $"Switched to {product.ProductName}",
                    redirectUrl = product.RedirectUrl
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SwitchProduct] Error: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while switching product. Please try again." });
            }
        }

        /// <summary>
        /// Gets the product configuration for the current user
        /// Filters based on company's purchased products
        /// </summary>
        /// <returns>List of configured products</returns>
        private List<Product> GetProductsForUser(int userId, int companyIdnew)
        {
            System.Diagnostics.Debug.WriteLine("========================================");
            System.Diagnostics.Debug.WriteLine("[ProductController.GetProductsForUser] 🔵 METHOD CALLED");
            System.Diagnostics.Debug.WriteLine($"[ProductController.GetProductsForUser] Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");

            // Get all available product definitions from API (dynamic)
            System.Diagnostics.Debug.WriteLine("[ProductController.GetProductsForUser] Calling CompanyProductRestClient.GetAllProductDefinitions()...");
            var allProducts = CompanyProductRestClient.GetAllProductDefinitions();
            System.Diagnostics.Debug.WriteLine($"[ProductController.GetProductsForUser] ✓ GetAllProductDefinitions returned {allProducts?.Count ?? 0} products");

            // Get company ID from session
            int companyId = 0;
            if (Session["ClientCompanyId"] != null)
            {
                companyId = Convert.ToInt32(Session["ClientCompanyId"]);
            }
            System.Diagnostics.Debug.WriteLine($"[ProductController.GetProductsForUser] Company ID from session: {companyId}");

            // Get products that company has purchased (via API)
            System.Diagnostics.Debug.WriteLine("[ProductController.GetProductsForUser] Calling GetCompanyPurchasedProducts...");
            var purchasedProductIds = CompanyProductRestClient.GetCompanyPurchasedProducts(companyId);
            System.Diagnostics.Debug.WriteLine($"[ProductController.GetProductsForUser] ✓ Company has access to products: [{string.Join(", ", purchasedProductIds)}]");

            // Filter products: only return products company has access to
            var filteredProducts = allProducts
                .Where(p => purchasedProductIds.Contains(p.ProductId))
                .ToList();

            System.Diagnostics.Debug.WriteLine($"[ProductController.GetProductsForUser] ✓ Filtered to {filteredProducts.Count} products");
            System.Diagnostics.Debug.WriteLine("========================================");

            return filteredProducts;
        }

        /// <summary>
        /// Gets the current product ID from session or URL
        /// </summary>
        /// <returns>Current product ID (default: 1 for TNA)</returns>
        private int GetCurrentProductId()
        {
            // Check session first
            if (Session["SelectedProductId"] != null)
            {
                return Convert.ToInt32(Session["SelectedProductId"]);
            }

            // Default to TNA
            return 1;
        }

        /// <summary>
        /// Smart redirect after login - redirects to appropriate dashboard based on active products
        /// SINGLE PRODUCT MODE: If only 1 product is active, redirect directly to that product's dashboard
        /// MULTI PRODUCT MODE: If multiple products active, redirect to default TNA dashboard
        /// </summary>
        /// <returns>JSON with redirect URL and product information</returns>
        [HttpGet]
        public JsonResult GetPostLoginRedirect()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine("[GetPostLoginRedirect] Smart redirect logic started");

                // Get all active products from backend
                var allProducts = CompanyProductRestClient.GetAllProductDefinitions();

                // Filter only active products
                var activeProducts = allProducts.Where(p => p.IsActive).ToList();

                System.Diagnostics.Debug.WriteLine($"[GetPostLoginRedirect] Total products: {allProducts.Count}");
                System.Diagnostics.Debug.WriteLine($"[GetPostLoginRedirect] Active products: {activeProducts.Count}");

                // Store active product count in session (for hiding/showing product switcher)
                Session["ActiveProductCount"] = activeProducts.Count;

                // SCENARIO 1: Single Product Mode - Only 1 product is active
                if (activeProducts.Count == 1)
                {
                    var singleProduct = activeProducts.First();

                    // Set session for the single product
                    Session["SelectedProductId"] = singleProduct.ProductId;
                    Session["SelectedProductName"] = singleProduct.ProductName;
                    Session["SelectedProductCode"] = singleProduct.ProductCode;
                    Session["CurrentApiUrl"] = singleProduct.ApiUrl;
                    Session["IsSingleProductMode"] = true;

                    System.Diagnostics.Debug.WriteLine($"[GetPostLoginRedirect] ✅ SINGLE PRODUCT MODE");
                    System.Diagnostics.Debug.WriteLine($"[GetPostLoginRedirect] Product: {singleProduct.ProductName} (ID: {singleProduct.ProductId})");
                    System.Diagnostics.Debug.WriteLine($"[GetPostLoginRedirect] Redirecting to: {singleProduct.RedirectUrl}");

                    return Json(new
                    {
                        success = true,
                        mode = "single",
                        productId = singleProduct.ProductId,
                        productName = singleProduct.ProductName,
                        redirectUrl = singleProduct.RedirectUrl,
                        message = $"Welcome to {singleProduct.ProductName}"
                    }, JsonRequestBehavior.AllowGet);
                }

                // SCENARIO 2: Multi Product Mode - Multiple products active
                else if (activeProducts.Count > 1)
                {
                    // Find TNA product (default main product)
                    var tnaProduct = activeProducts.FirstOrDefault(p => p.ProductId == 1);

                    // If TNA is not active, use the first available product
                    var defaultProduct = tnaProduct ?? activeProducts.First();

                    // Set session for default product
                    Session["SelectedProductId"] = defaultProduct.ProductId;
                    Session["SelectedProductName"] = defaultProduct.ProductName;
                    Session["SelectedProductCode"] = defaultProduct.ProductCode;
                    Session["CurrentApiUrl"] = defaultProduct.ApiUrl;
                    Session["IsSingleProductMode"] = false;

                    System.Diagnostics.Debug.WriteLine($"[GetPostLoginRedirect] ✅ MULTI PRODUCT MODE");
                    System.Diagnostics.Debug.WriteLine($"[GetPostLoginRedirect] Active products: {string.Join(", ", activeProducts.Select(p => p.ProductName))}");
                    System.Diagnostics.Debug.WriteLine($"[GetPostLoginRedirect] Default product: {defaultProduct.ProductName}");
                    System.Diagnostics.Debug.WriteLine($"[GetPostLoginRedirect] Redirecting to: {defaultProduct.RedirectUrl}");

                    return Json(new
                    {
                        success = true,
                        mode = "multi",
                        productCount = activeProducts.Count,
                        productId = defaultProduct.ProductId,
                        productName = defaultProduct.ProductName,
                        redirectUrl = defaultProduct.RedirectUrl,
                        availableProducts = activeProducts.Select(p => new { p.ProductId, p.ProductName }).ToList(),
                        message = $"Welcome! You have access to {activeProducts.Count} products"
                    }, JsonRequestBehavior.AllowGet);
                }

                // SCENARIO 3: No Active Products - Fallback to TNA
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[GetPostLoginRedirect] ⚠️ No active products found, using fallback");

                    Session["IsSingleProductMode"] = false;
                    Session["ActiveProductCount"] = 0;

                    return Json(new
                    {
                        success = true,
                        mode = "fallback",
                        redirectUrl = "/Dashboard/AdminDashboard",
                        message = "No products configured. Using default dashboard."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetPostLoginRedirect] ❌ ERROR: {ex.Message}");

                // On error, fallback to default TNA dashboard
                return Json(new
                {
                    success = false,
                    mode = "error",
                    redirectUrl = "/Dashboard/AdminDashboard",
                    message = "An error occurred. Redirecting to default dashboard."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Checks if system is in single product mode
        /// Used to hide/show product switcher icon
        /// </summary>
        /// <returns>JSON with single product mode status</returns>
        [HttpGet]
        public JsonResult IsSingleProductMode()
        {
            try
            {
                bool isSingleMode = Session["IsSingleProductMode"] != null &&
                                   Convert.ToBoolean(Session["IsSingleProductMode"]);

                int activeProductCount = Session["ActiveProductCount"] != null ?
                                        Convert.ToInt32(Session["ActiveProductCount"]) : 0;

                return Json(new
                {
                    success = true,
                    isSingleProductMode = isSingleMode,
                    activeProductCount = activeProductCount,
                    showProductSwitcher = !isSingleMode && activeProductCount > 1
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IsSingleProductMode] Error: {ex.Message}");
                return Json(new
                {
                    success = false,
                    message = "An error occurred while checking product mode.",
                    isSingleProductMode = false,
                    showProductSwitcher = true
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetCurrentProduct()
        {
            JsonResult result;
            try
            {
                bool flag = base.Session["SelectedProductId"] != null;
                if (flag)
                {
                    int productId = Convert.ToInt32(base.Session["SelectedProductId"]);
                    Product productById = GetProductById(productId);
                    result = base.Json(new
                    {
                        success = true,
                        data = new
                        {
                            productId = productById.ProductId,
                            productName = productById.ProductName,
                            productCode = productById.ProductCode,
                            iconClass = productById.IconClass
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    result = base.Json(new
                    {
                        success = true,
                        data = new
                        {
                            productId = 1,
                            productName = "Time & Attendance",
                            productCode = "TNA",
                            iconClass = "icon-clock"
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetCurrentProduct] Error: {ex.Message}");
                result = base.Json(new
                {
                    success = false,
                    message = "An error occurred while getting current product."
                }, JsonRequestBehavior.AllowGet);
            }
            return result;
        }

        private Product GetProductById(int productId)
        {
            int userId = (base.Session["UserId"] != null) ? Convert.ToInt32(base.Session["UserId"]) : 0;
            int companyId = (base.Session["CompanyId"] != null) ? Convert.ToInt32(base.Session["CompanyId"]) : 0;
            List<Product> productsForUser = this.GetProductsForUser(userId, companyId);
            return productsForUser.FirstOrDefault((Product p) => p.ProductId == productId);
        }

        #region For Add Product PlanPricing
        public ActionResult AddProduct()
        {
            Plan pl = new Plan();
            Getpara obj = new Getpara();

            try
            {
                planRestrictions(0);
                int companyid = Convert.ToInt32(Session["CompanyId"]);
                obj.pkid = companyid;
                obj.para = 1;
                obj.iscount = 0;
                obj.IScurrencycode = Convert.ToInt32(Session["IScurrencycode"]);

                if (TempData["pl"] != null)
                {
                    pl = TempData["pl"] as Plan;
                    ViewBag.isAdduser = 1;
                }
                else
                {
                    ViewBag.isAdduser = 0;
                    pl.PlanList = RestClient.PlanGetAll(obj);
                }
                return View(pl);                
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }            
        }
        private void planRestrictions(int mcmpid)
        {
            Getpara gp = new Getpara();
            gp.pkid = mcmpid;
            gp.para = 0;
            gp.iscount = 0;
            gp.companycode = Convert.ToString(Session["cmpcode"]);
            int nouser = 0;
            int isflg = 0;
            int noemp = 0;
            int isFace = 0;
            int isEss = 0;
            int isExp = 0;
            int isFplan = 0;
            int planid = 0;
            int rmDay = 0;
            int pDursn = 0;
            int isRecurring = 0;
            string rpcustid = "";
            string rptokenid = "";
            string Isman = "";
            int isStandard = 0;
            int iscurrencycode = 0;
            DataTable dtsub = RestClient.Restrictionasperplan(gp);
            if (dtsub != null && dtsub.Rows.Count > 0)
            {
                nouser = Convert.ToInt32(dtsub.Rows[0]["UseCount"]);
                isflg = Convert.ToInt32(dtsub.Rows[0]["islimit"]);
                noemp = Convert.ToInt32(dtsub.Rows[0]["empctn"]);
                isFace = Convert.ToInt32(dtsub.Rows[0]["isFace"]);
                isEss = Convert.ToInt32(dtsub.Rows[0]["isEss"]);
                isExp = Convert.ToInt32(dtsub.Rows[0]["Substs"]);
                isFplan = Convert.ToInt32(dtsub.Rows[0]["isFplan"]);
                planid = Convert.ToInt32(dtsub.Rows[0]["Planid"]);
                rmDay = Convert.ToInt32(dtsub.Rows[0]["Rmnday"]);
                pDursn = Convert.ToInt32(dtsub.Rows[0]["Pdrsn"]);
                rpcustid = dtsub.Rows[0]["rpcustid"].ToString();
                rptokenid = dtsub.Rows[0]["rptokenid"].ToString();
                isRecurring = Convert.ToInt32(dtsub.Rows[0]["IsRec"]);
                Isman = dtsub.Rows[0]["Isman"].ToString();
                isStandard = Convert.ToInt32(dtsub.Rows[0]["IsStandard"]);
                iscurrencycode = Convert.ToInt32(dtsub.Rows[0]["IScurrencycode"]);
            }

            Session["UseCount"] = nouser;
            Session["IsUserlimit"] = isflg;
            Session["Noofemp"] = noemp;
            Session["isPlanFace"] = isFace;
            Session["isPlanEss"] = isEss;
            Session["isPlanExp"] = isExp;
            Session["isFplan"] = isFplan;
            Session["planId"] = planid;
            Session["rmDay"] = rmDay;
            Session["Pdrsn"] = pDursn;
            Session["rzpcustid"] = rpcustid;
            Session["rzptokenid"] = rptokenid;
            Session["isRecurring"] = isRecurring;
            Session["isMan"] = Isman;
            Session["isStandard"] = isStandard;
            Session["IScurrencycode"] = iscurrencycode;
        }
        #endregion

    }
}
