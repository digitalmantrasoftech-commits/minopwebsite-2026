// Common Button search code 
$(document).on('click', '#btnsearch', function () {
    const $filter = $('.dataTables_filter');
    const isActive = $filter.hasClass('active');
    const $input = $filter.find('input[type="search"]');
    $filter.toggleClass('active');

    // If the filter was active and is now being hidden, clear the input
    if (isActive) {
        $filter.find('input[type="search"]').val('').trigger('input'); // 'input' triggers DataTables filtering reset
        $('.dataTable').each(function () {
            $(this).DataTable().search('').draw();
        });
        $filter.find('input[type="search"]').prop('disabled', true);
    } else {
        $filter.find('input[type="search"]').prop('disabled', false).focus();
    }
});
// Column Button search code
//$(document).on('click', '#btnsearch1', function () {
//    $('.serch_datatable thead tr:nth-child(2)').toggle();
//});

$(document).on('click', '.columnSearch_wrapper label', function (e) {
    const $filterRow = $('.serch_datatable thead tr:nth-child(2)');
    const isVisible = $filterRow.is(':visible');

    $filterRow.toggle(); // show/hide the filter row

    if (!isVisible) {
        // Delay a little to allow DOM to render, then focus the first input
        setTimeout(() => {
            $filterRow.find('input:visible').first().focus();
        }, 100);
    }

    // Prevent click from focusing the label's input if not desired
    e.preventDefault();
});

// Common Dom code
var _searchboxCommon = '<"form-group has-search"f>';
var _languageCommon = {
    "info": "Showing <span class='active'>_START_</span> to <span>_END_</span> <span class='dataTables_info_total'>of _TOTAL_</span>",
    "infoFiltered": "",
    "infoEmpty": "",
    "search": "<img src='../Newlayout/images/icon-search.svg' id='btnsearch' class='tooltips' data-placement='bottom' title='Search'  alt='search' width='20' height='20'>",
    "lengthMenu": "_MENU_",
    "paginate": {
        "next": "<img src='../Newlayout/images/icon-next.svg' width='12' height='12' />",
        "previous": "<img src='../Newlayout/images/icon-prev.svg' width='12' height='12' />"
    },
};
var _domCommon = `<"#dataTables_tbl_header.custom_dataTableHeader"
                    <"#element_dataTables_tbl_header.d-flex justify-content-end align-items-center"
                        <"#last_section.align-items-center d-flex"
                        ${_searchboxCommon}
                        <"#buttons.d-flex align-item-between"B>
                        <"#dt_length.custom_table_length"l>
                        <"#dt_info"i>
                        <"#dt_paginate.dt_paginate"p>
                        >
                        >
                        >t`;

$(document).ready(function () {
    const visibleTable = $('table:visible').attr('id');
    $("#buttons").append(`
        <div id="export-button" class="export-button tooltips" data-placement="bottom" data-original-title="Download" data-table-id="${visibleTable}">
            <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
        </div>
    `);
});


// Column Dom code
var searchboxColumn = '<"form-group has-search columnSearch_wrapper"f>';
var _languageColumn = {
    "info": "Showing <span class='active'>_START_</span> to <span>_END_</span> <span class='dataTables_info_total'>of _TOTAL_</span>",
    "infoFiltered": "",
    "infoEmpty": "",
    "search": "<img src='../Newlayout/images/icon-search.svg' id='btnsearch1' class='tooltips' data-placement='bottom' data-original-title='Search' alt='search' width='20' height='20'>",
    "lengthMenu": "_MENU_",
    "paginate": {
        "next": "<img src='../Newlayout/images/icon-next.svg' width='12' height='12' />",
        "previous": "<img src='../Newlayout/images/icon-prev.svg' width='12' height='12' />"
    },
};

var _domColumn = `<"#dataTables_tbl_header.custom_dataTableHeader"
                        <"#element_dataTables_tbl_header.d-flex justify-content-end align-items-center"
                            <"#last_section.align-items-center d-flex"
                            ${searchboxColumn}
                            <"#buttons.d-flex align-item-between"B> 
                            <"#dt_length.custom_table_length"l>
                            <"#dt_info"i>
                            <"#dt_paginate.dt_paginate"p>
                            >
                            >
                            >t`;

