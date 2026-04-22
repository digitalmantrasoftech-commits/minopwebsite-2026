/*
 * FieldSense Configuration Management
 * Simple, clean implementation for configuration management with dropdown cascading
 */

var FieldSenseConfiguration = {
    // Configuration
    config: {
        token: '',
        apiUrl: '',
        fieldSenseApiUrl: '',
        roleId: '',
        companyId: '',
        chunkSize: 25
    },

    // Dropdown instances
    dropdowns: {
        company: null,
        department: null,
        employee: null
    },

    // Initialize the application
    init: function() {
        var self = this;
        
        try {
            // Initialize department-employee mapping storage
            self.departmentEmployeeMapping = {};
            
            // Load configuration from global variable
            if (typeof window.FieldSenseConfig !== 'undefined') {
                self.config.token = window.FieldSenseConfig.token;
                self.config.apiUrl = window.FieldSenseConfig.apiUrl;
                self.config.fieldSenseApiUrl = window.FieldSenseConfig.fieldSenseApiUrl;
                self.config.roleId = window.FieldSenseConfig.roleId;
                self.config.companyId = window.FieldSenseConfig.companyId;
                self.config.chunkSize = window.FieldSenseConfig.chunkSize;
            }
            
            console.log('FieldSenseConfiguration initialized');
            
            // Initialize dropdowns
            self.initializeDropdowns();
            
            // Setup event handlers
            self.bindEvents();
            
        } catch (error) {
            console.error('Error initializing FieldSenseConfiguration:', error);
            self.showError('Failed to initialize configuration');
        }
    },

    // Initialize dropdown components
    initializeDropdowns: function() {
        var self = this;
        
        // Initialize company dropdown when window loads
        $(window).on("load", function () {
            self.dropdowns.company = new Select2Dropdown({
                dropdownId: "Company_Single_Dropdown",
                apiUrl: self.config.apiUrl + 'DataDropDownOptimize/GetCompanyDropDownPaginated',
                dataKey: "Id",
                labelKey: "Name",
                token: self.config.token,
                chunkSize: Number(self.config.chunkSize),
                extraParams: { 
                    RoleId: self.config.roleId, 
                    CompanyID: self.config.companyId 
                }
            });
        });

        // Initialize department dropdown as single-select
        self.dropdowns.department = new Select2Dropdown({
            dropdownId: "Department_Single_Dropdown",
            apiUrl: self.config.apiUrl + "DataDropDownOptimize/GetDepartmentDropDown_MulBranch_Paginated",
            dataKey: "Id",
            labelKey: "Name",
            token: self.config.token,
            chunkSize: Number(self.config.chunkSize),
            extraParams: {}
        });

        // Initialize employee dropdown
        self.dropdowns.employee = new ServerVirtualDropdown({
            apiUrl : self.config.fieldSenseApiUrl + "Configuration/GetEmployeesPaginated",
            tokan: self.config.token,
            dropdownId: "ExcludeEmployee_Multiple_Dropdown",
            selectAll: false,
            chunkSize: Number(self.config.chunkSize),
            dataKey: "id",
            labelKey: "name"
        });
    },

    // Bind event handlers
    bindEvents: function() {
        var self = this;

        // Save button
        $("#btnSave").click(function () {
            self.save();
        });

        // Reset button
        $("#btnReset").click(function() {
            self.reset();
        });

        // Company dropdown change - SIMPLIFIED
        $("#Company_Single_Dropdown").change(function () {
            var companyId = $(this).val();
            if (companyId) {
                self.clearForm();
                // Load existing configuration for the company
                // Fill departments
                self.fillDepartment();
                self.loadConfigurationForCompany(companyId);
            } else {
                // Company deselected - reset everything
                self.resetDependentDropdowns();
                self.clearForm();
            }
        });

        // Department dropdown change - now single-select
        $("#Department_Single_Dropdown").change(function () {
            self.handleDepartmentChange();
        });
    },

    // Fill department dropdown based on company selection
    fillDepartment: function() {
        var self = this;
        
        if (self.config.roleId == 1 || self.config.roleId == 6805) {
            if (self.dropdowns.department) {
                self.dropdowns.department.extraParams = { 
                    CompanyID: $("#Company_Single_Dropdown").val(), 
                    SelectAll: 1 
                };
                // Re-initialize the dropdown with new company
                self.dropdowns.department = new Select2Dropdown({
                    dropdownId: "Department_Single_Dropdown",
                    apiUrl: self.config.apiUrl + "DataDropDownOptimize/GetDepartmentDropDown_MulBranch_Paginated",
                    dataKey: "Id",
                    labelKey: "Name",
                    token: self.config.token,
                    chunkSize: Number(self.config.chunkSize),
                    extraParams: {
                        CompanyID: $("#Company_Single_Dropdown").val(),
                        SelectAll: 1
                    }
                });
            }
        }
    },


    // Handle department dropdown change - SIMPLIFIED for single-select
    handleDepartmentChange: function() {
        var self = this;
        
        let dID = $("#Department_Single_Dropdown").val();
        
        // Check if department is selected
        if (!dID || dID === "") {
            // No department selected - reset employee dropdown and checkbox
            console.log('No department selected, resetting configuration');
            self.resetMultipleDropdown('ExcludeEmployee_Multiple_Dropdown');
            $("#EmpCreateTaskPermission").prop("checked", false);
            return; // Exit early - don't load employees
        }
        
        // Department is selected, load configuration for this department
        console.log('Department changed, loading configuration for department:', dID);
        
        // Convert department ID to integer
        var selectedDepartmentId = parseInt(dID, 10);
        
        if (isNaN(selectedDepartmentId)) {
            console.log('Invalid department ID');
            self.resetMultipleDropdown('ExcludeEmployee_Multiple_Dropdown');
            $("#EmpCreateTaskPermission").prop("checked", false);
            return;
        }

        // Update employee dropdown configuration for new department filter
        self.dropdowns.employee.data = { 
            CompanyID: $("#Company_Single_Dropdown").val(), 
            DepartmentIds: dID, // Single department ID as string
            SelectAll: 0, 
            SelectAllSearchTerm: "", 
            IsActive: 1
        };
        
        // Load department-specific configuration (checkbox and excluded employees)
        self.loadDepartmentConfiguration(selectedDepartmentId);
    },

    // Save configuration - SIMPLIFIED for single department
    save: function() {
        var self = this;

        let IsSelectAllDepartment = 0;
        let SelectAllDepartmentSearchTerm = "";

        // Get single department ID
        let DepartmentId = $("#Department_Single_Dropdown").val();
        let DepartmentIds = DepartmentId || null; // Keep as single value, not comma-separated

        // Get employee IDs
        let EmployeeIds = $("#ExcludeEmployee_Multiple_Dropdown").val();
        if (EmployeeIds != null) {
            EmployeeIds = EmployeeIds.join(",");
        } else {
            EmployeeIds = null;
        }

        let configurationData = {
            CompanyId: $("#Company_Single_Dropdown").val(),
            DepartmentIds: DepartmentIds, // Single department ID
            EmployeeIds: EmployeeIds,
            IsSelectAllDepartment: IsSelectAllDepartment,
            SelectAllDepartmentSearchTerm: SelectAllDepartmentSearchTerm,
            IsExcludeEmployees: (EmployeeIds != null && EmployeeIds !== ""),
            IsEmployeeCreateTask: $("#EmpCreateTaskPermission").prop("checked")
        };

        var url = '/FieldSense/SaveConfiguration';

        // Show loading
        $('#btnSaveText').html('<i class="fas fa-spinner fa-spin"></i> Saving...');

        $.ajax({
            url: url,
            type: 'POST',
            data: JSON.stringify(configurationData),
            contentType: 'application/json',
            headers: {
                'Authorization': 'Bearer ' + self.config.token
            },
            success: function (response) {
                $('#btnSaveText').text('Save');
                
                if (response && response.success) {
                    self.showSuccess(response.message);
                    // Reset form after successful save - SIMPLE

                        self.reset();

                } else {
                    self.showError(response.message || 'Failed to save configuration');
                }
            },
            error: function (xhr, status, error) {
                $('#btnSaveText').text('Save');
                console.error('Error saving configuration:', xhr, status, error);
                self.showError('An error occurred while saving the configuration');
            }
        });
    },

    // Load configuration for selected company
    loadConfigurationForCompany: function(companyId) {
        var self = this;
        
        $.ajax({
            url: self.config.fieldSenseApiUrl + 'Configuration/GetConfiguration?companyId=' + companyId,
            type: 'GET',
            headers: {
                'Authorization': 'Bearer ' + self.config.token,
            },
            ContentType: 'application/json',
            success: function(response) {
                if (response && response.success && response.data) {
                    console.log('Configuration loaded for company:', companyId, response.data);
                    
                    // Store department-employee mapping for cascading dropdown functionality
                    self.departmentEmployeeMapping = response.data.departmentEmployeeMapping || {};
                    console.log('Department-Employee mapping stored:', Object.keys(self.departmentEmployeeMapping).length, 'departments');
                    
                    self.populateFormWithConfiguration(response.data);
                } else {
                    console.log('No existing configuration found for company:', companyId);
                    // Reset mapping when no configuration
                    self.departmentEmployeeMapping = {};
                    self.clearForm();
                }
            },
            error: function(xhr, status, error) {
                console.error('Error loading configuration:', xhr, status, error);
                self.showError('Failed to load configuration for company');
                self.clearForm();
            }
        });
    },

    // Populate form with loaded configuration
    populateFormWithConfiguration: function(config) {
        var self = this;
        
        try {
            // Set checkboxes based on configuration
            $("#EmpCreateTaskPermission").prop("checked", config.isEmployeeCreateTask || false);
            
            // Extract department ID and employee IDs from mapping
            if (config.departmentEmployeeMapping && Object.keys(config.departmentEmployeeMapping).length > 0) {
                // Get first department ID from mapping keys (should only be one for single-select)
                var departmentIds = Object.keys(config.departmentEmployeeMapping)
                    .filter(function(key) { return !isNaN(parseInt(key)); })
                    .map(function(key) { return parseInt(key); });
                
                // Should only have one department
                var departmentId = departmentIds.length > 0 ? departmentIds[0] : null;
                
                // Get employee IDs for this department (these are the excluded employees)
                var employeeIds = [];
                if (departmentId && config.departmentEmployeeMapping[departmentId]) {
                    employeeIds = config.departmentEmployeeMapping[departmentId];
                }
                
                // Convert to string for existing preselect logic
                var departmentIdString = departmentId ? departmentId.toString() : '';
                var employeeIdsString = employeeIds.join(',');
                
                // Use existing preselection logic
                if (departmentIdString) {
                    self.preselectDepartment(departmentIdString, employeeIdsString);
                }
            }
            
            console.log('Form populated with configuration');
        } catch (error) {
            console.error('Error populating form:', error);
            self.showError('Failed to populate form with configuration');
        }
    },

    // Pre-select department in dropdown - SIMPLE AJAX pattern for single-select
    preselectDepartment: function(departmentIdString , employeeIdString) {
        var self = this;
        
        try {
            console.log('Preselecting department:', departmentIdString);
            
            var departmentId = parseInt(departmentIdString.trim());
            
            if (!isNaN(departmentId) && self.dropdowns.department) {
                // Simple AJAX call to get department details
                $.ajax({
                    url: self.config.fieldSenseApiUrl + 'Configuration/GetSelectedDepartmentsByIds',
                    type: 'POST',
                    headers: {
                        'Authorization': 'Bearer ' + self.config.token,
                        'Content-Type': 'application/json'
                    },
                    data: JSON.stringify({ ids: [departmentId] }),  // Send as array for API compatibility
                    async: false,
                    success: function(response) {
                        if (response && response.success && response.data && response.data.length > 0) {
                            // Get the first (and should be only) department
                            var dept = response.data[0];
                            var deptValue = dept.Id || dept.id;
                            var deptText = dept.Name || dept.name;
                            
                            // Set the department in Select2 dropdown
                            var newOption = new Option(deptText, deptValue, true, true);
                            $('#Department_Single_Dropdown').append(newOption).trigger('change');
                            
                            // If there are employees to preselect, do it after department is set
                            if (employeeIdString) {
                                setTimeout(function() {
                                    self.preselectEmployees(employeeIdString, [departmentId]);
                                }, 100);
                            }

                            console.log('Department preselected:', deptText);
                        }
                    },
                    error: function(xhr, status, error) {
                        console.error('Error preselecting departments:', error);
                        // Just log error, dropdown already initialized
                    }
                });
            }
        }
        catch (error) {
            console.error('Error in preselectDepartment:', error);
        }
    },

    // Pre-select employees in dropdown with department filtering
    preselectEmployees: function(employeeIdsString, selectedDepartmentIds) {
        var self = this;
        
        try {
            console.log('Preselecting employees:', employeeIdsString, 'for departments:', selectedDepartmentIds);
            
            var employeeIds = employeeIdsString.split(',')
                .map(function(id) { return parseInt(id.trim()); })
                .filter(function(id) { return !isNaN(id); });
            
            // Filter employees based on selected departments using mapping
            var validEmployeeIds = self.filterEmployeesByDepartments(employeeIds, selectedDepartmentIds);
            console.log('Filtered employees:', validEmployeeIds.length, 'out of', employeeIds.length, 'original employees');
            
            if (validEmployeeIds.length > 0 && self.dropdowns.employee) {
                // Simple AJAX call to get employee details
                $.ajax({
                    url: self.config.fieldSenseApiUrl + 'Configuration/GetSelectedEmployeesByIds',
                    type: 'POST',
                    headers: {
                        'Authorization': 'Bearer ' + self.config.token,
                        'Content-Type': 'application/json'
                    },
                    data: JSON.stringify({ ids: validEmployeeIds }),
                    async:false,
                    success: function(response) {
                        if (response && response.success && response.data && response.data.length > 0) {
                            // Format data for select property
                            var arr = [];
                            for (var i = 0; i < response.data.length; i++) {
                                arr.push({ 
                                    key: response.data[i].id || response.data[i].Id, 
                                    value: response.data[i].name || response.data[i].Name 
                                });
                            }
                            let dID = $("#Department_Single_Dropdown").val();

                            // Check if department is selected
                            if (!dID || dID === "") {
                                // No department selected - reset employee dropdown and return
                                console.log('No department selected, resetting employee dropdown');
                                self.resetMultipleDropdown('ExcludeEmployee_Multiple_Dropdown');
                                return; // Exit early - don't load employees
                            }
                            // Set select property and load
                            self.dropdowns.employee.data = {
                                CompanyID: $("#Company_Single_Dropdown").val(),
                                DepartmentIds: dID,  // Single department ID
                                SelectAll: 0,
                                SelectAllSearchTerm: "",
                                IsActive: 1
                            };
                            self.dropdowns.employee.select = arr;
                            self.dropdowns.employee.loadInitialData();
                            
                            console.log('Employees preselected:', arr.length);
                        }
                    },
                    error: function(xhr, status, error) {
                        console.error('Error preselecting employees:', error);
                        // Just load without preselection on error
                        self.dropdowns.employee.loadInitialData();
                    }
                });
            }
        }
        catch (error) {
            console.error('Error in preselectEmployees:', error);
        }
    },



    // Reset dependent dropdowns - SIMPLE
    resetDependentDropdowns: function() {
        this.resetSingleDropdown('Department_Single_Dropdown');
        this.resetMultipleDropdown('ExcludeEmployee_Multiple_Dropdown');
    },

    // Clear form to default state
    clearForm: function() {
        $("#EmpCreateTaskPermission").prop("checked", false);
        this.resetDependentDropdowns();
    },

    // Reset form
    reset: function() {
        this.resetSingleDropdown("Company_Single_Dropdown");
        this.resetDependentDropdowns();
        $("#EmpCreateTaskPermission").prop("checked", false);
    },

    // Reset multiple dropdown
    resetMultipleDropdown: function(id) {
        $(`#${id}`).empty();
        $(`#${id} option:selected`).prop("selected", false);
        $(`#${id}`).multiselect('rebuild');
    },

    // Reset single dropdown
    resetSingleDropdown: function(id) {
        $(`#${id}`).empty();
        $(`#${id}`).val(null).trigger('change');
    },

    // Show success message
    showSuccess: function(message) {
        if (typeof toastr !== 'undefined') {
            toastr.success(message);
        } else {
            alert(message);
        }
    },

    // Show error message
    showError: function(message) {
        if (typeof toastr !== 'undefined') {
            toastr.error(message);
        } else {
            alert('Error: ' + message);
        }
    },

    // Show info message
    showInfo: function(message) {
        if (typeof toastr !== 'undefined') {
            toastr.info(message);
        } else {
            alert(message);
        }
    },

    // Helper function to filter employees based on selected departments
    filterEmployeesByDepartments: function(employeeIds, departmentIds) {
        var self = this;
        
        if (!employeeIds || employeeIds.length === 0) {
            return [];
        }
        
        if (!departmentIds || departmentIds.length === 0) {
            // No departments selected, return all employees (for backward compatibility)
            return employeeIds;
        }
        
        if (!self.departmentEmployeeMapping || Object.keys(self.departmentEmployeeMapping).length === 0) {
            // No mapping available, return all employees (fallback)
            console.warn('No department-employee mapping available, returning all employees');
            return employeeIds;
        }
        
        // Collect all employee IDs from selected departments
        var validEmployeeIds = [];
        for (var i = 0; i < departmentIds.length; i++) {
            var deptId = departmentIds[i];
            var deptEmployees = self.departmentEmployeeMapping[deptId];
            if (deptEmployees && deptEmployees.length > 0) {
                validEmployeeIds = validEmployeeIds.concat(deptEmployees);
            }
        }
        
        // Remove duplicates
        validEmployeeIds = validEmployeeIds.filter(function(value, index, self) {
            return self.indexOf(value) === index;
        });
        
        // Filter original employee list to only include those in valid departments
        var filteredEmployeeIds = employeeIds.filter(function(empId) {
            return validEmployeeIds.indexOf(empId) !== -1;
        });
        
        console.log('Employee filtering:', {
            originalCount: employeeIds.length,
            selectedDepartments: departmentIds.length,
            validEmployeesInDepts: validEmployeeIds.length,
            filteredCount: filteredEmployeeIds.length
        });
        
        return filteredEmployeeIds;
    },

    // Load configuration for a specific department
    loadDepartmentConfiguration: function(departmentId) {
        var self = this;
        
        var companyId = $("#Company_Single_Dropdown").val();
        if (!companyId || !departmentId) {
            console.log('Company or department not selected');
            return;
        }
        
        console.log('Loading configuration for department:', departmentId);
        
        $.ajax({
            url: self.config.fieldSenseApiUrl + 'Configuration/GetDepartmentConfiguration',
            type: 'GET',
            headers: {
                'Authorization': 'Bearer ' + self.config.token
            },
            data: {
                companyId: companyId,
                departmentId: departmentId
            },
            success: function(response) {
                if (response && response.success && response.data) {
                    console.log('Department configuration loaded:', response.data);
                    
                    // Update "Employee Can Create Task" checkbox
                    $("#EmpCreateTaskPermission").prop("checked", response.data.isEmployeeCreateTask || false);
                    
                    // If there are excluded employees, load them
                    if (response.data.excludedEmployeeIds && response.data.excludedEmployeeIds.length > 0) {
                        // Load and preselect the excluded employees
                        self.preselectEmployeesDirectly(response.data.excludedEmployeeIds, departmentId);
                    } else {
                        // No excluded employees - just load the dropdown
                        self.dropdowns.employee.select = [];
                        self.dropdowns.employee.loadInitialData();
                    }
                } else {
                    console.log('No configuration found for department, using defaults');
                    // Set defaults when no configuration exists
                    $("#EmpCreateTaskPermission").prop("checked", false);
                    self.dropdowns.employee.select = [];
                    self.dropdowns.employee.loadInitialData();
                }
            },
            error: function(xhr, status, error) {
                console.error('Error loading department configuration:', error);
                // Set defaults on error
                $("#EmpCreateTaskPermission").prop("checked", false);
                self.dropdowns.employee.select = [];
                self.dropdowns.employee.loadInitialData();
            }
        });
    },

    // Preselect employees directly by IDs
    preselectEmployeesDirectly: function(employeeIds, departmentId) {
        var self = this;
        
        if (!employeeIds || employeeIds.length === 0) {
            self.dropdowns.employee.select = [];
            self.dropdowns.employee.loadInitialData();
            return;
        }
        
        console.log('Preselecting employees directly:', employeeIds.length, 'employees');
        
        // Get employee details for preselection
        $.ajax({
            url: self.config.fieldSenseApiUrl + 'Configuration/GetSelectedEmployeesByIds',
            type: 'POST',
            headers: {
                'Authorization': 'Bearer ' + self.config.token,
                'Content-Type': 'application/json'
            },
            data: JSON.stringify({ ids: employeeIds }),
            success: function(response) {
                if (response && response.success && response.data && response.data.length > 0) {
                    // Format data for select property
                    var arr = [];
                    for (var i = 0; i < response.data.length; i++) {
                        arr.push({ 
                            key: response.data[i].id || response.data[i].Id, 
                            value: response.data[i].name || response.data[i].Name 
                        });
                    }
                    
                    // Update employee dropdown data for the department
                    self.dropdowns.employee.data = {
                        CompanyID: $("#Company_Single_Dropdown").val(),
                        DepartmentIds: departmentId.toString(),
                        SelectAll: 0,
                        SelectAllSearchTerm: "",
                        IsActive: 1
                    };
                    
                    // Set select property and load
                    self.dropdowns.employee.select = arr;
                    self.dropdowns.employee.loadInitialData();
                    
                    console.log('Employees preselected:', arr.length);
                } else {
                    // No employees found, just load empty
                    self.dropdowns.employee.select = [];
                    self.dropdowns.employee.loadInitialData();
                }
            },
            error: function(xhr, status, error) {
                console.error('Error preselecting employees:', error);
                // Just load without preselection on error
                self.dropdowns.employee.select = [];
                self.dropdowns.employee.loadInitialData();
            }
        });
    },

    // Load excluded employees for selected departments from configuration
    loadExcludedEmployeesForDepartments: function(selectedDepartmentIds) {
        var self = this;
        
        console.log('Loading excluded employees for departments:', selectedDepartmentIds.length);
        
        // Call API to get ALL excluded employees for selected departments
        $.ajax({
            url: self.config.fieldSenseApiUrl + 'Configuration/GetSelectedEmployeesByDepartments',
            type: 'POST',
            headers: {
                'Authorization': 'Bearer ' + self.config.token,
                'Content-Type': 'application/json'
            },
            data: JSON.stringify({
                companyId: $("#Company_Single_Dropdown").val(),
                departmentIds: selectedDepartmentIds  // Only send department IDs
            }),
            success: function(response) {
                if (response && response.success && response.data && response.data.length > 0) {
                    // Format data for select property
                    var arr = [];
                    for (var i = 0; i < response.data.length; i++) {
                        arr.push({ 
                            key: response.data[i].id || response.data[i].Id, 
                            value: response.data[i].name || response.data[i].Name 
                        });
                    }
                    
                    // Set select property and load
                    self.dropdowns.employee.select = arr;
                    self.dropdowns.employee.loadInitialData();
                    
                    console.log('Loaded excluded employees:', arr.length, 'employees for', selectedDepartmentIds.length, 'departments');
                } else {
                    // No excluded employees found for these departments
                    console.log('No excluded employees found for selected departments');
                    self.dropdowns.employee.select = [];
                    self.dropdowns.employee.loadInitialData();
                }
            },
            error: function(xhr, status, error) {
                console.error('Error loading excluded employees:', error);
                // Fallback: load all employees without preselection
                self.dropdowns.employee.select = [];
                self.dropdowns.employee.loadInitialData();
            }
        });
    }
};

// Initialize when document is ready
$(document).ready(function() {
    FieldSenseConfiguration.init();
});