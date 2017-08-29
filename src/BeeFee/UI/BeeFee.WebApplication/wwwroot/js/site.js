function eventsPageInit() {
    var page = 0;
    var size = 10;
    var allLoaded = false;
    var loading = false;
    var loadEvents = function () {
        if (allLoaded || loading)
            return;
        loading = true;
        var categories = [];
        $(".class-category").each(function () {
            if ($(this).prop("checked"))
                categories.push($(this).val());
        });
        var loadingContainer = $("<tr><td colspan='3'>Загружаю события...</td></tr>");
        if (page == 0)
            $("#listContainer tbody").html("");
        $("#listContainer tbody").append(loadingContainer);

        Events.loadEvents($("#txtSearchText").val(),
            $("#startDate").val(),
            $("#endDate").val(),
            $("#selectCity option:selected").val() != "0" ? $("#selectCity option:selected").text() : "",
            $("#chkConcert").prop("checked"),
            $("#chkExhibition").prop("checked"),
            $("#chkExcursion").prop("checked"),
            $("#chkAllCategories").prop("checked") ? [] : categories,
            page,
            size,
            $("#maxPrice").val()).done(function (events) {
                if (events.length < size)
                    allLoaded = true;
                if (events.length > 0) {
                    $.each(events,
                        function (i, item) {
                            $("#listContainer tbody").append("<tr><td><a target='_blank' href='/event/event/" + item.url + "'>" +
                                item.page.caption +
                                "</a></td><td>" +
                                item.page.date +
                                "</td><td>" +
                                item.page.cover +
                                "</td></tr>");
                        });
                } else
                    $("#listContainer tbody").html("<tr><td colspan='3'>Событий не найдено</td></tr>");
                page++;
                loadingContainer.remove();
            }).fail(function () { $("#listContainer tbody").html("<tr><td colspan='3' class='text-danger'>При выполнении запроса произошла ошибка</td></tr>"); }).always(function () { loading = false; });
    };
    $("#btnLoad").click(function () {
        allLoaded = false;
        page = 0;
        $("#listContainer tbody").html("");
        loadEvents();
    });
    $('#startDate').datetimepicker({
        format: "DD.MM.YYYY HH:mm"
    });
    $('#endDate').datetimepicker({
        format: "DD.MM.YYYY HH:mm"
    });
    $("#chkAllCategories").change(function () {
        if (!$('.class-category').prop('checked'))
            $("#chkAllCategories").prop("checked", true);
        else
            if ($(this).prop("checked")) {
                $('.class-category').prop('checked', true);
            }
    });
    $(".class-category").change(function () {
        if (!$('.class-category').prop('checked'))
            $("#chkAllCategories").prop("checked", true);
        else
            if (!$(this).prop("checked")) {
                $("#chkAllCategories").prop("checked", false);
            }
    });
    $("#btnLoad").click();
    $(window).scroll(function () {
        if ($(window).scrollTop() + $(window).height() == $(document).height()) {
            loadEvents();
        }
    });
}