function injectExportButtonsForDataTables(isServerSide, onClickCallback) {

    if ($('#buttons').length && $('#buttons').children().length === 0) {
        const exportBtnHtml = `
            <div id="export-button" class="export-button tooltips" data-placement="bottom" data-original-title="Download">
                <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
            </div>         
        `;

        $('#buttons').append(exportBtnHtml);

        // Tooltip init
        if ($.fn.tooltip) {
            $('[data-toggle="tooltip"]').tooltip();
        }

        // Optional: Add click events
        $('#export-button').on('click', function () {
            if (typeof onClickCallback === 'function') {
                onClickCallback();
            } else {
                toastr.info("Default export action...");
            }
        });
    }
}
function getFileSafeDateTime() {
    var now = new Date();
    var year = now.getFullYear();
    var month = String(now.getMonth() + 1).padStart(2, '0');
    var day = String(now.getDate()).padStart(2, '0');
    var hours = String(now.getHours()).padStart(2, '0');
    var minutes = String(now.getMinutes()).padStart(2, '0');
    var seconds = String(now.getSeconds()).padStart(2, '0');

    return `${year}${month}${day}`;
}
function getFormattedDateTime() {
    var now = new Date();
    var year = now.getFullYear();
    var month = String(now.getMonth() + 1).padStart(2, '0');
    var day = String(now.getDate()).padStart(2, '0');
    var hours = String(now.getHours()).padStart(2, '0');
    var minutes = String(now.getMinutes()).padStart(2, '0');
    var seconds = String(now.getSeconds()).padStart(2, '0');
    return `${year}/${month}/${day} - ${hours}:${minutes}:${seconds}`;
}
//async function exportTableToExcelFile(fileName, reportTitle) {

//    const MAX_ROWS_PER_SHEET = 65000;
//    const workbook = new ExcelJS.Workbook();

//    const tableId = $('.new_datatbl:visible').attr('id');
//    const dataTable = $(`#${tableId}`).DataTable();
//    const visibleColumnIndexes = dataTable.columns(':visible').indexes().toArray();

//    // Get visible headers
//    const headers = [];
//    $(`#${tableId} thead th`).each(function () {

//        const colIndex = parseInt($(this).attr("data-column-index"));
//        if (visibleColumnIndexes.includes(colIndex)) {

//            headers.push($(this).text().trim());
//        }
//    });

//    // Get all row indexes
//    const allRowIndexes = dataTable.rows({ page: 'all' }).indexes().toArray();

//    let sheetCount = 1;
//    let currentWorksheet = null;
//    let rowCounter = 0;

//    function createNewSheet(sheetIndex) {
//        const ws = workbook.addWorksheet(`Report_${sheetIndex}`);

//        // Report title
//        const titleRow = ws.addRow([reportTitle]);
//        ws.mergeCells(1, 1, 1, headers.length);
//        titleRow.font = { bold: true, size: 12, color: { argb: 'FFFFFFFF' } };
//        titleRow.alignment = { horizontal: 'center' };
//        ws.getCell(1, 1).fill = {
//            type: 'pattern',
//            pattern: 'solid',
//            fgColor: { argb: '295097' } // Navy blue
//        };

//        // Header row
//        const headerRow = ws.addRow(headers);
//        headerRow.font = { bold: true, color: { argb: 'FF000000' } };
//        headerRow.eachCell(cell => {
//            cell.fill = {
//                type: 'pattern',
//                pattern: 'solid',
//                fgColor: { argb: 'e3e3e3' }
//            };
//        });

//        return ws;
//    }

//    // Start with first worksheet
//    currentWorksheet = createNewSheet(sheetCount);
//    rowCounter = 2; // Title + header row

//    for (let i = 0; i < allRowIndexes.length; i++) {
//        if (rowCounter >= MAX_ROWS_PER_SHEET) {
//            sheetCount++;
//            currentWorksheet = createNewSheet(sheetCount);
//            rowCounter = 2;
//        }

//        const rowIndex = allRowIndexes[i];
//        const cleanRow = [];

//        visibleColumnIndexes.forEach(colIndex => {
//            const cellHtml = dataTable.cell(rowIndex, colIndex).render('display');
//            const tempDiv = document.createElement("div");
//            tempDiv.innerHTML = cellHtml !== undefined ? cellHtml : '';
//            cleanRow.push(tempDiv.textContent.trim());
//        });

//        currentWorksheet.addRow(cleanRow);
//        rowCounter++;
//    }

//    // Auto-adjust column widths
//    workbook.eachSheet(sheet => {
//        sheet.columns.forEach((column, colIndex) => {
//            let maxLength = headers[colIndex]?.length || 10;
//            for (let i = 2; i <= sheet.rowCount; i++) {
//                const cell = sheet.getRow(i).getCell(colIndex + 1);
//                let cellValue = cell.value;

//                if (cellValue && typeof cellValue === 'object') {
//                    cellValue = cellValue.richText?.map(t => t.text).join('') || cellValue.text || '';
//                }

//                const stringValue = cellValue ? cellValue.toString().trim() : '';
//                maxLength = Math.max(maxLength, stringValue.length);
//            }
//            column.width = Math.min(maxLength + 2, 50);
//        });
//    });

//    // Export to Excel
//    const buffer = await workbook.xlsx.writeBuffer();
//    const blob = new Blob([buffer], {
//        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
//    });

//    const link = document.createElement("a");
//    link.href = URL.createObjectURL(blob);
//    link.download = `${fileName}.xlsx`;
//    document.body.appendChild(link);
//    link.click();
//    document.body.removeChild(link);
//}

