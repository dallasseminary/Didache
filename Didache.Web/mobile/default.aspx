<%@ Page Language="C#" MasterPageFile="Mobile.Master" %>
<%@ Import Namespace="DTS.Online" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>

<script runat="server">


string baseConnection = ConfigurationManager.ConnectionStrings["didache"].ConnectionString;
string resetPasswordLink = "";

void Page_Load() {

		
	LoginHolder.Visible = !User.Identity.IsAuthenticated;

	CoursesRepeater.DataSource = DTS.Online.OnlineCourses.GetCourseGroups();
	CoursesRepeater.DataBind();
					
}


public void LoginButton_Click(Object sender, EventArgs e) {
	
	Page.Validate();
	if (!Page.IsValid) {
		return;
	}

	DidacheDb db = new DidacheDb();
	string username = Username.Text;
	string password = Password.Text;
	bool rememberMe = RememberMe.Checked;
	User user = null;
	
	// check on ID
	int id = 0;
	if (Int32.TryParse(username, out id)) {
		user = db.Users.Find(id);
		if (user != null) {
			username = user.Username;
		}
	}
		// check for email
	else if (username.IndexOf("@") > -1) {
		user = db.Users.SingleOrDefault(u => u.Email == username);
		if (user != null) {
			username = user.Username;
		}
	}

	if (Membership.ValidateUser(username, password)) {


		FormsAuthenticationTicket fat = new FormsAuthenticationTicket(2, username, DateTime.Now.AddMinutes(-5), DateTime.Now.AddMinutes(FormsAuthentication.Timeout.Minutes), rememberMe, username, FormsAuthentication.FormsCookiePath);
		HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
		cookie.Value = FormsAuthentication.Encrypt(fat);
		cookie.Expires = fat.Expiration;
		cookie.Domain = FormsAuthentication.CookieDomain; // ".dts.edu";

		Response.Cookies.Add(cookie);

		FormsAuthentication.SetAuthCookie(username, rememberMe);

		Response.Redirect("/mobile/");

	} else {
		Response.Write("invalid");
	}


}
</script>

<asp:Content ContentPlaceHolderID="MainContent" runat="ServeR">
	
<div data-role="page"> 


	<div data-role="header">
		<h1>DTS Mobile</h1>
	</div> 
		

	<div data-role="content">
		
	<asp:placeholder ID="LoginHolder" runat="server">
		<form runat="server" action="default.aspx">


		<p>
			<asp:literal id="ResetMessage" runat="serveR">You can preview the first two units of any class without logging in, 
			but you must login to see the full course.</asp:literal>
		</p>
			
		<div id="login" data-role="fieldcontain">
			<label for="Username">Username:</label>
			<asp:TextBox ID="Username" runat="server"  />
	
			<label for="Password">Password:</label>
			<asp:textbox id="Password" TextMode="password" runat="server" />

			<asp:CheckBox ID="RememberMe" Text="Keep Me Logged In" runat="server" />

			<asp:button Text="Login" id="LoginButton" onclick="LoginButton_Click" CssClass="FormButton" runat="server" />

			
		</div>	
		
		</form>			

	</asp:placeholder>			
		
		
	<asp:Repeater ID="CoursesRepeater" runat="server" enableviewstate="false">
		<ItemTemplate>
			<ul data-role="listview" data-inset="true">
				<li data-role="list-divider"><%# Eval("Name") %></li>
				<asp:Repeater DataSource='<%# Eval("Courses") %>' runat="server">
					<ItemTemplate>	
						<li>
							<a href="units.aspx?course=<%# Eval("CourseCode") %><%= (Request["admin"] +"" != "") ? "&admin=" + Request["admin"] : "" %>">
								<h3><%# Eval("CourseCode") %></h3>
								<p><%# Eval("DefaultCourseData.Title") %></p>
								<%-- <p><%# ((OnlineCourse) Container.DataItem).Languages["en-US"].Title %></p> --%>
							</a>
						</li> 
					</ItemTemplate>
				</asp:Repeater>								
			</ul>
		</ItemTemplate>
	</asp:Repeater>		
	

	
<!-- show login -->



	</div><!--/.content -->
	
</div><!--/.page -->

</asp:Content>