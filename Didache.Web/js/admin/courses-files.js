$(document).ready(function () {

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


	var courseID = $('#CourseID').val();
	loadFileGroups(courseID, null);
});