async function exportTableToExcelFile(fileName, reportTitle) {
    const MAX_ROWS_PER_SHEET = 65000;
    const workbook = new ExcelJS.Workbook();

    const tableId = $('.new_datatbl:visible').attr('id');
    const dataTable = $(`#${tableId}`).DataTable();
    const visibleColumnIndexes = dataTable.columns(':visible').indexes().toArray();

    // Get visible headers
    const headers = [];
    $(`#${tableId} thead th`).each(function () {
        const colIndex = parseInt($(this).attr("data-column-index"));
        if (visibleColumnIndexes.includes(colIndex)) {
            headers.push($(this).text().trim());
        }
    });

    // Get all row indexes
    const allRowIndexes = dataTable.rows({ page: 'all' }).indexes().toArray();

    // Preprocess all rows only once
    const allDataRows = allRowIndexes.map(rowIndex => {
        return visibleColumnIndexes.map(colIndex => {
            const cellHtml = dataTable.cell(rowIndex, colIndex).render('display');
            const tempDiv = document.createElement("div");
            tempDiv.innerHTML = cellHtml !== undefined ? cellHtml : '';
            return tempDiv.textContent.trim();
        });
    });

    // Split data into chunks of MAX_ROWS_PER_SHEET
    const totalSheets = Math.ceil(allDataRows.length / MAX_ROWS_PER_SHEET);

    for (let sheetIndex = 0; sheetIndex < totalSheets; sheetIndex++) {
        const ws = workbook.addWorksheet(`Report_${sheetIndex + 1}`);

        // Title Row
        const titleRow = ws.addRow([reportTitle]);
        ws.mergeCells(1, 1, 1, headers.length);
        titleRow.font = { bold: true, size: 12, color: { argb: 'FFFFFFFF' } };
        titleRow.alignment = { horizontal: 'center' };
        ws.getCell(1, 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: '295097' }
        };

        // Header Row
        const headerRow = ws.addRow(headers);
        headerRow.font = { bold: true, color: { argb: 'FF000000' } };
        headerRow.eachCell(cell => {
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'e3e3e3' }
            };
        });

        // Data Rows
        const chunk = allDataRows.slice(sheetIndex * MAX_ROWS_PER_SHEET, (sheetIndex + 1) * MAX_ROWS_PER_SHEET);
        chunk.forEach(row => ws.addRow(row));
    }

    // Auto-adjust column widths
    workbook.eachSheet(sheet => {
        sheet.columns.forEach((column, colIndex) => {
            let maxLength = headers[colIndex]?.length || 10;
            for (let i = 2; i <= sheet.rowCount; i++) {
                const cell = sheet.getRow(i).getCell(colIndex + 1);
                let cellValue = cell.value;

                if (cellValue && typeof cellValue === 'object') {
                    cellValue = cellValue.richText?.map(t => t.text).join('') || cellValue.text || '';
                }

                const stringValue = cellValue ? cellValue.toString().trim() : '';
                maxLength = Math.max(maxLength, stringValue.length);
            }
            column.width = Math.min(maxLength + 2, 50);
        });
    });

    // Download Excel file
    const buffer = await workbook.xlsx.writeBuffer();
    const blob = new Blob([buffer], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    });

    const link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.download = `${fileName}.xlsx`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}


