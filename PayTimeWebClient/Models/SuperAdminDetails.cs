using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class SuperAdminDetails
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string DomainName { get; set; }
        public string DbType { get; set; }
        public string CompanyDbName { get; set; }
        public int IsActiveRegCom { get; set; }
        public int IsActiveUser { get; set; }
        public int CompanyPlanId { get; set; }
        public string PlanStartDate { get; set; }
        public string PlanEndDate { get; set; }
        public string IsActivePlan { get; set; }
        public string PlanName { get; set; }
        public int PlanDuration { get; set; }
        public double PlanPrice { get; set; }
        public int ActiveCompany { get; set; }
        public IEnumerable<SuperAdminDetails> SuperAdminDetailslist { get; set; }
    }
    public class ActiveDevicesDetails
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string DomainName { get; set; }
        public string CompanyCode { get; set; }
        public string DeviceSrNo { get; set; }
        public string DeviceName { get; set; }
        public int DeviceCode { get; set; }
        public string DeviceIP { get; set; }
        public int BranchId { get; set; }
        public string DeviceStatus { get; set; }
        public string DeviceRegisteredWith { get; set; }
        public IEnumerable<ActiveDevicesDetails> ActiveDevicesDetailslist { get; set; }
    }

    public class  AnalyticsDashboardDetails
    {
        public string Titles { get; set; }
        public int NoOfClick { get; set; }
        public decimal TimeSpent { get; set; }
        public string Usr { get; set; }
        public string SubOperation { get; set; }
        public string Createddate { get; set; }
        public IEnumerable<AnalyticsDashboardDetails> AnalyticsDashboardDetailslist { get; set; }
    }
    public class Analyticsdata
    {
        public decimal Noofclick { get; set; }
        public int Noofvisits { get; set; }
        public decimal timeduration { get; set; }
        public string colname { get; set; }
    }

    public class srchdata
    {
        public string frmdate {get; set; }
        public string todate { get; set; }
        public IEnumerable<Analyticsdata> AnalyticsDashboardDetailslist { get; set; }
    }
    public  class srchfilter
    {

        public string frmdate { get; set; }
        public string todate { get; set; }
        public string Titles { get; set; }
        public IEnumerable<Analyticsdata> AnalyticsDashboardDetailslist { get; set; }
    }

    public class SuperAdminReportsFilter
    {
        public int rptid { get; set; }
        public string fdate { get; set; }
    }
}