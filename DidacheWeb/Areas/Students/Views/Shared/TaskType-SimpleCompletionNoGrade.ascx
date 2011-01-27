<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Didache.UserTaskData>" %>
<!-- same rendering as SimpleComlpletion, but the backend code will be different -->
<% Html.RenderPartial("TaskType-SimpleCompletion", Model); %> (no grade assigned)
