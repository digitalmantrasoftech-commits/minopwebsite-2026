

function loadDepartmentData(page, mainobj) {
    

    var _hdrcnt = 0;
    var P = 4;
    var _Cnttd = 0;
    var _num = "";
    var columns = [];
    var searchTimeout;

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
    /*$('#tbldepartment').empty();*/

    $.ajax({
        type: "POST",
        url: _urlNew,
        headers: { 'Authorization': mainobj.Token },
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(GlobalObjData),
        beforeSend: function () {
            $(".loading").show();
        },
        success: function (response) {

            if (response != null && response.Table != null) {
                const Department = response.Table;
                handleGridVisibility("tblempty", "NotificationGrid-empty", Department, {
                    imageUrl: "emptyscreen_1.png",
                    subText: "No entries found. Please add a record to get started."
                });
            }

            if (response != null || response != undefined || response != null || response != [] || response.Table.length > 0) {
                var totalRecordCount = response.Table1[0].TotalRecords;
                var data = response.Table;
                _isServerSide = totalRecordCount > mainobj.DataGridThreSold;

                var columnNames = [];
                $(".loading").hide();

                if (response.Table.length == 0) {

                }
                else {

                    $("#fieldlst tr td").remove();
                    _chkarrylst = 0;
                    columnNames = Object.keys(response.Table[0]);                  
                    var _CheckcolumnNames = Object.keys(response.Table[0]);
                    _num = "0,1,3,5,6,7,8,9,10,12,13";
                    var _numArray = _num.split(',').map(Number);
                    for (var i in _CheckcolumnNames) {

                        if (_hdrcnt > 13) {
                            _num += ", " + _hdrcnt
                        }

                        if (_hdrcnt == "0" || _hdrcnt == "1" || _hdrcnt == "3" || _hdrcnt == "5" || _hdrcnt == "6" || _hdrcnt == "7" || _hdrcnt == "8" || _hdrcnt == "9" || _hdrcnt == "10" || _hdrcnt == "12" || _hdrcnt == "13") {
                        }
                        else {
                            if (_Cnttd == 4) {
                                _Cnttd = 0;
                            }

                            _Cnttd = _Cnttd + 1;
                            if (_hdrcnt == "2" || _hdrcnt == "4" || _hdrcnt == "11") {

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
                        if (columnNames[i] === "Department EmailId") {
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
                $('#tbldepartment').destroy;
                if (response.Table.length > 0) {

                    commonConfig =
                    {
                        "destroy": true,
                        // dom: "<'row'<'col-lg-3 col-sm-5 col-xs-12'<'record_btn_main'l,><'dt-buttons'Bf>><'col-lg-9 col-sm-7 col-xs-12 mb_margin_bottom_15'<'resignblock text-right'>>>" +
                        // "<'row'<'col-sm-12 margin-bottom-10'<'customforms_table 'tr'>>>" +
                        //  "<'row'<'col-md-5 col-sm-5'i><'col-md-7 col-sm-7'p>>",
                        dom: _domColumn,
                        language: _languageColumn,
                        "data": data,
                        "columns": columns,
                        buttons: [
                            {
                                extend: 'excelHtml5',
                                text: 'Excel',
                                titleAttr: 'Excel',
                                exportOptions: {
                                    columns: ':visible:not(:last-child,:first-child)'
                                }
                            },
                        ],
                        "columnDefs": [

                            { targets: [columnNames.length], sTitle: "Action" },
                            {
                                "targets": [columnNames.length],
                                "sTitle": "Action",
                                orderable: false,
                                searchable: false,
                                data: null,
                                "render": function (data, type, full, meta) {
                                    if (mainobj.RoleId == 1) {
                                        data = "<a class='editclass btn btn-xs seagreen_btnnew tooltips' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a";
                                    }
                                    else if (mainobj.RoleId == 6805 && _canEditNew && (data.CompanyID != 0 && data.CompanyID == cmpid)) {
                                        data = "<a class='editclass btn btn-xs seagreen_btnnew tooltips' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a";
                                    }
                                    else if (mainobj.RoleId == 6806 && _canEditNew && (data.CompanyID != 0 && data.BranchID == branchid)) {
                                        data = "<a class='editclass btn btn-xs seagreen_btnnew tooltips' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a";
                                    }
                                    else if (mainobj.RoleId > 1 && _canEditNew && mainobj.RoleId != 6805 && mainobj.RoleId != 6806) {
                                        data = "<a class='editclass btn btn-xs seagreen_btnnew tooltips' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a";
                                    }
                                    else {
                                        data = "";
                                    }
                                    return data;
                                },

                            },
                            {
                                "searchable": false,
                                "orderable": false,
                                "targets": 0
                            }
                        ],
                        pageLength: 10,
                        responsive: true,
                        autoWidth: false,
                        initComplete: function (settings, json) {
                            const inittable = this.api();
                            //$('#tbldepartment tr:eq(0) th:eq(0)').html("Sr. No");

                            // Modify table header based on _planID and emprole
                            if (mainobj.RoleId == 6805 || mainobj.RoleId == 6806) {
                                // $('#tbldepartment tr:eq(0) th:eq(0)').html("Sr. No");
                            } else if (mainobj.PlanID == "4") {
                                // $('#tbldepartment tr:eq(0) th:eq(0)').html("Sr. No");
                            } else {
                                $('#tbldepartment tr:eq(0) th:eq(2)').html(
                                    "<a class='btn btn-xs tooltips' title='Columns' onclick='showvisibilityColumn();' id='btnColumnvisibility'>" +
                                    "<i class='fa-regular fa-columns' style='font-size: 14px; color: #295097;'></i></a> Department Name"
                                );
                            }

                            // Export button handling (avoid duplication)
                            if ($('.dt-buttons #Export').length === 0) {
                                $('.dt-buttons').empty();
                                const exportButton = '<button id="Export" class="dt-button buttons-excel buttons-html5" title="Excel"><span>Excel</span></button>';
                                $(".dt-buttons").append(exportButton);

                                $('[data-toggle="tooltip"]').tooltip();

                                $('#Export').on('click', function () {
                                    toastr.info("Preparing the Excel file for download...");
                                    setTimeout(function () {
                                        generateExcelDepartmentFile();
                                    }, 1000);
                                });
                            }

                            var columnSequence = ["Department Name", "Department EmailId", "Department Headname"];
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
                            $("#tbldepartment thead").append(footer);

                            //Add search inputs dynamically for only specified columns
                            $("#tbldepartment thead tr:eq(1) th").each(function (index) {
                                var title = $(this).text();
                                //if (index == 0) {
                                //    $(this).html('');
                                //} else
                                if (_isServerSide && columnSequence.includes(title)) {
                                    $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search" id=""></i>`);
                                }
                                else if (!_isServerSide) {
                                    $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search" id=""></i>`);
                                } else {

                                }
                            });

                            $('.nosearch').parents('th').each(function () {
                                inittable.column($(this).index()).search('');
                            });

                            let debounceTimer; // Declare this outside to keep reference between events

                            $("#tbldepartment thead").on("keyup", "input", function () {
                                clearTimeout(debounceTimer); // Clear the previous timer if user is still typing

                                const input = this;

                                debounceTimer = setTimeout(() => {
                                    var columnIndex = $(input).parent().index();
                                    var visibleColumns = inittable.columns(':visible').indexes().toArray();
                                    var actualIndex = visibleColumns[columnIndex];
                                    inittable.column(actualIndex).search(input.value).draw();
                                }, 1000); // 3000 milliseconds = 3 seconds
                            });



                            // Hide columns as per _num
                            _num = _num.replace(', ' + columnNames.length, '');
                            inittable.columns([_num]).visible(false);

                            //var _exportflage = "DepartMaster1";
                            injectExportButtonsForDataTables(_isServerSide, function () {
                                // Your export logic here
                                generateExcelDepartmentFile();
                            });
                        },
                        //rowCallback: function (row, data, displayIndex, displayIndexFull) {

                        //    var api = this.api();
                        //    var pageInfo = api.page.info();
                        //    var srNo = pageInfo.start + displayIndex + 1;
                        //    $('td:eq(0)', row).html(srNo); // Set Sr. No to the first cell
                        //},
                    }

                    if (_isServerSide) {

                        commonConfig.serverSide = true;
                        commonConfig.searching = true,
                            commonConfig.ordering = true,

                            commonConfig.ajax =
                            {
                                url: '/DataGridOptimize/GetPaginatedDepartment',
                                type: 'POST',
                                contentType: "application/json",
                                beforeSend: function () {
                                    $(".loading").show();
                                },
                                data: function (d) {
                                    searchCriteria = [];
                                    var field = '';
                                    var value = '';

                                    $('#tbldepartment thead .search_input').each(function () {
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

                    t = $('#tbldepartment').DataTable(commonConfig);

                    $("#tbldepartment  input").unbind() // Unbind previous default bindings
                        .bind("input", function (e) { // Bind our desired behavior
                            clearTimeout(searchTimeout); // Clear previous debounce timer
                            var searchValue = this.value.trim();
                            searchTimeout = setTimeout(function () {
                                t.search(searchValue).draw(); // Perform search
                            }, 1000);
                        });

                    $('#tbldepartment').on('draw.dt', function () {
                        $('.tooltips').tooltip();
                    });
                    $('.tooltips').tooltip();
                    setTimeout(function () {
                        $("#" + "tbldepartment" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive"));
                        $('.tooltips').tooltip();
                    }, 100);
                }
                else {
                    $(".loading").hide();
                    const headerHtml = `<thead>
                        <tr role='row'>
                            <th>Sr. No</th>
                            <th class='ui-state-default sorting' style='width: 102px;'>Department Name</th>
                            <th class='ui-state-default sorting' style='width: 135px;'>Department EmailId</th>
                            <th class='ui-state-default sorting' style='width: 135px;'>Department Headname</th>                            
                            <th class='ui-state-default sorting' style='width: 77px;'>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr role='row' class='odd'>
                            <td colspan='7' style="text-align: center;">No data available in table</td>
                        </tr>
                    </tbody>`;

                    // Append header and footer
                    $('#tbldepartment').html(headerHtml);
                    $('#tbldepartment').parent().removeClass('row');
                    const footerHtml = `
                <tr>                   
                    <th>Sr. No</th>
                    <th style="min-width: 100px">Department Name</th>
                    <th style="min-width: 80px">Department EmailId</th>
                    <th style="min-width: 80px">Department Headname</th>                  
                    <th></th>
                </tr>`;
                    $("#tbldepartment thead").append(footerHtml);
                    $("#tbldepartment thead tr:eq(1) th").each(function (index) {
                        var title = $(this).text();
                        if (["Department Name", "Department EmailId", "Department Headname"].includes(title)) {
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
                $(".loading").hide();
                const headerHtml = `<thead>
                        <tr role='row'>
                            <th class='ui-state-default sorting' style='width: 87px;'>Sr NO</th>
                            <th class='ui-state-default sorting' style='width: 102px;'>Department Name</th>
                            <th class='ui-state-default sorting' style='width: 135px;'>Department EmailId</th>
                            <th class='ui-state-default sorting' style='width: 135px;'>Department Headname</th>                            
                            <th class='ui-state-default sorting' style='width: 77px;'>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr role='row' class='odd'>
                            <td colspan='7' style="text-align: center;">No data available in table</td>
                        </tr>
                    </tbody>`;

                // Append header and footer
                $('#tbldepartment').html(headerHtml);
                $('#tbldepartment').parent().removeClass('row');
                const footerHtml = `
                <tr>                   
                    <th style="min-width: 150px">Sr NO</th>
                    <th style="min-width: 100px">Department Name</th>
                    <th style="min-width: 80px">Department EmailId</th>
                    <th style="min-width: 80px">Department Headname</th>                  
                    <th></th>
                </tr>`;
                $("#tbldepartment thead").append(footerHtml);
                $("#tbldepartment thead tr:eq(1) th").each(function (index) {
                    var title = $(this).text();
                    if (["Department Name", "Department EmailId", "Department Headname"].includes(title)) {
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
            $(".loading").hide();
            console.log("AJAX error:", status, error);
        }
    });
}

function generateExcelDepartmentFile() {
    checkSession();

    $.ajax({
        url: '/DataGridOptimize/GetAllDepartmentExcelData',
        type: 'POST',
        data: JSON.stringify(GlobalObjData),
        contentType: 'application/json',
        beforeSend: function () {
            $("#ajax_loader").show(); // Show loader before request is sent
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
            $("#ajax_loader").hide(); // Always hide loader after request completes
        }
    });

}