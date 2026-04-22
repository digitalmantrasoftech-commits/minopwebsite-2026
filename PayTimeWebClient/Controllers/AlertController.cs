using DevExpress.Web.Mvc;
using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class AlertController : Controller
    {
        #region Declaration

        static readonly AlertDataRestClient AlertRestClient = new AlertDataRestClient();
        static readonly MasterDataRestClient MasterRestClient = new MasterDataRestClient();
        static readonly ReportDataRestClient RestClientReport = new ReportDataRestClient();
        #endregion

        public ActionResult EmailSmtpConfiguration()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            EmailConfiguration cmp = new EmailConfiguration();
            try
            {
                ViewBag.Emailconfigurationlist = AlertRestClient.EmailconfigurationGetAll();
                if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                {
                    ViewBag.Companylist = MasterRestClient.CompanyGetAll();
                    ViewBag.Emailconfigurationlist = AlertRestClient.EmailconfigurationGetAll();
                }
                else
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    ViewBag.Companylist = MasterRestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                    ViewBag.Emailconfigurationlist = AlertRestClient.EmailconfigurationGetAll().Where(x => x.CompanyID == cmpid);
                }

                ViewBag.Emaildomaindetaillist = AlertRestClient.EmailDomainDetailGetAll();
                ViewBag.EmailTypeList = AlertRestClient.EmailTypeGetAll();
                return View(cmp);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        [HttpPost]
        public ActionResult EmailSmtpConfiguration(EmailConfiguration e)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ViewBag.msg = "";
                    ViewBag.error = "";
                    e.IsActive = true;
                    e.EmailType = GetEmailType(e.EmailTypeId);
                    e.SenderEmailID = e.CredentialEmailID;
                    e.SenderName = "Auto Response From Report";
                    e.DailySubject = "PayTime Attendance Report";
                    e.FromEmailID = e.CredentialEmailID;
                    e.FromName = "Auto Response From Report";
                    e.CompanyName = MasterRestClient.CompanyGetAll().Where(x => x.CompanyID == e.CompanyID).Select(c => c.CompanyName).FirstOrDefault();
                    e.EmailDomainName = AlertRestClient.EmailDomainDetailGetAll().Where(x => x.DomainId == e.DomainId && x.IsActive == true).Select(x => x.DomainName).FirstOrDefault();
                    if (e.EmailConfigId > 0)
                    {

                        if (!AlertRestClient.EmailconfigurationUpdate(e.EmailConfigId, e))
                        {
                            ViewBag.msg = "Email configuration not updated due to service issue.";
                        }
                        else
                        {
                            ViewBag.msg = "Email configuration is updated.";
                        }
                    }
                    else
                    {
                        e.Emailconfigurationlist = AlertRestClient.EmailconfigurationGetAll().Where(c => c.CredentialEmailID == e.CredentialEmailID && c.IsActive == true && c.CompanyID == e.CompanyID);
                        var EmailTypeExists = AlertRestClient.EmailconfigurationGetAll().Where(c => c.EmailTypeId == e.EmailTypeId && c.IsActive == true && c.CompanyID == e.CompanyID);
                        if (EmailTypeExists.Count() > 0)
                        {
                            ViewBag.error = e.EmailType + " Email type already Configured.";
                        }
                        else if (e.Emailconfigurationlist.Count() > 0)
                        {
                            ViewBag.error = e.CredentialEmailID + " already exists.";
                        }
                        else
                        {
                            AlertRestClient.EmailconfigurationAdd(e);
                            ViewBag.msg = "Email configuration added successfully.";
                        }
                    }
                }
                else
                {
                    ViewBag.error = "Not valid entry.";
                }

                ViewBag.Emaildomaindetaillist = AlertRestClient.EmailDomainDetailGetAll();
                ViewBag.EmailTypeList = AlertRestClient.EmailTypeGetAll();
                if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                {
                    ViewBag.Companylist = MasterRestClient.CompanyGetAll();
                    ViewBag.Emailconfigurationlist = AlertRestClient.EmailconfigurationGetAll();
                }
                else
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    ViewBag.Companylist = MasterRestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid);
                    ViewBag.Emailconfigurationlist = AlertRestClient.EmailconfigurationGetAll().Where(x => x.CompanyID == cmpid);
                }

                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }

        public string GetEmailType(int id)
        {
            try
            {
                var EmailTypeList = AlertRestClient.EmailTypeGetAll();
                var EmailTypeName = EmailTypeList.Where(x => x.EmailTypeID == id).Select(x => x.EmailTypeName).FirstOrDefault();
                return EmailTypeName;
            }
            catch (Exception)
            {

                return string.Empty;
            }

        }

        [HttpGet]
        public JsonResult GetSMTPDetails(int DomainId = 0)
        {
            JsonResult result;
            try
            {
                var Emaildomaindetaillist = AlertRestClient.EmailDomainDetailGetAll();
                var SmtpIp = Emaildomaindetaillist.Where(i => i.DomainId == DomainId).Select(i => i.SMTPServer).FirstOrDefault();
                var SmtpPort = Emaildomaindetaillist.Where(i => i.DomainId == DomainId).Select(i => i.ServerPort).FirstOrDefault();
                result = Json(new { SmtpIp = SmtpIp, SmtpPort = SmtpPort });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new { SmtpIp = "0", SmtpPort = "0" });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        [HttpGet]
        public JsonResult SendTestConnectionMail(string SMTPHost, int SMTPPort, string CredentialEmailId, string CredentialPassword)
        {
            JsonResult result;
            try
            {
                MailDetails mailDetails = new MailDetails();
                mailDetails.SMTPHost = SMTPHost;
                mailDetails.SMTPPort = SMTPPort;
                mailDetails.CredentialEmailId = CredentialEmailId;
                mailDetails.CredentialPassword = CredentialPassword;
                var Emaildomaindetaillist = AlertRestClient.SendTestConnectionMail(mailDetails);
                if (Emaildomaindetaillist)
                {
                    result = Json(new { Emaildomaindetaillist = Emaildomaindetaillist, message = "Email Send successfully" });
                }
                else
                {
                    result = Json(new { message = "Email not send successfully.Kindly reconfiguration and check again." });
                }
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new { message = "Email not send successfully.Kindly reconfiguration and check again." });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

        public ActionResult SendReportMail()
        {
            try
            {
                if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                {
                    var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                    ViewBag.Companyslist = MasterRestClient.CompanyGetAll().Where(x => x.CompanyID == cmpid).ToList();
                }
                else
                {
                    ViewBag.Companyslist = MasterRestClient.CompanyGetAll();
                }

                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }

        }

        [HttpPost]
        public JsonResult SendMailDetails(string FromDate, string Todate, string EmpID, string ReportName, string CompanyID, string RptID,
            string CompanyName, string BranchName, string IsFilo, string IsCompanyHead, string IsDepartmentHead, string IsEmployeeSendto,
            string LstEmpID, string[] selectedDep, string CustRptID, string CustRptName)
        {
         
            JsonResult result;
            try
            {
                #region CustreportList Declaration
                List<int> CustRptIdList = new List<int>();
                List<string> CustRptNameList = new List<string>();
                List<int> CustLstSltDepid = new List<int>();
                List<string> regularreportList = new List<string>();
                int RegularFlag = 0;
                if (!string.IsNullOrEmpty(CustRptID))
                {
                    CustRptIdList = CustRptID.Split(',').Select(int.Parse).ToList();
                    CustRptNameList = CustRptName.Split(',').ToList();
                    CustLstSltDepid = selectedDep.Select(i => int.Parse(i)).ToList();
                }
                #endregion

                #region Check RegularReportId Send Email For Employee
                if (!string.IsNullOrEmpty(RptID))
                {
                    List<int> RptIdList = RptID.Split(',').Select(int.Parse).ToList();
                    List<string> RptNameList = ReportName.Split(',').ToList();
                    List<int> LstSltDepid = selectedDep.Select(i => int.Parse(i)).ToList();
                    List<string> reports = new List<string>();
                    string MailSendResponse = string.Empty;
                    bool IsSend = false;
                    for (int j = 0; j < RptIdList.Count; j++)
                    {
                        string path = AppDomain.CurrentDomain.BaseDirectory + @"Uploads\";
                        string filename = RptNameList[j] + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".pdf";
                        string file = path + filename;
                        reports.Add(file);
                        Stream stream = new FileStream(file, FileMode.Create);
                        System.Diagnostics.Debug.WriteLine((new System.IO.StreamReader(stream)).ReadToEnd());
                        try
                        {
                            reqDailyInOutReport csreq = new reqDailyInOutReport();
                            csreq.BranchName = BranchName;
                            csreq.BranchId = BranchName;
                            csreq.CompanyID = CompanyID;
                            csreq.CompanyName = CompanyName;
                            csreq.EmpID = EmpID;
                            csreq.FromDate = FromDate;
                            csreq.ReportName = RptNameList[j];
                            csreq.RptID = RptIdList[j];
                            csreq.Todate = Todate;
                            if (csreq.ReportName.Contains("MonthlyMusterReport"))
                            {
                                var model = RestClientReport.ViewMonthlyMusterQueryGetAll(csreq);
                                GetReportDetails(csreq);
                                GridViewExtension.WritePdf(GridViewHelper.ExportReportsettings, model, stream);
                            }
                            else
                            {
                                var model = RestClientReport.ViewDailyReportQueryGetAll(csreq);
                                GetReportDetails(csreq);
                                GridViewExtension.WritePdf(GridViewHelper.ExportReportsettings, model, stream);
                            }
                            stream.Close();
                            stream.Dispose();
                        }
                        catch (Exception)
                        {
                            stream.Close();
                            stream.Dispose();
                            var item = reports[j];
                            if (System.IO.File.Exists(item))
                            {
                                System.IO.File.Delete(item);
                            }
                            reports.RemoveAt(j);
                        }
                    }
                    if (reports.Count > 0)
                    {
                        regularreportList = reports;
                        //Send Generated Report in Mail
                        var CompanyHeadEmails = MasterRestClient.CompanyGetAll().Where(x => x.CompanyID == Convert.ToInt32(CompanyID)).Select(x => Tuple.Create(x.CompanyEmail, x.CompanyName)).ToList();
                        var AllDeptCode = MasterRestClient.EmployeeGetAll();
                        var BranchWiseDeptCode = AllDeptCode.Where(x => x.BranchName == BranchName).Select(x => x.DepartmentId).Distinct();
                        var BranchWiseDept = MasterRestClient.DepartmentGetAll().Where(x => BranchWiseDeptCode.Contains(x.DepartmentId));
                        var SelectedDept = (BranchWiseDept.Where(r => LstSltDepid.Contains(r.DepartmentId)));
                        var DepartmentHeadEmails = (from Dep in SelectedDept.ToList()
                                                    join emp in MasterRestClient.EmployeeGetAll().ToList()
                                                    on Dep.DepartmentHead equals emp.EmpId
                                                    select new { emp.Email, emp.EmpName }).ToList();
                        var LstDepartmentHeadEmails = DepartmentHeadEmails.Select(s => Tuple.Create(s.Email, s.EmpName)).ToList();
                        var LstEmplyee = LstEmpID.Split(',').Select(int.Parse).ToList();
                        List<Tuple<string, string>> lstEmployeeEmails = new List<Tuple<string, string>>();
                        foreach (var item in LstEmplyee)
                        {
                            var email = MasterRestClient.EmployeeGetAll().Where(x => x.EmpId == item).Select(x => Tuple.Create(x.Email, x.EmpName)).FirstOrDefault();
                            lstEmployeeEmails.Add(email);
                        }
                        var SMTPDetails = AlertRestClient.EmailconfigurationGetAll().Where(x => x.CompanyID == Convert.ToInt32(CompanyID) && x.IsActive == true).FirstOrDefault();
                        if (IsCompanyHead == "1")
                        {
                            if (CompanyHeadEmails.Count > 0)
                            {
                                IsSend = ReportMailDetails(SMTPDetails, CompanyHeadEmails, reports);
                            }
                            else
                            {
                                MailSendResponse = "Company head email not found \n";
                            }
                        }
                        if (IsDepartmentHead == "1")
                        {
                            if (LstDepartmentHeadEmails.Count > 0)
                            {
                                IsSend = ReportMailDetails(SMTPDetails, LstDepartmentHeadEmails, reports);
                            }
                            else
                            {
                                MailSendResponse = "Department head email not found \n";
                            }
                        }
                        if (IsEmployeeSendto == "1")
                        {
                            if (lstEmployeeEmails.Count > 0)
                            {
                                IsSend = ReportMailDetails(SMTPDetails, lstEmployeeEmails, reports);
                            }
                            else
                            {
                                MailSendResponse = "Employees email not found \n";
                            }
                        }
                        if (IsSend)
                        {
                            MailSendResponse = MailSendResponse + "Email Sent successfully \n";
                        }
                        else
                        {
                            MailSendResponse = MailSendResponse + "Email Not Sent successfully \n";
                        }
                        result = Json(new { message = MailSendResponse });
                        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                        if (string.IsNullOrEmpty(CustRptID))
                        { return result; }
                        else
                        { RegularFlag = 1; }
                    }
                }
                #endregion//--- End regular
                #region CustomReport CreatePdfFile
                if (!string.IsNullOrEmpty(CustRptID))
                {
                    CustRptIdList = CustRptID.Split(',').Select(int.Parse).ToList();
                    CustRptNameList = CustRptName.Split(',').ToList();
                    List<string> Custreports = new List<string>();
                    string MailSendResponse = string.Empty;
                    bool IsCustSend = false;

                    for (int k = 0; k < CustRptIdList.Count; k++)
                    {
                        string path = AppDomain.CurrentDomain.BaseDirectory + @"Uploads\";
                        string filename = CustRptNameList[k] + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".pdf";
                        string file = path + filename;
                        Custreports.Add(file);
                        Stream stream = new FileStream(file, FileMode.Create);
                        System.Diagnostics.Debug.WriteLine((new System.IO.StreamReader(stream)).ReadToEnd());

                        try
                        {
                          
                            CustomerReportField csreq1 = new CustomerReportField();
                            csreq1.BranchName = BranchName;
                            csreq1.BranchId = BranchName;
                            //csreq1.CompanyID = Convert.ToInt32(CompanyID);
                            csreq1.CompanyID = CompanyID;
                            csreq1.CompanyName = CompanyName;
                            csreq1.EmpID = EmpID;
                            //  var Isdateformat = Session["IsDateFormat"];      
                            //if(Isdateformat.ToString().Trim() == "mm-dd-yyyy")
                            //{
                            //        var fromDate = csreq1.FromDate.Split('-');
                            //        csreq1.FromDate = fromDate[0] + "-" + fromDate[1] + "-" + fromDate[2];;
                            //}
                            //else{
                            csreq1.FromDate = FromDate;
                            var Isdateformat = Session["IsDateFormat"]; 
                            if(csreq1.FromDate != null)
                            {
                                if(Isdateformat.ToString().Trim() == "mm-dd-yyyy")
                                {
                                    var fromDate = csreq1.FromDate.Split('-');
                                    csreq1.FromDate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                                }
                                else
                                {
                                    csreq1.FromDate = Convert.ToDateTime(csreq1.FromDate).ToString("yyyy-MM-dd");
                                }
                             }
                            //}
                            csreq1.CustReportName = CustRptNameList[k];
                            csreq1.CustRptID = CustRptIdList[k];
                            //if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
                            //{
                            //    var toDate = csreq1.Todate.Split('-');
                            //    csreq1.Todate = toDate[0] + "-" + toDate[1] + "-" + toDate[2];
                            //}
                            //else
                            //{
                                csreq1.Todate = Todate;
                                if (csreq1.Todate != null)
                                {
                                    if (Isdateformat.ToString().Trim() == "mm-dd-yyyy")
                                    {
                                        var toDate = csreq1.Todate.Split('-');
                                        csreq1.Todate = toDate[2] + "-" + toDate[0] + "-" + toDate[1];
                                    }
                                    else
                                    {
                                        csreq1.Todate = Convert.ToDateTime(csreq1.Todate).ToString("yyyy-MM-dd");
                                    }
                                }

                            //}
                            csreq1.ReportID = CustRptIdList[k];
                            var model1 = RestClientReport.GetDynamicCustomReportList(csreq1);
                            GetcustReportDetails(csreq1);
                            GridViewExtension.WritePdf(GridViewHelper.CustomExportReportsettings, model1, stream);

                            stream.Close();
                            stream.Dispose();
                        }
                        catch (Exception)
                        {
                            stream.Close();
                            stream.Dispose();
                            var item = Custreports[k];
                            if (System.IO.File.Exists(item))
                            {
                                System.IO.File.Delete(item);
                            }
                            Custreports.RemoveAt(k);
                        }
                    }
                #endregion
                #region Customreport SendMailStart
                    if (Custreports.Count > 0)
                    {
                        //Send Generated Report in Mail
                        var CompanyHeadEmails = MasterRestClient.CompanyGetAll().Where(x => x.CompanyID == Convert.ToInt32(CompanyID)).Select(x => Tuple.Create(x.CompanyEmail, x.CompanyName)).ToList();
                        var AllDeptCode = MasterRestClient.EmployeeGetAll();
                        var BranchWiseDeptCode = AllDeptCode.Where(x => x.BranchName == BranchName).Select(x => x.DepartmentId).Distinct();
                        var BranchWiseDept = MasterRestClient.DepartmentGetAll().Where(x => BranchWiseDeptCode.Contains(x.DepartmentId));
                        var SelectedDept = (BranchWiseDept.Where(r => CustLstSltDepid.Contains(r.DepartmentId)));
                        var DepartmentHeadEmails = (from Dep in SelectedDept.ToList()
                                                    join emp in MasterRestClient.EmployeeGetAll().ToList()
                                                    on Dep.DepartmentHead equals emp.EmpId
                                                    select new { emp.Email, emp.EmpName }).ToList();
                        var LstDepartmentHeadEmails = DepartmentHeadEmails.Select(s => Tuple.Create(s.Email, s.EmpName)).ToList();
                        var LstEmplyee = LstEmpID.Split(',').Select(int.Parse).ToList();
                        List<Tuple<string, string>> lstEmployeeEmails = new List<Tuple<string, string>>();
                        foreach (var item in LstEmplyee)
                        {
                            var email = MasterRestClient.EmployeeGetAll().Where(x => x.EmpId == item).Select(x => Tuple.Create(x.Email, x.EmpName)).FirstOrDefault();
                            lstEmployeeEmails.Add(email);
                        }
                        var SMTPDetails = AlertRestClient.EmailconfigurationGetAll().Where(x => x.CompanyID == Convert.ToInt32(CompanyID) && x.IsActive == true).FirstOrDefault();
                        if (IsCompanyHead == "1")
                        {
                            if (CompanyHeadEmails.Count > 0)
                            {
                                IsCustSend = ReportMailDetails(SMTPDetails, CompanyHeadEmails, Custreports);
                            }
                            else
                            {
                                MailSendResponse = "Company head email not found \n";
                            }
                        }
                        if (IsDepartmentHead == "1")
                        {
                            if (LstDepartmentHeadEmails.Count > 0)
                            {
                                IsCustSend = ReportMailDetails(SMTPDetails, LstDepartmentHeadEmails, Custreports);
                            }
                            else
                            {
                                MailSendResponse = "Department head email not found \n";
                            }
                        }
                        if (IsEmployeeSendto == "1")
                        {
                            if (lstEmployeeEmails.Count > 0)
                            {
                                IsCustSend = ReportMailDetails(SMTPDetails, lstEmployeeEmails, Custreports);
                            }
                            else
                            {
                                MailSendResponse = "Employees email not found \n";
                            }
                        }
                        if (IsCustSend)
                        {
                            MailSendResponse = MailSendResponse + "Email Sent successfully \n";
                        }
                        else
                        {
                            MailSendResponse = MailSendResponse + "Email Not Sent successfully \n";
                        }

                        result = Json(new { message = MailSendResponse });
                        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                        return result;
                    }
                    else
                    {
                        result = Json(new { message = "No report generated to Send in mail" });
                        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                        return result;
                    }
                    #endregion

                #region For PDFFileDeleteCode
                    //Delete Generated Reports
                    if (regularreportList.Count > 0)
                    {
                        foreach (var item in regularreportList)
                        {
                            if (System.IO.File.Exists(item))
                            {
                                System.IO.File.Delete(item);
                            }
                        }
                    }
                    //Delete Generated CustomerReports
                    foreach (var item in Custreports)
                    {
                        if (System.IO.File.Exists(item))
                        {
                            System.IO.File.Delete(item);
                        }
                    }
                    #endregion
                }
                else
                {
                    result = Json(new { message = "No report generated to send in mail" });
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result = Json(new { message = "Email not send successfully.Kindly reconfiguration and check again." });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }
        public bool ReportMailDetails(EmailConfiguration SMTPDetails, List<Tuple<string, string>> lstSendTo, List<string> Reports)
        {
            bool IsmailSend = false;
            if (SMTPDetails != null && lstSendTo != null)
            {
                for (int i = 0; i < lstSendTo.Count(); i++)
                {
                    MailDetails mailDetails = new MailDetails();
                    mailDetails.SMTPHost = SMTPDetails.SMTPIP;
                    mailDetails.SMTPPort = SMTPDetails.SMTPPORT;
                    mailDetails.CredentialEmailId = SMTPDetails.CredentialEmailID;
                    mailDetails.CredentialPassword = SMTPDetails.CredentialEmailIDPassword;
                    mailDetails.fromEmail = SMTPDetails.FromEmailID;
                    mailDetails.Isattchement = true;
                    mailDetails.AttachementPath = new List<string>();
                    mailDetails.AttachementPath.AddRange(Reports);
                    mailDetails.MailBody = "<Html><Head></head><body><p>Dear " + lstSendTo[i].Item2 + ",</p><p>Please find selected reports as attachments.</p><p>Regards,</p><p></br>Mantra</p></body></Html>";
                    mailDetails.Subject = "Reports";
                    mailDetails.ToEmail = lstSendTo[i].Item1;
                    IsmailSend = AlertRestClient.SendReportMail(mailDetails);

                }
                return IsmailSend;
            }
            else
            {
                return false;
            }
        }
        public void GetcustReportDetails(CustomerReportField csreq)
        {
            ReportDetails.Rptid = csreq.CustRptID;
            ReportDetails.RptName = csreq.CustReportName;
            ReportDetails.RptTitle = csreq.CustReportName;
            ReportDetails.Fdate = csreq.FromDate;
            ReportDetails.Tdate = csreq.Todate;
            ReportDetails.bname = csreq.BranchName;
            ReportDetails.cname = csreq.CompanyName;
        }
        public void GetReportDetails(reqDailyInOutReport csreq)
        {
            if (csreq.ReportName == "DailyInOutReport")
            {
                ReportDetails.Rptid = 1;
                ReportDetails.RptName = "DailyInOutReport";
                ReportDetails.RptTitle = "Daily In-Out Report";
                ReportDetails.Fdate = csreq.FromDate;
                ReportDetails.Tdate = csreq.Todate;
                ReportDetails.bname = csreq.BranchName;
                ReportDetails.cname = csreq.CompanyName;
            }
            if (csreq.ReportName == "DailyInOutDeviceNameReport")
            {
                ReportDetails.Rptid = 2;
                ReportDetails.RptName = "DailyInOutDeviceNameReport";
                ReportDetails.RptTitle = "Daily In-Out With Device Name Report";
            }
            if (csreq.ReportName == "FirstINLastOUTReport")
            {
                ReportDetails.Rptid = 3;
                ReportDetails.RptName = "FirstINLastOUTReport";
                ReportDetails.RptTitle = "First IN Last OUT Report";
            }
            if (csreq.ReportName == "DailyInReport")
            {
                ReportDetails.Rptid = 4;
                ReportDetails.RptName = "DailyInReport";
                ReportDetails.RptTitle = "Daily In Report";
            }
            if (csreq.ReportName == "ErrorCaseReport")
            {
                ReportDetails.Rptid = 5;
                ReportDetails.RptName = "ErrorCaseReport";
                ReportDetails.RptTitle = "Error Case Report";
            }
            if (csreq.ReportName == "AbsentReport")
            {
                ReportDetails.Rptid = 6;
                ReportDetails.RptName = "AbsentReport";
                ReportDetails.RptTitle = "Absent Report";
            }

            if (csreq.ReportName == "LateINReport")
            {

                ReportDetails.Rptid = 7;
                ReportDetails.RptName = "LateINReport";
                ReportDetails.RptTitle = "Late IN Report";
            }
            if (csreq.ReportName == "EarlyINReport")
            {
                ReportDetails.Rptid = 8;
                ReportDetails.RptName = "EarlyINReport";
                ReportDetails.RptTitle = "Early IN Report";
            }
            if (csreq.ReportName == "EarlyOutReport")
            {
                ReportDetails.Rptid = 9;
                ReportDetails.RptName = "EarlyOutReport";
                ReportDetails.RptTitle = "Early Out Report";
            }
            if (csreq.ReportName == "LateOutReport")
            {
                ReportDetails.Rptid = 10;
                ReportDetails.RptName = "LateOutReport";
                ReportDetails.RptTitle = "Late Out Report";
            }
            if (csreq.ReportName == "OTReport")
            {
                ReportDetails.Rptid = 11;
                ReportDetails.RptName = "OTReport";
                ReportDetails.RptTitle = "OT Report";
            }
            if (csreq.ReportName == "LateArrivalReport")
            {
                ReportDetails.Rptid = 12;
                ReportDetails.RptName = "LateArrivalReport";
                ReportDetails.RptTitle = "CONTINUOUS LATE ARRIVAL REPORT";
            }
            if (csreq.ReportName == "ContinuousEarlyDepartureReport")
            {
                ReportDetails.Rptid = 13;
                ReportDetails.RptName = "ContinuousEarlyDepartureReport";
                ReportDetails.RptTitle = "CONTINUOUS EARLY DEPARTURE REPORT";
            }
            if (csreq.ReportName == "ContinuousAbsenteeismReport")
            {
                ReportDetails.Rptid = 14;
                ReportDetails.RptName = "ContinuousAbsenteeismReport";
                ReportDetails.RptTitle = "CONTINUOUS ABSENTEEISM REPORT";
            }
            if (csreq.ReportName == "MachineRawTransactionReport")
            {
                ReportDetails.Rptid = 15;
                ReportDetails.RptName = "MachineRawTransactionReport";
                ReportDetails.RptTitle = "MACHINE RAW TRANSACTION REPORT";
            }
            if (csreq.ReportName == "ManualPunchReport")
            {
                ReportDetails.Rptid = 16;
                ReportDetails.RptName = "ManualPunchReport";
                ReportDetails.RptTitle = "Manual Punch Report";
            }
            if (csreq.ReportName == "DepartmentSummaryReport")
            {
                ReportDetails.Rptid = 17;
                ReportDetails.RptName = "DepartmentSummaryReport";
                ReportDetails.RptTitle = "Department Summary Report";
            }
            if (csreq.ReportName == "EarlyInSummaryReport")
            {
                ReportDetails.Rptid = 18;
                ReportDetails.RptName = "EarlyInSummaryReport";
                ReportDetails.RptTitle = "Early-In Summary Report";
            }
            if (csreq.ReportName == "MonthlyMusterReport")
            {
                ReportDetails.Rptid = 19;
                ReportDetails.RptName = "MonthlyMusterReport";
                ReportDetails.RptTitle = "Monthly Muster Report";
            }
            if (csreq.ReportName == "MonthlyWorkingDurationReport")
            {
                ReportDetails.Rptid = 20;
                ReportDetails.RptName = "MonthlyWorkingDurationReport";
                ReportDetails.RptTitle = "Monthly Working Duration Report";
            }
            if (csreq.ReportName == "LeaveBalanceReport")
            {
                ReportDetails.Rptid = 21;
                ReportDetails.RptName = "LeaveBalanceReport";
                ReportDetails.RptTitle = "Leave Balance Report";
            }
            if (csreq.ReportName == "MonthlyBioMetricReport")
            {
                ReportDetails.Rptid = 22;
                ReportDetails.RptName = "MonthlyBioMetricReport";
                ReportDetails.RptTitle = "Monthly Bio-Metric Report";
            }
            if (csreq.ReportName == "MonthlyAttendanceReport")
            {
                ReportDetails.Rptid = 23;
                ReportDetails.RptName = "MonthlyAttendanceReport";
                ReportDetails.RptTitle = "Monthly Attendance Report";
            }
            if (csreq.ReportName == "DailyFlexiInOutReport")
            {
                ReportDetails.Rptid = 24;
                ReportDetails.RptName = "DailyFlexiInOutReport";
                ReportDetails.RptTitle = "Daily Flexi In-Out Report";
            }
            if (csreq.ReportName == "Dailypresentwithoutmaskreport")
            {
                ReportDetails.Rptid = 25;
                ReportDetails.RptName = "Dailypresentwithoutmaskreport";
                ReportDetails.RptTitle = "Mask Report";
            }
            if (csreq.ReportName == "Dailyhightempraturedetectedreport")
            {
                ReportDetails.Rptid = 26;
                ReportDetails.RptName = "Dailyhightempraturedetectedreport";
                ReportDetails.RptTitle = "Temperature Report";
            }
            if (csreq.ReportName == "Dailyhightempraturedetectedreport")
            {
                ReportDetails.Rptid = 27;
                ReportDetails.RptName = "Dailyhightempraturedetectedreport";
                ReportDetails.RptTitle = "Failed status Report";
            }
            if (csreq.ReportName == "FacePunchWebPunchReport")
            {
                ReportDetails.Rptid = 28;
                ReportDetails.RptName = "FacePunchWebPunchReport";
                ReportDetails.RptTitle = "FacePunch/WebPunch Report";
            }
            if (csreq.ReportName == "LeaveBalanceSummaryReport")
            {
                ReportDetails.Rptid = 29;
                ReportDetails.RptName = "LeaveBalanceSummaryReport";
                ReportDetails.RptTitle = "Leave Balance Summary Report";
            }

            if (csreq.ReportName == "WeeklyFlexiInOutReport")
            {
                ReportDetails.Rptid = 30;
                ReportDetails.RptName = "WeeklyFlexiInOutReport";
                ReportDetails.RptTitle = "Weekly Flexi In-Out Report";
            }

            if (csreq.ReportName == "MonthlyFlexiInOutReport")
            {
                ReportDetails.Rptid = 31;
                ReportDetails.RptName = "MonthlyFlexiInOutReport";
                ReportDetails.RptTitle = "Monthly Flexi In-Out Report";
            }

            ReportDetails.Fdate = csreq.FromDate;
            ReportDetails.Tdate = csreq.Todate;
            ReportDetails.bname = csreq.BranchName;
            ReportDetails.cname = csreq.CompanyName;
        }
        [HttpPost]
        public JsonResult GetEmployee()
        {
            JsonResult result;
            try
            {
                var AllEmployee = MasterRestClient.EmployeeGetAll();
                result = Json(new SelectList(AllEmployee, "EmpId", "EmpName"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception)
            {
                result = Json(new SelectList("", "0"));
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
        }

        [HttpGet]
        public JsonResult CheckIsConfigured(int CompanyId = 0)
        {
            JsonResult result;
            bool Isconfigured = false;
            try
            {
                if (CompanyId > 0)
                {
                    var EmailConfiguration = AlertRestClient.EmailconfigurationGetAll().Where(c => c.CompanyID == CompanyId && c.IsActive == true);
                    if (EmailConfiguration.Count() > 0)
                    {
                        Isconfigured = true;
                    }
                    result = Json(new { Isconfigured = Isconfigured });
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    return result;
                }
                else
                {
                    result = Json(new { Isconfigured = false });
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    return result;
                }

            }
            catch (Exception)
            {
                result = Json(new { Isconfigured = true });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }

        }


        [HttpPost]
        public JsonResult SavecreateTask(int Taskid, string FromDate, string Todate, string EmpID, string ReportName, string CompanyID, string RptID,
            string CompanyName, string BranchName, string IsFilo, string IsCompanyHead,string IsBranchHead, string IsDepartmentHead, string IsEmployeeSendto,
            string LstEmpID, string[] selectedDep, string[] selectedBranch, string Triggerid, string Startdate, string Starttime
            , string Schedulermonth, string Schedulerdayon, string Schedulerdayonmonth, string Scheduleronday, string Scheduleronweek,
            string CustRptID, string CustReportName
            )
        {
            JsonResult result;
            Taskscheduler obj = new Taskscheduler();
            List<int> LstSltDepid = selectedDep.Select(i => int.Parse(i)).ToList();
            List<int> LstSltbranchid = selectedBranch.Select(i => int.Parse(i)).ToList();

            var _branchdt = "";
            for (int i = 0; i < LstSltbranchid.Count; i++)
            {
                if (_branchdt == "")
                {
                    _branchdt = LstSltbranchid[i].ToString();
                }
                else
                {
                    _branchdt += "," + LstSltbranchid[i].ToString();
                }

            }

            var _deplidt = "";
            for (int i = 0; i < LstSltDepid.Count; i++)
            {
                if (_deplidt == "")
                {
                    _deplidt = LstSltDepid[i].ToString();
                }
                else
                {
                    _deplidt += "," + LstSltDepid[i].ToString();
                }

            }

            obj.Taskid = Taskid;
            obj.FromDate = FromDate;
            obj.Todate = Todate;
            obj.EmpID = EmpID;
            obj.ReportName = ReportName;
            obj.CompanyID = CompanyID;
            obj.RptID = RptID;
            obj.CompanyName = CompanyName;
            obj.BranchName = BranchName;
            obj.IsFilo = IsFilo;
            obj.IsCompanyHead = IsCompanyHead;
            obj.IsBranchHead = IsBranchHead;
            obj.IsDepartmentHead = IsDepartmentHead;
            obj.IsEmployeeSendto = IsEmployeeSendto;
            obj.LstEmpID = LstEmpID;
            obj.selectedBranch = _branchdt;
            obj.selectedDep = _deplidt;
            obj.Triggerid = Triggerid;
            obj.Startdate = Startdate;
            obj.Starttime = Starttime;
            obj.Schedulermonth = Schedulermonth;
            obj.Schedulerdayon = Schedulerdayon;
            obj.Schedulerdayonmonth = Schedulerdayonmonth;
            obj.Scheduleronday = Scheduleronday;
            obj.Scheduleronweek = Scheduleronweek;

            obj.EntryBy = Convert.ToInt32(Session["CompanyId"]);
            if (CustRptID.ToString() == ",")
            {
                obj.CustRptID = "";
            }
            else
            {
                if (!string.IsNullOrEmpty(CustRptID))
                {
                    obj.CustRptID = CustRptID;
                }
            }
            if (CustReportName.ToString() == ",")
            {
                obj.CustReportName = "";
            }
            else
            {
                if (!string.IsNullOrEmpty(CustReportName))
                {
                    obj.CustReportName = CustReportName;
                }
            }
            /*For this  block are Check in Dublicate EmpId are Enter in
              Database Level So Single Values are Check and Enter in Database Level 
              this Issue are happend in Select all employee time.(19-07-2021)*/
            if (!string.IsNullOrEmpty(obj.EmpID))
            {
                string[] str1;
                str1 = obj.EmpID.Split(',');
                var _EmpIdTest = "";
                var hashSet = new HashSet<string>();
                for (int i = 0; i < str1.Length; i++)
                {
                    int No = 0;
                    if (!hashSet.Add(str1[i]))
                    {
                        No = 1;
                    }
                    if (_EmpIdTest == "")
                    {
                        _EmpIdTest = str1[i];
                    }
                    else
                    {
                        if (No == 0)
                        {
                            _EmpIdTest += "," + str1[i];
                        }
                        obj.EmpID = _EmpIdTest;
                    }
                }
            }
            MRespo mres = AlertRestClient.SavecreateTask(obj);
            if (mres.MegSts == "Ok")
            {
                result = Json(new { message = "Scheduler added successfully.", MegSts = "Ok" });
                //ViewBag.msg = "taskscheduler added successfully.";
            }
            else
            {
                result = Json(new { message = "Scheduler on error.", MegSts = "error" });
                //ViewBag.msg = "taskscheduler added Error.";
            }

            //result = Json(new { message = "No report generated to Send in mail" });
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
            //return View();


        }
    }
}