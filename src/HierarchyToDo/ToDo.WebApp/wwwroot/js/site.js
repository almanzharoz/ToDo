// Write your Javascript code.
$(function() {
	$("#modal .btn-primary").click(function () { $("#modal .modal-body form").submit(); });
	$(document).on("click", '.modal-link', function (e) {
		var $this = $(this);
		var modal = $('#modal'), modalBody = $('#modal .modal-body');
		modalBody.html("");

		var showHtml = function(html) {
			modalBody.html(html);
			modal.find(".modal-title").html(modalBody.find("h").html());
			var form = modalBody.find("form");
			if (form.length > 0) {
				$("#modal .btn-primary").show();
				form.submit(function() {
					$.post(form.attr("action"), form.serializeArray(), showHtml);
					e.preventDefault();
					return false;
				});
			} else {
				$("#modal .btn-primary").hide();
				$("#content").load(window.location.toString());
				//modal.fadeOut(1000);
			}
		};

		var modalFunc = function() {
			modal.off('show.bs.modal');
			if ($this.attr("method") && ($this.attr("method").toLowerCase() == "post" || $this.attr("method").toLowerCase() == "delete"))
				$.ajax(e.currentTarget.href,
					{
						method: $this.attr("method"),
						headers: { "RequestVerificationToken": antiforgeryRequestToken }
					}).done(showHtml);
			else
				$.get(e.currentTarget.href, showHtml);
		};

		modal.on('show.bs.modal', modalFunc).modal();
		e.preventDefault();
		return false;
	});

	$(document).on("click",
		'input.typeahead',
		function () {
			var url = $(this).attr("url");
			var id = $(this).attr("set-id");
			$(this).typeahead({
				fitToElement: true,
				autoSelect: true,
				source: function(query, process) {
					return $.get(url,
						{ s: query },
						function(data) {
							return process(data);
						});
				},
				afterSelect: function (item) {
					$("#" + id).val(item.id);
				}
			});
		});

	$(document).on("blur", "input.typeahead", function() {
		var cur = $(this).typeahead("getActive");
		if (cur && cur.name === $(this).val())
			$("#" + $(this).attr("set-id")).val(cur.id);
	});
});