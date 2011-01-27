<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Didache.Campus>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Edit</h2>



<% using (Html.BeginForm()) { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <legend>Campus</legend>

        <%: Html.HiddenFor(model => model.CampusID) %>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.IsActive) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.IsActive) %>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.CampusCode) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.CampusCode) %>
            <%: Html.ValidationMessageFor(model => model.CampusCode) %>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.Name) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.Name) %>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.SortOrder) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.SortOrder) %>
            <%: Html.ValidationMessageFor(model => model.SortOrder) %>
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

