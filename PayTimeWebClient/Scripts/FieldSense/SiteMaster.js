/*
 * FieldSense Site Management - Modern Version
 * JavaScript/jQuery implementation following .NET MVC best practices
 * Enhanced with statistics, pagination, advanced search, and proper AJAX patterns
 * Uses modern ES5 patterns with backward compatibility
 * Author: Claude Code Assistant
 * Version: 2.0 - Complete Site Management Module
 */

var FieldSenseSite = {
    // Enhanced Configuration following modern JavaScript patterns
    config: {
        // Authentication & API
        token: '',
        apiUrl: '',
        roleId: '',
        companyId: '',
        userId: '',
        
        // Display & Formatting
        dateFormat: 'MM/dd/yyyy',
        timeFormat: 'HH:mm:ss',
        
        // Data Management
        sites: [],
        filteredSites: [],
        selectedSites: [],
        statistics: {},
        
        // Pagination & Search
        currentPage: 1,
        pageSize: 20,
        totalRecords: 0,
        totalPages: 0,
        sortBy: 'siteName',
        sortOrder: 'asc',
        searchRequest: {
            searchText: '',
            siteType: '',
            status: '',
            hasGPSLocation: null,
            page: 1,
            pageSize: 20,
            sortBy: 'siteName',
            sortDirection: 'asc'
        },
        
        // UI State
        isLoading: false,
        dataTable: null,
        lastRefresh: null,
        useEnhancedTable: true
    },

    // Initialize the application
    init: function() {
        var self = this;
        
        try {
            // Load configuration from global variable
            if (typeof window.FieldSenseConfig !== 'undefined') {
                self.config.token = window.FieldSenseConfig.token;
                self.config.apiUrl = window.FieldSenseConfig.apiUrl;
                self.config.roleId = window.FieldSenseConfig.roleId;
                self.config.companyId = window.FieldSenseConfig.companyId;
                self.config.userId = window.FieldSenseConfig.userId;
                self.config.dateFormat = window.FieldSenseConfig.dateFormat;
            }
            
            console.log('FieldSenseSite initialized with config:', self.config);
            
            // Initialize components first
            self.initializeComponents();
            
            // Setup event handlers
            self.bindEvents();
            
            // Load initial data with statistics
            self.refreshData();
            
            // Set up periodic refresh (every 5 minutes)
            self.setupPeriodicRefresh();
            
        } catch (error) {
            console.error('Error initializing FieldSenseSite:', error);
            self.showError('Failed to initialize site management');
        }
    },

    // Bind event handlers
    bindEvents: function() {
        var self = this;
        
        try {
            // Add Site button
            $('#btnAddSite').off('click').on('click', function(e) {
                e.preventDefault();
                self.showSiteModal();
            });
            
            // Filter Toggle
            $('#btnSiteFilterToggle').off('click').on('click', function(e) {
                e.preventDefault();
                $('#siteFilterContainer').slideToggle(300);
            });
            
            // Refresh button
            $('#btnSiteRefresh').off('click').on('click', function(e) {
                e.preventDefault();
                // Destroy existing DataTable if it exists
                if ($.fn.DataTable.isDataTable('#tblSite')) {
                    $('#tblSite').DataTable().destroy();
                    $('#tblSite').empty();
                }
                self.loadSites();
            });
            
            // Import button
            $('#btnSiteImport').off('click').on('click', function(e) {
                e.preventDefault();
                $('#fileSiteImport').click();
            });
            
            // Export button
            $('#btnSiteExport').off('click').on('click', function(e) {
                e.preventDefault();
                self.showExportModal();
            });
            
            // Download Template button
            $('#btnDownloadSiteTemplate').off('click').on('click', function(e) {
                e.preventDefault();
                self.downloadSiteTemplate();
            });
            
            // File import change
            $('#fileSiteImport').off('change').on('change', function(e) {
                self.handleFileImport(e.target.files[0]);
            });
            
            // Enhanced Search input with debounce
            var searchTimeout;
            $('#txtSiteSearch').off('keyup').on('keyup', function() {
                clearTimeout(searchTimeout);
                var searchTerm = $(this).val();
                searchTimeout = setTimeout(function() {
                    self.performSearch(searchTerm);
                }, 300);
            });
            
            // Advanced Filter controls
            $('#ddlFilterSiteType, #ddlFilterSiteStatus, #ddlFilterHasGPS').off('change').on('change', function() {
                self.applyAdvancedFilters();
            });
            
            // Date range filters
            $('#dtSiteCreatedFrom, #dtSiteCreatedTo').off('change').on('change', function() {
                self.applyAdvancedFilters();
            });
            
            // Apply Filter button
            $('#btnApplySiteFilter').off('click').on('click', function(e) {
                e.preventDefault();
                self.applyAdvancedFilters();
            });
            
            // Clear Filter button
            $('#btnClearSiteFilter').off('click').on('click', function(e) {
                e.preventDefault();
                self.clearAdvancedFilters();
            });
            
            // Quick filter buttons
            $('.site-quick-filter-btn').off('click').on('click', function(e) {
                e.preventDefault();
                var filterType = $(this).data('filter');
                var filterValue = $(this).data('value');
                self.applyQuickFilter(filterType, filterValue);
            });
            
            // Page size selector
            $('#ddlSitePageSize').off('change').on('change', function() {
                self.config.searchRequest.pageSize = parseInt($(this).val());
                self.config.searchRequest.page = 1; // Reset to first page
                self.loadSites();
            });
            
            // Enhanced Bulk Actions
            $('#btnBulkDeleteSites').off('click').on('click', function(e) {
                e.preventDefault();
                self.bulkDeleteSites();
            });
            
            $('#btnBulkExportSites').off('click').on('click', function(e) {
                e.preventDefault();
                self.bulkExportSites();
            });
            
            $('#btnBulkStatusUpdateSites').off('click').on('click', function(e) {
                e.preventDefault();
                self.bulkUpdateStatus();
            });
            
            // Statistics refresh
            $('#btnRefreshSiteStats').off('click').on('click', function(e) {
                e.preventDefault();
                self.loadStatistics();
            });
            
            // Column sorting
            $('.site-sortable-header').off('click').on('click', function(e) {
                e.preventDefault();
                var column = $(this).data('column');
                self.toggleSort(column);
            });
            
            // Auto-refresh toggle
            $('#chkSiteAutoRefresh').off('change').on('change', function() {
                if ($(this).is(':checked')) {
                    self.setupPeriodicRefresh();
                } else {
                    console.log('Auto-refresh disabled');
                }
            });
            
        } catch (error) {
            console.error('Error binding site events:', error);
        }
    },

    // Initialize components with modern patterns
    initializeComponents: function() {
        var self = this;
        
        try {
            // Initialize date pickers if available
            if ($.fn.datepicker) {
                $('.site-date-picker').datepicker({
                    format: 'mm/dd/yyyy',
                    autoclose: true,
                    todayHighlight: true
                });
            }
            
            // Initialize tooltips
            if ($.fn.tooltip) {
                $('[data-toggle="tooltip"]').tooltip();
            }
            
            // Setup progress indicators
            self.setupProgressIndicators();
            
            // Initialize modal handlers
            self.setupModalHandlers();
            
            console.log('FieldSenseSite: Components initialized successfully');
        } catch (error) {
            console.error('Error initializing site components:', error);
        }
    },

    // Load sites from API with enhanced search and pagination
    loadSites: function(searchRequest) {
        var self = this;
        
        // Use provided search request or default config
        var searchParams = searchRequest || self.config.searchRequest;
        
        // Set loading state
        self.setLoadingState(true);
        
        $.ajax({
            url: '/FieldSense/GetAllSites',
            type: 'GET',
            data: self.buildSiteSearchQueryParams(searchParams),
            headers: {
                'Authorization': 'Bearer ' + self.config.token,
                'Content-Type': 'application/json'
            },
            timeout: 30000, // 30 seconds timeout
            success: function(response) {
                self.handleSitesResponse(response);
            },
            error: function(xhr, status, error) {
                self.handleApiError('Failed to load sites', xhr, status, error);
            },
            complete: function() {
                self.setLoadingState(false);
                self.config.lastRefresh = new Date();
            }
        });
    },
    
    // Handle sites API response
    handleSitesResponse: function(response) {
        var self = this;
        
        try {
            if (response && response.success) {
                var data = response.data || [];
                
                // Update configuration
                self.config.sites = data;
                self.config.filteredSites = data.slice();
                self.config.totalRecords = response.pagination ? response.pagination.totalRecords : data.length;
                self.config.totalPages = response.pagination ? response.pagination.totalPages : 1;
                self.config.currentPage = response.pagination ? response.pagination.currentPage : 1;
                
                // Update UI
                self.updateDataDisplay(data);
                self.updatePaginationControls();
                
                console.log('FieldSenseSite: Loaded', data.length, 'sites');
            } else {
                self.showError(response.message || 'Failed to load sites');
                self.updateDataDisplay([]);
            }
        } catch (error) {
            console.error('Error handling sites response:', error);
            self.showError('Error processing site data');
        }
    },
    
    // Update data display
    updateDataDisplay: function(data) {
        var self = this;
        
        if (data.length === 0) {
            $('#sitetblempty').hide();
            $('#SiteGrid-empty').show();
            self.destroyDataTable();
        } else {
            $('#sitetblempty').show();
            $('#SiteGrid-empty').hide();
            self.initializeDataTable(data);
        }
    },

    // Load statistics from API
    loadStatistics: function() {
        var self = this;
        
        $.ajax({
            url: '/FieldSense/GetSiteStatistics',
            type: 'GET',
            data: { companyId: self.config.companyId },
            headers: {
                'Authorization': 'Bearer ' + self.config.token,
                'Content-Type': 'application/json'
            },
            success: function(response) {
                if (response && response.success && response.data) {
                    self.config.statistics = response.data;
                    self.renderStatistics(response.data);
                } else {
                    // Fallback to calculated statistics
                    self.calculateAndRenderStatistics();
                }
            },
            error: function(xhr, status, error) {
                console.warn('Failed to load site statistics from API, using calculated values');
                self.calculateAndRenderStatistics();
            }
        });
    },
    
    // Calculate statistics from loaded data (fallback)
    calculateAndRenderStatistics: function() {
        var self = this;
        
        var stats = {
            totalSites: 0,
            activeSites: 0,
            sitesWithGPS: 0,
            sitesAddedThisMonth: 0,
            totalVisitsAllSites: 0,
            officeSites: 0,
            warehouseSites: 0
        };
        
        if (self.config.sites && self.config.sites.length > 0) {
            stats.totalSites = self.config.sites.length;
            stats.activeSites = self.config.sites.filter(s => s.Status === 'Active').length;
            stats.sitesWithGPS = self.config.sites.filter(s => s.Latitude && s.Longitude).length;
            stats.officeSites = self.config.sites.filter(s => s.SiteType === 'Office').length;
            stats.warehouseSites = self.config.sites.filter(s => s.SiteType === 'Warehouse').length;
            stats.totalVisitsAllSites = self.config.sites.reduce(function(sum, s) { 
                return sum + (s.TotalVisits || 0); 
            }, 0);
        }
        
        self.config.statistics = stats;
        self.renderStatistics(stats);
    },

    // Render enhanced statistics dashboard
    renderStatistics: function(stats) {
        var self = this;
        var html = '';
        
        // Main statistics cards
        html += '<div class="site-stats-grid">';
        
        // Total Sites Card
        html += '<div class="stat-card stat-card-primary">';
        html += '<div class="stat-icon"><i class="fa fa-building"></i></div>';
        html += '<div class="stat-content">';
        html += '<div class="stat-value">' + (stats.totalSites || 0) + '</div>';
        html += '<div class="stat-label">Total Sites</div>';
        html += '<div class="stat-change">+' + (stats.sitesAddedThisMonth || 0) + ' this month</div>';
        html += '</div>';
        html += '</div>';
        
        // Active Sites Card
        html += '<div class="stat-card stat-card-success">';
        html += '<div class="stat-icon"><i class="fa fa-check-circle"></i></div>';
        html += '<div class="stat-content">';
        html += '<div class="stat-value">' + (stats.activeSites || 0) + '</div>';
        html += '<div class="stat-label">Active Sites</div>';
        var activePercent = stats.totalSites > 0 ? Math.round((stats.activeSites / stats.totalSites) * 100) : 0;
        html += '<div class="stat-change">' + activePercent + '% of total</div>';
        html += '</div>';
        html += '</div>';
        
        // GPS Coverage Card
        html += '<div class="stat-card stat-card-info">';
        html += '<div class="stat-icon"><i class="fa fa-location-arrow"></i></div>';
        html += '<div class="stat-content">';
        html += '<div class="stat-value">' + (stats.sitesWithGPS || 0) + '</div>';
        html += '<div class="stat-label">GPS Located</div>';
        var gpsPercent = stats.totalSites > 0 ? Math.round((stats.sitesWithGPS / stats.totalSites) * 100) : 0;
        html += '<div class="stat-change">' + gpsPercent + '% coverage</div>';
        html += '</div>';
        html += '</div>';
        
        // Office Sites Card
        html += '<div class="stat-card stat-card-warning">';
        html += '<div class="stat-icon"><i class="fa fa-briefcase"></i></div>';
        html += '<div class="stat-content">';
        html += '<div class="stat-value">' + (stats.officeSites || 0) + '</div>';
        html += '<div class="stat-label">Office Sites</div>';
        html += '<div class="stat-change">Primary locations</div>';
        html += '</div>';
        html += '</div>';
        
        // Warehouse Sites Card
        html += '<div class="stat-card stat-card-danger">';
        html += '<div class="stat-icon"><i class="fa fa-warehouse"></i></div>';
        html += '<div class="stat-content">';
        html += '<div class="stat-value">' + (stats.warehouseSites || 0) + '</div>';
        html += '<div class="stat-label">Warehouses</div>';
        html += '<div class="stat-change">Storage facilities</div>';
        html += '</div>';
        html += '</div>';
        
        // Total Visits Card
        html += '<div class="stat-card stat-card-secondary">';
        html += '<div class="stat-icon"><i class="fa fa-map-marker-alt"></i></div>';
        html += '<div class="stat-content">';
        html += '<div class="stat-value">' + (stats.totalVisitsAllSites || 0) + '</div>';
        html += '<div class="stat-label">Total Visits</div>';
        html += '<div class="stat-change">All sites combined</div>';
        html += '</div>';
        html += '</div>';
        
        html += '</div>'; // End site-stats-grid
        
        // Add last refresh time
        if (self.config.lastRefresh) {
            html += '<div class="stats-footer">';
            html += '<small class="text-muted">Last updated: ' + self.config.lastRefresh.toLocaleTimeString() + '</small>';
            html += '</div>';
        }
        
        $('#siteStatsContainer').html(html);
        
        // Store stats for other methods to use
        self.config.statistics = stats;
    },
    
    // Enhanced Search and Filter Methods
    
    // Perform search with term
    performSearch: function(searchTerm) {
        this.config.searchRequest.searchText = searchTerm;
        this.config.searchRequest.page = 1; // Reset to first page
        this.loadSites();
    },
    
    // Apply advanced filters
    applyAdvancedFilters: function() {
        var self = this;
        
        // Update search request with filter values
        self.config.searchRequest.siteType = $('#ddlFilterSiteType').val() || '';
        self.config.searchRequest.status = $('#ddlFilterSiteStatus').val() || '';
        
        var hasGPS = $('#ddlFilterHasGPS').val();
        if (hasGPS !== '') {
            self.config.searchRequest.hasGPSLocation = hasGPS === 'true';
        } else {
            self.config.searchRequest.hasGPSLocation = null;
        }
        
        // Date filters
        var createdFrom = $('#dtSiteCreatedFrom').val();
        var createdTo = $('#dtSiteCreatedTo').val();
        
        if (createdFrom) self.config.searchRequest.createdFromDate = createdFrom;
        if (createdTo) self.config.searchRequest.createdToDate = createdTo;
        
        self.config.searchRequest.page = 1; // Reset to first page
        self.loadSites();
    },
    
    // Clear advanced filters
    clearAdvancedFilters: function() {
        var self = this;
        
        // Clear form controls
        $('#txtSiteSearch').val('');
        $('#ddlFilterSiteType, #ddlFilterSiteStatus, #ddlFilterHasGPS').val('');
        $('#dtSiteCreatedFrom, #dtSiteCreatedTo').val('');
        
        // Reset search request
        self.config.searchRequest = {
            searchText: '',
            siteType: '',
            status: '',
            hasGPSLocation: null,
            page: 1,
            pageSize: self.config.searchRequest.pageSize || 20,
            sortBy: 'siteName',
            sortDirection: 'asc'
        };
        
        self.loadSites();
    },
    
    // Apply quick filter (for dashboard stat cards)
    applyQuickFilter: function(filterType, filterValue) {
        var self = this;
        
        // Clear existing filters first
        self.clearAdvancedFilters();
        
        // Apply the quick filter
        switch (filterType) {
            case 'status':
                $('#ddlFilterSiteStatus').val(filterValue);
                self.config.searchRequest.status = filterValue;
                break;
            case 'siteType':
                $('#ddlFilterSiteType').val(filterValue);
                self.config.searchRequest.siteType = filterValue;
                break;
            case 'hasGPS':
                $('#ddlFilterHasGPS').val(filterValue);
                self.config.searchRequest.hasGPSLocation = filterValue === 'true';
                break;
            case 'recent':
                // Show sites added in last 30 days
                var thirtyDaysAgo = new Date();
                thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);
                $('#dtSiteCreatedFrom').val(thirtyDaysAgo.toISOString().split('T')[0]);
                self.config.searchRequest.createdFromDate = thirtyDaysAgo.toISOString();
                break;
        }
        
        self.loadSites();
    },
    
    // Toggle column sorting
    toggleSort: function(column) {
        var self = this;
        
        if (self.config.searchRequest.sortBy === column) {
            // Toggle sort direction
            self.config.searchRequest.sortDirection = self.config.searchRequest.sortDirection === 'asc' ? 'desc' : 'asc';
        } else {
            // New column, default to ascending
            self.config.searchRequest.sortBy = column;
            self.config.searchRequest.sortDirection = 'asc';
        }
        
        self.config.searchRequest.page = 1; // Reset to first page
        self.loadSites();
    },

    // Initialize Enhanced DataTable
    initializeDataTable: function(data) {
        var self = this;
        console.log(data);
        $("#dataTables_site_tbl_header").remove();
        
        $('#tblSite').DataTable({
            dom: _domCommon,
            language: _languageCommon,
            deferRender: true,
            data: data,
            order: [[2, 'asc']], // Sort by Site Name by default
            destroy: true,
            initComplete: function () {
                const visibleTable = $('table:visible').attr('id');
                $("#site-buttons").append(`
                    <div id="site-export-button" class="export-button tooltips" data-placement="bottom" data-original-title="Download" data-table-id="${visibleTable}">
                        <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
                    </div>
                `);
                
                // Add checkbox column header
                $('#tblSite thead tr th:first').html('<input type="checkbox" id="chkSelectAllSites" />');
                
                // Bind checkbox events after table is drawn
                self.bindTableEvents();
            },
            columns: [
                {
                    title: "",
                    data: null,
                    orderable: false,
                    width: "30px",
                    render: function (data, type, row) {
                        return '<input type="checkbox" class="chk-site" value="' + row.SiteId + '" />';
                    }
                },
                {
                    title: "Sr No",
                    data: null,
                    visible: false,
                    render: function (data, type, row, meta) {
                        return meta.row + 1;
                    }
                },
                { 
                    title: "Site Code", 
                    data: "SiteCode",
                    render: function(data, type, row) {
                        return self.escapeHtml(data || '-');
                    }
                },
                { 
                    title: "Site Name", 
                    data: "SiteName",
                    render: function(data, type, row) {
                        var name = data || '';
                        var siteType = row.SiteType || '';
                        var html = '<strong>' + self.escapeHtml(name) + '</strong>';
                        if (siteType) {
                            html += '<br><small class="site-type ' + self.escapeHtml(siteType.toLowerCase().replace(/\s+/g, '-')) + '">' + self.escapeHtml(siteType) + '</small>';
                        }
                        return html;
                    }
                },
                { 
                    title: "Description", 
                    data: "Description",
                    render: function(data, type, row) {
                        var desc = self.escapeHtml(data || '-');
                        return desc.length > 50 ? desc.substring(0, 50) + '...' : desc;
                    }
                },
                { 
                    title: "Address", 
                    data: "Address",
                    render: function(data, type, row) {
                        var addr = self.escapeHtml(data || '-');
                        return addr.length > 40 ? addr.substring(0, 40) + '...' : addr;
                    }
                },
                {
                    title: "GPS Location",
                    data: null,
                    render: function(data, type, row) {
                        var lat = row.Latitude;
                        var lng = row.Longitude;
                        if (lat && lng) {
                            return '<i class="fa fa-location-arrow text-success"></i> Located<br><small>' + 
                                   parseFloat(lat).toFixed(4) + ', ' + parseFloat(lng).toFixed(4) + '</small>';
                        } else {
                            return '<i class="fa fa-location-arrow text-muted"></i> Not Set';
                        }
                    }
                },
                {
                    title: "Visits",
                    data: "TotalVisits",
                    render: function(data, type, row) {
                        var visits = data || 0;
                        var lastVisit = row.LastVisitDate;
                        var html = '<div class="text-center"><strong>' + visits + '</strong></div>';
                        if (lastVisit) {
                            html += '<div><small>Last: ' + self.formatDate(lastVisit) + '</small></div>';
                        }
                        return html;
                    }
                },
                {
                    title: "Status",
                    data: "Status",
                    render: function(data, type, row) {
                        return self.getStatusBadge(data);
                    }
                },
                {
                    title: "Action",
                    data: null,
                    orderable: false,
                    width: "120px",
                    render: function (data, type, row) {
                        var siteId = row.SiteId;
                        var str = '';
                        str += '<a class="viewclass btn seagreen_btnnew tooltips" data-placement="top" data-original-title="View" onclick="FieldSenseSite.viewSite(' + siteId + ')"><i class="fa-regular fa-eye"></i></a> ';
                        str += '<a class="editclass btn seagreen_btnnew tooltips" data-placement="top" data-original-title="Edit" onclick="FieldSenseSite.editSite(' + siteId + ')"><i class="fa-regular fa-pen-to-square"></i></a> ';
                        str += '<a class="deleteclass btn red_btnnew tooltips" data-placement="top" data-original-title="Delete" onclick="FieldSenseSite.deleteSite(' + siteId + ')"><i class="fa-regular fa-trash-can"></i></a>';
                        return str;
                    }
                }
            ],
            drawCallback: function () {
                $('.tooltips').tooltip();
                self.bindTableEvents();
            }
        });
        
        setTimeout(function () {
            $("#" + "tblSite" + "_wrapper #dataTables_site_tbl_header").insertBefore($("#siteTableResponsive"));
            $('.tooltips').tooltip();
        }, 100);
    },

    // Bind table events
    bindTableEvents: function() {
        var self = this;
        
        // Select All checkbox
        $(document).off('change', '#chkSelectAllSites').on('change', '#chkSelectAllSites', function() {
            var isChecked = $(this).is(':checked');
            $('.chk-site').prop('checked', isChecked);
            self.updateSelectedSites();
        });
        
        // Individual checkboxes
        $(document).off('change', '.chk-site').on('change', '.chk-site', function() {
            self.updateSelectedSites();
        });
    },

    // Update selected sites
    updateSelectedSites: function() {
        var self = this;
        
        self.config.selectedSites = [];
        $('.chk-site:checked').each(function() {
            self.config.selectedSites.push(parseInt($(this).val()));
        });
        
        // Update UI
        var selectedCount = self.config.selectedSites.length;
        $('#siteSelectedCount').text(selectedCount + ' site' + (selectedCount !== 1 ? 's' : '') + ' selected');
        
        if (selectedCount > 0) {
            $('#siteBulkActionsContainer').show();
        } else {
            $('#siteBulkActionsContainer').hide();
        }
        
        // Update Select All checkbox
        var totalVisible = $('.chk-site').length;
        $('#chkSelectAllSites').prop('checked', selectedCount > 0 && selectedCount === totalVisible);
    },

    // Show site modal (Add/Edit)
    showSiteModal: function(siteId) {
        var self = this;
        var isEdit = siteId ? true : false;
        var site = {};
        
        if (isEdit) {
            site = self.config.sites.find(s => s.SiteId === siteId) || {};
        }
        
        // Update modal title and button text
        if (isEdit) {
            $('#siteModalTitle').text('Edit Site');
            $('#saveSiteButtonText').text('Update Site');
        } else {
            $('#siteModalTitle').text('Add New Site');
            $('#saveSiteButtonText').text('Save Site');
        }
        
        // Populate form fields
        self.populateSiteForm(site, isEdit);
        
        // Show modal using Bootstrap modal
        $('#myModal').modal('show');
    },

    // Save site
    saveSite: function() {
        var self = this;
        
        // Validate form
        if (!self.validateSiteForm()) {
            return;
        }
        
        // Collect form data - use PascalCase for controller compatibility
        var siteData = {
            SiteId: $('#SiteId').val() || 0,
            SiteCode: $('#SiteCode').val() ? $('#SiteCode').val().trim() : '',
            SiteName: $('#SiteName').val() ? $('#SiteName').val().trim() : '',
            SiteType: $('#SiteType').val() || '',
            Description: $('#Description').val() ? $('#Description').val().trim() : '',
            Address: $('#Address').val() ? $('#Address').val().trim() : '',
            City: '',  // Field doesn't exist in form
            State: '',  // Field doesn't exist in form
            PinCode: '', // Field doesn't exist in form
            Latitude: $('#Latitude').val() || '',
            Longitude: $('#Longitude').val() || '',
            Radius: $('#Radius').val() || 100,
            Status: $('#Status').val() || 'Active',
            CompanyId: self.config.companyId,
            UserId: self.config.userId
        };

        var isEdit = parseInt(siteData.SiteId) > 0;
        var url = '/FieldSense/AddUpdateSite';

        // Show loading
        $('#saveSiteButtonText').html('<i class="fas fa-spinner fa-spin"></i> Saving...');
        
        $.ajax({
            url: url,
            type: 'POST',
            data: JSON.stringify(siteData),
            contentType: 'application/json',
            headers: {
                'Authorization': 'Bearer ' + self.config.token
            },
            success: function(response) {
                $('#saveSiteButtonText').text(isEdit ? 'Update Site' : 'Save Site');
                
                if (response && response.success) {
                    self.showSuccess(isEdit ? 'Site updated successfully' : 'Site added successfully');
                    self.closeSiteModal();
                    
                    // Refresh DataTable
                    if ($.fn.DataTable.isDataTable('#tblSite')) {
                        $('#tblSite').DataTable().destroy();
                        $('#tblSite').empty();
                    }
                    self.loadSites();
                    self.loadStatistics();
                } else {
                    self.showError(response.message || 'Failed to save site');
                }
            },
            error: function(xhr, status, error) {
                $('#saveSiteButtonText').text(isEdit ? 'Update Site' : 'Save Site');
                console.error('Error saving site:', xhr, status, error);
                self.showError('Error saving site: ' + (xhr.responseJSON?.message || error));
            }
        });
    },

    // Validate site form
    validateSiteForm: function() {
        var errors = [];
        
        if (!$('#SiteCode').val().trim()) {
            errors.push('Site code is required');
        }
        
        if (!$('#SiteName').val().trim()) {
            errors.push('Site name is required');
        }
        
        if (!$('#Address').val().trim()) {
            errors.push('Address is required');
        }
        
        // Validate GPS coordinates if provided
        var lat = $('#Latitude').val().trim();
        var lng = $('#Longitude').val().trim();
        if ((lat && !lng) || (!lat && lng)) {
            errors.push('Please provide both latitude and longitude coordinates');
        }
        
        if (errors.length > 0) {
            this.showError('Please fix the following errors:\n• ' + errors.join('\n• '));
            return false;
        }
        
        return true;
    },

    // View site details
    viewSite: function(siteId) {
        var self = this;
        var site = self.config.sites.find(s => s.SiteId === siteId);
        
        if (!site) {
            self.showError('Site not found');
            return;
        }
        
        // Show site details modal
        self.showSiteDetailsModal(site);
    },

    // Edit site
    editSite: function(siteId) {
        this.showSiteModal(siteId);
    },

    // Delete site
    deleteSite: function(siteId) {
        var self = this;
        
        if (typeof swal !== 'undefined') {
            swal({
                title: "Are you sure?",
                text: "You want to delete this site?",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Yes, delete it!",
                closeOnConfirm: false
            }, function() {
                self.performDeleteSite(siteId);
            });
        } else {
            if (confirm('Are you sure you want to delete this site?')) {
                self.performDeleteSite(siteId);
            }
        }
    },

    // Perform delete operation
    performDeleteSite: function(siteId) {
        var self = this;
        
        $.ajax({
            url: '/FieldSense/DeleteSite',
            type: 'POST',
            data: JSON.stringify({ siteId: siteId }),
            contentType: 'application/json',
            headers: {
                'Authorization': 'Bearer ' + self.config.token
            },
            success: function(response) {
                if (response && response.success) {
                    if (typeof swal !== 'undefined') {
                        swal("Deleted!", "Site has been deleted.", "success");
                    } else {
                        self.showSuccess('Site deleted successfully');
                    }
                    // Refresh DataTable
                    if ($.fn.DataTable.isDataTable('#tblSite')) {
                        $('#tblSite').DataTable().destroy();
                        $('#tblSite').empty();
                    }
                    self.loadSites();
                    self.loadStatistics();
                } else {
                    if (typeof swal !== 'undefined') {
                        swal("Error!", response.message || "Failed to delete site.", "error");
                    } else {
                        self.showError(response.message || 'Failed to delete site');
                    }
                }
            },
            error: function(xhr, status, error) {
                console.error('Error deleting site:', xhr, status, error);
                if (typeof swal !== 'undefined') {
                    swal("Error!", "Error deleting site.", "error");
                } else {
                    self.showError('Error deleting site');
                }
            }
        });
    },

    // Bulk delete sites
    bulkDeleteSites: function() {
        var self = this;
        
        if (self.config.selectedSites.length === 0) {
            self.showError('Please select sites to delete');
            return;
        }
        
        if (typeof swal !== 'undefined') {
            swal({
                title: "Are you sure?",
                text: "You want to delete " + self.config.selectedSites.length + " sites?",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Yes, delete them!",
                closeOnConfirm: false
            }, function() {
                self.performBulkDeleteSites();
            });
        } else {
            if (confirm('Are you sure you want to delete ' + self.config.selectedSites.length + ' sites?')) {
                self.performBulkDeleteSites();
            }
        }
    },

    // Perform bulk delete
    performBulkDeleteSites: function() {
        var self = this;
        
        $.ajax({
            url: '/FieldSense/BulkDeleteSites',
            type: 'POST',
            data: JSON.stringify(self.config.selectedSites),
            contentType: 'application/json',
            headers: {
                'Authorization': 'Bearer ' + self.config.token
            },
            success: function(response) {
                if (response && response.success) {
                    if (typeof swal !== 'undefined') {
                        swal("Deleted!", response.message, "success");
                    } else {
                        self.showSuccess(response.message);
                    }
                    self.config.selectedSites = [];
                    $('#siteBulkActionsContainer').hide();
                    // Refresh DataTable
                    if ($.fn.DataTable.isDataTable('#tblSite')) {
                        $('#tblSite').DataTable().destroy();
                        $('#tblSite').empty();
                    }
                    self.loadSites();
                    self.loadStatistics();
                } else {
                    if (typeof swal !== 'undefined') {
                        swal("Error!", response.message || "Failed to delete sites.", "error");
                    } else {
                        self.showError(response.message || 'Failed to delete sites');
                    }
                }
            },
            error: function(xhr, status, error) {
                console.error('Error deleting sites:', xhr, status, error);
                if (typeof swal !== 'undefined') {
                    swal("Error!", "Error deleting sites.", "error");
                } else {
                    self.showError('Error deleting sites');
                }
            }
        });
    },

    // Bulk update status
    bulkUpdateStatus: function() {
        var self = this;
        var selectedIds = self.config.selectedSites;
        
        if (selectedIds.length === 0) {
            self.showError('Please select at least one site to update.');
            return;
        }
        
        // Show status selection modal
        var newStatus = prompt('Enter new status (Active, Inactive):');
        if (newStatus && ['Active', 'Inactive'].includes(newStatus)) {
            self.performBulkStatusUpdate(selectedIds, newStatus);
        }
    },
    
    // Perform bulk status update
    performBulkStatusUpdate: function(siteIds, status) {
        var self = this;
        var promises = [];
        
        // Update each site status
        siteIds.forEach(function(siteId) {
            var promise = $.ajax({
                url: '/FieldSense/UpdateSiteStatus',
                type: 'POST',
                data: JSON.stringify({ siteId: siteId, status: status }),
                headers: {
                    'Authorization': 'Bearer ' + self.config.token,
                    'Content-Type': 'application/json'
                }
            });
            promises.push(promise);
        });
        
        // Wait for all updates to complete
        $.when.apply($, promises)
            .done(function() {
                self.showSuccess(siteIds.length + ' sites updated successfully.');
                self.refreshData();
            })
            .fail(function() {
                self.showError('Some sites could not be updated. Please try again.');
            });
    },

    // Show export modal
    showExportModal: function() {
        var self = this;
        
        var modalHtml = '';
        modalHtml += '<div class="modal active" id="siteExportModal">';
        modalHtml += '<div class="modal-content" style="max-width: 400px;">';
        modalHtml += '<div class="modal-header">';
        modalHtml += '<h3>Export Sites</h3>';
        modalHtml += '<button class="modal-close" onclick="FieldSenseSite.closeSiteModal()">&times;</button>';
        modalHtml += '</div>';
        modalHtml += '<div class="modal-body">';
        modalHtml += '<div class="form-group">';
        modalHtml += '<label>Export Format</label>';
        modalHtml += '<select class="form-control" id="ddlSiteExportFormat">';
        modalHtml += '<option value="excel">Excel (.xlsx)</option>';
        modalHtml += '<option value="csv">CSV (.csv)</option>';
        modalHtml += '</select>';
        modalHtml += '</div>';
        modalHtml += '<div class="form-group">';
        modalHtml += '<label>Export Data</label>';
        modalHtml += '<select class="form-control" id="ddlSiteExportData">';
        modalHtml += '<option value="all">All Sites (' + self.config.sites.length + ')</option>';
        modalHtml += '<option value="filtered">Filtered Sites (' + self.config.filteredSites.length + ')</option>';
        if (self.config.selectedSites.length > 0) {
            modalHtml += '<option value="selected">Selected Sites (' + self.config.selectedSites.length + ')</option>';
        }
        modalHtml += '</select>';
        modalHtml += '</div>';
        modalHtml += '<div class="form-group">';
        modalHtml += '<div class="checkbox">';
        modalHtml += '<label><input type="checkbox" id="chkIncludeVisitStats" checked> Include Visit Statistics</label>';
        modalHtml += '</div>';
        modalHtml += '<div class="checkbox">';
        modalHtml += '<label><input type="checkbox" id="chkIncludeGPSData" checked> Include GPS Data</label>';
        modalHtml += '</div>';
        modalHtml += '</div>';
        modalHtml += '</div>';
        modalHtml += '<div class="modal-footer">';
        modalHtml += '<button class="btn btn-secondary" onclick="FieldSenseSite.closeSiteModal()">Cancel</button>';
        modalHtml += '<button class="btn btn-primary" onclick="FieldSenseSite.performExport()">';
        modalHtml += '<i class="fas fa-download"></i> Export';
        modalHtml += '</button>';
        modalHtml += '</div>';
        modalHtml += '</div>';
        modalHtml += '</div>';
        
        $('#siteModalContainer').html(modalHtml);
    },

    // Perform export
    performExport: function() {
        var format = $('#ddlSiteExportFormat').val();
        var dataType = $('#ddlSiteExportData').val();
        var includeVisitStats = $('#chkIncludeVisitStats').is(':checked');
        var includeGPSData = $('#chkIncludeGPSData').is(':checked');
        
        var url = '/FieldSense/ExportSites?format=' + format + '&type=' + dataType;
        url += '&includeVisitStats=' + includeVisitStats + '&includeGPSData=' + includeGPSData;
        
        if (dataType === 'selected' && this.config.selectedSites.length > 0) {
            url += '&siteIds=' + this.config.selectedSites.join(',');
        }
        
        window.open(url, '_blank');
        this.closeSiteModal();
    },

    // Download site template
    downloadSiteTemplate: function(format) {
        format = format || 'excel';
        var url = '/FieldSense/GetSiteImportTemplate?format=' + format;
        window.open(url, '_blank');
    },

    // Handle file import
    handleFileImport: function(file) {
        var self = this;
        
        if (!file) return;
        
        var formData = new FormData();
        formData.append('file', file);
        
        $.ajax({
            url: '/FieldSense/ImportSites',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            headers: {
                'Authorization': 'Bearer ' + self.config.token
            },
            success: function(response) {
                if (response && response.success) {
                    self.showSuccess(response.message);
                    // Refresh DataTable
                    if ($.fn.DataTable.isDataTable('#tblSite')) {
                        $('#tblSite').DataTable().destroy();
                        $('#tblSite').empty();
                    }
                    self.loadSites();
                    self.loadStatistics();
                } else {
                    self.showError(response.message || 'Failed to import sites');
                }
            },
            error: function(xhr, status, error) {
                console.error('Error importing sites:', xhr, status, error);
                self.showError('Error importing sites');
            }
        });
    },

    // Modern Helper Methods
    
    // Refresh all data (sites + statistics)
    refreshData: function() {
        var self = this;
        self.loadSites();
        self.loadStatistics();
    },
    
    // Setup periodic refresh
    setupPeriodicRefresh: function() {
        var self = this;
        // Refresh every 5 minutes
        setInterval(function() {
            if (!self.config.isLoading) {
                self.refreshData();
            }
        }, 300000);
    },
    
    // Setup progress indicators
    setupProgressIndicators: function() {
        // Ensure loading indicators exist
        if ($('#ajax_site_loader').length === 0) {
            $('body').append('<div id="ajax_site_loader" style="display:none;"><div class="spinner"></div></div>');
        }
    },
    
    // Setup modal handlers
    setupModalHandlers: function() {
        var self = this;
        
        // Close modal when clicking outside or on close button
        $(document).on('click', '.modal-backdrop, .modal-close', function(e) {
            e.preventDefault();
            self.closeSiteModal();
        });
        
        // Prevent modal close when clicking inside modal content
        $(document).on('click', '.modal-content', function(e) {
            e.stopPropagation();
        });
    },
    
    // Set loading state
    setLoadingState: function(isLoading) {
        this.config.isLoading = isLoading;
        if (isLoading) {
            $('#ajax_site_loader').show();
            $('.btn-site-refresh').prop('disabled', true).find('i').addClass('fa-spin');
        } else {
            $('#ajax_site_loader').hide();
            $('.btn-site-refresh').prop('disabled', false).find('i').removeClass('fa-spin');
        }
    },
    
    // Handle API errors consistently
    handleApiError: function(message, xhr, status, error) {
        var self = this;
        var errorMessage = message;
        
        if (xhr && xhr.responseJSON) {
            errorMessage += ': ' + (xhr.responseJSON.message || xhr.responseJSON.meg || error);
        } else if (error) {
            errorMessage += ': ' + error;
        }
        
        console.error('Site API Error:', errorMessage, xhr);
        self.showError(errorMessage);
        
        // Handle specific error codes
        if (xhr && xhr.status === 401) {
            self.showError('Session expired. Please login again.');
        }
    },
    
    // Build search query parameters
    buildSiteSearchQueryParams: function(searchRequest) {
        var params = {};
        
        if (searchRequest.searchText) params.searchText = searchRequest.searchText;
        if (searchRequest.siteType) params.siteType = searchRequest.siteType;
        if (searchRequest.status) params.status = searchRequest.status;
        if (searchRequest.hasGPSLocation !== null) params.hasGPSLocation = searchRequest.hasGPSLocation;
        if (searchRequest.createdFromDate) params.createdFromDate = searchRequest.createdFromDate;
        if (searchRequest.createdToDate) params.createdToDate = searchRequest.createdToDate;
        
        params.page = searchRequest.page || 1;
        params.pageSize = searchRequest.pageSize || 20;
        params.sortBy = searchRequest.sortBy || 'siteName';
        params.sortDirection = searchRequest.sortDirection || 'asc';
        
        return params;
    },
    
    // Update pagination controls
    updatePaginationControls: function() {
        var self = this;
        var html = '';
        
        if (self.config.totalPages > 1) {
            html += '<div class="site-pagination-controls">';
            html += '<span class="pagination-info">Page ' + self.config.currentPage + ' of ' + self.config.totalPages + ' (' + self.config.totalRecords + ' total)</span>';
            
            // Previous button
            if (self.config.currentPage > 1) {
                html += '<button class="btn btn-sm btn-secondary" onclick="FieldSenseSite.goToPage(' + (self.config.currentPage - 1) + ')">Previous</button>';
            }
            
            // Page numbers (show up to 5 pages around current)
            var startPage = Math.max(1, self.config.currentPage - 2);
            var endPage = Math.min(self.config.totalPages, self.config.currentPage + 2);
            
            for (var i = startPage; i <= endPage; i++) {
                var activeClass = i === self.config.currentPage ? 'btn-primary' : 'btn-outline-secondary';
                html += '<button class="btn btn-sm ' + activeClass + '" onclick="FieldSenseSite.goToPage(' + i + ')">' + i + '</button>';
            }
            
            // Next button
            if (self.config.currentPage < self.config.totalPages) {
                html += '<button class="btn btn-sm btn-secondary" onclick="FieldSenseSite.goToPage(' + (self.config.currentPage + 1) + ')">Next</button>';
            }
            
            html += '</div>';
        }
        
        $('#sitePaginationContainer').html(html);
    },
    
    // Go to specific page
    goToPage: function(page) {
        this.config.searchRequest.page = page;
        this.config.currentPage = page;
        this.loadSites();
    },
    
    // Destroy DataTable safely
    destroyDataTable: function() {
        if (this.config.dataTable && $.fn.DataTable.isDataTable('#tblSite')) {
            this.config.dataTable.destroy();
            $('#tblSite').empty();
            this.config.dataTable = null;
        }
    },
    
    // Helper Functions
    
    getStatusBadge: function(status) {
        var badgeClass = '';
        switch(status) {
            case 'Active':
                badgeClass = 'badge badge-success';
                break;
            case 'Inactive':
                badgeClass = 'badge badge-warning';
                break;
            default:
                badgeClass = 'badge badge-secondary';
        }
        return '<span class="' + badgeClass + '">' + (status || 'Active') + '</span>';
    },

    formatDate: function(dateString) {
        if (!dateString) return '-';
        try {
            var date = new Date(dateString);
            return date.toLocaleDateString('en-US', { 
                year: 'numeric', 
                month: 'short', 
                day: 'numeric' 
            });
        } catch (error) {
            return '-';
        }
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
        } else {
            alert(message);
        }
    },

    showError: function(message) {
        if (typeof toastr !== 'undefined') {
            toastr.error(message);
        } else {
            alert('Error: ' + message);
        }
    },

    showInfo: function(message) {
        if (typeof toastr !== 'undefined') {
            toastr.info(message);
        } else {
            alert(message);
        }
    },

    // Populate site form with data
    populateSiteForm: function(site, isEdit) {
        if (isEdit && site) {
            $('#SiteId').val(site.SiteId || '');
            $('#SiteCode').val(site.SiteCode || '');
            $('#SiteName').val(site.SiteName || '');
            $('#SiteType').val(site.SiteType || '');
            $('#Description').val(site.Description || '');
            $('#Address').val(site.Address || '');
            $('#Latitude').val(site.Latitude || '');
            $('#Longitude').val(site.Longitude || '');
            $('#Radius').val(site.Radius || '100');
            $('#Status').val(site.Status || 'Active');
        } else {
            // Clear form for new site
            $('#SiteId').val('');
            $('#SiteCode').val('');
            $('#SiteName').val('');
            $('#SiteType').val('');
            $('#Description').val('');
            $('#Address').val('');
            $('#Latitude').val('');
            $('#Longitude').val('');
            $('#Radius').val('100');
            $('#Status').val('Active');
        }
    },

    // Close site modal
    closeSiteModal: function() {
        $('#myModal').modal('hide');
        $('#importSitesModal, #siteExportModal').modal('hide');
    },

    // Show site details modal
    showSiteDetailsModal: function(site) {
        var self = this;
        
        var html = '<div class="site-details-modal">';
        html += '<h4><i class="fa fa-building"></i> ' + self.escapeHtml(site.SiteName) + '</h4>';
        
        html += '<div class="detail-section">';
        html += '<h5>Basic Information</h5>';
        html += '<div class="row">';
        html += '<div class="col-md-6">';
        html += '<p><strong>Code:</strong> ' + self.escapeHtml(site.SiteCode || '-') + '</p>';
        html += '<p><strong>Type:</strong> ' + self.escapeHtml(site.SiteType || '-') + '</p>';
        html += '<p><strong>Status:</strong> ' + self.getStatusBadge(site.Status) + '</p>';
        html += '</div>';
        html += '<div class="col-md-6">';
        html += '<p><strong>Total Visits:</strong> ' + (site.TotalVisits || 0) + '</p>';
        if (site.LastVisitDate) {
            html += '<p><strong>Last Visit:</strong> ' + self.formatDate(site.LastVisitDate) + '</p>';
        }
        html += '</div>';
        html += '</div>';
        html += '</div>';
        
        if (site.Description) {
            html += '<div class="detail-section">';
            html += '<h5>Description</h5>';
            html += '<p>' + self.escapeHtml(site.Description) + '</p>';
            html += '</div>';
        }
        
        html += '<div class="detail-section">';
        html += '<h5>Address & Location</h5>';
        var fullAddr = (site.Address || '') + 
                      (site.City ? ', ' + site.City : '') + 
                      (site.State ? ', ' + site.State : '') + 
                      (site.PinCode ? ' ' + site.PinCode : '');
        html += '<p>' + self.escapeHtml(fullAddr || '-') + '</p>';
        if (site.Latitude && site.Longitude) {
            html += '<p><strong>GPS:</strong> ' + site.Latitude + ', ' + site.Longitude + '</p>';
            html += '<p><strong>Radius:</strong> ' + (site.Radius || '100') + ' meters</p>';
        }
        html += '</div>';
        
        html += '</div>';
        
        // Show details modal using Bootstrap
        $('#siteDetailsModal .modal-body').html(html);
        $('#siteDetailsModal').modal('show');
    },

    // Pick location on map
    pickLocationOnMap: function() {
        var currentLocation = null;
        
        // Get current coordinates if available
        var lat = parseFloat($('#Latitude').val());
        var lng = parseFloat($('#Longitude').val());
        
        if (!isNaN(lat) && !isNaN(lng)) {
            currentLocation = { lat: lat, lng: lng };
        }
        
        // Show location picker modal (if location service is available)
        if (typeof FieldSenseLocationService !== 'undefined') {
            FieldSenseLocationService.showLocationPicker({
                currentLocation: currentLocation,
                radius: parseInt($('#Radius').val()) || 100,
                showRadius: true,
                showSearch: true,
                showCurrentLocation: true,
                title: 'Select Site Location'
            }, function(result) {
                if (result) {
                    $('#Latitude').val(result.lat.toFixed(6));
                    $('#Longitude').val(result.lng.toFixed(6));
                    $('#Radius').val(result.radius || 100);
                    
                    // Auto-fill address if provided
                    if (result.address && !$('#Address').val().trim()) {
                        $('#Address').val(result.address);
                    }
                    
                    // Mark fields as modified for validation
                    $('#Latitude, #Longitude').trigger('change');
                }
            });
        } else {
            this.showInfo('Location picker functionality will be implemented with Google Maps API');
        }
    }
};

