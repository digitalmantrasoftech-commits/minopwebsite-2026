/**
 * FieldSense Location Service - Provider-Agnostic Abstraction Layer
 * This service provides a unified interface for location operations,
 * allowing seamless switching between different map providers (OpenSource/Google)
 */

var FieldSenseLocationService = (function() {
    'use strict';
    
    var service = {
        provider: null,
        config: {},
        isInitialized: false,
        cache: {},
        
        /**
         * Initialize the location service with configuration
         */
        init: function(callback) {
            var self = this;
            
            // Load configuration from server
            this.loadConfiguration(function(config) {
                self.config = config;
                
                // Create provider instance based on configuration
                var providerName = config.LocationProvider || 'opensource';
                
                // Load provider dynamically
                self.loadProvider(providerName, function(provider) {
                    self.provider = provider;
                    self.provider.initialize(self.config, function() {
                        self.isInitialized = true;
                        if (callback) callback();
                    });
                });
            });
        },
        
        /**
         * Load configuration from server
         */
        loadConfiguration: function(callback) {
            // In production, this would be loaded from server
            // For now, using default configuration
            var config = {
                LocationProvider: window.LocationProvider || 'opensource',
                DefaultCenter: {
                    lat: parseFloat(window.LocationDefaultLat || '28.6139'),
                    lng: parseFloat(window.LocationDefaultLng || '77.2090')
                },
                DefaultZoom: parseInt(window.LocationDefaultZoom || '10'),
                EnableCaching: window.LocationEnableCaching !== 'false',
                CacheDuration: parseInt(window.LocationCacheDuration || '3600'),
                EnableGeolocation: window.LocationEnableGeolocation !== 'false',
                GeofenceStyle: {
                    strokeColor: window.LocationGeofenceStrokeColor || '#3388ff',
                    fillColor: window.LocationGeofenceFillColor || '#3388ff',
                    fillOpacity: parseFloat(window.LocationGeofenceFillOpacity || '0.3')
                },
                OpenSource: {
                    TileServer: window.OpenSourceTileServer || 'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
                    GeocodingService: window.OpenSourceGeocodingService || 'https://nominatim.openstreetmap.org',
                    MaxZoom: parseInt(window.OpenSourceMaxZoom || '19'),
                    Attribution: window.OpenSourceAttribution || '© OpenStreetMap contributors'
                },
                Google: {
                    ApiKey: window.GoogleApiKey || '',
                    MapType: window.GoogleMapType || 'roadmap',
                    Libraries: window.GoogleLibraries || 'places,geometry,drawing'
                }
            };
            
            if (callback) callback(config);
        },
        
        /**
         * Load provider implementation dynamically
         */
        loadProvider: function(providerName, callback) {
            var providerPath = '';
            
            switch(providerName.toLowerCase()) {
                case 'google':
                    providerPath = '../Providers/GoogleMapsLocationProvider.js';
                    break;
                case 'opensource':
                default:
                    providerPath = '../Providers/OpenSourceLocationProvider.js';
                    break;
            }
            
            // Check if provider is already loaded based on provider name
            var providerClassName;
            if (providerName.toLowerCase() === 'google') {
                providerClassName = 'GoogleMapsLocationProvider';
            } else {
                providerClassName = 'OpenSourceLocationProvider';
            }

            if (window[providerClassName] && typeof window[providerClassName] !== 'undefined') {
                callback(window[providerClassName]);
                return;
            }

            // Provider not loaded
            console.error('Location provider not found: ' + providerName);
            console.error('Expected global variable: ' + providerClassName);
            callback(this.createBasicProvider());
        },
        
        /**
         * Create a basic provider as fallback
         */
        createBasicProvider: function() {
            return {
                initialize: function(config, callback) { if (callback) callback(); },
                initializeMap: function() { return null; },
                geocodeAddress: function(address, callback) { callback(null); },
                reverseGeocode: function(lat, lng, callback) { callback(null); },
                getCurrentLocation: function(callback) { callback(null); },
                createGeofence: function() { return null; },
                calculateDistance: function() { return 0; },
                destroy: function() {}
            };
        },
        
        // ========== Proxy Methods to Current Provider ==========

        /**
         * Initialize a map in the specified container
         */
        initializeMap: function(mapConfig, successCallback, errorCallback) {
            if (!this.isInitialized) {
                console.error('Location service not initialized');
                if (errorCallback) errorCallback('Location service not initialized');
                return null;
            }

            try {
                var map = this.provider.initializeMap(mapConfig.containerId, {
                    center: mapConfig.center,
                    zoom: mapConfig.zoom
                });

                if (successCallback) successCallback(map);
                return map;
            } catch (error) {
                console.error('Error initializing map:', error);
                if (errorCallback) errorCallback(error);
                return null;
            }
        },
        
        /**
         * Show location picker modal
         */
        showLocationPicker: function(options, callback) {
            if (!this.isInitialized) {
                console.error('Location service not initialized');
                return;
            }
            
            // Delegate to LocationPickerModal component
            if (typeof LocationPickerModal !== 'undefined') {
                LocationPickerModal.show(options, callback);
            } else {
                console.error('LocationPickerModal component not loaded');
            }
        },
        
        /**
         * Geocode an address to coordinates
         */
        geocodeAddress: function(address, callback) {
            if (!this.isInitialized) {
                console.error('Location service not initialized');
                callback(null);
                return;
            }
            
            var self = this;
            var cacheKey = 'geocode_' + address;
            
            // Check cache first
            if (this.config.EnableCaching && this.cache[cacheKey]) {
                var cached = this.cache[cacheKey];
                if (Date.now() - cached.timestamp < this.config.CacheDuration * 1000) {
                    callback(cached.data);
                    return;
                }
            }
            
            // Geocode using provider
            this.provider.geocodeAddress(address, function(result) {
                if (result && self.config.EnableCaching) {
                    self.cache[cacheKey] = {
                        data: result,
                        timestamp: Date.now()
                    };
                }
                callback(result);
            });
        },
        
        /**
         * Reverse geocode coordinates to address
         */
        reverseGeocode: function(lat, lng, callback) {
            if (!this.isInitialized) {
                console.error('Location service not initialized');
                callback(null);
                return;
            }
            
            var self = this;
            var cacheKey = 'reverse_' + lat + '_' + lng;
            
            // Check cache first
            if (this.config.EnableCaching && this.cache[cacheKey]) {
                var cached = this.cache[cacheKey];
                if (Date.now() - cached.timestamp < this.config.CacheDuration * 1000) {
                    callback(cached.data);
                    return;
                }
            }
            
            // Reverse geocode using provider
            this.provider.reverseGeocode(lat, lng, function(result) {
                if (result && self.config.EnableCaching) {
                    self.cache[cacheKey] = {
                        data: result,
                        timestamp: Date.now()
                    };
                }
                callback(result);
            });
        },
        
        /**
         * Get current location using browser geolocation
         */
        getCurrentLocation: function(callback) {
            if (!this.config.EnableGeolocation) {
                callback({
                    error: 'Geolocation is disabled',
                    lat: null,
                    lng: null
                });
                return;
            }
            
            if (!navigator.geolocation) {
                callback({
                    error: 'Geolocation is not supported by your browser',
                    lat: null,
                    lng: null
                });
                return;
            }
            
            navigator.geolocation.getCurrentPosition(
                function(position) {
                    callback({
                        lat: position.coords.latitude,
                        lng: position.coords.longitude,
                        accuracy: position.coords.accuracy
                    });
                },
                function(error) {
                    var errorMessage = 'Unable to get location';
                    switch(error.code) {
                        case error.PERMISSION_DENIED:
                            errorMessage = 'Location permission denied';
                            break;
                        case error.POSITION_UNAVAILABLE:
                            errorMessage = 'Location information unavailable';
                            break;
                        case error.TIMEOUT:
                            errorMessage = 'Location request timed out';
                            break;
                    }
                    callback({
                        error: errorMessage,
                        lat: null,
                        lng: null
                    });
                },
                {
                    enableHighAccuracy: true,
                    timeout: 10000,
                    maximumAge: 0
                }
            );
        },
        
        /**
         * Create a geofence
         */
        createGeofence: function(center, radius, options) {
            if (!this.isInitialized) {
                console.error('Location service not initialized');
                return null;
            }
            
            // Merge with default geofence style
            var geofenceOptions = $.extend({}, this.config.GeofenceStyle, options);
            return this.provider.createGeofence(center, radius, geofenceOptions);
        },
        
        /**
         * Calculate distance between two points
         */
        calculateDistance: function(point1, point2, unit) {
            if (!this.isInitialized) {
                console.error('Location service not initialized');
                return 0;
            }
            
            // Use Haversine formula as fallback
            if (!this.provider.calculateDistance) {
                return this.haversineDistance(point1, point2, unit);
            }
            
            return this.provider.calculateDistance(point1, point2, unit);
        },
        
        /**
         * Haversine formula for distance calculation
         */
        haversineDistance: function(point1, point2, unit) {
            var R = unit === 'km' ? 6371 : 3959; // Earth radius in km or miles
            var dLat = this.toRad(point2.lat - point1.lat);
            var dLon = this.toRad(point2.lng - point1.lng);
            var lat1 = this.toRad(point1.lat);
            var lat2 = this.toRad(point2.lat);
            
            var a = Math.sin(dLat/2) * Math.sin(dLat/2) +
                    Math.sin(dLon/2) * Math.sin(dLon/2) * 
                    Math.cos(lat1) * Math.cos(lat2);
            var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a));
            var d = R * c;
            
            if (unit === 'm') {
                d = d * 1000; // Convert km to meters
            }
            
            return d;
        },
        
        /**
         * Convert degrees to radians
         */
        toRad: function(degrees) {
            return degrees * (Math.PI / 180);
        },
        
        /**
         * Validate coordinates
         */
        isValidCoordinate: function(lat, lng) {
            return lat >= -90 && lat <= 90 && lng >= -180 && lng <= 180;
        },
        
        /**
         * Format coordinates for display
         */
        formatCoordinates: function(lat, lng, format) {
            format = format || 'decimal';
            
            if (!this.isValidCoordinate(lat, lng)) {
                return 'Invalid coordinates';
            }
            
            switch(format) {
                case 'dms':
                    return this.toDMS(lat, 'lat') + ', ' + this.toDMS(lng, 'lng');
                case 'decimal':
                default:
                    return lat.toFixed(6) + ', ' + lng.toFixed(6);
            }
        },
        
        /**
         * Convert decimal degrees to DMS (Degrees Minutes Seconds)
         */
        toDMS: function(dd, type) {
            var dir = dd < 0 ? (type === 'lat' ? 'S' : 'W') : 
                               (type === 'lat' ? 'N' : 'E');
            var absDd = Math.abs(dd);
            var degrees = Math.floor(absDd);
            var minutes = Math.floor((absDd - degrees) * 60);
            var seconds = Math.round((absDd - degrees - minutes/60) * 3600);
            
            return degrees + '°' + minutes + "'" + seconds + '"' + dir;
        },
        
        /**
         * Add marker to the map
         */
        addMarker: function(markerConfig, callback) {
            if (!this.isInitialized) {
                console.error('Location service not initialized');
                return;
            }

            this.provider.addMarker(markerConfig.position.lat, markerConfig.position.lng, {
                title: markerConfig.title,
                color: markerConfig.color,
                label: markerConfig.label,
                draggable: markerConfig.draggable || false
            }, callback);
        },

        /**
         * Add click listener to marker
         */
        addMarkerClickListener: function(marker, callback) {
            if (!this.isInitialized) {
                console.error('Location service not initialized');
                return;
            }

            if (this.provider.addMarkerClickListener) {
                this.provider.addMarkerClickListener(marker, callback);
            }
        },

        /**
         * Show popup on marker
         */
        showPopup: function(marker, content) {
            if (!this.isInitialized) {
                console.error('Location service not initialized');
                return;
            }

            if (this.provider.showPopup) {
                this.provider.showPopup(marker, content);
            }
        },

        /**
         * Draw polyline on map
         */
        drawPolyline: function(config, callback) {
            if (!this.isInitialized) {
                console.error('Location service not initialized');
                if (callback) callback(null);
                return;
            }

            if (this.provider.drawPolyline) {
                this.provider.drawPolyline(config, callback);
            } else if (callback) {
                callback(null);
            }
        },

        /**
         * Remove marker from map
         */
        removeMarker: function(marker) {
            if (!this.isInitialized) {
                console.error('Location service not initialized');
                return;
            }

            if (this.provider.removeMarker) {
                this.provider.removeMarker(marker);
            }
        },

        /**
         * Remove polyline from map
         */
        removePolyline: function(polyline) {
            if (!this.isInitialized) {
                console.error('Location service not initialized');
                return;
            }

            if (this.provider.removePolyline) {
                this.provider.removePolyline(polyline);
            }
        },

        /**
         * Get marker position
         */
        getMarkerPosition: function(marker) {
            if (!this.isInitialized) {
                console.error('Location service not initialized');
                return null;
            }

            if (this.provider.getMarkerPosition) {
                return this.provider.getMarkerPosition(marker);
            }
            return null;
        },

        /**
         * Center map on position
         */
        centerMap: function(position, zoom) {
            if (!this.isInitialized) {
                console.error('Location service not initialized');
                return;
            }

            if (this.provider.centerMap) {
                this.provider.centerMap(position, zoom);
            }
        },

        /**
         * Fit map to bounds
         */
        fitBounds: function(bounds) {
            if (!this.isInitialized) {
                console.error('Location service not initialized');
                return;
            }

            if (this.provider.fitBounds) {
                this.provider.fitBounds(bounds);
            }
        },

        /**
         * Clear cache
         */
        clearCache: function() {
            this.cache = {};
        },
        
        /**
         * Destroy the service and cleanup
         */
        destroy: function() {
            if (this.provider && this.provider.destroy) {
                this.provider.destroy();
            }
            this.provider = null;
            this.isInitialized = false;
            this.clearCache();
        }
    };
    
    return service;
})();

// Auto-initialize on document ready if jQuery is available
if (typeof jQuery !== 'undefined') {
    $(document).ready(function() {
        // Only initialize if on a page that needs location services
        if ($('#clientLatitude').length > 0 || $('#Latitude').length > 0) {
            FieldSenseLocationService.init();
        }
    });
}