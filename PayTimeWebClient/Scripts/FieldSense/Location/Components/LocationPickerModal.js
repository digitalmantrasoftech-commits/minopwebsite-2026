/**
 * Location Picker Modal Component
 * Provider-agnostic modal for selecting locations on a map
 */

var LocationPickerModal = (function() {
    'use strict';
    
    var modal = {
        isOpen: false,
        map: null,
        marker: null,
        geofence: null,
        callback: null,
        options: {},
        selectedLocation: null,
        searchTimeout: null,
        
        /**
         * Show the location picker modal
         */
        show: function(options, callback) {
            this.options = $.extend({
                title: 'Pick Location',
                currentLocation: null,
                radius: 100,
                showRadius: true,
                showSearch: true,
                showCurrentLocation: true,
                confirmText: 'Select Location',
                cancelText: 'Cancel'
            }, options);
            
            this.callback = callback;
            this.selectedLocation = this.options.currentLocation || {
                lat: FieldSenseLocationService.config.DefaultCenter.lat,
                lng: FieldSenseLocationService.config.DefaultCenter.lng
            };
            
            // Create modal if not exists
            if (!$('#locationPickerModal').length) {
                this.createModal();
            }
            
            // Show modal
            $('#locationPickerModal').modal('show');
            this.isOpen = true;
            
            // Initialize map after modal is shown
            var self = this;
            $('#locationPickerModal').on('shown.bs.modal', function() {
                self.initializeMap();
            });
        },
        
        /**
         * Create modal HTML structure
         */
        createModal: function() {
            var modalHtml = `
                <div class="modal fade" id="locationPickerModal" tabindex="-1" role="dialog">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                                <h4 class="modal-title" id="locationPickerTitle">${this.options.title}</h4>
                            </div>
                            <div class="modal-body">
                                <!-- Search Bar -->
                                <div class="location-search-container" style="display: ${this.options.showSearch ? 'block' : 'none'};">
                                    <div class="input-group">                                        
                                        <input type="text" id="locationSearchInput" class="form-control" 
                                               placeholder="Search for a location..." />
                                        ${this.options.showCurrentLocation ? `
                                        <span class="input-group-btn">
                                            <button class="btn btn-default" type="button" id="btnCurrentLocation" 
                                                    title="Use Current Location">
                                                <i class="fa fa-crosshairs"></i> Current Location
                                            </button>
                                        </span>` : ''}
                                    </div>
                                    <div id="searchResultsList" class="search-results-list" style="display:none;"></div>
                                </div>
                                
                                <!-- Map Container -->
                                <div id="locationMapContainer" style="height: 400px; margin-top: 10px; position: relative;">
                                    <div class="map-loading" style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); z-index: 1000;">
                                        <i class="fa fa-spinner fa-spin fa-3x"></i>
                                    </div>
                                </div>
                                
                                <!-- Location Info -->
                                <div class="location-info" style="margin-top: 10px;">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label>Latitude:</label>
                                                <input type="text" id="modalLatitude" class="form-control input-sm" readonly />
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label>Longitude:</label>
                                                <input type="text" id="modalLongitude" class="form-control input-sm" readonly />
                                            </div>
                                        </div>
                                    </div>
                                    ${this.options.showRadius ? `
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="form-group">
                                                <label>Radius (meters):</label>
                                                <select id="modalRadius" class="form-control input-sm">
                                                    <option value="50">50m</option>
                                                    <option value="100" selected>100m</option>
                                                    <option value="200">200m</option>
                                                    <option value="500">500m</option>
                                                    <option value="1000">1km</option>
                                                </select>
                                            </div>
                                        </div>
                                    </div>` : ''}
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="form-group">
                                                <label>Address:</label>
                                                <input type="text" id="modalAddress" class="form-control input-sm" readonly />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn clear_btn" data-dismiss="modal">
                                    <i class="fa-regular fa-xmark"></i> ${this.options.cancelText}
                                </button>
                                <button type="button" class="btn save_btn" id="btnConfirmLocation">
                                    <i class="fa fa-check"></i> ${this.options.confirmText}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            `;
            
            $('body').append(modalHtml);
            this.bindEvents();
        },
        
        /**
         * Bind modal events
         */
        bindEvents: function() {
            var self = this;
            
            // Search input
            $('#locationSearchInput').on('input', function() {
                clearTimeout(self.searchTimeout);
                var query = $(this).val();
                
                if (query.length > 2) {
                    self.searchTimeout = setTimeout(function() {
                        self.searchLocation(query);
                    }, 500);
                } else {
                    $('#searchResultsList').hide().empty();
                }
            });
            
            // Current location button
            $('#btnCurrentLocation').on('click', function() {
                self.useCurrentLocation();
            });
            
            // Radius change
            $('#modalRadius').on('change', function() {
                var radius = parseInt($(this).val());
                self.updateGeofence(radius);
            });
            
            // Confirm button
            $('#btnConfirmLocation').on('click', function() {
                self.confirmLocation();
            });
            
            // Modal hidden event
            $('#locationPickerModal').on('hidden.bs.modal', function() {
                self.cleanup();
            });
        },
        
        /**
         * Initialize the map
         */
        initializeMap: function() {
            var self = this;

            // Initialize map using the location service with correct API signature
            var mapConfig = {
                containerId: 'locationMapContainer',
                center: this.selectedLocation,
                zoom: 15,
                controls: true
            };

            FieldSenseLocationService.initializeMap(mapConfig, function(map) {
                // Success callback
                self.map = map;
                console.log('LocationPickerModal: Map initialized successfully');

                // Hide loading spinner
                $('.map-loading').hide();

                // Wait for map to load based on provider type
                if (typeof maplibregl !== 'undefined' && self.map instanceof maplibregl.Map) {
                    self.map.on('load', function() {
                        self.setupMapInteractions();
                        self.addMarker(self.selectedLocation.lat, self.selectedLocation.lng);
                        if (self.options.showRadius) {
                            self.updateGeofence(self.options.radius);
                        }
                    });
                } else if (typeof google !== 'undefined' && self.map instanceof google.maps.Map) {
                    // Google Maps: Wait for 'idle' event (map fully loaded)
                    google.maps.event.addListenerOnce(self.map, 'idle', function() {
                        self.setupMapInteractions();
                        self.addMarker(self.selectedLocation.lat, self.selectedLocation.lng);
                        if (self.options.showRadius) {
                            self.updateGeofence(self.options.radius);
                        }
                    });
                } else {
                    // For other map providers
                    setTimeout(function() {
                        self.setupMapInteractions();
                        self.addMarker(self.selectedLocation.lat, self.selectedLocation.lng);
                        if (self.options.showRadius) {
                            self.updateGeofence(self.options.radius);
                        }
                    }, 500);
                }
            }, function(error) {
                // Error callback
                console.error('LocationPickerModal: Failed to initialize map:', error);
                $('.map-loading').html('<p style="color: #dc3545; padding: 20px; text-align: center;"><i class="fa fa-exclamation-triangle"></i><br>Failed to load map<br><small>' + error + '</small></p>');
            });
        },
        
        /**
         * Setup map click interactions
         */
        setupMapInteractions: function() {
            var self = this;

            if (!this.map) return;

            // Handle map clicks
            if (typeof maplibregl !== 'undefined' && this.map instanceof maplibregl.Map) {
                this.map.on('click', function(e) {
                    var lat = e.lngLat.lat;
                    var lng = e.lngLat.lng;
                    self.updateLocation(lat, lng);
                });
            } else if (typeof google !== 'undefined' && this.map instanceof google.maps.Map) {
                // Google Maps click handler
                google.maps.event.addListener(this.map, 'click', function(e) {
                    var lat = e.latLng.lat();
                    var lng = e.latLng.lng();
                    self.updateLocation(lat, lng);
                });
            }
        },
        
        /**
         * Add or update marker
         */
        addMarker: function(lat, lng) {
            var self = this;
            
            console.log('Adding marker at:', lat, lng);
            
            // Remove existing marker
            if (this.marker) {
                try {
                    if (this.marker.remove) {
                        this.marker.remove();
                    } else if (this.marker.setMap) {
                        this.marker.setMap(null);
                    }
                } catch (error) {
                    console.warn('Error removing existing marker:', error);
                }
                this.marker = null;
            }
            
            // Add new marker using the appropriate provider
            try {
                if (typeof maplibregl !== 'undefined' && this.map instanceof maplibregl.Map) {
                    // Create MapLibre GL JS marker
                    var el = document.createElement('div');
                    el.className = 'maplibre-marker';
                    el.style.backgroundImage = 'url(data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPHBhdGggZD0iTTEyIDJDOC4xMyAyIDUgNS4xMyA1IDljMCA1LjI1IDcgMTMgNyAxM3M3LTcuNzUgNy0xM2MwLTMuODctMy4xMy03LTctN3ptMCA5LjVjLTEuMzggMC0yLjUtMS4xMi0yLjUtMi41czEuMTItMi41IDIuNS0yLjUgMi41IDEuMTIgMi41IDIuNS0xLjEyIDIuNS0yLjUgMi41eiIgZmlsbD0iI2YzM2QzZCIvPgo8L3N2Zz4=)';
                    el.style.width = '32px';
                    el.style.height = '32px';
                    el.style.backgroundSize = 'cover';
                    el.style.cursor = 'pointer';
                    
                    this.marker = new maplibregl.Marker({
                        element: el,
                        draggable: true
                    })
                    .setLngLat([lng, lat])
                    .addTo(this.map);
                    
                    // Handle marker drag
                    this.marker.on('dragend', function() {
                        var lngLat = self.marker.getLngLat();
                        self.updateLocation(lngLat.lat, lngLat.lng);
                    });
                    
                } else if (typeof google !== 'undefined' && this.map instanceof google.maps.Map) {
                    // Create Google Maps marker
                    this.marker = new google.maps.Marker({
                        position: {lat: lat, lng: lng},
                        map: this.map,
                        draggable: true,
                        animation: google.maps.Animation.DROP
                    });

                    // Handle marker drag
                    google.maps.event.addListener(this.marker, 'dragend', function(e) {
                        self.updateLocation(e.latLng.lat(), e.latLng.lng());
                    });

                } else if (FieldSenseLocationService.provider && FieldSenseLocationService.provider.addMarker) {
                    // Use provider's marker method
                    this.marker = FieldSenseLocationService.provider.addMarker(lat, lng, {
                        draggable: true
                    });

                    // Handle marker drag
                    if (this.marker && this.marker.on) {
                        this.marker.on('dragend', function() {
                            var lngLat = self.marker.getLngLat();
                            self.updateLocation(lngLat.lat, lngLat.lng);
                        });
                    }
                }
                
                console.log('Marker created successfully:', this.marker);
                
            } catch (error) {
                console.error('Error creating marker:', error);
            }
            
            // Update coordinate inputs
            $('#modalLatitude').val(lat.toFixed(6));
            $('#modalLongitude').val(lng.toFixed(6));
            
            // Reverse geocode to get address
            this.reverseGeocodeLocation(lat, lng);
        },
        
        /**
         * Update location
         */
        updateLocation: function(lat, lng) {
            console.log('Updating location to:', lat, lng);
            this.selectedLocation = {lat: lat, lng: lng};
            
            // Force marker update
            this.addMarker(lat, lng);
            
            // Update geofence if radius is enabled
            if (this.options.showRadius) {
                var currentRadius = parseInt($('#modalRadius').val()) || 100;
                this.updateGeofence(currentRadius);
            }
            
            // Trigger any location change callbacks
            if (this.onLocationChanged) {
                this.onLocationChanged(this.selectedLocation);
            }
        },
        
        /**
         * Update geofence radius
         */
        updateGeofence: function(radius) {
            if (!this.selectedLocation) {
                console.warn('No selected location for geofence update');
                return;
            }
            
            console.log('Updating geofence at location:', this.selectedLocation, 'with radius:', radius);
            
            // Remove existing geofence with comprehensive cleanup
            this.removeCurrentGeofence();
            
            // Create new geofence
            try {
                this.geofence = FieldSenseLocationService.createGeofence(
                    this.selectedLocation,
                    parseInt(radius) || 100,
                    {
                        fillColor: FieldSenseLocationService.config.GeofenceStyle ? FieldSenseLocationService.config.GeofenceStyle.fillColor : '#3388ff',
                        fillOpacity: FieldSenseLocationService.config.GeofenceStyle ? FieldSenseLocationService.config.GeofenceStyle.fillOpacity : 0.3,
                        strokeColor: FieldSenseLocationService.config.GeofenceStyle ? FieldSenseLocationService.config.GeofenceStyle.strokeColor : '#3388ff'
                    }
                );
                
                console.log('Geofence updated successfully:', this.geofence);
                
            } catch (error) {
                console.error('Error creating geofence:', error);
                this.geofence = null;
            }
        },

        /**
         * Remove current geofence with comprehensive cleanup
         */
        removeCurrentGeofence: function() {
            if (!this.geofence) {
                return;
            }
            
            try {
                console.log('Removing current geofence:', this.geofence);
                
                // Method 1: Use the geofence's own remove method
                if (this.geofence.remove && typeof this.geofence.remove === 'function') {
                    this.geofence.remove();
                }
                
                // Method 2: Try Google Maps style removal
                else if (this.geofence.setMap && typeof this.geofence.setMap === 'function') {
                    this.geofence.setMap(null);
                }
                
                // Method 3: Manual cleanup for MapLibre GL JS
                else if (this.map && typeof maplibregl !== 'undefined' && this.map instanceof maplibregl.Map) {
                    this.removeMapLibreGeofence();
                }
                
                // Method 4: Fallback for custom geofence objects
                else if (this.geofence.layerId || this.geofence.sourceId) {
                    this.removeCustomGeofence();
                }
                
            } catch (error) {
                console.warn('Error removing geofence:', error);
                // Force cleanup even if removal failed
                this.forceGeofenceCleanup();
            }
            
            // Clear the reference
            this.geofence = null;
        },

        /**
         * Remove MapLibre GL JS geofence layers and sources
         */
        removeMapLibreGeofence: function() {
            if (!this.map || !this.geofence) return;
            
            try {
                // Check if geofence has layer information
                if (this.geofence.layerId) {
                    var layerId = this.geofence.layerId;
                    var sourceId = this.geofence.sourceId || layerId.replace('-fill', '').replace('-outline', '');
                    
                    // Remove fill layer
                    if (this.map.getLayer(layerId + '-fill')) {
                        this.map.removeLayer(layerId + '-fill');
                    }
                    
                    // Remove outline layer  
                    if (this.map.getLayer(layerId + '-outline')) {
                        this.map.removeLayer(layerId + '-outline');
                    }
                    
                    // Remove source
                    if (this.map.getSource(sourceId)) {
                        this.map.removeSource(sourceId);
                    }
                } else {
                    // Fallback: try to find and remove any geofence layers
                    this.findAndRemoveGeofenceLayers();
                }
                
            } catch (error) {
                console.warn('Error in MapLibre geofence removal:', error);
            }
        },

        /**
         * Find and remove any geofence-related layers from the map
         */
        findAndRemoveGeofenceLayers: function() {
            if (!this.map) return;
            
            try {
                var style = this.map.getStyle();
                if (style && style.layers) {
                    var layersToRemove = [];
                    var sourcesToRemove = [];
                    
                    // Find geofence layers
                    style.layers.forEach(function(layer) {
                        if (layer.id && (layer.id.includes('geofence') || layer.id.includes('radius'))) {
                            layersToRemove.push(layer.id);
                            if (layer.source && sourcesToRemove.indexOf(layer.source) === -1) {
                                sourcesToRemove.push(layer.source);
                            }
                        }
                    });
                    
                    // Remove layers
                    layersToRemove.forEach(function(layerId) {
                        if (this.map.getLayer(layerId)) {
                            this.map.removeLayer(layerId);
                        }
                    }.bind(this));
                    
                    // Remove sources
                    sourcesToRemove.forEach(function(sourceId) {
                        if (this.map.getSource(sourceId)) {
                            this.map.removeSource(sourceId);
                        }
                    }.bind(this));
                    
                    console.log('Removed geofence layers:', layersToRemove, 'and sources:', sourcesToRemove);
                }
                
            } catch (error) {
                console.warn('Error finding and removing geofence layers:', error);
            }
        },

        /**
         * Remove custom geofence objects
         */
        removeCustomGeofence: function() {
            if (!this.geofence || !this.map) return;
            
            try {
                // Remove by layer ID
                if (this.geofence.layerId) {
                    var layerId = this.geofence.layerId;
                    if (this.map.getLayer && this.map.getLayer(layerId)) {
                        if (this.map.removeLayer) this.map.removeLayer(layerId);
                    }
                }
                
                // Remove by source ID
                if (this.geofence.sourceId) {
                    var sourceId = this.geofence.sourceId;
                    if (this.map.getSource && this.map.getSource(sourceId)) {
                        if (this.map.removeSource) this.map.removeSource(sourceId);
                    }
                }
                
            } catch (error) {
                console.warn('Error removing custom geofence:', error);
            }
        },

        /**
         * Force cleanup when normal removal methods fail
         */
        forceGeofenceCleanup: function() {
            if (!this.map) return;
            
            try {
                console.log('Performing force cleanup of geofence elements');
                
                // Try to remove any DOM elements that might be geofence-related
                var mapContainer = this.map.getContainer ? this.map.getContainer() : document.getElementById('locationMapContainer');
                if (mapContainer) {
                    var geofenceElements = mapContainer.querySelectorAll('[class*="geofence"], [id*="geofence"], [class*="radius"], [id*="radius"]');
                    geofenceElements.forEach(function(element) {
                        try {
                            element.remove();
                        } catch (e) {
                            console.warn('Could not remove geofence element:', e);
                        }
                    });
                }
                
            } catch (error) {
                console.warn('Error in force geofence cleanup:', error);
            }
        },
        
        /**
         * Search for a location
         */
        searchLocation: function(query) {
            var self = this;
            
            // Show loading indicator
            $('#searchResultsList').html('<div class="search-result-item"><i class="fa fa-spinner fa-spin"></i> Searching...</div>').show();
            
            FieldSenseLocationService.geocodeAddress(query, function(result) {
                if (result && result.error) {
                    // Handle error response
                    $('#searchResultsList').html(`
                        <div class="search-result-item search-error">
                            <i class="fa fa-exclamation-triangle"></i> ${result.error}
                            ${result.address ? `<br><small>Try entering coordinates manually or check your internet connection.</small>` : ''}
                        </div>
                    `).show();
                    
                    setTimeout(function() {
                        $('#searchResultsList').hide();
                    }, 5000);
                    
                } else if (result && result.lat && result.lng) {
                    // Show successful result
                    var resultsHtml = `
                        <div class="search-result-item" data-lat="${result.lat}" data-lng="${result.lng}">
                            <i class="fa fa-map-marker"></i> ${result.formatted_address}
                            <small class="text-muted"><br>Click to select this location</small>
                        </div>
                    `;
                    $('#searchResultsList').html(resultsHtml).show();
                    
                    // Bind click event
                    $('.search-result-item').on('click', function() {
                        var lat = parseFloat($(this).data('lat'));
                        var lng = parseFloat($(this).data('lng'));
                        
                        // Update location first
                        self.updateLocation(lat, lng);
                        
                        // Center map on location with improved MapLibre GL JS compatibility
                        if (self.map) {
                            try {
                                if (typeof maplibregl !== 'undefined' && self.map instanceof maplibregl.Map) {
                                    // MapLibre GL JS methods
                                    self.map.flyTo({
                                        center: [lng, lat],
                                        zoom: 15,
                                        duration: 1000
                                    });
                                } else if (typeof google !== 'undefined' && self.map instanceof google.maps.Map) {
                                    // Google Maps
                                    self.map.setCenter({lat: lat, lng: lng});
                                    self.map.setZoom(15);
                                } else if (self.map.setCenter && self.map.setZoom) {
                                    // Fallback for other map types (array format)
                                    self.map.setCenter([lng, lat]);
                                    self.map.setZoom(15);
                                } else if (self.map.panTo) {
                                    // Alternative method
                                    self.map.panTo([lng, lat]);
                                }
                            } catch (error) {
                                console.warn('Error centering map:', error);
                                // Manual fallback - trigger map update
                                setTimeout(function() {
                                    if (self.map.getCenter) {
                                        console.log('Map centered at:', self.map.getCenter());
                                    }
                                }, 100);
                            }
                        }
                        
                        $('#searchResultsList').hide();
                        $('#locationSearchInput').val('');
                    });
                    
                } else {
                    // No results found
                    $('#searchResultsList').html(`
                        <div class="search-result-item search-no-results">
                            <i class="fa fa-search"></i> No results found for "${query}"
                            <br><small class="text-muted">Try a different search term or click on the map to select a location.</small>
                        </div>
                    `).show();
                    
                    setTimeout(function() {
                        $('#searchResultsList').hide();
                    }, 4000);
                }
            });
        },
        
        /**
         * Use current location
         */
        useCurrentLocation: function() {
            var self = this;
            
            $('#btnCurrentLocation').html('<i class="fa fa-spinner fa-spin"></i> Getting location...');
            
            FieldSenseLocationService.getCurrentLocation(function(location) {
                $('#btnCurrentLocation').html('<i class="fa fa-crosshairs"></i> Current Location');
                
                if (location.error) {
                    if (typeof toastr !== 'undefined') {
                        toastr.error(location.error);
                    } else {
                        alert('Error: ' + location.error);
                    }
                } else {
                    self.updateLocation(location.lat, location.lng);
                    
                    // Center map on current location with improved MapLibre GL JS compatibility
                    if (self.map) {
                        try {
                            if (typeof maplibregl !== 'undefined' && self.map instanceof maplibregl.Map) {
                                // MapLibre GL JS methods
                                self.map.flyTo({
                                    center: [location.lng, location.lat],
                                    zoom: 15,
                                    duration: 1000
                                });
                            } else if (typeof google !== 'undefined' && self.map instanceof google.maps.Map) {
                                // Google Maps
                                self.map.setCenter({lat: location.lat, lng: location.lng});
                                self.map.setZoom(15);
                            } else if (self.map.setCenter && self.map.setZoom) {
                                // Fallback for other map types (array format)
                                self.map.setCenter([location.lng, location.lat]);
                                self.map.setZoom(15);
                            }
                        } catch (error) {
                            console.warn('Error centering map on current location:', error);
                        }
                    }
                }
            });
        },
        
        /**
         * Reverse geocode to get address
         */
        reverseGeocodeLocation: function(lat, lng) {
            // Show loading indicator in address field
            $('#modalAddress').val('Getting address...');
            
            FieldSenseLocationService.reverseGeocode(lat, lng, function(result) {
                if (result && result.error) {
                    // Handle error response
                    $('#modalAddress').val('Address not available').attr('placeholder', 'Unable to get address for this location');
                    console.warn('Reverse geocoding failed:', result.error);
                } else if (result && result.formatted_address) {
                    // Show successful result
                    $('#modalAddress').val(result.formatted_address);
                } else {
                    // No address found
                    $('#modalAddress').val('').attr('placeholder', 'Address not available');
                }
            });
        },
        
        /**
         * Confirm location selection
         */
        confirmLocation: function() {
            if (this.callback) {
                var result = {
                    lat: this.selectedLocation.lat,
                    lng: this.selectedLocation.lng,
                    address: $('#modalAddress').val(),
                    radius: this.options.showRadius ? parseInt($('#modalRadius').val()) : null
                };
                this.callback(result);
            }
            
            $('#locationPickerModal').modal('hide');
        },
        
        /**
         * Cleanup when modal is closed
         */
        cleanup: function() {
            this.isOpen = false;
            
            // Remove geofence using comprehensive cleanup
            this.removeCurrentGeofence();
            
            // Remove marker
            if (this.marker) {
                try {
                    if (this.marker.remove) {
                        this.marker.remove();
                    } else if (this.marker.setMap) {
                        this.marker.setMap(null);
                    }
                } catch (error) {
                    console.warn('Error removing marker:', error);
                }
                this.marker = null;
            }
            
            // Destroy map
            if (this.map) {
                try {
                    if (this.map.remove) {
                        this.map.remove();
                    }
                } catch (error) {
                    console.warn('Error removing map:', error);
                }
                this.map = null;
            }
            
            // Clear inputs
            $('#locationSearchInput').val('');
            $('#searchResultsList').hide().empty();
        }
    };
    
    return modal;
})();