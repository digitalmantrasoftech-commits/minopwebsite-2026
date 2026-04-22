
$(document).ready(function () {
    $("#btnApply").click(function (event)
    {
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
        $("#tblemployee thead tr:eq(1)").remove();

        // Add new footer based on visible columns with search inputs
        var visibleColumns = Directory_table.columns(':visible').indexes().toArray();
        var footer = '<tr class="footer">';

        $("#tblemployee thead th").each(function (index) {

            // Conditionally add search inputs
            var headerText = $(this).text().trim();
            if (headerText === "Status") {
                footer += '<th class="fixed-column-1"></th>';
            } else if (["EmpName", "Empcode", "EmpPunchID", "BranchName", "Email", "ShiftName", "ShiftGroupName", "DepartmentName", "EmpJoinDate", "PolicyName"].includes(headerText)) {
                footer += `<th><input type="text" class="search_input" data-column="${headerText}" placeholder="Search" /><i class="clm-search"></i></th>`;
            }
            else {
                footer += '<th></th>';
            }
        });
        footer += '</tr>';
        $("#tblemployee thead").append(footer);
        $('#frmpopupCustomfld').modal('toggle');
        $('#tblemployee thead tr:eq(1) th:last').addClass('fixed-column');
        $('#tblemployee thead tr:eq(2) th:last').addClass('fixed-column-1');

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
            _chkarrylst += "," + _rmidfield;
        });
    });

    $("#tblemployee").on('click', '.editclass', function () {
        var _a = t.row($(this).parent('td').parent('tr')).data()["EmpId"];
        $(".editclass").attr("data-toggle", "modal");
        $(".editclass").attr("data-target", "#frmpopup");
    });

});
function showvisibilityColumn() {
    var _chkarrylstsp = _chkarrylst;
    _chkarrylst = "";
    var _lst = "";
    var _idfield = "";
    $('#fieldlst :input').each(function () {
        $('#fieldlst').val('');
        _idfield = $(this).attr("id");
        var _rmidfield = _idfield.replace("chk_", "");
        $("#" + _idfield).prop('checked', false);
        var _ss = _chkarrylstsp.split(",")
        for (var i = 0; i <= _ss.length; i++) {
            if (_ss[i] == _rmidfield) {
                if (_ss[i] == 0) {
                    $("#" + _idfield).prop('checked', true);
                    $("#" + _idfield).attr('disabled', true);
                }
                else {
                    $("#" + _idfield).prop('checked', true);
                }
                var _rmidfield = _idfield.replace("chk_", "");
                _lst += "," + _rmidfield;
            }
        }
    });
    _chkarrylst = _lst;
    $("#btnColumnvisibility").attr("data-toggle", "modal");
    $("#btnColumnvisibility").attr("data-target", "#frmpopupCustomfld");
}
//***********************************Employee Edit & Delete***********************************//

function formatDate(data, dateFormat) {
    if (data != null && data !== "") {

        var timestamp;
        // Check if the data is in epoch format like /Date(1729535400000)/
        if (data.indexOf('/Date') !== -1) {

            timestamp = parseInt(data.match(/\d+/)[0], 10);  // Extract numeric timestamp
        }
        else {

            //// Assuming it's in yyyy-mm-dd format already
            //var date1 = data.split(' ')[0];  // Get the date part (before space)
            //var dateParts = date1.split('-');
            //return applyDateFormat(dateParts, dateFormat, null);  // Apply formatting for standard date

            if (data.indexOf('T') !== -1) {
                data = data.split('T')[0]; // Get the date part (before 'T')
            }
            // Assuming it's in yyyy-mm-dd or dd-mm-yyyy format
            var dateParts = data.split('-');
            return applyDateFormat(dateParts, dateFormat, null);
        }
        // Convert the timestamp to a Date object
        var dateObj = new Date(timestamp);
        // Now format the date based on the given format
        var dateParts =
            [
                dateObj.getFullYear(),
                ('0' + (dateObj.getMonth() + 1)).slice(-2),  // Month is 0-based, so we add 1
                ('0' + dateObj.getDate()).slice(-2)
            ];

        return applyDateFormat(dateParts, dateFormat, dateObj);  // Apply formatting for epoch date
    }
    return "";  // Return empty if the date is null or invalid
}
function applyDateFormat(dateParts, dateFormat, dateObj) {
    // This function arranges the date parts based on the required date format
    if (dateFormat === "dd-mm-yyyy") {
        return dateParts[2] + "-" + dateParts[1] + "-" + dateParts[0];  // dd-mm-yyyy
    }
    else if (dateFormat === "yyyy-mm-dd") {
        return dateParts[0] + "-" + dateParts[1] + "-" + dateParts[2];  // yyyy-mm-dd
    }
    else if (dateFormat === "mm-dd-yyyy") {
        return dateParts[1] + "-" + dateParts[2] + "-" + dateParts[0];  // mm-dd-yyyy
    }
    else if (dateFormat === "yyyy-M-dd" || dateFormat === "M-dd-yyyy" || dateFormat === "dd-M-yyyy") {
        if (!dateObj) {
            return "";  // If no dateObj is passed, return empty since we can't extract the month name
        }
        var month = dateObj.toLocaleString("en-us", { month: "short" });  // Get short month name
        if (dateFormat === "yyyy-M-dd") {
            return dateParts[0] + "-" + month + "-" + dateParts[2];  // yyyy-M-dd
        } else if (dateFormat === "M-dd-yyyy") {
            return month + "-" + dateParts[2] + "-" + dateParts[0];  // M-dd-yyyy
        } else if (dateFormat === "dd-M-yyyy") {
            return dateParts[2] + "-" + month + "-" + dateParts[0];  // dd-M-yyyy
        }
    }
    return "";  // Return empty if the date format is invalid
}

