<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<CourseUserGroup>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Groups
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Groups</h2>

<ul>
<% foreach (CourseUserGroup group in Model) { %>
<li><%= group.Name %>

	<ul>
		<% foreach (CourseUser user in group.Students) { %>
		<li><%= user.Profile.FullName %></li>
		<% } %>
	</ul>


</li>
<% } %>
</ul>
	   
</asp:Content>
