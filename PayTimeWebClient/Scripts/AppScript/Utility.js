//function GetCityDropDownData() {
//var myDropDownList = $("#"+ctlid+"");
//$.ajax({
//    type: "GET",
//    url: serurl,
//    contentType: "application/json; charset=utf-8",
//    dataType: "json",
//    beforeSend: function (xhr) {
//        xhr.setRequestHeader("Authorization", tokey)
//    },
//    success: function(data)
//    {
//        $.each(data.d, function (){
//            myDropDownList .append($("<option     />").val(this.KeyName).text(this.ValueName));
//        });
//    },
//    failure: function () {
//        alert("Failed!");
//    }
//});

//var course_data; // variable to hold data in once it is loaded
//$.get('App_Start/timezones.xml', function(data) { // get the courses.xml file
//    course_data = data; // save the data for future use
//    // so we don't have to call the file again
//    var that = $('#timezonesfile'); // that = the courses select
//    $('city', course_data).each(function() { // find courses in data
//        // dynamically create a new option element
//        // make its text the value of the "title" attribute in the XML
//        // and append it to the courses select
//        $('<option>').text($(this).attr('name')).appendTo(that);
//    });
//}, 'xml'); // specify what format the request will return - XML


//$.ajax({
//    type: "GET",
//    url: "../App_Start/City.xml",
//    dataType: "xml",
//    success: function (xml) {
//        var select = $('#drpcity');
//        $(xml).find('City').each(function () {
//            //alert($(xml).find('City').find('name').val());
//            $(this).find('Cities').each(function () {
//                var name = $(this).attr('name');
//                var value = $(this).attr('id');

//                var option = $("<option>" + name + "</option>");
//                option.attr("value", value);
//                select.append(option);
//            });
//        });
//        select.children(":first").text("Please Select City").attr("selected", true);

//    }
//});


//$('#spinner').bind("ajaxSend", function () {
//    $(this).show();
//}).bind("ajaxComplete", function () {
//    $(this).hide();
//});
//$('#CountryID').removeClass("form-control");
//$('#CountryID').addClass("selectpicker");
//$('#CountryID').attr("data-live-search", "true");
//$('#CountryID').attr("size", "false");
//$('#CountryID').attr("data-size", 5);
//$('#CountryID').attr("data-container", "body");
//$('#CountryID').attr("data-width", "100%");


//}




//function IsValidform() {
//    var _branchname = $('#BranchName').val();
//    var _companyid = $('#CompanyID').val();
//    var _Stateid = $('#StateID').val();
//    var _CountryID = $('#CountryID').val();
//    var _CityId = $("#CityId").val();

//}
//(function ($) {
//    // Retain count concept: http://stackoverflow.com/a/2420247/260665
//    // Callers should make sure that for every invocation of loadingSpinner method there has to be an equivalent invocation of removeLoadingSpinner
//    var retainCount = 0;

