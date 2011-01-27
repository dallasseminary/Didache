<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<Course>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>My Courses</h2>

<ul>
<% foreach (Course course in Model) { %>

<li><a href="/courses/<%= course.Slug %>"><%= course.CourseCode %></a> (<%: course.Session.Name %>)</li>

<% } %>
</ul>

</asp:Content>
