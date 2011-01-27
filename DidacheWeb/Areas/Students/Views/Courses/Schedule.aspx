<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Dashboard
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% Html.RenderPartial("CourseHeader"); %>

<% Html.RenderPartial("CourseMenu"); %>


<div id="main-column">
	
	<%: ViewBag.CurrentUnit != null ? "Unit " + ViewBag.CurrentUnit.SortOrder + ": " + ViewBag.CurrentUnit.Name : "" %>


	<div id="schedule-tasks">

	
	<% 	if (ViewBag.UserTasks != null) { %>
		<% foreach (Didache.UserTaskData userTaskData in ViewBag.UserTasks) { %>
		
		<div class="task-entry" id="task-<%= userTaskData.UserID %>-<%= userTaskData.TaskID %>">			
			
			<div class="task-info">
				<span class="task-status status-<%= userTaskData.TaskCompletionStatus.ToString().ToLower() %>"><%= userTaskData.TaskCompletionStatus %></span>				
			</div>

			<div class="task-details">
				<div class="task-instructions">
					<div class="task-name"><%= userTaskData.Task.Name %></div>
					
					<div class="task-instructions-text"><%= userTaskData.Task.Instructions %></div>					
				</div>	
											
				<div class="task-interaction">
					<% Html.RenderPartial("TaskType-" + userTaskData.Task.TaskTypeName + "", userTaskData); %>								
				</div>
			</div>

			<div class="clear"></div>
		</div>
		<% } %>
	<%} else { %>	
		
		Select a unit from the menu on the right

	<% }%>
	

	</div>
</div>
<div id="sub-column">
	<div id="schedule-units">
		<% if (ViewBag.Units != null) { %>
		<% foreach (Didache.Unit unit in ViewBag.Units) { %>
		
				<a href="/courses/<%= unit.Course.Slug %>/schedule/<%= unit.UnitID %>" <%: ViewBag.CurrentUnit != null && unit.UnitID == ViewBag.CurrentUnit.UnitID ? "class=\"selected\"" : "" %>>
					<%: unit.SortOrder + ". " + unit.Name%>
					<br />
					<%: unit.StartDate.ToString("MM/dd/yyyy")%> - <%: unit.EndDate.ToString("MM/dd/yyyy")%>
				</a>
				<br />
		<% } %>	
		<% } %>
	</div>

</div>

<script>
$('.task-instructions-text').expander({
  slicePoint: 100
  ,widow: 2
  //,expandEffect: 'show'
  , expandText: 'more'
  //, userCollapse: false
  ,userCollapseText: '[collapse]'
});
</script>


</asp:Content>
