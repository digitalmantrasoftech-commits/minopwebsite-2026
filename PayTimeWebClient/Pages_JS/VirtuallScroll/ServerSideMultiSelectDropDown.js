class ServerVirtualDropdown {
    constructor({ dropdownId, apiUrl, chunkSize, dataKey, labelKey, data = {}, tokan, selectAll, select }) {
        this.dropdownId = dropdownId || "";
        this.apiUrl = apiUrl || "";
        this.chunkSize = chunkSize || 500;
        this.dataKey = dataKey || "id";
        this.labelKey = labelKey || "name";
        this.page = 1;
        this.searchTerm = "";
        this.totalRecords = 0;
        this.data = data;
        this.select = select || [];
        this.tokan = tokan || "";
        this.isLoading = false; // 🔥 Prevent duplicate requests
        this.selectAll = selectAll;
        this.allSelected = false;
        this.isEmpty = false;
        this.isServerSide = true;// there is no data related to search
        //this.BindedData = [];
        this.initDropdown();
        this.bindEvents();
        //this.loadMoreData();
    }

    initDropdown() {

        $(`#${this.dropdownId}`).multiselect({
            enableFiltering: true,
            enableCaseInsensitiveFiltering: true,
            includeSelectAllOption: this.selectAll,
            includeFilterClearBtn: false,
            nonSelectedText: 'Please Select',
            allSelectedText: 'All Select',
            selectAllNumber: false,
            maxHeight: '300',
            buttonWidth: '100%',
            onSelectAll: () => this.handleSelectAll(true),
            onDeselectAll: () => this.handleSelectAll(false)
            //onDropdownShown: () => this.loadMoreData(),
            //onChange: (option, checked) => this.toggleSelection(option, checked)
        });
    }

    handleSelectAll(checked) {

        this.allSelected = checked;
        // You can perform any action here, like storing selected values
    }
    bindEvents() {
        const dropdownContainer = $(`#${this.dropdownId}`).next('.btn-group').find('.multiselect-container');
        // Virtual scrolling trigger
        dropdownContainer.on('scroll', (e) => this.handleScroll(e));
        // Live search event listener
        //$('body').on('input', `.multiselect-search`, (e) => this.handleSearch(e));
        $('body').on('input', `#${this.dropdownId}Container .multiselect-search`, (e) => this.handleSearch(e));
        //$('body').on('input', `#${this.dropdownId}Container .multiselect-search`, this.debounce((e) => this.handleSearch(e), 500));
    }

    async handleScroll(event) {
        if (!this.isServerSide) {
            return;
        }
        const container = event.target;
        if (this.isLoading || (this.page - 1) * this.chunkSize >= this.totalRecords) return; // 🔥 Prevent multiple requests
        const scrollTop = container.scrollTop;
        const containerHeight = container.offsetHeight;
        const scrollHeight = container.scrollHeight;

        if (scrollTop + containerHeight >= scrollHeight - this.chunkSize) {
            let option = await this.loadMoreData();
            $(`#${this.dropdownId}`).append(option.options);
            $(`#${this.dropdownId}`).multiselect('rebuild');
        }
        $(`#${this.dropdownId}Container input.multiselect-search`).val(this.searchTerm);
    }

    async handleSearch(event) {
       
        if (!this.isServerSide) {
            return;
        }
        this.searchTerm = $(event.target).val().trim();
        
        this.page = 1;
        this.totalRecords = 0;

        if (this.allSelected) { // condition for remove select all after every search

            this.allSelected = false;
            $(`#${this.dropdownId}`).empty();
            $(`#${this.dropdownId}`).trigger("change");
        }

        //$(`#${this.dropdownId}`).empty(); // Clear dropdown for new search
        let option = await this.loadMoreData();
        if (!this.isEmpty) //condition is for when user search and for perticular search relted data is empty and also there is no data selected than it not Rebind(preventing from dropdown close when searching)
        {
            //if (needToEmpty) {
            //    $(`#${this.dropdownId}`).empty();
            //    $(`#${this.dropdownId}`).trigger("change");
            //}

            $(`#${this.dropdownId}`).find('option:not(:selected)').remove(); // Only remove unselected options
            //$(`#${this.dropdownId}`).append(option.selectedOptions).append(option.options);
            //$(`#${this.dropdownId}`).multiselect('rebuild');
            $(`#${this.dropdownId}`).append(option.options).multiselect('rebuild');
            
        }
        $(`#${this.dropdownId}Container input.multiselect-search`).val($(event.target).val());
        $(`#${this.dropdownId}Container input.multiselect-search`).focus();
        //this.Result()
      

    }
    async loadInitialData(callback) {
        this.page = 1
        this.totalRecords = 0;
        this.isServerSide = true;
        this.searchTerm = "";

        this.allSelected = false;
        $(`#${this.dropdownId}`).empty();
        let option = await this.loadDataFirstTime();
        $(`#${this.dropdownId}`).empty();
        $(`#${this.dropdownId}`).append(option.selectedOptions).append(option.options);
        $(`#${this.dropdownId}`).multiselect('rebuild');
        if (callback && typeof callback === 'function') {
            callback();
        }
    }
    async loadDataFirstTime() { // first time when initilize this method is called
        //if (this.isLoading || this.page * this.chunkSize >= this.totalRecords) return;
        let option = {}
        this.isLoading = true; // 🔥 Set loading flag

        const requestData = {
            page: this.page,
            pageSize: this.chunkSize,
            searchTerm: this.searchTerm,
            ...this.data
        };

        try {
            const response = await $.ajax({
                url: this.apiUrl,
                type: 'POST',
                data: JSON.stringify(requestData),
                contentType: "application/json",
                headers: { 'Authorization': this.tokan }
            });

            if (response.Result && response.Result.data) {
                this.page = 1;
                    //this.BindedData = [];
                this.totalRecords = response.Result.totalRecords; // ✅ Only update once
                

                if (this.totalRecords <= this.chunkSize) {
                    this.isServerSide = false;
                }
                else {
                    this.isServerSide = true;
                }

                if (response.Result.data.length <= 0) {
                    this.isEmpty = true
                }
                else {
                    this.isEmpty = false
                }
                //this.BindedData = response.Result.data;
                option = await this.appendData(response.Result.data);
                this.page = 2;
            }
        } catch (error) {
            console.error('Error fetching data:', error);
        } finally {
            this.isLoading = false;

        }
        return option;
    }

    async loadMoreData() { // at the time of scroll and search this method is called
        //if (this.isLoading || this.page * this.chunkSize >= this.totalRecords) return;
        let option = {}
        this.isLoading = true; // 🔥 Set loading flag

        const requestData = {
            page: this.page,
            pageSize: this.chunkSize,
            searchTerm: this.searchTerm,
            ...this.data
        };

        try {
            const response = await $.ajax({
                url: this.apiUrl,
                type: 'POST',
                data: JSON.stringify(requestData),
                contentType: "application/json",
                headers: { 'Authorization': this.tokan }
            });

            if (response.Result && response.Result.data) {
                if (this.page === 1) {
                    //this.BindedData = [];
                    this.totalRecords = response.Result.totalRecords; // ✅ Only update once
                }

                if (this.totalRecords <= this.chunkSize) {
                    this.isServerSide = false;
                }
                else {
                    this.isServerSide = true;
                }

                if (response.Result.data.length <= 0) {
                    this.isEmpty = true
                }
                else {
                    this.isEmpty = false
                }
                //this.BindedData = response.Result.data;
                option = await this.appendData(response.Result.data);
                this.page++;
            }
        } catch (error) {
            console.error('Error fetching data:', error);
        } finally {
            this.isLoading = false;

        }
        return option;
    }

    async appendData(items) {
        let options = [];
        let selectedOptions = [];
        let preSelected = [...this.select];
        //When we want select item at initilize time this used for edit
        if (Array.isArray(preSelected) && preSelected.length > 0) {
            for (let item of preSelected) {
                if (item.value !== undefined && !selectedOptions.find(opt => opt.value == item.key)) {
                    selectedOptions.push(new Option(item.value, item.key, true, true));
                }
            }
        }
        preSelected = [];
        this.select = [];

        $(`#${this.dropdownId} option:selected`).each(function () {
            const Id = $(this).val();
            const val = $(this).text();
            //const data = entity.find(data => data[key] == Id);
            if (!selectedOptions.find(opt => opt.value == Id)) {
                selectedOptions.push(new Option(val, Id, true, true));
            }
        });

        if (this.allSelected) {

            for (let item of items) {
                if (!selectedOptions.find(opt => opt.value == item[this.dataKey])) {
                    options.push(
                        new Option(item[this.labelKey], item[this.dataKey], true, true)
                    );
                }
            }
        }
        else {

            for (let item of items) {
                if (!selectedOptions.find(opt => opt.value == item[this.dataKey])) {
                    options.push(
                        new Option(item[this.labelKey], item[this.dataKey])
                    );
                }
            }
        }
        return { options, selectedOptions };

    }

    empty() {
        this.page = 1
        this.totalRecords = 0;
        this.searchTerm = "";
        this.isServerSide = true;
        this.allSelected = false;
        $(`#${this.dropdownId}`).empty();
        $(`#${this.dropdownId}`).multiselect('rebuild');
    }

    debounce(func, delay) {
        let timer;
        return function (...args) {
            clearTimeout(timer);
            timer = setTimeout(() => func.apply(this, args), delay);
        };
    }
    Result() {
        let resultArray = [];
        let result = "";
        $(`#${this.dropdownId} option:selected`).each(function () {
            const Id = $(this).val();
            resultArray.push(Id);

        });
        result = resultArray.join(",");
        console.log(result)
        return result;
    }
}