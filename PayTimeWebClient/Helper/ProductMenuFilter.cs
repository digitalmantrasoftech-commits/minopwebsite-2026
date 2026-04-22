using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PayTimeWebClient.Models;

namespace PayTimeWebClient.Helper
{
    public class ProductMenuFilter
    {
        #region Common Methods for GetMenuRoleWise

        /// <summary>
        /// Determines if a menu item should be visible based on ProductId field from database.
        /// This method handles comma-separated ProductIds and special cases like Payroll Preparation.
        /// Called from ServerDataRestClient.GetMenuRoleWise to centralize product filtering logic.
        /// </summary>
        /// <param name="menu">The menu item with OperationName, ParentMenuName, and ProductId</param>
        /// <param name="productId">Current selected product ID</param>
        /// <returns>True if menu should be visible for the given product</returns>
        public static bool IsMenuVisibleByProductId(RoleRightsMapping menu, int productId)
        {
            // Dynamic filtering based on ProductId field from database (supports comma-separated values)
            if (!string.IsNullOrEmpty(menu.ProductId))
            {
                // Split comma-separated ProductIds and trim whitespace
                var productIds = menu.ProductId.Split(',').Select(p => p.Trim()).ToList();

                // FIX (10-Dec-2025): ProductId = "0" means TNA only (not global)
                // If menu has "0", treat it as "1" (TNA product)
                // This prevents TNA menus from showing in Payroll, EMS, FieldSens etc.
                if (productIds.Contains("0"))
                {
                    productIds.Remove("0");
                    productIds.Add("1");  // Convert 0 to TNA (ProductId=1)
                }

                // Menu visible only if current productId matches
                return productIds.Contains(productId.ToString());
            }
            else
            {
                // If ProductId is null/empty, default to TNA (ProductId=1)
                return (productId == 1);
            }
        }

        /// <summary>
        /// Applies the appropriate dashboard URL to a menu item based on productId.
        /// Updates menu.ActionUrl for dashboard menus to point to product-specific dashboards.
        /// Called from ServerDataRestClient.GetMenuRoleWise to centralize ActionUrl mapping logic.
        /// </summary>
        /// <param name="menu">The menu item to update</param>
        /// <param name="productId">Current selected product ID</param>
        public static void ApplyDashboardUrl(RoleRightsMapping menu, int productId)
        {
            if (menu.OperationName == null)
                return;

            // Generic "Dashboard" menu - redirect based on selected product
            bool isDashboard = menu.OperationName.Equals("Dashboard", StringComparison.OrdinalIgnoreCase);
            if (isDashboard)
            {
                switch (productId)
                {
                    case 2:
                        menu.ActionUrl = "/MinopProduct/PayrollDashboard";
                        break;
                    case 3:
                        menu.ActionUrl = "/MinopProduct/EMSDashboard";
                        break;                    
                    case 4:
                        menu.ActionUrl = "/MinopProduct/PMSDashboard";
                        break;
                    case 5:
                        menu.ActionUrl = "/MinopProduct/FieldSensDashboard";
                        break;
                    case 6:
                        menu.ActionUrl = "/MinopProduct/CMSDashboard";
                        break;
                    case 7:
                        menu.ActionUrl = "/MinopProduct/VMSDashboard";
                        break;
                    case 8:
                        menu.ActionUrl = "/MinopProduct/OKRDashboard";
                        break;
                }
                return;
            }

            // Payroll Dashboard
            bool isPayrollDashboard = menu.OperationName.Equals("Payroll Dashboard", StringComparison.OrdinalIgnoreCase);
            if (isPayrollDashboard)
            {
                menu.ActionUrl = "/MinopProduct/PayrollDashboard";
                return;
            }

            // EMS Dashboard
            bool isEMSDashboard = menu.OperationName.Equals("EMS Dashboard", StringComparison.OrdinalIgnoreCase);
            if (isEMSDashboard)
            {
                menu.ActionUrl = "/MinopProduct/EMSDashboard";
                return;
            }

            // Expense Dashboard
            bool isExpenseDashboard = menu.OperationName.Equals("Expense Dashboard", StringComparison.OrdinalIgnoreCase);
            if (isExpenseDashboard)
            {
                menu.ActionUrl = "/MinopProduct/EMSDashboard";
                return;
            }

            // OKR Dashboard
            bool isOKRDashboard = menu.OperationName.Equals("OKR Dashboard", StringComparison.OrdinalIgnoreCase);
            if (isOKRDashboard)
            {
                menu.ActionUrl = "/MinopProduct/OKRDashboard";
                return;
            }

            // Performance Dashboard
            bool isPerformanceDashboard = menu.OperationName.Equals("Performance Dashboard", StringComparison.OrdinalIgnoreCase);
            if (isPerformanceDashboard)
            {
                menu.ActionUrl = "/MinopProduct/PMSDashboard";
                return;
            }
        }

