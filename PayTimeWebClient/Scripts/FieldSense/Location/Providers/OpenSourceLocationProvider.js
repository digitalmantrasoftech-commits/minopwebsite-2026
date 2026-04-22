/**
 * OpenSource Location Provider
 * Uses MapLibre GL JS for maps and Nominatim for geocoding
 * Completely free and open source implementation
 */

var OpenSourceLocationProvider = (function() {
    'use strict';
    
    var provider = {
        map: null,
        maplibreLoaded: false,
        turfLoaded: false,
        config: null,
        markers: [],
        geofences: [],
        activePopup: null, // Track currently open popup
        mapReady: false, // Track if map style has finished loading
        operationQueue: [], // Queue operations until map is ready
        
        /**
         * Initialize the provider with configuration
         */
        initialize: function(config, callback) {
            var self = this;
            this.config = config;
            
            // Load required libraries
            this.loadDependencies(function() {
                self.maplibreLoaded = true;
                self.turfLoaded = true;
                if (callback) callback();
            });
        },
        
        /**
         * Load MapLibre GL JS and Turf.js dynamically
         */
        loadDependencies: function(callback) {
            var self = this;
            var scriptsToLoad = [];
            var stylesToLoad = [];
            
            // Check if MapLibre is already loaded
            if (typeof maplibregl === 'undefined') {
                scriptsToLoad.push({
                    url: 'https://unpkg.com/maplibre-gl@3.6.2/dist/maplibre-gl.js',
                    check: function() { return typeof maplibregl !== 'undefined'; }
                });
                stylesToLoad.push('https://unpkg.com/maplibre-gl@3.6.2/dist/maplibre-gl.css');
            }
            
            // Check if Turf.js is already loaded
            if (typeof turf === 'undefined') {
                scriptsToLoad.push({
                    url: 'https://unpkg.com/@turf/turf@6.5.0/turf.min.js',
                    check: function() { return typeof turf !== 'undefined'; }
                });
            }
            
            // Load styles
            stylesToLoad.forEach(function(url) {
                if (!document.querySelector('link[href="' + url + '"]')) {
                    var link = document.createElement('link');
                    link.rel = 'stylesheet';
                    link.href = url;
                    document.head.appendChild(link);
                }
            });
            
            // Load scripts sequentially
            var loadNext = function(index) {
                if (index >= scriptsToLoad.length) {
                    if (callback) callback();
                    return;
                }
                
                var scriptInfo = scriptsToLoad[index];
                if (scriptInfo.check()) {
                    loadNext(index + 1);
                    return;
                }
                
                var script = document.createElement('script');
                script.src = scriptInfo.url;
                script.onload = function() {
                    loadNext(index + 1);
                };
                script.onerror = function() {
                    console.error('Failed to load script: ' + scriptInfo.url);
                    loadNext(index + 1);
                };
                document.head.appendChild(script);
            };
            
            if (scriptsToLoad.length > 0) {
                loadNext(0);
            } else {
                if (callback) callback();
            }
        },
        
        /**
         * Initialize a map in the specified container
         */
        initializeMap: function(container, options) {
            if (!this.maplibreLoaded || typeof maplibregl === 'undefined') {
                console.error('MapLibre GL JS not loaded');
                return null;
            }
            
            var mapOptions = {
                container: container,
                style: this.getMapStyle(),
                center: [
                    options.center ? options.center.lng : this.config.DefaultCenter.lng,
                    options.center ? options.center.lat : this.config.DefaultCenter.lat
                ],
                zoom: options.zoom || this.config.DefaultZoom,
                maxZoom: this.config.OpenSource.MaxZoom || 19,
                attributionControl: true
            };
            
            this.map = new maplibregl.Map(mapOptions);
            this.mapReady = false;
            var self = this;

            // Wait for map style to load before allowing operations
            this.map.on('load', function() {
                self.mapReady = true;
                console.log('MapLibre GL map style loaded, executing queued operations (' + self.operationQueue.length + ')');

                // Execute all queued operations
                while (self.operationQueue.length > 0) {
                    var operation = self.operationQueue.shift();
                    try {
                        operation();
                    } catch (error) {
                        console.error('Error executing queued operation:', error);
                    }
                }
            });

            // Add navigation controls
            if (options.controls !== false) {
                this.map.addControl(new maplibregl.NavigationControl(), 'top-right');

                // Add geolocation control if enabled
                if (this.config.EnableGeolocation) {
                    this.map.addControl(new maplibregl.GeolocateControl({
                        positionOptions: {
                            enableHighAccuracy: true
                        },
                        trackUserLocation: false
                    }), 'top-right');
                }

                // Add fullscreen control
                this.map.addControl(new maplibregl.FullscreenControl(), 'top-right');
            }

            // Add attribution
            if (this.config.OpenSource.Attribution) {
                this.map.addControl(new maplibregl.AttributionControl({
                    customAttribution: this.config.OpenSource.Attribution
                }));
            }

            return this.map;
        },
        
        /**
         * Get map style configuration
         */
        getMapStyle: function() {
            return {
                version: 8,
                sources: {
                    'osm-tiles': {
                        type: 'raster',
                        tiles: [this.config.OpenSource.TileServer],
                        tileSize: 256,
                        attribution: this.config.OpenSource.Attribution
                    }
                },
                layers: [{
                    id: 'osm-tiles',
                    type: 'raster',
                    source: 'osm-tiles',
                    minzoom: 0,
                    maxzoom: this.config.OpenSource.MaxZoom || 19
                }]
            };
        },

        /**
         * Helper method to execute operations only when map is ready
         * Queues operations if map style hasn't finished loading
         */
        executeWhenReady: function(operation) {
            if (this.mapReady) {
                // Map is ready, execute immediately
                operation();
            } else {
                // Map not ready, queue the operation
                this.operationQueue.push(operation);
            }
        },

        /**
         * Add a marker to the map
         */
        addMarker: function (lat, lng, options, callback) {
            if (!this.map || typeof maplibregl === 'undefined') {
                console.error('Map not initialized');
                if (callback) callback(null);
                return null;
            }

            options = options || {};
            var self = this;
            var marker = null;

            // Wrap marker creation in executeWhenReady
            this.executeWhenReady(function() {
                var el = document.createElement('div');
                el.className = 'maplibre-marker';
                el.style.backgroundImage = options.icon || 'url(data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPHBhdGggZD0iTTEyIDJDOC4xMyAyIDUgNS4xMyA1IDljMCA1LjI1IDcgMTMgNyAxM3M3LTcuNzUgNy0xM2MwLTMuODctMy4xMy03LTctN3ptMCA5LjVjLTEuMzggMC0yLjUtMS4xMi0yLjUtMi41czEuMTItMi41IDIuNS0yLjUgMi41IDEuMTIgMi41IDIuNS0xLjEyIDIuNS0yLjUgMi41eiIgZmlsbD0iI2YzM2QzZCIvPgo8L3N2Zz4=)';
                el.style.width = options.width || '32px';
                el.style.height = options.height || '32px';
                el.style.backgroundSize = 'cover';
                el.style.cursor = 'pointer';

                marker = new maplibregl.Marker({
                    element: el,
                    draggable: options.draggable || false
                })
                .setLngLat([lng, lat])
                .addTo(self.map);

                if (options.popup) {
                    var popup = new maplibregl.Popup({offset: 25})
                        .setHTML(options.popup);
                    marker.setPopup(popup);
                }

                self.markers.push(marker);
                if (callback) callback(marker);
                return marker;
            });
           
        },
        
        /**
         * Geocode an address using Nominatim
         */
        geocodeAddress: function(address, callback) {
            if (!address) {
                callback(null);
                return;
            }
            
            var self = this;
            var url = this.config.OpenSource.GeocodingService + 
                     '/search?format=json&limit=5&q=' + 
                     encodeURIComponent(address);
            
            // Use JSONP to avoid CORS issues with Nominatim
            $.ajax({
                url: url,
                dataType: 'jsonp',
                jsonp: 'json_callback',
                timeout: 10000,
                success: function(results) {
                    if (results && results.length > 0) {
                        var result = results[0];
                        callback({
                            lat: parseFloat(result.lat),
                            lng: parseFloat(result.lon),
                            formatted_address: result.display_name,
                            place_id: result.place_id,
                            components: {
                                street: (result.address && result.address.road) || '',
                                city: (result.address && (result.address.city || result.address.town || result.address.village)) || '',
                                state: (result.address && result.address.state) || '',
                                country: (result.address && result.address.country) || '',
                                postalCode: (result.address && result.address.postcode) || ''
                            },
                            bounds: result.boundingbox ? {
                                northeast: {
                                    lat: parseFloat(result.boundingbox[1]),
                                    lng: parseFloat(result.boundingbox[3])
                                },
                                southwest: {
                                    lat: parseFloat(result.boundingbox[0]),
                                    lng: parseFloat(result.boundingbox[2])
                                }
                            } : null
                        });
                    } else {
                        callback(null);
                    }
                },
                error: function(jqXHR, textStatus, errorThrown) {
                    console.error('Geocoding failed:', textStatus, errorThrown);
                    // Fallback to direct JSON request if JSONP fails
                    self.geocodeAddressDirectFallback(address, callback);
                }
            });
        },
        
        /**
         * Fallback geocoding method using direct JSON (may have CORS issues)
         */
        geocodeAddressDirectFallback: function(address, callback) {
            var url = this.config.OpenSource.GeocodingService + 
                     '/search?format=json&limit=1&q=' + 
                     encodeURIComponent(address);
            
            $.ajax({
                url: url,
                dataType: 'json',
                timeout: 8000,
                success: function(results) {
                    if (results && results.length > 0) {
                        var result = results[0];
                        callback({
                            lat: parseFloat(result.lat),
                            lng: parseFloat(result.lon),
                            formatted_address: result.display_name,
                            place_id: result.place_id,
                            components: {
                                street: (result.address && result.address.road) || '',
                                city: (result.address && (result.address.city || result.address.town || result.address.village)) || '',
                                state: (result.address && result.address.state) || '',
                                country: (result.address && result.address.country) || '',
                                postalCode: (result.address && result.address.postcode) || ''
                            }
                        });
                    } else {
                        callback(null);
                    }
                },
                error: function(jqXHR, textStatus, errorThrown) {
                    console.error('Direct geocoding fallback also failed:', textStatus, errorThrown);
                    // Try alternative service as last resort
                    this.geocodeAddressAlternativeService(address, callback);
                }.bind(this)
            });
        },
        
        /**
         * Alternative geocoding service as last resort
         */
        geocodeAddressAlternativeService: function(address, callback) {
            // Use a simpler approach or show user-friendly error
            console.warn('All geocoding services failed for address:', address);
            callback({
                error: 'Unable to find location. Please try entering coordinates manually.',
                address: address
            });
        },
        
        /**
         * Reverse geocode coordinates using Nominatim
         */
        reverseGeocode: function(lat, lng, callback) {
            var url = this.config.OpenSource.GeocodingService + 
                     '/reverse?format=json&lat=' + lat + '&lon=' + lng;
            
            $.ajax({
                url: url,
                dataType: 'jsonp',
                jsonp: 'json_callback',
                timeout: 10000,
                success: function(result) {
                    if (result) {
                        callback({
                            lat: parseFloat(result.lat),
                            lng: parseFloat(result.lon),
                            formatted_address: result.display_name,
                            place_id: result.place_id,
                            components: {
                                street: (result.address && result.address.road) || '',
                                city: (result.address && (result.address.city || result.address.town || result.address.village)) || '',
                                state: (result.address && result.address.state) || '',
                                country: (result.address && result.address.country) || '',
                                postalCode: (result.address && result.address.postcode) || ''
                            }
                        });
                    } else {
                        callback(null);
                    }
                },
                error: function(jqXHR, textStatus, errorThrown) {
                    console.error('Reverse geocoding failed:', textStatus, errorThrown);
                    // Fallback to direct JSON request
                    this.reverseGeocodeDirectFallback(lat, lng, callback);
                }.bind(this)
            });
        },
        
        /**
         * Fallback reverse geocoding method using direct JSON (may have CORS issues)
         */
        reverseGeocodeDirectFallback: function(lat, lng, callback) {
            var url = this.config.OpenSource.GeocodingService + 
                     '/reverse?format=json&lat=' + lat + '&lon=' + lng;
            
            $.ajax({
                url: url,
                dataType: 'json',
                timeout: 8000,
                success: function(result) {
                    if (result) {
                        callback({
                            lat: parseFloat(result.lat),
                            lng: parseFloat(result.lon),
                            formatted_address: result.display_name,
                            place_id: result.place_id,
                            components: {
                                street: (result.address && result.address.road) || '',
                                city: (result.address && (result.address.city || result.address.town || result.address.village)) || '',
                                state: (result.address && result.address.state) || '',
                                country: (result.address && result.address.country) || '',
                                postalCode: (result.address && result.address.postcode) || ''
                            }
                        });
                    } else {
                        callback(null);
                    }
                },
                error: function(jqXHR, textStatus, errorThrown) {
                    console.error('Direct reverse geocoding fallback failed:', textStatus, errorThrown);
                    callback({
                        error: 'Unable to get address for this location.',
                        lat: lat,
                        lng: lng
                    });
                }
            });
        },
        
        /**
         * Create a geofence using Turf.js
         */
        createGeofence: function(center, radius, options) {
            if (!this.turfLoaded || typeof turf === 'undefined') {
                console.error('Turf.js not loaded');
                return null;
            }

            options = options || {};
            var self = this;

            // Generate unique IDs for this geofence
            var geofenceIndex = this.geofences.length;
            var sourceId = 'geofence-' + Date.now() + '-' + geofenceIndex;
            var fillLayerId = sourceId + '-fill';
            var outlineLayerId = sourceId + '-outline';

            // Create circle geometry using Turf.js
            var circle = turf.circle([center.lng, center.lat], radius, {
                steps: 64,
                units: 'meters'
            });

            var geofence = {
                center: center,
                radius: radius,
                geometry: circle,
                sourceId: sourceId,
                layerId: sourceId,
                fillLayerId: fillLayerId,
                outlineLayerId: outlineLayerId,
                provider: self,
                contains: function(point) {
                    if (typeof turf !== 'undefined') {
                        var pt = turf.point([point.lng, point.lat]);
                        return turf.booleanPointInPolygon(pt, circle);
                    }
                    return false;
                },
                remove: function() {
                    try {
                        console.log('Removing geofence with IDs:', {
                            source: this.sourceId,
                            fill: this.fillLayerId,
                            outline: this.outlineLayerId
                        });

                        // Remove from map if exists
                        if (this.provider && this.provider.map) {
                            this.provider.removeGeofenceFromMap(this.sourceId, this.fillLayerId, this.outlineLayerId);
                        }

                        // Remove from provider's geofences array
                        if (this.provider && this.provider.geofences) {
                            var index = this.provider.geofences.indexOf(this);
                            if (index > -1) {
                                this.provider.geofences.splice(index, 1);
                            }
                        }

                    } catch (error) {
                        console.error('Error removing geofence:', error);
                    }
                }
            };

            // Add to map if available - wrap in executeWhenReady
            if (this.map) {
                this.executeWhenReady(function() {
                    try {
                        // Remove existing source if it exists (cleanup)
                        if (self.map.getSource(sourceId)) {
                            console.warn('Source already exists, removing:', sourceId);
                            self.removeGeofenceFromMap(sourceId, fillLayerId, outlineLayerId);
                        }

                        // Add source
                        self.map.addSource(sourceId, {
                            type: 'geojson',
                            data: circle
                        });

                        // Add fill layer
                        self.map.addLayer({
                            id: fillLayerId,
                            type: 'fill',
                            source: sourceId,
                            paint: {
                                'fill-color': options.fillColor || '#3388ff',
                                'fill-opacity': options.fillOpacity || 0.3
                            }
                        });

                        // Add outline layer
                        self.map.addLayer({
                            id: outlineLayerId,
                            type: 'line',
                            source: sourceId,
                            paint: {
                                'line-color': options.strokeColor || '#3388ff',
                                'line-width': options.strokeWidth || 2
                            }
                        });

                        console.log('Created geofence with IDs:', {
                            source: sourceId,
                            fill: fillLayerId,
                            outline: outlineLayerId
                        });

                    } catch (error) {
                        console.error('Error adding geofence to map:', error);
                    }
                });
            }

            this.geofences.push(geofence);
            return geofence;
        },

        /**
         * Remove geofence layers and source from map
         */
        removeGeofenceFromMap: function(sourceId, fillLayerId, outlineLayerId) {
            if (!this.map) return;
            
            try {
                // Remove fill layer
                if (this.map.getLayer(fillLayerId)) {
                    this.map.removeLayer(fillLayerId);
                    console.log('Removed fill layer:', fillLayerId);
                }
                
                // Remove outline layer  
                if (this.map.getLayer(outlineLayerId)) {
                    this.map.removeLayer(outlineLayerId);
                    console.log('Removed outline layer:', outlineLayerId);
                }
                
                // Remove source
                if (this.map.getSource(sourceId)) {
                    this.map.removeSource(sourceId);
                    console.log('Removed source:', sourceId);
                }
                
            } catch (error) {
                console.error('Error removing geofence from map:', error);
            }
        },

        /**
         * Remove all geofences from map
         */
        removeAllGeofences: function() {
            // Create a copy of the geofences array to avoid modification during iteration
            var geofencesToRemove = this.geofences.slice();
            
            geofencesToRemove.forEach(function(geofence) {
                if (geofence && geofence.remove) {
                    geofence.remove();
                }
            });
            
            // Clear the array
            this.geofences = [];
        },
        
        /**
         * Calculate distance between two points
         */
        calculateDistance: function(point1, point2, unit) {
            if (typeof turf !== 'undefined') {
                var from = turf.point([point1.lng, point1.lat]);
                var to = turf.point([point2.lng, point2.lat]);
                var options = {units: unit || 'kilometers'};
                return turf.distance(from, to, options);
            }
            
            // Fallback to Haversine formula
            return this.haversineDistance(point1, point2, unit);
        },
        
        /**
         * Haversine distance calculation
         */
        haversineDistance: function(point1, point2, unit) {
            var R = unit === 'km' || unit === 'kilometers' ? 6371 : 3959;
            var dLat = this.toRad(point2.lat - point1.lat);
            var dLon = this.toRad(point2.lng - point1.lng);
            var lat1 = this.toRad(point1.lat);
            var lat2 = this.toRad(point2.lat);
            
            var a = Math.sin(dLat/2) * Math.sin(dLat/2) +
                    Math.sin(dLon/2) * Math.sin(dLon/2) * 
                    Math.cos(lat1) * Math.cos(lat2);
            var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a));
            var d = R * c;
            
            if (unit === 'meters' || unit === 'm') {
                d = d * 1000;
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
         * Get current location (delegates to browser geolocation)
         */
        getCurrentLocation: function(callback) {
            // This is handled by the main service
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                    function(position) {
                        callback({
                            lat: position.coords.latitude,
                            lng: position.coords.longitude,
                            accuracy: position.coords.accuracy
                        });
                    },
                    function(error) {
                        callback({
                            error: error.message,
                            lat: null,
                            lng: null
                        });
                    }
                );
            } else {
                callback({
                    error: 'Geolocation not supported',
                    lat: null,
                    lng: null
                });
            }
        },
        
        /**
         * Add marker click listener
         */
        addMarkerClickListener: function(marker, callback) {
            if (!marker) return;

            var el = marker.getElement();
            if (el) {
                el.addEventListener('click', callback);
            }
        },

        /**
         * Show popup on marker
         */
        showPopup: function(marker, content) {
            if (!marker || typeof maplibregl === 'undefined') {
                return;
            }

            // Close any previously open popup

            this.closePopup();

            var lngLat = marker.getLngLat();

            this.activePopup = new maplibregl.Popup({
                offset: 25,
                closeButton: true,
                closeOnClick: false,
                maxWidth: '320px',
                className: 'custom-popup'
            })
            .setLngLat(lngLat)
            .setHTML(content)
            .addTo(this.map);

            // Style the popup container after it's added to DOM
            setTimeout(function() {
                var popupContent = document.querySelector('.maplibregl-popup-content');
                if (popupContent) {
                    popupContent.style.padding = '0';
                    popupContent.style.borderRadius = '12px';
                    popupContent.style.boxShadow = '0 8px 24px rgba(0,0,0,0.15)';
                    popupContent.style.overflow = 'hidden';
                }

                // Style close button
                var closeBtn = document.querySelector('.maplibregl-popup-close-button');
                if (closeBtn) {
                    closeBtn.style.color = '#333';
                    closeBtn.style.fontSize = '20px';
                    closeBtn.style.padding = '8px';
                    closeBtn.style.opacity = '0.7';
                    closeBtn.style.right = '4px';
                    closeBtn.style.top = '4px';
                }

                // Remove the arrow tip
                var popupTip = document.querySelector('.maplibregl-popup-tip');
                if (popupTip) {
                    popupTip.style.display = 'none';
                }
            }, 10);
        },

        /**
         * Close currently open popup
         */
        closePopup: function() {
            if (this.activePopup) {
                this.activePopup.remove();
                this.activePopup = null;
            }
        },

        /**
         * Draw a polyline on the map
         */
        drawPolyline: function(config, callback) {
            if (!this.map || typeof maplibregl === 'undefined') {
                console.error('Map not initialized');
                if (callback) callback(null);
                return null;
            }

            var self = this;
            var sourceId = 'polyline-' + Date.now() + '-' + Math.random();
            var layerId = sourceId + '-layer';

            // Convert path to GeoJSON LineString
            var coordinates = config.path.map(function(point) {
                return [point.lng, point.lat];
            });

            var geojson = {
                type: 'Feature',
                geometry: {
                    type: 'LineString',
                    coordinates: coordinates
                }
            };

            var polyline = {
                sourceId: sourceId,
                layerId: layerId,
                provider: this,
                remove: function() {
                    if (this.provider && this.provider.map) {
                        try {
                            if (this.provider.map.getLayer(this.layerId)) {
                                this.provider.map.removeLayer(this.layerId);
                            }
                            if (this.provider.map.getSource(this.sourceId)) {
                                this.provider.map.removeSource(this.sourceId);
                            }
                        } catch (error) {
                            console.error('Error removing polyline:', error);
                        }
                    }
                }
            };

            // Wrap polyline creation in executeWhenReady
            this.executeWhenReady(function() {
                try {
                    // Add source
                    self.map.addSource(sourceId, {
                        type: 'geojson',
                        data: geojson
                    });

                    // Add layer
                    var layerConfig = {
                        id: layerId,
                        type: 'line',
                        source: sourceId,
                        layout: {
                            'line-join': 'round',
                            'line-cap': 'round'
                        },
                        paint: {
                            'line-color': config.strokeColor || '#007bff',
                            'line-width': config.strokeWeight || 3,
                            'line-opacity': config.strokeOpacity || 1
                        }
                    };

                    // Handle dashed lines
                    if (config.strokeDashArray) {
                        var dashArray = config.strokeDashArray.split(',').map(function(val) {
                            return parseInt(val.trim());
                        });
                        layerConfig.paint['line-dasharray'] = dashArray;
                    }

                    self.map.addLayer(layerConfig);

                    if (callback) callback(polyline);
                } catch (error) {
                    console.error('Error drawing polyline:', error);
                    if (callback) callback(null);
                }
            });

            return polyline;
        },

        /**
         * Remove a marker from the map
         */
        removeMarker: function(marker) {
            if (marker && marker.remove) {
                marker.remove();
                var index = this.markers.indexOf(marker);
                if (index > -1) {
                    this.markers.splice(index, 1);
                }
            }
        },

        /**
         * Remove a polyline from the map
         */
        removePolyline: function(polyline) {
            if (polyline && polyline.remove) {
                polyline.remove();
            }
        },

        /**
         * Get marker position
         */
        getMarkerPosition: function(marker) {
            if (!marker || !marker.getLngLat) {
                return null;
            }
            var lngLat = marker.getLngLat();
            return {
                lat: lngLat.lat,
                lng: lngLat.lng
            };
        },

        /**
         * Center map on a position with optional zoom
         */
        centerMap: function(position, zoom) {
            if (!this.map) {
                return;
            }
            var options = {
                center: [position.lng, position.lat]
            };
            if (zoom) {
                options.zoom = zoom;
            }
            this.map.flyTo(options);
        },

        /**
         * Fit map to show all bounds
         */
        fitBounds: function(bounds) {
            if (!this.map || !bounds || bounds.length === 0 || typeof maplibregl === 'undefined') {
                return;
            }

            var self = this;

            // Wrap in executeWhenReady to ensure map style is fully loaded
            this.executeWhenReady(function() {
                var lngLatBounds = new maplibregl.LngLatBounds();
                bounds.forEach(function(point) {
                    lngLatBounds.extend([point.lng, point.lat]);
                });

                self.map.fitBounds(lngLatBounds, {
                    padding: 100
                });
            });
        },

        /**
         * Get OSRM route between two points
         * Uses OpenStreetMap Routing Machine for actual driving routes
         */
        getOSRMRoute: function(origin, destination, callback) {
            // OSRM API expects coordinates in lon,lat format
            var url = 'https://router.project-osrm.org/route/v1/driving/' +
                      origin.lng + ',' + origin.lat + ';' +
                      destination.lng + ',' + destination.lat +
                      '?overview=full&geometries=geojson';

            $.ajax({
                url: url,
                type: 'GET',
                dataType: 'json',
                timeout: 10000,
                success: function(response) {
                    if (response && response.code === 'Ok' && response.routes && response.routes.length > 0) {
                        var route = response.routes[0];

                        // Convert GeoJSON coordinates to our format
                        var path = route.geometry.coordinates.map(function(coord) {
                            return {
                                lat: coord[1],
                                lng: coord[0]
                            };
                        });

                        // Format distance and duration
                        var distanceKm = (route.distance / 1000).toFixed(2);
                        var durationMin = Math.round(route.duration / 60);
                        var durationText = durationMin < 60
                            ? durationMin + ' min'
                            : Math.floor(durationMin / 60) + 'h ' + (durationMin % 60) + 'm';

                        callback({
                            success: true,
                            path: path,
                            distance: route.distance, // in meters
                            duration: route.duration, // in seconds
                            distanceText: distanceKm + ' km',
                            durationText: durationText
                        });
                    } else {
                        console.warn('OSRM routing failed:', response);
                        callback({
                            success: false,
                            error: 'Route not found'
                        });
                    }
                },
                error: function(xhr, status, error) {
                    console.error('OSRM API error:', error);
                    callback({
                        success: false,
                        error: 'Failed to calculate route: ' + error
                    });
                }
            });
        },

        /**
         * Clean up and destroy the provider
         */
        destroy: function() {
            try {
                // Remove all markers
                this.markers.forEach(function(marker) {
                    try {
                        if (marker && marker.remove) {
                            marker.remove();
                        }
                    } catch (error) {
                        console.warn('Error removing marker:', error);
                    }
                });
                this.markers = [];

                // Remove all geofences
                this.removeAllGeofences();

                // Remove map
                if (this.map) {
                    try {
                        this.map.remove();
                    } catch (error) {
                        console.warn('Error removing map:', error);
                    }
                    this.map = null;
                }

            } catch (error) {
                console.error('Error in provider destroy:', error);
            }
        }
    };

    return provider;
})();