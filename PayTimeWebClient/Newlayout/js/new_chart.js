// =========================== Attendance Overview Chart : Begin ===========================

// This condition for legend adjust in responsive screen
function getResponsiveMargin_overview() {
    if (window.innerWidth < 479) { // For small screens
        return 100;
    } else if (window.innerWidth < 768) { // For medium screens
        return 80;
    } else { // For larger screens
        return 50;
    }
}

Highcharts.chart('overview_chart', {
    chart: {
        type: 'column',
        height: 275, // Set the height of the chart
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
        categories: ['14-Oct', '15-Oct', '16-Oct', '17-Oct', '18-Oct', '19-Oct', '20-Oct'],
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
        },
    },
    series: [{
        name: 'Present',
        data: [260, 250, 230, 200, 255, 240, 200],
        color: '#49B196', // Set color to green for Present
    }, {
        name: 'Absent',
        data: [50, 20, 30, 26, 22, 30, 22],
        color: '#D35A5A',
    }, {
        name: 'On Leave',
        data: [40, 30, 25, 35, 28, 30, 26],
        color: '#F87716',
    }, {
        name: 'Holiday',
        data: [20, 25, 30, 34, 25, 30, 28],
        color: '#A572B9',
    }, {
        name: 'Miss Punch',
        data: [30, 35, 25, 21, 30, 28, 25],
        color: '#7F99DB',
    }, {
        name: 'Weekoff',
        data: [32, 20, 28, 30, 22, 45, 30],
        color: '#BBBBBB',
    }],
    exporting: {
        buttons: {
            contextButton: {
                enabled: false // Disable the context button (export button)
            }
        }
    }
});
// =========================== Attendance Overview Chart : End ===========================



// =========================== Punchtyp Chart : Begin ===========================
Highcharts.chart('Punchtyp_chart', {
    chart: {
        plotBackgroundColor: null,
        plotBorderWidth: null,
        plotShadow: false,
        type: 'pie',
        width: 220,  // Fixed width
        height: 320 , // Fixed height
        style: {
            fontFamily: 'Roboto', // Set font family for y-axis labels
        }
    },
    title: {
        text: '',
        align: 'left'
    },
    tooltip: {
        pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
    },
    accessibility: {
        point: {
            valueSuffix: '%'
        }
    },
    legend: {
        itemStyle: {
            fontSize: '13px',
            fontWeight: 'normal' // Set font weight to normal
        },
        itemMarginTop: 5,   // Set top margin (padding)
        itemMarginBottom: 5  // Set bottom margin (padding)
    },
    plotOptions: {
        pie: {
            allowPointSelect: true,
            cursor: 'pointer',
            dataLabels: {
                enabled: false
            },
            showInLegend: true
        }
    },
    series: [{
        name: 'Percentage',
        colorByPoint: true,
        data: [{
            name: 'Device (250)',
            y: 70,
            //sliced: true,
            selected: true,
            color: '#7CA2CE',
        }, {
            name: 'Web (20)',
            y: 20,
            color: '#A18FB9',
        }, {
            name: 'Mobile (30)',
            y: 10,
            color: '#B5CC88',
        }]
    }],
    exporting: {
        buttons: {
            contextButton: {
                enabled: false // Disable the context button (export button)
            }
        }
    }
});
// =========================== Punchtyp Chart : End ===========================



// =========================== Present Patterns Chart : Begin ===========================
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
        width: 220,  // Fixed width
        height: 320 , // Fixed height
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
                y: 80
            };
        }
    },
    pane: {
        startAngle: 0,
        endAngle: 360,
        background: [{
            outerRadius: '112%', innerRadius: '88%', backgroundColor: '#e7eaed', borderWidth: 0
        }, {
            outerRadius: '87%',  innerRadius: '63%', backgroundColor: '#c6cde3', borderWidth: 0
        }, {
            outerRadius: '62%',  innerRadius: '38%', backgroundColor: '#cdecde', borderWidth: 0
        }]
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
            y: 80
        }],
        showInLegend: true
    }, {
        name: 'Monthly',
        data: [{
            color: '#8399d9',
            radius: '87%',
            innerRadius: '63%',
            y: 65
        }],
        showInLegend: true
    }, {
        name: 'Yearly',
        data: [{
            color: '#8ed8b7',
            radius: '62%',
            innerRadius: '38%',
            y: 50
        }],
        showInLegend: true
    }],
    exporting: {
        buttons: {
            contextButton: {
                enabled: false // Disable the context button (export button)
            }
        }
    }
});
// =========================== Present Patterns Chart : End ===========================



// =========================== Genderwise Chart : Begin ===========================
Highcharts.chart('genderwise_chart', {
    chart: {
        type: 'column',
        height: 300, // Set the height of the chart
        style: {
            fontFamily: 'Roboto', // Set font family for y-axis labels
        }
    },
    title: {
        text: '',
        align: 'left'
    },
    xAxis: {
        categories: ['Ahmedabad', 'Delhi', 'Banglore', 'Pune', 'Vijaywada'],
        crosshair: true,
        accessibility: {
            description: 'Countries'
        }
    },
    yAxis: {
        min: 0,
        title: {
            text: 'Total Gender Count'
        }
    },
    legend: {
        itemStyle: {
            fontSize: '11px', // Set legend font size here
            fontWeight: 'normal', // Set font weight to normal
            fontFamily: 'Roboto',
        }
    },
    tooltip: {
        valueSuffix: ''
    },
    plotOptions: {
        column: {
            pointPadding: 0.90,
            borderWidth: 0,
            borderRadius: 3, // Set column border radius
            pointWidth: 8 // Set the column width to 8px
        }
    },
    series: [
        {
            name: 'Male',
            data: [200, 100, 125, 180, 175],
            color: '#7CBEC0',
        },
        {
            name: 'Female',
            data: [170, 130, 145, 160, 165],
            color: '#DDBDE3',
        }
    ],
    exporting: {
        buttons: {
            contextButton: {
                enabled: false // Disable the context button (export button)
            }
        }
    }
});
// =========================== Genderwise Chart : End ===========================



