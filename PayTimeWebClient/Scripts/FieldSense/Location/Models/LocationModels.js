/**
 * Location Data Models
 * Standardized data models for location operations
 */

var LocationModels = (function() {
    'use strict';
    
    var models = {};
    
    /**
     * Standard Location Model
     */
    models.Location = function(data) {
        data = data || {};
        
        this.lat = data.lat || data.latitude || null;
        this.lng = data.lng || data.longitude || data.lon || null;
        this.address = data.address || data.formatted_address || '';
        this.name = data.name || '';
        this.place_id = data.place_id || data.placeId || '';
        this.components = data.components || {
            street: '',
            city: '',
            state: '',
            country: '',
            postalCode: ''
        };
        
        // Validation method
        this.isValid = function() {
            return LocationUtils.isValidCoordinate(this.lat, this.lng);
        };
        
        // Convert to object
        this.toObject = function() {
            return {
                lat: this.lat,
                lng: this.lng,
                address: this.address,
                name: this.name,
                place_id: this.place_id,
                components: this.components
            };
        };
        
        // Format for display
        this.toString = function() {
            if (this.address) {
                return this.address;
            } else if (this.isValid()) {
                return LocationUtils.formatCoordinates(this.lat, this.lng);
            }
            return 'Unknown Location';
        };
    };
    
    /**
     * Geofence Model
     */
    models.Geofence = function(data) {
        data = data || {};
        
        this.id = data.id || null;
        this.center = new models.Location(data.center);
        this.radius = data.radius || 100; // meters
        this.name = data.name || '';
        this.description = data.description || '';
        this.type = data.type || 'circle';
        this.isActive = data.isActive !== undefined ? data.isActive : true;
        this.style = data.style || {
            strokeColor: '#3388ff',
            strokeWidth: 2,
            fillColor: '#3388ff',
            fillOpacity: 0.3
        };
        this.createdAt = data.createdAt || new Date();
        this.updatedAt = data.updatedAt || new Date();
        
        // Check if a point is within this geofence
        this.contains = function(point) {
            if (!this.center.isValid() || !LocationUtils.isValidCoordinate(point.lat, point.lng)) {
                return false;
            }
            
            if (this.type === 'circle') {
                return LocationUtils.isPointInGeofence(point, this.center, this.radius);
            }
            
            return false;
        };
        
        // Get the area of the geofence
        this.getArea = function() {
            if (this.type === 'circle') {
                return Math.PI * Math.pow(this.radius, 2); // square meters
            }
            return 0;
        };
        
        // Validation method
        this.isValid = function() {
            return this.center.isValid() && 
                   this.radius > 0 && 
                   this.radius <= 10000;
        };
        
        // Convert to object
        this.toObject = function() {
            return {
                id: this.id,
                center: this.center.toObject(),
                radius: this.radius,
                name: this.name,
                description: this.description,
                type: this.type,
                isActive: this.isActive,
                style: this.style,
                createdAt: this.createdAt,
                updatedAt: this.updatedAt
            };
        };
    };
    
    /**
     * Map Options Model
     */
    models.MapOptions = function(data) {
        data = data || {};
        
        this.center = data.center || [77.2090, 28.6139]; // Default to Delhi, India
        this.zoom = data.zoom || 10;
        this.minZoom = data.minZoom || 1;
        this.maxZoom = data.maxZoom || 19;
        this.style = data.style || 'streets';
        this.interactive = data.interactive !== undefined ? data.interactive : true;
        this.controls = data.controls || {
            zoom: true,
            fullscreen: true,
            geolocate: true,
            navigation: true
        };
        this.attribution = data.attribution || '';
        
        // Convert to provider-specific options
        this.toProviderOptions = function(provider) {
            var options = {
                center: this.center,
                zoom: this.zoom,
                minZoom: this.minZoom,
                maxZoom: this.maxZoom,
                interactive: this.interactive
            };
            
            if (provider === 'maplibre') {
                options.attributionControl = this.controls.attribution;
                options.style = this.getMapLibreStyle();
            } else if (provider === 'google') {
                options.mapTypeId = this.getGoogleMapType();
                options.zoomControl = this.controls.zoom;
                options.fullscreenControl = this.controls.fullscreen;
            }
            
            return options;
        };
        
        // Get MapLibre style
        this.getMapLibreStyle = function() {
            return {
                version: 8,
                sources: {
                    'osm-tiles': {
                        type: 'raster',
                        tiles: [window.OpenSourceTileServer || 'https://tile.openstreetmap.org/{z}/{x}/{y}.png'],
                        tileSize: 256,
                        attribution: this.attribution
                    }
                },
                layers: [{
                    id: 'osm-tiles',
                    type: 'raster',
                    source: 'osm-tiles'
                }]
            };
        };
        
        // Get Google Maps map type
        this.getGoogleMapType = function() {
            switch (this.style) {
                case 'satellite':
                    return 'satellite';
                case 'hybrid':
                    return 'hybrid';
                case 'terrain':
                    return 'terrain';
                default:
                    return 'roadmap';
            }
        };
    };
    
    /**
     * Search Result Model
     */
    models.SearchResult = function(data) {
        data = data || {};
        
        this.location = new models.Location(data);
        this.relevance = data.relevance || 0;
        this.types = data.types || [];
        this.bounds = data.bounds || null;
        this.viewport = data.viewport || null;
        
        // Convert to object
        this.toObject = function() {
            return {
                location: this.location.toObject(),
                relevance: this.relevance,
                types: this.types,
                bounds: this.bounds,
                viewport: this.viewport
            };
        };
    };
    
    /**
     * Visit Record Model
     */
    models.VisitRecord = function(data) {
        data = data || {};
        
        this.id = data.id || null;
        this.userId = data.userId || null;
        this.clientId = data.clientId || null;
        this.siteId = data.siteId || null;
        this.location = new models.Location(data.location);
        this.timestamp = data.timestamp || new Date();
        this.duration = data.duration || 0; // minutes
        this.purpose = data.purpose || '';
        this.notes = data.notes || '';
        this.status = data.status || 'completed';
        this.accuracy = data.accuracy || 0; // meters
        this.isWithinGeofence = data.isWithinGeofence || false;
        this.distanceFromTarget = data.distanceFromTarget || 0; // meters
        
        // Validate visit record
        this.isValid = function() {
            return this.userId && 
                   (this.clientId || this.siteId) && 
                   this.location.isValid() &&
                   this.timestamp instanceof Date;
        };
        
        // Convert to object
        this.toObject = function() {
            return {
                id: this.id,
                userId: this.userId,
                clientId: this.clientId,
                siteId: this.siteId,
                location: this.location.toObject(),
                timestamp: this.timestamp,
                duration: this.duration,
                purpose: this.purpose,
                notes: this.notes,
                status: this.status,
                accuracy: this.accuracy,
                isWithinGeofence: this.isWithinGeofence,
                distanceFromTarget: this.distanceFromTarget
            };
        };
    };
    
    /**
     * Location Statistics Model
     */
    models.LocationStats = function(data) {
        data = data || {};
        
        this.totalLocations = data.totalLocations || 0;
        this.validLocations = data.validLocations || 0;
        this.invalidLocations = data.invalidLocations || 0;
        this.averageAccuracy = data.averageAccuracy || 0;
        this.bounds = data.bounds || null;
        this.centerPoint = data.centerPoint || null;
        this.lastUpdated = data.lastUpdated || new Date();
        
        // Calculate accuracy percentage
        this.getAccuracyPercentage = function() {
            if (this.totalLocations === 0) return 0;
            return Math.round((this.validLocations / this.totalLocations) * 100);
        };
        
        // Convert to object
        this.toObject = function() {
            return {
                totalLocations: this.totalLocations,
                validLocations: this.validLocations,
                invalidLocations: this.invalidLocations,
                averageAccuracy: this.averageAccuracy,
                bounds: this.bounds,
                centerPoint: this.centerPoint,
                lastUpdated: this.lastUpdated,
                accuracyPercentage: this.getAccuracyPercentage()
            };
        };
    };
    
    /**
     * Error Response Model
     */
    models.ErrorResponse = function(data) {
        data = data || {};
        
        this.code = data.code || 'UNKNOWN_ERROR';
        this.message = data.message || 'An unknown error occurred';
        this.details = data.details || '';
        this.timestamp = data.timestamp || new Date();
        this.retryable = data.retryable !== undefined ? data.retryable : true;
        
        // Convert to object
        this.toObject = function() {
            return {
                code: this.code,
                message: this.message,
                details: this.details,
                timestamp: this.timestamp,
                retryable: this.retryable
            };
        };
    };
    
    /**
     * Factory methods
     */
    models.createLocation = function(lat, lng, address) {
        return new models.Location({
            lat: lat,
            lng: lng,
            address: address
        });
    };
    
    models.createGeofence = function(centerLat, centerLng, radius, name) {
        return new models.Geofence({
            center: {
                lat: centerLat,
                lng: centerLng
            },
            radius: radius,
            name: name
        });
    };
    
    models.createMapOptions = function(centerLat, centerLng, zoom) {
        return new models.MapOptions({
            center: [centerLng, centerLat],
            zoom: zoom
        });
    };
    
    return models;
})();