using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PayTimeWebClient.Models
{

    public class Companys
    {

        public int CompanyID { get; set; }
        public string CompanyUrl { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyContact { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyCode { get; set; }
        public string MobNoSMS { get; set; }
        public int SettingId { get; set; }
        public  bool IsActive { get; set; }
        public string LicenseType { get; set; }
        public string CompanyPincode { get; set; }
        public string CompanyGstNO { get; set; }
        public string CountryCode { get; set; }
        public string CompanyZipcode { get; set; }
        public string AreaCode { get; set; }
        public string Status { get; set; }
        public string Startdate { get; set; }
        public string CreatedDtFlag { get; set; }
        public IEnumerable<Companys> Companyslist { get; set; }
        
    }

    public class Branches
    {
        #region Properties
        public virtual int BranchId { get; set; }
        public virtual string BranchName { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string CityName { get; set; }
        public virtual int CompanyID { get; set; }
        public virtual int CityId { get; set; }
        public virtual int CountryID { get; set; }
        public virtual int StateID { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual int ReportingBranchId { get; set; }
        public virtual int BranchLevel { get; set; }
        public virtual string ReportingBranchName { get; set; }
        public virtual string BranchHeadEmail { get; set; }
        public virtual string BranchHeadPassword { get; set; }
        public virtual string BranchAddress { get; set; }
        public virtual string ReportingBranch { get; set; }
        public IEnumerable<Branches> Brancheslist { get; set; }
        public IEnumerable<Companys> Companyslist { get; set; }
        public IEnumerable<CityMaster> Citylist { get; set; }
        public IEnumerable<CountryMaster> Countrylist { get; set; }
        public IEnumerable<StateMaster> Statelist { get; set; }
        public IEnumerable<CountryTimeZone> CountryTimeZoneList { get; set; }
        
        

      
        #endregion
    }
    public class BUMaster
    {
        public virtual int BUId { get; set; }
        public virtual string BUName { get; set; }
    }
    public class SBUMaster
    {
        public virtual int SubBUId { get; set; }
        public virtual string SubBUName { get; set; }
    }

    public class CityMaster
    {
        public virtual int CityID { get; set; }
        public virtual string CityName { get; set; }
        public virtual int StateID { get; set; }
        public virtual bool IsActive { get; set; }
    }

    public class StateMaster
    {
        public virtual int StateId { get; set; }
        public virtual string StateName { get; set; }
        public virtual int CountryId { get; set; }
        public virtual bool IsActive { get; set; }
    }

    public class CountryMaster
    {
        public virtual int CountryId { get; set; }
        public virtual string CountryName { get; set;}
        public virtual bool IsActive { get; set; }
        public virtual string CountryCode { get; set; }
        public virtual string CountryCodes { get; set; }
    }

    public class CustomerMaster
    {
        #region Properties
        public virtual int CustID { get; set; }
        public virtual string CustName { get; set; }
        public virtual string CustMobile { get; set; }
        public virtual string CustEmail { get; set; }
        public virtual string CustAddress { get; set; }
        public virtual string CustLatitude { get; set; }
        public virtual string CustLongitude { get; set; }
        public virtual int CreatedBy { get; set; }
        public virtual string CreatedDate { get; set; }
        public virtual int ModifyBy { get; set; }
        public virtual string ModifyDate { get; set; }

        public IEnumerable<CustomerMaster> Customerlist { get; set; }
        


        #endregion
    }

    public class CountryTimeZone
    {
        public virtual string TimeZoneID { get; set; }
        public virtual string TimeZoneName { get; set; }


    }

    public class ImportResponse
    {
        //public string msg { get; set; }
        public int statusCode { get; set; }
        public bool status  { get; set; }
        public List<Companys> companyList { get; set; }
        public List<Branches> branchList { get; set; }
        public List<Departments> departmentList { get; set; }
        public List<Designations> designationList { get; set; }
        public List<ImportedDataEmp> employeeList { get; set; }
    }
}