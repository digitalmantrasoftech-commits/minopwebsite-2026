
var commonConfig = "";
var columns = [];

function restoreCheckboxState() {
    $('#tblempmaster').find('.ClsChkEmp').each(function () {
        var empPunchID = $(this).attr('id').replace('select_', '');
        if (checkboxStates[empPunchID]) {
            $(this).prop('checked', true);
        }
    });
}

function Employeeselect() {

    $("#tblempmaster .ClsChkEmp").each(function (key, value) {
        var empPunchID = $(this).parent('td').parent('tr').children(key)[0].innerText;
        if ($(this).is(':checked') == true) {
            var objemp = {};
            objemp.EmpPunchID = $(this).parent('td').parent('tr').children(key)[0].innerText;
            objemp.EmpName = $(this).parent('td').parent('tr').children(key)[1].innerText;
            objemp.Email = $(this).parent('td').parent('tr').children(key)[2].innerText;
            objemp.EmpPhoto = $(this).parent('td').parent('tr').children(key)[3].innerText;
            objemp.Deviceid = "";
            if (!allEmp.some(emp => emp.EmpPunchID === objemp.EmpPunchID)) {
                allEmp.push(objemp);
            }
            //else {
            // If unchecked, remove the corresponding object from the allEmp array
            //     allEmp = allEmp.filter(emp => emp.EmpPunchID !== empPunchID);
            // }
            // allEmp.push(objemp);
        }
        else {
            allEmp = allEmp.filter(emp => emp.EmpPunchID !== empPunchID);
        }
    });
    if (allEmp.length == 0) {
        return false;
    }
    return true;
}
function LoadEnrollUserGridData(page, mainObj)
{
    
    if ($.fn.DataTable.isDataTable('#tblempmaster')) {
        $('#tblempmaster').DataTable().destroy();
    }
    $("#tblempmaster").destroy;
    var _newURL = mainObj.webapiurl;

    $.ajax({
        type: "POST",
        headers: { 'Authorization': mainObj.tokan },
        url: _newURL + 'DataGridOptimize/EmployeelstEnrollUserPaginatList',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(mainObj),
        beforeSend: function () {
            $("#ajax_loader").show();
        },
        success: function (response)
        {            
            if (response != null || response != undefined || response != [])
            {
                $("#ajax_loader").hide();

                var totalRecordCount = response.Result.TotalItems;
                var data = response.Result.ItemsForEnrollUser;               

                var _isServerSide = totalRecordCount > mainObj.DataGridValues;
                // $("#dataTables_tbl_header").remove();
                $('#s-2 #dataTables_tbl_header').remove();
                commonConfig =
                {
                    dom: _domCommon,
                    language: _languageCommon,
                    draw: page,
                    recordsTotal: totalRecordCount,
                    recordsFiltered: totalRecordCount,
                    "destroy": true,
                    lengthMenu: [
                        [10, 25, 50, 100, 500, 700, 1000],
                        [10, 25, 50, 100, 500, 700, 1000],
                    ],
                    "data": data,
                    "columns": [
                        { "data": "EmpPunchID" },
                        { "data": "EmpName" },
                        { "data": "Email" },
                        { "data": "EmpPhoto" }
                    ],
                    "columnDefs": [
                        {
                            "targets": [3],
                            "visible": false
                        },
                        {
                            "targets": [0, 1, 2],
                            /*  orderable: false,*/
                        },
                        {
                            //orderable: false,
                            "targets": 4,
                            "data": null,
                            "width": "3%",
                            "render": function (data, type, full, meta) {
                                data = "<input type='checkbox'  id='select_" + data.EmpPunchID + "' class='ClsChkEmp'>";
                                return data;
                            },

                        }
                    ],
                    pageLength: 10,
                    responsive: true,
                    autoWidth: false,
                    "fnDrawCallback": function (oSettings) {
                        restoreCheckboxState();
                        $("#selectall").prop('checked', false)

                        $('#tblempmaster').off('change', '.ClsChkEmp').on('change', '.ClsChkEmp', function () {
                            var empPunchID = $(this).attr('id').replace('select_', '');
                            checkboxStates[empPunchID] = $(this).prop('checked');
                            Employeeselect();
                        });
                    }

                }

                if (_isServerSide)
                {
                    MainObjFilter =
                    {
                        "CompanyIDs": mainObj.CmpId,
                        "BranchIDs": mainObj.BranchId,
                        "DepartmentIDs": mainObj.DptId,

                    };

                    commonConfig.searching = true,
                        commonConfig.ordering = true,
                        commonConfig.paging = true,
                        commonConfig.serverSide = true; // Enable server-side processing

                    commonConfig.ajax =
                    {
                        url: '/DataGridOptimize/GetPaginatedEnrollUser',                       
                        type: 'POST',
                        contentType: "application/json",
                        headers: { 'Authorization': tokan },
                        data: function (d) {

                            const requestData =
                            {

                                StatusId: mainObj.StatusId,
                                DatagridThresold: mainObj.DataGridValues,
                                draw: d.draw,
                                search: {
                                    value: d.search.value || ''
                                },
                                order: d.order,
                                columns: d.columns,
                                length: d.length,
                                start: d.start,
                                CustFilter: null,
                                SelectAll: mainObj.SelectAll,
                                SelectAllSearchTerm: mainObj.SelectAllSearchTerm,
                                EmployeeFilter: MainObjFilter// Make sure this references the correct variable
                                //CustFilter: combinedSearchValue
                            };
                            return JSON.stringify(requestData);
                        },
                        dataSrc: function (json)
                        {
                            if (json.data) {
                                return json.data.ItemsForEnrollUser; // Return the data array if it exists
                            }
                            return [];
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
                t = $('#tblempmaster').DataTable(commonConfig); // Initialize the DataTable
                setTimeout(function () {
                    $("#" + "tblempmaster" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive1"));
                    $('.tooltips').tooltip();
                }, 100);
                Directory_table = t;
                $('#tblempmaster').on('draw.dt', function ()
                {
                    $('.tooltips').tooltip();
                });
                $('.tooltips').tooltip();
            }            
        },
        error: function (jqXHR, textStatus, errorThrown)
        {
            console.error('AJAX Error:', textStatus, errorThrown);
            console.error('Response:', jqXHR.responseText); // Log the response text for debugging
        },
        complete: function ()
        {
            $("#ajax_loader").hide(); // Hide the loader
        }
    });        
     
}
