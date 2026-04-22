using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class Plan
    {
        public int PlanId { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public double Price { get; set; }
        public int NoOfAdminUsers { get; set; }
        public int NoOfEmployee { get; set; }
        public bool IsActive { get; set; }
        public int Quantity { get; set; }
        public string TaxRate { get; set; }
        public double CGST { get; set; }
        public double SGST { get; set; }
        public double TotalAmount { get; set; }
        public double ppupm { get; set; }
        public double ppupy { get; set; }
        public double upm { get; set; }
        public string Feature { get; set; }
        public double Disccount { get; set; }
        public double DisccountPer { get; set; }
        public int Companyid { get; set; }
        public IEnumerable<Plan> PlanList { get; set; }
        public string Taxofpyament { get; set; }
        public string GSTNo { get; set; }
        public List<companyAddon> lstcmpaddonpack { get; set; }
        public string ReceptNo { get; set; }
        public string Paymentmethod { get; set; }
        public int Isrecuring { get; set; }
        public int IScurrencycode { get; set; }
    }

   
    //public class Paymenttrans
    //{
    //    public int TransId { get; set; }
    //    public int Companyid { get; set; }
    //    public int DealerId { get; set; }
    //    public string InvoiceNo { get; set; }
    //    public string InvoiceData { get; set; }
    //    public int PaymentMode { get; set; }
    //    public int IsApproved { get; set; }
    //    public double Amount { get; set; }
    //    public double CGST { get; set; }
    //    public double SGST { get; set; }
    //    public double IGST { get; set; }
    //    public double TotalAmount { get; set; }
    //    public string PaymentId { get; set; }
    //    public string RejectReason { get; set; }
    //    public string Companycode { get; set; }
    //}
    public class Paymenttrans
    {
        public int Companyid { get; set; }
        public int DealerId { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceData { get; set; }
        public int PaymentMode { get; set; }
        public int IsApproved { get; set; }
        public double Amount { get; set; }
        public double CGST { get; set; }
        public double SGST { get; set; }
        public double IGST { get; set; }
        public double TotalAmount { get; set; }
        public string PaymentId { get; set; }
        public string RejectReason { get; set; }
        public string Companycode { get; set; }
        public int Dplanid { get; set; }
        public double DisccountPer { get; set; }
        public double DisccountAmt { get; set; }
        public double CredittAmt { get; set; }
        public int IsRedeemcrAmt { get; set; }
        public int Duration { get; set; }
        public int Noofuser { get; set; }
        public string ReceptNo { get; set; }
        public int IScurrencycode { get; set; }        
    }

    public class Paymentres
    {
        public string PaymentID { get; set; }
        public string rzppaymentid { get; set; }
        public string txnid { get; set; }
        public string Uniqhash { get; set; }
        public string Paymentstatus { get; set; }
        public DateTime ResDate { get; set; }
        public string IsActive { get; set; }
        public string customerId { get; set; }
        public bool Isrecurring { get; set; }
        public string tokenid { get; set; }
    }


    


    
}