//    // http://stackoverflow.com/a/13992290/260665 difference between $.fn.extend and $.extend
//    $.extend({
//        loadingSpinner: function () {
//            // add the overlay with loading image to the page
//            var over = '<div id="custom-loading-overlay">' +
//                '<i id="custom-loading" class="fa fa-spinner fa-spin fa-3x fa-fw" style="font-size:48px; color: #470A68;"></i>' +
//                '</div>';
//            if (0 === retainCount) {
//                $(over).appendTo('body');
//            }
//            retainCount++;
//        },
//        removeLoadingSpinner: function () {
//            retainCount--;
//            if (retainCount <= 0) {
//                $('#custom-loading-overlay').remove();
//                retainCount = 0;
//            }
//        }
//    });
//}(jQuery));
function AdminPiachart(series, FirstDayWithSlashes, lastDayWithSlashes, cmpId, branchId, _FirstDayWithSlashes) {
    Highcharts.chart('containerAdmin', {
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: 0,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            text: 'Attendance Summary <br/> ' + _FirstDayWithSlashes//+ ' To ' + lastDayWithSlashes + '  '
        },
        tooltip: {
            pointFormat: '{point.name}: {point.y} '
        },

        plotOptions: {
            series: {
                cursor: 'pointer',
                point: {
                    events: {
                        click: function () {
                            var nm = this.name;
                            var i = this.name;
                            if (i == "Present") {
                                i = "P";
                            }
                            else if (i == "Absent") {
                                i = "A";
                            }
                            else if (i == "Error") {
                                i = "E";
                            }
                            else if (i == "Leave") {
                                i = "Leave";
                            }
                            else if (i == "Weekly Off") {
                                i = "WO";
                            }
                            else if (i == "HalfDay") {
                                i = "HD";
                            }
                            else if (i == "Holiday") {
                                i = "H";
                            }
                            else if (i == "LessHours") {
                                i = "LH";
                            }
                            else if (i == "At Work") {
                                i = "E";
                            }
                            else {
                                i = "";
                            }

                            $.ajax({
                                type: "GET",
                                url: "/PayTime/FillChartAdminDashboard",
                                data: { fromdate: FirstDayWithSlashes, todate: lastDayWithSlashes, statusflg: 2, finalStatus: i, cmpId: cmpId, branchId: branchId },
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data) {
                                    $("#tblattndstatus").empty();
                                    $('#eventContent').modal('show');
                                    $("#statusTitle").text(nm);
                                    var table = "";
                                    if (data != null) {
                                        var convertdata = JSON.parse(data);
                                        for (var j = 0; j < convertdata.length; j++) {
                                            var tbl = "<tr class='odd gradeX'>";
                                            tbl = tbl + "<td hidden>" + convertdata[j].empid;
                                            tbl = tbl + "<td>" + convertdata[j].EmpName;
                                            tbl = tbl + "<td>" + convertdata[j].AttnDate;
                                            tbl = tbl + "</td>";
                                            if (convertdata[j].FinalStatus == "A" || convertdata[j].FinalStatus == "H" || convertdata[j].FinalStatus == "L" || convertdata[j].FinalStatus == "WO") {
                                                tbl = tbl + "<td>00:00";
                                                tbl = tbl + "</td>";
                                                tbl = tbl + "<td>00:00";
                                                tbl = tbl + "</td>";
                                            }
                                            else {
                                                tbl = tbl + "<td>" + convertdata[j].InTime;
                                                tbl = tbl + "</td>";
                                                tbl = tbl + "<td>" + convertdata[j].OutTime;
                                                tbl = tbl + "</td>";

                                            }
                                            tbl = tbl + "<td>" + convertdata[j].FinalStatus;
                                            tbl = tbl + "</td>";
                                            tbl = tbl + "</tr>";
                                            table += tbl;
                                        }
                                    }
                                    $("#tblattndstatus").destroy;
                                    $('#tblattndstatus').append(table);
                                    $('#tblattndstatus').dataTable({
                                        "columnDefs": [
                                  {
                                      "targets": [0],
                                      "visible": false,
                                      "searchable": false,
                                  }],
                                        // "data": data.d,
                                        "destroy": true,
                                        "columns": [
                                                { "title": "ID" },
                                                { "title": "Employee" },
                                                { "title": "Attendance Date" },
                                             { "title": "In Time" },
                                            { "title": "Out Time" },
                                            { "title": "Status" }
                                        ]
                                    });
                                    $(".loading").hide();
                                },
                                beforeSend: function () {
                                    $(".loading").show();
                                },
                                error: function () {
                                    alert("Sorry Something wrong occured.");
                                }
                            });
                        }
                    }
                }
            },
            //setOptions: {
            //    lang: {
            //        noData: 'Personalized no data message'
            //    }
            //},

            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false,
                    format: '<b>{point.name}</b>: {point.y:.0f}'
                },
                showInLegend: true
            }
        },
        series: [{
            name: 'Status',
            minRange : 0.1,
            colorByPoint: true,
            data: series
        }]
    });
}

