$(document).ready(function () {

	function loadUserGroups(courseID, callback) {
		$.ajax({
			url: '/api/getcourseusergroups/' + courseID,
			success: function (d) {

				renderUserGroups(d);

				setupUserSorting();
			}
		});
	}

	function renderUserGroups(userGroups) {

		var userGroupArea = $('#course-user-groups');

		for (var groupIndex in userGroups) {

			var userGroup = userGroups[groupIndex],
				userGroupRow = renderUserGroupRow(userGroup, userGroupArea);

			for (var userIndex in userGroup.Students) {
				renderUserRow(userGroup.Students[userIndex], userGroupRow);
			}

		}
	}

	function renderUserGroupRow(userGroup, userGroupArea) {
		var userGroupRow =
				$('<div class="course-user-group nested-item">' +
					'<div class="user-group-header nested-item-row">' +
		//'<span class="user-group-drag-handle drag-handle"></span>' +
						'<span class="name">' +
							'<input type="text" class="user-group-name" value=""  placeholder="name" />' +
						'</span>' +
						'<a class="user-group-edit edit-link" href="/admin/courses/userGroup/0">Edit</a>' +
		//'<a class="user-group-edit delete-link" href="/admin/courses/deleteuserGroup/0">Delete</a>' +
					'</div>' +
					'<div class="user-group-users nested-child-list">' +
					'</div>' +
				'</div>'
				);

		userGroupArea.append(userGroupRow);

		updateUserGroupRow(userGroupRow, userGroup);

		return userGroupRow;
	}

	function updateUserGroupRow(userGroupRow, userGroup) {
		userGroupRow.attr('data-groupid', userGroup.GroupID);
		userGroupRow.find('.user-group-name').val(userGroup.Name);
	}

	function renderUserRow(courseUser, userGroupRow) {

		var userRow = $(
			'<div class="course-user nested-child-item">' +
				'<span class="user-drag-handle drag-handle"></span>' +
				'<span class="user-name"></span>' +
				'<span class="user-id"></span>' +
			'</div>');

		userGroupRow.find('.user-group-users').append(userRow);

		updateUserRow(userRow, courseUser);
	}

	function updateUserRow(userRow, courseUser) {
		//userRow.data('userid', user.TaskID);
		userRow.attr('data-userid', courseUser.UserID);
		userRow.find('.user-name').html(courseUser.User.FormattedName);
		userRow.find('.user-id').html(courseUser.UserID);
	}

	// INLINE group editing
	$('#course-user-groups').delegate('.course-user-group input', 'change', function () {
		var 
			row = $(this).closest('.course-user-group'),
			group = {
				GroupID: row.data('groupid'),
				Name: row.find('.user-group-name').val()
			};

		saveUserGroup(group, function (d) {
			if (d.success) {
				row.find('.user-group-header').effect("highlight");
			} else {
				console.log('error' + d.error)
			}
		});
	});

	function saveUserGroup(group, callback) {

		$.ajax({
			url: '/admin/courses/UpdateUserGroup/',
			data: group,
			type: 'POST',
			success: function (d) {
				callback(d);
			},
			error: function (d) {
				console.log('error saving user', d, d.message, d.errors, d.model);
			}
		});
	}

	function loadUserGroup(groupID, callback) {

		$.ajax({
			url: '/api/GetCourseUserGroup/' + groupID,
			success: function (d) {
				callback(d);
			},
			error: function (d) {
				console.log('loadUserGroup error', groupID);
			}
		});
	}


	// Group Edit button
	$('#course-user-groups').delegate('.user-group-edit', 'click', function (e) {
		e.preventDefault();

		var groupid = $(this).closest('.course-user-group').data('groupid');

		loadUserGroup(groupid, function (obj) {

			fillEditor('#user-group-editor', obj);

			$('#user-group-editor').dialog('open');
		});

		return false;
	});


	$('#user-group-editor').dialog({
		autoOpen: false,
		height: 300,
		width: 300,
		modal: true,
		title: 'Group Editor',
		buttons: [
			{
				text: 'Save Group',
				click: function () {

					// serialize
					var 
						group = serializeEditor('#user-group-editor'),
						isNew = false;

					if (group.GroupID == '') {
						group.GroupID = 0;
						isNew = true;
					}

					saveUserGroup(group, function (data) {
						$('#user-group-editor').dialog("close");

						if (isNew) {
							var groupRow = $('#course-user-groups div[data-groupid="' + data.model.GroupID + '"]');
							renderUserGroupRow(data.model, groupRow);
							setupUserSorting();
						} else {
							var taskRow = $('#course-user-groups div[data-groupid="' + data.model.GroupID + '"]');
							updateUserGroupRow(taskRow, data.model);
						}
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


	// add new group
	$('#add-user-group').click(function (e) {
		e.preventDefault();

		clearEditor('#user-group-editor');
		$('#user-group-editor').dialog('open');
		$('#user-group-editor').find('[name="CourseID"]').val($('#CourseID').val());

		return false;
	});

	// sort units
	function setupUserSorting() {

		// sort tasks
		$('.user-group-users').sortable('destroy').sortable({
			handle: '.user-drag-handle',
			connectWith: '.user-group-users',
			axis: 'y',
			update: function (event, ui) {
				startSaveOrder();
			},
			start: cancelSaveOrder
		});

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
				groupArray = [],
				group,
				user;

			$('.course-user-group').each(function (groupSortOrder, groupElement) {

				// store the position of the group
				group = {
					groupid: parseInt($(groupElement).data('groupid'), 10),
					users: []
				};
				groupArray.push(group);

				$(groupElement).find('.course-user').each(function (userSortOrder, userElement) {

					user = {
						userid: parseInt($(userElement).data('userid'), 10)
					};

					// store the position of the file
					group.users.push(user);

				});
			});

			// save the group order
			console.log('group order', groupArray);

			$.ajax({
				url: '/admin/courses/UpdateUserGroupSorting/' + $('#CourseID').val(),
				data: JSON.stringify(groupArray),
				type: 'POST',
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


	var courseID = $('#CourseID').val();
	loadUserGroups(courseID, null);


	/** Search **/

	$('#user-search-button').click(function () {
		var searchText = $('#user-search-text').val();

		$.ajax({
			url: '/api/findusers/',
			data: { query: searchText },
			success: function (d) {

			}
		});
	});

});