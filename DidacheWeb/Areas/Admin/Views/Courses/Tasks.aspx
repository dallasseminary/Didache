<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<Didache.Task>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Courses
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Courses</h2>

<table class="admin-list">
<thead>
	<tr>
		<th>Active</th>
		<th>Skippable</th>
		<th>Sort</th>
		<th>Name</th>
		<th>Due</th>
		<th></th>
	</tr>
</thead>
<tbody>
	<% foreach (Didache.Task task in Model) { %>
	<tr>
		<td><%: task.IsActive%></td>
		<td><%: task.IsSkippable %></td>
		<td><%: task.SortOrder%></td>
		<td><%: task.Name %></td>
		<td><%: (task.DueDate.HasValue) ? task.DueDate.Value.ToString("MM/dd/yyyy") : "" %></td>
		<td><a href="/admin/courses/task/<%= task.TaskID %>">edit</a></td>
	</tr>
	<% } %>
</tbody>
</table>

</asp:Content>
