<%@ Control Language="C#" Inherits="Didache.TaskControls.VideoListing" %>


<div class="video-list">
<% foreach (Didache.TaskControls.VideoInfo v in Videos) { %>
	<div class="video">

		<div class="video-thumb"  title="You have watched about <%= (Double)v.Minutes*(Double)v.PercentComplete/100 %> out of the <%= v.Minutes %> minutes in this video">
			<div class="video-options-container" style="background-image:url(<%= v.ThumbnailUrl %>);">
				<div class="video-options">
					<a class="icon watchvideo" href="javascript:void(0);">watch</a>
					<a class="icon slides" href="javascript:void(0);">slides</a>
					<a class="icon transcript" href="javascript:void(0);">transcript</a>
				</div>
			</div>
			<div class="progress-outline" >
				<div class="progress-percent" style="width:<%= v.PercentComplete %>px"></div>
			</div>					
		</div>					
				
		<a href="javascript:void(0);" title="You have watched about <%= (Double)v.Minutes*(Double)v.PercentComplete/100 %> out of the <%= v.Minutes %> minutes in this video" >				
			<span class="video-title"><%= v.Title %></span>
			<span class="video-length"><%= v.FormattedDuration%></span>
		</a>
	</div>

<%} %>
</div>

<% Html.RenderPartial("TaskType-SimpleCompletion", Model); %>

