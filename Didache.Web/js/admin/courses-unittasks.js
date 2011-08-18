$(document).ready(function () {


	$('#task-editor').dialog({
		autoOpen: false,
		height: 550,
		width: 550,
		modal: true,
		title: 'Task Editor',
		open: function (event, ui) {
			setupAdminEditors();
		},
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
		height: 400,
		width: 550,
		modal: true,
		title: 'Unit Editor',
		open: function (event, ui) {
			setupAdminEditors();
		},
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

		var button = $(this),
			unitID = parseInt(button.closest('.course-unit').data('unitid'));

		clearEditor('#task-editor');
		$('#task-editor').dialog('open');
		$('#task-editor').find('[name="UnitID"]').val(unitID);
		$('#task-editor').find('[name="CourseID"]').val($('#CourseID').val());
		$('#task-editor').find('[name="Priority"]').val('0');
		$('#task-editor').find('input[value="Default"]').attr('checked', true);

		fillRelatedTasks(unitID, 0);

		return false;
	});

	function fillRelatedTasks(unitID, excludeTaskID) {

		excludeTaskID = parseInt(excludeTaskID);

		var select =
			$('#task-editor').find('select[name="RelatedTaskID"]')
				.empty()
				.append($('<option value="0">--none--</option>'));



		// grab all tasks in the unit area
		$('div.course-unit[data-unitid="' + unitID.toString() + '"]').find('.course-task').each(function () {
			var taskRow = $(this),
				taskName = taskRow.find('input.task-name').val(),
				taskID = parseInt(taskRow.data('taskid'), 10);

			if (excludeTaskID !== taskID) {
				select.append($('<option value="' + taskID + '">' + taskName + '</option>'));
			}
		});
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
							'<span class="unit-sortorder">##</span>' +
							'<input type="checkbox" class="unit-active"  />' +
							'<input type="text" class="unit-name" value=""  placeholder="name" />' +
						'</span>' +
						'<span class="meta">' +
							'<input type="text" class="unit-start-date date" value="" placeholder="start" />' +
							'<input type="text" class="unit-end-date date" value="" placeholder="end" />' +
						'</span>' +
						'<a class="unit-edit edit-link" href="/admin/courses/unit/0">Edit</a>' +
						'<a class="unit-edit delete-link" href="/admin/courses/deleteunit/0">Delete</a>' +
					'</div>' +
					'<div>' +
						'<button type="button" class="add-task d-button action">Add Task</button>' +
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
				    '<span class="task-sortorder">##</span>' +
					'<input type="checkbox" class="task-active"  />' +
				    '<input type="text" class="task-name" value=""  placeholder="name" />' +
                '</span>' +
		//'<span>'+
		//'<select name="task-type" class="task-type">'+
		//   ' <option></option>'+
		//'</select>'+
				    '<span class="task-type"></span>' +
					'<input type="text" class="task-due-date date" value="" placeholder="due" />' +
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
	function updateUnitAndTaskNumbers() {

		$('.course-unit').each(function (i, el) {

			$(this).find('.unit-sortorder').html(i + 1);

			$(this).find('.course-task').each(function (j, el) {
				$(this).find('.task-sortorder').html(j + 1);
			});
		});
	}

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

		updateUnitAndTaskNumbers();


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

			showLoading('Saving order');

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

						hideLoading();

						updateUnitAndTaskNumbers();
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

		loadTask(taskID, function (task) {

			fillRelatedTasks(task.UnitID, task.TaskID);

			fillEditor('#task-editor', task);

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



	var courseID = $('#CourseID').val();
	loadUnits(courseID, null);


});