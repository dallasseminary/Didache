if (typeof window.console == 'undefined')
	window.console = { log: function () { } };

var Didache = window.Didache || {};
Didache.TaskTypes = [];

jQuery(document).ready(function ($) {

	// turn in files
	$(".transparent-upload input[type=file]").change(function () {

		var fileInput = $(this),
				filename = fileInput.val();

		if (filename.indexOf('\\') > -1)
			filename = filename.substring(filename.lastIndexOf('\\') + 1);

		fileInput.siblings('.file-name').html(filename);

	});

	// shorten task descriptions
	$('.task-instructions-text').expander({
		slicePoint: 1000
		, widow: 2
		//,expandEffect: 'show'
		, expandText: 'more »'
		//, userCollapse: false
		, userCollapseText: '« less'
	});


	// shorten task descriptions
	$('.task-instructions-completed').expander({
		slicePoint: 0
		, widow: 2
		//,expandEffect: 'show'
		, expandText: 'show instructions »'
		//, userCollapse: false
		, userCollapseText: '« hide'
	});


	// get all task types
	$('div.task-entry').each(function () {

		// hook up 
		var taskNode = $(this),
			taskID = $(this).data('taskid'),
			taskType = $(this).data('tasktype'),
			typeFunction = Didache.TaskTypes[taskType];

		if (typeFunction != null) {
			typeFunction(taskID, taskNode);
		}

	});


	// interactions stuff

	// find the nearest post
	var urlHash = self.document.location.hash.toString();
	if (urlHash.length > 0) {
		var threadID = urlHash = urlHash.substring(1);

		//console.log(threadID, urlHash);

		// make sure we have the thread
		if (urlHash.indexOf('post') > -1) {
			// find the thread
			threadID = $('#' + urlHash).closest('.task-interaction-thread').attr('id');
		}

		//console.log(threadID, urlHash);

		// open the thread
		$('#' + threadID)
			.find('.task-interaction-list')
				.show()
			.end()
			.find('.task-interaction-main .task-interaction-text')
				.show();

		// find the post if needed and scroll to it
		if (urlHash.indexOf('post') > -1) {
			var post = $('#' + urlHash);
			$(document.body).attr({ scrollTop: post.offset().top });
			post.effect('highlight');

		}
	}

	// open/close threads
	$('div.task-interaction .total-replies').toggle(function () {
		$(this)
			.closest('.task-interaction-thread')
				.find('.task-interaction-list')
					.slideDown()
				.end()
			.find('.task-interaction-main .task-interaction-text')
				.fadeIn();
	}, function () {
		$(this)
			.closest('.task-interaction-thread')
				.find('.task-interaction-list')
					.slideUp()
			.end()
			.find('.task-interaction-main .task-interaction-text')
				.fadeOut();
	});

	$('.add-reply .collapse').click(function () {
		$(this)
			.closest('.task-interaction-list')
				.slideUp();
	});

	// do reply
	$('div.task-interaction input.reply-button').click(function (e) {

		e.preventDefault();

		var button = $(this).prop('disabled', true),
				taskID = button.closest('.task-entry').data('taskid'),
				text = button.closest('.add-reply').find('textarea').prop('disabled', true).val(),
				threadID = button.closest('.task-interaction-thread').data('threadid');

		// prevent empty responses
		if (text.length == '') {

			button.closest('.add-reply').find('textarea').prop('disabled', false).val('');
			button.prop('disabled', false);

			return;
		}


		showLoading('Saving...');

		$.ajax({
			url: '/courses/api/interactionreply',
			type: 'POST',
			data: {
				text: text,
				taskID: taskID,
				threadID: threadID
			},
			success: function (d) {

				if (d.success) {
					// create response
					var target = button.closest('.add-reply');
					$('<div class="task-interaction-post">' +

						'<div class="task-interaction-userimage">' +
							'<a href="' + d.user.ProfileDisplayUrl + '">' +
								'<img src="' + d.user.ProfileImageUrl + '&width=30&height=30" alt="' + d.user.SecureShortName + '" />' +
							'</a>' +
						'</div>' +
						'<div class="task-interaction-content">' +
							'<div class="task-interaction-meta">' +
								'<a class="user-name" href="' + d.user.ProfileDisplayUrl + '">' + d.user.SecureShortName + '</a>' +
								'<span class="post-date">' + d.post.PostDate + '</span>' +
							'</div>' +
							'<div class="task-interaction-text">' +
								d.post.PostContentFormatted +
							'</div>' +
						'</div>' +

					'</div>').insertBefore(target);

					// increase reply count
					/*
					var replies = button.closest('.task-interaction').find('.total-replies'),
					count = parseInt(replies.html(), 10);

					replies.html((count + 1).toString());
					*/

					// clean up
					button.closest('.add-reply').find('textarea').prop('disabled', false).val('');
					button.prop('disabled', false);

					// set status
					if (d.isCompleted) {
						setTaskStatus(taskID, 1);
					}

					hideLoading();

				} else {
					alert('there was an error posting');
					console.log(d);
				}
			}
		});

		return false;
	});

	// admin functions
	$('.task-interaction-thread-toggle').click(function () {

		showLoading('Updating thread');

		var toggle = $(this),
			thread = toggle.closest('.task-interaction-thread'),
			threadID = thread.data('threadid'),
			isDeleted = thread.hasClass('thread-deleted');

		// send data
		$.ajax({
			url: '/courses/api/deletethread',
			type: 'POST',
			data: {
				threadID: threadID,
				isDeleted: !isDeleted
			},
			success: function () {
				if (isDeleted) {
					thread.removeClass('thread-deleted');
					toggle.html('Delete Thread');
				} else {
					thread.addClass('thread-deleted');
					toggle.html('Restore Thread');
				}

				hideLoading();

			}
		});

	});


	$('.task-interaction-post-toggle').click(function () {

		showLoading('Updating post');

		var toggle = $(this),
			post = toggle.closest('.task-interaction-post'),
			postID = post.data('postid'),
			isDeleted = post.hasClass('post-deleted');

		// send data
		$.ajax({
			url: '/courses/api/deletepost',
			type: 'POST',
			data: {
				postID: postID,
				isDeleted: !isDeleted
			},
			success: function () {
				if (isDeleted) {
					post.removeClass('post-deleted');
					toggle.html('Delete Post');
				} else {
					post.addClass('post-deleted');
					toggle.html('Restore Post');
				}

				hideLoading();

			}
		});

	});



});            // document.ready



function sendTaskData(taskID, data, callback) {

	showLoading('Saving...');

	$.ajax({
		url: '/courses/api/taskstatus/' + taskID,
		type: 'POST',
		data: data,
		success: function (x) {
			hideLoading();
			callback(x);
		}
	});
}

function setTaskStatus(taskID, status) {
	var area = $('#task-' + taskID + '');

	switch (status) {
		case 1:
			area.find('.task-status')
				.removeClass('status-overdue')
				.removeClass('status-notstarted')
				.removeClass('status-skipped')
				.addClass('status-completed');
			break;
		default:
		case 0:
			area.find('.task-status')
				.removeClass('status-overdue')
				.removeClass('status-completed')
				.removeClass('status-skipped')
				.addClass('status-notstarted')
				.find('span')
					.html('Completed');
			break;
		case -1:
			area.find('.task-status')
				.removeClass('status-overdue')
				.removeClass('status-notstarted')
				.removeClass('status-completed')
				.addClass('status-skipped');
			break;
	}
}