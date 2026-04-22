
var globalDataObj;
var searchTimeout;
function LoadLeaveSanctionGridData(_ojbClassTransParamData) {
    
    var _dateformate = isDateFormat;   
    var mainObj =
    {
        "searchTerm": $('#searchTerm').val() || "",
        "sortColumn": "",  // Default sort column
        "sortOrder": "asc",     // Default sort order
        "pageSize": _ojbClassTransParamData.PageSize,
        "page": _ojbClassTransParamData.Page,
        "DataGridthresold": _ojbClassTransParamData.DataThreshHold,
        "BranchID": _ojbClassTransParamData.BranchID,
        "EmpIds": _ojbClassTransParamData.Empid,           
        "cmpid": _ojbClassTransParamData.cmpid,
        "Brchid": _ojbClassTransParamData.Brchid,
        "RoleId": _ojbClassTransParamData.RoleId,
        "CustomFilters": "",
        "Export_flg": 0,
        "Empid": _ojbClassTransParamData.loginempid,
        "SelectAllEmp": _ojbClassTransParamData.SelectAllEmp,
        "SelectAllEmpSearchTerm": _ojbClassTransParamData.SelectAllEmpSearchTerm,
        "SelectAllBranch": _ojbClassTransParamData.SelectAllBranch,
        "SelectAllBranchSearchTerm": _ojbClassTransParamData.SelectAllBranchSearchTerm,
        "IsActive": _ojbClassTransParamData.IsActive
    };
    globalDataObj = mainObj;
    //tokan
    if ($.fn.DataTable.isDataTable('#tblLeaveSanction')) {
        $('#tblLeaveSanction').DataTable().destroy();
    }
    var _newURL = _ojbClassTransParamData.webapiurl;

    $.ajax({
        type: "POST",
        headers: { 'Authorization': _ojbClassTransParamData.tokan },
        url: _newURL + 'Transaction/LeaveOpeningGetAll',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
/*        timeout: 30000,*/
        data: JSON.stringify(mainObj),
        beforeSend: function () {
            $("#ajax_loader").show();
        },
        success: function (response) {            
            if (response != null && response.Table2 != null) {
                const LeaveOpeningGetAll = response.Table2;
                handleGridVisibility("tblempty", "NotificationGrid-empty", LeaveOpeningGetAll, {
                    imageUrl: "emptyscreen_3.png",
                    subText: "No matching records found for the selected filters."
                });
            }


            if (response != null && response != undefined && response.Table2 != undefined) {
                var totalRecordCount = response.Table1 && response.Table1.length > 0 ? response.Table1[0].TotalCount : 0;
                var data = response.Table2;
                var _isServerSide = totalRecordCount > _ojbClassTransParamData.DataThreshHold;

                $("#ajax_loader").hide();
                commonConfig =
                {
                    draw: _ojbClassTransParamData.Page,
                    recordsTotal: totalRecordCount,
                    recordsFiltered: totalRecordCount,
                    "destroy": true,
                    lengthMenu:
                        [
                            [10, 25, 50, 100, 1700],
                            ['10', '25', '50', '100', 'All']
                        ],
                    //"dom": 'flrtip',
                    dom: _domCommon,
                    language: _languageCommon,
                    "data": data,
                    "bDestroy": true,
                    buttons: [
                        {
                            extend: 'excelHtml5',
                            text: 'Excel',
                            titleAttr: 'Download Excel',
                            className: 'buttons-excel',
                            exportOptions: {
                                columns: ':visible'
                            }
                        }
                    ],
                    "columns":
                        [
                            { "data": "EmpName", "title": "Name" },
                            { "data": "EmpCode", "title": "EmpCode" },
                            { "data": "LeaveTypeName", "title": "Leave Type" },
                            { "data": "LeaveBalance", "title": "Leave Balance" },
                            {
                                "data": "BalanceDateStr",                                
                                title: `Balance Date <i class='fa-regular fa-circle-info tooltips' data-placement='bottom' title='Date format is : ${_dateformate}' style='color: #295097;'></i>`
                            }
                        ],
                    pageLength: 10,
                    responsive: true,
                    autoWidth: false,
                    initComplete: function () {
                        if ($('#export-button').length) {
                         
                        }
                        else {
                            const visibleTable = $('table:visible').attr('id');
                            $("#buttons").append(`
                              <div id="export-button" class="export-button tooltips" data-placement="bottom" data-original-title="Download" data-table-id="${visibleTable}">
                                  <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
                              </div>
                           `);
                        }
                    },
                }
                if (_isServerSide) {

                    commonConfig.searching = true,
                        commonConfig.ordering = true,
                        commonConfig.paging = true,
                        commonConfig.serverSide = true; // Enable server-side processing

                    commonConfig.ajax =
                    {
                        url: '/DataGridOptimize/GetPaginatedLeaveSanctionprGridData',
                        type: 'POST',
                        contentType: "application/json",
                        headers: { 'Authorization': _ojbClassTransParamData.tokan },
                        beforeSend: function () {
                            $("#ajax_loader").show();
                        },
                        data: function (d) {
                            const requestData =
                            {
                                draw: d.draw,
                                search:
                                {
                                    value: d.search.value || ''
                                },
                                order: d.order,
                                columns: d.columns,
                                length: d.length,
                                start: d.start,
                                CustFilter: null,
                                DatagridThresold: globalDataObj.DataGridthresold,
                                BranchID: globalDataObj.BranchID,
                                Export_flg: 0,
                                CompanyID: globalDataObj.cmpid,
                                EmpIds: globalDataObj.EmpIds,
                                SelectAllEmp: globalDataObj.SelectAllEmp,
                                SelectAllEmpSearchTerm: globalDataObj.SelectAllEmpSearchTerm,
                                SelectAllBranch: globalDataObj.SelectAllBranch,
                                SelectAllBranchSearchTerm: globalDataObj.SelectAllBranchSearchTerm,
                                IsActive:globalDataObj.IsActive
                            };
                            return JSON.stringify(requestData);
                        },
                        dataSrc: function (json) {
                            if (json.data) {
                                $("#ajax_loader").hide();
                                return json.data; // Return the data array if it exists
                            }
                            return [];
                        },
                        complete: function () {
                            $("#ajax_loader").hide();
                        }
                    };
                }
                else {
                    commonConfig.data = data;
                    commonConfig.serverSide = false; // Disable server-side processing
                    commonConfig.ajax = null; // Use the initial data provided
                    // // Set data directly for client-side processing
                    commonConfig.deferRender = true; // For performance with large data                    
                    commonConfig.searching = true; // <-- ADD THIS LINE
                    commonConfig.ordering = true;  // Optional: enable sorting                    
                }
                $("#dataTables_tbl_header").remove();
                t = $('#tblLeaveSanction').DataTable(commonConfig); // Initialize the DataTable
                $('#tblLeaveSanction').on('draw.dt', function () {
                    $('.tooltips').tooltip();
                });
                $('.tooltips').tooltip();

                setTimeout(function () {
                    $("#" + "tblLeaveSanction" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive"));
                    $('.tooltips').tooltip();
                }, 100);

                $("#tblLeaveSanction_filter  input").unbind() // Unbind previous default bindings
                    .bind("input", function (e) { // Bind our desired behavior
                        clearTimeout(searchTimeout); // Clear previous debounce timer
                        var searchValue = this.value.trim();
                        searchTimeout = setTimeout(function () {
                            t.search(searchValue).draw(); // Perform search
                        }, 1000);
                    });

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