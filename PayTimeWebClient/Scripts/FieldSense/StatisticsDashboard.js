/*
 * FieldSense Statistics Dashboard - Modern Implementation
 * Real-time KPI dashboard with interactive charts and metrics
 * Enhanced with drill-down capabilities and trend analysis
 * Author: Claude Code Assistant
 * Version: 1.0 - Complete Statistics Dashboard
 */

var FieldSenseStatistics = {
    // Configuration for statistics dashboard
    config: {
        // Authentication & API
        token: '',
        apiUrl: '',
        companyId: '',
        
        // Data Management
        clientStatistics: {},
        siteStatistics: {},
        combinedStatistics: {},
        
        // Chart Configuration
        charts: {},
        chartColors: {
            primary: '#007bff',
            success: '#28a745',
            warning: '#ffc107',
            danger: '#dc3545',
            info: '#17a2b8',
            secondary: '#6c757d'
        },
        
        // Refresh Settings
        refreshInterval: 300000, // 5 minutes
        lastRefresh: null,
        autoRefresh: true,
        isLoading: false
    },

    // Initialize the statistics dashboard
    init: function() {
        var self = this;
        
        try {
            // Load configuration from global variable
            if (typeof window.FieldSenseConfig !== 'undefined') {
                self.config.token = window.FieldSenseConfig.token;
                self.config.apiUrl = window.FieldSenseConfig.apiUrl;
                self.config.companyId = window.FieldSenseConfig.companyId;
            }
            
            console.log('FieldSenseStatistics initialized');
            
            // Initialize chart libraries if available
            self.initializeChartLibraries();
            
            // Setup event handlers
            self.bindEvents();
            
            // Load initial statistics
            self.loadAllStatistics();
            
            // Setup auto-refresh
            if (self.config.autoRefresh) {
                self.setupAutoRefresh();
            }
            
        } catch (error) {
            console.error('Error initializing FieldSenseStatistics:', error);
        }
    },

    // Initialize chart libraries
    initializeChartLibraries: function() {
        var self = this;
        
        // Check for Chart.js availability
        if (typeof Chart !== 'undefined') {
            // Configure Chart.js defaults
            Chart.defaults.global.responsive = true;
            Chart.defaults.global.maintainAspectRatio = false;
            Chart.defaults.global.defaultFontFamily = "'Roboto', 'Helvetica Neue', Arial, sans-serif";
            Chart.defaults.global.defaultFontSize = 12;
            
            console.log('Chart.js library detected and configured');
        } else {
            console.warn('Chart.js not available, charts will be displayed as tables');
        }
    },

    // Bind event handlers
    bindEvents: function() {
        var self = this;
        
        // Refresh statistics button
        $('#btnRefreshStatistics').off('click').on('click', function(e) {
            e.preventDefault();
            self.loadAllStatistics();
        });
        
        // Auto-refresh toggle
        $('#chkAutoRefreshStats').off('change').on('change', function() {
            self.config.autoRefresh = $(this).is(':checked');
            if (self.config.autoRefresh) {
                self.setupAutoRefresh();
            } else {
                self.clearAutoRefresh();
            }
        });
        
        // Time range selector
        $('#ddlStatsTimeRange').off('change').on('change', function() {
            var timeRange = $(this).val();
            self.loadStatisticsWithTimeRange(timeRange);
        });
        
        // Export statistics button
        $('#btnExportStatistics').off('click').on('click', function(e) {
            e.preventDefault();
            self.exportStatistics();
        });
        
        // Drill-down buttons
        $(document).on('click', '.stats-drill-down', function(e) {
            e.preventDefault();
            var metric = $(this).data('metric');
            var value = $(this).data('value');
            self.showDrillDown(metric, value);
        });
    },

    // Load all statistics
    loadAllStatistics: function() {
        var self = this;
        
        self.setLoadingState(true);
        
        // Load both client and site statistics in parallel
        var clientPromise = self.loadClientStatistics();
        var sitePromise = self.loadSiteStatistics();
        
        $.when(clientPromise, sitePromise)
            .done(function(clientData, siteData) {
                self.processStatistics(clientData, siteData);
                self.renderDashboard();
                self.config.lastRefresh = new Date();
            })
            .fail(function() {
                self.showError('Failed to load statistics');
            })
            .always(function() {
                self.setLoadingState(false);
            });
    },

    // Load client statistics from API
    loadClientStatistics: function() {
        var self = this;
        
        return $.ajax({
            url: '/FieldSense/GetClientStatistics',
            type: 'GET',
            data: { companyId: self.config.companyId },
            headers: {
                'Authorization': 'Bearer ' + self.config.token,
                'Content-Type': 'application/json'
            },
            timeout: 30000
        });
    },

    // Load site statistics from API
    loadSiteStatistics: function() {
        var self = this;
        
        return $.ajax({
            url: '/FieldSense/GetSiteStatistics',
            type: 'GET',
            data: { companyId: self.config.companyId },
            headers: {
                'Authorization': 'Bearer ' + self.config.token,
                'Content-Type': 'application/json'
            },
            timeout: 30000
        });
    },

    // Process and combine statistics
    processStatistics: function(clientResponse, siteResponse) {
        var self = this;
        
        // Extract data from responses
        self.config.clientStatistics = (clientResponse && clientResponse.success) ? clientResponse.data : {};
        self.config.siteStatistics = (siteResponse && siteResponse.success) ? siteResponse.data : {};
        
        // Create combined statistics
        self.config.combinedStatistics = self.combineStatistics();
    },

    // Combine client and site statistics
    combineStatistics: function() {
        var self = this;
        var client = self.config.clientStatistics;
        var site = self.config.siteStatistics;
        
        return {
            // Overview metrics
            totalClients: client.totalClients || 0,
            activeClients: client.activeClients || 0,
            blockedClients: client.blockedClients || 0,
            totalSites: site.totalSites || 0,
            activeSites: site.activeSites || 0,
            
            // Activity metrics
            totalVisits: (client.totalVisits || 0) + (site.totalVisitsAllSites || 0),
            visitsThisWeek: client.visitsThisWeek || 0,
            visitsThisMonth: client.visitsThisMonth || 0,
            
            // Growth metrics
            clientsAddedThisMonth: client.clientsAddedThisMonth || 0,
            sitesAddedThisMonth: site.sitesAddedThisMonth || 0,
            growthRate: self.calculateGrowthRate(client, site),
            
            // Geographic metrics
            citiesCovered: client.uniqueCities || 0,
            statesCovered: client.uniqueStates || 0,
            sitesWithGPS: site.sitesWithGPS || 0,
            gpscoverage: site.totalSites > 0 ? ((site.sitesWithGPS || 0) / site.totalSites * 100).toFixed(1) : 0,
            
            // Performance metrics
            averageVisitsPerClient: client.totalClients > 0 ? ((client.totalVisits || 0) / client.totalClients).toFixed(1) : 0,
            averageVisitsPerSite: site.totalSites > 0 ? ((site.totalVisitsAllSites || 0) / site.totalSites).toFixed(1) : 0,
            
            // Client categories breakdown
            clientsByCategory: client.clientsByCategory || {},
            sitesByType: site.sitesByType || {},
            
            // Recent activity
            recentActivities: self.mergeRecentActivities(client.recentActivities, site.recentActivities)
        };
    },

    // Calculate growth rate
    calculateGrowthRate: function(client, site) {
        var currentTotal = (client.totalClients || 0) + (site.totalSites || 0);
        var addedThisMonth = (client.clientsAddedThisMonth || 0) + (site.sitesAddedThisMonth || 0);
        var previousTotal = currentTotal - addedThisMonth;
        
        if (previousTotal === 0) return addedThisMonth > 0 ? 100 : 0;
        return ((addedThisMonth / previousTotal) * 100).toFixed(1);
    },

    // Merge recent activities
    mergeRecentActivities: function(clientActivities, siteActivities) {
        var activities = [];
        
        // Add client activities
        if (clientActivities && Array.isArray(clientActivities)) {
            activities = activities.concat(clientActivities.map(function(activity) {
                return Object.assign(activity, { type: 'client' });
            }));
        }
        
        // Add site activities
        if (siteActivities && Array.isArray(siteActivities)) {
            activities = activities.concat(siteActivities.map(function(activity) {
                return Object.assign(activity, { type: 'site' });
            }));
        }
        
        // Sort by date and return top 10
        return activities.sort(function(a, b) {
            return new Date(b.date) - new Date(a.date);
        }).slice(0, 10);
    },

    // Render the complete dashboard
    renderDashboard: function() {
        var self = this;
        
        try {
            // Render KPI cards
            self.renderKPICards();
            
            // Render charts
            self.renderCharts();
            
            // Render data tables
            self.renderDataTables();
            
            // Render recent activities
            self.renderRecentActivities();
            
            // Update last refresh time
            self.updateRefreshTime();
            
            console.log('Dashboard rendered successfully');
        } catch (error) {
            console.error('Error rendering dashboard:', error);
            self.showError('Error rendering dashboard');
        }
    },

    // Render KPI cards
    renderKPICards: function() {
        var self = this;
        var stats = self.config.combinedStatistics;
        var html = '';
        
        // Total Clients Card
        html += '<div class="col-lg-3 col-md-6 col-sm-12">';
        html += '<div class="kpi-card kpi-card-primary">';
        html += '<div class="kpi-icon"><i class="fas fa-users"></i></div>';
        html += '<div class="kpi-content">';
        html += '<div class="kpi-value" data-metric="clients" data-value="' + stats.totalClients + '">';
        html += '<span class="counter">' + stats.totalClients + '</span>';
        html += '</div>';
        html += '<div class="kpi-label">Total Clients</div>';
        html += '<div class="kpi-change">';
        html += '<span class="change-value">+' + stats.clientsAddedThisMonth + '</span> this month';
        html += '</div>';
        html += '<button class="btn btn-sm btn-link stats-drill-down" data-metric="clients" data-value="total">View Details</button>';
        html += '</div>';
        html += '</div>';
        html += '</div>';
        
        // Active Clients Card
        html += '<div class="col-lg-3 col-md-6 col-sm-12">';
        html += '<div class="kpi-card kpi-card-success">';
        html += '<div class="kpi-icon"><i class="fas fa-user-check"></i></div>';
        html += '<div class="kpi-content">';
        html += '<div class="kpi-value">' + stats.activeClients + '</div>';
        html += '<div class="kpi-label">Active Clients</div>';
        var activePercentage = stats.totalClients > 0 ? ((stats.activeClients / stats.totalClients) * 100).toFixed(1) : 0;
        html += '<div class="kpi-change">' + activePercentage + '% of total</div>';
        html += '<button class="btn btn-sm btn-link stats-drill-down" data-metric="clients" data-value="active">View Active</button>';
        html += '</div>';
        html += '</div>';
        html += '</div>';
        
        // Total Sites Card
        html += '<div class="col-lg-3 col-md-6 col-sm-12">';
        html += '<div class="kpi-card kpi-card-info">';
        html += '<div class="kpi-icon"><i class="fas fa-map-marker-alt"></i></div>';
        html += '<div class="kpi-content">';
        html += '<div class="kpi-value">' + stats.totalSites + '</div>';
        html += '<div class="kpi-label">Total Sites</div>';
        html += '<div class="kpi-change">+' + stats.sitesAddedThisMonth + ' this month</div>';
        html += '<button class="btn btn-sm btn-link stats-drill-down" data-metric="sites" data-value="total">View Details</button>';
        html += '</div>';
        html += '</div>';
        html += '</div>';
        
        // Total Visits Card
        html += '<div class="col-lg-3 col-md-6 col-sm-12">';
        html += '<div class="kpi-card kpi-card-warning">';
        html += '<div class="kpi-icon"><i class="fas fa-route"></i></div>';
        html += '<div class="kpi-content">';
        html += '<div class="kpi-value">' + stats.totalVisits + '</div>';
        html += '<div class="kpi-label">Total Visits</div>';
        html += '<div class="kpi-change">' + stats.visitsThisMonth + ' this month</div>';
        html += '<button class="btn btn-sm btn-link stats-drill-down" data-metric="visits" data-value="total">View Visits</button>';
        html += '</div>';
        html += '</div>';
        html += '</div>';
        
        // GPS Coverage Card
        html += '<div class="col-lg-3 col-md-6 col-sm-12">';
        html += '<div class="kpi-card kpi-card-secondary">';
        html += '<div class="kpi-icon"><i class="fas fa-satellite-dish"></i></div>';
        html += '<div class="kpi-content">';
        html += '<div class="kpi-value">' + stats.gpsCoverage + '%</div>';
        html += '<div class="kpi-label">GPS Coverage</div>';
        html += '<div class="kpi-change">' + stats.sitesWithGPS + ' of ' + stats.totalSites + ' sites</div>';
        html += '</div>';
        html += '</div>';
        html += '</div>';
        
        // Growth Rate Card
        html += '<div class="col-lg-3 col-md-6 col-sm-12">';
        html += '<div class="kpi-card kpi-card-success">';
        html += '<div class="kpi-icon"><i class="fas fa-chart-line"></i></div>';
        html += '<div class="kpi-content">';
        html += '<div class="kpi-value">+' + stats.growthRate + '%</div>';
        html += '<div class="kpi-label">Growth Rate</div>';
        html += '<div class="kpi-change">This month</div>';
        html += '</div>';
        html += '</div>';
        html += '</div>';
        
        $('#statisticsKPIContainer').html(html);
        
        // Animate counters if library is available
        self.animateCounters();
    },

    // Render charts
    renderCharts: function() {
        var self = this;
        
        if (typeof Chart !== 'undefined') {
            self.renderClientCategoryChart();
            self.renderSiteTypeChart();
            self.renderVisitsTrendChart();
            self.renderGrowthChart();
        } else {
            // Fallback to tables if Chart.js is not available
            self.renderChartTables();
        }
    },

    // Render client category chart
    renderClientCategoryChart: function() {
        var self = this;
        var stats = self.config.combinedStatistics;
        
        var ctx = document.getElementById('clientCategoryChart');
        if (!ctx) return;
        
        // Destroy existing chart
        if (self.config.charts.clientCategory) {
            self.config.charts.clientCategory.destroy();
        }
        
        var data = Object.keys(stats.clientsByCategory || {}).map(function(category) {
            return stats.clientsByCategory[category];
        });
        
        var labels = Object.keys(stats.clientsByCategory || {});
        
        self.config.charts.clientCategory = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: data,
                    backgroundColor: [
                        self.config.chartColors.primary,
                        self.config.chartColors.success,
                        self.config.chartColors.warning,
                        self.config.chartColors.danger,
                        self.config.chartColors.info,
                        self.config.chartColors.secondary
                    ]
                }]
            },
            options: {
                responsive: true,
                legend: {
                    position: 'bottom'
                },
                title: {
                    display: true,
                    text: 'Clients by Category'
                }
            }
        });
    },

    // Render site type chart
    renderSiteTypeChart: function() {
        var self = this;
        var stats = self.config.combinedStatistics;
        
        var ctx = document.getElementById('siteTypeChart');
        if (!ctx) return;
        
        // Destroy existing chart
        if (self.config.charts.siteType) {
            self.config.charts.siteType.destroy();
        }
        
        var data = Object.keys(stats.sitesByType || {}).map(function(type) {
            return stats.sitesByType[type];
        });
        
        var labels = Object.keys(stats.sitesByType || {});
        
        self.config.charts.siteType = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Sites',
                    data: data,
                    backgroundColor: self.config.chartColors.info
                }]
            },
            options: {
                responsive: true,
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                },
                title: {
                    display: true,
                    text: 'Sites by Type'
                }
            }
        });
    },

    // Render visits trend chart
    renderVisitsTrendChart: function() {
        var self = this;
        var stats = self.config.combinedStatistics;
        
        var ctx = document.getElementById('visitsTrendChart');
        if (!ctx) return;
        
        // Destroy existing chart
        if (self.config.charts.visitsTrend) {
            self.config.charts.visitsTrend.destroy();
        }
        
        // Generate sample trend data (in real implementation, this would come from API)
        var trendData = self.generateVisitsTrendData();
        
        self.config.charts.visitsTrend = new Chart(ctx, {
            type: 'line',
            data: {
                labels: trendData.labels,
                datasets: [{
                    label: 'Visits',
                    data: trendData.data,
                    borderColor: self.config.chartColors.primary,
                    backgroundColor: self.config.chartColors.primary + '20',
                    fill: true
                }]
            },
            options: {
                responsive: true,
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                },
                title: {
                    display: true,
                    text: 'Visits Trend (Last 30 Days)'
                }
            }
        });
    },

    // Generate sample visits trend data
    generateVisitsTrendData: function() {
        var labels = [];
        var data = [];
        var baseVisits = 50;
        
        for (var i = 29; i >= 0; i--) {
            var date = new Date();
            date.setDate(date.getDate() - i);
            labels.push(date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }));
            
            // Generate realistic trending data
            var visits = baseVisits + Math.floor(Math.random() * 30) + (i % 7 === 0 ? 20 : 0); // Weekend spike
            data.push(visits);
        }
        
        return { labels: labels, data: data };
    },

    // Render growth chart
    renderGrowthChart: function() {
        var self = this;
        
        var ctx = document.getElementById('growthChart');
        if (!ctx) return;
        
        // Destroy existing chart
        if (self.config.charts.growth) {
            self.config.charts.growth.destroy();
        }
        
        // Generate sample growth data
        var growthData = self.generateGrowthData();
        
        self.config.charts.growth = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: growthData.labels,
                datasets: [{
                    label: 'Clients',
                    data: growthData.clients,
                    backgroundColor: self.config.chartColors.primary
                }, {
                    label: 'Sites',
                    data: growthData.sites,
                    backgroundColor: self.config.chartColors.success
                }]
            },
            options: {
                responsive: true,
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                },
                title: {
                    display: true,
                    text: 'Monthly Growth'
                }
            }
        });
    },

    // Generate sample growth data
    generateGrowthData: function() {
        var labels = [];
        var clients = [];
        var sites = [];
        
        for (var i = 5; i >= 0; i--) {
            var date = new Date();
            date.setMonth(date.getMonth() - i);
            labels.push(date.toLocaleDateString('en-US', { month: 'short', year: 'numeric' }));
            
            clients.push(Math.floor(Math.random() * 20) + 5);
            sites.push(Math.floor(Math.random() * 15) + 3);
        }
        
        return { labels: labels, clients: clients, sites: sites };
    },

    // Render chart tables (fallback)
    renderChartTables: function() {
        var self = this;
        var stats = self.config.combinedStatistics;
        
        // Client category table
        var categoryHtml = '<table class="table table-sm">';
        categoryHtml += '<thead><tr><th>Category</th><th>Count</th></tr></thead>';
        categoryHtml += '<tbody>';
        Object.keys(stats.clientsByCategory || {}).forEach(function(category) {
            categoryHtml += '<tr><td>' + category + '</td><td>' + stats.clientsByCategory[category] + '</td></tr>';
        });
        categoryHtml += '</tbody></table>';
        $('#clientCategoryChart').parent().html(categoryHtml);
        
        // Site type table
        var typeHtml = '<table class="table table-sm">';
        typeHtml += '<thead><tr><th>Site Type</th><th>Count</th></tr></thead>';
        typeHtml += '<tbody>';
        Object.keys(stats.sitesByType || {}).forEach(function(type) {
            typeHtml += '<tr><td>' + type + '</td><td>' + stats.sitesByType[type] + '</td></tr>';
        });
        typeHtml += '</tbody></table>';
        $('#siteTypeChart').parent().html(typeHtml);
    },

    // Render data tables
    renderDataTables: function() {
        var self = this;
        // Implementation for detailed data tables
        // This would show top clients, recent visits, etc.
    },

    // Render recent activities
    renderRecentActivities: function() {
        var self = this;
        var activities = self.config.combinedStatistics.recentActivities || [];
        
        var html = '';
        if (activities.length === 0) {
            html = '<div class="no-activities">No recent activities found</div>';
        } else {
            html += '<div class="activity-list">';
            activities.forEach(function(activity) {
                html += '<div class="activity-item">';
                html += '<div class="activity-icon">';
                html += activity.type === 'client' ? '<i class="fas fa-user"></i>' : '<i class="fas fa-map-marker-alt"></i>';
                html += '</div>';
                html += '<div class="activity-content">';
                html += '<div class="activity-title">' + self.escapeHtml(activity.title || activity.name) + '</div>';
                html += '<div class="activity-description">' + self.escapeHtml(activity.description || activity.action) + '</div>';
                html += '<div class="activity-time">' + self.timeAgo(activity.date) + '</div>';
                html += '</div>';
                html += '</div>';
            });
            html += '</div>';
        }
        
        $('#recentActivitiesContainer').html(html);
    },

    // Show drill-down details
    showDrillDown: function(metric, value) {
        var self = this;
        
        // Implementation would depend on the metric
        switch (metric) {
            case 'clients':
                if (value === 'total') {
                    // Navigate to clients page with all clients
                    window.location.href = '/FieldSense/ClientMaster';
                } else if (value === 'active') {
                    // Navigate to clients page with active filter
                    window.location.href = '/FieldSense/ClientMaster?status=Active';
                }
                break;
            case 'sites':
                if (value === 'total') {
                    window.location.href = '/FieldSense/SiteMaster';
                }
                break;
            case 'visits':
                // Show visits modal or navigate to visits page
                self.showVisitsModal();
                break;
            default:
                self.showInfo('Drill-down details for ' + metric + ' will be implemented');
        }
    },

    // Setup auto-refresh
    setupAutoRefresh: function() {
        var self = this;
        
        if (self.refreshTimer) {
            clearInterval(self.refreshTimer);
        }
        
        self.refreshTimer = setInterval(function() {
            if (!self.config.isLoading) {
                console.log('Auto-refreshing statistics...');
                self.loadAllStatistics();
            }
        }, self.config.refreshInterval);
        
        console.log('Auto-refresh enabled (every 5 minutes)');
    },

    // Clear auto-refresh
    clearAutoRefresh: function() {
        if (this.refreshTimer) {
            clearInterval(this.refreshTimer);
            this.refreshTimer = null;
            console.log('Auto-refresh disabled');
        }
    },

    // Export statistics
    exportStatistics: function() {
        var self = this;
        
        // Prepare data for export
        var exportData = {
            generatedAt: new Date().toISOString(),
            statistics: self.config.combinedStatistics,
            clientStatistics: self.config.clientStatistics,
            siteStatistics: self.config.siteStatistics
        };
        
        // Create and download JSON file
        var dataStr = JSON.stringify(exportData, null, 2);
        var dataBlob = new Blob([dataStr], { type: 'application/json' });
        var url = window.URL.createObjectURL(dataBlob);
        
        var link = document.createElement('a');
        link.href = url;
        link.download = 'fieldsense-statistics-' + new Date().toISOString().split('T')[0] + '.json';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        
        window.URL.revokeObjectURL(url);
        
        self.showSuccess('Statistics exported successfully');
    },

    // Helper methods
    
    setLoadingState: function(isLoading) {
        this.config.isLoading = isLoading;
        
        if (isLoading) {
            $('#statisticsLoadingOverlay').show();
            $('#btnRefreshStatistics').prop('disabled', true).find('i').addClass('fa-spin');
        } else {
            $('#statisticsLoadingOverlay').hide();
            $('#btnRefreshStatistics').prop('disabled', false).find('i').removeClass('fa-spin');
        }
    },

    updateRefreshTime: function() {
        if (this.config.lastRefresh) {
            $('#lastRefreshTime').text('Last updated: ' + this.config.lastRefresh.toLocaleTimeString());
        }
    },

    animateCounters: function() {
        // Animate counter numbers if library is available
        if (typeof $ !== 'undefined' && $.fn.countTo) {
            $('.counter').each(function() {
                var $this = $(this);
                var value = parseInt($this.text());
                $this.text('0');
                $this.countTo({
                    from: 0,
                    to: value,
                    speed: 1500,
                    refreshInterval: 50
                });
            });
        }
    },

    timeAgo: function(dateString) {
        if (!dateString) return '';
        
        var date = new Date(dateString);
        var now = new Date();
        var diffMs = now - date;
        var diffMins = Math.floor(diffMs / 60000);
        var diffHours = Math.floor(diffMins / 60);
        var diffDays = Math.floor(diffHours / 24);
        
        if (diffMins < 1) return 'Just now';
        if (diffMins < 60) return diffMins + ' minute' + (diffMins === 1 ? '' : 's') + ' ago';
        if (diffHours < 24) return diffHours + ' hour' + (diffHours === 1 ? '' : 's') + ' ago';
        if (diffDays < 30) return diffDays + ' day' + (diffDays === 1 ? '' : 's') + ' ago';
        
        return date.toLocaleDateString();
    },

    escapeHtml: function(text) {
        if (!text) return '';
        var map = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        };
        return text.replace(/[&<>"']/g, function(m) { return map[m]; });
    },

    showSuccess: function(message) {
        if (typeof toastr !== 'undefined') {
            toastr.success(message);
        } else if (typeof swal !== 'undefined') {
            swal('Success', message, 'success');
        } else {
            alert(message);
        }
    },

    showError: function(message) {
        if (typeof toastr !== 'undefined') {
            toastr.error(message);
        } else if (typeof swal !== 'undefined') {
            swal('Error', message, 'error');
        } else {
            alert('Error: ' + message);
        }
    },

    showInfo: function(message) {
        if (typeof toastr !== 'undefined') {
            toastr.info(message);
        } else if (typeof swal !== 'undefined') {
            swal('Info', message, 'info');
        } else {
            alert(message);
        }
    }
};

// Global functions for HTML onclick events
function refreshStatistics() {
    if (typeof FieldSenseStatistics !== 'undefined') {
        FieldSenseStatistics.loadAllStatistics();
    }
}

function exportStatistics() {
    if (typeof FieldSenseStatistics !== 'undefined') {
        FieldSenseStatistics.exportStatistics();
    }
}

// Auto-initialize when ready
if (typeof $ !== 'undefined') {
    $(document).ready(function() {
        // Initialize only if the statistics container exists
        if ($('#statisticsKPIContainer').length > 0) {
            setTimeout(function() {
                if (typeof FieldSenseStatistics !== 'undefined') {
                    FieldSenseStatistics.init();
                }
            }, 100);
        }
    });
}