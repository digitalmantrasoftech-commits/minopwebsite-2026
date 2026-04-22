using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    /// <summary>
    /// ViewModel for RightDistribution page with product-wise menu grouping
    /// </summary>
    public class ProductMenuRightsViewModel
    {
        public ProductMenuRightsViewModel()
        {
            ProductMenuGroups = new List<ProductMenuGroup>();
            ActiveProducts = new List<Product>();
        }

        /// <summary>
        /// Selected Role ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// List of menu groups organized by product
        /// </summary>
        public List<ProductMenuGroup> ProductMenuGroups { get; set; }

        /// <summary>
        /// List of active products available in the system
        /// </summary>
        public List<Product> ActiveProducts { get; set; }

        /// <summary>
        /// Flattened list of all menus across all product groups (for saving permissions)
        /// </summary>
        public IEnumerable<RoleRightsMapping> AllMenus
        {
            get
            {
                if (ProductMenuGroups == null)
                    return Enumerable.Empty<RoleRightsMapping>();

                // Get distinct menus by OperationId to avoid duplicates when menu appears in multiple products
                return ProductMenuGroups
                    .Where(g => g.Menus != null)
                    .SelectMany(g => g.Menus)
                    .GroupBy(m => m.OperationId)
                    .Select(g => g.First());
            }
        }
    }

    /// <summary>
    /// Represents a group of menus belonging to a specific product
    /// </summary>
    public class ProductMenuGroup
    {
        public ProductMenuGroup()
        {
            Menus = new List<RoleRightsMapping>();
            ParentMenuGroups = new List<ParentMenuGroup>();
        }

        /// <summary>
        /// Product ID (1=TNA, 2=Payroll, 3=EMS, 4=OKR, 5=PMS, 6=FieldSens, 7=CMS, 0=Common)
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Display name of the product
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Short code for the product (TNA, PAYROLL, EMS, etc.)
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Font Awesome icon class for the product
        /// </summary>
        public string IconClass { get; set; }

        /// <summary>
        /// Whether this panel should be expanded by default
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// List of menus belonging to this product
        /// </summary>
        public List<RoleRightsMapping> Menus { get; set; }

        /// <summary>
        /// Menus grouped by ParentMenuName for display
        /// </summary>
        public List<ParentMenuGroup> ParentMenuGroups { get; set; }

        /// <summary>
        /// Total count of menus in this product group
        /// </summary>
        public int MenuCount => Menus?.Count ?? 0;
    }

    /// <summary>
    /// Represents a group of menus under a parent menu within a product
    /// </summary>
    public class ParentMenuGroup
    {
        public ParentMenuGroup()
        {
            ChildMenus = new List<RoleRightsMapping>();
        }

        /// <summary>
        /// Parent menu name (used for grouping)
        /// </summary>
        public string ParentMenuName { get; set; }

        /// <summary>
        /// List of child menus under this parent
        /// </summary>
        public List<RoleRightsMapping> ChildMenus { get; set; }
    }
}
