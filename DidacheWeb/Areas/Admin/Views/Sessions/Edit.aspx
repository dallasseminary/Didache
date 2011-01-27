<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Didache.Session>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Session
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Session</h2>

<script src="<%: Url.Content("~/js/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/js/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm()) { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <legend>Session</legend>

        <%: Html.HiddenFor(model => model.SessionID) %>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.IsActive) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.IsActive) %>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.SessionCode) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.SessionCode) %>
            <%: Html.ValidationMessageFor(model => model.SessionCode) %>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.Name) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.Name) %>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.StartDate) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.StartDate) %>
            <%: Html.ValidationMessageFor(model => model.StartDate) %>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.EndDate) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.EndDate) %>
            <%: Html.ValidationMessageFor(model => model.EndDate) %>
        </div>

        <p>
            <input type="submit" value="Save" />
        </p>
    </fieldset>
<% } %>

<div>
    <%: Html.ActionLink("Back to Sessions", "Sessions")%>
</div>

</asp:Content>

