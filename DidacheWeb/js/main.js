jQuery(document).ready(function ($) {

	// main menu
	$('#main-menu a').click(function (e) {

		// stop click from working
		e.preventDefault();

		// change class
		var a = $(this);
		a.addClass('selected').parent().siblings().find('a').removeClass('selected');

		// show sub menu
		$('#' + a.attr('id') + '-details').addClass('selected').siblings().removeClass('selected');

	});


	// date editors
	$('input.date, input[type=date]').datepicker();


	// instruactions


});