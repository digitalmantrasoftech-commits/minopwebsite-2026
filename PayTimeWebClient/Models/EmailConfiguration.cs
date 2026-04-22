using System.Collections.Generic;

namespace PayTimeWebClient.Models
{
    public class EmailConfiguration
    {
        public int EmailConfigId { get; set; }
        public int DomainId { get; set; }
        public string SMTPIP { get; set; }
        public int SMTPPORT { get; set; }
        public string FromEmailID { get; set; }
        public string FromName { get; set; }
        public string SenderEmailID { get; set; }
        public string SenderName { get; set; }
        public string DailySubject { get; set; }
        public string CredentialEmailID { get; set; }
        public string CredentialEmailIDPassword { get; set; }
        public int EmailTypeId { get; set; }
        public string EmailType { get; set; }
        public string EmailDomainName { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public bool IsActive { get; set; }

        public IEnumerable<EmailConfiguration> Emailconfigurationlist { get; set; }
    }
    public class MailDetails
    {
        public virtual string SMTPHost { get; set; }
        public virtual int SMTPPort { get; set; }
        public virtual string CredentialEmailId { get; set; }
        public virtual string CredentialPassword { get; set; }
        public virtual string ToEmail { get; set; }
        public virtual string fromEmail { get; set; }
        public virtual string Subject { get; set; }
        public virtual string MailBody { get; set; }
        public virtual bool Isattchement { get; set; }
        public virtual List<string> AttachementPath { get; set; }
    }

    public class Taskscheduler
    {
        public int Taskid { get; set; }
        public string FromDate { get; set; }
        public string Todate { get; set; }
        public string EmpID { get; set; }
        public string ReportName { get; set; }
        public string CompanyID { get; set; }
        public string RptID { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string IsFilo { get; set; }
        public string IsCompanyHead { get; set; }
        public string IsBranchHead { get; set; }
        public string IsDepartmentHead { get; set; }
        public string IsEmployeeSendto { get; set; }
        public string LstEmpID { get; set; }
        public string selectedBranch { get; set; }
        public string selectedDep { get; set; }
        public string Triggerid { get; set; }
        public string Startdate { get; set; }
        public string Starttime { get; set; }
        public int EntryBy { get; set; }

        public string Schedulermonth { get; set; }
        public string Schedulerdayon { get; set; }
        public string Schedulerdayonmonth { get; set; }
        public string Scheduleronday { get; set; }
        public string Scheduleronweek { get; set; }

        public string CustRptID { get; set; }
        public string CustReportName { get; set; }

    }
}