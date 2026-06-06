/*
  MinopCloud Admin Dashboard V3 chart polish.
  Frontend-only: updates already-rendered Highcharts instances and responsive reflow.
*/
(function (window, document, $) {
  'use strict';

  var palette = {
    purple: '#6d5dfc',
    blue: '#4f7cff',
    green: '#21a36a',
    red: '#ef5b5b',
    orange: '#f59e0b',
    cyan: '#18a7c8',
    pink: '#d75a95',
    slate: '#aab2c0',
    grid: '#edf0f5',
    muted: '#7b8494',
    ink: '#172033'
  };

  function commonOptions() {
    return {
      chart: {
        backgroundColor: 'transparent',
        style: { fontFamily: 'Poppins, sans-serif' },
        spacingTop: 8,
        spacingRight: 8,
        spacingBottom: 8,
        spacingLeft: 8,
        animation: { duration: 450 }
      },
      credits: { enabled: false },
      exporting: { enabled: false },
      title: { style: { color: palette.ink, fontWeight: '400', fontSize: '14px' } },
      subtitle: { style: { color: palette.muted, fontWeight: '300', fontSize: '10px' } },
      xAxis: {
        lineColor: '#dfe4ec',
        tickColor: '#dfe4ec',
        labels: { style: { color: palette.muted, fontSize: '10px', fontWeight: '300' } },
        title: { style: { color: palette.muted, fontSize: '10px', fontWeight: '300' } }
      },
      yAxis: {
        gridLineColor: palette.grid,
        gridLineDashStyle: 'Dash',
        lineColor: '#dfe4ec',
        tickColor: '#dfe4ec',
        labels: { style: { color: palette.muted, fontSize: '10px', fontWeight: '300' } },
        title: { style: { color: palette.muted, fontSize: '10px', fontWeight: '300' } }
      },
      legend: {
        itemStyle: { color: palette.muted, fontSize: '10px', fontWeight: '300' },
        itemHoverStyle: { color: palette.ink },
        itemHiddenStyle: { color: '#c8ced8' },
        symbolRadius: 5,
        symbolHeight: 8,
        symbolWidth: 8,
        itemDistance: 15
      },
      tooltip: {
        backgroundColor: 'rgba(23,32,51,.96)',
        borderColor: 'transparent',
        borderRadius: 10,
        shadow: false,
        style: { color: '#ffffff', fontSize: '10px', fontWeight: '300' }
      },
      plotOptions: {
        series: {
          animation: { duration: 500 },
          states: { inactive: { opacity: .42 }, hover: { halo: { size: 7, opacity: .08 } } },
          marker: { lineWidth: 2, lineColor: '#ffffff', radius: 4 }
        },
        column: {
          borderWidth: 0,
          borderRadius: 6,
          groupPadding: .16,
          pointPadding: .08,
          maxPointWidth: 22
        },
        bar: {
          borderWidth: 0,
          borderRadius: 6,
          groupPadding: .16,
          pointPadding: .08,
          maxPointWidth: 22
        },
        pie: {
          borderWidth: 0,
          innerSize: '66%',
          size: '86%',
          dataLabels: { enabled: false },
          showInLegend: true,
          states: { hover: { brightness: .04 } }
        },
        spline: {
          lineWidth: 3,
          marker: { enabled: true, radius: 4, lineWidth: 2, lineColor: '#ffffff' }
        },
        areaspline: {
          lineWidth: 3,
          marker: { enabled: true, radius: 4, lineWidth: 2, lineColor: '#ffffff' },
          fillOpacity: .12
        }
      }
    };
  }

  function chartByContainerId(id) {
    if (!window.Highcharts || !Array.isArray(window.Highcharts.charts)) return null;
    return window.Highcharts.charts.find(function (chart) {
      return chart && chart.renderTo && chart.renderTo.id === id;
    }) || null;
  }

  function applySeriesColors(chart, colors) {
    if (!chart || !chart.series) return;
    chart.series.forEach(function (series, index) {
      var color = colors[index % colors.length];
      try {
        series.update({ color: color, lineColor: color }, false);
      } catch (e) { }
    });
  }

  function modernizeChart(chart) {
    if (!chart || !chart.renderTo) return;
    var id = chart.renderTo.id;

    try {
      chart.update(commonOptions(), false, true, false);

      if (id === 'overview_chart') {
        chart.update({
          chart: { type: 'column', height: 280 },
          colors: [palette.green, palette.red, palette.orange, '#8b74e8', palette.blue, palette.slate],
          legend: { align: 'left', verticalAlign: 'bottom', floating: false, y: 0 },
          plotOptions: { column: { stacking: 'normal', borderRadius: 5, maxPointWidth: 18 } }
        }, false, true, false);
        applySeriesColors(chart, [palette.green, palette.red, palette.orange, '#8b74e8', palette.blue, palette.slate]);
      } else if (id === 'Punchtyp_chart') {
        chart.update({
          chart: { type: 'pie', height: 270 },
          colors: [palette.purple, palette.green, palette.orange, palette.blue],
          legend: { align: 'center', verticalAlign: 'bottom', layout: 'horizontal' },
          plotOptions: { pie: { innerSize: '68%', size: '82%' } }
        }, false, true, false);
      } else if (id === 'Presentpattern_chart') {
        chart.update({
          chart: { height: 270 },
          colors: [palette.purple, palette.blue, palette.green],
          legend: { align: 'center', verticalAlign: 'bottom', layout: 'horizontal' }
        }, false, true, false);
      } else if (id === 'OT_chart') {
        chart.update({
          chart: { type: 'areaspline', height: 285 },
          colors: [palette.purple],
          legend: { enabled: false },
          plotOptions: { areaspline: { fillColor: 'rgba(109,93,252,.12)', threshold: null } }
        }, false, true, false);
        applySeriesColors(chart, [palette.purple]);
      } else if (id === 'attrition_chart') {
        chart.update({
          chart: { type: 'areaspline', height: 285 },
          colors: [palette.purple, palette.pink],
          plotOptions: { areaspline: { fillOpacity: .10 } }
        }, false, true, false);
        applySeriesColors(chart, [palette.purple, palette.pink]);
      } else if (id === 'genderwise_chart') {
        chart.update({
          chart: { type: 'column', height: 285 },
          colors: [palette.blue, palette.pink, '#8b74e8', palette.orange],
          legend: { align: 'left', verticalAlign: 'bottom' },
          plotOptions: { column: { borderRadius: 6, maxPointWidth: 18 } }
        }, false, true, false);
        applySeriesColors(chart, [palette.blue, palette.pink, '#8b74e8', palette.orange]);
      } else {
        chart.update({ chart: { height: chart.chartHeight || 285 } }, false, true, false);
      }

      chart.redraw(false);
      chart.reflow();
      chart.renderTo.setAttribute('data-minop-v3', 'true');
    } catch (e) {
      if (window.console && console.warn) console.warn('Minop chart styling skipped for', id, e);
    }
  }

  function modernizeAllCharts() {
    if (!window.Highcharts || !Array.isArray(window.Highcharts.charts)) return;
    window.Highcharts.charts.forEach(function (chart) {
      if (chart && chart.renderTo && chart.renderTo.getAttribute('data-minop-v3') !== 'true') {
        modernizeChart(chart);
      }
    });
  }

  function reflowCharts() {
    if (!window.Highcharts || !Array.isArray(window.Highcharts.charts)) return;
    window.Highcharts.charts.forEach(function (chart) {
      if (chart && chart.reflow) chart.reflow();
    });
  }

  function fixDashboardIcons() {
    document.querySelectorAll('#dashboard-sortable-row .card_header img, #Activity-1 .card_header img, #Activity-1 .activity_block img, #pendingrequest_6 .pr_detail img').forEach(function (img) {
      img.style.display = 'block';
      img.style.visibility = 'visible';
      img.style.opacity = '1';
      img.addEventListener('error', function () {
        this.style.visibility = 'hidden';
        this.parentElement && this.parentElement.classList.add('minop-icon-fallback');
      }, { once: true });
    });
  }

  function initDashboardV3() {
    if (!document.getElementById('dashboard-sortable-row')) return;

    fixDashboardIcons();
    modernizeAllCharts();

    var observer = new MutationObserver(function () {
      window.clearTimeout(observer._timer);
      observer._timer = window.setTimeout(function () {
        fixDashboardIcons();
        modernizeAllCharts();
        reflowCharts();
      }, 120);
    });

    observer.observe(document.getElementById('dashboard-sortable-row'), {
      childList: true,
      subtree: true,
      attributes: true,
      attributeFilter: ['style', 'class']
    });

    var timer = 0;
    window.addEventListener('resize', function () {
      window.clearTimeout(timer);
      timer = window.setTimeout(reflowCharts, 160);
    });

    document.addEventListener('shown.bs.dropdown', reflowCharts);
    document.addEventListener('shown.bs.modal', reflowCharts);

    [500, 1200, 2500, 4500].forEach(function (delay) {
      window.setTimeout(function () {
        modernizeAllCharts();
        reflowCharts();
      }, delay);
    });
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initDashboardV3);
  } else {
    initDashboardV3();
  }
})(window, document, window.jQuery);
