<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Didache.Task>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    EditTask
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>EditTask</h2>

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm()) { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <legend>Task</legend>

        <%: Html.HiddenFor(model => model.TaskID) %>
		<%: Html.HiddenFor(model => model.UnitID)%>
		<%: Html.HiddenFor(model => model.CourseID)%>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.IsActive) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.IsActive) %>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.SortOrder) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.SortOrder) %>
            <%: Html.ValidationMessageFor(model => model.SortOrder) %>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.Name) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.Name) %>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.Instructions) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.Instructions) %>
            <%: Html.ValidationMessageFor(model => model.Instructions) %>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.Description) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.Description) %>
            <%: Html.ValidationMessageFor(model => model.Description) %>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.DueDate) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.DueDate) %>
            <%: Html.ValidationMessageFor(model => model.DueDate) %>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.IsSkippable) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.IsSkippable) %>
            <%: Html.ValidationMessageFor(model => model.IsSkippable) %>
        </div>

		<%--
        <div class="editor-label">
            <%: Html.LabelFor(model => model.TaskTypeName) %>
        </div>
        <div class="editor-field">
            <%= Html.DropDownList("TaskTypeName", new SelectList(Didache.TaskTypes.TaskTypeManager.GetTaskTypes(), "ClassName", "FriendlyName", Model.TaskTypeName))%>
        </div>
		--%>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.TaskTypeName) %>
        </div>
        <div class="editor-field">
			<% foreach (Didache.TaskTypes.TaskTypeInfo info in Didache.TaskTypes.TaskTypeManager.GetTaskTypes()) {%>
				<%= String.Format("<input type=\"radio\" value=\"{0}\" name=\"{1}\" id=\"{2}\"{4}><label for=\"{2}\">{3}</label>",
	info.FriendlyName, "TaskTypeName", "TaskTypeName-" + info.FriendlyName, info.FriendlyName, info.FriendlyName == Model.TaskTypeName ? " checked=\"checked\"" : "")  %><br />
			<% } %>
           
        </div>


        <p>
            <input type="submit" value="Save" />
        </p>
    </fieldset>
<% } %>

<div>
    <%: Html.ActionLink("Back to List", "Index") %>
</div>

</asp:Content>

