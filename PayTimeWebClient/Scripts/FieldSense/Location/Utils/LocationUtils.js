/**
 * Location Utilities
 * Common utility functions for location operations
 */

var LocationUtils = (function() {
    'use strict';
    
    var utils = {
        
        /**
         * Calculate distance between two points using Haversine formula
         */
        calculateDistance: function(point1, point2, unit) {
            unit = unit || 'km';
            
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
            
            if (unit === 'm' || unit === 'meters') {
                d = d * 1000; // Convert km to meters
            }
            
            return Math.round(d * 100) / 100; // Round to 2 decimal places
        },
        
        /**
         * Convert degrees to radians
         */
        toRad: function(degrees) {
            return degrees * (Math.PI / 180);
        },
        
        /**
         * Convert radians to degrees
         */
        toDeg: function(radians) {
            return radians * (180 / Math.PI);
        },
        
        /**
         * Validate GPS coordinates
         */
        isValidCoordinate: function(lat, lng) {
            if (typeof lat !== 'number' || typeof lng !== 'number') {
                try {
                    lat = parseFloat(lat);
                    lng = parseFloat(lng);
                } catch (e) {
                    return false;
                }
            }
            
            return !isNaN(lat) && !isNaN(lng) && 
                   lat >= -90 && lat <= 90 && 
                   lng >= -180 && lng <= 180;
        },
        
        /**
         * Format coordinates for display
         */
        formatCoordinates: function(lat, lng, format) {
            format = format || 'decimal';
            
            if (!this.isValidCoordinate(lat, lng)) {
                return 'Invalid coordinates';
            }
            
            lat = parseFloat(lat);
            lng = parseFloat(lng);
            
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
         * Convert DMS to decimal degrees
         */
        fromDMS: function(dms) {
            // Parse DMS string like "40°26'46"N" or "40 26 46 N"
            var match = dms.match(/(\d+)[°\s]+(\d+)['\s]+(\d+)["'\s]*([NSEW])/i);
            if (!match) return null;
            
            var degrees = parseInt(match[1]);
            var minutes = parseInt(match[2]);
            var seconds = parseInt(match[3]);
            var direction = match[4].toUpperCase();
            
            var decimal = degrees + minutes/60 + seconds/3600;
            if (direction === 'S' || direction === 'W') {
                decimal = -decimal;
            }
            
            return decimal;
        },
        
        /**
         * Check if a point is within a geofence (circle)
         */
        isPointInGeofence: function(point, center, radius) {
            var distance = this.calculateDistance(point, center, 'm');
            return distance <= radius;
        },
        
        /**
         * Get the bounds for a list of coordinates
         */
        getBounds: function(coordinates) {
            if (!coordinates || coordinates.length === 0) {
                return null;
            }
            
            var bounds = {
                north: -90,
                south: 90,
                east: -180,
                west: 180
            };
            
            coordinates.forEach(function(coord) {
                if (coord.lat > bounds.north) bounds.north = coord.lat;
                if (coord.lat < bounds.south) bounds.south = coord.lat;
                if (coord.lng > bounds.east) bounds.east = coord.lng;
                if (coord.lng < bounds.west) bounds.west = coord.lng;
            });
            
            return bounds;
        },
        
        /**
         * Get center point of multiple coordinates
         */
        getCenterPoint: function(coordinates) {
            if (!coordinates || coordinates.length === 0) {
                return null;
            }
            
            var totalLat = 0;
            var totalLng = 0;
            
            coordinates.forEach(function(coord) {
                totalLat += coord.lat;
                totalLng += coord.lng;
            });
            
            return {
                lat: totalLat / coordinates.length,
                lng: totalLng / coordinates.length
            };
        },
        
        /**
         * Generate a circle of points around a center
         */
        generateCirclePoints: function(center, radius, numPoints) {
            numPoints = numPoints || 64;
            var points = [];
            var angleIncrement = (2 * Math.PI) / numPoints;
            
            // Convert radius from meters to degrees (approximate)
            var radiusInDegrees = radius / 111320; // 1 degree ≈ 111,320 meters at equator
            
            for (var i = 0; i < numPoints; i++) {
                var angle = i * angleIncrement;
                var lat = center.lat + (radiusInDegrees * Math.cos(angle));
                var lng = center.lng + (radiusInDegrees * Math.sin(angle) / Math.cos(this.toRad(center.lat)));
                
                points.push({
                    lat: lat,
                    lng: lng
                });
            }
            
            return points;
        },
        
        /**
         * Parse various coordinate formats
         */
        parseCoordinates: function(input) {
            if (!input || typeof input !== 'string') {
                return null;
            }
            
            input = input.trim();
            
            // Try decimal format: "40.7128, -74.0060"
            var decimalMatch = input.match(/^(-?\d+\.?\d*),?\s*(-?\d+\.?\d*)$/);
            if (decimalMatch) {
                var lat = parseFloat(decimalMatch[1]);
                var lng = parseFloat(decimalMatch[2]);
                if (this.isValidCoordinate(lat, lng)) {
                    return { lat: lat, lng: lng };
                }
            }
            
            // Try DMS format: "40°42'46"N 74°0'21"W"
            var dmsMatch = input.match(/(\d+[°\s]\d+['\s]\d+["\s]*[NSEW])\s*,?\s*(\d+[°\s]\d+['\s]\d+["\s]*[NSEW])/i);
            if (dmsMatch) {
                var lat = this.fromDMS(dmsMatch[1]);
                var lng = this.fromDMS(dmsMatch[2]);
                if (lat !== null && lng !== null && this.isValidCoordinate(lat, lng)) {
                    return { lat: lat, lng: lng };
                }
            }
            
            return null;
        },
        
        /**
         * Format distance for display
         */
        formatDistance: function(distance, unit, precision) {
            unit = unit || 'auto';
            precision = precision || 2;
            
            if (unit === 'auto') {
                if (distance < 1000) {
                    return Math.round(distance) + 'm';
                } else {
                    return (distance / 1000).toFixed(precision) + 'km';
                }
            } else if (unit === 'm' || unit === 'meters') {
                return Math.round(distance) + 'm';
            } else if (unit === 'km' || unit === 'kilometers') {
                return (distance / 1000).toFixed(precision) + 'km';
            } else if (unit === 'mi' || unit === 'miles') {
                return (distance * 0.000621371).toFixed(precision) + 'mi';
            }
            
            return distance.toFixed(precision);
        },
        
        /**
         * Debounce function for search inputs
         */
        debounce: function(func, wait, immediate) {
            var timeout;
            return function() {
                var context = this, args = arguments;
                var later = function() {
                    timeout = null;
                    if (!immediate) func.apply(context, args);
                };
                var callNow = immediate && !timeout;
                clearTimeout(timeout);
                timeout = setTimeout(later, wait);
                if (callNow) func.apply(context, args);
            };
        },
        
        /**
         * Generate a random point within a circle
         */
        randomPointInCircle: function(center, radius) {
            var r = radius * Math.sqrt(Math.random());
            var theta = Math.random() * 2 * Math.PI;
            
            // Convert to degrees
            var radiusInDegrees = r / 111320;
            var lat = center.lat + (radiusInDegrees * Math.cos(theta));
            var lng = center.lng + (radiusInDegrees * Math.sin(theta) / Math.cos(this.toRad(center.lat)));
            
            return {
                lat: lat,
                lng: lng
            };
        },
        
        /**
         * Validate and normalize a location object
         */
        normalizeLocation: function(location) {
            if (!location) return null;
            
            var lat = parseFloat(location.lat || location.latitude);
            var lng = parseFloat(location.lng || location.longitude || location.lon);
            
            if (!this.isValidCoordinate(lat, lng)) {
                return null;
            }
            
            return {
                lat: lat,
                lng: lng,
                address: location.address || location.formatted_address || '',
                name: location.name || '',
                place_id: location.place_id || location.placeId || ''
            };
        }
    };
    
    return utils;
})();