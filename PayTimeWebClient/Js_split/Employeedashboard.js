
const token = window.AppConfig.token;
const roleId = window.AppConfig.roleId;
const webApiUrl = window.AppConfig.webApiUrl;
let emprole = window.AppConfig.roleId;
let cmpId = window.AppConfig.cmpId;
let branchId = window.AppConfig.branchId;
var empid = window.AppConfig.empid;
const UserId = window.AppConfig.UserId;
const DateFormate = window.AppConfig.DateFormate;
const DepartmentId = window.AppConfig.DepartmentId;
const CompanyCode = window.AppConfig.CompanyCode;
let locakattendeceday = window.AppConfig.locakattendeceday;
let planid = window.AppConfig.planid;
let worf = window.AppConfig.worf;
var hashedWorf = CryptoJS.SHA256(worf.toString()).toString(CryptoJS.enc.Hex);
let empdata = window.AppConfig.empdata;
let IsApprovalForWebpunch = window.AppConfig.IsApprovalForWebpunch;
let firstempid = window.AppConfig.firstempid;
let originalempid = empid;
let hierarchy = window.AppConfig.hierarchy;
let originalemployeeid = window.AppConfig.originalemployeeid;
const emsUrl = window.AppConfig.webApiEmsUrl;
var selectedFlag = 0;
$(document).ready(function () {            

    if (CompanyCode == "MAN5F0F") {
        loadBUSummary(selectedFlag, getBUDate());
        $(document).on('click', '.filter_dm ul li a', function () {
            $('.filter_dm ul li').removeClass('active');
            $(this).parent().addClass('active');
            selectedFlag = $(this).data('flag');
            if (selectedFlag == 0) {
                $('#summaryHeader').text('BU');
            }
            else if (selectedFlag == 1) {
                $('#summaryHeader').text('Sub-BU');
            }
            loadBUSummary(
                selectedFlag,
                getBUDate()
            );

        });
        $("#BUdtpickerID").on('changeDate', function () {
            loadBUSummary(
                selectedFlag,
                getBUDate()
            );
        });

        //$('.date-picker').datepicker({
        //    format: 'yyyy-mm-dd',
        //    autoclose: true,
        //    todayHighlight: true
        //});

        $('.BUdtpicker').datepicker({
            format: 'yyyy-mm-dd',
            autoclose: true,
            todayHighlight: true
        }).on('changeDate', function () {

            loadBUSummary(
                selectedFlag,
                getBUDate()
            );
        });

        function loadBUSummary(flag, attnDate) {
            $.ajax({
                url: webapiuri + "Dashboard/BUWiseCheckinSummary",
                type: "GET",
                data: {
                    flag: flag,
                    attnDate: attnDate
                },
                headers: {
                    "Authorization": token
                },
                success: function (res) {
                    bindBUTable(res);
                },
                error: function (err) {
                    console.error(err);
                    $('#BUSummaryBody').html(
                        '<tr><td colspan="3" class="text-center text-danger">Failed to load data</td></tr>'
                    );
                }
            });
        }
    }
    
    function getSelectedDate() {
        var dateVal = $('#attnDate').val();

        if (dateVal && dateVal.trim() !== '') {
            return dateVal;  
        }
        return moment().format('YYYY-MM-DD');
    }

    function getBUDate() {
        var dateVal = $('#BUDate').val();

        if (dateVal && dateVal.trim() !== '') {
            return dateVal;
        }
        return moment().format('YYYY-MM-DD');
    }
    

    function bindBUTable(data) {
        var tbody = '';
        if (!data || data.length === 0) {
            tbody = '<tr><td colspan="3" class="text-center">No data available</td></tr>';
            $('#BUSummaryBody').html(tbody);
            return;
        }
        $.each(data, function (i, item) {
            tbody += '<tr>';
            tbody += '<td class="text-left">' + (item.SubBU || '-') + '</td>';
            tbody += '<td class="green_font open-checkin" style="cursor:pointer;" ' +
                'data-branchid="' + item.BranchID + '" ' +
                'data-companyid="' + item.CompanyID + '" ' +
                'data-status="0" ' +
                'data-name="' + item.SubBU + '">' +
                item.CheckIn +
                '</td>';
            tbody += '<td class="red_font open-checkin" style="cursor:pointer;" ' +
                'data-branchid="' + item.BranchID + '" ' +
                'data-companyid="' + item.CompanyID + '" ' +
                'data-status="1" ' +
                'data-name="' + item.SubBU + '">' +
                item.NotCheckIn +
                '</td>';
            tbody += '</tr>';
        });
        $('#BUSummaryBody').html(tbody);
    }
    $(document).on("click", ".open-checkin", function () {
        var branchId = $(this).data("branchid");
        var companyId = $(this).data("companyid");
        var status = $(this).data("status");
        var name = $(this).data("name");
        openBUCheckinModal(companyId, branchId, status, name);
    });
    function openBUCheckinModal(companyId, branchId, checkinStatus, name) {
        var attnDate = getBUDate();
        var titleText = name +
            (checkinStatus == 0 ? " (Checked In)" : " (Not Checked In)");
        $("#bu_modal_title").text(titleText);
        $("#bucheckin_modal").modal("show");
        loadBUCheckinDetails(companyId, branchId, checkinStatus, attnDate);
    }
    function loadBUCheckinDetails(companyId, branchId, checkinStatus, attnDate) {
        $.ajax({
            type: "GET",
            url: webapiuri + "Dashboard/BUWiseCheckinDetails",
            data: {
                companyId: companyId,
                branchId: branchId,
                checkin: checkinStatus,
                attnDate: attnDate
            },
            headers: { 'Authorization': token },
            beforeSend: function () {
                $(".loading").show();
            },
            success: function (response) {
                if ($.fn.DataTable.isDataTable("#bucheckin_tbl")) {
                    $("#bucheckin_tbl").DataTable().clear().destroy();
                    $('#bucheckin_tbl_wrapper').remove();
                    $('#dataTables_tbl_header').remove();
                }
                if (response && response.length > 0) {
                    $("#bu-empty").hide();
                    $("#tableResponsive_bu").show();
                    var columnDefs = [
                        { data: "EmpCode", title: "Employee Code", width: "160px" },
                        { data: "Name", title: "Name", width: "200px" },
                        { data: "PunchId", title: "Punch ID", width: "120px" },
                        { data: "CompanyName", title: "Company", width: "180px" },
                        { data: "Branch", title: "Branch", width: "160px" },
                        { data: "Department", title: "Department", width: "160px" },
                        { data: "Designation", title: "Designation", width: "160px" },
                        { data: "Date", title: "Date", width: "120px" },
                        { data: "PunchTime", title: "PunchTime", width: "120px" }
                    ];                   
                    var table = $("#bucheckin_tbl").DataTable({
                        data: response,
                        columns: columnDefs,
                        paging: true,
                        searching: true,
                        ordering: true,
                        lengthChange: true,
                        autoWidth: false,
                        dom: _domCommon,
                        language: _languageCommon,
                        destroy: true,
                        buttons: [
                            {
                                extend: 'excelHtml5',
                                text: 'Excel',
                                className: 'buttons-excel',
                                exportOptions: { columns: ':visible' }
                            }
                        ],
                        initComplete: function () {
                            const reqid = 'bucheckin_tbl';
                            $("#export-button").remove();
                            $("#buttons").append(`
                            <div id="export-button"
                                 class="export-button tooltips"
                                 data-placement="bottom"
                                 data-original-title="Download"
                                 data-table-id="${reqid}">
                                <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
                            </div>
                             `);
                            $('.tooltips').tooltip();
                        }
                    });
                    setTimeout(function () {
                        $("#bucheckin_tbl_wrapper #dataTables_tbl_header")
                            .insertBefore($("#tableResponsive_bu"));
                    }, 100);

                } else {
                    $("#bu-empty").show();
                    $("#tableResponsive_bu").hide();
                }

                $(".loading").hide();
            },
            error: function () {
                $(".loading").hide();
                console.log("Error while fetching BU Checkin Details.");
            }
        });
    }


    checkSession();
    var employeeids = 0; 
    //hirarchy changes location reload.

    //if superadmin login then set first empid for employeeid
    if (empid == 0) {
        empid = firstempid;
        originalemployeeid = firstempid;
    }
    //only show this button who's autoapprove webpunch not allowed
    if (IsApprovalForWebpunch == 0) {
        $("#webpunchdiv").css("display", "flex");
    }
    //only show search button to super admin
    if (roleId == 1 || roleId == 6806 || roleId == 6805) {
        $("#Filter_search").css("display", "block");
        $(".webpunch_block").css("display", "none");
    }
    //attendence wizard
    Date.prototype.toInputFormat = function () {
        var yyyy = this.getFullYear().toString();
        var mm = (this.getMonth() + 1).toString(); // getMonth() is zero-based
        var dd = this.getDate().toString();
        return yyyy + "-" + (mm[1] ? mm : "0" + mm[0]) + "-" + (dd[1] ? dd : "0" + dd[0]); // padding
    };
    var date = new Date();
    var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
    var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);
    var i = "";
    var j = "";
    var k = "";
    var l = "";
    if (firstDay.getMonth() < 9) {
        i = "0" + (firstDay.getMonth() + 1);
    }
    else {
        i = (firstDay.getMonth() + 1);
    }
    if (lastDay.getMonth() < 9) {
        j = "0" + (lastDay.getMonth() + 1);
    }
    else {
        j = (lastDay.getMonth() + 1);
    }

    if (firstDay.getDate() < 10) {
        k = "0" + (firstDay.getDate());
    }
    else {
        k = (firstDay.getDate());
    }
    if (lastDay.getDate() < 10) {
        l = "0" + (lastDay.getDate());
    }
    else {
        l = (lastDay.getDate());
    }

    var FirstDayWithSlashes = (firstDay.getFullYear()) + '-' + i + '-' + k;
    var lastDayWithSlashes = (lastDay.getFullYear()) + '-' + j + '-' + l;
    var currentTime = new Date();
    var currentHour = ('0' + currentTime.getHours()).slice(-2);
    var currentMinute = ('0' + currentTime.getMinutes()).slice(-2);
    var currentFormattedTime = currentHour + ':' + currentMinute;
    //---------------------------

    var selectedemp = 0;
    var checktime;
    var count = 0;
    var mode;// for attendece summary from filter


    // this is for page load 
    quicksearch(empid);
    GetPunchInformation(originalemployeeid);
    CalendarFill(empid);
    CalendarListFill(FirstDayWithSlashes, lastDayWithSlashes, empid);
    getpunchmode(originalemployeeid);

    setTimeout(() => {
        Getpendingleave_count();
        MyLeave(empid);

        Filltblleavebalance(empid);
        $(".loading").hide();
    }, 1000);

    // Schedule Group 3 -- 1.5 seconds

    setTimeout(() => {

        EmployeeDropdownForCelender(empid);
        EmployeeListForLeavebalance(empid);
        PolicyReference();
        getLocation();
        $(".loading").hide();
        
    }, 500);

    $("#dvTPfeedback").hide();
    if (CompanyCode == "TAT02D3" || CompanyCode == "MINC2E0") {
        LoadFeedbackTypes();
        totalFeedback();
        $("#dvTPfeedback").show();
    }

    $('#attendanceTabLink').on('click', function () {
        sessionStorage.setItem('activeTab', '#attendancetab_2');
    });
    $('#leaveTabLink').on('click', function () {
        sessionStorage.setItem('activeTab', '#leavetab_1');
    });
    $('#webpunchTabLink').on('click', function () {
        sessionStorage.setItem('activeTab', '#webpunchtab_3');
    });
    $('#PD_btn').click(function () {
        $('#PD_detailblock').show();
        $('#myattendance_block').hide();
        Emppersonaldetails(empid);
    });
    $('#PD_close').click(function () {
        $('#PD_detailblock').hide();
        $('#myattendance_block').show();
    });
    $("#webpunchdata").on('click', function () {
        $("#webpunchdata").attr("data-toggle", "modal");
        $("#webpunchdata").attr("data-target", "#webpunchdata_modal");
    });

    $("#Workinghrsdata").on('click', function () {
        $("#Workinghrsdata").attr("data-toggle", "modal");
        $("#Workinghrsdata").attr("data-target", "#Workinghrsdata_modal");
    });

    $("#Addfeedback_btn").on('click', function () {
        $("#Addfeedback_btn").attr("data-toggle", "modal");
        $("#Addfeedback_btn").attr("data-target", "#Addfeedbackbtn_modal");
    });

    //---for global search
    var availableNames = [];
    $("#SearchName").autocomplete({

        source: availableNames
    });
    $("#SearchName").keypress(function (e) {

        if (e.which == 13) {
            e.preventDefault();
            $("#btnsearch").click();
        }
    });
    $("#btnsearch").click(function () {
        if ($("#searchselection").val() == "0" || $("#searchselection").val() == "" || $("#searchselection").val() == null) {
            toastr.remove();
            toastr.error("Please select search type.");
            return false;
        }
        Search();
        const wrapper = $(".filter_portlet_wrapper");
        wrapper.addClass("full-width");
    });
    $("#searchselection").change(function () {

        var selectedValue = $(this).val();

        if (selectedValue === "EmpName") {
            SearchNames();
        }
        else {
            availableNames = [];

        }

        $("#SearchName").val("");
        $("#SearchName").attr("readonly", false);
    });
    
    $("#Filter_search").click(function () {
        const wrapper = $(".filter_portlet_wrapper");
        wrapper.toggleClass("full-width");

        var screenWidth = $(window).width();
        if (screenWidth >= 300 && screenWidth <= 600) {
            $('.filter_portlet_tbl').toggle();
            $('.filter_portlet_wrapper .filter_boxModal').css('width', '100%');
        }
    });

    // Close filter panel on close button click
    $(".filterclose_btn").click(function () {
        const wrapper = $(".filter_portlet_wrapper");
        wrapper.addClass("full-width");

        var screenWidth = $(window).width();
        if (screenWidth >= 300 && screenWidth <= 600) {
            $(".filter_portlet_tbl").css("display", "block");
        }
    });

    $(document).on('click', '#closedatten', function () {

        $('#dataTables_tbl_header').remove();
    });

    $("#closeModal1").click(function () {
        $('#Feedbacktype').val('company-0');
        $('#feedbackComments').val('');
    })

    $('#subitFeedback').click(function () {
        let type = $("#Feedbacktype").val();
        let Comments = $("#feedbackComments").val().trim();
        if (!type || type === "company-0") {
            toastr.remove();
            toastr.error("Please select feedback type.");
            return false;
        }
        if (!Comments) {
            toastr.remove();
            toastr.error("Please Add Comments.");
            return false;
        }
        const FeedbackData = {
            CompanyID: cmpId,
            EmpID: empid,
            FeedbackType: parseInt(type),
            Description: Comments,
        }

        $.ajax({
            type: 'POST',
            url: webapiuri + "DashBoard/TpFeedback",
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': token
            },
            dataType: "json",
            data: JSON.stringify(FeedbackData),
            beforeSend: function () {
                $("#ajax_loader").show();
            },
            success: function (data) {
                totalFeedback();
                $('#Feedbacktype').val('company-0');
                $('#feedbackComments').val('');
                $("#Addfeedbackbtn_modal").modal('hide');
                toastr.success("Feedback Added.");
            },  
            error: function (xhr, status, error) {
                toastr.remove();
                toastr.error("Error in submit feedback");
            },
            complete: function () {
                $("#ajax_loader").hide();
            }
        })
    });
    
        function totalFeedback() {
            $.ajax({
                type: 'GET',
                url: webapiuri + "DashBoard/ApprovedFeedbackOfEmployee?empId=" + empid + "&companyId=" + cmpId,
                contentType: "application/json; charset=utf-8",
                headers: {
                    'Authorization': token
                },
                success: function (data) {

                    $('#totalCount').text(data.TotalSubmitted);
                    $('#addressedCount').text(data.TotalAddressed);
                },
                error: function (xhr, status, error) {
                    toastr.remove();
                    toastr.error("Error occurred while loading feedback counts.");
                }
            });
        }

        function LoadFeedbackTypes() {
            $.ajax({
                type: 'GET',
                url: webapiuri + "DashBoard/GetFeedbackTypes",
                contentType: "application/json; charset=utf-8",
                headers: {
                    'Authorization': token
                },
                success: function (data) {
                    let $dropdown = $("#Feedbacktype");
                    $dropdown.empty();
                    $dropdown.append(`<option value="company-0">Select Type</option>`); 
                    $.each(data, function (index, item) {
                        $dropdown.append(`<option value="${item.TPid}">${item.FeedBackType}</option>`);
                    });
                },
                error: function () {
                    toastr.error("Failed to load feedback types.");
                }
            });
        }

    $("#feedbackOverview").click(function () {       
        if ($("#addressedCount").text() == 0) {
            return false;
        } else {
            loadFeedbackData();
            $('#feedbackoverviewblock_modal').modal('show');
        }
    });
    function loadFeedbackData() {
        $.ajax({
            url: webapiuri + "DashBoard/FeedbackOverviewOfEmployee?EmpId=" + empid + "&flag=4",
            type: 'POST',
            data: JSON.stringify({ EmpId: empid, flag: 4 }),
            headers: { 'Authorization': token },
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                FeedbackOverview(data)
            },
            error: function (xhr, status, error) {
                console.log("AJAX error:", status, error);
            }
        });
    }
    function FeedbackOverview(data) {
        $("#feedbackoverviewblock_modal #dataTables_tbl_header").remove();
        var table = $("#feedbackview_table").DataTable({
            dom: _domCommon,
            language: _languageCommon,
            ordering: true,
            deferRender: true,
            data: data,
            destroy: true,
            columns: [
                { title: "TPid", data: "TPid", visible: false },
                { title: "TraineeId", data: "TraineeId" },
                { title: "Date of feedback", data: "Dateof feedback", visible: false },
                { title: "Addressed Date", data: "Addressed Date", visible: false },
                { title: "Feedback Type", data: "FeedbackType" },
                { title: "Description", data: "Description" },
                { title: "Comments", data: "Approvefeedback", visible: false },
                { title: "IS The Response given by BHR", data: "EmpComment" },
                {
                    title: "Action",
                    data: null, orderable:false,
                    class: "fixed-column",
                    render: function (data, type, row) {
                        if (!row.EmpComment) {
                            return `
                              <a class="btncomment btn blue_btnnew tooltips" data-toggle="modal" data-target="#btncomment_modal"
                                 data-placement="left" data-original-title="Comment"><i class="fa-solid fa-check"></i>
                              </a>`;
                        } else {
                            return "";
                        }
                    }
                },   
            ],
            initComplete: function () {
                const visibleTable = $('table:visible').attr('id');
                $("#buttons").append(`
                    <div id="export-button" class="export-button tooltips" data-placement="bottom" data-original-title="Download" data-table-id="${visibleTable}">
                       <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
                    </div>
                `);
            },
        });
        setTimeout(function () {
            $("#" + "feedbackview_table" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive_feedbackview"));
            $('.tooltips').tooltip();
        }, 100);
    }
    let selectedTPid = null;
    $('#feedbackview_table').on('click', '.btncomment', function (e) {
        const rowData = $('#feedbackview_table').DataTable().row($(this).closest('tr')).data();
        selectedTPid = rowData.TPid;       
        e.preventDefault();
        e.stopPropagation();
        $('#feedbackCommentmodal').modal('show');
    });
    $("#CommentSubmit").click(function () {        
        let comment = $("#commentText").val();
        if (!comment || comment === "0") {
            toastr.remove();
            toastr.error("Please Select Option.");
            return false;
        }
        const FeedbackData = {
            EmpID: empid,
            Comments: comment,
            TPid: selectedTPid,
            flag:1
        }       
        $.ajax({
            type: 'POST',
            url: webapiuri + "DashBoard/AddAdminComment",
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': token
            },
            dataType: "json",
            data: JSON.stringify(FeedbackData),
            success: function (data) {
                toastr.success("Comment saved.");
                $('#feedbackCommentmodal').modal('hide');  
                $("#commentText").val('0');
                loadFeedbackData();
            },
            error: function (xhr, status, error) {
                toastr.remove();
                toastr.error("Error in submit Comment");
            }
        })
    })

    $.ajax({
        type: 'GET',
        url: webapiuri + "DashBoard/CheckEmsRights?flag=0&roleId=" + roleId,
        contentType: "application/json; charset=utf-8",
        headers: {
            'Authorization': token
        },
        success: function (data) {         
            if (data > 0 || roleId == 1 || roleId == 6805 || roleId==6806) {
                $("#emsCountblock").show();
                var currentMonth = new Date().getMonth() + 1;
                $("#empMonth").val(currentMonth).trigger("change");
                $("#emsMonth").val(currentMonth).trigger("change");
                loadEmsDataCount(1, currentMonth);
            } else {
                $("#emsCountblock").hide();
            }
        },
        error: function () {
            toastr.error("Failed to load Expense Block.");
        }
    }); 

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href");

        var flag = 0;
        if (target === "#overview") {
            var month = $("#emsMonth").val();
            flag = 2;
        } else if (target === "#myexpense") {
            flag = 1;
            var month = $("#empMonth").val();
        }

        loadEmsDataCount(flag, month);
    });

    $("#emsMonth").on('change', function (e) {
        var target = $(this).closest(".tab_block").find(".tab-pane.active").attr("id");
        if (target === "overview") {
            flag = 2;
        } else if (target === "myexpense") {
            flag = 1;
        }
        var month = $("#emsMonth").val();
        loadEmsDataCount(flag, month);
    });
    $("#empMonth").on('change', function (e) {
        var target = $(this).closest(".tab_block").find(".tab-pane.active").attr("id");
        var flag = 0;
        if (target === "overview") {
            flag = 2;
        } else if (target === "myexpense") {
            flag = 1;
        }
        var month = $("#empMonth").val();
        loadEmsDataCount(flag, month);
    });   

    function loadEmsDataCount(flag, month) {
        var emsObj = {
            RoleId: parseInt(roleId) || 0,
            EmpId: parseInt(empid) || 0,
            Month: parseInt(month) || 0,
            CompanyId: parseInt(cmpId) || 0,
            BranchId: branchId ? parseInt(branchId) : 0,
            flag: parseInt(flag) || 0
        };
        $.ajax({
            url: emsUrl + "CommanMaster/GetAdminExpenseCount",
            type: 'POST',
            data: JSON.stringify(emsObj),
            headers: { 'Authorization': token },
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (emsObj.flag === 2) {
                    $("#exp_requestCount").text(response.requests);
                    $("#exp_approvedCount").text(response.approved);
                    $("#exp_pendingCount").text(response.pending);
                    $("#exp_declinedCount").text(response.declined);
                } else if (emsObj.flag === 1) {
                    $("#emp_request").text(response.requests);
                    $("#emp_approve").text(response.approved);
                    $("#emp_pending").text(response.pending);
                    $("#emp_decliend").text(response.declined);
                }
            },
            error: function () {
                console.error("Failed to load counts");
            }
        });
    }

    $(".emsoverview_block").on('click', function () {
        var target = $(this).closest(".tab_block").find(".tab-pane.active").attr("id");
        if (target === "overview") {
            var month = $("#emsMonth").val();
            flag = 2;
            var department = $("#emsDepartment").val();
        } else if (target === "myexpense") {
            flag = 1;
            var month = $("#empMonth").val();
            department = null;
        }
        var type = $(this).find(".fb_subttl").text().toLowerCase();
        var excelFileName = "";
        if (type == "approved") {
            $("#emsTitle").text("Expense-Approved");
            excelFileName = "Expense-Approved";
        } else if (type == "declined") {
            $("#emsTitle").text("Expense-Declined");
            excelFileName = "Expense-Declined";
        } else if (type == "pending") {
            $("#emsTitle").text("Expense-Pending");
            excelFileName = "Expense-Pending";
        } else {
            $("#emsTitle").text("Expense-Request");
            excelFileName = "Expense-Request";
        }
        window.selectedExcelFileName = excelFileName;
        var objEms = {
            RoleId: parseInt(roleId) || 0,
            EmpId: parseInt(empid) || 0,
            Month: parseInt(month) || 0,
            CompanyId: parseInt(cmpId) || 0,
            BranchId: branchId ? parseInt(branchId) : 0,
            DepartmentId: department ? parseInt(department) : null,
            flag: parseInt(flag) || 0,
            type: type
        }

        $.ajax({
            url: emsUrl + "CommanMaster/GetDataOfExpense",
            type: 'POST',
            data: JSON.stringify(objEms),
            headers: { 'Authorization': token },
            contentType: "application/json; charset=utf-8",
            success: function (res) {
                if (!res || res.length === 0) {
                    $("#ems-empty").show();
                    $("#tableResponsive_emsview").hide();
                    $("#dataTables_tbl_header").hide();
                } else {
                    $("#ems-empty").hide();
                    $("#tableResponsive_emsview").show();
                    $("#dataTables_tbl_header").show();
                    loadEmsData(res);
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX error:", status, error);
                console.error("Response text:", xhr.responseText);
                console.error("Status code:", xhr.status);
            }
        });

        $(".emsoverview_block").attr("data-toggle", "modal");
        $(".emsoverview_block").attr("data-target", "#emsoverviewblock_modal");
    });

    function loadEmsData(data) {
        $("#emsoverviewblock_modal #dataTables_tbl_header").remove();
        var table = $("#emsdataview_table").DataTable({
            dom: _domCommon,
            language: _languageCommon,
            ordering: true,
            deferRender: true,
            data: data,
            destroy: true,
            columns: [
                { title: "ExpenseId", data: "expenseId", visible: false },
                { title: "EmpID", data: "empID", visible: false },
                { title: "EmpName", data: "empName" },
                { title: "ExpenseDate", data: "expenseDate" },
                { title: "Empcode", data: "empcode" },
                { title: "ExpenseAmount", data: "expenseAmount" }
            ],
            initComplete: function () {
                const visibleTable = $('table:visible').attr('id');
                $("#buttons").append(`
                  <div id="export-button" class="export-button tooltips" data-placement="bottom" data-original-title="Download" data-table-id="${visibleTable}">
                      <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
                  </div>
              `);
            },
        });
        setTimeout(function () {
            $("#" + "emsdataview_table" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive_emsview"));
            $('.tooltips').tooltip();
        }, 100);
    }

    function Search() {

        $("#ajax_loader").show();

        var id = $("#SearchName").val();
        var searchselection = $("#searchselection").val();
        $.ajax({
            type: 'GET',
            async: false,
            url: "/PayTime/EmployeeFilteration",
            data: { id: id, searchselection: searchselection },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () {
                $("#ajax_loader").show();
            },
            success: function (result) {


                if (result.isRedirect) {
                    window.location.href = result.redirectUrl;
                }
                var obj = JSON.parse(result);
                if (obj == "No Data Found") {
                    $("#SearchName").val("");
                    //swal({ title: "Oops.!", text: "No Data Found. Please Search Again.", type: "error", showConfirmButton: false, timer: 2000 });
                    Swal.fire({
                        html: '<div class="swal_subtitle"><b>Oops.!</b><p>No Data Found. Please Search Again.</p></div>',
                        title: "<img class='margin-right-5' src='/assets/images/icons/swal_icon/icon_warning.svg' alt=''>No Data",
                        closeOnConfirm: true,
                        showCloseButton: true,
                        showCancelButton: false,
                        focusConfirm: false,
                        cancelButtonText: 'No',
                        confirmButtonText: 'OK',
                        allowOutsideClick: false,
                        allowEscapeKey: false,
                        customClass: {
                            confirmButton: 'swal-confirm-button-class',
                            cancelButton: 'swal-cancel-button-class'
                        }
                    });
                    $('img[src="/assets/images/icons/swal_icon/icon_warning.svg"]').parent().parent().parent().addClass('swal_warning');
                    $("#ajax_loader").hide();

                }
                else {
                    empid = obj.EmpId;
                    quicksearch(empid)
                    Emppersonaldetails(empid);
                    Fillactivity_HolidayData()
                    //GetPunchInformation(empid);
                    MyLeave(empid);
                    Filltblleavebalance(empid);
                    Teamattendence(empid);
                    CalendarListFill(FirstDayWithSlashes, lastDayWithSlashes, empid);
                    getpunchmode(originalemployeeid);
                    if (roleId == 1 || roleId == 6805 || roleId == 6806) {
                        $('#PD_detailblock').show();
                        $('#myattendance_block').hide();
                        Emppersonaldetails(empid);
                    }
                    $("#loajax_loader").hide();

                }
            },
            error: function (jqXHR, ajaxOptions, thrownError) {
                $('.loading').hide();

                if (jqXHR.status == 500) { //InternalServerError = 500,
                    toastr.error("InternalServerError");
                } else if (jqXHR.status == 400) { //BadRequest = 400,
                    toastr.error("BadRequest");
                } else if (jqXHR.status == 401) {
                    window.location.href = "/PayTime/LoginPage";
                }
            },
            complete: function () {

                $("#ajax_loader").hide();
            }
        });
        $("#ajax_loader").show();
        var b = $('#MyCalendar').fullCalendar('getDate');
        /* var FirstDayoftheMonth = b.format("YYYY-MM-DD");*/

        var addmonth = "";
        var StartDate = new Date(b.format("YYYY-MM-DD"));
        var FirstDayoftheMonth = StartDate.getFullYear() + "-" +
            (StartDate.getMonth() + 1).toString().padStart(2, '0') +
            "-01";
        var EndDate = new Date(StartDate.getFullYear(), StartDate.getMonth() + 1, 0);
        if (EndDate.getMonth() < 10 && EndDate.getMonth() != 9) {
            addmonth = "0" + (EndDate.getMonth() + 1);
        }
        else {
            addmonth = (EndDate.getMonth() + 1);
        }
        var lastDayoftheMonth = EndDate.getFullYear() + "-" + addmonth + "-" + EndDate.getDate();

        $.ajax({
            type: "GET",
            url: webapiuri + "DashBoard/GetEmpAttendanceStatusSummary",
            headers: { 'Authorization': token },
            data: {
                empid: empid,
                fromdate: FirstDayoftheMonth,
                todate: lastDayoftheMonth,
                statusflg: 3,
                finalStatus: "",
                searchName: $("#searchselection").val(),
                searchBy: $("#SearchName").val()
            },
            success: function (events) {

                if (events.isRedirect) {
                    window.location.href = events.redirectUrl;
                }
                $('#MyCalendar').fullCalendar('removeEvents');
                $('#MyCalendar').fullCalendar('addEventSource', events);
                $('#MyCalendar').fullCalendar('rerenderEvents');
                $("#ajax_loader").hide();
            },
            error: function () {
                alert("error");
            },
            complete: function () {

                $("#ajax_loader").hide();
            }
        });
    }

    function getcelenderData(empid) {
        var b = $('#MyCalendar').fullCalendar('getDate');
        /* var FirstDayoftheMonth = b.format("YYYY-MM-DD");*/

        var addmonth = "";
        var StartDate = new Date(b.format("YYYY-MM-DD"));
        var FirstDayoftheMonth = StartDate.getFullYear() + "-" +
            (StartDate.getMonth() + 1).toString().padStart(2, '0') +
            "-01";
        var EndDate = new Date(StartDate.getFullYear(), StartDate.getMonth() + 1, 0);
        if (EndDate.getMonth() < 10 && EndDate.getMonth() != 9) {
            addmonth = "0" + (EndDate.getMonth() + 1);
        }
        else {
            addmonth = (EndDate.getMonth() + 1);
        }
        var lastDayoftheMonth = EndDate.getFullYear() + "-" + addmonth + "-" + EndDate.getDate();

        $.ajax({
            type: "GET",
            url: webapiuri + "DashBoard/GetEmpAttendanceStatusSummary",
            headers: { 'Authorization': token },
            data: {
                empid: empid,
                fromdate: FirstDayoftheMonth,
                todate: lastDayoftheMonth,
                statusflg: 3,
                finalStatus: "",
                searchName: $("#searchselection").val(),
                searchBy: $("#SearchName").val()
            },
            success: function (events) {                
                if (events.isRedirect) {
                    window.location.href = events.redirectUrl;
                }
                $('#MyCalendar').fullCalendar('removeEvents');
                $('#MyCalendar').fullCalendar('addEventSource', events);
                $('#MyCalendar').fullCalendar('rerenderEvents');
                // $('#MyCalendar').fullCalendar('refetchEvents')
                $("#ajax_loader").hide();
            },
            error: function () {
                alert("error");
            },
            complete: function () {

                $("#ajax_loader").hide();
            }
        });
    }

    function SearchNames() {

        var jsonObj = empdata;
        var compressedData = atob(jsonObj);;
        var jsonArr = JSON.parse(pako.inflate(compressedData, { to: 'string' }));
        availableNames = [];
        $("#Filter_search").css("display", "block");
        for (var i = 0; i < jsonArr.length; i++) {
            availableNames.push(jsonArr[i].EmpName);
            if (emprole > 1 && emprole != 6805 && emprole != 6806) {
                if (jsonArr.length == 1) {
                    if (jsonArr[i].EmpId == empid) {
                        $("#Filter_search").css("display", "none");
                    }
                }
            }
        }
        $("#SearchName").autocomplete("option", "source", availableNames);
    }

    //set todays date on clock
    const today = new Date();
    const day = today.getDate();
    const month = today.toLocaleString('default', { month: 'long' });
    document.getElementById('currentDay').textContent = day;
    document.getElementById('currentMonth').textContent = month;


    // security of webpunch
    var hashedOne = CryptoJS.SHA256("1").toString(CryptoJS.enc.Hex);
    var hashedThree = CryptoJS.SHA256("3").toString(CryptoJS.enc.Hex);
    //quick search and role rights
    function quicksearch(empid) {
        checkSession();
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 1, hierarchyState: hierarchy },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },

            success: function (response) {

                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                }
                if (response == null || response == "" || response.length <= 1) {
                    $("#teamAttendce").css("display", "none");
                    $("#clicking").css("display", "none");
                    $("#filterclicking").css("display", "none");

                    if (empid == originalemployeeid) {
                        if (hashedWorf === hashedOne || hashedWorf === hashedThree) {
                            $(".webpunch_block").css("display", "flex");
                            getpunchmode(originalemployeeid);

                        }


                    }
                    else {
                        $(".webpunch_block").css("display", "none");
                    }
                    $(".loading").hide();
                }
                else {
                    $("#teamAttendce").css("display", "block");
                    $("#clicking").css("display", "block");
                    $("#filterclicking").css("display", "block");

                    if (empid == originalemployeeid) {
                        if (hashedWorf === hashedOne || hashedWorf === hashedThree) {
                            $(".webpunch_block").css("display", "flex");
                        }
                    }
                    else {
                        $(".webpunch_block").css("display", "none");
                    }
                    Teamattendence(empid);                    
                }
                $(".loading").hide();
                if ($("#teamAttendce").is(":visible")) {
                    $('a[href="#overview"]').closest("li").show();
                } else {
                    $('a[href="#overview"]').closest("li").hide();
                    $('a[href="#myexpense"]').tab('show');
                }

                if (roleId == 1 || roleId == 6805 || roleId == 6806) {
                    $("#filterclicking").css("display", "none");
                    $("#clicking").css("display", "none");
                }
            },
            beforeSend: function () {
                $(".loading").show();
            },
            error: function () {
                $(".loading").hide();
            }
        });
    }
    //Team Attendance - Total modal open code ===============================
    $("#total_strength").on('click', function () {

        var totalCount = parseInt($("#Total_count").text(), 10);
        if (!isNaN(totalCount) && totalCount > 0) {
            $("#total_strength").attr("data-toggle", "modal");
            $("#total_strength").attr("data-target", "#teamtotal_modal");
        }
        strengthDetails();

    });

    //Team Attendance - Checkin modal open code ===============================
    $("#checking_strength").on('click', function () {
        var totalCount1 = parseInt($("#checkin_count").text(), 10);
        if (!isNaN(totalCount1) && totalCount1 > 0) {
            $("#checking_strength").attr("data-toggle", "modal");
            $("#checking_strength").attr("data-target", "#teamcheckin_modal");
        }
        CheckinDetails();


    });

    //Team Attendance - Absent modal open code ===============================
    $("#absent_strength").on('click', function () {

        var totalCount2 = parseInt($("#absent_count").text(), 10);
        if (!isNaN(totalCount2) && totalCount2 > 0) {
            $("#absent_strength").attr("data-toggle", "modal");
            $("#absent_strength").attr("data-target", "#teamabsent_modal");
        }
        AbsentDetails();


    });

    //Team Attendance - Leave modal open code ===============================
    $("#leave_strength").on('click', function () {
        var totalCount3 = parseInt($("#leave_count").text(), 10);
        if (!isNaN(totalCount3) && totalCount3 > 0) {
            $("#leave_strength").attr("data-toggle", "modal");
            $("#leave_strength").attr("data-target", "#teamleave_modal");
        }
        LeaveDetailsDetails();

    });

    //Team Attendance - Late In modal open code ===============================
    $("#latein_strength").on('click', function () {
        var totalCount4 = parseInt($("#latein_count").text(), 10);
        if (!isNaN(totalCount4) && totalCount4 > 0) {
            $("#latein_strength").attr("data-toggle", "modal");
            $("#latein_strength").attr("data-target", "#teamlatein_modal");
        }
        LateinDetails()

    });
    //Team Attendance - Late Out modal open code ===============================
    $("#lateout_strength").on('click', function () {
        var totalCount5 = parseInt($("#lateout_count").text(), 10);
        if (!isNaN(totalCount5) && totalCount5 > 0) {
            $("#lateout_strength").attr("data-toggle", "modal");
            $("#lateout_strength").attr("data-target", "#teamlateout_modal");
        }
        LateOutDetails();

    });

    //Team Attendance - Early In modal open code ===============================
    $("#earlyin_strength").on('click', function () {
        var totalCount6 = parseInt($("#earlyin_count").text(), 10);
        if (!isNaN(totalCount6) && totalCount6 > 0) {
            $("#earlyin_strength").attr("data-toggle", "modal");
            $("#earlyin_strength").attr("data-target", "#teamearlyin_modal");
        }
        EarlyInDetails();

    });
    //Team Attendance - Early Out modal open code ===============================
    $("#earlyout_strength").on('click', function () {
        var totalCount7 = parseInt($("#earlyout_count").text(), 10);
        if (!isNaN(totalCount7) && totalCount7 > 0) {
            $("#earlyout_strength").attr("data-toggle", "modal");
            $("#earlyout_strength").attr("data-target", "#teamearlyout_modal");
        }
        Earlyoutdetails();

    });


    // block for anonoucements table--no change in api and sp
    function annoucement_getData() {
        const data = {
            CompanyId: cmpId,
            BranchId: branchId,
            StartDate: "",
            EndDate: "",
            Title: "",
            Description: "",
            EmailNotification: 0,
            SmsNotification: 0,
            ReqId: 1,
        };
        $.ajax({
            type: "POST",
            url: `${webApiUrl}DashBoard/GetAnnoucementdata`,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(data),
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {
                if (data == null || data == "") {
                    $("#annoucement-empty").show();
                    $("#annoucement-data").hide();
                } else {
                    $("#annoucement-empty").hide();
                    $("#annoucement-data").show();



                    if (data.isRedirect) {
                        window.location.href = data.redirectUrl;
                    } else {

                        let availableSlides = data.length;
                        if (availableSlides < 1) {
                            $("#annoucementdetails").show();
                        }
                        else {

                            $("#annoucementdetails").hide();

                            for (let i = 0; i < availableSlides; i++) {
                                if (data[i]) {

                                    document.getElementById(`annouce_title_${i + 1}`).textContent = data[i].annoucementtitle;
                                    document.getElementById(`annouce_des_${i + 1}`).textContent = data[i].Description;
                                    if (data[i].AnnoucementImage) {
                                        document.getElementById(`announceimg_${i + 1}`).src = data[i].AnnoucementImage;
                                        $(`#announceimg_${i + 1}`).css("display", "inline-block"); // or "block"
                                        document.getElementById(`download_img_${i + 1}`).href = data[i].AnnoucementImage;
                                        document.getElementById(`download_img_${i + 1}`).setAttribute("download", `announcement_${i + 1}.jpg`);
                                    } else {
                                        $(`#announceimg_${i + 1}`).css("display", "none");
                                    }
                                    $(`#annouce_title_${i + 1}`).css("visibility", "visible");
                                    $(`#annouce_des_${i + 1}`).css("visibility", "visible");
                                    $(`#dot_${i + 1}`).css("visibility", "visible");
                                }
                            }


                            for (let i = availableSlides; i < 3; i++) {
                                $(`#annouce_title_${i + 1}`).css("display", "none");
                                $(`#annouce_des_${i + 1}`).css("display", "none");
                                $(`#announceimg_${i + 1}`).css("display", "none");
                                $(`#dot_${i + 1}`).css("display", "none");
                            }
                        }
                    }
                }
            },
            error: function () {
                console.log("Error in annoucements");
            }
        });
        Fillactivity_HolidayData();
    }

    //Block for holiday show----no change in api and sp

    function Fillactivity_HolidayData() {
        $.ajax({
            type: "GET",
            //url: `${webApiUrl}DashBoard/GetActivityMonitorCounts`,
            //data: { cmpId: cmpId, branchId: branchId, reqid: 10 },
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 17, hierarchyState: hierarchy },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {

                if (data == null || data == "") {
                    $("#holiday-empty").show();
                    $("#holiday-data").hide();

                } else {
                    $("#holiday-empty").hide();
                    $("#holiday-data").show();
                    if (data.isRedirect) {
                        window.location.href = data.redirectUrl;
                    } else {
                        const holidayContainer = $("#Main_holiday");
                        holidayContainer.empty(); // Clear previous holidays if any

                        data.forEach(holiday => {

                            const holidayHtml = `                        
                          <div class="holiday_detail pr_detail emp_dash">
                            <p class="h_month">${holiday.HolidayMonth} ${holiday.HolidayYear}</p>
                            <div class="d-flex align-items-center">
                                <div class="h_box">
                                    <div class="h_date">${holiday.HolidayDay}</div>
                                    <div class="h_day">${new Date(holiday.HolidayYear, new Date(holiday.HolidayMonth + " 1").getMonth(), holiday.HolidayDay).toLocaleDateString("en-US", { weekday: 'short' })}</div>
                                </div>
                                <div class="margin-left-15">
                                    <p class="pr_ttl">${holiday.HolidayName}</p>
                                    
                                </div>
                            </div>
                        </div>                  
                    `;

                            holidayContainer.append(holidayHtml);
                        });
                    }
                }
            },
            error: function () {
                console.log("Error in Holiday Section.");
            }
        });

    }

    //get leave balance for the  employee
    function Filltblleavebalance(empids) {

        $('.leavebalance_block').html(''); 

        $.ajax({
            type: "GET",
            url: webApiUrl + "DashBoard/Fillleavesummary?typeid=1&empid=" + empids,
            headers: { 'Authorization': token },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data == null || data == "") {
                    $("#leavesummary1").hide();
                    $("#leavesummary-empty").show();
                    $("#leavetype_balance").hide();
                }
                else {
                    $("#leavesummary1").show();
                    $("#leavesummary-empty").hide();
                    $("#leavetype_balance").show();

                   
                    var totalleave = 0;
                    var remainingleavetotal = 0;
                    var consumedleave = 0;
                    var requestedleave = 0;

                    for (var i = 0; i < data.length; i++) {
                        const leaveType = data[i].LeaveTypeName;
                        var leaveBalance = parseFloat(data[i].LeaveBalance) || 0;
                        var consumDays = parseFloat(data[i].ConsumDays) || 0;
                        var requestedDays = parseFloat(data[i].RequestedDays) || 0;
                        var RemainingLeave = leaveBalance - consumDays - requestedDays;

                        
                        var consumedPercentage = leaveBalance > 0 ? ((consumDays / leaveBalance) * 100).toFixed(2) : 0;
                        const requestedPercentage = leaveBalance > 0 ? ((requestedDays / leaveBalance) * 100).toFixed(2) : 0;
                        const remainingPercentage = leaveBalance > 0 ? ((RemainingLeave / leaveBalance) * 100).toFixed(2) : 0;

                        consumedPercentage = 100 - requestedPercentage - remainingPercentage;
                        if (consumedPercentage < 0) consumedPercentage = 0;
                   
                        const leaveBlock = `
                        <div class="lb_progressbar">
                            <div class="d-flex align-items-center justify-content-between margin-bottom-5">
                                <p class="lb_progressbar_title">${leaveType}</p>
                                <p class="lb_progressbar_count">
                                    <span class="remaining-leave" style="color:#64b790;">${RemainingLeave}</span>
                                    /<span>${leaveBalance}</span>
                                </p>
                            </div>
                            <div class="progress">
                               
                                <div class="progress-bar tooltips" role="progressbar"
                                     style="width: ${remainingPercentage}%; background-color: #64b790;"
                                     aria-valuenow="${remainingPercentage}"
                                     aria-valuemin="0" aria-valuemax="100"
                                     data-original-title="Remaining: ${RemainingLeave}"
                                     data-placement="top"></div>
                            
                                ${requestedDays > 0 ? `<div class="progress-bar tooltips" role="progressbar"
                                     style="width: ${requestedPercentage}%; background-color: #FBA06D;"
                                     aria-valuenow="${requestedPercentage}"
                                     aria-valuemin="0" aria-valuemax="100"
                                     data-original-title="Pending: ${requestedDays}"
                                     data-placement="top"></div>` : ''}
                            
                                <div class="progress-bar tooltips" role="progressbar"
                                     style="width: ${consumedPercentage}%; background-color: #dd5e6a;"
                                     aria-valuenow="${consumedPercentage}"
                                     aria-valuemin="0" aria-valuemax="100"
                                     data-original-title="Consumed: ${consumDays}"
                                     data-placement="top"></div>
                            </div>
                        </div>
                    `;

                       
                        totalleave += leaveBalance;
                        remainingleavetotal += RemainingLeave;
                        consumedleave += consumDays;
                        requestedleave += requestedDays;

                   
                        $('.leavebalance_block').append(leaveBlock);
                    }

                   
                    $("#total_counts1").text(totalleave);
                    $("#consumed_counts1").text(consumedleave);
                    $("#remainanig_counts1").text(remainingleavetotal);
                    $("#requested_counts1").text(requestedleave);
                    
                    // Initialize tooltips for progress bars
                    $('.tooltips').tooltip();
                }
            },
            error: function () {
                console.log("Error while fetching leave balance data.");
            }
        });
    }

    //fill dropdown for celeneder 
    $("#filterclicking").on("click", function (e) {

        e.stopPropagation();
        e.preventDefault();
        $('#filterInputs').val('');
        $('#FilterList li').show();
        $(".employeecelender").toggle();
    });


    function EmployeeDropdownForCelender(empid) {

        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 1, hierarchyState: hierarchy },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },

            success: function (response) {

                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                }
                if (response != null) {


                    $('#FilterList').empty();


                    for (var i = 0; i < response.length; i++) {
                        $('#FilterList').append(
                            '<li data-id="' + response[i].empid + '">' +
                            '<a href="#" class="name-link">' + response[i].empname + '</a>' +
                            '</li>'
                        );
                    }
                }
                $(".loading").hide();
            },
            beforeSend: function () {
                $(".loading").show();
            },
            error: function () {
                $(".loading").hide();
            }
        });
    }


    //// On Search dropdown should not close.
    $('.employeecelender, .employeeleavebalance').on('click keyup', function (e) {
        e.stopPropagation();
    });

    //Get data according to the particular employee
    $('#FilterList').on('click', 'li', function (e) {
        const btn = document.querySelector("#filterclicking .btn");
        if (btn) {
            btn.style.color = "green";
        }
        checkSession();
        employeeids = $(this).data('id');
        var b = $('#MyCalendar').fullCalendar('getDate');
        /* var FirstDayoftheMonth = b.format("YYYY-MM-DD");*/
        $("#FilterList li").removeClass("active");
        $(`#FilterList li[data-id="${employeeids}"]`).addClass("active");
        var addmonth = "";
        var StartDate = new Date(b.format("YYYY-MM-DD"));
        var FirstDayoftheMonth = StartDate.getFullYear() + "-" +
            (StartDate.getMonth() + 1).toString().padStart(2, '0') +
            "-01";
        var EndDate = new Date(StartDate.getFullYear(), StartDate.getMonth() + 1, 0);
        if (EndDate.getMonth() < 10 && EndDate.getMonth() != 9) {
            addmonth = "0" + (EndDate.getMonth() + 1);
        }
        else {
            addmonth = (EndDate.getMonth() + 1);
        }
        var lastDayoftheMonth = EndDate.getFullYear() + "-" + addmonth + "-" + EndDate.getDate();
        $.ajax({
            type: "GET",
            url: webapiuri + "DashBoard/GetEmpAttendanceStatusSummary",
            headers: { 'Authorization': token },
            data: {
                empid: employeeids,
                fromdate: FirstDayoftheMonth,
                todate: lastDayoftheMonth,
                statusflg: 3,
                finalStatus: "",
                searchName: $("#searchselection").val(),
                searchBy: $("#SearchName").val()
            },
            success: function (events) {
                if (events.isRedirect) {
                    window.location.href = events.redirectUrl;
                }
                $('#MyCalendar').fullCalendar('removeEvents');
                $('#MyCalendar').fullCalendar('addEventSource', events);
                $('#MyCalendar').fullCalendar('rerenderEvents');
            },
            error: function () {
                alert("error");
            }
        });
        CalendarListFill(FirstDayWithSlashes, lastDayWithSlashes, employeeids);


        if (empid != employeeids) {
            $('.fc-prev-button').click(function () {

                $('#MyCalendar').fullCalendar('removeEvents');
                var b = $('#MyCalendar').fullCalendar('getDate');
                var FirstDayoftheMonth = b.format("YYYY-MM-DD");

                var addmonth = "";
                var StartDate = new Date(b.format("YYYY-MM-DD"));
                var EndDate = new Date(StartDate.getFullYear(), StartDate.getMonth() + 1, 0);
                if (EndDate.getMonth() < 10 && EndDate.getMonth() != 9) {
                    addmonth = "0" + (EndDate.getMonth() + 1);
                }
                else {
                    addmonth = (EndDate.getMonth() + 1);
                }
                var lastDayoftheMonth = EndDate.getFullYear() + "-" + addmonth + "-" + EndDate.getDate();
                //if ($("#employeeId").val() != '0')
                //{
                //    ids = $("#employeeId").val();
                //}
                CalendarListFill(FirstDayoftheMonth, lastDayoftheMonth, employeeids)
                $.ajax({
                    type: "GET",
                    url: webApiUrl + "DashBoard/GetEmpAttendanceStatusSummary",
                    headers: { 'Authorization': token },
                    data: {
                        empid: employeeids,
                        fromdate: FirstDayoftheMonth,
                        todate: lastDayoftheMonth,
                        statusflg: 3,
                        finalStatus: "",
                        searchName: $("#searchselection").val(),
                        searchBy: $("#SearchName").val()
                    },
                    success: function (events) {
                        $('#MyCalendar').fullCalendar('removeEvents');
                        $('#MyCalendar').fullCalendar('addEventSource', events);
                        $('#MyCalendar').fullCalendar('rerenderEvents');
                    },
                    error: function () {
                        alert("error");
                    }
                });
            });
            $('.fc-next-button').click(function () {
                $('#MyCalendar').fullCalendar('removeEvents');
                var b = $('#MyCalendar').fullCalendar('getDate');
                var FirstDayoftheMonth = b.format("YYYY-MM-DD");
                var addmonth = "";
                var StartDate = new Date(b.format("YYYY-MM-DD"));
                var EndDate = new Date(StartDate.getFullYear(), StartDate.getMonth() + 1, 0);
                if (EndDate.getMonth() < 10 && EndDate.getMonth() != 9) {
                    addmonth = "0" + (EndDate.getMonth() + 1);
                }
                else {
                    addmonth = (EndDate.getMonth() + 1);
                }
                var lastDayoftheMonth = EndDate.getFullYear() + "-" + addmonth + "-" + EndDate.getDate();
                //if ($("#employeeId").val() != '0') {
                //    ids = $("#employeeId").val();
                //}
                CalendarListFill(FirstDayoftheMonth, lastDayoftheMonth, employeeids)
                $.ajax({
                    type: "GET",
                    url: webApiUrl + "DashBoard/GetEmpAttendanceStatusSummary",
                    headers: { 'Authorization': token },
                    data: {
                        empid: employeeids,
                        fromdate: FirstDayoftheMonth,
                        todate: lastDayoftheMonth,
                        statusflg: 3,
                        finalStatus: "",
                        searchName: $("#searchselection").val(),
                        searchBy: $("#SearchName").val()
                    },
                    success: function (events) {
                        $('#MyCalendar').fullCalendar('removeEvents');
                        $('#MyCalendar').fullCalendar('addEventSource', events);
                        $('#MyCalendar').fullCalendar('rerenderEvents');
                    },
                    error: function () {
                        alert("error");
                    }
                });
            });

        }

        $('#filterInputs').val('');
        $('#FilterList li').show();
        $('.employeecelender').hide();
        const selectedName = $(this).text().trim();
        const tooltipElement = $('#filterclicking a.tooltips');
        tooltipElement.attr('data-original-title', `${selectedName}`).tooltip('fixTitle');


    });
    $("#filterInputs").on('keyup', function (e) {

        const filter = $(this).val().toLowerCase();
        $("#FilterList li").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(filter) > -1);
        });
    });

    //fill dropdown for leave balance

    $("#clicking").on("click", function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#myInput').val('');
        $('#nameList li').show();
        $(".employeeleavebalance").toggle(); // Toggle dropdown visibility
    });

    function EmployeeListForLeavebalance(empid) {
        checkSession();

        $('a[href="#leavesummary"]').tab('show');

        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 1, hierarchyState: hierarchy },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },

            success: function (response) {

                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                }
                if (response != null) {

                    //response.sort(function (a, b) {
                    //    return a.Text.localeCompare(b.Text);
                    //});


                    $('#nameList').empty();


                    for (var i = 0; i < response.length; i++) {


                        $('#nameList').append(
                            '<li data-id="' + response[i].empid + '">' +
                            '<a href="#" class="name-link">' + response[i].empname + '</a>' +
                            '</li>'
                        );
                    }
                }
                $(".loading").hide();
            },
            beforeSend: function () {
                $(".loading").show();
            },
            error: function () {
                $(".loading").hide();
            }
        });

    };

    $(document).on('click', function (e) {

        const $dropdown = $(".employeecelender");
        const $toggleButton = $(".employeeleavebalance");

        if (!$(e.target).closest($dropdown).length && !$dropdown.is(e.target)) {
            $('#myInput').val('');
            $('#filterInputs').val('');
            $('#nameList li').show();
            $('#FilterList li').show();
            $dropdown.hide();
        }

        if (!$(e.target).closest($toggleButton).length && !$toggleButton.is(e.target)) {
            $toggleButton.hide();
        }
    });

    //Get data according to the particular employee
    $('#nameList').on('click', 'li', function (e) {
        const btn = document.querySelector("#clicking .btn");
        if (btn) {
            btn.style.color = "green";
        }
        e.preventDefault();
        $('#nameList li').removeClass('active');

        // Add the active class to the clicked list item
        $(this).addClass('active');

        var employeeid = $(this).data('id');
        Filltblleavebalance(employeeid);
        selectedemp = employeeid;
        $('#myInput').val('');
        $('#nameList li').show();
        $('.employeeleavebalance').hide();
        const selectedName = $(this).text().trim();
        const tooltipElement = $('#clicking a.tooltips');
        tooltipElement.attr('data-original-title', `${selectedName}`).tooltip('fixTitle');
    });

    // Add the autocomplete filter logic
    $("#myInput").on('keyup', function () {

        const filter = $(this).val().toLowerCase();
        $("#nameList li").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(filter) > -1);
        });
    });


    //on click on attendece summary
    $("#leaveconsumption").on('click', function (e) {

        /*  $('[href="#leavesummary"]').tab('hide');*/


        let employeeIdToUse = selectedemp ? selectedemp : empid;
        Fillleavesummary(employeeIdToUse);
    });

    // Get details for leave balance
    function Fillleavesummary(empids) {
        if ($.fn.DataTable.isDataTable('#attendancesummary_tbles')) {
            $('#attendancesummary_tbles').DataTable().destroy();
        }
        $('#attendancesummary_tbles tbody').html('');
        $.ajax({
            type: "GET",
            url: webApiUrl + "DashBoard/Fillleavesummary?typeid=2&empid=" + empids,
            headers: { 'Authorization': token },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data == null || data == "") {
                    $("#leaveconsume1").hide();
                    $("#summary1-empty").show();
                }
                else {
                    $("#leaveconsume1").show();
                    $("#summary1-empty").hide();

                    $('#attendancesummary_tbles tbody').empty();
                    var tr;
                    for (var i = 0; i < data.length; i++) {
                        tr = $('<tr/>');
                        tr.append("<td>" + data[i].LeaveTypeName + "</td>");
                        tr.append("<td>" + data[i].fromdate + "</td>");
                        tr.append("<td>" + data[i].Todate + "</td>");
                        tr.append("<td>" + data[i].ApplyReason + "</td>");
                        tr.append("<td>" + data[i].ConsumDays + "</td>");
                        $('#attendancesummary_tbles tbody').append(tr);
                    }
                    $('#attendancesummary_tbles').DataTable({
                        paging: false,
                        searching: false,
                        info: false,
                        ordering: true,
                        order: []
                    });
                }
            },
            error: function () {
                console.log("Error while fetching leave summary data.");
            }
        });
    }

    // Get data for my leaves
    function MyLeave(empid) {
        checkSession();
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 2 },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },

            success: function (response) {

                if (response == null || response == "") {
                    $("#leaveDataContainer").hide();
                    $("#myleave-empty").show();
                    $(".loading").hide();
                }
                else {
                    $("#leaveDataContainer").show();
                    $("#myleave-empty").hide();

                    if (response.isRedirect) {
                        window.location.href = response.redirectUrl;
                    }
                    if (response != null) {
                        const getStatusText = (status) => {
                            switch (status) {
                                case 1:
                                    return "Approved";
                                case 2:
                                    return "Rejected";
                                case 3:
                                    return "Pending";
                                case 4:
                                    return "Cancelled";
                                default:
                                    return "Unknown";
                            }
                        };

                        const leaveDataContainer = document.getElementById("leaveDataContainer");
                        leaveDataContainer.innerHTML = "";

                        response.forEach((leave) => {
                            const leaveHTML = `
                                    <div class="myleave_detail">
                                        <div class="d-flex align-items-center justify-content-between">
                                            <div class="d-flex align-items-center">
                                                <div class="h_box">
                                                    <div class="h_date">${leave.from_day}${leave.from_day !== leave.to_day ? '-' + leave.to_day : ''}</div>
                                                    <div class="h_day">${leave.from_month}</div>
                                                </div>
                                                <div class="margin-left-10">
                                                    <p class="pr_ttl_new">${leave.applyreason}</p>
                                                    <p class="pr_ttlsub">${leave.leavetypename}</p>
                                                </div>
                                            </div>
                                            <div class="${leave.leavestatus === 1 ? 'green_font' : (leave.leavestatus === 4 || leave.leavestatus === 2 ? 'red_font' : 'orange_font')}">
                                                ${getStatusText(leave.leavestatus)}
                                            </div>

                                        </div>
                                    </div>
                                `;

                            leaveDataContainer.innerHTML += leaveHTML;
                        });


                    }
                    $(".loading").hide();
                }
            },
            beforeSend: function () {
                $(".loading").show();
            },
            error: function () {
                $(".loading").hide();
            }
        });

    }

    $("#leaveStatus").on('click', function () {

        whosonleave();

    });

    //Get Data for who's on leave
    function whosonleave() {
        checkSession();
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, DepartmentId: DepartmentId, reqid: 3 },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },

            success: function (response) {

                if (response == null || response == "") {
                    $("#leaveDataContainerwho").hide();
                    $("#whosonleave-empty").show();
                    $(".loading").hide();
                }
                else {
                    $("#leaveDataContainerwho").show();
                    $("#whosonleave-empty").hide();

                    if (response.isRedirect) {
                        window.location.href = response.redirectUrl;
                    }
                    if (response != null) {

                        let leaveHTML = ''; // Initialize HTML string
                        const leaveDataContainer = document.getElementById("leaveDataContainerwho");

                        const getDateString = (date) => {
                            const parsedDate = new Date(date);
                            const year = parsedDate.getFullYear();
                            const month = String(parsedDate.getMonth() + 1).padStart(2, '0');
                            const day = String(parsedDate.getDate()).padStart(2, '0');
                            return `${year}-${month}-${day}`;
                        };

                        response.forEach(item => {

                            // Format leave duration and status
                            const today = new Date();
                            const fromDate = new Date(item.fromdate);
                            const toDate = new Date(item.todate);
                            const todayDateString = getDateString(today);
                            const fromDateString = getDateString(fromDate);
                            const toDateString = getDateString(toDate);

                            let leaveStatus;
                            if (todayDateString === fromDateString && todayDateString === toDateString) {
                                leaveStatus = 'Today';
                            } else if (fromDateString === toDateString) {
                                leaveStatus = `${item.to_day} ${item.to_month}`;
                            } else {
                                leaveStatus = `${item.from_day}-${item.to_day} ${item.to_month}`;
                            }

                            const leaveDuration = item.days === 1 ? '1 Day' : `${item.days} Days`;
                            const leaveClass = item.days <= 1 ? 'green_font' : 'green_font';

                            // Construct HTML for each leave entry
                            leaveHTML += `
                        <div class="myleave_detail">
                            <div class="d-flex align-items-center justify-content-between">
                                <div class="d-flex align-items-center">
                                    <img class="user_icon" src="/assets/images/avtar.png" alt="User Image" />
                                    <div class="margin-left-10">
                                        <p class="pr_ttl_new">${item.empname}</p>
                                        <p class="pr_ttlsub">${item.designationname}</p>
                                    </div>
                                </div>
                                <div class="text-right">
                                    <div class="${leaveClass}">${leaveStatus}</div>
                                    <span class="pr_ttlsub">${leaveDuration}</span>
                                </div>
                            </div>
                        </div>`;
                        });

                        // Append the constructed HTML to the container once
                        leaveDataContainer.innerHTML = leaveHTML;
                    }

                    $(".loading").hide();
                }
            },
            beforeSend: function () {
                /* $(".loading").show();*/
            },
            error: function () {
                $(".loading").hide();
            }
        });
    }

    //get pending leave counts
    function Getpendingleave_count() {
        $("#test_changes").text("Last 30 Days");
        $.ajax({
            type: "POST",
            url: `${webApiUrl}/DashBoard/GetEmployeePendingCount?roleid=${emprole}&companyid=${cmpId}&branchid=${branchId}&empId=${originalempid}`,
            headers: { 'Authorization': token },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (response) {

                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                } else {
                    response.forEach(activity => {
                        var cnt = activity.cnt || 0;
                        var statusClass = cnt <= 5 ? 'status_green' : cnt <= 10 ? 'status_orange' : 'status_red';
                        if (activity.type === 'webpunch') {
                            $('#webpunchCount').text(cnt).removeClass('status_green status_orange status_red').addClass(statusClass);
                        } else if (activity.type === 'Attendance') {
                            $('#attendanceCount').text(cnt).removeClass('status_green status_orange status_red').addClass(statusClass);
                        } else if (activity.type === 'leave') {
                            $('#leaveCount').text(cnt).removeClass('status_green status_orange status_red').addClass(statusClass);
                        }
                    });
                }
            },
            error: function () {
                toastr.remove();
                toastr.error("Error in the pending leave counts.");
                //setTimeout(() => {
                //    window.location.href = "/PayTime/LoginPage";
                //}, 2000);
            }
        });


        annoucement_getData();

    }
    $("#pendingrequest2").click(function () {
        Getpendingleave_count();

    });

    $("#Myattendancecorrection").click(function () {
        $("#test_changes").text("Last 7 days");
        attendenceCorrection();

    });

    $("#Mywebpunchcorrection").click(function () {
        $("#test_changes").text("Last 7 days");
        Webpunch();

    });

    //get myattendencecorrection data
    function attendenceCorrection() {

        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 4 },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },

            success: function (response) {
                if (response == null || response == "") {
                    $("#correction-data").hide();
                    $("#correction-empty").show();
                    $(".loading").hide();
                }
                else {
                    $("#correction-data").show();
                    $("#correction-empty").hide();

                    const getStatusText = (status) => {
                        switch (status) {
                            case 1:
                                return { text: "Approved", class: "green_font" };
                            case 2:
                                return { text: "Rejected", class: "red_font" };
                            case 3:
                                return { text: "Pending", class: "orange_font" };
                            case 4:
                                return { text: "cancelled", class: "red_font" };
                            default:
                                return { text: "Unknown", class: "grey_font" };
                        }
                    };
                    if (response.isRedirect) {
                        window.location.href = response.redirectUrl;
                    }
                    if (response != null) {
                        const tableBody = $("#attendancesummary_tbl tbody");
                        tableBody.empty();

                        response.forEach(item => {
                            const { text: statusText, class: statusClass } = getStatusText(item.status); // Get status text and class
                            const row = `
                        <tr>
                            <td>${item.Punchdate}</td>
                            <td>${item.Inpunch}</td>
                            <td>${item.Outpunch}</td>
                            <td>${item.applyreason}</td>
                            <td class="${statusClass}">${statusText}</td>

                        </tr>
                    `;
                            tableBody.append(row);
                        });






                    }
                    $(".loading").hide();
                }
            },
            beforeSend: function () {
                $(".loading").show();
            },
            error: function () {
                $(".loading").hide();
            }
        });
    }

    //get WEBPUNCH   data
    function Webpunch() {

        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 5 },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },

            success: function (response) {

                if (response == null || response == "") {
                    $("#web-data").hide();
                    $("#webpunch-empty").show();
                    $(".loading").hide();
                }
                else {
                    $("#web-data").show();
                    $("#webpunch-empty").hide();

                    const getStatusText = (isapprove) => {
                        switch (isapprove) {
                            case 1:
                                return { text: "Approved", class: "green_font" };
                            case 0:
                                return { text: "Pending", class: "orange_font" };
                            case 3:
                                return { text: "Cancelled", class: "red_font" };
                            case 2:
                                return { text: "Rejected", class: "red_font" };
                            default:
                                return { text: "Unknown", class: "grey_font" };
                        }
                    };
                    if (response.isRedirect) {
                        window.location.href = response.redirectUrl;
                    }
                    if (response != null) {
                        const tableBody = $("#webpunchs_tbl tbody");
                        tableBody.empty();

                        response.forEach(item => {
                            const { text: statusText, class: statusClass } = getStatusText(item.isapprove); // Get status text and class
                            const row = `
                        <tr>
                            <td>${item.punchdate}</td>
                            <td>${item.Inpunch}</td>
                           
                           <td>${item.Punchid}</td>
                            <td class="${statusClass}">${statusText}</td>

                        </tr>
                    `;
                            tableBody.append(row);
                        });






                    }
                    $(".loading").hide();
                }
            },
            beforeSend: function () {
                $(".loading").show();
            },
            error: function () {
                $(".loading").hide();
            }
        });
    }

    $("#webpunchdata").click(webpunchdetails);
    function webpunchdetails() {
        checkSession();
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 8 },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },
            success: function (response) {



                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                } else {
                    // Check if data is present
                    if (response && response.length > 0) {
                        $("#webdetails").hide();
                        $("#webpunchdata_table tbody").empty();
                        $("#webpunchdata_table").show();

                        response.forEach((item, index) => {
                            const status = item.IsApprove === 1 ? "Approved" : item.IsApprove === 2 ? "Rejected" : "Pending";

                            const row = `
                            <tr>
                                <td>${index + 1}</td>
                                <td>${item.PunchDate}</td>
                                <td>${item.mode}</td>
                                <td>${item.punchtime}</td>
                                <td>${status}</td>
                            </tr>`;
                            $("#webpunchdata_table tbody").append(row);
                        });


                        $("#webpunchdata_modal").modal("show");
                    } else {
                        $("#webpunchdata_table").hide();
                        $("#webdetails").show();
                    }
                }
                $(".loading").hide();
            },
            beforeSend: function () {
                $(".loading").show();
            },
            error: function () {
                $(".loading").hide();
                toastr.error("Error in Webpunch Details!");
            }
        });
    }
    //Mantra Remaning workinghrs

    $("#Workinghrsdata").click(PunchWorkinghrsInfo);
    function PunchWorkinghrsInfo() {
        checkSession();
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/GetPunchWorkinghrsInfo`,
            data: { empid: empid },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },
            success: function (response) {
                $(".loading").hide();
                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                } else {
                    // Check if data is present
                    if (response && response.length > 0) {
                        $("#Workinghrs_summary").hide();
                        $("#Workinghrsdata_table tbody").empty();
                        $("#Workinghrsdata_table").show();

                        response.forEach((item, index) => {
                            var row = '';
                            if (item.Attn_Dt === "Total") {
                                row = `<tr style='color: #3F51B5;'>
                                <td colspan='4'>${item.Out_Time}</td>
                                <td>${item.Wrkhrs}</td>
                            </tr>`;
                            }
                            else {
                                row = `<tr>
                                <td>${index + 1}</td>
                                <td>${item.Attn_Dt}</td>
                                <td>${item.In_Time}</td>
                                <td>${item.Out_Time}</td>
                                <td>${item.Wrkhrs}</td>
                                </tr>`;
                            }
                            $("#Workinghrsdata_table tbody").append(row);
                        });

                        $("#Workinghrsdata_modal").modal("show");
                    } else {
                        $("#Workinghrsdata_table").hide();
                        $("#Workinghrs_summary").show();
                    }
                }
                $(".loading").hide();
            },
            beforeSend: function () {
                $(".loading").show();
            },
            error: function () {
                $(".loading").hide();
                toastr.error("Error in Working hrs Details!");
            }
        });
    }
    //Get Team Attendece data
    function Teamattendence(empid) {
        // Get details for leave balance
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 6, hierarchyState: hierarchy },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: {
                'Authorization': token
            },
            success: function (response) {

                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                } else {
                    $("#Total_count").text(response[0].total);
                    $("#checkin_count").text(response[0].checkin || '0');
                    $("#absent_count").text(response[0].notcheckin);
                    $("#leave_count").text(response[0].leavecount || '0');
                    $("#latein_count").text(response[0].latein);
                    $("#lateout_count").text(response[0].lateout);
                    $("#earlyin_count").text(response[0].earlyin);
                    $("#earlyout_count").text(response[0].earlyout);

                    // Calculate percentages for progress bars
                    let total = response[0].total || 1;
                    let checkinPercent = ((response[0].checkin || 0) / total) * 100;
                    let absentPercent = ((response[0].notcheckin || 0) / total) * 100;
                    let leavePercent = ((response[0].leavecount || 0) / total) * 100;

                    // Update progress bar widths
                    $(".progress-bar.progress_success_bar").css("width", `${checkinPercent}%`).attr("aria-valuenow", checkinPercent);
                    $(".progress-bar.progress_danger_bar").css("width", `${absentPercent}%`).attr("aria-valuenow", absentPercent);
                    $(".progress-bar.progress_info_bar").css("width", `${leavePercent}%`).attr("aria-valuenow", leavePercent);
                }

            },
            error: function () {
                console.log("Error while fetching Team Attendence data.");
            }
        });

    }

    //Punch details
    function GetPunchInformation(empid) {
        checkSession();
        if ((roleId == '6805' || roleId == '6806') && (empid == 0)) {
            $("#lblInTime").html("00:00:00");
            $("#lblOutTime").html("00:00:00");
            $("#lblRemainTime").html("00:00:00");
        }
        else {
            $.ajax({
                type: "GET",
                url: webApiUrl + "DashBoard/GetPunchTimeDetails?empid=" + empid,
                headers: { 'Authorization': token },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {

                    var obj = data

                    $("#lblInTime").html("00:00:00");
                    $("#lblOutTime").html("00:00:00");
                    $("#lblRemainTime").html("00:00:00");


                    if (obj.length > 0) {
                        var _isNight = obj[0].isNight;
                        $("#hidenightshiftflag").text(_isNight);

                        if (obj[0].InTime == null) {
                            $("#lblInTime").html("00:00:00");
                        }
                        else {
                            //$("#lblInTime").html(obj[0].InTime + ":00");
                            $("#lblInTime").html(obj[0].InTime);
                        }

                        if (obj[0].OutTime == null) {
                            $("#lblOutTime").html("00:00:00");
                        }
                        else {
                            //$("#lblOutTime").html(obj[0].OutTime + ":00");
                            $("#lblOutTime").html(obj[0].OutTime);
                        }

                        if (obj[0].InTime == null && obj[0].OutTime == null) {
                            $("#lblRemainTime").html("00:00:00");
                            $("#hideremaintime").val("00:00:00");
                        }
                        else {
                            var _RemainTime = obj[0].RemainTime;
                            var _OutTime = obj[0].OutTime;

                            $("#hideremaintime").val(obj[0].OutTime);
                            if (_isNight == 0) {
                                if (_OutTime < _RemainTime) {
                                    if (_OutTime > "00:00") {
                                        $("#hidenightshiftflag").text("0");
                                    }
                                    else {
                                        $("#lblRemainTime").html("00:00:00");
                                        $("#hideremaintime").val("00:00:00");
                                    }
                                }
                            }
                            else {
                                if (_OutTime < _RemainTime) {
                                    if (_OutTime > "00:00") {
                                        $("#hidenightshiftflag").text("1");
                                    }
                                    else {
                                        $("#lblRemainTime").html("00:00:00");
                                        $("#hideremaintime").val("00:00:00");
                                    }
                                }
                            }


                        }
                        if ($("#hideremaintime").val() == "00:00:00" || $("#hideremaintime").val() == null) {

                        }
                        else {

                            window.onload = updateclock();
                            setInterval(function () { updateclock(); }, 1000);
                        }
                    }
                },
                error: function () {
                }
            });
        }
    }
    function updateclock() {
        var upday = "";
        var upmonth = "";
        var d = new Date();
        if (d.getDate() < 10) {

            upday = "0" + (d.getDate());
            if ($("#hidenightshiftflag").text() == "1") // Night shift flag
            {
                upday = "0" + (d.getDate() + 1);
            }
        }
        else {
            upday = (d.getDate());
            if ($("#hidenightshiftflag").text() == "1") // Night shift punch flag
            {
                upday = (d.getDate() + 1);
            }
        }

        if ((d.getMonth() + 1) < 10) {
            upmonth = "0" + (d.getMonth() + 1);
        }
        else {
            upmonth = (d.getMonth() + 1);
        }

        var day = d.getFullYear() + "-" + upmonth + "-" + upday;
        var _hideremaintime = $("#hideremaintime").val();
        var rtime = _hideremaintime + ":00";
        var data = new Date(day + " " + rtime);

        var d2 = new Date(data);
        d2.setHours(d2.getHours());
        var DateDiff = new Date(d2) - new Date();
        if (isNaN(DateDiff) == false) {
            if (Math.sign(DateDiff) === 1) {

                var diff = Math.abs(new Date(d2) - new Date());
                var hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
                var minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
                var seconds = Math.floor((diff % (1000 * 60)) / 1000);
                if (hours < 10) {
                    hours = "0" + hours;
                }
                if (minutes < 10) {
                    minutes = "0" + minutes;
                }
                if (seconds < 10) {
                    seconds = "0" + seconds;
                }
                $("#lblRemainTime").html(hours + ":" + minutes + ":" + seconds);
            }
            else {
                $("#lblRemainTime").html("00:00:00");
            }
        }
    }
    checkSession();
    //code for moment 
    $("#EventBtn").on('click', function () {
        $("#EventBtn").attr("data-toggle", "modal");
        $("#EventBtn").attr("data-target", "#EventBtn_modal");
        $.ajax({
            type: "GET",
            url: webApiUrl + "Master/GetanniversaryData",
            headers: { 'Authorization': token },
            success: function (response) {

                if (response != null) {

                    // Destroy any existing DataTable instances before re-initialization
                    if ($.fn.DataTable.isDataTable('#tblbirthday')) {
                        $('#tblbirthday').DataTable().clear().destroy();
                        $("#dataTables_tbl_header").remove();
                    }
                    if ($.fn.DataTable.isDataTable('#tblanniversary')) {
                        $('#tblanniversary').DataTable().clear().destroy();
                        $("#dataTables_tbl_header").remove();
                    }
                    if ($.fn.DataTable.isDataTable('#tbljoining')) {
                        $('#tbljoining').DataTable().clear().destroy();
                        $("#dataTables_tbl_header").remove();
                    }

                    // Initialize DataTable for Birthday Table
                    if (response.Table1 != null && response.Table1.length > 0) {
                        $('#tblbirthday').DataTable({
                            dom: _domCommon,
                            language: _languageCommon,
                            data: response.Table1,
                            //scrollY: 'calc(100vh - 338px)',
                            scrollCollapse: true,
                            paging: true,
                            order: [],
                            columns: [
                                {
                                    data: 'Empcode', title: 'Employee No',
                                    "createdCell": function (td) {
                                        $(td).css({ "min-width": "120px" });
                                    }
                                },   // Map object property to column
                                { data: 'EmpName', title: 'Employee Name' },
                                { data: 'DesignationName', title: 'Designation' },
                                { data: 'BranchName', title: 'Branch' },
                                { data: 'DepartmentName', title: 'Department' },
                                { data: 'EmpDOB', title: 'Birthdate' }
                            ],

                        })
                    }

                    setTimeout(function () {
                        $("#" + "tblbirthday" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsiveBirthday"));
                        $('.tooltips').tooltip();
                    }, 100);

                    // Initialize DataTable for Work Anniversary Table
                    if (response.Table2 != null && response.Table2.length > 0) {
                        $('#tblanniversary').DataTable({
                            dom: _domCommon,
                            language: _languageCommon,
                            data: response.Table2,
                            //scrollY: 'calc(100vh - 338px)',
                            scrollCollapse: true,
                            paging: true,
                            order: [],
                            columns: [
                                {
                                    data: 'Empcode', title: 'Employee No',
                                    "createdCell": function (td) {
                                        $(td).css({ "min-width": "120px" });
                                    }
                                },
                                { data: 'EmpName', title: 'Employee Name' },
                                { data: 'DesignationName', title: 'Designation' },
                                { data: 'BranchName', title: 'Branch' },
                                { data: 'DepartmentName', title: 'Department' },
                                { data: 'EmpJoinDate', title: 'Joining Date' },
                                { data: 'Year', title: 'Year Complete' }
                            ],
                        })
                    }

                    setTimeout(function () {
                        $("#" + "tblanniversary" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsiveAnniversary"));
                        $('.tooltips').tooltip();
                    }, 100);

                    // Initialize DataTable for New Join Employee Table
                    if (response.Table3 != null && response.Table3.length > 0) {

                        $('#tbljoining').DataTable({
                            dom: _domCommon,
                            language: _languageCommon,
                            data: response.Table3,
                            //scrollY: 'calc(100vh - 338px)',
                            scrollCollapse: true,
                            paging: true,
                            order: [],
                            columns: [
                                {
                                    data: 'Empcode', title: 'Employee No',
                                    "createdCell": function (td) {
                                        $(td).css({ "min-width": "120px" });
                                    }
                                },
                                { data: 'EmpName', title: 'Employee Name' },
                                { data: 'DesignationName', title: 'Designation' },
                                { data: 'BranchName', title: 'Branch' },
                                { data: 'DepartmentName', title: 'Department' },
                                { data: 'EmpJoinDate', title: 'Joining Date' }
                            ],
                        });
                    }

                    setTimeout(function () {
                        $("#" + "tbljoining" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsivejoining"));
                        $('.tooltips').tooltip();
                    }, 100);

                    if (response.Table1 != null && response.Table1.length > 0) {

                        $("#divtblbirthday").hide();
                        $("#tblbirthday").show();
                    }
                    else {
                        $("#divtblbirthday").show();
                        $("#tblbirthday").hide();
                    }

                    if (response.Table2 != null && response.Table2.length > 0) {

                        $("#divtblanniversary").hide();
                        $("#tblanniversary").show();
                    }
                    else {
                        $("#divtblanniversary").show();
                        $("#tblanniversary").hide();
                    }

                    if (response.Table3 != null && response.Table3.length > 0) {
                        $("#divttbljoining").hide();
                        $("#tbljoining").show();
                    }
                    else {
                        $("#divttbljoining").show();
                        $("#tbljoining").hide();

                    }
                }
            },
            error: function () {
                toastr.error("Error in moments section.");
            }
        });

    });

    //code for personal details
    function Emppersonaldetails(empid) {
        // Get details for leave balance
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 7 },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: {
                'Authorization': token
            },
            success: function (response) {

                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                } else {
                    $("#empname").text(response[0].empname);
                    $("#reportingname").text(response[0].reportingperson || 'N/A');
                    $("#empcode").text(response[0].empcode || 'N/A');
                    $("#punchid").text(response[0].emppunchid || 'N/A');
                    $("#policyname").text(response[0].policyname || 'N/A');
                    $("#companyname").text(response[0].companyname || 'N/A');
                    $("#branchname").text(response[0].branchname || 'N/A');
                    $("#departmentname").text(response[0].departmentname || 'N/A');
                    $("#designationname").text(response[0].designationname || 'N/A');

                    if (roleId == '1' || roleId == '6805') {
                        $("#mobileno").text(response[0].empmno || 'N/A');
                    }
                    else {
                        let mobileNo = response[0].empmno || 'N/A';
                        if (mobileNo != 'N/A') {
                            let maskedNo = "*******" + mobileNo.slice(-3);
                            $("#mobileno").text(maskedNo);
                        }
                        else {
                            $("#mobileno").text('N/A');
                        }
                    }
                    $("#rolename").text(response[0].rolename || 'N/A');
                    $("#shiftname").text(response[0].shiftname || 'N/A');
                    $("#shiftgroup").text(response[0].ShiftGroupName || 'N/A');
                    $("#shifttime").text((response[0].starttime || 'N/A') + " - " + (response[0].endtime || 'N/A'));
                    $("#joiningdate").text(response[0].empjoindate || 'N/A');
                    $("#dob").text(response[0].empdob || 'N/A');
                    if (CompanyCode == "MAN5F0F") {
                        $("#lblBU").text(response[0].BU || 'N/A');
                        $("#lblSubBU").text(response[0].SubBU || 'N/A');
                    }
                }

            },
            error: function () {
                console.log("Error while fetching empolyee summary data.");
            }
        });

    }

    //code for policy references
    function PolicyReference() {
        // Get details for leave balance
        $.ajax({
            type: "GET",
            url: webApiUrl + 'Master/GetEmployeeReferenceGuide?Roleid=' + emprole + "&ComanyID=" + cmpId + "&BranchID=" + branchId + "&flag=1",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: {
                'Authorization': token
            },
            success: function (response) {


                if (Array.isArray(response) && response.length > 0) {
                    $("#guidlines-empty").hide();
                    $("#guidlines").show();

                    $('.empguid_block ul').empty();
                    response.forEach(function (policy) {

                        const listItem = `
                        <li>
                            <a data-document-path="${policy.document_path}" id="previews">
                                <div>${policy.policy_title}</div>
                                <i class="fa-regular fa-eye" aria-hidden="true"></i>
                            </a>

                        </li>`;

                        $('.empguid_block ul').append(listItem);
                    });
                } else {


                    $("#guidlines-empty").show();
                    $("#guidlines").hide();
                }
            },
            error: function () {
                console.log("Error while fetching policy data.");
            }
        });

    }



    $("#closeModal").click(function () {

        $('#policy_modal').modal('hide');

        // Clear the iframe source to stop loading
        document.getElementById('pdfViewer').src = '';

    })

    document.querySelector('.empguid_block').addEventListener('click', function (event) {

        if (event.target.closest('#previews')) {
            const documentPath = event.target.closest('#previews').getAttribute('data-document-path');
            var documentURL = '/UploadEmployeePolicies/' + CompanyCode + '/' + documentPath;

            $('#policy_modal').modal('show'); // Show the modal
            document.getElementById('pdfViewer').src = documentURL; // Set the PDF source
        }
    });

    //get data for webpunch mode
    function getpunchmode(empid) {
        $.ajax({
            type: "GET",
            async: false,
            url: "/ESS/GetPunchMode",
            data: { id: empid },
            //data: { id: id, IsAutoApprove: IsAutoApprove },
            contentType: "application/json; charset=utf-8",
            dataType: "text",
            success: function (data) {

                if (data.isRedirect) {
                    window.location.href = data.redirectUrl;
                }

                if (data == "IN") {
                    //$("#btnCheckIn").removeAttr('disabled');
                    //$("#btnCheckOut").attr('disabled', 'disabled');
                    $("#btnCheckOut").css("display", "none");
                    $("#btnCheckIn").css("display", "block");
                }
                if (data == "OUT") {
                    //$("#btnCheckIn").attr('disabled', 'disabled');
                    //$("#btnCheckOut").removeAttr('disabled');
                    $("#btnCheckOut").css("display", "block");
                    $("#btnCheckIn").css("display", "none");
                }
            },
            error: function (data) {
                window.location.href = data;
            }
        });
    }
    $("#btnCheckIn").click(function () {

        count++;
        operation = "Check IN";
        mode = "IN";
        var now = new Date(),
            now = now.getHours() + ':' + now.getMinutes() + ':' + now.getSeconds();
        $("#lblInTime").val(now);
        checktime = $("#lblInTime").val();
        Gcode();
        checkSession();
    });
    $("#btnCheckOut").click(function () {

        count++;
        operation = "Check OUT";
        mode = "OUT";
        var now = new Date(),
            now = now.getHours() + ':' + now.getMinutes() + ':' + now.getSeconds();
        $("#lblOutTime").val(now);
        checktime = $("#lblOutTime").val();
        //$("#Reason").css("border", "1px solid #c2cad8");
        //if ($("#Reason").val() == "") {
        //    $("#Reason").css("border", "1px solid red");
        //    return false;
        //}
        //getLocation();
        //save();
        Gcode();
        checkSession();
    });
    function Gcode() {

        var _lat = $("#lat").val();
        var _lag = $("#lag").val();


        var LatLng = new google.maps.LatLng(_lat, _lag);

        var geocoder = new google.maps.Geocoder;
        geocoder.geocode({ 'location': LatLng }, function (results, status) {
            if (status === 'OK') {
                if (results[0]) {
                    $("#latlagAddress").val(results[0].formatted_address);
                    savewebpunch();
                } else {
                    window.alert('No results found');
                }
            } else {
                //window.alert('Geocoder failed due to: ' + status);
                var apikey = 'AIzaSyDAtmmQTRUSi5Yc5X_09t-HW6fBbQaUjMQ';
                var query = 'https://maps.googleapis.com/maps/api/geocode/json?address=' + _lat + ',' + _lag + '&key=' + apikey;
                $.getJSON(query, function (data) {
                    if (data.status === 'OK') {
                        //var geo_data = data.results[0];
                        $("#latlagAddress").val(data.results[0].formatted_address);
                        savewebpunch();
                    }
                    else {
                        savewebpunch();
                    }
                });


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
    function savewebpunch() {
        checkSession();
        $("#ajax_loader").show();
        var allVals = [];
        allVals.push(empid); //Empid
        var inoutmodes = [];
        inoutmodes.push(mode);

        var inouttimes = [];
        inouttimes.push(checktime);

        //var WebPunch = [];
        //WebPunch["In_Out_Time"] = "25:00";
        //WebPunch["Mode"] = "IN";
        //var ip=GetUserIP();
        var WebPunch = {
            "In_Out_Time": checktime,
            "Mode": mode,
            "Attn_dt": checktime,
            "IPaddress": "",
            "DeviceId": 0,
            "EntryMode": 2,
            "Punchid": 1,
            "Empid": empid,
            "Location": $("#latlag").val(),
            "LocAddress": $("#latlagAddress").val(),
            "Reason": $("#Reason").val(),
            "IsGeoPunch": 0

        }
        //var notificationParams =
        //{
        //    EmpId: empid,
        //    InPunchTime: checktime,
        //    OutPunchTime: checktime,
        //    ApplyReason: $("#Reason").val(),
        //    EventId: 9,
        //    CorectionType: mode,
        //    EmployeeName: $("#empname").val()
        //};
        $.ajax(
            {
                type: "POST",
                url: "/ESS/WebPunch?WebPunch=" + JSON.stringify(WebPunch),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function (xhr) {
                    $('.loading').show();
                },
                success: function (response) {

                    if (response.isRedirect) {
                        window.location.href = response.redirectUrl;
                    }
                    $('.loading').hide();
                    if (response != null) {
                        if (String(response).toLowerCase() === "ok")
                        {                           

                            $("#ajax_loaders").hide();
                            if (mode == "IN") {
                                //  swal({ title: "Thank you.!", text: "You Have Successfully Check In", type: "success", showConfirmButton: false, timer: 3000 });
                                // $('.showSweetAlert').addClass('sa_webpunch');

                                Swal.fire({
                                    html: '<div class="swal_subtitle"><b>Thank you.!</b><p>You Have Successfully Check In</p></div>',
                                    title: "<img class='margin-right-5' src='/assets/images/icons/swal_icon/icon_success.svg' alt=''>Success",
                                    closeOnConfirm: true,
                                    showCloseButton: true,
                                    showCancelButton: false,
                                    focusConfirm: false,
                                    cancelButtonText: 'No',
                                    confirmButtonText: 'Done',
                                    allowOutsideClick: false,
                                    allowEscapeKey: false,
                                    customClass: {
                                        confirmButton: 'swal-confirm-button-class',
                                        cancelButton: 'swal-cancel-button-class'
                                    }
                                });
                                $('img[src="/assets/images/icons/swal_icon/icon_success.svg"]').parent().parent().parent().addClass('swal_success');


                                $("#btnCheckOut").css("display", "block");
                                $("#btnCheckIn").css("display", "none");
                            }
                            if (mode == "OUT") {
                                //swal({ title: "Thank you.!", text: "You Have Successfully Check Out", type: "success", showConfirmButton: false, timer: 3000 });
                                //$('.showSweetAlert').addClass('sa_webpunch');

                                Swal.fire({
                                    html: '<div class="swal_subtitle"><b>Thank you.!</b><p>You Have Successfully Check Out</p></div>',
                                    title: "<img class='margin-right-5' src='/assets/images/icons/swal_icon/icon_success.svg' alt=''>Success",
                                    closeOnConfirm: true,
                                    showCloseButton: true,
                                    showCancelButton: false,
                                    focusConfirm: false,
                                    cancelButtonText: 'No',
                                    confirmButtonText: 'Done',
                                    allowOutsideClick: false,
                                    allowEscapeKey: false,
                                    customClass: {
                                        confirmButton: 'swal-confirm-button-class',
                                        cancelButton: 'swal-cancel-button-class'
                                    }
                                });
                                $('img[src="/assets/images/icons/swal_icon/icon_success.svg"]').parent().parent().parent().addClass('swal_success');


                                $("#btnCheckOut").css("display", "none");
                                $("#btnCheckIn").css("display", "block");
                            }
                            GetPunchInformation(originalemployeeid);
                             //📤 Send notification in background (non-blocking)
                            //setTimeout(function () {
                                //sendWebPunchRequestNotification(notificationParams);
                            //}, 1000);

                        }
                        else {
                            $("#ajax_loaders").hide();
                            //swal({ title: response, text: "", type: "error", showConfirmButton: false, timer: 2000 });

                            Swal.fire({
                                html: '<div class="swal_subtitle"><b>' + response + '</b></div>',
                                title: "<img class='margin-right-5' src='/assets/images/icons/swal_icon/icon_error.svg' alt=''>Error",
                                closeOnConfirm: true,
                                showCloseButton: true,
                                showCancelButton: false,
                                focusConfirm: false,
                                cancelButtonText: 'No',
                                confirmButtonText: 'Done',
                                allowOutsideClick: false,
                                allowEscapeKey: false,
                                customClass: {
                                    confirmButton: 'swal-confirm-button-class',
                                    cancelButton: 'swal-cancel-button-class'
                                }
                            });
                            $('img[src="/assets/images/icons/swal_icon/icon_error.svg"]').parent().parent().parent().addClass('swal_error');


                        }
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $("#ajax_loader").hide();
                    // swal({ title: "", text: "Error on punching", type: "error", showConfirmButton: false, timer: 2000 });

                    Swal.fire({
                        html: '<div class="swal_subtitle"><b>Error on punching</b></div>',
                        title: "<img class='margin-right-5' src='/assets/images/icons/swal_icon/icon_error.svg' alt=''>Error",
                        closeOnConfirm: true,
                        showCloseButton: true,
                        showCancelButton: false,
                        focusConfirm: false,
                        cancelButtonText: 'No',
                        confirmButtonText: 'Done',
                        allowOutsideClick: false,
                        allowEscapeKey: false,
                        customClass: {
                            confirmButton: 'swal-confirm-button-class',
                            cancelButton: 'swal-cancel-button-class'
                        }
                    });
                    $('img[src="/assets/images/icons/swal_icon/icon_error.svg"]').parent().parent().parent().addClass('swal_error');

                }
            });

    }
    function getLocation() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(showPosition, showError);
        } else {
            x.innerHTML = "Geolocation is not supported by this browser.";
        }
    }
    function showError(error) {
        switch (error.code) {
            case error.PERMISSION_DENIED:
                //$("#btnCheckIn").attr('disabled', 'disabled');
                //$("#btnCheckOut").attr('disabled', 'disabled');
                showerrormsg("Geolocation is unavailable or user denied the request for geolocation.");
                break;
            case error.POSITION_UNAVAILABLE:
                //$("#btnCheckIn").attr('disabled', 'disabled');
                //$("#btnCheckOut").attr('disabled', 'disabled');
                showerrormsg("Location information is unavailable.");
                break;
            case error.TIMEOUT:
                //$("#btnCheckIn").attr('disabled', 'disabled');
                //$("#btnCheckOut").attr('disabled', 'disabled');
                showerrormsg("The request to get user location timed out.");
                break;
            case error.UNKNOWN_ERROR:
                //$("#btnCheckIn").attr('disabled', 'disabled');
                //$("#btnCheckOut").attr('disabled', 'disabled');
                showerrormsg("An unknown error occurred.");
                break;
        }
    }
    function showerrormsg(errormsg) {
        toastr.remove();
        toastr.warning(errormsg);
    }
    function showPosition(position) {

        var lat = position.coords.latitude;
        var long = position.coords.longitude;

        $("#lat").val(lat);
        $("#lag").val(long);
        var LatLng = new google.maps.LatLng(lat, long);
        $("#latlag").val(LatLng);
    }
    function showPosition1(position) {
        var lat = position.coords.latitude;
        var long = position.coords.longitude;

        //var city = position.coords.locality;
        var LatLng = new google.maps.LatLng(lat, long);
        $("#latlag").val(LatLng);

        var mapOptions = {
            center: LatLng,
            zoom: 15,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        var map = new google.maps.Map(document.getElementById("MyMapLOC"), mapOptions);
        var geocoder = new google.maps.Geocoder;
        var infowindow = new google.maps.InfoWindow;
        var address = '';
        geocoder.geocode({ 'location': LatLng }, function (results, status) {
            if (status === 'OK') {
                if (results[0]) {

                    var marker = new google.maps.Marker({
                        position: LatLng,
                        title: results[0].formatted_address
                    });
                    $("#latlagAddress").val(results[0].formatted_address);
                    marker.setMap(map);
                    var getInfoWindow = new google.maps.InfoWindow({
                        content: "<b>Your Current Location</b><br/> :" +
                            results[0].formatted_address + ""
                    });
                    getInfoWindow.open(map, marker);
                } else {
                    window.alert('No results found');
                }
            } else {
                window.alert('Geocoder failed due to: ' + status);
            }
        });

    }

    //attendence wizard----------------------------------------------------------------------------
    function CalendarFill(empid) {

        $('#MyCalendar').fullCalendar({
            displayEventTime: false,
            firstDay: 0,
            events: {
                url: webApiUrl + "DashBoard/GetEmpAttendanceStatusSummary",
                headers: { 'Authorization': token },
                backgroundColor: "red",
                cache: true,
                type: 'GET',
                data: {
                    empid: empid,
                    fromdate: FirstDayWithSlashes,
                    todate: lastDayWithSlashes,
                    statusflg: 3,
                    finalStatus: "",
                    searchName: $("#searchselection").val(),
                    searchBy: $("#SearchName").val()
                },
                error: function () {
                    alert('There was an error while fetching events!');
                },
            },

            dayClick: function (date, jsEvent, view) {

                var dateFormat = DateFormate;
                var clickedDate = date.format(dateFormat.toUpperCase());

                var clickedDateMoment = moment(clickedDate, dateFormat.toUpperCase());
                var currentDateMoment = moment().startOf('day');
                // Find events on the clicked date

                var events = $('#MyCalendar').fullCalendar('clientEvents', function (event) {
                    return event.start.format(dateFormat.toUpperCase()) === clickedDate;
                });

                // Remove 'bd_box' class from all elements

                $('.attetype_box').css({
                    'display': 'none', // Hide attetype_box initially
                });

                var hasWHEvent = events.some(function (event) {
                    return ["W", "H"].includes(event.title);
                });

                if (clickedDateMoment > currentDateMoment && !hasWHEvent) {
                    // If the clicked date is greater than the current date
                    SelectedDate = clickedDate;

                    $('#fromdate').val(SelectedDate);
                    $('#txtFromDate').val(SelectedDate);
                    $('#txtToDate').val(SelectedDate);
                    $('.date-picker').datepicker('setDate', SelectedDate);
                    popUpShow(jsEvent);
                    /*  clickedCell.addClass('bd_box');*/

                    // Show only the dot_leave option
                    $('.attetype_box').css({
                        'display': 'block', // Show attetype_box
                    });
                    $('.attetype_box .dot_leave').css({
                        'display': 'block',  // Show Leave Request option
                    });
                    $('.attetype_box .dot_correction').css({
                        'display': 'none',   // Show Attendance Correction option
                    });
                } else {
                    // Handle past dates
                    if (events.length > 0 && !["P", "W", "PW", "PH", "H", "HW"].includes(events[0].title)) {
                        SelectedDate = clickedDate;

                        $('#fromdate').val(SelectedDate);
                        $('#txtFromDate').val(SelectedDate);
                        $('#txtToDate').val(SelectedDate);
                        $('.date-picker').datepicker('setDate', SelectedDate);
                        popUpShow(jsEvent);
                        /* clickedCell.addClass('bd_box');*/

                        // Show full attetype_box
                        $('.attetype_box').css({
                            'display': 'block', // Show attetype_box
                        });
                        $('.attetype_box .dot_leave').css({
                            'display': 'block',  // Show Leave Request option
                        });
                        $('.attetype_box .dot_correction').css({
                            'display': 'block',   // Show Attendance Correction option
                        });
                    }
                }
            },


            eventRender: function (event, element) {
                element.find('.fc-event-time').hide();
                $(element).find(".fc-event-time").remove();
            },

            eventAfterAllRender: function () {
                // Remove all previously appended "more-dot" buttons
                $('.attendanceblock').remove();

                // Get the current view's visible range (start and end of the current view range)
                var view = $('#MyCalendar').fullCalendar('getView');
                var start = view.start.clone(); // Start of the current view (might be in the previous month)
                var end = view.end.clone();     // End of the current view

                // Get the current date for comparison
                var currentDate = moment().startOf('day'); // Today's date without the time part

                // Loop through each visible day in the calendar view
                while (start.isBefore(end)) {
                    var currentDayFormatted = start.format('YYYY-MM-DD');
                    var currentDayCell = $('.fc-day[data-date="' + currentDayFormatted + '"]');

                    // Check if the current day is greater than today
                    if (start.isAfter(currentDate)) {
                        var eventsOnDate = $('#MyCalendar').fullCalendar('clientEvents', function (event) {
                            return event.start.format('YYYY-MM-DD') === currentDayFormatted;
                        });

                        var hasWHEvent = eventsOnDate.some(function (event) {
                            return ["W", "H", "HW"].includes(event.title);
                        });

                        if (!hasWHEvent && currentDayCell.find('.attendanceblock').length === 0) {
                            currentDayCell.append('<div class="more-options attendanceblock" id="moreoptions"><button class="more-btn"><span class="more-dot"></span><span class="more-dot"></span><span class="more-dot"></span></button></div>');
                        }
                    }

                    start.add(1, 'day');
                }

                // Handle appending dots for events as usual
                $('#MyCalendar').fullCalendar('getView').calendar.clientEvents().forEach(function (event) {
                    var eventDate = event.start.format('YYYY-MM-DD');
                    var cell = $('.fc-day[data-date="' + eventDate + '"]');

                    // Append the dots only if the event title is not in the restricted list
                    if (!["H", "W", "P", "PW", "PH", "HW"].includes(event.title)) {
                        if (cell.find('.attendanceblock').length === 0) {
                            cell.append('<div class="more-options attendanceblock" id="moreoptions"><button class="more-btn"><span class="more-dot"></span><span class="more-dot"></span><span class="more-dot"></span></button></div>');
                        }
                    }
                });
            },
            eventMouseover: function (event) {
                var word = event.title;
                var FinalStatus = event.FinalStatus;
                var InTime = event.InTime;
                var OutTime = event.OutTime;
                var holidayName = event.holidayName;
                var tooltipword = "";
                if (InTime == null || InTime == "") {
                    InTime = '00:00:00';
                }

                if (OutTime == null || OutTime == "") {
                    OutTime = '00:00:00';
                }

                if (word == "A") {
                    tooltipword = "Absent " + FinalStatus;
                    if (FinalStatus == word) {
                        tooltipword = "Absent";
                    }
                }
                else if (word == "E") {
                    //tooltipword = "Error";
                    tooltipword = "Error " + FinalStatus + "<br/> InTime :" + InTime + "<br/> OutTime :" + OutTime;
                    if (FinalStatus == word) {
                        tooltipword = "Error <br/> InTime :" + InTime + "<br/> OutTime :" + OutTime;
                    }
                }
                else if (word == "W" || word == "WO" || word == "POW") {
                    tooltipword = "Week off" + FinalStatus;
                    if (FinalStatus == word || FinalStatus == "A") {
                        tooltipword = "Week off";
                    }

                }
                else if (word == "H") {

                    tooltipword = "Holiday" + holidayName;
                    if (FinalStatus == word || FinalStatus == "A") {
                        tooltipword = holidayName;
                    }
                }
                else if (word == "P") {
                    tooltipword = "Present " + FinalStatus + "<br/> InTime :" + InTime + "<br/> OutTime :" + OutTime;
                    if (FinalStatus == word) {
                        tooltipword = "Present <br/> InTime :" + InTime + "<br/> OutTime :" + OutTime;
                    }
                }
                else if (word == "PW") {
                    //tooltipword = "Present on Weekoff";
                    tooltipword = "Present on Weekoff  " + FinalStatus + "<br/> InTime :" + InTime + "<br/> OutTime :" + OutTime;
                    if (FinalStatus == word) {
                        tooltipword = "Present on Weekoff <br/> InTime :" + InTime + "<br/> OutTime :" + OutTime;
                    }
                }
                else if (word == "HD") {
                    //tooltipword = "Half Day";
                    //tooltipword = "Half Day " + FinalStatus + "<br/> InTime :" + InTime + "<br/> OutTime :" + OutTime;
                    tooltipword = FinalStatus + "<br/> InTime :" + InTime + "<br/> OutTime :" + OutTime;
                    if (FinalStatus == word) {
                        tooltipword = "Half Day <br/> InTime :" + InTime + "<br/> OutTime :" + OutTime;
                    }
                }
                else if (word == "PH") {
                    //tooltipword = "Present on Holiday";
                    tooltipword = "Present on Holiday  " + FinalStatus + "<br/> InTime :" + InTime + "<br/> OutTime :" + OutTime;
                    if (FinalStatus == word) {
                        tooltipword = "Present on Holiday <br/> InTime :" + InTime + "<br/> OutTime :" + OutTime;
                    }
                }
                else if (word == "HW") {
                    //tooltipword = "Present on Holiday";
                    tooltipword = "Holiday/Weekoff  " + FinalStatus + "<br/> InTime :" + InTime + "<br/> OutTime :" + OutTime;
                    if (FinalStatus == word) {
                        tooltipword = "Holiday/Weekoff <br/> InTime :" + InTime + "<br/> OutTime :" + OutTime;
                    }
                }
                else if (word == "LH") {
                    //tooltipword = "Less Hour";
                    tooltipword = "Less Hour  " + FinalStatus + "<br/> InTime :" + InTime + "<br/> OutTime :" + OutTime;
                    if (FinalStatus == word) {
                        tooltipword = "Less Hour <br/> InTime :" + InTime + "<br/> OutTime :" + OutTime;
                    }
                }
                else if (word == "CL" || word == "OD" || word == "SL" || word == "EL" || word == "SP" || word == "PL" || word == "ML" || word == "LC" || word == "TL" || word == "BL") {

                    tooltipword = "Leave " + FinalStatus;
                    if (FinalStatus == word) {
                        tooltipword = "Leave ";
                    }
                }
                else {
                    tooltipword = word + " " + FinalStatus;
                    if (FinalStatus == word) {
                        tooltipword = word;
                    }
                }

                var tooltip = '<div class="tooltipevent" >' + tooltipword + '</div>';
                var $tooltip = $(tooltip).appendTo('body');
                $(this).mouseover(function (e) {
                    $(this).css('z-index', 10000);
                    $tooltip.fadeIn('500');
                    $tooltip.fadeTo('10', 1.9);
                }).mousemove(function (e) {
                    $tooltip.css('top', e.pageY + 10);
                    $tooltip.css('left', e.pageX + 20);
                    $tooltip.css({
                        'top': e.pageY + -12,  // Slightly below the cursor
                        'left': e.pageX - $tooltip.outerWidth() - 20 // To the left of the cursor
                    });
                });

            },
            eventMouseout: function () {
                $(this).css('z-index', 8);
                $('.tooltipevent').remove();
            },

        });

        // Outside Click attetype_box and bd_box hide code
        $(document).on('click', function (e) {
            if (!$(e.target).closest('.fc-view-container').length) {
                $('.attetype_box').css('display', 'none');
                /*     $('.fc-day').removeClass("bd_box");*/
            }
        });

    }
    function CalendarListFill(FirstDayWithSlashes, lastDayWithSlashes, empid) {

        checkSession();
        $('#listviewtbl tbody').html('');
        $.ajax({
            type: 'GET',
            url: webApiUrl + "DashBoard/GetEmpAttendanceStatusSummary",
            headers: { 'Authorization': token },
            data: {
                empid: empid,
                fromdate: FirstDayWithSlashes,
                todate: lastDayWithSlashes,
                statusflg: 3,
                finalStatus: "",
                searchName: $("#searchselection").val(),
                searchBy: $("#SearchName").val()
            },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                $('#listviewtbl tbody').empty();
                var tr;
                for (var i = 0; i < data.length; i++) {
                    var inTime = data[i].InTime ? data[i].InTime : '00:00';
                    var outTime = data[i].OutTime ? data[i].OutTime : '00:00';
                    var hours = '00';
                    var minutes = '00';
                    var seconds = '00';

                    // Check if InTime and OutTime are valid (not '00:00:00')
                    if (inTime !== '00:00' && outTime !== '00:00') {
                        var inTimeParts = inTime.split(':');
                        var outTimeParts = outTime.split(':');
                        var inMinutes = parseInt(inTimeParts[0]) * 60 + parseInt(inTimeParts[1]);
                        var outMinutes = parseInt(outTimeParts[0]) * 60 + parseInt(outTimeParts[1]);

                        // Handle overnight shift
                        if (outMinutes < inMinutes) {
                            outMinutes += 24 * 60;
                        }

                        var workingMinutes = outMinutes - inMinutes;

                        hours = Math.floor(workingMinutes / 60);
                        minutes = workingMinutes % 60;

                        hours = String(hours).padStart(2, '0');
                        minutes = String(minutes).padStart(2, '0');
                    }

                    // Create new row
                    tr = $('<tr/>');
                    tr.append("<td>" + data[i].start + "</td>");
                    tr.append("<td>" + inTime.substring(0, 8) + "</td>"); // Show in HH:MM:SS format
                    tr.append("<td>" + outTime.substring(0, 8) + "</td>"); // Show in HH:MM:SS format
                    tr.append("<td>" + hours + ':' + minutes + "</td>"); // Show working hours in HH:MM:SS format
                    //tr.append("<td style='color:" + data[i].color + "'>" + data[i].title  + "</td>"); // Set the color for title
                    tr.append("<td style='color:" + data[i].Subcolor + "'>" + data[i].FinalStatus + "</td>"); // Set the color for title

                    $('#listviewtbl').append(tr);
                }
            },
            error: function () {
                // Handle error cases
            }
        });



    }
    $('.fc-prev-button').click(function () {
        $('#MyCalendar').fullCalendar('removeEvents');
        var b = $('#MyCalendar').fullCalendar('getDate');
        var FirstDayoftheMonth = b.format("YYYY-MM-DD");

        var addmonth = "";
        var StartDate = new Date(b.format("YYYY-MM-DD"));
        var EndDate = new Date(StartDate.getFullYear(), StartDate.getMonth() + 1, 0);
        if (EndDate.getMonth() < 10 && EndDate.getMonth() != 9) {
            addmonth = "0" + (EndDate.getMonth() + 1);
        }
        else {
            addmonth = (EndDate.getMonth() + 1);
        }
        var lastDayoftheMonth = EndDate.getFullYear() + "-" + addmonth + "-" + EndDate.getDate();
        //if ($("#employeeId").val() != '0')
        //{
        //    ids = $("#employeeId").val();
        //}
        CalendarListFill(FirstDayoftheMonth, lastDayoftheMonth, empid)
        $.ajax({
            type: "GET",
            url: webApiUrl + "DashBoard/GetEmpAttendanceStatusSummary",
            headers: { 'Authorization': token },
            data: {
                empid: empid,
                fromdate: FirstDayoftheMonth,
                todate: lastDayoftheMonth,
                statusflg: 3,
                finalStatus: "",
                searchName: $("#searchselection").val(),
                searchBy: $("#SearchName").val()
            },
            success: function (events) {
                $('#MyCalendar').fullCalendar('removeEvents');
                $('#MyCalendar').fullCalendar('addEventSource', events);
                $('#MyCalendar').fullCalendar('rerenderEvents');
            },
            error: function () {
                toastr.error("Error while fetching events!");
            }
        });
    });
    $('.fc-next-button').click(function () {
        $('#MyCalendar').fullCalendar('removeEvents');
        var b = $('#MyCalendar').fullCalendar('getDate');
        var FirstDayoftheMonth = b.format("YYYY-MM-DD");
        var addmonth = "";
        var StartDate = new Date(b.format("YYYY-MM-DD"));
        var EndDate = new Date(StartDate.getFullYear(), StartDate.getMonth() + 1, 0);
        if (EndDate.getMonth() < 10 && EndDate.getMonth() != 9) {
            addmonth = "0" + (EndDate.getMonth() + 1);
        }
        else {
            addmonth = (EndDate.getMonth() + 1);
        }
        var lastDayoftheMonth = EndDate.getFullYear() + "-" + addmonth + "-" + EndDate.getDate();
        //if ($("#employeeId").val() != '0') {
        //    ids = $("#employeeId").val();
        //}
        CalendarListFill(FirstDayoftheMonth, lastDayoftheMonth, empid)
        $.ajax({
            type: "GET",
            url: webApiUrl + "DashBoard/GetEmpAttendanceStatusSummary",
            headers: { 'Authorization': token },
            data: {
                empid: empid,
                fromdate: FirstDayoftheMonth,
                todate: lastDayoftheMonth,
                statusflg: 3,
                finalStatus: "",
                searchName: $("#searchselection").val(),
                searchBy: $("#SearchName").val()
            },
            success: function (events) {
                $('#MyCalendar').fullCalendar('removeEvents');
                $('#MyCalendar').fullCalendar('addEventSource', events);
                $('#MyCalendar').fullCalendar('rerenderEvents');
            },
            error: function () {
                toastr.error("Error while fetching Attendance Summary!");
            }
        });
    });
    // Click event for the Tableview / Listview button
    $('#listview_btn').click(function () {
        $('#tableview').hide();
        $('#listview').show();

        $('#tableview_btn').removeClass('active');
        $(this).addClass('active');
    });
    $('#tableview_btn').click(function () {
        $('#listview').hide();
        getcelenderData(empid);
        $('#tableview').show();

        $('#listview_btn').removeClass('active');
        $(this).addClass('active');
    });

    // Click event for the Modal correction/Leave


    function popUpShow(jsEvent) {
        // Get the click position relative to the body
        var clickX = jsEvent.pageX;
        var clickY = jsEvent.pageY;

        // Get the offset and dimensions of the #MyCalendar container
        var calendarOffset = $('#MyCalendar').offset();
        var calendarHeight = $('#MyCalendar').outerHeight();
        var calendarWidth = $('#MyCalendar').outerWidth();

        // Calculate the position relative to #MyCalendar
        var relativeX = clickX - calendarOffset.left;
        var relativeY = clickY - calendarOffset.top;

        // Get the dimensions of .attetype_box
        var attetype_boxHeight = $('.attetype_box').outerHeight();
        var attetype_boxWidth = $('.attetype_box').outerWidth();

        // Calculate available space
        var spaceBelow = calendarHeight - relativeY;
        var spaceOnRight = calendarWidth - relativeX;

        // Reset arrow classes
        $('.attetype_box').removeClass('arrow-bottom arrow-right');

        // Determine the arrow direction and position
        if (spaceBelow < attetype_boxHeight) {
            relativeY = relativeY - attetype_boxHeight - 35; // Position above
            $('.attetype_box').addClass('arrow-bottom');
        }

        // Adjust position if there's not enough space on the right
        if (spaceOnRight < attetype_boxWidth) {
            relativeX = relativeX - attetype_boxWidth - -50; // Adjust to the left
            $('.attetype_box').addClass('arrow-right');
        }

        // Show the .attetype_box element relative to #MyCalendar
        $('.attetype_box').css({
            'position': 'absolute',
            'top': (relativeY + 20) + 'px',
            'left': relativeX + 'px',
            'display': 'block', // Make it visible
        });

        //$('.attetype_box').find('attetype_box::before, attetype_box::after').css({
        //    'left': (relativeX + 6) + 'px' // Adjust to align with click position
        //});
    }
    function Clearatadd() {
        $("#ddlLeaveType").val("0");
        $("#ddlLeavePaid").val("0");
        $("#OnDutyLeaveId").val("0");

        $("#txtReason").val("");
        $("#chkIsHalfLeave").prop('checked', false);
        $(".alert-danger").hide();
        $(".alert-success").hide();
        $(".alert-danger").text("");
        $(".alert-success").text("");
        $("#lblLeaveBalance").text("0");
        $("#lblEncashLeave").text("0");
        $("#lblTotalBalance").text("0");
        $("#hfAvailBal").val(0);
        $("#hfEmpCode").val("0");
        $("#chkIsHalfLeave").prop('disabled', true);
        $("#uploadDoc").hide();
        $('#UploadLeaveDoc').val('');
        $("#leaveTypeDurationBlock").hide();
    }

    function save() {
        
        checkSession();
        $(".loading").show();
        if (DateFormate == "dd-mm-yyyy") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[2] + "-" + toDate[1] + "-" + toDate[0];
        }
        else if (DateFormate == "mm-dd-yyyy") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[2] + "-" + toDate[0] + "-" + toDate[1];
        }
        else if (DateFormate == "yyyy-mm-dd") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[0] + "-" + fromDate[1] + "-" + fromDate[2];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[0] + "-" + toDate[1] + "-" + toDate[2];
        }
        else if (DateFormate == "yyyy-M-dd") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[0] + "-" + fromDate[1] + "-" + fromDate[2];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[0] + "-" + toDate[1] + "-" + toDate[2];
        }
        else if (DateFormate == "M-dd-yyyy") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[2] + "-" + toDate[0] + "-" + toDate[1];
        }
        else if (DateFormate == "dd-M-yyyy") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[2] + "-" + toDate[1] + "-" + toDate[0];
        }

        if (valid()) {
            var IsHalfLeave = 0;
            var ddlLeaveDuration = 0;
            if ($('#chkIsHalfLeave').is(':checked') == true) {
                IsHalfLeave = true;
            }
            else {
                IsHalfLeave = false;
            }
            var Empcodee = 0;
            if ($("#OnDutyLeaveId").val() == "0") {
                Empcodee = parseInt(empid);
            }
            else {
                Empcodee = parseInt($("#hfEmpCode").val());
            }
            var days;
            var start1 = _Fromdate;
            var end1 = _toDate;

            if (IsHalfLeave == 1) {
                var diff = Date.parse(end1) - Date.parse(start1);
                days = diff / 1000 / 60 / 60 / 24;
                days = (days + 1) / 2;
                ddlLeaveDuration = parseInt($("#ddlLeaveDuration").val());
            }
            else {
                var diff = Date.parse(end1) - Date.parse(start1);
                days = diff / 1000 / 60 / 60 / 24;
                days = days + 1;
            }

            var LeaveDoc = $("#UploadLeaveDoc").text();
            var dt = new Date();
            var EmpIdToPass = (employeeids == 0 || employeeids == empid) ? empid : employeeids;
            var OnDutyLeave =
            {
                "EmpId": EmpIdToPass == 0 ? empid : EmpIdToPass,
                "LeaveTypeId": parseInt($("#ddlLeaveType").val()),
                "FromDate": _Fromdate,
                "ToDate": _toDate,
                "ApplyReason": $("#txtReason").val(),
                "LeaveStatus": 3,
                "IsHalfLeave": IsHalfLeave,
                "LeavePaid": parseInt($("#IsPaidFlag").val()),
                "CreatedDate": dt,
                "CreatedBy": empid, //Empcodee
                "LeaveDoc": LeaveDoc,
                "Location": $("#latlag").val(),
                "LocAddress": $("#latlagAddress").val(),
                "LeaveTypeDuration": parseInt(ddlLeaveDuration)
            };            
            var notificationParams =
            {
                EmpId: EmpIdToPass,
                InPunchTime: _Fromdate,
                OutPunchTime: _toDate,
                ApplyReason: $("#txtReason").val(),
                LeaveTypeName: $("#ddlLeaveType option:selected").text(),
                EmployeeName: $("#empname").val(),
                EventId: 1,
                LeavePaid: parseInt($("#IsPaidFlag").val()),
                LeaveTypeId: parseInt($("#ddlLeaveType").val()),
                IsHalfLeave: IsHalfLeave
            };

            var LeaveType = $("#ddlLeavePaid").val();
            if (LeaveType == 0) {
                $(".loading").hide();
                Swal.fire({
                    title: `<img class='margin-right-5' src='/assets/images/icons/swal_icon/icon_info.svg' alt=''> Unpaid Leave`,
                    html: `
                            <div class="swal_subtitle">
                                <b>This is an UNPAID leave.</b>
                                <p>Are you sure you want to continue?</p>
                            </div>
                        `,
                    showCloseButton: true,
                    showCancelButton: true,
                    focusConfirm: false,
                    cancelButtonText: 'No, Cancel',
                    confirmButtonText: 'Yes, Continue',
                    allowOutsideClick: false,
                    allowEscapeKey: false,
                    customClass: {
                        confirmButton: 'swal-confirm-button-class',
                        cancelButton: 'swal-cancel-button-class'
                    }
                }).then((result) => {
                    if (result.isConfirmed) {
                        LeaveAdd(OnDutyLeave, notificationParams);
                    } else {
                        return false;
                    }
                });
                $('img[src="/assets/images/icons/swal_icon/icon_info.svg"]')
                    .parent().parent().parent()
                    .addClass('swal_info');
                return;
            }
            LeaveAdd(OnDutyLeave, notificationParams);
        }

    }

    function LeaveAdd(OnDutyLeave, notificationParams) {
        $.ajax({
            type: "POST",
            url: "/ESS/LeaveCreate?OnDutyLeave=" + JSON.stringify(OnDutyLeave),
            contentType: "application/json",
            success: function (response) {

                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                }
                //if (response.startsWith('\r\n\r')) {
                //    window.location = "/PayTime/Index";
                //}
                if (response != null) {

                    var message = response.message;
                    var leaveId = response.leaveId;
                    var _Meg = response.Meg;

                    if (message === "ok") {
                        $('#dot_leave_modal').modal('hide');
                        $('#frmpopup').modal('toggle');
                        toastr.remove();
                        toastr.success("Leave added successfully.");
                        $("#leaveTypeDurationBlock").hide();
                        var uploaddoc = $("#UploadLeaveDoc").val();
                        if (uploaddoc != null && uploaddoc !== "") {
                            window.setTimeout(function () {
                                $("#frmupload").submit();
                            }, 1000);
                        } else {
                            setTimeout(function () {
                                Clear();

                            }, 2000);
                        }
                        // ✅ Add leaveId into notificationParams here
                        if (leaveId != 0) {
                            notificationParams.LeaveCorrectionID = leaveId;
                        }
                        // 📤 Send notification in background (non-blocking)
                        setTimeout(function () {
                            sendLeaveRequestNotification(notificationParams);
                        }, 1000);
                    }
                    else {
                        $('#dot_leave_modal').modal('hide');
                        toastr.remove();
                        toastr.error(_Meg);
                    }
                }
                /* location.reload();*/
                Getpendingleave_count();
                MyLeave(empid);
                Filltblleavebalance(empid);
            },
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            error: function (jqXHR, ajaxOptions, thrownError) {
                $(".loading").hide();

                if (jqXHR.status == 500) { //InternalServerError = 500,
                    toastr.error("InternalServerError");
                } else if (jqXHR.status == 400) { //BadRequest = 400,
                    toastr.error("BadRequest");
                } else if (jqXHR.status == 401) {
                    window.location.href = "/PayTime/LoginPage";
                } else {
                    toastr.error("An error occurred while processing your request.");
                }
            }
        });
    }
    function frmClear() {
        $("#AttCorrectionId").val(0);

        $("#ApplyReason").val('');
    }
    function valid() {
        var Employee = empid;
        var Leave = $("#ddlLeaveType").val();
        var FromDate = $("#txtFromDate").val();
        var ToDate = $("#txtToDate").val();
        var Reason = $("#txtReason").val();
        var IsHalfLeave = 0;
        if ($('#chkIsHalfLeave').is(':checked') == true) {
            IsHalfLeave = true;
        }
        else {
            IsHalfLeave = false;
        }
        var days;
        if (DateFormate == "dd-mm-yyyy") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[2] + "-" + toDate[1] + "-" + toDate[0];
        }
        else if (DateFormate == "mm-dd-yyyy") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[2] + "-" + toDate[0] + "-" + toDate[1];
        }
        else if (DateFormate == "yyyy-mm-dd") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[0] + "-" + fromDate[1] + "-" + fromDate[2];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[0] + "-" + toDate[1] + "-" + toDate[2];
        }
        else if (DateFormate == "yyyy-M-dd") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[0] + "-" + fromDate[1] + "-" + fromDate[2];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[0] + "-" + toDate[1] + "-" + toDate[2];
        }
        else if (DateFormate == "M-dd-yyyy") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[2] + "-" + toDate[0] + "-" + toDate[1];
        }
        else if (DateFormate == "dd-M-yyyy") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[2] + "-" + toDate[1] + "-" + toDate[0];
        }
        var start1 = _Fromdate;
        var end1 = _toDate;
        if ($("#hfAvailBal").val().indexOf("-") != -1) {
        }
        else {

            if (IsHalfLeave == 1) {
                var diff = Date.parse(end1) - Date.parse(start1);
                days = diff / 1000 / 60 / 60 / 24;
                days = (days + 1) / 2;
            }
            else {
                var diff = Date.parse(end1) - Date.parse(start1);
                days = diff / 1000 / 60 / 60 / 24;
                days = days + 1;
            }

            if ($("#hfAvailBal").val() != 0) {
                var _dydif = parseFloat($("#hfAvailBal").val()) >= parseFloat(days);
                if (_dydif != "1") {
                    toastr.remove();
                    toastr.error("You have not enough leave balanace..!!");
                    $(".loading").hide();
                    return false;
                }
            }
        }
        if ($("#OnDutyLeaveId").val() == "0") {
            if (Employee == "0") {
                toastr.remove();
                toastr.error("Please select employee.");
                $(".loading").hide();
                return false;
            }
        }

        if (end1 < start1) {
            toastr.remove();
            toastr.error("To date should be greater than from date.");
            $(".loading").hide();
            return false;
        }

        if (Leave == "0") {
            $("#ddlLeaveType").addClass('field_error');
            $(".loading").hide();
            return false;
        }
        if (FromDate.trim() == "") {
            $("#txtFromDate").addClass('field_error');
            $(".loading").hide();
            return false;
        }
        else if ($("#txtFromDate").val() != "" && locakattendeceday != "" && (planid == '6' || planid == '5' || planid == '3')) {
            var _data = $("#txtFromDate").val();
            var date = _data.split('-');
            if (DateFormate == "dd-mm-yyyy") {
                var firstValue = date[2] + "-" + date[1] + "-" + date[0] //yyyy-mm-dd
            }
            else if (DateFormate == "yyyy-mm-dd") {
                var firstValue = date[0] + "-" + date[1] + "-" + date[2] //yyyy-mm-dd
            }
            else if (DateFormate == "mm-dd-yyyy") {
                var firstValue = date[2] + "-" + date[1] + "-" + date[0] //yyyy-mm-dd
            }
            else if (DateFormate == "yyyy-M-dd") {
                var _date = new Date(date[2] + " " + date[1] + " " + date[0]);
                var locale = "en-us";
                var month = _date.toLocaleString(locale, { month: "numeric" });
                var firstValue = date[0] + "-" + month + "-" + date[2] //yyyy-mm-dd
            }
            else if (DateFormate == "M-dd-yyyy") {
                var _date = new Date(date[1] + " " + date[0] + " " + date[2]);
                var locale = "en-us";
                var month = _date.toLocaleString(locale, { month: "numeric" });
                var firstValue = date[2] + "-" + month + "-" + date[1] //yyyy-mm-dd
            }
            else if (DateFormate == "dd-M-yyyy") {
                var _date = new Date(date[0] + " " + date[1] + " " + date[2]);
                var locale = "en-us";
                var month = _date.toLocaleString(locale, { month: "numeric" });
                var firstValue = date[2] + "-" + month + "-" + date[0] //yyyy-mm-dd
            }
            firstValue = firstValue.split('-');
            var s_date = new Date();
            var secondValue = (s_date.getFullYear() + "-" + (s_date.getMonth() + 1) + "-" + locakattendeceday).split('-');
            var firstDate = new Date();
            firstDate.setFullYear(firstValue[0], (firstValue[1] - 1), firstValue[2]);
            var secondDate = new Date();
            secondDate.setFullYear(secondValue[0], (secondValue[1] - 1), 1);
            var secondDate_pev_month = new Date();
            secondDate_pev_month.setFullYear(secondValue[0], (secondValue[1] - 2), 1);
            var Current_date = new Date();
            var Lock_date_day = new Date();
            Lock_date_day.setFullYear(secondValue[0], (secondValue[1] - 1), locakattendeceday);
            if (firstDate < secondDate) {
                if (firstDate >= secondDate_pev_month && Current_date <= Lock_date_day) {

                } else {
                    toastr.remove();
                    toastr.error("Please change the date because your attendance is locked on this date");
                    event.preventDefault();
                    $("#txtFromDate").addClass('field_error');
                    $(".loading").hide();
                    return false;
                }
            }
        }
        if (ToDate.trim() == "") {
            $("#txtToDate").addClass('field_error');
            $(".loading").hide();
            return false;
        }
        if (Reason.trim() == "") {
            $("#txtReason").addClass('field_error');
            $(".loading").hide();
            return false;
        }
        else {
            $(".popMsg").hide();
        }
        if ($("#uploadDoc").is(":visible") && $("#UploadLeaveDoc").val() == "") {
            toastr.remove();
            toastr.error("Please upload document.");
            $(".loading").hide();
            return false;
        }
        return true;
    }
    $("body").on('click', '.dot_leave', function () {

        count++;
        operation = "Add Form";
        Clearatadd();
        /*      $("#leaveDatePicker").prop('disabled', true);*/
        $('#leaveDatePicker input').attr('disabled', true);  // Disable all input fields inside the div
        $('#leaveDatePicker button').attr('disabled', true);
        $(".dot_leave").attr("data-toggle", "modal");
        $(".dot_leave").attr("data-target", "#dot_leave_modal");

    });
    $("body").on('click', '.dot_correction', function () {

        frmClear();
        /* $("#AttendanceDatePicker").prop('disabled', true);*/
        $('#AttendanceDatePicker input').attr('disabled', true);  // Disable all input fields inside the div
        $('#AttendanceDatePicker button').attr('disabled', true); // Disable the button inside the div

        // Optionally, add a 'disabled' class to visually indicate the entire div is disabled

        $('#InPunchTime').timepicker('setTime', currentFormattedTime);
        $('#OutPunchTime').timepicker('setTime', currentFormattedTime);
        $(".dot_correction").attr("data-toggle", "modal");
        $(".dot_correction").attr("data-target", "#dot_correction_modal");

    }); 
    $("#btnsaveleave").click(function () { save(); });
    $("#btnSave").click(function () {
        checkSession();
        $(".loading").show();
        if ($("#fromdate").val() == "") {
            $("#fromdate").addClass('field_error');
            $("#fromdate").focus();
            event.preventDefault();
            $(".loading").hide();
            return false;
        }
        if (empid == 0) {
            toastr.remove();
            toastr.error("Employee not found.");
            event.preventDefault();
            $(".loading").hide();
            return false;
        }

        else if ($("#fromdate").val() != "" && locakattendeceday != "" && (planid == '6' || planid == '5' || planid == '3')) {
            var _data = $("#fromdate").val();
            var date = _data.split('-');
            if (DateFormate == "dd-mm-yyyy") {
                var firstValue = date[2] + "-" + date[1] + "-" + date[0] //yyyy-mm-dd
            }
            else if (DateFormate == "yyyy-mm-dd") {
                var firstValue = date[0] + "-" + date[1] + "-" + date[2] //yyyy-mm-dd
            }
            else if (DateFormate == "mm-dd-yyyy") {
                var firstValue = date[2] + "-" + date[1] + "-" + date[0] //yyyy-mm-dd
            }
            else if (DateFormate == "yyyy-M-dd") {
                var _date = new Date(date[2] + " " + date[1] + " " + date[0]);
                var locale = "en-us";
                var month = _date.toLocaleString(locale, { month: "numeric" });
                var firstValue = date[0] + "-" + month + "-" + date[2] //yyyy-mm-dd
            }
            else if (DateFormate == "M-dd-yyyy") {
                var _date = new Date(date[1] + " " + date[0] + " " + date[2]);
                var locale = "en-us";
                var month = _date.toLocaleString(locale, { month: "numeric" });
                var firstValue = date[2] + "-" + month + "-" + date[1] //yyyy-mm-dd
            }
            else if (DateFormate == "dd-M-yyyy") {
                var _date = new Date(date[0] + " " + date[1] + " " + date[2]);
                var locale = "en-us";
                var month = _date.toLocaleString(locale, { month: "numeric" });
                var firstValue = date[2] + "-" + month + "-" + date[0] //yyyy-mm-dd
            }
            firstValue = firstValue.split('-');
            var s_date = new Date();
            var secondValue = (s_date.getFullYear() + "-" + (s_date.getMonth() + 1) + "-" + locakattendeceday).split('-');
            var firstDate = new Date();
            firstDate.setFullYear(firstValue[0], (firstValue[1] - 1), firstValue[2]);
            var secondDate = new Date();
            secondDate.setFullYear(secondValue[0], (secondValue[1] - 1), 1);
            var secondDate_pev_month = new Date();
            secondDate_pev_month.setFullYear(secondValue[0], (secondValue[1] - 2), 1);
            var Current_date = new Date();
            var Lock_date_day = new Date();
            Lock_date_day.setFullYear(secondValue[0], (secondValue[1] - 1), locakattendeceday);
            if (firstDate < secondDate) {
                if (firstDate >= secondDate_pev_month && Current_date <= Lock_date_day) {
                    $("#fromdate").removeClass('field_error');
                } else {
                    toastr.remove();
                    toastr.error("Please change the date because your attendance is locked on this date.");
                    event.preventDefault();
                    $("#fromdate").addClass('field_error');
                    $(".loading").hide();
                    return false;
                }
            }
        } else {
            $("#fromdate").removeClass('field_error');
        }
        if ($("#InPunchTime").val() == "") {
            $("#InPunchTime").addClass('field_error');
            $("#InPunchTime").focus();
            event.preventDefault();
            $(".loading").hide();
            return false;
        }
        else {
            $("#InPunchTime").removeClass('field_error');
        }
        if ($("#OutPunchTime").val() == "") {
            $("#OutPunchTime").addClass('field_error');
            $("#OutPunchTime").focus();
            event.preventDefault();
            $(".loading").hide();
            return false;
        }
        else {
            $("#OutPunchTime").removeClass('field_error');
        }
        if (DateFormate == "dd-mm-yyyy") {
            var _inpunchdate = $("#fromdate").val().split("-");
            var inpunchdates = _inpunchdate[2] + "-" + _inpunchdate[1] + "-" + _inpunchdate[0];
            var Inpunchdate = inpunchdates + " " + $("#InPunchTime").val();
            var Outpunchdate = inpunchdates + " " + $("#OutPunchTime").val();
        }
        else if (DateFormate == "mm-dd-yyyy") {
            var _inpunchdate = $("#fromdate").val().split("-");
            var inpunchdates = _inpunchdate[2] + "-" + _inpunchdate[0] + "-" + _inpunchdate[1];
            var Inpunchdate = inpunchdates + " " + $("#InPunchTime").val();
            var Outpunchdate = inpunchdates + " " + $("#OutPunchTime").val();
        }
        else if (DateFormate == "yyyy-mm-dd") {
            var _inpunchdate = $("#fromdate").val().split("-");
            var inpunchdates = _inpunchdate[0] + "-" + _inpunchdate[1] + "-" + _inpunchdate[2];
            var Inpunchdate = inpunchdates + " " + $("#InPunchTime").val();
            var Outpunchdate = inpunchdates + " " + $("#OutPunchTime").val();
        }
        else if (DateFormate == "yyyy-M-dd") {
            var _inpunchdate = $("#fromdate").val().split("-");
            var inpunchdates = _inpunchdate[0] + "-" + _inpunchdate[1] + "-" + _inpunchdate[2];
            var Inpunchdate = inpunchdates + " " + $("#InPunchTime").val();
            var Outpunchdate = inpunchdates + " " + $("#OutPunchTime").val();
        }
        else if (DateFormate == "M-dd-yyyy") {
            var _inpunchdate = $("#fromdate").val().split("-");
            var inpunchdates = _inpunchdate[2] + "-" + _inpunchdate[0] + "-" + _inpunchdate[1];
            var Inpunchdate = inpunchdates + " " + $("#InPunchTime").val();
            var Outpunchdate = inpunchdates + " " + $("#OutPunchTime").val();
        }
        else if (DateFormate == "dd-M-yyyy") {
            var _inpunchdate = $("#fromdate").val().split("-");
            var inpunchdates = _inpunchdate[2] + "-" + _inpunchdate[1] + "-" + _inpunchdate[0];
            var Inpunchdate = inpunchdates + " " + $("#InPunchTime").val();
            var Outpunchdate = inpunchdates + " " + $("#OutPunchTime").val();
        }
        var EmpIdToPass = (employeeids == 0 || employeeids == empid) ? empid : employeeids;
        var CorrectionDetails =
        {
            "AttendanceId": $("#AttCorrectionId").val(),
            "EmpId": EmpIdToPass,
            "InPunchTime": Inpunchdate,
            "OutPunchTime": Outpunchdate,
            "ApplyReason": $("#ApplyReason").val(),
            "Status": 3
        }
        var notificationParams =
        {
            EmpId: EmpIdToPass,
            InPunchTime: Inpunchdate,
            OutPunchTime: Outpunchdate,
            ApplyReason: $("#ApplyReason").val(),
            EventId: 5,
            CorectionType: 'Miss Punch',
            EmployeeName: $("#empname").val()
        };

        $.post("/ESS/AttendanceCorrectionAdd?CorrectionDetails=" + JSON.stringify(CorrectionDetails), function (response)
        {          
            //var response = JSON.parse(response);
            if (response != null)
            {                
                var message = response.message;
                var OnCorrectionID = response.OnCorrectionID;
                var _Meg = response.Meg;

                if (message == "ok") {
                    $('#dot_correction_modal').modal('hide');
                    toastr.remove();
                    toastr.success("Attendace correction added successfully.");
                    /*location.reload();*/
                    Getpendingleave_count();

                    if (OnCorrectionID != 0) {
                        notificationParams.AttCorrectionID = OnCorrectionID;
                    }
                    // 📤 Send notification in background (non-blocking)
                    setTimeout(function () {
                        sendAttedanceRequestNotification(notificationParams);
                    }, 1000);
                }
                else if (message == "Un Authorized Access") {
                    window.setTimeout(function () {
                        toastr.remove();
                        toastr.error("No entry! This is a restricted area, even for ninjas like you.");
                        window.setTimeout(function () {
                            window.location.href = "/PayTime/LoginPage";
                        }, 2000);
                        return false;
                    }, 1000);
                }

                else {
                    $('#dot_correction_modal').modal('hide');
                    toastr.remove();
                    toastr.error(_Meg);
                }
            }
            $(".loading").hide();
        });

    });



    $("#ddlLeaveType").change(function () {        
        $("#ddlLeaveType").removeClass('field_error');
        $(".popMsg").hide();
        CheckHalfLeave();
        GetLeaveBalance();
        CheckDocumentRequire();
        CheckLocationEnabled();
        $("#leaveTypeDurationBlock").hide();
    });
    $("#txtToDate").change(function () {
        $("#txtToDate").removeClass('field_error');
        GetLeaveBalance();
        CheckDocumentRequire();
    });
    $("#chkIsHalfLeave").on("change", function () {

        if ($(this).is(":checked")) {
            CheckDocumentRequire();
            $("#leaveTypeDurationBlock").show();
        }
        else {
            CheckDocumentRequire();
            $("#leaveTypeDurationBlock").hide();
            $("#ddlLeaveDuration").val("1");
        }
    });
    function CheckHalfLeave() {
        var ltype = $("#ddlLeaveType").val();
        $.ajax({
            type: "GET",
            url: "/Transactions/HalfLeaveValidate?LeaveType=" + parseInt(ltype),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            async: "false",
            success: function (response) {
                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                }
                if (response != null) {
                    if (response == "yes") {
                        $("#chkIsHalfLeave").prop('disabled', false);
                        $("#chkIsHalfLeave").prop('checked', false);
                    }
                    else {
                        $("#chkIsHalfLeave").prop('disabled', true);
                        $("#chkIsHalfLeave").prop('checked', false);
                    }
                }
                $(".loading").hide();
            },
            beforeSend: function () {
                $(".loading").show();
            },
            error: function (jqXHR, ajaxOptions, thrownError) {
                $(".loading").hide();

                if (jqXHR.status == 500) { //InternalServerError = 500,
                    toastr.error("InternalServerError");
                } else if (jqXHR.status == 400) { //BadRequest = 400,
                    toastr.error("BadRequest");
                } else if (jqXHR.status == 401) {
                    window.location.href = "/PayTime/LoginPage";
                }
            }
        });
    }
    function GetLeaveBalance() {
        checkSession();
        var EmpCode;
        if ($("#hfEmpCode").val() == "0") {
            EmpCode = empid;
        }
        else {
            EmpCode = empid;
        }
        if (DateFormate == "dd-mm-yyyy") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[2] + "-" + toDate[1] + "-" + toDate[0];
        }
        else if (DateFormate == "mm-dd-yyyy") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[2] + "-" + toDate[0] + "-" + toDate[1];
        }
        else if (DateFormate == "yyyy-mm-dd") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[0] + "-" + fromDate[1] + "-" + fromDate[2];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[0] + "-" + toDate[1] + "-" + toDate[2];
        }
        else if (DateFormate == "yyyy-M-dd") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[0] + "-" + fromDate[1] + "-" + fromDate[2];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[0] + "-" + toDate[1] + "-" + toDate[2];
        }
        else if (DateFormate == "M-dd-yyyy") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[2] + "-" + toDate[0] + "-" + toDate[1];
        }
        else if (DateFormate == "dd-M-yyyy") {
            var fromDate = $("#txtFromDate").val().split("-");
            var _Fromdate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
            var toDate = $("#txtToDate").val().split("-");
            var _toDate = toDate[2] + "-" + toDate[1] + "-" + toDate[0];
        }

        var OnDutyLeave = {

            "EmpId": (employeeids !== null && employeeids !== 0) ? employeeids : empid,
            "LeaveTypeId": parseInt($("#ddlLeaveType").val()),
            "FromDate": _Fromdate,
            "ToDate": _toDate,
            "PkEmpId": empid,
        };
        console.log(OnDutyLeave);
        $.ajax({
            type: "POST",
            url: "/Transactions/GetBalanceByLeaveType?OnDutyLeave=" + JSON.stringify(OnDutyLeave),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            async: "false",
            success: function (response) {
                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                }
                if (response.Msg == "Un Authorized Access") {
                    window.setTimeout(function () {
                        toastr.remove();
                        toastr.error("No entry! This is a restricted area, even for ninjas like you.");
                        window.setTimeout(function () {
                            window.location.href = "/PayTime/LoginPage";
                        }, 2000);
                        return false;
                    }, 1000);
                }
                else if (response != null) {
                    $("#lblLeaveBalance").text(response.LeaveBalance);
                    $("#lblTotalBalance").text(response.Total);
                    var lv = $("#lblLeaveBalance").text();
                    var a = lv.indexOf("-");
                    if (a != '-1') {
                        $("#lblLeaveBalance").text("0");
                        $("#lblTotalBalance").text("0");
                    }
                    var _lv = $("#lblTotalBalance").text();
                    var b = _lv.indexOf("-");
                    if (b != '-1') {
                        $("#lblTotalBalance").text("0");
                        $("#hfAvailBal").val(parseFloat(response.Total));
                    }
                    $("#lblEncashLeave").text(response.EncashLeaveBalance);

                    $("#hfAvailBal").val(parseFloat(response.Total));
                }
                else {
                    $("#lblLeaveBalance").text("0");
                    $("#lblEncashLeave").text("0");
                    $("#lblTotalBalance").text("0");
                    $("#hfAvailBal").val("0");
                }
                var leavebalance = $('#lblLeaveBalance').text();
                var ddlLeaveType = $("#ddlLeaveType").val();
                if (ddlLeaveType == 1) {
                    $("#ddlLeavePaid").empty();
                    $("#ddlLeavePaid").append("<option value='1' selected> Paid </option>");
                }
                else {
                    if (leavebalance > 0) {
                        $("#ddlLeavePaid").empty();
                        $("#ddlLeavePaid").append("<option value='1' selected> Paid </option>");
                    }
                    else {
                        $("#ddlLeavePaid").empty();
                        $("#ddlLeavePaid").append("<option value='0' selected> Unpaid </option>");
                    }
                }
                if (response.Total > 0) {
                    $("#IsPaidFlag").val("1");
                } else {
                    $("#IsPaidFlag").val("0");
                }
                $(".loading").hide();
            },
            beforeSend: function () {
                $(".loading").show();
            },
            error: function (jqXHR, ajaxOptions, thrownError) {
                $(".loading").hide();

                if (jqXHR.status == 500) { //InternalServerError = 500,
                    toastr.error("InternalServerError");
                } else if (jqXHR.status == 400) { //BadRequest = 400,
                    toastr.error("BadRequest");
                } else if (jqXHR.status == 401) {
                    window.location.href = "/PayTime/LoginPage";
                }
            }
        });
        window.onload = function () {
            timeSpentOnPage();
        }
        $(window).on("beforeunload", function () {
            if (operation == "" || operation == null || operation == '') {
                operation = "View Leave Request";
            }
            var url = '@_webapiurl' + '/DashBoard/AddAnalyticsUser';
            var timeSpent = timeSpentOnPage();
            analyticsDataUsage(url, '@_dbName', form, '@_tokan', count, operation, timeSpent)
        })
    }
    function CheckDocumentRequire() {        
        var ltype = $("#ddlLeaveType").val();
        $.ajax({
            type: "GET",
            url: "/Transactions/DocumentRequireForLeave?LeaveType=" + parseInt(ltype),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            async: "false",
            success: function (response) {
                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                }
                if (response != null && response.length > 0) {
                    if (response[0].IsDocRequire) {
                        if (DateFormate == "dd-mm-yyyy") {
                            var fromDate = $("#txtFromDate").val().split("-");
                            var _Fromdate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                            var toDate = $("#txtToDate").val().split("-");
                            var _toDate = toDate[2] + "-" + toDate[1] + "-" + toDate[0];
                        }
                        else if (DateFormate == "mm-dd-yyyy") {
                            var fromDate = $("#txtFromDate").val().split("-");
                            var _Fromdate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                            var toDate = $("#txtToDate").val().split("-");
                            var _toDate = toDate[2] + "-" + toDate[0] + "-" + toDate[1];
                        }
                        else if (DateFormate == "yyyy-mm-dd") {
                            var fromDate = $("#txtFromDate").val().split("-");
                            var _Fromdate = fromDate[0] + "-" + fromDate[1] + "-" + fromDate[2];
                            var toDate = $("#txtToDate").val().split("-");
                            var _toDate = toDate[0] + "-" + toDate[1] + "-" + toDate[2];
                        }
                        else if (DateFormate == "yyyy-M-dd") {
                            var fromDate = $("#txtFromDate").val().split("-");
                            var _Fromdate = fromDate[0] + "-" + fromDate[1] + "-" + fromDate[2];
                            var toDate = $("#txtToDate").val().split("-");
                            var _toDate = toDate[0] + "-" + toDate[1] + "-" + toDate[2];
                        }
                        else if (DateFormate == "M-dd-yyyy") {
                            var fromDate = $("#txtFromDate").val().split("-");
                            var _Fromdate = fromDate[2] + "-" + fromDate[0] + "-" + fromDate[1];
                            var toDate = $("#txtToDate").val().split("-");
                            var _toDate = toDate[2] + "-" + toDate[0] + "-" + toDate[1];
                        }
                        else if (DateFormate == "dd-M-yyyy") {
                            var fromDate = $("#txtFromDate").val().split("-");
                            var _Fromdate = fromDate[2] + "-" + fromDate[1] + "-" + fromDate[0];
                            var toDate = $("#txtToDate").val().split("-");
                            var _toDate = toDate[2] + "-" + toDate[1] + "-" + toDate[0];
                        }
                        if ($("#chkIsHalfLeave").prop("checked")) {
                            var diff = Date.parse(_toDate) - Date.parse(_Fromdate);
                            days = diff / 1000 / 60 / 60 / 24;
                            daysDiff = (days + 1) / 2;
                            if (daysDiff >= 0.5 && daysDiff >= response[0].LimitDays) {
                                $("#uploadDoc").show();
                            } else {
                                $("#uploadDoc").hide();
                                $("#UploadLeaveDoc").text("");
                                $("#UploadLeaveDoc").val("");
                            }
                        }
                        else {
                            var dateDiff = Math.abs(new Date(_toDate) - new Date(_Fromdate));
                            var daysDiff = Math.ceil(dateDiff / (1000 * 60 * 60 * 24));
                            if (daysDiff === response[0].LimitDays - 1 || daysDiff >= response[0].LimitDays - 1) {
                                $("#uploadDoc").show();
                            }
                            else {
                                $("#uploadDoc").hide();
                                $("#UploadLeaveDoc").text("");
                                $("#UploadLeaveDoc").val("");
                            }
                        }
                    }
                }
                else {
                    $("#uploadDoc").hide();
                }
                $(".loading").hide();
            },
            beforeSend: function () {
                $(".loading").show();
            },
            error: function (jqXHR, ajaxOptions, thrownError) {
                $(".loading").hide();

                if (jqXHR.status == 500) { //InternalServerError = 500,
                    toastr.error("InternalServerError");
                } else if (jqXHR.status == 400) { //BadRequest = 400,
                    toastr.error("BadRequest");
                } else if (jqXHR.status == 401) {
                    window.location.href = "/PayTime/LoginPage";
                }
            }
        });
    }
    function CheckLocationEnabled() {
        var ltype = $("#ddlLeaveType").val();
        $.ajax({
            type: "GET",
            url: "/Transactions/DocumentRequireForLeave?LeaveType=" + parseInt(ltype),
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            async: "false",
            success: function (response) {
                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                }
                if (response != null && response.length > 0) {
                    locationenabled = response[0].IsLocationEnabled;
                    if (locationenabled != 0) {
                        Gcode();
                    }
                }
                $(".loading").hide();
            },
            beforeSend: function () {
                $(".loading").show();
            },
            error: function (jqXHR, ajaxOptions, thrownError) {
                $(".loading").hide();

                if (jqXHR.status == 500) { //InternalServerError = 500,
                    toastr.error("InternalServerError");
                } else if (jqXHR.status == 400) { //BadRequest = 400,
                    toastr.error("BadRequest");
                } else if (jqXHR.status == 401) {
                    window.location.href = "/PayTime/LoginPage";
                }
            }
        });
    }
    //----------------------------Details of counts---------------------------
    function strengthDetails() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 9, hierarchyState: hierarchy },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },

            success: function (response) {

                if (Array.isArray(response) && response.length > 0) {
                    // Destroy any existing DataTable instance to avoid reinitialization issues
                    if ($.fn.DataTable.isDataTable("#teamtotal_tbl")) {
                        $("#teamtotal_tbl").DataTable().clear().destroy();
                        $('#dataTables_tbl_header').remove();
                    }

                    // Initialize the DataTable
                    $("#teamtotal_tbl").DataTable({
                        data: response, // Use the response data directly
                        columns: [
                            { data: "empcode", title: "Employee Code" },
                            { data: "empname", title: "Name" },
                            { data: "emppunchid", title: "Punch ID" },
                            { data: "departmentname", title: "Department" },
                            { data: "designationname", title: "Designation" },
                            { data: "shiftname", title: "Shift" },
                            { data: "useremail", title: "Email" },
                            {
                                data: "empmno",
                                title: "Mobile No",
                                render: function (data, type, row) {
                                    if (roleId == '1' || roleId == '6805') {
                                        return data || 'N/A';
                                    } else {
                                        if (data) {
                                            return "*******" + data.slice(-3);
                                        } else {
                                            return 'N/A';
                                        }
                                    }
                                }
                            }
                        ],
                        paging: true,
                        searching: true,
                        ordering: true,
                        lengthChange: true,
                        autoWidth: false,

                        dom: _domCommon,
                        language: _languageCommon,

                        "bDestroy": true,
                        "buttons": [
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

                        initComplete: function () {


                            const reqid = 'teamtotal_tbl';
                            $("#buttons").append(`
                            <div id="export-button" class="export-button tooltips" data-placement="bottom" data-original-title="Download" data-table-id="${reqid}">
                                <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
                            </div>
                           `);

                        },


                    });

                    setTimeout(function () {

                        // Move the header
                        $("#" + "teamtotal_tbl" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive_total"));
                        $('.tooltips').tooltip();
                    }, 100);
                    //var customDiv = $('<div class="customforms_table"></div>');
                    //customDiv.insertBefore("#teamtotal_tbl_wrapper > .dataTables_info");
                    //$("#teamtotal_tbl_wrapper > table").appendTo(customDiv);
                    //$("#total_strength").attr("data-toggle", "modal");
                    //$("#total_strength").attr("data-target", "#teamtotal_modal");
                } else {
                    console.log("no data available");
                }
                $(".loading").hide();
            },

            beforeSend: function () {
                $(".loading").show();
            },
            error: function () {
                $(".loading").hide();
            }
        });
    }

    function CheckinDetails() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 10, hierarchyState: hierarchy },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },

            success: function (response) {


                if (Array.isArray(response) && response.length > 0) {
                    // Destroy any existing DataTable instance to avoid reinitialization issues
                    if ($.fn.DataTable.isDataTable("#teamcheckin_tbl")) {
                        $("#teamcheckin_tbl").DataTable().clear().destroy();
                        $('#dataTables_tbl_header').remove();
                        $('#dataTables_tbl_header').remove();
                    }

                    // Initialize the DataTable
                    $("#teamcheckin_tbl").DataTable({
                        data: response, // Use the response data directly
                        columns: [
                            { data: "EmpCode", title: "Employee Code" },
                            { data: "EmpName", title: "Name" },
                            { data: "EmpPunchId", title: "Punch ID" },
                            { data: "departmentname", title: "Department" },
                            { data: "designationname", title: "Designation" },
                            { data: "ShiftName", title: "Shift" },
                            { data: "Mode", title: "Status" },
                            { data: "InTime", title: "PunchTime" },
                            { data: "currentdate", title: "Date" },
                        ],
                        paging: true,
                        searching: true,
                        ordering: true,
                        lengthChange: true,
                        autoWidth: false,
                        dom: _domCommon,
                        language: _languageCommon,

                        "bDestroy": true,
                        "buttons": [
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

                        initComplete: function () {


                            const reqid = 'teamcheckin_tbl';
                            $("#buttons").append(`
                            <div id="export-button" class="export-button tooltips" data-placement="bottom" data-original-title="Download" data-table-id="${reqid}">
                                <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
                            </div>
                           `);

                        },
                    });
                    setTimeout(function () {

                        // Move the header
                        $("#" + "teamcheckin_tbl" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive_CheckIn"));
                        $('.tooltips').tooltip();
                    }, 100);
                    //var customDiv = $('<div class="customforms_table"></div>');
                    //customDiv.insertBefore("#teamcheckin_tbl_wrapper > .dataTables_info");
                    //$("#teamcheckin_tbl_wrapper > table").appendTo(customDiv);
                    //$("#checking_strength").attr("data-toggle", "modal");
                    //$("#checking_strength").attr("data-target", "#teamcheckin_modal");
                } else {

                }
                $(".loading").hide();
            },

            beforeSend: function () {
                $(".loading").show();
            },
            error: function () {
                $(".loading").hide();
            }
        });
    }
    function AbsentDetails() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 11, hierarchyState: hierarchy },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },

            success: function (response) {


                if (Array.isArray(response) && response.length > 0) {
                    // Destroy any existing DataTable instance to avoid reinitialization issues
                    if ($.fn.DataTable.isDataTable("#teamabsent_tbl")) {
                        $("#teamabsent_tbl").DataTable().clear().destroy();
                        $('#dataTables_tbl_header').remove();
                        $('#dataTables_tbl_header').remove();
                    }

                    // Initialize the DataTable
                    $("#teamabsent_tbl").DataTable({
                        data: response, // Use the response data directly
                        columns: [
                            { data: "Empcode", title: "Empcode" },
                            { data: "EmpName", title: "EmpName" },
                            { data: "emppunchid", title: "Punch ID" },
                            { data: "departmentname", title: "Department" },
                            { data: "designationname", title: "Designation" },
                            { data: "ShiftName", title: "Shift" },
                            { data: "currentdate", title: "Date" },
                        ],
                        paging: true,
                        searching: true,
                        ordering: true,
                        lengthChange: true,
                        autoWidth: false,

                        dom: _domCommon,
                        language: _languageCommon,

                        "bDestroy": true,
                        "buttons": [
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

                        initComplete: function () {


                            const reqid = 'teamabsent_tbl';
                            $("#buttons").append(`
                            <div id="export-button" class="export-button tooltips" data-placement="buttom" data-original-title="Download" data-table-id="${reqid}">
                                <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
                            </div>
                           `);

                        },

                    });
                    setTimeout(function () {

                        // Move the header
                        $("#" + "teamabsent_tbl" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive_Absent"));
                        $('.tooltips').tooltip();
                    }, 100);
                    //var customDiv = $('<div class="customforms_table"></div>');
                    //customDiv.insertBefore("#teamabsent_tbl_wrapper > .dataTables_info");
                    //$("#teamabsent_tbl_wrapper > table").appendTo(customDiv);
                    //$("#absent_strength").attr("data-toggle", "modal");
                    //$("#absent_strength").attr("data-target", "#teamabsent_modal");
                } else {

                }
                $(".loading").hide();
            },

            beforeSend: function () {
                $(".loading").show();
            },
            error: function () {
                $(".loading").hide();
            }
        });
    }
    function LeaveDetailsDetails() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 12, hierarchyState: hierarchy },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },

            success: function (response) {


                if (Array.isArray(response) && response.length > 0) {
                    // Destroy any existing DataTable instance to avoid reinitialization issues
                    if ($.fn.DataTable.isDataTable("#teamleave_tbl")) {
                        $("#teamleave_tbl").DataTable().clear().destroy();
                        $('#dataTables_tbl_header').remove();
                        $('#dataTables_tbl_header').remove();
                    }

                    // Initialize the DataTable
                    $("#teamleave_tbl").DataTable({
                        data: response, // Use the response data directly
                        columns: [
                            { data: "empcode", title: "Empcode" },
                            { data: "empname", title: "EmpName" },
                            { data: "emppunchid", title: "Punch ID" },
                            { data: "departmentname", title: "Department" },
                            { data: "designationname", title: "Designation" },
                            { data: "finalstatus", title: "LeaveType" },
                            { data: "HalfLeave", title: "HalfLeave" },
                            { data: "LeavePaid", title: "Paid/UnPaid" },
                            { data: "currentdate", title: "Date" },
                        ],
                        paging: true,
                        searching: true,
                        ordering: true,
                        lengthChange: true,
                        autoWidth: false,

                        dom: _domCommon,
                        language: _languageCommon,

                        "bDestroy": true,
                        "buttons": [
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

                        initComplete: function () {

                            const reqid = 'teamleave_tbl';
                            $("#buttons").append(`
                            <div id="export-button" class="export-button tooltips" data-placement="bottom" data-original-title="Download" data-table-id="${reqid}">
                                <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
                            </div>
                           `);

                        },
                    });
                    setTimeout(function () {

                        $("#" + "teamleave_tbl" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive_Leave"));
                        $('.tooltips').tooltip();
                    }, 100);
                    //var customDiv = $('<div class="customforms_table"></div>');
                    //customDiv.insertBefore("#teamleave_tbl_wrapper > .dataTables_info");
                    //$("#teamleave_tbl_wrapper > table").appendTo(customDiv);
                    //$("#leave_strength").attr("data-toggle", "modal");
                    //$("#leave_strength").attr("data-target", "#teamleave_modal");
                } else {

                }
                $(".loading").hide();
            },

            beforeSend: function () {
                $(".loading").show();
            },
            error: function () {
                $(".loading").hide();
            }
        });
    }
    function LateinDetails() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 13, hierarchyState: hierarchy },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },

            success: function (response) {


                if (Array.isArray(response) && response.length > 0) {
                    // Destroy any existing DataTable instance to avoid reinitialization issues
                    if ($.fn.DataTable.isDataTable("#teamlatein_tbl")) {
                        $("#teamlatein_tbl").DataTable().clear().destroy();
                        $('#dataTables_tbl_header').remove();
                        $('#dataTables_tbl_header').remove();

                    }

                    // Initialize the DataTable
                    $("#teamlatein_tbl").DataTable({
                        data: response, // Use the response data directly
                        columns: [
                            { data: "EmployeeID", title: "Empcode" },
                            { data: "EmpName", title: "EmpName" },
                            { data: "EmpPunchID", title: "Punch ID" },
                            { data: "DepartmentName", title: "Department" },
                            { data: "DesignationName", title: "Designation" },
                            { data: "shiftname", title: "Shift" },
                            { data: "ShiftStartTime", title: "Shift Start Time" },
                            { data: "ShiftEndTime", title: "Shift End Time" },
                            { data: "SubqueryInTime", title: "PunchInTime" },
                            { data: "LateInMins", title: "Late In Minutes" },
                            { data: "currentdate", title: "Date" },
                        ],
                        paging: true,
                        searching: true,
                        ordering: true,
                        lengthChange: true,
                        autoWidth: false,

                        dom: _domCommon,
                        language: _languageCommon,

                        "bDestroy": true,
                        "buttons": [
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

                        initComplete: function () {


                            const reqid = 'teamlatein_tbl';
                            $("#buttons").append(`
                            <div id="export-button" class="export-button tooltips" data-placement="bottom" data-original-title="Download" data-table-id="${reqid}">
                                <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
                            </div>
                           `);

                        },

                    });

                    setTimeout(function () {

                        // Move the header
                        $("#" + "teamlatein_tbl" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive_LateIn"));
                        $('.tooltips').tooltip();
                    }, 100);
                    //var customDiv = $('<div class="customforms_table"></div>');
                    //customDiv.insertBefore("#teamlatein_tbl_wrapper > .dataTables_info");
                    //$("#teamlatein_tbl_wrapper > table").appendTo(customDiv);
                    //$("#latein_strength").attr("data-toggle", "modal");
                    //$("#latein_strength").attr("data-target", "#teamlatein_modal");
                } else {

                }
                $(".loading").hide();
            },

            beforeSend: function () {
                $(".loading").show();
            },
            error: function () {
                $(".loading").hide();
            }
        });
    }
    function Earlyoutdetails() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 14, hierarchyState: hierarchy },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },

            success: function (response) {


                if (Array.isArray(response) && response.length > 0) {
                    // Destroy any existing DataTable instance to avoid reinitialization issues
                    if ($.fn.DataTable.isDataTable("#teamearlyout_tbl")) {
                        $("#teamearlyout_tbl").DataTable().clear().destroy();
                        $('#dataTables_tbl_header').remove();
                        $('#dataTables_tbl_header').remove();
                    }

                    // Initialize the DataTable
                    $("#teamearlyout_tbl").DataTable({
                        data: response, // Use the response data directly
                        columns: [
                            { data: "EmployeeID", title: "Empcode" },
                            { data: "EmpName", title: "EmpName" },
                            { data: "EmpPunchID", title: "Punch ID" },
                            { data: "DepartmentName", title: "Department" },
                            { data: "DesignationName", title: "Designation" },
                            { data: "shiftname", title: "Shift" },
                            { data: "ShiftStartTime", title: "Shift Start Time" },
                            { data: "ShiftEndTime", title: "Shift End Time" },
                            { data: "OutTime", title: "PunchoutTime" },
                            { data: "EarlyOutMin", title: "Early Out Minutes" },
                            { data: "currentdate", title: "Date" },
                        ],
                        paging: true,
                        searching: true,
                        ordering: true,
                        lengthChange: true,
                        autoWidth: false,

                        dom: _domCommon,
                        language: _languageCommon,

                        "bDestroy": true,
                        "buttons": [
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

                        initComplete: function () {

                            const reqid = 'teamearlyout_tbl';
                            $("#buttons").append(`
                            <div id="export-button" class="export-button tooltips" data-placement="bottom" data-original-title="Download" data-table-id="${reqid}">
                                <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
                            </div>
                           `);

                        },

                    });

                    setTimeout(function () {

                        // Move the header
                        $("#" + "teamearlyout_tbl" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive_EarlyOut"));
                        $('.tooltips').tooltip();
                    }, 100);


                    //var customDiv = $('<div class="customforms_table"></div>');
                    //customDiv.insertBefore("#teamearlyout_tbl_wrapper > .dataTables_info");
                    //$("#teamearlyout_tbl_wrapper > table").appendTo(customDiv);
                    //$("#earlyout_strength").attr("data-toggle", "modal");
                    //$("#earlyout_strength").attr("data-target", "#teamearlyout_modal");
                } else {

                }
                $(".loading").hide();
            },

            beforeSend: function () {
                $(".loading").show();
            },
            error: function () {
                $(".loading").hide();
            }
        });
    }
    function EarlyInDetails() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 16, hierarchyState: hierarchy },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },

            success: function (response) {


                if (Array.isArray(response) && response.length > 0) {
                    // Destroy any existing DataTable instance to avoid reinitialization issues
                    if ($.fn.DataTable.isDataTable("#teamearlyin_tbl")) {
                        $("#teamearlyin_tbl").DataTable().clear().destroy();
                        $('#dataTables_tbl_header').remove();
                        $('#dataTables_tbl_header').remove();
                    }

                    // Initialize the DataTable
                    $("#teamearlyin_tbl").DataTable({
                        data: response, // Use the response data directly
                        columns: [
                            { data: "EmployeeID", title: "Empcode" },
                            { data: "EmpName", title: "EmpName" },
                            { data: "EmpPunchID", title: "Punch ID" },
                            { data: "DepartmentName", title: "Department" },
                            { data: "DesignationName", title: "Designation" },
                            { data: "shiftname", title: "Shift" },
                            { data: "ShiftStartTime", title: "Shift Start Time" },
                            { data: "ShiftEndTime", title: "Shift End Time" },
                            { data: "SubqueryInTime", title: "PunchInTime" },
                            { data: "EarlyInMin", title: "Early In Minutes" },
                            { data: "currentdate", title: "Date" },
                        ],
                        paging: true,
                        searching: true,
                        ordering: true,
                        lengthChange: true,
                        autoWidth: false,

                        dom: _domCommon,
                        language: _languageCommon,

                        "bDestroy": true,
                        "buttons": [
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

                        initComplete: function () {


                            const reqid = 'teamearlyin_tbl';
                            $("#buttons").append(`
                            <div id="export-button" class="export-button tooltips" data-placement="bottom" data-original-title="Download" data-table-id="${reqid}">
                                <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
                            </div>
                           `);

                        },

                    });

                    setTimeout(function () {

                        // Move the header
                        $("#" + "teamearlyin_tbl" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive_EarlyIn"));
                        $('.tooltips').tooltip();
                    }, 100);



                    //var customDiv = $('<div class="customforms_table"></div>');
                    //customDiv.insertBefore("#teamearlyin_tbl_wrapper > .dataTables_info");
                    //$("#teamearlyin_tbl_wrapper > table").appendTo(customDiv);
                    //$("#earlyin_strength").attr("data-toggle", "modal");
                    //$("#earlyin_strength").attr("data-target", "#teamearlyin_modal");
                } else {

                }
                $(".loading").hide();
            },

            beforeSend: function () {
                $(".loading").show();
            },
            error: function () {
                $(".loading").hide();
            }
        });
    }
    function LateOutDetails() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/newEmployeeDashboard`,
            data: { empid: empid, reqid: 15, hierarchyState: hierarchy },
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            headers: {
                'Authorization': token
            },

            success: function (response) {


                if (Array.isArray(response) && response.length > 0) {
                    // Destroy any existing DataTable instance to avoid reinitialization issues
                    if ($.fn.DataTable.isDataTable("#teamlateout_tbl")) {
                        $("#teamlateout_tbl").DataTable().clear().destroy();
                        $('#dataTables_tbl_header').remove();
                        $('#dataTables_tbl_header').remove();
                    }

                    // Initialize the DataTable
                    $("#teamlateout_tbl").DataTable({
                        data: response, // Use the response data directly
                        columns: [
                            { data: "EmployeeID", title: "Empcode" },
                            { data: "EmpName", title: "EmpName" },
                            { data: "EmpPunchID", title: "Punch ID" },
                            { data: "DepartmentName", title: "Department" },
                            { data: "DesignationName", title: "Designation" },
                            { data: "shiftname", title: "Shift" },
                            { data: "ShiftStartTime", title: "Shift Start Time" },
                            { data: "ShiftEndTime", title: "Shift End Time" },
                            { data: "SubqueryInTime", title: "PunchoutTime" },
                            { data: "LateOutMins", title: "Late Out Minutes" },
                            { data: "currentdate", title: "Date" },
                        ],
                        paging: true,
                        searching: true,
                        ordering: true,
                        lengthChange: true,
                        autoWidth: false,

                        dom: _domCommon,
                        language: _languageCommon,

                        "bDestroy": true,
                        "buttons": [
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

                        initComplete: function () {


                            const reqid = 'teamlateout_tbl';
                            $("#buttons").append(`
                            <div id="export-button" class="export-button tooltips" data-placement="bottom" data-original-title="Download" data-table-id="${reqid}">
                                <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
                            </div>
                           `);

                        },


                    });

                    setTimeout(function () {

                        // Move the header
                        $("#" + "teamlateout_tbl" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive_LateOut"));
                        $('.tooltips').tooltip();
                    }, 100);


                    //var customDiv = $('<div class="customforms_table"></div>');
                    //customDiv.insertBefore("#teamlateout_tbl_wrapper > .dataTables_info");
                    //$("#teamlateout_tbl_wrapper > table").appendTo(customDiv);
                    //$("#lateout_strength").attr("data-toggle", "modal");
                    //$("#lateout_strength").attr("data-target", "#teamlateout_modal");
                } else {

                }
                $(".loading").hide();
            },

            beforeSend: function () {
                $(".loading").show();
            },
            error: function () {
                $(".loading").hide();
            }
        });
    }

     //----------------------------Request Call For Notification Event---------------------------

    function sendAttedanceRequestNotification(params, successCallback, errorCallback)
    {        
        //?? Call to send notification (Backend API call)
        $.ajax({
            url: `${webApiUrl}` + "NotificationRuleDetails/AttedanceCorrectionRequestNotification",
            type: 'POST',
            headers: { 'Authorization': token },
            dataType: "json",
            data: {
                EmpId: params.EmpId,
                InPunchTime: params.InPunchTime,
                OutPunchTime: params.OutPunchTime,
                ApplyReason: params.ApplyReason,
                CorectionType: params.CorectionType,
                EmployeeName: params.EmployeeName,
                EventId: params.EventId,
                AttCorrectionID: params.AttCorrectionID,
            },
            success: function (response) {
                //console.log("Notification sent successfully:", response);
                if (typeof successCallback === 'function') {
                    successCallback(response);
                }
            },
            error: function (xhr, status, error) {
                //console.warn("Notification sending failed:", error);
                if (typeof errorCallback === 'function') {
                    errorCallback(xhr, status, error);
                }
            }
        });
    }
    //function sendWebPunchRequestNotification(params, successCallback, errorCallback)
    //{        
    //    //?? Call to send notification (Backend API call)
    //    $.ajax({
    //        url: `${webApiUrl}` + "NotificationRuleDetails/WebPunchRequestNotification",
    //        type: 'POST',
    //        headers: { 'Authorization': token },
    //        dataType: "json",
    //        data: {
    //            EmpId: params.EmpId,
    //            InPunchTime: params.InPunchTime,
    //            OutPunchTime: params.OutPunchTime,
    //            ApplyReason: params.ApplyReason,
    //            CorectionType: params.CorectionType,
    //            EmployeeName: params.EmployeeName,
    //            EventId: params.EventId,
    //        },
    //        success: function (response) {
    //            console.log("Notification sent successfully:", response);
    //            if (typeof successCallback === 'function')
    //            {
    //                successCallback(response);
    //            }
    //        },
    //        error: function (xhr, status, error) {
    //            console.warn("Notification sending failed:", error);
    //            if (typeof errorCallback === 'function') {
    //                errorCallback(xhr, status, error);
    //            }
    //        }
    //    });
    //}
    function sendLeaveRequestNotification(params, successCallback, errorCallback) {
        //?? Call to send notification (Backend API call)
        $.ajax({
            url: `${webApiUrl}`  + "NotificationRuleDetails/LeaveRequestNotification",
            type: 'POST',
            headers: { 'Authorization': token },
            dataType: "json",
            data:
            {
                EmpId: params.EmpId,
                InPunchTime: params.InPunchTime,
                OutPunchTime: params.OutPunchTime,
                ApplyReason: params.ApplyReason,
                LeaveTypeName: params.LeaveTypeName,
                EmployeeName: params.EmployeeName,
                EventId: params.EventId,
                LeaveCorrectionID: params.LeaveCorrectionID,
            },
            success: function (response) {
               //console.log("Notification sent successfully:", response);
                if (typeof successCallback === 'function') {
                    successCallback(response);
                }
            },
            error: function (xhr, status, error) {
                //console.warn("Notification sending failed:", error);
                if (typeof errorCallback === 'function') {
                    errorCallback(xhr, status, error);
                }
            }
        });
    }
});

