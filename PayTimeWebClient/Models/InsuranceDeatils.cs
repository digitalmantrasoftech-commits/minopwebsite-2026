using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTimeWebClient.Models
{
    public class InsuranceDetails
    {
        public int insurance_id { get; set; }
        public string employee_code { get; set; }
        public string insurance_of { get; set; }
        public string policy_type { get; set; }
        public string insurance_company_name { get; set; }
        public string policy_number { get; set; }
        public string dependent_name { get; set; }
        public string dependent_age { get; set; }
        public string dependent_birthdate { get; set; }
        public string dependent_gender { get; set; }
        public string registration_date { get; set; }
        public string due_date { get; set; }
        public string expire_date { get; set; }
        public string insurance_amount { get; set; }
        public string annual_amount { get; set; }
        public string monthly_premium { get; set; }
        public int premium_deduct_salary { get; set; }
        public string Salary_deduct_date { get; set; }
        public int is_verify { get; set; }
        public string link { get; set; }
        public string UID { get; set; }
    }



    public class ImportInsurancefaillog
    {
        public int ImpDataId { get; set; }
        public string employee_code { get; set; }
        public string insurance_of { get; set; }
        public string policy_type { get; set; }
        public string insurance_company_name { get; set; }
        public string policy_number { get; set; }
        public string dependent_name { get; set; }
        public int? dependent_age { get; set; }
        public string dependent_birthdate { get; set; }
        public string dependent_gender { get; set; }
        public string registration_date { get; set; }
        public string due_date { get; set; }
        public string expire_date { get; set; }
        public string insurance_amount { get; set; }
        public string annual_amount { get; set; }
        public string monthly_premium { get; set; }
        public bool premium_deduct_salary { get; set; }
        public string Salary_deduct_date { get; set; }
        public bool is_verify { get; set; }
        public string link { get; set; }
        public string UID { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string Startdate { get; set; }
        public string CreatedDtFlag { get; set; }
    }

    public class ImportResponseInsurance
    {
        public int statusCode { get; set; }
        public bool status { get; set; }
        public List<InsuranceDetails> InsuranceList { get; set; }
    }

    public class ImportFailLogInsurance
    {
        public int ImportedFailId { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string Startdate { get; set; }
        public string CreatedDtFlag { get; set; }
        public string Err { get; set; }
        public IEnumerable<ImportInsurancefaillog> ImportInsurancefaillogList { get; set; }
    }
}