const dict = [
    
    { Id: "tbluserDetails", filename: "Leave Type Master" },
    { Id: "tblShiftGroup", filename: "Shift Group" },
    { Id: "tblOnDutyLeave", filename: "OnDuty/Leave Entry" },
    { Id: "tblLeaveEncashCarry", filename: "Leave CarryForward" },
    { Id: "tblrole", filename: "Role Master" },
    { Id: "tblManualPunch", filename: "Manual Punching" },
    { Id: "tblCompanyMaster", filename: "Import Employees company" },
    { Id: "tblBranchMaster", filename: "Import Employees Branch" },
    { Id: "tblDepartmentMaster", filename: "Import Employees Department" },
    { Id: "tblDesignationMaster", filename: "Import Employees Designation" },
    { Id: "tblEmployeeMaster", filename: "Import Employees Employee" },
    { Id: "tblpolicy", filename: "HR Policy" },
    { Id: "tblEmailConfiguration", filename: "Email Smtp Configuration" },
    { Id: "tblScheduler", filename: "Send Mail" },
    { Id: "tbldevice", filename: "Devices" },
    { Id: "tbldevice", filename: "Device Command" },
    { Id: "tblRoom", filename: "Room Master" },
    { Id: "tblClient", filename: "Client Master" },
    { Id: "tblAmenity", filename: "Amenity Master" },
    { Id: "exitclearance_tbl", filename: "Exit Employee Request" },
    { Id: "tblleaveapproval", filename: "Leave Approve" },
    { Id: "tblAttendanceApproval", filename: "Attendance Approve" },
    { Id: "tblWebpunchApproval", filename: "WebPunch Approve" },
    { Id: "tblFacePunchApproval", filename: "FacePunch Approve" },
    { Id: "tblIdentityVerification", filename: "Identity Verification" },
    { Id: "tblInsuranceDetails", filename: "Insurance Create" },
    { Id: "tblinsuranceMaster", filename: "Import Insurance Details" },
    { Id: "tblPolicies", filename: "Policy Creation" },
    { Id: "present_tbl", filename: "Present" },
    { Id: "weekoff_tbl", filename: "Weekoff" },
    { Id: "holiday_tbl", filename: "Holiday" },
    { Id: "misspunch_tbl", filename: "Misspunch" },
    { Id: "absent_tbl", filename: "Absent" },
    { Id: "less_tbl", filename: "Less" },
    { Id: "Halfday_tbl", filename: "Halfday" },
    { Id: "onleave_tbl", filename: "Onleave" },
    { Id: "devicepunch_tbl", filename: "Device Punch" },
    { Id: "webpunch_tbl", filename: "Web Punch" },
    { Id: "mobilepunch_tbl", filename: "Mobile Punch" },
    { Id: "leavetype_tbl", filename: "Leave Type" },
    { Id: "othour_tbl", filename: "OT Hour" },
    { Id: "malegender_tbl", filename: "Male Gender" },
    { Id: "femalegender_tbl", filename: "Female Gender" },
    { Id: "Attration_tbl", filename: "Male Attration" },
    { Id: "FeAttration_tbl", filename: "Female Attration" },
    { Id: "teamtotal_tbl", filename: "Team Detail" },
    { Id: "teamcheckin_tbl", filename: "Team Checkin" },
    { Id: "teamabsent_tbl", filename: "Team Absent" },
    { Id: "teamleave_tbl", filename: "Team Leave" },
    { Id: "teamlatein_tbl", filename: "Team Latein" },
    { Id: "teamearlyout_tbl", filename: "Team Earlyout" },
    { Id: "teamearlyin_tbl", filename: "Team Earlyin" },
    { Id: "tblHoliday", filename: "Holiday" },
    { Id: "tblholidayoptionallist", filename: "Holiday Optional List" },
    { Id: "tblholidayoptionalListManger", filename: "Holiday Optional ListManger" },
    { Id: "tblAttendancecorrection", filename: "Attendance Correction" },
    { Id: "approvaltable", filename: "Payroll Approval" },
    { Id: "Fasttrack_Payroll_Distrubution", filename: "Fasttrack Payroll Distrubution" },

    { Id: "hrattendancetable", filename: "Attendance Finalization" },
    { Id: "Generate_TDS_table", filename: "Salary Details" },
    { Id: "summarizedreporttable", filename: "Summarized Report" },
    { Id: "bonusregisterreporttable", filename: "BonusRegister Report" },
    { Id: "wedgeregisterreporttable", filename: "WedgeRegister Report" },
    { Id: "salaryregisterreporttable", filename: "SalaryRegister Report" },
    { Id: "banksalaryreporttable", filename: "BankSalary Report" },
    { Id: "payroll_disbursementtable", filename: "Payroll Disbursement Report" },
    { Id: "monthWise_salary_register_table", filename: "Month Wise Salary Report" },
    { Id: "tbloutstandingdashboard", filename: "Outstanding Advances Dashboard Report" },
    { Id: "tblrecoverystatement", filename: "Bulk Recovery Statement Report" },
    { Id: "tblmonthlydeduction", filename: "Advance Deduction Report" },
    { Id: "ESICchallantbl", filename: "ESIC Challan" },
    { Id: "pfchallantbl", filename: "PF Challan" },
    { Id: "workflow_tbl", filename: "Work Flow" },
    { Id: "anoucement_tbl", filename: "Announcements" },
    { Id: "tblAttendanceSheet", filename: "Attendance" },
    { Id: "okrassigntbl", filename: "Okr Assign" },



    { Id: "tblImportError", filename: "Import Employees List" },
    { Id: "tblEnrollUsers", filename: "Enroll Users" },
    { Id: "device_tbl", filename: "Device" },
    { Id: "tblstengthdetails", filename: "Stength Details" },
    { Id: "tblgpapproval", filename: "GatePass Approval" },
    { Id: "tblissuepass", filename: "Issue Pass" },

    { Id: "head_list_table", filename: "Salary Heads" },
    { Id: "strcturetable", filename: "Pay Structure" },
    { Id: "employee_ctc_table", filename: "Employee Salary" },
    { Id: "loan_tbl", filename: "Non Recurring Income" },
    { Id: "GetallColumntable", filename: "Custom Column" },
    { Id: "companyLogotable", filename: "Payslip Template" },
    { Id: "payrollCycleDetails", filename: "Payroll Cycle" },
    { Id: "tblincrementapproval", filename: "Increment Approval" },
    { Id: "tbladdonSubcription", filename: "Add-On Subscription" },
    { Id: "tblTransactionDetails", filename: "Transaction Year" },
    { Id: "tblbranch", filename: "Audit-Branch" },
    { Id: "tblshift", filename: "Audit-Shift" },
    { Id: "tblBulkchanges", filename: "Employee Bulk Changes" },
    //{ Id: "okrreviewtbl", filename: "OKR Review" },
    { Id: "tblLeaveSanction", filename: "Leave Sanction" },
    { Id: "crpttbl", filename: "Analytics Reports" },
    { Id: "tblholidayoptionalapllied", filename: "Optinoal Holiday" },
    { Id: "tbltrasmonitor", filename: "Transaction Monitor" },
    { Id: "tbldeviceCommand", filename: "Device Command " },
    { Id: "tblSchedulersettings", filename: "Scheduler" },
    { Id: "tblCustomrepotemplate", filename: "Custom Reports Template" },
    { Id: "tblAttendanceSummary", filename: "AttendanceList" },
    { Id: "tblempjoinlist", filename: "Employee JoiningMaster" },
    { Id: "tblbatchdetails", filename: "Increment Planning" },
    { Id: "birthday_tbl", filename: "Birthday" },
    { Id: "tblEmployee_Strength", filename: "Strength" },
    { Id: "tblatwork", filename: "At Work" },
    { Id: "tbllateIn", filename: "late In" },
    { Id: "tblearlyout", filename: "Early Out" },
    { Id: "tbldevice", filename: "Device" },
    { Id: "tblMissPunch", filename: "MissPunch" },
    { Id: "pfmastertable", filename: "Provident Fund" },
    { Id: "esic_tbl", filename: "ESIC" },
    { Id: "taxMaster", filename: "Professional Tax" },
    { Id: "gratuitymastertable", filename: "Gratuity" },
    { Id: "LabourWelfareFundmastertable", filename: "Labour Welfare Fund" },
    { Id: "Employee_dataTable", filename: "Increment planning" },
    { Id: "tbldepartment1", filename: "Transaction Detail View Data" },
    { Id: "tblpendingdata", filename: "Transaction Detail Pending List" },
    { Id: "tblAttndancelist", filename: "Attendancelist" },
    { Id: "loan_tbl_view", filename: window.selectedEmpName },
    { Id: "loan_tbl_view_earning", filename: window.selectedEmpName },
    { Id: "tblAuditpolicy", filename: "Audit Policy" },
    { Id: "tblHrpolicy", filename: "HR Policy" },
    { Id: "tblHrpolicyAllocation", filename: "HR Policy Allocation" },
    { Id: "tblInsuranceDetail", filename: "Insurance Details" },
    { Id: "tblemployeeReferenceGuide", filename: "Employee Reference Guide" },
    { Id: "tblsuperAdmin", filename: "Registered Companies" },
    { Id: "tblPlan", filename: "Plans" },
    { Id: "tbltemplates", filename: "Templates" },
    { Id: "tbldropCompanies", filename: "Drop Companies" },
    { Id: "tblsysConfig", filename: "System Configuration" },
    { Id: "tblactivedevices", filename: "Registered Devices" },
    { Id: "tbldevicenotificationmail", filename: "Device Notification" },
    { Id: "tblsuperAdminAnalyticsdata", filename: "Analytics Dashboard" },
    { Id: "tblFeedback", filename: "Feedback" },
    { Id: "tbl_Policies", filename: "Policies" },
    { Id: "tblApprovalFlow", filename: "Approval Flow" },
    { Id: "tblexpesnePaymentGetAll", filename: "Expense Payment Sheet" },
    { Id: "tblExpenseRequest", filename: "Expense Request" },
    { Id: "tblReviewExpense", filename: "Review Expense" },
    { Id: "TripRequest_tbl", filename: "Trip Request" },
    { Id: "tblReviewTrip", filename: "Review Trip" },
    { Id: "tblCategoryGetall", filename: "Expense Category" },
    { Id: "emsdataview_table", filename: "Expense" },  
    { Id: "tbladvsalary", filename: "Advance Salary Request" },
    { Id: "tbladvsalaryapproval", filename: "Advance Salary Approval" },
    { Id: "tbldvenrolluser", filename: "Device Verification EnrollUser" },
    { Id: "tbldeviceinfo", filename: "Advance Salary Approval" },
    { Id: "tblNotificationruleConfigDetails", filename: "Rule Config Master" },
    { Id: "tblNotificationEventLogData", filename: "Notification Event LogData" },
    { Id: "feedbackview_table", filename: "Feedback Overview" },
    { Id: "tblShiftAllocationApprove", filename: "Shift Allocation Approve" },
    { Id: "tblTraineReq", filename: "Trainee Request" },
    { Id: "tblmedicaldoc", filename: "Trainee Medical Data" },
    { Id: "tblOnboarding", filename: "Trainee Education Data" },
    { Id: "tbladdlocation", filename: "Location" },
]
var columnsToRemoveByTable = {
    'tbluserDetails': [12],
    'tblShiftGroup': [4],
    'tblOnDutyLeave': [],
    'GetallColumntable': [2],
    'tblholidayoptionalapllied': [7],
    'tblrole': [3],
    'workflow_tbl': [4],
    'tblIdentityVerification': [0],
    'tblpolicy': [4,5],
    'companyLogotable': [6],
    'tblbatchdetails': [8],
    'tblincrementapproval': [8],
    'Employee_dataTable': [8],
    'exitclearance_tbl': [11],
    'tblCustomrepotemplate': [0],
    'tblPolicies': [0, 4],
    'tbldevice': [0],
    'tblInsuranceDetails': [0, 25],
    'tblstengthdetails': [12],
    'tblAttendanceSummary': [0, 11],
    'tblgpapproval': [0, 1, 14],
    'tblissuepass': [0, 13],
    'tblScheduler': [0],
    'anoucement_tbl': [5],
    'tblleaveapproval': [0],
    'tblAttendanceApproval': [0, 17],
    'tblWebpunchApproval': [0],
    'tblFacePunchApproval': [0],
    'hrattendancetable': [0],
    'Generate_TDS_table': [0],
    'tblCompanyMaster': [5],
    'tblBranchMaster': [5],
    'tblDepartmentMaster': [5],
    'tblDesignationMaster': [5],
    'tblEmployeeMaster': [5],
    'tblholidayoptionallist': [0],
    'tblOnDutyLeave': [0, 11],
    'tblAttendancecorrection': [0, 9],
    'pfmastertable': [0, 8],
    'esic_tbl': [0, 5],
    'taxMaster': [0, 3],
    'gratuitymastertable': [0, 6, 7],
    'LabourWelfareFundmastertable': [0, 5],
    'okrassigntbl': [0],
    'approvaltable': [12],
    'Fasttrack_Payroll_Distrubution': [9],
    'loan_tbl': [10],
    'tblinsuranceMaster': [5],
    'tblAmenity': [3],
    'tblRoom': [8],
    'tblHrpolicy': [3],
    'tblemployeeReferenceGuide': [3],
    'tblPlan': [7],
    'tbltemplates': [4],
    'tbldropCompanies': [0, 8],
    'tblsysConfig': [17],
    'tblFeedback': [12],
    'tbl_Policies': [15],
    'tblApprovalFlow': [10],
    'tblexpesnePaymentGetAll': [10],
    'tblExpenseRequest': [9],
    'tblReviewExpense': [7],
    'TripRequest_tbl': [5],
    'tblReviewTrip': [5],
    'tblCategoryGetall': [3],
    'tbladvsalary': [4],
    'tbladvsalaryapproval':[10],
    'feedbackview_table':[8],
    'tblmonthlydeduction': [10],
    'tblShiftAllocationApprove': [0, 9],
    'tblTraineReq': [30, 29, 27, 32],
    'tblOnboarding': [18,16,17,20],
    'tblmedicaldoc':[3,5]
};
function checkCloumnOfExcel(tableId) {
    return columnsToRemoveByTable[tableId] || [];
}
async function exportTableToExcelFileNormal(fileName) {
    const MAX_ROWS_PER_SHEET = 65000;
    const workbook = new ExcelJS.Workbook();

    /*const tableId = $('.new_datatbl:visible').attr('id');*/
    const $visibleTables = $('.new_datatbl:visible');

    const tableId = $visibleTables.last().attr('id');
    
    const dataTable = $(`#${tableId}`).DataTable();
    let visibleColumnIndexes = dataTable.columns(':visible').indexes().toArray();
 
    const columnsToRemove = checkCloumnOfExcel(tableId);
    visibleColumnIndexes = visibleColumnIndexes.filter(i => !columnsToRemove.includes(i));

    const headers = visibleColumnIndexes.map(index => {
        return dataTable.column(index).header().innerText.trim();
    });

    const allRowIndexes = dataTable.rows({ page: 'all' }).indexes().toArray();
    const totalRows = allRowIndexes.length;
    const totalSheets = Math.ceil(totalRows / MAX_ROWS_PER_SHEET);

    const createNewSheet = (sheetIndex) => {
        const ws = workbook.addWorksheet(`Report_${sheetIndex}`);
        const titleRow = ws.addRow([fileName.filename]);
        ws.mergeCells(1, 1, 1, headers.length);
        titleRow.font = { bold: true, size: 12, color: { argb: 'FFFFFFFF' } };
        titleRow.alignment = { horizontal: 'center' };
        ws.getCell(1, 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: '295097' }
        };

        const headerRow = ws.addRow(headers);
        headerRow.font = { bold: true, color: { argb: 'FF000000' } };
        headerRow.eachCell(cell => {
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'e3e3e3' }
            };
        });

        return ws;
    };

    for (let sheetIndex = 0; sheetIndex < Math.max(1, totalSheets); sheetIndex++) {
        const currentWorksheet = createNewSheet(sheetIndex + 1);
        const start = sheetIndex * MAX_ROWS_PER_SHEET;
        const end = Math.min(start + MAX_ROWS_PER_SHEET, totalRows);

        for (let i = start; i < end; i++) {
            const rowIndex = allRowIndexes[i];
            const cleanRow = visibleColumnIndexes.map(colIndex => {
                const cellHtml = dataTable.cell(rowIndex, colIndex).render('display');
                const tempDiv = document.createElement("div");
                tempDiv.innerHTML = cellHtml ?? '';
                return tempDiv.textContent.trim();
            });

            currentWorksheet.addRow(cleanRow);
        }
    }

    // Auto-fit column width
    workbook.eachSheet(sheet => {
        sheet.columns.forEach((column, colIndex) => {
            let maxLength = headers[colIndex]?.length || 10;
            for (let i = 2; i <= sheet.rowCount; i++) {
                const cell = sheet.getRow(i).getCell(colIndex + 1);
                let val = cell.value;
                if (val && typeof val === 'object') {
                    val = val.richText?.map(t => t.text).join('') || val.text || '';
                }
                maxLength = Math.max(maxLength, val?.toString().trim().length || 0);
            }
            column.width = Math.min(maxLength + 2, 50);
        });
    });

    // Export to Excel
    const buffer = await workbook.xlsx.writeBuffer();
    const blob = new Blob([buffer], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    });

    const link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.download = `${fileName.filename}.xlsx`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

