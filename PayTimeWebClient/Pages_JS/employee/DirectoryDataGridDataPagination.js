const { debug } = require("util");


function restoreCheckboxState() {
    $('#tbldirectorydetails').find('.ClsChkEmp').each(function () {
        var empPunchID = $(this).val();
        if (checkboxStates[empPunchID]) {
            $(this).prop('checked', true);
        }
    });
}


function loadDirectoryData(page, mainobj) {
    var headerCount = 0;
    var countTd = 0;
    var columns = [];
    var rowCount = 1;
     GlobalObjData = {
        "searchTerm": "",
        "sortColumn": "OnBordingID",  // Default sort column
        "sortOrder": "asc",     // Default sort order
        "pageSize": 10,
        "page": 1,
        "DataGridValues": mainobj.DataGridThreSold,
        "OnBoardingID": mainobj.OnBoardingID,
        "RoleId": mainobj.RoleId,
        "CompanyId": mainobj.CompanyId,
        "BranchId": mainobj.BranchId,
        "LoginEmpId": mainobj.LoginEmpId,
        "DepartmentID": mainobj.DepartmentID,
        "DesignationID": mainobj.DesignationID,
        "DeactivateID": mainobj.DeactivateID,
        "CustomFilters": []
    }
    $.ajax({
        type: "POST",
        //url: _urlcmp,
        url: webapiurl + 'DataGridOptimize/EmployeeDirectoryPaginatList',
        headers: { 'Authorization': tokan },
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(GlobalObjData),
        beforeSend: function () {
            $(".loading").show();
        },
        success: function (response)
        {
            if (response.Table.length==0) {
                const wrapper = $(".filter_portlet_wrapper");
                wrapper.removeClass("full-width");
            }
            if (response != null) {
                const payrollC = response.Table;
                handleGridVisibility("tblempty", "NotificationGrid-empty", payrollC, {
                    imageUrl: "emptyscreen_3.png",
                    subText: "No matching records found for the selected filters."
                });
            }
            if (response != null || response != undefined || response != null || response != [])
            {
                var totalRecordCount = response.Table1[0].TotalRecords;
                var data = response.Table;
                _isServerSide = totalRecordCount > DataGridThreSold;              
                var _numArray = [];
                var columnNames = [];
                if (response.Table.length == 0) {
                    checkedColumnsList = [];
                    $('#tbldirectorydetails').html('<thead><tr><th>All</th> <th>Emp Name</th> <th>Emp Code</th> <th>Punch ID</th> <th>Branch</th> <th>Department</th> <th>Designation</th> <th>Mobile</th> <th>Email</th> <th>Emp Category</th> <th>Emp Type</th> <th>Aadhar Card Number</th> <th style="min-width: 160px">EmpRFID Card</th> <th class="fixed-column-1">Status</th> <th class="fixed-column">Action</th> </tr></thead>');

                    var footer = '<tr><th style="min-width: 50px; text-align: center;"></th><th style="min-width: 150px">Emp Name</th> <th style="min-width: 100px">Emp Code</th> <th style="min-width: 80px">Punch ID</th> <th style="min-width: 80px">Branch</th> <th style="min-width: 100px">Department</th> <th style="min-width: 100px">Designation</th> <th style="min-width: 80px">Mobile</th> <th style="min-width: 120px">Email</th> <th style="min-width: 100px">Emp Category</th> <th style="min-width: 100px">Emp Type</th> <th style="min-width: 160px">Aadhar Card Number</th> <th style="min-width: 160px">EmpRFID Card</th> <th class="fixed-column-1"></th> <th class="fixed-column"></th> </tr>';

                    $("#tbldirectorydetails thead").append(footer);

                    $("#tbldirectorydetails thead tr:eq(1) th").each(function (index) {
                        var totalTh = $(this).parent().children('th').length;
                        var title = $(this).text().trim();
                        if (index == 0) {
                            $(this).html('<input type="checkbox" class="text-right selectall" id="selectall">');
                        } else if (index == totalTh - 2) {
                            $(this).html('');
                        } else if (["Emp Name", "Joining Date", "Emp Code", "Punch ID", "Branch", "Department", "Designation", "Mobile", "Email", "Bank Account Number", "IFSCCode", "Aadhar Card Number", "EmpRFIDCard", "IDCardNumber"].includes(title)) {
                            $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search" id=""></i>`);
                        } 
                    });
                }
                else {
                    //if (DeactivateID > 0)
                    //{
                    //    $('#btnResignAll').removeClass('disabled');
                    //}

                    $("#fieldlst tr td").remove();
                    _chkarrylst = 0;
                    columnNames = Object.keys(response.Table[0]);
                   

                    var _CheckcolumnNames = Object.keys(response.Table[0]);
                    $('#fieldlst').append('<tr id="tr_' + rowCount + '"></tr>');


                    // this _num is used to hide the datatable.
                    var _num = "1,3,4,7,8,9,11,13,15,16,17,18,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,96,97,98,99,100,101,102,103,104,105,106,107,108,109,111,113,114,115,117,118,119";
                    _numArray = _num.split(',').map(Number);
                    for (var i in _CheckcolumnNames) {
                        var columnClass = "";
                        if (countTd == 4) {
                            countTd = 0;
                            rowCount++;
                            $('#fieldlst').append('<tr id="tr_' + rowCount + '"></tr>');
                        }
                        //Hide Columns List in Selected Columns PopUp.
                        if (i == "0" || i == "1" || i == "4" || i == "8" || i == "7" || i == "9" || i == "11" || i == "13" || i == "15" || i == "17" || i == "47" || i == "48" || i == "54" || i == "55" || i == "72" || i == "73" || i == "75" || i == "76" || i == "54" || i == "71" || i == "72" || i == "74" || i == "75" || i == "76" || i == "77" || i == "79" || i == "82" || i == "83" || i == "85" || i == "86" || i == "88" || i == "90" || i == "92" || i == "96" || i == "97" || i == "100" || i == "103" || i == "104" || i == "105" || i == "108" || i == "111" || i == "114" || i == "116" || i == "117" || i == "118" || i == "119") {
                            countTd--;
                        }
                        else {
                            if (i == "2" || i == "112" || i == "94" || i == "5" || i == "95" || i == "6" || i == "10" || i == "12" || i == "14" || i == "19" || i == "20" || i == "110" || i == "114") {
                                checkedColumnsList += "," + i;
                                $('#tr_' + rowCount + '').append("<td  ID=" + headerCount + "  class='" + columnClass + "'><label class='toggle-vis' data-column=" + headerCount + "><input type='checkbox' name= '" + columnNames[i] + "'class='toggle-vis' data-column=" + headerCount + " ID=chk_" + headerCount + " checked><span> " + columnNames[i] + " </span></label></td>");
                            }
                            else {
                                $('#tr_' + rowCount + '').append("<td  ID=" + headerCount + " class='" + columnClass + "'><label class='toggle-vis' data-column=" + headerCount + "><input type='checkbox' name= '" + columnNames[i] + "' class='toggle-vis' data-column=" + headerCount + " ID=chk_" + headerCount + "><span> " + columnNames[i] + " </span></label></td>");
                            }
                        }
                        countTd = countTd + 1;
                        headerCount = headerCount + 1
                        columns.push({
                            data: columnNames[i],
                            title: columnNames[i]
                        });
                    }
                }
                $('#tbldirectorydetails').destroy;
                if (response.Table.length > 0)
                {                   
                    
                    commonConfig =
                    {

                        "destroy": true,
                        //dom: "<'row'<'col-lg-3 col-sm-5 col-xs-12'<'record_btn_main'l,><'dt-buttons'Bf>><'col-lg-9 col-sm-7 col-xs-12 mb_margin_bottom_15'<'resignblock text-right'>>>" +
                        //    "<'row'<'col-sm-12 margin-bottom-10'<'customforms_table EmpNew_tbl_scroll'tr'>>>" +
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
                                "defaultContent": "",
                                "targets": "_all"
                            },                           
                            {
                                "targets": [columnNames.length],
                                'orderable': false,
                                sTitle: "Action",
                                "data": null
                            },
                            {
                                "targets": 116,
                                'orderable': false                                 
                            },
                            {
                                "targets": [columnNames.length],
                                render: function (data, type, row) {
                                    var html = '';
                                    if (data.Status == 'Active') {
                                        html = "<a class='editclass btn seagreen_btnnew tooltips' id='btnEdit' data-placement='left' data-original-title='Edit' ><i class='fa-regular fa-pen-to-square'></i></a>";
                                        html += "<a class='Inactiveclass btn red_btnnew tooltips' id='' data-placement='left' data-original-title='Inactive' style='margin-left: 0 !important;'><i class='fa-solid fa-xmark'></i></a>";
                                    } else if (data.Status == 'InActive') {
                                        html = "<a class='editclass btn seagreen_btnnew tooltips' id='btnEdit' data-placement='left' data-original-title='Edit' disabled><i class='fa-regular fa-pen-to-square'></i></a>";
                                        html += "<a class='activeclass btn green_btnnew tooltips' id='' data-placement='left' data-original-title='Active'><i class='fa-solid fa-check'></i></a>";
                                    }
                                    return html;
                                }
                            },
                            {
                                "targets": 0,
                                "orderable":false,
                                render: function (data, type, row) {
                                    var html = '';
                                    if (row.Status == 'Active') {
                                        html = "<td> <input type='checkbox' class='ClsChkEmp nosearch' value='" + row.EmpId + "'></td>";
                                    } else if (row.ActiveStatus == 'InActive') {
                                        html = '';
                                    }
                                    return html;
                                }

                            },
                            {                               
                                "targets": [0, 7, 11,15, 16, 17, 18,21,22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32,33,34, 35,36, 37, 38, 39, 40],
                                 "orderable":!_isServerSide

                            },{
                                "targets": [41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66],
                                "orderable": !_isServerSide

                            },{
                                "targets": [67,68,69,70,71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96],
                                "orderable": !_isServerSide

                            },{
                                "targets": [97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 114, 115, 117, 119, 120],
                                "orderable": !_isServerSide

                            },                           
                            {
                                "targets": [3, 39, 45],
                                render: function (data, type, row) {
                                    if (data != null) {
                                        var date1 = data.split(' ')[0];
                                        var date = date1.split('-');
                                        if (_isdateformat == "dd-mm-yyyy") {
                                            var firstValue = date[2] + "-" + date[1] + "-" + date[0] //yyyy-mm-dd
                                        }
                                        else if (_isdateformat == "yyyy-mm-dd") {
                                            var firstValue = date[0] + "-" + date[1] + "-" + date[2] //yyyy-mm-dd
                                        }
                                        else if (_isdateformat == "mm-dd-yyyy") {
                                            var firstValue = date[1] + "-" + date[2] + "-" + date[0] //yyyy-mm-dd
                                        }
                                        else if (_isdateformat == "yyyy-M-dd") {
                                            var _date = new Date(date[1] + "-" + date[2] + "-" + date[0]);
                                            var locale = "en-us";
                                            var month = _date.toLocaleString(locale, { month: "short" });
                                            var firstValue = date[0] + "-" + month + "-" + date[2] //yyyy-mm-dd
                                        }
                                        else if (_isdateformat == "M-dd-yyyy") { //done
                                            var _date = new Date(date[1] + "-" + date[2] + "-" + date[0]);
                                            var locale = "en-us";
                                            var month = _date.toLocaleString(locale, { month: "short" });
                                            var firstValue = month + "-" + date[2] + "-" + date[0] //yyyy-mm-dd
                                        }
                                        else if (_isdateformat == "dd-M-yyyy") {
                                            var _date = new Date(date[1] + "-" + date[2] + "-" + date[0]);
                                            var locale = "en-us";
                                            var month = _date.toLocaleString(locale, { month: "short" });
                                            var firstValue = date[2] + "-" + month + "-" + date[0]  //yyyy-mm-dd
                                        }
                                    }
                                    return firstValue;
                                }
                            },
                            {
                                
                                "targets": 116,
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    var statusClass = ''; // Initialize the variable for the status class
                                    if (rowData.Status == 'Active') {
                                        statusClass = 'green_font';
                                    } else if (rowData.Status == 'InActive') {
                                        statusClass = 'red_font';
                                    }
                                    $(td).addClass(statusClass); // Add the class to the cell
                                    $(td).addClass('fixed-column-1');
                                }
                            },
                            {
                                "targets": 120,
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    $(td).addClass('fixed-column');
                                    $(td).css('min-width', '80px');
                                }
                            },
                            {
                                "targets": 0,
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    $(td).css('min-width', '50px');
                                }
                            },
                            {
                                "targets": 2,
                                "render": function (data, type, row) {
                                    var html = '';
                                    var nameParts = data.replace(/\s+/g, ' ').trim().split(" ");
                                    var initials = nameParts[0].charAt(0).toUpperCase();
                                    if (nameParts.length > 1) {
                                        initials += nameParts[1].charAt(0).toUpperCase();
                                    }
                                    const colors = ["#e74c3c", "#8e44ad", "#3498db", "#16a085", "#f39c12", "#d35400", "#2c3e50"];
                                    //const randomColor = colors[Math.floor(Math.random() * colors.length)];
                                    const randomColor = "#295097";
                                    html = `<div class="emp_box">
                                            <img alt="" class="user_img zoom-image img-circle tooltips" data-placement="right" data-original-title="Punch ID: ${row["Punch ID"]}" src="/UploadEmpPhoto/${row.EmployeePhoto.trim()}"  style="border-radius: 50%!important;width:34px;height:34px;"
                                            onerror="this.style.display='none'; this.nextElementSibling.style.display='inline-block';"/>
                                            <span class="user_img tooltips" data-placement="right" data-original-title="Punch ID : ${row["Punch ID"]}" style="display: none; min-width: 30px; height: 30px; line-height: 30px; text-align: center; background-color: #ecf3ff; color:  ${randomColor}; border-radius: 50% !important; border: 1px solid #8ab1f7; margin-right: 8px;">
                                            ${initials}</span> ${data}</div>`;
                                    //if (row.EmployeePhoto == "" || row.EmployeePhoto == null) {
                                    //    html = '<div class="emp_box"><img src="/assets/images/avtar.png" alt="User icon" class="user_img"><span><p>' + data + '</p></span></div>';
                                    //} else {
                                    //    html = '<div class="emp_box"><img src="/UploadEmpPhoto/' + row.EmployeePhoto.trim() + '" style="border-radius: 50%!important;width:34px;height:34px;" class="zoom-image"; alt="User icon" class="user_img"><span><p>' + data + '</p></span></div>';
                                    //}
                                    return html;
                                },
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    $(td).css('min-width', '150px');
                                }
                            },
                            {
                                "targets": 110,
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    $(td).css('min-width', '160px');
                                }
                            },
                            {
                                "targets": [5, 12, 14, 94, 95],
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    $(td).css('min-width', '100px');
                                }
                            },

                            {
                                "targets": [6, 10, 19],
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    $(td).css('min-width', '80px');
                                }
                            },
                            {
                                "targets": 20,
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    $(td).css('min-width', '120px');
                                }
                            }

                        ],
                        pageLength: 10,
                        responsive: true,
                        autoWidth: false,
                        initComplete: function (settings, json)
                        {                            
                            $('#tbldirectorydetails thead th').eq(116).addClass('fixed-column-1');
                            $('#tbldirectorydetails thead th').eq(120).addClass('fixed-column');

                            //var footer = '<tr><th>All</th><th>Emp Name</th><th>Joining Date</th><th>Emp Code</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 80px;">Branch</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 100px;">Department</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 100px;">Designation</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 68px;">Shift Name</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 107px;">ShiftGroup Name</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 80px;">Mobile</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 135px;">Email</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 79px;">Father Name</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 86px;">Place Of Birth</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 100px;">Age On Date</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 67px;">Nationality</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 80px;">Blood Group</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 115px;">Birth Identification</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 111px;">Physical Ailments</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 89px;">Ref One Name</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 95px;">Ref One Mobile</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 105px;">Ref One Address</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 89px;">Ref Two Name</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 95px;">Ref Two Mobile</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 105px;">Ref Two Address</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 102px;">Nominee1 Name</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 86px;">Nominee1 Rel</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 133px;">Nominee1 Proportion</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 118px;">Nominee1 Address</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 98px;">Nominnee1 Age</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 95px;">Nominee1 DOB</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 102px;">Nominee2 Name</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 86px;">Nominee2 Rel</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 133px;">Nominee2 Proportion</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 118px;">Nominee2 Address</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 90px;">Nominee2 Age</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 95px;">Nominee2 DOB</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 147px;">Criminal Record In Past</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 84px;">Notice Period</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 71px;">Bank Name</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 140px;">Bank Account Number</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 75px;">IFSCCode</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 87px;">Bank Address</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 109px;">PermanentVillage</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 129px;">PermanentLandMark</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 108px;">PermanentTaluka</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 112px;">PermanentDistrict</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 99px;">PermanentState</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 118px;">PermanentPincode</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 133px;">PermanentPostOffice</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 150px;">PermanentPoliceStation</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 90px;">PresentVillage</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 110px;">PresentLandMark</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 88px;">PresentTaluka</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 93px;">PresentDistrict</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 80px;">PresentState</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 99px;">PresentPincode</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 113px;">PresentPostOffice</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 46px;">Gender</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 82px;">MaritalStatus</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 76px;">StatusName</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 109px;">PunchType Name</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 137px;">Multiple Branch Name</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 77px;">ReportingTo</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 78px;">Policy Name</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 81px;">Date Of Birth</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 100px;">Emp Category</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 100px;">Emp Type</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 73px;">Emp Height</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 73px;">Emp weight</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 144px;">Ref One Pan Card Num</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 157px;">Ref One Adhar Card Num</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 157px;">Ref Two Adhar Card Num</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 143px;">Ref Two Pan Card Num</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 77px;">MonthlyCTC</th><th class="ui-state-default sorting_disabled" rowspan="1" colspan="1" style="width: 160px;">Aadhar Card Number</th><th class="ui-state-default sorting_disabled fixed-column-1" rowspan="1" colspan="1" style="width: 80px;">Status</th><th class="ui-state-default sorting_disabled fixed-column" rowspan="1" colspan="1" style="width: 100px;">Action</th></tr>';

                            var footer = '<tr>';
                            columnNames.forEach(function (columnName, index) {
                                if (_isServerSide && !_numArray.includes(index) && ["Emp Name", "Joining Date", "Emp Code", "Punch ID", "Branch", "Department", "Designation", "Mobile", "Email", "Bank Account Number", "IFSCCode", "Aadhar Card Number", "EmpRFIDCard", "IDCardNumber"].includes(columnName)) {
                                    footer += `<th>${columnName}</th>`;
                                }
                                else if (!_isServerSide && !_numArray.includes(index)) {
                                    footer += `<th>${columnName}</th>`;
                                }
                                else if (!_numArray.includes(index)) {
                                    footer += `<th></th>`;
                                }
                            });
                            footer += '<th class="fixed-column"></th></tr>';

                            $("#tbldirectorydetails thead").append(footer);

                            $("#tbldirectorydetails thead tr:eq(1) th").each(function (index) {
                                var title = $(this).text();
                                if (index == 0) {
                                    $(this).html('');
                                    $(this).html('<input type="checkbox" class="selectall" id="selectall">');
                                }
                                else if (_isServerSide && ["Emp Name", "Joining Date", "Emp Code", "Punch ID", "Branch", "Department", "Designation", "Mobile", "Email", "Bank Account Number", "IFSCCode", "Aadhar Card Number", "EmpRFIDCard","IDCardNumber"].includes(title)) {
                                    $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search" id=""></i>`);
                                }
                                else if (!_isServerSide) {
                                    $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search" id=""></i>`);
                                }
                                if (index == 13) {
                                    $(this).addClass('fixed-column-1').find('input,i').remove();
                                }
                            });

                            $('.nosearch').parents('th').each(function () {
                                Directory_table.column($(this).index()).search('');
                            });

                            $('#tbldirectorydetails tr:eq(0) th:eq(0)').html("<a class='btn btn-xs' onclick='showvisibilityColumn();' id='btnColumnvisibility'><i class='fa-regular fa-columns tooltips' data-placement='right' title='Columns' style='font-size: 14px; color: #295097;'></i></a> All");
                            if (DeactivateID > 0) {
                                $('.resignblock').html('<a id="btnResignAll" class="btn add_btn">Deactivate All</a>');
                            } else {
                                $('.resignblock').html('<a id="btnResignAll" class="btn add_btn" disabled>Deactivate All</a>');
                            }
                            $('#tbldirectorydetails').DataTable().columns([_num]).visible(false);

                            // Apply the Search
                            $("#tbldirectorydetails thead").on("keyup", "input", function () {
                                var columnIndex = $(this).parent().index();

                                if ($(this).closest('tr').index() === 1 && columnIndex === 0) {
                                    return;
                                }
                                var visibleColumns = Directory_table.columns(':visible').indexes().toArray();
                                var actualIndex = visibleColumns[columnIndex];

                                Directory_table.column(actualIndex).search(this.value).draw();
                            });

                            $('.ClsChkEmp').change(function () {
                                var allChecked = true;
                                checkedCount = $('.ClsChkEmp:checked').length;
                                $('.ClsChkEmp').each(function () {
                                    if (!$(this).prop('checked')) {
                                        allChecked = false;
                                        return false;
                                    }
                                });
                                $('.selectall').prop('checked', allChecked);
                            });

                            //Export Excel Logic
                            if ($('.dt-buttons #Export').length === 0) {
                                // Clear existing dt-buttons content to avoid duplication (if necessary)
                                $('.dt-buttons').empty();
                                // Append custom export button for server-side processing
                                const exportButton = '<button id="Export" class="dt-button buttons-excel buttons-html5" title="Excel"><span>Excel</span></button>';
                                //const exportButton = '<button type="button" id="Export" class="dwicon_btn tooltips" data-toggle="tooltip" data-placement="top" data-original-title="Download Excel"><i class="fa-light fa-file-excel"></i></button>';
                                $(".dt-buttons").append(exportButton);

                                // Initialize tooltips (this is initialized before the button is appended)
                                $('[data-toggle="tooltip"]').tooltip();  // Potential issue: tooltip may not apply to the new button

                                // Add event listener for the custom button
                                $('#Export').on('click', function () {
                                    toastr.info("Preparing the Excel file for download...");

                                    setTimeout(function () {
                                        generateExcelFile();
                                    }, 1000);

                                });
                            }

                        },
                        drawCallback: function (settings) {
                            $(".loading").hide();

                            restoreCheckboxState();
                            $("#selectall").prop('checked', false)

                            $('#tbldirectorydetails').off('change', '.ClsChkEmp').on('change', '.ClsChkEmp', function () {
                                var empPunchID = $(this).val();
                                checkboxStates[empPunchID] = $(this).prop('checked');
                          
                            });

                           // var _exportflage = "directoryMaster1";
                            //injectExportButtonsForDataTables(_isServerSide, function () {
                            //    generateExcelFile();
                            //});
                        }

                         

                    }

                    if (_isServerSide)
                    {
                        commonConfig.searching = true,
                        commonConfig.ordering = true,
                        commonConfig.paging = true,
                        commonConfig.serverSide = true;
                        commonConfig.processing = false;                   

                        // Enable server-side processing
                        commonConfig.ajax =
                        {
                            url: '/DataGridOptimize/GetPaginatedEmployeeDirectory',
                            type: 'POST',
                            contentType: "application/json",
                            beforeSend: function () {                                
                                $(".loading").show();
                            },
                            data: function (d) {
                                //let combinedSearchValue = '';
                                searchCriteria = [];
                                var field = '';
                                var value = '';

                                $('#tbldirectorydetails thead .search_input').each(function () {
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

                                // Add search_input values to the `search.value`
                                //d.search.value = combinedSearchValue || d.search.value || '';                           
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
                                    "OnBoardingID": mainobj.OnBoardingID,
                                    "RoleId": mainobj.RoleId,
                                    "CompanyId": mainobj.CompanyId,
                                    "BranchId": mainobj.BranchId,
                                    "LoginEmpId": mainobj.LoginEmpId,
                                    "DepartmentID": mainobj.DepartmentID,
                                    "DesignationID": mainobj.DesignationID,
                                    "DeactivateID": mainobj.DeactivateID,
                                };
                                return JSON.stringify(requestData);
                            },
                            dataSrc: function (json)
                            {
                                if (json.data)
                                {
                                    $(".loading").hide();
                                    return json.data; // Return the data array if it exists
                                }
                                return [];
                            },
                            complete: function ()
                            {
                                $(".loading").hide();                                
                            }
                        };
                    }
                    else {
                        commonConfig.serverSide = false; // Disable server-side processing
                        commonConfig.ajax = null; // Use the initial data provided
                        commonConfig.data = data; // Set data directly for client-side processing
                        commonConfig.deferRender = true;
                        // For performance with large data
                    }
                    t = $('#tbldirectorydetails').DataTable(commonConfig); // Initialize the DataTable
                    Directory_table = t;
                    $('#tbldirectorydetails').on('draw.dt', function () {
                        $('.tooltips').tooltip();
                    });

                    $('.tooltips').tooltip();

                    setTimeout(function () {                        
                        $("#" + "tbldirectorydetails" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive"));
                        $('.tooltips').tooltip();
                        injectExportButtonsForDataTables(_isServerSide, function () {
                            generateExcelFile();
                        });
                    }, 100);


                } else {
                    $("#tbldirectorydetails").DataTable({
                        //dom: "<'row'<'col-lg-3 col-sm-5 col-xs-12'<'record_btn_main'l,><'dt-buttons'Bf>><'col-lg-9 col-sm-7 col-xs-12 mb_margin_bottom_15'<'resignblock text-right'>>>" +
                        //    "<'row'<'col-sm-12 margin-bottom-10'<'customforms_table EmpNew_tbl_scroll'tr'>>>" +
                        //    "<'row'<'col-md-5 col-sm-5'i><'col-md-7 col-sm-7'p>>",
                        dom: _domColumn,
                        language: _languageColumn,
                        paging: true,
                        autoWidth: true,
                        ordering: false,
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
                        initComplete: function (settings, json) {
                            $('.resignblock').html('<a id="btnResignAll" class="btn add_btn" disabled>Deactivate All</a>');
                        },
                        drawCallback: function (settings) {
                            $(".loading").hide();
                        }

                    });
                    setTimeout(function () {
                        $("#" + "tbldirectorydetails" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive"));
                        $('.tooltips').tooltip();
                        injectExportButtonsForDataTables(_isServerSide, function () {
                            generateExcelFile();
                        });
                        $('.tooltips').tooltip();
                    }, 100);
                }
            }
            else {

            }

        },
        complete: function () {
            //$(".loading").hide();
        },
        error: function (xhr, status, error) {
            console.log("AJAX error:", status, error);
            // This will allow you to catch errors in the response.
            // This will allow you to catch errors in the response.
        }
    });
}

function generateExcelFile() {   
    checkSession();

    $.ajax({
        url: '/DataGridOptimize/GetAllDirectoryExcelData',
        type: 'POST',
        data: JSON.stringify(GlobalObjData),
        contentType: 'application/json',
        beforeSend: function () {
            $("#ajax_loader").show(); // Show loader before sending the request
        },
        success: function (response) {
            if (response.success && response.filePath) {
              /*  toastr.success("Export completed. Starting download...");*/
                // Trigger the download using the returned file path
                window.location.href = response.filePath;
            } else {
                toastr.error(response.message || "Failed to generate export file.");
            }

        },
        error: function () {
            toastr.error("An error occurred while exporting data.");
        },
         complete: function () {
             $("#ajax_loader").hide(); // Hide loader after request completes
        }
    });
}