function getEditButton(empcode, isActive) {
    if (isActive) {
        return '<a class="editclass btn btn-xs blue tooltips" title="Edit"><span id="sp3" hidden>' + empcode + '</span><i class="fa-regular fa-pen-to-square"></i></a><a class="Inactiveclass btn btn-xs blue tooltips" title="Inactive"><i class="fa-solid fa-xmark"></i></a>';
    }
    else {
        return '<a class="editclass btn btn-xs blue tooltips" title="Edit"><span id="sp3" hidden>' + empcode + '</span><i class="fa-regular fa-pen-to-square"></i></a><a class="Activeclass btn btn-xs blue tooltips" title="Active"><i class="fa-solid fa-check"></i></a>';
    }
}
function gatherSearchFields() {
    let searchFields = [];
    $('.custom-search-field').each(function () {
        let field = $(this).find('select').val();
        let value = $(this).find('input').val();
        if (value) {
            searchFields.push({ field: field, value: value });
        }
    });
    return searchFields;
}

function getColumnConfig() {
    return [
        { "data": "EmpId" },
        { "data": "UserId" },
        { "data": "EmpName" },
        { "data": "Empcode" },
        { "data": "RoleId" },
        { "data": "ReligionId" },
        { "data": "IsSMS" },
        { "data": "Gender" },
        { "data": "EmpMarried" },
        { "data": "EmpJoinDate" },
        { "data": "EmpPunchID" },
        { "data": "BranchId" },
        { "data": "BranchName" },
        { "data": "DesignationId" },
        { "data": "DepartmentId" },
        { "data": "DepartmentName" },
        { "data": "DesignationName" },
        { "data": "ShiftId" },
        { "data": "ShiftName" },
        { "data": "CompanyID" },
        { "data": "CompanyName" },
        { "data": "Email" },
        { "data": "Password" },
        { "data": "EmpDOB" },
        { "data": "EmpAddress" },
        { "data": "EmpPhNo" },
        { "data": "EmpMNo" },
        { "data": "EmpPhoto" },
        { "data": "MobNoSMS" },
        { "data": "EmpResignDate" },
        { "data": "TypeId" },
        { "data": "EmpTypeName" },
        { "data": "GradeId" },
        { "data": "ShiftGroupId" },
        { "data": "ShiftGroupName" },
        { "data": "ContractorId" },
        { "data": "ContractorName" },
        { "data": "CategoryId" },
        { "data": "Categoryname" },
        { "data": "ReportingTo" },
        { "data": "PolicyId" },
        { "data": "PolicyName" },
        { "data": "EmpWeekOff" },
        { "data": "EmpSecondWeekOff" },
        { "data": "EmpSecondWeekOffRule" },
        { "data": "EmpHalfDay" },
        { "data": "EmpHalfDayRule" },
        { "data": "ShiftShortName" },
        { "data": "ShiftGroupShortName" },
        { "data": "isActive" },
        { "data": "Status" },
        { "data": "Married" },
        { "data": "JoinDate" },
        { "data": "BirthDate" },
        { "data": "EmpGender" },
        { "data": "RoleName" },
        { "data": "TagId" },
        { "data": "worf" },
        { "data": "geoenable" },
        { "data": "CountryCode" },
        { "data": "BranchGeolocation" },
        { "data": "ReportingName" }
    ];
}