async function exportToExcel(filename, jsonData) {
    const MAX_ROWS_PER_SHEET = 65000;
    if (!jsonData || jsonData.length === 0) {        
        return;
    }

    const workbook = new ExcelJS.Workbook();
    const headers = Object.keys(jsonData[0]);
    const totalRows = jsonData.length;
    const totalSheets = Math.ceil(totalRows / MAX_ROWS_PER_SHEET);

    const createSheet = (sheetIndex) => {
        const ws = workbook.addWorksheet(`Report_${sheetIndex}`);

        const titleRow = ws.addRow([filename]);
        ws.mergeCells(1, 1, 1, headers.length);
        titleRow.font = { bold: true, size: 12, color: { argb: 'FFFFFFFF' } };
        titleRow.alignment = { horizontal: 'center' };
        ws.getCell(1, 1).fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: '295097' }
        };

        const headerRow = ws.addRow(headers);
        headerRow.font = { bold: true, color: { argb: 'FF000000' } };
        headerRow.eachCell(cell => {
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'e3e3e3' }
            };
            cell.alignment = { horizontal: 'center' };
        });

        return ws;
    };

    for (let sheetIndex = 0; sheetIndex < totalSheets; sheetIndex++) {
        const ws = createSheet(sheetIndex + 1);
        const start = sheetIndex * MAX_ROWS_PER_SHEET;
        const end = Math.min(start + MAX_ROWS_PER_SHEET, totalRows);

        for (let i = start; i < end; i++) {
            const rowData = headers.map(h => jsonData[i][h]);
            ws.addRow(rowData);
        }
    }

    workbook.eachSheet(sheet => {
        sheet.columns.forEach((column, colIndex) => {
            let maxLength = headers[colIndex]?.length || 10;
            for (let i = 2; i <= sheet.rowCount; i++) {
                const val = sheet.getRow(i).getCell(colIndex + 1).value;
                if (val && typeof val === 'object') {
                    const strVal = val.richText?.map(t => t.text).join('') || val.text || '';
                    maxLength = Math.max(maxLength, strVal.length);
                } else {
                    maxLength = Math.max(maxLength, (val?.toString() || '').length);
                }
            }
            column.width = Math.min(maxLength + 2, 50);
        });
    });

    const buffer = await workbook.xlsx.writeBuffer();
    const blob = new Blob([buffer], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    });

    const link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.download = `${filename}.xlsx`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

