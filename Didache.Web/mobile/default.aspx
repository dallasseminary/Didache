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
		
	string username = Username.Text;
	string password = Password.Text;
	string userdata = "";
	bool rememberMe = RememberMe.Checked;

	string returnUrl = Request["ReturnUrl"];
	string loginUrl = Request["LoginUrl"];

	MembershipUser user = null;
	TempProfile profile = null;
			
	if (FindUser(username, out user, out profile)) {

		username = user.UserName;
		
		// Membership.ValidateUser
		//if ((username=="cjones" || username=="mrodriguez" || username =="bbittiker" || username == "johndyer") && password == "password")
		if (Membership.ValidateUser(username, password)) {

			FormsAuthenticationTicket fat = new FormsAuthenticationTicket(
				2,
				username,
				DateTime.Now.AddMinutes(-5),
				DateTime.Now.AddMinutes(20160),
				rememberMe,
				userdata,
				FormsAuthentication.FormsCookiePath);

			HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
			cookie.Value = FormsAuthentication.Encrypt(fat);
			cookie.Expires = fat.Expiration;
			cookie.Domain = FormsAuthentication.CookieDomain;

			Response.Cookies.Add(cookie);

			// allow cross-app redirects is true now
			FormsAuthentication.SetAuthCookie(username, rememberMe);
			// don't redirect?
			Response.Redirect(Request.Url.ToString());

		} else {
			// invalid login
			ResetMessage.Text = "Invalid Login";
			//MessageHolder.Visible = true;
		}				
				
	}
	

}


public bool FindUser(string usernameEmailOrId, out MembershipUser user, out TempProfile profile) {

	user = null;
	profile = null;

	string username = "";
	int userID = 0;

	// is this an email?
	if (usernameEmailOrId.IndexOf("@") > -1) {

		string email = usernameEmailOrId;

		// Email 1: check for email in Membership table
		string membershipUsername = Membership.GetUserNameByEmail(email);

		if (membershipUsername != null) {
			// use the membership username to find dts ID
			username = membershipUsername;

			user = Membership.GetUser(username);
			profile = GetProfileByUsername(username);

			if (user != null && profile != null)
				return true;
			else
				return false;
		} else {
			// Email 2: check for profile email (might be different from membership)
			profile = GetProfileByEmail(usernameEmailOrId);

			// Email 3: check for email in employees table
			if (profile == null)
				profile = GetProfileByDtsEmail(usernameEmailOrId);

			if (profile != null) {
				username = profile.Username;
				user = Membership.GetUser(username);
				return true;
			} else {
				return false;
			}
		}

		// check for userid in login
	} else if (Int32.TryParse(usernameEmailOrId, out userID)) {

		profile = GetProfileByUserID(userID);

		if (profile != null) {
			username = profile.Username;
			user = Membership.GetUser(username);
			return true;
		} else {
			return false;
		}
	} else {
		// not an email, not a number

		user = Membership.GetUser(usernameEmailOrId);

		if (user != null) {
			profile = GetProfileByUsername(user.UserName);
			return true;
		} else {
			return false;
		}

	}


}

public class TempProfile {

	private int _userID = 0;
	private string _email = "";
	private string _firstName = "";
	private string _lastName = "";
	private string _username = "";
	private string _dtsEmail = "";

	public int UserID {
		get { return _userID; }
		set { _userID = value; }
	}
	public string Email {
		get { return _email; }
		set { _email = value; }
	}
	public string FirstName {
		get { return _firstName; }
		set { _firstName = value; }
	}
	public string LastName {
		get { return _lastName; }
		set { _lastName = value; }
	}
	public string Username {
		get { return _username; }
		set { _username = value; }
	}
	public string DtsEmail {
		get { return _dtsEmail; }
		set { _dtsEmail = value; }
	}

}

TempProfile GetProfileByUsername(string username) {
	return GetProfile("SELECT * FROM dts_cars_Users LEFT OUTER JOIN dts_cars_Users_Employees ON dts_cars_Users.UserID = dts_cars_Users_Employees.UserID WHERE dts_cars_Users.username = @username;", "username", SqlDbType.NVarChar, username);
}

TempProfile GetProfileByEmail(string email) {
	return GetProfile("SELECT * FROM dts_cars_Users LEFT OUTER JOIN dts_cars_Users_Employees ON dts_cars_Users.UserID = dts_cars_Users_Employees.UserID WHERE dts_cars_Users.email = @email;", "email", SqlDbType.NVarChar, email);
}

TempProfile GetProfileByUserID(int userID) {
	return GetProfile("SELECT * FROM dts_cars_Users LEFT OUTER JOIN dts_cars_Users_Employees ON dts_cars_Users.UserID = dts_cars_Users_Employees.UserID WHERE dts_cars_Users.userID = @userID;", "userID", SqlDbType.Int, userID);
}

TempProfile GetProfileByDtsEmail(string dtsEmail) {
	return GetProfile("SELECT * FROM dts_cars_Users LEFT OUTER JOIN dts_cars_Users_Employees ON dts_cars_Users.UserID = dts_cars_Users_Employees.UserID WHERE dts_cars_Users.userID = (SELECT UserID FROM dts_cars_Users_Employees WHERE dtsEmail = @dtsEmail);", "dtsEmail", SqlDbType.NVarChar, dtsEmail);
}

TempProfile GetProfile(string sql, string parameterName, SqlDbType sqlDbType, object value) {
	TempProfile profile = null;

	SqlConnection sqlConnection = new SqlConnection(baseConnection);
	SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
	sqlCommand.Parameters.Add(parameterName, sqlDbType).Value = value;

	sqlConnection.Open();
	SqlDataReader reader = sqlCommand.ExecuteReader();
	if (reader.Read()) {
		profile = new TempProfile();
		profile.UserID = (int)reader["UserID"];
		profile.Email = (string)reader["Email"];
		profile.FirstName = (string)reader["FirstName"];
		profile.LastName = (string)reader["LastName"];
		profile.Username = (string)reader["Username"];
		profile.DtsEmail = (Convert.IsDBNull(reader["DtsEmail"])) ? "" : (string)reader["DtsEmail"];
	}
	reader.Close();
	sqlConnection.Close();

	return profile;
}


/*
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
		Response.Redirect("/playermobile/");
		
	} else if(loginStatus == LoginUserStatus.InvalidCredentials) { // Invalid Credentials
		//Error.Text = "Invalid creditials. Please note that your username is now <b>case sensitive</b> (as of 4/12/2004).";	
	} else if(loginStatus == LoginUserStatus.UnknownError) { // Unknown error because of miss-syncronization of internal data
		//Error.Text = "unknown error.";	
	} else {
		//Error.Text = "Your username or password was not found.";	
	}
}	
*/
	
</script>

<asp:Content ContentPlaceHolderID="MainContent" runat="ServeR">
	
<div data-role="page"> 


	<div data-role="header">
		<h1>DTS Mobile</h1>
	</div> 
		

	<div data-role="content">
		
	<asp:placeholder ID="LoginHolder" runat="server">
	
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

			<asp:button Text="Login" id="LoginButton" onClick="LoginButton_Click" CssClass="FormButton" runat="server" />
		</div>				

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