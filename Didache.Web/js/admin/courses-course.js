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
							renderCourseRow(data.course, $('#course-list'));
						} else {
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


});