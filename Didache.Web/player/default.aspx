<!DOCTYPE html>
<html>
<head>
	<meta http-equiv="X-UA-Compatible" content="edge" />

	<title>DTS HTML5 player</title>

	<meta name="viewport" content="width=device-width, height=device-height, user-scalable=yes" />
	<meta name="apple-mobile-web-app-capable" content="yes">
	
	<link rel="apple-touch-icon" href="dts-mobile-icon.png" type="image/png" />
	<link href="dts-player.css" rel="stylesheet" />
		
	<script src="jquery.js"></script>
	<script src="mediaelement.js"></script>
	<script src="dts-player.js"></script>
	<script src="app.js"></script>
</head>
<body class="landscape-smallvideo">

	<div id="video1-frame" class="video-display-abs">	
		<video id="video1-view" width="320" height="240" type="video/mp4"></video>
	</div>

	<div id="player-container" class="player-container">

		<div id="video-selector" class="video-selector">				
			<a href="http://www.dts.edu/" title="Dallas Theological Seminary"><img src="dts-flame-shadow.png" width="66" height="24" alt="Dallas Theological Seminary - flame logo" /></a>
			<select class="course-list"></select>
			<select class="unit-list"></select>
			<select class="video-list"></select>
			<input type="checkbox" id="use-lo" /><label for="use-lo">Lo</label>
			<select class="language-list"></select>
		</div>

		<div id="title">
			<h1>Online Education Player</h1>
			<div class="downloads">				
				<a href="javascript:void(0);" class="download-transcript" >transcript</a>	
				<a href="javascript:void(0);" class="download-slides" >slides</a>	
				<a href="javascript:void(0);" class="download-video" >download video</a>								
			</div>
		</div>

		<div class="content-frame">

			<div id="video-frame" class="video-frame">
			
				
				<div id="video1-frame" class="video-display">	
					<%--			
					<video id="video1-view" width="320" height="240"></video>
					--%>
				</div>			
				
				
				<div id="video1-controls" class="media-controls"><div class="media-controls-inner">
					
					<div id="video1-playpause" class="media-playpause" title="Play/Pause">						
						<span class="play">play</span>
						<span class="pause" style="display:none;">pause</span>
					</div>
										
					<span id="video1-currentTime" class="media-currentTime">00:00</span><span id="video1-duration" class="media-duration">00:00</span>			

					<div id="video1-mute" class="media-mute" title="Mute">
						<span class="mute">mute</span>
						<span class="unmute" style="display:none;">unmute</span>					
					</div>
					
					<div id="video1-toggle-transcript" class="media-toggle-transcript" title="Toggle Transcript">
						<span class="transcript-on">hide cc</span>
						<span class="transcript-off" style="display:none;">show cc</span>
					</div>
					
					<div id="video1-progress-wrapper" class="media-progress-wrapper" >			
						<div id="video1-loaded" value="0" max="100" class="media-loaded"></div>
						<div id="video1-progress" value="0" max="100" class="media-progress"></div>												
						<span class="media-progress-handle"></span>
					</div>	
					
					<div id="video1-fullscreen" class="media-fullscreen"></div>
								
				</div></div>

				<div id="transcript" class="transcript">
					<div class="highlighter"></div>					
					<div class="text"></div>					
				</div>

				<div style="clear:both;"></div>
			</div>

			<div id="slides-frame" class="slides-frame">
				<div id="slide-display" class="slide-display">
					<br />
				</div>
				<div id="slide-sorter" class="slide-sorter">
				
				</div>
				<div style="clear:both;"></div>
			</div>

			<div style="clear:both;"></div>
			<div id="info"></div>
		</div>

		<div class="orientations">
			<a id="orientation-smallvideo" href="javascript:void(0);"><img src="widescreen-smallvideo.png" /></a>
			<a id="orientation-largevideo" href="javascript:void(0);"><img src="widescreen-largevideo.png" /></a>
			<a id="orientation-portrait" href="javascript:void(0);"><img src="portrait.png" /></a>
		</div>	
		
		<div id="report-problem">
			Problems with video, slides, or transcript?
		</div>		
				
		<div id="message-box1" class="message-box"><div class="message-box-inner">
			<div class="message-box-header">
				<div class="message-box-title">Message</div>
				<div class="message-box-close">X</div>
			</div>
			<div class="message-box-content">Content</div>
		</div></div>				
				
	</div>
	

	
<script type="text/javascript">

	var _gaq = _gaq || [];
	_gaq.push(['_setAccount', 'UA-64595-14']);
	_gaq.push(['_trackPageview']);

	(function () {
		var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
		ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
		var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
	})();

</script>

</body>
</html>