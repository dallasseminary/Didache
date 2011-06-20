
function loadCourse(courseID, callback) {
	$.ajax({
		url: '/api/getcourse/' + courseID,
		success: function (d) {

			if (callback)
				callback(d);
		}
	});
}

function saveCourse(course, callback) {
	$.ajax({
		type: 'POST',
		url: '/admin/courses/updatecourse/',
		data: course,
		success: function (d) {

			//console.log('Saved coures', d, callback);

			if (callback)
				callback(d);
		}
	});
}



function loadUnit(unitID, callback) {
	$.ajax({
		url: '/api/getunit/' + unitID,
		success: function (d) {
			if (callback)
				callback(d);
		}
	});
}

function saveUnit(unit, callback) {
	$.ajax({
		type: 'POST',
		url: '/admin/courses/updateunit/',
		data: unit,
		success: function (d) {

			if (callback)
				callback(d);

		}
	});
}

function loadTask(taskID, callback) {
	$.ajax({
		url: '/api/gettask/' + taskID,
		success: function (t) {
			callback(t);
		}
	});
}


function saveTask(task, callback) {
	$.ajax({
		type: 'POST',
		url: '/admin/courses/updatetask/',
		data: task,
		success: function (d) {
			if (callback)
				callback(d);
		}
	});
}



function loadCourseFileGroup(groupID, callback) {
	$.ajax({
		url: '/api/getcoursefilegroup/' + groupID,
		success: function (d) {

			if (callback)
				callback(d);
		}
	});
}

function saveCourseFileGroup(group, callback) {
	$.ajax({
		type: 'POST',
		url: '/admin/courses/UpdateCourseFileGroup/',
		data: group,
		success: function (d) {

			//console.log('Saved group', d, callback);

			if (callback)
				callback(d);
		}
	});
}