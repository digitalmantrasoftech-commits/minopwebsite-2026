
let WidgetIdSession = sessionStorage.getItem("WidgetId");

const token = window.AppConfig.token;
const roleId = window.AppConfig.roleId;
const webApiUrl = window.AppConfig.webApiUrl;
const emprole = window.AppConfig.roleId;
let cmpId = window.AppConfig.cmpId;
let branchId = window.AppConfig.branchId;
const empid = window.AppConfig.empid;
let widgetId = window.AppConfig.widgetId;
const UserId = window.AppConfig.UserId;

// for page refresh set widgetid
if (WidgetIdSession != "" && WidgetIdSession != null) {

    if (WidgetIdSession != widgetId) {
        widgetId = WidgetIdSession;
    }
}
$(document).ready(function () {
    $('#attendanceTabLink').on('click', function () {
        sessionStorage.setItem('activeTab', '#attendancetab_2' );
    });
    $('#leaveTabLink').on('click', function () {
        sessionStorage.setItem('activeTab', '#leavetab_1' );
    });
    $('#webpunchTabLink').on('click', function () {
        sessionStorage.setItem('activeTab', '#webpunchtab_3');
    });
    checkSession();
    var currentdate;
    var employeePresent;
    var employeeAbsent;
    let companyIds = "";
    let branchIds = "";

    var $Companyselect = $("#CompanyID").multiselect({
        selectAllValue: 'multiselect-all',
        enableCaseInsensitiveFiltering: true,
        includeSelectAllOption: true,
        allSelectedText: 'All Select',
        enableFiltering: true,
        maxHeight: '300',
        buttonWidth: '100%',
        enableClickableOptGroups: true,
        nonSelectedText: 'Select Company',
    });
    var $Branchselect = $("#BranchID").multiselect({
        selectAllValue: 'multiselect-all',
        enableCaseInsensitiveFiltering: true,
        includeSelectAllOption: true,
        allSelectedText: 'All Select',
        enableFiltering: true,
        maxHeight: '300',
        buttonWidth: '100%',
        enableClickableOptGroups: true,
        nonSelectedText: 'Select Branch',

    });
    var $Departmentselect = $("#DepartmentID").multiselect({
        selectAllValue: 'multiselect-all',
        enableCaseInsensitiveFiltering: true,
        includeSelectAllOption: true,
        allSelectedText: 'All Select',
        enableFiltering: true,
        maxHeight: '300',
        buttonWidth: '100%',
        enableClickableOptGroups: true,
        nonSelectedText: 'Select Department',
    });

    $('#CompanyID').change(function () {
        companyIds = ""; // Reset the global variable

        $('#CompanyID :selected').each(function () {
            if (companyIds === "") {
                companyIds = $(this).val();
            } else {
                companyIds += "," + $(this).val();
            }
        });

        var s = $('#CompanyID').val();
        if (s === "" || s === null) {
            location.reload();
        } else {
            $('#CompanyName').val($("#CompanyID option:selected").map(function () {
                return $(this).text();
            }).get().join(", "));

            fillBranchbycompnyid(s, "");
        }
    });


    $('#BranchID').change(function () {
        branchIds = "";

        $('#BranchID :selected').each(function () {
            if (branchIds === "") {
                branchIds = $(this).val();
            } else {
                branchIds += "," + $(this).val();
            }
        });

        var b = $('#BranchID').val();
        if (b === "" || b === null) {
            location.reload();
        } else {
            $('#BranchName').val($('#BranchID option:selected').map(function () {
                return $(this).text();
            }).get().join(", "));


        }
    });

    //for quick serach
    $('#CompanyIDNew').change(function () {

        var s = $('#CompanyIDNew').val();
        fillBranchbycompnyidquick(s, "");
        if ($("#CompanyIDNew").val() == "" || $("#CompanyIDNew").val() == null) {
            cmpId = "0";
            branchId = "0";
        }
        else {
            cmpId = $('#CompanyIDNew').val();
        }
    });

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

    //comman validation of form
    function commonvalidation(event) {
       
        

        function containsEmoji(text) {
            const emojiRegex = /[\u{1F600}-\u{1F64F}\u{1F300}-\u{1F5FF}\u{1F680}-\u{1F6FF}\u{1F700}-\u{1F77F}\u{1F780}-\u{1F7FF}\u{1F800}-\u{1F8FF}\u{1F900}-\u{1F9FF}\u{1FA00}-\u{1FA6F}\u{1FA70}-\u{1FAFF}\u{2600}-\u{26FF}\u{2700}-\u{27BF}\u{FE0F}\u{1F1E0}-\u{1F1FF}]/u;
            return emojiRegex.test(text);
        }

        //validation for company


        var fromDt = $("#startDate").val();
        var toDt = $("#endDate").val();

        // Validation for FromDate
        if (fromDt == "") {
            $("#startDate").addClass('field_error');
            event.preventDefault(); // prevents form submission
            return false;
        } else {
            $("#startDate").removeClass('field_error');
        }

        // Validation for ToDate
        if (toDt == "") {
            $("#endDate").addClass('field_error');
            event.preventDefault();
            return false;
        } else {
            $("#endDate").removeClass('field_error'); 
        }

        var fromDate = $("#startDate").val();
        var toDate = $("#endDate").val();

        // Compare dates
        if (fromDate > toDate) {
            toastr.remove();
            toastr.error("ToDate should be greater than FromDate.");
            return false;
        }

        //validation for title

        var titles = $('#title').val();
        if (titles == null || titles == "" || titles == undefined) {
            $("#title").addClass('field_error');
            event.preventDefault();
            return false;
        }
        else if (titles.length > 100) {
            toastr.error("Title text limit exceeds 100 words!");
            return false;
        }
        
        else {
            $("#title").removeClass('field_error');
        }

        //validation for description
        var Des = $('#descriptions').val();
        if (Des == null || Des == "" || Des == undefined) {
            $("#descriptions").addClass('field_error');
            event.preventDefault();
            return false;
        } else if (Des.length > 1000) {
            toastr.error("Description text limit exceeds 1000 words!");
            return false;
        }
        else {
            $("#descriptions").removeClass('field_error');
        }
        var _CompanyId = $("#CompanyID option:selected").text();
        if (_CompanyId == "Select Company" || _CompanyId == "") {
            toastr.remove();
            toastr.error("Please select company.");
            $("#CompanyID").addClass('field_error');
            event.preventDefault();//or return false
            $(".loading").hide();
            return false;
        }
        else {
            $("#CompanyID").removeClass('field_error');
        }
        if (containsEmoji(Des)) {
            toastr.error("Emojis are not allowed in the description.");
            $("#descriptions").addClass('field_error');
            event.preventDefault();
            return false;
        }
        else {
            $("#descriptions").removeClass('field_error');
        }
        if (containsEmoji(titles)) {

            toastr.error("Emojis are not allowed in the titles.");
            $("#title").addClass('field_error');
            event.preventDefault();
            return false;
        }
        else {
            $("#title").removeClass('field_error');
        }



        return true;

    }
    $("#title").on('input', function () {
        if ($(this).val() != "") {
            $(this).removeClass('field_error');
        }
    });

    $("#descriptions").on('input', function () {
        if ($(this).val() != "") {
            $(this).removeClass('field_error');
        }
    });

    $("#CompanyID").on('change', function () {
        if ($(this).val() != null && $(this).val().length > 0) {
            $(this).removeClass('field_error');
        }
    });
    $("#startDate").on('input', function () {
        if ($(this).val() != "") {
            $(this).removeClass('field_error');
        }
    });

    $("#endDate").on('input', function () {
        if ($(this).val() != "") {
            $(this).removeClass('field_error');
        }
    });


    if (roleId == 6805 || roleId == 6806) {
        cmpId = cmpId;
        branchId = branchId;
    }

    if (cmpId == null || cmpId == "") {
        cmpId = 0;
    }
    if (branchId == "" || branchId == null) {
        branchId = 0;
    }
    $("#customwidget_btn").on('click', function () {
        $("#customwidget_btn").attr("data-toggle", "modal");
        $("#customwidget_btn").attr("data-target", "#customwidget_modal");
    });

    $("#addanoucement_btn").on('click', function () {
        $("#addanoucement_btn").attr("data-toggle", "modal");
        $("#addanoucement_btn").attr("data-target", "#addanoucement_modal");
    });


    //Call function on page load according to widget id
    IntialpageLoad();
    /*customized widget Intilization*/
    function IntialpageLoad() {
       
        const widgetArray = widgetId.split(',').map(Number);
        if (widgetArray.includes(1)) {


            $("#Activity-1").css("display", "block");
            $("#chk_1").prop("checked", true);
            Fillactivity_Checkin();
            Fillactivity_NotCheckin();
            Fillactivity_Latein();
            Fillactivity_EarlyOut();
            Fillactivity_Birthday();
            Fillactivity_DeviceCount();

        } if (widgetArray.includes(2)) {

            $("#Overview_2").css("display", "block");
            $("#chk_2").prop("checked", true);
            Attendence_Overview();

        } if (widgetArray.includes(3)) {
            $("#Annoucements_3").css("display", "block");
            $("#chk_3").prop("checked", true);

            annoucement_getData();

        } if (widgetArray.includes(4)) {
            $("#punchtype_4").css("display", "block");
            $("#chk_4").prop("checked", true);
            GetPunchData();

        } if (widgetArray.includes(5)) {
            $("#pp_5").css("display", "block");
            $("#chk_5").prop("checked", true);
            PresentPattern();

        } if (widgetArray.includes(6)) {
            $("#chk_6").prop("checked", true);
            $("#pendingrequest_6").css("display", "block");
            Getpendingleave_count();

        } if (widgetArray.includes(7)) {
            $("#chk_7").prop("checked", true);
            $("#leavebifuraction_7").css("display", "block");

            const currentMonth = new Date().toLocaleString('default', { month: 'long' });
            $("#monthdropdown .dropdown-menu li").removeClass("active");
            $("#monthdropdown .dropdown-menu a:contains(" + currentMonth + ")").parent().addClass("active");
            selectMonth(currentMonth);

        } if (widgetArray.includes(8)) {
            $("#AttendenceSummary_8").css("display", "block");
            $("#chk_8").prop("checked", true);
            Getattendencesummary(8);

        } if (widgetArray.includes(9)) {
            $("#OverTime_9").css("display", "block");
            $("#chk_9").prop("checked", true);
            Attendence_Overview();

        } if (widgetArray.includes(10)) {
            $("#Holiday_10").css("display", "block");
            $("#chk_10").prop("checked", true);
            Fillactivity_HolidayData();

        } if (widgetArray.includes(11)) {
            $("#Genderwise_11").css("display", "block");
            $("#chk_11").prop("checked", true);
            GenderWiseChart(7);

        } if (widgetArray.includes(12)) {
            $("#Attration_12").css("display", "block");
            $("#chk_12").prop("checked", true);
            AttrationChart();
        }
        if (!widgetArray.includes(4) && !widgetArray.includes(5)) {
            $("#bothcharts").css("display", "none");
        }
        if (widgetArray.includes(4) && !widgetArray.includes(5)) {
            $("#punchtype_4").addClass("w-100");
        }
        else {
            $("#punchtype_4").removeClass("w-100"); 
        }
        if (!widgetArray.includes(4) && widgetArray.includes(5)) {
            $("#pp_5").addClass("w-100");
        }
        else {
            $("#pp_5").removeClass("w-100");
        }
    }
    //Get those whose widget checked---

    $("#widgetSave").on("click", function () {


        const checkedValues = [];


        $("#customwidget_modal input[type='checkbox']").each(function () {
            if ($(this).prop("checked")) {

                const checkboxId = $(this).attr("id").split('_')[1];
                checkedValues.push(checkboxId);


            }
        });
        const result = checkedValues.join(',');
        const params = new URLSearchParams({
            roleId: roleId,
            WidgetIds: result,
            UserId: UserId
        });
        const urlWithQuery = `${webApiUrl}DashBoard/UpdateWidgetIds?${params.toString()}`;
        $.ajax({
            type: "POST",
            url: urlWithQuery,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {

                if (data.isRedirect) {
                    window.location.href = data.redirectUrl;
                } else {
                    toastr.success("Widgets updated successfully.");
                    const widgetArrays = result.split(',').map(Number);
                    if (widgetArrays.includes(1)) {


                        $("#Activity-1").css("display", "block");
                        $("#chk_1").prop("checked", true);
                        Fillactivity_Checkin();
                        Fillactivity_NotCheckin();
                        Fillactivity_Latein();
                        Fillactivity_EarlyOut();
                        Fillactivity_Birthday();
                        Fillactivity_DeviceCount();

                    } else {
                        $("#Activity-1").css("display", "none");
                        $("#chk_1").prop("checked", false);
                    }
                    if (widgetArrays.includes(2)) {

                        $("#Overview_2").css("display", "block");
                        $("#chk_2").prop("checked", true);
                        Attendence_Overview();

                    } else {
                        $("#Overview_2").css("display", "none");
                        $("#chk_2").prop("checked", false);
                    }
                    if (widgetArrays.includes(3)) {
                        $("#Annoucements_3").css("display", "block");
                        $("#chk_3").prop("checked", true);

                        annoucement_getData();

                    } else {
                        $("#Annoucements_3").css("display", "none");
                        $("#chk_3").prop("checked", false);
                    }
                    if (widgetArrays.includes(4)) {
                        $("#bothcharts").css("display", "block");
                        $("#punchtype_4").css("display", "block");
                        $("#chk_4").prop("checked", true);
                        GetPunchData();

                    }
                    else {

                        $("#punchtype_4").css("display", "none");
                        $("#chk_4").prop("checked", false);

                    }
                    if (widgetArrays.includes(5)) {
                        $("#bothcharts").css("display", "block");
                        $("#pp_5").css("display", "block");
                        $("#chk_5").prop("checked", true);
                        PresentPattern();

                    }
                    else {
                        $("#pp_5").css("display", "none");
                        $("#chk_5").prop("checked", false);
                    }
                    if (widgetArrays.includes(6)) {
                        $("#chk_6").prop("checked", true);
                        $("#pendingrequest_6").css("display", "block");
                        Getpendingleave_count();

                    } else {
                        $("#pendingrequest_6").css("display", "none");
                        $("#chk_6").prop("checked", false);
                    }
                    if (widgetArrays.includes(7)) {
                        $("#chk_7").prop("checked", true);
                        $("#leavebifuraction_7").css("display", "block");

                        const currentMonth = new Date().toLocaleString('default', { month: 'long' });
                        $("#monthdropdown .dropdown-menu li").removeClass("active");
                        $("#monthdropdown .dropdown-menu a:contains(" + currentMonth + ")").parent().addClass("active");
                        selectMonth(currentMonth);

                    }
                    else {
                        $("#leavebifuraction_7").css("display", "none");
                        $("#chk_7").prop("checked", false);
                    }
                    if (widgetArrays.includes(8)) {
                        $("#AttendenceSummary_8").css("display", "block");
                        $("#chk_8").prop("checked", true);
                        Getattendencesummary(8);

                    } else {
                        $("#AttendenceSummary_8").css("display", "none");
                        $("#chk_8").prop("checked", false);

                    }
                    if (widgetArrays.includes(9)) {
                        $("#OverTime_9").css("display", "block");
                        $("#chk_9").prop("checked", true);
                        Attendence_Overview();

                    } else {
                        $("#OverTime_9").css("display", "none");
                        $("#chk_9").prop("checked", false);
                    }
                    if (widgetArrays.includes(10)) {
                        $("#Holiday_10").css("display", "block");
                        $("#chk_10").prop("checked", true);
                        Fillactivity_HolidayData();

                    } else {
                        $("#Holiday_10").css("display", "none");
                        $("#chk_10").prop("checked", false);
                    }
                    if (widgetArrays.includes(11)) {
                        $("#Genderwise_11").css("display", "block");
                        $("#chk_11").prop("checked", true);
                        GenderWiseChart(7);

                    } else {
                        $("#Genderwise_11").css("display", "none");
                        $("#chk_11").prop("checked", false);
                    }
                    if (widgetArrays.includes(12)) {
                        $("#Attration_12").css("display", "block");
                        $("#chk_12").prop("checked", true);
                        AttrationChart();
                    } else {
                        $("#Attration_12").css("display", "none");
                        $("#chk_12").prop("checked", false);
                    }
                    if (!widgetArrays.includes(4) && !widgetArrays.includes(5)) {
                        $("#bothcharts").css("display", "none");
                    }
                    if (widgetArrays.includes(4) && !widgetArrays.includes(5)) {
                        $("#punchtype_4").addClass("w-100");
                    }
                    else {
                        $("#punchtype_4").removeClass("w-100");
                    }
                    if (!widgetArrays.includes(4) && widgetArrays.includes(5)) {
                        $("#pp_5").addClass("w-100");
                    }
                    else {
                        $("#pp_5").removeClass("w-100");
                    }
                }

                sessionStorage.setItem("WidgetId", result);
                $('#customwidget_modal').modal('hide');

            },
            error: function () {
                console.log("Error in WidgetSection.");
            }
        });



    });





    //Data for branch dropdown

    function fillBranchbycompnyid(companyid, selectedBranch) {
        $('#BranchID').empty();
        $.ajax({
            type: "GET",
            url: "/MasterData/GetBranchbycomapnyid?CompanyID=" + companyid,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {

                var markup = '';

                // Sort data once, outside of the loop
                data.sort(function (a, b) {
                    return a.BranchName.localeCompare(b.BranchName);
                });

                // Loop through data
                for (var x = 0; x < data.length; x++) {
                    if (selectedBranch && selectedBranch.length !== 0) {
                        var re = new RegExp("(^|,)" + data[x].BranchId + "($|,)");
                        if (re.test(selectedBranch)) {
                            markup += '<option selected="selected" value="' + data[x].BranchId + '">' + data[x].BranchName + '</option>';
                        } else {
                            markup += '<option value="' + data[x].BranchId + '">' + data[x].BranchName + '</option>';
                        }
                    } else {
                        markup += '<option value="' + data[x].BranchId + '">' + data[x].BranchName + '</option>';
                    }
                }

                $('#BranchID').append(markup);
                $('#BranchID').multiselect('rebuild');
            },
            error: function (jqXHR) {
                if (jqXHR.status === 500) {
                    toastr.error("InternalServerError");
                } else if (jqXHR.status === 400) {
                    toastr.error("BadRequest");
                } else if (jqXHR.status === 401) {
                    window.location.href = "/PayTime/LoginPage";
                }
            }
        });
    }

    //Quick search dropdown--
    function fillBranchbycompnyidquick(companyid, selectbranch) {

        $('#BranchIdNew').empty();
        $.ajax(
            {
                type: "GET",
                url: "/MasterData/GetBranchbycomapny",
                data: { comapanyid: companyid },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function (xhr) {
                    $('.loading').show();
                },
                success: function (data) {
                    if (data.isRedirect) {
                        window.location.href = data.redirectUrl;
                    }
                    $('.loading').hide();
                    var markup = '';
                    for (var x = 0; x < data.length; x++) {
                        markup += "<option value=" + data[x].BranchId + ">" + data[x].BranchName + "</option>";
                    }
                    var newOption = '<option selected="selected"  value="">Select Branch</option>';
                    $('#BranchIdNew').html(newOption);
                    $('#BranchIdNew').append(markup);
                    $('#BranchIdNew').val(selectbranch);
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
            });
    }
    //for quick search
    $('#quichsearching').on('click', function () {
      
        var _CompanyId = $("#CompanyIDNew option:selected").text();
        if (_CompanyId == "Select Company" || _CompanyId == "") {
            toastr.remove();
            toastr.error("Please select company.");
            $("#CompanyIDNew").addClass('field_error');
            event.preventDefault();//or return false
            $(".loading").hide();
            return false;
        }
        else {
            $("#CompanyIDNew").removeClass('field_error');
        }
        var _BranchId = $("#BranchIdNew option:selected").text();
        if (_BranchId == "Select Branch" || _BranchId == "") {
            toastr.remove();
            toastr.error("Please select Branch.");
            $("#BranchIdNew").addClass('field_error');
            event.preventDefault();//or return false
            $(".loading").hide();
            return false;
        }
        else {
            $("#BranchIdNew").removeClass('field_error');
        }

        cmpIds = $('#CompanyIDNew').val();
        branchIds = $('#BranchIdNew').val();

        if (cmpId != cmpIds) {
            cmpId = cmpIds;
        }
        if (branchId != branchIds) {
            branchId = branchIds;
        }


        IntialpageLoad();

    })

    $("#Clear_OPration").on('click', function () {
        window.location.reload();
    })


    function Fillactivity_Checkin() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/GetActivityMonitorCounts`,
            data: { cmpId: cmpId, branchId: branchId, reqid: 1 },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {

                if (data.isRedirect) {
                    window.location.href = data.redirectUrl;
                } else {
                    $("#checkinCount").text(data[0].Rescnt);

                    employeePresent = data[0].Rescnt;
                }
            },
            error: function () {
                console.log("Error in Checkin counts.");
            }
        });

    }
    function Fillactivity_NotCheckin() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/GetActivityMonitorCounts`,
            data: { cmpId: cmpId, branchId: branchId, reqid: 6 },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {


                if (data.isRedirect) {
                    window.location.href = data.redirectUrl;
                } else {
                    $("#notcheckinCount").text(data[0].Rescnt);
                    employeeAbsent = data[0].Rescnt;
                }
            },
            error: function () {
                console.log("Error in notcheckin counts.");
            }
        });

    }
    function Fillactivity_Birthday() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/GetActivityMonitorCounts`,
            data: { cmpId: cmpId, branchId: branchId, reqid: 2 },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {

                if (data.isRedirect) {
                    window.location.href = data.redirectUrl;
                } else {
                    $("#birthdayCount").text(data[0].Rescnt);
                }
            },
            error: function () {
                console.log("Error in birthday counts.");
            }
        });

    }
    function Fillactivity_Latein() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/GetActivityMonitorCounts`,
            data: { cmpId: cmpId, branchId: branchId, reqid: 3 },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {

                if (data.isRedirect) {
                    window.location.href = data.redirectUrl;
                } else {
                    $("#lateinCount").text(data[0].Rescnt);
                }
            },
            error: function () {
                console.log("Error in latein counts.");
            }
        });

    }
    function Fillactivity_EarlyOut() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/GetActivityMonitorCounts`,
            data: { cmpId: cmpId, branchId: branchId, reqid: 4 },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {

                if (data.isRedirect) {
                    window.location.href = data.redirectUrl;
                } else {
                    $("#earlyOutCount").text(data[0].Rescnt);
                }
            },
            error: function () {
                console.log("Error in earlyout counts.");
            }
        });

    }
    function Fillactivity_DeviceCount() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/GetActivityMonitorCounts`,
            data: { cmpId: cmpId, branchId: branchId, reqid: 5 },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {

                if (data.isRedirect) {
                    window.location.href = data.redirectUrl;
                } else {
                    $("#deviceCount").text(data[0].Rescnt);
                }
            },
            error: function () {
                console.log("Error in device counts.");
            }
        });

    }

    /* get data for attendence/leave/webpunch counts*/
    function Getpendingleave_count() {
        $.ajax({
            type: "POST",
            url: `${webApiUrl}/DashBoard/GetEmployeePendingCount?roleid=${emprole}&companyid=${cmpId}&branchid=${branchId}&empId=${empid}`,
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
                toastr.error("Error in Pending approval counts");
                //setTimeout(() => {
                //    window.location.href = "/PayTime/LoginPage";
                //}, 2000);
            }
        });




    }


    /*  get data for genderwise chart*/
    function GenderWiseChart(reqid) {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/GetActivityMonitorCounts`,
            data: { cmpId: cmpId, branchId: branchId, reqid: reqid },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {
                
                if (data == null || data == "") {
                    $("#gender-empty").show();
                    $("#genderwise-charts1").hide();
                }
                else {
                    $("#gender-empty").hide();
                    $("#genderwise-charts1").show();
                    if (data.isRedirect) {
                        window.location.href = data.redirectUrl;
                    } else {

                        let branchid;
                        let branchNames;
                        if (reqid == 7) {
                            branchid = data.map(item => item.branchid);
                            branchNames = data.map(item => item.branchname);
                        }
                        else {
                            branchid = data.map(item => item.DepartmentId);
                            branchNames = data.map(item => item.DepartmentName);
                        }
                       
                        const maleCounts = data.map(item => item.male_count);
                        const femaleCounts = data.map(item => item.female_count);
                        const PresentmaleCounts = data.map(item => item.atworkMale);
                        const PresentfemaleCounts = data.map(item => item.atworkFemale);
                        const AbsentmaleCounts = data.map(item => item.AbsentMale);
                        const AbsentfemaleCounts = data.map(item => item.AbsentFemale);

                        // Call the function to render the chart
                        renderGenderWiseChart(branchNames, branchid, PresentmaleCounts, PresentfemaleCounts, AbsentmaleCounts,AbsentfemaleCounts,reqid);
                    }
                }
            },
            error: function () {
                console.log("Error in GenderWiseChart.");
            }
        });
    }

    //get data for present patterns charts

    function PresentPattern() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/GetActivityMonitorCounts`,
            data: { cmpId: cmpId, branchId: branchId, reqid: 12 },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {
                if (!data || Object.keys(data).length === 0 || (data[0].Last7DaysPercentage == null && data[0].Last30DaysPercentage == null)) {
                    $("#presentchart-empty").show();
                    $("#presentchart-data").hide();
                }
                else {
                    $("#presentchart-empty").hide();
                    $("#presentchart-data").show();
                    if (data.isRedirect) {
                        window.location.href = data.redirectUrl;
                    } else {
                        renderPresentpattchart(data);
                    }
                }
            },
            error: function () {
                console.log("Error in Present pattern counts.");
            }
        });

    }
    $("#branchWisegender").click(function () {

      
        GenderWiseChart(7);


        $(this).parent().addClass("active").siblings().removeClass("active");
    });
    $("#departwisegender").click(function () {

    
        GenderWiseChart(16);


        $(this).parent().addClass("active").siblings().removeClass("active");
    });
    $("#branchWise").click(function () {
       
       
        Getattendencesummary(8); 

      
        $(this).parent().addClass("active").siblings().removeClass("active");
    });

    $("#departwise").click(function () {
       
        $("#Filtername").text("Department");
        Getattendencesummary(15); 

       
        $(this).parent().addClass("active").siblings().removeClass("active");
    });


    /* get data for attendencesummary*/
    function Getattendencesummary(reqid) {

        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/GetActivityMonitorCounts`,
            data: { cmpId: cmpId, branchId: branchId, reqid: reqid },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {
                if (data == null || data == "") {
                    $("#summary-empty").show();
                    $("#summary-details").hide();
                } else {
                    $("#summary-empty").hide();
                    $("#summary-details").show();
                    if (data.isRedirect) {
                        window.location.href = data.redirectUrl;
                    } else {
                        $('#attendancesummary_tbl tbody').empty();
                        data.forEach(item => {
                            const newRow = `<tr>
                                    <td style="text-align:left;">${reqid === 8 ? item.branchname : item.departmentname}</td>
                                    <td class="green_font">${item.atwork_count}</td>
                                    <td class="red_font">${item.notcheckedin_count}</td>
                                </tr>`;
                            $('#attendancesummary_tbl tbody').append(newRow);
                        });

                        /*document.getElementById("curdates").textContent = currentdate;*/
                    }
                }
            },
            error: function () {
                console.log("Error in Attendencesummary.");
            }
        });

    }


    //get holidays data
    function Fillactivity_HolidayData() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/GetActivityMonitorCounts`,
            data: { cmpId: cmpId, branchId: branchId, reqid: 10 },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {
                if (data == null || data == "") {
                    $("#holiday-empty").show();
                    $("#holiday-data").hide();

                }
                else {
                    $("#holiday-empty").hide();
                    $("#holiday-data").show();
                    if (data.isRedirect) {
                        window.location.href = data.redirectUrl;
                    } else {
                        const holidayContainer = $("#Main_holiday");
                        holidayContainer.empty(); // Clear previous holidays if any

                        data.forEach(holiday => {
                            const holidayHtml = `
                        
                          <div class="holiday_detail pr_detail">
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
                console.log("Error in Holiday counts.");
            }
        });

    }

    /*  get data for punch type data*/
    function GetPunchData() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/GetActivityMonitorCounts`,
            data: { cmpId: cmpId, branchId: branchId, reqid: 9 },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {
              
                if (data == null || data == "" || data.every(item => item.empid === 0))  {
                    $("#punchchart-empty").show();
                    $("#punchcharts-data").hide();
                }
                else {
                    $("#punchchart-empty").hide();
                    $("#punchcharts-data").show();
                    // Set the current date in the desired format
                    const date = new Date();
                    const day = date.getDate();
                    const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                    const month = monthNames[date.getMonth()];
                   /* document.getElementById("curdate").textContent = `${day}-${month}`;*/
                    currentdate = `${day}-${month}`;
                    if (data.isRedirect) {
                        window.location.href = data.redirectUrl;
                    } else {
                        const entryModeLabels = {
                            0: 'Manual',
                            1: 'Device',
                            2: 'Web',
                            3: 'Face'
                        };
                        let punchData = [
                            { name: 'Manual', y: 0 },
                            { name: 'Device', y: 0 },
                            { name: 'Web', y: 0 },
                            { name: 'Face', y: 0 }
                        ];

                        punchData = data.map(item => ({
                            name: entryModeLabels[item.entrymode] || 'Unknown',
                            y: item["empid"] || 0
                        }));
                        //In activity monitor to show date
                       /* $("#Todays_date").text('Today');*/
                        renderPunchTypeChart(punchData);
                    }
                }
            },
            error: function () {
                console.log("Error in GetPunchData.");
            }
        });

    }

    /* get data for Leave Bifurcation */

    const currentDate = new Date();
    const currentMonthName = currentDate.toLocaleString('default', { month: 'long' });

    // Set the current month as active in the dropdown and display it
    $("#monthdropdown .dropdown-menu li").each(function () {
        if ($(this).text().trim() === currentMonthName) {
            $(this).addClass("active");
        }
    });
    $("#_curemonth").text(currentMonthName);

    // Call the function with the current month
    selectMonth(currentMonthName);

    $("#monthdropdown .dropdown-menu a").click(function () {

        var selectedMonth = $(this).text();
        $("#monthdropdown .dropdown-menu li").removeClass("active");
        $(this).parent().addClass("active");
        $("#_curemonth").text(selectedMonth);
        selectMonth(selectedMonth);
    });
    function selectMonth(monthName) {

        const monthMap = {
            "January": 1,
            "February": 2,
            "March": 3,
            "April": 4,
            "May": 5,
            "June": 6,
            "July": 7,
            "August": 8,
            "September": 9,
            "October": 10,
            "November": 11,
            "December": 12
        };

        const monthNumber = monthMap[monthName];
        sendMonthData(monthNumber);
        $("#_curemonth").text(monthName);
    }

    function sendMonthData(month) {

        Leavebifuractaion(month);
    }
    function Leavebifuractaion(month) {

        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/LeaveBifurcation`,
            data: { cmpId: cmpId, branchId: branchId, reqid: 13, month: month },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {
                if (data == null || data == "") {
                    $("#leave-empty").show();
                    $("#chartdiv").hide();
                }
                else {
                    $("#leave-empty").hide();
                    $("#chartdiv").show();

                    if (data.isRedirect) {
                        window.location.href = data.redirectUrl;
                    } else {
                        leaveBifurcationChart(data, month);
                    }
                }
            },
            error: function () {
                console.log("Error in Leavebifurcation.");
            }
        });

    }
    $("#close_model").on('click', function () {
        document.getElementById("Savedata").disabled = false;
    });
    //Data send In announcement tables

    $("#Savedata").on('click', function () {
        document.getElementById("Savedata").disabled = true;

        if (commonvalidation(event)) {
            annoucement_setData();
        }

    });

    function annoucement_setData() {
        $("#ajax_loader").show();

        var companyid = companyIds;
        var branchid = branchIds;

        var startdate = $('#startDate').val();
        var enddate = $('#endDate').val();
        var title = $('#title').val();
        var description = $('#descriptions').val();
        var emailnotification = $('#emailnoti').prop('checked') ? 1 : 0;
        var smsnotification = $('#smsnoti').prop('checked') ? 1 : 0;

        function convertToYYYYMMDD(dateString) {
            // If the date is already in 'YYYY-MM-DD' format, return it as is
            if (/^\d{4}-\d{2}-\d{2}$/.test(dateString)) return dateString;

            // If the date is in 'DD-MM-YYYY' format
            if (/^\d{2}-\d{2}-\d{4}$/.test(dateString)) {
                var parts = dateString.split("-");
                return `${parts[2]}-${parts[1]}-${parts[0]}`;
            }

            // If the date is in 'MMM-DD-YYYY' format (e.g., Dec-11-2024)
            if (/^[A-Za-z]{3}-\d{2}-\d{4}$/.test(dateString)) {
                var parts = dateString.split("-");
                var monthNames = {
                    Jan: "01", Feb: "02", Mar: "03", Apr: "04", May: "05", Jun: "06",
                    Jul: "07", Aug: "08", Sep: "09", Oct: "10", Nov: "11", Dec: "12"
                };
                var month = monthNames[parts[0]]; // Convert month name to number
                return `${parts[2]}-${month}-${parts[1]}`;
            }

            // Default: Return the original string if format is not recognized
            return dateString;
        }

        startdate = convertToYYYYMMDD(startdate);
        enddate = convertToYYYYMMDD(enddate);


        const data = {
            CompanyId: companyid,
            BranchId: branchid,
            StartDate: startdate,
            EndDate: enddate,
            Title: title,
            Description: description,
            EmailNotification: emailnotification,
            SmsNotification: smsnotification,
            reqid: 0,
        };

        $.ajax({
            type: "POST",
            //url: `${webApiUrl}DashBoard/SetAnnoucementdata`,
            url: `${webApiUrl}DashBoard/SetAnnoucementdata`,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(data),
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {
               
                if (data.isRedirect) {
                    window.location.href = data.redirectUrl;
                } else {

                    $("#addanoucement_modal").modal('hide');
                   
                    toastr.success("Annoucement Successfully Saved");
                   
                    $('#CompanyID').multiselect('clearSelection');
                    $('#BranchID').multiselect('clearSelection');
                    $('#DepartmentID').multiselect('clearSelection');
                    location.reload();
                    document.getElementById("Savedata").disabled = false;
                    $("#ajax_loader").hide();
                }
            },
            error: function () {
                console.log("Error in annocement saving");
                toastr.error("Error! Annoucement not Saved yet.");
                document.getElementById("Savedata").disabled = false;
                $("#ajax_loader").hide();
            }
        })
    }

    

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
                }
                else {
                    $("#annoucement-empty").hide();
                    $("#annoucement-data").show();
                    if (data.isRedirect) {
                        window.location.href = data.redirectUrl;
                    } else {
                        let availableSlides = data.length;

                        for (let i = 0; i < availableSlides; i++) {
                            if (data[i]) {
                                document.getElementById(`annouce_title_${i + 1}`).textContent = data[i].annoucementtitle;
                                document.getElementById(`annouce_des_${i + 1}`).textContent = data[i].Description;
                                $(`#annouce_title_${i + 1}`).css("visibility", "visible");
                                $(`#annouce_des_${i + 1}`).css("visibility", "visible");
                                $(`#dot_${i + 1}`).css("visibility", "visible");
                            }
                        }

                        for (let i = availableSlides; i < 3; i++) {
                            $(`#annouce_title_${i + 1}`).css("display", "none");
                            $(`#annouce_des_${i + 1}`).css("display", "none");
                            $(`#dot_${i + 1}`).css("display", "none");
                        }

                        // Activate the first slide (or whichever you want to display first)
                        activateSlide(0);
                    }
                }
            },
            error: function () {
                console.log("Error in annoucement data");
            }
        });
    }

    function activateSlide(index) {
       
        $(".announcement-slide").removeClass("active");
        $(".dot").removeClass("active");

       
        $(`.announcement-slide:eq(${index})`).addClass("active");
        $(`#dot_${index + 1}`).addClass("active");
    }
    
    let currentSlide = 0;
    const totalSlides = $(".announcement-slide").length;

    //setInterval(function () {
    //    currentSlide = (currentSlide + 1) % totalSlides;  // Cycle through slides
    //    activateSlide(currentSlide);
    //}, 3000);  // 5 seconds delay

    // Dot navigation
    $(".dot").on("click", function () {
        const index = $(this).index();
        activateSlide(index);
    });


    /*get data for present pattern chart */
    function renderPresentpattchart(ppdata) {
        function renderIcons() {

            const chart = this;
            chart.series.forEach(series => {
                if (!series.icon) {
                    // Create a div element for the icon
                    const iconDiv = document.createElement('div');
                    iconDiv.innerHTML = '<i class="fa fa-${series.options.custom.icon}"></i>';
                    //iconDiv.style.color = series.options.custom.iconColor;
                    iconDiv.style.fontSize = '1.5em';
                    iconDiv.style.position = 'absolute';
                    iconDiv.style.zIndex = '10';

                    chart.container.parentNode.appendChild(iconDiv);
                    series.icon = iconDiv; // Store the icon reference in the series
                }

                // Position the icon relative to the chart dimensions
                const point = series.points[0].shapeArgs;
                series.icon.style.left = '${chart.plotLeft + chart.plotWidth / 2 - 15}px';
                series.icon.style.top = '${chart.plotTop + chart.plotHeight / 2 - point.innerR - (point.r - point.innerR) / 2 + 8}px';
            });
        }

        const trackColors = Highcharts.getOptions().colors.map(color =>
            new Highcharts.Color(color).setOpacity(0.3).get()
        );

        Highcharts.chart('Presentpattern_chart', {
            chart: {
                type: 'solidgauge',
               /* width: 220,  // Fixed width*/
                height: 320, // Fixed height
                marginTop: 20, // Set top margin to 6px
                style: {
                    fontFamily: 'Roboto', // Set font family for y-axis labels
                },
                events: {
                    render: renderIcons,
                    load: function () {
                        const chart = this;
                        chart.tooltip.refresh(chart.series[0].points[0]); // Show "Weekly" tooltip on load
                    }
                }
            },
            title: {
                text: '',
                align: 'left',
                style: {
                    fontSize: '24px'
                }
            },
            tooltip: {
                borderWidth: 0,
                backgroundColor: 'none',
                shadow: false,
                style: {
                    fontSize: '12px',
                },
                valueSuffix: '%',
                pointFormat: '{series.name}<br>' + '<span style="font-size: 16px; color: {point.color}; ' + 'font-weight: bold">{point.y}</span>',
                positioner: function (labelWidth) {
                    return {
                        x: (this.chart.chartWidth - labelWidth) / 2,
                        y: 90
                    };
                }
            },
            pane: {
                startAngle: 0,
                endAngle: 360,
                background: [{
                    outerRadius: '112%', innerRadius: '88%', backgroundColor: '#e7eaed', borderWidth: 0
                }, {
                    outerRadius: '87%', innerRadius: '63%', backgroundColor: '#c6cde3', borderWidth: 0
                }
                ]
            },
            yAxis: {

                min: 0,
                max: 100,
                lineWidth: 0,
                tickPositions: []
            },
            plotOptions: {
                solidgauge: {
                    dataLabels: {
                        enabled: false
                    },
                    linecap: 'round',
                    stickyTracking: false,
                    rounded: true
                }
            },
            legend: {
                enabled: true,
                layout: 'horizontal',
                align: 'center',
                verticalAlign: 'bottom',
                width: 100,
                itemMarginTop: 5,
                itemMarginBottom: 5,
                symbolRadius: 5, // Round symbols to match the look of points
                symbolWidth: 5,
                itemStyle: {
                    fontSize: '13px',
                    fontWeight: 'normal',
                    color: '#333333',
                }
            },
            series: [{
                name: 'Weekly',
                data: [{
                    color: '#b6bec7',
                    radius: '112%',
                    innerRadius: '88%',
                    y: parseFloat(ppdata[0].Last7DaysPercentage.toFixed(2))
                }],
                showInLegend: true
            }, {
                name: 'Monthly',
                data: [{
                    color: '#8399d9',
                    radius: '87%',
                    innerRadius: '63%',
                    y: parseFloat(ppdata[0].Last30DaysPercentage.toFixed(2))
                }],
                showInLegend: true
            }
                //,
                //{
                //name: 'Yearly',
                //data: [{
                //    color: '#8ed8b7',
                //    radius: '62%',
                //    innerRadius: '38%',
                //    y: 6
                //}],
                //showInLegend: true
                //}
            ]

        });
    }
    /* get data for Attendence Overview*/
    function Attendence_Overview() {
      
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/GetActivityMonitorCounts`,
            data: { cmpId: cmpId, branchId: branchId, reqid: 11 },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {
               
                const OtHours = data.map(day => day.OTHr);
                if (data == null || data == "") {
                    $("#attendence-empty").show();
                    $("#attendence-overview").hide();
                }
                else {
                    $("#attendence-empty").hide();
                    $("#attendence-overview").show();
                }
                if (data.isRedirect) {
                    window.location.href = data.redirectUrl;
                } else {
                    if (OtHours == null || OtHours == "") {
                        $("#overtime-empty").show();
                        $("#ot_charts1").hide();
                    } else {
                        $("#overtime-empty").hide();
                        $("#ot_charts1").show();
                    }
                  
                    var tot_strength = employeePresent + employeeAbsent;
                /*    document.getElementById("Currdate").textContent = currentdate;*/
                    document.getElementById("Tot_strength").textContent = tot_strength || 0;
                    const lastItem = data[data.length - 1] || {};

                    // Use optional chaining and default values for the counts
                    document.getElementById("present_counts").textContent = lastItem.PresentCount || 0;
                    document.getElementById("absent_counts").textContent = lastItem.AbsentCount || 0;
                    document.getElementById("leave_counts").textContent = lastItem.ODCount || 0;

                    renderAttendeceChart(data);
                    renderOThrscharts(data);

                }
            },
            error: function () {
                console.log("Error in AttendanceOverview Chart.");
            }
        });

    }

    /*  get data for Attration chart*/
    function AttrationChart() {
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/GetActivityMonitorCounts`,
            data: { cmpId: cmpId, branchId: branchId, reqid: 14 },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {
                if (data == null || data == "") {
                    $("#attration-empty").show();
                    $("#attration-charts1").hide();
                } else {
                    $("#attration-empty").hide();
                    $("#attration-charts1").show();
                }

                if (data.isRedirect) {
                    window.location.href = data.redirectUrl;
                } else {

                   
                    const currentMonth = new Date().getMonth();
                    const lastThreeMonths = [
                        (currentMonth - 2 + 12) % 12,
                        (currentMonth - 1 + 12) % 12,
                        currentMonth
                    ];


                    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                    const selectedMonths = lastThreeMonths.map(index => months[index]);


                    const maleCounts = Array(3).fill(0);
                    const femaleCounts = Array(3).fill(0);


                    data.forEach(item => {
                        const monthIndex = item.resign_month - 1;
                        const targetIndex = lastThreeMonths.indexOf(monthIndex);
                        if (targetIndex !== -1) {
                            maleCounts[targetIndex] = item.male_count;
                            femaleCounts[targetIndex] = item.female_count;
                        }
                    });


                    renderAttrationChart(selectedMonths, maleCounts, femaleCounts, 0);

                }
            },
            error: function () {
                console.log("Error in AttrationChart.");
            }
        });
    }


    //----------------------------------------------------Details of modal start------------------------------------------------------------
    $("#checkinbtn").on('click', function () {
        var totalCount = parseInt($("#checkinCount").text(), 10);
        if (!isNaN(totalCount) && totalCount > 0) {
            $("#checkinbtn").attr("data-toggle", "modal");
            $("#checkinbtn").attr("data-target", "#checkinbtn_modal");
            $('#checkin_table').show();
            $('#notcheckedin_tbl').hide();
            $('#lateclockin_tbl').hide();
            $('#earlyclockout_tbl').hide();
            $('#birthday_tbl').hide();
            $('#device_tbl').hide();
            checkinDetails();
        }
      
        
        
    });
    function checkinDetails() {
        $.ajax({
            url: `${webApiUrl}DashBoard/GetAdminDashBoardDetails`,
            type: "GET",
            data: { cmpId: cmpId, branchId: branchId, reqid: 1 },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (response) {

                if (response != null && response != "") {
                    if (response.isRedirect) {
                        window.location.href = response.redirectUrl;
                        return;
                    }


                    if (Array.isArray(response)) {
                        response.forEach(function (item) {
                            const dateParts = item.currentdate.split('T')[0]; 
                            const [year, month, day] = dateParts.split('-'); 
                            item.currentdate = `${day}-${month}-${year}`; 
                        });


                        if ($.fn.dataTable.isDataTable('#checkin_table')) {
                            $('#checkin_table').DataTable().clear().destroy();
                        }


                        $('#checkin_table').DataTable({
                            data: response,
                            "dom": 'lBfrtip',
                            "bDestroy": true,
                            "buttons": [
                                {
                                    extend: 'excelHtml5',
                                    filename: function () {

                                        const currentDate = response[0].currentdate;
                                        return `checkedin-${currentDate}`;
                                    },
                                    title: 'Checkin Details',
                                    exportOptions: {

                                        columns: [0, 1, 2, 3, 4, 5, 6,7]
                                    },
                                }
                            ],
                            "columns": [
                                { "data": "Empcode" },
                                { "data": "EmpName" },
                                { "data": "emppunchid" },
                                { "data": "departmentname" },
                                { "data": "designationname" },
                                { "data": "ShiftName" },
                                { "data": "Mode" },
                                { "data": "InTime" },
                                { "data": "currentdate" }
                            ]
                        });


                        var customDiv = $('<div class="customforms_table"></div>');
                        customDiv.insertBefore("#checkin_table_wrapper > .dataTables_info");
                        $("#checkin_table_wrapper > table").appendTo(customDiv);
                    } else {
                        console.error("Data is not in the expected format:", response);
                    }
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data:", error);
            }
        });
    }
    $("#birthdaybtn").on('click', function () {

        var totalCount1 = parseInt($("#birthdayCount").text(), 10);
        if (!isNaN(totalCount1) && totalCount1 > 0) {
            $("#birthdaybtn").attr("data-toggle", "modal");
            $("#birthdaybtn").attr("data-target", "#birthday_modal");
            $('#checkin_table').hide();
            $('#notcheckedin_tbl').hide();
            $('#lateclockin_tbl').hide();
            $('#earlyclockout_tbl').hide();
            $('#birthday_tbl').show();
            $('#device_tbl').hide();
            BirthdayDetails();

        }
       
    });
    //details for birthday
    function BirthdayDetails() {
       

            $.ajax({
                url: `${webApiUrl}DashBoard/GetAdminDashBoardDetails`,
                type: "GET",
                data: { cmpId: cmpId, branchId: branchId, reqid: 2 },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Authorization", token);
                },
                success: function (response) {
                    if (response != null && response != "") {



                        if (response.isRedirect) {
                            window.location.href = response.redirectUrl;
                            return;
                        }


                        if (Array.isArray(response)) {
                            response.forEach(function (item) {
                                const dateParts = item.currentdate.split('T')[0];
                                const [year, month, day] = dateParts.split('-');
                                item.currentdate = `${day}-${month}-${year}`;
                            });


                            if ($.fn.dataTable.isDataTable('#birthday_tbl')) {
                                $('#birthday_tbl').DataTable().clear().destroy();
                            }


                            $('#birthday_tbl').DataTable({
                                data: response,
                                "dom": 'lBfrtip',
                                "bDestroy": true,
                                "buttons": [
                                    {
                                        extend: 'excelHtml5',
                                        filename: function () {

                                            const currentDate = response[0].currentdate;
                                            return `Birthday-${currentDate}`;
                                        },
                                        title: 'Birthday Details',
                                        exportOptions: {

                                            columns: [0, 1, 2, 3, 4]
                                        },
                                    }
                                ],
                                "columns": [
                                    { "data": "empcode" },
                                    { "data": "EmpName" },
                                    { "data": "emppunchid" },
                                    { "data": "DepartmentName" },
                                    { "data": "designationname" },
                                    { "data": "currentdate" }
                                ]
                            });


                            var customDiv = $('<div class="customforms_table"></div>');
                            customDiv.insertBefore("#birthday_tbl_wrapper > .dataTables_info");
                            $("#birthday_tbl_wrapper > table").appendTo(customDiv);
                        } else {
                            console.error("Data is not in the expected format:", response);
                        }
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error fetching data:", error);
                }
            });
       

    }
    //details for late in
    $("#lateclockinbtn").on('click', function () {

        var totalCount2 = parseInt($("#lateinCount").text(), 10);
        if (!isNaN(totalCount2) && totalCount2 > 0) {
            $("#lateclockinbtn").attr("data-toggle", "modal");
            $("#lateclockinbtn").attr("data-target", "#lateclockin_modal");
            $('#checkin_table').hide();
            $('#notcheckedin_tbl').hide();
            $('#lateclockin_tbl').show();
            $('#earlyclockout_tbl').hide();
            $('#birthday_tbl').hide();
            $('#device_tbl').hide();

            LateinDetails();
        }

       

    });

    function LateinDetails() {
        $.ajax({
            url: `${webApiUrl}DashBoard/GetAdminDashBoardDetails`,
            type: "GET",
            data: { cmpId: cmpId, branchId: branchId, reqid: 3 },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (response) {
                if (response != null && response != "") {


                    if (response.isRedirect) {
                        window.location.href = response.redirectUrl;
                        return;
                    }


                    if (Array.isArray(response)) {
                        response.forEach(function (item) {
                            const dateParts = item.currentdate.split('T')[0];
                            const [year, month, day] = dateParts.split('-');
                            item.currentdate = `${day}-${month}-${year}`;
                        });


                        if ($.fn.dataTable.isDataTable('#lateclockin_tbl')) {
                            $('#lateclockin_tbl').DataTable().clear().destroy();
                        }


                        $('#lateclockin_tbl').DataTable({
                            data: response,
                            "dom": 'lBfrtip',
                            "bDestroy": true,
                            "buttons": [
                                {
                                    extend: 'excelHtml5',
                                    filename: function () {

                                        const currentDate = response[0].currentdate;
                                        return `Lateclockin-${currentDate}`;
                                    },
                                    title: 'Lateclockin Details',
                                    exportOptions: {

                                        columns: [ 1, 2, 3, 4, 5, 6,7,8,9]
                                    },
                                }
                            ],
                            "columns": [
                                { "data": "EmployeeID" },
                                { "data": "EmpName" },
                                { "data": "EmpPunchID" },
                                { "data": "DepartmentName" },
                                { "data": "DesignationName" },
                                { "data": "shiftname" },
                                { "data": "ShiftStartTime" },
                                { "data": "ShiftEndTime" },
                                { "data": "SubqueryInTime" },
                                { "data": "LateInMins" },
                                { "data": "currentdate" }
                            ]
                        });


                        var customDiv = $('<div class="customforms_table"></div>');
                        customDiv.insertBefore("#lateclockin_tbl_wrapper > .dataTables_info");
                        $("#lateclockin_tbl_wrapper > table").appendTo(customDiv);
                    } else {
                        console.error("Data is not in the expected format:", response);
                    }
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data:", error);
            }
        });
    }

    //Early out counts
    $("#earlyclockoutbtn").on('click', function () {

        var totalCount3 = parseInt($("#earlyOutCount").text(), 10);
        if (!isNaN(totalCount3) && totalCount3 > 0) {
            $("#earlyclockoutbtn").attr("data-toggle", "modal");
            $("#earlyclockoutbtn").attr("data-target", "#earlyclockout_modal");
            $('#checkin_table').hide();
            $('#notcheckedin_tbl').hide();
            $('#lateclockin_tbl').hide();
            $('#earlyclockout_tbl').show();
            $('#birthday_tbl').hide();
            $('#device_tbl').hide();

            EarlyOutDetails();
        }

       

    });

    function EarlyOutDetails() {
        $.ajax({
            url: `${webApiUrl}DashBoard/GetAdminDashBoardDetails`,
            type: "GET",
            data: { cmpId: cmpId, branchId: branchId, reqid: 4 },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (response) {

                if (response != null && response != "") {



                    if (response.isRedirect) {
                        window.location.href = response.redirectUrl;
                        return;
                    }


                    if (Array.isArray(response)) {
                        response.forEach(function (item) {
                            const dateParts = item.currentdate.split('T')[0];
                            const [year, month, day] = dateParts.split('-');
                            item.currentdate = `${day}-${month}-${year}`;
                        });


                        if ($.fn.dataTable.isDataTable('#earlyclockout_tbl')) {
                            $('#lateclockin_tbl').DataTable().clear().destroy();
                        }


                        $('#earlyclockout_tbl').DataTable({
                            data: response,
                            "dom": 'lBfrtip',
                            "bDestroy": true,
                            "buttons": [
                                {
                                    extend: 'excelHtml5',
                                    filename: function () {

                                        const currentDate = response[0].currentdate;
                                        return `Earlyclockout-${currentDate}`;
                                    },
                                    title: 'Earlyclockout Details',
                                    exportOptions: {

                                        columns: [1, 2, 3, 4, 5, 6,7,8,9]
                                    },
                                }
                            ],
                            "columns": [
                                { "data": "EmployeeID" },
                                { "data": "EmpName" },
                                { "data": "EmpPunchID" },
                                { "data": "DepartmentName" },
                                { "data": "DesignationName" },
                                { "data": "shiftname" },
                                { "data": "ShiftStartTime" },
                                { "data": "ShiftEndTime" },
                                { "data": "OutTime" },
                                { "data": "EarlyOutMin" },
                                { "data": "currentdate" }
                            ]
                        });


                        var customDiv = $('<div class="customforms_table"></div>');
                        customDiv.insertBefore("#earlyclockout_tbl_wrapper > .dataTables_info");
                        $("#earlyclockout_tbl_wrapper > table").appendTo(customDiv);
                    } else {
                        console.error("Data is not in the expected format:", response);
                    }
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data:", error);
            }
        });

    }
    //device counts
    $("#devicebtn").on('click', function () {
        var totalCount4 = parseInt($("#deviceCount").text(), 10);
        if (!isNaN(totalCount4) && totalCount4 > 0) {
            $("#devicebtn").attr("data-toggle", "modal");
            $("#devicebtn").attr("data-target", "#device_modal");
            $('#checkin_table').hide();
            $('#notcheckedin_tbl').hide();
            $('#lateclockin_tbl').hide();
            $('#earlyclockout_tbl').hide();
            $('#birthday_tbl').hide();
            $('#device_tbl').show();
            deviceDeatails();
        }
      
    });
       
    function deviceDeatails() {
        $.ajax({
            url: `${webApiUrl}DashBoard/GetAdminDashBoardDetails`,
            type: "GET",
            data: { cmpId: cmpId, branchId: branchId, reqid: 5 },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (response) {

                if (response != null && response != "") {


                    if (response.isRedirect) {
                        window.location.href = response.redirectUrl;
                        return;
                    }


                    if (Array.isArray(response)) {
                        //response.forEach(function (item) {
                        //    const date = new Date(item.currentdate);
                        //    item.currentdate = date.toISOString().split('T')[0];  // Format as YYYY-MM-DD
                        //});


                        if ($.fn.dataTable.isDataTable('#device_tbl')) {
                            $('#device_tbl').DataTable().clear().destroy();
                        }


                        $('#device_tbl').DataTable({
                            data: response,
                            "dom": 'lBfrtip',
                            "bDestroy": true,
                            "buttons": [
                                {
                                    extend: 'excelHtml5',
                                    filename: 'Device',
                                    title: 'Device Details',
                                    exportOptions: {

                                        columns: [0, 1, 2, 3, 4, 5, 6,7]
                                    },
                                }
                            ],
                            "columns": [
                                { "data": "branchname" },
                                { "data": "DeviceName" },
                                { "data": "DeviceSrNo" },
                                { "data": "DeviceCode" },
                                { "data": "DevicePort" },
                                { "data": "DeviceType" },
                                { "data": "LastActivity" },
                                { "data": "DeviceStatus" },
                            ],
                            "createdRow": function (row, data, dataIndex) {
                               
                                var statusCell = $(row).find('td').eq(7); 

                                if (data.DeviceStatus === 'Offline') {
                                    statusCell.css('color', 'red');  
                                } else if (data.DeviceStatus === 'Online') {
                                    statusCell.css('color', 'green'); 
                                }
                                else if (data.DeviceStatus === 'NA') {
                                    statusCell.css('color', 'orange');
                                }
                                else if (data.DeviceStatus === 'standby') {
                                    statusCell.css('color', 'orange');
                                }
                            }
                        });


                        var customDiv = $('<div class="customforms_table"></div>');
                        customDiv.insertBefore("#device_tbl_wrapper > .dataTables_info");
                        $("#device_tbl_wrapper > table").appendTo(customDiv);
                    } else {
                        console.error("Data is not in the expected format:", response);
                    }
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data:", error);
            }
        });

    }
    //genderwise counts details
    function handleChartClick(branchId, gender, flag,reqid1) {
        
        const modalContent = ``;

        // Determine request ID and modal
        var reqid;
        var modelname;

        if (gender === 'Female') {
            if (reqid1 == 7) { // for branch
                if (flag == 0) {
                    reqid = 7;
                }
                else {
                    reqid = 13;
                }
            }
            else {
                if (flag == 0) {
                    reqid = 15;
                }
                else {
                    reqid = 18;
                }
            }
            modelname = '#femalegender_tbl';
            document.getElementById('modalContent').innerHTML = modalContent;
            $('#femalegender_modal').modal('show');

            var ExportExcel = {
                extend: 'excelHtml5',
                filename: function () {


                    return 'FemaleList';
                },
                title: 'FemaleList',
                exportOptions: {

                    columns: [0, 1, 2, 3, 4,5]
                },
            };
        } else {
            if (reqid1 == 7) { // for branch
                if (flag == 0) {
                    reqid = 8;
                }
                else {
                    reqid = 14;
                }
            }
            else {
                if (flag == 0) {
                    reqid = 16;
                }
                else {
                    reqid = 17;
                }
            }
            modelname = '#malegender_tbl';
            document.getElementById('modalContentmale').innerHTML = modalContent;
            $('#malegender_modal').modal('show');
            var ExportExcel = {
                extend: 'excelHtml5',
                filename: function () {


                    return 'MaleList';
                },
                title: 'MaleList',
                exportOptions: {

                    columns: [0, 1, 2, 3, 4, 5]
                },
            };
        }

        $.ajax({
            url: `${webApiUrl}DashBoard/GetAdminDashBoardDetails`,
            type: "GET",
            data: { cmpId: cmpId, branchId: branchId, reqid: reqid },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (response) {



                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                    return;
                }


                if (Array.isArray(response)) {
                    //response.forEach(function (item) {
                    //    const date = new Date(item.currentdate);
                    //    item.currentdate = date.toISOString().split('T')[0];  // Format as YYYY-MM-DD
                    //});


                    if ($.fn.dataTable.isDataTable(modelname)) {
                        $(modelname).DataTable().clear().destroy();
                    }


                    $(modelname).DataTable({
                        data: response,
                        "dom": 'lBfrtip',
                        "scrollX" :true,
                        "bDestroy": true,
                        "buttons": [ExportExcel],
                        "columns": [
                            { "data": "empcode" },
                            { "data": "empname" },
                            { "data": "emppunchid" },
                            { "data": "departmentname" },
                            {
                                "data": "designationname", 
                                "render": function (data, type, row) {
                                    return data ? data : ""; // If data is null or undefined, return "N/A"
                                }
                            },
                            { "data": "shiftname" },
                        ]
                    });


                    var customDiv = $('<div class="customforms_table"></div>');
                    customDiv.insertBefore("#femalegender_tbl_wrapper > .dataTables_info");
                    $("#femalegender_tbl_wrapper > table").appendTo(customDiv);
                } else {
                    console.error("Data is not in the expected format:", response);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data:", error);
            }
        });

    }

    //punchtype chart details
    function handlePunchChartClick(punchtype) {
        

        const modalContent = ``;

        // Determine request ID and modal
        var reqid;
        var modelname;
        //export excel

        const date = new Date();
        const year = date.getFullYear();  
        const month = String(date.getMonth() + 1).padStart(2, '0');  
        const day = String(date.getDate()).padStart(2, '0');  

        const formattedDate = `${day}-${month}-${year}`;

        if (punchtype == 0) {
            reqid = 10;
            modelname = '#webpunch_tbl';
            document.getElementById('wpunch').innerHTML = modalContent;
            $('#webpunch_modal').modal('show'); webpunch_modal

            var ExportExcel = {
                extend: 'excelHtml5',
                filename: function () {


                    return `WebPunch Details-${formattedDate}`;
                },
                title: 'WebPunch',
                exportOptions: {

                    columns: [0, 1, 2, 3, 4, 5]
                },
            };
        }
        else if (punchtype == 1) {
            reqid = 11;
            modelname = '#devicepunch_tbl';
            document.getElementById('dpunch').innerHTML = modalContent;
            $('#devicepunch_modal').modal('show');
            var ExportExcel = {
                extend: 'excelHtml5',
                filename: function () {


                    return `DevicePunch Details-${formattedDate}`;
                },
                title: 'DevicePunch',
                exportOptions: {

                    columns: [0, 1, 2, 3, 4,5]
                },
            };
          
        }
        else if (punchtype == 2) {
            reqid = 12;
            modelname = '#mobilepunch_tbl';
            document.getElementById('mpunch').innerHTML = modalContent;
            $('#mobilepunch_modal').modal('show');

            var ExportExcel = {
                extend: 'excelHtml5',
                filename: function () {


                    return `Mobile/Face Punch Details-${formattedDate}`;
                },
                title: 'Mobile/Face Punch',
                exportOptions: {

                    columns: [0, 1, 2, 3, 4, 5]
                },
            };
        }

        $.ajax({
            url: `${webApiUrl}DashBoard/GetAdminDashBoardDetails`,
            type: "GET",
            data: { cmpId: cmpId, branchId: branchId, reqid: reqid },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (response) {



                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                    return;
                }


                if (Array.isArray(response)) {
                    response.forEach(function (item) {
                        const dateParts = item.currentdate.split('T')[0];
                        const [year, month, day] = dateParts.split('-');
                        item.currentdate = `${day}-${month}-${year}`;
                    });


                    if ($.fn.dataTable.isDataTable(modelname)) {
                        $(modelname).DataTable().clear().destroy();
                    }


                    $(modelname).DataTable({
                        data: response,
                        "dom": 'lBfrtip',
                        "bDestroy": true,
                        "buttons": [ExportExcel],
                        "scrollX": true,
                        "columns": [
                            { "data": "empcode" },
                            { "data": "empname" },
                            { "data": "emppunchid" },
                            { "data": "departmentname" },
                            {
                                "data": "designationname",
                                "render": function (data, type, row) {
                                    return data ? data : "";
                                }
                            },
                            { "data": "currentdate" },
                        ]
                    });


                    var customDiv = $('<div class="customforms_table"></div>');
                    customDiv.insertBefore("#femalegender_tbl_wrapper > .dataTables_info");
                    $("#femalegender_tbl_wrapper > table").appendTo(customDiv);
                } else {
                    console.error("Data is not in the expected format:", response);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data:", error);
            }
        });

    }

    //leave bifurcation details

    function leaveColumnClicked(leavedata, month) {

        const modalContent = ``;

        // Determine request ID and modal
        var reqid;
        var modelname = '#leavetype_tbl';
        document.getElementById('leavetypes').innerHTML = modalContent;
        $('#leavetype_modal').modal('show');


        $.ajax({
            url: `${webApiUrl}DashBoard/GetAdminDashBoardLeaveDetails`,
            type: "GET",
            data: { cmpId: cmpId, branchId: branchId, leavetype: leavedata, month: month },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (response) {



                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                    return;
                }
                response.forEach(function (item) {
                    const dateParts = item.fromdate.split('T')[0];
                    const [year, month, day] = dateParts.split('-');
                    item.fromdate = `${day}-${month}-${year}`;

                    const datePartsto = item.todate.split('T')[0];
                    const [year1, month1, day1] = datePartsto.split('-');
                    item.todate = `${day1}-${month1}-${year1}`;
                });

                if (Array.isArray(response)) {
                    //response.forEach(function (item) {
                    //    const date = new Date(item.fromdate);
                    //    item.fromdate = date.toISOString().split('T')[0];
                    //    const date1 = new Date(item.todate);
                    //    item.todate = date1.toISOString().split('T')[0];
                    //});


                    if ($.fn.dataTable.isDataTable(modelname)) {
                        $(modelname).DataTable().clear().destroy();
                    }


                    $(modelname).DataTable({
                        data: response,
                        "dom": 'lBfrtip',
                        "bDestroy": true,
                        "buttons": [
                            {
                                extend: 'excelHtml5',
                                filename: function () {


                                    return `LeaveBifurcation_${leavedata}`;
                                },
                                title: 'LeaveBifurcation',
                                exportOptions: {

                                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
                                },
                            }
                        ],
                      
                        scrollX: true,
                        "columns": [
                            { "data": "empcode" },
                            { "data": "empname" },
                            { "data": "emppunchid" },
                            { "data": "departmentname" },
                            {
                                "data": "designationname",
                                "render": function (data, type, row) {
                                    return data ? data : ""; // If data is null or undefined, return "N/A"
                                }
                            },
                            { "data": "PaidLeave" },
                            { "data": "HalfLeave" },
                            { "data": "leavetypename" },
                            { "data": "fromdate" },
                            { "data": "todate" },
                        ]
                    });


                    var customDiv = $('<div class="customforms_table"></div>');
                    customDiv.insertBefore("#femalegender_tbl_wrapper > .dataTables_info");
                    $("#femalegender_tbl_wrapper > table").appendTo(customDiv);
                } else {
                    console.error("Data is not in the expected format:", response);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data:", error);
            }
        });
    }

    //Othrs details

    function Othrsdetails(ClickedDate) {

        const modalContent = ``;

        // Determine request ID and modal
        var reqid;
        var modelname = '#othour_tbl';
        document.getElementById('otid').innerHTML = modalContent;
        $('#othour_modal').modal('show');


        $.ajax({
            url: `${webApiUrl}DashBoard/GetAdminDashBoardAttenDetails`,
            type: "GET",
            data: { cmpId: cmpId, branchId: branchId, date: ClickedDate, reqid: 0 },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (response) {



                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                    return;
                }



                if (Array.isArray(response)) {
                    response.forEach(function (item) {
                        const date = new Date(item.currentdate);

                        const year = date.getFullYear();
                        const month = String(date.getMonth() + 1).padStart(2, '0');
                        const day = String(date.getDate()).padStart(2, '0');
                        item.currentdate = `${day}-${month}-${year}`;

                    });
                   
                    const date = new Date(ClickedDate);

                        const year = date.getFullYear();
                        const month = String(date.getMonth() + 1).padStart(2, '0');
                        const day = String(date.getDate()).padStart(2, '0');
                        const formatedate = `${day}-${month}-${year}`;

                    if ($.fn.dataTable.isDataTable(modelname)) {
                        $(modelname).DataTable().clear().destroy();
                    }


                    $(modelname).DataTable({
                        data: response,
                        "dom": 'lBfrtip',
                        "bDestroy": true,
                        "buttons": [
                            {
                                extend: 'excelHtml5',
                                filename: function () {


                                    return `Overtime Details-${formatedate}`;
                                },
                                title: 'Overtime Details',
                                exportOptions: {

                                    columns: [0, 1, 2, 3, 4, 5, 6,7]
                                },

                            }
                        ],
                        "scrollX": true,
                        "columns": [
                            { "data": "empcode" },
                            { "data": "empname" },
                            { "data": "emppunchid" },
                            { "data": "departmentname" },
                            {
                                "data": "designationname",
                                "render": function (data, type, row) {
                                    return data ? data : "";
                                }
                            },
                            { "data": "shiftname" },
                            { "data": "othr" },
                            { "data": "currentdate" },

                        ]
                    });


                    var customDiv = $('<div class="customforms_table"></div>');
                    customDiv.insertBefore("#femalegender_tbl_wrapper > .dataTables_info");
                    $("#femalegender_tbl_wrapper > table").appendTo(customDiv);
                } else {
                    console.error("Data is not in the expected format:", response);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data:", error);
            }
        });

    }

    //attendence overview chart details

    function Attn_overviewdetails(ClickedDate, seriesName) {

        const modalContent = ``;

        // Determine request ID and modal
        var reqid;
        var modelname;
        if (seriesName == 'Present') {
            reqid = 1;
            modelname = '#present_tbl';
            document.getElementById('presentid').innerHTML = modalContent;
            $('#present_modal').modal('show');
        } else if (seriesName == 'Weekoff') {
            reqid = 2;
            modelname = '#weekoff_tbl';
            document.getElementById('weekoffid').innerHTML = modalContent;
            $('#weekoff_modal').modal('show');
        }
        else if (seriesName == 'Holiday') {
            reqid = 3;
            modelname = '#holiday_tbl';
            document.getElementById('holidayids').innerHTML = modalContent;
            $('#holiday_modal').modal('show');
        }
        else if (seriesName == 'Miss Punch') {
            reqid = 4;
            modelname = '#misspunch_tbl';
            document.getElementById('misspunchid').innerHTML = modalContent;
            $('#misspunch_modal').modal('show');
        }
        else if (seriesName == 'Absent') {
            reqid = 5;
            modelname = '#absent_tbl';
            document.getElementById('absentid').innerHTML = modalContent;
            $('#absent_modal').modal('show');
        }
        else if (seriesName == 'Less Hours') {
            reqid = 6;
            modelname = '#less_tbl';
            document.getElementById('lesshourhid').innerHTML = modalContent;
            $('#lesshour_modal').modal('show');
        }
        else if (seriesName == 'Half Day') {
            reqid = 7;
            modelname = '#Halfday_tbl';
            document.getElementById('Halfdayid').innerHTML = modalContent;
            $('#HalfDay_modal').modal('show');
        }
        else if (seriesName == 'On Leave') {
            reqid = 8;
            modelname = '#onleave_tbl';
            document.getElementById('onleaveid').innerHTML = modalContent;
            $('#onleave_modal').modal('show');
        }



        $.ajax({
            url: `${webApiUrl}DashBoard/GetAdminDashBoardAttenDetails`,
            type: "GET",
            data: { cmpId: cmpId, branchId: branchId, date: ClickedDate, reqid: reqid },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (response) {
            

                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                    return;
                }
                response.forEach(function (item) {

                    const date = new Date(item.currentdate);

                    const year = date.getFullYear();
                    const month = String(date.getMonth() + 1).padStart(2, '0');
                    const day = String(date.getDate()).padStart(2, '0');
                    item.currentdate = `${day}-${month}-${year}`;

                });
               
                //for excel export
                
                const date = new Date(ClickedDate);  

                const year = date.getFullYear();  
                const month = String(date.getMonth() + 1).padStart(2, '0'); 
                const day = String(date.getDate()).padStart(2, '0'); 

                // Format the date as dd-mm-yyyy
                const formattedDate = `${day}-${month}-${year}`;

                if (Array.isArray(response)) {
                    if (reqid === 1) { // Example for 'Present'
                        dynamicColumns = [
                            { "data": "empcode" },
                            { "data": "empname" },
                            { "data": "emppunchid" },
                            { "data": "departmentname" },
                            {
                                "data": "designationname",
                                "render": function (data, type, row) {
                                    return data ? data : "";
                                }
                            },
                            { "data": "shiftname" },
                            { "data": "finalstatus" },
                            { "data": "intime" },
                            { "data": "outtime" },
                            { "data": "tothour" },
                            { "data": "currentdate" },
                        ];

                        var ExportExcel = {
                            extend: 'excelHtml5',
                            filename: function () {

                               
                                return `Present-${formattedDate}`;
                            },
                            title: 'Present',
                            exportOptions: {

                                columns: [0,1, 2, 3, 4, 5, 6, 7, 8, 9,10]
                            },
                        };


                    } else if (reqid === 2) {

                        response.forEach(function (item) {

                            const dateParts = item.currentdate.split('T')[0];
                            const [year, month, day] = dateParts.split('-');
                            item.currentdate = `${year}-${month}-${day}`;

                        });// Example for 'Weekoff'
                        dynamicColumns = [
                            { "data": "empcode" },
                            { "data": "empname" },
                            { "data": "emppunchid" },
                            { "data": "departmentname" },
                            {
                                "data": "designationname",
                                "render": function (data, type, row) {
                                    return data ? data : "";
                                }
                            },
                            { "data": "shiftname" },
                            { "data": "finalstatus" },
                            { "data": "currentdate" },
                        ];

                        var ExportExcel = {
                            extend: 'excelHtml5',
                            filename: function () {


                                return `Weekoff-${formattedDate}`;
                            },
                            title: 'Weekoff',
                            exportOptions: {

                                columns: [0, 1, 2, 3, 4, 5, 6,7]
                            },
                        };
                    } else if (reqid === 3) {

                        response.forEach(function (item) {

                            const dateParts = item.currentdate.split('T')[0];
                            const [year, month, day] = dateParts.split('-');
                            item.currentdate = `${year}-${month}-${day}`;

                        });// Example for holiday
                        dynamicColumns = [
                            { "data": "empcode" },
                            { "data": "empname" },
                            { "data": "emppunchid" },
                            { "data": "departmentname" },
                            {
                                "data": "designationname",
                                "render": function (data, type, row) {
                                    return data ? data : "";
                                }
                            },
                            { "data": "shiftname" },
                            { "data": "finalstatus" },
                            { "data": "currentdate" },
                        ];
                        var ExportExcel = {
                            extend: 'excelHtml5',
                            filename: function () {


                                return `Holiday-${formattedDate}`;
                            },
                            title: 'Holiday',
                            exportOptions: {

                                columns: [0, 1, 2, 3, 4, 5, 6, 7]
                            },
                        };
                    }
                    else if (reqid === 4) {

                        response.forEach(function (item) {

                            const dateParts = item.currentdate.split('T')[0];
                            const [year, month, day] = dateParts.split('-');
                            item.currentdate = `${year}-${month}-${day}`;

                        });// Misspunch
                        dynamicColumns = [
                            { "data": "empcode" },
                            { "data": "empname" },
                            { "data": "emppunchid" },
                            { "data": "departmentname" },
                            {
                                "data": "designationname",
                                "render": function (data, type, row) {
                                    return data ? data : "";
                                }
                            },
                            { "data": "shiftname" },
                            { "data": "finalstatus" },
                            { "data": "currentdate" },
                        ];
                        var ExportExcel = {
                            extend: 'excelHtml5',
                            filename: function () {


                                return `Misspunch-${formattedDate}`;
                            },
                            title: 'Misspunch',
                            exportOptions: {

                                columns: [0, 1, 2, 3, 4, 5, 6, 7]
                            },
                        };
                    }
                    else if (reqid === 5) {
                        response.forEach(function (item) {

                            const dateParts = item.currentdate.split('T')[0];
                            const [year, month, day] = dateParts.split('-');
                            item.currentdate = `${year}-${month}-${day}`;

                        }); //absent counts
                        dynamicColumns = [
                            { "data": "empcode" },
                            { "data": "empname" },
                            { "data": "emppunchid" },
                            { "data": "departmentname" },
                            {
                                "data": "designationname",
                                "render": function (data, type, row) {
                                    return data ? data : "";
                                }
                            },
                            { "data": "shiftname" },
                            { "data": "finalstatus" },
                            { "data": "currentdate" },

                        ];
                        var ExportExcel = {
                            extend: 'excelHtml5',
                            filename: function () {


                                return `Absent-${formattedDate}`;
                            },
                            title: 'Absent',
                            exportOptions: {

                                columns: [0, 1, 2, 3, 4, 5, 6, 7]
                            },
                        };
                    }
                    else if (reqid === 6) {
                        response.forEach(function (item) {

                            //const date = new Date(item.currentdate);

                            //const year = date.getFullYear();
                            //const month = String(date.getMonth() + 1).padStart(2, '0');
                            //const day = String(date.getDate()).padStart(2, '0');
                            //item.currentdate = `${day}-${month}-${year}`;

                            const dateParts = item.currentdate.split('T')[0];
                            const [year, month, day] = dateParts.split('-');
                            item.currentdate = `${year}-${month}-${day}`;

                        }); //less hours
                        dynamicColumns = [
                            { "data": "empcode" },
                            { "data": "empname" },
                            { "data": "emppunchid" },
                            { "data": "departmentname" },
                            {
                                "data": "designationname",
                                "render": function (data, type, row) {
                                    return data ? data : "";
                                }
                            },
                            { "data": "shiftname" },
                            { "data": "finalstatus" },
                            { "data": "intime" },
                            { "data": "outtime" },
                            { "data": "currentdate" },

                        ];
                        var ExportExcel = {
                            extend: 'excelHtml5',
                            filename: function () {


                                return `Lesshours-${formattedDate}`;
                            },
                            title: 'Lesshours',
                            exportOptions: {

                                columns: [0, 1, 2, 3, 4, 5, 6, 7,8,9]
                            },
                        };
                    }
                    else if (reqid === 7) {
                        response.forEach(function (item) {

                            const dateParts = item.currentdate.split('T')[0];
                            const [year, month, day] = dateParts.split('-');
                            item.currentdate = `${year}-${month}-${day}`;

                        }); //HalfDay hours
                        dynamicColumns = [
                            { "data": "empcode" },
                            { "data": "empname" },
                            { "data": "emppunchid" },
                            { "data": "departmentname" },
                            {
                                "data": "designationname",
                                "render": function (data, type, row) {
                                    return data ? data : "";
                                }
                            },
                            { "data": "shiftname" },
                            { "data": "finalstatus" },
                            { "data": "intime" },
                            { "data": "outtime" },
                            { "data": "currentdate" },

                        ];
                        var ExportExcel = {
                            extend: 'excelHtml5',
                            filename: function () {


                                return `Halfday-${formattedDate}`;
                            },
                            title: 'Halfday',
                            exportOptions: {

                                columns: [0, 1, 2, 3, 4, 5, 6, 7,8,9]
                            },
                        };
                    }
                    else if (reqid === 8) {
                        response.forEach(function (item) {

                            const dateParts = item.currentdate.split('T')[0];
                            const [year, month, day] = dateParts.split('-');
                            item.currentdate = `${year}-${month}-${day}`;

                        }); //On Leave
                        dynamicColumns = [
                            { "data": "empcode" },
                            { "data": "empname" },
                            { "data": "emppunchid" },
                            { "data": "departmentname" },
                            {
                                "data": "designationname",
                                "render": function (data, type, row) {
                                    return data ? data : "";
                                }
                            },
                            /* { "data": "shiftname" },*/
                            { "data": "finalstatus" },
                            { "data": "currentdate" },

                        ];
                        var ExportExcel = {
                            extend: 'excelHtml5',
                            filename: function () {


                                return `Onleave-${formattedDate}`;
                            },
                            title: 'Onleave',
                            exportOptions: {

                                columns: [0, 1, 2, 3, 4, 5, 6]
                            },
                        };
                    }






                    if ($.fn.dataTable.isDataTable(modelname)) {
                        $(modelname).DataTable().clear().destroy();
                    }


                    $(modelname).DataTable({
                        data: response,
                        "dom": 'lBfrtip',
                        "bDestroy": true,
                        "buttons": [ExportExcel],
                        "scrollX": true,
                        "columns": dynamicColumns,
                    });


                    var customDiv = $('<div class="customforms_table"></div>');
                    customDiv.insertBefore("#femalegender_tbl_wrapper > .dataTables_info");
                    $("#femalegender_tbl_wrapper > table").appendTo(customDiv);
                } else {
                    console.error("Data is not in the expected format:", response);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data:", error);
            }
        });

    }


    //Attration chart details
    function AttrationChartDetails(Month, Gender, departmentid) {

        const modalContent = ``;

        // Determine request ID and modal

        var modelname;

        if (Gender === 'Female') {
            Gender = 1;
            modelname = '#FeAttration_tbl';
            document.getElementById('femapunch').innerHTML = modalContent;
            $('#FemaleAttration_modal').modal('show');
            var ExportExcel = {
                extend: 'excelHtml5',
                filename: function () {


                    return 'FemaleAttration';
                },
                title: 'FemaleAttration',
                exportOptions: {

                    columns: [0, 1, 2, 3, 4, 5,6]
                },
            };
        } else {
            Gender = 0;
            modelname = '#Attration_tbl';
            document.getElementById('mapunch').innerHTML = modalContent;
            $('#MaleAttration_modal').modal('show');
            var ExportExcel = {
                extend: 'excelHtml5',
                filename: function () {


                    return 'MaleAttration';
                },
                title: 'MaleAttration',
                exportOptions: {

                    columns: [0, 1, 2, 3, 4, 5, 6]
                },
            };
        }

        $.ajax({
            url: `${webApiUrl}DashBoard/GetAdminDashBoardAttrationDetails`,
            type: "GET",
            data: { cmpId: cmpId, branchId: branchId, Month: Month, Gender: Gender, reqid: 0, departmentid: departmentid },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (response) {



                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                    return;
                }


                if (Array.isArray(response)) {
                    response.forEach(function (item) {
                        const dateParts = item.EmpResignDate.split('T')[0];
                        const [year, month, day] = dateParts.split('-');
                        item.EmpResignDate = `${day}-${month}-${year}`;
                    });



                    if ($.fn.dataTable.isDataTable(modelname)) {
                        $(modelname).DataTable().clear().destroy();
                    }


                    $(modelname).DataTable({
                        data: response,
                        "dom": 'lBfrtip',
                        "bDestroy": true,
                        "buttons": [ExportExcel],
                        "scrollX": true,
                        "columns": [
                            { "data": "empcode" },
                            { "data": "empname" },
                            { "data": "emppunchid" },
                            { "data": "departmentname" },
                            {
                                "data": "designationname",
                                "render": function (data, type, row) {
                                    return data ? data : "";
                                }
                            },

                            { "data": "branchname" },
                            { "data": "EmpResignDate" },

                        ]
                    });


                    var customDiv = $('<div class="customforms_table"></div>');
                    customDiv.insertBefore("#femalegender_tbl_wrapper > .dataTables_info");
                    $("#femalegender_tbl_wrapper > table").appendTo(customDiv);
                } else {
                    console.error("Data is not in the expected format:", response);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data:", error);
            }
        });

    }
    //not checked in count
    //function checkinDetails() {
    //    $.ajax({
    //        url: `${webApiUrl}DashBoard/GetAdminDashBoardDetails`,
    //        type: "GET",
    //        data: { cmpId: cmpId, branchId: branchId, reqid: 1 },
    //        beforeSend: function (xhr) {
    //            xhr.setRequestHeader("Authorization", token);
    //        },
    //        success: function (response) {

    //            if (response != null && response != "") {
    //                if (response.isRedirect) {
    //                    window.location.href = response.redirectUrl;
    //                    return;
    //                }


    //                if (Array.isArray(response)) {
    //                    response.forEach(function (item) {
    //                        const dateParts = item.currentdate.split('T')[0]; // Assuming input is in ISO format
    //                        item.currentdate = dateParts;
    //                    });


    //                    if ($.fn.dataTable.isDataTable('#checkin_table')) {
    //                        $('#checkin_table').DataTable().clear().destroy();
    //                    }


    //                    $('#checkin_table').DataTable({
    //                        data: response,
    //                        "dom": 'lBfrtip',
    //                        "bDestroy": true,
    //                        "buttons": [
    //                            {
    //                                extend: 'excelHtml5',
    //                                filename: function () {

    //                                    const currentDate = response[0].currentdate;
    //                                    return `checkedin-${currentDate}`;
    //                                },
    //                                title: 'checkindetails',
    //                                exportOptions: {

    //                                    columns: [0, 1, 2, 3, 4, 5, 6,7]
    //                                },
    //                            }
    //                        ],
    //                        "columns": [
    //                            { "data": "Empcode" },
    //                            { "data": "EmpName" },
    //                            { "data": "emppunchid" },
    //                            { "data": "departmentname" },
    //                            { "data": "designationname" },
    //                            { "data": "ShiftName" },
    //                            { "data": "Mode" },
    //                            { "data": "InTime" },
    //                            { "data": "currentdate" }
    //                        ]
    //                    });


    //                    var customDiv = $('<div class="customforms_table"></div>');
    //                    customDiv.insertBefore("#checkin_table_wrapper > .dataTables_info");
    //                    $("#checkin_table_wrapper > table").appendTo(customDiv);
    //                } else {
    //                    console.error("Data is not in the expected format:", response);
    //                }
    //            }
    //        },
    //        error: function (xhr, status, error) {
    //            console.error("Error fetching data:", error);
    //        }
    //    });
    //}
    $("#notcheckedinbtn").on('click', function () {

        var totalCount5 = parseInt($("#notcheckinCount").text(), 10);
        if (!isNaN(totalCount5) && totalCount5 > 0) {
            $("#notcheckedinbtn").attr("data-toggle", "modal");
            $("#notcheckedinbtn").attr("data-target", "#notcheckedin_modal");

            $('#checkin_table').hide();
            $('#notcheckedin_tbl').show();
            $('#lateclockin_tbl').hide();
            $('#earlyclockout_tbl').hide();
            $('#birthday_tbl').hide();
            $('#device_tbl').hide();
            NotcheckinDeatails();

        }

    });
   


    function NotcheckinDeatails() {


        $.ajax({
            url: `${webApiUrl}DashBoard/GetAdminDashBoardDetails`,
            type: "GET",
            data: { cmpId: cmpId, branchId: branchId, reqid: 6 },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (response) {



                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                    return;
                }


                if (Array.isArray(response)) {
                    response.forEach(function (item) {
                        const dateParts = item.currentdate.split('T')[0];
                        const [year, month, day] = dateParts.split('-');
                        item.currentdate = `${day}-${month}-${year}`;
                    });


                    if ($.fn.dataTable.isDataTable('#notcheckedin_tbl')) {
                        $('#notcheckedin_tbl').DataTable().clear().destroy();
                    }


                    $('#notcheckedin_tbl').DataTable({
                        data: response,
                        "dom": 'lBfrtip',
                        "bDestroy": true,
                        "buttons": [
                            {
                                extend: 'excelHtml5',
                                filename: function () {
                                   
                                    const currentDate = response[0].currentdate; 
                                    return `Notcheckedin-${currentDate}`; 
                                },
                                title: 'Notcheckin Details', 
                                exportOptions: {
                                   
                                    columns: [0,1,2,3,4,5,6] 
                                },
                            }
                        ],
                        "columns": [
                            { "data": "Empcode" },
                            { "data": "EmpName" },
                            { "data": "emppunchid" },
                            { "data": "departmentname" },
                            { "data": "designationname" },
                            { "data": "ShiftName" },
                            {
                                "data": null,
                                "render": function () {
                                    return "A";
                                }
                            },
                            { "data": "currentdate" }
                        ]
                    });


                    var customDiv = $('<div class="customforms_table"></div>');
                    customDiv.insertBefore("#notcheckedin_tbl_wrapper > .dataTables_info");
                    $("#notcheckedin_tbl_wrapper > table").appendTo(customDiv);
                } else {
                    console.error("Data is not in the expected format:", response);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data:", error);
            }
        });
    }


    function renderPunchTypeChart(punchData) {
        
        function getDataForName(name, punchData) {
            const data = punchData.find(item => item.name === name);
            return data ? data.y : 0; 
        }

        console.log(punchData);
        const datas = [
            {
                name: "Web",
                y: getDataForName("Web", punchData),
                selected: true,
                color: '#A18FB9'
            },
            {
                name: "Device",
                y: getDataForName("Device", punchData),
                color: '#7CA2CE'
            },
            {
                name: "Face",
                y: getDataForName("Face", punchData),
                color: '#B5CC88'
            }
        ];
        Highcharts.chart('Punchtyp_chart', {
            chart: {
                type: 'pie',
                backgroundColor: '#ffffff',
                style: {
                    fontFamily: 'Roboto',
                },
            },
            title: {
                text: '',
                align: 'center',
                verticalAlign: 'middle',
                y: 50
            },
            plotOptions: {
                pie: {
                    innerSize: '75%',
                    size: '125%',
                    depth: 45,
                    startAngle: -90,
                    endAngle: 90,
                    center: ['50%', '80%'],
                    dataLabels: {
                        enabled: false
                    },
                    showInLegend: true,
                    borderRadius: 0,
                },
                series: {
                    point: {
                        events: {
                            click: function () {

                                const chartindex = this.index;
                                // Call your custom function with branchName and gender
                                handlePunchChartClick(chartindex);
                            }
                        }
                    }
                }
            },
            legend: {
                align: 'center',
                x: -5,
                y: 0,
                verticalAlign: 'bottom',
                layout: 'horizontal',
                itemDistance: 5,  // Remove space between horizontal legend items
                labelFormatter: function () {
                    // Assign a different image for each legend item
                    var imageUrl;
                    if (this.name === 'Device') {
                        imageUrl = '/assets/images/icons/icon_punchdevice.svg';
                    } else if (this.name === 'Web') {
                        imageUrl = '../../assets/images/icons/icon_web.svg';
                    } else if (this.name === 'Face') {
                        imageUrl = '../../assets/images/icons/icon_mobile.svg';
                    }

                    // Return HTML for custom legend with image, count, and name
                    return '<div class="custom-legend"><img src="' + imageUrl + '" class="legend-image" /><div class="legend-text"><div class="legend-count">' + this.y + '</div><div class="legend-name">' + this.name + '</div></div></div>';
                },
                itemStyle: {
                    // Optionally, add any custom item styles here
                },
                useHTML: true,  // Allow HTML in legend labels
                symbolWidth: 0,  // Hide the default dot
                symbolHeight: 0, // Hide the default dot
                itemMarginTop: 6,
                itemMarginBottom: 5,
            },
            series: [{
                name: 'Punch Type',
                data: datas
            }]

        });
    }


    //Function to render Genderwise chart
    function renderGenderWiseChart(branchNames, branchid, PresentmaleCounts, PresentfemaleCounts, AbsentmaleCounts,AbsentfemaleCounts,reqid) {
        
        const numCategories = branchIds.length;
        var baseWidthPerCategory;


        if (numCategories <= 10) {
            baseWidthPerCategory = 490 / numCategories; // Ensure the total width is at least 490px
        } else {
            baseWidthPerCategory = 35; // Use 35px per category for more than 13 categories
        }
        // Calculate the total width
        let calculatedWidth = Math.max(490, numCategories * baseWidthPerCategory); // Minimum width is 490px
        Highcharts.chart('genderwise_chart', {
            chart: {
                type: 'column',
                height: 300,
                style: {
                    fontFamily: 'Roboto',
                },
                spacingRight: 20,
                panning: false,
                pinchType: 'x',
                width: calculatedWidth,
            },

            title: {
                text: '',
                align: 'left'
            },

            xAxis: {
                categories: branchNames,
                crosshair: true,
                accessibility: {
                    description: 'Categories from 1 to 30'
                },
                labels: {
                    style: {
                        fontSize: '10px'
                    }
                },
                tickWidth: 0,
                //min: 0,
                //max: numCategories - 1, // Allow a limited view for scroll behavior
                //scrollbar: {
                //    enabled: true, // Enable scrollbar for horizontal scrolling
                //},
            },

            yAxis: {
                min: 0,
                title: {
                    text: 'Number of Employees'
                },
                stackLabels: {
                    enabled: false,
                    style: {
                        fontWeight: 'bold'
                    }
                }
            }, 
            legend: {
                x: 50,
                align: 'left', // Align legend horizontally at the center
                verticalAlign: 'bottom', // Place legend at the bottom
                layout: 'horizontal', // Arrange legend items in a row
                backgroundColor: 'rgba(255, 255, 255, 0.9)', // Optional: Add background to enhance visibility
                itemStyle: {
                    fontSize: '11px',
                    fontWeight: 'normal',
                    fontFamily: 'Roboto',
                }
            },

            tooltip: {
                shared: false,
                pointFormat: '{series.name}: {point.y}'
            },

            plotOptions: {
               
                    column: {
                    pointPadding: 0.1,
                    groupPadding: 0.2,
                    borderWidth: 0,
                    borderRadius: 3,
                    pointWidth: 8,
                    minPointLength: 3,
                    stacking: 'normal',
                    borderWidth: 0,
                    dataLabels: {
                        enabled: false,
                        format: '{y}'
                    },
                    pointWidth: 8,
                },
                series: {
                    point: {
                        events: {
                            click: function () {
                                
                                const branchIndex = this.index; 
                                const branchId = branchid[branchIndex];
                                const seriesName = this.series.name; 

                                let flag = 0; 
                                let gender = '';

                              
                                if (seriesName.includes("Present")) {
                                    flag = 1;
                                }

                          
                                if (seriesName.includes("Male")) {
                                    gender = "Male";
                                } else if (seriesName.includes("Female")) {
                                    gender = "Female";
                                }

                                
                            
                                handleChartClick(branchId, gender, flag,reqid);
                            }
                        }
                    }
                }
            },
            
            

            series: [
                {
                    name: 'Male Absent',
                    data: AbsentmaleCounts,
                    color: '#7CBEC0',
                    stack: 'Male'
                },
                {
                    name: 'Male Present',
                    data: PresentmaleCounts,
                    color: '#249196',
                    stack: 'Male'
                },
                {
                    name: 'Female Absent',
                    data: AbsentfemaleCounts,
                    color: '#DDBDE3',
                    stack: 'Female'
                },
                {
                    name: 'Female Present',
                    data: PresentfemaleCounts,
                    color: '#C892D2',
                    stack: 'Female'
                }
            ]
        });
    }

    // Function to handle clicks
    



    //function renderGenderWiseChart(branchNames, branchIds, maleCounts, femaleCounts) {

    //    const numCategories = branchIds.length;
    //    var baseWidthPerCategory;


    //    if (numCategories <= 10) {
    //        baseWidthPerCategory = 490 / numCategories; // Ensure the total width is at least 490px
    //    } else {
    //        baseWidthPerCategory = 35; // Use 35px per category for more than 13 categories
    //    }
    //    // Calculate the total width
    //    let calculatedWidth = Math.max(490, numCategories * baseWidthPerCategory); // Minimum width is 490px

    //    //Create the chart with the dynamically calculated width
    //    Highcharts.chart('genderwise_chart', {
    //        chart: {
    //            type: 'column',
    //            height: 300,
    //            style: {
    //                fontFamily: 'Roboto',
    //            },
    //            spacingRight: 20,
    //            panning: false,
    //            pinchType: 'x',
    //            width: calculatedWidth,
    //        },
    //        title: {
    //            text: '',
    //            align: 'left'
    //        },
    //        xAxis: {
    //            categories: branchNames,
    //            crosshair: true,
    //            accessibility: {
    //                description: 'Categories from 1 to 30'
    //            },
    //            labels: {
    //                style: {
    //                    fontSize: '10px'
    //                }
    //            },
    //            tickWidth: 0,
    //            min: 0,
    //            max: numCategories - 1, // Allow a limited view for scroll behavior
    //            scrollbar: {
    //                enabled: true, // Enable scrollbar for horizontal scrolling
    //            },
    //        },
    //        yAxis: {
    //            min: 0,
    //            title: {
    //                text: 'Total Gender Count'
    //            }
    //        },
    //        legend: {
    //            x: 175,
    //            align: 'left', // Align legend horizontally at the center
    //            verticalAlign: 'bottom', // Place legend at the bottom
    //            layout: 'horizontal', // Arrange legend items in a row
    //            backgroundColor: 'rgba(255, 255, 255, 0.9)', // Optional: Add background to enhance visibility
    //            itemStyle: {
    //                fontSize: '11px',
    //                fontWeight: 'normal',
    //                fontFamily: 'Roboto',
    //            }
    //        },
    //        tooltip: {
    //            valueSuffix: ''
    //        },
    //        plotOptions: {
    //            column: {
    //                pointPadding: 0.1,
    //                groupPadding: 0.2,
    //                borderWidth: 0,
    //                borderRadius: 3,
    //                pointWidth: 8,
    //                minPointLength: 3,
    //            },
    //            series: {
    //                point: {
    //                    events: {
    //                        click: function () {

    //                            const branchIndex = this.index;
    //                            const branchId = branchIds[branchIndex];
    //                            const gender = this.series.name;  // Series name (Male or Female)

    //                            // Call your custom function with branchName and gender
    //                            handleChartClick(branchId, gender);
    //                        }
    //                    }
    //                }
    //            }
    //        },
    //        series: [
    //            { name: 'Male', data: maleCounts, color: '#7CBEC0' },
    //            { name: 'Female', data: femaleCounts, color: '#DDBDE3' }
    //        ]


    //    });
       
    //}
    function leaveBifurcationChart(leavedata, month) {

        am4core.useTheme(am4themes_animated);

        var chart = am4core.create("chartdiv", am4charts.XYChart);
        var data = leavedata.map(item => ({
            category: item.leavetypename,
            value: item.total_leave_days
        }));

        chart.data = data;

        // Set chart font family
        chart.fontFamily = 'Roboto';

        //////////////////// Configure the X Axis ////////////////////
        var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
        categoryAxis.renderer.grid.template.location = 0;
        categoryAxis.dataFields.category = "category";
        categoryAxis.renderer.minGridDistance = 10;
        categoryAxis.renderer.grid.template.location = 0.5;
        categoryAxis.renderer.grid.template.strokeDasharray = "1,3";
        categoryAxis.renderer.labels.template.horizontalCenter = "left";
        categoryAxis.renderer.labels.template.location = 0.5;
        categoryAxis.renderer.grid.template.disabled = true; // Disables grid lines

        // Set x-axis title
        categoryAxis.title.text = "Leave Bifurcation";
        categoryAxis.title.fontSize = 12;
        categoryAxis.title.fill = am4core.color("#666666");

        // Set axis labels color and font
        categoryAxis.renderer.labels.template.fill = am4core.color("#666666");

        categoryAxis.renderer.labels.template.adapter.add("dx", function (dx, target) {
            return -target.maxRight / 2;
        });

        //////////////////// Configure the Y Axis ////////////////////
        var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
        valueAxis.tooltip.disabled = true;
        valueAxis.renderer.ticks.template.disabled = true;
        valueAxis.renderer.axisFills.template.disabled = true;

        // Set y-axis title
        valueAxis.title.text = "Leave Count";
        valueAxis.title.fontSize = 12;
        valueAxis.title.fill = am4core.color("#666666");

        // Set axis labels color and font
        valueAxis.renderer.labels.template.fill = am4core.color("#666666");

        //////////////////// Configure the Series ////////////////////
        var series = chart.series.push(new am4charts.ColumnSeries());
        series.dataFields.categoryX = "category";
        series.dataFields.valueY = "value";
        series.tooltipText = "[font-size:11px]Leave Type \n{categoryX}: [bold]{valueY}";
        series.sequencedInterpolation = true;
        series.fillOpacity = 0;
        series.strokeOpacity = 1;
        series.strokeDashArray = "1,3";
        series.columns.template.width = 0.01;

        // Set the column fill color
        series.columns.template.fill = am4core.color("#49B196");
        series.columns.template.stroke = am4core.color("#49B196");

        // Ensure minimum bar height
        series.columns.template.adapter.add("height", function (height, target) {
            return height < 50 ? 50 : height;
        });

        // Create tooltip
        series.tooltip.getFillFromObject = false;
        series.tooltip.background.fill = am4core.color("#ffffff");
        series.tooltip.label.fill = am4core.color("#333333");
        series.tooltip.background.stroke = am4core.color("#7cb5ec");
        series.tooltip.background.strokeWidth = 1;
        series.tooltip.background.cornerRadius = 3;

        //////////////////// Create Bullet ////////////////////
        var bullet = series.bullets.create(am4charts.CircleBullet);
        bullet.circle.fill = am4core.color("#49B196");

        // Bullet click event
        bullet.events.on("hit", function (event) {
            var dataItem = event.target.dataItem;
            var category = dataItem.categoryX;
            leaveColumnClicked(category, month); // Trigger your custom logic

        });

        //////////////////// Cursor Configuration ////////////////////
        chart.cursor = new am4charts.XYCursor();
        chart.cursor.lineX.disabled = true;
        chart.cursor.lineY.disabled = true;
        chart.cursor.tooltipText = "";
        chart.logo.disabled = true;
    }



    //function to render Attendence Overview Chart
    function renderAttendeceChart(data) {
       
        function getResponsiveMargin_overview() {
            if (window.innerWidth < 479) { // For small screens
                return 100;
            } else if (window.innerWidth < 768) { // For medium screens
                return 80;
            } else { // For larger screens
                return 70;
            }
        }


        const presentData = data.map(day => day.PresentCount);
        const absentData = data.map(day => day.AbsentCount);
        const onLeaveData = data.map(day => day.ODCount);
        const holidayData = data.map(day => day.HolidayCount);
        const missPunchData = data.map(day => day.MisspunchCount);
        const weekoffData = data.map(day => day.WeekoffCount);
        const categories = data.map(item => item.FormattedDate);
        const LessHours = data.map(item => item.LessHour);
        const HalfDay = data.map(item => item.HalfDayCount);

        Highcharts.chart('overview_chart', {
            chart: {
                type: 'column',
                height: 270, // Set the height of the chart
                marginBottom: getResponsiveMargin_overview(), // Set the bottom margin
                style: {
                    fontFamily: 'Roboto', // Set font family for y-axis labels
                }
            },
            title: {
                text: '',
                align: 'left',
            },
            xAxis: {
                categories: categories,
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'Represents the number of employees',
                    x: -10 // Set vertical position of y-axis labels to 0
                },
            },
            legend: {
                align: 'center',
                x: -3,
                verticalAlign: 'bottom',
                y: 20,
                floating: true,
                borderWidth: 0,
                shadow: false,
                itemStyle: {
                    fontSize: '11px', // Set legend font size here
                    fontWeight: 'normal', // Set font weight to normal
                    fontFamily: 'Roboto',
                },
                itemMarginTop: 0, // Vertical space between legend items (top margin)
                itemMarginBottom: 5, // Vertical space between legend items (bottom margin)
                symbolPadding: 0 // Horizontal space between the legend symbol and text
            },
            tooltip: {
                headerFormat: '<b>{point.x}</b><br/>',
                pointFormat: '{series.name}: {point.y}<br/>Total: {point.stackTotal}'
            },
            plotOptions: {
                column: {
                    stacking: 'normal',
                    pointWidth: 10, // Set the column width to 10px
                    minPointLength: 3, // Set a minimum height for the smallest values
                },
                series: {
                    point: {
                        events: {
                            click: function () {

                                const pointIndex = this.index;
                                const attnDate = data[pointIndex].Attn_Dt;
                                const seriesName = this.series.name;
                                Attn_overviewdetails(attnDate, seriesName);
                            }
                        }
                    }
                }
            },
            series: [
                {
                    name: 'Present',
                    data: presentData,
                    color: '#49B196',
                },
                {
                    name: 'Absent',
                    data: absentData,
                    color: '#D35A5A',
                },
                {
                    name: 'On Leave',
                    data: onLeaveData,
                    color: '#F87716',
                },
                {
                    name: 'Holiday',
                    data: holidayData,
                    color: '#A572B9',
                },
                {
                    name: 'Miss Punch',
                    data: missPunchData,
                    color: '#7F99DB',
                },
                {
                    name: 'Weekoff',
                    data: weekoffData,
                    color: '#BBBBBB',
                },
                {
                    name: 'Less Hours',
                    data: LessHours,
                    color: '#B1628E',
                }, {
                    name: 'Half Day',
                    data: HalfDay,
                    color: '#B6997C',
                }],

        });


    }

    //function to render chart for othrs
    function renderOThrscharts(data) {

        const categories = data.map(item => item.FormattedDate);
        const OtHours = data.map(day => {

            const othrString = day.OTHr.toString();

            if (othrString.includes(':')) {
                const timeParts = day.OTHr.split(':');
                const hours = parseInt(timeParts[0], 10);
                const minutes = parseInt(timeParts[1], 10);


                const totalMinutes = hours * 60 + minutes;
                return totalMinutes;
            }
        });
        Highcharts.chart('OT_chart', {
            chart: {
                type: 'spline',
                style: {
                    fontFamily: 'Roboto', // Set font family for y-axis labels
                }
            },
            title: {
                text: ''
            },
            xAxis: {
                title: {
                    text: 'Overtime Weeks'
                },
                categories: categories,
                accessibility: {
                    description: 'Months of the year'
                }
            },
            yAxis: {
                title: {
                    text: 'Overtime  Total  Minutes'
                },
                labels: {
                    format: '{value}'
                }
            },
            tooltip: {
                crosshairs: true,
                shared: true
            },
            plotOptions: {
                spline: {
                    marker: {
                        radius: 4,
                        lineColor: '#666666',
                        lineWidth: 1
                    }
                },
                series: {
                    point: {
                        events: {
                            click: function () {

                                const pointIndex = this.index;
                                const attnDate = data[pointIndex].Attn_Dt;
                                Othrsdetails(attnDate);
                            }
                        }
                    }
                }
            },
            legend: {
                enabled: false // Hide the legend
            },
            series: [{
                name: 'Minutes',
                marker: {
                    symbol: 'circle'
                },
                data: OtHours

            }],

        });
    }

    //Function to render Attration chart
    function renderAttrationChart(resignedmonth, malecount, femalecount, departmentid) {

        function getResponsiveMargin_overview() {
            if (window.innerWidth < 479) { // For small screens
                return 100;
            } else if (window.innerWidth < 768) { // For medium screens
                return 80;
            } else { // For larger screens
                return 50;
            }
        }
        const monthMapping = {
            Jan: 1,
            Feb: 2,
            Mar: 3,
            Apr: 4,
            May: 5,
            Jun: 6,
            Jul: 7,
            Aug: 8,
            Sep: 9,
            Oct: 10,
            Nov: 11,
            Dec: 12
        };

        Highcharts.chart('attrition_chart', {
            chart: {
                type: 'column',
                height: 300, // Set the height of the chart
                marginBottom: getResponsiveMargin_overview(), // Set the bottom margin
                style: {
                    fontFamily: 'Roboto', // Set font family for y-axis labels
                }
            },
            title: {
                text: '',
                align: 'left',
            },
            xAxis: {
                categories: resignedmonth,
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'Number of employees Attrition',
                    x: -10 // Set vertical position of y-axis labels to 0
                },
            },
            legend: {
                align: 'center',
                x: -3,
                verticalAlign: 'bottom',
                y: 20,
                floating: true,
                borderWidth: 0,
                shadow: false,
                itemStyle: {
                    fontSize: '11px', // Set legend font size here
                    fontWeight: 'normal', // Set font weight to normal
                    fontFamily: 'Roboto',
                },
                itemMarginTop: 0, // Vertical space between legend items (top margin)
                itemMarginBottom: 5, // Vertical space between legend items (bottom margin)
                symbolPadding: 0 // Horizontal space between the legend symbol and text
            },
            tooltip: {
                headerFormat: '<b>{point.x}</b><br/>',
                pointFormat: '{series.name}: {point.y}<br/>Total: {point.stackTotal}'
            },
            plotOptions: {
                column: {
                    stacking: 'normal',
                    pointWidth: 10, // Set the column width to 10px
                },
                series: {
                    point: {
                        events: {
                            click: function () {


                                const months = this.category;
                                const month = monthMapping[months];
                                const gender = this.series.name;


                                AttrationChartDetails(month, gender, departmentid);
                            }
                        }
                    }
                }
            },
            series: [{
                name: 'Male',
                data: malecount,
                color: '#7CBEC0', // Set color to green for Present
            }, {
                name: 'Female',
                data: femalecount,
                color: '#DDBDE3',
            }],

        });
    }
    


    //Function for fill department
    $("#clicking").on('click', function (e) {

       
        //$("#clicking .dropdown-menu li").removeClass("active");
        //$(this).parent().addClass("active");
        e.preventDefault();
        $.ajax({
            type: "POST",
            url: "/Transactions/GetDeptbyBranch?BranchId=" + branchId,
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            async: false,
            success: function (response) {

                if (response.isRedirect) {
                    window.location.href = response.redirectUrl;
                }
                if (response != null) {

                    response.sort(function (a, b) {
                        return a.Text.localeCompare(b.Text);
                    });


                    $('#nameList').empty();


                    for (var i = 0; i < response.length; i++) {
                        $('#nameList').append(
                            '<li data-id="' + response[i].Value + '">' +
                            '<a href="#" class="name-link" onclick="return false;" >' + response[i].Text + '</a>' +
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
    });
    // Add the autocomplete filter logic
    $("#myInput").on('keyup', function () {
        const filter = $(this).val().toLowerCase();
        $("#nameList li").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(filter) > -1);
        });
    });

    // this list call departmentwise attration chart.
    $('#nameList').on('click', 'li', function (e) {

        e.stopPropagation();
        $("#nameList li").removeClass("active");
        $(this).addClass("active");

        var selectedDeptId = $(this).data('id');
        $.ajax({
            type: "GET",
            url: `${webApiUrl}DashBoard/GetAdminDashBoardAttrationDetails`,
            data: { cmpId: cmpId, branchId: branchId, reqid: 1, departmentid: selectedDeptId, Gender: 0, Month: 0 },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", token);
            },
            success: function (data) {

                const currentMonth = new Date().getMonth();
                const lastThreeMonths = [
                    (currentMonth - 2 + 12) % 12,
                    (currentMonth - 1 + 12) % 12,
                    currentMonth
                ];


                const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                const selectedMonths = lastThreeMonths.map(index => months[index]);


                const maleCounts = Array(3).fill(0);
                const femaleCounts = Array(3).fill(0);


                data.forEach(item => {
                    const monthIndex = item.resign_month - 1;
                    const targetIndex = lastThreeMonths.indexOf(monthIndex);
                    if (targetIndex !== -1) {
                        maleCounts[targetIndex] = item.male_count;
                        femaleCounts[targetIndex] = item.female_count;
                    }
                });


                renderAttrationChart(selectedMonths, maleCounts, femaleCounts, selectedDeptId);



            },
            error: function () {
                console.log("Error in Attration chart.");
            }
        });




    });





    //Model empty when click on close button
    $("#close_model").on('click', function () {

        removeFieldErrors();
        $('#announcementForm')[0].reset();
        $('#CompanyID').multiselect('clearSelection');
        $('#BranchID').multiselect('clearSelection');
        $('#DepartmentID').multiselect('clearSelection');
    });
    function removeFieldErrors() {


        $('#announcementForm .form-control').removeClass('field_error');  // Remove from input fields
        $('#announcementForm textarea').removeClass('field_error');  // Remove from textarea fields
        $('#announcementForm select').removeClass('field_error');  // Remove from select fields (like dropdowns)
    }

  

});

