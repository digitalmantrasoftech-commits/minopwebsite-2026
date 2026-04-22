using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    /// <summary>
    /// Represents a product in the Minop Enterprise Suite
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Unique identifier for the product
        /// </summary>
        /// <summary>
        /// Unique identifier for the product
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Display name of the product
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Short code identifier for the product (e.g., TNA, PAYROLL, EMS)
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Description of the product
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// API URL for the product
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// Font Awesome icon class for the product (e.g., fa-solid fa-clock)
        /// </summary>
        public string IconClass { get; set; }

        /// <summary>
        /// Indicates if this is the main product (TNA)
        /// </summary>
        public bool IsMainProduct { get; set; }

        /// <summary>
        /// Indicates if the product is active and should be shown in the product switcher
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Display order for sorting products
        /// </summary>
        public int? DisplayOrder { get; set; }

        /// <summary>
        /// Redirect URL for the product
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Category for grouping products in UI (e.g., "Core HR", "Finance & Payroll", "Operations")
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Category icon class for UI display
        /// </summary>
        public string CategoryIcon { get; set; }
    }
}
