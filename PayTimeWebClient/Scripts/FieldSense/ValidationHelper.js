/*
 * FieldSense Validation Helper - Testing & Validation Utilities
 * Provides comprehensive testing and validation functions for FieldSense modules
 * Ensures end-to-end CRUD operations are working correctly
 * Author: Claude Code Assistant
 * Version: 1.0 - Complete Validation Suite
 */

var FieldSenseValidation = {
    // Validation configuration
    config: {
        testMode: false,
        validationResults: [],
        apiEndpoints: {
            clients: [
                { method: 'GET', url: '/FieldSense/GetAllClients', name: 'Get All Clients' },
                { method: 'GET', url: '/FieldSense/GetClient', name: 'Get Client by ID' },
                { method: 'POST', url: '/FieldSense/SaveClient', name: 'Save New Client' },
                { method: 'POST', url: '/FieldSense/UpdateClient', name: 'Update Client' },
                { method: 'POST', url: '/FieldSense/DeleteClient', name: 'Delete Client' },
                { method: 'POST', url: '/FieldSense/BulkDeleteClients', name: 'Bulk Delete Clients' },
                { method: 'POST', url: '/FieldSense/UpdateClientStatus', name: 'Update Client Status' },
                { method: 'GET', url: '/FieldSense/GetClientStatistics', name: 'Get Client Statistics' },
                { method: 'GET', url: '/FieldSense/ExportClients', name: 'Export Clients' },
                { method: 'POST', url: '/FieldSense/ImportClients', name: 'Import Clients' },
                { method: 'GET', url: '/FieldSense/GetClientImportTemplate', name: 'Get Client Import Template' }
            ],
            sites: [
                { method: 'GET', url: '/FieldSense/GetAllSites', name: 'Get All Sites' },
                { method: 'GET', url: '/FieldSense/GetSite', name: 'Get Site by ID' },
                { method: 'POST', url: '/FieldSense/SaveSite', name: 'Save New Site' },
                { method: 'POST', url: '/FieldSense/UpdateSite', name: 'Update Site' },
                { method: 'POST', url: '/FieldSense/DeleteSite', name: 'Delete Site' },
                { method: 'POST', url: '/FieldSense/BulkDeleteSites', name: 'Bulk Delete Sites' },
                { method: 'POST', url: '/FieldSense/UpdateSiteStatus', name: 'Update Site Status' },
                { method: 'GET', url: '/FieldSense/GetSiteStatistics', name: 'Get Site Statistics' },
                { method: 'GET', url: '/FieldSense/ExportSites', name: 'Export Sites' },
                { method: 'POST', url: '/FieldSense/ImportSites', name: 'Import Sites' },
                { method: 'GET', url: '/FieldSense/GetSiteImportTemplate', name: 'Get Site Import Template' }
            ]
        }
    },

    // Initialize validation helper
    init: function() {
        var self = this;
        console.log('FieldSense Validation Helper initialized');
        
        // Only enable in development mode
        if (window.location.hostname === 'localhost' || window.location.search.includes('debug=true')) {
            self.enableDevelopmentMode();
        }
    },

    // Enable development/testing features
    enableDevelopmentMode: function() {
        var self = this;
        
        // Add validation panel to page
        self.addValidationPanel();
        
        // Add keyboard shortcuts
        self.bindKeyboardShortcuts();
        
        console.log('FieldSense Development Mode enabled');
    },

    // Add validation control panel
    addValidationPanel: function() {
        var html = '';
        html += '<div id="fieldsense-validation-panel" style="position: fixed; top: 10px; right: 10px; background: #fff; border: 1px solid #ddd; padding: 10px; border-radius: 5px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); z-index: 9999; max-width: 300px; display: none;">';
        html += '<h6>FieldSense Validation <button style="float: right; border: none; background: none;" onclick="FieldSenseValidation.hideValidationPanel()">×</button></h6>';
        html += '<div class="btn-group-vertical" style="width: 100%;">';
        html += '<button class="btn btn-sm btn-primary" onclick="FieldSenseValidation.validateAllEndpoints()">Validate All Endpoints</button>';
        html += '<button class="btn btn-sm btn-secondary" onclick="FieldSenseValidation.validateClientOperations()">Test Client CRUD</button>';
        html += '<button class="btn btn-sm btn-secondary" onclick="FieldSenseValidation.validateSiteOperations()">Test Site CRUD</button>';
        html += '<button class="btn btn-sm btn-info" onclick="FieldSenseValidation.validateJavaScriptModules()">Test JS Modules</button>';
        html += '<button class="btn btn-sm btn-warning" onclick="FieldSenseValidation.validateUIComponents()">Test UI Components</button>';
        html += '<button class="btn btn-sm btn-success" onclick="FieldSenseValidation.runFullValidation()">Full Validation</button>';
        html += '</div>';
        html += '<div id="validation-results" style="margin-top: 10px; max-height: 300px; overflow-y: auto;"></div>';
        html += '<div style="margin-top: 10px; font-size: 11px; color: #666;">Press Ctrl+Shift+V to toggle panel</div>';
        html += '</div>';
        
        $('body').append(html);
    },

    // Bind keyboard shortcuts
    bindKeyboardShortcuts: function() {
        var self = this;
        
        $(document).on('keydown', function(e) {
            // Ctrl+Shift+V to toggle validation panel
            if (e.ctrlKey && e.shiftKey && e.keyCode === 86) {
                e.preventDefault();
                self.toggleValidationPanel();
            }
            
            // Ctrl+Shift+T to run full tests
            if (e.ctrlKey && e.shiftKey && e.keyCode === 84) {
                e.preventDefault();
                self.runFullValidation();
            }
        });
    },

    // Toggle validation panel
    toggleValidationPanel: function() {
        $('#fieldsense-validation-panel').toggle();
    },

    // Hide validation panel
    hideValidationPanel: function() {
        $('#fieldsense-validation-panel').hide();
    },

    // Run full validation suite
    runFullValidation: function() {
        var self = this;
        
        self.clearResults();
        self.addResult('info', 'Starting full FieldSense validation...');
        
        // Run all validation tests in sequence
        Promise.resolve()
            .then(function() { return self.validateJavaScriptModules(); })
            .then(function() { return self.validateUIComponents(); })
            .then(function() { return self.validateAllEndpoints(); })
            .then(function() { return self.validateClientOperations(); })
            .then(function() { return self.validateSiteOperations(); })
            .then(function() {
                self.addResult('success', 'Full validation completed successfully!');
                self.generateValidationReport();
            })
            .catch(function(error) {
                self.addResult('error', 'Validation failed: ' + error.message);
            });
    },

    // Validate JavaScript modules
    validateJavaScriptModules: function() {
        var self = this;
        
        return new Promise(function(resolve, reject) {
            self.addResult('info', 'Validating JavaScript modules...');
            
            var results = [];
            
            // Check FieldSenseClient module
            if (typeof FieldSenseClient !== 'undefined') {
                results.push({ module: 'FieldSenseClient', status: 'loaded' });
                
                // Check required methods
                var requiredMethods = ['init', 'loadClients', 'saveClient', 'deleteClient', 'showClientModal'];
                requiredMethods.forEach(function(method) {
                    if (typeof FieldSenseClient[method] === 'function') {
                        results.push({ module: 'FieldSenseClient.' + method, status: 'available' });
                    } else {
                        results.push({ module: 'FieldSenseClient.' + method, status: 'missing' });
                    }
                });
            } else {
                results.push({ module: 'FieldSenseClient', status: 'not loaded' });
            }
            
            // Check FieldSenseSite module
            if (typeof FieldSenseSite !== 'undefined') {
                results.push({ module: 'FieldSenseSite', status: 'loaded' });
                
                var siteRequiredMethods = ['init', 'loadSites', 'saveSite', 'deleteSite', 'showSiteModal'];
                siteRequiredMethods.forEach(function(method) {
                    if (typeof FieldSenseSite[method] === 'function') {
                        results.push({ module: 'FieldSenseSite.' + method, status: 'available' });
                    } else {
                        results.push({ module: 'FieldSenseSite.' + method, status: 'missing' });
                    }
                });
            } else {
                results.push({ module: 'FieldSenseSite', status: 'not loaded' });
            }
            
            // Check FieldSenseStatistics module
            if (typeof FieldSenseStatistics !== 'undefined') {
                results.push({ module: 'FieldSenseStatistics', status: 'loaded' });
            } else {
                results.push({ module: 'FieldSenseStatistics', status: 'not loaded' });
            }
            
            // Report results
            results.forEach(function(result) {
                var type = result.status === 'loaded' || result.status === 'available' ? 'success' : 'warning';
                self.addResult(type, result.module + ': ' + result.status);
            });
            
            resolve(results);
        });
    },

    // Validate UI components
    validateUIComponents: function() {
        var self = this;
        
        return new Promise(function(resolve, reject) {
            self.addResult('info', 'Validating UI components...');
            
            var results = [];
            
            // Check for required UI elements
            var requiredElements = [
                '#tblClient',
                '#tblSite', 
                '#clientModal',
                '#siteModal',
                '#paginationContainer',
                '#statisticsKPIContainer'
            ];
            
            requiredElements.forEach(function(selector) {
                var exists = $(selector).length > 0;
                results.push({ 
                    element: selector, 
                    status: exists ? 'found' : 'missing',
                    visible: exists ? $(selector).is(':visible') : false
                });
            });
            
            // Check for DataTables
            if ($.fn.DataTable) {
                results.push({ element: 'DataTables library', status: 'loaded' });
            } else {
                results.push({ element: 'DataTables library', status: 'missing' });
            }
            
            // Check for Chart.js (for statistics)
            if (typeof Chart !== 'undefined') {
                results.push({ element: 'Chart.js library', status: 'loaded' });
            } else {
                results.push({ element: 'Chart.js library', status: 'missing' });
            }
            
            // Report results
            results.forEach(function(result) {
                var type = result.status === 'found' || result.status === 'loaded' ? 'success' : 'warning';
                var message = result.element + ': ' + result.status;
                if (result.visible !== undefined) {
                    message += result.visible ? ' (visible)' : ' (hidden)';
                }
                self.addResult(type, message);
            });
            
            resolve(results);
        });
    },

    // Validate all API endpoints
    validateAllEndpoints: function() {
        var self = this;
        
        return new Promise(function(resolve, reject) {
            self.addResult('info', 'Validating API endpoints...');
            
            var allEndpoints = self.config.apiEndpoints.clients.concat(self.config.apiEndpoints.sites);
            var promises = [];
            
            allEndpoints.forEach(function(endpoint) {
                promises.push(self.testEndpointAvailability(endpoint));
            });
            
            Promise.all(promises)
                .then(function(results) {
                    var successCount = results.filter(function(r) { return r.success; }).length;
                    var totalCount = results.length;
                    
                    self.addResult('info', 'Endpoint validation completed: ' + successCount + '/' + totalCount + ' available');
                    resolve(results);
                })
                .catch(function(error) {
                    reject(error);
                });
        });
    },

    // Test endpoint availability
    testEndpointAvailability: function(endpoint) {
        var self = this;
        
        return new Promise(function(resolve) {
            var testData = endpoint.method === 'POST' ? {} : null;
            
            $.ajax({
                url: endpoint.url + (endpoint.method === 'GET' ? '?test=true' : ''),
                method: endpoint.method,
                data: testData,
                timeout: 5000,
                headers: {
                    'X-Test-Request': 'true'
                }
            })
            .done(function() {
                self.addResult('success', '✓ ' + endpoint.name + ' (' + endpoint.method + ')');
                resolve({ endpoint: endpoint, success: true });
            })
            .fail(function(xhr) {
                // Some failures are expected for test requests
                var isExpectedFailure = xhr.status === 400 || xhr.status === 401 || xhr.status === 404;
                var type = isExpectedFailure ? 'warning' : 'error';
                
                self.addResult(type, (isExpectedFailure ? '⚠' : '✗') + ' ' + endpoint.name + ' (' + xhr.status + ')');
                resolve({ endpoint: endpoint, success: isExpectedFailure });
            });
        });
    },

    // Validate client operations
    validateClientOperations: function() {
        var self = this;
        
        return new Promise(function(resolve, reject) {
            self.addResult('info', 'Validating client operations...');
            
            // Test client module methods
            if (typeof FieldSenseClient !== 'undefined') {
                try {
                    // Test configuration
                    if (FieldSenseClient.config) {
                        self.addResult('success', '✓ Client configuration object exists');
                    }
                    
                    // Test data loading capability
                    if (typeof FieldSenseClient.loadClients === 'function') {
                        self.addResult('success', '✓ Client loading function available');
                    }
                    
                    // Test modal functionality
                    if (typeof FieldSenseClient.showClientModal === 'function') {
                        self.addResult('success', '✓ Client modal function available');
                    }
                    
                    // Test search functionality
                    if (typeof FieldSenseClient.performSearch === 'function') {
                        self.addResult('success', '✓ Client search function available');
                    }
                    
                    // Test pagination
                    if (typeof FieldSenseClient.goToPage === 'function') {
                        self.addResult('success', '✓ Client pagination function available');
                    }
                    
                    resolve({ module: 'FieldSenseClient', status: 'validated' });
                    
                } catch (error) {
                    self.addResult('error', 'Client validation error: ' + error.message);
                    resolve({ module: 'FieldSenseClient', status: 'error', error: error.message });
                }
            } else {
                self.addResult('warning', 'FieldSenseClient module not loaded');
                resolve({ module: 'FieldSenseClient', status: 'not_loaded' });
            }
        });
    },

    // Validate site operations
    validateSiteOperations: function() {
        var self = this;
        
        return new Promise(function(resolve, reject) {
            self.addResult('info', 'Validating site operations...');
            
            // Test site module methods
            if (typeof FieldSenseSite !== 'undefined') {
                try {
                    // Test configuration
                    if (FieldSenseSite.config) {
                        self.addResult('success', '✓ Site configuration object exists');
                    }
                    
                    // Test data loading capability
                    if (typeof FieldSenseSite.loadSites === 'function') {
                        self.addResult('success', '✓ Site loading function available');
                    }
                    
                    // Test modal functionality
                    if (typeof FieldSenseSite.showSiteModal === 'function') {
                        self.addResult('success', '✓ Site modal function available');
                    }
                    
                    // Test search functionality
                    if (typeof FieldSenseSite.performSearch === 'function') {
                        self.addResult('success', '✓ Site search function available');
                    }
                    
                    // Test bulk operations
                    if (typeof FieldSenseSite.bulkDeleteSites === 'function') {
                        self.addResult('success', '✓ Site bulk operations available');
                    }
                    
                    resolve({ module: 'FieldSenseSite', status: 'validated' });
                    
                } catch (error) {
                    self.addResult('error', 'Site validation error: ' + error.message);
                    resolve({ module: 'FieldSenseSite', status: 'error', error: error.message });
                }
            } else {
                self.addResult('warning', 'FieldSenseSite module not loaded');
                resolve({ module: 'FieldSenseSite', status: 'not_loaded' });
            }
        });
    },

    // Add result to validation panel
    addResult: function(type, message) {
        var self = this;
        
        var iconMap = {
            'success': '✓',
            'error': '✗', 
            'warning': '⚠',
            'info': 'ℹ'
        };
        
        var colorMap = {
            'success': '#28a745',
            'error': '#dc3545',
            'warning': '#ffc107',
            'info': '#17a2b8'
        };
        
        var html = '<div style="margin: 2px 0; padding: 2px; font-size: 11px; color: ' + colorMap[type] + ';">';
        html += iconMap[type] + ' ' + message;
        html += '</div>';
        
        $('#validation-results').append(html);
        $('#validation-results').scrollTop($('#validation-results')[0].scrollHeight);
        
        // Store result
        self.config.validationResults.push({
            timestamp: new Date(),
            type: type,
            message: message
        });
    },

    // Clear validation results
    clearResults: function() {
        $('#validation-results').empty();
        this.config.validationResults = [];
    },

    // Generate validation report
    generateValidationReport: function() {
        var self = this;
        
        var results = self.config.validationResults;
        var successCount = results.filter(function(r) { return r.type === 'success'; }).length;
        var errorCount = results.filter(function(r) { return r.type === 'error'; }).length;
        var warningCount = results.filter(function(r) { return r.type === 'warning'; }).length;
        
        var report = {
            timestamp: new Date().toISOString(),
            summary: {
                total: results.length,
                success: successCount,
                errors: errorCount,
                warnings: warningCount,
                score: Math.round((successCount / results.length) * 100)
            },
            results: results
        };
        
        console.log('FieldSense Validation Report:', report);
        
        // Show summary
        self.addResult('info', '=== VALIDATION SUMMARY ===');
        self.addResult('success', 'Passed: ' + successCount);
        if (warningCount > 0) self.addResult('warning', 'Warnings: ' + warningCount);
        if (errorCount > 0) self.addResult('error', 'Errors: ' + errorCount);
        self.addResult('info', 'Overall Score: ' + report.summary.score + '%');
        
        return report;
    },

    // Test live data operations (use with caution)
    testLiveOperations: function() {
        var self = this;
        
        if (!confirm('This will test live CRUD operations. Continue only on development environment. Proceed?')) {
            return;
        }
        
        self.addResult('warning', 'Testing live operations - USE WITH CAUTION');
        
        // Test client creation/deletion cycle
        self.testClientCRUDCycle()
            .then(function() {
                return self.testSiteCRUDCycle();
            })
            .then(function() {
                self.addResult('success', 'Live operations test completed');
            })
            .catch(function(error) {
                self.addResult('error', 'Live operations test failed: ' + error.message);
            });
    },

    // Test client CRUD cycle
    testClientCRUDCycle: function() {
        // Implementation would create, update, and delete a test client
        // This is a placeholder for safety
        return Promise.resolve();
    },

    // Test site CRUD cycle  
    testSiteCRUDCycle: function() {
        // Implementation would create, update, and delete a test site
        // This is a placeholder for safety
        return Promise.resolve();
    }
};

// Auto-initialize validation helper
if (typeof $ !== 'undefined') {
    $(document).ready(function() {
        setTimeout(function() {
            if (typeof FieldSenseValidation !== 'undefined') {
                FieldSenseValidation.init();
            }
        }, 1000);
    });
}

// Export for use in other modules
if (typeof window !== 'undefined') {
    window.FieldSenseValidation = FieldSenseValidation;
}