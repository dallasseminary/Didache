<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Course>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Dashboard
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% Html.RenderPartial("CourseHeader"); %>

<% Html.RenderPartial("CourseMenu"); %>

<ul>
<% foreach (Didache.Unit unit in Model.Units) { %>
	<li>
		<%: unit.SortOrder + ". " + unit.Name %>
		<ul>
		<% foreach (Task task in unit.Tasks) { %>
			<li>
				<%: task.Name %>
			</li>
		<% } %>
		</ul>	
	</li>
<% } %>
</ul>

</asp:Content>
