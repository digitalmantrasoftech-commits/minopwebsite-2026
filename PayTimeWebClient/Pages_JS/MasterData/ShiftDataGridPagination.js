
function loadShiftData(page, mainobj) {

    var columnSequence = [];
    var serverSidecolumnSequence = [];

    GlobalObjData =
    {
        "searchTerm": "",
        "sortColumn": "",
        "sortOrder": "asc",
        "pageSize": 10,
        "page": 1,
        "DataGridValues": mainobj.DataGridThreSold,
        "RoleId": mainobj.RoleId,
        "CompanyID": mainobj.CompanyID,
        "BranchID": mainobj.BranchID,
        "LoginEmpId": mainobj.LoginEmpId,
        "CustomFilters": []
    }
    $.ajax({
        type: "POST",
        url: _urlNew,
        headers: { 'Authorization': token },
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(GlobalObjData),
        beforeSend: function () {
            $("#ajax_loaderbranch").show();
        },
        success: function (response) {

            if (response != null && response.Table != null) {
                const sift = response.Table;
                handleGridVisibility("tblempty", "NotificationGrid-empty", sift, {
                    imageUrl: "emptyscreen_1.png",
                    subText: "No entries found. Please add a record to get started."
                });
            }

            if (response != null || response != undefined || response != null || response != [] || response.Table.length > 0) {
                var totalRecordCount = response.Table1[0].TotalRecords;
                var data = response.Table;
                _isServerSide = totalRecordCount > mainobj.DataGridThreSold;

                var columnNames = [];

                if (response.Table.length == 0) {

                }
                else {
                    $("#fieldlst tr td").remove();
                    _chkarrylst = 0;
                    columnNames = Object.keys(response.Table[0]);                
                }
                $('#tblShift').destroy;

                if (response.Table.length > 0) {
                    commonConfig =
                    {
                        "destroy": true,
                       // dom: "<'row'<'col-lg-3 col-sm-5 col-xs-12'<'record_btn_main'l,><'dt-buttons'Bf>><'col-lg-9 col-sm-7 col-xs-12 mb_margin_bottom_15'<'resignblock text-right'>>>" +
                        //    "<'row'<'col-sm-12 margin-bottom-10'<'customforms_table 'tr'>>>" +
                        //    "<'row'<'col-md-5 col-sm-5'i><'col-md-7 col-sm-7'p>>",
                         dom: _domColumn,
                        language: _languageColumn,
                        "data": data,
                        "columns": [
                            {
                                data: null,
                                orderable: null,
                                visible: false,
                            },
                            { data: 'ShiftId', visible: false },
                            { data: 'ShiftName' },
                            { data: 'ShiftShortName' },
                            { data: 'ShiftGroupId', visible: false },
                            { data: 'StartTime' },
                            { data: 'EndTime' },
                            { data: 'GraceBefore', visible: false },
                            { data: 'GraceAfter', visible: false },
                            { data: 'ShiftDur', visible: false },
                            { data: 'MinHrsHalfDay' },
                            { data: 'MinHrsFullDay' },
                            { data: 'RecessDur', visible: false },
                            { data: 'SMSScheduleTime', visible: false },
                            { data: 'IsFlexi', visible: false },
                            { data: 'IsFlexi' },
                            //{ data: 'IsActive', visible: false },
                            { data: 'ShiftGroupName' },
                            { data: null, orderable: false },
                            { data: 'IsCalculateHolidayHrs', visible: false },
                            { data: 'IsCalculateWOHrs', visible: false },
                            { data: 'IsCalculatePaidLeaveHrs', visible: false },
                            { data: 'IsCalculateLeaveHrs', visible: false },
                        ],
                        buttons: [
                            {
                                extend: 'excelHtml5',
                                text: 'Excel',
                                titleAttr: 'Excel',
                                exportOptions: {
                                    columns: ':visible:not(:last-child,:first-child)'
                                }
                            }
                        ],
                        "columnDefs": [
                            {
                                "targets": 17,
                                "sTitle": "Action",
                                "data": null,
                                "render": function (data, type, full, meta) {
                                    var actionHtml = "";

                                    // Edit button conditions
                                    if (roleid == 1) {
                                        actionHtml += "<a class='editclass btn btn-xs seagreen_btnnew tooltips' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a>";
                                    }
                                    else if (roleid == 6805 && canEdit && (data.GraceBefore != 0 && data.GraceBefore == cmpid)) {
                                        actionHtml += "<a class='editclass btn btn-xs seagreen_btnnew tooltips' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a>";
                                    }
                                    else if (roleid == 6806 && canEdit && (data.GraceBefore != 0 && data.GraceAfter == branchid)) {
                                        actionHtml += "<a class='editclass btn btn-xs seagreen_btnnew tooltips' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a>";
                                    }
                                    else if (roleid > 1 && canEdit && roleid != 6805 && roleid != 6806) {
                                        actionHtml += "<a class='editclass btn btn-xs seagreen_btnnew tooltips' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a>";
                                    }

                                    //Delete button condition
                                    //if (canDelete == "true") {
                                    //    if (actionHtml) actionHtml += "<span>|</span><span> </span>";
                                    //    actionHtml += "<a class='deleteclass btn btn-xs red tooltips' title='Delete'><i class='fa-regular fa-trash-can'></i></a>";
                                    //}
                                    return actionHtml;
                                }
                            },
                            {
                                "targets": 15,
                                "sTitle": "Shift Type",
                                "data": "IsFlexi",
                                "orderable": false,
                                "render": function (data, type, full, meta) {
                                    return data == 1 ? "Flexi" : "Regular";
                                }
                            },
                            {
                                "searchable": false,
                                "orderable": false,
                                "targets": 0
                            },
                        ],
                        pageLength: 10,
                        responsive: true,
                        autoWidth: false,
                        initComplete: function (settings, json) {

                            const inittable = this.api();

                            // Export button handling (avoid duplication)
                            if ($('.dt-buttons #Export').length === 0) {
                                $('.dt-buttons').empty();
                                const exportButton = '<button id="Export" class="dt-button buttons-excel buttons-html5" title="Excel"><span>Excel</span></button>';
                                $(".dt-buttons").append(exportButton);

                                $('[data-toggle="tooltip"]').tooltip();

                                $('#Export').on('click', function () {
                                    toastr.info("Preparing the Excel file for download...");
                                    setTimeout(function () {
                                        generateShiftDataExcelFile();
                                    }, 1000);
                                });
                            }

                            // Add serach functionality with footer append to thead
                            var footer = '<tr>';
                            columnSequence = ["Shift Name", "Short Name", "Start Time", "End Time", "Min Hrs Half Day", "Min Hrs Full Day", "Shift Type", "Shift Group"];

                            // Loop through the desired column sequence to build footer
                            columnSequence.forEach(function (columnName, index) {
                                footer += `<th>${columnName}</th>`;
                            });

                            // Add a final fixed column
                            footer += '<th></th></tr>';
                            $("#tblShift thead").append(footer);

                            // Add search inputs dynamically for only specified columns
                            $("#tblShift thead tr:eq(1) th").each(function (index) {
                                var title = $(this).text();
                                if (index === 11  ) {
                                    $(this).html('');
                                } else {
                                    $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search"></i>`);
                                }
                            });

                            $('.nosearch').parents('th').each(function () {
                                table.column($(this).index()).search('');
                            });

                            //// Apply the Search for all Columns
                            //$("#tblShift thead").on("keyup", "input", function () {
                            //    var columnIndex = $(this).parent().index();
                            //    var visibleColumns = table.columns(':visible').indexes().toArray();
                            //    var actualIndex = visibleColumns[columnIndex];
                            //    table.column(actualIndex).search(this.value).draw();
                            //});
                            let debounceTimer;
                            // Apply the Search all Columns
                            $("#tblShift thead").on("input", "input", function () {
                                clearTimeout(debounceTimer);
                                debounceTimer = setTimeout(() => {
                                    var columnIndex = $(this).parent().index();
                                    var visibleColumns = table.columns(':visible').indexes().toArray();
                                    var actualIndex = visibleColumns[columnIndex];

                                    table.column(actualIndex).search(this.value).draw();
                                }, 1000);
                            });
                           // var _exportflage = "ShiftMaster1";
                            injectExportButtonsForDataTables(_isServerSide, function () {
                                // Your export logic here
                                generateShiftDataExcelFile();
                            });
                        },
                        //rowCallback: function (row, data, index) {
                        //    var table = $('#tblShift').DataTable();
                        //    var pageInfo = table.page.info();
                        //    var rowIndex = pageInfo.start + index + 1; // Dynamic Sr No based on pagination
                        //    $('td:eq(0)', row).html(rowIndex);
                        //}
                    }

                    if (_isServerSide)
                    {
                            commonConfig.searching = true,
                            commonConfig.ordering = true,                           
                            commonConfig.serverSide = true;
                       
                        //commonConfig.data = servsidetableData,

                        // Enable server-side processing
                        commonConfig.ajax =
                        {
                            url: '/DataGridOptimize/GetPaginatedShift',
                            type: 'POST',
                            contentType: "application/json",
                            beforeSend: function () {
                                $("#ajax_loaderbranch").show();
                            },
                            data: function (d) {
                                var columnData = [];
                                var serverSidecolumnNames = [];
                                searchCriteria = [];
                                var field = '';
                                var value = '';

                                $('#tblShift thead th').each(function () {
                                    serverSidecolumnNames.push($(this).text().trim());
                                });

                                serverSidecolumnSequence = ["ShiftName", "ShiftShortName", "StartTime", "EndTime", "MinHrsHalfDay", "MinHrsFullDay", "ShiftType", "ShiftGroupName"];
                                columnSequence = ["Shift Name", "Short Name", "Start Time", "End Time", "Min Hrs Half Day", "Min Hrs Full Day", "Shift Type", "Shift Group"];

                                d.columns.forEach(function (col) {
                                    if (col.data && serverSidecolumnSequence.includes(col.data)) {
                                        // Get the corresponding name from columnSequence using the same index
                                        col.name = columnSequence[serverSidecolumnSequence.indexOf(col.data)];
                                    }
                                });

                                $('#tblShift thead .search_input').each(function () {
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
                                    DatagridThresold: mainobj.DataGridThreSold,
                                    CustFilter: searchCriteria.length > 0 ? searchCriteria : null,
                                    "RoleId": mainobj.RoleId,
                                    "LoginEmpId": mainobj.LoginEmpId,
                                    "CompanyID": mainobj.CompanyID,
                                    "BranchID": mainobj.BranchID,
                                };
                                return JSON.stringify(requestData);
                            },
                            dataSrc: function (json)
                            {
                                if (json.data)
                                {
                                    $("#ajax_loaderbranch").hide();
                                    return json.data;
                                }
                                return [];
                            },
                            complete: function ()
                            {
                                $(".ajax_loaderbranch").hide();
                            }
                        };
                    }
                    else {
                        commonConfig.serverSide = false;
                        commonConfig.ajax = null;
                        commonConfig.data = data;
                        commonConfig.deferRender = true;
                    }
                    $('#tblShift').on('draw.dt', function () {
                        $('.tooltips').tooltip();
                    });
                    $('.tooltips').tooltip();

                    setTimeout(function () {
                        $("#" + "tblShift" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive"));
                        $('.tooltips').tooltip();
                    }, 100);
                    // Reinitialize DataTable
                    table = $('#tblShift').DataTable(commonConfig);
                    $("#ajax_loaderbranch").hide();

                    $("#tblShift  input").unbind() // Unbind previous default bindings
                        .bind("input", function (e) { // Bind our desired behavior
                            clearTimeout(searchTimeout); // Clear previous debounce timer
                            var searchValue = this.value.trim();
                            searchTimeout = setTimeout(function () {
                                t.search(searchValue).draw(); // Perform search
                            }, 1000);
                        });

                }
                else
                {                    
                    $("#ajax_loaderbranch").hide();
                    const headerHtml = `<thead>
                        <tr role='row'>
                            <th>Sr. No</th>
                            <th class='ui-state-default sorting' style='width: 87px;'>ShiftName</th>
                            <th class='ui-state-default sorting' style='width: 102px;'>ShiftShortName</th>
                            <th class='ui-state-default sorting' style='width: 102px;'>Start Time</th>
                            <th class='ui-state-default sorting' style='width: 102px;'>End Time</th>
                            <th class='ui-state-default sorting' style='width: 135px;'>MinHrsHalfDay</th>
                            <th class='ui-state-default sorting' style='width: 135px;'>MinHrsFullDay</th>
                            <th class='ui-state-default sorting' style='width: 177px;'>ShiftType</th>
                            <th class='ui-state-default sorting' style='width: 76px;'>ShiftGroupName</th>
                            <th class='ui-state-default sorting' style='width: 77px;'>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr role='row' class='odd'>
                            <td colspan='9' style="text-align: center;">No data available in table</td>
                        </tr>
                    </tbody>`;

                    // Append header and footer
                    $('#tblShift').html(headerHtml);
                    const footerHtml = `
                <tr>
                    <th>Sr. No</th>
                    <th style="min-width: 150px">ShiftName</th>
                    <th style="min-width: 100px">ShiftShortName</th>
                    <th style="min-width: 100px">Start Time</th>
                    <th style="min-width: 100px">End Time</th>
                    <th style="min-width: 80px">MinHrsHalfDay</th>
                    <th style="min-width: 80px">MinHrsFullDay</th>
                    <th style="min-width: 80px">ShiftType</th>
                    <th style="min-width: 80px">ShiftGroupName</th>
                    <th></th>
                </tr>`;
                    $("#tblShift thead").append(footerHtml);
                    $("#tblShift thead tr:eq(1) th").each(function (index)
                    {
                        var title = $(this).text();
                        if (["ShiftName", "ShiftShortName", "MinHrsHalfDay", "MinHrsFullDay", "ShiftType", "ShiftGroupName"].includes(title)) {
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

            }
            else {
                $(".ajax_loaderbranch").hide();
                const headerHtml = `<thead>
                        <tr role='row'>
                            <th class='ui-state-default sorting' style='width: 87px;'>ShiftName</th>
                            <th class='ui-state-default sorting' style='width: 102px;'>ShiftShortName</th>
                            <th class='ui-state-default sorting' style='width: 102px;'>Start Time</th>
                            <th class='ui-state-default sorting' style='width: 102px;'>End Time</th>
                            <th class='ui-state-default sorting' style='width: 135px;'>MinHrsHalfDay</th>
                            <th class='ui-state-default sorting' style='width: 135px;'>MinHrsFullDay</th>
                            <th class='ui-state-default sorting' style='width: 177px;'>ShiftType</th>
                            <th class='ui-state-default sorting' style='width: 76px;'>ShiftGroupName</th>
                            <th class='ui-state-default sorting' style='width: 77px;'>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr role='row' class='odd'>
                            <td colspan='7' style="text-align: center;">No data available in table</td>
                        </tr>
                    </tbody>`;

                // Append header and footer
                $('#tblShift').html(headerHtml);
                const footerHtml = `
                <tr>                   
                    <th style="min-width: 150px">ShiftName</th>
                    <th style="min-width: 100px">ShiftShortName</th>
                    <th style="min-width: 100px">Start Time</th>
                    <th style="min-width: 100px">End Time</th>
                    <th style="min-width: 80px">MinHrsHalfDay</th>
                    <th style="min-width: 80px">MinHrsFullDay</th>
                    <th style="min-width: 80px">ShiftType</th>
                    <th style="min-width: 80px">ShiftGroupName</th>
                    <th></th>
                </tr>`;
                $("#tblShift thead").append(footerHtml);
                $("#tblShift thead tr:eq(1) th").each(function (index) {
                    var title = $(this).text();
                    if (["ShiftName", "ShiftShortName", "MinHrsHalfDay", "MinHrsFullDay", "ShiftType", "ShiftGroupName"].includes(title)) {
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
        complete: function () {
            //$("#ajax_loaderbranch").hide();
        },
        error: function (xhr, status, error) {
            console.log("AJAX error:", status, error);
        }
    });
}

function showvisibilityColumn() {
    var _chkarrylstsp = _chkarrylst;
    var _lst = 0;
    $('#fieldlst :input').each(function () {
        var _idfield = $(this).attr("id");
        var _rmidfield = _idfield.replace("chk_", "");
        $("#" + _idfield).prop('checked', false);
        var _ss = _chkarrylstsp.split(",")
        for (var i = 0; i <= _ss.length; i++) {
            if (_ss[i] == _rmidfield) {
                $("#" + _idfield).prop('checked', true);
                var _rmidfield = _idfield.replace("chk_", "");
                _lst += "," + _rmidfield;
            }
        }
    });
    _chkarrylst = _lst;
    $("#btnColumnvisibility").attr("data-toggle", "modal");
    $("#btnColumnvisibility").attr("data-target", "#frmpopupCustomfld");
}

function checkSession() {
    $.ajax({
        type: "GET",
        async: false,
        url: "/ESS/checkSession",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.isRedirect) {
                window.location.href = data.redirectUrl;
            }
        },
        error: function (data) {
            window.location.href = data;
        }
    });
}

function generateShiftDataExcelFile() {

    checkSession();

    $.ajax({
        url: '/DataGridOptimize/GetAllShiftExcelData',
        type: 'POST',
        data: JSON.stringify(GlobalObjData),
        contentType: 'application/json',
        //beforeSend: function () {
        //    $("#ajax_loaderbranch").show(); // Show loader before sending the request
        //},
        success: function (response) {
            if (response.success && response.filePath) {
                // toastr.success("Export completed. Starting download...");
                window.location.href = response.filePath; // Start file download
            } else {
                toastr.error(response.message || "Failed to generate export file.");
            }
        },
        error: function () {
            toastr.error("An error occurred while exporting data.");
        },
        //complete: function () {
        //    $("#ajax_loaderbranch").hide(); // Hide loader after request completes
        //}
    });

}