		#endregion

		#region Methods for RightDistribution Product-wise Grouping

		/// <summary>
		/// Groups menus by product for RightDistribution page.
		/// Menus with comma-separated ProductIds appear in multiple product groups.
		/// </summary>
		/// <param name="menus">All menus from RoleRightsGetAll</param>
		/// <param name="activeProducts">List of active products</param>
		/// <returns>List of ProductMenuGroup with menus grouped by product</returns>
		public static List<ProductMenuGroup> GroupMenusByProduct(
			IEnumerable<RoleRightsMapping> menus,
			List<Product> activeProducts)
		{
			List<ProductMenuGroup> result = new List<ProductMenuGroup>();

			if (menus == null || !menus.Any())
				return result;

			// Filter menus - only those with OperationId > 0 and ParentMenuId != 0
			var filteredMenus = menus
				.Where(x => x.OperationId > 0 && x.ParentMenuId != 0)
				.ToList();

			// Create dictionary for product wise menus
			Dictionary<int, List<RoleRightsMapping>> productMenus =
				new Dictionary<int, List<RoleRightsMapping>>();

			productMenus[0] = new List<RoleRightsMapping>();

			foreach (var product in activeProducts)
			{
				productMenus[product.ProductId] = new List<RoleRightsMapping>();
			}

			// Assign menus to products
			foreach (var menu in filteredMenus)
			{
				var productIds = GetProductIdsFromMenu(menu);

				foreach (var pid in productIds)
				{
					if (productMenus.ContainsKey(pid))
					{
						productMenus[pid].Add(menu);
					}
				}
			}

			// Build product menu groups
			foreach (var product in activeProducts.OrderBy(p => p.ProductId))
			{
				if (productMenus.ContainsKey(product.ProductId) &&
					productMenus[product.ProductId].Any())
				{
					var group = new ProductMenuGroup
					{
						ProductId = product.ProductId,
						ProductName = product.ProductName,
						ProductCode = product.ProductCode,
						IconClass = product.IconClass ??
									GetDefaultIconForProduct(product.ProductId),
						IsExpanded = product.ProductId == 1,
						Menus = productMenus[product.ProductId]
					};

					group.ParentMenuGroups = group.Menus
						.Where(m => !string.IsNullOrEmpty(m.ParentMenuName))
						.GroupBy(m => m.ParentMenuName)
						.Select(g => new ParentMenuGroup
						{
							ParentMenuName = g.Key,
							ChildMenus = g.ToList()
						})
						.OrderBy(g => g.ParentMenuName)
						.ToList();

					result.Add(group);
				}
			}

			// Other menus
			if (productMenus[0].Any())
			{
				var otherGroup = new ProductMenuGroup
				{
					ProductId = 0,
					ProductName = "Other Menus",
					ProductCode = "OTHER",
					IconClass = "fa-solid fa-globe",
					IsExpanded = false,
					Menus = productMenus[0]
				};

				otherGroup.ParentMenuGroups = otherGroup.Menus
					.Where(m => !string.IsNullOrEmpty(m.ParentMenuName))
					.GroupBy(m => m.ParentMenuName)
					.Select(g => new ParentMenuGroup
					{
						ParentMenuName = g.Key,
						ChildMenus = g.ToList()
					})
					.OrderBy(g => g.ParentMenuName)
					.ToList();

				result.Add(otherGroup);
			}

			return result;
		}



