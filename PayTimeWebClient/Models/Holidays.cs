using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class Holidays
    {

        public int HolidayId { get; set; }
        public string HolidayName { get; set; }
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        //public DateTime HolidayDate { get; set; }
        //public string HolidayDate { get; set; }
        //public int BranchId { get; set; }
        public string HolidayDate { get; set; }
        public string HolidayToDate { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int StateId { get; set; }
        public string StateName { get; set; }
        public string TimeZoneId { get; set; }
        public string TimeZoneName { get; set; }
        public string ReligionId { get; set; }
        public string ReligionName { get; set; }
        public bool IsHolidayType { get; set; } //0 for optional 1 for compulsory
        public int HolidayApplicable { get; set; } //0 default,1 for National ,2 for International
        //public int CompanyID { get; set; }
        public string CompanyID { get; set; }
        public int CityId { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual int CreatedBy { get; set; }
        public virtual string CreatedDate { get; set; }
        public virtual int ModifyBy { get; set; }
        public virtual string ModifyDate { get; set; }

        public string[] BranchIds { get; set; }
        public string[] CompanyIDs { get; set; }
        public string[] ReligionIds { get; set; }

        public IEnumerable<Holidays> Holidayslist { get; set; }
        public IEnumerable<CountryMaster> Countrylist { get; set; }
        public IEnumerable<Branches> BranchList { get; set; }
        public IEnumerable<Religion> ReligionList { get; set; }
        public IEnumerable<Companys> CompanyList { get; set;}
        public int RoleId { get; set; }
        public int LoginEmpId { get; set; }

        
    }
}