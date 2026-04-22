using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class ImportedStudent
    {
        public int SchoolId { get; set; }
        public int ClassId { get; set; }
        public string Division { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PunchId { get; set; }
        public string ContactNo { get; set; }
        public string EmgContactNo { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Shift { get; set; }
        public string Policy { get; set; }
        public string ParentType { get; set; }
        public int DivisionId { get; set; }
        public string ParentName { get; set; }
        public string ParentContactNo { get; set; }
        public string ParentOccupation { get; set; }
        public string Tagid { get; set; }
        public string EmpCode { get; set; }
    }
    public class ImportedGate
    {
        public string GateNumber { get; set; }
        public string VehicleNumber { get; set; }
    }

    public class ImportedStaffType {
        public string StaffTypeName { get; set; }
    }
    public class ImportedDriver
    {
        public string DriverName { get; set; }
        public string ContactNo { get; set; }
        public string PresentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string DateOfBirth { get; set; }
        public string VehicleNumber { get; set; }
        public string LicenseNumber { get; set; }
    }

    public class ImportedStudentfaillog
    {
        public int ImportedFailId { get; set; }
        public string PunchId { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string Dateofbirth { get; set; }
        public string Gender { get; set; }
        public string ContactNo { get; set; }
        public string Shift { get; set; }
        public string Division { get; set; }
        public string EmgContactNo { get; set; }
        public string Address { get; set; }
        public string ParentType { get; set; }
        public string ParentName { get; set; }
        public string ParentContactNo { get; set; }
        public string Reason { get; set; }
        public IEnumerable<ImportedStudentfaillog> ImportedStudentfailloglist { get; set; }
    }
    public class ImportedFailLog
    {
        public int ImportedFailId { get; set; }
        public string RowNo { get; set; }
        public string Reason { get; set; }
        public IEnumerable<ImportedFailLog> ImportedFailLoglist { get; set; }
    }
    public class ImportedStaff
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string DateOfBirth { get; set; }
        public string strDateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Genderid { get; set; }
        public string MaritalStatus { get; set; }
        public int Marriedid { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Speciality { get; set; }
        public string Salary { get; set; }
        public string Punchid { get; set; }
        public string StaffTypeName { get; set; }
        public int StaffTypeId { get; set; }
        public string ReportingPerson { get; set; }
        public int ReportingTo { get; set; }
    }
    public class ImportedDevice
    {
        public int DeviceId { get; set; }
        public string DeviceCode { get; set; }
        public int BranchId { get; set; }
        public string DeviceIP { get; set; }
        public string DevicePort { get; set; }
        public string DevicePassword { get; set; }
        public string DeviceName { get; set; }
        public string Mode { get; set; }
        public int DeviceTypeCode { get; set; }
        public string DeviceType { get; set; }
        public int IsPushData { get; set; }
        public bool IsActive { get; set; }
        public string DeviceSrNo { get; set; }
        public string RegCompanyCode { get; set; }
        public string DeviceStatus { get; set; }
        public string BranchName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public int isDisebleDw { get; set; }
        public int isDisebleUp { get; set; }
        public bool IsSchool { get; set; }
        public string IsAttendance { get; set; }
        public int LocatedAt { get; set; }
        public string Location { get; set; }
    }
    public class ImportedVehicle
    {
        public int VehicleId { get; set; }
        public string VehicleNo { get; set; }
        public string NoofSeats { get; set; }
        public string VehicleTypeName { get; set; }
        public int VehicleType { get; set; }
        public string Imeinumber { get; set; }
        public string Simcardnumber { get; set; }
        public string Simphonenumber { get; set; }
        public int TrackingDevicetypeid { get; set; }
        public string Enginenumber { get; set; }
        public string Makeyear { get; set; }
        public string Insurancestartdate { get; set; }
        public string Insuranceenddate { get; set; }
        public string Registrationstartdate { get; set; }
        public string Registrationenddate { get; set; }
        public string Isownvehicle { get; set; }
        public int Fleetmanagerid { get; set; }
        public string ChassisVinnumber { get; set; }
        public string FueltypeName { get; set; }
        public int Fueltype { get; set; }
        public int Actionby { get; set; }
        public bool IsActive { get; set; }
    }
    public class ImportedShift
    {
        public string ShiftName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Duration { get; set; }
    }
    public class ImportedClass
    {
        public string SchoolName { get; set; }
        public int CompanyId { get; set; }
        public string ClassName { get; set; }
        public int BranchId { get; set; }
    }
    public class ImportedDivision
    {
        public string DivisionName { get; set; }
    }

}