// =========================== Announcement Slider : Begin ===========================
$(document).ready(function () {
    $('.announcement-slider').each(function () {
        var $slider = $(this);
        var currentSlide = 0;
        var $slides = $slider.find('.announcement-slide');
        var $dots = $slider.find('.dot');

        // Set interval for automatic sliding
        setInterval(function () {
            currentSlide = (currentSlide + 1) % $slides.length;
            $slides.removeClass('active').eq(currentSlide).addClass('active');
            $dots.removeClass('active').eq(currentSlide).addClass('active');
        }, 6000); // Change slide every 5 seconds

        // Handle dot click
        $dots.click(function () {
            currentSlide = $(this).index();
            $slides.removeClass('active').eq(currentSlide).addClass('active');
            $dots.removeClass('active').eq(currentSlide).addClass('active');
        });
    });
});
// =========================== Announcement Slider : End ===========================



// =========================== OT Chart : Begin ===========================
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
        categories: ['21 Oct', '22 Oct', '23 Oct', '24 Oct', '25 Oct'],
        accessibility: {
            description: 'Months of the year'
        }
    },
    yAxis: {
        title: {
            text: 'Overtime  Total  Hours'
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
        }
    },
    legend: {
        enabled: false // Hide the legend
    },
    series: [{
        name: 'Hours',
        marker: {
            symbol: 'circle'
        },
        data: [80, 140, 100, 120, 80]

    }],
    exporting: {
        buttons: {
            contextButton: {
                enabled: false // Disable the context button (export button)
            }
        }
    }
});
// =========================== OT Chart : End =============================



// =========================== Leavetype Bifurcation Chart : Begin ===========================
am4core.useTheme(am4themes_animated);
var chart = am4core.create("chartdiv", am4charts.XYChart);
var data = [];
var value = 500;
var data = [
    { category: "BL", value: 11 },
    { category: "BR", value: 18 },
    { category: "CL", value: 50 },
    { category: "CO", value: 10 },
    { category: "EL", value: 5 },
    { category: "LL", value: 8 },
    { category: "ML", value: 3 },
    { category: "OD", value: 30 },
    { category: "SL", value: 32 },
    { category: "TL", value: 22 },
    { category: "WL", value: 14 }
];

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
categoryAxis.title.fontSize = 12; // Optional: set title font size
categoryAxis.title.fill = am4core.color("#666666"); // Set title color to red

// Set axis labels color and font
categoryAxis.renderer.labels.template.fill = am4core.color("#666666"); // Set labels color to red

categoryAxis.renderer.labels.template.adapter.add("dx", function(dx, target) {
    return -target.maxRight / 2;
})



//////////////////// Configure the Y Axis ////////////////////
var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
valueAxis.tooltip.disabled = true;
valueAxis.renderer.ticks.template.disabled = true;
valueAxis.renderer.axisFills.template.disabled = true;

// Set y-axis title
valueAxis.title.text = "Leave Count";
valueAxis.title.fontSize = 12; // Optional: set title font size
valueAxis.title.fill = am4core.color("#666666"); // Set title color to red

// Set axis labels color and font
valueAxis.renderer.labels.template.fill = am4core.color("#666666"); // Set labels color to red

var series = chart.series.push(new am4charts.ColumnSeries());
series.dataFields.categoryX = "category";
series.dataFields.valueY = "value";
series.tooltipText = "[font-size:11px]Leave Type \n{categoryX}: [bold]{valueY}"; // Custom tooltip text
series.sequencedInterpolation = true;
series.fillOpacity = 0;
series.strokeOpacity = 1;
series.strokeDashArray = "1,3";
series.columns.template.width = 0.01;

// Set the column fill color
series.columns.template.fill = am4core.color("#49B196"); // Set column color to #49B196
series.columns.template.stroke = am4core.color("#49B196"); // Optional: Set column stroke color if needed

// Create tooltip
series.tooltip.getFillFromObject = false; // Prevent using the default fill color
series.tooltip.background.fill = am4core.color("#ffffff"); // Set tooltip background color to white
series.tooltip.label.fill = am4core.color("#333333"); // Set text color to #333333

// Set tooltip border properties
series.tooltip.background.stroke = am4core.color("#7cb5ec"); // Set border stroke color
series.tooltip.background.strokeWidth = 1; // Set stroke width
series.tooltip.background.cornerRadius = 3; // Optional: Set corner radius

// Create bullet and set color
var bullet = series.bullets.create(am4charts.CircleBullet);
bullet.circle.fill = am4core.color("#49B196"); // Set circle color to #49B196

chart.cursor = new am4charts.XYCursor();
chart.cursor.lineX.disabled = true;  // Disable X-axis line on hover
chart.cursor.lineY.disabled = true;  // Disable Y-axis line on hover
chart.cursor.tooltipText = ""; // Ensure cursor tooltip is empty
chart.logo.disabled = true; //For logo hidden at bottom

// =========================== Leavetype Bifurcation Chart : End =============================