function getColumnDefs(RoleId, isedit, columnNames) {
    return [
        {
            "defaultContent": "-",
            "targets": "_all"
        },
        {
            targets: [columnNames.length],
            'orderable': false,
            sTitle: "Action",
            "data": null,
            className: 'fixed-column'
        },
        {
            "targets": [9, 23, 29],
            render: function (data, type) {
                return formatDate(data, isdateformat); // Common date format function
            }
        },
        {
            "targets": [columnNames.length],
            render: function (data, type, row) {
                if (RoleId == 1 || (RoleId == "6805" && isedit === "True")) {
                    return row.isActive ? getEditButton(row.Empcode, true) : getEditButton(row.Empcode, false);
                }
                return '';
            }
        },
        {
            "targets": 60, // Target the "Status" column
            "createdCell": function (td, cellData, rowData, row, col) {
                let statusClass = ''; // Initialize the class for the cell

                // Determine the class based on the Status value
                if (rowData.Status === 'Active') {
                    statusClass = 'green_font'; // Apply green font class for Active
                } else if (rowData.Status === 'InActive') {
                    statusClass = 'red_font'; // Apply red font class for InActive
                }

                // Add the computed class to the cell
                $(td).addClass(statusClass);
                $(td).addClass('fixed-column-1'); // Add 'fixed-column-1' class for fixed styling
            }
        },
        {
            "targets": [2, 12, 21],
            "createdCell": function (td, cellData, rowData, row, col) {
                $(td).css('min-width', '160px');
            }
        },
        {
            "targets": [3, 10],
            "createdCell": function (td, cellData, rowData, row, col) {
                $(td).css('min-width', '120px');
            }
        },
        {
            "searchable": false,
            "orderable": false,
            "targets": 0
        }
    ];
}

function initTableCustomization(PlanId, RoleId, columnNames, _num, _numArray, tableElement)
{
    
    const tableInstance = $(tableElement).DataTable();
    // Column header update based on PlanId or RoleId
    $('#tblemployee thead th').eq(60).addClass('fixed-column-1');

    if (PlanId == "4") {
        $('#tblemployee tr:eq(0) th:eq(2)').html("EmpName");
    } else {
        $('#tblemployee tr:eq(0) th:eq(2)').html("<a class='btn btn-xs tooltips' title='Columns' onclick='showvisibilityColumn();' id='btnColumnvisibility'><i class='fa-regular fa-columns' style='font-size: 14px; color: #295097;'></i></a> EmpName");
    }

    if (RoleId == 6805 || RoleId == 6806) {
        $('#tblemployee tr:eq(0) th:eq(2)').html("EmpName");
    }

    // Hide specific columns
    _num = _num.replace(', ' + columnNames.length, '');
    tableInstance.columns([_num]).visible(false);

    // Hide loader
    $("#ajax_loaderbranch").hide();

    // Build footer with columns
    var footer = '<tr>';
    columnNames.forEach(function (columnName, index) {
        if (!_numArray.includes(index)) {
            // Add the class for index 60
            if (index === 60) {
                footer += `<th class="fixed-column-1">${columnName}</th>`;
            } else {
                footer += `<th>${columnName}</th>`;
            }
        }
    });
    footer += '<th class="fixed-column"></th></tr>';
    $("#tblemployee thead").append(footer);

    // Add search inputs dynamically
    $("#tblemployee thead tr:eq(1) th").each(function (index) {
        var title = $(this).text();
        if (["EmpName", "Empcode", "EmpPunchID", "BranchName", "Email"].includes(title)) {
            $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search" id=""></i>`);
        } else {
            $(this).html('');
        }
    });

    // Apply the Search for specific columns
    $('.nosearch').parents('th').each(function () {
        tableInstance.column($(this).index()).search('');
    });

    // Apply the Search for all Columns
    $("#tblemployee thead").on("keyup", "input", function () {
        var columnIndex = $(this).parent().index();
        var visibleColumns = tableInstance.columns(':visible').indexes().toArray();
        var actualIndex = visibleColumns[columnIndex];
        tableInstance.column(actualIndex).search(this.value).draw();
    });
}


