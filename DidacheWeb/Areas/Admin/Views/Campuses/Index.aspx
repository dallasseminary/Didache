<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Didache.Campus>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Index</h2>

<p>
    <%: Html.ActionLink("Create New", "Create") %>
</p>
<table class="admin-list">
<thead>
    <tr>
        <th>Active</th>
        <th>CampusCode</th>
        <th>Name</th>
        <th>SortOrder</th>
    </tr>
</thead>
<tbody>
<% foreach (var item in Model) { %>
    <tr>
        <td>
            <%: item.IsActive %>
        </td>
        <td>
            <%: item.CampusCode %>
        </td>
        <td>
            <%: item.Name %>
        </td>
        <td>
            <%: item.SortOrder %>
        </td>
        <td>
            <td><a href="/admin/campuses/edit/<%= item.CampusID %>">edit</a></td>
        </td>
    </tr>  
<% } %>
</tbody>
</table>

</asp:Content>

