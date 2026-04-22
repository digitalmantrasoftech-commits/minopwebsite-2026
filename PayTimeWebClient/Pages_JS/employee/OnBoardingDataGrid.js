


function loadOnBoardingData(page, mainobj) {
    table = $('#tblonboardingdetails');

    // Clear previous table data and headers
    table.DataTable().clear().destroy();
    table.find("tbody").empty();
    table.find("thead tr:eq(1)").remove();
    var tableData = [];

    var columnNames = [];
    var columnsdata = [];

    ObjData = {
        searchTerm: "",
        sortColumn: "Tf.OnBordingID",
        sortOrder: "asc",
        pageSize: 10,
        page: page,
        DataGridValues: mainobj.DataGridThreSold,
        OnBoardingID: mainobj.OnBoardingID,
        RoleId: mainobj.RoleId,
        CompanyID: mainobj.CompanyID,
        BranchID: mainobj.BranchID,
        LoginEmpId: mainobj.LoginEmpId,
        DeaprtmentID: mainobj.DeaprtmentID,
        DesignationID: mainobj.DesignationID,
        ExportFlage: 0,
        CustomFilters: []
    };
    // AJAX request to fetch onboarding data
    $.ajax({
        type: "POST",
        headers: { 'Authorization': token },
        url: webapiurl + "DataGridOptimize/EmployeeOnBoardingPaginatList",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 30000,
        data: JSON.stringify(ObjData),
        beforeSend: function () {
            $(".ajax_loader").show();
        },
        success: function (response) {
            if (response.Table.length == 0) {
                const wrapper = $(".filter_portlet_wrapper");
                wrapper.removeClass("full-width");
            }
            if (response != null) {
                const list = response.Table;
                handleGridVisibility("tblempty", "NotificationGrid-empty", list, {
                    imageUrl: "emptyscreen_1.png",
                    subText: " No entries found. Please add a record to get started."
                });
            }
            if (response && response.Table && response.Table.length > 0) {
                // Handle education and family data
                //var eduMap = new Map();
                if (response.Table1) {
                    response.Table1.forEach(function (rowData) {
                        // Make sure that the mapping holds an array for cases where multiple education records exist for one OnBordingID
                        if (!eduMap.has(rowData.BordingID)) {
                            eduMap.set(rowData.BordingID, []);
                        }
                        eduMap.get(rowData.BordingID).push({
                            'EduID': rowData.EduID,
                            'UniversityName': rowData.UniversityName,
                            'Percentage': rowData.Percentage,
                            'FromYear': rowData.FromYear,
                            'ToYear': rowData.ToYear,
                            'Specialization': rowData.Specialization,
                        });
                    });
                }

                //var familyMap = new Map();
                if (response.Table2) {
                    response.Table2.forEach(function (rowData) {
                        // Make sure that the mapping holds an array for cases where multiple education records exist for one OnBordingID
                        if (!familyMap.has(rowData.BordingID)) {
                            familyMap.set(rowData.BordingID, []);
                        }
                        familyMap.get(rowData.BordingID).push({
                            'FID': rowData.FID,
                            'Fname': rowData.Fname,
                            'Relation': rowData.Relation,
                            'Occupation': rowData.Occupation,
                            'Age': rowData.Age,
                        });

                    });
                }

                // Process the main data and prepare table rows
                tableData = response.Table.map(function (rowData) {
                    var OnBordingID = parseInt(rowData.OnBordingID, 10);
                    return {
                        'OnBordingID': rowData.OnBordingID,
                        'EmployeeCode': rowData.EmployeeCode,
                        'StatusID': rowData.StatusID,
                        'EmployeeTypeID': rowData.EmployeeTypeID,
                        'EmployeeCategoryID': rowData.EmployeeCategoryID,
                        'EmpCompanyID': rowData.CompanyID,
                        'EmpBranchID': rowData.BranchID,
                        'EmpDepartmentID': rowData.DepartmentID,
                        'EmpDesignationID': rowData.DesignationID,
                        'EmpRoleID': rowData.RoleID,
                        'EmpReportingPersonID': rowData.ReportingTo,
                        'EmpShiftID': rowData.ShiftID,
                        'EmpShiftGroupID': rowData.ShiftGroupId,
                        'EmpNoticePeriod': rowData.NoticePeriod,
                        'EmpPolicyID': rowData.PolicyID,
                        'EmpPunchID': rowData.PunchID,
                        'EmpPunchType': rowData.PunchTypeID,
                        'EmpResignDate': rowData.ResignDate,
                        'EmpGeoEnable': rowData.GeoEnable,
                        'EmpIsSMSAllow': rowData.IsSMSAllow,
                        'NameOfApplicant': rowData.NameOfApplicant,
                        'FatherName': rowData.FatherName,
                        'PlaceOfBirth': rowData.PlaceOfBirth,
                        'DateOfBirth': rowData.DateOfBirth,
                        'AgeOnDate': rowData.AgeOnDate,
                        'Nationality': rowData.Nationality,
                        'BloodGroup': rowData.BloodGroup,
                        'EmployeeHeight': rowData.EmployeeHeight,
                        'EmpWeight': rowData.EmployeeWeight,
                        'EmpBirthMarkIdentification': rowData.BirthMarkIdentification,
                        'EmpPhysicalAilment': rowData.PhysicalAilment,
                        'EmpMobileNumber': rowData.MobileNumber,
                        'EmailID': rowData.EmailID,
                        'CriminalRecordInPast': rowData.CriminalRecordInPast,
                        'ReferenceOneName': rowData.ReferenceOneName,
                        'ReferenceOneMobileNumber': rowData.ReferenceOneMobileNumber,
                        'ReferenceOneAddress': rowData.ReferenceOneAddress,
                        'AadharCardNumber': rowData.AadharCardNumber,
                        'PancardNumber': rowData.PancardNumber,
                        'AadharCardPhoto': rowData.AadharCardPhoto,
                        'PanCardPhoto': rowData.PanCardPhoto,
                        'ReferenceTwoName': rowData.ReferenceTwoName,
                        'ReferenceTwoMobileNumber': rowData.ReferenceTwoMobileNumber,
                        'ReferenceTwoAddress': rowData.ReferenceTwoAddress,
                        'AadharCardTwoNumber': rowData.AadharCardTwoNumber,
                        'PancardTwoNumber': rowData.PancardTwoNumber,
                        'AadharCardTwoPhoto': rowData.AadharCardTwoPhoto,
                        'PanCardTwoPhoto': rowData.PanCardPhoto,
                        'Nominee1Name': rowData.Nominee1Name,
                        'Nominee1Relationship': rowData.Nominee1Relationship,
                        'Nominee1Proportion': rowData.Nominee1Proportion,
                        'Nominee1Address': rowData.Nominee1Address,
                        'Nominee1Age': rowData.Nominee1Age,
                        'Nominee1DOB': rowData.Nominee1DOB,
                        'Nominee2Name': rowData.Nominee2Name,
                        'Nominee2Relationship': rowData.Nominee2Relationship,
                        'Nominee2Proportion': rowData.Nominee2Proportion,
                        'Nominee2Address': rowData.Nominee2Address,
                        'Nominee2Age': rowData.Nominee2Age,
                        'Nominee2DOB': rowData.Nominee2DOB,
                        'BankAccountNumber': rowData.BankAccountNumber,
                        'BankName': rowData.BankName,
                        'IFSCCode': rowData.IFSCCode,
                        'BankAddress': rowData.BankAddress,
                        'PassbookPhoto': rowData.PassbookPhoto,
                        'CheckbookPhoto': rowData.CheckbookPhoto,
                        'PermanentVillage': rowData.PermanentVillage,
                        'PermanentLandMark': rowData.PermanentLandMark,
                        'PermanentTaluka': rowData.PermanentTaluka,
                        'PermanentDistrict': rowData.PermanentDistrict,
                        'PermanentState': rowData.PermanentState,
                        'PermanentPincode': rowData.PermanentPincode,
                        'PermanentNearPostOffice': rowData.PermanentNearPostOffice,
                        'PermanentNearPolishStation': rowData.PermanentNearPolishStation,
                        'PresentVillage': rowData.PresentVillage,
                        'PresentLandMark': rowData.PresentLandMark,
                        'PresentTaluka': rowData.PresentTaluka,
                        'PresentDistrict': rowData.PresentDistrict,
                        'PresentState': rowData.PresentState,
                        'PresentPincode': rowData.PresentPincode,
                        'PresentNearPostOffice': rowData.PresentNearPostOffice,
                        'PresentNearPolishStation': rowData.PresentNearPolishStation,
                        'PaystuctureID': rowData.PaystuctureID,
                        'DeviceSrNo': rowData.DeviceSrNo,
                        'EmployeePhoto': rowData.EmployeePhoto,
                        'EducationDetails': eduMap.get(OnBordingID) || [],  // Add educational data if available
                        'FamilyDetails': familyMap.get(OnBordingID) || [], // Add educational data if available
                        'MultipleBranchID': rowData.MultipleBranchID,
                        'BranchName': rowData.BranchName,
                        'DepartmentName': rowData.DepartmentName,
                        'EmployeeCode': rowData.EmployeeCode,
                        'JoiningDate': rowData.JoiningDate,
                        'MonthlyCTC': rowData.MonthlyCTC,
                        'EmpAdharCardNumber': rowData.EmpAdharcardInfo,
                        'EmpPanCardNumber': rowData.EmpPancardInfo,
                        'EmpAdharCardPhotoDetails': rowData.AdharCardPhotoInfo,
                        'EmpPancardPhotoDetails': rowData.PanCardPhotoInfo,
                        'StatusName': rowData.StatusName,
                        'GenderID': rowData.GenderID,
                        'MaritalStatusID': rowData.MaritalStatusID,
                        'EmpRFIDCard': rowData.EmpRFIDCard,
                        'EnrollUserDeviceID': rowData.EnrollUserDeviceID,
                        'RejectReason': rowData.RejectReason,
                        'IDCardNumber': rowData.IDCardNumber,
                        'GatePassPolicyID': rowData.GatePassPolicyID,
                    };
                });

                columnNames = response.Table.length > 0 ? Object.keys(response.Table[0]) : [];
                

                totalRecordCount = response.Table3 && response.Table3[0] ? response.Table3[0].TotalRecords : 0;
                isServerSide = totalRecordCount > mainobj.DataGridThreSold;             

                var hiddenColumns = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 92, 94, 95, 96, 97, 98, 99, 102, 103];

                // Adjust search and filter visibility based on record count
                if (isServerSide) {
                    $('.custom-search-wrapper').show();
                    $('.dataTables_filter').hide();
                } else {
                    $('.dataTables_filter').show();
                    $('.custom-search-wrapper').hide();
                }
                $('#tblonboardingdetails').destroy;
                // DataTable configuration
                var commonConfig =
                {
                    draw: page,
                    recordsTotal: totalRecordCount,
                    recordsFiltered: totalRecordCount,
                    destroy: true,
                    //dom: "<'row'<'col-lg-3 col-sm-5 col-xs-12'<'record_btn_main'l,><'dt-buttons'Bf>><'col-lg-9 col-sm-7 col-xs-12 mb_margin_bottom_15'<'resignblock text-right'>>>" +
                    //    "<'row'<'col-sm-12 margin-bottom-10'<'customforms_table EmpNew_tbl_scroll'tr'>>>" +
                    //    "<'row'<'col-md-5 col-sm-5'i><'col-md-7 col-sm-7'p>>",
                    dom: _domColumn,
                    language: _languageColumn,
                    columnDefs: [
                        { targets: 0, searchable: false, orderable: false },
                        // Additional columnDefs if needed
                    ],
                    //buttons: [
                    //    {
                    //        extend: 'excelHtml5',
                    //        text: 'Excel',
                    //        titleAttr: 'Excel',
                    //        exportOptions: {
                    //            columns: ':visible:not(:last-child,:first-child)'
                    //        }
                    //    }
                    //],
                    lengthMenu: [
                        [10, 25, 50, 100, 500, 700, 1000],
                        [10, 25, 50, 100, 500, 700, 1000]
                    ],
                    //data: tableData,
                    columns: [
                        {
                            data: null,
                            orderable: false,
                            visible: false,
                        },
                        { data: 'OnBordingID' },
                        { data: 'EmployeeCode' },
                        { data: 'StatusID' },
                        { data: 'EmployeeTypeID' },
                        { data: 'EmployeeCategoryID' },
                        { data: 'EmpCompanyID' },
                        { data: 'EmpBranchID' },
                        { data: 'EmpDepartmentID' },
                        { data: 'EmpDesignationID' },
                        { data: 'EmpRoleID' },
                        { data: 'EmpReportingPersonID' },
                        { data: 'EmpShiftID' },
                        { data: 'EmpShiftGroupID' },
                        { data: 'EmpNoticePeriod' },
                        { data: 'EmpPolicyID' },
                        { data: 'EmpPunchID' },
                        { data: 'EmpPunchType' },
                        { data: 'EmpResignDate' },
                        { data: 'EmpGeoEnable' },
                        { data: 'EmpIsSMSAllow' },
                        { data: 'NameOfApplicant' },
                        { data: 'FatherName' },
                        { data: 'PlaceOfBirth' },
                        { data: 'DateOfBirth' },
                        { data: 'AgeOnDate' },
                        { data: 'Nationality' },
                        { data: 'BloodGroup' },
                        { data: 'EmployeeHeight' },
                        { data: 'EmpWeight' },
                        { data: 'EmpBirthMarkIdentification' },
                        { data: 'EmpPhysicalAilment' },
                        { data: 'EmpMobileNumber' },
                        { data: 'EmailID' },
                        { data: 'CriminalRecordInPast' },
                        { data: 'ReferenceOneName' },
                        { data: 'ReferenceOneMobileNumber' },
                        { data: 'ReferenceOneAddress' },
                        { data: 'AadharCardNumber' },
                        { data: 'PancardNumber' },
                        { data: 'AadharCardPhoto' },
                        { data: 'PanCardPhoto' },
                        { data: 'ReferenceTwoName' },
                        { data: 'ReferenceTwoMobileNumber' },
                        { data: 'ReferenceTwoAddress' },
                        { data: 'AadharCardTwoNumber' },
                        { data: 'PancardTwoNumber' },
                        { data: 'AadharCardTwoPhoto' },
                        { data: 'PanCardTwoPhoto' },
                        { data: 'Nominee1Name' },
                        { data: 'Nominee1Relationship' },
                        { data: 'Nominee1Proportion' },
                        { data: 'Nominee1Address' },
                        { data: 'Nominee1Age' },
                        { data: 'Nominee1DOB' },
                        { data: 'Nominee2Name' },
                        { data: 'Nominee2Relationship' },
                        { data: 'Nominee2Proportion' },
                        { data: 'Nominee2Address' },
                        { data: 'Nominee2Age' },
                        { data: 'Nominee2DOB' },
                        { data: 'BankAccountNumber' },
                        { data: 'BankName' },
                        { data: 'IFSCCode' },
                        { data: 'BankAddress' },
                        { data: 'PassbookPhoto' },
                        { data: 'CheckbookPhoto' },
                        { data: 'PermanentVillage' },
                        { data: 'PermanentLandMark' },
                        { data: 'PermanentTaluka' },
                        { data: 'PermanentDistrict' },
                        { data: 'PermanentState' },
                        { data: 'PermanentPincode' },
                        { data: 'PermanentNearPostOffice' },
                        { data: 'PermanentNearPolishStation' },
                        { data: 'PresentVillage' },
                        { data: 'PresentLandMark' },
                        { data: 'PresentTaluka' },
                        { data: 'PresentDistrict' },
                        { data: 'PresentState' },
                        { data: 'PresentPincode' },
                        { data: 'PresentNearPostOffice' },
                        { data: 'PresentNearPolishStation' },
                        { data: 'PaystuctureID' },
                        { data: 'DeviceSrNo' },
                        { data: 'EmployeePhoto' },
                        { data: 'EducationDetails' },
                        { data: 'FamilyDetails' },
                        { data: 'MultipleBranchID' },
                        { data: 'BranchName' },
                        { data: 'DepartmentName' },
                        { data: 'EmployeeCode' },
                        { data: 'JoiningDate' },
                        { data: 'MonthlyCTC' },
                        { data: 'EmpAdharCardNumber' },
                        { data: 'EmpPanCardNumber' },
                        { data: 'EmpAdharCardPhotoDetails' },
                        { data: 'EmpPancardPhotoDetails' },
                        { data: 'GenderID' },
                        { data: 'MaritalStatusID' },
                        { data: 'RejectReason' },
                        { data: 'StatusName' },
                        { data: 'EmpRFIDCard' },
                        { data: 'EnrollUserDeviceID' }
                    ],

                    //columns: columnsdata,

                    columnDefs: [
                        {
                            targets: 0,
                            searchable: false,
                            orderable: false
                        },
                        // Hide specified columns

                        {
                            targets: hiddenColumns,
                            visible: false
                        },
                        // ... (other columnDefs)
                        {
                            targets: 21,
                            render: function (data, type, row) {

                                var nameParts = row.NameOfApplicant.replace(/\s+/g, ' ').trim().split(" ");
                                var initials = nameParts[0].charAt(0).toUpperCase();
                                if (nameParts.length > 1) {
                                    initials += nameParts[1].charAt(0).toUpperCase();
                                }
                                const colors = ["#e74c3c", "#8e44ad", "#3498db", "#16a085", "#f39c12", "#d35400", "#2c3e50"];
                                //const randomColor = colors[Math.floor(Math.random() * colors.length)];
                                const randomColor = "#295097";
                               return  `<div class="emp_box">
                                            <img alt="" class="user_img zoom-image img-circle tooltips" data-placement="right" data-original-title="Emp Id: ${row["EmployeeCode"]}" src="/UploadEmpPhoto/${row.EmployeePhoto.trim()}" style="border-radius: 50%!important; width:34px; height:34px;"
                                            onerror="this.style.display='none'; this.nextElementSibling.style.display='inline-block';"/>
                                            <span class=" tooltips" data-placement="right" data-original-title="Emp Id : ${row["EmployeeCode"]}" style="display: none; min-width: 30px; height: 30px; line-height: 30px; text-align: center; background-color: #ecf3ff; color:  ${randomColor}; border-radius: 50% !important; border: 1px solid #8ab1f7; margin-right: 8px;">
                                            ${initials}</span> ${row.NameOfApplicant}</div>`;

                                //if (!row.EmployeePhoto) {
                                //    return '<div class="emp_box">' +
                                //        '<img src="/assets/images/avtar.png" alt="User icon" class="user_img">' +
                                //        '<span><p>' + row.NameOfApplicant + '</p></span>' +
                                //        '</div>';
                                //} else {
                                //    return '<div class="emp_box">' +
                                //        '<img src="/UploadEmpPhoto/' + row.EmployeePhoto.trim() + '" ' +
                                //        'style="border-radius: 50%!important; width:34px; height:34px;" ' +
                                //        'class="zoom-image user_img" alt="User icon">' +
                                //        '<span><p>' + row.NameOfApplicant + '</p></span>' +
                                //        '</div>';
                                //}
                            }
                        },
                        {
                            "targets": 92,
                            render: function (data, type, row) {
                                if (data != null) {
                                    var date1 = data.split(' ')[0];
                                    var date = date1.split('-');

                                    if (isdateformat == "dd-mm-yyyy") {
                                        var firstValue = date[2] + "-" + date[1] + "-" + date[0] //yyyy-mm-dd
                                    }
                                    else if (isdateformat == "yyyy-mm-dd") {
                                        var firstValue = date[0] + "-" + date[1] + "-" + date[2] //yyyy-mm-dd
                                    }
                                    else if (isdateformat == "mm-dd-yyyy") {
                                        var firstValue = date[1] + "-" + date[2] + "-" + date[0] //yyyy-mm-dd
                                    }
                                    else if (isdateformat == "yyyy-M-dd") {
                                        var _date = new Date(date[1] + "-" + date[2] + "-" + date[0]);
                                        var locale = "en-us";
                                        var month = _date.toLocaleString(locale, { month: "short" });
                                        var firstValue = date[0] + "-" + month + "-" + date[2] //yyyy-mm-dd
                                    }
                                    else if (isdateformat == "M-dd-yyyy") {
                                        var _date = new Date(date[1] + "-" + date[2] + "-" + date[0]);
                                        var locale = "en-us";
                                        var month = _date.toLocaleString(locale, { month: "short" });
                                        var firstValue = month + "-" + date[2] + "-" + date[0] //yyyy-mm-dd
                                    }
                                    else if (isdateformat == "dd-M-yyyy") {
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
                            targets: 101, // Conditional formatting for status
                            createdCell: function (td, cellData) {
                                var statusClasses = {
                                    "Pending": "grey_font",
                                    "Drafted": "navyblue_font",
                                    "Rejected": "red_font",
                                    "Approved": "green_font",
                                    "Deactivated": "red_font",
                                    "Resigned": "red_font",
                                    "Suspended": "red_font",
                                    "Biometric pending": "orange_font"
                                };
                                $(td).addClass("fixed-column-1 " + (statusClasses[cellData] || ""));
                            }
                        },
                        {
                            "targets": 104,
                            "data": null,
                            "render": function (data, type, full, meta) {
                                var status_id = data.StatusID;
                                if (status_id == 1) {
                                    //Pending
                                    var empstatus = '';
                                    if (roleid == 1 || roleid == 6805 || roleid == 6806) {
                                        empstatus = '<a href="javascript:;" class="btn green_btnnew approve tooltips" data-placement="left" data-original-title="Approve"> <i class="fa-light fa-check"></i> </a> <a href="javascript:;" class="btn red_btnnew reject tooltips" data-placement="left" data-original-title="Reject"> <i class="fa-solid fa-xmark"></i> </a>';
                                    }
                                    else if (roleid > 1 && (canAdd === 'True')) {
                                        empstatus = '<a href="javascript:;" class="btn green_btnnew approve tooltips" data-placement="left" data-original-title="Approve"> <i class="fa-light fa-check"></i> </a> <a href="javascript:;" class="btn red_btnnew reject tooltips" data-placement="left" data-original-title="Reject"> <i class="fa-solid fa-xmark"></i> </a>';
                                    }
                                }
                                else if (status_id == 2) {
                                    //Drafted
                                    var empstatus = '';
                                    if (roleid == 1 || roleid == 6805 || roleid == 6806) {
                                        empstatus = '<a class="editclass btn seagreen_btnnew tooltips" data-placement="left" data-original-title="Edit"> <span id="sp3" hidden>' + data.EmployeeCode + '</span> <i class="fa-regular fa-pen-to-square"> </i> </a> <span></span> <a href="javascript:;" class="btn red_btnnew reject tooltips" data-placement="left" data-original-title="Reject" > <i class="fa-solid fa-xmark"></i> </a>';
                                    }
                                    else if (roleid > 1 && (canAdd === 'True')) {
                                        empstatus = '<a class="editclass btn seagreen_btnnew tooltips" data-placement="left" data-original-title="Edit"> <span id="sp3" hidden>' + data.EmployeeCode + '</span> <i class="fa-regular fa-pen-to-square"> </i> </a> <span></span> <a href="javascript:;" class="btn red_btnnew reject tooltips" data-placement="left" data-original-title="Reject" > <i class="fa-solid fa-xmark"></i> </a>';
                                    }
                                }
                                else if (status_id == 3) {
                                    //Rejected
                                    var empstatus = '';
                                }
                                else if (status_id == 4) {
                                    //Approved
                                    var empstatus = '';
                                    if (roleid == 1 || roleid == 6805 || roleid == 6806) {
                                        empstatus = '<a  href="javascript:;" class="PrintDocs btn blue_btnnew tooltips" data-placement="left" data-original-title="Print"><i class="fa-light fa-print"></i></a>';
                                    }
                                    else if (roleid > 1 && (canAdd === 'True')) {
                                        empstatus = '<a  href="javascript:;" class="PrintDocs btn blue_btnnew tooltips"data-placement="left" data-original-title="Print"><i class="fa-light fa-print"></i></a>';
                                    }
                                }
                                else if (status_id == 8) {
                                    //Drafted
                                    var empstatus = '';
                                    if (roleid == 1 || roleid == 6805 || roleid == 6806) {
                                        empstatus = '<a class="editclass btn seagreen_btnnew tooltips" data-placement="left" data-original-title="Edit"> <span id="sp3" hidden>' + data.EmployeeCode + '</span> <i class="fa-regular fa-pen-to-square"> </i> </a> <span></span> <a href="javascript:;" class="btn red_btnnew reject tooltips" data-placement="left" data-original-title="Reject" > <i class="fa-solid fa-xmark"></i> </a>';
                                    }
                                    else if (roleid > 1 && (canAdd === 'True')) {
                                        empstatus = '<a class="editclass btn seagreen_btnnew tooltips" data-placement="left" data-original-title="Edit"> <span id="sp3" hidden>' + data.EmployeeCode + '</span> <i class="fa-regular fa-pen-to-square"> </i> </a> <span></span> <a href="javascript:;" class="btn red_btnnew reject tooltips" data-placement="left" data-original-title="Reject" > <i class="fa-solid fa-xmark"></i> </a>';
                                    }
                                }
                                else {
                                    empstatus = '';
                                }
                                return empstatus;
                            },
                            "createdCell": function (td, cellData, rowData, row, col) {
                                $(td).addClass('fixed-column');
                            },

                        },
                    ],
                    pageLength: 10,
                    responsive: true,
                    autoWidth: false,
                    initComplete: function () {
                        const inittable = this.api();
                        //$('#tblonboardingdetails tr:eq(0) th:eq(0)').html("Sr. No");

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
                        //            generateOnBoardingExcelFile();
                        //        }, 1000);
                        //    });
                        //}

                        // Add serach functionality with footer append to thead
                        var footer = '<tr>';
                        //var columnSequence = ["NameOfApplicant", "BranchName", "DepartmentName", "EmployeeCode", "JoiningDate", "MonthlyCTC", "RejectReason", "StatusName"];

                        var columnSequence = ["NameOfApplicant", "BranchName", "DepartmentName", "EmployeeCode", "MonthlyCTC", "RejectReason"];

                        // Mapping of StatusName to StatusID
                        //var statusMapping = {
                        //    "Pending": 1,
                        //    "Drafted": 2,
                        //    "Rejected": 3,
                        //    "Approved": 4,
                        //    "Deactivated": 5,
                        //    "Resigned": 6,
                        //    "Suspended": 7,
                        //    "Biometric Pending": 8
                        //};

                        //var currentStatusName = rowData.length > 0 ? rowData[0].StatusName : "";

                        // Get StatusID dynamically
                        //var statusId = statusMapping[currentStatusName] || 0;

                        // Loop through the desired column sequence to build footer
                        columnSequence.forEach(function (columnName, index) {

                            //if (columnName === "StatusName") {
                            //    // Add a specific class for "StatusName"
                            //    footer += `<th class="fixed-column-1">${columnName}</th>`;
                            //} else {
                            //    footer += `<th>${columnName}</th>`;
                            //}
                            footer += `<th>${columnName}</th>`;

                        });

                        // Add a final fixed column
                        footer += '<th class="fixed-column-1"></th><th class="fixed-column"></th></tr>';
                        $("#tblonboardingdetails thead").append(footer);

                        // Add search inputs dynamically for only specified columns
                        $("#tblonboardingdetails thead tr:eq(1) th").each(function (index) {
                            var title = $(this).text();
                            if (index == 11) {
                                $(this).html('');
                            } else
                                if (columnSequence.includes(title)) {
                                    $(this).html(`<input type="text" class="search_input" data-column="${title}" placeholder="Search" /><i class="clm-search" id=""></i>`);
                                } else {
                                    $(this).html('');
                                }
                        });

                        // Apply the Search for specific columns
                        $('.nosearch').parents('th').each(function () {
                            inittable.column($(this).index()).search('');
                        });

                        // Apply the Search for all Columns
                        $("#tblonboardingdetails thead").on("keyup", "input", function () {
                            var columnIndex = $(this).parent().index();
                            var visibleColumns = inittable.columns(':visible').indexes().toArray();
                            var actualIndex = visibleColumns[columnIndex];
                            inittable.column(actualIndex).search(this.value).draw();
                        });

                        //var _exportflage = "onboardMaster1";                   
                        injectExportButtonsForDataTables(isServerSide, function () {
                           
                            
                            generateOnBoardingExcelFile(event);
                        });
                        $('.tooltips').tooltip();


                    },
                    //drawCallback: function (settings) {
                    //    //var drawtable = this.api();
                    //    //var startIndex = drawtable.page.info().start;
                    //   
                    //    var table = $('#tblonboardingdetails').DataTable();
                    //    var pageInfo = table.page.info();

                    //    var startIndex = pageInfo.start + 1; // Correct row start index
                    //    $('td:eq(0)', row).html(startIndex + displayIndex); // Set numbering dynamically
                    //}

                    //rowCallback: function (row, data, displayIndex, displayIndexFull) {
                    //    var pageInfo = this.api().page.info();
                    //    var startIndex = pageInfo.start + 1;
                    //    $('td:eq(0)', row).html(startIndex + displayIndex);
                    //}
                };


                if (isServerSide) {
                    var servsidetableData = [];
                    // Server-side processing logic
                    commonConfig.serverSide = true;
                    commonConfig.processing = false;
                    commonConfig.data = servsidetableData,
                        commonConfig.ajax = {
                            url: '/DataGridOptimize/GetPaginatedEmployeeOnBoarding',
                            type: 'POST',
                            contentType: "application/json",
                            headers: { 'Authorization': token },
                            beforeSend: function () {
                                $(".loading").show();
                            },
                            data: function (d) {
                                let searchCriteria = [];
                                var field = '';
                                var value = '';

                                $('#tblonboardingdetails thead .search_input').each(function () {

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
                                return JSON.stringify({
                                    draw: d.draw,
                                    search: { value: d.search.value || '' },
                                    order: d.order,
                                    columns: d.columns,
                                    length: d.length,
                                    start: d.start,
                                    DatagridThreSold: mainobj.DataGridThreSold,
                                    CustFilter: searchCriteria.length > 0 ? searchCriteria : null,
                                    "OnBoardingID": mainobj.OnBoardingID,
                                    "RoleId": mainobj.RoleId,
                                    "CompanyID": mainobj.CompanyID,
                                    "BranchID": mainobj.BranchID,
                                    "LoginEmpId": mainobj.LoginEmpId,
                                    "DeaprtmentID": mainobj.DeaprtmentID,
                                    "DesignationID": mainobj.DesignationID,
                                });
                            },
                            dataSrc: function (json) {
                                $(".loading").hide();
                                var seversideTable = json.data.Table || [];
                                var seversideTable1 = json.data.Table1 || [];
                                var seversideTable2 = json.data.Table2 || [];

                                serversideeduMap = '';
                                serversideeduMap = new Map();
                                if (seversideTable1) {
                                    seversideTable1.forEach(function (rowData) {
                                        // Make sure that the mapping holds an array for cases where multiple education records exist for one OnBordingID
                                        if (!serversideeduMap.has(rowData.BordingID)) {
                                            serversideeduMap.set(rowData.BordingID, []);
                                        }
                                        serversideeduMap.get(rowData.BordingID).push({
                                            'EduID': rowData.EduID,
                                            'UniversityName': rowData.UniversityName,
                                            'Percentage': rowData.Percentage,
                                            'FromYear': rowData.FromYear,
                                            'ToYear': rowData.ToYear,
                                            'Specialization': rowData.Specialization,
                                        });
                                    });
                                }


                                serversidefamilyMap = '';
                                serversidefamilyMap = new Map();
                                if (seversideTable2) {
                                    seversideTable2.forEach(function (rowData) {
                                        // Make sure that the mapping holds an array for cases where multiple education records exist for one OnBordingID
                                        if (!serversidefamilyMap.has(rowData.BordingID)) {
                                            serversidefamilyMap.set(rowData.BordingID, []);
                                        }
                                        serversidefamilyMap.get(rowData.BordingID).push({
                                            'FID': rowData.FID,
                                            'Fname': rowData.Fname,
                                            'Relation': rowData.Relation,
                                            'Occupation': rowData.Occupation,
                                            'Age': rowData.Age,
                                        });

                                    });
                                }

                                // Process the main table data
                                servsidetableData = seversideTable.map(function (rowData) {

                                    var OnBordingID = parseInt(rowData.OnBordingID, 10);
                                    return {
                                        'OnBordingID': rowData.OnBordingID,
                                        'EmployeeCode': rowData.EmployeeCode,
                                        'StatusID': rowData.StatusID,
                                        'EmployeeTypeID': rowData.EmployeeTypeID,
                                        'EmployeeCategoryID': rowData.EmployeeCategoryID,
                                        'EmpCompanyID': rowData.CompanyID,
                                        'EmpBranchID': rowData.BranchID,
                                        'EmpDepartmentID': rowData.DepartmentID,
                                        'EmpDesignationID': rowData.DesignationID,
                                        'EmpRoleID': rowData.RoleID,
                                        'EmpReportingPersonID': rowData.ReportingTo,
                                        'EmpShiftID': rowData.ShiftID,
                                        'EmpShiftGroupID': rowData.ShiftGroupId,
                                        'EmpNoticePeriod': rowData.NoticePeriod,
                                        'EmpPolicyID': rowData.PolicyID,
                                        'EmpPunchID': rowData.PunchID,
                                        'EmpPunchType': rowData.PunchTypeID,
                                        'EmpResignDate': rowData.ResignDate,
                                        'EmpGeoEnable': rowData.GeoEnable,
                                        'EmpIsSMSAllow': rowData.IsSMSAllow,
                                        'NameOfApplicant': rowData.NameOfApplicant,
                                        'FatherName': rowData.FatherName,
                                        'PlaceOfBirth': rowData.PlaceOfBirth,
                                        'DateOfBirth': rowData.DateOfBirth,
                                        'AgeOnDate': rowData.AgeOnDate,
                                        'Nationality': rowData.Nationality,
                                        'BloodGroup': rowData.BloodGroup,
                                        'EmployeeHeight': rowData.EmployeeHeight,
                                        'EmpWeight': rowData.EmployeeWeight,
                                        'EmpBirthMarkIdentification': rowData.BirthMarkIdentification,
                                        'EmpPhysicalAilment': rowData.PhysicalAilment,
                                        'EmpMobileNumber': rowData.MobileNumber,
                                        'EmailID': rowData.EmailID,
                                        'CriminalRecordInPast': rowData.CriminalRecordInPast,
                                        'ReferenceOneName': rowData.ReferenceOneName,
                                        'ReferenceOneMobileNumber': rowData.ReferenceOneMobileNumber,
                                        'ReferenceOneAddress': rowData.ReferenceOneAddress,
                                        'AadharCardNumber': rowData.AadharCardNumber,
                                        'PancardNumber': rowData.PancardNumber,
                                        'AadharCardPhoto': rowData.AadharCardPhoto,
                                        'PanCardPhoto': rowData.PanCardPhoto,
                                        'ReferenceTwoName': rowData.ReferenceTwoName,
                                        'ReferenceTwoMobileNumber': rowData.ReferenceTwoMobileNumber,
                                        'ReferenceTwoAddress': rowData.ReferenceTwoAddress,
                                        'AadharCardTwoNumber': rowData.AadharCardTwoNumber,
                                        'PancardTwoNumber': rowData.PancardTwoNumber,
                                        'AadharCardTwoPhoto': rowData.AadharCardTwoPhoto,
                                        'PanCardTwoPhoto': rowData.PanCardPhoto,
                                        'Nominee1Name': rowData.Nominee1Name,
                                        'Nominee1Relationship': rowData.Nominee1Relationship,
                                        'Nominee1Proportion': rowData.Nominee1Proportion,
                                        'Nominee1Address': rowData.Nominee1Address,
                                        'Nominee1Age': rowData.Nominee1Age,
                                        'Nominee1DOB': rowData.Nominee1DOB,
                                        'Nominee2Name': rowData.Nominee2Name,
                                        'Nominee2Relationship': rowData.Nominee2Relationship,
                                        'Nominee2Proportion': rowData.Nominee2Proportion,
                                        'Nominee2Address': rowData.Nominee2Address,
                                        'Nominee2Age': rowData.Nominee2Age,
                                        'Nominee2DOB': rowData.Nominee2DOB,
                                        'BankAccountNumber': rowData.BankAccountNumber,
                                        'BankName': rowData.BankName,
                                        'IFSCCode': rowData.IFSCCode,
                                        'BankAddress': rowData.BankAddress,
                                        'PassbookPhoto': rowData.PassbookPhoto,
                                        'CheckbookPhoto': rowData.CheckbookPhoto,
                                        'PermanentVillage': rowData.PermanentVillage,
                                        'PermanentLandMark': rowData.PermanentLandMark,
                                        'PermanentTaluka': rowData.PermanentTaluka,
                                        'PermanentDistrict': rowData.PermanentDistrict,
                                        'PermanentState': rowData.PermanentState,
                                        'PermanentPincode': rowData.PermanentPincode,
                                        'PermanentNearPostOffice': rowData.PermanentNearPostOffice,
                                        'PermanentNearPolishStation': rowData.PermanentNearPolishStation,
                                        'PresentVillage': rowData.PresentVillage,
                                        'PresentLandMark': rowData.PresentLandMark,
                                        'PresentTaluka': rowData.PresentTaluka,
                                        'PresentDistrict': rowData.PresentDistrict,
                                        'PresentState': rowData.PresentState,
                                        'PresentPincode': rowData.PresentPincode,
                                        'PresentNearPostOffice': rowData.PresentNearPostOffice,
                                        'PresentNearPolishStation': rowData.PresentNearPolishStation,
                                        'PaystuctureID': rowData.PaystuctureID,
                                        'DeviceSrNo': rowData.DeviceSrNo,
                                        'EmployeePhoto': rowData.EmployeePhoto,
                                        'EducationDetails': serversideeduMap.get(OnBordingID) || [],  // Add educational data if available
                                        'FamilyDetails': serversidefamilyMap.get(OnBordingID) || [], // Add educational data if available
                                        'MultipleBranchID': rowData.MultipleBranchID,
                                        'BranchName': rowData.BranchName,
                                        'DepartmentName': rowData.DepartmentName,
                                        'EmployeeCode': rowData.EmployeeCode,
                                        'JoiningDate': rowData.JoiningDate,
                                        'MonthlyCTC': rowData.MonthlyCTC,
                                        'EmpAdharCardNumber': rowData.EmpAdharcardInfo,
                                        'EmpPanCardNumber': rowData.EmpPancardInfo,
                                        'EmpAdharCardPhotoDetails': rowData.AdharCardPhotoInfo,
                                        'EmpPancardPhotoDetails': rowData.PanCardPhotoInfo,
                                        'StatusName': rowData.StatusName,
                                        'GenderID': rowData.GenderID,
                                        'MaritalStatusID': rowData.MaritalStatusID,
                                        'EmpRFIDCard': rowData.EmpRFIDCard,
                                        'EnrollUserDeviceID': rowData.EnrollUserDeviceID,
                                        'RejectReason': rowData.RejectReason,
                                        'IDCardNumber': rowData.IDCardNumber,
                                        'GatePassPolicyID': rowData.GatePassPolicyID,
                                    };
                                });
                                return servsidetableData;
                            }
                        };
                }
                else {
                    // Client-side data
                    commonConfig.serverSide = false; // Disable server-side processing
                    commonConfig.ajax = null; // Use the initial data provided
                    commonConfig.data = tableData; // Set data directly for client-side processing
                    commonConfig.deferRender = true; // For performance with large data
                }

                $('#tblonboardingdetails').on('draw.dt', function () {
                    $('.tooltips').tooltip();
                });
                $('.tooltips').tooltip();

                setTimeout(function () {
                    
                    $("#" + "tblonboardingdetails" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive"));
                    $('.tooltips').tooltip();
                }, 100);

                if ($.fn.DataTable.isDataTable('#tblonboardingdetails')) {
                    $('#tblonboardingdetails').DataTable().clear().destroy();
                    $('#tblonboardingdetails').empty(); // Optional: Clears HTML content
                }

                // Initialize DataTable with the configuration                
                table = $('#tblonboardingdetails').DataTable(commonConfig);
                $(".ajax_loader").hide();
            }
        },
        error: function (xhr, status, error) {
            $(".loading").hide();
            toastr.error("Error loading onboarding data: " + error);
        }
    });
}

function generateOnBoardingExcelFile(event) { 
    
    var mainObj =
    {
        "searchTerm": $('#searchTerm').val() || "",
        "sortColumn": "bm.branchname",
        "sortOrder": "asc",
        "pageSize": 10,
        "page": 1,
        "Export_flg": 1,
        "DataGridValues": DataGridThreSold,
        "LoginId": LoginId,
        "RoleId": emprole,
        "CustomFilters": searchCriteria.length > 0 ? searchCriteria : []
    };

    $.ajax({
        url: '/DataGridOptimize/GetAllOnBoardingExcelData',
        type: 'POST',
        data: JSON.stringify({ mainObj }),
        contentType: 'application/json',
        success: function (response) {            
            if (response.success && response.filePath) {
                toastr.success("Export completed. Starting download...");
                window.location.href = response.filePath;
            } else {
                toastr.error(response.message || "Failed to generate export file.");
            }
        },
        error: function () {
            toastr.error("An error occurred while exporting data.");
        }
    });
}


function generateOnBoardingExcelFile() {

    checkSession();

    $.ajax({
        url: '/DataGridOptimize/GetAllOnBoardingExcelData',
        type: 'POST',
        data: JSON.stringify(ObjData),
        contentType: 'application/json',
        beforeSend: function () {
            $("#ajax_loader").show(); // Show loader before sending the request
        },
        success: function (response) {
            if (response.success && response.filePath) {
               /* toastr.success("Export completed. Starting download...");*/
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