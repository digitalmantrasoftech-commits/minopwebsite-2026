using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace PayTimeWebClient.Helper
{
    public class ExportExcelFileMaster
    {
        public ExportResult SaveExcelToServer(DataTable dataTable, string filePrefix, string deleteFilePattern)
        {
            try
            {
                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return new ExportResult { success = false, message = "No data available for export." };
                }
                var byteArray = GenerateExcelContent(dataTable, filePrefix);
                var appRootPath = HttpContext.Current.Server.MapPath("~/");
                var folderName = "ExportsExcelData";
                var fileDirectory = Path.Combine(appRootPath, folderName);

                if (!Directory.Exists(fileDirectory))
                    Directory.CreateDirectory(fileDirectory);

                var existingFiles = Directory.GetFiles(fileDirectory, deleteFilePattern + "*");
                foreach (var file in existingFiles)
                    System.IO.File.Delete(file);

                var fileName = $"{filePrefix}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var filePath = Path.Combine(fileDirectory, fileName);
                System.IO.File.WriteAllBytes(filePath, byteArray);

                var downloadUrl = "/ExportsExcelData/" + fileName;

                return new ExportResult { success = true, filePath = downloadUrl };
            }
            catch (Exception ex)
            {
                return new ExportResult { success = false, message = ex.Message };
            }
        }
        public ExportResult SaveExcelToServerrpt(DataTable dataTable, string filePrefix, string deleteFilePattern)
        {
            try
            {
                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return new ExportResult { success = false, message = "No data available for export." };
                }
                var byteArray = GenerateExcelContentReport(dataTable, filePrefix);
                var appRootPath = HttpContext.Current.Server.MapPath("~/");
                var folderName = "ExportsExcelData";
                var fileDirectory = Path.Combine(appRootPath, folderName);

                if (!Directory.Exists(fileDirectory))
                    Directory.CreateDirectory(fileDirectory);

                var existingFiles = Directory.GetFiles(fileDirectory, deleteFilePattern + "*");
                foreach (var file in existingFiles)
                    System.IO.File.Delete(file);

                var fileName = $"{filePrefix}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var filePath = Path.Combine(fileDirectory, fileName);
                System.IO.File.WriteAllBytes(filePath, byteArray);

                var downloadUrl = "/ExportsExcelData/" + fileName;

                return new ExportResult { success = true, filePath = downloadUrl };
            }
            catch (Exception ex)
            {
                return new ExportResult { success = false, message = ex.Message };
            }
        }
        public ExportResult SaveExcelToServerrptMuster(DataTable dataTable, string filePrefix, string deleteFilePattern)
        {
            try
            {
                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return new ExportResult { success = false, message = "No data available for export." };
                }
                var byteArray = GenerateExcelContentReportMuster(dataTable, filePrefix);
                var appRootPath = HttpContext.Current.Server.MapPath("~/");
                var folderName = "ExportsExcelData";
                var fileDirectory = Path.Combine(appRootPath, folderName);

                if (!Directory.Exists(fileDirectory))
                    Directory.CreateDirectory(fileDirectory);

                var existingFiles = Directory.GetFiles(fileDirectory, deleteFilePattern + "*");
                foreach (var file in existingFiles)
                    System.IO.File.Delete(file);

                var fileName = $"{filePrefix}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var filePath = Path.Combine(fileDirectory, fileName);
                System.IO.File.WriteAllBytes(filePath, byteArray);

                var downloadUrl = "/ExportsExcelData/" + fileName;

                return new ExportResult { success = true, filePath = downloadUrl };
            }
            catch (Exception ex)
            {
                return new ExportResult { success = false, message = ex.Message };
            }
        }

        public ExportResult SaveExcelToServerrptDuraction(DataTable dataTable, string filePrefix, string deleteFilePattern)
        {
            try
            {
                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return new ExportResult { success = false, message = "No data available for export." };
                }
                var byteArray = GenerateExcelContentReportDuraction(dataTable, filePrefix);
                var appRootPath = HttpContext.Current.Server.MapPath("~/");
                var folderName = "ExportsExcelData";
                var fileDirectory = Path.Combine(appRootPath, folderName);

                if (!Directory.Exists(fileDirectory))
                    Directory.CreateDirectory(fileDirectory);

                var existingFiles = Directory.GetFiles(fileDirectory, deleteFilePattern + "*");
                foreach (var file in existingFiles)
                    System.IO.File.Delete(file);

                var fileName = $"{filePrefix}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var filePath = Path.Combine(fileDirectory, fileName);
                System.IO.File.WriteAllBytes(filePath, byteArray);

                var downloadUrl = "/ExportsExcelData/" + fileName;

                return new ExportResult { success = true, filePath = downloadUrl };
            }
            catch (Exception ex)
            {
                return new ExportResult { success = false, message = ex.Message };
            }
        }
        public ExportResult SaveExcelToServerrptDuractionAttendance(DataTable dataTable, string filePrefix, string deleteFilePattern)
        {
            try
            {
                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return new ExportResult { success = false, message = "No data available for export." };
                }

                // Generate the File Content
                var byteArray = GenerateExcelContentVertical(dataTable);

                // Save logic
                var appRootPath = System.Web.HttpContext.Current.Server.MapPath("~/");
                var folderName = "ExportsExcelData";
                var fileDirectory = System.IO.Path.Combine(appRootPath, folderName);

                if (!System.IO.Directory.Exists(fileDirectory))
                    System.IO.Directory.CreateDirectory(fileDirectory);

                // Delete old files
                var existingFiles = System.IO.Directory.GetFiles(fileDirectory, deleteFilePattern + "*");
                foreach (var file in existingFiles)
                {
                    try { System.IO.File.Delete(file); } catch { }
                }

                // Save new file
                var fileName = $"{filePrefix}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var filePath = System.IO.Path.Combine(fileDirectory, fileName);
                System.IO.File.WriteAllBytes(filePath, byteArray);

                var downloadUrl = "/ExportsExcelData/" + fileName;

                return new ExportResult { success = true, filePath = downloadUrl };
            }
            catch (Exception ex)
            {
                return new ExportResult { success = false, message = ex.Message };
            }
        }

        private byte[] GenerateExcelContent(DataTable dataTable, string headername)
            {
                try
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (ExcelPackage package = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                        int totalColumns = dataTable.Columns.Count;

                        // Use DataTable.TableName as the title
                        //string dynamicTitle = !string.IsNullOrWhiteSpace(dataTable.) ? dataTable.TableName : "Report";

                        // 1. Add merged title row at Row 1
                        var titleCell = worksheet.Cells[1, 1, 1, totalColumns];
                        titleCell.Merge = true;
                        titleCell.Value = headername;
                        titleCell.Style.Font.Bold = true;
                        titleCell.Style.Font.Size = 14;
                        titleCell.Style.Font.Color.SetColor(Color.White);
                        titleCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        titleCell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#2F75B5"));
                        titleCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        titleCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        // 2. Header row
                        for (int i = 0; i < totalColumns; i++)
                        {
                            var cell = worksheet.Cells[2, i + 1];
                            cell.Value = dataTable.Columns[i].ColumnName;
                            cell.Style.Font.Bold = true;
                            cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        }

                        // 3. Data rows
                        for (int row = 0; row < dataTable.Rows.Count; row++)
                        {
                            for (int col = 0; col < totalColumns; col++)
                            {
                                worksheet.Cells[row + 3, col + 1].Value = dataTable.Rows[row][col];
                            }
                        }

                        worksheet.Cells.AutoFitColumns();

                        return package.GetAsByteArray();
                    }
                }
                catch (Exception ex)
                {
                    using (ExcelPackage emptyPackage = new ExcelPackage())
                    {
                        ExcelWorksheet emptySheet = emptyPackage.Workbook.Worksheets.Add("Error");
                        emptySheet.Cells[1, 1].Value = "An error occurred while generating Excel.";
                        return emptyPackage.GetAsByteArray();
                    }
                }
            }
        private byte[] GenerateExcelContentReport(DataTable dataTable, string headername)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
                    int totalColumns = dataTable.Columns.Count;

                    // 1. Title row (Row 1)
                    var titleCell = worksheet.Cells[1, 1, 1, totalColumns];
                    titleCell.Merge = true;
                    titleCell.Value = headername;
                    titleCell.Style.Font.Bold = true;
                    titleCell.Style.Font.Size = 14;
                    titleCell.Style.Font.Color.SetColor(Color.White);
                    titleCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    titleCell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#2F75B5"));
                    titleCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    titleCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    // 2. Header row (Row 2)
                    for (int i = 0; i < totalColumns; i++)
                    {
                        var cell = worksheet.Cells[2, i + 1];
                        cell.Value = dataTable.Columns[i].ColumnName;
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    }

                    // 3. Data rows using fast method
                    worksheet.Cells[3, 1].LoadFromDataTable(dataTable, false);

                    worksheet.Cells.AutoFitColumns();

                    return package.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                using (ExcelPackage emptyPackage = new ExcelPackage())
                {
                    ExcelWorksheet emptySheet = emptyPackage.Workbook.Worksheets.Add("Error");
                    emptySheet.Cells[1, 1].Value = "An error occurred while generating Excel.";
                    return emptyPackage.GetAsByteArray();
                }
            }
        }
        private byte[] GenerateExcelContentReportMuster(DataTable dataTable, string headername)
        {
            try
            {
              
                
                if (dataTable == null)
                {
                   
                    throw new ArgumentNullException(nameof(dataTable), "DataTable cannot be null for Excel generation");
                }
                
                if (dataTable.Rows.Count == 0)
                {
                  
                    throw new ArgumentException("DataTable must contain data for Excel generation", nameof(dataTable));
                }
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Monthly Muster Report");
                    int currentRow = 1;

                    // Get month info from backend Month_Year field or current date
                    DateTime reportMonth = DateTime.Now;
                    if (dataTable.Rows.Count > 0 && dataTable.Columns.Contains("Month-Year"))
                    {
                        var monthYearStr = dataTable.Rows[0].Field<string>("Month-Year");
                        if (!string.IsNullOrEmpty(monthYearStr))
                        {
                            
                            if (DateTime.TryParseExact(
                                    monthYearStr,
                                    "MMM-yyyy",  
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    System.Globalization.DateTimeStyles.None,
                                    out DateTime parsedDate))
                            {
                                reportMonth = parsedDate;
                            }
                        }
                    }

                    int daysInMonth = DateTime.DaysInMonth(reportMonth.Year, reportMonth.Month);

                    // Analyze data variation to determine which company columns are needed
                    var allCompanyNames = dataTable.AsEnumerable().Select(r => r.Field<string>("Company") ?? "").Distinct().ToList();
                    var allBranchNames = dataTable.AsEnumerable().Select(r => r.Field<string>("Branch") ?? "").Distinct().ToList();
                    var allDepartmentNames = dataTable.AsEnumerable().Select(r => r.Field<string>("Department") ?? "").Distinct().ToList();

                    // Always show all company columns - make them variable, not conditional
                    bool showCompanyColumn = true;    // Always show
                    bool showBranchColumn = true;     // Always show  
                    bool showDepartmentColumn = true; // Always show

                    // Create dynamic column mapping
                    var dynamicColumns = new List<dynamic>
                    {
                        new { header = "Company", field = "Company", show = showCompanyColumn },
                        new { header = "Branch", field = "Branch", show = showBranchColumn },
                        new { header = "Department", field = "Department", show = showDepartmentColumn },
                        new { header = "Designation", field = "Designation", show = true }  // Add Designation as variable column
                    };

                    int dynamicColumnCount = dynamicColumns.Count(c => c.show);
                    
                    // Get custom leave columns from data - all columns after known standard columns
                    var customLeaveColumns = new List<string>();
                    if (dataTable.Rows.Count > 0)
                    {
                        // Define known standard columns that are not leave columns
                        var knownColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                        {
                            "Company", "Branch", "Department", "Designation",
                            "EmpCode", "EmployeeName", "EmpName", "HolidayCount", "WeekendCount", 
                            "PresentCount", "AbsentCount", "PaidLeave", "TotalHours", "PresentDays", "OTHours", "Month_Year",
                            "P", "A", "H", "Week Off", "Paid_Leave", "TotalHrs", "Present Days(P+WO+H+PL)", "OT Hours", "P_day", "OTHr","Error_Case","Less_Hrs","Half_Day"
                        };

                        // Dynamically detect actual day columns in the data (preserve backend order)
                        var dayColumns = dataTable.Columns.Cast<DataColumn>()
                            .Where(c => System.Text.RegularExpressions.Regex.IsMatch(c.ColumnName, @"^d\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                            .Select(c => c.ColumnName)
                            .ToList();
                            
                        // Add detected day columns to known columns
                        foreach (var dayCol in dayColumns)
                        {
                            knownColumns.Add(dayCol);
                        }

                        // All remaining columns are leave columns
                        customLeaveColumns = dataTable.Columns.Cast<DataColumn>()
                            .Where(c => !knownColumns.Contains(c.ColumnName))
                            .Select(c => c.ColumnName)
                            .ToList();
                    }
                    
                    // Calculate total columns for the detailed table
                    int fixedColumns = 1; // Only Description column is fixed
                    int variableCompanyColumns = dynamicColumnCount; // Company columns are variable/movable
                    int dailyColumns = customLeaveColumns.Count > 0 ? 
                        dataTable.Columns.Cast<DataColumn>()
                            .Count(c => System.Text.RegularExpressions.Regex.IsMatch(c.ColumnName, @"^d\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)) :
                        daysInMonth; // Use detected day columns or fallback to daysInMonth
                    int summaryColumns = 2 + customLeaveColumns.Count; // PresentDays + OTHours + leave columns
                    int totalDetailedColumns = fixedColumns + variableCompanyColumns + dailyColumns + summaryColumns;

                    // 1. HEADER SECTION
                    // Title row (Row 1)
                    var titleCell = worksheet.Cells[currentRow, 1, currentRow, Math.Max(totalDetailedColumns, 10)];
                    titleCell.Merge = true;
                    titleCell.Value = headername;
                    titleCell.Style.Font.Bold = true;
                    titleCell.Style.Font.Size = 14;
                    titleCell.Style.Font.Color.SetColor(Color.White);
                    titleCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    titleCell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#295097"));
                    titleCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    titleCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Row(currentRow).Height = 25;
                    currentRow++;

                    // Month/Year row (Row 2)
                    var monthCell = worksheet.Cells[currentRow, 1, currentRow, Math.Max(totalDetailedColumns, 10)];
                    monthCell.Merge = true;
                    monthCell.Value = reportMonth.ToString("MMMM yyyy");
                    monthCell.Style.Font.Bold = true;
                    monthCell.Style.Font.Size = 12;
                    monthCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    monthCell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#E7F3FF"));
                    monthCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    monthCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Row(currentRow).Height = 20;
                    currentRow++;

                    // Empty spacing row (Row 3)
                    currentRow++;

                    // 2. EMPLOYEE DATA SECTION - Each employee's summary followed by their detailed data
                    var employeeGroups = dataTable.AsEnumerable()
                        .GroupBy(row => new
                        {
                            EmpCode = row.Field<string>("EmpCode") ?? "",
                            EmpName = row.Field<string>("EmployeeName") ?? "",
                            Designation = row.Field<string>("Designation") ?? ""
                        })
                        .ToList();

                    // Create new table structure with Description + dynamic company columns + numbered day headers
                    int headerRow = currentRow;
                    int headerCol = 1;

                    // Description column header
                    worksheet.Cells[currentRow, headerCol].Value = "Description";
                    worksheet.Cells[currentRow, headerCol].Style.Font.Bold = true;
                    worksheet.Cells[currentRow, headerCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, headerCol].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    worksheet.Cells[currentRow, headerCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksheet.Cells[currentRow, headerCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[currentRow, headerCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Column(headerCol).Width = 30; // Wide column for employee names and summary
                    headerCol++;

                    // Add dynamic company columns based on data variation
                    foreach (var column in dynamicColumns.Where(c => c.show))
                    {
                        worksheet.Cells[currentRow, headerCol].Value = column.header;
                        worksheet.Cells[currentRow, headerCol].Style.Font.Bold = true;
                        worksheet.Cells[currentRow, headerCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[currentRow, headerCol].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        worksheet.Cells[currentRow, headerCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[currentRow, headerCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[currentRow, headerCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        headerCol++;
                    }

                    // Dynamic day headers based on actual columns in data (preserve backend order)
                    var actualDayColumns = dataTable.Columns.Cast<DataColumn>()
                        .Where(c => System.Text.RegularExpressions.Regex.IsMatch(c.ColumnName, @"^d\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                        .ToList();

                    foreach (var dayCol in actualDayColumns)
                    {
                        // Extract day number from column name (d1 -> 1, d2 -> 2, etc.)
                        string dayNumber = dayCol.ColumnName.Substring(1);
                        worksheet.Cells[currentRow, headerCol].Value = dayNumber;
                        worksheet.Cells[currentRow, headerCol].Style.Font.Bold = true;
                        worksheet.Cells[currentRow, headerCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[currentRow, headerCol].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5B9BD5"));
                        worksheet.Cells[currentRow, headerCol].Style.Font.Color.SetColor(Color.White);
                        worksheet.Cells[currentRow, headerCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[currentRow, headerCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[currentRow, headerCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        headerCol++;
                    }

                    // Add PresentDays header
                    worksheet.Cells[currentRow, headerCol].Value = "Present Days(P+WO+H+PL)";
                    worksheet.Cells[currentRow, headerCol].Style.Font.Bold = true;
                    worksheet.Cells[currentRow, headerCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, headerCol].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    worksheet.Cells[currentRow, headerCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksheet.Cells[currentRow, headerCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[currentRow, headerCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    headerCol++;

                    // Add OT Hours header
                    worksheet.Cells[currentRow, headerCol].Value = "OT Hours";
                    worksheet.Cells[currentRow, headerCol].Style.Font.Bold = true;
                    worksheet.Cells[currentRow, headerCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, headerCol].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    worksheet.Cells[currentRow, headerCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksheet.Cells[currentRow, headerCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[currentRow, headerCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    headerCol++;

                    // Add custom leave column headers
                    foreach (var leaveColumnName in customLeaveColumns)
                    {
                        worksheet.Cells[currentRow, headerCol].Value = leaveColumnName;
                        worksheet.Cells[currentRow, headerCol].Style.Font.Bold = true;
                        worksheet.Cells[currentRow, headerCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[currentRow, headerCol].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        worksheet.Cells[currentRow, headerCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[currentRow, headerCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[currentRow, headerCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        headerCol++;
                    }
                    
                    // Set header row height for better visibility
                    worksheet.Row(headerRow).Height = 25;

                    currentRow++;

                    // Now process each employee: Summary row followed by detailed row
                    foreach (var empGroup in employeeGroups)
                    {
                        var employee = empGroup.Key;
                        var empData = empGroup.ToList(); // Backend sends grouped data, no need to sort by date
                        var firstRecord = empData.FirstOrDefault();
                        if (firstRecord == null) continue;

                        // Calculate summary statistics using backend field names (all are strings)
                        string totalP = "";
                        string totalA = "";
                        string totalH = "";
                        string totalWO = "";
                        string totalPL = "";
                        string totalHD = "";
                        string totalLH = "";
                        string totalE = "";
                        string totalHours = "";
                        
                        // DEBUG: Log employee and available summary columns
                       
                        // Get string values directly from summary fields (no parsing needed)
                        totalP = firstRecord.Field<string>("P") ?? "0";
                        totalA = firstRecord.Field<string>("A") ?? "0";
                        totalH = firstRecord.Field<string>("H") ?? "0";
                        totalWO = firstRecord.Field<string>("Week Off") ?? "0";
                        totalE = firstRecord.Field<string>("Error_Case") ?? "0";
                        totalHD = firstRecord.Field<string>("Half_Day") ?? "0";
                        totalLH = firstRecord.Field<string>("Less_Hrs") ?? "0";
                        totalPL = firstRecord.Field<string>("Paid_Leave") ?? "0";
                        
                        // Handle TotalHrs parameter (comes in time format like "190:00:00")
                       
                        if (dataTable.Columns.Contains("TotalHrs"))
                        {
                            string totalHrsValue = firstRecord.Field<string>("TotalHrs");
                          
                            
                            // Handle undefined, null, or empty values
                            if (string.IsNullOrWhiteSpace(totalHrsValue) || 
                                totalHrsValue.Equals("undefined", StringComparison.OrdinalIgnoreCase) ||
                                totalHrsValue.Equals("null", StringComparison.OrdinalIgnoreCase))
                            {
                                totalHours = "0:00:00";
                                
                            }
                            else
                            {
                                // Display time format as-is (e.g., "190:00:00")
                                totalHours = totalHrsValue;
                               
                            }
                        }
                        else
                        {
                          
                            // Check for similar column names
                            var hourColumns = dataTable.Columns.Cast<DataColumn>()
                                .Where(c => c.ColumnName.ToLower().Contains("hour") || c.ColumnName.ToLower().Contains("hrs"))
                                .Select(c => c.ColumnName)
                                .ToList();
                         
                            totalHours = "0:00:00";
                        }

                      

                        // EMPLOYEE SUMMARY ROW - New format
                        int col = 1;
                        
                        // Column 1: Employee Name (EmpCode) in Description column - merge 2 rows vertically
                        string employeeDescription = $"{employee.EmpName} ({employee.EmpCode})";
                        var descriptionRange = worksheet.Cells[currentRow, col, currentRow + 1, col];
                        descriptionRange.Merge = true;
                        descriptionRange.Value = employeeDescription;
                        descriptionRange.Style.Font.Bold = true;
                        descriptionRange.Style.Font.Size = 11; // Larger font for employee names
                        descriptionRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        descriptionRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        descriptionRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        descriptionRange.Style.Indent = 1; // Add slight indentation
                        col++;
                        
                        // Get additional info for summary
                        string companyName = firstRecord.Field<string>("Company") ?? "";
                        string branchName = firstRecord.Field<string>("Branch") ?? "";
                        string departmentName = firstRecord.Field<string>("Department") ?? "";
                        string designation = employee.Designation ?? "";
                        
                        // Get additional summary values with comprehensive column name matching
                        string presentDays = "";
                        string otHours = "";
                        
                        // DEBUG: Log all available columns
                        var allColumns = dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
                        
                        
                        // Get string values directly from Present Days and OT Hours columns (no parsing needed)
                        presentDays = firstRecord.Field<string>("Present Days(P+WO+H+PL)") ?? "0";
                        otHours = firstRecord.Field<string>("OT Hours") ?? "0";

                       
                        
                        // Note: PresentDays, OTHours, and Custom Leaves will appear as separate columns only
                        
                        // Create merged cell spanning 2 rows vertically and from after fixed columns to last column
                        int actualDayColumnsCountForSummary = dataTable.Columns.Cast<DataColumn>()
                            .Count(c => System.Text.RegularExpressions.Regex.IsMatch(c.ColumnName, @"^d\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase));
                        int lastCol = fixedColumns + variableCompanyColumns + actualDayColumnsCountForSummary + summaryColumns;
                        var mergedRange = worksheet.Cells[currentRow, col, currentRow + 1, lastCol];
                        
                        // Merge the cells vertically (2 rows)
                        mergedRange.Merge = true;
                        
                        // Create simplified summary with only basic attendance counts
                        var attendanceSummary = new List<string>
                        {
                            $"P: {totalP}",
                            $"A: {totalA}",
                            $"WO: {totalWO}",
                            $"H: {totalH}",
                            $"E: {totalE}",
                            $"HD: {totalHD}",
                            $"LH: {totalLH}",
                            $"PL: {totalPL}",
                            $"TotalHrs: {totalHours}"
                        };
                        
                        // Create formatted summary with proper spacing
                        string comprehensiveSummary = string.Join("    ", attendanceSummary);
                        
                        // Set the value and styling for merged cell
                        mergedRange.Value = comprehensiveSummary;
                        mergedRange.Style.Font.Bold = false;
                        mergedRange.Style.Font.Size = 12;
                        mergedRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        mergedRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        mergedRange.Style.WrapText = true; // Enable text wrapping for line breaks
                        mergedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        mergedRange.Style.Indent = 1; // Add slight indentation
                        
                        // Set row heights for the 2 merged rows
                        worksheet.Row(currentRow).Height = 25;
                        worksheet.Row(currentRow + 1).Height = 25;

                        // Apply light background to entire summary rows (including Description column and merged summary)
                        var summaryRowRange = worksheet.Cells[currentRow, 1, currentRow + 1, lastCol];
                        summaryRowRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        summaryRowRange.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#F8F9FA"));

                        // Skip 2 rows since we merged vertically
                        currentRow += 2;

                        // EMPLOYEE DETAILED ROW - Daily attendance data only
                        int dataCol = 1;

                        // Column 1: Empty in detailed row (Description column)
                        worksheet.Cells[currentRow, dataCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        dataCol++;

                        // Add dynamic company data columns based on what's shown in headers
                        foreach (var column in dynamicColumns.Where(c => c.show))
                        {
                            string columnValue = "";
                            switch (column.field)
                            {
                                case "Company":
                                    columnValue = companyName;
                                    break;
                                case "Branch":
                                    columnValue = branchName;
                                    break;
                                case "Department":
                                    columnValue = departmentName;
                                    break;
                                case "Designation":
                                    columnValue = designation;
                                    break;
                            }

                            worksheet.Cells[currentRow, dataCol].Value = columnValue;
                            worksheet.Cells[currentRow, dataCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            // Ensure bottom border is visible
                            worksheet.Cells[currentRow, dataCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[currentRow, dataCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            worksheet.Cells[currentRow, dataCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            worksheet.Cells[currentRow, dataCol].Style.Font.Size = 10;
                            
                            // Apply header-style background color to company information columns
                            worksheet.Cells[currentRow, dataCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[currentRow, dataCol].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            
                            dataCol++;
                        }

                        // Get actual day columns from the data for this employee (preserve backend order)
                        var actualDayColumnsForData = dataTable.Columns.Cast<DataColumn>()
                            .Where(c => System.Text.RegularExpressions.Regex.IsMatch(c.ColumnName, @"^d\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                            .ToList();

                        // Daily attendance columns - use actual detected day columns
                        foreach (var dayCol in actualDayColumnsForData)
                        {
                            string status = firstRecord.Field<string>(dayCol.ColumnName) ?? "";
                            worksheet.Cells[currentRow, dataCol].Value = status;
                            worksheet.Cells[currentRow, dataCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            worksheet.Cells[currentRow, dataCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            worksheet.Cells[currentRow, dataCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            worksheet.Cells[currentRow, dataCol].Style.Font.Size = 10;
                            worksheet.Cells[currentRow, dataCol].Style.Font.Bold = true; // Make attendance status bold for better visibility

                            // Apply text colors based on status (no background colors)
                            Color textColor = Color.Black; // Default text color
                            
                            if (status == "HD")
                            {
                                textColor = System.Drawing.ColorTranslator.FromHtml("#ffc107");
                            }
                            else if (status == "E")
                            {
                                textColor = System.Drawing.ColorTranslator.FromHtml("#343a40");
                            }
                            else if (status == "P" || status == "PHW")
                            {
                                textColor = System.Drawing.ColorTranslator.FromHtml("#28a745");
                            }
                            else if (status == "H" || status == "PH" || status == "HW")
                            {
                                textColor = System.Drawing.ColorTranslator.FromHtml("#343a40");
                            }
                            else if (status == "A" || status == "AB" || status == "XX")
                            {
                                textColor = System.Drawing.ColorTranslator.FromHtml("#ff0000");
                            }
                            else if (status == "W" || status == "PW")
                            {
                                textColor = System.Drawing.ColorTranslator.FromHtml("#007bff");
                            }
                            else if (status == "LH")
                            {
                                textColor = System.Drawing.ColorTranslator.FromHtml("#fd7e14");
                            }
                            // Others = Black text (default)
                            
                            // Apply the text color
                            worksheet.Cells[currentRow, dataCol].Style.Font.Color.SetColor(textColor);

                            dataCol++;
                        }

                        // Add Present Days value
                        worksheet.Cells[currentRow, dataCol].Value = presentDays;
                        worksheet.Cells[currentRow, dataCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[currentRow, dataCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[currentRow, dataCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[currentRow, dataCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[currentRow, dataCol].Style.Font.Size = 10;
                        worksheet.Cells[currentRow, dataCol].Style.Font.Bold = true;
                        dataCol++;

                        // Add OT Hours value
                        worksheet.Cells[currentRow, dataCol].Value = otHours;
                        worksheet.Cells[currentRow, dataCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[currentRow, dataCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[currentRow, dataCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[currentRow, dataCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[currentRow, dataCol].Style.Font.Size = 10;
                        worksheet.Cells[currentRow, dataCol].Style.Font.Bold = true;
                        dataCol++;

                        // Add custom leave column data
                        foreach (var leaveColumnName in customLeaveColumns)
                        {
                            string leaveValue = firstRecord.Field<string>(leaveColumnName) ?? "0";
                            worksheet.Cells[currentRow, dataCol].Value = leaveValue;
                            worksheet.Cells[currentRow, dataCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            worksheet.Cells[currentRow, dataCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[currentRow, dataCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            worksheet.Cells[currentRow, dataCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            worksheet.Cells[currentRow, dataCol].Style.Font.Size = 10;
                            worksheet.Cells[currentRow, dataCol].Style.Font.Bold = true;
                            dataCol++;
                        }

                        // Set daily data row height for better visibility
                        worksheet.Row(currentRow).Height = 22;

                        currentRow++;
                    }

                    // Set column widths for better readability
                    // Description column width will be auto-fitted based on content
                    
                    // Set wider width for daily columns for better visibility  
                    int actualDayColumnsCount = dataTable.Columns.Cast<DataColumn>()
                        .Count(c => System.Text.RegularExpressions.Regex.IsMatch(c.ColumnName, @"^d\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase));
                    for (int i = fixedColumns + variableCompanyColumns + 1; i <= fixedColumns + variableCompanyColumns + actualDayColumnsCount; i++)
                    {
                        worksheet.Column(i).Width = 7; // Daily columns wider for better readability
                    }

                    // Set appropriate width for PresentDays and OTHours columns
                    int presentDaysColIndex = fixedColumns + variableCompanyColumns + actualDayColumnsCount + 1;
                    int otHoursColIndex = presentDaysColIndex + 1;
                    worksheet.Column(presentDaysColIndex).Width = 12; // Present Days column
                    worksheet.Column(otHoursColIndex).Width = 10; // OT Hours column

                    // Set freeze panes to freeze only Description column
                    worksheet.View.FreezePanes(headerRow + 1, 2); // Only freeze after Description column (column 1)

                    // Auto-fit all columns to their content
                    worksheet.Cells.AutoFitColumns();
                    
                    // Ensure Description column has minimum width for readability
                    if (worksheet.Column(1).Width < 25) 
                        worksheet.Column(1).Width = 25;

                    return package.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
              
                
                // Log specific information about the error context
                if (dataTable != null)
                {
                  
                    var columnNames = dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
                   
                }
                
                using (ExcelPackage emptyPackage = new ExcelPackage())
                {
                    ExcelWorksheet emptySheet = emptyPackage.Workbook.Worksheets.Add("Error");
                    emptySheet.Cells[1, 1].Value = $"Error generating Excel: {ex.Message}";
                    emptySheet.Cells[2, 1].Value = $"Error Type: {ex.GetType().Name}";
                    if (dataTable != null)
                    {
                        emptySheet.Cells[3, 1].Value = $"DataTable Rows: {dataTable.Rows.Count}";
                        emptySheet.Cells[4, 1].Value = $"DataTable Columns: {dataTable.Columns.Count}";
                        
                        if (dataTable.Columns.Count > 0)
                        {
                            var columnNames = dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
                            emptySheet.Cells[5, 1].Value = $"Available Columns: {string.Join(", ", columnNames)}";
                        }
                    }
                    return emptyPackage.GetAsByteArray();
                }
            }
        }
        private byte[] GenerateExcelContentReportDuraction(DataTable dataTable, string headername)
        {
            try
            {
                

                if (dataTable == null)
                {
                   
                    throw new ArgumentNullException(nameof(dataTable), "DataTable cannot be null for Excel generation");
                }

                if (dataTable.Rows.Count == 0)
                {
                
                    throw new ArgumentException("DataTable must contain data for Excel generation", nameof(dataTable));
                }
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Monthly Working Duration");
                    int currentRow = 1;

                    // Get month info from backend Month_Year field or current date
                    DateTime reportMonth = DateTime.Now;
                    string monthYearString = "";
                    if (dataTable.Rows.Count > 0 && dataTable.Columns.Contains("Month-Year"))
                    {
                        monthYearString = dataTable.Rows[0].Field<string>("Month-Year");
                        if (!string.IsNullOrEmpty(monthYearString))
                        {
                            if (DateTime.TryParseExact(
                                    monthYearString,
                                    "MMM-yyyy",
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    System.Globalization.DateTimeStyles.None,
                                    out DateTime parsedDate))
                            {
                                reportMonth = parsedDate;
                            }
                        }
                    }

                    // Get all columns except those that will be handled as fixed columns or excluded entirely
                    var dataColumns = dataTable.Columns.Cast<DataColumn>()
                        .Where(c => !c.ColumnName.Equals("Month_Year", StringComparison.OrdinalIgnoreCase) && 
                                    !c.ColumnName.Equals("Status", StringComparison.OrdinalIgnoreCase) &&
                                    !c.ColumnName.Equals("EmpCode", StringComparison.OrdinalIgnoreCase) &&
                                    !c.ColumnName.Equals("EmployeeName", StringComparison.OrdinalIgnoreCase) &&
                                    !c.ColumnName.Equals("EmpName", StringComparison.OrdinalIgnoreCase) &&
                                    !c.ColumnName.Equals("Company", StringComparison.OrdinalIgnoreCase) &&
                                    !c.ColumnName.Equals("Branch", StringComparison.OrdinalIgnoreCase) &&
                                    !c.ColumnName.Equals("Department", StringComparison.OrdinalIgnoreCase))
                        .Select(c => c.ColumnName)
                        .ToList();

                    // Group data by employee
                    var employeeGroups = dataTable.AsEnumerable()
                        .GroupBy(row => new
                        {
                            EmpCode = row.Field<string>("EmpCode") ?? "",
                            EmpName = row.Field<string>("EmployeeName") ?? row.Field<string>("EmpName") ?? ""
                        })
                        .ToList();

                    // Calculate total columns for the report
                    int totalColumns = dataColumns.Count + 4; // +4 for fixed columns (Status, Company, Branch, Department)

                    // 1. HEADER SECTION
                    // Title row (Row 1)
                    var titleCell = worksheet.Cells[currentRow, 1, currentRow, Math.Max(totalColumns, 10)];
                    titleCell.Merge = true;
                    titleCell.Value = "Monthly Working Duration";
                    titleCell.Style.Font.Bold = true;
                    titleCell.Style.Font.Size = 14;
                    titleCell.Style.Font.Color.SetColor(Color.White);
                    titleCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    titleCell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#295097"));
                    titleCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    titleCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Row(currentRow).Height = 25;
                    currentRow++;

                    // Month/Year row (Row 2)
                    var monthCell = worksheet.Cells[currentRow, 1, currentRow, Math.Max(totalColumns, 10)];
                    monthCell.Merge = true;
                    monthCell.Value = reportMonth.ToString("MMMM yyyy");
                    monthCell.Style.Font.Bold = true;
                    monthCell.Style.Font.Size = 12;
                    monthCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    monthCell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#E7F3FF"));
                    monthCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    monthCell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Row(currentRow).Height = 20;
                    currentRow++;

                    // Empty spacing row (Row 3)
                    currentRow++;

                    // 2. TABLE HEADERS
                    int headerRow = currentRow;
                    int headerCol = 1;

                    // Fixed column headers (Status, Company, Branch, Department)
                    var fixedColumnHeaders = new[] { "Status", "Company", "Branch", "Department" };
                    foreach (var fixedHeader in fixedColumnHeaders)
                    {
                        worksheet.Cells[currentRow, headerCol].Value = fixedHeader;
                        worksheet.Cells[currentRow, headerCol].Style.Font.Bold = true;
                        worksheet.Cells[currentRow, headerCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[currentRow, headerCol].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        worksheet.Cells[currentRow, headerCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[currentRow, headerCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[currentRow, headerCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Column(headerCol).Width = fixedHeader == "Status" ? 30 : 15; // Status wider, others standard
                        headerCol++;
                    }

                    // Data column headers (all columns except Month_Year, Status, EmpCode, EmployeeName)
                    foreach (var columnName in dataColumns)
                    {
                        worksheet.Cells[currentRow, headerCol].Value = columnName;
                        worksheet.Cells[currentRow, headerCol].Style.Font.Bold = true;
                        worksheet.Cells[currentRow, headerCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        
                        // Check if this is a day column (d1, d2, d3...d31) and apply blue color
                        if (System.Text.RegularExpressions.Regex.IsMatch(columnName, @"^d\d+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                        {
                            worksheet.Cells[currentRow, headerCol].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5B9BD5"));
                            worksheet.Cells[currentRow, headerCol].Style.Font.Color.SetColor(Color.White); // White text for better contrast
                        }
                        else
                        {
                            worksheet.Cells[currentRow, headerCol].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        }
                        
                        worksheet.Cells[currentRow, headerCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[currentRow, headerCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[currentRow, headerCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        headerCol++;
                    }

                    worksheet.Row(headerRow).Height = 25;
                    currentRow++;

                    // 3. DATA ROWS - Each employee and their data rows
                    foreach (var empGroup in employeeGroups)
                    {
                        var employee = empGroup.Key;
                        var empData = empGroup.ToList();

                        // Extract OT Hours and Total Hours from employee data using Status column method
                        string otHours = "0:00:00";
                        string totalHours = "0:00:00";
                        
                        // Find row where Status == "OTHr" and get its Total value
                        var otHourRow = empData.FirstOrDefault(row => row.Field<string>("Status") == "OTHr");
                        if (otHourRow != null && otHourRow.Table.Columns.Contains("Total"))
                        {
                            var otValue = otHourRow.Field<string>("Total");
                            if (!string.IsNullOrEmpty(otValue))
                            {
                                otHours = otValue;
                            }
                        }
                        
                        // Find row where Status == "Tot Hour" or "TotHour" and get its Total value
                        var totalHourRow = empData.FirstOrDefault(row => 
                            row.Field<string>("Status") == "Tot Hour" || 
                            row.Field<string>("Status") == "TotHour");
                        if (totalHourRow != null && totalHourRow.Table.Columns.Contains("Total"))
                        {
                            var totalValue = totalHourRow.Field<string>("Total");
                            if (!string.IsNullOrEmpty(totalValue))
                            {
                                totalHours = totalValue;
                            }
                        }

                        // Employee name row
                        worksheet.Cells[currentRow, 1].Value = $"{employee.EmpName} ({employee.EmpCode})";
                        worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                        worksheet.Cells[currentRow, 1].Style.Font.Size = 11;
                        worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        worksheet.Cells[currentRow, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[currentRow, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[currentRow, 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#F8F9FA"));

                        // Merge remaining columns and add OT Hours and Total Hours summary with color coding
                        var summaryRange = worksheet.Cells[currentRow, 2, currentRow, totalColumns];
                        summaryRange.Merge = true;
                        
                        // Create rich text with colored segments
                        var richText = summaryRange.RichText;
                        richText.Clear();
                        
                        // Add "OT Hrs: " in black
                        var otLabel = richText.Add("OT Hrs: ");
                        otLabel.Bold = false;
                        otLabel.Size = 10;
                        otLabel.Color = Color.Black;
                        
                        // Add OT Hours value in purple
                        var otRichText = richText.Add(otHours);
                        otRichText.Bold = true;
                        otRichText.Size = 10;
                        otRichText.Color = ColorTranslator.FromHtml("#6f42c1");
                        
                        // Add separator in black
                        var separator = richText.Add("    Total Hrs: ");
                        separator.Bold = false;
                        separator.Size = 10;
                        separator.Color = Color.Black;
                        
                        // Add Total Hours value in blue
                        var totalRichText = richText.Add(totalHours);
                        totalRichText.Bold = true;
                        totalRichText.Size = 10;
                        totalRichText.Color = ColorTranslator.FromHtml("#007bff");
                        
                        summaryRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        summaryRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        summaryRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        summaryRange.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#F8F9FA"));
                        summaryRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        summaryRange.Style.Indent = 1;
                        
                        currentRow++;

                        // Data rows for this employee (7 rows as mentioned)
                        foreach (var dataRow in empData)
                        {
                            int dataCol = 1;

                            // Fixed columns data (Status, Company, Branch, Department)
                            var fixedColumnValues = new[]
                            {
                                dataRow.Field<string>("Status") ?? "",
                                dataRow.Field<string>("Company") ?? "",
                                dataRow.Field<string>("Branch") ?? "",
                                dataRow.Field<string>("Department") ?? ""
                            };
                            
                            foreach (var fixedValue in fixedColumnValues)
                            {
                                worksheet.Cells[currentRow, dataCol].Value = fixedValue;
                                worksheet.Cells[currentRow, dataCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                worksheet.Cells[currentRow, dataCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                worksheet.Cells[currentRow, dataCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                worksheet.Cells[currentRow, dataCol].Style.Font.Size = 10;
                                worksheet.Cells[currentRow, dataCol].Style.Indent = 1; // Add slight indentation
                                dataCol++;
                            }

                            // Fill data columns
                            foreach (var columnName in dataColumns)
                            {
                                var cellValue = dataRow.Field<string>(columnName) ?? "";
                                worksheet.Cells[currentRow, dataCol].Value = cellValue;
                                worksheet.Cells[currentRow, dataCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                worksheet.Cells[currentRow, dataCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                worksheet.Cells[currentRow, dataCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                worksheet.Cells[currentRow, dataCol].Style.Font.Size = 10;

                                // Apply color coding based on cell content (from Muster report)
                                Color textColor = Color.Black;
                                if (cellValue == "P")
                                {
                                    textColor = ColorTranslator.FromHtml("#28a745");
                                }
                                else if (cellValue == "A" || cellValue == "AB" || cellValue == "XX")
                                {
                                    textColor = ColorTranslator.FromHtml("#ff0000");
                                }
                                else if (cellValue == "H" || cellValue == "PH" || cellValue == "PHW")
                                {
                                    textColor = ColorTranslator.FromHtml("#343a40");
                                }
                                else if (cellValue == "W" || cellValue == "PW" || cellValue == "HW")
                                {
                                    textColor = ColorTranslator.FromHtml("#007bff");
                                }
                                else if (cellValue == "HD")
                                {
                                    textColor = ColorTranslator.FromHtml("#ffc107");
                                }
                                else if (cellValue == "LH")
                                {
                                    textColor = ColorTranslator.FromHtml("#fd7e14");
                                }
                                else if (cellValue == "E")
                                {
                                    textColor = ColorTranslator.FromHtml("#343a40");
                                }

                                worksheet.Cells[currentRow, dataCol].Style.Font.Color.SetColor(textColor);
                                dataCol++;
                            }

                            worksheet.Row(currentRow).Height = 22;
                            currentRow++;
                        }
                        
                        // Add spacing after each employee (skip 1 row)
                        currentRow++;
                    }

                    // Freeze the fixed columns (Status, Company, Branch, Department) for horizontal scrolling
                    worksheet.View.FreezePanes(headerRow + 1, 5); // Freeze first 4 columns

                    // Auto-fit all columns
                    worksheet.Cells.AutoFitColumns();

                    // Ensure minimum width for Status column
                    if (worksheet.Column(1).Width < 25)
                        worksheet.Column(1).Width = 25;

                    return package.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
               

                using (ExcelPackage emptyPackage = new ExcelPackage())
                {
                    ExcelWorksheet emptySheet = emptyPackage.Workbook.Worksheets.Add("Error");
                    emptySheet.Cells[1, 1].Value = $"Error generating Excel: {ex.Message}";
                    emptySheet.Cells[2, 1].Value = $"Error Type: {ex.GetType().Name}";
                    if (dataTable != null)
                    {
                        emptySheet.Cells[3, 1].Value = $"DataTable Rows: {dataTable.Rows.Count}";
                        emptySheet.Cells[4, 1].Value = $"DataTable Columns: {dataTable.Columns.Count}";
                        
                        if (dataTable.Columns.Count > 0)
                        {
                            var columnNames = dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
                            emptySheet.Cells[5, 1].Value = $"Available Columns: {string.Join(", ", columnNames)}";
                        }
                    }
                    return emptyPackage.GetAsByteArray();
                }
            }
        }

        private byte[] GenerateExcelContentVertical(DataTable dataTable)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Attendance Details");
                int currentRow = 1;

                var titleRange = worksheet.Cells[currentRow, 1, currentRow, 7];
                titleRange.Merge = true;
                titleRange.Value = "Monthly Attendance Report";
                titleRange.Style.Font.Bold = true;
                titleRange.Style.Font.Size = 16;
                titleRange.Style.Font.Color.SetColor(Color.White);
                titleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                titleRange.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#2F5597"));
                titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                titleRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Row(currentRow).Height = 35;
                currentRow += 2;

                var empGroups = dataTable.AsEnumerable()
                    .GroupBy(r => new {
                        EmpId = r["EmpId"],
                        EmpCode = r["EmpCode"],
                        EmpName = r["EmpName"],
                        Designation = r.Table.Columns.Contains("DesignationName") ? r["DesignationName"] : "",
                        Department = r.Table.Columns.Contains("DepartmentName") ? r["DepartmentName"] : "",
                        Company = r.Table.Columns.Contains("CompanyName") ? r["CompanyName"] : ""
                    })
                    .ToList();

                foreach (var group in empGroups)
                {
                    var emp = group.Key;
                    var logs = group.ToList();

                    var firstRow = logs.FirstOrDefault();

                    double GetVal(DataRow row, string colName)
                    {
                        if (row != null && row.Table.Columns.Contains(colName) && row[colName] != DBNull.Value)
                            return Convert.ToDouble(row[colName]);
                        return 0;
                    }

                    double present = GetVal(firstRow, "Present");
                    double absent = GetVal(firstRow, "Absent");
                    double weekOff = GetVal(firstRow, "WeekOff");
                    double holidays = GetVal(firstRow, "Holiday");
                    double paidLeave = GetVal(firstRow, "PaidLeave");

                    double payDays = present + weekOff + holidays + paidLeave;

                    TimeSpan totalHrs = TimeSpan.Zero;
                    TimeSpan otHrs = TimeSpan.Zero;
                    foreach (var log in logs)
                    {
                        if (log.Table.Columns.Contains("TotHour") && TimeSpan.TryParse(log["TotHour"].ToString(), out TimeSpan t)) totalHrs += t;
                        if (log.Table.Columns.Contains("OTHr") && TimeSpan.TryParse(log["OTHr"].ToString(), out TimeSpan ot)) otHrs += ot;
                    }
                    string totalHrsStr = $"{(int)totalHrs.TotalHours}:{totalHrs.Minutes:D2}";
                    string otHrsStr = $"{(int)otHrs.TotalHours}:{otHrs.Minutes:D2}";

                    var empHeaderRange = worksheet.Cells[currentRow, 1, currentRow, 7];
                    empHeaderRange.Merge = true;
                    empHeaderRange.Value = $"{emp.EmpName} ({emp.EmpCode}) | {emp.Designation} | COM: {emp.Company} | DEP: {emp.Department} \r\n" +
                                           $"Pay Days: {payDays} | P: {present} | A: {absent} | WO: {weekOff} | H: {holidays} | L: {paidLeave} | Work Hrs: {totalHrsStr} | OT: {otHrsStr}";

                    empHeaderRange.Style.Font.Bold = true;
                    empHeaderRange.Style.Font.Size = 12;
                    empHeaderRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    empHeaderRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    empHeaderRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    empHeaderRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    empHeaderRange.Style.WrapText = true;
                    worksheet.Row(currentRow).Height = 50;
                    currentRow++;

                    string[] headers = { "Date", "Shift", "In Time", "Out Time", "Total Hrs", "OT Hrs", "Status" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[currentRow, i + 1].Value = headers[i];
                        worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[currentRow, i + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[currentRow, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    worksheet.Row(currentRow).Height = 20;
                    currentRow++;

                    foreach (var row in logs)
                    {
                        worksheet.Cells[currentRow, 1].Value = row.Table.Columns.Contains("Attn_Dt") ? row["Attn_Dt"] : "";
                        worksheet.Cells[currentRow, 2].Value = row.Table.Columns.Contains("ShiftName") ? row["ShiftName"] : "";
                        worksheet.Cells[currentRow, 3].Value = row.Table.Columns.Contains("FirstIn") ? row["FirstIn"] : "";
                        worksheet.Cells[currentRow, 4].Value = row.Table.Columns.Contains("LastOut") ? row["LastOut"] : "";
                        worksheet.Cells[currentRow, 5].Value = row.Table.Columns.Contains("TotHour") ? row["TotHour"] : "";
                        worksheet.Cells[currentRow, 6].Value = row.Table.Columns.Contains("OTHr") ? row["OTHr"] : "";

                        var statusCell = worksheet.Cells[currentRow, 7];
                        string status = row.Table.Columns.Contains("FinalStatus") ? row["FinalStatus"].ToString().Trim() : "";
                        statusCell.Value = status;
                        statusCell.Style.Font.Bold = true;
                        statusCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        if (status == "A") statusCell.Style.Font.Color.SetColor(Color.Red);
                        else if (status == "P") statusCell.Style.Font.Color.SetColor(Color.Green);
                        else if (status == "WO") statusCell.Style.Font.Color.SetColor(Color.Blue);
                        else if (status.Contains("L") || status.Contains("HD")) statusCell.Style.Font.Color.SetColor(Color.Orange);
                        else if (status.Contains("H")) statusCell.Style.Font.Color.SetColor(Color.Purple);

                        worksheet.Cells[currentRow, 1, currentRow, 7].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                        currentRow++;
                    }
                    currentRow++;
                }

                worksheet.Cells.AutoFitColumns();
                if (worksheet.Column(1).Width < 12) worksheet.Column(1).Width = 12;
                if (worksheet.Column(2).Width < 15) worksheet.Column(2).Width = 15;
                if (worksheet.Column(7).Width < 10) worksheet.Column(7).Width = 10;

                return package.GetAsByteArray();
            }
        }

        public class ExportResult
        {
            public bool success { get; set; }
            public string filePath { get; set; }
            public string message { get; set; }
        }

        #region For Developer Account Large Data Excel File Download Code 
        //----Like 65k Above
        public ExportResult SaveExcelToServerLargeFile(DataTable dataTable, string sheetName, string filePrefix)
        {
            try
            {
                string filePath = HttpContext.Current.Server.MapPath("~/ExportsExcelData/");
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                var appRootPath = HttpContext.Current.Server.MapPath("~/");
                var folderName = "ExportsExcelData";
                var fileDirectory = Path.Combine(appRootPath, folderName);

                var existingFiles = Directory.GetFiles(fileDirectory, filePrefix + "*");
                foreach (var file in existingFiles)
                    System.IO.File.Delete(file);

                string fileName = filePrefix + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                string fullPath = Path.Combine(filePath, fileName);

                using (var workbook = new ClosedXML.Excel.XLWorkbook())
                {
                    int rowLimit = 65000;
                    int totalRows = dataTable.Rows.Count;
                    int sheetCount = (int)Math.Ceiling((double)totalRows / rowLimit);

                    for (int i = 0; i < sheetCount; i++)
                    {
                        var sheetRows = dataTable.AsEnumerable()
                                                 .Skip(i * rowLimit)
                                                 .Take(rowLimit)
                                                 .CopyToDataTable();

                        string currentSheetName = $"Sheet{i + 1}";
                        var worksheet = workbook.Worksheets.Add(sheetRows, currentSheetName);
                    }
                    workbook.SaveAs(fullPath);
                }
                var downloadUrl = "/ExportsExcelData/" + fileName;
                return new ExportResult { success = true, filePath = downloadUrl };
            }
            catch (Exception ex)
            {
                return new ExportResult
                {
                    success = false,
                    message = ex.Message
                };
            }
        }
        #endregion

        #region For pdf serverside 
        private byte[] GeneratePdfContent(DataTable dataTable, string headerName)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A2.Rotate(), 10f, 10f, 20f, 20f);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.WHITE);
                PdfPCell titleCell = new PdfPCell(new Phrase(headerName, titleFont))
                {
                    Colspan = dataTable.Columns.Count,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BackgroundColor = new BaseColor(47, 117, 181),
                    Padding = 8f
                };

                PdfPTable table = new PdfPTable(dataTable.Columns.Count);
                table.WidthPercentage = 100;
                table.AddCell(titleCell);

                // Header Row
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                foreach (DataColumn col in dataTable.Columns)
                {
                    PdfPCell header = new PdfPCell(new Phrase(col.ColumnName, headerFont))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        NoWrap = true, // prevent text wrap
                        MinimumHeight = 20f // optional: to align nicely
                    };
                    table.AddCell(header);

                }

                // Data Rows
                var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                foreach (DataRow row in dataTable.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        table.AddCell(new Phrase(item?.ToString() ?? "", dataFont));
                    }
                }

                pdfDoc.Add(table);
                pdfDoc.Close();

                return stream.ToArray();
            }
        }
        public ExportResult SavePdfToServer(DataTable dataTable, string filePrefix, string deleteFilePattern)
        {
            try
            {
                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return new ExportResult { success = false, message = "No data available for export." };
                }

                var byteArray = GeneratePdfContent(dataTable, filePrefix);
                var appRootPath = HttpContext.Current.Server.MapPath("~/");
                var folderName = "ExportsExcelData"; // keep same folder
                var fileDirectory = Path.Combine(appRootPath, folderName);

                if (!Directory.Exists(fileDirectory))
                    Directory.CreateDirectory(fileDirectory);

                var existingFiles = Directory.GetFiles(fileDirectory, deleteFilePattern + "*");
                foreach (var file in existingFiles)
                    System.IO.File.Delete(file);

                var fileName = $"{filePrefix}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                var filePath = Path.Combine(fileDirectory, fileName);
                System.IO.File.WriteAllBytes(filePath, byteArray);

                var downloadUrl = "/ExportsExcelData/" + fileName;

                return new ExportResult { success = true, filePath = downloadUrl };
            }
            catch (Exception ex)
            {
                return new ExportResult { success = false, message = ex.Message };
            }
        }


       
        #endregion
    }
}