// Global functions for HTML onclick events
function openSiteModal(siteId) {
    if (typeof FieldSenseSite !== 'undefined') {
        FieldSenseSite.showSiteModal(siteId);
    }
}

function closeSiteModal() {
    if (typeof FieldSenseSite !== 'undefined') {
        FieldSenseSite.closeSiteModal();
    }
}

function saveSite() {
    if (typeof FieldSenseSite !== 'undefined') {
        FieldSenseSite.saveSite();
    }
}

function pickSiteLocationOnMap() {
    if (typeof FieldSenseSite !== 'undefined') {
        FieldSenseSite.pickLocationOnMap();
    }
}

function openSiteModal(modalId) {
    if (modalId === 'importSitesModal') {
        $('#importSitesModal').show();
        $('body').addClass('modal-open');
    }
}

function closeSiteModal(modalId) {
    $('#' + modalId).hide();
    $('body').removeClass('modal-open');
}

function importSites() {
    if (typeof FieldSenseSite !== 'undefined') {
        FieldSenseSite.handleFileImport(document.getElementById('csvSiteInput').files[0]);
        FieldSenseSite.closeSiteModal();
    }
}

function downloadSiteTemplate(format) {
    if (typeof FieldSenseSite !== 'undefined') {
        FieldSenseSite.downloadSiteTemplate(format);
    }
}

// Auto-initialize if jQuery is ready
if (typeof $ !== 'undefined') {
    $(document).ready(function() {
        // Wait a bit for the page to fully load
        setTimeout(function() {
            if (typeof FieldSenseSite !== 'undefined') {
                // Don't auto-initialize here, let the view handle it
                console.log('FieldSenseSite ready for initialization');
            }
        }, 100);
    });
}