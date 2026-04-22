// const { debug } = require("node:util");
// Define global FilterParamobj with default values
var FilterParamobj =
{
    CompanyID: "0",
    BranchID: "0",
    EmpID: "0",
    EmployeeFilter:
    {
        CompanyIDs: [],
        BranchIDs: [],
        DepartmentIDs: [],
        DesignationIDs: []
    }
};
$(document).ready(function () {
    $("#btnApply").click(function (event) {
        var checkedColumnIds = [];
        $('#fieldlst :input').each(function () {
            var fieldId = $(this).attr("id");
            var column = Directory_table.column($(this).attr('data-column'));

            // Force "EmpName" (chk_2) to always be checked and visible
            if (fieldId === "chk_2") {
                $(this).prop("checked", true);  // Ensure EmpName checkbox is always checked
                column.visible(true);          // Ensure EmpName column is always visible
            }

            if ($(this).is(':checked')) {
                column.visible(true);
                var removedPrefixId = fieldId.replace("chk_", "");
                checkedColumnIds.push(removedPrefixId);
            } else {
                column.visible(false);
            }
        });

        checkedColumnsList = checkedColumnIds.join(",");

        // Remove existing footer
        $("#tblEmployee thead tr:eq(1)").remove();
        var footer = '<tr class="footer">';
        $("#tblEmployee thead th").each(function (index) {
            var headerText = $(this).text().trim();
            if (headerText === "Status") {
                footer += `<th><input type="text" class="search_input" data-column="${headerText}" placeholder="Search" /><i class="clm-search"></i></th>`;
            } else if (["EmpName", "Empcode", "EmpPunchID", "BranchName", "Email", "ShiftName", "ShiftGroupName", "DepartmentName", "EmpJoinDate", "PolicyName", "CompanyName", "ReportingName"].includes(headerText)) {
                footer += `<th><input type="text" class="search_input" data-column="${headerText}" placeholder="Search" /><i class="clm-search"></i></th>`;
            } else {
                footer += '<th></th>';
            }
        });

        footer += '</tr>';
        $("#tblEmployee thead").append(footer);
        $('#frmpopupCustomfld').modal('toggle');
        $('#tblEmployee thead tr:eq(1) th:last').addClass('fixed-column');
        $('#tblEmployee thead tr:eq(2) th:last').addClass('fixed-column-1');
    });


    $("#btnchkSelectAll").click(function (event) {

        _chkarrylst = 0;
        $('#fieldlst :input').each(function () {
            var _idfield = $(this).attr("id");
            $("#" + _idfield).prop('checked', true);
            var _rmidfield = _idfield.replace("chk_", "");
            _chkarrylst += "," + _rmidfield;
        });
    });

    $("#btnchkSelectnone").click(function (event) {
        _chkarrylst = 0;
        $('#fieldlst :input[type="checkbox"]').each(function () {
            var _idfield = $(this).attr("id");
            var _rmidfield = _idfield.replace("chk_", "");

            // Uncheck all checkboxes except the one for column 2
            if (_rmidfield == 2) {
                $("#" + _idfield).prop('checked', true);
            } else {
                $("#" + _idfield).prop('checked', false);
            }
            _chkarrylst += "," + _rmidfield;
        });
    });

    ///////For Custome Columns Select Event End///////////////    

    $("#tblEmployee").on('click', '.Inactiveclass', function () {
        $("#EmpInactiveResignDate").val("");
        $(".Inactiveclass").attr("data-toggle", "modal");
        $(".Inactiveclass").attr("data-target", "#frmInactiveEmp");
        $("#EmpId").val(t.row($(this).parent('td').parent('tr')).data()["EmpId"]);
    });

});
function showvisibilityColumn() {
    var _chkarrylstsp = _chkarrylst;
    _chkarrylst = "";
    var _lst = "";
    var _idfield = "";

    if (checkedColumnsList != 0) {
        _chkarrylstsp = checkedColumnsList;
    }
    var _ss = (_chkarrylstsp || "").toString().split(",");
    var _lst = 0;

    $('#fieldlst :input').each(function () {
        $('#fieldlst').val('');
        _idfield = $(this).attr("id");
        var _rmidfield = _idfield.replace("chk_", "");
        $("#" + _idfield).prop('checked', false);
        var _ss = _chkarrylstsp.split(",")
        for (var i = 0; i <= _ss.length; i++) {
            if (_ss[i] == _rmidfield) {
                if (_ss[i] == 0) {
                    $("#" + _idfield).prop('checked', true);
                    $("#" + _idfield).attr('disabled', true);
                }
                else {
                    $("#" + _idfield).prop('checked', true);
                }
                var _rmidfield = _idfield.replace("chk_", "");
                _lst += "," + _rmidfield;
            }
        }
    });
    _chkarrylst = _lst;
    $("#btnColumnvisibility").attr("data-toggle", "modal");
    $("#btnColumnvisibility").attr("data-target", "#frmpopupCustomfld");
}

//***********************************Employee Edit & Delete***********************************//

function CommonDateformatNew(data, dateFormat, returnDateObject = false) {
    if (data != null && data !== "") {
        let timestamp;
        if (data.indexOf('/Date') !== -1) {
            timestamp = parseInt(data.match(/\d+/)[0], 10); // Extract numeric timestamp
        } else {
            if (data.indexOf('T') !== -1) {
                data = data.split('T')[0]; // Get the date part (before 'T')
            }
            const dateParts = data.split('-');
            if (dateFormat === "yyyy-mm-dd") {
                return new Date(dateParts[0], dateParts[1] - 1, dateParts[2]);
            }
            // Handle other formats if needed...
        }

        const dateObj = new Date(timestamp);
        if (returnDateObject) {
            return dateObj; // Return Date object directly
        }

        const dateParts = [
            dateObj.getFullYear(),
            ('0' + (dateObj.getMonth() + 1)).slice(-2),
            ('0' + dateObj.getDate()).slice(-2)
        ];
        return applyDateFormat(dateParts, dateFormat, dateObj); // Apply formatting for epoch date
    }
    return ""; // Return empty if the date is null or invalid
}


