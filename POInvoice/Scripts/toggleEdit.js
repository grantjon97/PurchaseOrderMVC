// Currently, this script takes care of the situation where a user might want to edit an approved form.
// However, users are unable to edit approved forms at this time (I'm not sure if they would ever want to,
// but if they ever did, the code above could be tweaked slightly to accomodate for that.)

$(document).ready(function () {

    if ($("#status").val() == "Approved") {
        $("#PoNumber").removeAttr('readonly');
    }

    // This gets rid of the extra line break and copyright claim at the bottom of the page
    $("#bottomPageStuff").remove();
});

function Toggle(toggleButton) {
    var isChecked = document.getElementById("toggle").checked;
    document.getElementById("fieldset").disabled = isChecked ? false : true;

    if (isChecked && $("#status").val() == "Approved") {
        bootbox.alert("<strong>WARNING: If you edit and save changes to this form, its status will be \"PENDING\".</strong>");
    }
}