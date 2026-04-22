using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
namespace PayTimeWebClient.Models
{
    public class LoginModel
    {

        [Required(ErrorMessage = "Account Name Is Required")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Company Code Is Required")]
        public string CompanyCode { get; set; }
        [Required(ErrorMessage = "User Email Is Required")]
        //[RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$", ErrorMessage = "Please Enter Valid UserEmail")]
        public string UserEmail { get; set; }
        [Required(ErrorMessage = "Password Is Required")]
        public string Password { get; set; }
        public int PlanId { get; set; }
        public int IsSchool { get; set; }
        public int IsOffice { get; set; }
        public int IsCart { get; set; }
    }
    public class MRespo
    {
        public string MegSts { get; set; }
        public string Meg { get; set; }
    }

    public class Resp
    {
        public string MsgSts { get; set; }
        public string Msg { get; set; }
    }
    public class ResetPassword
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
    public class OTPCheck
    {
        public int OTP { get; set; }
        public string CompanyCode { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; }
    }
    
    public class EmployeeResetPassword
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string UserEmail { get; set; }
        public int EmpId { get; set; }
        public int RoleId { get; set; }
    }


    public class ReqUpdateAdminInfo
    {
        public virtual int UserId { get; set; }
        public virtual string Name { get; set; }
        public virtual int Gender { get; set; }
        public virtual Boolean IsMarried { get; set; }
        public virtual string Address { get; set; }
        public virtual string DateOfBirth { get; set; }
        public virtual string MobileNo { get; set; }
        public virtual string PhoneNo { get; set; }
        public virtual string Photo { get; set; }
        public virtual string CountryCode { get; set; }
        public virtual string ModifyDate { get; set; }
        public virtual int ModifyBy { get; set; }
        public virtual string DealerCode { get; set; }
        public virtual string DealerId { get; set; }
        public virtual int ManagerId { get; set; }
        public virtual int ActManagerId { get; set; }
        public virtual string hdnDealerCode { get; set; }
        public virtual string CompanyName { get; set; }

        public IEnumerable<ReqUpdateAdminInfo> admininfolist { get; set; }
    }

    public class EnrollDataSave
    {
        public int DeviceUserId { get; set; }
        public string EmpId { get; set; }
        public string EmpName { get; set; }
        public string DeviceId { get; set; }
        public Byte[] Photo { get; set; }
        public string Email { get; set; }
        public string[] DeviceIds { get; set; }
        public string Photopath { get; set; }
    }

    public class Supportticketreq
    {
        public string priority { get; set; }
        public string status { get; set; }
        public string subject { get; set; }
        public string email { get; set; }
        public string description { get; set; }
        public string[] strfilename { get; set; }
    }

    public class Supportticketzohoreq
    {
        public string subject { get; set; }
        public string departmentId { get; set; }
        public string description { get; set; }
        public string[] uploads { get; set; }
        public classcontact contact { get; set; }
        
        
    }

    public class classcontact
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }

    public class Attechments
    {
        public string strfilename { get; set; }
        public byte[] filedata { get; set; }
    }

    public class Supportticketres
    {
        
       
        public string[] cc_emails { get; set; }
        public string[] fwd_emails { get; set; }
        public string[] reply_cc_emails { get; set; }
        public string[] ticket_cc_emails { get; set; }
        public bool fr_escalated { get; set; }
        public bool spam { get; set; }
        public string email_config_id { get; set; }
        public int priority { get; set; }
        public int requester_id { get; set; }
        public int responder_id { get; set; }
        public int source { get; set; }
        public int company_id { get; set; }
        public string subject { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string fr_due_by { get; set; }
        public bool is_escalated { get; set; }
        public string description { get; set; }
        public string description_text { get; set; }
        public string created_at { get; set; }
        public errors error { get; set; }
    }

    public class errors
    {

        public string field { get; set; }
        public string message { get; set; }
        public string code { get; set; }
    }


    public class reqEmployeePhoto
    {
        public int EmpId { get; set; }
        public string EmpPhoto { get; set; }
        public int IsApprove { get; set; }
        public string Email { get; set; }
        public string EmpName { get; set; }
    }



}