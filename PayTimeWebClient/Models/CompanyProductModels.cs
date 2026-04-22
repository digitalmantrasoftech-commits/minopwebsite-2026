using System;
using System.Collections.Generic;

namespace PayTimeWebClient.Models
{
    /// <summary>
    /// Model for company information
    /// </summary>
    public class CompanyInfo
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Model for company-product mapping
    /// </summary>
    public class CompanyProductMapping
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int ProductId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    /// <summary>
    /// Request model for getting company products
    /// </summary>
    public class GetCompanyProductsRequest
    {
        public int CompanyId { get; set; }
    }

    /// <summary>
    /// Response model for getting company products
    /// </summary>
    public class GetCompanyProductsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<int> ProductIds { get; set; }
        public List<CompanyProductMapping> ProductMappings { get; set; }
    }

    /// <summary>
    /// Request model for adding product to company
    /// </summary>
    public class AddProductToCompanyRequest
    {
        public int CompanyId { get; set; }
        public int ProductId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? CreatedBy { get; set; }
        public string Remarks { get; set; }
    }

    /// <summary>
    /// Request model for removing product from company
    /// </summary>
    public class RemoveProductFromCompanyRequest
    {
        public int CompanyId { get; set; }
        public int ProductId { get; set; }
        public int? ModifiedBy { get; set; }
    }

    /// <summary>
    /// Generic API response model
    /// </summary>
    public class CompanyProductApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    /// <summary>
    /// Request model for getting user companies
    /// </summary>
    public class GetUserCompaniesRequest
    {
        public int UserId { get; set; }
    }

    /// <summary>
    /// Response model for getting user companies
    /// </summary>
    public class GetUserCompaniesResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<CompanyInfo> Companies { get; set; }
    }

    /// <summary>
    /// Response model for getting all product master data
    /// </summary>
    public class ProductMasterApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<Product> Products { get; set; }
    }
}
