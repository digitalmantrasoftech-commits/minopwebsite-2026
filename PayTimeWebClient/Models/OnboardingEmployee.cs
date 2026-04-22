using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class OnBordingEmployee
    {
        public EmployeeDetail[] Table { get; set; }
        public EducationDetail[] Table1 { get; set; }
        public FamilyDetail[] Table2 { get; set; }
    }

    #region For OnBording Class Properties

    public class EmployeeDetail
    {
        public int OnBordingID { get; set; }
        public string NameOfApplicant { get; set; }
        public string JoiningDate { get; set; }
        public string ResignDate { get; set; }
        public string EmployeeCode { get; set; }
        public int PunchID { get; set; }
        public bool GeoEnable { get; set; }
        public bool IsSMSAllow { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int DesignationID { get; set; }
        public string DesignationName { get; set; }
        public int ShiftID { get; set; }
        public string ShiftName { get; set; }
        public int ShiftGroupId { get; set; }
        public string ShiftGroupName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailID { get; set; }
        public string FatherName { get; set; }
        public string PlaceOfBirth { get; set; }
        public string AgeOnDate { get; set; }
        public string Nationality { get; set; }
        public string BirthMarkIdentification { get; set; }
        public string PhysicalAilment { get; set; }
        public string ReferenceOneName { get; set; }
        public string ReferenceOneMobileNumber { get; set; }
        public string ReferenceOneAddress { get; set; }
        public string ReferenceTwoName { get; set; }
        public string ReferenceTwoMobileNumber { get; set; }
        public string ReferenceTwoAddress { get; set; }
        public string Nominee1Name { get; set; }
        public string Nominee1Relationship { get; set; }
        public string Nominee1Proportion { get; set; }
        public string Nominee1Address { get; set; }
        public string Nominee1Age { get; set; }
        public string Nominee1DOB { get; set; }
        public string Nominee2Name { get; set; }
        public string Nominee2Relationship { get; set; }
        public string Nominee2Proportion { get; set; }
        public string Nominee2Address { get; set; }
        public string Nominee2Age { get; set; }
        public string Nominee2DOB { get; set; }
        public string CriminalRecordInPast { get; set; }
        public string ExpectedSalaryMonthly { get; set; }
        public string ExpectedSalaryYearly { get; set; }
        public string NoticePeriod { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public string BankAddress { get; set; }
        public string PassbookPhoto { get; set; }
        public string CheckbookPhoto { get; set; }
        public string PermanentVillage { get; set; }
        public string PermanentLandMark { get; set; }
        public string PermanentTaluka { get; set; }
        public string PermanentDistrict { get; set; }
        public string PermanentState { get; set; }
        public string PermanentPincode { get; set; }
        public string PermanentNearPostOffice { get; set; }
        public string PermanentNearPolishStation { get; set; }
        public string PresentVillage { get; set; }
        public string PresentLandMark { get; set; }
        public string PresentTaluka { get; set; }
        public string PresentDistrict { get; set; }
        public string PresentState { get; set; }
        public string PresentPincode { get; set; }
        public string PresentNearPostOffice { get; set; }
        public string PresentNearPolishStation { get; set; }
        public string PresentNearPolishStation1 { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
        public int GenderID { get; set; }
        public string GenderName { get; set; }
        public int MaritalStatusID { get; set; }
        public string MaritalStatus { get; set; }
        public string StatusName { get; set; }
        public int StatusID { get; set; }
        public int PunchTypeID { get; set; }
        public string PunchTypeName { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        public string MultipleBranchID { get; set; }
        public int ReportingTo { get; set; }
        public int PolicyId { get; set; }
        public string PolicyName { get; set; }
        public int RoleID { get; set; }
        public string DateOfBirth { get; set; }
        public string EmployeeCategoryName { get; set; }
        public string EmployeeTypeName { get; set; }
        public int EmployeeCategoryID { get; set; }
        public int EmployeeTypeID { get; set; }
        public string EmployeeHeight { get; set; }
        public string Employeeweight { get; set; }
        public string PanCardPhoto { get; set; }
        public string PancardNumber { get; set; }
        public string AadharCardNumber { get; set; }
        public string AadharCardPhoto { get; set; }
        public string AadharCardTwoPhoto { get; set; }
        public string PanCardTwoPhoto { get; set; }
        public string AadharCardTwoNumber { get; set; }
        public string PancardTwoNumber { get; set; }
        public int PaystuctureID { get; set; }
        public string MonthlyCTC { get; set; }
        public string GrossSalary { get; set; }
        public string OnhandSalary { get; set; }
        public string BloodGroup { get; set; }
        public string RoleName { get; set; }
        public string repotingpersonname { get; set; }
        public string NationalityName { get; set; }
        public string BloodGroupName { get; set; }
        public string EmployeePhoto { get; set; }
        public string DeviceSrNo { get; set; }
        public string EmpAdharcardInfo { get; set; }
        public string EmpPancardInfo { get; set; }
        public string AdharCardPhotoInfo { get; set; }
        public string PanCardPhotoInfo { get; set; }
        public string MultipleBranchAttaName { get; set; }
        public string DeviceSrName { get; set; }

        public string EmpRFIDCard { get; set; }
        public int EnrollUserDeviceID { get; set; }

        public string RejectReason { get; set; }
        public string IDCardNumber { get; set; }


        public int GatePassPolicyID { get; set; }
        public string GatepassPolicyName { get; set; }

    }
    public class EducationDetail
    {
        public int EduID { get; set; }
        public string UniversityName { get; set; }
        public string Percentage { get; set; }
        public string FromYear { get; set; }
        public string ToYear { get; set; }
        public string Specialization { get; set; }
        public int BordingID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class FamilyDetail
    {
        public int FID { get; set; }
        public string Fname { get; set; }
        public int Relation { get; set; }
        public int Age { get; set; }
        public int BordingID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public string Occupation { get; set; }
        public string RelationName { get; set; }
    }

    #endregion

    #region For payroll
    public class PayrollEmpDetails
    {
        public ResultData resultData { get; set; }
        public object employeeBankDetail { get; set; }
        public object lstData { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public object validationErrors { get; set; }
        public object responsedata { get; set; }
        public int responseCode { get; set; }
        public string responseInfo { get; set; }
    }
    public class ResultData
    {
        public int structure_id { get; set; }
        public string structure_name { get; set; }
        public string structure_type { get; set; }
        public List<Earning> earnings { get; set; }
        public List<Deduction> deductions { get; set; }
        public bool isError { get; set; }
        public bool isValidFormula { get; set; }
        public string errorMessage { get; set; }
        public double totalMonthlyDeduction { get; set; }
        public double grossMonthlyEarning { get; set; }
        public double netMonthlySalary { get; set; }
        public double fixedPayMonthly { get; set; }
        public double pay_netMonthlySalary { get; set; }
        public double pay_grossMonthlyEarning { get; set; }
        public bool emp_Gratuity { get; set; }
        public bool is_Pf_Only_Actual { get; set; }
        public bool isPayStructure_Pf_Applicable { get; set; }
        public bool isPayStruct_PFFixed { get; set; }
        public double payStruct_Employee_PFFixedAmount { get; set; }
        public double payStruct_Employer_PFFixedAmount { get; set; }
        public bool isPayStruct_PFActual { get; set; }
        public double payStruct_Employee_PFActualPercent { get; set; }
        public double payStruct_Employer_PFActualPercent { get; set; }
        public bool isEmployee_Pf_Applicable { get; set; }
        public bool isEmployee_pf_Fixed { get; set; }
        public double emp_Employee_PFFixedAmount { get; set; }
        public double emp_Employer_PFFixedAmount { get; set; }
        public bool isEmployeePFActual { get; set; }
        public double emp_Employee_PFActualPercent { get; set; }
        public double emp_Employer_PFActualPercent { get; set; }
        public object isEmp_PF_No { get; set; }
        public bool isPFCalculateOnGross { get; set; }
    }

    public class Deduction
    {
        public int salary_head_id { get; set; }
        public string head_name { get; set; }
        public string head_type_title { get; set; }
        public string strcalcbasedOn { get; set; }
        public double amount { get; set; }
        public double payAmount { get; set; }
        public bool is_excludein_sal_slip { get; set; }
    }

    public class Earning
    {
        public int salary_head_id { get; set; }
        public string head_name { get; set; }
        public string head_type_title { get; set; }
        public string strcalcbasedOn { get; set; }
        public double amount { get; set; }
        public double payAmount { get; set; }
        public bool is_excludein_sal_slip { get; set; }
    }
    #endregion


    #region For OnBording Response ParamList
    public class OnboardingEmployeeGetAllData
    {
        public int OnBordingID { get; set; }
        public int roleid { get; set; }
        public int loginempid { get; set; }
        public int CompanyID { get; set; }
        public string BranchID { get; set; }
        public string DeaprtmentID { get; set; }
        public int DesignationID { get; set; }
    }
    #endregion


    #region For tbltaskscheduler
    public class SchedulerFasttrack
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
        public string IsCompanyHead { get; set; }
        public string IsDepartmentHead { get; set; }
        public string IsEmployeeSendto { get; set; }
        public string LstEmpID { get; set; }
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
    #endregion


    #region For DirectoryGetData Request ParamList
    public class DirectoryEmployeeGetAllData
    {
        public int OnBoardingID { get; set; }
        public int RoleId { get; set; }
        public int CompanyId { get; set; }
        public string BranchId { get; set; }
        public int LoginEmpId { get; set; }
        public string DepartmentID { get; set; }
        public int DesignationID { get; set; }
        public int DeactivateID { get; set; }
    }
    #endregion


    public class EmployeeModel
    {
        public string All { get; set; }
        public int OnboardingID { get; set; }
        public string EmpName { get; set; }
        public DateTime JoiningDate { get; set; }
        public DateTime? ResignDate { get; set; }
        public string EmpCode { get; set; }
        public int PunchID { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int BranchID { get; set; }
        public string Branch { get; set; }
        public int DepartmentID { get; set; }
        public string Department { get; set; }
        public int DesignationID { get; set; }
        public string Designation { get; set; }
        public int ShiftID { get; set; }
        public string ShiftName { get; set; }
        public int ShiftGroupID { get; set; }
        public string ShiftGroupName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string FatherName { get; set; }
        public string PlaceOfBirth { get; set; }
        public string AgeOnDate { get; set; }
        public string Nationality { get; set; }
        public string BloodGroup { get; set; }
        public string BirthIdentification { get; set; }
        public string PhysicalAilments { get; set; }
        public string RefOneName { get; set; }
        public string RefOneMobile { get; set; }
        public string RefOneAddress { get; set; }
        public string RefTwoName { get; set; }
        public string RefTwoMobile { get; set; }
        public string RefTwoAddress { get; set; }
        public string Nominee1Name { get; set; }
        public string Nominee1Rel { get; set; }
        public int Nominee1Proportion { get; set; }
        public string Nominee1Address { get; set; }
        public int Nominee1Age { get; set; }
        public DateTime Nominee1DOB { get; set; }
        public string Nominee2Name { get; set; }
        public string Nominee2Rel { get; set; }
        public int Nominee2Proportion { get; set; }
        public string Nominee2Address { get; set; }
        public int Nominee2Age { get; set; }
        public DateTime Nominee2DOB { get; set; }
        public string CriminalRecordInPast { get; set; }
        public decimal? ExpectedSalaryMonthly { get; set; }
        public decimal? ExpectedSalaryYearly { get; set; }
        public int NoticePeriod { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public string BankAddress { get; set; }
        public string PassbookPhoto { get; set; }
        public string CheckbookPhoto { get; set; }
        public string PermanentVillage { get; set; }
        public string PermanentLandMark { get; set; }
        public string PermanentTaluka { get; set; }
        public string PermanentDistrict { get; set; }
        public string PermanentState { get; set; }
        public string PermanentPincode { get; set; }
        public string PermanentPostOffice { get; set; }
        public string PermanentPoliceStation { get; set; }
        public string PresentVillage { get; set; }
        public string PresentLandMark { get; set; }
        public string PresentTaluka { get; set; }
        public string PresentDistrict { get; set; }
        public string PresentState { get; set; }
        public string PresentPincode { get; set; }
        public string PresentPostOffice { get; set; }
        public string PresentPoliceStation { get; set; }
        public string EmployeePhoto { get; set; }
        public string EmpPhotoData { get; set; }
        public string DeviceSrNo { get; set; }
        public bool IsActive { get; set; }
        public bool? Deleted { get; set; }
        public string GenderID { get; set; }
        public string Gender { get; set; }
        public string MaritalStatusID { get; set; }
        public string MaritalStatus { get; set; }
        public string StatusName { get; set; }
        public int StatusID { get; set; }
        public int PunchTypeID { get; set; }
        public string PunchTypeName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string MultipleBranchAttaName { get; set; }
        public string MultipleBranchID { get; set; }
        public string ReportingTo { get; set; }
        public int PolicyId { get; set; }
        public string PolicyName { get; set; }
        public int RoleID { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmpCategory { get; set; }
        public string EmpType { get; set; }
        public string EmployeeCategoryID { get; set; }
        public string EmployeeTypeID { get; set; }
        public string EmpHeight { get; set; }
        public string EmpWeight { get; set; }
        public string RefOnePanCardPhoto { get; set; }
        public string RefOnePanCardNum { get; set; }
        public string RefOneAdharCardNum { get; set; }
        public string RefOneAdharCardPhoto { get; set; }
        public string RefTwoAdharCardPhoto { get; set; }
        public string RefTwoPanCardPhoto { get; set; }
        public string RefTwoAdharCardNum { get; set; }
        public string RefTwoPanCardNum { get; set; }
        public string PaystuctureID { get; set; }
        public decimal MonthlyCTC { get; set; }
        public string AadharCardNumber { get; set; }
        public string PANCardNumber { get; set; }
        public string EmpRFIDCard { get; set; }
        public string IDCardNumber { get; set; }
        public string Status { get; set; }
        public int EmpId { get; set; }
        public string AdharCardPhoto { get; set; }
        public string PanCardPhoto { get; set; }
    }

    public class EducationModel
    {
        public int EduID { get; set; }
        public string UniversityName { get; set; }
        public string Percentage { get; set; }
        public DateTime FromYear { get; set; }
        public DateTime ToYear { get; set; }
        public string Specialization { get; set; }
        public int BordingID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
    }
    public class FamilyModel
    {
        public int FID { get; set; }
        public string Fname { get; set; }
        public string Relation { get; set; }
        public string Occupation { get; set; }
        public int Age { get; set; }
        public int BordingID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
    }
    public class DirectoryDetailsViewModel
    {
        public EmployeeModel[] Table { get; set; }
        public EducationModel[] Table1 { get; set; }
        public FamilyModel[] Table2 { get; set; }
    }
}