class VirtualDropdown {
    constructor({ dropdownId, chunkSize, loadData, dataKey, labelKey }) {
        this.dropdownId = dropdownId; // ID of the dropdown
        this.chunkSize = chunkSize || 100; // Number of items to load per chunk
        this.loadData = loadData; // Function to fetch/filter data
        this.dataKey = dataKey; // Key for value in options
        this.labelKey = labelKey; // Key for label in options
        this.loadedItems = 0; // Tracks how many items are currently loaded
        this.filteredData = []; // Filtered dataset
        this.allData = []; // Entire dataset
        this.searchTerm = "";
        this.result = "";
        this.initDropdown();
      
        this.bindEvents();
    }
  
    initDropdown() {
        $(`#${this.dropdownId}`).multiselect({
            selectAllValue: 'multiselect-all',
            enableCaseInsensitiveFiltering: true,
            includeSelectAllOption: false,
            allSelectedText: 'All Select',
            enableFiltering: true,
            maxHeight: '300',
            buttonWidth: '100%',
            enableClickableOptGroups: true,
            nonSelectedText: 'Please Select',
            onDropdownShown: () => this.loadInitialData(),
        });
       
    }

    bindEvents() {

        $(`#${this.dropdownId}`).next('.btn-group').find('.multiselect-container').on('scroll', (e) => this.handleScroll(e));

        // Attach input event to search box inside dropdown
        $('body').on('input', `#${this.dropdownId}Container .multiselect-search`, (e) => this.handleSearch(e));
    }
    //SCROLL FUNCIONALITY
    handleScroll(event) {
        const container = event.target;
        const scrollTop = container.scrollTop;
        const containerHeight = container.offsetHeight;
        const scrollHeight = container.scrollHeight;

        // Load more data if scrolled near the bottom
        if (scrollTop + containerHeight >= scrollHeight - 100 && this.loadedItems < this.filteredData.length) {
           let option = this.appendData(this.loadedItems, this.loadedItems + this.chunkSize);
            $(`#${this.dropdownId}`).append(option.options);
            $(`#${this.dropdownId}`).multiselect('rebuild');
            this.loadedItems += option.options.length;
        }
        /*$('input.multiselect-search').val(this.searchTerm);*/
        $(`#${this.dropdownId}Container input.multiselect-search`).val(this.searchTerm);
    }
    //SEARCH FUNCIONALITY
    handleSearch(event) {
        this.searchTerm = $(event.target).val().toLowerCase();
        if (filteredEmployees.length === 0) {
            $(`#${this.dropdownId}`).append(new Option('No results found', '', false, false)).multiselect('rebuild');
            //$('input.multiselect-search').val($(event.target).val()); // Keep the search term in the input

            //$('input.multiselect-search').focus();
            $(`#${this.dropdownId}Container input.multiselect-search`).val($(event.target).val()); // Keep the search term in the input

            $(`#${this.dropdownId}Container input.multiselect-search`).focus();
        }
        else {
           
            // Filter the data
            this.filteredData = this.allData.filter(item =>
                item[this.labelKey].toLowerCase().includes(this.searchTerm)
            );
            this.loadedItems = 0;
            let option = this.appendData(0, this.chunkSize);
            //let option = this.resetDropdown();
            if (option.options.length > 0) {
                $(`#${this.dropdownId}`).empty();
                $(`#${this.dropdownId}`).append(option.selectedOptions).append(option.options);
                $(`#${this.dropdownId}`).multiselect('rebuild');

                this.loadedItems += (option.options.length + option.selectedOptions.length);
            }
            //$('input.multiselect-search').val($(event.target).val());
            //$('input.multiselect-search').focus();
            $(`#${this.dropdownId}Container input.multiselect-search`).val($(event.target).val());
            $(`#${this.dropdownId}Container input.multiselect-search`).focus();
        }
    }
    //FIRST TIME DATA BIND WHEN INITILIZE DROPDOWN
    loadInitialData() {
        if (this.loadedItems === 0) {
            this.allData = this.loadData;
            this.filteredData = [...this.allData]; // Copy all data initially
            let option =this.appendData(0, this.chunkSize);
            $(`#${this.dropdownId}`).append(option.options);
            $(`#${this.dropdownId}`).multiselect('rebuild');
            this.loadedItems += option.options.length;
        }
    }

    appendData(startIndex, endIndex) {
        const options = [];
        let selectedOptions = [];
        let entity = [...this.allData];
        const key = this.dataKey
        const value = this.labelKey
        //For binding selected item on top every time when datatable reinitialize or reform
        $(`#${this.dropdownId} option:selected`).each(function () {
                const Id = $(this).val();
            const data = entity.find(data => data[key] == Id);
            if (data && !selectedOptions.find(opt => opt.value == data[key]))
            {
                selectedOptions.push(new Option(data[value], data[key], true, true));
            }
        });
        //Regulre binding of data on scroll
        for (let i = startIndex; i < endIndex && i < this.filteredData.length; i++) {
            const item = this.filteredData[i];
            //same record not bind if it selected or already in the Dropdown
            if (!selectedOptions.find(opt => opt.value == item[this.dataKey])) {
                options.push(
                    new Option(item[this.labelKey], item[this.dataKey])
                );
            }
        }
      
        return { options, selectedOptions };
        // Append options and rebuild
        //$(`#${this.dropdownId}`).append(options);
        //$(`#${this.dropdownId}`).multiselect('rebuild');
        //this.loadedItems += options.length;
     
    }

    resetDropdown() {
        // Clear dropdown and reset loading state
        this.loadedItems = 0;
        $(`#${this.dropdownId}`).empty();
        this.loadInitialData();
    }
    Result() {
        let resultArray = [];
        $(`#${this.dropdownId} option:selected`).each(function () {
            const Id = $(this).val();
            resultArray.push(Id);
            
        });
        this.result = resultArray.join(",");
       
        return this.result;
    }
}