<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<Didache.Unit>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Courses
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Courses</h2>

<table class="admin-list">
<thead>
	<tr>
		<th></th>
		<th>Sort</th>
		<th>Name</th>
		<th>Start</th>
		<th>End</th>
		<th></th>
		<th></th>
	</tr>
</thead>
<tbody>
	<% foreach (Didache.Unit unit in Model) { %>
	<tr>
		<td><%: unit.IsActive%></td>
		<td><%: unit.SortOrder %></td>
		<td><%: unit.Name %></td>
		<td><%: unit.StartDate.ToString("MM/dd/yyyy")%></td>
		<td><%: unit.EndDate.ToString("MM/dd/yyyy")%></td>
		<td><a href="/admin/courses/tasks/<%= unit.UnitID %>">tasks</a></td>
		<td><a href="/admin/courses/editunit/<%= unit.UnitID %>">edit</a></td>
	</tr>
	<% } %>




</tbody>
</table>

</asp:Content>
