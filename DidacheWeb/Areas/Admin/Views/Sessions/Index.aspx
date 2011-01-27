<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Sessions</h2>

<a href="/admin/session/edit/0">New Session</a>

<table class="admin-list">
<thead>
	<tr>
		<th></th>
		<th>Code</th>
		<th>Name</th>
		<th>Start</th>
		<th>End</th>
		<th></th>
		<th></th>
	</tr>
</thead>
<tbody>
	<% foreach (Session session in Model) { %>
	<tr>
		<td><%: session.IsActive %></td>
		<td><%: session.SessionCode %></td>
		<td><%: session.Name %></td>
		<td><%: session.StartDate.ToString("MM/dd/yyyy") %></td>
		<td><%: session.EndDate.ToString("MM/dd/yyyy")%></td>
		<td><a href="/admin/courses/bysession/<%= session.SessionID %>">courses</a></td>
		<td><a href="/admin/sessions/edit/<%= session.SessionID %>">edit</a></td>
	</tr>
	<% } %>
</tbody>
</table>

</asp:Content>