function exportTableToExcel(tableID, filename = 'ShiftGroupData.xlsx') {
    var table = document.getElementById(tableID);
    var html = table.outerHTML.replace(/ /g, '%20');
    var a = document.createElement('a');
    a.href = 'data:application/vnd.ms-excel,' + html;
    a.download = filename;
    a.click();
}



$(document).on('click', '#export-button', function () {
    var _StatusValue = sessionStorage.getItem("StatusValue");
    var _MonthYear = sessionStorage.getItem("MonthYear");
    var CheckFastrackdb = sessionStorage.getItem("CheckFastrackdb");
    //const tableId = $('.new_datatbl:visible').attr('id');
    const $visibleTables = $('.new_datatbl:visible');

    const tableId = $visibleTables.last().attr('id');
    const dataTable = $(`#${tableId}`).DataTable();
    let visibleColumnIndexes = dataTable.columns(':visible').indexes().toArray();

   const filename1 = dict.find(item => item.Id == tableId);
    if (tableId == "hrattendancetable") {
        const removeIndex1 = visibleColumnIndexes.length;
        const removeIndex2 = visibleColumnIndexes.length - 1;

        // Ensure the array exists first
        if (!columnsToRemoveByTable['hrattendancetable']) {
            columnsToRemoveByTable['hrattendancetable'] = [];
        }

        // Push the index
        columnsToRemoveByTable['hrattendancetable'].push(removeIndex1, removeIndex2);
        var AttendanceName = `AttendanceFinalize_${_StatusValue}_${_MonthYear}`
        if (filename1 !== -1) {
            filename1.filename = AttendanceName;
        }
    } else if (tableId == "Generate_TDS_table") {

        if (_StatusValue == "ESF" || _StatusValue == 'ESD' || _StatusValue == 'Rejected') {
            if (CheckFastrackdb == 1) {              
                var updatedfilename = `FasttrackSalaryDisbursement_${_StatusValue}_${_MonthYear}`
                columnsToRemoveByTable['Generate_TDS_table'].push(6);
            }
            else {
                const removeIndex1 = visibleColumnIndexes.length - 1;
                // Ensure the array exists first
                if (!columnsToRemoveByTable['Generate_TDS_table']) {
                    columnsToRemoveByTable['Generate_TDS_table'] = [];
                }

                // Push the index
                columnsToRemoveByTable['Generate_TDS_table'].push(removeIndex1);
                var updatedfilename = `SalaryDisbursement_${_StatusValue}_${_MonthYear}`
            }
        } else {
            if (CheckFastrackdb == 1 && _StatusValue == "null" ){
                var updatedfilename = `EmployeeList`
                columnsToRemoveByTable['Generate_TDS_table'].pop();
            } else {
                const removeIndex1 = visibleColumnIndexes.length - 1;
                const removeIndex2 = visibleColumnIndexes.length - 3;

            // Ensure the array exists first
            if (!columnsToRemoveByTable['Generate_TDS_table']) {
                columnsToRemoveByTable['Generate_TDS_table'] = [];
            }

                // Push the index
                columnsToRemoveByTable['Generate_TDS_table'].push(removeIndex1, removeIndex2);
                var updatedfilename = `SalaryProcess_${_StatusValue}_${_MonthYear}`
            }
        }
        if (filename1 !== -1) {
            filename1.filename = updatedfilename;
        } 
    }
    if (tableId === "loan_tbl_view") {
        var empName = window.selectedEmpName;
         filename1.filename = empName
    }
    if (tableId === "loan_tbl_view_earning") {
        var empName = window.selectedEmpName;
        filename1.filename = empName
    }
    if (tableId === "tblpolicy" && window.selectedName === "Policy") {
        filename1.filename = "GatePass Policy"
        
    }
    if (tableId === "Employee_dataTable") {
        var _policyName = sessionStorage.getItem("PolicyName");
        filename1.filename = `Employeelist_Increment_${_policyName}`;
    }
    if (tableId == "emsdataview_table") {
        var fileName = window.selectedExcelFileName;
        filename1.filename = fileName;
    }
    if (tableId == "tbldeviceinfo") {
        var fileName = window.selectedExcelFileNameDevice;
        filename1.filename = fileName;
    }
  
    exportTableToExcelFileNormal(filename1);
})

