$(document).ready(function () {

	/*********** List of classes **************/

	// create course editor for add/edit
	$('#course-editor').dialog({
		autoOpen: false,
		height: 300,
		width: 550,
		modal: true,
		title: 'Course Editor',
		buttons: [
			{
				text: 'Save',
				click: function () {

					// serialize
					var 
						course = serializeEditor('#course-editor'),
						isNew = false;

					if (course.CourseID == '') {
						course.CourseID = 0;
						isNew = true;
					}

					//console.log('saving', course);

					saveCourse(course, function (data) {
						$('#course-editor').dialog("close");

						if (isNew) {
							renderCourseRow(data.course, $('#course-editor table'));
						} else {
							console.log($('#course-list tr[data-courseid="' + data.course.CourseID + '"]'), data.course);
							updateCourseRow($('#course-list tr[data-courseid="' + data.course.CourseID + '"]'), data.course);
						}



						// update row?
					});
				}
			},
			{
				text: 'Cancel',
				click: function () {
					$('#course-editor').dialog("close");
				}
			}
		]
	});

	// add new
	$('#course-create').click(function (e) {
		e.preventDefault();

		clearEditor('#course-editor');
		$('#course-editor').dialog('open');

		return false;
	});


	// edit
	$('#course-list').delegate('.course-edit', 'click', function (e) {
		e.preventDefault();

		var courseID = $(this).closest('.course-task').data('courseid');

		loadCourse(courseID, function (data) {

			//console.log(data);

			fillEditor('#course-editor', data);

			$('#course-editor').dialog('open');

		});

		return false;
	});

	// inline editing
	$('#course-list').delegate('input', 'change', function () {
		var 
			isActive = $(this),
			row = isActive.closest('tr'),
			course = {
				CourseID: row.data('courseid'),
				IsActive: isActive.is(':checked')
			}

		//console.log('saving', course);

		saveCourse(course, function (data) {
			row.effect('highlight', null, 1000);

		});
	});


	/******** units and tasks */

	$('#task-editor').dialog({
		autoOpen: false,
		height: 550,
		width: 550,
		modal: true,
		title: 'Task Editor',
		buttons: [
			{
				text: 'Save Task',
				click: function () {

					// serialize
					var 
						task = serializeEditor('#task-editor'),
						isNew = false;

					if (task.TaskID == '') {
						task.TaskID = 0;
						isNew = true;
					}

					saveTask(task, function (data) {
						$('#task-editor').dialog("close");

						if (isNew) {
							var unitRow = $('#course-units div[data-unitid="' + task.UnitID + '"]');
							renderTaskRow(data.task, unitRow);

							setupUnitAndTaskSorting();
						} else {
							var taskRow = $('#course-units div[data-taskid="' + data.task.TaskID + '"]');
							updateTaskRow(taskRow, data.task);
						}



						// update row?
					});
				}
			},
			{
				text: 'Cancel',
				click: function () {
					$('#task-editor').dialog("close");
				}
			}
		]
	});

	$('#unit-editor').dialog({
		autoOpen: false,
		height: 300,
		width: 550,
		modal: true,
		title: 'Unit Editor',
		buttons: [
			{
				text: 'Save Unit',
				click: function () {

					// serialize
					var 
						unit = serializeEditor('#unit-editor'),
						isNew = false;

					if (unit.UnitID == '') {
						unit.UnitID = 0;
						unit.CourseID = $('#CourseID').val();
						isNew = true;
					}

					saveUnit(unit, function (data) {
						$('#unit-editor').dialog("close");

						if (isNew) {
							var list = $('#course-units');
							renderUnitRow(data.unit, list);

							setupUnitAndTaskSorting();
						} else {
							var unitRow = $('#course-units [data-unitid="' + data.unit.UnitID + '"]');
							updateUnitRow(unitRow, data.unit);
						}

					});
				}
			},
			{
				text: 'Cancel',
				click: function () {
					$('#unit-editor').dialog("close");
				}
			}
		]
	});

	// add new unit
	$('#add-unit').click(function (e) {
		e.preventDefault();

		clearEditor('#unit-editor');
		$('#unit-editor').dialog('open');
		$('#unit-editor').find('[name="CourseID"]').val($('#CourseID').val());

		return false;
	});


	// add new task
	$('#course-units').delegate('.add-task', 'click', function (e) {
		e.preventDefault();

		var button = $(this);

		clearEditor('#task-editor');
		$('#task-editor').dialog('open');
		$('#task-editor').find('[name="UnitID"]').val(button.closest('.course-unit').data('unitid'));
		$('#task-editor').find('[name="CourseID"]').val($('#CourseID').val());
		$('#task-editor').find('[name="Priority"]').val('0');
		$('#task-editor').find('input[value="Default"]').attr('checked', true);

		return false;
	});


	/******** rendering ***********/
	function renderCourseRow(course, courseTable) {

		var courseRow = $(
			'<tr class="course-task">' +
				'<td><input type="checkbox" class="course-isactive" /></td>' +
				'<td><span class="course-coursecode">code</span></td>' +
				'<td><span class="course-section">section</span></td>' +
				'<td><span class="course-name">name</span></td>' +
				'<td><span class="course-startdate">start</span></td>' +
				'<td><span class="course-enddate">end</span></td>' +
				'<td><a class="course-grading" href="#">grading</a></td>' +
				'<td><a class="course-files" href="#">files</a></td>' +
				'<td><a class="course-users" href="#">users</a></td>' +
				'<td><a class="course-units" href="#">uni/tasks</a></td>' +
				'<td><a class="course-edit" href="#">edit</a></td>' +
			'</tr>');

		courseTable.find('tbody').prepend(courseRow);

		updateCourseRow(courseRow, course);
	}

	function updateCourseRow(courseRow, course) {
		courseRow.data('courseid', course.CourseID);
		courseRow.find('.course-isactive').prop('checked', course.IsActive);
		courseRow.find('.course-coursecode').html(course.CourseCode);
		courseRow.find('.course-section').html(course.Section);
		courseRow.find('.course-name').html(course.Name);
		courseRow.find('.course-start-date').html(course.StartDate.toString().replace(' 12:00:00 AM', ''));
		courseRow.find('.course-end-date').html(course.EndDate.toString().replace(' 12:00:00 AM', ''));

		courseRow.find('.course-grading').attr('href', '/admin/courses/grading/' + course.CourseID);
		courseRow.find('.course-files').attr('href', '/admin/courses/files/' + course.CourseID);
		courseRow.find('.course-users').attr('href', '/admin/courses/users/' + course.CourseID);
		courseRow.find('.course-units').attr('href', '/admin/courses/courseeditor/' + course.CourseID);

		courseRow.find('.course-edit').attr('href', '/admin/courses/edit/' + course.CourseID);
		courseRow.find('.course-delete').attr('href', '/admin/courses/deletecourse/' + course.CourseID);
	}






	/******* loading ****************/

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


	function clearEditor(id) {
		var 
			fields = $(id).find('input,textarea,select');

		fields.each(function (x) {
			var field = $(this);

			if (field.attr('type') == 'checkbox' || field.attr('type') == 'radio') {
				field.prop('checked', false);
			} else if (field.prop('tagName') == 'select') {
				field[0].selectedIndex = 0;
			} else {
				field.val('');
			}
		});
	}

	function fillEditor(id, obj) {
		for (var prop in obj) {
			// TODO: checkbox, radios?
			var field = $(id + ' [name="' + prop + '"]');

			if (field.attr('type') == 'checkbox') {
				field.prop('checked', obj[prop]);
			} else if (field.attr('type') == 'radio') {
				field.parent().find('[value="' + obj[prop] + '"]').prop('checked', true);
			} else {
				field.val(obj[prop].toString().replace(' 12:00:00 AM', ''));
			}
		}
	}

	function serializeEditor(id) {
		var obj = {},
			fields = $(id).find('input,textarea,select');

		fields.each(function (x) {
			var field = $(this);
			if (field.attr('type') == 'checkbox') {
				obj[field.attr('name')] = field.prop('checked');
			} else if (field.attr('type') == 'radio') {
				if (field.prop('checked')) {
					obj[field.attr('name')] = field.val();
				}
			} else {
				obj[field.attr('name')] = field.val();
			}
		});

		return obj;
	}

	/**** unit list *****/


	var courseID = $('#CourseID').val();

	if (courseID > 0) {
		if (window.location.href.toString().toLowerCase().indexOf('courseeditor') > -1)
			loadUnits(courseID, null);

		if (window.location.href.toString().toLowerCase().indexOf('files') > -1)
			loadFileGroups(courseID, null);

		// GRADING?!

		// USERS?!
	}

	function loadUnits(courseID, callback) {
		$.ajax({
			url: '/api/getcourseunits/' + courseID,
			success: function (d) {

				renderUnits(d);

				setupUnitAndTaskSorting();
			}
		});
	}

	function renderUnits(units) {

		var unitArea = $('#course-units');

		for (var unitIndex in units) {

			var unit = units[unitIndex],
				unitRow = renderUnitRow(unit, unitArea);

			for (var taskIndex in unit.Tasks) {
				renderTaskRow(unit.Tasks[taskIndex], unitRow);
			}

		}
	}

	function renderUnitRow(unit, unitArea) {
		var unitRow =
				$('<div class="course-unit nested-item">' +
					'<div class="unit-header nested-item-row">' +
						'<span class="unit-drag-handle drag-handle"></span>' +
						'<span class="name">' +
							'<input type="checkbox" class="unit-active"  />' +
							'<input type="text" class="unit-name" value=""  placeholder="name" />' +
						'</span>' +
						'<span class="meta">' +
							'<input type="date" class="unit-start-date" value="" placeholder="start" />' +
							'<input type="date" class="unit-end-date" value="" placeholder="end" />' +
						'</span>' +
						'<a class="unit-edit edit-link" href="/admin/courses/unit/0">Edit</a>' +
						'<a class="unit-edit delete-link" href="/admin/courses/deleteunit/0">Delete</a>' +
					'</div>' +
					'<div>' +
						'<button type="button" class="add-task new-button">Add Task</button>' +
					'</div>' +
					'<div class="course-tasks nested-child-list">' +
					'</div>' +
				'</div>'
				);

		unitArea.append(unitRow);

		updateUnitRow(unitRow, unit);

		return unitRow;
	}

	function updateUnitRow(unitRow, unit) {
		//unitRow.data('unitid', unit.UnitID);
		unitRow.attr('data-unitid', unit.UnitID);
		unitRow.find('.unit-active').prop('checked', unit.IsActive);
		unitRow.find('.unit-name').val(unit.Name);
		unitRow.find('.unit-start-date').val(unit.StartDate == null ? '' : unit.StartDate.replace(' 12:00:00 AM', ''));
		unitRow.find('.unit-end-date').val(unit.EndDate == null ? '' : unit.EndDate.replace(' 12:00:00 AM', ''));
		unitRow.find('.unit-edit').attr('href', '/admin/courses/unit/' + unit.UnitID);
		unitRow.find('.unit-delete').attr('href', '/admin/courses/deleteunit/' + unit.UnitID);
	}

	function renderTaskRow(task, unitRow) {

		var taskRow = $(
			'<div class="course-task nested-child-item">' +
				'<span class="task-drag-handle drag-handle"></span>' +
                '<span class="name">' +
				    '<input type="checkbox" class="task-active"  />' +
				    '<input type="text" class="task-name" value=""  placeholder="name" />' +
                '</span>' +
		//'<span>'+
		//'<select name="task-type" class="task-type">'+
		//   ' <option></option>'+
		//'</select>'+
				    '<span class="task-type"></span>' +
					'<input type="date" class="task-due-date" value=""  placeholder="due"/>' +
		//'</span>'+
				'<a class="task-edit edit-link" href="/admin/courses/task/0">Edit</a>' +
				'<a class="task-delete delete-link" href="/admin/courses/deletetask/0">Delete</a>' +
			'</div>');

		unitRow.find('.course-tasks').append(taskRow);

		updateTaskRow(taskRow, task);
	}

	function updateTaskRow(taskRow, task) {
		//taskRow.data('taskid', task.TaskID);
		taskRow.attr('data-taskid', task.TaskID);
		taskRow.find('.task-active').prop('checked', task.IsActive);
		taskRow.find('.task-type').html(task.TaskTypeName);
		taskRow.find('.task-name').val(task.Name);
		taskRow.find('.task-due-date').val((task.DueDate != null) ? task.DueDate.replace(' 12:00:00 AM', '') : '');
		//taskRow.find('.task-due-date').val(task.DueDate);
		taskRow.find('.task-edit').attr('href', '/admin/courses/task/' + task.TaskID);
		taskRow.find('.task-delete').attr('href', '/admin/courses/deletetask/' + task.TaskID);
	}



	// SORTING
	// sort units
	function setupUnitAndTaskSorting() {
		$('#course-units').sortable('destroy').sortable({
			handle: '.unit-drag-handle',
			axis: 'y',
			update: function (event, ui) {
				startSaveOrder();
			},
			start: cancelSaveOrder
		}); //.disableSelection();

		// sort tasks
		$('.course-tasks').sortable('destroy').sortable({
			handle: '.task-drag-handle',
			axis: 'y',
			update: function (event, ui) {
				startSaveOrder();
			},
			start: cancelSaveOrder
		}); //.disableSelection();

		var saveTimeout = null;
		function startSaveOrder() {
			cancelSaveOrder();
			saveTimeout = setTimeout(saveOrderChanges, 1000);
		}
		function cancelSaveOrder() {
			if (saveTimeout != null) {
				clearTimeout(saveTimeout);
				delete saveTimeout;
				saveTimeout = null;
			}
		}

		function saveOrderChanges() {

			cancelSaveOrder();

			var 
				unitArray = [],
				unit,
				task;

			$('.course-unit').each(function (unitSortOrder, unitElement) {

				// store the position of the group
				unit = {
					unitid: parseInt($(unitElement).data('unitid'), 10),
					//name: $(unitElement).find('.unit-name').val(),
					sortorder: unitSortOrder + 1,
					tasks: []
				};
				unitArray.push(unit);

				$(unitElement).find('.course-task').each(function (taskSortOrder, taskElement) {

					task = {
						unitid: unit.unitid,
						taskid: parseInt($(taskElement).data('taskid'), 10),
						//name: $(taskElement).find('.task-name').val(),
						sortorder: taskSortOrder + 1
					};

					// store the position of the file
					unit.tasks.push(task);

				});
			});

			// save the group order
			console.log('group order', unitArray);
			console.log(JSON.stringify({ units: unitArray }));

			$.ajax({
				url: '/admin/courses/UpdateUnitSorting/' + $('#CourseID').val(),
				data: JSON.stringify(unitArray),
				type: 'POST',
				//dataType: 'json',
				success: function (d) {
					if (d.success) {
						console.log('saved order')
					} else {
						console.log('error' + d.error)
					}
				},
				error: function (d) {
					console.log('error saving order')
				}
			});
		}
	}

	// TASKS
	$('#course-units').delegate('.course-task input', 'change', function () {
		var 
				row = $(this).closest('.course-task'),
				task = {
					TaskID: row.data('taskid'),
					Name: row.find('.task-name').val(),
					DueDate: row.find('.task-due-date').val()
				};

		$.ajax({
			url: '/admin/courses/UpdateTask/',
			data: task,
			type: 'POST',
			success: function (d) {
				if (d.success) {
					console.log('saved task');
					row.effect("highlight");
				} else {
					console.log('error' + d.error)
				}
			},
			error: function (d) {
				console.log('error saving task', d, d.message, d.errors, d.model);
			}
		});
	});


	// UNITS
	$('#course-units').delegate('.unit-header input', 'change', function () {
		var 
				row = $(this).closest('.unit-header'),
				unit = {
					UnitID: row.closest('.course-unit').data('unitid'),
					Name: row.find('.unit-name').val(),
					StartDate: row.find('.unit-start-date').val(),
					EndDate: row.find('.unit-end-date').val()
				};

		$.ajax({
			url: '/admin/courses/UpdateUnit/',
			data: unit,
			type: 'POST',
			success: function (d) {
				if (d.success) {
					console.log('saved unit');
					row.effect("highlight");
				} else {
					console.log('error' + d.error)
				}
			},
			error: function (d) {
				console.log('error saving unit', d, d.message, d.errors, d.model);
			}
		});
	});

	// TASK major
	$('#course-units').delegate('.task-edit', 'click', function (e) {
		e.preventDefault();

		var taskID = $(this).closest('.course-task').data('taskid');

		loadTask(taskID, function (obj) {

			fillEditor('#task-editor', obj);

			$('#task-editor').dialog('open');
		});

		return false;
	});

	// UNIT major
	$('#course-units').delegate('.unit-edit', 'click', function (e) {
		e.preventDefault();

		var unitID = $(this).closest('.course-unit').data('unitid');

		loadUnit(unitID, function (obj) {

			fillEditor('#unit-editor', obj);

			$('#unit-editor').dialog('open');

		});

		return false;
	});


	/******* FILES **********/

	$('#file-group-editor').dialog({
		autoOpen: false,
		height: 300,
		width: 550,
		modal: true,
		title: 'File Group Editor',
		buttons: [
			{
				text: 'Save',
				click: function () {

					// serialize
					var 
						courseFileGroup = serializeEditor('#file-group-editor'),
						isNew = false;

					if (courseFileGroup.GroupID == '') {
						courseFileGroup.GroupID = 0;
						isNew = true;
					}

					saveCourseFileGroup(courseFileGroup, function (data) {
						$('#file-group-editor').dialog("close");

						if (isNew) {
							var row = renderFileGroupRow(data.courseFileGroup, $('#course-file-groups'));

							setupFileSorting();

							setupFileUploading(row.find('form'));
						} else {
							updateFileGroupRow($('#course-file-groups [data-groupid="' + data.courseFileGroup.GroupID + '"]'), data.courseFileGroup);
						}
					});
				}
			},
			{
				text: 'Cancel',
				click: function () {
					$('#file-group-editor').dialog("close");
				}
			}
		]
	});

	// add new
	$('#add-file-group').click(function (e) {
		e.preventDefault();

		clearEditor('#file-group-editor');

		$('#file-group-editor').find('[name="CourseID"]').val($('#CourseID').val());

		$('#file-group-editor').dialog('open');

		return false;
	});


	function loadFileGroups(courseID, callback) {
		$.ajax({
			url: '/api/GetCourseFileGroups/' + courseID,
			success: function (d) {

				renderFileGroups(d);

				setupFileSorting();

				setupFileUploading('.upload form');
			}
		});
	}

	function setupFileSorting() {
		// sort the file groups

		$('#course-file-groups').sortable('destroy').sortable({
			handle: '.group-handle',
			update: function (event, ui) {
				startSaveOrder();
			},
			start: cancelSaveOrder
		}).disableSelection();

		// sort the files
		$('.files').sortable('destroy').sortable({
			connectWith: '.files',
			handle: '.file-handle',
			update: function (event, ui) {
				startSaveOrder();
			},
			start: cancelSaveOrder
		}).disableSelection();

		var saveTimeout = null;
		function startSaveOrder() {
			cancelSaveOrder();
			saveTimeout = setTimeout(saveOrderChanges, 1000);
		}
		function cancelSaveOrder() {
			if (saveTimeout != null) {
				clearTimeout(saveTimeout);
				delete saveTimeout;
				saveTimeout = null;
			}
		}

		function saveOrderChanges() {

			cancelSaveOrder();

			var fileGroupArray = [],
			group,
			file;

			$('.filegroup').each(function (groupSortOrder, groupElement) {

				// store the position of the group
				group = {
					groupid: parseInt($(groupElement).data('groupid'), 10),
					//name: $(groupElement).find('.group-name').val(),
					sortorder: groupSortOrder + 1,
					files: []
				};
				fileGroupArray.push(group);

				$(groupElement).find('.coursefile').each(function (fileSortOrder, fileElement) {

					file = {
						groupid: group.groupid,
						fileid: parseInt($(fileElement).data('fileid'), 10),
						sortorder: fileSortOrder + 1
					};

					// store the position of the file
					group.files.push(file);

				});
			});

			// save the group order
			console.log('group order', fileGroupArray);
			console.log(JSON.stringify({ groups: fileGroupArray }));

			$.ajax({
				url: '/admin/courses/UpdateFileSorting/' + $('#CourseID').val(),
				data: JSON.stringify(fileGroupArray),
				//data: fileGroupArray,
				type: 'POST',
				complete: function (d) {

				}
			});
		}

	}

	function renderFileGroups(fileGroups) {

		var unitArea = $('#course-file-groups');

		for (var fileGroupIndex in fileGroups) {

			var fileGroup = fileGroups[fileGroupIndex],
				fileGroupRow = renderFileGroupRow(fileGroup, unitArea);

			for (var fileIndex in fileGroup.CourseFileAssociations) {
				renderFileRow(fileGroup.CourseFileAssociations[fileIndex], fileGroupRow);
			}

		}
	}

	function renderFileGroupRow(fileGroup, fileGroupArea) {
		var fileGroupRow = $(
			'<div class="filegroup nested-item">' +
				'<div class="nested-item-row">' +
					'<span class="group-handle drag-handle"></span>' +
		//'<span class="filegroup"><img src="javascript:void(0);"></span>' +
					'<input class="group-name" />' +
					'<a class="filegroup-edit edit-link" href="javascript:void(0);">Edit</a>' +
					'<a class="filegroup-delete delete-link" href="javascript:void(0);">Delete</a>' +
				'</div>' +
				'<div class="upload">' +
					'<form action="javascript:void(0);" enctype="multipart/form-data" method="post">' +
						'<input type="file" name="file" multiple>' +
						'<button>Upload</button>' +
						'<div>Click or drag and drop files</div>	' +
					'</form>' +
				'</div>' +
				'<div class="files nested-child-list"></div>' +
			'</div>'
		);

		fileGroupArea.append(fileGroupRow);

		updateFileGroupRow(fileGroupRow, fileGroup);

		return fileGroupRow;
	}

	function updateFileGroupRow(fileGroupRow, fileGroup) {
		//unitRow.data('unitid', unit.UnitID);
		fileGroupRow.attr('data-groupid', fileGroup.GroupID);
		//fileGroupRow.find('.unit-active').prop('checked', fileGroup.IsActive);
		fileGroupRow.find('.group-name').val(fileGroup.Name);

		fileGroupRow.find('form').attr('action', '/admin/courses/fileupload/' + fileGroup.CourseID);
	}

	function renderFileRow(file, fileGroupRow) {

		var fileRow = $(
			'<div class="coursefile nested-child-item">' +
				'<span class="file-handle drag-handle"></span>' +
                '<span class="file-type"><img src="javascript:void(0);"></span>' +
				'<span class="name">' +
					'<input type="checkbox" class="file-active"  />' +
					'<input type="text" class="file-title" />' +
				'</span>' +
				'<span class="file-user"></span>' +
				'<a class="file-edit edit-link" href="javascript:void(0);">Edit</a>' +
				'<a class="file-delete delete-link" href="javascript:void(0);">Delete</a>' +
			'</div>');

		fileGroupRow.find('.files').append(fileRow);

		updateFileRow(fileRow, file);
	}

	function updateFileRow(taskRow, file) {
		taskRow.attr('data-fileid', file.FileID);
		taskRow.find('.file-active').prop('checked', file.IsActive);
		taskRow.find('.file-type img').attr('src', '/css/images/' + file.CourseFile.FileType + '.png');
		taskRow.find('.file-title').val(file.CourseFile.Title);
		taskRow.find('.file-user').val((file.CourseFile.User && file.CourseFile.User.FullName) ? file.CourseFile.User.FullName : '');
		taskRow.find('.file-edit').attr('href', '/admin/courses/file/' + file.FileID);
		taskRow.find('.file-delete').attr('href', '/admin/courses/deletefile/' + file.FileID);
	}

	/// inline group edit
	$('#course-file-groups').delegate('.filegroup .nested-item-row input', 'change', function () {
		var 
				row = $(this).closest('.filegroup'),
				fileGroup = {
					GroupID: row.data('groupid'),
					Name: row.find('.group-name').val()
				};

		$.ajax({
			url: '/admin/courses/UpdateCourseFileGroup/',
			data: fileGroup,
			type: 'POST',
			success: function (d) {
				if (d.success) {
					console.log('saved file group');
					row.effect("highlight");
				} else {
					console.log('error' + d.error)
				}
			},
			error: function (d) {
				console.log('error saving group', d, d.message, d.errors, d.model);
			}
		});
	});

	// popup group edit
	$('#course-file-groups').delegate('.filegroup-edit', 'click', function (e) {
		e.preventDefault();

		var groupID = $(this).closest('.filegroup').data('groupid');

		loadCourseFileGroup(groupID, function (data) {

			//console.log(data);

			fillEditor('#file-group-editor', data);

			$('#file-group-editor').dialog('open');

		});

		return false;
	});

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

	function setupFileUploading(selector) {

		console.log('uploads for ', selector);

		// setup upload
		$(selector).fileUpload({
			formData: {
			},
			initUpload: function (event, files, index, xhr, handler, callback) {

				// from the file input go up to the top element
				var groupid = $(event.target).closest('.filegroup').data('groupid');

				handler.formData = { groupid: groupid }; // JSON.stringify({ groupid: groupid });

				console.log(groupid, files[index].name);

				callback();
			},
			onProgress: function (event, files, index, xhr, handler) {
				var percent = parseInt(event.loaded / event.total * 100, 10);

				/*
				handler
				.report
				.find('progress')
				.attr('value', percent);
				*/
			},
			// when the upload completes
			onLoad: function (event, files, index, xhr, handler) {
				var json = null;

				if (typeof xhr.responseText != 'undefined') {
					json = $.parseJSON(xhr.responseText);
				} else {
					// Instead of an XHR object, an iframe is used for legacy browsers:
					json = $.parseJSON(xhr.contents().text());
				}

				console.log(xhr, json);

				renderFileRow(json, $(handler.dropZone).closest('.filegroup'));

				/*
				// create node
				var node = $('<div class="coursefile" data-fileid="' + json.fileid + '">' +
						'<span class="file-handle drag-handle"></span>' +
						'<span class="file-title">' + json.title + '</span>' +
						'<span class="file-name">' + json.filename + '</span>' +
						'<span class="file-user">' + json.user + '</span>' +
					'</div>');

				// from the file input go up to the top element
				$(handler.dropZone).closest('.filegroup').find('.files').append(node);
				*/

			},
			onError: function (event, files, index, xhr, handler) {
				// For JSON parsing errors, the load event is saved as handler.originalEvent:
				if (handler.originalEvent) {
					/* handle JSON parsing errors ... */
				} else {
					/* handle XHR upload errors ... */
				}

				console.log('error uploading', event);
			}
		});

	}

});