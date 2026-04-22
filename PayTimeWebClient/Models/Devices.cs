using System.Collections.Generic;

namespace PayTimeWebClient.Models
{
    public class Devices
    {
        public int DeviceId { get; set; }
        public string DeviceCode { get; set; }
        public int BranchId { get; set; }
        public int DepartmentId { get; set; }
        public string DeviceIP { get; set; }
        public int DevicePort { get; set; }
        public string DevicePassword { get; set; }
        public string DeviceName { get; set; }
        public string Mode { get; set; }
        public int DeviceTypeCode { get; set; }
        public string DeviceType { get; set; }
        //public string IsDeviceSrNo { get; set; }
        //public  int CompCode { get; set; }
        //public  string DeviceLastAccessTime { get; set; }
        //public string DeviceLastDownloadRecordTime { get; set; }
        //public  string DeviceSrNo { get; set; }
        //public  string DeviceBackupNo { get; set; }
        public string LastActivity { get; set; }
        public int IsPushData { get; set; }
        public bool IsActive { get; set; }
        //public  int CreatedBy { get; set; }
        //public string CreatedDate { get; set; }
        //public  int ModifyBy { get; set; }
        //public string ModifyDate { get; set; }
        public string DeviceSrNo { get; set; }
        public string RegCompanyCode { get; set; }
        public string DeviceStatus { get; set; }
        public string BranchName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public int isDisebleDw { get; set; }
        public int isDisebleUp { get; set; }
        public bool IsSchool { get; set; }
        public bool IsAttendance { get; set; }
        public int LocatedAt { get; set; }


        public IEnumerable<Devices> DeviceList { get; set; }
        public IEnumerable<DeviceTypes> DeviceTypeList { get; set; }

        public int DevicePunchMode { get; set; }
        public int IsEnrollUser { get; set; }
        public string TempSrno { get; set; }
    }


    public class AcDevice
    {
        public string id { get; set; }
        public string SrNo { get; set; }
        public int flg { get; set; }
    }

    public class AddressInformation
    {
        public string receiptno { get; set; }
        public string company_name_bd { get; set; }
        public string user_name_bd { get; set; }
        public string mobile_number_bd { get; set; }
        public string address_bd { get; set; }
        public string country_bd { get; set; }
        public string state_bd { get; set; }
        public string city_bd { get; set; }
        public string zipcode_bd { get; set; }
        public string gstn_bd { get; set; }
        public string address_sd { get; set; }
        public string country_sd { get; set; }
        public string state_sd { get; set; }
        public string city_sd { get; set; }
        public string zipcode_sd { get; set; }
        public int Isreccuring { get; set; }
        public string Paymentmethod { get; set; }

        public int Isexistingcust { get; set; }
        public string Totalamt { get; set; }
        public string PlanId { get; set; }
        public string Duration { get; set; }
        public string Quantity { get; set; }
        public string Taxofpyament { get; set; }
        public string camt { get; set; }
        public string product { get; set; }



    }

    public class productcartreq
    {
        public string receiptno { get; set; }
        public int companyid { get; set; }
        public int entryby { get; set; }
        public int CustomerType { get; set; }
        public string Duration { get; set; }
        public string PlanType { get; set; }
        public decimal PlanPrice { get; set; }
        public int NopfEmployee { get; set; }
        public decimal Total { get; set; }
        public decimal SubTotal { get; set; }
        public decimal SubscriptionPlan { get; set; }
        public decimal GrandTotal { get; set; }
        public List<Products> lstproduct { get; set; }
        public string saveType { get; set; }
        public int Isreccuring { get; set; }
        public string Paymentmethod { get; set; }

    }

    public class Products
    {
        public string productname { get; set; }
        public string productid { get; set; }
        public string extendedwarranty { get; set; }
        public string intallationservice { get; set; }
        public string intallservicepincode { get; set; }
        public decimal productprice { get; set; }
        public decimal productQuantity { get; set; }
        public decimal productdicprice { get; set; }
        public decimal productExtWrntAmt { get; set; }
        public decimal productIntaSerAmt { get; set; }
        public decimal Productperoff { get; set; }
        public int Productqtn { get; set; }
    }
}