jQuery(document).ready(function ($) {


	// date editors
	$('input.date, input[type=date]').datepicker();


	// instruactions
	//$('textarea.htmltext-admin').cleditor({ useCSS: false });


	var userMessage = $('<div class="loading" id="user-message"></div>').appendTo($(document.body)).hide();

	window.showLoading = function (msg) {
		userMessage
			.html(msg)
			.show();
	}
	window.hideLoading = function () {
		userMessage
			.fadeOut();
	}

});