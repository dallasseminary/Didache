
function sendTaskData(userID, taskID, data, callback) {
	$.ajax({
		url: '/courses/api/taskstatus/' + userID + '/' + taskID,
		type: 'POST',
		data: data,
		success: callback
	});
}
function setTaskStatus(userID, taskID, status) {
	var area = $('#task-' + userID + '-' + taskID + '');

	switch (status) {
		case 1:
			area.find('.block-list-item-label')
				.removeClass('notstarted')
				.removeClass('skipped')
				.addClass('completed')
				.find('span')
					.html('Completed');
			break;
		default:
		case 0:
			area.find('.block-list-item-labe')
				.removeClass('completed')
				.removeClass('skipped')
				.addClass('notstarted')
				.find('span')
					.html('Completed');
			break;
		case -1:
			area.find('.block-list-item-labe')
				.removeClass('notstarted')
				.removeClass('completed')
				.addClass('skipped')
				.find('span')
					.html('Completed');
			break;
	}

	/*
	switch (status) {
	case 1:
	area.find('.task-status')
	.removeClass('status-notstarted')
	.removeClass('status-skipped')
	.addClass('status-completed')
	.html('Completed');
	break;
	default:
	case 0:
	area.find('.task-status')
	.removeClass('status-completed')
	.removeClass('status-skipped')
	.addClass('status-notstarted')
	.html('Not Started');
	break;
	case -1:
	area.find('.task-status')
	.removeClass('status-notstarted')
	.removeClass('status-completed')
	.addClass('status-skipped')
	.html('Skipped');
	break;
	}
	*/
}



jQuery(document).ready(function ($) {
	// date editors
	$('input.date, input[type=date]').datepicker();

});


