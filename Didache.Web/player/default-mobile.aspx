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
		
		List<OnlineCourseGroup> courseGroups = new List<OnlineCourseGroup>();
		
		courseGroups.Add(new OnlineCourseGroup("Biblical Studies"));
		courseGroups[0].Courses.Add(new OnlineCourse("BC101", "Christian Counseling", "Gary Barnes & Chip Dickens"));
		courseGroups[0].Courses.Add(new OnlineCourse("BE101", "Bible Study Methods and Hermeneutics ", "Mark Bailey"));
		courseGroups[0].Courses.Add(new OnlineCourse("BE102", "OT History I", "James Allman "));
		courseGroups[0].Courses.Add(new OnlineCourse("BE103", "OT History II", "Gene Pond "));
		courseGroups[0].Courses.Add(new OnlineCourse("BE104", "OT Prophets", "Stephen Bramer"));
		courseGroups[0].Courses.Add(new OnlineCourse("BE105", "The Gospels", "Charles Baylis"));
		courseGroups[0].Courses.Add(new OnlineCourse("BE107", "Hebrews, General Epistles and Revelation", "Stanley Toussaint"));
		courseGroups[0].Courses.Add(new OnlineCourse("BE109", "Ruth, Psalms, Jonah, and Selected Epistles", "Ron Allen"));
		courseGroups[0].Courses.Add(new OnlineCourse("BE510", "Life of Christ on Earth", "J. Dwight Pentecost"));
		courseGroups[0].Courses.Add(new OnlineCourse("BE547", "A Biblical Theology of Suffering, Disability, and the Church", "Multiple Speakers"));
		
		courseGroups.Add(new OnlineCourseGroup("Languages"));
		courseGroups[1].Courses.Add(new OnlineCourse("NT101", "Elements of Greek I", "Michael Burer"));
		courseGroups[1].Courses.Add(new OnlineCourse("NT102", "Elements of Greek II", "Michael Burer"));
		
		
		courseGroups.Add(new OnlineCourseGroup("Theological Studies"));
		courseGroups[2].Courses.Add(new OnlineCourse("ST101", "Intro to Theology", "Glenn Kreider"));
		courseGroups[2].Courses.Add(new OnlineCourse("ST102", "Trinitarianism", "J. Scott Horrell"));
		courseGroups[2].Courses.Add(new OnlineCourse("ST103", "Humanity and Sin", "Nathan Holsteen"));
		courseGroups[2].Courses.Add(new OnlineCourse("ST104", "Soteriology", "Robert Pyne"));		
		courseGroups[2].Courses.Add(new OnlineCourse("HT200", "History of Doctrine", "John Hannah"));
		courseGroups[2].Courses.Add(new OnlineCourse("NT111", "Intertestamental History", "John Grassmick"));
		
		courseGroups.Add(new OnlineCourseGroup("Practical Ministry"));
		courseGroups[3].Courses.Add(new OnlineCourse("CE102", "History and Philosophy of Christian Education" , "Mike Lawson"));
		courseGroups[3].Courses.Add(new OnlineCourse("CE310", "Administration in Christian Higher Education", "Kenneth Gangel"));
		courseGroups[3].Courses.Add(new OnlineCourse("PM101", "Spiritual Life", "Ramesh Richard"));
		courseGroups[3].Courses.Add(new OnlineCourse("PM102", "Evangelism", "Doug Cecil"));
		courseGroups[3].Courses.Add(new OnlineCourse("PM103", "Preaching I", "Timothy Warren"));
		courseGroups[3].Courses.Add(new OnlineCourse("RS101", "Orientation and Research Methods", "Buist Fanning"));	
		courseGroups[3].Courses.Add(new OnlineCourse("SL305", "Dynamics of Leadership", "Howard Hendricks"));
		courseGroups[3].Courses.Add(new OnlineCourse("WM101", "Intro to World Missions", "Mark Young"));

		CoursesRepeater.DataSource = courses;
		CoursesRepeater.DataBind();		
		
		
		

	} else {
		CoursesHolder.Visible = false;
		LoginHolder.Visible = true;
		
		
		
		
	}
	
	
	
	
}

public class OnlineCourseGroup {

	public OnlineCourseGroup(string name) {
		this.name = name;
		courses = new List<OnlineCourse>();

	}


    private string name;
    private List<OnlineCourse> courses;
   
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    
    public List<OnlineCourse> Courses
    {
        get { return courses; }
    }    
}

public class OnlineCourse
{
	public OnlineCourse(string code, string name, string professor) {
		this.name = name;
		this.code = code;
		this.professor = professor;

	}

    private string name;
    private string code;
    private string professor;
   
    public string Code
    {
        get { return code; }
        set { code = value; }
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

	public string Professor
    {
        get { return professor; }
        set { professor = value; }
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
		Response.Redirect("/player4/default-mobile.aspx");
		
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
	
	<link rel="stylesheet" href="http://code.jquery.com/mobile/1.0a1/jquery.mobile-1.0a1.min.css" />
	<script src="http://code.jquery.com/jquery-1.4.3.min.js"></script>
	<script src="http://code.jquery.com/mobile/1.0a1/jquery.mobile-1.0a1.min.js"></script>	
	
</head>
<body>
<form runat="server">

	
<div data-role="page"> 




<asp:placeholder ID="LoginHolder" runat="server">
		
	<div data-role="header">
		<h1>Login</h1>
	</div> 

	<div data-role="content">

		<!-- show login -->

		<div data-role="fieldcontain">
			<label for="Username">Username:</label>
			<asp:TextBox ID="Username" runat="server"  />
	
			<label for="Password">Password:</label>
			<asp:textbox id="Password" TextMode="password" runat="server" />

			<asp:CheckBox ID="AutoLogin" Text="Keep Me Logged In" runat="server" />

			<asp:button Text="Login" id="LoginButton" onClick="LoginButton_Click" CssClass="FormButton" runat="server" />
		</div>				

		

	</div>

</asp:placeholder>
<asp:placeholder ID="CoursesHolder" runat="server">

	<div data-role="header">
		<h1>Courses</h1>
	</div> 
	
	<div data-role="content">
		
		
		
		<asp:Repeater ID="CoursesRepeater">
			<ItemTemplate>
				<ul data-role="listview" data-inset="true">
			
					<li data-role="list-divider" role="heading"><%# Eval("Name") %></li>
				
				</ul>
			</ItemTemplate>
		</asp:Repeater>
		
		
	
	</div>
	
</asp:placeholder>

</div> 

</form>
</body>
</html>