const fileNameMap = {
    "tblWorkPolicyAllocation": "Work Policy Allocation",
    "tblClient": "Client",
    "tblWorkPolicy": "Work Policy"
};
$(document).on('click', '#export-button1', function () {

    const tableId = $(this).data('table-id');
    const dataTable = $(`#${tableId}`).DataTable();

    const allData = dataTable.rows({ page: 'all' }).data().toArray();

    const columnsToRemoveMap = {
        "tblWorkPolicyAllocation": [1, 4, 6, 8],
        "tblClient": [1],
        "tblWorkPolicy": [0, 1]  // Remove Sr. No and Policy ID columns from export
    };
    const columnIndexesToRemove = columnsToRemoveMap[tableId] || [];

    const columnDefs = dataTable.settings().init().columns;
    const keysToRemove = columnIndexesToRemove.map(idx => columnDefs[idx].data).filter(Boolean);

    const filteredData = allData.map(row => {
        const obj = { ...row };
        keysToRemove.forEach(key => delete obj[key]);
        return obj;
    });

    const fileName = fileNameMap[tableId];

    exportToExcel(fileName, filteredData);

});

function injectExportButtonsForDataTablesMaster(isServerSide, onClickCallback, _exportflage)
{
  
    
    var exportflageId = _exportflage;

    if ($('#buttons').length && $('#buttons').children().length === 0) {
        const exportBtnHtml = `
            <div id="export-button_${exportflageId}" class="export-button tooltips" data-placement="bottom" data-original-title="Download">
                <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
            </div>         
        `;

        $('#buttons').append(exportBtnHtml);

        // Tooltip init
        if ($.fn.tooltip) {
            $('[data-toggle="tooltip"]').tooltip();
        }

        // Optional: Add click events
        $(`#export-button_${exportflageId}`).on('click', function () {
  
            if (typeof onClickCallback === 'function') {
                onClickCallback();
            } else {
                toastr.info("Default export action...");
            }
        });
    }
}