function Piachart(series, ids, FirstDayWithSlashes, lastDayWithSlashes, searchName, searchBy,webapiurl,tokan) {

    Highcharts.chart('container', {
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: 0,
            plotShadow: false,
            type: 'pie',
            //width: 300, // Set the width in pixels
            height: 360, // Set the height in pixels
        },
        title: {
            text: 'Attendance Summary'
        },
        tooltip: {
            pointFormat: '{point.name}: {point.y} '
        },

        plotOptions: {
            series: {
                cursor: 'pointer',
                point: {
                    events: {
                        click: function () {
                            var k = this.name;
                            var i = this.name;
                            if (i == "Present") {
                                i = "P";
                            }
                            else if (i == "Absent") {
                                i = "A";
                            }
                            else if (i == "Error") {
                                i = "E";
                            }
                            else if (i == "Weekly Off") {
                                i = "WO";
                            }
                            else if (i == "HalfDay") {
                                i = "HD";
                            }
                            else if (i == "Holiday") {
                                i = "H";
                            }
                            else if (i == "LessHours") {
                                i = "LH";
                            }
                            else if (i == "Leave") {
                                i = "L";
                            }
                            else {
                                i = "";
                            }

                            $.ajax({
                                type: "GET",
                                headers: { 'Authorization': tokan },
                                url: webapiurl + "DashBoard/GetEmpAttendanceStatusSummary",
                                data: { empid: ids, fromdate: FirstDayWithSlashes, todate: lastDayWithSlashes, statusflg: 2, finalStatus: i, searchName: searchName, searchBy: searchBy },
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data) {
                                    $("#tblattndstatus").empty();
                                    $('#eventContent').modal('show');
                                    $("#empattndstatus").html(k);
                                    var table = "";
                                    if (data != null) {
                                        var convertdata = data;
                                        for (var j = 0; j < convertdata.length; j++) {
                                            var tbl = "<tr class='odd gradeX'>";
                                            tbl = tbl + "<td>" + convertdata[j].AttnDate;
                                            tbl = tbl + "</td>";
                                            if (convertdata[j].FinalStatus != "A") {
                                                tbl = tbl + "<td>" + convertdata[j].InTime;
                                                tbl = tbl + "</td>";
                                                tbl = tbl + "<td>" + convertdata[j].OutTime;
                                                tbl = tbl + "</td>";
                                            }
                                            else {
                                                tbl = tbl + "<td>00:00";
                                                tbl = tbl + "</td>";
                                                tbl = tbl + "<td>00:00";
                                                tbl = tbl + "</td>";
                                            }
                                            tbl = tbl + "<td>" + convertdata[j].FinalStatus;
                                            tbl = tbl + "</td>";
                                            tbl = tbl + "</tr>";
                                            table += tbl;
                                        }
                                    }
                                    $("#tblattndstatus").destroy;
                                    $('#tblattndstatus').append(table);
                                    $('#tblattndstatus').dataTable({
                                        // "data": data.d,
                                        "destroy": true,
                                        "columns": [
                                                { "title": "Attendance Date" },
                                             { "title": "In Time" },
                                            { "title": "Out Time" },
                                            { "title": "Status" }
                                        ]
                                    });
                                },
                                error: function () {
                                    alert("Sorry Something wrong occured.");
                                }
                            });
                        }
                    }
                }
            },
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false,
                    format: '<b>{point.name}</b>: {point.y:.0f}'
                },
                showInLegend: true
            }
        },

        series: [{
            name: 'Status',
            colorByPoint: true,
            //color: (function () {
            //    $(series).each(function () {
            //        switch (series) {
            //            case 'Absent':
            //                return '#C6F9D2';
            //            case 'Error':
            //                return '#FF0000';
            //            case 'Holiday':
            //                return '#CECEFF';
            //            case 'Present':
            //                return '#FFCAFF';
            //            case 'Weekly Off':
            //                return '#D0CCCD';
            //        }
            //    });
            //}),
            data: series

        }]
    });
}

//function Columnchart() {


//    Highcharts.chart('containercol', {
//        chart: {
//            type: 'column'
//        },
//        title: {
//            text: 'Hours Summary<br/>' + $('#AttendanceDate').text() + ''
//        },
//        subtitle: {
//            text: 'Click the columns to view Hours Summary.</a>.'
//        },
//        xAxis: {
//            type: 'category'
//        },
//        yAxis: {
//            title: {
//                text: 'Hours(HH:MM)'
//            }