		/// <summary>
		/// Extracts product IDs from a menu's ProductId field.
		/// Handles comma-separated values and null/empty cases.
		/// </summary>
		public static List<int> GetProductIdsFromMenu(RoleRightsMapping menu)
        {
            var productIds = new List<int>();

            if (string.IsNullOrEmpty(menu.ProductId))
            {
                // If ProductId is null/empty, determine based on legacy logic or default to TNA
                productIds.Add(1); // Default to TNA
                return productIds;
            }

            // Split comma-separated ProductIds
            var parts = menu.ProductId.Split(',');
            foreach (var part in parts)
            {
                if (int.TryParse(part.Trim(), out int id))
                {
                    // ProductId = 0 means TNA (Product 1) in database
                    if (id == 0)
                    {
                        productIds.Add(1);
                    }
                    else
                    {
                        productIds.Add(id);
                    }
                }
            }

            // If no valid IDs found, default to TNA
            if (!productIds.Any())
            {
                productIds.Add(1);
            }

            return productIds;
        }

        /// <summary>
        /// Returns default Font Awesome icon class for a product
        /// </summary>
        public static string GetDefaultIconForProduct(int productId)
        {
            switch (productId)
            {
                case 1: return "fa-solid fa-clock";           // TNA
                case 2: return "fa-solid fa-money-bill-wave"; // Payroll
                case 3: return "fa-solid fa-receipt";         // EMS                
                case 4: return "fa-solid fa-chart-line";      // PMS
                case 5: return "fa-solid fa-location-dot";    // FieldSens
                case 6: return "fa-solid fa-utensils";        // CMS
                case 7: return "fa-solid fa-id-card-clip";    // VMS
                case 8: return "fa-solid fa-bullseye";        // OKR
                default: return "fa-solid fa-cube";
            }
        }

        #endregion

