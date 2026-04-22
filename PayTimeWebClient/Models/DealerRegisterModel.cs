using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class DealerRegisterModel
    {

        public string DealersName { get; set; }
        public int TypeDealer { get; set; }
        public string ContactPerson { get; set; }
        public int Refrencefrom { get; set; }
        public string ReferenceName { get; set; }
        public string PartnerName { get; set; }
        public string Logoimage { get; set; }
        public int NewExisting { get; set; }
        [Required]
        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$", ErrorMessage = "Email Invalid")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string UserEmail { get; set; }
        public string Website { get; set; }
        public int Actmanagerid { get; set; }

    }
    public class DealerLogin
    {

        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$", ErrorMessage = "Email Invalid")]
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }
    public class DealerResetPassword
    {
        public int UserId { get; set; }
        public string UserPass { get; set; }
        [Required(ErrorMessage = "User Email Is Required")]
        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$", ErrorMessage = "Please Enter Valid UserEmail")]
        public string Email { get; set; }
        public string ConfirmPassword { get; set; }
        public string OTP { get; set; }
        public string CompanyCode { get; set; }
    }

    public class DealerOtpVerification
    {

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Otp is required")]
        public string OTP { get; set; }
        public string enotp { get; set; }

    }

    public class RegistrationOtpVerification
    {
        [Required(ErrorMessage = "Otp is required")]
        public string Otp { get; set; }
        public string dealerId { get; set; }
    }

    public class NewMobileNumberVerification
    {
        [Required(ErrorMessage = "New mobileno is required")]
        public string NewNumber { get; set; }
        public string newDealerId { get; set; }

    }


    public class DelaerPlan
    {
        public int PlanId { get; set; }
        public string Planname { get; set; }
        public int Duration { get; set; }
        public double Price { get; set; }
        public int NoOfAdminUsers { get; set; }
        public int NoOfEmployee { get; set; }
        public int NoOfCompany { get; set; }
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
        public IEnumerable<DelaerPlan> PlanList { get; set; }
        public string Taxofpyament { get; set; }
        public List<companyAddon> lstcmpaddonpack { get; set; }



    }
    public class companyAddon
    {
        public int Addonid { get; set; }
        public int AddonPackgeid { get; set; }
        public decimal Packageunitrate { get; set; }
        public int Unitquantity { get; set; }
        public DateTime Packagestartdate { get; set; }
        public DateTime Packageenddate { get; set; }
        public int Createby { get; set; }
        public decimal Packagetotalamt { get; set; }
        public string PaymentId { get; set; }
        public int Companyid { get; set; }

    }




    public class Getpara
    {
        public int pkid { get; set; }
        public int iscount { get; set; }
        public int para { get; set; }
        public int IScurrencycode { get; set; }
        public string companycode { get; set; }
    }

    public class Getplanpricingreq
    {
        public int para { get; set; }
        public int planid { get; set; }
    }
    public class partnerlicence
    {
        public int partnerid { get; set; }
        public int planid { get; set; }
        public int Quantity { get; set; }
        public string Paymentid { get; set; }
        public int Createby { get; set; }
        public List<partnerlicence> lstplicence { get; set; }
    }
    public class RegisterAccountmanager
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public int Branchid { get; set; }
        public string Empcode { get; set; }
        public string CreatedBy { get; set; }
        public int IsActive { get; set; }
    }

    public class Companyplan
    {
        public int PlanId { get; set; }
        public int CompanyId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}