//        },
//        legend: {
//            enabled: false
//        },
//        plotOptions: {
//            series: {
//                borderWidth: 0,
//                dataLabels: {
//                    enabled: true,
//                    format: '{point.y:.1f}%'
//                }
//            }
//        },

//        tooltip: {
//            headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
//            pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y:.2f}%</b> of total<br/>'
//        },

//        series: [{
//            name: 'Hours Summary',
//            colorByPoint: true,
//            data: [{
//                name: 'Total Hours',
//                y: 0,
//                drilldown: 'Total Hours'
//            },
//                {
//                    name: 'Early IN',
//                    y: 0,
//                    drilldown: 'Early IN'
//                }, {
//                    name: 'Late IN',
//                    y: 0,
//                    drilldown: 'Late IN'
//                }, {
//                    name: 'Early OUT',
//                    y: 0,
//                    drilldown: 'Early OUT'
//                }, {
//                    name: 'Over Time',
//                    y: 0,
//                    drilldown: 'Over Time'
//                }]
//        }],
//        drilldown: {
//            series: [
//                {
//                    name: 'Total Hours',
//                    id: 'Total Hours',
//                    data: [
//                        [
//                            'Early IN',
//                            0
//                        ],
//                        [
//                            'Late IN',
//                            0
//                        ],
//                        [
//                            'Early OUT',
//                            0
//                        ]
//                    ]
//                }, {
//                    name: 'Early IN',
//                    id: 'Early IN',
//                    data: [
//                        [
//                            'v11.0',
//                            0
//                        ],
//                        [
//                            'v8.0',
//                            0
//                        ],
//                        [
//                            'v9.0',
//                            0
//                        ],
//                        [
//                            'v10.0',
//                            0
//                        ],
//                        [
//                            'v6.0',
//                            0
//                        ],
//                        [
//                            'v7.0',
//                            0
//                        ]
//                    ]
//                }, {
//                    name: 'Late IN',
//                    id: 'Late IN',
//                    data: [
//                        [
//                            'v40.0',
//                            0
//                        ],
//                        [
//                            'v41.0',
//                            0
//                        ],
//                        [
//                            'v42.0',
//                            0
//                        ],
//                        [
//                            'v39.0',
//                            0
//                        ],
//                        [
//                            'v36.0',
//                           0
//                        ],
//                        [
//                            'v43.0',
//                            0
//                        ],
//                        [
//                            'v31.0',
//                            0
//                        ],
//                        [
//                            'v35.0',
//                       0
//                        ],
//                        [
//                            'v38.0',
//                            0
//                        ],
//                        [
//                            'v32.0',
//                         0
//                        ],
//                        [
//                            'v37.0',
//                            0
//                        ],
//                        [
//                            'v33.0',
//                            0
//                        ],
//                        [
//                            'v34.0',
//                            0
//                        ]


//                    ]
//                }, {
//                    name: 'Early OUT',
//                    id: 'Early OUT',
//                    data: [
//                        [
//                            'v35',
//                        0
//                        ],
//                        [
//                            'v36',
//                            0
//                        ],
//                        [
//                            'v37',
//                          0
//                        ],
//                        [
//                            'v34',
//                           0
//                        ],
//                        [
//                            'v38',
//                            0
//                        ],
//                        [
//                            'v31',
//                            0
//                        ],
//                        [
//                            'v33',
//                         0
//                        ],
//                        [
//                            'v32',
//                            0
//                        ]
//                    ]
//                }, {
//                    name: 'Over Time',
//                    id: 'Over Time',
//                    data: [
//                        [
//                            'v8.0',
//                           0
//                        ],
//                        [
//                            'v7.1',
//                           0
//                        ],
//                        [
//                            'v5.1',
//                            0
//                        ],
//                        [
//                            'v5.0',
//                            0
//                        ],
//                        [
//                            'v6.1',
//                            0
//                        ],
//                        [
//                            'v7.0',
//                            0
//                        ],
//                        [
//                            'v6.2',
//                           0
//                        ]
//                    ]
//                }]
//        }
//    });
//}


