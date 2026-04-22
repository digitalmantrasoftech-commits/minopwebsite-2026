//class SimpleDropdown {
//    constructor({ dropdownId, apiUrl, chunkSize = 50, dataKey, labelKey, data = {}, token }) {
//        this.dropdownId = dropdownId;
//        this.apiUrl = apiUrl;
//        this.chunkSize = chunkSize;
//        this.dataKey = dataKey;
//        this.labelKey = labelKey;
//        this.page = 1;
//        this.totalRecords = 0;
//        this.searchTerm = "";
//        this.data = data;
//        this.token = token;
//        this.isLoading = false;

//        this.initDropdown();
//        this.bindEvents();
//    }

//    initDropdown() {
//        const dropdown = $(`#${this.dropdownId}`);
//        dropdown.empty();
//        dropdown.append(new Option("Please Select", "")); // Default placeholder
//        this.loadMoreData();
//    }

//    bindEvents() {
//        const dropdown = $(`#${this.dropdownId}`);

//        // Handle search
//        dropdown.on("input", this.debounce((e) => this.handleSearch(e), 200));

//        // Handle scroll
//        dropdown.on("scroll", (e) => this.handleScroll(e));

//        // Handle selection
//        dropdown.on("change", (e) => this.handleSelection(e));
//    }

//    async handleScroll(event) {
        
//        const container = event.target;
//        if (this.isLoading || this.page * this.chunkSize >= this.totalRecords) return; // 🔥 Prevent multiple requests

//        const scrollTop = container.scrollTop;
//        const containerHeight = container.offsetHeight;
//        const scrollHeight = container.scrollHeight;

//        if (scrollTop + containerHeight >= scrollHeight - this.chunkSize) {
//            await this.loadMoreData();
//        }
//    }

//    async handleSearch(event) {
//        this.searchTerm = $(event.target).val().trim();
//        this.page = 1;
//        this.totalRecords = 0;
//        await this.loadMoreData();
//    }

//    handleSelection(event) {
//        const selectedValue = $(event.target).val();
//        console.log("Selected Value:", selectedValue);
//    }

//    async loadMoreData() {
//        if (this.isLoading) return;
//        this.isLoading = true;

//        const requestData = {
//            page: this.page,
//            pageSize: this.chunkSize,
//            searchTerm: this.searchTerm,
//            ...this.data
//        };

//        try {
//            const response = await $.ajax({
//                url: this.apiUrl,
//                type: 'POST',
//                data: JSON.stringify(requestData),
//                contentType: "application/json",
//                headers: { 'Authorization': this.token }
//            });

//            if (response.Result && response.Result.data) {
//                if (this.page === 1) {
//                    this.totalRecords = response.Result.totalRecords;
//                    $(`#${this.dropdownId}`).empty();
//                    $(`#${this.dropdownId}`).append(new Option("Please Select", ""));
//                }

//                this.appendData(response.Result.data);
//                this.page++;
//            }
//        } catch (error) {
//            console.error("Error fetching data:", error);
//        } finally {
//            this.isLoading = false;
//        }
//    }

//    appendData(items) {
//        const dropdown = $(`#${this.dropdownId}`);
//        for (let item of items) {
//            dropdown.append(new Option(item[this.labelKey], item[this.dataKey]));
//        }
//    }

//    resetDropdown() {
//        this.page = 1;
//        this.totalRecords = 0;
//        this.searchTerm = "";
//        this.initDropdown();
//    }

//    debounce(func, delay) {
//        let timer;
//        return function (...args) {
//            clearTimeout(timer);
//            timer = setTimeout(() => func.apply(this, args), delay);
//        };
//    }
//}
class Select2Dropdown {
    constructor({ dropdownId, apiUrl, dataKey = "id", labelKey = "text", chunkSize = 50, token = "", extraParams = {}, data = {} }) {
        this.dropdownId = dropdownId;
        this.apiUrl = apiUrl;
        this.dataKey = dataKey;
        this.labelKey = labelKey;
        this.chunkSize = chunkSize || 500;
        this.token = token;
        this.extraParams = extraParams;
        this.data = data;
        this.initDropdown();
    }

    initDropdown() {

        const dropdown = $(`#${this.dropdownId}`);


        dropdown.select2({

            allowClear: true,
            width: 'style',
            placeholder: {
                id: '0', // the value of the option
                text: 'Select an option'
            },
            //minimumInputLength: 1, // Only search when user types at least 1 character
            ajax: {
                type: "POST",
                url: this.apiUrl,
                headers: { 'Authorization': this.token },
                contentType: "application/json",
                dataType: "json",
                delay: 1000, // Delay to reduce API calls
                data: (params) => {
                    return JSON.stringify({
                        searchTerm: params.term || "", // Search input
                        page: params.page || 1,  // Pagination
                        pageSize: this.chunkSize, // Number of records per request
                        ...this.extraParams  // Additional parameters
                    });
                },
                processResults: (response) => {
                    return {
                        results: response.Result.data.map(item => ({
                            id: item[this.dataKey],
                            text: item[this.labelKey]
                        })),
                        pagination: {
                            more: response.Result.data.length >= this.chunkSize // Enable pagination if more data is available
                        }
                    };
                }
                //cache: true
            }
        });
        this.setSelectedValue();
    }

    // Method to clear the dropdown
    clearDropdown() {
        $(`#${this.dropdownId}`).val(null).trigger("change");
    }

    // Method to reset the dropdown with default placeholder
    resetDropdown() {
        $(`#${this.dropdownId}`).empty().append(new Option("Please Select", "")).trigger("change");
    }

    setSelectedValue() {
        const dropdown = $(`#${this.dropdownId}`);
        let selectedId = this.data.key;
        let selectedText = this.data.val;

        if (selectedId && selectedText) {
            // Check if the option already exists
            if (dropdown.find(`option[value="${selectedId}"]`).length === 0) {
                // Manually add the option if not present
                const newOption = new Option(selectedText, selectedId, true, true);
                dropdown.append(newOption);
            }

            // Set the selected value
            dropdown.val(selectedId).trigger("change");
        }
    }
}