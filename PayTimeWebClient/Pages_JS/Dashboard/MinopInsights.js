/* Presentation-only analytics. Observes existing jQuery responses; makes no requests. */
(function (root) {
    'use strict';
    function count(value) {
        if (typeof value !== 'number' && typeof value !== 'string') return null;
        if (typeof value === 'string' && !/^\d+$/.test(value.trim())) return null;
        var n = Number(value);
        return Number.isSafeInteger(n) && n >= 0 ? n : null;
    }
    function summarize(values) {
        var checked = count(values[1]), missing = count(values[6]);
        var total = checked === null || missing === null ? null : checked + missing;
        if (total !== null && !Number.isSafeInteger(total)) total = null;
        return { checked: checked, missing: missing, late: count(values[3]), early: count(values[4]),
            total: total, rate: total > 0 ? Math.round(checked / total * 1000) / 10 : null };
    }
    /* Each refresh starts with reqid 1 in the existing dashboard. Track exact xhrs
       so a slower response from an older filter selection can never enter the new view. */
    function createCycle() {
        var state = { scope: null, pending: {}, values: {} };
        return {
            start: function (scope, id, xhr) {
                if (id === 1 || scope !== state.scope) state = { scope: scope, pending: {}, values: {} };
                state.scope = scope; state.pending[id] = xhr; delete state.values[id];
            },
            finish: function (scope, id, xhr, value) {
                if (scope !== state.scope || state.pending[id] !== xhr) return false;
                delete state.pending[id]; state.values[id] = count(value); return true;
            },
            snapshot: function () { return { values: Object.assign({}, state.values), loading: Object.keys(state.pending).length > 0 }; }
        };
    }
    if (typeof module === 'object' && module.exports) { module.exports = { count: count, summarize: summarize, createCycle: createCycle }; return; }
    var panel = document.querySelector('[data-mc-insights]');
    if (!panel || !root.jQuery) return;
    var $ = root.jQuery, cycle = createCycle();
    function request(settings) {
        try {
            var url = new URL(settings.url, location.href);
            if (!/\/DashBoard\/GetActivityMonitorCounts\/?$/i.test(url.pathname)) return null;
            var params = new URLSearchParams(typeof settings.data === 'string' ? settings.data : settings.data || '');
            url.searchParams.forEach(function (v, k) { if (!params.has(k)) params.set(k, v); });
            var id = Number(params.get('reqid'));
            if ([1, 6, 3, 4].indexOf(id) === -1 || !params.has('cmpId') || !params.has('branchId')) return null;
            return { id: id, scope: params.get('cmpId') + '/' + params.get('branchId') };
        } catch (_) { return null; }
    }
    function write(name, value) { panel.querySelector('[data-mc-value="' + name + '"]').textContent = value; }
    function number(n) { return n === null ? '—' : n.toLocaleString(); }
    function render() {
        var snapshot = cycle.snapshot(), m = summarize(snapshot.values), loading = snapshot.loading;
        write('rate', m.rate === null ? '—' : m.rate.toFixed(1) + '%');
        write('missing', number(m.missing)); write('late', number(m.late)); write('early', number(m.early));
        write('headline', m.total === null ? (loading ? 'Updating your workforce view' : 'Attendance data unavailable') : m.total === 0 ? 'No employees reported' : number(m.checked) + ' checked in');
        write('summary', m.total === null ? 'Waiting for both check-in counts from the same selection.' : m.total === 0 ? 'The current selection returned zero checked-in and zero not-checked-in employees.' : number(m.missing) + ' not checked in, from ' + number(m.total) + ' reported across both counts.');
        panel.querySelector('[data-mc-ring]').style.setProperty('--mc-share', (m.rate || 0) + '%');
        panel.querySelector('[data-mc-ring]').setAttribute('aria-label', m.rate === null ? 'Check-in share unavailable' : 'Check-in share ' + m.rate + ' percent');
        var complete = [m.checked, m.missing, m.late, m.early].every(function (n) { return n !== null; });
        panel.querySelector('[data-mc-status]').textContent = loading ? 'Updating current selection…' : complete ? 'Based on the latest loaded counts. Change the dashboard filters to refresh.' : 'Some counts are unavailable. No estimates are shown.';
        [['notcheckedinbtn',m.missing],['lateclockinbtn',m.late],['earlyclockoutbtn',m.early]].forEach(function (pair) {
            panel.querySelector('[data-mc-review="' + pair[0] + '"]').disabled = loading || pair[1] === null || !document.getElementById(pair[0]);
        });
    }
    $(document).on('ajaxSend.minopInsights', function (_, xhr, settings) {
        var r = request(settings); if (!r) return;
        cycle.start(r.scope, r.id, xhr); render();
    }).on('ajaxComplete.minopInsights', function (_, xhr, settings) {
        var r = request(settings); if (!r) return;
        var data = xhr.responseJSON;
        if (!data && xhr.responseText) { try { data = JSON.parse(xhr.responseText); } catch (_) { data = null; } }
        var value = xhr.status >= 200 && xhr.status < 300 && Array.isArray(data) && data.length && data[0] ? data[0].Rescnt : null;
        if (cycle.finish(r.scope, r.id, xhr, value)) render();
    });
    panel.addEventListener('click', function (event) {
        var button = event.target.closest('[data-mc-review]');
        if (!button || button.disabled) return;
        var target = document.getElementById(button.getAttribute('data-mc-review'));
        if (target) target.click();
    });
}(typeof window !== 'undefined' ? window : this));