function EmployeeAtWork(series) {
    Highcharts.chart('EmpAtworkChart', {
        chart: {
            type: 'bar'
        },
        title: {
            text: ''
        },
        //subtitle: {
        //    text: 'Source: <a href="https://en.wikipedia.org/wiki/World_population">Wikipedia.org</a>'
        //},
        xAxis: {
            categories: ['Employee At work', 'Total Strength'],
            title: {
                text: null
            }
        },
        yAxis: {
            title: {
                text: '',
                align: 'high'
            },
            labels: {
                overflow: 'justify'
            }
        },
        //tooltip: {
        //    valueSuffix: ' Employees'
        //},
        plotOptions: {
            bar: {
                dataLabels: {
                    enabled: true
                }
            }
        },
        //legend: {
        //    layout: 'vertical',
        //    align: 'right',
        //    verticalAlign: 'top',
        //    x: -40,
        //    y: 80,
        //    floating: false,
        //    borderWidth: 1,
        //    // backgroundColor: ((Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'),
        //    shadow: true
        //},
        credits: {
            enabled: false
        },
        series: [{
            name: 'Employee',
            data: series
        }]
    });
}


function EmployeeOnLeave(series) {
    //Highcharts.getOptions().plotOptions.pie.colors = ['#492970', '#f28f43', '#c42525'];
    Highcharts.chart('containerleave', {
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: 0,
            plotShadow: false
        },
        title: {
            text: 'Leave<br>Summary',
            align: 'center',
            verticalAlign: 'middle',
            y: 40
        },
        tooltip: {
            pointFormat: '{point.name}: {point.y} '
            //pointFormat: '{series.name}: <b>{point.percentage:.0f}%</b>'
        },
        plotOptions: {
            pie: {
                dataLabels: {
                    enabled: true,
                    distance: -50,
                    style: {
                        fontWeight: 'bold',
                        color: 'white'
                    }
                },
                point: {
                    events: {
                        click: function (event) {
                            var x = this.name;
                            if (x == "Today") {
                                x = 0;
                            }
                            else if (x == "Tomorrow") {
                                x = 1;
                            }
                            else if (x == "Next 7 Days") {
                                x = 2;
                            }
                            else {
                                x = "";
                            }

                            var d = new Date();
                            var day = d.getDate();
                            var month = d.getMonth() + 1;
                            var year = d.getFullYear();
                            if (day < 10) {
                                day = "0" + day;
                            }
                            if (month < 10) {
                                month = "0" + month;
                            }
                            var date = year + "-" + month + "-" + day;


                            $.ajax({
                                type: "GET",
                                url: "/PayTime/FillLeaveChartDetails",
                                data: { LeaveDaysType: x, leavedate: date, cmpId: 0, branchId: 0 },
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data) {
                                    $("#tblleaveemployee").empty();
                                    $('#frmleavedetails').modal('show');
                                    var table = "";
                                    if (data != null) {
                                        var convertdata = JSON.parse(data);
                                        for (var j = 0; j < convertdata.length; j++) {
                                            var tbl = "<tr class='odd gradeX'>";
                                            tbl = tbl + "<td>" + convertdata[j].Empcode;
                                            tbl = tbl + "</td>";
                                            tbl = tbl + "<td>" + convertdata[j].Name;
                                            tbl = tbl + "</td>";
                                            tbl = tbl + "<td>" + convertdata[j].PunchID;
                                            tbl = tbl + "</td>";
                                            tbl = tbl + "<td>" + convertdata[j].Department;
                                            tbl = tbl + "</td>";
                                            tbl = tbl + "<td>" + convertdata[j].Designation;
                                            tbl = tbl + "</td>";
                                            tbl = tbl + "<td>" + convertdata[j].Leave;
                                            tbl = tbl + "</td>";
                                            tbl = tbl + "<td>" + convertdata[j].FromDate;
                                            tbl = tbl + "</td>";
                                            tbl = tbl + "<td>" + convertdata[j].ToDate;
                                            tbl = tbl + "</td>";
                                            tbl = tbl + "<td>" + convertdata[j].Reason;
                                            tbl = tbl + "</td>";
                                            tbl = tbl + "<td>" + convertdata[j].Company;
                                            tbl = tbl + "</td>";
                                            tbl = tbl + "<td>" + convertdata[j].Branch;
                                            tbl = tbl + "</td>";
                                            tbl = tbl + "</tr>";
                                            table += tbl;
                                        }
                                    }
                                    $("#tblleaveemployee").destroy;
                                    $('#tblleaveemployee').append(table);
                                    $('#tblleaveemployee').dataTable({
                                        // "data": data.d,
                                        "destroy": true,
                                        "columns": [
                                                { "title": "Emp Code" },
                                                { "title": "Employee" },
                                                { "title": "Punch ID" },
                                             { "title": "Department" },
                                            { "title": "Designation" },
                                            { "title": "Leave" },
                                               { "title": "FromDate" },
                                                  { "title": "ToDate" },
                                                     { "title": "Reason" },
                                                     { "title": "Company" },
                                                     { "title": "Branch" }
                                        ]
                                    });
                                },
                                error: function () {
                                    alert("Sorry Something wrong occured.");
                                }
                            });

                        }
                    }
                },
                startAngle: -90,
                endAngle: 90,
                center: ['50%', '75%']
            }
        },
        series: [{
            type: 'pie',
            name: 'Employee On Leave',
            innerSize: '50%',
            data: series
        }]
    });
}


