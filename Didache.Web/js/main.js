jQuery(document).ready(function ($) {


	// date editors
	$('input.date, input[type=date]').datepicker({ dateFormat: 'yy-mm-dd' });


	// instruactions
	//$('textarea.htmltext-admin').cleditor({ useCSS: false });

	// 
	$('.task-interaction-post textarea').autoResize();


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


	// add classmates links
	$('a.addclassmate-button').bind('click', function (e) {
		e.preventDefault();

		var link = $(this),
			urlParts = link.attr('href').split('/'),
			requesterUserID = urlParts[urlParts.length - 2],
			targetUserID = urlParts[urlParts.length - 1];

		$.ajax({
			url: '/api/addclassmate/',
			type: 'POST',
			data: { requesterUserID: requesterUserID, targetUserID: targetUserID },
			success: function (data) {
				link
					.after('<span class="relationship-status">Pending</span>')
					.remove();
			}
		});

		return false;
	});

	// approve friends!
	$('a.approveclassmate-button').bind('click', function (e) {
		e.preventDefault();

		var link = $(this),
			urlParts = link.attr('href').split('/'),
			requesterUserID = urlParts[urlParts.length - 2],
			targetUserID = urlParts[urlParts.length - 1];

		$.ajax({
			url: '/api/approveclassmate/',
			type: 'POST',
			data: { requesterUserID: requesterUserID, targetUserID: targetUserID },
			success: function (data) {
				link
					.after('<span class="relationship-status">Classmate</span>')
					.remove();
			}
		});

		return false;
	});

	/*

	//news feed shorten
	// shorten task descriptions
	$('.user-post .post-text').expander({
		slicePoint: 500
		, widow: 2
		//,expandEffect: 'show'
		, expandText: 'more »'
		//, userCollapse: false
		, userCollapseText: ''
	});
	*/





	//$('.user-post .post-text').truncate({ max_length: 500 });

});