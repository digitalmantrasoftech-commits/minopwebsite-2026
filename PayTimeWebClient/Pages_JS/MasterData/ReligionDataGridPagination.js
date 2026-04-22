
function loadReligionData(page, mainobj) {

    //var _urlNew = webapiurl + 'DataGridOptimize/GetReligionGridData';
    var rowCount = 1;
    var _hdrcnt = 0;
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
        "CompanyID": 0,
        "BranchID": mainobj.BranchID,
        "LoginEmpId": mainobj.LoginEmpId,
        //"DepartmentID": mainobj.DepartmentID,
        //"DesignationID": mainobj.DesignationID,
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
        success: function (response) {

            if (response != null && response.Table != null) {
                const Religion = response.Table;
                handleGridVisibility("tblempty", "NotificationGrid-empty", Religion, {
                    imageUrl: "emptyscreen_1.png",
                    subText: "No entries found. Please add a record to get started."
                });
            }

            if (response != null || response != undefined || response != null || response != []) {
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
                    console.log(columnNames);
                }
                $('#tblReligion').destroy;

                if (response.Table.length > 0) {
                    commonConfig =
                    {
                        "destroy": true,
                        //dom: "<'row'<'col-lg-3 col-sm-5 col-xs-12'<'record_btn_main'l,><'dt-buttons'Bf>><'col-lg-9 col-sm-7 col-xs-12 mb_margin_bottom_15'<'resignblock text-right'>>>" +
                        //   "<'row'<'col-sm-12 margin-bottom-10'<'customforms_table'tr'>>>" +
                        //  "<'row'<'col-md-5 col-sm-5'i><'col-md-7 col-sm-7'p>>",
                        dom: _domColumn,
                        language: _languageColumn,
                        "data": data,
                        "columns": [
                            {
                                data: null,
                                orderable: false,
                                visible: false,
                            },
                            { data: 'ReligionId', visible: false }, // Hidden column
                            { data: 'ReligionName' },
                            { data: 'CompanyID', visible: false }, // Hidden column
                            { data: 'BranchID', visible: false }, // Hidden column
                            { data: 'IsActive', visible: false }, // Hidden column
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
                                "targets": 6,
                                "sTitle": "Action",
                                "data": null,
                                "render": function (data, type, full, meta) {

                                    var actionHtml = "";

                                    // Edit button conditions
                                    if (roleid == 1) {
                                        actionHtml += "<a class='editclass seagreen_btnnew tooltips' data-placement='top' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a>";
                                    }
                                    else if (roleid == 6805 && canEdit && (data.CompanyID != 0 && data.CompanyID == cmpid)) {
                                        actionHtml += "<a class='editclass seagreen_btnnew tooltips' data-placement='top' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a>";
                                    }
                                    else if (roleid == 6806 && canEdit && (data.CompanyID != 0 && data.BranchID == branchid)) {
                                        actionHtml += "<a class='editclass seagreen_btnnew tooltips' data-placement='top' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a>";
                                    }
                                    else if (roleid > 1 && canEdit && roleid != 6805 && roleid != 6806) {
                                        actionHtml += "<a class=' v   seagreen_btnnew tooltips' data-placement='top' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a>";
                                    }
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
                                    toastr.info("Preparing the Excel file for download...");
                                    setTimeout(function () {
                                        generateExcelFile();
                                    }, 1000);
                                });
                            }

                            // Add serach functionality with footer append to thead
                            var footer = '<tr>';
                            columnSequence = ["ReligionName"];

                            // Loop through the desired column sequence to build footer
                            columnSequence.forEach(function (columnName, index) {

                                footer += `<th>${columnName}</th>`;
                            });

                            // Add a final fixed column
                            footer += '<th></th></tr>';
                            $("#tblReligion thead").append(footer);

                            // Add search inputs dynamically for only specified columns
                            $("#tblReligion thead tr:eq(1) th").each(function (index) {
                                var title = $(this).text();
                                if (index === 0) {
                                    $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search"></i>`);
                                    //$(this).html('');
                                } else {
                                    $(this).html('');
                                    //$(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search"></i>`);
                                }
                            });

                            $('.nosearch').parents('th').each(function () {
                                table.column($(this).index()).search('');
                            });

                            //// Apply the Search for all Columns
                            //$("#tblReligion thead").on("keyup", "input", function () {
                            //    var columnIndex = $(this).parent().index();
                            //    var visibleColumns = table.columns(':visible').indexes().toArray();
                            //    var actualIndex = visibleColumns[columnIndex];
                            //    table.column(actualIndex).search(this.value).draw();
                            //});
                            let debounceTimer;
                            // Apply the Search all Columns
                            $("#tblReligion thead").on("input", "input", function () {
                                clearTimeout(debounceTimer);
                                debounceTimer = setTimeout(() => {
                                    var columnIndex = $(this).parent().index();
                                    var visibleColumns = table.columns(':visible').indexes().toArray();
                                    var actualIndex = visibleColumns[columnIndex];

                                    table.column(actualIndex).search(this.value).draw();
                                }, 1000);
                            });
                            // var _exportflage = "ReligionMaster1";
                            injectExportButtonsForDataTables(_isServerSide, function () {
                                // Your export logic here
                                generateExcelFile();
                            });
                        },
                        //rowCallback: function (row, data, index)
                        //{
                        //    var table = $('#tblReligion').DataTable();
                        //    var pageInfo = table.page.info();
                        //    var rowIndex = pageInfo.start + index + 1; // Dynamic Sr No based on pagination
                        //    $('td:eq(0)', row).html(rowIndex);
                        //}
                    }

                    if (_isServerSide) {
                        commonConfig.serverSide = true;
                        commonConfig.ajax =
                        {
                            url: '/DataGridOptimize/GetPaginatedReligion',
                            type: 'POST',
                            contentType: "application/json",
                            beforeSend: function () {
                                $(".loading").show();
                            },
                            data: function (d) {
                                searchCriteria = [];
                                var field = '';
                                var value = '';

                                $('#tblReligion thead .search_input').each(function () {
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
                                    "LoginEmpId": mainobj.LoginEmpId
                                };
                                return JSON.stringify(requestData);
                            },
                            dataSrc: function (json) {

                                if (json.data) {
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
                    $('#tblReligion').on('draw.dt', function () {
                        $('.tooltips').tooltip();
                    });
                    $('.tooltips').tooltip();

                    setTimeout(function () {
                        $("#" + "tblReligion" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive"));
                        $('.tooltips').tooltip();
                    }, 100);


                }
                else {
                    $(".loading").hide();
                    const headerHtml = `<thead>
                        <tr role='row'>
                            <th class='ui-state-default sorting' style='width: 87px;'>Religion Name</th>                          
                            <th class='ui-state-default sorting' style='width: 77px;'>Action</th>
                         </tr>
                        </thead>
                        <tbody>
                        <tr role='row' class='odd'>
                            <td colspan='7' style="text-align: left;">No data available in table</td>
                        </tr>
                    </tbody>`;

                    // Append header and footer
                    $('#tblReligion').html(headerHtml);
                    const footerHtml = `
                    <tr>                   
                        <th style="min-width: 150px">ReligionName</th>                   
                        <th></th>
                    </tr>`;
                    $("#tblReligion thead").append(footerHtml);
                    $("#tblReligion thead tr:eq(1) th").each(function (index) {
                        var title = $(this).text();
                        if (["ReligionName"].includes(title)) {
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
                // Reinitialize DataTable
                table = $('#tblReligion').DataTable(commonConfig);

                $(".loading").hide();
            }
            else {
                $(".loading").hide();
                const headerHtml = `<thead>
                        <tr role='row'>
                            <th class='ui-state-default sorting' style='width: 87px;'>ReligionName</th>                          
                            <th class='ui-state-default sorting' style='width: 77px;'>Action</th>
                         </tr>
                        </thead>
                        <tbody>
                        <tr role='row' class='odd'>
                            <td colspan='7' style="text-align: center;">No data available in table</td>
                        </tr>
                    </tbody>`;

                // Append header and footer
                $('#tblReligion').html(headerHtml);
                const footerHtml = `
                <tr>                   
                    <th style="min-width: 150px">ReligionName</th>                   
                    <th></th>
                </tr>`;
                $("#tblReligion thead").append(footerHtml);
                $("#tblReligion thead tr:eq(1) th").each(function (index) {
                    var title = $(this).text();
                    if (["ReligionName"].includes(title)) {
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
            $(".loading").hide();
        },
        error: function (xhr, status, error) {
            console.log("AJAX error:", status, error);
        }
    });
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
        url: '/DataGridOptimize/GetAllReligionExcelData',
        type: 'POST',
        data: JSON.stringify(GlobalObjData),
        contentType: 'application/json',
        beforeSend: function () {
            $("#ajax_loader").show(); // Show loader before sending request
        },
        success: function (response) {
            if (response.success && response.filePath) {
                // toastr.success("Export completed. Starting download...");
                window.location.href = response.filePath; // Trigger file download
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