function HoursSummaryChart(series) {
    Highcharts.chart('containercol', {
        chart: {
            type: 'column'
        },
        title: {
            text: 'Hours Summary'
        },
        xAxis: {
            categories: ['Late IN', 'Early OUT']
        },
        yAxis: {
            min: 0,
            title: {
                text: 'Minutes'
            },
            stackLabels: {
                enabled: true,
                style: {
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        },
        legend: {
            align: 'right',
            x: -30,
            verticalAlign: 'top',
            y: 25,
            floating: true,
            backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || 'white',
            borderColor: '#CCC',
            borderWidth: 1,
            shadow: false
        },
        tooltip: {
            headerFormat: '<b>{point.x}</b><br/>',
            pointFormat: '{series.name}: {point.y}'
        },
        plotOptions: {
            column: {
                stacking: 'normal',
                dataLabels: {
                    enabled: true,
                    color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'white'
                }
            }
        },
        series: [{
            name: 'Minutes',
            data: series
        }]
    });
}


//function BarChart() {
//    Highcharts.chart('containerbar', {
//        chart: {
//            type: 'bar'
//        },
//        title: {
//            text: 'Employee At Work'
//        },
//        subtitle: {
//            text: '' + $('#AttendanceDate').text() + ''
//        },
//        xAxis: {
//            categories: ['Year-2015', 'Year-2016', 'Year-2017'],
//            title: {
//                text: null
//            }
//        },
//        yAxis: {
//            min: 0,
//            title: {
//                text: 'Employee Strength',
//                align: 'high'
//            },
//            labels: {
//                overflow: 'justify'
//            }
//        },
//        tooltip: {
//            valueSuffix: ' Strength'
//        },
//        plotOptions: {
//            bar: {
//                dataLabels: {
//                    enabled: true
//                }
//            }
//        },
//        legend: {
//            layout: 'vertical',
//            align: 'right',
//            verticalAlign: 'top',
//            x: -40,
//            y: 80,
//            floating: true,
//            borderWidth: 1,
//            backgroundColor: ((Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'),
//            shadow: true
//        },
//        credits: {
//            enabled: true
//        },
//        series: [{
//            name: 'Employee Strength',
//            data: [0, 0, 0]
//        }, {
//            name: 'Employee at Work',
//            data: [0, 0, 0]
//        }]
//    });
//}

//function ActivityGauge() {
//    Highcharts.chart('containerleave', {

//        chart: {
//            type: 'solidgauge',
//            marginTop: 50
//        },

//        title: {
//            text: 'Leave Summary',
//            style: {
//                fontSize: '24px'
//            }
//        },

//        tooltip: {
//            borderWidth: 0,
//            backgroundColor: 'none',
//            shadow: false,
//            style: {
//                fontSize: '16px'
//            },
//            pointFormat: '{series.name}<br><span style="font-size:2em; color: {point.color}; font-weight: bold">{point.y}</span>',
//            positioner: function (labelWidth) {
//                return {
//                    x: 200 - labelWidth / 2,
//                    y: 180
//                };
//            }
//        },

//        pane: {
//            startAngle: 0,
//            endAngle: 360,
//            background: [{ // Track for Today
//                outerRadius: '112%',
//                innerRadius: '88%',
//                backgroundColor: Highcharts.Color(Highcharts.getOptions().colors[0]).setOpacity(0.3).get(),
//                borderWidth: 0

//            }, { // Track for Tomorrow
//                outerRadius: '87%',
//                innerRadius: '63%',
//                backgroundColor: Highcharts.Color(Highcharts.getOptions().colors[1]).setOpacity(0.3).get(),
//                borderWidth: 0
//            }, { // Track for Next 7 Days
//                outerRadius: '62%',
//                innerRadius: '38%',
//                backgroundColor: Highcharts.Color(Highcharts.getOptions().colors[2]).setOpacity(0.3).get(),
//                borderWidth: 0

//            }]
//        },

//        yAxis: {
//            min: 0,
//            max: 100,
//            lineWidth: 0,
//            tickPositions: []
//        },

//        plotOptions: {
//            solidgauge: {
//                series: {
//                    dataLabels: {
//                        enabled: false,
//                        format: '{series.name}',
//                    }
//                },

//                linecap: 'round',
//                stickyTracking: false,
//                rounded: true
//            },
//            showInLegend: true,
//        },

//        series: [{
//            name: 'Today',
//            borderColor: Highcharts.getOptions().colors[0],
//            data: [{
//                color: Highcharts.getOptions().colors[0],
//                radius: '112%',
//                innerRadius: '88%',
//                y: 0
//            }]
//        }, {
//            name: 'Tomorrow',
//            borderColor: Highcharts.getOptions().colors[1],
//            data: [{
//                color: Highcharts.getOptions().colors[1],
//                radius: '87%',
//                innerRadius: '63%',
//                y: 0

//            }]
//        }, {
//            name: 'Next 7 Days',
//            borderColor: Highcharts.getOptions().colors[2],
//            data: [{
//                color: Highcharts.getOptions().colors[2],
//                radius: '62%',
//                innerRadius: '38%',
//                y: 0
//            }]
//        }]
//    },

//        /**
//         * In the chart load callback, add icons on top of the circular shapes
//         */
//        function callback() {

//            // Move icon
//            this.renderer.path(['M', -8, 0, 'L', 8, 0, 'M', 0, -8, 'L', 8, 0, 0, 8])
//                .attr({
//                    'stroke': '#303030',
//                    'stroke-linecap': 'round',
//                    'stroke-linejoin': 'round',
//                    'stroke-width': 2,
//                    'zIndex': 10
//                })
//        .translate(190, 26)
//        .add(this.series[2].group);

//            // Exercise icon
//            this.renderer.path(['M', -8, 0, 'L', 8, 0, 'M', 0, -8, 'L', 8, 0, 0, 8, 'M', 8, -8, 'L', 16, 0, 8, 8])
//                .attr({
//                    'stroke': '#303030',
//                    'stroke-linecap': 'round',
//                    'stroke-linejoin': 'round',
//                    'stroke-width': 2,
//                    'zIndex': 10
//                })
//                .translate(190, 61)
//                .add(this.series[2].group);

//            // Stand icon
//            this.renderer.path(['M', 0, 8, 'L', 0, -8, 'M', -8, 0, 'L', 0, -8, 8, 0])
//                .attr({
//                    'stroke': '#303030',
//                    'stroke-linecap': 'round',
//                    'stroke-linejoin': 'round',
//                    'stroke-width': 2,
//                    'zIndex': 10
//                })
//                .translate(190, 96)
//                .add(this.series[2].group);
//        });

//}

function validate(uname, password) {
    if (uname == "Admin" && password=="Admin@123") {
        return true;
    }
    else {
        return false;
    }
}