        public static bool IsMenuVisibleForProduct(string menuOperationName, string parentMenuName, int productId)
			{
			//	bool flag = string.IsNullOrEmpty(menuOperationName);
			//	bool result;
			//	if (flag)
			//	{
			//		result = false;
			//	}
			//	else
			//	{
			//		bool flag2 = menuOperationName != null && (menuOperationName.IndexOf("Payroll", StringComparison.OrdinalIgnoreCase) >= 0 || menuOperationName.IndexOf("Expense", StringComparison.OrdinalIgnoreCase) >= 0 || menuOperationName.IndexOf("EMS", StringComparison.OrdinalIgnoreCase) >= 0);
			//		if (flag2)
			//		{						
			//		}
			//		bool flag3 = menuOperationName != null && menuOperationName.Equals("Payroll Preperation", StringComparison.OrdinalIgnoreCase);
			//		if (flag3)
			//		{
			//			result = (productId == 2);
			//		}
			//		else
			//		{
			//			bool flag4 = !string.IsNullOrEmpty(parentMenuName) && parentMenuName.Equals("Payroll Preperation", StringComparison.OrdinalIgnoreCase);
			//			if (flag4)
			//			{
			//				result = (productId == 2);
			//			}
			//			else
			//			{
			//				bool flag5 = ProductMenuFilter.IsCommonMenu(menuOperationName);
			//				if (flag5)
			//				{
			//					result = true;
			//				}
			//				else
			//				{
			//					bool flag6;
			//					switch (productId)
			//					{
			//						case 1:
			//							flag6 = ProductMenuFilter.IsTNAMenu(menuOperationName, parentMenuName);
			//							break;
			//						case 2:
			//							flag6 = ProductMenuFilter.IsPayrollMenu(menuOperationName, parentMenuName);
			//							break;
			//						case 3:
			//							flag6 = ProductMenuFilter.IsEMSMenu(menuOperationName, parentMenuName);
			//							break;
			//						case 4:
			//							flag6 = ProductMenuFilter.IsOKRMenu(menuOperationName, parentMenuName);
			//							break;
			//						case 5:
			//							flag6 = ProductMenuFilter.IsPMSMenu(menuOperationName, parentMenuName);
			//							break;
			//						case 6:
			//							flag6 = ProductMenuFilter.IsFieldSensMenu(menuOperationName, parentMenuName);
			//							break;
			//						default:
			//							result = true;
			//							return result;
			//					}
			//					bool flag7 = flag6;
			//					if (flag7)
			//					{
			//						result = true;
			//					}
			//					else
			//					{
			//						bool flag8 = productId != 1;
			//						if (flag8)
			//						{
			//							string[] source = new string[]
			//							{
			//							"Admin Dashboard",
			//							"Employee Dashboard",
			//							"Analytics Dashboard",
			//							"Announcements"
			//							};
			//							bool flag9 = source.Any((string m) => m.Equals(menuOperationName, StringComparison.OrdinalIgnoreCase));
			//							if (flag9)
			//							{
			//								result = false;
			//								return result;
			//							}
			//						}
			//						bool flag10 = productId != 2 && menuOperationName.Equals("Payroll Dashboard", StringComparison.OrdinalIgnoreCase);
			//						if (flag10)
			//						{
			//							result = false;
			//						}
			//						else
			//						{
			//							bool flag11 = productId != 3;
			//							if (flag11)
			//							{
			//								bool flag12 = menuOperationName.Equals("EMS Dashboard", StringComparison.OrdinalIgnoreCase) || menuOperationName.Equals("Expense Dashboard", StringComparison.OrdinalIgnoreCase);
			//								if (flag12)
			//								{
			//									result = false;
			//									return result;
			//								}
			//							}
			//							bool flag13 = productId != 4 && menuOperationName.Equals("OKR Dashboard", StringComparison.OrdinalIgnoreCase);
			//							if (flag13)
			//							{
			//								result = false;
			//							}
			//							else
			//							{
			//								bool flag14 = productId != 5;
			//								if (flag14)
			//								{
			//									bool flag15 = menuOperationName.Equals("PMS Dashboard", StringComparison.OrdinalIgnoreCase) || menuOperationName.Equals("Performance Dashboard", StringComparison.OrdinalIgnoreCase);
			//									if (flag15)
			//									{
			//										result = false;
			//										return result;
			//									}
			//								}
			//								bool flag16 = productId != 6;
			//								if (flag16)
			//								{
			//									bool flag17 = menuOperationName.Equals("FieldSens Dashboard", StringComparison.OrdinalIgnoreCase) || menuOperationName.Equals("Field Service Dashboard", StringComparison.OrdinalIgnoreCase);
			//									if (flag17)
			//									{
			//										result = false;
			//										return result;
			//									}
			//								}
			//								bool flag18 = !string.IsNullOrEmpty(parentMenuName);
			//								if (flag18)
			//								{
			//									bool flag19 = ProductMenuFilter.IsMenuVisibleForProduct(parentMenuName, string.Empty, productId);
			//									bool flag20 = flag19;
			//									if (flag20)
			//									{
			//										result = true;
			//										return result;
			//									}
			//								}
			//								bool flag21 = menuOperationName != null && menuOperationName.IndexOf("Payroll", StringComparison.OrdinalIgnoreCase) >= 0;
			//								if (flag21)
			//								{
			//									//Debug.WriteLine(string.Format("[MENU FILTER RESULT] Menu: '{0}' → HIDDEN in Product {1}", menuOperationName, productId));
			//								}
			//								result = false;
			//							}
			//						}
			//					}
			//				}
			//			}
			//		}
			//	}
			//	return result;
			//}

			//private static bool IsCommonMenu(string menuName)
			//{
			//	string[] source = new string[]
			//	{
			//	"Profile",
			//	"My Profile",
			//	"Account Settings",
			//	"Settings",
			//	"Notifications",
			//	"Help",
			//	"Support",
			//	"Change Password",
			//	"Logout",
			//	"Master",
			//	"Report"
			//	};
			//	return source.Any((string m) => m.Equals(menuName, StringComparison.OrdinalIgnoreCase));
			//}

			//private static bool IsTNAMenu(string menuName, string parentMenuName)
			//{
			//	string[] source = new string[]
			//	{
			//	"Payroll",
			//	"Payroll Management",
			//	"Payroll Dashboard",
			//	"Payrun",
			//	"Pay Run",
			//	"Payroll Preperation",
			//	"Payroll Processing",
			//	"Salary Structure",
			//	"Salary Components",
			//	"Pay Grade",
			//	"Pay Scale",
			//	"Salary Revision",
			//	"Increment",
			//	"Bonus",
			//	"Arrears",
			//	"Tax Configuration",
			//	"PF Configuration",
			//	"ESI Configuration",
			//	"Form 16",
			//	"Form 24Q",
			//	"Tax Report",
			//	"Payslip",
			//	"Pay Register",
			//	"FNF Flow",
			//	"Full and Final",
			//	"FNF Settlement",
			//	"F&F",
			//	"Exit Process"
			//	};
			//	string[] source2 = new string[]
			//	{
			//	"Expenses",
			//	"Expense Management",
			//	"EMS Dashboard",
			//	"Expense Dashboard",
			//	"EMS",
			//	"EMS Management",
			//	"EMS Preparation",
			//	"Expense Preparation",
			//	"Expense Request",
			//	"Expense Approval",
			//	"Expense Claim",
			//	"Trip Management",
			//	"Travel Approval",
			//	"Expense Category",
			//	"Expense Policy",
			//	"Per Diem",
			//	"Mileage",
			//	"Reimbursement",
			//	"Expense Report",
			//	"Trip Report"
			//	};
			//	string[] source3 = new string[]
			//	{
			//	"OKR",
			//	"OKR Management",
			//	"OKR Dashboard",
			//	"Objectives",
			//	"Objective Management",
			//	"Create Objective",
			//	"Add Objective",
			//	"My Objectives",
			//	"Team Objectives",
			//	"Company Objectives",
			//	"Key Results",
			//	"Key Result",
			//	"KR",
			//	"OKR Alignment",
			//	"Objective Alignment",
			//	"OKR Progress",
			//	"OKR Tracking",
			//	"OKR Review",
			//	"OKR Report",
			//	"OKR Analytics"
			//	};
			//	string[] source4 = new string[]
			//	{
			//	"Performance",
			//	"Performance Management",
			//	"PMS Dashboard",
			//	"Performance Dashboard",
			//	"PMS",
			//	"Appraisals",
			//	"Appraisal",
			//	"Appraisal Forms",
			//	"Appraisal Form",
			//	"Goals",
			//	"Goal",
			//	"Goal Setting",
			//	"Set Goals",
			//	"Reviews",
			//	"Review",
			//	"Performance Review",
			//	"Annual Review",
			//	"Mid-Year Review",
			//	"Feedback",
			//	"Give Feedback",
			//	"360 Feedback",
			//	"360 Review",
			//	"Competency",
			//	"Competency Matrix",
			//	"Performance Report",
			//	"Performance Analytics"
			//	};
			//	string[] source5 = new string[]
			//	{
			//	"FieldSens",
			//	"Field Service",
			//	"FieldSens Dashboard",
			//	"Field Service Dashboard",
			//	"Field Tracking",
			//	"Field Service Management",
			//	"Field Management",
			//	"Field Technicians",
			//	"Technician Management",
			//	"Field Workers",
			//	"Work Orders",
			//	"Work Order",
			//	"Work Order Management",
			//	"Route Optimization",
			//	"Routes",
			//	"Route Planning",
			//	"Field Map",
			//	"Location Tracking",
			//	"GPS Tracking",
			//	"Service Requests",
			//	"Service Request",
			//	"Customer Service",
			//	"Field Reports",
			//	"Field Service Reports",
			//	"Technician Reports",
			//	"Asset Management",
			//	"Field Assets",
			//	"Job Scheduling",
			//	"Schedule Jobs",
			//	"Task Assignment",
			//	"Time Tracking",
			//	"Timesheet",
			//	"Field Timesheet"
			//	};
			//	bool flag = source.Any((string m) => m.Equals(menuName, StringComparison.OrdinalIgnoreCase));
			//	bool result;
			//	if (flag)
			//	{
			//		result = false;
			//	}
			//	else
			//	{
			//		bool flag2 = source2.Any((string m) => m.Equals(menuName, StringComparison.OrdinalIgnoreCase));
			//		if (flag2)
			//		{
			//			result = false;
			//		}
			//		else
			//		{
			//			bool flag3 = source3.Any((string m) => m.Equals(menuName, StringComparison.OrdinalIgnoreCase));
			//			if (flag3)
			//			{
			//				result = false;
			//			}
			//			else
			//			{
			//				bool flag4 = source4.Any((string m) => m.Equals(menuName, StringComparison.OrdinalIgnoreCase));
			//				if (flag4)
			//				{
			//					result = false;
			//				}
			//				else
			//				{
			//					bool flag5 = source5.Any((string m) => m.Equals(menuName, StringComparison.OrdinalIgnoreCase));
			//					if (flag5)
			//					{
			//						result = false;
			//					}
			//					else
			//					{
			//						bool flag6 = !string.IsNullOrEmpty(parentMenuName);
			//						if (flag6)
			//						{
			//							bool flag7 = source.Any((string m) => m.Equals(parentMenuName, StringComparison.OrdinalIgnoreCase));
			//							if (flag7)
			//							{
			//								result = false;
			//								return result;
			//							}
			//							bool flag8 = source2.Any((string m) => m.Equals(parentMenuName, StringComparison.OrdinalIgnoreCase));
			//							if (flag8)
			//							{
			//								result = false;
			//								return result;
			//							}
			//							bool flag9 = source3.Any((string m) => m.Equals(parentMenuName, StringComparison.OrdinalIgnoreCase));
			//							if (flag9)
			//							{
			//								result = false;
			//								return result;
			//							}
			//							bool flag10 = source4.Any((string m) => m.Equals(parentMenuName, StringComparison.OrdinalIgnoreCase));
			//							if (flag10)
			//							{
			//								result = false;
			//								return result;
			//							}
			//							bool flag11 = source5.Any((string m) => m.Equals(parentMenuName, StringComparison.OrdinalIgnoreCase));
			//							if (flag11)
			//							{
			//								result = false;
			//								return result;
			//							}
			//						}
			//						result = true;
			//					}
			//				}
			//			}
			//		}
			//	}
			//	return result;
			//}

			//private static bool IsPayrollMenu(string menuName, string parentMenuName)
			//{
			//	string[] source = new string[]
			//	{
			//	"Dashboard",
			//	"Payroll Dashboard",
			//	"Payroll",
			//	"Payroll Management",
			//	"Payrun",
			//	"Pay Run",
			//	"Payroll Preperation",
			//	"Payroll Processing",
			//	"Salary Structure",
			//	"Salary Master",
			//	"Salary Component",
			//	"Salary Components",
			//	"Salary Processing",
			//	"Process Salary",
			//	"Salary Slip",
			//	"Salary Slips",
			//	"Pay Slip",
			//	"Salary Register",
			//	"Payroll Register",
			//	"Tax Configuration",
			//	"Tax Master",
			//	"Tax Calculation",
			//	"Tax Deduction",
			//	"Tax Reports",
			//	"PF Configuration",
			//	"PF Master",
			//	"ESI Configuration",
			//	"ESI Master",
			//	"Payment Processing",
			//	"Payment Gateway",
			//	"Bank Transfer",
			//	"Bank Payment",
			//	"Payroll Reports",
			//	"Salary Report",
			//	"Employee Salary",
			//	"Employee Salary Master",
			//	"Increment",
			//	"Salary Increment",
			//	"Bonus",
			//	"Bonus Configuration",
			//	"Arrears",
			//	"Salary Arrears",
			//	"Deductions",
			//	"Salary Deductions",
			//	"Advance",
			//	"Advance Payment",
			//	"Loan Management",
			//	"Loan Configuration",
			//	"Employee Loan",
			//	"Reimbursement",
			//	"Salary Reimbursement",
			//	"Final Settlement",
			//	"Full and Final",
			//	"F&F Settlement",
			//	"FNF Flow",
			//	"Exit Process",
			//	"Form 16",
			//	"Form 16A",
			//	"TDS Certificate",
			//	"Gratuity",
			//	"Gratuity Configuration",
			//	"CTC Configuration",
			//	"CTC Structure",
			//	"Variable Pay",
			//	"Incentive"
			//	};
			//	string[] source2 = new string[]
			//	{
			//	"Payroll",
			//	"Payroll Management",
			//	"Payrun",
			//	"Payroll Preparation",
			//	"FNF Flow"
			//	};
			//	bool flag = !string.IsNullOrEmpty(parentMenuName) && source2.Any((string p) => p.Equals(parentMenuName, StringComparison.OrdinalIgnoreCase));
			//	return flag || source.Any((string m) => m.Equals(menuName, StringComparison.OrdinalIgnoreCase));
			//}

			//private static bool IsEMSMenu(string menuName, string parentMenuName)
			//{
			//	string[] source = new string[]
			//	{
			//	"Dashboard",
			//	"EMS Dashboard",
			//	"Expense Dashboard",
			//	"Expenses",
			//	"Expense",
			//	"Expense Management",
			//	"EMS",
			//	"EMS Management",
			//	"EMS Preparation",
			//	"Expense Preparation",
			//	"ExpenseManagement",
			//	"Create Expense",
			//	"Add Expense",
			//	"New Expense",
			//	"Expense Entry",
			//	"Submit Expense",
			//	"My Expenses",
			//	"Expense List",
			//	"Expense History",
			//	"View Expenses",
			//	"All Expenses",
			//	"Expense Master",
			//	"Expense Approvals",
			//	"Approve Expense",
			//	"Expense Approval",
			//	"Expense Pending",
			//	"Pending Approvals",
			//	"Expense Reports",
			//	"Expense Report",
			//	"Expense Summary",
			//	"Expense Categories",
			//	"Expense Category",
			//	"Expense Type",
			//	"Expense Policy",
			//	"Expense Configuration",
			//	"Expense Settings",
			//	"Expense Master Data",
			//	"Expense Setup",
			//	"Expense Claims",
			//	"Claim Expense",
			//	"Claims",
			//	"Reimbursement",
			//	"Expense Reimbursement",
			//	"Reimburse",
			//	"Travel Expense",
			//	"Travel Management",
			//	"Trip",
			//	"Travel",
			//	"Conveyance",
			//	"Conveyance Expense",
			//	"Per Diem",
			//	"Daily Allowance",
			//	"Hotel Booking",
			//	"Travel Booking",
			//	"Expense Analysis",
			//	"Expense Analytics",
			//	"Expense Budget",
			//	"Budget Configuration",
			//	"Advance Request",
			//	"Travel Advance",
			//	"Advance",
			//	"Mileage",
			//	"Mileage Configuration"
			//	};
			//	string[] source2 = new string[]
			//	{
			//	"Expenses",
			//	"Expense",
			//	"Expense Management",
			//	"EMS",
			//	"EMS Preparation",
			//	"Expense Preparation",
			//	"ExpenseManagement"
			//	};
			//	bool flag = !string.IsNullOrEmpty(parentMenuName) && source2.Any((string p) => p.Equals(parentMenuName, StringComparison.OrdinalIgnoreCase));
			//	return flag || source.Any((string m) => m.Equals(menuName, StringComparison.OrdinalIgnoreCase));
			//}

			//private static bool IsOKRMenu(string menuName, string parentMenuName)
			//{
			//	string[] source = new string[]
			//	{
			//	"Dashboard",
			//	"PMS",
			//	"OKR Report",
			//	"PMS Dashboard",
			//	"OKR",
			//	"OKR Management",
			//	"Objectives",
			//	"Objective Management",
			//	"Create Objective",
			//	"Add Objective",
			//	"My Objectives",
			//	"Team Objectives",
			//	"Company Objectives",
			//	"Key Results",
			//	"Key Result",
			//	"KR",
			//	"Progress Tracking",
			//	"Track Progress",
			//	"Progress Update",
			//	"Team Alignment",
			//	"Alignment",
			//	"OKR Reports",
			//	"OKR Report",
			//	"Goal Setting",
			//	"Set Goals",
			//	"Quarterly Review",
			//	"Q1 Review",
			//	"OKR DepartmentWise Report",
			//	"Q4 Review",
			//	"Annual Review",
			//	"OKR Assign",
			//	"OKR Generate",
			//	"OKR Review",
			//	"My OKR"
			//	};
			//	string[] source2 = new string[]
			//	{
			//	"OKR",
			//	"Objectives"
			//	};
			//	bool flag = !string.IsNullOrEmpty(parentMenuName) && source2.Any((string p) => p.Equals(parentMenuName, StringComparison.OrdinalIgnoreCase));
			//	return flag || source.Any((string m) => m.Equals(menuName, StringComparison.OrdinalIgnoreCase));
			//}

			//private static bool IsPMSMenu(string menuName, string parentMenuName)
			//{
			//	string[] source = new string[]
			//	{
			//	"Dashboard",
			//	"PMS Dashboard",
			//	"Performance Dashboard",
			//	"Performance",
			//	"Performance Management",
			//	"PMS",
			//	"Appraisals",
			//	"Appraisal",
			//	"Appraisal Forms",
			//	"Appraisal Form",
			//	"Create Appraisal",
			//	"Add Appraisal",
			//	"My Appraisals",
			//	"Team Appraisals",
			//	"Goals",
			//	"Goal",
			//	"Goal Setting",
			//	"Set Goals",
			//	"My Goals",
			//	"Team Goals",
			//	"Goal Management",
			//	"Reviews",
			//	"Review",
			//	"Performance Review",
			//	"Annual Review",
			//	"Mid-Year Review",
			//	"Quarterly Review",
			//	"Feedback",
			//	"Give Feedback",
			//	"360 Feedback",
			//	"360 Review",
			//	"Competency",
			//	"Competency Matrix",
			//	"Skill Matrix",
			//	"Development Plan",
			//	"IDP",
			//	"Individual Development Plan",
			//	"Performance Report",
			//	"Performance Analytics",
			//	"Performance Insights",
			//	"Rating",
			//	"Rating Scale",
			//	"Performance Rating",
			//	"Self Assessment",
			//	"Manager Assessment",
			//	"Peer Review"
			//	};
			//	string[] source2 = new string[]
			//	{
			//	"Performance",
			//	"PMS"
			//	};
			//	bool flag = !string.IsNullOrEmpty(parentMenuName) && source2.Any((string p) => p.Equals(parentMenuName, StringComparison.OrdinalIgnoreCase));
			//	return flag || source.Any((string m) => m.Equals(menuName, StringComparison.OrdinalIgnoreCase));
			//}

			//private static bool IsFieldSensMenu(string menuName, string parentMenuName)
			//{
			//	string[] source = new string[]
			//	{
			//	"Dashboard",
			//	"FieldSens Dashboard",
			//	"Field Service Dashboard",
			//	"FieldSens",
			//	"Field Service",
			//	"Field Service Management",
			//	"Field Tracking",
			//	"Clients",
			//	"Client Management",
			//	"Client Master",
			//	"Task Management",
			//	"Tasks",
			//	"Task Master",
			//	"Employee Journey",
			//	"Journey",
			//	"Field Employee Journey",
			//	"Work Policy",
			//	"Work Policy Configuration",
			//	"Work Policy Master",
			//	"View Work Policy",
			//	"Work Policy List",
			//	"Work Policy Allocation",
			//	"Allocate Work Policy",
			//	"Field Technicians",
			//	"Technician Management",
			//	"Field Workers",
			//	"Technician List",
			//	"Work Orders",
			//	"Work Order",
			//	"Work Order Management",
			//	"Create Work Order",
			//	"Route Optimization",
			//	"Routes",
			//	"Route Planning",
			//	"Optimize Routes",
			//	"Field Map",
			//	"Location Tracking",
			//	"GPS Tracking",
			//	"Live Tracking",
			//	"Service Requests",
			//	"Service Request",
			//	"Customer Requests",
			//	"Field Reports",
			//	"Field Service Reports",
			//	"Technician Reports",
			//	"Service Reports",
			//	"Asset Management",
			//	"Field Assets",
			//	"Equipment Management",
			//	"Job Scheduling",
			//	"Schedule Jobs",
			//	"Task Assignment",
			//	"Assign Tasks",
			//	"Time Tracking",
			//	"Timesheet",
			//	"Field Timesheet",
			//	"Technician Timesheet",
			//	"Customer Management",
			//	"Customer Service",
			//	"Service History",
			//	"Inventory Management",
			//	"Parts Inventory",
			//	"Stock Management",
			//	"Field Analytics",
			//	"Service Analytics",
			//	"Performance Metrics"
			//	};
			//	string[] source2 = new string[]
			//	{
			//	"FieldSens",
			//	"Field Service",
			//	"Field Tracking",
			//	"Field Service Management"
			//	};
			//	bool flag = !string.IsNullOrEmpty(parentMenuName) && source2.Any((string p) => p.Equals(parentMenuName, StringComparison.OrdinalIgnoreCase));
			//	return flag || source.Any((string m) => m.Equals(menuName, StringComparison.OrdinalIgnoreCase));

			return false;
			}

		}
	
}




