<%@ Page Language="C#" %>
<%@ Import Namespace="DTS.My" %>
<%@ Import Namespace="AspNetForums" %>
<%@ Import Namespace="AspNetForums.Components" %>
<%@ Import Namespace="AspNetForums.Enumerations" %>
<script runat="server">
void Page_Load() {
	if (User.Identity.IsAuthenticated) {
		CoursesHolder.Visible = true;
		LoginHolder.Visible = false;
		// bind classes

	} else {
		CoursesHolder.Visible = false;
		LoginHolder.Visible = true;
	}	
}

public void LoginButton_Click(Object sender, EventArgs e) {

	AspNetForums.Components.User userToLogin2 = new AspNetForums.Components.User();

	userToLogin2.Username = Username.Text;
	userToLogin2.Password = Password.Text;
	userToLogin2.PasswordFormat = AspNetForums.Enumerations.UserPasswordFormat.ClearText;

	ForumsLogin(userToLogin2);
}

public void ForumsLogin(AspNetForums.Components.User userToLogin) {
	LoginUserStatus loginStatus = AspNetForums.Users.ValidUser(userToLogin);

	if (loginStatus == LoginUserStatus.Success) {
	
		FormsAuthentication.SetAuthCookie(userToLogin.Username, AutoLogin.Checked);				
		
	} else if(loginStatus == LoginUserStatus.InvalidCredentials) { // Invalid Credentials
		//Error.Text = "Invalid creditials. Please note that your username is now <b>case sensitive</b> (as of 4/12/2004).";	
	} else if(loginStatus == LoginUserStatus.UnknownError) { // Unknown error because of miss-syncronization of internal data
		//Error.Text = "unknown error.";	
	} else {
		//Error.Text = "Your username or password was not found.";	
	}
}	
	
</script>
<!DOCTYPE html>
<html>
<head>
	<title>DTS Player Mobile</title>
		<meta name="viewport" content="width=device-width" />
	<meta name="viewport" content="initial-scale=1.0" />
</head>
<body>
<form runat="server">

<asp:placeholder ID="LoginHolder" runat="server">
		<!-- show login -->

		<ul>
			<li>
				<label for="Username">Username</label>
				<asp:TextBox ID="Username" runat="server"  />
			</li>
			<li>
				<label for="Password">Password</label>
				<asp:textbox id="Password" TextMode="password" runat="server" />
			</li>
			<li>
				<asp:CheckBox ID="AutoLogin" Text="Remember Me" runat="server" />
			</li>
		</ul>

		<asp:button Text="Login" id="LoginButton" onClick="LoginButton_Click" CssClass="FormButton" runat="server" />

</asp:placeholder>
<asp:placeholder ID="CoursesHolder" runat="server">

		<!-- show list -->

		<asp:Repeater ID="CoursesRepeater">
			<ItemTemplate>
				<li><a href="default-mobile-unit.aspx">CODE - [title]</a></li>
			</ItemTemplate>
		</asp:Repeater>

</asp:placeholder>

</form>
</body>
</html>