<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Didache.UserTaskData>" %>


<% if (Model.TaskCompletionStatus == TaskCompletionStatus.NotStarted) { %>
	
	<form method="post" enctype="multipart/form-data" action="/courses/api/taskfile/<%= Model.UserID %>/<%= Model.TaskID %>">
		<input type="file" name="StudentFile" />
		<input type="submit" value="Turn In Assignment" />
	</form>

	<% if (Model.Task.IsSkippable) { %>
		<input type="submit" name="TaskStatus" value="Skipped"  />

	<% }  %>	

<% }  %>


