/**
 * Employee Journey Module
 * Displays employee's daily journey based on task check-in/out events
 * Supports both Google Maps and OpenStreetMap providers
 */

var EmployeeJourney = (function () {
    'use strict';

    // Private variables
    var config = {};
    var locationService = null;
    var mapInstance = null;
    var journeyData = null;
    var markers = [];
    var polylines = [];
    var autoRefreshInterval = null;
    var AUTO_REFRESH_INTERVAL = 180000; // 3 minutes in milliseconds
    var calculatedDistance = 0; // Stores total distance from Google routing (in km)
    var routingEnabled = false; // Flag to indicate if routing was successful
    var currentPopupMarker = null; // Track currently open popup
    var currentPopupInstance = null; // Track popup instance for manual closing

    // Status colors matching the UI
    var STATUS_COLORS = {
        'Working': '#555555',      // Grey
        'OnTheWay': '#ffc107',     // Yellow
        'Completed': '#1D8B3C',    // Green
        'Pending': '#FE7A00',      // Orange
        'Overdue': '#dc3545'       // Red
    };

    var EMPLOYEE_COLOR = '#dc3545'; // Red for employee marker

    /**
     * Generate custom SVG marker icon based on status
     */
    function createTaskMarkerIcon(status, number) {
        var color = STATUS_COLORS[status] || STATUS_COLORS['Pending'];
        var iconPath = '';

        // FontAwesome icon paths for different statuses
        switch (status) {
            case 'Working':
                // User-check icon
                iconPath = 'M224 256c70.7 0 128-57.3 128-128S294.7 0 224 0 96 57.3 96 128s57.3 128 128 128zm89.6 32h-16.7c-22.2 10.2-46.9 16-72.9 16s-50.6-5.8-72.9-16h-16.7C60.2 288 0 348.2 0 422.4V464c0 26.5 21.5 48 48 48h352c26.5 0 48-21.5 48-48v-41.6c0-74.2-60.2-134.4-134.4-134.4zM352 304c0-35.3 28.7-64 64-64h32c35.3 0 64 28.7 64 64v40c0 13.3-10.7 24-24 24h-48V304z';
                break;
            case 'Completed':
                // Check-circle icon
                iconPath = 'M504 256c0 136.967-111.033 248-248 248S8 392.967 8 256 119.033 8 256 8s248 111.033 248 248zM227.314 387.314l184-184c6.248-6.248 6.248-16.379 0-22.627l-22.627-22.627c-6.248-6.249-16.379-6.249-22.628 0L216 308.118l-70.059-70.059c-6.248-6.248-16.379-6.248-22.628 0l-22.627 22.627c-6.248 6.248-6.248 16.379 0 22.627l104 104c6.249 6.249 16.379 6.249 22.628.001z';
                break;
            case 'OnTheWay':
                // Walking icon
                iconPath = 'M208 96c26.5 0 48-21.5 48-48S234.5 0 208 0s-48 21.5-48 48 21.5 48 48 48zm94.5 149.1l-23.3-11.8-9.7-29.4c-14.7-44.6-55.7-75.8-102.2-75.9-36-.1-55.9 10.1-93.3 25.2-21.6 8.7-39.3 25.2-49.7 46.2L17.6 213c-7.8 15.8-1.5 35 14.2 42.9 15.6 7.9 34.6 1.5 42.5-14.3L81 228c3.5-7 9.3-12.5 16.5-15.4l26.8-10.8-15.2 60.7c-5.2 20.8.4 42.9 14.9 58.8l59.9 65.4c7.2 7.9 12.3 17.4 14.9 27.7l18.3 73.3c4.3 17.1 21.7 27.6 38.8 23.3 17.1-4.3 27.6-21.7 23.3-38.8l-22.2-89c-2.6-10.3-7.7-19.9-14.9-27.7l-45.5-49.7 17.2-68.7 5.5 16.5c5.3 16.1 16.7 29.4 31.7 37l23.3 11.8c15.6 7.9 34.6 1.5 42.5-14.3 7.7-15.7 1.4-35.1-14.3-43z';
                break;
            case 'Pending':
            default:
                // Clock icon
                iconPath = 'M256 8C119 8 8 119 8 256s111 248 248 248 248-111 248-248S393 8 256 8zm0 448c-110.5 0-200-89.5-200-200S145.5 56 256 56s200 89.5 200 200-89.5 200-200 200zm61.8-104.4l-84.9-61.7c-3.1-2.3-4.9-5.9-4.9-9.7V116c0-6.6 5.4-12 12-12h32c6.6 0 12 5.4 12 12v141.7l66.8 48.6c5.4 3.9 6.5 11.4 2.6 16.8L334.6 349c-3.9 5.3-11.4 6.5-16.8 2.6z';
                break;
        }

        // Create SVG with pin shape, icon, and number
        var svg = '<svg xmlns="http://www.w3.org/2000/svg" width="48" height="64" viewBox="0 0 48 64">' +
            // Drop shadow
            '<defs>' +
            '<filter id="shadow-' + status + '" x="-50%" y="-50%" width="200%" height="200%">' +
            '<feGaussianBlur in="SourceAlpha" stdDeviation="2"/>' +
            '<feOffset dx="0" dy="2" result="offsetblur"/>' +
            '<feComponentTransfer><feFuncA type="linear" slope="0.3"/></feComponentTransfer>' +
            '<feMerge><feMergeNode/><feMergeNode in="SourceGraphic"/></feMerge>' +
            '</filter>' +
            '</defs>' +
            // Pin shape
            '<path d="M24 0 C10 0 0 10 0 24 C0 32 24 64 24 64 S48 32 48 24 C48 10 38 0 24 0 Z" ' +
            'fill="' + color + '" stroke="#ffffff" stroke-width="2" filter="url(#shadow-' + status + ')"/>' +
            // White circle background for icon
            '<circle cx="24" cy="22" r="14" fill="#ffffff" opacity="0.9"/>';

        // Add icon
        if (iconPath) {
            svg += '<g transform="translate(16, 14) scale(0.015)">' +
                '<path d="' + iconPath + '" fill="' + color + '"/>' +
                '</g>';
        }

        // Add number if provided
        if (number) {
            svg += '<text x="24" y="50" font-family="Arial, sans-serif" font-size="12" font-weight="bold" ' +
                'fill="#ffffff" text-anchor="middle" stroke="' + color + '" stroke-width="3" paint-order="stroke">' +
                number + '</text>';
        }

        svg += '</svg>';

        return 'data:image/svg+xml;charset=UTF-8,' + encodeURIComponent(svg);
    }

    /**
     * Create employee marker icon
     */
    function createEmployeeMarkerIcon() {
        var svg = '<svg xmlns="http://www.w3.org/2000/svg" width="48" height="64" viewBox="0 0 48 64">' +
            '<defs>' +
            '<filter id="shadow-employee" x="-50%" y="-50%" width="200%" height="200%">' +
            '<feGaussianBlur in="SourceAlpha" stdDeviation="2"/>' +
            '<feOffset dx="0" dy="2" result="offsetblur"/>' +
            '<feComponentTransfer><feFuncA type="linear" slope="0.3"/></feComponentTransfer>' +
            '<feMerge><feMergeNode/><feMergeNode in="SourceGraphic"/></feMerge>' +
            '</filter>' +
            '</defs>' +
            '<path d="M24 0 C10 0 0 10 0 24 C0 32 24 64 24 64 S48 32 48 24 C48 10 38 0 24 0 Z" ' +
            'fill="' + EMPLOYEE_COLOR + '" stroke="#ffffff" stroke-width="2" filter="url(#shadow-employee)"/>' +
            '<circle cx="24" cy="22" r="14" fill="#ffffff" opacity="0.9"/>' +
            // User icon
            '<g transform="translate(16, 14) scale(0.015)">' +
            '<path d="M224 256c70.7 0 128-57.3 128-128S294.7 0 224 0 96 57.3 96 128s57.3 128 128 128zm89.6 32h-16.7c-22.2 10.2-46.9 16-72.9 16s-50.6-5.8-72.9-16h-16.7C60.2 288 0 348.2 0 422.4V464c0 26.5 21.5 48 48 48h352c26.5 0 48-21.5 48-48v-41.6c0-74.2-60.2-134.4-134.4-134.4z" ' +
            'fill="' + EMPLOYEE_COLOR + '"/>' +
            '</g>' +
            '</svg>';

        return 'data:image/svg+xml;charset=UTF-8,' + encodeURIComponent(svg);
    }

    /**
     * Initialize the module
     */
    function initialize(appConfig) {
        config = appConfig;

        // Check if coming from Task Management (context-aware initialization)
        if (config.sourceContext === 'task' && config.sourceTaskId) {
            toastr.info('Loading employee journey for task #' + config.sourceTaskId);
        }

        // Set date: use pre-selected date if available, otherwise today
        if (config.preSelectedDate) {
            $('#journeyDate').val(config.preSelectedDate);
        } else {
            setDefaultDate();
        }

        // Set up event listeners
        setupEventListeners();

        // Initialize location service, then load employees after it's ready
        initializeLocationService(function () {
            // Location service is ready, now load employees
            loadEmployees();
        });
    }

    /**
     * Initialize location service based on provider
     */
    function initializeLocationService(callback) {
        if (typeof window.FieldSenseLocationService !== 'undefined') {
            locationService = window.FieldSenseLocationService;

            // Initialize the service before using it
            locationService.init(function () {
                // Call the callback to indicate location service is ready
                if (callback) callback();
            });
        } else {
            toastr.error('Map service not available. Please refresh the page.');
        }
    }

    /**
     * Set up event listeners
     */
    function setupEventListeners() {
        // Refresh button
        $('#btnRefresh').on('click', function () {
            loadEmployeeJourney();
        });

        // Employee selection change
        $('#employeeSelect').on('change', function () {
            var employeeId = $(this).val();
            if (employeeId) {
                loadEmployeeJourney();
            }
        });

        // Date change
        $('#journeyDate').on('change', function () {
            var employeeId = $('#employeeSelect').val();
            if (employeeId) {
                loadEmployeeJourney();
            }
        });
    }

    /**
     * Set default date to today
     */
    function setDefaultDate() {
        var today = new Date();
        var dateString = today.toISOString().split('T')[0];
        $('#journeyDate').val(dateString);
    }

    /**
     * Check if selected date is today
     */
    function isSelectedDateToday() {
        var selectedDate = $('#journeyDate').val();
        if (!selectedDate) return false;

        var today = new Date();
        var todayString = today.toISOString().split('T')[0];

        return selectedDate === todayString;
    }

    /**
     * Load employees dropdown based on role
     */
    function loadEmployees() {
        var url = config.apiUrl + 'master/employees/accessible';



        $.ajax({
            url: url,
            type: 'GET',
            headers: {
                'Authorization': 'Bearer ' + config.token
            },
            contentType: 'application/json',
            success: function (response) {
                if (response.success && response.data) {
                    populateEmployeeDropdown(response.data);
                } else {
                    toastr.error('Failed to load employees');
                }
            },
            error: function (xhr, status, error) {
                toastr.error('Error loading employees');
            }
        });
    }

    /**
     * Populate employee dropdown with priority-based pre-selection
     */
    function populateEmployeeDropdown(employees) {
        var $select = $('#employeeSelect');
        $select.empty();
        $select.append('<option value="">Select Employee</option>');

        if (employees && employees.length > 0) {
            employees.forEach(function (emp) {
                $select.append(
                    $('<option>', {
                        value: emp.empId,
                        text: emp.empName + ' (' + emp.empcode + ')'
                    })
                );
            });

            // Auto-select with priority: URL parameter > current user > first employee
            var selectedEmployeeId = null;
            var selectionSource = '';

            // Priority 1: Pre-selected employee from URL parameter (e.g., from Task Management)
            if (config.preSelectedEmployeeId) {
                var preSelectedOption = $select.find('option[value="' + config.preSelectedEmployeeId + '"]');
                if (preSelectedOption.length > 0) {
                    selectedEmployeeId = config.preSelectedEmployeeId;
                    selectionSource = 'URL parameter';
                } else {
                    toastr.warning('Selected employee not found in accessible employees. Showing default.');
                }
            }

            // Priority 2: Current logged-in user (if not already selected)
            if (!selectedEmployeeId) {
                var currentEmpOption = $select.find('option[value="' + config.empId + '"]');
                if (currentEmpOption.length > 0) {
                    selectedEmployeeId = config.empId;
                    selectionSource = 'current user';
                }
            }

            // Priority 3: First employee in the list (fallback)
            if (!selectedEmployeeId && employees.length > 0) {
                selectedEmployeeId = employees[0].empId;
                selectionSource = 'first employee';
            }

            // Apply selection and load journey
            if (selectedEmployeeId) {
                $select.val(selectedEmployeeId);

                // Show context-aware notification when redirected from Task Management
                if (config.sourceContext === 'task' && selectionSource === 'URL parameter') {
                    var selectedEmployeeName = $select.find('option:selected').text();
                    toastr.success('Viewing journey for: ' + selectedEmployeeName);
                }

                loadEmployeeJourney();
            }
        } else {
            toastr.warning('No field employees found');
        }
    }

    /**
     * Load employee journey data from API
     */
    function loadEmployeeJourney() {
        var employeeId = $('#employeeSelect').val();
        var date = $('#journeyDate').val();

        if (!employeeId) {
            toastr.warning('Please select an employee');
            return;
        }

        if (!date) {
            toastr.warning('Please select a date');
            return;
        }

        // Show loading
        showLoading(true);
        $('#btnRefresh').prop('disabled', true);

        var url = config.apiUrl + 'taskmanagement/employee-journey';
        var params = '?employeeId=' + employeeId + '&date=' + date;

        $.ajax({
            url: url + params,
            type: 'GET',
            headers: {
                'Authorization': 'Bearer ' + config.token
            },
            success: function (response) {
                if (response.success && response.data) {
                    journeyData = response.data;
                    renderJourney();
                } else {
                    toastr.error('Failed to load journey data');
                    showEmptyState();
                }
            },
            error: function (xhr, status, error) {
                toastr.error('Error loading journey data');
                showEmptyState();
            },
            complete: function () {
                showLoading(false);
                $('#btnRefresh').prop('disabled', false);
            }
        });
    }

    /**
     * Render complete journey on map
     */
    function renderJourney() {
        if (!journeyData || !journeyData.tasks || journeyData.tasks.length === 0) {
            showEmptyState();
            return;
        }

        // Sort tasks by check-in time to show journey in chronological order
        // Tasks without check-in time (pending) will be placed at the end
        journeyData.tasks.sort(function (a, b) {
            // If neither has check-in time, maintain original order (by sequenceNumber if available)
            if (!a.checkInTime && !b.checkInTime) {
                if (a.sequenceNumber && b.sequenceNumber) {
                    return a.sequenceNumber - b.sequenceNumber;
                }
                return 0;
            }
            // Tasks without check-in go to end
            if (!a.checkInTime) return 1;
            if (!b.checkInTime) return -1;

            // Both have check-in time - sort chronologically
            return new Date(a.checkInTime) - new Date(b.checkInTime);
        });

        // Clear existing markers and polylines
        clearMap();

        // Reset calculated distance
        calculatedDistance = 0;
        routingEnabled = false;

        // Initialize map if not already done
        if (!mapInstance) {
            initializeMap();
        }

        // Remove empty state if it exists
        $('.right-sidebar').find('.journey-empty-state').remove();

        // Track async rendering completion
        var markersReady = false;
        var routesReady = false;

        function checkAndFitBounds() {
            if (markersReady && routesReady) {
                fitMapToMarkers();
            }
        }

        // Render all components with completion callbacks
        renderTaskMarkers(function() {
            markersReady = true;
            checkAndFitBounds();
        });

        // Only show employee current location marker when viewing today's journey
        if (isSelectedDateToday()) {
           // renderEmployeeMarker();
        }

        renderJourneyPath(function() {
            routesReady = true;
            checkAndFitBounds();
        });

        updateTaskList();

        // Show panels
        $('#summaryPanel').show();
        $('#taskListSidebar').show();

        // Start auto-refresh
        startAutoRefresh();
    }

    /**
     * Initialize map
     */
    function initializeMap() {
        if (!locationService) {
            toastr.error('Map service not ready. Please wait...');
            return;
        }

        if (!locationService.isInitialized) {
            toastr.error('Map service is loading. Please wait...');
            return;
        }

        var mapConfig = {
            containerId: 'journeyMap',
            center: { lat: 20.5937, lng: 78.9629 }, // India center as default
            zoom: 6
        };

        locationService.initializeMap(mapConfig, function (map) {
            mapInstance = map;
        }, function (error) {
            toastr.error('Failed to initialize map');
        });
    }

    /**
     * Render task markers on map
     */
    function renderTaskMarkers(onComplete) {
        if (!journeyData.tasks || !locationService || !locationService.isInitialized) {
            if (onComplete) onComplete();
            return;
        }

        // Count total markers to add
        var totalMarkers = journeyData.tasks.filter(function(task) {
            return task.latitude && task.longitude;
        }).length;

        if (totalMarkers === 0) {
            if (onComplete) onComplete();
            return;
        }

        // Track markers added
        var markersAdded = 0;

        // Track journey sequence number (only for checked-in tasks)
        var journeySequence = 0;

        journeyData.tasks.forEach(function (task, index) {
            if (!task.latitude || !task.longitude) return;

            var position = { lat: task.latitude, lng: task.longitude };

            // Only number tasks that have been checked in (have checkInTime)
            // Pending tasks will not have a number
            var markerNumber = '';
            if (task.checkInTime) {
                journeySequence++;
                markerNumber = journeySequence.toString();
            }

            // Create custom icon for this task
            var customIcon = createTaskMarkerIcon(task.mapStatus, markerNumber);

            // Create marker with custom icon
            var markerConfig = {
                position: position,
                title: task.taskTitle,
                icon: customIcon,
                width: 48,
                height: 64,
                zIndex: task.checkInTime ? (100 + journeySequence) : 50 // Pending tasks have lower z-index
            };

            locationService.addMarker(markerConfig, function (marker) {
                markers.push(marker);

                // Add click handler for marker
                locationService.addMarkerClickListener(marker, function () {
                    showTaskPopup(task);
                });

                // Track completion
                markersAdded++;
                if (markersAdded === totalMarkers && onComplete) {
                    onComplete();
                }
            });
        });
    }

    /**
     * Render employee current location marker
     */
    function renderEmployeeMarker() {
        if (!journeyData.currentLocation || !locationService || !locationService.isInitialized) {
            return;
        }

        var loc = journeyData.currentLocation;
        if (!loc.latitude || !loc.longitude) return;

        var position = { lat: loc.latitude, lng: loc.longitude };

        // Create custom employee icon
        var customIcon = createEmployeeMarkerIcon();

        var markerConfig = {
            position: position,
            title: 'Current Location',
            icon: customIcon,
            width: 48,
            height: 64,
            zIndex: 1000 // Highest priority
        };

        locationService.addMarker(markerConfig, function (marker) {
            markers.push(marker);

            // Add popup for current location
            var popupContent = '<div style="padding: 10px;">' +
                '<h4 style="margin: 0 0 10px 0;">Current Location</h4>' +
                '<p style="margin: 5px 0;"><strong>Time:</strong> ' + formatDateTime(loc.lastSeenTime) + '</p>' +
                '<p style="margin: 5px 0;"><strong>Status:</strong> ' + loc.status + '</p>' +
                '</div>';

            locationService.addMarkerClickListener(marker, function () {
                closeCurrentPopup();
                locationService.showPopup(marker, popupContent);
                currentPopupMarker = marker;
            });
        });
    }

    /**
     * Render journey path as polyline
     * Uses Google Directions API or OSRM routing based on provider
     */
    function renderJourneyPath(onComplete) {
        if (!journeyData.tasks || journeyData.tasks.length === 0 || !locationService || !locationService.isInitialized) {
            if (onComplete) onComplete();
            return;
        }

        // Detect current location provider from location service config
        var locationProvider = '';
        if (locationService.config && locationService.config.LocationProvider) {
            locationProvider = locationService.config.LocationProvider.toLowerCase();
        }

        if (locationProvider === 'google' && locationService.provider && locationService.provider.calculateRoute) {
            // Use Google Directions API for routed paths
            renderRoutedPath(onComplete);
        } else if (locationProvider === 'opensource' && locationService.provider && locationService.provider.getOSRMRoute) {
            // Use OSRM for OpenSource provider routing
            renderOSRMRoutedPath(onComplete);
        } else {
            // Use straight-line paths for unknown providers or fallback
            renderStraightPath(onComplete);
        }
    }

    /**
     * Render routed paths using Google Directions API
     */
    function renderRoutedPath(onComplete) {
        // Get tasks with valid coordinates that have been checked in (chronologically sorted)
        var validTasks = journeyData.tasks.filter(function (task) {
            return task.latitude && task.longitude && task.mapStatus != "Pending" && task.mapStatus != "Overdue" /*task.checkInTime;*/
        });

        if (validTasks.length < 2) {
            // Not enough checked-in points to create a route
            updateSummaryPanel();
            if (onComplete) onComplete();
            return;
        }

        var routeSegments = [];
        var totalDistanceMeters = 0;
        var routesCompleted = 0;
        var routesToCalculate = validTasks.length - 1;

        // Request routes for each consecutive pair of tasks
        for (var i = 0; i < validTasks.length - 1; i++) {
            (function (index) {
                var origin = { lat: validTasks[index].latitude, lng: validTasks[index].longitude };
                var destination = { lat: validTasks[index + 1].latitude, lng: validTasks[index + 1].longitude };
                var status = validTasks[index + 1].mapStatus;

                locationService.provider.calculateRoute(origin, destination, {}, function (result) {
                    routesCompleted++;

                    if (result && result.success) {
                        routeSegments.push({
                            index: index,
                            path: result.path,
                            distance: result.distance,
                            status: status
                        });
                        totalDistanceMeters += result.distance;
                    } else {
                        // Route failed, use straight line as fallback
                        routeSegments.push({
                            index: index,
                            path: [origin, destination],
                            distance: 0,
                            status: status,
                            fallback: true
                        });
                    }

                    // Check if all routes are calculated
                    if (routesCompleted === routesToCalculate) {
                        // Sort segments by index to ensure correct order
                        routeSegments.sort(function (a, b) { return a.index - b.index; });

                        // Draw all route segments with enhanced styling
                        routeSegments.forEach(function (segment, idx) {
                            var color, outlineColor, isDashed;

                            // Determine color and style based on task status
                            if (segment.status === 'Completed') {
                                color = '#007bff'; // Blue
                                outlineColor = '#0056b3';
                                isDashed = false;
                            } else if (segment.status === 'Working') {
                                color = '#28a745'; // Green
                                outlineColor = '#1e7e34';
                                isDashed = false;
                            } else if (segment.status === 'OnTheWay') {
                                color = '#ffc107'; // Yellow
                                outlineColor = '#d39e00';
                                isDashed = false;
                            } else {
                                color = '#6c757d'; // Gray for Pending
                                outlineColor = '#495057';
                                isDashed = false;
                            }

                            // Draw outline (shadow effect) for non-fallback routes
                            if (!segment.fallback) {
                                var outlineConfig = {
                                    path: segment.path,
                                    strokeColor: outlineColor,
                                    strokeWeight: 8,
                                    strokeOpacity: 0.3,
                                    zIndex: 1
                                };
                                locationService.drawPolyline(outlineConfig, function (polyline) {
                                    polylines.push(polyline);
                                });
                            }

                            // Draw main line
                            var lineConfig = {
                                path: segment.path,
                                strokeColor: color,
                                strokeWeight: segment.fallback ? 3 : 6,
                                strokeOpacity: segment.fallback ? 0.6 : 1,
                                zIndex: 2
                            };

                            if (isDashed) {
                                lineConfig.strokeDashArray = '15, 10';
                            }

                            locationService.drawPolyline(lineConfig, function (polyline) {
                                polylines.push(polyline);
                            });
                        });

                        // Store calculated distance and update summary
                        calculatedDistance = totalDistanceMeters / 1000; // Convert to km
                        routingEnabled = true;
                        updateSummaryPanel();

                        // Call completion callback
                        if (onComplete) {
                            onComplete();
                        }
                    }
                });
            })(i);
        }
    }

    /**
     * Render routed paths using OSRM (OpenStreetMap Routing Machine)
     */
    function renderOSRMRoutedPath(onComplete) {
        // Get tasks with valid coordinates that have been checked in (chronologically sorted)
        var validTasks = journeyData.tasks.filter(function (task) {
            return task.latitude && task.longitude && task.mapStatus != "Pending" && task.mapStatus != "Overdue";
        });

        if (validTasks.length < 2) {
            // Not enough checked-in points to create a route
            updateSummaryPanel();
            if (onComplete) onComplete();
            return;
        }

        var routeSegments = [];
        var totalDistanceMeters = 0;
        var routesCompleted = 0;
        var routesToCalculate = validTasks.length - 1;

        // Request OSRM routes for each consecutive pair of tasks
        for (var i = 0; i < validTasks.length - 1; i++) {
            (function (index) {
                var origin = { lat: validTasks[index].latitude, lng: validTasks[index].longitude };
                var destination = { lat: validTasks[index + 1].latitude, lng: validTasks[index + 1].longitude };
                var status = validTasks[index + 1].mapStatus;

                locationService.provider.getOSRMRoute(origin, destination, function (result) {
                    routesCompleted++;

                    if (result && result.success) {
                        routeSegments.push({
                            index: index,
                            path: result.path,
                            distance: result.distance, // in meters
                            status: status
                        });
                        totalDistanceMeters += result.distance;
                    } else {
                        // OSRM routing failed, use straight line as fallback
                        routeSegments.push({
                            index: index,
                            path: [origin, destination],
                            distance: 0,
                            status: status,
                            fallback: true
                        });
                    }

                    // Check if all routes are calculated
                    if (routesCompleted === routesToCalculate) {
                        // Sort segments by index to ensure correct order
                        routeSegments.sort(function (a, b) { return a.index - b.index; });

                        // Draw all route segments with enhanced styling
                        routeSegments.forEach(function (segment, idx) {
                            var color, outlineColor, isDashed;

                            // Determine color and style based on task status
                            if (segment.status === 'Completed') {
                                color = '#007bff'; // Blue
                                outlineColor = '#0056b3';
                                isDashed = false;
                            } else if (segment.status === 'Working') {
                                color = '#28a745'; // Green
                                outlineColor = '#1e7e34';
                                isDashed = false;
                            } else if (segment.status === 'OnTheWay') {
                                color = '#ffc107'; // Yellow
                                outlineColor = '#d39e00';
                                isDashed = true;
                            } else {
                                color = '#6c757d'; // Gray for Pending
                                outlineColor = '#495057';
                                isDashed = true;
                            }

                            // Draw outline (shadow effect) for non-fallback routes
                            if (!segment.fallback) {
                                var outlineConfig = {
                                    path: segment.path,
                                    strokeColor: outlineColor,
                                    strokeWeight: 8,
                                    strokeOpacity: 0.3,
                                    zIndex: 1
                                };
                                locationService.drawPolyline(outlineConfig, function (polyline) {
                                    polylines.push(polyline);
                                });
                            }

                            // Draw main line
                            var lineConfig = {
                                path: segment.path,
                                strokeColor: color,
                                strokeWeight: segment.fallback ? 3 : 6,
                                strokeOpacity: segment.fallback ? 0.6 : 1,
                                zIndex: 2
                            };

                            if (isDashed) {
                                lineConfig.strokeDashArray = '15, 10';
                            }

                            locationService.drawPolyline(lineConfig, function (polyline) {
                                polylines.push(polyline);
                            });
                        });

                        // Store calculated distance and update summary
                        calculatedDistance = totalDistanceMeters / 1000; // Convert to km
                        routingEnabled = true;
                        updateSummaryPanel();

                        // Call completion callback
                        if (onComplete) {
                            onComplete();
                        }
                    }
                });
            })(i);
        }
    }

    /**
     * Render straight-line paths (fallback for OpenStreetMap or when routing fails)
     */
    function renderStraightPath(onComplete) {
        // Build path segments only from checked-in tasks (chronologically sorted)
        var checkedInTasks = journeyData.tasks.filter(function (task) {
            return task.latitude && task.longitude && task.checkInTime;
        });

        // Draw journey path with enhanced styling
        if (checkedInTasks.length > 1) {
            for (var i = 0; i < checkedInTasks.length - 1; i++) {
                var startTask = checkedInTasks[i];
                var endTask = checkedInTasks[i + 1];

                var segment = [
                    { lat: startTask.latitude, lng: startTask.longitude },
                    { lat: endTask.latitude, lng: endTask.longitude }
                ];

                var color, outlineColor;

                // Color based on destination task status
                if (endTask.mapStatus === 'Completed') {
                    color = '#007bff'; // Blue
                    outlineColor = '#0056b3';
                } else if (endTask.mapStatus === 'Working') {
                    color = '#28a745'; // Green
                    outlineColor = '#1e7e34';
                } else if (endTask.mapStatus === 'OnTheWay') {
                    color = '#ffc107'; // Yellow
                    outlineColor = '#d39e00';
                } else {
                    color = '#6c757d'; // Gray
                    outlineColor = '#495057';
                }

                // Draw outline (shadow effect)
                var outlineConfig = {
                    path: segment,
                    strokeColor: outlineColor,
                    strokeWeight: 8,
                    strokeOpacity: 0.3,
                    zIndex: 1
                };
                locationService.drawPolyline(outlineConfig, function (polyline) {
                    polylines.push(polyline);
                });

                // Draw main line
                var lineConfig = {
                    path: segment,
                    strokeColor: color,
                    strokeWeight: 6,
                    strokeOpacity: 1,
                    zIndex: 2
                };

                locationService.drawPolyline(lineConfig, function (polyline) {
                    polylines.push(polyline);
                });
            }
        }

        // Update summary panel (using API distance for straight paths)
        updateSummaryPanel();

        // Call completion callback (straight paths are synchronous)
        if (onComplete) {
            onComplete();
        }
    }

    /**
     * Update summary panel with statistics
     */
    function updateSummaryPanel() {
        if (!journeyData.summary) return;

        var summary = journeyData.summary;

        // Update stats (using API field names)
        $('#statWorking').text(summary.inProgressTasks || 0);
        $('#statCompleted').text(summary.completedTasks || 0);
        $('#statPending').text(summary.pendingTasks || 0);
        $('#statOverdue').text(summary.overdueTasks || 0);

        // Update info (using API field names)
        $('#infoTotalTasks').text(summary.totalTasks || 0);

        // Use calculated distance from Google routing if available, otherwise use API distance
        var displayDistance = routingEnabled ? calculatedDistance : (summary.totalDistanceTraveled || 0);
        $('#infoTotalDistance').text(displayDistance.toFixed(2) + ' km');

        $('#infoTotalTime').text(formatDuration(summary.totalTimeMinutes || 0));
        $('#infoWorkingHours').text(formatDuration(summary.totalWorkDurationMinutes || 0));
        $('#infoTravelTime').text(formatDuration(summary.travelTimeMinutes || 0));
    }

    /**
     * Update task list sidebar
     */
    function updateTaskList() {
        var $content = $('#taskListContent');
        $content.empty();

        if (!journeyData.tasks || journeyData.tasks.length === 0) {
            $content.html('<div class="empty-state" style="padding: 20px;"><p>No tasks found</p></div>');
            return;
        }

        // Track journey sequence number (only for checked-in tasks)
        var journeySequence = 0;

        journeyData.tasks.forEach(function (task, index) {
            var statusClass = task.mapStatus.toLowerCase().replace('_', '');
            var $taskItem = $('<div>', {
                class: 'task-item ' + statusClass,
                'data-task-id': task.taskId
            });

            var timeRange = '';
            if (task.taskDate && task.expectedDurationMinutes) {
                // Show task date with expected duration
                var date = new Date(task.taskDate);
                var dateStr = date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
                timeRange = dateStr + ' (' + formatDuration(task.expectedDurationMinutes) + ')';
            }

            var checkInOut = '';
            if (task.checkInTime) {
                checkInOut += 'In: ' + formatLocalTime(task.checkInTime);
            }
            if (task.checkOutTime) {
                checkInOut += (checkInOut ? ' | ' : '') + 'Out: ' + formatLocalTime(task.checkOutTime);
            }

            // Display client name or location address
            var locationDisplay = task.locationAddress || 'Location not specified';

            // Only number tasks that have been checked in
            var taskNumber = '';
            if (task.checkInTime) {
                journeySequence++;
                taskNumber = journeySequence + '. ';
            }

            $taskItem.html(
                '<div class="task-name">' + taskNumber + task.taskTitle + '</div>' +
                '<div class="task-client"><i class="fa-regular fa-location-dot"></i> ' + locationDisplay + '</div>' +
                '<div class="task-time"><i class="fa-regular fa-clock"></i> ' + timeRange + '</div>' +
                (checkInOut ? '<div class="task-time">' + checkInOut + '</div>' : '') +
                '<span class="task-status-badge ' + statusClass + '">' + task.mapStatus + '</span>'
            );

            // Click handler to focus on task marker
            $taskItem.on('click', function () {
                if (task.latitude && task.longitude) {
                    locationService.centerMap({ lat: task.latitude, lng: task.longitude }, 15);
                    showTaskPopup(task);
                }
            });

            $content.append($taskItem);
        });
    }

    /**
     * Close currently open popup
     */
    function closeCurrentPopup() {
        // Try to close popup using provider's method if available
        if (locationService && locationService.provider && locationService.provider.closePopup) {
            locationService.provider.closePopup();
        }
        currentPopupMarker = null;
        currentPopupInstance = null;
    }

    /**
     * Show task details popup with modern card design
     */
    function showTaskPopup(task) {
        // Close previous popup before opening new one
        closeCurrentPopup();

        var statusColor = STATUS_COLORS[task.mapStatus] || STATUS_COLORS['Pending'];

        // Build modern popup content with card layout
        // Using a wrapper to ensure no scrollbars and proper styling
        var popupContent = '<div style="' +
            'font-family: -apple-system, BlinkMacSystemFont, \'Segoe UI\', Roboto, \'Helvetica Neue\', Arial, sans-serif;' +
            'width: 300px;' +
            'background: #ffffff;' +
            'border-radius: 12px;' +
            'box-shadow: 0 8px 24px rgba(0,0,0,0.15);' +
            'overflow: hidden;' +
            'margin: 0;' +
            'padding: 0;' +
            '">';

        // Header with status color
        popupContent += '<div style="' +
            'background: linear-gradient(135deg, ' + statusColor + ' 0%, ' + statusColor + 'dd 100%);' +
            'color: #ffffff;' +
            'padding: 10px 16px;' +
            'font-size: 14px;' +
            'font-weight: 500;' +
            'text-shadow: 0 1px 2px rgba(0,0,0,0.1);' +
            'white-space: nowrap;' +
            'overflow: hidden;' +
            'text-overflow: ellipsis;' +
            '">' +
            '<i class="fa-regular fa-list-check" style="margin-right: 8px;"></i>' +
            task.taskTitle +
            '</div>';

        // Content area
        popupContent += '<div style="padding: 14px 16px; max-height: 200px; overflow-y: auto; overflow-x: hidden;">';

        // Status badge
        popupContent += '<div style="' +
            'display: inline-block;' +
            'background: ' + statusColor + '20;' +
            'color: ' + statusColor + ';' +
            'padding: 5px 12px;' +
            'border-radius: 20px;' +
            'font-size: 12px;' +
            'font-weight: 600;' +
            'margin-bottom: 12px;' +
            'border: 1px solid ' + statusColor + '40;' +
            '">' +
            '<i class="fa-regular fa-circle" style="font-size: 8px; margin-right: 6px;"></i>' +
            task.mapStatus +
            '</div>';

        // Location info
        if (task.clientName) {
            popupContent += '<div style="margin-bottom: 9px; color: #333; font-size: 13px; line-height: 1.4;">' +
                '<i class="fa-regular fa-building" style="color: ' + statusColor + '; width: 16px; margin-right: 6px;"></i>' +
                '<strong style="font-weight: 600;">Client:</strong> ' + task.clientName +
                '</div>';
        }
        if (task.locationAddress) {
            let text = task.locationAddress;
            let maxLength = 100;
            const shortAddress = text.length > maxLength ? text.substring(0, maxLength) + "..." : text;
            popupContent += '<div style="margin-bottom: 9px; color: #333; font-size: 13px; line-height: 1.4; word-wrap: break-word;">' +
                '<i class="fa-regular fa-location-do" style="color: ' + statusColor + '; width: 16px; margin-right: 6px;"></i>' +
                '<strong style="font-weight: 600;">Address:</strong> ' + shortAddress +
                '</div>';
        }
        if (!task.clientName && !task.locationAddress) {
            popupContent += '<div style="margin-bottom: 9px; color: #666; font-size: 13px; font-style: italic;">' +
                '<i class="fa-regular fa-location-do" style="color: #999; width: 16px; margin-right: 6px;"></i>' +
                'Location not specified' +
                '</div>';
        }

        // Divider
        popupContent += '<div style="border-top: 1px solid #e9ecef; margin: 10px 0;"></div>';

        // Date and duration
        popupContent += '<div style="margin-bottom: 9px; color: #333; font-size: 13px; line-height: 1.4;">' +
            '<i class="fa-regular fa-calendar-days" style="color: ' + statusColor + '; width: 16px; margin-right: 6px;"></i>' +
            '<strong style="font-weight: 600;">Date:</strong> ' + formatLocalDate(task.taskDate) +
            '</div>';

        popupContent += '<div style="margin-bottom: 9px; color: #333; font-size: 13px; line-height: 1.4;">' +
            '<i class="fa-regular fa-hourglass" style="color: ' + statusColor + '; width: 16px; margin-right: 6px;"></i>' +
            '<strong style="font-weight: 600;">Duration:</strong> ' + formatDuration(task.expectedDurationMinutes) +
            '</div>';

        // Check-in/out times
        if (task.checkInTime || task.checkOutTime) {
            popupContent += '<div style="border-top: 1px solid #e9ecef; margin: 10px 0;"></div>';
        }

        if (task.checkInTime) {
            popupContent += '<div style="margin-bottom: 7px; color: #333; font-size: 13px; line-height: 1.4;">' +
                '<i class="fa-regular fa-right-to-bracket" style="color: #28a745; width: 16px; margin-right: 6px;"></i>' +
                '<strong style="font-weight: 600;">Check-In:</strong> ' +
                '<span style="background: #d4edda; color: #155724; padding: 2px 8px; border-radius: 4px; font-size: 12px; white-space: nowrap;">' +
                formatLocalTime(task.checkInTime) +
                '</span>' +
                '</div>';
        }

        if (task.checkOutTime) {
            popupContent += '<div style="margin-bottom: 0; color: #333; font-size: 13px; line-height: 1.4;">' +
                '<i class="fa-regular fa-arrow-right-from-bracket" style="color: #dc3545; width: 16px; margin-right: 6px;"></i>' +
                '<strong style="font-weight: 600;">Check-Out:</strong> ' +
                '<span style="background: #f8d7da; color: #721c24; padding: 2px 8px; border-radius: 4px; font-size: 12px; white-space: nowrap;">' +
                formatLocalTime(task.checkOutTime) +
                '</span>' +
                '</div>';
        }

        popupContent += '</div>'; // Close content area
        popupContent += '</div>'; // Close popup container

        // Find the marker for this task
        var taskMarker = markers.find(function (marker) {
            var markerPos = locationService.getMarkerPosition(marker);
            return markerPos &&
                Math.abs(markerPos.lat - task.latitude) < 0.0001 &&
                Math.abs(markerPos.lng - task.longitude) < 0.0001;
        });

        if (taskMarker) {
            locationService.showPopup(taskMarker, popupContent);
            currentPopupMarker = taskMarker;
        }
    }

    /**
     * Fit map to show all markers
     */
    function fitMapToMarkers() {
        if (markers.length === 0 || !locationService) return;

        var bounds = [];

        markers.forEach(function (marker) {
            var position = locationService.getMarkerPosition(marker);
            if (position) {
                bounds.push(position);
            }
        });

        if (bounds.length > 0) {
            locationService.fitBounds(bounds);
        }
    }

    /**
     * Clear all markers and polylines from map
     */
    function clearMap() {
        closeCurrentPopup();
        // Clear markers
        if (locationService && markers.length > 0) {
            markers.forEach(function (marker) {
                locationService.removeMarker(marker);
            });
            markers = [];
        }

        // Clear polylines
        if (locationService && polylines.length > 0) {
            polylines.forEach(function (polyline) {
                locationService.removePolyline(polyline);
            });
            polylines = [];
        }
    }

    /**
     * Show/hide loading overlay
     */
    function showLoading(show) {
        if (show) {
            $('#loadingOverlay').addClass('active');
        } else {
            $('#loadingOverlay').removeClass('active');
        }
    }

    /**
     * Show empty state with friendly message
     */
    function showEmptyState() {
        clearMap();
        $('#summaryPanel').hide();
        $('#taskListSidebar').hide();

        // Remove any existing empty state first
        $('.right-sidebar').find('.journey-empty-state').remove();

        // Show empty state message in the right sidebar
        var emptyStateHTML = `
        
        <div class="datanotfound journey-empty-state">
            <img src="/assets/images/emptyscreen_3.png" width="280" height="148" alt="No Tasks Found">
            <p class="datanotfound_title">No Tasks Found</p>
            <p>This employee has no tasks scheduled for the selected date.</p>
        </div>`;

        // Append empty state to right sidebar (panels are hidden, not removed)
        $('.right-sidebar').append(emptyStateHTML);

        if (!mapInstance) {
            initializeMap();
        }
    }

    /**
     * Start auto-refresh timer
     */
    function startAutoRefresh() {
        stopAutoRefresh(); // Clear any existing interval

        autoRefreshInterval = setInterval(function () {
            loadEmployeeJourney();
        }, AUTO_REFRESH_INTERVAL);
    }

    /**
     * Stop auto-refresh timer
     */
    function stopAutoRefresh() {
        if (autoRefreshInterval) {
            clearInterval(autoRefreshInterval);
            autoRefreshInterval = null;
        }
    }

    /**
     * Format time from HH:mm:ss to HH:mm AM/PM
     */
    function formatTime(timeString) {
        if (!timeString) return 'N/A';

        try {
            var parts = timeString.split(':');
            var hours = parseInt(parts[0]);
            var minutes = parts[1];
            var ampm = hours >= 12 ? 'PM' : 'AM';
            hours = hours % 12 || 12;
            return hours + ':' + minutes + ' ' + ampm;
        } catch (e) {
            return timeString;
        }
    }

    /**
     * Format date time
     */
    function formatDateTime(dateTimeString) {
        if (!dateTimeString) return 'N/A';

        try {
            var date = new Date(dateTimeString);
            return date.toLocaleString('en-US', {
                month: 'short',
                day: 'numeric',
                hour: '2-digit',
                minute: '2-digit'
            });
        } catch (e) {
            return dateTimeString;
        }
    }

    /**
     * Format date to readable format
     */
    function formatDate(dateString) {
        if (!dateString) return 'N/A';

        try {
            var date = new Date(dateString);
            return date.toLocaleDateString('en-US', {
                year: 'numeric',
                month: 'short',
                day: 'numeric'
            });
        } catch (e) {
            return dateString;
        }
    }

    /**
     * Format duration in minutes to hours and minutes
     */
    function formatDuration(minutes) {
        if (!minutes || minutes === 0) return '0h 0m';

        var hours = Math.floor(minutes / 60);
        var mins = Math.round(minutes % 60);
        return hours + 'h ' + mins + 'm';
    }

    /**
     * Parse server timestamp as UTC (adds 'Z' if missing)
     */
    function parseUTCDate(dateString) {
        if (!dateString) return null;

        // If date string doesn't end with 'Z' and looks like ISO format, add 'Z' to treat as UTC
        if (typeof dateString === 'string' && dateString.includes('T') && !dateString.endsWith('Z')) {
            dateString = dateString + 'Z';
        }

        var date = new Date(dateString);
        return isNaN(date.getTime()) ? null : date;
    }

    /**
     * Format time only in user's local timezone
     */
    function formatLocalTime(utcDate) {
        if (!utcDate) return 'N/A';

        var date = parseUTCDate(utcDate);
        if (!date) return 'N/A';

        // Format: 3:30 PM
        // Automatically uses browser's timezone and locale
        var options = {
            hour: 'numeric',
            minute: '2-digit',
            hour12: true
        };

        return date.toLocaleTimeString(undefined, options);
    }

    /**
     * Format date only in user's local timezone
     */
    function formatLocalDate(utcDate) {
        if (!utcDate) return 'N/A';

        var date = parseUTCDate(utcDate);
        if (!date) return 'N/A';

        // Format: Jan 7, 2025
        // Automatically uses browser's timezone and locale
        var options = {
            month: 'short',
            day: 'numeric',
            year: 'numeric'
        };

        return date.toLocaleDateString(undefined, options);
    }

    // Public API
    return {
        initialize: initialize,
        loadEmployeeJourney: loadEmployeeJourney,
        startAutoRefresh: startAutoRefresh,
        stopAutoRefresh: stopAutoRefresh
    };

})();
