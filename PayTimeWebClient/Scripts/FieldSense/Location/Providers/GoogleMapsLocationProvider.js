/**
 * Google Maps Location Provider
 * Uses Google Maps JavaScript API for maps and Google Geocoding API for address resolution
 */

var GoogleMapsLocationProvider = (function() {
    'use strict';

    var provider = {
        map: null,
        googleLoaded: false,
        config: null,
        markers: [],
        geofences: [],
        geocoder: null,
        activeInfoWindow: null, // Track currently open InfoWindow

        /**
         * Initialize the provider with configuration
         */
        initialize: function(config, callback) {
            var self = this;
            this.config = config;

            // Load Google Maps API
            this.loadGoogleMaps(function() {
                self.googleLoaded = true;
                self.geocoder = new google.maps.Geocoder();
                if (callback) callback();
            });
        },

        /**
         * Load Google Maps JavaScript API dynamically
         */
        loadGoogleMaps: function(callback) {
            // Check if Google Maps is already loaded
            if (typeof google !== 'undefined' && typeof google.maps !== 'undefined') {
                if (callback) callback();
                return;
            }

            // Check if script is already being loaded
            if (document.querySelector('script[src*="maps.googleapis.com"]')) {
                // Wait for it to load
                var checkInterval = setInterval(function() {
                    if (typeof google !== 'undefined' && typeof google.maps !== 'undefined') {
                        clearInterval(checkInterval);
                        if (callback) callback();
                    }
                }, 100);
                return;
            }

            // Create callback function name
            window.initGoogleMaps = function() {
                if (callback) callback();
            };

            // Build Google Maps API URL
            var apiKey = this.config.Google.ApiKey || '';
            var libraries = this.config.Google.Libraries || 'places,geometry,drawing';
            var url = 'https://maps.googleapis.com/maps/api/js?key=' + apiKey +
                     '&libraries=' + libraries + '&callback=initGoogleMaps';

            // Load script
            var script = document.createElement('script');
            script.src = url;
            script.async = true;
            script.defer = true;
            script.onerror = function() {
                console.error('Failed to load Google Maps API');
                // Fallback or error handling
            };
            document.head.appendChild(script);
        },

        /**
         * Initialize a map in the specified container
         */
        initializeMap: function(container, options) {
            if (!this.googleLoaded || typeof google === 'undefined') {
                console.error('Google Maps API not loaded');
                return null;
            }

            var mapOptions = {
                center: {
                    lat: options.center ? options.center.lat : this.config.DefaultCenter.lat,
                    lng: options.center ? options.center.lng : this.config.DefaultCenter.lng
                },
                zoom: options.zoom || this.config.DefaultZoom,
                mapTypeId: this.config.Google.MapType || 'roadmap',
                mapTypeControl: options.controls !== false,
                streetViewControl: options.controls !== false,
                fullscreenControl: options.controls !== false,
                zoomControl: options.controls !== false
            };

            this.map = new google.maps.Map(document.getElementById(container), mapOptions);

            return this.map;
        },

        /**
         * Add a marker to the map
         */
        addMarker: function(lat, lng, options, callback) {
            if (!this.map || typeof google === 'undefined') {
                console.error('Map not initialized');
                if (callback) callback(null);
                return null;
            }

            options = options || {};

            var markerOptions = {
                position: { lat: lat, lng: lng },
                map: this.map,
                draggable: options.draggable || false,
                title: options.title || ''
            };

            // Add custom icon or color
            if (options.icon) {
                markerOptions.icon = {
                    url: options.icon,
                    scaledSize: new google.maps.Size(
                        parseInt(options.width) || 32,
                        parseInt(options.height) || 32
                    )
                };
            } else if (options.color) {
                // Use colored marker with label
                var pinSVG = 'M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7zm0 9.5c-1.38 0-2.5-1.12-2.5-2.5s1.12-2.5 2.5-2.5 2.5 1.12 2.5 2.5-1.12 2.5-2.5 2.5z';
                markerOptions.icon = {
                    path: pinSVG,
                    fillColor: options.color,
                    fillOpacity: 1,
                    strokeColor: '#000',
                    strokeWeight: 1,
                    scale: 1.5,
                    anchor: new google.maps.Point(12, 24)
                };

                if (options.label) {
                    markerOptions.label = {
                        text: options.label.toString(),
                        color: '#fff',
                        fontSize: '12px',
                        fontWeight: 'bold'
                    };
                }
            }

            var marker = new google.maps.Marker(markerOptions);

            // Add popup/info window if provided
            if (options.popup) {
                var infoWindow = new google.maps.InfoWindow({
                    content: options.popup
                });

                marker.addListener('click', function() {
                    infoWindow.open(this.map, marker);
                }.bind(this));
            }

            this.markers.push(marker);

            if (callback) callback(marker);
            return marker;
        },

        /**
         * Geocode an address using Google Geocoding API
         */
        geocodeAddress: function(address, callback) {
            if (!address) {
                callback(null);
                return;
            }

            if (!this.geocoder) {
                console.error('Geocoder not initialized');
                callback(null);
                return;
            }

            this.geocoder.geocode({ address: address }, function(results, status) {
                if (status === 'OK' && results && results.length > 0) {
                    var result = results[0];
                    var location = result.geometry.location;

                    // Parse address components
                    var components = {
                        street: '',
                        city: '',
                        state: '',
                        country: '',
                        postalCode: ''
                    };

                    if (result.address_components) {
                        result.address_components.forEach(function(component) {
                            var types = component.types;
                            if (types.indexOf('route') > -1) {
                                components.street = component.long_name;
                            } else if (types.indexOf('locality') > -1) {
                                components.city = component.long_name;
                            } else if (types.indexOf('administrative_area_level_1') > -1) {
                                components.state = component.long_name;
                            } else if (types.indexOf('country') > -1) {
                                components.country = component.long_name;
                            } else if (types.indexOf('postal_code') > -1) {
                                components.postalCode = component.long_name;
                            }
                        });
                    }

                    callback({
                        lat: location.lat(),
                        lng: location.lng(),
                        formatted_address: result.formatted_address,
                        place_id: result.place_id,
                        components: components,
                        bounds: result.geometry.bounds ? {
                            northeast: {
                                lat: result.geometry.bounds.getNorthEast().lat(),
                                lng: result.geometry.bounds.getNorthEast().lng()
                            },
                            southwest: {
                                lat: result.geometry.bounds.getSouthWest().lat(),
                                lng: result.geometry.bounds.getSouthWest().lng()
                            }
                        } : null
                    });
                } else {
                    console.error('Geocoding failed:', status);
                    callback(null);
                }
            });
        },

        /**
         * Reverse geocode coordinates using Google Geocoding API
         */
        reverseGeocode: function(lat, lng, callback) {
            if (!this.geocoder) {
                console.error('Geocoder not initialized');
                callback(null);
                return;
            }

            var latlng = { lat: lat, lng: lng };

            this.geocoder.geocode({ location: latlng }, function(results, status) {
                if (status === 'OK' && results && results.length > 0) {
                    var result = results[0];

                    // Parse address components
                    var components = {
                        street: '',
                        city: '',
                        state: '',
                        country: '',
                        postalCode: ''
                    };

                    if (result.address_components) {
                        result.address_components.forEach(function(component) {
                            var types = component.types;
                            if (types.indexOf('route') > -1) {
                                components.street = component.long_name;
                            } else if (types.indexOf('locality') > -1) {
                                components.city = component.long_name;
                            } else if (types.indexOf('administrative_area_level_1') > -1) {
                                components.state = component.long_name;
                            } else if (types.indexOf('country') > -1) {
                                components.country = component.long_name;
                            } else if (types.indexOf('postal_code') > -1) {
                                components.postalCode = component.long_name;
                            }
                        });
                    }

                    callback({
                        lat: lat,
                        lng: lng,
                        formatted_address: result.formatted_address,
                        place_id: result.place_id,
                        components: components
                    });
                } else {
                    console.error('Reverse geocoding failed:', status);
                    callback({
                        error: 'Unable to get address for this location.',
                        lat: lat,
                        lng: lng
                    });
                }
            });
        },

        /**
         * Create a geofence using Google Maps Circle
         */
        createGeofence: function(center, radius, options) {
            if (!this.map || typeof google === 'undefined') {
                console.error('Map not initialized');
                return null;
            }

            options = options || {};
            var self = this;

            // Create circle
            var circle = new google.maps.Circle({
                center: { lat: center.lat, lng: center.lng },
                radius: radius, // in meters
                map: this.map,
                fillColor: options.fillColor || '#3388ff',
                fillOpacity: options.fillOpacity || 0.3,
                strokeColor: options.strokeColor || '#3388ff',
                strokeOpacity: 1,
                strokeWeight: options.strokeWidth || 2,
                clickable: false,
                editable: false
            });

            var geofence = {
                center: center,
                radius: radius,
                circle: circle,
                provider: self,
                contains: function(point) {
                    if (typeof google !== 'undefined' && google.maps && google.maps.geometry) {
                        var distance = google.maps.geometry.spherical.computeDistanceBetween(
                            new google.maps.LatLng(center.lat, center.lng),
                            new google.maps.LatLng(point.lat, point.lng)
                        );
                        return distance <= radius;
                    }
                    // Fallback to simple distance calculation
                    return this.provider.haversineDistance(center, point, 'meters') <= radius;
                },
                remove: function() {
                    try {
                        if (this.circle) {
                            this.circle.setMap(null);
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

            this.geofences.push(geofence);
            return geofence;
        },

        /**
         * Remove all geofences from map
         */
        removeAllGeofences: function() {
            var geofencesToRemove = this.geofences.slice();

            geofencesToRemove.forEach(function(geofence) {
                if (geofence && geofence.remove) {
                    geofence.remove();
                }
            });

            this.geofences = [];
        },

        /**
         * Calculate distance between two points using Google Geometry library
         */
        calculateDistance: function(point1, point2, unit) {
            if (typeof google !== 'undefined' && google.maps && google.maps.geometry) {
                var distance = google.maps.geometry.spherical.computeDistanceBetween(
                    new google.maps.LatLng(point1.lat, point1.lng),
                    new google.maps.LatLng(point2.lat, point2.lng)
                );

                // Distance is in meters by default
                if (unit === 'km' || unit === 'kilometers') {
                    return distance / 1000;
                } else if (unit === 'miles') {
                    return distance / 1609.34;
                }
                return distance; // meters
            }

            // Fallback to Haversine formula
            return this.haversineDistance(point1, point2, unit);
        },

        /**
         * Haversine distance calculation (fallback)
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
            if (!marker || typeof google === 'undefined') {
                return;
            }
            google.maps.event.addListener(marker, 'click', callback);
        },

        /**
         * Show popup/info window on marker
         */
        showPopup: function(marker, content) {
            if (!marker || !this.map || typeof google === 'undefined') {
                return;
            }

            // Close any previously open InfoWindow
            this.closePopup();

            this.activeInfoWindow = new google.maps.InfoWindow({
                content: content,
                maxWidth: 320,
                disableAutoPan: false,
                pixelOffset: new google.maps.Size(0, -10)
            });

            this.activeInfoWindow.open(this.map, marker);

            // Add custom styling to remove default InfoWindow styling
            google.maps.event.addListenerOnce(this.activeInfoWindow, 'domready', function() {
                // Find the InfoWindow container
                var iwOuter = document.querySelector('.gm-style-iw');
                var iwBackground = document.querySelector('.gm-style-iw-d');
                var iwCloseBtn = document.querySelector('.gm-style-iw-t button');

                if (iwOuter) {
                    // Remove padding and styling from outer container
                    iwOuter.style.padding = '0';
                    iwOuter.style.borderRadius = '12px';
                    iwOuter.style.boxShadow = '0 8px 24px rgba(0,0,0,0.15)';
                    iwOuter.style.overflow = 'hidden';
                }

                if (iwBackground) {
                    // Remove padding from content div
                    iwBackground.style.overflow = 'hidden';
                    iwBackground.style.padding = '0';
                    iwBackground.style.maxHeight = 'none';
                }

                if (iwCloseBtn) {
                    // Style close button
                    iwCloseBtn.style.top = '0px';
                    iwCloseBtn.style.right = '0px';
                    iwCloseBtn.style.opacity = '0.8';
                }

                // Hide the default arrow/pointer
                var iwArrow = document.querySelector('.gm-style-iw-tc');
                if (iwArrow) {
                    iwArrow.style.display = 'none';
                }
            });
        },

        /**
         * Close currently open popup/info window
         */
        closePopup: function() {
            if (this.activeInfoWindow) {
                this.activeInfoWindow.close();
                this.activeInfoWindow = null;
            }
        },

        /**
         * Draw a polyline on the map
         */
        drawPolyline: function(config, callback) {
            if (!this.map || typeof google === 'undefined') {
                console.error('Map not initialized');
                if (callback) callback(null);
                return;
            }

            var polylineOptions = {
                path: config.path,
                geodesic: true,
                strokeColor: config.strokeColor || '#007bff',
                strokeOpacity: config.strokeOpacity || 1.0,
                strokeWeight: config.strokeWeight || 3,
                map: this.map
            };

            // Handle dashed lines
            // Google Maps doesn't support CSS dash patterns natively, so we use symbol icons
            if (config.strokeDashArray) {
                var dashArray = config.strokeDashArray.split(',').map(function(val) {
                    return parseInt(val.trim());
                });

                // Create a dashed effect using line symbols
                // This creates visible dashes by drawing repeated short line segments
                var lineSymbol = {
                    path: 'M 0,-1 0,1',
                    strokeOpacity: 1,
                    strokeColor: config.strokeColor,
                    strokeWeight: 2,  // Define stroke width for the dash segment
                    fillOpacity: 0,   // No fill, stroke only
                    scale: 4          // Scale for visibility
                };

                // Very faint base line - necessary for Google Maps to render the icon pattern
                // Setting to 0 would prevent the icons from showing
                polylineOptions.strokeOpacity = 0.1;
                polylineOptions.icons = [{
                    icon: lineSymbol,
                    offset: '0',
                    repeat: (dashArray[0] + dashArray[1]) + 'px'
                }];
            }

            var polyline = new google.maps.Polyline(polylineOptions);

            if (callback) callback(polyline);
            return polyline;
        },

        /**
         * Remove a marker from the map
         */
        removeMarker: function(marker) {
            if (marker && marker.setMap) {
                marker.setMap(null);
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
            if (polyline && polyline.setMap) {
                polyline.setMap(null);
            }
        },

        /**
         * Get marker position
         */
        getMarkerPosition: function(marker) {
            if (!marker || !marker.getPosition) {
                return null;
            }
            var pos = marker.getPosition();
            return {
                lat: pos.lat(),
                lng: pos.lng()
            };
        },

        /**
         * Center map on a position with optional zoom
         */
        centerMap: function(position, zoom) {
            if (!this.map) {
                return;
            }
            this.map.setCenter(new google.maps.LatLng(position.lat, position.lng));
            if (zoom) {
                this.map.setZoom(zoom);
            }
        },

        /**
         * Fit map to show all bounds
         */
        fitBounds: function(bounds) {
            if (!this.map || !bounds || bounds.length === 0 || typeof google === 'undefined') {
                return;
            }

            var googleBounds = new google.maps.LatLngBounds();
            bounds.forEach(function(point) {
                googleBounds.extend(new google.maps.LatLng(point.lat, point.lng));
            });

            this.map.fitBounds(googleBounds);
        },

        /**
         * Calculate route between two points using Google Directions API
         */
        calculateRoute: function(origin, destination, options, callback) {
            if (typeof google === 'undefined' || !google.maps || !google.maps.DirectionsService) {
                console.error('Google Maps Directions Service not available');
                if (callback) callback(null);
                return;
            }

            // Default options
            options = options || {};
            var travelMode = options.travelMode || 'DRIVING';

            var directionsService = new google.maps.DirectionsService();

            var request = {
                origin: new google.maps.LatLng(origin.lat, origin.lng),
                destination: new google.maps.LatLng(destination.lat, destination.lng),
                travelMode: google.maps.TravelMode[travelMode],
                optimizeWaypoints: options.optimizeWaypoints || false,
                avoidHighways: options.avoidHighways || false,
                avoidTolls: options.avoidTolls || false
            };

            // Add waypoints if provided
            if (options.waypoints && options.waypoints.length > 0) {
                request.waypoints = options.waypoints.map(function(waypoint) {
                    return {
                        location: new google.maps.LatLng(waypoint.lat, waypoint.lng),
                        stopover: waypoint.stopover !== false
                    };
                });
            }

            directionsService.route(request, function(response, status) {
                if (status === 'OK' && response.routes && response.routes.length > 0) {
                    var route = response.routes[0];
                    var totalDistance = 0;
                    var totalDuration = 0;

                    // Calculate total distance and duration
                    route.legs.forEach(function(leg) {
                        if (leg.distance) {
                            totalDistance += leg.distance.value; // in meters
                        }
                        if (leg.duration) {
                            totalDuration += leg.duration.value; // in seconds
                        }
                    });

                    // Decode overview polyline to get path coordinates
                    var path = google.maps.geometry.encoding.decodePath(route.overview_polyline);
                    var pathCoordinates = path.map(function(latLng) {
                        return {
                            lat: latLng.lat(),
                            lng: latLng.lng()
                        };
                    });

                    var result = {
                        success: true,
                        distance: totalDistance, // meters
                        distanceKm: (totalDistance / 1000).toFixed(2),
                        duration: totalDuration, // seconds
                        durationMinutes: Math.round(totalDuration / 60),
                        path: pathCoordinates,
                        encodedPath: route.overview_polyline,
                        legs: route.legs.map(function(leg) {
                            return {
                                distance: leg.distance ? leg.distance.value : 0,
                                duration: leg.duration ? leg.duration.value : 0,
                                start_address: leg.start_address,
                                end_address: leg.end_address
                            };
                        })
                    };

                    if (callback) callback(result);
                } else {
                    console.warn('Directions request failed:', status);
                    if (callback) callback({
                        success: false,
                        error: status,
                        message: 'Unable to calculate route: ' + status
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
                        if (marker && marker.setMap) {
                            marker.setMap(null);
                        }
                    } catch (error) {
                        console.warn('Error removing marker:', error);
                    }
                });
                this.markers = [];

                // Remove all geofences
                this.removeAllGeofences();

                // Clear map reference
                if (this.map) {
                    this.map = null;
                }

            } catch (error) {
                console.error('Error in provider destroy:', error);
            }
        }
    };

    return provider;
})();
