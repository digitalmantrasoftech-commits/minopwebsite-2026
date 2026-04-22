using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class SchoolUtilitiesController : Controller
    {
        SchoolMasterDataRestClient RestClient = new SchoolMasterDataRestClient();
        public ActionResult ImportStudent(ImportedFailLog faillog)
        {
            return View(faillog);
        }
        [HttpPost]
        public ActionResult ImportStudent(HttpPostedFileBase FileUpload, int SchoolId, int ClassId, int DivisionId, int Mastervalue)
        {
            string filePath = string.Empty;
            DataTable dt = new DataTable();
            Utility utl = new Utility();
            //ImportedStudentfaillog faillog = new ImportedStudentfaillog();
            ImportedFailLog faillog = new ImportedFailLog();
            if (Request.Files["FileUpload"].ContentLength > 0)
            {
                string extension = System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName).ToLower();
                string connString = "";
                string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
                string path1 = string.Format("{0}/{1}", Server.MapPath("~/Uploads"), Request.Files["FileUpload"].FileName);
                if (!Directory.Exists(path1))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Uploads"));
                }
                if (validFileTypes.Contains(extension))
                {
                    if (System.IO.File.Exists(path1))
                    {
                        System.IO.File.Delete(path1);
                    }
                    Request.Files["FileUpload"].SaveAs(path1);
                    if (extension == ".csv")
                    {
                        dt = utl.ConvertCSVtoDataTable(path1);
                        ViewBag.Data = dt;
                    }
                    //Connection String to Excel Workbook
                    else if (extension.Trim() == ".xls")
                    {
                        connString = "Provider=Microsoft.Ace.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=YES\"";
                        dt = utl.ConvertStudentXSLXtoDataTable(path1, connString);
                        if (dt.Rows.Count > 0)
                        {
                            dt = dt.Rows.Cast<DataRow>()
                                    .Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field as string), string.Empty) == 0))
                                    .CopyToDataTable();
                        }
                        ViewBag.Data = dt;
                    }
                    else if (extension.Trim() == ".xlsx")
                    {
                        connString = "Provider=Microsoft.Ace.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=YES\"";
                        dt = utl.ConvertStudentXSLXtoDataTable(path1, connString);
                        if (dt.Rows.Count > 0)
                        {
                            dt = dt.Rows.Cast<DataRow>()
                                    .Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field as string), string.Empty) == 0))
                                    .CopyToDataTable();
                        }
                        ViewBag.Data = dt;
                    }
                    if (Mastervalue == 1)
                    {
                        try
                        {
                            List<ImportedClass> Classlist = new List<ImportedClass>();
                            Classlist = (from DataRow row in dt.Rows
                                         select new ImportedClass
                                         {
                                             SchoolName = row["SchoolName"].ToString(),
                                             ClassName = row["ClassName"].ToString()
                                         }).ToList();
                            faillog.ImportedFailLoglist = RestClient.ImportedClassfaillogGetAll(Classlist);
                        }
                        catch (Exception)
                        {
                            ImportedFailLog i = new ImportedFailLog();
                            i.ImportedFailId = 1;
                            i.Reason = "File imported is not in proper format.";
                            List<ImportedFailLog> list = new List<ImportedFailLog>();
                            list.Add(i);
                            faillog.ImportedFailLoglist = list;
                        }
                    }
                    else if (Mastervalue == 2)
                    {
                        try
                        {
                            List<ImportedDivision> Divisionlist = new List<ImportedDivision>();
                            Divisionlist = (from DataRow row in dt.Rows
                                            select new ImportedDivision
                                            {
                                                DivisionName = row["DivisionName"].ToString()
                                            }).ToList();
                            faillog.ImportedFailLoglist = RestClient.ImportedDivisionfaillogGetAll(Divisionlist);
                        }
                        catch (Exception)
                        {
                            ImportedFailLog i = new ImportedFailLog();
                            i.ImportedFailId = 1;
                            i.Reason = "File imported is not in proper format.";
                            List<ImportedFailLog> list = new List<ImportedFailLog>();
                            list.Add(i);
                            faillog.ImportedFailLoglist = list;
                        }
                    }
                    else if (Mastervalue == 3)
                    {
                        try
                        {
                            List<ImportedShift> Shiftlist = new List<ImportedShift>();
                            Shiftlist = (from DataRow row in dt.Rows
                                         select new ImportedShift
                                         {
                                             ShiftName = row["ShiftName"].ToString(),
                                             StartTime = row["StartTime"].ToString(),
                                             EndTime = row["EndTime"].ToString()
                                         }).ToList();
                            faillog.ImportedFailLoglist = RestClient.ImportedShiftfaillogGetAll(Shiftlist);
                        }
                        catch (Exception)
                        {
                            ImportedFailLog i = new ImportedFailLog();
                            i.ImportedFailId = 1;
                            i.Reason = "File imported is not in proper format.";
                            List<ImportedFailLog> list = new List<ImportedFailLog>();
                            list.Add(i);
                            faillog.ImportedFailLoglist = list;
                        }
                    }
                    else if (Mastervalue == 5)
                    {
                        try
                        {
                            List<ImportedStudent> itemsStudent = new List<ImportedStudent>();
                            itemsStudent = (from DataRow row in dt.Rows
                                            select new ImportedStudent
                                            {
                                                DivisionId = DivisionId,
                                                FirstName = row["FirstName"].ToString(),
                                                LastName = row["LastName"].ToString(),
                                                DateOfBirth = row["DateOfBirth"].ToString(),
                                                Gender = row["Gender"].ToString(),
                                                PunchId = row["PunchId"].ToString(),
                                                ContactNo = row["ContactNo"].ToString(),
                                                EmgContactNo = row["EmgContactNo"].ToString(),
                                                Address = row["Address"].ToString(),
                                                Email = row["Email"].ToString(),
                                                Shift = row["Shift"].ToString(),
                                                SchoolId = SchoolId,
                                                ClassId = ClassId,
                                                ParentType = row["ParentType"].ToString(),
                                                ParentName = row["ParentName"].ToString(),
                                                ParentContactNo = row["ParentContactNo"].ToString(),
                                                ParentOccupation = row["ParentOccupation"].ToString(),
                                                Policy = row["Policy"].ToString(),
                                                Tagid = row["TagId"].ToString(),
                                                EmpCode = row["GRNo"].ToString()
                                            }).ToList();
                            faillog.ImportedFailLoglist = RestClient.ImportedStudentfaillogGetAll(itemsStudent);
                        }
                        catch (Exception)
                        {
                            ImportedFailLog i = new ImportedFailLog();
                            i.ImportedFailId = 1;
                            i.Reason = "File imported is not in proper format.";
                            List<ImportedFailLog> list = new List<ImportedFailLog>();
                            list.Add(i);
                            faillog.ImportedFailLoglist = list;
                        }
                    }
                    else if (Mastervalue == 6)
                    {
                        try
                        {
                            List<ImportedStaffType> StaffTypelist = new List<ImportedStaffType>();
                            StaffTypelist = (from DataRow row in dt.Rows
                                             select new ImportedStaffType
                                             {
                                                 StaffTypeName = row["StaffTypeName"].ToString()
                                             }).ToList();

                            faillog.ImportedFailLoglist = RestClient.ImportedStaffTypefaillogGetAll(StaffTypelist);
                        }
                        catch (Exception)
                        {
                            ImportedFailLog i = new ImportedFailLog();
                            i.ImportedFailId = 1;
                            i.Reason = "File imported is not in proper format.";
                            List<ImportedFailLog> list = new List<ImportedFailLog>();
                            list.Add(i);
                            faillog.ImportedFailLoglist = list;
                        }
                    }
                    else if (Mastervalue == 7)
                    {
                        try
                        {
                            List<ImportedStaff> Stafflist = new List<ImportedStaff>();
                            Stafflist = (from DataRow row in dt.Rows
                                         select new ImportedStaff
                                         {
                                             SchoolName = row["SchoolName"].ToString(),
                                             Name = row["FirstName"].ToString(),
                                             SurName = row["LastName"].ToString(),
                                             DateOfBirth = row["DateOfBirth"].ToString(),
                                             Gender = row["Gender"].ToString(),
                                             MaritalStatus = row["MaritalStatus"].ToString(),
                                             ContactNo = row["ContactNo"].ToString(),
                                             Email = row["Email"].ToString(),
                                             Address = row["Address"].ToString(),
                                             Speciality = row["Speciality"].ToString(),
                                             Salary = row["Salary"].ToString(),
                                             Punchid = row["PunchID"].ToString(),
                                             StaffTypeName = row["StaffType"].ToString(),
                                             ReportingPerson = row["ReportingTo"].ToString()
                                         }).ToList();

                            faillog.ImportedFailLoglist = RestClient.ImportedStafffaillogGetAll(Stafflist);
                        }
                        catch (Exception)
                        {
                            ImportedFailLog i = new ImportedFailLog();
                            i.ImportedFailId = 1;
                            i.Reason = "File imported is not in proper format.";
                            List<ImportedFailLog> list = new List<ImportedFailLog>();
                            list.Add(i);
                            faillog.ImportedFailLoglist = list;
                        }
                    }
                    else if (Mastervalue == 8)
                    {
                        try
                        {
                            List<ImportedDevice> Deviceslist = new List<ImportedDevice>();
                            Deviceslist = (from DataRow row in dt.Rows
                                           select new ImportedDevice
                                           {
                                               CompanyName = row["SchoolName"].ToString(),
                                               DeviceType = row["DeviceType"].ToString(),
                                               DeviceSrNo = row["DeviceSrNo"].ToString(),
                                               Mode = row["DeviceMode"].ToString(),
                                               DeviceName = row["DeviceName"].ToString(),
                                               DeviceCode = row["DeviceCode"].ToString(),
                                               DeviceIP = row["DeviceIP"].ToString(),
                                               DevicePassword = row["DevicePassword"].ToString(),
                                               DevicePort = row["DevicePort"].ToString(),
                                               Location = row["Location"].ToString(),
                                               IsAttendance = row["IsAttendance"].ToString()
                                           }).ToList();
                            faillog.ImportedFailLoglist = RestClient.ImportedDevicefaillogGetAll(Deviceslist);
                        }
                        catch (Exception)
                        {
                            ImportedFailLog i = new ImportedFailLog();
                            i.ImportedFailId = 1;
                            i.Reason = "File imported is not in proper format.";
                            List<ImportedFailLog> list = new List<ImportedFailLog>();
                            list.Add(i);
                            faillog.ImportedFailLoglist = list;
                        }
                    }
                    else if (Mastervalue == 9)
                    {
                        try
                        {
                            List<ImportedGate> Gatelist = new List<ImportedGate>();
                            Gatelist = (from DataRow row in dt.Rows
                                        select new ImportedGate
                                        {
                                            GateNumber = row["GateNumber"].ToString(),
                                            VehicleNumber = row["VehicleNumber"].ToString()
                                        }).ToList();
                            faillog.ImportedFailLoglist = RestClient.ImportedGatefaillogGetAll(Gatelist);
                        }
                        catch (Exception)
                        {
                            ImportedFailLog i = new ImportedFailLog();
                            i.ImportedFailId = 1;
                            i.Reason = "File imported is not in proper format.";
                            List<ImportedFailLog> list = new List<ImportedFailLog>();
                            list.Add(i);
                            faillog.ImportedFailLoglist = list;
                        }
                    }
                    else if (Mastervalue == 10)
                    {
                        try
                        {
                            List<ImportedDriver> Driverlist = new List<ImportedDriver>();
                            Driverlist = (from DataRow row in dt.Rows
                                          select new ImportedDriver
                                          {
                                              DriverName = row["DriverName"].ToString(),
                                              ContactNo = row["ContactNo"].ToString(),
                                              PresentAddress = row["PresentAddress"].ToString(),
                                              PermanentAddress = row["PermanentAddress"].ToString(),
                                              DateOfBirth = row["DateOfBirth"].ToString(),
                                              VehicleNumber = row["VehicleNumber"].ToString(),
                                              LicenseNumber = row["LicenseNumber"].ToString()
                                          }).ToList();
                            faillog.ImportedFailLoglist = RestClient.ImportedDriverfaillogGetAll(Driverlist);
                        }
                        catch (Exception)
                        {
                            ImportedFailLog i = new ImportedFailLog();
                            i.ImportedFailId = 1;
                            i.Reason = "File imported is not in proper format.";
                            List<ImportedFailLog> list = new List<ImportedFailLog>();
                            list.Add(i);
                            faillog.ImportedFailLoglist = list;
                        }
                    }
                    else if (Mastervalue == 11)
                    {
                        try
                        {
                            List<ImportedVehicle> Vehiclelist = new List<ImportedVehicle>();
                            Vehiclelist = (from DataRow row in dt.Rows
                                           select new ImportedVehicle
                                           {
                                               VehicleNo = row["VehicleNo"].ToString(),
                                               NoofSeats = row["NoofSeats"].ToString(),
                                               VehicleTypeName = row["VehicleType"].ToString(),
                                               Imeinumber = row["Imeinumber"].ToString(),
                                               Enginenumber = row["Enginenumber"].ToString(),
                                               Makeyear = row["Makeyear"].ToString(),
                                               Insurancestartdate = row["Insurancestartdate"].ToString(),
                                               Insuranceenddate = row["Insuranceenddate"].ToString(),
                                               Registrationstartdate = row["Registrationstartdate"].ToString(),
                                               Registrationenddate = row["Registrationenddate"].ToString(),
                                               Isownvehicle = row["Isownvehicle"].ToString(),
                                               ChassisVinnumber = row["Chassisnumber"].ToString(),
                                               FueltypeName = row["Fueltype"].ToString()
                                           }).ToList();
                            faillog.ImportedFailLoglist = RestClient.ImportedVehiclefaillogGetAll(Vehiclelist);
                        }
                        catch (Exception)
                        {
                            ImportedFailLog i = new ImportedFailLog();
                            i.ImportedFailId = 1;
                            i.Reason = "File imported is not in proper format.";
                            List<ImportedFailLog> list = new List<ImportedFailLog>();
                            list.Add(i);
                            faillog.ImportedFailLoglist = list;
                        }
                    }
                }
                else
                {
                    ViewBag.error = "Please Upload Files in .xls, .xlsx or .csv format";
                }

                //faillog.ImportedFailLoglist = RestClient.ImportedStudentfaillogGetAll(items);
                if (faillog.ImportedFailLoglist.ToList().Count > 0)
                {
                    TempData["IsValid"] = false;
                }
                else
                {
                    TempData["IsValid"] = true;
                }
                return ImportStudent(faillog);
            }
            else
            {
                return ImportStudent(faillog);
            }
        }
    }
}