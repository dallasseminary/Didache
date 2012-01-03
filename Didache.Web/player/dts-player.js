/* 
FEATURES
- need to start a class by querystring
- volume control?
- remember orientation/volume/hi/lo/langauge
x progress bar interaction

BUGS
- widescreen gets crunched in Flash
- Silverlight can't resize, or reload?
- IE8 throws SSL error
- Changing orientation Firefox stops the video
x Changing orientation IE distorts the video
x Can't change languages on the fly
*/


(function () {

	function convertSecondsToTimecode2(seconds) {
		seconds = parseInt(seconds);

		if (seconds < 10) {
			return '00:0' + seconds.toString();
		} else if (seconds < 60) {
			return '00:' + seconds.toString();
		} else {
			var minutes = Math.round(seconds / 60);
			var seconds = Math.round(seconds % 60);

			return ((minutes < 10) ? '0' : '') + minutes.toString() +
				':' +
				((seconds < 10) ? '0' : '') + seconds.toString();
		}
	}

	function convertSecondsToTimecode(seconds) {
		seconds = Math.round(seconds);
		minutes = Math.floor(seconds / 60);
		minutes = (minutes >= 10) ? minutes : "0" + minutes;
		seconds = Math.floor(seconds % 60);
		seconds = (seconds >= 10) ? seconds : "0" + seconds;
		return minutes + ":" + seconds;
	}


	function convertTimecodeToSeconds(timecode) {
		timecode = timecode.replace(/&#xD;/g, '').replace(/&#xA;/g, '');

		var parts = timecode.split(':');

		if (parts.length == 3)
			return parseInt(parts[0], 10) * 360 +
					parseInt(parts[1], 10) * 60 +
					parseInt(parts[2], 10);
		else if (parts.length == 2)
			return parseInt(parts[0], 10) * 60 +
					parseInt(parts[1], 10);
		else if (parts.length == 1)
			return parseInt(parts[0], 10);
		else
			return -1;
	}

	// class=be101&unit=1 ==> {class: 'be1o1', unit: 1}
	function parseTokens(input) {
		var tokens = {};
		var parts = input.replace('#', '').split('&');
		for (var i in parts) {
			var p = parts[i].split('=');
			tokens[p[0]] = p[1];
		}
		return tokens;
	}

	// {class: 'be1o1', unit: 1} ==> class=be101&unit=1
	function tokensToString(tokens) {
		var parts = [];
		for (var i in tokens) {
			parts.push(i + '=' + tokens[i]);
		}
		return parts.join('&');
	}

	window.videoUtils = {
		convertSecondsToTimecode: convertSecondsToTimecode,
		convertTimecodeToSeconds: convertTimecodeToSeconds
	}



	function getCourseInfoUrl(courseCode, language) {
		//return 'en-US.xml';

		return 'video-list.ashx?course=' + courseCode.toString().toLowerCase() + '&language=' + language + '&' + document.location.search.replace('?', '');
		return '/playerfiles/' + courseCode.toString().toLowerCase() + '/Titles/' + language + '.xml';
	}

	function getVideoUrl(courseCode, unitNumber, videoNumber, isLow) {
		var unit = ((unitNumber < 10) ? '00' : '0') + unitNumber.toString();
		var video = ((videoNumber < 10) ? '00' : '0') + videoNumber.toString();
		var low = (isLow) ? '_lo' : '';

		//'https://oefiles.dts.edu/'
		// 'https://dtsoe.s3.amazonaws.com/' 
		return 'https://d16d701c6pwcqb.cloudfront.net/' + courseCode.toString().toLowerCase() + '/' + courseCode.toString().toUpperCase().replace('V2', 'v2') + '_u' + unit + '_v' + video + low + '.mp4';
	}

	function getTranscriptUrl(courseCode, unitNumber, videoNumber, language) {
		var unit = ((unitNumber < 10) ? '00' : '0') + unitNumber.toString();
		var video = ((videoNumber < 10) ? '00' : '0') + videoNumber.toString();

		//return 'NT113/Transcripts/en-US/nt113_u001_v001_transcript.xml';

		return '/playerfiles/' + courseCode.toString().toLowerCase() + '/Transcripts/' + language + '/' + courseCode.toString().toUpperCase().replace('V2', 'v2') + '_u' + unit + '_v' + video + '_transcript.xml';
	}

	function getSlidesUrl(courseCode, unitNumber, videoNumber, language) {
		var unit = ((unitNumber < 10) ? '00' : '0') + unitNumber.toString();
		var video = ((videoNumber < 10) ? '00' : '0') + videoNumber.toString();

		//return 'NT113/Slides/en-US/nt113_u001_v001_slides.xml';

		return '/playerfiles/' + courseCode.toString().toLowerCase() + '/Slides/' + language + '/' + courseCode.toString().toUpperCase().replace('V2', 'v2') + '_u' + unit + '_v' + video + '_slides.xml';
	}

	function getSlidesBasePath(courseCode, unitNumber, videoNumber, language) {
		var unit = ((unitNumber < 10) ? '00' : '0') + unitNumber.toString();
		var video = ((videoNumber < 10) ? '00' : '0') + videoNumber.toString();

		//return 'NT113/Slides/en-US/nt113_u001_v001_slides.xml';

		return '/playerfiles/' + courseCode.toString().toLowerCase() + '/Slides/' + language + '/';
	}

	var DtsUserControls = function (video) {

		if (navigator.userAgent.match(/iPad/i) != null) {
			
			video.setAttribute('controls','controls');
		}

		var loaded = false;

		var controls = $('#video1-controls');
		/*
		controls.css('opacity', 0.3);
		video.addEventListener('loadeddata', function() {
		controls.css('opacity', 1);
		});
		*/

		var playPauseButton = $('#video1-playpause');
		var muteButton = $('#video1-mute');
		var toggleTranscriptButton = $('#video1-toggle-transcript');

		var currentTime = $('#video1-currentTime');
		var duration = $('#video1-duration');

		var progressWrapper = $('#video1-progress-wrapper');
		var progreesLoaded = $('#video1-loaded');
		var progressPosition = $('#video1-progress');
		var progressHandle = $('.media-progress-handle');

		// PLAY/PAUSE button
		playPauseButton.bind('click', function () {
			console.log('play/pause clicked', video);
			if (video.paused)
				video.play();
			else
				video.pause();
		});

		function handlePlayerState(e) {
			video = e.target;

			console.log('state change: ' + e.type, e);

			switch (e.type) {
				case 'playing':
				case 'play':
					playPauseButton.find('.play').hide();
					playPauseButton.find('.pause').css('display', 'block');
					break;
				case 'paused':
				case 'pause':
					playPauseButton.find('.pause').hide();
					playPauseButton.find('.play').css('display', 'block');
					break;
				case 'loadstart':
					progreesLoaded.width('0%');
					progressPosition.width('0%');
			}
		}

		// events
		var events = 'loadstart abort loadedmetadata loadeddata play playing pause paused stop seeking seeked canplay canplaythrough waiting'.split(' ');
		for (var i = 0; i < events.length; i++) {
			console.log('adding', events[i]);
			video.addEventListener(events[i], handlePlayerState, true);
		}

		// MUTE: todo
		muteButton.toggle(

			function () {
				video.setMuted(true);
				$(this).find('.mute').hide();
				$(this).find('.unmute').css('display', 'block');
			},
			function () {
				video.setMuted(false);
				$(this).find('.mute').css('display', 'block');
				$(this).find('.unmute').hide();
			});

		// TRANSCRIPT
		toggleTranscriptButton.toggle(

			function () {
				$('#transcript').hide();
				$(this).find('.transcript-on').hide();
				$(this).find('.transcript-off').css('display', 'block');
			},
			function () {
				$('#transcript').show();
				$(this).find('.transcript-on').css('display', 'block');
				$(this).find('.transcript-off').hide();
			});


		// DURATION/PROGRESS
		video.addEventListener('timeupdate', function () {
			currentTime.html(convertSecondsToTimecode(video.currentTime));

			if (!isNaN(video.duration)) {
				duration.html(convertSecondsToTimecode(video.duration));

				// bars
				var percent = (video.currentTime / video.duration * 100);
				progressPosition.width(percent.toString() + '%');
			}
		}, false);


		video.addEventListener('progress', function (e) {

			if (e.target == null)
				return;

			var percent = 0;

			// flash/silverlight (html5 early browsers)
			if (!isNaN(e.target.loaded) && !isNaN(e.target.total)) {
				percent = (e.loaded / e.total);
				// html5 revision (safari 5 supports this, but chrome mis-reports it as always having 100% buffered)
			} else if (e.target.buffered && e.target.buffered.end) {
				try {
					percent = e.target.buffered.end() / e.target.duration;
				} catch (e) { }
			}

			try {
				progreesLoaded.width(progressWrapper.width() * percent);
			} catch (e) { }


		}, false);

		function handleProgressClick(e) {
			console.log('clicked progressWrapper');

			// mouse position relative to the object!
			var x = e.pageX,
				offset = progressWrapper.offset(),
				percentage = ((x - offset.left) / progressWrapper.outerWidth(true)),
				newTime = percentage * video.duration;

			video.setCurrentTime(newTime);
		}

		progressWrapper.bind('click', handleProgressClick);
		//progreesLoaded.bind('click', handleProgressClick);
		//progressPosition.bind('click', handleProgressClick);

	};


	var DtsCoursesController = function (id, player, mediaController) {

		/*
		var defaultLanguages = {};
		defaultLanguages['en-US'] = { name: "English", englishName: "English" };
		defaultLanguages['zh-TW'] = { name: "漢語(繁體字)t", englishName: "Traditional Chinese" };
		defaultLanguages['zh-CN'] = { name: "汉语(简体字)s", englishName: "Simplified Chinese" };
		defaultLanguages['es-ES'] = { name: "Español", englishName: "Spanish", dir: 'rtl' };
		defaultLanguages['ar-AR'] = { name: "العربية", englishName: "Arabic", dir: 'rtl' };
		*/

		var defaultLanguages = {
			"en-US": { "name": "English", "englishName": "English", dir: 'ltr' },
			"zh-TW": { "name": "漢語(繁體字)t", "englishName": "Traditional Chinese", dir: 'ltr' },
			"zh-CN": { "name": "汉语(简体字)s", "englishName": "Simplified Chinese", dir: 'ltr' },
			"es-ES": { "name": "Español", "englishName": "Spanish", dir: 'ltr' },
			"ar-AR": { "name": "العربية", "englishName": "Arabic", "dir": "rtl" }
		};

		var container = $('#' + id);
		var courseList = $('.course-list');
		var unitList = $('.unit-list');
		var videoList = $('.video-list');
		var languageList = $('.language-list');

		var downloadBtn = $('#title .downloads .download-video');
		downloadBtn.bind('click', function () {
			var course = getSelectedCourse();
			var unit = getSelectedUnit();
			var video = getSelectedVideo();
			var lang = getSelectedLanguage();

			// load video
			var loVideo = getVideoUrl(course, unit, video, true);
			var hiVideo = getVideoUrl(course, unit, video, false);

			if (videoList[0].selectedIndex > 0) {

				showMessage('Download Video',
					'<div class="video-downloads">' +
						'<a href="' + loVideo + '" class="button" target="_blank">Lo Quality Video</a>' +
						'<a href="' + hiVideo + '" class="button" target="_blank">Hi Quality Video</a>' +
						'<br />' +
						((unitList[0].options.length > 0) ? '<a href="/student/courserssfeed.aspx?course=' + course + '" class="button rss" target="_blank">Subscribe in iTunes</a>' : '') +
					'</div>'
				);
			}

		})


		var courseInfoData = [];
		var selectedCourseUnits = {};

		var isSwitchingLanguage = false;
		var isLoadingFromHash = false;
		var selectedUnit = 0;
		var selectedVideo = 0;

		function getSelectedLanguage() {
			return languageList.val();
		}

		function getSelectedCourse() {
			return courseList.val();
		}

		function getSelectedUnit() {
			return parseInt(unitList.val(), 10);
		}

		function getSelectedVideo() {
			return parseInt(videoList.val(), 10);
		}

		function loadAllLanguages() {

			var langHtml = '';
			for (var langCode in defaultLanguages) {
				langHtml += '<option value="' + langCode + '">' + defaultLanguages[langCode].name + '</option>';
			}
			languageList.html(langHtml);
		}

		function loadCourseInfoData() {

			courseList.attr('disabled', 'disabled');
			courseList.html('<option>Loading...</option>');
			unitList.attr('disabled', 'disabled');
			videoList.attr('disabled', 'disabled');

			$.ajax({
				type: 'GET',
				//url: 'dts-courses-list.xml',
				url: 'user-courses.ashx' + document.location.search,
				success: function (data) {

					console.log('Got classes list');

					// parse course structure
					var xml = $(data);

					xml.find('course').each(function () {
						var courseCode = $(this).attr('code');
						var courseInfo = [];

						$(this).find('language').each(function () {
							courseInfo.push({ code: $(this).attr('code'), name: $(this).attr('name') });
						});

						courseInfoData[courseCode] = courseInfo;
					});

					// now fill in the main dropdownlist
					loadCoursesByLanguage();

					loadClassFromHash();
				},
				error: function (e) {
					console.log('ERROR', e);
					alert('There was an error loading the class list');

				}
			});
		}

		function loadCoursesByLanguage() {
			// TODO: dynamic languages
			var selectedLang = getSelectedLanguage();

			// build HTML list of languages
			var htmlOptions = '';
			for (var courseCode in courseInfoData) {
				var courseLanguages = courseInfoData[courseCode];

				// see if course has this language, if so add it to the list
				for (var i = 0; i < courseLanguages.length; i++) {
					if (courseLanguages[i].code == selectedLang) {
						htmlOptions += '<option value="' + courseCode + '">' + courseCode.replace('v2', '') + ': ' + courseLanguages[i].name + ((courseCode.indexOf('v2') > -1) ? ' (2)' : '') + '</option>'; ;
					}
				}
			}

			courseList.prop('disabled', false);
			courseList.html('<option value="">--Select--</option>' + htmlOptions);
		}

		function loadClassFromHash() {
			var hashValues = parseTokens(document.location.hash);

			// language
			if (typeof (hashValues.language) != 'undefined') {
				languageList.val(hashValues.language);
				setLanguage();
			}

			// select course
			if (typeof (hashValues.course) != 'undefined') {

				// check of there is an option with the exact value
				if (courseList.find('option[value=' + hashValues.course + ']').length > 1) {
					courseList.val(hashValues.course);
				} else {
					// look for BE101 in the <option>s and then use that to select BE101v2

					console.log('checking for class');
					console.log('--' + courseList.find('option[value*=' + hashValues.course + ']').attr('value'));

					courseList.val(courseList.find('option[value*=' + hashValues.course + ']').attr('value'));
				}

				// store unit and video
				if (typeof (hashValues.unit) != 'undefined' && typeof (hashValues.video) != 'undefined') {

					selectedUnit = hashValues.unit;
					selectedVideo = hashValues.video;

					isLoadingFromHash = true;
				}

				loadUnits();
			}

			// TODO: the rest of the class
		}

		function updateHash() {
			document.location.hash = tokensToString({ course: courseList.val(), unit: unitList.val(), video: videoList.val(), language: languageList.val() });
		}

		function loadLanguagesForCourse() {

			// store the current one
			var currentLang = getSelectedLanguage();

			// re-fill with just the langauges for this course (or all)
			var courseCode = getSelectedCourse();
			if (courseCode == '') {
				loadAllLanguages();
			} else {
				var courseLanguages = courseInfoData[courseCode];

				var langHtml = '';
				for (var i = 0; i < courseLanguages.length; i++) {
					langHtml += '<option value="' + courseLanguages[i].code + '">' + defaultLanguages[courseLanguages[i].code].name + '</option>';
				}
				languageList.html(langHtml);
			}

			// attempt to reselect the same one
			languageList.val(currentLang);
		}

		function setLanguage() {

			var lang = getSelectedLanguage();

			$('body').attr('lang', lang);
			$('.transcript').attr('dir', defaultLanguages[lang].dir);

		}

		function loadUnits() {

			courseList.attr('disabled', 'disabled');

			// load videos
			unitList.html('<option>Loading...</option>');
			unitList.attr('disabled', 'disabled');
			//videoList.html('<option>Loading...</option>');
			videoList.attr('disabled', 'disabled');

			var selectedLang = getSelectedLanguage();
			var courseCode = getSelectedCourse();

			var url = getCourseInfoUrl(courseCode, selectedLang);

			$.ajax({
				url: url,
				success: function (data) {

					selectedCourseUnits = {};

					var xml = $(data);

					xml.find('unit').each(function () {
						var unitNode = $(this);
						var unitNumber = parseInt(unitNode.attr('number'));
						var unitInfo = {
							number: unitNumber,
							name: unitNode.attr('name'),
							videos: {}
						}

						unitNode.find('video').each(function () {
							var videoNode = $(this);

							var videoNumber = parseInt(videoNode.attr('number'));
							var videoInfo = {
								number: videoNumber,
								speaker: videoNode.attr('speaker'),
								name: videoNode.attr('name'),
								duration: videoNode.attr('duration')
							}

							unitInfo.videos[videoNumber] = videoInfo;
						});

						selectedCourseUnits[unitNumber] = unitInfo;

					});

					// reenable the course list
					courseList.prop('disabled', false);

					// now fill the unit list
					var optionsHtml = '';
					for (var unitNumber in selectedCourseUnits) {
						optionsHtml += '<option value="' + unitNumber + '">' + unitNumber.toString() + '. ' + selectedCourseUnits[unitNumber].name + '</option>';
					}
					unitList.html('<option value="">--Select--</option>' + optionsHtml);
					unitList.prop('disabled', false);

					if (isSwitchingLanguage || isLoadingFromHash) {
						// select the same unit
						unitList.val(selectedUnit);

						// do the next step
						loadVideos();
					}

				},
				error: function (e) {
					console.log('ERROR', e);
					alert('There was an error loading this classes units and video ' + url + ' ' + e);
				}
			});
		}

		function loadVideos() {
			var selectedUnitNumber = getSelectedUnit();
			var selectedUnit = selectedCourseUnits[selectedUnitNumber];

			//console.log('unit changed', selectedUnit, selectedUnit.videos);

			// now fill the video list
			var optionsHtml = '';
			for (var videoNumber in selectedUnit.videos) {
				//console.log(videoNumber);
				optionsHtml += '<option value="' + videoNumber + '">' + videoNumber.toString() + '. ' + selectedUnit.videos[videoNumber].name + ' (' + selectedUnit.videos[videoNumber].speaker + ') ' + selectedUnit.videos[videoNumber].duration.substring(3, selectedUnit.videos[videoNumber].duration.indexOf('.')) + '</option>';
			}
			videoList.html('<option value="">--Select--</option>' + optionsHtml);
			videoList.prop('disabled', false);

			if (isSwitchingLanguage || isLoadingFromHash) {
				// select the same unit
				videoList.val(selectedVideo);

				// do the next step
				loadClass();
			}
		}

		function loadClass() {
			var useLo = $('#use-lo')[0].checked;
			var course = getSelectedCourse();
			var unit = getSelectedUnit();
			var video = getSelectedVideo();
			var lang = getSelectedLanguage();

			// load video
			var videoUrl = getVideoUrl(course, unit, video, useLo);
			var slidesUrl = getSlidesUrl(course, unit, video, lang);
			var transcriptUrl = getTranscriptUrl(course, unit, video, lang);
			var slidesPath = getSlidesBasePath(course, unit, video, lang);

			// change the title
			var videoNode = selectedCourseUnits[unit].videos[getSelectedVideo()];
			$('#title h1').html(course + ': ' + videoNode.name + ' with ' + videoNode.speaker);

			// update download buttons
			//$('#title .downloads .download-video').attr('href', videoUrl);
			$('#title .downloads .download-slides')
				.attr('target', '_blank')
				.attr('href', '/player/print-slides.aspx?course=' + course + '&unit=' + unit + '&video=' + video + '&language=' + lang);
			$('#title .downloads .download-transcript')
				.attr('target', '_blank')
				.attr('href', '/player/print-transcript.aspx?course=' + course + '&unit=' + unit + '&video=' + video + '&language=' + lang);


			if (isSwitchingLanguage) {
				mediaController.changeLanguage(slidesUrl, transcriptUrl, slidesPath);

			} else {
				console.log('loading: ' + videoUrl);

				//player.setSrc(videoUrl);
				//player.load();

				mediaController.loadClass(videoUrl, slidesUrl, transcriptUrl, slidesPath);
			}

			// reset for next one
			isSwitchingLanguage = false;
			isLoadingFromHash = false;
			selectedUnit = 0;
			selectedVideo = 0;

			// update URL for snazzy users
			updateHash();
		}

		languageList.bind('change', function (e) {
			console.log('language changed', e);

			var lang = getSelectedLanguage();

			setLanguage();

			// if video is playing
			if (videoList[0].selectedIndex > 0) {

				isSwitchingLanguage = true;

				// store selected value
				selectedUnit = getSelectedUnit();
				selectedVideo = getSelectedVideo();

				// rebuild unit and video lists with new language
				loadUnits();

				// load new slides, transcript

			} else {

				// clear video, unit
				unitList.html('');
				unitList.attr('disabled', 'disabled');
				videoList.html('');
				videoList.attr('disabled', 'disabled');

				loadCoursesByLanguage();
			}
		}, false);

		courseList.bind('change', function (e) {
			loadLanguagesForCourse();
			loadUnits();
		});
		unitList.bind('change', function (e) {
			loadVideos();
		});
		videoList.bind('change', function (e) {
			loadClass();
		});

		// START UP
		loadAllLanguages();
		loadCourseInfoData();

	}


	var DtsPlayerController = function (player) {

		var slidesData = [];
		var transcriptData = [];

		var _lastTime = 0;

		player.addEventListener('timeupdate', function () {

			var currentTime = player.currentTime;

			// TODO: new, super efficient pattern?
			if (!isNaN(currentTime) && currentTime > 0) {
				// slids
				for (var i = 0; i < slidesData.length; i++) {
					var cueTime = slidesData[i].time;
					if (cueTime >= _lastTime && cueTime <= currentTime) {
						showSlide('s-' + i.toString());
						break;
					}
				}
				// ts
				for (var i = 0; i < transcriptData.length; i++) {
					var cueTime = transcriptData[i].time;
					if (cueTime >= _lastTime && cueTime <= currentTime) {
						showTranscriptLine('t-' + i.toString());
						break;
					}
				}

				_lastTime = currentTime;
			}

		}, false);

		function showTranscriptLine(lineName) {

			var number = parseInt(lineName.split('-')[1], 10);

			// unhighlight old
			$('.transcript').find('.highlight').removeClass('highlight'); ;

			// find this one and highlight it
			var l = $('#' + lineName).addClass('highlight');
			var newPos = l.outerHeight(true) * (number - 1);

			// scroll to it
			$('.transcript .text')
			//.attr('scrollTop',l.height(true) * (number - 1)  +'px');
					.animate({ 'scrollTop': newPos }, 250);

		}

		function showSlide(slideName) {
			console.log('show slide', slideName);

			// THUMB
			// unhlight selected thumb
			$('.slide-sorter').find('.highlight').removeClass('highlight'); ;

			// highlight current thumb
			$('#' + slideName + '-thumb').addClass('highlight');

			// SLIDEs
			// hide current slide
			var current = $('.slide-display').find('.highlight');
			console.log('- current slide', current.attr('id'), slideName);

			if (current.attr('id') != slideName) {
				current
					.removeClass('highlight')
					.fadeOut(500);
			}

			var slide = $('#' + slideName);

			if (!slide.hasClass('highlight')) {
				console.log('- showing slide', slide.attr('id'), slide.hasClass('highlight'));

				slide
					.addClass('highlight')
					.fadeIn(500, function () { console.log('- done fading', slide.attr('id')); });
			} else {
				console.log('- skipping slide', slide.attr('id'), slide.hasClass('highlight'));
			}


		}

		function slideClicked(e) {

			var index = parseInt(e.target.getAttribute('data-index'), 10);

			// quit and allow timing to handle it
			//showSlide('s-' + index.toString());

			var slide = slidesData[index];
			player.setCurrentTime(slide.time);

		}


		return {

			changeLanguage: function (slidesUrl, transcriptUrl, slidesPath) {
				this.load(true, '', slidesUrl, transcriptUrl, slidesPath);
			},

			loadClass: function (videoUrl, slidesUrl, transcriptUrl, slidesPath) {
				this.load(false, videoUrl, slidesUrl, transcriptUrl, slidesPath);
			},

			load: function (changeLanguage, videoUrl, slidesUrl, transcriptUrl, slidesPath) {

				console.log(changeLanguage, videoUrl, slidesUrl, transcriptUrl, slidesPath);

				// stop and clear
				if (!changeLanguage)
					player.pause();

				// always clear out the cues
				//player.clearCuePoints();

				// start loading video here to allow iPad
				/*
				if (videoUrl != '' && !changeLanguage) {
				player.setSrc(videoUrl);
				player.load();

				setTimeout(function () {
				console.log('load state');

				//player.load();
				//player.play();
				}, 100);
				}
				*/

				// reset
				slidesData = [];
				transcriptData = [];

				// clear ts and 
				$('.slide-display').html();
				$('.slide-sorter').html();
				$('.transcript .text').html();

				function loadSlides() {
					console.log('loading slides', slidesUrl);

					$.ajax({
						url: slidesUrl,
						success: function (data) {
							console.log('- received slide data', data);

							var doc = $(data);

							var slideNodes = doc.find('cue');
							console.log('total slides', slideNodes.length);
							console.log('-first slide', slideNodes[0]);

							slideNodes.each(function () {
								var slideNode = $(this);
								slidesData.push({
									time: window.videoUtils.convertTimecodeToSeconds(slideNode.attr('timeCode')),
									url: slidesPath + slideNode.attr('slideFileName')
								});
							});

							console.log('- done loading slides');
							loadTranscript();
						},
						error: function () {
							//alert('There was an error loading the slides for this class');

							console.log('- error loading slides');
							// skip to transcript
							loadTranscript();
						}

					});
				}

				function loadTranscript() {
					console.log('loading transcript', transcriptUrl);

					$.ajax({
						url: transcriptUrl,
						success: function (data) {
							console.log('- received transcript data', data);

							var doc = $(data);
							var textNodes = doc.find('cue');
							textNodes.each(function () {
								var textNode = $(this);
								transcriptData.push({
									time: window.videoUtils.convertTimecodeToSeconds(textNode.attr('timeCode')),
									text: textNode.attr('text')
								});
							});

							console.log('- done loading transcript');
							buildClass();
						},
						error: function () {
							//alert('There was an error loading the trnnscript for this class');

							// skip to setting up the class
							buildClass();
						}
					});
				}

				function buildClass() {

					console.log('building class');

					// add slide time
					// TODO: only load the image that's needed, pre-load, etc.
					var slideHtml = '';
					var slideSorterHtml = '';
					for (var i = 0; i < slidesData.length; i++) {
						var slideData = slidesData[i];
						slideHtml += '<img ' + ((i == 0) ? ' class="highlight"' : '') + ' id="s-' + i.toString() + '" src="' + slideData.url + '" style="z-index:' + i.toString() + ((i > 0) ? ';display: none;' : '') + '" />';
						//TODO; new cue
						//player.addCuePoint('s-' + i.toString(), slideData.time);

						slideSorterHtml += '<img ' + ((i == 0) ? ' class="highlight"' : '') + ' id="s-' + i.toString() + '-thumb" data-index="' + i.toString() + '" src="' + slideData.url + '" title="' + videoUtils.convertSecondsToTimecode(slideData.time) + '" alt"' + videoUtils.convertSecondsToTimecode(slideData.time) + '" />';
					}
					$('.slide-display')
						.html(slideHtml)
						.find('img')
							.hide()
							.first()
								.show();

					$('.slide-sorter')
						.html(slideSorterHtml);

					// add click events					
					for (var i = 0; i < slidesData.length; i++) {
						$('#s-' + i.toString() + '-thumb').bind('click', slideClicked);
					}

					// add transcript
					var transcriptHtml = '';
					for (var i = 0; i < transcriptData.length; i++) {
						var textData = transcriptData[i];
						transcriptHtml += '<span id="t-' + i.toString() + '">' + textData.text + '</span>';

						//TODO; new cue
						//player.addCuePoint('t-' + i.toString(), textData.time);
					}
					$('.transcript .text').html(transcriptHtml + '<span class="hilighter"></span>');


					//console.log('-loading video file: ' + videoUrl);

					hideMessage();

					//console.log('current ' + player.currentSrc);

					//player.play();


					if (videoUrl != '' && !changeLanguage) {
						console.log('current ' + player.currentSrc);
						console.log('loading ' + videoUrl);

						player.setSrc(videoUrl);
						player.load();

						console.log('after ' + player.currentSrc);

						
						setTimeout(function () {
							//player.load();
							player.play();
						}, 100);
					}

					/*
					setTimeout(function () {
					player.play();
					}, 250);
					*/
				}

				// start 
				loadSlides();
			}

		}


	}

	var dts = {
		DtsUserControls: DtsUserControls,
		DtsCoursesController: DtsCoursesController,
		DtsPlayerController: DtsPlayerController
	};

	window.dts = dts;

})();