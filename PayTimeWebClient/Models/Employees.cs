using System.Collections.Generic;

namespace PayTimeWebClient.Models
{
    public class Employees
    {
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public int UserId { get; set; }
        public string EmpAddress { get; set; }
        public string EmpPhNo { get; set; }
        public string EmpMNo { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool Gender { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Empcode { get; set; }
        public string EmpDOB { get; set; }
        public bool EmpMarried { get; set; }
        public string EmpJoinDate { get; set; }
        public string EmpResignDate { get; set; }
        public string EmpPunchID { get; set; }
        public string EmpPhoto { get; set; }
        public string shifttimename { get; set; }
        public string MobNoSMS { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }

        public int BU { get; set; }
        public int SubBU { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TypeId { get; set; }
        public string EmpTypeName { get; set; }
        public int GradeId { get; set; }
        public int ShiftId { get; set; }
        public string ShiftName { get; set; }
        public string ShiftShortName { get; set; }
        public int ShiftGroupId { get; set; }
        public string ShiftGroupName { get; set; }
        public string ShiftGroupShortName { get; set; }
        public int ContractorId { get; set; }
        public string ContractorName { get; set; }
        public int ReportingTo { get; set; }
        public int PolicyId { get; set; }
        public string PolicyName { get; set; }
        public string StartTime { get; set; }
        public int EmpWeekOff { get; set; }
        public int EmpSecondWeekOff { get; set; }
        public string EmpSecondWeekOffRule { get; set; }
        public int EmpHalfDay { get; set; }
        public string EmpHalfDayRule { get; set; }
        public int ReligionId { get; set; }
        public int IsSMS { get; set; }
        public string Status { get; set; }
        public int ModifyBy { get; set; }
        public string ModifyDate { get; set; }
        public bool IsActive { get; set; }
        public string Married { get; set; }
        public string JoinDate { get; set; }
        public string BirthDate { get; set; }
        public string EmpGender { get; set; }
        public IEnumerable<Employees> EmployeeList { get; set; }
        public IEnumerable<Religion> ReligionList { get; set; }
        public string TagId { get; set; }
        public string EmpphotoData { get; set; }
        public int isPlanFace { get; set; }
        public string CustomFields { get; set; }
        public string Customvalues { get; set; }
        public string geoenable { get; set; }
        public string worf { get; set; }
        public string hdnuploadfile { get; set; }
        public string CountryCode { get; set; }
        public string BranchGeolocation { get; set; }
        public string[] BranchGeoLocations { get; set; }
        public int IsVerification { get; set; }
        public string ReportingName { get; set; }
        public int PlanId { get; set; }
        public string EmpphotoBase { get; set; }
		public int totalCount { get; set; }

    }

    public class EmployeesExtended : Employees
    {
        public int SelectAll { get; set; } = 1;
        public string SelectAllSearchTerm { get; set; } = string.Empty;
    }

    public class EmpInfo
    {
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string EmpAddress { get; set; }
        public string EmpPhNo { get; set; }
        public string EmpMNo { get; set; }
        public bool Gender { get; set; }
        public string EmpDOB { get; set; }
        public bool EmpMarried { get; set; }
        public int ModifyBy { get; set; }
        public string ModifyDate { get; set; }
        public string CountryCode { get; set; }
        public string EmpPhoto { get; set; }
    }
    public class EmpImg
    {
        public int EmpId { get; set; }
        public string EmpPhoto { get; set; }
        public int ModifyBy { get; set; }
        public string ModifyDate { get; set; }
    }

    public class ImportedDataEmp
    {
        #region Properties

        public string Empcode { get; set; }
        public string EmpName { get; set; }
        public string EmpPunchID { get; set; }
        //public double TEmpPunchID { get; set; }
        public string EmpMarried { get; set; }
        public string EmpJoinDate { get; set; }
        public string EmpBirthDate { get; set; }
        public string EmpDepartment { get; set; }
        public string EmpDesignation { get; set; }
        public string EmpShift { get; set; }
        public string EmpAddress { get; set; }
        public string EmpPhone { get; set; }
        public string EmpMobile { get; set; }
        public string EmpEmail { get; set; }
        public string Rolename { get; set; }
        public string Gender { get; set; }
        public string PolicyName { get; set; }
        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public string EmpShiftGroup { get; set; }
        public string CountryCode { get; set; }
        public string BranchName { get; set; }
        public string AdharCard { get; set; }
        #endregion
    }

    public class Importfaillog
    {
        public int ImportedFailId { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string EmpPunchID { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string Startdate { get; set; }
        public string CreatedDtFlag { get; set; }
        public string Err { get; set; }
        public IEnumerable<Importfaillog> Importfailloglist { get; set; }
        public IEnumerable<ImportCompfaillog> ImportCompfailloglist { get; set; }
        public IEnumerable<ImportBranchfaillog> ImportBranchfailloglist { get; set; }
        public IEnumerable<ImportDeptfaillog> ImportDeptfailloglist { get; set; }
        public IEnumerable<ImportDesgfaillog> ImportDesgfailloglist { get; set; }
    }

    public class Importstatuslog
    {
        public string Status { get; set; }
        public string Startdate { get; set; }

    }

    public class Importexcelfaillog
    {
        public int ImportedFailId { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string EmpPunchID { get; set; }
        public string Reason { get; set; }

        public string EmpMarried { get; set; }
        public string EmpJoinDate { get; set; }
        public string EmpBirthDate { get; set; }
        public string EmpDepartment { get; set; }
        public string EmpDesignation { get; set; }
        public string EmpShift { get; set; }
        public string EmpAddress { get; set; }
        public string EmpPhone { get; set; }
        public string EmpMobile { get; set; }
        public string EmpEmail { get; set; }
        public string Rolename { get; set; }
        public string Gender { get; set; }
        public string PolicyName { get; set; }
        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public string EmpShiftGroup { get; set; }
        public string CountryCode { get; set; }
        public string BranchName { get; set; }
        public string Status { get; set; }
        public IEnumerable<Importfaillog> Importfailloglist { get; set; }
        public IEnumerable<ImportCompfaillog> ImportCompfailloglist { get; set; }
        public IEnumerable<ImportBranchfaillog> ImportBranchfailloglist { get; set; }
        public IEnumerable<ImportDeptfaillog> ImportDeptfailloglist { get; set; }
        public IEnumerable<ImportDesgfaillog> ImportDesgfailloglist { get; set; }
    }

    public class ImportCompfaillog
    {
        public int ImportedFailId { get; set; }
        public string CompanyName { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string Startdate { get; set; }
        public string CreatedDtFlag { get; set; }

        public string Email { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string CompanyWebsite { get; set; }
    }

    public class ImportBranchfaillog
    {
        public int ImportedFailId { get; set; }
        public string CompanyID { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }
        public string CompanyName { get; set; }
        public string BranchAddress { get; set; }
        public string BranchHeadEmail { get; set; }
        public string BranchHeadPassword { get; set; }
        public string ReportingBranchName { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string Startdate { get; set; }
        public string CreatedDtFlag { get; set; }
    }

    public class ImportDeptfaillog
    {
        public int ImportedFailId { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentEmail { get; set; }
        public string DepartmentHeadName { get;set; }
        public string Reason { get; set; }
    }

    public class ImportDesgfaillog
    {
        public int ImportedFailId { get; set; }
        public string DesignationID { get; set; }
        public string DesignationName { get; set; }
        public string Reason { get; set; }
    }

    public class ValidEmployee
    {
        public int EmpId { get; set; }
        public string Empcode { get; set; }
        public string EmpName { get; set; }
        public string EmpPunchID { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int DeviceUserId { get; set; }
        public string EnrollNo { get; set; }
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceSrNo { get; set; }
        public string DeviceCode { get; set; }
        public string DeviceIP { get; set; }
        public int DeviceType { get; set; }
        public string DeviceTypeName { get; set; }
    }
    public class EmployeeGetAllByDept
    {
        public string departmentid { get; set; }
        public string branchid { get; set; }
        public string isActiveEmployee { get; set; }
        public string Empcode { get; set; }     
    }
	
	public class EnrollUserParameter
    {
        public int CmpId { get; set; }
        public int BranchId { get; set; }
        public string DepartmentId { get; set; }
        public int StatusId { get; set; }
    }
    public class GetAllBranchstr
    {
        public string BranchId { get; set; }
    }

    public class GetAllBranchDepartstr
    {
        public string BranchId { get; set; }
        public string DeptId { get; set; }
    }


    public class Policies
    {
        public int policy_id { get; set; }
        public string policy_title { get; set; }

        public string policy_description { get; set; }

        public string document_path { get; set; }

    }


    public class DeHiring
    {
        public string EmpPunchID { get; set; }
        public string EmpName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Gender { get; set; }
        public string LastWorkingDate { get; set; }
        public string ReasonForLeaving { get; set; }
        public string RehireStatus { get; set; }
    }

    public class ImportDeHiringResponse
    {
        public int statusCode { get; set; }
        public bool status { get; set; }
        public List<DeHiring> DeHiringList { get; set; }
    }

    public class ImportDeHiringfaillog
    {
        public int EmpPunchID { get; set; }
        public string EmpName { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string Startdate { get; set; }
        public string CreatedDtFlag { get; set; }
        public IEnumerable<ImportDeHiringfaillog> DehiringError { get; set; }
    }

    public class EmpFilter
    {
        public string BranchStr { get; set; }
        public string DeptStr { get; set; }
        public string DesgStr { get; set; }
    }
}