let searchTimeout;
var globalDataObj;
var globalSearchCriteria;
var GlobalServerSide = false;
function LoadAttendanceDetialsGridData(mainObj) {
    if ($.fn.DataTable.isDataTable('#tblAttendanceSummary')) {
        $('#tblAttendanceSummary').DataTable().destroy();


    }
    $("#tblAttendanceSummary").destroy;
    $('#dataTables_tbl_header').remove();
    var _newURL = mainObj.webapiurl;
    var columns = [];

    var _columnsname = "";
    var _num = "";
    var P = 4;
    var _numArray = [];
    var searchCriteria = [];

    var Obj =
    {
        searchTerm: $('#searchTerm').val() || "",
        sortColumn: "",  // Default sort column
        sortOrder: "asc",     // Default sort order
        pageSize: mainObj.pageSize,
        page: 1,
        DataGridValues: mainObj.DataGridValues,
        RoleID: mainObj.RoleID,
        loginemployeeid: mainObj.loginemployeeid,
        Month: mainObj.Month,
        Year: mainObj.Year,

        status: mainObj.status,
        Employee: mainObj.Employee,
        SelectAll: mainObj.SelectAll,
        SelectAllSearchTerm: mainObj.SelectAllSearchTerm,
        CustomFilters: searchCriteria.length > 0 ? searchCriteria : []
    };
    globalDataObj = mainObj;
    $.ajax({
        type: "POST",
        headers: { 'Authorization': mainObj.tokan },
        url: _newURL + 'DataGridOptimize/AttendaceSummaryPaginatList',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 30000,
        data: JSON.stringify(Obj),
        beforeSend: function () {
            $("#ajax_loader").show();
        },
        success: function (response) {
            if (response.Table.length == 0) {
                const wrapper = $(".filter_portlet_wrapper");
                wrapper.removeClass("full-width");
            }
            if (response != null && response.Table != null) {
                const list = response.Table;
                handleGridVisibility("tblempty", "NotificationGrid-empty", list, {
                    imageUrl: "emptyscreen_3.png",
                    subText: "No matching records found for the selected filters."
                });
            }
            if (data == "") {
                window.setTimeout(function () {
                    toastr.remove();
                    toastr.error("No entry! This is a restricted area, even for ninjas like you.");
                    window.setTimeout(function () {
                        window.location.href = "/PayTime/LoginPage";
                    }, 2000);
                    return false;
                }, 1000);
                $('#no_data').show();
                $("#tblAttendanceSummary").hide();
                $("#tblAttndancelist").hide();
                $("#tblAttendanceSummarylst").hide();

            }
            if (response.Table.length == 0) {
                $("#ajax_loader").hide();
                toastr.error("No data available for the current selection.");
                return;
            }
            if (response != null || response != undefined || response != []) {
                $("#ajax_loader").hide();
                var totalRecordCount = response.Table1[0].TotalRecords;
                var data = response.Table;
                columnNames = Object.keys(data[0]);
                //_num = "0,1,3,4,5,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24";

                //_numArray = _num.split(',').map(Number);

                var _isServerSide = totalRecordCount > Number(mainObj.DataGridValues);
                GlobalServerSide = _isServerSide;
                $("#tblAttendanceSummarylst").show();
                commonConfig =
                {
                    draw: 1,
                    recordsTotal: totalRecordCount,
                    recordsFiltered: totalRecordCount,
                    "destroy": true,
                    //dom: "<'row'<'col-lg-3 col-sm-5 col-xs-12'<'record_btn_main'l,><'dt-buttons'Bf>><'col-lg-9 col-sm-7 col-xs-12 mb_margin_bottom_15'<'resignblock text-right'>>>" +
                    //    "<'row'<'col-sm-12 margin-bottom-10'<'customforms_table'tr'>>>" +
                    //    "<'row'<'col-md-5 col-sm-5'i><'col-md-7 col-sm-7'p>>",
                    /* dom: 'lBfrtip',*/
                    dom: _domColumn,
                    language: _languageColumn,
                    "buttons": _isServerSide ? [] :
                        [
                            {
                                extend: 'excelHtml5',
                                text: 'Excel',
                                titleAttr: 'Excel',
                                exportOptions: {
                                    columns: '@Actcode' === "MAN5F0F" ? [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12] : [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]
                                }
                            }],
                    lengthMenu: [
                        [10, 25, 50, 100],
                        [10, 25, 50, 100],
                    ],
                    "data": data,
                    "columns": [
                        { "title": "Sr. No", data: null, orderable: null, visible: false },
                        {
                            "title": "Name", "data": "EmpName",
                            "createdCell": function (td) {
                                $(td).css({ "min-width": "120px" });
                            },
                        },
                        { "title": "Empcode", "data": "Empcode" },
                        { "title": "Shift", "data": "ShiftName" },
                        { "title": "Present", "data": "PresentCount" },
                        { "title": "Absent", "data": "AbsentCount" },
                        { "title": "W", "data": "WeekoffCount" },
                        { "title": "H", "data": "HolidayCount" },
                        { "title": "Leave", "data": "ODCount" },
                        { "title": "Total Hrs", "data": "TotalHours" },
                        { "title": "OT Hrs", "data": "OTHr" },
                        {
                            title: "Details",
                            orderable: false,
                            render: function (data, type, row) {
                                return '<a href="javascript:;" style="cursor: pointer;" class="btn seagreen_btnnew tooltips" data-placement="top" data-original-title="View" onclick="GetAttndancelist(' + row.EmpID + ')"><i class="fa-regular fa-eye"></i></span>';
                            }
                        },
                    ],
                    "columnDefs": [
                        {
                            "targets": [1],
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
                               <img alt="" class="user_img img-circle tooltips" data-placement="right" data-original-title="Empcode : ${row.Empcode}" src="/UploadEmpPhoto/${row.EmpPhoto}" onerror="this.style.display='none'; this.nextElementSibling.style.display='inline-block';"/>
                                    <span class="tooltips" data-placement="right" data-original-title="Empcode : ${row.Empcode}"  style="display: none; min-width: 30px; height: 30px; line-height: 30px; text-align: center; background-color: #ecf3ff; color:  ${randomColor}; border-radius: 50% !important; border: 1px solid #8ab1f7; margin-right: 8px;">
                                    ${initials}</span><span>${row.EmpName}</span></div>`;

                                return data;
                            }
                        }
                    ],
                    pageLength: 10,
                    responsive: true,
                    autoWidth: false,
                    //rowCallback: function (row, data, index) {
                    //    var table = $('#tblAttendanceSummary').DataTable();
                    //    var pageInfo = table.page.info();
                    //    var rowIndex = pageInfo.start + index + 1; // Dynamic Sr No based on pagination
                    //    $('td:eq(0)', row).html(rowIndex);
                    //},
                    initComplete: function (settings, json) {

                        if (_isServerSide) {
                            if ($('.dt-buttons #Export').length === 0) {
                                // Clear existing dt-buttons content to avoid duplication (if necessary)
                                $('.dt-buttons').empty();
                                // Append custom export button for server-side processing
                                const exportButton = '<button id="Export" class="dt-button buttons-excel buttons-html5" title="Excel"><span>Excel</span></button>';
                                //const exportButton = '<button type="button" id="Export" class="dwicon_btn tooltips" data-toggle="tooltip" data-placement="top" data-original-title="Download Excel"><i class="fa-light fa-file-excel"></i></button>';
                                $(".dt-buttons").append(exportButton);

                                // Initialize tooltips (this is initialized before the button is appended)
                                $('[data-toggle="tooltip"]').tooltip();  // Potential issue: tooltip may not apply to the new button

                                // Add event listener for the custom button
                                $('#Export').on('click', function () {
                                    toastr.info("Preparing the Excel file for download...");

                                    setTimeout(function () {
                                        generateAttendanceDetailExcelFile();
                                    }, 1000);

                                });
                            }
                        }
                        else {
                            if ($('.dt-buttons #Export').length === 0) {
                                // Clear existing dt-buttons content to avoid duplication (if necessary)
                                $('.dt-buttons').empty();
                                // Append custom export button for server-side processing
                                const exportButton = '<button id="Export" class="dt-button buttons-excel buttons-html5" title="Excel"><span>Excel</span></button>';
                                //const exportButton = '<button type="button" id="Export" class="dwicon_btn tooltips" data-toggle="tooltip" data-placement="top" data-original-title="Download Excel"><i class="fa-light fa-file-excel"></i></button>';
                                $(".dt-buttons").append(exportButton);

                                // Initialize tooltips (this is initialized before the button is appended)
                                $('[data-toggle="tooltip"]').tooltip();  // Potential issue: tooltip may not apply to the new button

                                // Add event listener for the custom button
                                $('#Export').on('click', function () {
                                    toastr.info("Preparing the Excel file for download...");

                                    setTimeout(function () {
                                        generateAttendanceDetailExcelFile();
                                    }, 1000);

                                });
                            }
                        }
                        //, "Present", "Absent", "W", "H", "Leave", "Total Hrs", "OT Hrs"
                        var footer = '<tr>';
                        this.api().columns().every(function (index) {
                            if (this.visible()) { // Check if column is visible
                                var columnName = $(this.header()).text().trim();
                                footer += `<th>${columnName}</th>`;
                            }
                        });
                        footer += '</tr>';
                        $("#tblAttendanceSummary thead").append(footer);
                        $("#tblAttendanceSummary thead tr:eq(1) th").each(function (index) {
                            var title = $(this).text();
                            if (["Name", "Empcode", "Shift"].includes(title)) {
                                $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" />
                                    <i class="clm-search" id=""></i>`);
                            }
                            else {
                                // If it's not one of the desired columns, you can leave it as is or apply a different action
                                $(this).html('');  // For example, you can leave the column header unchanged
                            }
                        });


                        //var _exportflage = "AttendanceMaster1";
                        //injectExportButtonsForDataTablesMaster(_isServerSide, function () {
                        //    // Your export logic here
                        //    generateAttendanceDetailExcelFile();
                        //}, _exportflage);

                        let debounceTimer;
                        // Apply the Search all Columns
                        $("#tblAttendanceSummary thead").on("input", "input", function () {
                            clearTimeout(debounceTimer);
                            debounceTimer = setTimeout(() => {
                                var columnIndex = $(this).parent().index();
                                var visibleColumns = Directory_table.columns(':visible').indexes().toArray();
                                var actualIndex = visibleColumns[columnIndex];

                                Directory_table.column(actualIndex).search(this.value).draw();
                            }, 500);
                        });
                    }
                }
                if (_isServerSide) {

                    commonConfig.searching = true,
                        commonConfig.ordering = true,
                        commonConfig.paging = true,
                        commonConfig.serverSide = true;
                    commonConfig.processing = true;

                    // Enable server-side processing
                    //commonConfig.searchDelay = 1000;  // Enable server-side processing

                    commonConfig.ajax =
                    {
                        url: '/DataGridOptimize/GetPaginatedAttendanceSummary',
                        type: 'POST',
                        contentType: "application/json",

                        data: function (d) {
                            //let combinedSearchValue = '';
                            searchCriteria = [];
                            var field = '';
                            var value = '';

                            $('#tblAttendanceSummary thead .search_input').each(function () {
                                const columnName = $(this).data('column'); // Use data attribute for column mapping
                                const columnValue = $(this).val().trim();
                                if (columnValue) {
                                    //combinedSearchValue += `${columnName}:${columnValue};`;
                                    field += `${columnName}`;
                                    value += `${columnValue}`;
                                }
                                if (columnValue !== "" && value) {
                                    searchCriteria.push({ field: columnName, value: columnValue });
                                    /*  searchCriteria.push({ field, value });*/
                                }
                            });
                            globalSearchCriteria = searchCriteria.length > 0 ? searchCriteria : null;
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
                                CustFilter: searchCriteria.length > 0 ? searchCriteria : null,
                                RoleID: mainObj.RoleID,
                                loginemployeeid: mainObj.loginemployeeid,
                                Month: mainObj.Month,
                                Year: mainObj.Year,

                                status: mainObj.status,
                                Employee: mainObj.Employee,
                                SelectAll: mainObj.SelectAll,
                                SelectAllSearchTerm: mainObj.SelectAllSearchTerm
                            };
                            return JSON.stringify(requestData);
                        },
                        dataSrc: function (json) {
                            if (json.data) {
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
                $("#dataTables_tbl_header").remove();
                t = $('#tblAttendanceSummary').DataTable(commonConfig);

                $("#tblAttendanceSummary_filter  input")
                    .unbind() // Unbind previous default bindings
                    .bind("input", function (e) { // Bind our desired behavior
                        clearTimeout(searchTimeout); // Clear previous debounce timer
                        var searchValue = this.value.trim();
                        searchTimeout = setTimeout(function () {
                            t.search(searchValue).draw(); // Perform search
                        }, 1000);
                    });
                Directory_table = t;
                $('#tblAttendanceSummary').on('draw.dt', function () {
                    $('.tooltips').tooltip();
                });
                $('.tooltips').tooltip();

                setTimeout(function () {
                    $("#" + "tblAttendanceSummary" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive"));
                    $('.tooltips').tooltip();

                    var _exportflage = "AttendanceMaster1";
                    injectExportButtonsForDataTablesMaster(_isServerSide, function () {
                        // Your export logic here
                        generateAttendanceDetailExcelFile();
                    }, _exportflage);

                }, 100);
            }
            else {

            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('AJAX Error:', textStatus, errorThrown);
            console.error('Response:', jqXHR.responseText); // Log the response text for debugging
        },
        complete: function () {
            $("#ajax_loader").hide(); // Hide the loader
        }
    });

}

function generateAttendanceDetailExcelFile() {
    if (globalDataObj != null) {

        if (!GlobalServerSide) {
            var searchCriteria = [];
            var field = '';
            var value = '';

            $('#tblAttendanceSummary thead .search_input').each(function () {
                const columnName = $(this).data('column'); // Use data attribute for column mapping
                const columnValue = $(this).val().trim();
                if (columnValue) {
                    //combinedSearchValue += `${columnName}:${columnValue};`;
                    field += `${columnName}`;
                    value += `${columnValue}`;
                }
                if (columnValue !== "" && value) {
                    searchCriteria.push({ field: columnName, value: columnValue });
                    /*  searchCriteria.push({ field, value });*/
                }
            });
            globalSearchCriteria = searchCriteria.length > 0 ? searchCriteria : null;
        }

        var Obj =
        {     // Default sort order
            "pageSize": globalDataObj.pageSize,
            "page": 1,
            "DatagridThresold": globalDataObj.DataGridValues,
            RoleID: globalDataObj.RoleID,
            loginemployeeid: globalDataObj.loginemployeeid,
            Month: globalDataObj.Month,
            Year: globalDataObj.Year,

            status: globalDataObj.status,
            Employee: globalDataObj.Employee || "",
            SelectAll: globalDataObj.SelectAll,
            SelectAllSearchTerm: globalDataObj.SelectAllSearchTerm,
            "CustFilter": globalSearchCriteria,
            "ExportFlage": 1
        };
        $.ajax({
            url: '/DataGridOptimize/GetAllDeviceAttendanceSummaryExcelData',
            type: 'POST',
            data: JSON.stringify(Obj),
            contentType: 'application/json',
            beforeSend: function () {
                $("#ajax_loader").show(); // Show loader before sending the request
            },
            success: function (response) {
                if (response.success && response.filePath) {
                    // toastr.success("Export completed. Starting download...");
                    window.location.href = response.filePath; // Trigger download
                } else {
                    toastr.error(response.message || "Failed to generate export file.");
                }
            },
            error: function () {
                toastr.error("An error occurred while exporting data.");
            },
            complete: function () {
                $("#ajax_loader").hide(); // Hide loader after request completes
            }
        });

    }
}