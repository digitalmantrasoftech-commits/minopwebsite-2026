
function loadHolidayData(page, mainobj) {

    //var _urlNew = webapiurl + 'DataGridOptimize/GetHolidayGridData';

    var P = 4;
    var _Cnttd = 0;
    var _num = "";
    var columns = [];
    var columnSequence = [];
    var serverSidecolumnSequence = [];

    GlobalObjData = {
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
            $(".loading").show();
        },
        success: function (response)
        {
            if (response != null && response.Table != null) {
                const Holiday = response.Table;
                handleGridVisibility("tblempty", "NotificationGrid-empty", Holiday, {
                    imageUrl: "emptyscreen_1.png",
                    subText: "No entries found. Please add a record to get started."
                });
            }
            if (response != null || response != undefined || response != null || response != [] || response.Table.length > 0)
            {
                var totalRecordCount = response.Table1[0].TotalRecords;
                var data = response.Table;
                _isServerSide = totalRecordCount > mainobj.DataGridThreSold;

                var columnNames = [];

                if (response.Table.length == 0)
                {

                }
                else
                {
                    
                    $("#fieldlst tr td").remove();
                    _chkarrylst = 0;
                    columnNames = Object.keys(response.Table[0]);
                   
                }
                if ($.fn.dataTable.isDataTable('#tblHolidays')) {
                    $('#tblHolidays').DataTable().clear().destroy();
                    $("#tblHolidays").empty();
                }
                commonConfig =
                {
                    "destroy": true,
                   // dom: "<'row'<'col-lg-3 col-sm-5 col-xs-12'<'record_btn_main'l,><'dt-buttons'Bf>><'col-lg-9 col-sm-7 col-xs-12 mb_margin_bottom_15'<'resignblock text-right'>>>" +
                     //   "<'row'<'col-sm-12 margin-bottom-10'<'customforms_table 'tr'>>>" +
                     //   "<'row'<'col-md-5 col-sm-5'i><'col-md-7 col-sm-7'p>>",
                    dom: _domColumn,
                    language: _languageColumn,
                    "data": data,
                    "columns": [
                        {
                            data: null,
                            orderable: null,
                            visible : false 
                        },
                        { data: 'HolidayId', visible: false }, // Hidden column
                        { data: 'HolidayName' },
                        { data: 'HolidayDate' },
                        { data: 'HolidayToDate' },
                        { data: 'BranchId', visible: false },
                        { data: 'CountryName', visible: false },
                        { data: 'StateName', visible: false },
                        { data: 'TimezoneName', visible: false },
                        { data: 'ReligionId', visible: false },
                        { data: 'IsHolidayType', visible: false },
                        { data: 'HolidayApplicable', visible: false },
                        { data: 'CountryId', visible: false },
                        { data: 'StateId', visible: false },
                        { data: 'CompanyId', visible: false },
                        { data: 'CreatedDate' },
                        { data: 'ModifyDate' },
                        { data: null, orderable: false, searchable: false },
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
                                    actionHtml += "<a class='editclass seagreen_btnnew tooltips' data-placement='top' data-original-title='Edit'><i class='fa-regular fa-pen-to-square'></i></a>";
                                }
                                else if (roleid == 6805 && canEdit && (data.CompanyID != 0 && data.CompanyID == cmpid)) {
                                    actionHtml += "<a class='editclass seagreen_btnnew tooltips' data-placement='top' data-original-title='Edit'><i class='fa-regular fa-pen-to-square'></i></a>";
                                }
                                else if (roleid == 6806 && canEdit && (data.CompanyID != 0 && data.BranchId == branchid)) {
                                    actionHtml += "<a class='editclass seagreen_btnnew tooltips' data-placement='top' data-original-title='Edit'><i class='fa-regular fa-pen-to-square'></i></a>";
                                }
                                else if (roleid > 1 && canEdit && roleid != 6805 && roleid != 6806) {
                                    actionHtml += "<a class='editclass seagreen_btnnew tooltips' data-placement='top' data-original-title='Edit'><i class='fa-regular fa-pen-to-square'></i></a>";
                                }

                                // Delete button condition
                                //if (canDelete == "true") {
                                //    if (actionHtml) actionHtml += "<span>|</span> ";
                                //    actionHtml += "<a class='deleteclass btn btn-xs red tooltips' style='font-weight: bold; text-decoration: none; cursor: pointer;' title='Delete'><i class='fa-regular fa-trash-can'></i></a>";
                                //}
                                return actionHtml;
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
                                //toastr.info("Preparing the Excel file for download...");
                                //setTimeout(function () {
                                //    generateExcelFile();
                                //}, 1000);
                            });
                        }

                        // Add serach functionality with footer append to thead
                        var footer = '<tr>';
                        columnSequence = ["Holiday Name", "From Date", "To Date", "Entry Date", "Updated Date"];

                        // Loop through the desired column sequence to build footer
                        columnSequence.forEach(function (columnName, index) {
                            footer += `<th>${columnName}</th>`;
                        });

                        // Add a final fixed column
                        footer += '<th></th></tr>';
                        $("#tblHolidays thead").append(footer);

                        // Add search inputs dynamically for only specified columns
                        $("#tblHolidays thead tr:eq(1) th").each(function (index) {
                            var title = $(this).text();
                            if ( index === 5) {
                                $(this).html('');
                            } else {
                                $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search"></i>`);
                            }
                        });

                        $('.nosearch').parents('th').each(function () {
                            table.column($(this).index()).search('');
                        });

                        //// Apply the Search for all Columns
                        //$("#tblHolidays thead").on("keyup", "input", function () {
                        //    var columnIndex = $(this).parent().index();
                        //    var visibleColumns = table.columns(':visible').indexes().toArray();
                        //    var actualIndex = visibleColumns[columnIndex];
                        //    table.column(actualIndex).search(this.value).draw();
                        //});
                        let debounceTimer;
                        // Apply the Search all Columns
                        $("#tblHolidays thead").on("input", "input", function () {
                            clearTimeout(debounceTimer);
                            debounceTimer = setTimeout(() => {
                                var columnIndex = $(this).parent().index();
                                var visibleColumns = table.columns(':visible').indexes().toArray();
                                var actualIndex = visibleColumns[columnIndex];

                                table.column(actualIndex).search(this.value).draw();
                            }, 1000);
                        });
                        setTimeout(function () {
                            $("#" + "tblHolidays" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive"));

                            injectExportButtonsForDataTables(_isServerSide, function () {
                                generateExcelFile();
                            });
                            $('.tooltips').tooltip();

                        }, 100);
                     //var _exportflage = "HolidaytMaster1";
                        

                    },
                    //rowCallback: function (row, data, index)
                    //{
                    //    var table = $('#tblHolidays').DataTable();
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
                        url: '/DataGridOptimize/GetPaginatedHoliday',
                        type: 'POST',
                        contentType: "application/json",
                        beforeSend: function () {
                            $(".loading").show();
                        },
                        data: function (d) {
                            var columnData = [];
                            var serverSidecolumnNames = [];
                            searchCriteria = [];
                            var field = '';
                            var value = '';

                            $('#tblHolidays thead th').each(function () {
                                serverSidecolumnNames.push($(this).text().trim());
                            });

                            serverSidecolumnSequence = ["HolidayName", "HolidayDate", "HolidayToDate", "CreatedDate", "CreatedDate"];
                            columnSequence = ["Holiday Name", "From Date", "To Date", "Entry Date", "Updated Date"];

                            d.columns.forEach(function (col) {
                                if (col.data && serverSidecolumnSequence.includes(col.data)) {
                                    // Get the corresponding name from columnSequence using the same index
                                    col.name = columnSequence[serverSidecolumnSequence.indexOf(col.data)];
                                }
                            });

                            $('#tblHolidays thead .search_input').each(function () {
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
                                $(".loading").hide();
                                return json.data;
                            }
                            return [];
                        },
                        complete: function () {
                            $(".loading").hide();
                        }
                    };
                }
                else {
                    commonConfig.serverSide = false;
                    commonConfig.ajax = null;
                    commonConfig.data = data;
                    commonConfig.deferRender = true;
                }

                //table = $('#tblHoliday').DataTable(commonConfig);
                //setTimeout(function () {
                //    let trCount = $('#tblHoliday thead tr').length;
                //    if (trCount > 1) {
                //        $('#tblHoliday thead tr:nth-child(2)').remove();
                //    }
                //}, 500);
                $('#tblHolidays').on('draw.dt', function ()
                {
                    $('.tooltips').tooltip();
                });
                $('.tooltips').tooltip();
                  

               
                // Reinitialize DataTable
                table = $('#tblHolidays').DataTable(commonConfig);

                $("#tblHolidays  input").unbind() // Unbind previous default bindings
                    .bind("input", function (e) { // Bind our desired behavior
                        clearTimeout(searchTimeout); // Clear previous debounce timer
                        var searchValue = this.value.trim();
                        searchTimeout = setTimeout(function () {
                            t.search(searchValue).draw(); // Perform search
                        }, 1000);
                    });
                $(".loading").hide();
            }
            else {
                $(".loading").hide();
                const headerHtml = `<thead>
                        <tr role='row'>
                            <th class='ui-state-default sorting' style='width: 87px;'>HolidayName</th>
                            <th class='ui-state-default sorting' style='width: 102px;'>From Date</th>
                            <th class='ui-state-default sorting' style='width: 102px;'>To Date</th>
                            <th class='ui-state-default sorting' style='width: 102px;'>Entry Date</th>
                            <th class='ui-state-default sorting' style='width: 135px;'>Updated Date</th>                           
                            <th class='ui-state-default sorting' style='width: 77px;'>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr role='row' class='odd'>
                            <td colspan='7' style="text-align: center;">No data available in table</td>
                        </tr>
                    </tbody>`;

                // Append header and footer
                $('#tblHolidays').html(headerHtml);
                const footerHtml = `
                <tr>                   
                    <th style="min-width: 150px">HolidayName</th>
                    <th style="min-width: 100px">From Date</th>
                    <th style="min-width: 100px">To Date</th>
                    <th style="min-width: 100px">End Time</th>
                    <th style="min-width: 80px">Entry Date</th>
                    <th style="min-width: 80px">Updated Date</th>                  
                    <th></th>
                </tr>`;
                $("#tblHolidays thead").append(footerHtml);
                $("#tblHolidays thead tr:eq(1) th").each(function (index)
                {
                    var title = $(this).text();
                    if (["HolidayName"].includes(title)) {
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
            //$(".loading").hide();
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

function generateExcelFile() {

    checkSession();

    $.ajax({
        url: '/DataGridOptimize/GetAllHolidayExcelData',
        type: 'POST',
        data: JSON.stringify(GlobalObjData),
        contentType: 'application/json',
        beforeSend: function () {
            $("#ajax_loader").show(); // Show loader before request starts
        },
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
        complete: function () {
            $("#ajax_loader").hide(); // Hide loader after request finishes
        }
    });

}