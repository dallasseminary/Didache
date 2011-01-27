<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    DataTest
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>DataTest</h2>

<ul>
<% foreach (Session session in Model) { %>

	<li>
		<%: session.Name %>
		<ul>
		<% foreach (Course course in session.Courses) { %>
			<li><%: course.CourseCode %></li>		
		<% } %>
		</ul>
	</li>

<% } %>
</ul>

</asp:Content>
