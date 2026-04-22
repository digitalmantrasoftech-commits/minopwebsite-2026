
var globalDataObj;
function LoadTransctionDeviceData(_ojbClassTransParamData) {
    var mainObj =
    {
        "searchTerm": $('#searchTerm').val() || "",
        "sortColumn": "",  // Default sort column
        "sortOrder": "asc",     // Default sort order
        "pageSize": _ojbClassTransParamData.pageSize,
        "page": _ojbClassTransParamData.page,
        "DatagridThresold": _ojbClassTransParamData.DataGridThreSold,
        "DeviceCode": _ojbClassTransParamData.DeviceCode,
        "FilterFromDate": _ojbClassTransParamData.FilterFromDate,
        "FilterToDate": _ojbClassTransParamData.FilterToDate,
        "BranchID": _ojbClassTransParamData.bID,
        "Export_flg": 0,
        "CustomFilters": "",
        "PunchID": _ojbClassTransParamData._PunchID
    };
    globalDataObj = mainObj;
    //tokan
    if ($.fn.DataTable.isDataTable('#tblTransactionData')) {
        $('#tblTransactionData').DataTable().destroy();
    }
    var _newURL = _ojbClassTransParamData.webapiurl;

    $.ajax({
        type: "POST",
        headers: { 'Authorization': _ojbClassTransParamData.tokan },
        url: _newURL + 'Master/developertmpDmpTerminalGetAllCounts_New',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 30000,
        data: JSON.stringify(mainObj),
        beforeSend: function () {
            $("#ajax_loaderTransData").show();
        },
        success: function (response) {
            if (response != null && response != undefined && response.Table2 != undefined) {
                var totalRecordCount = response.Table1 && response.Table1.length > 0 ? response.Table1[0].TotalCount : 0;
                var data = response.Table2;
                var _isServerSide = totalRecordCount > mainObj.DatagridThresold;
                $("#ajax_loaderTransData").hide();
                commonConfig =
                {
                    draw: _ojbClassTransParamData.page,
                    recordsTotal: totalRecordCount,
                    recordsFiltered: totalRecordCount,
                    "destroy": true,
                    lengthMenu:
                        [
                            [10, 25, 50, 100, 1700],
                            ['10', '25', '50', '100', 'All']
                        ],
                    //"dom": 'Bflrtip',
                    dom: _domCommon,
                    language: _languageCommon,
                    buttons: _isServerSide ? [] :
                        [
                            {
                                extend: 'excelHtml5',
                                filename: 'TransactionsData',
                                exportOptions: {
                                    columns: [2, 3, 4, 5, 6,7,8,9]
                                }
                            }
                        ],
                    "data": data,
                    "bDestroy": true,
                    "columns":
                        [
                            { "data": null },
                            { "data": "txnId" },
                            { "data": "punchId" },
                            { "data": "dvcId" },
                            { "data": "txnDateTime" },
                            { "data": "isSync" },
                            { "data": "LastActivity" },
                            { "data": "EntryDate" },
                            { "data": "Remarks" },
                            { "data": "RemarksDate" }
                        ],
                    "columnDefs": [
                        {
                            "searchable": false,
                            "bSort": false,
                            "targets": 0
                        },
                        {
                            "targets": [1],
                            "visible": false
                        },
                        {
                            "targets": 5, // isSync column
                            "render": function (data, type, row)
                            {                               
                                return (data === "1" || data === 1 || data === true || data === "true")
                                    ? 'True' : 'False';
                            }
                        }
                    ],
                    pageLength: 10,
                    responsive: true,
                    autoWidth: false,
                    rowCallback: function (row, data, displayIndex, displayIndexFull)
                    {
                        var api = this.api();
                        var pageInfo = api.page.info();
                        var srNo = pageInfo.start + displayIndex + 1;
                        $('td:eq(0)', row).html(srNo); // Set Sr. No to the first cell
                    },
                    initComplete: function (settings, json)
                    {
                        //$('.dataTables_filter').hide(); // Hide the default Serach Button                        
                        if (_isServerSide)
                        {
                            if ($('.dt-buttons #Export').length === 0) {
                                $('.dt-buttons').empty(); // Optional: clear existing buttons
                              const exportButton = `
                                <button id="Export" class="dt-button buttons-excel buttons-html5" title="Excel">
                                <span>Excel</span>
                                </button>`;
                                $(".dt-buttons").append(exportButton);
                                $('[data-toggle="tooltip"]').tooltip(); // Reapply tooltip

                                $('#Export').on('click', function ()
                                {
                                    //toastr.info("Preparing the Excel file for download...");
                                    //setTimeout(function ()
                                    //{
                                    //    generateExcelFileForTransctionData(globalDataObj);                                        
                                    //}, 1000);
                                });
                            }
                        }
                        //const visibleTable = $('table:visible').attr('id');
                        //$("#buttons").append(`
                        //    <div id="export-button" class="export-button tooltips" data-placement="top" data-original-title="Download" data-table-id="${visibleTable}">
                        //        <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
                        //    </div>
                        // `);

                    }
                }
                
                if (_isServerSide)
                {
                    commonConfig.searching = true,
                    commonConfig.ordering = true,
                    commonConfig.paging = true,
                    commonConfig.serverSide = true; // Enable server-side processing

                    commonConfig.ajax =
                    {
                        url: '/DataGridOptimize/GetPaginatedTransctionDeveloeprGridData',
                        type: 'POST',
                        contentType: "application/json",
                        headers: { 'Authorization': _ojbClassTransParamData.tokan },
                        beforeSend: function () {
                            $("#ajax_loaderTransData").show();
                        },
                        data: function (d)
                        {
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
                                DatagridThresold: mainObj.DatagridThresold,
                                DeviceCode: _ojbClassTransParamData.DeviceCode,
                                FilterFromDate: _ojbClassTransParamData.FilterFromDate,
                                FilterToDate: _ojbClassTransParamData.FilterToDate,
                                BranchID: _ojbClassTransParamData.bID,
                                Export_flg: 0,
                                PunchID: _ojbClassTransParamData._PunchID
                            };
                            return JSON.stringify(requestData);
                        },
                        dataSrc: function (json)
                        {
                            if (json.data)
                            {
                                $("#ajax_loaderTransData").hide();
                                return json.data; // Return the data array if it exists
                            }
                            return [];
                        },
                        complete: function ()
                        {
                            $("#ajax_loaderTransData").hide();
                        }
                    };
                }
                else
                {
                    commonConfig.serverSide = false; // Disable server-side processing
                    commonConfig.ajax = null; // Use the initial data provided
                    commonConfig.data = data; // Set data directly for client-side processing
                    commonConfig.deferRender = true; // For performance with large data
                }
                t = $('#tblTransactionData').DataTable(commonConfig); // Initialize the DataTable
                Directory_table = t;
                $('#tblTransactionData').on('draw.dt', function () {
                    $('.tooltips').tooltip();
                });
                $('.tooltips').tooltip();
                setTimeout(function () {
                    $("#" + "tblTransactionData" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive"));
                    $('.tooltips').tooltip();

                    injectExportButtonsForDataTables(_isServerSide, function () {                     
                        generateExcelFileForTransctionData(globalDataObj);
                    });
                }, 100);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('AJAX Error:', textStatus, errorThrown);
            console.error('Response:', jqXHR.responseText); // Log the response text for debugging
        },
        complete: function () {
            $("#ajax_loaderTransData").hide(); // Hide the loader
        }
    });
}
function generateExcelFileForTransctionData(globalDataObj)
{    
    if (globalDataObj != null)
    {
        var Obj =
        {
            "searchTerm": $('#searchTerm').val() || "",
            "sortColumn": "",  // Default sort column
            "sortOrder": "asc",     // Default sort order
            "pageSize": globalDataObj.pageSize,
            "page": globalDataObj.page,
            "DatagridThresold": globalDataObj.DatagridThresold,
            "DeviceCode": globalDataObj.DeviceCode,
            "FilterFromDate": globalDataObj.FilterFromDate,
            "FilterToDate": globalDataObj.FilterToDate,
            "BranchID": globalDataObj.bID,
            "Export_flg": 1,
            "CustomFilters": "",
            "PunchID": globalDataObj._PunchID
        };
        $.ajax({
            url: '/DataGridOptimize/GetPaginatedTransctionDeveloeprGridDataExport',
            type: 'POST',
            data: JSON.stringify(Obj),
            contentType: 'application/json',
            beforeSend: function ()
            {
                $("#ajax_loaderTransData").show();
            },
            success: function (response)
            {
                if (response.success && response.filePath)
                {
                    $("#ajax_loaderTransData").hide();
                    toastr.success("Export completed. Starting download...");                    
                    window.location.href = response.filePath;
                }
                else
                {
                    toastr.error(response.message || "Failed to generate export file.");
                }
            },
            error: function () {
                toastr.error("An error occurred while exporting data.");
            },
            complete: function ()
            {
                $("#ajax_loaderTransData").hide();
            }
        });
    }
}
