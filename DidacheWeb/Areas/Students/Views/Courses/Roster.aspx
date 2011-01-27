<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<CourseUserGroup>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Dashboard
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% Html.RenderPartial("CourseHeader"); %>

<% Html.RenderPartial("CourseMenu"); %>

ROSTER

<div id="main-column">

	<div id="course-roster">

	<% foreach (Didache.CourseUserGroup group in Model) { %>
		<div class="course-group">
			<span class="course-group-name"><%: group.Name %></span>
			<div class="course-group-members">
			<% foreach (CourseUser user in group.Students) { %>
				<div class="course-group-member">				
					<img src="<%: user.Profile.ProfileImageUrl %>" />
					<span><%: user.Profile.FullName %></span>
				</div>
			<% } %>
			</div>
		</div>
	<% } %>

	</div>

</div>

</asp:Content>
