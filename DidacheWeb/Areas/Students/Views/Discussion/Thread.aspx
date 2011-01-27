<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Thread>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Thread
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% Html.RenderPartial("CourseHeader"); %>

<% Html.RenderPartial("CourseMenu"); %>

<a href="/courses/<%= ViewContext.RouteData.Values["slug"] %>/discussion/forum/<%= Model.Forum.ForumID %>"><%= Model.Forum.Name%></a>
- 
<%= Model.Subject %>


<table class="forum-posts">
	<tbody>

<% foreach (Post post in Model.Posts) { %>
	<tr>
		<th>			
			<%= post.User.FormattedName %>
			<br />
			<img src="http://www.dts.edu/images/carsphotos/photo.ashx?id=<%= post.UserID %>" alt="<%= post.UserName %>" class="post-author" />
		</th>
		<td>
			<div class="date">
				<%= post.PostDate %>
			</div>
			<div class="content">
				<%= post.PostContentFormatted %>
			</div>
		</td>
	</tr>
<% } %>

	</tbody>
</table>

<form method="post" action="/courses/<%= ViewContext.RouteData.Values["slug"] %>/discussion/reply/<%= Model.ThreadID %>">
	 <input type="hidden" id="ThreadID" name="ThreadID" value="<%= Model.ThreadID %>" />
	 <input type="hidden" id="ForumID" name="ForumID" value="<%= Model.ForumID %>" />
	 <input type="hidden" id="ReplyToPostID" name="ReplyToPostID" value="<%= Model.LastPostID %>" />

	 <table>
		<tbody>
			<tr>
				<td>Subject</td>
				<td>
					<input type="text" id="Subject" name="Subject" value="RE: <%= Model.LastPostSubject %>" />					
				</td>
			</tr>
			<tr>
				<td>Message</td>
				<td>
					<textarea id="PostContent" name="PostContent"></textarea>
				</td>
			</tr>
		</tbody>
	 </table>

	 <input type="submit" value="Reply" />

</form>

</asp:Content>
