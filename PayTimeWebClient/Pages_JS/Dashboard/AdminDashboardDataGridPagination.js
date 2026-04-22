const { debug } = require("node:util");

let searchTimeout;
var globalDataObj;
var globalSearchCriteria;
function LoadDeviceMasterGridData(page, mainObj, selector, type) {
    let DateFormate = window.AppConfig.DateFormate == "yyyy-M-dd" ? "YYYY-MMM-DD" : window.AppConfig.DateFormate == "M-dd-yyyy" ? "MMM-DD-YYYY" : window.AppConfig.DateFormate == "dd-M-yyyy" ? "DD-MMM-YYYY" : window.AppConfig.DateFormate;
    if ($.fn.DataTable.isDataTable(selector)) {
        $(selector).DataTable().clear().destroy();
        $('#dataTables_tbl_header').remove();
    }
    var headerId = ".custom_dataTableHeader";
    $(selector + "_wrapper").find(headerId).remove();
    $('#dataTables_tbl_header').remove();
    
    
    var _newURL = mainObj.webapiurl;
    let GridColumns = [];
    switch (type) {
        case 1: //checked in
            GridColumns = [
                { "data": "Empcode" },
                { "data": "EmpName" },
                { "data": "emppunchid" },
                { "data": "departmentname" },
                { "data": "designationname" },
                { "data": "ShiftName" },
                { "data": "Mode", "orderable": false },
                { "data": "InTime" },
                {
                    "data": "currentdate",
                    "render": function (data, type, row) {
                        if (data != "" || data != null || DateFormate != "") {
                            return moment(data).format(DateFormate.toUpperCase());
                        }
                        else {
                            return "";
                        }
                    }
                }
            ]
            break;
        case 3: //Late in
            GridColumns = [
                { "data": "EmployeeID" },
                { "data": "EmpName" },
                { "data": "EmpPunchID" },
                { "data": "DepartmentName" },
                { "data": "DesignationName" },
                { "data": "shiftname" },
                { "data": "ShiftStartTime", "orderable": false },
                { "data": "ShiftEndTime", "orderable": false },
                { "data": "SubqueryInTime", "orderable": false },
                { "data": "LateInMins", "orderable": false },
                {
                    "data": "currentdate",
                    "render": function (data, type, row) {
                        if (data != "" || data != null || DateFormate != "") {
                            return moment(data).format(DateFormate.toUpperCase());
                        }
                        else {
                            return "";
                        }
                    }
                }
            ]
            break;
        case 4: //Early Out
            GridColumns = [
                { "data": "EmployeeID" },
                { "data": "EmpName" },
                { "data": "EmpPunchID" },
                { "data": "DepartmentName" },
                { "data": "DesignationName" },
                { "data": "shiftname" },
                { "data": "ShiftStartTime", "orderable": false },
                { "data": "ShiftEndTime", "orderable": false },
                { "data": "OutTime", "orderable": false },
                { "data": "EarlyOutMin", "orderable": false },
                {
                    "data": "currentdate",
                    "render": function (data, type, row) {
                        if (data != "" || data != null || DateFormate != "") {
                            return moment(data).format(DateFormate.toUpperCase());
                        }
                        else {
                            return "";
                        }
                    }
                }
            ]
            break;
        case 6: //Not checked in
            GridColumns = [
                { "data": "Empcode" },
                { "data": "EmpName" },
                { "data": "emppunchid" },
                { "data": "departmentname" },
                { "data": "designationname" },
                { "data": "ShiftName" },
                {
                    "data": null,
                    "render": function () {
                        return "A";
                    }
                },
                {
                    "data": "currentdate",
                    "render": function (data, type, row) {

                        if (data != "" || data != null || DateFormate != "") {
                            return moment(data).format(DateFormate.toUpperCase());
                        }
                        else {
                            return "";
                        }
                    }
                }
            ]
            break;
        default:
            GridColumns = [];
            break;

    }

    var Obj =
    {
        "searchTerm": $('#searchTerm').val() || "",
        "sortColumn": "",  // Default sort column
        "sortOrder": "",     // Default sort order
        "pageSize": mainObj.pageSize,
        "page": 1,
        "DataGridValues": mainObj.DataGridValues,
        "BranchID": mainObj.BranchID,
        "CompanyID": mainObj.CompanyID,
        "ReqID": type
    };
    globalDataObj = mainObj;
    //mainObj.DataGridValues = 50;
    $.ajax({
        type: "POST",
        headers: { 'Authorization': mainObj.tokan },
        url: _newURL + 'DataGridOptimize/AdminDashBoardPaginatList',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 30000,
        data: JSON.stringify(Obj),
        beforeSend: function () {
            //$("#loadernotcheking").show();
            const loaderId = getLoaderIdByType(type);
            if (loaderId) $(loaderId).show();
        },
        success: function (response) {
            if (response != null || response != undefined || response != []) {
                /*  * $("#loadernotcheking").hide();*/
                let columnOrder = "ASC";
                let column = "EmpId";
                var totalRecordCount = response.Table1[0].TotalRecords;
                var data = response.Table;
                var _isServerSide = totalRecordCount > Number(mainObj.DataGridValues);
                const loaderId = getLoaderIdByType(type);
                if (loaderId) $(loaderId).hide();
                commonConfig =
                {
                    draw: page,
                    recordsTotal: totalRecordCount,
                    recordsFiltered: totalRecordCount,
                    destroy: true,
                    //"dom": 'lBfrtip',

                    dom: _domCommon,
                    language: _languageCommon,
                    buttons: _isServerSide ? [] : [
                        {
                            extend: 'excelHtml5',
                            text: 'Excel',
                            titleAttr: 'Excel',
                            exportOptions: {
                                columns: [5, 6, 7, 8, 10, 14, 16, 17]
                            }
                        }],
                    lengthMenu: [
                        [10, 25, 50, 100],
                        [10, 25, 50, 100],
                    ],
                    data: data,
                    columns: GridColumns,
                    pageLength: 10,
                    responsive: true,
                    autoWidth: false,
                    //rowCallback: function (row, data, index) {
                    //    var table = $(selector).DataTable();
                    //    var pageInfo = table.page.info();
                    //    var rowIndex = pageInfo.start + index + 1; // Dynamic Sr No based on pagination
                    //    $('td:eq(0)', row).html(rowIndex);
                    //},
                    initComplete: function (settings, json) {

                        // if (_isServerSide) {
                        //     if ($(`${selector}_wrapper .dt-buttons #Export`).length === 0) {
                        // Clear existing dt-buttons content to avoid duplication (if necessary)
                        //       $('.dt-buttons').empty();
                        // Append custom export button for server-side processing
                        //      const exportButton = '<button id="Export" class="dt-button buttons-excel buttons-html5" title="Excel"><span>Excel</span></button>';
                        //const exportButton = '<button type="button" id="Export" class="dwicon_btn tooltips" data-toggle="tooltip" data-placement="top" data-original-title="Download Excel"><i class="fa-light fa-file-excel"></i></button>';
                        //     $(`${selector}_wrapper .dt-buttons`).append(exportButton);

                        // Initialize tooltips (this is initialized before the button is appended)
                        //     $('[data-toggle="tooltip"]').tooltip();  // Potential issue: tooltip may not apply to the new button

                        // Add event listener for the custom button
                        //     $(`${selector}_wrapper .dt-buttons #Export`).on('click', function () {
                        //        toastr.info("Preparing the Excel file for download...");

                        //       setTimeout(function () {
                        //          let searchTerm = $(`${selector}_filter  input`).val();
                        //         generateExcelFile(mainObj, type, searchTerm, columnOrder, column);
                        //     }, 1000);

                        //  });
                        //  }
                        //   }
                        //  else {
                        //    if ($(`${selector}_wrapper .dt-buttons #Export`).length === 0) {
                        // Clear existing dt-buttons content to avoid duplication (if necessary)
                        //      $('.dt-buttons').empty();
                        // Append custom export button for server-side processing
                        //      const exportButton = '<button id="Export" class="dt-button buttons-excel buttons-html5" title="Excel"><span>Excel</span></button>';
                        //const exportButton = '<button type="button" id="Export" class="dwicon_btn tooltips" data-toggle="tooltip" data-placement="top" data-original-title="Download Excel"><i class="fa-light fa-file-excel"></i></button>';
                        //    $(`${selector}_wrapper .dt-buttons`).append(exportButton);

                        // Initialize tooltips (this is initialized before the button is appended)
                        //      $('[data-toggle="tooltip"]').tooltip();  // Potential issue: tooltip may not apply to the new button

                        // Add event listener for the custom button
                        //     $(`${selector}_wrapper .dt-buttons #Export`).on('click', function () {
                        //         toastr.info("Preparing the Excel file for download...");

                        injectExportButtonsForDataTables(_isServerSide, function () {
                            //generateExcelFile(mainObj, type, searchTerm, columnOrder, column);
                            setTimeout(function () {
                                let searchTerm = $(`${selector}_filter  input`).val();
                                generateExcelFile(mainObj, type, searchTerm, columnOrder, column);
                            }, 1000);
                           

                        });
                        $('.tooltips').tooltip();
                    }
                }




                if (_isServerSide) {


                    commonConfig.searching = true,
                        commonConfig.ordering = true,
                        commonConfig.paging = true,
                        commonConfig.serverSide = true;
                    commonConfig.processing = false;
                    // Enable server-side processing
                    //commonConfig.searchDelay = 1000;  // Enable server-side processing

                    commonConfig.ajax =
                    {
                        url: '/DataGridOptimize/GetPaginatedAdminDashBoard',
                        type: 'POST',
                        contentType: "application/json",
                        beforeSend: function () {
                            const loaderId = getLoaderIdByType(type);
                            if (loaderId) $(loaderId).show();
                        },
                        data: function (d) {
                            columnOrder = d.order;
                            column = d.columns;
                            const requestData =
                            {

                                DatagridThresold: mainObj.DataGridValues,
                                draw: d.draw,
                                search: {
                                    value: d.search.value || ''
                                },
                                order: d.order,
                                columns: d.columns,
                                length: d.length,
                                start: d.start,
                                CompanyID: mainObj.CompanyID,
                                BranchID: mainObj.BranchID,
                                ReqID: type
                            };
                            return JSON.stringify(requestData);
                        },
                        dataSrc: function (json) {
                            if (json.data) {
                                const loaderId = getLoaderIdByType(type);
                                if (loaderId) $(loaderId).hide();
                                return json.data; // Return the data array if it exists
                            }
                            return [];
                        }
                    };
                }
                else {
                    commonConfig.serverSide = false; // Disable server-side processing
                    commonConfig.ajax = null; // Use the initial data provided
                    commonConfig.data = data; // Set data directly for client-side processing
                    commonConfig.deferRender = true; // For performance with large data
                }
                t = $(selector).DataTable(commonConfig);

                //var customDiv = $('<div class="customforms_table"></div>');
                // customDiv.insertBefore(`${selector}_wrapper > .dataTables_info`);
                //$(`${selector}_wrapper  > table`).appendTo(customDiv);

                let searchTimeout;
                $(`${selector}_filter  input`)
                    .unbind() // Unbind previous default bindings
                    .bind("input", function (e) { // Bind our desired behavior
                        clearTimeout(searchTimeout); // Clear previous debounce timer
                        var searchValue = this.value.trim();
                        searchTimeout = setTimeout(function () {
                            t.search(searchValue).draw(); // Perform search
                        }, 800);
                    });

                $(selector).on('draw.dt', function () {
                    $('.tooltips').tooltip();
                });
                $('.tooltips').tooltip();
            }
            if (selector == '#checkin_table') {
                setTimeout(function () {
                    // Remove any existing header with same ID before inserting
                    /* $('#dataTables_tbl_header').remove();*/

                    // Move the header (from DataTables wrapper) before your custom container
                    $(selector + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive_checkIn"));

                    $('.tooltips').tooltip();
                }, 100);
            } else if (selector == '#notcheckedin_tbl') {
                setTimeout(function () {
                    // Remove any existing header with same ID before inserting
                    /*  $('#dataTables_tbl_header').remove();*/

                    $(selector + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive_notcheckedin"));

                    $('.tooltips').tooltip();
                }, 100);
            } else if (selector == '#lateclockin_tbl') {
                setTimeout(function () {
                    // Remove any existing header with same ID before inserting
                    /*  $('#dataTables_tbl_header').remove();*/

                    $(selector + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive_lateclockin"));

                    $('.tooltips').tooltip();
                }, 100);
            } else if (selector == '#earlyclockout_tbl') {
                setTimeout(function () {
                    // Remove any existing header with same ID before inserting
                    /*  $('#dataTables_tbl_header').remove();*/

                    $(selector + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive_earlyclockout"));

                    $('.tooltips').tooltip();
                }, 100);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (loaderId) $(loaderId).hide();
            console.error('AJAX Error:', textStatus, errorThrown);
            console.error('Response:', jqXHR.responseText); // Log the response text for debugging
        },
        complete: function () {
            const loaderId = getLoaderIdByType(type);
            if (loaderId) $(loaderId).hide();
        }
    });

}

function generateExcelFile(mainObj, type, searchTerm, columnorder, column) {
    if (globalDataObj != null) {
        var Obj =
        {     // Default sort order
            "pageSize": mainObj.pageSize,
            "page": 1,
            "DatagridThresold": mainObj.DataGridValues,
            "BranchId": mainObj.BranchID,
            "CompanyId": mainObj.CompanyID,
            "ReqID": type,
            "ExportFlage": 1,
            "search": {
                value: searchTerm || ""
            },
            "order": columnorder,
            "columns": column
        };
        $.ajax({
            url: '/DataGridOptimize/GetAllAdminDashBoardExcelData',
            type: 'POST',
            data: JSON.stringify(Obj),
            contentType: 'application/json',
            success: function (response) {
                if (response.success && response.filePath) {
                    /* toastr.success("Export completed. Starting download...");*/
                    // Trigger the download using the returned file path
                    window.location.href = response.filePath;
                } else {
                    toastr.error(response.message || "Failed to generate export file.");
                }

            },
            error: function () {
                toastr.error("An error occurred while exporting data.");
            }
        });
    }
}

function getLoaderIdByType(type) {
    switch (type) {
        case 1: return "#loader_checkin";
        case 3: return "#loader_lateclockin";
        case 4: return "#loader_earlyclockout";
        case 6: return "#loader_notcheckedin";
        default: return null;
    }
}