function CommonDateformat(data, dateFormat) {
    if (data != null && data !== "") {

        var timestamp;
        // Check if the data is in epoch format like /Date(1729535400000)/
        if (data.indexOf('/Date') !== -1) {

            timestamp = parseInt(data.match(/\d+/)[0], 10);  // Extract numeric timestamp
        }
        else {

            //// Assuming it's in yyyy-mm-dd format already
            //var date1 = data.split(' ')[0];  // Get the date part (before space)
            //var dateParts = date1.split('-');
            //return applyDateFormat(dateParts, dateFormat, null);  // Apply formatting for standard date

            if (data.indexOf('T') !== -1) {
                data = data.split('T')[0]; // Get the date part (before 'T')
            }
            // Assuming it's in yyyy-mm-dd or dd-mm-yyyy format
            var dateParts = data.split('-');
            return applyDateFormat(dateParts, dateFormat, null);
        }
        // Convert the timestamp to a Date object
        var dateObj = new Date(timestamp);
        // Now format the date based on the given format
        var dateParts =
            [
                dateObj.getFullYear(),
                ('0' + (dateObj.getMonth() + 1)).slice(-2),  // Month is 0-based, so we add 1
                ('0' + dateObj.getDate()).slice(-2)
            ];

        return applyDateFormat(dateParts, dateFormat, dateObj);  // Apply formatting for epoch date
    }
    return "";  // Return empty if the date is null or invalid
}
function applyDateFormat(dateParts, dateFormat, dateObj) {
    // This function arranges the date parts based on the required date format
    if (dateFormat === "dd-mm-yyyy") {
        return dateParts[2] + "-" + dateParts[1] + "-" + dateParts[0];  // dd-mm-yyyy
    }
    else if (dateFormat === "yyyy-mm-dd") {
        return dateParts[0] + "-" + dateParts[1] + "-" + dateParts[2];  // yyyy-mm-dd
    }
    else if (dateFormat === "mm-dd-yyyy") {
        return dateParts[1] + "-" + dateParts[2] + "-" + dateParts[0];  // mm-dd-yyyy
    }
    else if (dateFormat === "yyyy-M-dd" || dateFormat === "M-dd-yyyy" || dateFormat === "dd-M-yyyy") {
        if (!dateObj) {
            return "";  // If no dateObj is passed, return empty since we can't extract the month name
        }
        var month = dateObj.toLocaleString("en-us", { month: "short" });  // Get short month name
        if (dateFormat === "yyyy-M-dd") {
            return dateParts[0] + "-" + month + "-" + dateParts[2];  // yyyy-M-dd
        } else if (dateFormat === "M-dd-yyyy") {
            return month + "-" + dateParts[2] + "-" + dateParts[0];  // M-dd-yyyy
        } else if (dateFormat === "dd-M-yyyy") {
            return dateParts[2] + "-" + month + "-" + dateParts[0];  // dd-M-yyyy
        }
    }
    return "";  // Return empty if the date format is invalid
}

function getEditButton(empcode, isActive) {
    if (isActive) {
        var Action = '<a class="editclass btn btn-xs seagreen_btnnew tooltips editbtn" title="Edit">' +
            '<span id="sp3" hidden>' + empcode + '</span>' +
            '<i class="fa-regular fa-pen-to-square"></i></a>' +
            '<a class="Inactiveclass btn btn-xs red_btnnew tooltips" title="Inactive">' +
            '<i class="fa-solid fa-xmark"></i></a>';
        if (Companycode == "TAT02D3" || Companycode == "MINC2E0") {
            Action += '<a class="quick_block btn btn-xs seagreen_btnnew tooltips discountinuation" title="Discontinuation"><i class="fa-light fa-circle-minus"></i></a>' +
                '<a class="quick_block btn btn-xs seagreen_btnnew tooltips Conversion" title="Conversion "><i class="fa-regular fa-arrows-rotate"></i></i></a>'
        }
        return Action;
    } else {
        return '<a class="editclass btn btn-xs seagreen_btnnew tooltips editbtn" title="Edit">' +
            '<span id="sp3" hidden>' + empcode + '</span>' +
            '<i class="fa-regular fa-pen-to-square"></i></a>' +
            '<a class="Activeclass btn btn-xs green_btnnew tooltips" title="Active">' +
            '<i class="fa-solid fa-check"></i></a>';
    }
}

function getColumnConfig() {
    return [
        { "data": "EmpId" },
        { "data": "UserId" },
        { "data": "EmpName" },
        { "data": "Empcode" },
        { "data": "RoleId" },
        { "data": "ReligionId" },
        { "data": "IsSMS" },
        { "data": "Gender" },
        { "data": "EmpMarried" },
        { "data": "EmpJoinDate" },
        { "data": "EmpPunchID" },
        { "data": "BranchId" },
        { "data": "BranchName" },
        { "data": "DesignationId" },
        { "data": "DepartmentId" },
        { "data": "DepartmentName" },
        { "data": "DesignationName" },
        { "data": "ShiftId" },
        { "data": "ShiftName" },
        { "data": "CompanyID" },
        { "data": "CompanyName" },
        { "data": "Email" },
        { "data": "Password" },
        { "data": "EmpDOB" },
        { "data": "EmpAddress" },
        { "data": "EmpPhNo" },
        { "data": "EmpMNo" },
        { "data": "EmpPhoto" },
        { "data": "MobNoSMS" },
        { "data": "EmpResignDate" },
        { "data": "TypeId" },
        { "data": "EmpTypeName" },
        { "data": "GradeId" },
        { "data": "ShiftGroupId" },
        { "data": "ShiftGroupName" },
        { "data": "ContractorId" },
        { "data": "ContractorName" },
        { "data": "CategoryId" },
        { "data": "Categoryname" },
        { "data": "ReportingTo" },
        { "data": "PolicyId" },
        { "data": "PolicyName" },
        { "data": "EmpWeekOff" },
        { "data": "EmpSecondWeekOff" },
        { "data": "EmpSecondWeekOffRule" },
        { "data": "EmpHalfDay" },
        { "data": "EmpHalfDayRule" },
        { "data": "ShiftShortName" },
        { "data": "ShiftGroupShortName" },
        { "data": "isActive" },
        { "data": "Status" },
        { "data": "Married" },
        { "data": "JoinDate" },
        { "data": "BirthDate" },
        { "data": "EmpGender" },
        { "data": "RoleName" },
        { "data": "TagId" },
        { "data": "worf" },
        { "data": "geoenable" },
        { "data": "CountryCode" },
        { "data": "BranchGeolocation" },
        { "data": "ReportingName" },
        { "data": "BUName" },
        { "data": "SubBUName" },
        { "data": "BUId" },
        { "data": "SubBUId" },

    ];
}

