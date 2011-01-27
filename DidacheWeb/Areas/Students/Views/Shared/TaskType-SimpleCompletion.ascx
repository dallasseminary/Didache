﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Didache.UserTaskData>" %>

<% if (Model.TaskCompletionStatus != TaskCompletionStatus.Completed) { %>
	<input type="button" class="mark-completed" value="Mark Completed" />
<% } %>
	
<% if (Model.Task.IsSkippable && Model.TaskCompletionStatus == TaskCompletionStatus.NotStarted) { %>
	<input type="button" class="mark-skipped" value="Skip" />
<% }  %>	

<script>

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
}


jQuery(function ($) {
	var 
		userID = <%= Model.UserID %>,
		taskID = <%= Model.TaskID %>,
		area = $('#task-' + userID + '-' + taskID + '');
	
	area.find('.mark-completed').click(function () {
		var button = $(this);

		sendTaskData(userID,taskID,{TaskStatus: 1},function(d) {
			setTaskStatus(userID,taskID,1);
			button.hide();
		});
	});
	area.find('.mark-skipped').click(function () {
		var button = $(this);

		sendTaskData(userID,taskID,{TaskStatus: -1},function(d) {
			setTaskStatus(userID,taskID,-1);
			button.hide();
		});
	});
});
</script>

