var Didache = window.Didache || {};
Didache.TaskTypes = [];

jQuery(document).ready(function ($) {

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

	var urlHash = self.document.location.hash.substring(1);
	if (urlHash) { $('div.task-interaction-thread[id=' + urlHash + '] > .task-interaction-list').show(); }

	// open/close threads
	$('div.task-interaction .total-replies').toggle(function () {
		$(this).closest('.task-interaction-thread').find('.task-interaction-list').slideDown();
	}, function () {
		$(this).closest('.task-interaction-thread').find('.task-interaction-list').slideUp();
	});
	$('.add-reply .collapse').click(function () {
		$(this).closest('.task-interaction-list').slideUp();
	});

	// do reply
	$('div.task-interaction input.reply-button').click(function () {

		var button = $(this).prop('disabled', true),
				taskID = button.closest('.task-entry').data('taskid'),
				text = button.closest('.add-reply').find('textarea').prop('disabled', true).val(),
				threadID = button.closest('.task-interaction-thread').data('threadid');

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
							'<a href="' + d.user.ProfileUrl + '">' +
								'<img src="' + d.user.ProfileImageUrl + '&width=30&height=30" alt="' + d.user.SecureName + '" />' +
							'</a>' +
						'</div>' +
						'<div class="task-interaction-content">' +
							'<div class="task-interaction-meta">' +
								'<a class="user-name" href="' + d.user.ProfileUrl + '">' + d.user.SecureName + '</a>' +
								'<span class="post-date">' + d.post.PostDate + '</span>' +
							'</div>' +
							'<div class="task-interaction-text">' +
								d.post.PostContentFormatted +
							'</div>' +
						'</div>' +

					'</div>').insertBefore(target);

					// increase reply count
					var replies = button.closest('.task-interaction').find('.total-replies'),
							count = parseInt(replies.html(), 10);

					replies.html((count + 1).toString());

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


	});

});



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