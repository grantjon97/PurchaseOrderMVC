$(document).ready(function () {

    CalculateAllSubTotals();
    UpdateGrandTotal();
});

function Save() {

    ReAssignIndexes();

    if ($("#vendorName").val() === "") {
        toastr.warning("Please enter a value for the vendor name.");
    }
    else {

        var dialog = bootbox.dialog({
            title: "Additional Information",
            message: $("#saveInfo").html(),
            backdrop: true,
            buttons: {
                danger: {
                    label: "Cancel",
                    className: "btn btn-default"
                },
                success: {
                    label: "Done",
                    className: "btn btn-primary",
                    callback: function () {
                        if ($("#bootbox-saveOverview").val().trim() == "") {
                            Save();
                        }
                        else {

                            //TODO: put these values into hidden fields in FillablePo.cshtml
                            document.getElementsByName("Overview")[0].value = $("#bootbox-saveOverview").val();
                            $("#responsibleParty").val($("#bootbox-saveResponsibleParty").val());
                            $("#expirationDate").val($("#bootbox-saveExpirationDate").val());

                            $("#form").submit();
                        }
                    }
                }
            }
        });

        dialog.init(function () {
            CreateSaveCalendar();
        });
    }
}

function ReAssignIndexes() {

    var rows = $("#tab_logic >tbody >tr");

    for (var i = 0; i < rows.length - 1; i++) {

        $(rows[i]).attr("id", "addr" + i);
        $(rows[i]).find("[data-info=accountnumber]").attr("name", "LineItems[" + i + "].AccountNumber");
        $(rows[i]).find("[data-info=description]").attr("name", "LineItems[" + i + "].Description");
        $(rows[i]).find("[data-money=quantity]").attr("name", "LineItems[" + i + "].Quantity");
        $(rows[i]).find("[data-money=unitcost]").attr("name", "LineItems[" + i + "].UnitCost");
        $(rows[i]).find("[data-money=total]").attr("name", "LineItems[" + i + "].Total");
    }

}

function AddRow() {

    if ($('#tab_logic tbody tr').length < 16) {
        $('#tab_logic >tbody tr:last').remove();
        var rowToCopy = $('#tab_logic >tbody tr:last');
        var oldIndex = parseInt(rowToCopy.attr("id")[rowToCopy.attr("id").length - 1]);
        var newIndex = oldIndex + 1;

        var newRow = rowToCopy.clone();


        newRow.attr("id", "addr" + newIndex);
        newRow.find("[data-info=accountnumber]").attr("name", "LineItems[" + newIndex + "].AccountNumber").attr("placeholder", "123-456-789").val("");
        newRow.find("[data-info=description]").attr("name", "LineItems[" + newIndex + "].Description").attr("placeholder", "Description").val("");
        newRow.find("[data-money=quantity]").attr("name", "LineItems[" + newIndex + "].Quantity").attr("placeholder", "0").val("");
        newRow.find("[data-money=unitcost]").attr("name", "LineItems[" + newIndex + "].UnitCost").attr("placeholder", "0.00").val("");
        newRow.find("[data-money=total]").attr("name", "LineItems[" + newIndex + "].Total").val("0.00");

        $("#tab_logic >tbody").append(newRow);

        $("#numberOfRows").val(parseInt($("#numberOfRows").val()) + 1);
        $('#tab_logic').append('<tr id="addr' + (newIndex + 1) + '"></tr>');

        CheckRowNumbers();
    }
    else {
        toastr.warning("You cannot add any more rows.");
    }
}

function CheckRowNumbers() {
    for (var i = 0; i < $('#tab_logic >tbody >tr').length; i++)
        $($('#tab_logic >tbody >tr')[i]).find("[data-row=true]").html(i + 1);
}

function DeleteInnerRow(buttonId) {
    var buttonIndex = buttonId.id.toString()[buttonId.id.toString().length - 1];

    if ($('#tab_logic >tbody >tr').length > 2) {

        // Delete hidden input of lineItem.Id (we only want to send existing lineItem Id's through AJAX call)
        var lineId = $(buttonId).closest("tr").attr("data-lineid");
        $("[data-hiddenId=" + lineId + "]").remove();

        $(buttonId).closest("tr").remove();

        UpdateGrandTotal();
        CheckRowNumbers();

        toastr.success("Row deleted.");
    }
    else {
        toastr.warning("You cannot delete any more rows.");
    }
}

function CalculateCosts(input) {

    var quantity = parseInt($(input).closest("tr").find("[data-money=quantity]").val());
    quantity = isNaN(quantity) ? 0 : quantity;

    var unitCost = parseFloat($(input).closest("tr").find("[data-money=unitcost]").val());
    unitCost = isNaN(unitCost) ? 0 : unitCost;

    var totalInputBox = $(input).closest("tr").find("[data-money=total]");

    totalInputBox.val(parseFloat(quantity * unitCost).toFixed(2));

    UpdateGrandTotal();
}

function UpdateGrandTotal() {

    $("#grandTotal").val("0.00");
    for (var j = 0; j < $("[data-money=total]").length; j++) {
        $("#grandTotal").val((parseFloat($("#grandTotal").val()) +
                              parseFloat($("[data-money=total]")[j].value))
                              .toFixed(2));
    }
}

function CalculateAllSubTotals() {

    var numberOfRows = $('#tab_logic >tbody >tr').length - 1;

    for (var i = 0; i < numberOfRows; i++) {

        var quantity = parseInt($("[data-money=quantity]")[i].value);
        quantity = isNaN(quantity) ? 0 : quantity;

        var unitCost = parseFloat($("[data-money=unitcost]")[i].value);
        unitCost = isNaN(unitCost) ? 0 : unitCost;

        $("[data-money=total]")[i].value = (quantity * unitCost).toFixed(2);
    }
}