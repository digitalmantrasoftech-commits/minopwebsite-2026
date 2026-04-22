importScripts('https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.17.0/xlsx.full.min.js');

self.onmessage = function (e) {
   
    const { data } = e.data;

    try {
      
        const parsedData = Object.entries(data);

        if (!Array.isArray(parsedData)) {
            self.postMessage({ action: 'error', message: 'Unexpected data format, expected an array.' });
            return;
        }

        function stripHtmlTags(input) {
            if (typeof input === 'string') {
                return input.replace(/<[^>]+>/g, '').trim();
            }
            return input;
        }
        const dataArray = parsedData.find(item => item[0] === 'data')[1];
       
        const formattedData = dataArray.map(item => ({
            EmployeeCode: item.EmpId,
            Punchid: item.Punchid,
            EmpolyeeName: item.EmpName,
            PunchTime: item.PunchTime || '00:00',
            DeviceCode: item.DeviceId,
            DeviceName: item.DeviceName,
            DeviceIP: item.DeviceIP,
            DeviceType: item.DeviceType
        }));



        const worksheet = XLSX.utils.json_to_sheet(formattedData);
        const workbook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, "TransactionMonitor");
        const wbout = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
        self.postMessage({ action: 'download', data: wbout });
    } catch (error) {
        self.postMessage({ action: 'error', message: 'Failed to process data: ' + error.message });
    }
};
