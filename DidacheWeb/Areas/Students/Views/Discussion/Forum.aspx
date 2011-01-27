<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Forum>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Forum
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% Html.RenderPartial("CourseHeader"); %>

<% Html.RenderPartial("CourseMenu"); %>


<table class="forum-threads">
	<thead>
		<tr>
			<th>Subject</th>
			<th>Started</th>
			<th>Last Post</th>
		</tr>
	</thead>
	<tbody>

<% foreach (Thread thread in Model.Threads) { %>
	<tr>
		<td><a href="/courses/<%= ViewContext.RouteData.Values["slug"] %>/discussion/thread/<%= thread.ThreadID %>"><%= thread.Subject %></a></td>
		<td><%= thread.ThreadDate %></td>
		<td><%= thread.LastPostSubject %> by <%= thread.LastPostUserName %></td>
	</tr>
<% } %>

	</tbody>
</table>

</asp:Content>