function NewloadEmployeeGridData(page) {

    let searchCriteria = [];
    var columns = [];
    var isedit = _objview;
    var _columnsname = "";
    var _num = "";
    var _hdrcnt = 0;
    var P = 4;
    var _Cnttd = 0;
    var fieldCount = 1;
    let selectedFields = [];
    var _numArray = [];
    var totalRecordCount = 0;
    var _PagesizeDefault = 50;

    $.ajax({
        type: "POST",
        headers: { 'Authorization': token },
        url: webapiurl + 'DataGridOptimize/GetEmployeeGridData',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 30000,
        data: JSON.stringify(
        {
            searchTerm: $('#searchTerm').val(),
            pageSize: _PagesizeDefault,
            sortColumn: "EmpID",
            sortOrder: "asc",
            page: page
        }),
        beforeSend: function () {
            $("#ajax_loaderEmployee").show();
        },
        success: function (response) {
            if (response.Result != null && response.Result.Items.length > 0)
            {
                $("#ajax_loaderEmployee").hide();

                $("#fieldlst tr td").remove();
                var totalRecordCount = response.Result.TotalItems;
                var data = response.Result.Items;
                columnNames = Object.keys(data[0]);
                //console.log(columnNames);
                _chkarrylst = 0;
                var _CheckcolumnNames = Object.keys(data[0]);
                _num = "0,1,4,5,6,7,8,9,11,13,14,15,16,17,18,19,20,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59";

                _numArray = _num.split(',').map(Number);

                for (var i in _CheckcolumnNames) {
                    if (_hdrcnt > 60) {
                        _num += ", " + _hdrcnt
                    }
                    if (_hdrcnt == "0" || _hdrcnt == "1" || _hdrcnt == "4" || _hdrcnt == "5" || _hdrcnt == "6" || _hdrcnt == "7" || _hdrcnt == "8" || _hdrcnt == "11" || _hdrcnt == "13" || _hdrcnt == "14" || _hdrcnt == "17" || _hdrcnt == "19" || _hdrcnt == "22" || _hdrcnt == "25" || _hdrcnt == "27" || _hdrcnt == "28" || _hdrcnt == "30" || _hdrcnt == "31" | _hdrcnt == "32" || _hdrcnt == "33" || _hdrcnt == "35" || _hdrcnt == "36" || _hdrcnt == "37" || _hdrcnt == "38" || _hdrcnt == "39" || _hdrcnt == "40" || _hdrcnt == "42" || _hdrcnt == "43" || _hdrcnt == "44" || _hdrcnt == "45" || _hdrcnt == "46" || _hdrcnt == "47" || _hdrcnt == "48" || _hdrcnt == "49" || _hdrcnt == "51" || _hdrcnt == "52" || _hdrcnt == "53" || _hdrcnt == "56" || _hdrcnt == "57" || _hdrcnt == "58" || _hdrcnt == "59") {
                    }
                    else {
                        if (_Cnttd == 6) {
                            _Cnttd = 0;
                        }
                        _Cnttd = _Cnttd + 1;
                        if (_hdrcnt == "2" || _hdrcnt == "3" || _hdrcnt == "10" || _hdrcnt == "12" || _hdrcnt == "21" || _hdrcnt == "60") {
                            _chkarrylst += "," + _hdrcnt;
                            $('#tr_' + _Cnttd + '').append("<td  ID=" + _hdrcnt + "><label class='toggle-vis' data-column=" + _hdrcnt + "><input type='checkbox' name=" + columnNames[i] + " class='toggle-vis' data-column=" + _hdrcnt + " ID=chk_" + _hdrcnt + " checked><span> " + columnNames[i] + " </span></label></td>");
                        }
                        else {
                            $('#tr_' + _Cnttd + '').append("<td  ID=" + _hdrcnt + "><label class='toggle-vis' data-column=" + _hdrcnt + "><input type='checkbox' name=" + columnNames[i] + " class='toggle-vis' data-column=" + _hdrcnt + " ID=chk_" + _hdrcnt + "><span> " + columnNames[i] + " </span></label></td>");
                        }
                    }
                    _hdrcnt = _hdrcnt + 1;
                }
                // Check if we need server-side processing
                for (var i in columnNames) {
                    if (columnNames[i] === "EmpMNo")
                    {
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
                    else if (columnNames[i] === "Email") {
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
                _isServerSide = totalRecordCount > 20000;

                if (totalRecordCount > 20000)
                {
                    $('.custom-search-wrapper').show();
                    $('.dataTables_filter').hide(); // Hide the default filter
                }
                else {
                    // Show default DataTable filter if total records <= 20k
                    $('.dataTables_filter').show();
                    $('.custom-search-wrapper').hide();
                }
                // DataTable configuration

                var columnDefs = getColumnDefs(RoleId, isedit, columnNames);

                commonConfig =
                {
                    draw: page,
                    recordsTotal: totalRecordCount,
                    recordsFiltered: totalRecordCount,
                    "destroy": true,
                    dom: "<'row'<'col-lg-3 col-sm-5 col-xs-12'<'record_btn_main'l,><'dt-buttons'Bf>><'col-lg-9 col-sm-7 col-xs-12 mb_margin_bottom_15'<'resignblock text-right'>>>" +
                        "<'row'<'col-sm-12 margin-bottom-10'<'customforms_table EmpNew_tbl_scroll'tr'>>>" +
                        "<'row'<'col-md-5 col-sm-5'i><'col-md-7 col-sm-7'p>>",                   
                    columnDefs: columnDefs,
                    buttons: [
                        {
                            extend: 'excelHtml5',
                            text: 'Excel',
                            titleAttr: 'Excel',
                            exportOptions: {
                                columns: [2, 3, 6, 7, 8, 9, 10, 12, 15, 16, 18, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 31, 36, 39, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60]
                            }
                        }
                    ],
                    lengthmenu: [
                        [10, 25, 50, 100, 500, 700, 1000],
                        [10, 25, 50, 100, 500, 700, 1000],
                    ],
                    "data": data,
                    "columns": columns,
                    pageLength: 50,
                    responsive: true,
                    autoWidth: false,

                    initComplete: function () {
                        initTableCustomization(PlanId, RoleId, columnNames, _num, _numArray, this);
                    },
                };
                ////Working Code/////
                // If server-side processing is needed, adjust the AJAX configuration

                var requestData = "";

                if (_isServerSide)
                {
                    commonConfig.searching = true,
                    commonConfig.ordering = true,
                    commonConfig.paging = false,
                    commonConfig.serverSide = false; // Enable server-side processing

                    

                    // First load data from server
                    fetchServerData(1, function (initialData) {
                        if (initialData.length > 0) {
                            initialData.forEach(row => {
                                t.row.add(row).draw(false); // Add initial rows without resetting the table
                            });
                        }
                    });

                    //$('#tblemployee thead .search_input').each(function ()
                    //{                        
                    //    const columnName = $(this).data('column'); // Use data attribute for column mapping
                    //    const columnValue = $(this).val().trim();

                    //    if (columnValue) {
                    //        searchCriteria.push({ field: columnName, value: columnValue });
                    //    }
                    //});                   
                }
                else
                {
                    commonConfig.serverSide = false; // Disable server-side processing
                    commonConfig.ajax = null; // Use the initial data provided
                    commonConfig.data = data; // Set data directly for client-side processing
                    commonConfig.deferRender = true; // For performance with large data
                }

                // Initialize the DataTable
             
                t = $('#tblemployee').DataTable(commonConfig);

                
                Directory_table = t;


              
                // Fetch Data from the Server
                function fetchServerData(page, callback, draw)
                {
                    const searchCriteria = [];
                    //$('#tblemployee thead .search_input').each(function () {
                    //    const columnName = $(this).data('column'); // Use data attribute for column mapping
                    //    const columnValue = $(this).val().trim();

                    //    if (columnValue) {
                    //        searchCriteria.push({ field: columnName, value: columnValue });
                    //    }
                    //});

                    requestData =
                    {
                        page: page,
                        pageSize: _PagesizeDefault,
                        searchCriteria: searchCriteria.length > 0 ? searchCriteria : null,
                    };
                    $.ajax({
                        url: '/DataGridOptimize/GetPaginatedEmployees',
                        type: 'POST',
                        contentType: 'application/json',
                        headers: { 'Authorization': '@_tokan' },
                        data: JSON.stringify(requestData),
                        beforeSend: function () {
                            $("#ajax_loaderEmployee").show();
                        },
                        success: function (response) {
                            $("#ajax_loaderEmployee").hide();
                            totalRecordCount = response.recordsTotal;

                            if (t)
                            {
                                t.page.info().recordsTotal = totalRecordCount;
                                t.page.info().recordsFiltered = totalRecordCount;
                            }
                            if (callback) {
                                callback(response.data.Items || []);
                            }
                            // Update the custom information display
                            const startRecord = (page - 1) * _PagesizeDefault + 1;
                            const endRecord = Math.min(page * _PagesizeDefault, totalRecordCount);
                            const infoText = `Showing ${startRecord} to ${endRecord} of ${totalRecordCount} entries`;
                            $('#tblemployee_info').html(infoText);
                        },
                        error: function () {
                            console.error('Error fetching data from server');
                            if (callback) {
                                callback([]); // Provide an empty array in case of an error
                            }
                        },
                        complete: function () {
                            $("#ajax_loader").hide(); // Hide the loader
                        }
                    });
                }

                // Lazy Loading Logic on Scroll
                $('.customforms_table.EmpNew_tbl_scroll').on('scroll', function () {
                    const scrollContainer = $(this);
                    const scrollTop = scrollContainer.scrollTop();
                    const scrollHeight = scrollContainer[0].scrollHeight;
                    const clientHeight = scrollContainer[0].clientHeight;

                    if (scrollTop + clientHeight >= scrollHeight - 50 && !t.isLoading) {
                        t.isLoading = true; // Prevent concurrent requests
                        const nextPage = Math.ceil(t.rows().count() / 50) + 1; // Determine the next page

                        // Fetch the next set of data from the server
                        fetchServerData(nextPage, function (newData) {
                            if (newData.length > 0) {
                                newData.forEach(row => {
                                    t.row.add(row).draw(false); // Add rows without resetting the table
                                });
                            }
                            t.isLoading = false; // Allow further loading
                        });
                    }
                });
            }
            else {
                $("#ajax_loaderbranch").hide();
                $('#tblemployee').html("<thead><tr role='row'><th class='ui-state-default sorting' tabindex='0' aria-controls='tblEmployee' rowspan='1' colspan='1' style='width: 87px;' aria-label='EmpName: activate to sort column ascending'> EmpName</th><th class='ui-state-default sorting' tabindex='0' aria-controls='tblEmployee' rowspan='1' colspan='1' style='width: 102px;' aria-label='Empcode: activate to sort column ascending'>Empcode</th><th class='ui-state-default sorting' tabindex='0' aria-controls='tblEmployee' rowspan='1' colspan='1' style='width: 135px;' aria-label='EmpPunchID: activate to sort column ascending'>EmpPunchID</th><th class='ui-state-default sorting' tabindex='0' aria-controls='tblEmployee' rowspan='1' colspan='1' style='width: 135px;' aria-label='BranchName: activate to sort column ascending'>BranchName</th><th class='ui-state-default sorting' tabindex='0' aria-controls='tblEmployee' rowspan='1' colspan='1' style='width: 177px;' aria-label='Email: activate to sort column ascending'>Email</th><th class='ui-state-default sorting' tabindex='0' aria-controls='tblEmployee' rowspan='1' colspan='1' style='width: 76px;' aria-label='Status: activate to sort column ascending'>Status</th><th class='ui-state-default sorting' tabindex='0' aria-controls='tblEmployee' rowspan='1' colspan='1' style='width: 77px;' aria-label='Action: activate to sort column ascending'>Action</th></tr></thead><tbody><tr role='row' class='odd'><td colspan='7'>No data available in table</td></tr></tbody>")

                var footer = '<tr><th style="min-width: 50px; text-align: center;"></th><th style="min-width: 150px">Emp Name</th> <th style="min-width: 100px">Emp Code</th> <th style="min-width: 80px">Punch ID</th> <th style="min-width: 80px">Branch</th> <th style="min-width: 100px">Department</th> <th style="min-width: 100px">Designation</th> <th style="min-width: 80px">Mobile</th> <th style="min-width: 120px">Email</th> <th style="min-width: 100px">Emp Category</th> <th style="min-width: 100px">Emp Type</th> <th style="min-width: 160px">Aadhar Card Number</th> <th style="min-width: 160px">EmpRFID Card</th> <th class="fixed-column-1"></th> <th class="fixed-column"></th> </tr>';
                $("#tblemployee thead").append(footer);

                $("#tblemployee thead tr:eq(1) th").each(function (index) {
                    var totalTh = $(this).parent().children('th').length;
                    var title = $(this).text();
                    if (index == 0) {
                        $(this).html('<input type="checkbox" class="text-right selectall" id="selectall">');
                    }
                    else if (index == totalTh - 2) {
                        $(this).html('');
                    }
                    else {
                        $(this).html('<input type="text" class="search_input" placeholder="Search" /><i class="clm-search" id=""></i>');
                    }
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