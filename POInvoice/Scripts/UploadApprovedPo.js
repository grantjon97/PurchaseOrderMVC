function UploadFinalizedPo() {

    var bootboxMessage = $("#js-exampleDiv").html().replace("js-exampleForm", 'js-bootboxForm');

    bootbox.confirm({
        title: "Add some info...",
        message: bootboxMessage,
        buttons: {
            cancel: {
                label: "Cancel",
                className: "btn btn-default"
            },
            confirm: {
                label: "Submit",
                className: "btn btn-primary"
            }
        },
        callback: function (result) {

            if (result) {
                var poNumber = $("#poNumber", ".js-bootboxForm").val();

                console.log(poNumber);

                selectFileToUpload(poNumber);
            }
        }
    });

}

function selectFileToUpload(poNumber) {
    bootbox.dialog({
        title: "Upload your PO Form (Optional, must be a PDF)",
        message:

            '<form method="post" id="uploader" class="form-horizontal"> ' +

            '<input type="file" id="filesToUpload" accept=".pdf" />' +

            '</form>',

        buttons: {
            danger: {
                label: "Cancel",
                className: "btn btn-default"
            },
            success: {
                label: "Submit",
                className: "btn btn-primary",
                callback: function () {
                    var formData = new FormData();
                    formData.append("file", $("#filesToUpload")[0].files[0]);
                    
                    SendDataToUpload($("#id").val(), poNumber, formData, $("#filesToUpload")[0].files.length);

                }
            }
        }
    });
}

function SendDataToUpload(id, poNumber, formData, fileLength) {

    bootbox.dialog({ closeButton: false, message: '<div align="center"><i class="fa fa-spin fa-spinner"></i> Loading...</div>' });

    $.ajax({
        url: "/api/Upload/AddPdfToPendingForm/?strId=" + id +
            "&poNumber=" + poNumber +
            "&numberOfFiles=" + fileLength,

        method: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (result) {

            if (fileLength <= 0) {
                ReWritePoFormWithPoNumber(id);
            }

            location.reload();
        }
    });
}

function ReWritePoFormWithPoNumber(id) {
    // This just rewrites the electronic form
    // with the PO number filled in.

    $.ajax({
        url: "/Home/WritePoFormToFile?strId=" + id,
        method: "POST",
        contentType: false,
        processData: false,
        success: function (result) {
            // nothing to do here yet
        }
    });
}