<%@ Page Language="C#" MasterPageFile="Mobile.Master" %>
<%@ Import Namespace="DTS.My" %>
<%@ Import Namespace="AspNetForums" %>
<%@ Import Namespace="AspNetForums.Components" %>
<%@ Import Namespace="AspNetForums.Enumerations" %>
<%@ Import Namespace="DTS.Online" %>

<script runat="server">
void Page_Load() {

	if (User.Identity.IsAuthenticated) {
		Response.Redirect("/mobile/");
	}
		
}


public void LoginButton_Click(Object sender, EventArgs e) {


}

	
</script>
<asp:Content ContentPlaceHolderID="MainContent" runat="ServeR">

<div data-role="page"> 

	<div data-role="header">
		<h1>DTS Mobile</h1>
	</div> 
	

	<div data-role="content">
		
		<div id="login" data-role="fieldcontain">
			<label for="Username">Username:</label>
			<asp:TextBox ID="Username" runat="server"  />
	
			<label for="Password">Password:</label>
			<asp:textbox id="Password" TextMode="password" runat="server" />

			<asp:CheckBox ID="AutoLogin" Text="Keep Me Logged In" runat="server" />

			<asp:button Text="Login" id="LoginButton" onClick="LoginButton_Click" CssClass="FormButton" runat="server" />
		</div>

	</div><!--/.content -->
	
</div><!--/.page -->

</asp:Content>