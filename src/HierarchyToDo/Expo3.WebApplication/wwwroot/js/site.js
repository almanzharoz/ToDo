function eventsPageInit() {
    var page = 0;
    var size = 10;
    var allLoaded = false;
    var loadEvents = function (pageIndex, pageSize) {
        if (allLoaded)
            return;
        var categories = [];
        $(".class-category").each(function () {
            if ($(this).prop("checked"))
                categories.push($(this).val());
        });
        Events.loadEvents($("#txtSearchText").val(),
            $("#startDate").val(),
            $("#endDate").val(),
            $("#selectCity option:selected").val() != "0" ? $("#selectCity option:selected").text() : "",
            $("#chkConcert").prop("checked"),
            $("#chkExhibition").prop("checked"),
            $("#chkExcursion").prop("checked"),
            $("#chkAllCategories").prop("checked") ? [] : categories,
            pageIndex,
            pageSize,
            $("#maxPrice").val()).done(function (events) {
                if (events.length < size)
                    allLoaded = true;
                if (events.length > 0) {
                    $.each(events,
                        function (i, item) {
                            $("#listContainer tbody").append("<tr><td><a target='_blank' href='/event/event/" + item.id + "'>" +
                                item.title +
                                "</a></td><td>" +
                                item.dateTimeString +
                                "</td><td>" +
                                item.imageUrl +
                                "</td></tr>");
                        });
                } else
                    $("#listContainer tbody").html("<tr><td colspan='3'>Событий не найдено</td></tr>");
                page++;
            }).fail(function () { $("#listContainer tbody").html("<tr><td colspan='3' class='text-danger'>При выполнении запроса произошла ошибка</td></tr>"); });
    };
    $("#btnLoad").click(function () {
        allLoaded = false;
        $("#listContainer tbody").html("");
        loadEvents(0, size);
    });
    $('#startDate').datetimepicker({
        format: "DD.MM.YYYY HH:mm"
    });
    $('#endDate').datetimepicker({
        format: "DD.MM.YYYY HH:mm"
    });
    $("#chkAllCategories").change(function () {
        if ($(this).prop("checked")) {
            $('.class-category').prop('checked', true);
        }
    });
    $(".class-category").change(function () {
        if (!$(this).prop("checked")) {
            $("#chkAllCategories").prop("checked", false);
        }
    });
    $("#btnLoad").click();
}