function getColumnDefs(RoleId, isedit, columnNames) {
    return [
        {
            "defaultContent": "-",
            "targets": "_all"
        },
        {
            targets: [columnNames.length],
            'orderable': false,
            sTitle: "Action",
            "data": null,
            className: 'fixed-column',
            createdCell: function (td) {
                $(td).css({ "min-width": "80px", "white-space": "nowrap" });
            }
        },
        {
            "targets": [9, 23, 29], // Apply to all the target columns at once
            render: function (data, type, row) {
                if (data != null && data != "") {

                    var date1 = data.split(' ')[0]; // Extract the date part (ignore time)
                    var date = date1.split('-'); // Split the date into an array
                    // Declare a variable for the final formatted date
                    var firstValue = "";

                    // Check if the date is in a valid format (length of array is 3)
                    if (date.length === 3) {
                        // Format the date based on the `isdateformat` variable
                        if (isdateformat == "dd-mm-yyyy") {
                            firstValue = date[2] + "-" + date[1] + "-" + date[0]; // Convert to yyyy-mm-dd
                        }
                        else if (isdateformat == "yyyy-mm-dd") {
                            firstValue = date[0] + "-" + date[1] + "-" + date[2]; // Keep in yyyy-mm-dd
                        }
                        else if (isdateformat == "mm-dd-yyyy") {
                            firstValue = date[1] + "-" + date[2] + "-" + date[0]; // Convert to mm-dd-yyyy
                        }
                        else if (isdateformat == "yyyy-M-dd") {
                            var _date = new Date(date[2] + "-" + date[1] + "-" + date[0]);
                            var locale = "en-us";
                            var month = _date.toLocaleString(locale, { month: "short" });
                            firstValue = date[0] + "-" + month + "-" + date[2]; // Convert to yyyy-M-dd
                        }
                        else if (isdateformat == "M-dd-yyyy") {
                            var _date = new Date(date[0] + "-" + date[1] + "-" + date[2]);
                            var locale = "en-us";
                            var month = _date.toLocaleString(locale, { month: "short" });
                            firstValue = date[2] + "-" + month + "-" + date[1]; // Convert to M-dd-yyyy
                        }
                        else if (isdateformat == "dd-M-yyyy") {
                            var _date = new Date(date[0] + "-" + date[1] + "-" + date[2]);
                            var locale = "en-us";
                            var month = _date.toLocaleString(locale, { month: "short" });
                            firstValue = date[2] + "-" + month + "-" + date[0]; // Convert to dd-M-yyyy
                        }
                    }
                    else {
                        firstValue = ""; // Return an empty string if the date is not valid
                    }
                } else {
                    firstValue = ""; // Handle null or empty data
                }

                return firstValue;
            }
        },
        {
            "targets": [columnNames.length],
            render: function (data, type, row) {
                if (RoleId == 1) {
                    return isedit ? getEditButton(row.Empcode, row.isActive) : '';
                }
                else if (RoleId == "6805" || RoleId == "6806" && isedit === "True") {
                    return getEditButton(row.Empcode, row.isActive);
                }
                else {
                    return '';
                }
            },
        },
        {
            "targets": 62,
            'orderable': false
        },
        {
            "targets": 62, // Target the "Status" column
            "createdCell": function (td, cellData, rowData, row, col) {
                let statusClass = ''; // Initialize the class for the cell

                // Determine the class based on the Status value
                if (rowData.Status === 'Active') {
                    statusClass = 'green_font'; // Apply green font class for Active
                } else if (rowData.Status === 'InActive') {
                    statusClass = 'red_font'; // Apply red font class for InActive
                }

                // Add the computed class to the cell
                $(td).addClass(statusClass);
                $(td).addClass('fixed-column-1'); // Add 'fixed-column-1' class for fixed styling
            }
        },
        {
            "targets": [2, 12, 21],
            "createdCell": function (td, cellData, rowData, row, col) {
                $(td).css('min-width', '160px');
            }
        },
        {
            "targets": [2],
            "render": function (data, type, row) {
                var nameParts = row.EmpName.replace(/\s+/g, ' ').trim().split(" ");
                var initials = nameParts[0].charAt(0).toUpperCase();
                if (nameParts.length > 1) {
                    initials += nameParts[1].charAt(0).toUpperCase();
                }
                const colors = ["#e74c3c", "#8e44ad", "#3498db", "#16a085", "#f39c12", "#d35400", "#2c3e50"];
                //const randomColor = colors[Math.floor(Math.random() * colors.length)];
                const randomColor = "#295097";

                data = `<div class="emp_box empbox_pending" >
                         <img alt="" class="user_img img-circle tooltips" data-placement="right" data-original-title="Empcode : ${row.Empcode ? row.Empcode : null}" src="/UploadEmpPhoto/${row.EmpPhoto ? row.EmpPhoto : null}" onerror="this.style.display='none'; this.nextElementSibling.style.display='inline-block';"/>
                         <span class="tooltips" data-placement="right" data-original-title="Empcode : ${row.Empcode}"  style="display: none; min-width: 30px; height: 30px; line-height: 30px; text-align: center; background-color: #ecf3ff; color:  ${randomColor}; border-radius: 50% !important; border: 1px solid #8ab1f7; margin-right: 8px;">
                         ${initials}</span><span>${row.EmpName}</span></div>`;

                return data;
            }
        },
        {
            "targets": [3, 10],
            "createdCell": function (td, cellData, rowData, row, col) {
                $(td).css('min-width', '120px');
            }
        },
        {
            "searchable": false,
            "orderable": false,
            "targets": 0
        }
    ];
}

