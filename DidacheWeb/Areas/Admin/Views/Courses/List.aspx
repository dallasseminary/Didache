<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<Course>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Courses
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Courses</h2>

<table class="admin-list">
<thead>
	<tr>
		<th></th>
		<th>Code</th>
		<th>Sec</th>
		<th>Name</th>
		<th>Start</th>
		<th>End</th>
		<th></th>
		<th></th>
		<th></th>
		<th></th>
		<th></th>
		<th></th>
		<th></th>
	</tr>
</thead>
<tbody>
	<% foreach (Course course in Model) { %>
	<tr>
		<td><%: course.IsActive %></td>
		<td><%: course.CourseCode %></td>
		<td><%: course.Section %></td>
		<td><%: course.Name %></td>
		<td><%: course.StartDate.ToString("MM/dd/yyyy") %></td>
		<td><%: course.EndDate.ToString("MM/dd/yyyy")%></td>
		<td><a href="/admin/courses/files/<%= course.CourseID %>">files</a></td>
		<td><a href="/admin/courses/users/<%= course.CourseID %>">users</a></td>
		<td><a href="/admin/grading/<%= course.CourseID %>">grading</a></td>
		<td><a href="/admin/courses/groups/<%= course.CourseID %>">groups</a></td>
		<td><a href="/admin/courses/units/<%= course.CourseID %>">units</a></td>
		<td><a href="/admin/courses/edit<%= course.SessionID %>">edit</a></td>
	</tr>
	<% } %>




</tbody>
</table>

</asp:Content>
