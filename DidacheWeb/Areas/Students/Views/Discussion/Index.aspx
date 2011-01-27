<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<Forum>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Discussion
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% Html.RenderPartial("CourseHeader"); %>

<% Html.RenderPartial("CourseMenu"); %>

<% foreach (Forum forum in Model) { %>

<div class="course-forum">
	<a href="/courses/<%= ViewContext.RouteData.Values["slug"] %>/discussion/forum/<%= forum.ForumID %>"><%= forum.Name %></a>
	<br />
	<%= forum.Description %>
</div>

<% } %>

</asp:Content>
