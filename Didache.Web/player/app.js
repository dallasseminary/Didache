jQuery(document).ready(function($) {


	console.log('APP START');

	window.showMessage = function(title, content) {
		/*
		$('#mesage-box1')
			.find('.message-box-title').html(title)
				.end()
			.find('.message-box-content').html(content)
				.end()
			.show();
			*/
		$('#message-box1').find('.message-box-title').html(title);	
		$('#message-box1').find('.message-box-content').html(content)
		$('#message-box1').show();			
	}
	window.hideMessage = function() {
		$('#message-box1').hide();
	}	
	$('#message-box1').find('.message-box-close').bind('click', hideMessage);

	if (window.location.hash == '' || window.location.hash == null) {
		
		var extra = '';
		
		var v = document.createElement('video');
		
		if (typeof(v.canPlayType) == 'undefined' || v.canPlayType('video/mp4') == '')
			extra = '<br /><br /><b>Please note:</b> for the best experience, we recommend <a href="http://google.com/chrome/" target="_blank">Google Chrome</a>, ' + 
				'<a href="http://apple.com/safari/" target="_blank">Apple Safari</a>, ' + 
				'or <a href="http://ie.microsoft.com/testdrive/" target="_blank">Internet Explorer 9</a>. ' + 
				'Other please be sure to update your <a href="http://get.adobe.com/flashplayer/">Flash player</a> to 10.3.';
	
		showMessage('Welcome!', 'Please select a class at the top. Then select a unit and video. ' + extra );
	}
	
	// IE Fixies (dropdownlist issues)
	if($.browser.msie && jQuery.browser.version <= 8) {
		$("select").mousedown(function() {					
			$(this).css("width","auto").siblings('select').hide();
		}).bind('change',function(){
			$(this).css("width","").siblings('select').show();
		});	
	}
	
	// fullscreen
	$('#video1-fullscreen').bind('click', function() {
		showMessage('Fullscreen',
					'To watch video fullscreen, there are two steps' + 
					'<ol>' + 						
						'<li>Hold down the Ctrl key (Command on the Mac) and press the + or - buttons to increase or decrease the size of the player.</li>' +
						'<li>Optional: Press F11 to put your browser in fullscreen mode (or Shift+Command+F on Mac Firefox)</li>' + 
						//'<li>Adjust the size of the video by pressing these two buttons:<br />' + 
						//	' <a href="javascript:void(0);" onclick="$(document.body).css(\'zoom\', parseFloat($(document.body).css(\'zoom\')) + 0.1);">+ BIGGER</a>' + 
						//	' <a href="javascript:void(0);" onclick="$(document.body).css(\'zoom\', 1.0);">Normal</a>' + 
						//	' <a href="javascript:void(0);" onclick="$(document.body).css(\'zoom\', parseFloat($(document.body).css(\'zoom\')) - 0.1);">- smaller</a>' + 
						//'</li>' + 
					'</ol>'
				);
	});
	
	
	// PROBLEMS
	$('#report-problem').bind('click', function() {
		
		if ( $('.video-list')[0].selectedIndex <= 0)
			return;
		
		video.pause();
		
		showMessage('Report a Problem',
					'<label for="problem-type">Type</label><br/>' + 
					'<select id="problem-type">' + 
						'<option value="Video">Video problem</option>' + 
						'<option value="Video">Transcript problem</option>' + 
						'<option value="Video">Slides problem</option>' + 
						'<option value="Video">Other</option>' + 
					'</select><br />' + 
					'<label for="problem-text">Description</label><br/>' + 
					'<textarea id="problem-text"></textarea>' + 
					'<input type="button" id="report-problem-button" value="Send Report" />'					
				);
		
		$('#problem-type').width(250);
		$('#problem-text').width(250).height(100);
		$('#report-problem-button').bind('click', function() {
			
			var course = 
				$('.course-list').val() + ' ' + 
				$('.unit-list').val() + ' ' + 
				$('.video-list').val();
				
			var transcript = 
				$('#transcript').find('.highlight').attr('id') + ' ' +
				$('#transcript').find('.highlight').html();

			var slide = 
				$('#slide-display').find('.highlight').attr('id') + ' ' + 
				$('#slide-display').find('.highlight').attr('src');

			
			$.ajax({
					type: 'POST',
					url: 'player-reporting.asmx/ReportProblem',

					data: 							 								 
								 'type=' + 	$('#problem-type').val() + '&' + 								 								 
								 'course=' + course + '&' + 
								 'time=' + 	$('#video1-currentTime').html() + '&' + 
								 'slide=' + 	slide + '&' +
								 'transcript=' + transcript + '&' + 								 
								 'text=' + 	$('#problem-text').val() + '',
					/*
					data: '{' + 
								 '"data":"test",' + 								 								 
								 '"type":"' + 	$('#problem-type').val() + '",' + 								 								 
								 '"course":"' + course + '",' + 
								 '"time":"' + 	$('#video1-currentTime').html() + '",' + 
								 '"slide":"' + 	slide + '",' +
								 '"transcript":"' + transcript + '", ' + 								 
								 '"text":"' + 	$('#problem-text').val() + '"' + 
								'}',
					*/
					//contentType: "application/json; charset=utf-8",
					dataType: "text/plain",
					success: function(xhr) {
						hideMessage();
					},
					error: function(xhr) {
						console.log('error',xhr);
					}
			});
			
			//hideMessage();
				
		
		});	
	});
	
	
	

	// setup the video player
	var video = null;
	new MediaElement('video1-view', {
		type:'video/mp4',
		plugins: ['flash'],
		enablePluginDebug: false,
		error: function () {
			console.log('fail');
		},		
		success: function(v) {
			console.log('video ready (HTML5, Flash, or Silverlight)');
				
			video = v;
			
			// setup user controls
			var dtsUserControls = new dts.DtsUserControls(video);
		
			// setup slides/transcript controller
			var dtsPlayerController = new dts.DtsPlayerController(video);

			// setup the dropdownlist controller	
			var dtsController = new dts.DtsCoursesController('video-selector', video,  dtsPlayerController);

			// add rate for admins
			if (window.location.search.indexOf('admin=true') > -1) {
				var pbRate = $('<input type="text" id="playback-rate" value="1.0" />');
				pbRate.width(25);
				$('#use-lo').before(pbRate);
				
				pbRate.bind('keyup', function() {
					video.playbackRate = $(this).val();
				});
			
			}				
		}
	});
		
	updateOrientation();

	function setOrientation(orientation) {
	
		var container = document.getElementsByTagName('body')[0];
	
		switch (orientation) {

			case 0:
			case 180:
				container.className = 'portrait-1';
				video.setVideoSize(640,480);	
				break;
			case 90:
				container.className = 'landscape-smallvideo';
				video.setVideoSize(320,240);
				break;

			case -90:
				container.className = 'landscape-largevideo';
				video.setVideoSize(480,360);
				break;
		}		
	}


	function updateOrientation() {
		/*window.orientation returns a value that indicates whether iPhone is in portrait mode, landscape mode with the screen turned to the
		left, or landscape mode with the screen turned to the right. */
		var orientation = window.orientation;

		setOrientation(orientation);

	}
	
	$('#orientation-smallvideo').bind('click', function() { setOrientation(90); });
	$('#orientation-largevideo').bind('click', function() { setOrientation(-90); });
	$('#orientation-portrait').bind('click', function() { setOrientation(0); });

	// Point to the updateOrientation function when iPhone switches between portrait and landscape modes.
	$(window).bind('orientationchange', function() { updateOrientation(); });
});