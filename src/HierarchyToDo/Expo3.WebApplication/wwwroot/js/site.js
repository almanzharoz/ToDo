function eventsPageInit(url) {
    var page = 0;
    var size = 10;
    var allLoaded = false;
    var loadEvents = function (pageIndex, pageSize) {
        if (allLoaded)
            return;
        $.get(url,
            {
                Text: $("#txtSearchText").val(), StartDate: $("#startDate").val(), EndDate: $("#endDate").val(), City: $("#selectCity option:selected").text(),LoadConcert: $("#chkConcert").prop("checked"),
                LoadExhibition: $("#chkExhibition").prop("checked"), LoadExcursion: $("#chkExcursion").prop("checked"), PageIndex: pageIndex, PageSize: pageSize, MaxPrice: $("#maxPrice").val()
            },
            function (data) {
                if (data.length < size)
                    allLoaded = true;
                $.each(data, function (i, item) {

                });
                page++;
            });
    };
    $("#btnLoad").click(function () {
        allLoaded = false;
        loadEvents(0, size);
    });
}
