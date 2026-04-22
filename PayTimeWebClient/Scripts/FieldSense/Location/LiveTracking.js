var LiveTrackingManager = (function() {
    'use strict';

    var map = null;
    var markers = {};
    var selectedEmployeeId = null;
    var locationService = null;
    var updateInterval = null;
    var config = {};

    // Mock data for employees with realistic Indian locations
    var mockEmployees = [
        {
            id: 'EMP001',
            name: 'Rajesh Kumar',
            department: 'sales',
            status: 'online',
            position: { lat: 28.6139, lng: 77.2090 }, // New Delhi
            currentTask: 'Client meeting at Connaught Place',
            battery: 85,
            speed: 15,
            distanceTraveled: 32.5,
            tasksCompleted: 4,
            lastUpdate: new Date().toISOString(),
            route: [
                { lat: 28.6139, lng: 77.2090 },
                { lat: 28.6229, lng: 77.2195 },
                { lat: 28.6319, lng: 77.2300 }
            ]
        },
        {
            id: 'EMP002',
            name: 'Priya Sharma',
            department: 'delivery',
            status: 'online',
            position: { lat: 28.5355, lng: 77.3910 }, // Noida
            currentTask: 'Package delivery at Sector 62',
            battery: 67,
            speed: 25,
            distanceTraveled: 45.2,
            tasksCompleted: 7,
            lastUpdate: new Date().toISOString()
        },
        {
            id: 'EMP003',
            name: 'Amit Patel',
            department: 'service',
            status: 'idle',
            position: { lat: 28.4595, lng: 77.0266 }, // Gurgaon
            currentTask: 'Equipment maintenance at DLF Phase 3',
            battery: 92,
            speed: 0,
            distanceTraveled: 18.7,
            tasksCompleted: 3,
            lastUpdate: new Date(Date.now() - 600000).toISOString()
        },
        {
            id: 'EMP004',
            name: 'Sunita Verma',
            department: 'support',
            status: 'online',
            position: { lat: 28.6692, lng: 77.4538 }, // Ghaziabad
            currentTask: 'Customer support visit',
            battery: 45,
            speed: 18,
            distanceTraveled: 28.3,
            tasksCompleted: 5,
            lastUpdate: new Date().toISOString()
        },
        {
            id: 'EMP005',
            name: 'Mohammed Ali',
            department: 'sales',
            status: 'offline',
            position: { lat: 28.7041, lng: 77.1025 }, // Delhi
            currentTask: 'Product demonstration',
            battery: 12,
            speed: 0,
            distanceTraveled: 52.1,
            tasksCompleted: 8,
            lastUpdate: new Date(Date.now() - 3600000).toISOString()
        },
        {
            id: 'EMP006',
            name: 'Anita Singh',
            department: 'delivery',
            status: 'online',
            position: { lat: 28.4089, lng: 77.3178 }, // Faridabad
            currentTask: 'Multiple deliveries in Sector 15',
            battery: 78,
            speed: 30,
            distanceTraveled: 61.4,
            tasksCompleted: 11,
            lastUpdate: new Date().toISOString()
        },
        {
            id: 'EMP007',
            name: 'Vikram Reddy',
            department: 'service',
            status: 'online',
            position: { lat: 28.5245, lng: 77.1855 }, // South Delhi
            currentTask: 'AC repair at Saket',
            battery: 56,
            speed: 12,
            distanceTraveled: 22.8,
            tasksCompleted: 2,
            lastUpdate: new Date().toISOString()
        },
        {
            id: 'EMP008',
            name: 'Neha Gupta',
            department: 'support',
            status: 'idle',
            position: { lat: 28.6328, lng: 77.2197 }, // Central Delhi
            currentTask: 'Waiting for next assignment',
            battery: 88,
            speed: 0,
            distanceTraveled: 15.5,
            tasksCompleted: 6,
            lastUpdate: new Date(Date.now() - 300000).toISOString()
        }
    ];

    function initialize(options) {
        config = options || {};

        // Initialize the map
        initializeMap();

        // Load employees
        loadEmployees();

        // Setup event handlers
        setupEventHandlers();

        // Start auto-refresh
        startAutoRefresh();

        // Update stats
        updateStats();
    }

    function initializeMap() {
        // Initialize map using MapLibre GL JS with OpenStreetMap
        map = new maplibregl.Map({
            container: 'liveTrackingMap',
            style: {
                version: 8,
                sources: {
                    osm: {
                        type: 'raster',
                        tiles: ['https://tile.openstreetmap.org/{z}/{x}/{y}.png'],
                        tileSize: 256,
                        attribution: '© OpenStreetMap contributors'
                    }
                },
                layers: [{
                    id: 'osm',
                    type: 'raster',
                    source: 'osm'
                }]
            },
            center: [77.2090, 28.6139], // New Delhi center
            zoom: 10
        });

        // Add navigation controls
        map.addControl(new maplibregl.NavigationControl());

        // Add fullscreen control
        map.addControl(new maplibregl.FullscreenControl());
    }

    function loadEmployees() {
        var employeeList = $('#employeeList');
        employeeList.empty();

        // Apply filters
        var departmentFilter = $('#departmentFilter').val();
        var statusFilter = $('#statusFilter').val();
        var searchTerm = $('#employeeSearch').val().toLowerCase();

        var filteredEmployees = mockEmployees.filter(function(emp) {
            var matchDept = !departmentFilter || emp.department === departmentFilter;
            var matchStatus = !statusFilter || emp.status === statusFilter;
            var matchSearch = !searchTerm ||
                              emp.name.toLowerCase().includes(searchTerm) ||
                              emp.id.toLowerCase().includes(searchTerm);
            return matchDept && matchStatus && matchSearch;
        });

        // Render employee list
        filteredEmployees.forEach(function(employee) {
            var statusClass = 'status-' + employee.status;
            var employeeHtml = `
                <div class="employee-item" data-employee-id="${employee.id}">
                    <div>
                        <strong>${employee.name}</strong> (${employee.id})
                    </div>
                    <div class="employee-status">
                        <span style="color: #6c757d; font-size: 12px;">${employee.currentTask}</span>
                        <span class="status-badge ${statusClass}">${employee.status.toUpperCase()}</span>
                    </div>
                </div>
            `;
            employeeList.append(employeeHtml);

            // Add marker to map
            addOrUpdateMarker(employee);
        });

        // Update counts
        updateStats();
    }

    function addOrUpdateMarker(employee) {
        // Remove existing marker if any
        if (markers[employee.id]) {
            markers[employee.id].remove();
        }

        // Create custom marker element
        var markerEl = document.createElement('div');
        markerEl.className = 'custom-marker';
        markerEl.style.width = '40px';
        markerEl.style.height = '40px';

        // Set marker color based on status
        var color = employee.status === 'online' ? '#28a745' :
                   employee.status === 'offline' ? '#dc3545' : '#ffc107';

        markerEl.innerHTML = `
            <div style="
                width: 40px;
                height: 40px;
                background: ${color};
                border: 3px solid white;
                border-radius: 50%;
                box-shadow: 0 2px 4px rgba(0,0,0,0.3);
                display: flex;
                align-items: center;
                justify-content: center;
                position: relative;
            ">
                <span style="color: white; font-weight: bold; font-size: 12px;">
                    ${employee.name.split(' ').map(n => n[0]).join('')}
                </span>
                ${employee.status === 'online' ? '<div style="position: absolute; top: -2px; right: -2px; width: 10px; height: 10px; background: #28a745; border: 2px solid white; border-radius: 50%; animation: pulse 2s infinite;"></div>' : ''}
            </div>
        `;

        // Create marker
        var marker = new maplibregl.Marker({
            element: markerEl,
            anchor: 'center'
        })
        .setLngLat([employee.position.lng, employee.position.lat])
        .addTo(map);

        // Add popup
        var popup = new maplibregl.Popup({ offset: 25 })
            .setHTML(`
                <div style="padding: 10px;">
                    <h6 style="margin: 0 0 5px 0;">${employee.name}</h6>
                    <p style="margin: 0; font-size: 12px; color: #6c757d;">
                        ${employee.currentTask}<br>
                        Battery: ${employee.battery}% | Speed: ${employee.speed} km/h
                    </p>
                </div>
            `);

        marker.setPopup(popup);

        // Store marker reference
        markers[employee.id] = marker;

        // Add click handler to marker element
        markerEl.addEventListener('click', function() {
            selectEmployee(employee.id);
        });
    }

    function selectEmployee(employeeId) {
        selectedEmployeeId = employeeId;
        var employee = mockEmployees.find(e => e.id === employeeId);

        if (!employee) return;

        // Update active state in list
        $('.employee-item').removeClass('active');
        $(`.employee-item[data-employee-id="${employeeId}"]`).addClass('active');

        // Update details panel
        $('#employeeDetailsPanel').show();
        $('#selectedEmployeeName').text(employee.name);
        $('#selectedEmployeeId').text(employee.id);
        $('#selectedDepartment').text(employee.department);
        $('#selectedTask').text(employee.currentTask);
        $('#selectedLocation').text(`${employee.position.lat.toFixed(4)}, ${employee.position.lng.toFixed(4)}`);
        $('#selectedBattery').html(`${employee.battery}% ${getBatteryIcon(employee.battery)}`);
        $('#selectedSpeed').text(employee.speed + ' km/h');
        $('#distanceTraveled').text(employee.distanceTraveled.toFixed(1));
        $('#tasksCompleted').text(employee.tasksCompleted);
        $('#lastUpdateTime').text(formatTime(employee.lastUpdate));

        // Center map on employee
        map.flyTo({
            center: [employee.position.lng, employee.position.lat],
            zoom: 14,
            essential: true
        });

        // Draw route if available
        if (employee.route && employee.route.length > 1) {
            drawRoute(employee.route);
        }
    }

    function drawRoute(route) {
        // Remove existing route if any
        if (map.getSource('route')) {
            map.removeLayer('route');
            map.removeSource('route');
        }

        // Convert route to GeoJSON
        var routeGeoJSON = {
            type: 'Feature',
            geometry: {
                type: 'LineString',
                coordinates: route.map(point => [point.lng, point.lat])
            }
        };

        // Add route to map
        map.addSource('route', {
            type: 'geojson',
            data: routeGeoJSON
        });

        map.addLayer({
            id: 'route',
            type: 'line',
            source: 'route',
            layout: {
                'line-join': 'round',
                'line-cap': 'round'
            },
            paint: {
                'line-color': '#007bff',
                'line-width': 4,
                'line-opacity': 0.7
            }
        });
    }

    function setupEventHandlers() {
        // Employee list click
        $(document).on('click', '.employee-item', function() {
            var employeeId = $(this).data('employee-id');
            selectEmployee(employeeId);
        });

        // Search
        $('#employeeSearch').on('input', function() {
            loadEmployees();
        });

        // Filters
        $('#departmentFilter, #statusFilter').on('change', function() {
            loadEmployees();
        });

        // Date filter
        $('#dateFilter').on('change', function() {
            // In real implementation, this would filter by date
            loadEmployees();
        });
    }

    function startAutoRefresh() {
        // Auto refresh every 30 seconds
        updateInterval = setInterval(function() {
            simulateMovement();
            loadEmployees();

            if (selectedEmployeeId) {
                selectEmployee(selectedEmployeeId);
            }
        }, 30000);
    }

    function simulateMovement() {
        // Simulate employee movement for demo
        mockEmployees.forEach(function(employee) {
            if (employee.status === 'online') {
                // Small random movement
                employee.position.lat += (Math.random() - 0.5) * 0.002;
                employee.position.lng += (Math.random() - 0.5) * 0.002;
                employee.speed = Math.floor(Math.random() * 40);
                employee.battery = Math.max(10, employee.battery - Math.floor(Math.random() * 3));
                employee.distanceTraveled += Math.random() * 2;
                employee.lastUpdate = new Date().toISOString();

                // Randomly complete tasks
                if (Math.random() > 0.7) {
                    employee.tasksCompleted++;
                }
            }
        });
    }

    function refreshTracking() {
        // Manual refresh
        simulateMovement();
        loadEmployees();

        if (selectedEmployeeId) {
            selectEmployee(selectedEmployeeId);
        }

        // Show refresh animation
        $('.refresh-btn i').addClass('fa-spin');
        setTimeout(function() {
            $('.refresh-btn i').removeClass('fa-spin');
        }, 1000);
    }

    function updateStats() {
        var total = mockEmployees.length;
        var online = mockEmployees.filter(e => e.status === 'online').length;

        $('#totalCount').text(total);
        $('#onlineCount').text(online);
    }

    function getBatteryIcon(battery) {
        if (battery > 75) return '<i class="fas fa-battery-full" style="color: #28a745;"></i>';
        if (battery > 50) return '<i class="fas fa-battery-three-quarters" style="color: #28a745;"></i>';
        if (battery > 25) return '<i class="fas fa-battery-half" style="color: #ffc107;"></i>';
        if (battery > 10) return '<i class="fas fa-battery-quarter" style="color: #ffc107;"></i>';
        return '<i class="fas fa-battery-empty" style="color: #dc3545;"></i>';
    }

    function formatTime(isoString) {
        var date = new Date(isoString);
        var now = new Date();
        var diff = Math.floor((now - date) / 1000); // seconds

        if (diff < 60) return 'Just now';
        if (diff < 3600) return Math.floor(diff / 60) + ' min ago';
        if (diff < 86400) return Math.floor(diff / 3600) + ' hours ago';
        return date.toLocaleString();
    }

    function destroy() {
        if (updateInterval) {
            clearInterval(updateInterval);
        }

        if (map) {
            map.remove();
        }

        markers = {};
    }

    // Public API
    return {
        initialize: initialize,
        refreshTracking: refreshTracking,
        selectEmployee: selectEmployee,
        destroy: destroy
    };
})();

// Add pulse animation CSS
$(document).ready(function() {
    var style = document.createElement('style');
    style.innerHTML = `
        @keyframes pulse {
            0% {
                box-shadow: 0 0 0 0 rgba(40, 167, 69, 0.7);
            }
            70% {
                box-shadow: 0 0 0 10px rgba(40, 167, 69, 0);
            }
            100% {
                box-shadow: 0 0 0 0 rgba(40, 167, 69, 0);
            }
        }
    `;
    document.head.appendChild(style);
});