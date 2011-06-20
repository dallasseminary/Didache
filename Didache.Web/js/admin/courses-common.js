﻿function clearEditor(id) {
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
		var field = $(id + ' [name="' + prop + '"]'),
			val = (obj[prop] != null) ? obj[prop] : '';

		if (field.attr('type') == 'checkbox') {
			field.prop('checked', val);
		} else if (field.attr('type') == 'radio') {
			field.parent().find('[value="' + val + '"]').prop('checked', true);
		} else {
			field.val(val.toString().replace(' 12:00:00 AM', ''));
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