function initTableCustomization(PlanId, RoleId, columnNames, _num, _numArray, tableElement) {

    const tableInstance = $(tableElement).DataTable();
    // Column header update based on PlanId or RoleId
    $('#tblEmployee thead th').eq(62).addClass('fixed-column-1');


    if (PlanId == "4") {
        //$('#tblEmployee tr:eq(0) th:eq(2)').html("EmpName");
        $('#tblEmployee tr:eq(0) th:eq(2)').html("<a class='btn btn-xs tooltips' title='Columns' onclick='showvisibilityColumn();' id='btnColumnvisibility'><i class='fa-regular fa-columns' style='font-size: 14px; color: #295097;'></i></a> EmpName");
    } else {
        $('#tblEmployee tr:eq(0) th:eq(2)').html("<a class='btn btn-xs tooltips' title='Columns' onclick='showvisibilityColumn();' id='btnColumnvisibility'><i class='fa-regular fa-columns' style='font-size: 14px; color:#295097;'></i></a> EmpName");
    }

    if (RoleId == 6805 || RoleId == 6806) {
        $('#tblEmployee tr:eq(0) th:eq(2)').html("EmpName");
    }

    // Hide specific columns
    _num = _num.replace(', ' + columnNames.length, '');
    tableInstance.columns([_num]).visible(false);

    // Hide loader
    $("#ajax_loaderbranch").hide();

    // Build footer with columns
    var footer = '<tr>';
    columnNames.forEach(function (columnName, index) {
        if (!_numArray.includes(index)) {
            // Add the class for index 60
            if (index === 62) {
                footer += `<th class="fixed-column-1">${columnName}</th>`;
            } else {
                footer += `<th>${columnName}</th>`;
            }
        }
    });
    footer += '<th class="fixed-column"></th></tr>';
    $("#tblEmployee thead").append(footer);

    // Add search inputs dynamically
    $("#tblEmployee thead tr:eq(1) th").each(function (index) {
        var title = $(this).text();
        if (["EmpName", "Empcode", "EmpPunchID", "BranchName", "Email", "Status"].includes(title)) {
            $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search" id=""></i>`);
        } else {
            $(this).html('');
        }
    });

    // Apply the Search for specific columns
    $('.nosearch').parents('th').each(function () {
        tableInstance.column($(this).index()).search('');
    });
    let debounceTimer;
    // Apply the Search for all Columns
    $("#tblEmployee thead").on("input", "input", function () {
        clearTimeout(debounceTimer); // Clear previous timer
        debounceTimer = setTimeout(() => {
            var columnIndex = $(this).parent().index();
            var visibleColumns = tableInstance.columns(':visible').indexes().toArray();
            var actualIndex = visibleColumns[columnIndex];
            tableInstance.column(actualIndex).search(this.value).draw();
        }, 1000);
    });

}


function GenerateEmployeeExcelFile(MainObjFilter, RoleWiseFilter) {

    var _PagesizeDefault = 10;
    var searchCriteria = [];

    var MainObjFilter =
    {
        "CompanyIDs": MainObjFilter.CompanyIDs || "",
        "BranchIDs": MainObjFilter.BranchIDs || "",
        "DepartmentIDs": MainObjFilter.DepartmentIDs || "",
        "DesignationIDs": MainObjFilter.DesignationIDs || ""
    };
    var RoleWiseFilter =
    {
        "CompanyID": RoleWiseFilter.CompanyID || "0",
        "BranchID": RoleWiseFilter.BranchID || "0",
        "EmpID": RoleWiseFilter.EmpID || "0"
    };
    var mainObj =
    {
        "search": $('#searchTerm').val() || "",
        "sortColumn": "EmpID",  // Default sort column
        "sortOrder": "asc",     // Default sort order
        "pageSize": _PagesizeDefault,
        "page": 1,
        "ExportFlage": 1,
        "DatagridThresold": 10000,
        "CustFilter": searchCriteria.length > 0 ? searchCriteria : [],
        "EmployeeFilter": MainObjFilter,
        "RoleWiseFilter": RoleWiseFilter
    };
    $.ajax({
        url: '/DataGridOptimize/GetAllEmployeeExcelData',
        type: 'POST',
        data: JSON.stringify(mainObj),
        contentType: 'application/json',
        beforeSend: function () {
            $("#ajax_loaderbranch").show();
        },
        success: function (response) {
            if (response.success && response.filePath) {
                /* toastr.success("Export completed. Starting download...");*/
                // Trigger the download using the returned file path
                window.location.href = response.filePath;
            }
            else {
                toastr.error(response.message || "Failed to generate export file.");
            }
        },
        error: function () {
            toastr.error("An error occurred while exporting data.");
        },
        complete: function () {
            $("#ajax_loaderbranch").hide(); // Hide the loader
        }
    });
}

