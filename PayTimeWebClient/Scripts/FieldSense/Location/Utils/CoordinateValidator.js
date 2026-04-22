/**
 * Coordinate Validator
 * Validation utilities for GPS coordinates and location data
 */

var CoordinateValidator = (function() {
    'use strict';
    
    var validator = {
        
        /**
         * Validate latitude value
         */
        validateLatitude: function(lat) {
            var result = {
                isValid: false,
                value: null,
                error: null
            };
            
            if (lat === null || lat === undefined || lat === '') {
                result.error = 'Latitude is required';
                return result;
            }
            
            var numLat = parseFloat(lat);
            if (isNaN(numLat)) {
                result.error = 'Latitude must be a valid number';
                return result;
            }
            
            if (numLat < -90 || numLat > 90) {
                result.error = 'Latitude must be between -90 and 90 degrees';
                return result;
            }
            
            result.isValid = true;
            result.value = numLat;
            return result;
        },
        
        /**
         * Validate longitude value
         */
        validateLongitude: function(lng) {
            var result = {
                isValid: false,
                value: null,
                error: null
            };
            
            if (lng === null || lng === undefined || lng === '') {
                result.error = 'Longitude is required';
                return result;
            }
            
            var numLng = parseFloat(lng);
            if (isNaN(numLng)) {
                result.error = 'Longitude must be a valid number';
                return result;
            }
            
            if (numLng < -180 || numLng > 180) {
                result.error = 'Longitude must be between -180 and 180 degrees';
                return result;
            }
            
            result.isValid = true;
            result.value = numLng;
            return result;
        },
        
        /**
         * Validate coordinate pair
         */
        validateCoordinates: function(lat, lng) {
            var result = {
                isValid: false,
                latitude: null,
                longitude: null,
                errors: []
            };
            
            var latResult = this.validateLatitude(lat);
            var lngResult = this.validateLongitude(lng);
            
            if (!latResult.isValid) {
                result.errors.push(latResult.error);
            } else {
                result.latitude = latResult.value;
            }
            
            if (!lngResult.isValid) {
                result.errors.push(lngResult.error);
            } else {
                result.longitude = lngResult.value;
            }
            
            result.isValid = latResult.isValid && lngResult.isValid;
            return result;
        },
        
        /**
         * Validate radius value
         */
        validateRadius: function(radius) {
            var result = {
                isValid: false,
                value: null,
                error: null
            };
            
            if (radius === null || radius === undefined || radius === '') {
                result.error = 'Radius is required';
                return result;
            }
            
            var numRadius = parseFloat(radius);
            if (isNaN(numRadius)) {
                result.error = 'Radius must be a valid number';
                return result;
            }
            
            if (numRadius <= 0) {
                result.error = 'Radius must be greater than 0';
                return result;
            }
            
            if (numRadius > 10000) {
                result.error = 'Radius cannot exceed 10,000 meters';
                return result;
            }
            
            result.isValid = true;
            result.value = numRadius;
            return result;
        },
        
        /**
         * Validate location object
         */
        validateLocation: function(location) {
            var result = {
                isValid: false,
                location: null,
                errors: []
            };
            
            if (!location) {
                result.errors.push('Location object is required');
                return result;
            }
            
            var coordResult = this.validateCoordinates(location.lat || location.latitude, 
                                                      location.lng || location.longitude);
            
            if (!coordResult.isValid) {
                result.errors = result.errors.concat(coordResult.errors);
                return result;
            }
            
            // Validate optional fields
            var validatedLocation = {
                lat: coordResult.latitude,
                lng: coordResult.longitude
            };
            
            // Validate address if provided
            if (location.address && typeof location.address === 'string') {
                if (location.address.length > 500) {
                    result.errors.push('Address cannot exceed 500 characters');
                } else {
                    validatedLocation.address = location.address.trim();
                }
            }
            
            // Validate radius if provided
            if (location.radius !== null && location.radius !== undefined) {
                var radiusResult = this.validateRadius(location.radius);
                if (!radiusResult.isValid) {
                    result.errors.push(radiusResult.error);
                } else {
                    validatedLocation.radius = radiusResult.value;
                }
            }
            
            // Validate name if provided
            if (location.name && typeof location.name === 'string') {
                if (location.name.length > 100) {
                    result.errors.push('Location name cannot exceed 100 characters');
                } else {
                    validatedLocation.name = location.name.trim();
                }
            }
            
            result.isValid = result.errors.length === 0;
            if (result.isValid) {
                result.location = validatedLocation;
            }
            
            return result;
        },
        
        /**
         * Validate coordinate precision
         */
        validatePrecision: function(lat, lng, maxDecimalPlaces) {
            maxDecimalPlaces = maxDecimalPlaces || 6;
            
            var result = {
                isValid: true,
                errors: []
            };
            
            var latStr = lat.toString();
            var lngStr = lng.toString();
            
            var latDecimals = latStr.includes('.') ? latStr.split('.')[1].length : 0;
            var lngDecimals = lngStr.includes('.') ? lngStr.split('.')[1].length : 0;
            
            if (latDecimals > maxDecimalPlaces) {
                result.isValid = false;
                result.errors.push('Latitude precision cannot exceed ' + maxDecimalPlaces + ' decimal places');
            }
            
            if (lngDecimals > maxDecimalPlaces) {
                result.isValid = false;
                result.errors.push('Longitude precision cannot exceed ' + maxDecimalPlaces + ' decimal places');
            }
            
            return result;
        },
        
        /**
         * Validate if coordinates are within a specific country/region
         */
        validateRegion: function(lat, lng, region) {
            // Basic region validation - can be expanded
            var regions = {
                india: {
                    north: 37.6,
                    south: 6.4,
                    east: 97.25,
                    west: 68.7
                },
                usa: {
                    north: 71.5,
                    south: 18.9,
                    east: -66.9,
                    west: 179.9
                }
            };
            
            var result = {
                isValid: false,
                error: null
            };
            
            if (!regions[region.toLowerCase()]) {
                result.error = 'Unknown region: ' + region;
                return result;
            }
            
            var bounds = regions[region.toLowerCase()];
            
            if (lat >= bounds.south && lat <= bounds.north && 
                lng >= bounds.west && lng <= bounds.east) {
                result.isValid = true;
            } else {
                result.error = 'Coordinates are outside the specified region';
            }
            
            return result;
        },
        
        /**
         * Validate if coordinates represent a reasonable location
         */
        validateReasonableLocation: function(lat, lng) {
            var result = {
                isValid: true,
                warnings: []
            };
            
            // Check for common invalid coordinates
            if (lat === 0 && lng === 0) {
                result.isValid = false;
                result.warnings.push('Coordinates appear to be null island (0,0)');
                return result;
            }
            
            // Check if coordinates are in the middle of an ocean (very basic check)
            var oceanCoords = [
                { name: 'Pacific Ocean', lat: 0, lng: -160, radius: 2000 },
                { name: 'Atlantic Ocean', lat: 0, lng: -30, radius: 1500 },
                { name: 'Indian Ocean', lat: -20, lng: 80, radius: 1000 }
            ];
            
            for (var i = 0; i < oceanCoords.length; i++) {
                var ocean = oceanCoords[i];
                var distance = LocationUtils.calculateDistance(
                    { lat: lat, lng: lng },
                    { lat: ocean.lat, lng: ocean.lng },
                    'km'
                );
                
                if (distance < ocean.radius) {
                    result.warnings.push('Coordinates appear to be in the ' + ocean.name);
                    break;
                }
            }
            
            return result;
        },
        
        /**
         * Format validation errors for display
         */
        formatErrors: function(errors) {
            if (!errors || errors.length === 0) {
                return '';
            }
            
            if (errors.length === 1) {
                return errors[0];
            }
            
            return '• ' + errors.join('\n• ');
        },
        
        /**
         * Validate coordinate string format
         */
        validateCoordinateFormat: function(coordString) {
            var result = {
                isValid: false,
                format: null,
                coordinates: null,
                error: null
            };
            
            if (!coordString || typeof coordString !== 'string') {
                result.error = 'Coordinate string is required';
                return result;
            }
            
            coordString = coordString.trim();
            
            // Check decimal format: "40.7128, -74.0060" or "40.7128 -74.0060"
            var decimalRegex = /^(-?\d+\.?\d*)[,\s]+(-?\d+\.?\d*)$/;
            var decimalMatch = coordString.match(decimalRegex);
            
            if (decimalMatch) {
                var lat = parseFloat(decimalMatch[1]);
                var lng = parseFloat(decimalMatch[2]);
                var coordResult = this.validateCoordinates(lat, lng);
                
                if (coordResult.isValid) {
                    result.isValid = true;
                    result.format = 'decimal';
                    result.coordinates = { lat: lat, lng: lng };
                    return result;
                }
            }
            
            // Check DMS format
            var dmsRegex = /(\d+)[°\s]+(\d+)['\s]+(\d+(?:\.\d+)?)["\s]*([NSEW])[,\s]*(\d+)[°\s]+(\d+)['\s]+(\d+(?:\.\d+)?)["\s]*([NSEW])/i;
            var dmsMatch = coordString.match(dmsRegex);
            
            if (dmsMatch) {
                result.isValid = true;
                result.format = 'dms';
                // DMS parsing would be implemented here
                result.error = 'DMS format parsing not yet implemented';
                return result;
            }
            
            result.error = 'Invalid coordinate format. Expected formats: "40.7128, -74.0060" or DMS';
            return result;
        }
    };
    
    return validator;
})();