var isAdvancedUpload = function () {
    var div = document.createElement('div');
    return (('draggable' in div) || ('ondragstart' in div && 'ondrop' in div)) && 'FormData' in window && 'FileReader' in window;
}();
let draggableFileArea = document.querySelector(".drag-file-area");
let browseFileText = document.querySelector(".browse-files");
let uploadIcon = document.querySelector(".upload-icon");
let dragDropText = document.querySelector(".dynamic-message");
let fileInput = document.querySelector(".default-file-input");
let uploadedFile = document.querySelector(".file-block");
let fileName = document.querySelector(".file-name");
let fileSize = document.querySelector(".file-size");
let progressBar = document.querySelector(".progress-bar");
let removeFileButton = document.querySelector(".remove-file-icon");
//let uploadButton = document.querySelector(".upload-button");
let fileFlag = 0;

$(".default-file-input").on("click", function () {
    $(".default-file-input").val('');
    console.log($(".default-file-input").val());
});

$('.default-file-input').on('change', function () {
    console.log(" > " + $(this).val());
    //uploadIcon.html('check_circle');
    //dragDropText.html('File Dropped Successfully!');
    //uploadButton.html('Upload');
    $('.file-name').html($(this)[0].files[0].name);
    $('.file-size').html(($(this)[0].files[0].size / 1024).toFixed(1) + " KB");
    $('.file-block').css('display', 'block');
    fileFlag = 0;
});
//uploadButton.addEventListener("click", function(){
//    let isFileUploaded = fileInput.value;
//    if(isFileUploaded != '') {
//        if (fileFlag == 0) {
//            fileFlag = 1;
//            var width = 0;
//            var id = setInterval(frame, 50);
//            function frame() {
//                if (width >= 390) {
//                    clearInterval(id);
//                    uploadButton.innerHTML = '<span class="material-icons-outlined upload-button-icon"> check_circle </span> Uploaded';
//                } else {
//                    width += 5;
//                    progressBar.style.width = width + "px";
//                }
//            }
//        }
//    } else {
//        cannotUploadMessage.style.cssText = "display: flex; animation: fadeIn linear 1.5s;";
//    }
//});


if (isAdvancedUpload) {
    ["drag", "dragstart", "dragend", "dragover", "dragenter", "dragleave", "drop"].forEach(
    //    evt, function (evt) {
    //    draggableFileArea.addEventListener(evt, function(){
    //        e.preventDefault();
    //        e.stopPropagation();
    //    })
    //}
    function (evt) {
        $('.drag-file-area').on('click mouseover dragover dragenter dragleave drop', function (e) {
            e.preventDefault();
            e.stopPropagation();
        });
    }
    );
    //btns.forEach(function (i) {
    //    i.addEventListener('click', function () {
    //        document.querySelector('.msg').innerHTML = i.innerHTML;
    //    });
    //});
    ["dragover", "dragenter"].forEach(
        //evt, function (evt) {
        //{
        //    draggableFileArea.addEventListener(evt, function () {
        //        e.preventDefault();
        //        e.stopPropagation();
        //        uploadIcon.innerHTML = 'file_download';
        //        dragDropText.innerHTML = 'Drop your file here!';
        //    });
        //}
        //}
        function (evt) {
            $('.drag-file-area').on('click mouseover dragover dragenter', function (e) {
                e.preventDefault();
                e.stopPropagation();
                $('.upload-icon').html('file_download');
                $('.dynamic-message').html('Drop your file here!');
            });
        }
    );

    $('.drag-file-area').on('drop', function (e) {
        e.preventDefault();
        e.stopPropagation();

        $('.upload-icon').html('check_circle');
        $('.dynamic-message').html('File Dropped Successfully!');
        $('.upload-button').html('Upload');

        let files = e.originalEvent.dataTransfer.files;
        $('.default-file-input').prop('files', files);
        console.log(files[0].name + " " + files[0].size);
        console.log($('.default-file-input').val());
        $('.file-name').html(files[0].name);
        $('.file-size').html((files[0].size / 1024).toFixed(1) + " KB");
        $('.file-block').css('display', 'flex');
        $('.progress-bar').css('width', '0');
        fileFlag = 0;
    });
}

//removeFileButton.addEventListener("click", function () {
//    uploadedFile.style.cssText = "display: none;";
//    fileInput.value = '';
//    uploadIcon.innerHTML = 'file_upload';
//    dragDropText.innerHTML = 'Drag & drop any file here';
//    uploadButton.innerHTML = 'Upload';
//});

$("#removeFileButton").on("click", function () {
    $("#uploadedFile").css("display", "none");
    $("#fileInput").val("");
    $("#uploadIcon").html("file_upload");
    $("#dragDropText").html("Drag &amp; drop any file here");
    $("#uploadButton").html("Upload");
});
