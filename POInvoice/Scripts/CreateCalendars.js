$(document).ready(function () {
    CreateCalendars();
});

function CreateCalendars() {

    CalendarDefaults();

    calendar.createMany($("[data-calendar=true]"), true, true);
}

function CreateSaveCalendar() {

    CalendarDefaults();

    calendar.create($("#bootbox-saveExpirationDate"), true, true);
}

function CalendarDefaults() {
    var pad = function (text, length, padChar) {
        length = length ? length : 2;
        padChar = padChar ? padChar : "0";

        var result = text.toString();
        while (result.length < length)
            result = padChar + result;
        return result;
    }

    calendar.defaultDateFormat = function (year, month, day) {
        return (month + 1) + "/" + pad(day) + "/" + pad(year, 4);
    }

    calendar.defaultTimeFormat = function (hour, minute, second) {
        return ((hour - 1) % 12 + 1) + ":" + pad(minute) + ":" + pad(second) + " " + (hour >= 12 ? "PM" : "AM");
    }

    calendar.defaultDateTimeFormat = function (date, time) {
        return date + " " + time;
    }
}
