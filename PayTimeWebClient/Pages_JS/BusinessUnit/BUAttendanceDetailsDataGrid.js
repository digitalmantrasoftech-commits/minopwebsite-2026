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
        BUId: mainObj.BUId,
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
        url: _newURL + 'BU/AttendaceSummaryBUPaginatList',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 30000,
        data: JSON.stringify(Obj),
        beforeSend: function () {
            $("#ajax_loader").show();
        },
        success: function (response) {
            if (response == null || response.Table == null || response.Table == undefined) {
                $("#ajax_loader").hide();
                toastr.error("No data available for the current selection.");
                const wrapper = $(".filter_portlet_wrapper");
                wrapper.removeClass("full-width");
                return;
            }
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

                var _isServerSide = totalRecordCount > Number(mainObj.DataGridValues);
                GlobalServerSide = _isServerSide;
                $("#tblAttendanceSummarylst").show();
                commonConfig =
                {
                    draw: 1,
                    recordsTotal: totalRecordCount,
                    recordsFiltered: totalRecordCount,
                    "destroy": true,
                    dom: _domColumn,
                    language: _languageColumn,
                    "buttons": _isServerSide ? [] :
                        [
                            {
                                extend: 'excelHtml5',
                                text: 'Excel',
                                titleAttr: 'Excel',
                                exportOptions: {
                                    columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]
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
                    initComplete: function (settings, json) {

                        if (_isServerSide) {
                            if ($('.dt-buttons #Export').length === 0) {
                                $('.dt-buttons').empty();
                                const exportButton = '<button id="Export" class="dt-button buttons-excel buttons-html5" title="Excel"><span>Excel</span></button>';
                                $(".dt-buttons").append(exportButton);
                                $('[data-toggle="tooltip"]').tooltip();
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
                                $('.dt-buttons').empty();
                                const exportButton = '<button id="Export" class="dt-button buttons-excel buttons-html5" title="Excel"><span>Excel</span></button>';
                                $(".dt-buttons").append(exportButton);
                                $('[data-toggle="tooltip"]').tooltip();
                                $('#Export').on('click', function () {
                                    toastr.info("Preparing the Excel file for download...");
                                    setTimeout(function () {
                                        generateAttendanceDetailExcelFile();
                                    }, 1000);
                                });
                            }
                        }
                        var footer = '<tr>';
                        this.api().columns().every(function (index) {
                            if (this.visible()) {
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
                                $(this).html('');
                            }
                        });

                        let debounceTimer;
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

                    commonConfig.ajax =
                    {
                        url: '/DataGridOptimize/GetPaginatedAttendanceSummaryBU',
                        type: 'POST',
                        contentType: "application/json",

                        data: function (d) {
                            searchCriteria = [];
                            var field = '';
                            var value = '';

                            $('#tblAttendanceSummary thead .search_input').each(function () {
                                const columnName = $(this).data('column');
                                const columnValue = $(this).val().trim();
                                if (columnValue) {
                                    field += `${columnName}`;
                                    value += `${columnValue}`;
                                }
                                if (columnValue !== "" && value) {
                                    searchCriteria.push({ field: columnName, value: columnValue });
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
                                BUId: mainObj.BUId,
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
                                return json.data;
                            }
                            return [];
                        }
                    };
                }
                else {
                    commonConfig.serverSide = false;
                    commonConfig.ajax = null;
                    commonConfig.data = data;
                    commonConfig.deferRender = true;
                }
                $("#dataTables_tbl_header").remove();
                t = $('#tblAttendanceSummary').DataTable(commonConfig);

                $("#tblAttendanceSummary_filter  input")
                    .unbind()
                    .bind("input", function (e) {
                        clearTimeout(searchTimeout);
                        var searchValue = this.value.trim();
                        searchTimeout = setTimeout(function () {
                            t.search(searchValue).draw();
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
                        generateAttendanceDetailExcelFile();
                    }, _exportflage);

                }, 100);
            }
            else {

            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('AJAX Error:', textStatus, errorThrown);
            console.error('Response:', jqXHR.responseText);
        },
        complete: function () {
            $("#ajax_loader").hide();
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
                const columnName = $(this).data('column');
                const columnValue = $(this).val().trim();
                if (columnValue) {
                    field += `${columnName}`;
                    value += `${columnValue}`;
                }
                if (columnValue !== "" && value) {
                    searchCriteria.push({ field: columnName, value: columnValue });
                }
            });
            globalSearchCriteria = searchCriteria.length > 0 ? searchCriteria : null;
        }

        var Obj =
        {
            "pageSize": globalDataObj.pageSize,
            "page": 1,
            "DatagridThresold": globalDataObj.DataGridValues,
            BUId: globalDataObj.BUId,
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
            url: '/DataGridOptimize/GetAllDeviceAttendanceSummaryBUExcelData',
            type: 'POST',
            data: JSON.stringify(Obj),
            contentType: 'application/json',
            beforeSend: function () {
                $("#ajax_loader").show();
            },
            success: function (response) {
                if (response.success && response.filePath) {
                    window.location.href = response.filePath;
                } else {
                    toastr.error(response.message || "Failed to generate export file.");
                }
            },
            error: function () {
                toastr.error("An error occurred while exporting data.");
            },
            complete: function () {
                $("#ajax_loader").hide();
            }
        });

    }
}
