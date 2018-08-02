function AddSupportingDocs() {

    bootbox.dialog({
        title: "Add Supporting Documents",
        message:

            '<form method="post" id="uploader" class="form-horizontal"> ' +

            '<input multiple type="file" id="filesToUpload" />' +

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
                    var numberOfFilesAdded = 0;

                    var filesAlreadyListed = $("[data-file=true]")

                    for (var i = 0; i < $("#filesToUpload")[0].files.length; i++) {

                        var fileAlreadyExists = false;
                        for (var j = 0; j < $("#documentsTable tbody tr").length; j++) {

                            if ($("#filesToUpload")[0].files[i].name === filesAlreadyListed[j].text)
                                fileAlreadyExists = true;
                        }

                        if (!fileAlreadyExists) {
                            formData.append("file" + i, $("#filesToUpload")[0].files[i]);
                            formData.append("time" + i, $("#filesToUpload")[0].files[i].lastModified.toString());
                            numberOfFilesAdded++;
                        }
                    }

                    if (numberOfFilesAdded > 0) {

                        bootbox.dialog({ closeButton: false, message: '<div align="center"><i class="fa fa-spin fa-spinner"></i> Loading...</div>' });

                        $.ajax({
                            url: "/api/UploadSupportingDoc/AddDoc?id=" + $("#id").val(),

                            method: "POST",
                            data: formData,
                            contentType: false,
                            processData: false,
                            success: function (result) {
                                location.reload();
                            }
                        });
                    }
                    else {
                        toastr.warning("No new files needed to be uploaded.\nCheck that the file(s) already exist.",
                            { timeOut: 9000 });
                    }

                }
            }
        }
    });
}