function downloadFile(fileId) {
    const link = document.createElement('a');
    link.href = `/api/export/download/${fileId}`;
    link.download = `Export_${fileId}.csv`; // Set desired file name
    link.style.display = 'none';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

function processColumnsAndAppend(_CheckcolumnNames, columnNames, _hdrcntStart, _CnttdStart) {
    let _num = "";
    let _chkarrylst = "";
    let _Cnttd = _CnttdStart || 0;
    let columns = [];
    let _hdrcnt = _hdrcntStart || 0;

    // Process columns
    for (let i in _CheckcolumnNames) {

        if (_hdrcnt > 65) {
            _num += ", " + _hdrcnt;
        }

        if (['0', '1', '4', '5', '6', '7', '8', '11', '13', '14', '17', '19', '22', '25', '27', '28', '30', '31', '32', '33', '35', '36', '37', '38', '39', '40', '42', '43', '44', '45', '46', '47', '48', '49', '51', '52', '53', '56', '57', '58', '59', '60'].includes(_hdrcnt.toString())) {
            // Skip these columns
        } else {
            if (_Cnttd === 6) {
                _Cnttd = 0;
            }
            _Cnttd += 1;

            if (['2', '3', '10', '12', '21', '62'].includes(_hdrcnt.toString())) {
                _chkarrylst += "," + _hdrcnt;
                $(`#tr_${_Cnttd}`).append(
                    `<td ID=${_hdrcnt}><label class='toggle-vis' data-column=${_hdrcnt}><input type='checkbox' name=${columnNames[i]} class='toggle-vis' data-column=${_hdrcnt} ID=chk_${_hdrcnt} checked><span> ${columnNames[i]} </span></label></td>`
                );
            } else {
                $(`#tr_${_Cnttd}`).append(
                    `<td ID=${_hdrcnt}><label class='toggle-vis' data-column=${_hdrcnt}><input type='checkbox' name=${columnNames[i]} class='toggle-vis' data-column=${_hdrcnt} ID=chk_${_hdrcnt}><span> ${columnNames[i]} </span></label></td>`
                );
            }
        }
        _hdrcnt += 1;
    } 

    // Create column definitions
    for (let i in columnNames) {
        if (columnNames[i] === "EmpMNo") {
            columns.push({
                data: columnNames[i],
                title: columnNames[i],
                render: function (data, type) {
                    return type === 'display' && data?.trim() ? astMobileno(data) : data;
                }
            });
        } else if (columnNames[i] === "Email") {
            columns.push({
                data: columnNames[i],
                title: columnNames[i],
                render: function (data, type) {
                    return type === 'display' && data?.trim() ? obfuscateEmail(data) : data;
                }
            });
        } else {
            columns.push({
                data: columnNames[i],
                title: columnNames[i]
            });
        }
    }
    return { _num, _chkarrylst, columns };
}

function ensureString(val) {
    if (typeof val === 'string') {

        return val;
    } else if (Array.isArray(val)) {
        return val.join(",");
    } else {
        // Handle unexpected types or return an empty array
        return "";
    }
}

function LoadEmployeeGridData(page, FilterParamobj, DataGridParameterObject) {
    var columns = [];
    var isedit = _objview;
    var _num = "";
    var _hdrcnt = 0;
    var _Cnttd = 0;
    var _numArray = [];
    var P = 4;
    var fieldCount = 1;
    var searchCriteria = [];
    var _NewURL = DataGridParameterObject._webApiUrl;



    var MainObjFilter =
    {
        "CompanyIDs": ensureString(FilterParamobj.EmployeeFilter.CompanyIDs),
        "BranchIDs": ensureString(FilterParamobj.EmployeeFilter.BranchIDs),
        "DepartmentIDs": ensureString(FilterParamobj.EmployeeFilter.DepartmentIDs),
        "DesignationIDs": ensureString(FilterParamobj.EmployeeFilter.DesignationIDs)
    };
    RoleWiseFilter =
    {
        "CompanyID": FilterParamobj.CompanyID,
        "BranchID": FilterParamobj.BranchID,
        "EmpID": FilterParamobj.EmpID
    };
    var mainObj =
    {
        "searchTerm": $('#searchTerm').val() || "",
        "sortColumn": "EmpID",  // Default sort column
        "sortOrder": "asc",     // Default sort order
        "pageSize": DataGridParameterObject._PagesizeDefault,
        "page": 1,
        "ExportFlage": 0,
        "DatagridThresold": DataGridParameterObject.DataGridThreSold,
        "CustomFilters": [],    // Default empty filter list
        "EmployeeFilter": MainObjFilter,
        "RoleWiseFilter": RoleWiseFilter
    };
    if ($.fn.DataTable.isDataTable('#tblEmployee')) {
        $('#tblEmployee').DataTable().destroy();
        $('#dataTables_tbl_header').remove();
    }
    $('#tblEmployee').empty();

    $.ajax({
        type: "POST",
        headers: { 'Authorization': DataGridParameterObject._token },
        url: _NewURL + 'DataGridOptimize/GetEmployeeGridData',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 30000,
        data: JSON.stringify(mainObj),

        beforeSend: function () {
            $("#ajax_loaderbranch").show();
        },
        success: function (response) {
            if (response != null && response.Result != null) {

                const employeeList = response.Result.Items;
                if (MainObjFilter.CompanyIDs.length > 0 && MainObjFilter.BranchIDs.length > 0 && MainObjFilter.DepartmentIDs.length > 0 && MainObjFilter.DesignationIDs.length > 0) {
                    handleGridVisibility("tblempty", "NotificationGrid-empty", employeeList, {
                        imageUrl: "emptyscreen_3.png",
                        subText: "No matching records found for the selected filters."
                    });
                }
                else {
                    handleGridVisibility("tblempty", "NotificationGrid-empty", employeeList, {
                        imageUrl: "emptyscreen_1.png",
                        subText: "No entries found. Please add a record to get started."
                    });
                }
            }

            if (response.Result != null && Array.isArray(response.Result.Items) && response.Result.Items.length > 0) {
                $("#ajax_loaderbranch").hide();
                $("#fieldlst tr td").remove();
                var totalRecordCount = response.Result.totalRecords;
                var data = response.Result.Items;
                columnNames = Object.keys(data[0]);
                _chkarrylst = 0;
                var _CheckcolumnNames = Object.keys(data[0]);
                var _Total_hdrcnt = 67
                _num = "0,1,4,5,6,7,8,9,11,13,14,15,16,17,18,19,20,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,63,64";
                if (Companycode == "MAN5F0F") {
                    _num = "0,1,4,5,6,7,8,9,11,13,14,15,16,17,18,19,20,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66";
                    _Total_hdrcnt = 67;
                }

                _numArray = _num.split(',').map(Number);


                for (var i in _CheckcolumnNames) {
                    if (_hdrcnt > _Total_hdrcnt) {
                        _num += ", " + _hdrcnt
                    }

                    if (Companycode == "MAN5F0F") {

                        if (_hdrcnt == "0" || _hdrcnt == "1" || _hdrcnt == "4" || _hdrcnt == "5" || _hdrcnt == "6" || _hdrcnt == "7" || _hdrcnt == "8" || _hdrcnt == "11" || _hdrcnt == "13" || _hdrcnt == "14" || _hdrcnt == "17" || _hdrcnt == "19" || _hdrcnt == "22" || _hdrcnt == "25" || _hdrcnt == "27" || _hdrcnt == "28" || _hdrcnt == "30" || _hdrcnt == "31" | _hdrcnt == "32" || _hdrcnt == "33" || _hdrcnt == "35" || _hdrcnt == "36" || _hdrcnt == "37" || _hdrcnt == "38" || _hdrcnt == "41" || _hdrcnt == "40" || _hdrcnt == "43" || _hdrcnt == "44" || _hdrcnt == "45" || _hdrcnt == "46" || _hdrcnt == "47" || _hdrcnt == "48" || _hdrcnt == "49" || _hdrcnt == "50" || _hdrcnt == "51" || _hdrcnt == "52" || _hdrcnt == "53" || _hdrcnt == "56" || _hdrcnt == "57" || _hdrcnt == "58" || _hdrcnt == "59" || _hdrcnt == "60" || _hdrcnt == "65" || _hdrcnt == "66") {
                        }
                        else {
                            if (_Cnttd == 6) {
                                _Cnttd = 0;
                            }
                            _Cnttd = _Cnttd + 1;
                            if (_hdrcnt == "2" || _hdrcnt == "3" || _hdrcnt == "10" || _hdrcnt == "12" || _hdrcnt == "21") {
                                _chkarrylst += "," + _hdrcnt;
                                $('#tr_' + _Cnttd + '').append("<td  ID=" + _hdrcnt + "><label class='toggle-vis' data-column=" + _hdrcnt + "><input type='checkbox' name=" + columnNames[i] + " class='toggle-vis' data-column=" + _hdrcnt + " ID=chk_" + _hdrcnt + " checked><span> " + columnNames[i] + " </span></label></td>");
                            }
                            else {
                                $('#tr_' + _Cnttd + '').append("<td  ID=" + _hdrcnt + "><label class='toggle-vis' data-column=" + _hdrcnt + "><input type='checkbox' name=" + columnNames[i] + " class='toggle-vis' data-column=" + _hdrcnt + " ID=chk_" + _hdrcnt + "><span> " + columnNames[i] + " </span></label></td>");
                            }
                        }
                    }
                    else {
                        if (_hdrcnt == "0" || _hdrcnt == "1" || _hdrcnt == "4" || _hdrcnt == "5" || _hdrcnt == "6" || _hdrcnt == "7" || _hdrcnt == "8" || _hdrcnt == "11" || _hdrcnt == "13" || _hdrcnt == "14" || _hdrcnt == "17" || _hdrcnt == "19" || _hdrcnt == "22" || _hdrcnt == "25" || _hdrcnt == "27" || _hdrcnt == "28" || _hdrcnt == "30" || _hdrcnt == "31" | _hdrcnt == "32" || _hdrcnt == "33" || _hdrcnt == "35" || _hdrcnt == "36" || _hdrcnt == "37" || _hdrcnt == "38" || _hdrcnt == "41" || _hdrcnt == "40" || _hdrcnt == "43" || _hdrcnt == "44" || _hdrcnt == "45" || _hdrcnt == "46" || _hdrcnt == "47" || _hdrcnt == "48" || _hdrcnt == "49" || _hdrcnt == "50" || _hdrcnt == "51" || _hdrcnt == "52" || _hdrcnt == "53" || _hdrcnt == "56" || _hdrcnt == "57" || _hdrcnt == "58" || _hdrcnt == "59" || _hdrcnt == "60"
                            || _hdrcnt == "63" || _hdrcnt == "64" || _hdrcnt == "65" || _hdrcnt == "66") {
                        }
                        else {
                            if (_Cnttd == 6) {
                                _Cnttd = 0;
                            }
                            _Cnttd = _Cnttd + 1;
                            if (_hdrcnt == "2" || _hdrcnt == "3" || _hdrcnt == "10" || _hdrcnt == "12" || _hdrcnt == "21") {
                                _chkarrylst += "," + _hdrcnt;
                                $('#tr_' + _Cnttd + '').append("<td  ID=" + _hdrcnt + "><label class='toggle-vis' data-column=" + _hdrcnt + "><input type='checkbox' name=" + columnNames[i] + " class='toggle-vis' data-column=" + _hdrcnt + " ID=chk_" + _hdrcnt + " checked><span> " + columnNames[i] + " </span></label></td>");
                            }
                            else {
                                $('#tr_' + _Cnttd + '').append("<td  ID=" + _hdrcnt + "><label class='toggle-vis' data-column=" + _hdrcnt + "><input type='checkbox' name=" + columnNames[i] + " class='toggle-vis' data-column=" + _hdrcnt + " ID=chk_" + _hdrcnt + "><span> " + columnNames[i] + " </span></label></td>");
                            }
                            
                        }
                    }
                    _hdrcnt = _hdrcnt + 1;
                }
                // Check if we need server-side processing
                for (var i in columnNames) {
                    if (columnNames[i] === "EmpMNo") {
                        columns.push({
                            data: columnNames[i],
                            title: columnNames[i],
                            render: function (data, type, full, meta) {
                                if (type === 'display' && data != null && data.trim() !== '') {
                                    return astMobileno(data);
                                }
                                return data;
                            }
                        });
                    }
                    else if (columnNames[i] === "Email") {
                        columns.push({
                            data: columnNames[i],
                            title: columnNames[i],
                            render: function (data, type, full, meta) {
                                if (type === 'display' && data != null && data.trim() !== '') {
                                    return obfuscateEmail(data);
                                }
                                return data;
                            }
                        });
                    }
                    else {
                        columns.push({
                            data: columnNames[i],
                            title: columnNames[i]
                        });
                    }
                }
                _isServerSide = totalRecordCount > DataGridParameterObject.DataGridThreSold;
                //_isServerSide = 25000 > 20000;
                if (DataGridParameterObject.DataGridThreSold > totalRecordCount) {
                    $('.custom-search-wrapper').show();
                    $('.dataTables_filter').hide(); // Hide the default filter
                }
                else {
                    // Show default DataTable filter if total records <= 20k
                    $('.dataTables_filter').show();
                    $('.custom-search-wrapper').hide();
                }
                // DataTable configuration
                var columnDefs = getColumnDefs(DataGridParameterObject.RoleId, isedit, columnNames);

                commonConfig =
                {
                    draw: page,
                    "destroy": true,
                    dom: _domColumn,
                    language: _languageColumn,
                    columnDefs: columnDefs,
                    buttons: _isServerSide ? [] :
                        [
                            {
                                extend: 'excelHtml5',
                                text: 'Excel',
                                titleAttr: 'Excel',
                                exportOptions: {
                                    columns: [2, 3, 6, 7, 8, 9, 10, 12, 15, 16, 18, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 31, 36, 39, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60]
                                }
                            }
                        ],
                    lengthmenu: [
                        [10, 25, 50, 100, 500, 700, 1000],
                        [10, 25, 50, 100, 500, 700, 1000],
                    ],
                    "data": data,
                    "columns": columns,
                    pageLength: 10,
                    responsive: true,
                    autoWidth: false,

                    initComplete: function () {
                        if (_isServerSide) {



                            if ($('.dt-buttons #Export').length === 0) {
                                // Clear existing dt-buttons content to avoid duplication (if necessary)
                                $('.dt-buttons').empty();
                                // Append custom export button for server-side processing
                                const exportButton = '<button id="Export" class="dt-button buttons-excel buttons-html5" title="Excel"><span>Excel</span></button>';
                                //const exportButton = '<button type="button" id="Export" class="dwicon_btn tooltips" data-toggle="tooltip" data-placement="top" data-original-title="Download Excel"><i class="fa-regular fa-file-excel"></i></button>';
                                $(".dt-buttons").append(exportButton);

                                // Initialize tooltips (this is initialized before the button is appended)
                                $('[data-toggle="tooltip"]').tooltip();  // Potential issue: tooltip may not apply to the new button

                                // Add event listener for the custom button
                                $('#Export').on('click', function () {
                                    toastr.info("Preparing the Excel file for download...");
                                    setTimeout(function () {
                                        GenerateEmployeeExcelFile(MainObjFilter, RoleWiseFilter);
                                    }, 1000);
                                });

                                injectExportButtonsForDataTables(_isServerSide, function () {
                                    GenerateEmployeeExcelFile(MainObjFilter, RoleWiseFilter);
                                });
                                $('.tooltips').tooltip();
                            }
                        }
                        else {
                            if ($('.dt-buttons #Export').length === 0) {
                                // Clear existing dt-buttons content to avoid duplication (if necessary)
                                $('.dt-buttons').empty();
                                // Append custom export button for server-side processing
                                // const exportButton = '<button type="button" id="Export" class="dwicon_btn tooltips" data-toggle="tooltip" data-placement="top" data-original-title="Download Excel"><i class="fa-regular fa-file-excel"></i></button>';
                                const exportButton = '<button id="Export" class="dt-button buttons-excel buttons-html5" title="Excel"><span>Excel</span></button>';
                                $(".dt-buttons").append(exportButton);

                                // Initialize tooltips (this is initialized before the button is appended)
                                $('[data-toggle="tooltip"]').tooltip();  // Potential issue: tooltip may not apply to the new button

                                // Add event listener for the custom button
                                $('#Export').on('click', function () {
                                    toastr.info("Preparing the Excel file for download...");
                                    setTimeout(function () {
                                        GenerateEmployeeExcelFile(MainObjFilter, RoleWiseFilter);
                                    }, 1000);
                                });
                                injectExportButtonsForDataTables(_isServerSide, function () {
                                    GenerateEmployeeExcelFile(MainObjFilter, RoleWiseFilter);
                                });
                                $('.tooltips').tooltip();
                            }
                        }
                        initTableCustomization(DataGridParameterObject.NewPlanID, DataGridParameterObject.RoleId, columnNames, _num, _numArray, this);
                    },
                };
                // If server-side processing is needed, adjust the AJAX configuration               
                if (_isServerSide) {
                    commonConfig.searching = true,
                        commonConfig.ordering = true,
                        commonConfig.paging = true,
                        commonConfig.serverSide = true; // Enable server-side processing
                    commonConfig.ajax =
                    {
                        url: '/DataGridOptimize/GetPaginatedEmployees',
                        type: 'POST',
                        contentType: "application/json",
                        headers: { 'Authorization': '@_tokan' },
                        beforeSend: function () {
                            $("#ajax_loaderbranch").show();
                        },
                        data: function (d) {
                            //let combinedSearchValue = '';
                            searchCriteria = [];
                            var field = '';
                            var value = '';
                            var validSearch = false;
                            var hasSearch = false;
                            $('#tblEmployee thead .search_input').each(function () {
                                const columnName = $(this).data('column'); // Use data attribute for column mapping
                                const columnValue = $(this).val().trim();

                                if (columnValue) {
                                    //combinedSearchValue += `${columnName}:${columnValue};`;
                                    field += `${columnName}`;
                                    value += `${columnValue}`;
                                }
                                if (columnValue !== "" && value) {
                                    searchCriteria.push({ field: columnName, value: columnValue });
                                }
                            });

                            const requestData =
                            {
                                draw: d.draw,
                                search: {
                                    value: d.search.value || ''
                                },
                                order: d.order,
                                columns: d.columns,
                                length: d.length,
                                start: d.start,
                                DatagridThresold: DataGridParameterObject.DataGridThreSold,
                                CustFilter: searchCriteria.length > 0 ? searchCriteria : null,
                                ExportFlage: 0,
                                EmployeeFilter: MainObjFilter, // Include the filter object correctly
                                RoleWiseFilter: RoleWiseFilter
                            };
                            return JSON.stringify(requestData);
                        },
                        dataSrc: function (json) {
                            $("#ajax_loaderbranch").hide();
                            if (json.data && json.data.Items) {
                                // Map server response to DataTables expected format
                                json.recordsTotal = json.data.totalRecords || json.data.Items.length;
                                json.recordsFiltered = json.data.totalRecords || json.data.Items.length;
                                return json.data.Items;
                            }
                            json.recordsTotal = 0;
                            json.recordsFiltered = 0;
                            return [];
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            $("#ajax_loaderbranch").hide();
                            console.error('Server-side AJAX Error:', textStatus, errorThrown);
                        },
                        complete: function () {
                            $("#ajax_loaderbranch").hide();
                            // Hide loader after request completes
                        }
                    };
                }
                else {
                    commonConfig.serverSide = false; // Disable server-side processing
                    commonConfig.ajax = null; // Use the initial data provided
                    commonConfig.data = data; // Set data directly for client-side processing
                    commonConfig.deferRender = true; // For performance with large data
                }
                $('#dataTables_tbl_header').remove();
                t = $('#tblEmployee').DataTable(commonConfig); // Initialize the DataTable
                Directory_table = t;
                $('#tblEmployee').on('draw.dt', function () {
                    $('.tooltips').tooltip();
                });
                $('.tooltips').tooltip();

                setTimeout(function () {
                    $("#" + "tblEmployee" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive"));
                    $('.tooltips').tooltip();
                }, 100);
                /////////////////For Dyanamics Custome Filte Code Hide////////////////////////////             

            }
            else {
                $("#ajax_loaderbranch").hide();
                //$('#tblEmployee').html("<thead><tr role='row'><th class='ui-state-default sorting' tabindex='0' aria-controls='tblEmployee' rowspan='1' colspan='1' style='width: 87px;' aria-label='EmpName: activate to sort column ascending'> EmpName</th><th class='ui-state-default sorting' tabindex='0' aria-controls='tblEmployee' rowspan='1' colspan='1' style='width: 102px;' aria-label='Empcode: activate to sort column ascending'>Empcode</th><th class='ui-state-default sorting' tabindex='0' aria-controls='tblEmployee' rowspan='1' colspan='1' style='width: 135px;' aria-label='EmpPunchID: activate to sort column ascending'>EmpPunchID</th><th class='ui-state-default sorting' tabindex='0' aria-controls='tblEmployee' rowspan='1' colspan='1' style='width: 135px;' aria-label='BranchName: activate to sort column ascending'>BranchName</th><th class='ui-state-default sorting' tabindex='0' aria-controls='tblEmployee' rowspan='1' colspan='1' style='width: 177px;' aria-label='Email: activate to sort column ascending'>Email</th><th class='ui-state-default sorting' tabindex='0' aria-controls='tblEmployee' rowspan='1' colspan='1' style='width: 76px;' aria-label='Status: activate to sort column ascending'>Status</th><th class='ui-state-default sorting' tabindex='0' aria-controls='tblEmployee' rowspan='1' colspan='1' style='width: 77px;' aria-label='Action: activate to sort column ascending'>Action</th></tr></thead><tbody><tr role='row' class='odd'><td colspan='7'>No data available in table</td></tr></tbody>")
                const headerHtml = `<thead>
                        <tr role='row'>
                            <th class='ui-state-default sorting' style='width: 87px;'>EmpName</th>
                            <th class='ui-state-default sorting' style='width: 102px;'>EmpCode</th>
                            <th class='ui-state-default sorting' style='width: 135px;'>EmpPunchID</th>
                            <th class='ui-state-default sorting' style='width: 135px;'>BranchName</th>
                            <th class='ui-state-default sorting' style='width: 177px;'>Email</th>
                            <th class='ui-state-default sorting' style='width: 76px;'>Status</th>
                            <th class='ui-state-default sorting' style='width: 77px;'>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr role='row' class='odd'>
                            <td colspan='7' style="text-align: center;">No data available in table</td>
                        </tr>
                    </tbody>`;

                // Append header and footer
                $('#tblEmployee').html(headerHtml);
                const footerHtml = `
                <tr>                   
                    <th style="min-width: 150px">EmpName</th>
                    <th style="min-width: 100px">Emp Code</th>
                    <th style="min-width: 80px">EmpPunchID</th>
                    <th style="min-width: 80px">BranchName</th>
                    <th style="min-width: 80px">Email</th>                  
                    <th></th>
                    <th></th>
                </tr>`;
                $("#tblEmployee thead").append(footerHtml);
                $("#tblEmployee thead tr:eq(1) th").each(function (index) {
                    var title = $(this).text();
                    if (["EmpName", "Emp Code", "EmpPunchID", "BranchName", "Email", "Status"].includes(title)) {
                        $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search" id=""></i>`);
                    }
                    else if (index == 0) {
                        $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search" id=""></i>`);
                    }
                    else {
                        $(this).html('');
                    }
                });
            }

        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('AJAX Error:', textStatus, errorThrown);
            console.error('Response:', jqXHR.responseText); // Log the response text for debugging
        },
        complete: function () {
            $("#ajax_loaderbranch").hide(); // Hide the loader
        }
    });
}
