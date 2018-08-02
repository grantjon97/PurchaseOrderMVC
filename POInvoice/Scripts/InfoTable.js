function DeleteRow(button, id, typeOfDocument) {

    // if the link is to the main PO form
    if (typeOfDocument == 0) {
        DeleteForm();
    }
    else {

        $.ajax({

            url: "/api/Home/DeleteSupportingDocument?id=" + id,

            method: "DELETE",
            contentType: false,
            processData: false,
            success: function (result) {

                $(button).closest("tr").remove();
                toastr.success("Deleted the file from the database.");

                return false;
            },
            failure: function (result) {

                toastr.error("ERROR: Could not delete that file.");
                return false;
            }
        });
    }
}

function DeleteForm() {
    bootbox.confirm({
        message: "<strong>Are you sure you want to delete this entire purchase order form?</strong>" +
                 "<p>You will lose all data and history of the form and any supporting documents.</p>",
        buttons: {
            confirm: {
                label: "Yes, I'm sure",
                className: "btn-success"
            },
            cancel: {
                label: "No",
                className: "btn btn-cps-red"
            }
        },
        callback: function (result) {
            if (result) {

                bootbox.dialog({ closeButton: false, message: '<div align="center"><i class="fa fa-spin fa-spinner"></i> Deleting file...</div>' });

                $.ajax({
                    url: "/api/Home/DeleteEntireForm?id=" + $("#id").val(),
                    method: "DELETE",
                    success: function (result) {
                        window.location.href = "/Home";
                    }
                });
            }
        }
    });
}

function DuplicatePo() {
    var id = $("#id").val();

    bootbox.prompt({
        size: "small",
        title: "Type a name for the copy:",
        value: $("#overview").val() + " (copy)",
        callback: function (requestedOverviewName) {

            if (requestedOverviewName) {

                bootbox.confirm({
                    message: "<p><strong>The duplicated form will have a PENDING status.</strong></p>" +
                        "<p><strong>You may edit the duplicated form, but it will be re-formatted to this site's PO format.</strong></p>",

                    buttons: {
                        confirm: {
                            label: "Okay",
                            className: "btn-success"
                        },
                        cancel: {
                            label: "Cancel",
                            className: "btn btn-cps-red"
                        }
                    },

                    callback: function (result) {
                        if (result) {
                            SendDuplicatePoData(id, requestedOverviewName);
                        }
                    }
                });


            }

        }
    });
}

function SendDuplicatePoData(id, requestedOverviewName) {

    var dialog = bootbox.dialog({ closeButton: false, message: '<div align="center"><i class="fa fa-spin fa-spinner"></i> Loading...</div>' });

    $.ajax({
        url: "/api/Home/DuplicatePo?id=" + id +
            "&overview=" + requestedOverviewName +
            "&username=" + $("#userName").val(),
        method: "POST",
        success: function (result) {
            if (result.success) {
                window.location.href = "/Home/Info/" + result.newId;
            }
            else {
                dialog.modal('hide');
                toastr.error("The name you specified is already in use.");
            }
            return false;
        },
        failure: function (result) {
            toastr.error("Error!");
            return false;
        }
    });
}