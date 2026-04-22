


function loadCompanyData(page, mainobj) {
    //var _urlNew = webapiurl + 'DataGridOptimize/GetCompanyGridData';
    var rowCount = 1;
    var _hdrcnt = 0;
    var P = 4;
    var _Cnttd = 0;
    var _num = "";
    var columns = [];
    var _NewURL = mainobj._webApiUrl;

    GlobalObjData =
    {
        "searchTerm": "",
        "sortColumn": "",  // Default sort column
        "sortOrder": "asc",     // Default sort order
        "pageSize": 10,
        "page": 1,
        "DataGridValues": mainobj.DataGridThreSold,
        "RoleId": mainobj.RoleId,
        "LoginEmpId": mainobj.LoginEmpId,
        "CustomFilters": []
    }
    
    $.ajax({
        type: "POST",
        url: _NewURL + 'DataGridOptimize/GetCompanyGridData',
        headers: { 'Authorization': mainobj._Token },
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(GlobalObjData),
        beforeSend: function () {
            $(".loading").show();
        },
        success: function (response) {

            if (response != null && response.Table != null) {
                const companylist = response.Table;
                handleGridVisibility("tblempty", "NotificationGrid-empty", companylist, {
                    imageUrl: "emptyscreen_1.png",
                    subText: "No entries found. Please add a record to get started."
                });
            }

            if (response.length > 0 || response != null || response != undefined || response != [] || response != {} || response.Table.length > 0) {
                var totalRecordCount = response.Table1[0].TotalRecords;
                var data = response.Table;
                _isServerSide = totalRecordCount > DataGridThreSold;

                var columnNames = [];
                if (response.Table.length == 0) {

                }
                else {
                    $("#fieldlst tr td").remove();
                    _chkarrylst = 0;
                    columnNames = Object.keys(response.Table[0]);
                    console.log(columnNames);
                    var _CheckcolumnNames = Object.keys(response.Table[0]);
                    $('#fieldlst').append('<tr id="tr_' + rowCount + '"></tr>');

                    var _num = "0,1,5,7,8,10,9,11";
                    var _numArray = _num.split(',').map(Number);
                    for (var i in _CheckcolumnNames) {

                        if (_hdrcnt > 11) {
                            _num += ", " + _hdrcnt
                        }

                        if (_hdrcnt == "0" || _hdrcnt == "1" || _hdrcnt == "8" || _hdrcnt == "9" || _hdrcnt == "10" || _hdrcnt == "11") {
                        }
                        else {
                            if (_Cnttd == 3) {
                                _Cnttd = 0;
                            }

                            _Cnttd = _Cnttd + 1;
                            if (_hdrcnt == "2" || _hdrcnt == "3" || _hdrcnt == "4" || _hdrcnt == "6") {
                                _chkarrylst += "," + _hdrcnt;
                                $('#tr_' + _Cnttd + '').append("<td  ID=" + _hdrcnt + "><label class='toggle-vis' data-column=" + _hdrcnt + "><input type='checkbox' name=" + columnNames[i] + " class='toggle-vis' data-column=" + _hdrcnt + " ID=chk_" + _hdrcnt + " checked><span> " + columnNames[i] + " </span></label></td>");
                            }
                            else {
                                $('#tr_' + _Cnttd + '').append("<td  ID=" + _hdrcnt + "><label class='toggle-vis' data-column=" + _hdrcnt + "><input type='checkbox' name=" + columnNames[i] + " class='toggle-vis' data-column=" + _hdrcnt + " ID=chk_" + _hdrcnt + "><span> " + columnNames[i] + " </span></label></td>");
                            }
                        }
                        _hdrcnt = _hdrcnt + 1;
                    }
                    for (var i in columnNames) {
                        if (columnNames.indexOf("file-") != -1) {
                            columns.push({
                                data: columnNames[i],
                                title: columnNames[i]
                            });
                        }
                        else if (columnNames[i] === "Company Contact") {
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
                        else if (columnNames[i] === "CompanyEmail") {
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
                }

                $('#tblCompanyDetails').destroy;
                //if (response.Table.length > 0)
                //{                    
                commonConfig =
                {
                    "destroy": true,
                    //dom: "<'row'<'col-lg-3 col-sm-5 col-xs-12'<'record_btn_main'l,><'dt-buttons'Bf>><'col-lg-9 col-sm-7 col-xs-12 mb_margin_bottom_15'<'resignblock text-right'>>>" +
                    //    "<'row'<'col-sm-12 margin-bottom-10'<'customforms_table'tr'>>>" +
                    //    "<'row'<'col-md-5 col-sm-5'i><'col-md-7 col-sm-7'p>>",
                    dom: _domColumn,
                    language: _languageColumn,
                    "data": data,
                    "columns": columns,
                    "buttons": _isServerSide ? [] : [
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
                            targets: 0,
                            searchable: false,
                            orderable: false
                        },
                        {
                            targets: 9,
                            searchable: false,
                            orderable: false
                        },
                        {
                            "targets": [columnNames.length],
                            "sTitle": "Action",
                            "data": null,
                            "searchable": false,
                            "orderable": false,
                            "render": function (data, type, full, meta) {
                                if (emprole == 1) {
                                    data = "<a class='editclass btn btn-xs seagreen_btnnew tooltips' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a";
                                }
                                else if (emprole == 6805 && _canEditNew && (data.CompanyID != 0 && data.CompanyID == cmpid)) {
                                    data = "<a class='editclass btn btn-xs seagreen_btnnew tooltips' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a";
                                }
                                else {
                                    data = "";
                                }
                                return data;
                            }
                        }
                    ],
                    pageLength: 10,
                    responsive: true,
                    autoWidth: false,
                    initComplete: function (settings, json) {

                        const inittable = this.api();
                        //$('#tblCompanyDetails tr:eq(0) th:eq(0)').html("Sr. No");

                        // Modify table header based on _planID and emprole
                        if (emprole == 6805 || emprole == 6806) {
                            // $('#tblCompanyDetails tr:eq(0) th:eq(0)').html("Sr. No");
                        } else if (_planID == "4") {
                            //$('#tblCompanyDetails tr:eq(0) th:eq(0)').html("Sr. No");
                        } else {
                            $('#tblCompanyDetails tr:eq(0) th:eq(2)').html(
                                "<a class='btn btn-xs tooltips' title='Columns' onclick='showvisibilityColumn();' id='btnColumnvisibility'>" +
                                "<i class='fa-regular fa-columns' style='font-size: 14px; color: #295097;'></i></a> Company Name"
                            ).css('min-width', '80px');
                        }
                        // Export button handling (avoid duplication)
                        //if ($('.dt-buttons #Export').length === 0)
                        //{
                        //    $('.dt-buttons').empty();
                        //    const exportButton = '<button id="Export" class="dt-button buttons-excel buttons-html5" title="Excel"><span>Excel</span></button>';
                        //    $(".dt-buttons").append(exportButton);

                        //    $('[data-toggle="tooltip"]').tooltip();

                        //    $('#Export').on('click', function () {
                        //        toastr.info("Preparing the Excel file for download...");
                        //        setTimeout(function () {
                        //            generateExcelFile();
                        //        }, 1000);
                        //    });
                        //}

                        var columnSequence = ["Company Name", "Company EmailId", "Company URL", "Company Contact", "Company Code", "Company Address", "IsActive"];
                        var footer = '<tr>';

                        columnNames.forEach(function (columnName, index) {
                            if (_isServerSide && !_numArray.includes(index) && columnSequence.includes(columnName)) {
                                footer += `<th>${columnName}</th>`;
                            }
                            else if (!_isServerSide && !_numArray.includes(index)) {
                                footer += `<th>${columnName}</th>`;
                            }
                            else if (!_numArray.includes(index)) {
                                footer += `<th></th>`;
                            }
                        });

                        footer += '<th></th></tr>';
                        $("#tblCompanyDetails thead").append(footer);

                        //Add search inputs dynamically for only specified columns
                        $("#tblCompanyDetails thead tr:eq(1) th").each(function (index) {
                            var title = $(this).text();

                            if (index == 5) {
                                $(this).html('');
                            }
                            else if (_isServerSide && columnSequence.includes(title)) {
                                $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search" id=""></i>`);
                            }
                            else if (!_isServerSide && columnSequence.includes(title)) {
                                $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search" id=""></i>`);
                            } else {

                            }
                        });
                        $('.nosearch').parents('th').each(function () {
                            inittable.column($(this).index()).search('');
                        });

                        //// Apply the Search for all Columns
                        //$("#tblCompanyDetails thead").on("keyup", "input", function () {
                        //    var columnIndex = $(this).parent().index();
                        //    var visibleColumns = inittable.columns(':visible').indexes().toArray();
                        //    var actualIndex = visibleColumns[columnIndex];
                        //    inittable.column(actualIndex).search(this.value).draw();
                        //});

                        // Hide columns as per _num
                        _num = _num.replace(', ' + columnNames.length, '');
                        inittable.columns([_num]).visible(false);

                        //var _exportflage = "CompanyMaster1";                            
                        injectExportButtonsForDataTables(_isServerSide, function () {
                            generateExcelFile();
                        });

                        initTableCustomization(_planID, emprole, columnNames, _num, _numArray, this);
                    }
                    //,
                    //rowCallback: function (row, data, displayIndex, displayIndexFull)
                    //{                            
                    //    var api = this.api();
                    //    var pageInfo = api.page.info();
                    //    var srNo = pageInfo.start + displayIndex + 1;
                    //    $('td:eq(0)', row).html(srNo); // Set Sr. No to the first cell
                    //}                       
                }

                if (_isServerSide) {
                    commonConfig.serverSide = true;
                    commonConfig.searching = true,
                        commonConfig.ordering = true,
                        commonConfig.ajax =
                        {
                            url: '/DataGridOptimize/GetPaginatedCompany',
                            type: 'POST',
                            contentType: "application/json",
                            beforeSend: function () {
                                $(".loading").show();
                            },
                            data: function (d) {
                                searchCriteria = [];
                                var field = '';
                                var value = '';

                                $('#tblCompanyDetails thead .search_input').each(function () {
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
                                    RoleId: mainobj.RoleId,
                                    LoginEmpId: mainobj.LoginEmpId
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
                    commonConfig.serverSide = false; // Disable server-side processing
                    commonConfig.ajax = null; // Use the initial data provided
                    commonConfig.data = data; // Set data directly for client-side processing
                    commonConfig.deferRender = true;
                }

                t = $('#tblCompanyDetails').DataTable(commonConfig);
                $('#tblCompanyDetails').on('draw.dt', function () {
                    $('.tooltips').tooltip();
                });
                $('.tooltips').tooltip();

                setTimeout(function () {
                    $("#" + "tblCompanyDetails" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive"));
                    $('.tooltips').tooltip();
                }, 100);
            }
            else {
                $(".loading").hide();
                const headerHtml = `<thead>
                        <tr role='row'>
                            <th class='ui-state-default sorting' style='width: 87px;'>Company Name</th>
                            <th class='ui-state-default sorting' style='width: 102px;'>Company EmailId</th>
                            <th class='ui-state-default sorting' style='width: 135px;'>Company URL</th>
                            <th class='ui-state-default sorting' style='width: 135px;'>Company Contact</th>
                            <th class='ui-state-default sorting' style='width: 177px;'>Company Code</th>
                            <th class='ui-state-default sorting' style='width: 76px;'>Company Address</th>
                            <th class='ui-state-default sorting' style='width: 77px;'>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr role='row' class='odd'>
                            <td colspan='7' style="text-align: center;">No data available in table</td>
                        </tr>
                    </tbody>`;

                // Append header and footer
                $('#tblCompanyDetails').html(headerHtml);
                const footerHtml = `
                <tr>                   
                    <th style="min-width: 150px">Company Name</th>
                    <th style="min-width: 100px">Company EmailId</th>
                    <th style="min-width: 80px">Company URL</th>
                    <th style="min-width: 80px">Company Contact</th>
                    <th style="min-width: 80px">Company Code</th>
                    <th style="min-width: 80px">Company Address</th>
                    <th></th>
                </tr>`;
                $("#tblCompanyDetails thead").append(footerHtml);
                $("#tblCompanyDetails thead tr:eq(1) th").each(function (index) {
                    var title = $(this).text();
                    if (["Company Name", "Company EmailId", "Company URL", "Company Contact", "Company Code", "Company Address"].includes(title)) {
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
        error: function (xhr, status, error) {
            console.error('AJAX Error:', textStatus, errorThrown);
            console.error('Response:', jqXHR.responseText); // Log the response text for debugging
        },
        complete: function () {
            $(".loading").hide(); // Hide the loader
        }
    });
}

function generateExcelFile() {
    checkSession();

    $.ajax({
        url: '/DataGridOptimize/GetAllCompanyExcelData',
        type: 'POST',
        data: JSON.stringify(GlobalObjData),
        contentType: 'application/json',
        beforeSend: function () {
            $("#ajax_loader").show(); // Show loader before the request is sent
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
            $("#ajax_loader").hide(); // Always hide loader when request completes (success or error)
        }
    });

}

function initTableCustomization(PlanId, RoleId, columnNames, _num, _numArray, tableElement) {

    const tableInstance = $(tableElement).DataTable();
    // Apply the Search for specific columns
    $('.nosearch').parents('th').each(function () {
        tableInstance.column($(this).index()).search('');
    });
    let debounceTimer;
    // Apply the Search for all Columns
    $("#tblCompanyDetails thead").on("input", "input", function () {
        clearTimeout(debounceTimer); // Clear previous timer
        debounceTimer = setTimeout(() => {
            var columnIndex = $(this).parent().index();
            var visibleColumns = tableInstance.columns(':visible').indexes().toArray();
            var actualIndex = visibleColumns[columnIndex];
            tableInstance.column(actualIndex).search(this.value).draw();
        }, 1000);
    });
}

