$(document).ready(function () {
    $("#btnApply").click(function (event) {
        var checkedColumnIds = [];
        $('#fieldlst :input').each(function () {

            var fieldId = $(this).attr("id");
            var column = Directory_table.column($(this).attr('data-column'));
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
        $("#tblBranchDetails thead tr:eq(1)").remove();

        // Add new footer based on visible columns with search inputs
        var visibleColumns = Directory_table.columns(':visible').indexes().toArray();
        var footer = '<tr class="footer">';

        $("#tblBranchDetails thead th").each(function (index) {
            var headerText = $(this).text().trim();
            if (["BranchName", "BranchAddress", "CompanyName", "ReportingBranchName"].includes(headerText)) {
                footer += `<th><input type="text" class="search_input" data-column="${headerText}" placeholder="Search" /><i class="clm-search"></i></th>`;
            }
            else {
                footer += '<th></th>';
            }
        });
        footer += '</tr>';
        $("#tblBranchDetails thead").append(footer);
        $('#frmpopupCustomfld').modal('toggle');
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
        $('#fieldlst :input').each(function () {
            var _idfield = $(this).attr("id");
            $("#" + _idfield).prop('checked', false);
            var _rmidfield = _idfield.replace("chk_", "");
        });
    });
});

function configureColumnDefs(columnNames, emprole, _objview, cmpid, branchid) {
    return [
        {
            targets: [columnNames.length],
            sTitle: "Action",
            orderable: false
        },
        {
            targets: [columnNames.length],
            sTitle: "Action",
            data: null,
            render: function (data, type, full, meta) {
                if (emprole == 1) {
                    data = "<a class='editclass btn btn-xs seagreen_btnnew tooltips' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a>";
                } else if (emprole == 6805 && _objview == 'True' && (data.CompanyID != 0 && data.CompanyID == cmpid)) {
                    data = "<a class='editclass btn btn-xs seagreen_btnnew tooltips' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a>";
                } else if (emprole == 6806 && _objview == 'True' && (data.CompanyID != 0 && data.BranchId == branchid)) {
                    data = "<a class='editclass btn btn-xs seagreen_btnnew tooltips' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a>";
                } else if (emprole > 1 && emprole != 6806 && emprole != 6805 && _objview == 'True' && (data.CompanyID != 0 && data.BranchId == branchid)) {
                    data = "<a class='editclass btn btn-xs seagreen_btnnew tooltips' title='Edit'><i class='fa-regular fa-pen-to-square'></i></a>";
                } else {
                    data = "";
                }
                return data;
            }
        },
        {
            searchable: false,
            orderable: false,
            targets: 0
        }
    ];
}



function LoadBranchGridData(page, _URL, DataGridParameterObject) {
    var columns = [];
    var isedit = _objview;
    var _columnsname = "";
    var _num = "";
    var _hdrcnt = 0;
    var P = 4;
    var _Cnttd = 0;
    var _numArray = [];
    var searchCriteria = [];


    var mainObj =
    {
        "searchTerm": $('#searchTerm').val() || "",
        "sortColumn": "bm.branchname",  // Default sort column
        "sortOrder": "asc",     // Default sort order
        "pageSize": DataGridParameterObject._PagesizeDefault,
        "page": 1,
        "Export_flg": 0,
        "DataGridValues": DataGridParameterObject.DataGridThreSold,
        "LoginId": LoginId,
        "RoleId": emprole,
        "CustomFilters": searchCriteria.length > 0 ? searchCriteria : []
    };

    if ($.fn.DataTable.isDataTable('#tblBranchDetails')) {
        $('#tblBranchDetails').DataTable().destroy();
    }
    $('#tblBranchDetails').empty();
    
    $.ajax({

        type: "POST",
        headers: { 'Authorization': DataGridParameterObject._token },
        url: _URL,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 30000,
        data: JSON.stringify(mainObj),
        beforeSend: function () {
            $("#ajax_loaderbranch").show();
        },
        success: function (response) {

            if (response != null && response.Result != null) {
                const Branch = response.Result.ItemsForBranch;
                handleGridVisibility("tblempty", "NotificationGrid-empty", Branch, {
                    imageUrl: "emptyscreen_1.png",
                    subText: "No entries found. Please add a record to get started."
                });
            }

            if (response.Result != null && response.Result.ItemsForBranch.length > 0) {
                $("#fieldlst tr td").remove();
                var totalRecordCount = response.Result.TotalItems;
                var data = response.Result.ItemsForBranch;
                columnNames = Object.keys(data[0]);
                console.log(columnNames);
                _chkarrylst = 0;
                var _CheckcolumnNames = Object.keys(data[0]);
                _num = "0,3,4,5,6,7,8,10,11,12,13,14,15,16,17,18,19,20,21";

                _numArray = _num.split(',').map(Number);

                for (var i in _CheckcolumnNames) {
                    if (_hdrcnt > 18) {
                        _num += ", " + _hdrcnt
                    }
                    if (_hdrcnt == "0" || _hdrcnt == "3" || _hdrcnt == "4" || _hdrcnt == "5" || _hdrcnt == "6" || _hdrcnt == "7" || _hdrcnt == "8" || _hdrcnt == "10" || _hdrcnt == "11" || _hdrcnt == "12" || _hdrcnt == "13" || _hdrcnt == "16" || _hdrcnt == "17" || _hdrcnt == "19" || _hdrcnt == "20" || _hdrcnt == "21") {
                    }
                    else {
                        if (_Cnttd == 4) {
                            _Cnttd = 0;
                        }

                        _Cnttd = _Cnttd + 1;
                        if (_hdrcnt == "1" || _hdrcnt == "2" || _hdrcnt == "9") {

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
                    columns.push({
                        data: columnNames[i],
                        title: columnNames[i]
                    });
                }

                _isServerSide = totalRecordCount > DataGridParameterObject.DataGridThreSold;
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


                var columnDefs = configureColumnDefs(columnNames, emprole, isedit, cmpid, branchid);
                commonConfig =
                {
                    draw: page,
                    recordsTotal: totalRecordCount,
                    recordsFiltered: totalRecordCount,
                    "destroy": true,
                    // dom: "<'row'<'col-lg-3 col-sm-5 col-xs-12'<'record_btn_main'l,><'dt-buttons'Bf>><'col-lg-9 col-sm-7 col-xs-12 mb_margin_bottom_15'<'resignblock text-right'>>>" +
                    //     "<'row'<'col-sm-12 margin-bottom-10'<'customforms_table 'tr'>>>" +
                    //    "<'row'<'col-md-5 col-sm-5'i><'col-md-7 col-sm-7'p>>",
                    dom: _domColumn,
                    language: _languageColumn,
                    columnDefs: columnDefs,
                    buttons: [
                        {
                            extend: 'excelHtml5',
                            text: 'Excel',
                            titleAttr: 'Excel',
                            exportOptions:
                            {
                                columns: ':visible:not(:last-child,:first-child)'
                            }
                        },

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
                                    // Show the toast notification
                                    toastr.info("Preparing the Excel file for download...");

                                    // Delay the AJAX function by 1 second (1000 milliseconds)
                                    setTimeout(function () {
                                        generateBranchExcelFile();
                                    }, 1000);
                                });
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
                                    // Show the toast notification
                                    toastr.info("Preparing the Excel file for download...");

                                    // Delay the AJAX function by 1 second (1000 milliseconds)
                                    setTimeout(function () {
                                        generateBranchExcelFile();
                                    }, 1000);
                                });
                            }
                        }
                        const table = this.api();
                        // Column header update based on PlanId or RoleId

                        if (DataGridParameterObject.NewPlanID == "4") {
                            //$('#tblBranchDetails tr:eq(0) th:eq(0)').html("Sr.No");
                        }
                        else {
                            $('#tblBranchDetails tr:eq(0) th:eq(1)').html("<a class='btn btn-xs tooltips' title='Columns' onclick='showvisibilityColumn();' id='btnColumnvisibility'><i class='fa-regular fa-columns' style='font-size: 14px; color: #295097;'></i></a> BranchName").css('min-width', '100px');
                        }
                        if (emprole == 6805 || emprole == 6806) {
                            // $('#tblBranchDetails tr:eq(0) th:eq(0)').html("Sr.No");
                        }

                        // Hide specific columns                        
                        _num = _num.replace(', ' + columnNames.length, '');
                        $('#tblBranchDetails').DataTable().columns([_num]).visible(false);
                        if ($(".customforms_table").length) {
                        }
                        // Hide loader
                        $("#ajax_loaderbranch").hide();
                        var footer = '<tr>';
                        columnNames.forEach(function (columnName, index) {
                            if (!_numArray.includes(index)) {
                                // Add the class for index 60
                                if (index === 22) {
                                    footer += `<th class="fixed-column-1">${columnName}</th>`;
                                } else {
                                    footer += `<th>${columnName}</th>`;
                                }
                            }
                        });
                        footer += '<th class="fixed-column"></th></tr>';
                        $("#tblBranchDetails thead").append(footer);
                        $("#tblBranchDetails thead tr:eq(1) th").each(function (index) {
                            var title = $(this).text();
                            if (["BranchName", "BranchAddress", "CompanyName", "ReportingBranchName"].includes(title)) {
                                $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" />
                                    <i class="clm-search" id=""></i>`);
                            }
                            else {
                                // If it's not one of the desired columns, you can leave it as is or apply a different action
                                $(this).html('');  // For example, you can leave the column header unchanged
                            }
                        });
                        let debounceTimer;
                        // Apply the Search for all Columns
                
                        $("#tblBranchDetails thead").on("input", "input", function () {
                            clearTimeout(debounceTimer); // Clear previous timer
                            debounceTimer = setTimeout(() => {
                                var columnIndex = $(this).parent().index();
                                var visibleColumns = Directory_table.columns(':visible').indexes().toArray();
                                var actualIndex = visibleColumns[columnIndex];
                                Directory_table.column(actualIndex).search(this.value).draw();
                            }, 1000);
                        });

                        $('.nosearch').parents('th').each(function () {
                            Directory_table.column($(this).index()).search('');
                        });
                        
                        // Apply the Search all Columns
                        //$("#tblBranchDetails thead").on("keyup", "input", function () {
                        //    var columnIndex = $(this).parent().index();
                        //    var visibleColumns = Directory_table.columns(':visible').indexes().toArray();
                        //    var actualIndex = visibleColumns[columnIndex];

                        //    Directory_table.column(actualIndex).search(this.value).draw();
                        //});
                        
                        //var _exportflage = "BranchMaster1";
                        injectExportButtonsForDataTables(_isServerSide, function () {

                            // Your export logic here
                            generateBranchExcelFile();
                        });

                    },
                };
                // If server-side processing is needed, adjust the AJAX configuration
                if (_isServerSide) {
                    // Enable server-side processing

                    commonConfig.searching = true,
                        commonConfig.ordering = true,
                        commonConfig.serverSide = true;

                    commonConfig.ajax =
                    {
                        url: '/DataGridOptimize/GetPaginatedBranches',
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

                            $('#tblBranchDetails thead .search_input').each(function () {
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
                                CustFilter: searchCriteria.length > 0 ? searchCriteria : null,
                                LoginId: LoginId, // Include the filter object correctly
                                RoleId: emprole
                                // Make sure this references the correct variable
                                //CustFilter: combinedSearchValue
                            };
                            return JSON.stringify(requestData);
                        },
                        dataSrc: function (json) {
                            if (json.data) {
                                $("#ajax_loaderbranch").hide();
                                return json.data.ItemsForBranch; // Return the data array if it exists
                            }
                            return [];
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
                    //commonConfig.processing = false;
                }
                t = $('#tblBranchDetails').DataTable(commonConfig); // Initialize the DataTable
                Directory_table = t;
                $('#tblBranchDetails').on('draw.dt', function () {
                    $('.tooltips').tooltip();
                });
                $('.tooltips').tooltip();
                setTimeout(function () {
                    $("#" + "tblBranchDetails" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive"));
                    $('.tooltips').tooltip();
                }, 100);
            }
            else {
                $("#ajax_loaderbranch").hide();
                const headerHtml = `<thead>
                        <tr role='row'>
                            <th class='ui-state-default sorting' style='width: 87px; display: none;'>Sr. No</th>
                            <th class='ui-state-default sorting' style='width: 102px;'>BranchName</th>
                            <th class='ui-state-default sorting' style='width: 135px;'>Reporting Branch Name</th>
                            <th class='ui-state-default sorting' style='width: 135px;'>Company Name</th>
                            <th class='ui-state-default sorting' style='width: 77px;'>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr role='row' class='odd'>
                            <td colspan='7' style="text-align: center;">No data available in table</td>
                        </tr>
                    </tbody>`;

                // Append header and footer
                $('#tblBranchDetails').html(headerHtml);

                const footerHtml = `
                <tr>                   
                    <th style="min-width: 50px">Sr. No</th>
                    <th style="min-width: 100px">BranchName</th>
                    <th style="min-width: 80px">Reporting Branchname</th>
                    <th style="min-width: 80px">Company Name</th>                              
                    <th style="min-width: 60px"></th>
                </tr>`;
                $("#tblBranchDetails thead").append(footerHtml);

                $("#tblBranchDetails thead tr:eq(1) th").each(function (index) {
                    var title = $(this).text();
                    if (["BranchName", "Reporting Branchname", "Company Name"].includes(title)) {
                        $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search" id=""></i>`);
                    }
                    else if (index == 0) {
                        $(this).html('<input type="checkbox" class="text-right selectall" id="selectall">');
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

function generateBranchExcelFile(event) {
    var searchCriteria = [];
    var mainObj =
    {
        "searchTerm": $('#searchTerm').val() || "",
        "sortColumn": "bm.branchname",  // Default sort column
        "sortOrder": "asc",     // Default sort order
        "pageSize": 10,
        "page": 1,
        "Export_flg": 1,
        "DataGridValues": 10000,
        "LoginId": LoginId,
        "RoleId": emprole,
        "CustomFilters": searchCriteria.length > 0 ? searchCriteria : []
    };
    $.ajax({
        url: '/DataGridOptimize/GetAllBranchExcelData',
        type: 'POST',
        data: JSON.stringify(mainObj), // No need to wrap in { mainObj }
        contentType: 'application/json',
        beforeSend: function () {
            $("#ajax_loaderbranch").show(); // Show loader before request
        },
        success: function (response) {
            if (response.success && response.filePath) {
                // toastr.success("Export completed. Starting download...");
                window.location.href = response.filePath;
            } else {
                toastr.error(response.message || "Failed to generate export file.");
            }
        },
        error: function () {
            toastr.error("An error occurred while exporting data.");
        },
        complete: function () {
            $("#ajax_loaderbranch").hide(); // Always hide loader after request finishes
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


