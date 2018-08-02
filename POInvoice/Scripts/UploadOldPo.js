var userName = $("#userName").val();

function InputSkeletonFormData() {

    var uploadDialog = bootbox.dialog({
        title: "Input required information first...",
        message: $("#uploadInfo").html(),
        backdrop: true,
        buttons: {
            danger: {
                label: "Cancel",
                className: "btn btn-default"
            },
            success: {
                label: "Submit",
                className: "btn btn-primary",
                callback: function () {

                    var total = $("#uploadTotal").val();
                    var accountNumber = $("#uploadAccountNumber").val();
                    var vendorName = $("#uploadVendorName").val();
                    var poNumber = $("#uploadPoNumber").val();
                    var responsibleParty = $("#uploadResponsibleParty").val();
                    var submitDate = $("#submitDate").val();
                    var expireDate = $("#expireDate").val();

                    if (total != "" &&
                        accountNumber != "" &&
                        vendorName.trim() != "" &&
                        $.isNumeric(total))
                    {
                        Upload(total, accountNumber, vendorName, poNumber, responsibleParty, submitDate, expireDate);
                    }
                    else {
                        toastr.warning("Something went wrong...\nCheck that you entered all required fields correctly.");
                        InputSkeletonFormData();
                    }
                }
            }
        }
    });

    uploadDialog.init(function () {
        CreateCalendars();
        InitializeUploadVendorNameTypeAhead();
    });
}

function Upload(total, accountNumber, vendorName, poNumber, responsibleParty, submitDate, expireDate) {
    bootbox.dialog({
        title: "Upload your old PO Forms (must be PDF's)",
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

                    bootbox.dialog({ closeButton: false, message: '<div align="center"><i class="fa fa-spin fa-spinner"></i> Loading...</div>' });

                    var formData = new FormData();

                    for (var i = 0; i < $("#filesToUpload")[0].files.length; i++) {

                        formData.append("file" + i, $("#filesToUpload")[0].files[i]);

                        // Append the time the file was last modified
                        // This value is the number of milliseconds from Jan 1, 1970, 12:00:00 AM.
                        formData.append("time" + i, $("#filesToUpload")[0].files[i].lastModified.toString());
                    }

                    $.ajax({

                        // Parenetheses are very important in this url. These parameters are explicitly
                        // passed through the url to avoid having to parse different parts of
                        // the formData, which has file information in it.
                        url: "/api/Upload/AddOldPoForms?total=" + total +
                             "&accountNumber=" + accountNumber +
                             "&vendorName=" + vendorName +
                            (poNumber.trim() == "" ? "" : "&poNumber=" + poNumber) +
                            (responsibleParty.trim() == "" ? "" : "&responsibleParty=" + responsibleParty) +
                            (submitDate.trim() == "" ? "" : "&submitDate=" + submitDate.replace(/\//g, "-")) +
                            (expireDate.trim() == "" ? "" : "&expireDate=" + expireDate.replace(/\//g, "-")),

                        method: "POST",
                        data: formData,
                        contentType: false,
                        processData: false,
                        success: function (result) {
                            window.location.href = "/Home";
                        },
                        failure: function (result) {
                            toastr.error("ERROR: Could not upload the PO form.\nCheck that your input fields are in the correct format.");
                        }
                    });
                }
            }
        }
    });
}

function InitializeUploadVendorNameTypeAhead() {

    var vendors = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('Name'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: '/api/Vendors/GetVendors?query=%QUERY',
            wildcard: '%QUERY'
        }
    });

    $('#uploadVendorName').typeahead({
        minLength: 2,
        highlight: true
    }, {
            name: 'vendors',
            display: 'Name',
            source: vendors
        });

}