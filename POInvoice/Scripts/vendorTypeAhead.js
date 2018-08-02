$(document).ready(function () {

    var vendors = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('Name'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: '/api/Vendors/GetVendors?query=%QUERY',
            wildcard: '%QUERY'
        }
    });

    $('#vendorName').typeahead({
        minLength: 2,
        highlight: true
    }, {
        name: 'vendors',
        display: 'Name',
        source: vendors
        }).on("typeahead:select", function (e, vendor) {

        // Autofill the rest of the vendor information
        $("#Vendor_Address").val(vendor.Address);
        $("#Vendor_City").val(vendor.City);
        document.getElementsByName("Vendor.State")[0].value = vendor.State;
        $("#Vendor_Zip").val(vendor.Zip);
    });
});