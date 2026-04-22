/*
 * FieldSense Client Management
 * JavaScript/jQuery implementation for client management with minimal Razor syntax
 * Uses client-side operations and AJAX for backend communication
 */

var FieldSenseClient = {
    // Configuration
    config: {
        token: '',
        apiUrl: '',
        roleId: '',
        companyId: '',
        userId: '',
        dateFormat: 'MM/dd/yyyy',
        clients: [],
        filteredClients: []
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
                self.config.canEdit = window.FieldSenseConfig.canEdit;
                self.config.canDelete = window.FieldSenseConfig.canDelete;
            }
            
            console.log('FieldSenseClient initialized with config:', self.config);
            
            // Load initial data
            self.loadClients();
            /*self.loadStatistics();*/
            
            // Setup event handlers
            self.bindEvents();
            
            // Initialize components
            self.initializeComponents();
            
        } catch (error) {
            console.error('Error initializing FieldSenseClient:', error);
            self.showError('Failed to initialize client management');
        }
    },

    // Bind event handlers
    bindEvents: function() {
        var self = this;
        
        try {
            // Add Client button
            $('#btnAddClient').off('click').on('click', function(e) {
                e.preventDefault();
                self.showClientModal();
            });
            
            //// Filter Toggle
            //$('#btnFilterToggle').off('click').on('click', function(e) {
            //    e.preventDefault();
            //    $('#filterContainer').slideToggle(300);
            //});
            
            // Refresh button
            //$('#btnRefresh').off('click').on('click', function(e) {
            //    e.preventDefault();
            //    // Destroy existing DataTable if it exists
            //    if ($.fn.DataTable.isDataTable('#tblClient')) {
            //        $('#tblClient').DataTable().destroy();
            //        $('#tblClient').empty();
            //    }
            //    self.loadClients();
            //});
            
            // Search input with debounce
            var searchTimeout;
            $('#txtSearch').off('keyup').on('keyup', function() {
                clearTimeout(searchTimeout);
                searchTimeout = setTimeout(function() {
                    self.applyFilters();
                }, 300);
            });
            
            // Filter controls
            $('#ddlFilterCategory, #ddlFilterStatus').off('change').on('change', function() {
                self.applyFilters();
            });
            
            // Apply Filter button
            $('#btnApplyFilter').off('click').on('click', function(e) {
                e.preventDefault();
                self.applyFilters();
            });
            
            // Clear Filter button
            $('#btnClearFilter').off('click').on('click', function(e) {
                e.preventDefault();
                self.clearFilters();
            });

        } catch (error) {
            console.error('Error binding events:', error);
        }
    },

    // Initialize components
    initializeComponents: function() {
        // Setup any additional components like date pickers, etc.
        console.log('Components initialized');
    },

    // Load clients from API
    loadClients: function() {
        var self = this;

        $.ajax({
            url: self.config.apiUrl + 'ClientMaster/all',
            type: 'GET',
            headers: {
                'Authorization': 'Bearer ' + self.config.token,
                'Content-Type': 'application/json'
            },
            beforeSend: function () {
                $("#ajax_loader").show();
            },
            success: function(response) {
                var data = [];
                if (response && response.success) {
                    data = response.data || [];
                    self.config.clients = data;
                    self.config.filteredClients = data.slice();
                    self.config.totalRecords = response.totalCount || data.length;
                }
                
                if (data.length == 0) {
                    $('#tblempty').hide();
                    $('#ClientGrid-empty').show();
                } else {
                    $('#tblempty').show();
                    $('#ClientGrid-empty').hide();
                    self.initializeDataTable(data);
                }
            },
            error: function(xhr, status, error) {
                toastr.error('Failed to load clients. Please try again.');
                $('#tblempty').hide();
                $('#ClientGrid-empty').show();
            },
            complete: function() {
                $("#ajax_loader").hide();
            }
        });
    },

    // Load statistics from backend API
    loadStatistics: function() {
        var self = this;

        $.ajax({
            url: self.config.apiUrl + 'ClientMaster/statistics',
            type: 'GET',
            headers: {
                'Authorization': 'Bearer ' + self.config.token
            },
            success: function(response) {
                try {
                    if (response && response.success) {
                        self.renderStatistics(response.data);
                    } else {
                        console.warn('Statistics API returned error:', response.message);
                        self.loadStatisticsFallback();
                    }
                } catch (error) {
                    console.error('Error processing statistics response:', error);
                    self.loadStatisticsFallback();
                }
            },
            error: function(xhr, status, error) {
                console.error('Error loading statistics:', error);
                self.loadStatisticsFallback();
            }
        });
    },

    // Fallback statistics calculation from loaded data
    loadStatisticsFallback: function() {
        var self = this;
        
        var stats = {
            totalClients: 0,
            activeClients: 0,
            inactiveClients: 0,
            newClients: 0,
            premiumClients: 0,
            regularClients: 0,
            totalVisitsThisMonth: 0,
            pendingVisits: 0,
            averageRating: 0,
            topPerformingClient: 'N/A'
        };
        
        // Calculate from loaded data
        if (self.config.clients && self.config.clients.length > 0) {
            stats.totalClients = self.config.clients.length;
            stats.activeClients = self.config.clients.filter(c => c.status === 'Active').length;
            stats.inactiveClients = self.config.clients.filter(c => c.status === 'Inactive').length;
            stats.premiumClients = self.config.clients.filter(c => c.clientCategory === 'Premium').length;
            stats.regularClients = self.config.clients.filter(c => c.clientCategory === 'Regular').length;
            stats.newClients = self.config.clients.filter(c => c.clientCategory === 'New').length;
            stats.totalVisitsThisMonth = self.config.clients.reduce((sum, c) => sum + (c.totalVisits || 0), 0);
        }
        
        self.renderStatistics(stats);
    },

    // Render statistics cards with enhanced backend data
    renderStatistics: function(stats) {
        var html = '';
        
        // Ensure default values for missing fields
        stats = stats || {};
        
        html += '<div class="client-stat-card">';
        html += '<div class="client-stat-value">' + (stats.totalClients || 0) + '</div>';
        html += '<div class="client-stat-label">Total Clients</div>';
        html += '</div>';
        
        html += '<div class="client-stat-card">';
        html += '<div class="client-stat-value">' + (stats.activeClients || 0) + '</div>';
        html += '<div class="client-stat-label">Active Clients</div>';
        html += '</div>';
        
        html += '<div class="client-stat-card">';
        html += '<div class="client-stat-value">' + (stats.premiumClients || 0) + '</div>';
        html += '<div class="client-stat-label">Premium Clients</div>';
        html += '</div>';
        
        html += '<div class="client-stat-card">';
        html += '<div class="client-stat-value">' + (stats.newClients || 0) + '</div>';
        html += '<div class="client-stat-label">New Clients</div>';
        html += '</div>';
        
        html += '<div class="client-stat-card">';
        html += '<div class="client-stat-value">' + (stats.totalVisitsThisMonth || stats.totalVisits || 0) + '</div>';
        html += '<div class="client-stat-label">Visits This Month</div>';
        html += '</div>';
        
        html += '<div class="client-stat-card">';
        html += '<div class="client-stat-value">' + (stats.inactiveClients || 0) + '</div>';
        html += '<div class="client-stat-label">Inactive Clients</div>';
        html += '</div>';
        
        // Show average rating if available
        if (stats.averageRating !== undefined) {
            html += '<div class="client-stat-card">';
            html += '<div class="client-stat-value">' + (stats.averageRating || 0).toFixed(1) + '</div>';
            html += '<div class="client-stat-label">Avg Rating</div>';
            html += '</div>';
        }
        
        // Show pending visits if available
        if (stats.pendingVisits !== undefined) {
            html += '<div class="client-stat-card">';
            html += '<div class="client-stat-value">' + (stats.pendingVisits || 0) + '</div>';
            html += '<div class="client-stat-label">Pending Visits</div>';
            html += '</div>';
        }
        
        $('#statsContainer').html(html);
        
        // Show top performing client if available
        if (stats.topPerformingClient && stats.topPerformingClient !== 'N/A') {
            $('.top-client-info').html('Top Performer: <strong>' + stats.topPerformingClient + '</strong>');
        }
    },

    // Initialize DataTable
    initializeDataTable: function(data) {
        var self = this;

        $("#dataTables_tbl_header").remove();


        $('#tblClient').DataTable({
            dom: _domCommon,
            language: _languageCommon,
            deferRender: true,
            data: data,
            order: [[1, 'asc']], // Sort by Client Name by default
            destroy: true,
            //initComplete: function () {
            //    const visibleTable = $('table:visible').attr('id');
            //    $("#buttons").append(`
            //        <div id="export-button" class="export-button tooltips" data-placement="bottom" data-original-title="Download" data-table-id="${visibleTable}">
            //            <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
            //        </div>
            //    `);
            //},
            columns: [
                {
                    title: "Sr No",
                    data: null,
                    visible: false,
                    render: function (data, type, row, meta) {
                        return meta.row + 1;
                    }
                },
                { title: "clientId", data: "clientId",visible:false },
                {
                    title: "Client Name",
                    data: "clientName",
                    createdCell: function (td) {
                        $(td).css("min-width", "180px");
                    },
                    render: function(data, type, row) {
                        var html = '<div class="tablettl_main">' + self.escapeHtml(data || '') + '</div>';
                        if (row.clientCategory) {
                            html += '<small class="client-category ' + self.escapeHtml(row.clientCategory.toLowerCase()) + '">' + self.escapeHtml(row.clientCategory) + '</small>';
                        }
                        return html;
                    }
                },
                {
                    title: "Client Code",
                    data: "clientCode",
                    visible: false,
                    render: function(data) {
                        return self.escapeHtml(data || '-');
                    }
                },
                {
                    title: "Category",
                    data: "clientCategory",
                    visible: false,
                    render: function(data) {
                        return self.escapeHtml(data || '-');
                    }
                },
                {
                    title: "Description",
                    data: "description",
                    visible: false,
                    render: function(data) {
                        return self.escapeHtml(data || '-');
                    }
                },
                {
                    title: "Contact Person",
                    data: "contactPerson",
                    render: function(data) {
                        return self.escapeHtml(data || '-');
                    }
                },
                {
                    title: "Contact Person Phone",
                    data: "contactPersonPhone",
                    visible: false,
                    render: function(data) {
                        return self.escapeHtml(data || '-');
                    }
                },
                {
                    title: "Contact Person Email",
                    data: "contactPersonEmail",
                    visible: false,
                    render: function(data) {
                        return self.escapeHtml(data || '-');
                    }
                },
                {
                    title: "Phone",
                    data: "phone",
                    render: function(data) {
                        return self.escapeHtml(data || '-');
                    }
                },
                {
                    title: "Alternate Phone",
                    data: "alternatePhone",
                    visible: false,
                    render: function(data) {
                        return self.escapeHtml(data || '-');
                    }
                },
                {
                    title: "Email",
                    data: "email",
                    render: function(data) {
                        return self.escapeHtml(data || '-');
                    }
                },
                {
                    title: "Website",
                    data: "website",
                    visible: false,
                    render: function(data) {
                        return self.escapeHtml(data || '-');
                    }
                },
                {
                    title: "Address",
                    data: "address",
                    createdCell: function (td) {
                        $(td).css("min-width", "200px");
                    },
                    render: function(data, type, row) {
                        var addr = self.escapeHtml(data || '');
                        if (row.city) addr += ', ' + row.city;
                        return addr.length > 40 ? addr.substring(0, 40) + '...' : addr;
                    }
                },
                {
                    title: "Full Address",
                    data: "address",
                    visible: false,
                    render: function(data) {
                        return self.escapeHtml(data || '-');
                    }
                },
                {
                    title: "City",
                    data: "city",
                    visible: false,
                    render: function(data) {
                        return self.escapeHtml(data || '-');
                    }
                },
                {
                    title: "State",
                    data: "state",
                    visible: false,
                    render: function(data) {
                        return self.escapeHtml(data || '-');
                    }
                },
                {
                    title: "Country",
                    data: "country",
                    visible: false,
                    render: function(data) {
                        return self.escapeHtml(data || '-');
                    }
                },
                {
                    title: "Pin Code",
                    data: "pinCode",
                    visible: false,
                    render: function(data) {
                        return self.escapeHtml(data || '-');
                    }
                },
                {
                    title: "Latitude",
                    data: "latitude",
                    visible: false,
                    render: function(data) {
                        return data ? parseFloat(data).toFixed(6) : '-';
                    }
                },
                {
                    title: "Longitude",
                    data: "longitude",
                    visible: false,
                    render: function(data) {
                        return data ? parseFloat(data).toFixed(6) : '-';
                    }
                },
                {
                    title: "Geofence",
                    data: "geofenceRadius",
                    render: function(data) {
                        if (!data) return '100m';
                        if (data >= 1000) {
                            return (data / 1000) + 'km';
                        }
                        return data + 'm';
                    }
                },
                {
                    title: "Notes",
                    data: "notes",
                    visible: false,
                    render: function(data) {
                        return self.escapeHtml(data || '-');
                    }
                },
                {
                    title: "Status",
                    data: "status",
                    render: function(data) {
                        return self.getStatusBadge(data);
                    }
                },
                {
                    title: "Created Date",
                    data: "createdDate",
                    visible: false,
                    render: function(data) {
                        return data ? self.formatDate(data) : '-';
                    }
                },
                {
                    title: "Modified Date",
                    data: "modifiedDate",
                    visible: false,
                    render: function(data) {
                        return data ? self.formatDate(data) : '-';
                    }
                },
                {
                    title: "Action",
                    data: null,
                    orderable: false,
                    width: "130px",
                    createdCell: function (td) {
                        $(td).css("min-width", "130px");
                    },
                    render: function (data, type, row) {
                        var str = '';
                        str += '<a class="viewclass btn seagreen_btnnew tooltips" data-placement="top" data-original-title="View" onclick="FieldSenseClient.viewClient(' + row.clientId + ')"><i class="fa-regular fa-eye"></i></a> ';
                        if (self.config.canEdit) {
                            str += '<a class="editclass btn seagreen_btnnew tooltips" data-placement="top" data-original-title="Edit" onclick="FieldSenseClient.editClient(' + row.clientId + ')"><i class="fa-regular fa-pen-to-square"></i></a> ';
                        }
                        if (self.config.canDelete) {
                            str += '<a class="deleteclass btn red_btnnew tooltips" data-placement="top" data-original-title="Delete" onclick="FieldSenseClient.deleteClient(' + row.clientId + ')"><i class="fa-regular fa-trash-can"></i></a>';
                        }
                        return str;
                    }
                }
            ],
            initComplete: function () {
                const visibleTable = $('table:visible').attr('id');
                $("#buttons").append(`
                    <div id="export-button1" class="export-button tooltips" data-placement="bottom" data-original-title="Download" data-table-id="${visibleTable}">
                         <img src="../Newlayout/images/download_icon.svg" alt="Download icon">
                    </div>
                `);
            },
            drawCallback: function () {
                $('.tooltips').tooltip();
            }
        });
        
        setTimeout(function () {
            $("#" + "tblClient" + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive"));
            $('.tooltips').tooltip();
        }, 100);
    },

    // Apply filters
    applyFilters: function() {
        var self = this;
        
        var searchTerm = $('#txtSearch').val().toLowerCase().trim();
        var categoryFilter = $('#ddlFilterCategory').val();
        var statusFilter = $('#ddlFilterStatus').val();
        
        // Get DataTable instance
        var table = $('#tblClient').DataTable();
        
        // Apply custom filtering
        $.fn.dataTable.ext.search.push(
            function(settings, data, dataIndex) {
                if (settings.nTable.id !== 'tblClient') return true;
                
                var row = table.row(dataIndex).data();
                
                // Search filter
                var matchSearch = !searchTerm ||
                    (row.clientName && row.clientName.toLowerCase().includes(searchTerm)) ||
                    (row.phone && row.phone.includes(searchTerm)) ||
                    (row.email && row.email.toLowerCase().includes(searchTerm)) ||
                    (row.description && row.description.toLowerCase().includes(searchTerm)) ||
                    (row.clientCategory && row.clientCategory.toLowerCase().includes(searchTerm)) ||
                    (row.address && row.address.toLowerCase().includes(searchTerm));

                // Category filter
                var matchCategory = !categoryFilter || row.clientCategory === categoryFilter;

                // Status filter
                var matchStatus = !statusFilter || row.status === statusFilter;
                
                return matchSearch && matchCategory && matchStatus;
            }
        );
        
        // Redraw table with filters
        table.draw();
        
        // Update statistics
        /*self.loadStatistics();*/
    },

    // Clear filters
    clearFilters: function() {
        $('#txtSearch').val('');
        $('#ddlFilterCategory').val('');
        $('#ddlFilterStatus').val('');
        
        // Clear custom filters
        $.fn.dataTable.ext.search.pop();
        
        // Redraw table without filters
        var table = $('#tblClient').DataTable();
        table.draw();
        
       /* this.loadStatistics();*/
    },

    // Show client modal (Add/Edit)
    showClientModal: function(clientId) {
        var self = this;
        var isEdit = clientId ? true : false;
        var client = {};

        if (isEdit) {
            client = self.config.clients.find(c => c.clientId === clientId) || {};
        }

        // Update modal title and button text
        if (isEdit) {
            $('#clientModalTitle').text('Edit Client');
            $('#saveButtonText').text('Update Client');
            $('#editInfo').text("Changes to the client's address and radius are not reflected in existing tasks linked to that client.");
        } else {
            $('#clientModalTitle').text('Add New Client');
            $('#saveButtonText').text('Save Client');
            $('#editInfo').empty();
        }

        // Populate form fields
        self.populateClientForm(client, isEdit);

        // Show modal using Bootstrap modal method
        $('#btnAddClient_modal').modal('show');

    },


    // Save client
    saveClient: function() {
        var self = this;
        
        // Validate form
        if (!self.validateClientForm()) {
            return;
        }
        
        // Collect form data matching API structure
        var clientData = {
            clientId: $('#clientId').val() || 0,
            clientName: $('#clientName').val().trim(),
            clientCode: '', // Auto-generated by backend
            clientCategory: $('#clientCategory').val(),
            description: $('#clientDescription').val().trim(),
            address: $('#clientAddress').val().trim(),
            city: $('#clientCity').val() || '',
            state: $('#clientState').val() || '',
            country: $('#clientCountry').val() || 'India',
            pinCode: $('#clientPinCode').val() || '',
            latitude: parseFloat($('#clientLatitude').val()) || null,
            longitude: parseFloat($('#clientLongitude').val()) || null,
            geofenceRadius: parseInt($('#modalRadius').val()) || 100,
            phone: $('#clientPhone').val().trim(),
            alternatePhone: $('#clientAlternatePhone').val() || '',
            email: $('#clientEmail').val().trim(),
            website: $('#clientWebsite').val() || '',
            contactPerson: $('#contactPerson').val() || '',
            contactPersonPhone: $('#contactPersonPhone').val() || '',
            contactPersonEmail: $('#contactPersonEmail').val() || '',
            status: $('#clientActive').is(':checked') ? 'Active' : 'Inactive',
            notes: $('#clientNotes').val() || ''
        };

        var isEdit = parseInt(clientData.clientId) > 0;
        var url = isEdit ? self.config.apiUrl + 'ClientMaster/update/' + clientData.clientId : self.config.apiUrl + 'ClientMaster/create';

        // Show loading
        $('#saveButtonText').html('<i class="fas fa-spinner fa-spin"></i> Saving...');
        
        $.ajax({
            url: url,
            type: isEdit ? 'PUT' : 'POST',
            data: JSON.stringify(clientData),
            contentType: 'application/json',
            headers: {
                'Authorization': 'Bearer ' + self.config.token
            },
            beforeSend: function () {
                $("#ajax_loader").show();
            },
            complete: function () {
                $('#ajax_loader').hide();
            },
            success: function(response) {
                $('#saveButtonText').text(isEdit ? 'Update Client' : 'Save Client');
                // Handle API response structure
                if (response && response.success) {
                    self.showSuccess(response.message || (isEdit ? 'Client updated successfully' : 'Client added successfully'));
                    self.closeModal();
                    
                    // Refresh DataTable
                    if ($.fn.DataTable.isDataTable('#tblClient')) {
                        $('#tblClient').DataTable().destroy();
                        $('#tblClient').empty();
                    }
                    self.loadClients();
                   /* self.loadStatistics();*/
                } else {
                    self.showError(response.message || 'Failed to save client');
                }
            },
            error: function(xhr, status, error) {
                $('#saveButtonText').text(isEdit ? 'Update Client' : 'Save Client');
                console.error('Error saving client:', xhr, status, error);
                self.showError('Error saving client: ' + (xhr.responseJSON?.message || error));
            }
        });
    },


    // Validate client form
    validateClientForm: function() {
        var errors = [];
        
        if (!$('#clientName').val().trim()) {
            errors.push('Client name is required');
        }
        
        if (!$('#clientAddress').val().trim()) {
            errors.push('Address is required');
        }
        
        var email = $('#clientEmail').val().trim();
        if (email && !this.isValidEmail(email)) {
            errors.push('Please enter a valid email address');
        }
        
        // Validate GPS coordinates if provided
        var lat = $('#clientLatitude').val().trim();
        var lng = $('#clientLongitude').val().trim();
        if ((lat && !lng) || (!lat && lng)) {
            errors.push('Please provide both latitude and longitude coordinates');
        }
        
        if (errors.length > 0) {
            this.showError( errors.join('\n | '));
            return false;
        }
        
        return true;
    },

    // View client details
    viewClient: function(clientId) {
        var self = this;
        var client = self.config.clients.find(c => c.clientId === clientId);
        
        if (!client) {
            self.showError('Client not found');
            return;
        }
        
        // Show client details modal
        self.showClientDetailsModal(client);
    },

    // Edit client
    editClient: function(clientId) {
        this.showClientModal(clientId);
    },

    // Delete client
    deleteClient: function(clientId) {
        var self = this;

        if (typeof Swal !== 'undefined') {
            Swal.fire({
                html: '<div class="swal_subtitle"><b>You want to delete this client?</b></div>',
                title: "<img class='margin-right-5' src='/assets/images/icons/swal_icon/icon_error.svg' alt=''>Are you sure?",
                showCloseButton: true,
                showCancelButton: true,
                focusConfirm: false,
                cancelButtonText: 'Cancel',
                confirmButtonText: 'Yes, delete it!',
                allowOutsideClick: false,
                allowEscapeKey: false,
                customClass: {
                    confirmButton: 'swal-confirm-button-class',
                    cancelButton: 'swal-cancel-button-class'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    self.performDelete(clientId);
                }
            });
            $('img[src="/assets/images/icons/swal_icon/icon_error.svg"]').parent().parent().parent().addClass('swal_error');
        } else {
            if (confirm('Are you sure you want to delete this client?')) {
                self.performDelete(clientId);
            }
        }
    },

    // Perform delete operation
    performDelete: function(clientId) {
        var self = this;

        $.ajax({
            url: self.config.apiUrl + 'ClientMaster/' + clientId,
            type: 'DELETE',
            contentType: 'application/json',
            headers: {
                'Authorization': 'Bearer ' + self.config.token
            },
            beforeSend: function () {
                $("#ajax_loader").show();
            },
            complete: function () {
                $('#ajax_loader').hide();
            },
            success: function(response) {
                if (response && response.success) {
                    if (typeof Swal !== 'undefined') {
                        Swal.fire({
                            html: '<div class="swal_subtitle"><b>Client has been deleted.</b></div>',
                            title: "<img class='margin-right-5' src='/assets/images/icons/swal_icon/icon_success.svg' alt=''>Deleted!",
                            showCloseButton: true,
                            confirmButtonText: 'OK',
                            allowOutsideClick: false,
                            allowEscapeKey: false,
                            customClass: {
                                confirmButton: 'swal-confirm-button-class'
                            }
                        });
                        $('img[src="/assets/images/icons/swal_icon/icon_success.svg"]').parent().parent().parent().addClass('swal_success');
                    } else {
                        self.showSuccess('Client deleted successfully');
                    }
                    // Refresh DataTable
                    if ($.fn.DataTable.isDataTable('#tblClient')) {
                        $('#tblClient').DataTable().destroy();
                        $('#tblClient').empty();
                    }
                    self.loadClients();
                } else {
                    if (typeof Swal !== 'undefined') {
                        Swal.fire({
                            html: '<div class="swal_subtitle"><b>' + (response.message || "Failed to delete client.") + '</b></div>',
                            title: "<img class='margin-right-5' src='/assets/images/icons/swal_icon/icon_error.svg' alt=''>Error!",
                            showCloseButton: true,
                            confirmButtonText: 'OK',
                            allowOutsideClick: false,
                            allowEscapeKey: false,
                            customClass: {
                                confirmButton: 'swal-confirm-button-class'
                            }
                        });
                        $('img[src="/assets/images/icons/swal_icon/icon_error.svg"]').parent().parent().parent().addClass('swal_error');
                    } else {
                        self.showError(response.message || 'Failed to delete client');
                    }
                }
            },
            error: function(xhr, status, error) {
                console.error('Error deleting client:', xhr, status, error);
                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        html: '<div class="swal_subtitle"><b>Error deleting client.</b></div>',
                        title: "<img class='margin-right-5' src='/assets/images/icons/swal_icon/icon_error.svg' alt=''>Error!",
                        showCloseButton: true,
                        confirmButtonText: 'OK',
                        allowOutsideClick: false,
                        allowEscapeKey: false,
                        customClass: {
                            confirmButton: 'swal-confirm-button-class'
                        }
                    });
                    $('img[src="/assets/images/icons/swal_icon/icon_error.svg"]').parent().parent().parent().addClass('swal_error');
                } else {
                    self.showError('Error deleting client');
                }
            }
        });
    },

    // Open map picker (placeholder)
    openMapPicker: function() {
        this.showInfo('Map picker functionality will be implemented with Google Maps API');
    },

    // Show client details modal
    showClientDetailsModal: function(client) {
        var self = this;

        // Populate header section
        $('#detailClientName').text(client.clientName || 'N/A');
        $('#detailClientCode').text(client.clientCode || 'N/A');
        $('#detailClientCategory').text(client.clientCategory || 'N/A');

        // Set status with appropriate styling
        var statusElement = $('#detailClientStatus');
        statusElement.text(client.status || 'Active');
        statusElement.css('background', client.status === 'Active' ? '#fff' : '#fff');
        statusElement.css('color', client.status === 'Active' ? '#1D8B3C' : '#1D8B3C');

        // Basic Information
        $('#detailDescription').text(client.description || 'No description available');

        // Contact Information
        $('#detailPhone').html(client.phone ? '<i class="fa fa-phone"></i> ' + client.phone : 'Not provided');
        $('#detailAlternatePhone').text(client.alternatePhone || 'Not provided');
        $('#detailEmail').html(client.email ? '<a href="mailto:' + client.email + '">' + client.email + '</a>' : 'Not provided');
        $('#detailWebsite').html(client.website ? '<a href="' + client.website + '" target="_blank">' + client.website + '</a>' : 'Not provided');

        // Contact Person
        $('#detailContactPerson').text(client.contactPerson || 'Not provided');
        $('#detailContactPersonPhone').html(client.contactPersonPhone ? '<i class="fa fa-phone"></i> ' + client.contactPersonPhone : 'Not provided');
        $('#detailContactPersonEmail').html(client.contactPersonEmail ? '<a href="mailto:' + client.contactPersonEmail + '">' + client.contactPersonEmail + '</a>' : 'Not provided');

        // Location Information
        $('#detailFullAddress').text(client.address || 'Not provided');
        $('#detailCity').text(client.city || 'Not provided');
        $('#detailState').text(client.state || 'Not provided');
        $('#detailCountry').text(client.country || 'Not provided');
        $('#detailPinCode').text(client.pinCode || 'Not provided');

        // GPS and Geofence
        if (client.latitude && client.longitude) {
            $('#detailGPS').html('<i class="fa fa-map-marker"></i> ' + parseFloat(client.latitude).toFixed(6) + ', ' + parseFloat(client.longitude).toFixed(6));
        } else {
            $('#detailGPS').text('Not set');
        }

        var geofenceText = 'Not set';
        if (client.geofenceRadius) {
            if (client.geofenceRadius >= 1000) {
                geofenceText = (client.geofenceRadius / 1000) + ' km';
            } else {
                geofenceText = client.geofenceRadius + ' meters';
            }
        }
        $('#detailGeofence').text(geofenceText);

        // Additional Information
        $('#detailNotes').text(client.notes || 'No additional notes');
        $('#detailTotalVisits').text(client.totalVisits || '0');
        $('#detailLastVisit').text(client.lastVisitDate ? self.formatDate(client.lastVisitDate) : 'Never visited');

        // Timestamps
        $('#detailCreatedDate').text(client.createdDate ? self.formatDate(client.createdDate) : 'N/A');
        $('#detailModifiedDate').text(client.modifiedDate ? self.formatDate(client.modifiedDate) : 'N/A');

        // Store client ID for edit functionality
        $('#btnViewClient_modal').data('clientId', client.clientId);

        // Show modal using Bootstrap modal method
        $('#btnViewClient_modal').modal('show');
    },

    // Helper functions

    getStatusBadge: function(status) {
        var badgeClass = '';
        switch(status) {
            case 'Active':
                badgeClass = 'green_font';
                break;
            case 'Inactive':
                badgeClass = 'red_font';
                break;
            case 'Blocked':
                badgeClass = 'orange_font';
                break;
            default:
                badgeClass = 'navyblue_font';
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

    isValidEmail: function(email) {
        var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
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


    // Populate client form with data
    populateClientForm: function(client, isEdit) {
        if (isEdit && client) {
            $('#clientId').val(client.clientId || '');
            $('#clientName').val(client.clientName || '');
            // clientCode removed - auto-generated by backend
            $('#clientCategory').val(client.clientCategory || '');
            $('#clientDescription').val(client.description || '');
            $('#clientAddress').val(client.address || '');
            $('#clientCity').val(client.city || '');
            $('#clientState').val(client.state || '');
            $('#clientCountry').val(client.country || 'India');
            $('#clientPinCode').val(client.pinCode || '');
            $('#clientLatitude').val(client.latitude || '');
            $('#clientLongitude').val(client.longitude || '');
            $('#modalRadius').val(client.geofenceRadius || 100);

            // Update location status if coordinates exist
            if (client.latitude && client.longitude) {
                $('#locationStatus').text('Location set: ' + parseFloat(client.latitude).toFixed(4) + ', ' + parseFloat(client.longitude).toFixed(4)).removeClass('text-muted').addClass('text-success');
            }
            $('#clientPhone').val(client.phone || '');
            $('#clientAlternatePhone').val(client.alternatePhone || '');
            $('#clientEmail').val(client.email || '');
            $('#clientWebsite').val(client.website || '');
            $('#contactPerson').val(client.contactPerson || '');
            $('#contactPersonPhone').val(client.contactPersonPhone || '');
            $('#contactPersonEmail').val(client.contactPersonEmail || '');
            $('#clientNotes').val(client.notes || '');
            $('#clientActive').prop('checked', client.status === 'Active');
        } else {
            // Clear form for new client
            $('#clientForm')[0].reset();
            $('#clientId').val('');
            $('#clientCountry').val('India');
            $('#modalRadius').val('100');
            $('#clientActive').prop('checked', true);

            // Reset location status to default
            $('#locationStatus')
                .html('<i class="fas fa-info-circle"></i> No location selected')
                .removeClass('text-success')
                .addClass('text-muted');

            // Clear hidden coordinate fields
            $('#clientLatitude').val('');
            $('#clientLongitude').val('');
        }
    },

    // Close client modal using Bootstrap modal method
    closeModal: function() {
        // Hide modal using Bootstrap modal method
        $('#btnAddClient_modal').modal('hide');
    },

    // Global function for closing any modal (called from HTML)
    closeClientModal: function() {
        this.closeModal();
    },


    // Pick location on map
    pickLocationOnMap: function() {
        var self = this;
        var currentLocation = null;
        var radiusView = false;
        if (self.config.roleId == 1 || self.config.roleId == 6805 || self.config.roleId == 6806) {
            radiusView = true;
        }
        // Get current coordinates if available
        var lat = parseFloat($('#clientLatitude').val());
        var lng = parseFloat($('#clientLongitude').val());

        if (!isNaN(lat) && !isNaN(lng) && FieldSenseLocationService.isValidCoordinate(lat, lng)) {
            currentLocation = { lat: lat, lng: lng };
        }

        // Show location picker modal
        FieldSenseLocationService.showLocationPicker({
            currentLocation: currentLocation,
            showRadius: radiusView,
            showSearch: true,
            showCurrentLocation: true,
            title: 'Select Client Location'
        }, function(result) {
            if (result) {
                // Set coordinates
                $('#clientLatitude').val(result.lat.toFixed(6));
                $('#clientLongitude').val(result.lng.toFixed(6));

                // Update location status
                $('#locationStatus').text('Location set: ' + result.lat.toFixed(4) + ', ' + result.lng.toFixed(4)).removeClass('text-muted').addClass('text-success');

                // Always fill address field if available (remove the check for empty field)
                if (result.address) {
                    $('#clientAddress').val(result.address);

                    // Try to parse the address string to fill city, state, etc.
                    self.parseAddressString(result.address);
                }

                // Mark fields as modified for validation
                $('#clientLatitude, #clientLongitude').trigger('change');
            }
        });
    },

    // Helper function to parse address string when components not available
    parseAddressString: function(addressStr) {
        if (!addressStr) return;

        console.log('Parsing address:', addressStr);

        // Try to extract city, state, pincode from address string
        // Common formats:
        // "Street, Area, City, State Pincode, Country"
        // "Place, City, State, Country"
        var parts = addressStr.split(',').map(function(s) { return s.trim(); });

        // Try to find and extract pincode (6 digits for India)
        var pincodeMatch = addressStr.match(/\b\d{6}\b/);
        if (pincodeMatch) {
            $('#clientPinCode').val(pincodeMatch[0]);
        }

        // Check if last part is country
        var lastPart = parts[parts.length - 1];
        if (lastPart && (lastPart.toLowerCase().includes('india') || lastPart.toLowerCase() === 'in')) {
            $('#clientCountry').val('India');
            parts.pop(); // Remove country from parts for easier parsing
        }

        // Extended list of Indian states
        var statePatterns = [
            'Andhra Pradesh', 'Arunachal Pradesh', 'Assam', 'Bihar', 'Chhattisgarh',
            'Goa', 'Gujarat', 'Haryana', 'Himachal Pradesh', 'Jharkhand',
            'Karnataka', 'Kerala', 'Madhya Pradesh', 'Maharashtra', 'Manipur',
            'Meghalaya', 'Mizoram', 'Nagaland', 'Odisha', 'Punjab',
            'Rajasthan', 'Sikkim', 'Tamil Nadu', 'Telangana', 'Tripura',
            'Uttar Pradesh', 'Uttarakhand', 'West Bengal', 'Delhi', 'NCR'
        ];

        var stateFound = false;
        var cityFound = false;

        // Look for state in the address parts (usually towards the end)
        for (var i = parts.length - 1; i >= 0; i--) {
            if (!stateFound) {
                for (var j = 0; j < statePatterns.length; j++) {
                    // Check if part contains state name (case insensitive)
                    if (parts[i].toLowerCase().includes(statePatterns[j].toLowerCase())) {
                        $('#clientState').val(statePatterns[j]);
                        stateFound = true;

                        // The part before state is likely the city
                        if (i > 0 && !cityFound) {
                            var cityPart = parts[i - 1];
                            // Remove pincode from city if present
                            cityPart = cityPart.replace(/\d{6}/g, '').trim();
                            if (cityPart) {
                                $('#clientCity').val(cityPart);
                                cityFound = true;
                            }
                        }
                        break;
                    }
                }
            }
        }

        // If state not found but we have parts, try to guess city from second-to-last or third-to-last part
        if (!cityFound && parts.length >= 2) {
            // Usually city is in the later parts of address but before state/country
            var possibleCityIndex = stateFound ? parts.length - 2 : parts.length - 1;
            if (possibleCityIndex >= 0 && possibleCityIndex < parts.length) {
                var possibleCity = parts[possibleCityIndex];
                // Clean up - remove pincode and numbers
                possibleCity = possibleCity.replace(/\d{6}/g, '').replace(/^\d+\s*/, '').trim();
                if (possibleCity && possibleCity.length > 2) {
                    $('#clientCity').val(possibleCity);
                }
            }
        }

        // Set default country if not already set
        if (!$('#clientCountry').val()) {
            $('#clientCountry').val('India');
        }
    },

    // Email validation helper
    isValidEmail: function(email) {
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }
};

// Global functions for HTML onclick events
function openClientModal(clientId) {
    if (typeof FieldSenseClient !== 'undefined') {
        FieldSenseClient.showClientModal(clientId);
    }
}

function closeClientModal() {
    if (typeof FieldSenseClient !== 'undefined') {
        FieldSenseClient.closeModal();
    }
}

function saveClient() {
    if (typeof FieldSenseClient !== 'undefined') {
        FieldSenseClient.saveClient();
    }
}

function pickLocationOnMap() {
    if (typeof FieldSenseClient !== 'undefined') {
        FieldSenseClient.pickLocationOnMap();
    }
}

function closeClientDetailsModal() {
    // Hide modal using Bootstrap modal method
    $('#btnViewClient_modal').modal('hide');
}

function editClientFromDetails() {
    var clientId = $('#btnViewClient_modal').data('clientId');
    if (clientId && typeof FieldSenseClient !== 'undefined') {
        // Close details modal using Bootstrap method
        $('#btnViewClient_modal').modal('hide');

        // Wait for modal to close, then open edit modal
        $('#btnViewClient_modal').on('hidden.bs.modal', function() {
            // Remove the event handler to prevent multiple bindings
            $(this).off('hidden.bs.modal');
            // Open edit modal
            FieldSenseClient.editClient(clientId);
        });
    }
}

// Auto-initialize if jQuery is ready
if (typeof $ !== 'undefined') {
    $(document).ready(function() {
        // Wait a bit for the page to fully load
        setTimeout(function() {
            if (typeof FieldSenseClient !== 'undefined') {
                // Don't auto-initialize here, let the view handle it
                console.log('FieldSenseClient ready for initialization');
            }
        }, 100);
    });
}