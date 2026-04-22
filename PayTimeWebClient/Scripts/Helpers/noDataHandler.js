function handleGridVisibility(tableId, emptyDivId, dataList, emptyOptions = {}) {
    const hasData = Array.isArray(dataList) && dataList.length > 0;
    const defaulPath = "/assets/images/";
    if (hasData) {
        $(`#${tableId}`).show();
        $(`#${emptyDivId}`).hide();
    } else {
       
        const { imageUrl, titleText, subText } = emptyOptions;

        if (imageUrl) {
            $(`#${emptyDivId} img`).attr("src", defaulPath + imageUrl);
           
          
        } else {
            $(`#${emptyDivId} img`).attr("src", defaulPath + "emptyscreen_1.png");
        }

        if (titleText) {    
            $(`#${emptyDivId} p:eq(0)`).text(titleText);
        } else {
            $(`#${emptyDivId} p:eq(0)`).text("No data found");
        }

        if (subText) {
            $(`#${emptyDivId} p:eq(1)`).text(subText);
        }

        $(`#${tableId}`).hide();
        $(`#${emptyDivId}`).show();
        $('.resignblock').hide();
    }
}
