<%@ Page Language="C#" MasterPageFile="Mobile.Master" EnableViewState="False" %>
<%@ Import Namespace="DTS.Online" %>
<%@ Import Namespace="System.Xml" %>

<script runat="server">
string language = "";
void Page_Load() {

	language = (String.IsNullOrEmpty(Request["language"]+"")) ? "en-US" : Request["language"]+"";
	OnlineCourse course = OnlineCourses.GetCourse(Request["course"]+"");
	int unitNumber = 0;
	int videoNumber = 0;
	
	if ( 
		Int32.TryParse(Request["unit"]+"", out unitNumber) && 
		Int32.TryParse(Request["video"]+"", out videoNumber) && 
		course.DefaultCourseData.Units.Count >= unitNumber  &&
		course.DefaultCourseData.Units[unitNumber-1].Videos.Count >= videoNumber ) {
		
		OnlineCourseVideo video = course.DefaultCourseData.Units[unitNumber-1].Videos[videoNumber-1];
		
		XmlDocument xmlDocument = new XmlDocument();
		//xmlDocument.Load( Server.MapPath("~/playerfiles/" + course.CourseCode + "/slides/" + language + "/" + video.CourseCode + "_u" + video.UnitNumber.ToString().PadLeft(3,'0') + "_v" + video.VideoNumber.ToString().PadLeft(3,'0') + "_slides.xml" ));
		xmlDocument.Load( @"e:\websites\my.dts.edu\web\playerfiles\" + course.CourseCode + "/slides/" + language + "/" + video.CourseCode + "_u" + video.UnitNumber.ToString().PadLeft(3,'0') + "_v" + video.VideoNumber.ToString().PadLeft(3,'0') + "_slides.xml" );
		
		TranscriptTextRepeater.DataSource = xmlDocument.SelectSingleNode("slides").ChildNodes;
		TranscriptTextRepeater.DataBind();
		
	}
			
		
}

string GetCourseString() {
	return Request["Course"].ToString().ToLower() + "/" + Request["Course"].ToString().ToUpper() + "_u" + Request["Unit"].ToString().PadLeft(3,'0') + "_v" + Request["Video"].ToString().PadLeft(3,'0');
}

</script>
<asp:Content ContentPlaceHolderID="MainContent" runat="ServeR">
 
<div data-role="page"> 
 
	<div data-role="header"> 
		<h1><%= Request["Course"] %> Slides <%= Request["unit"] %>-<%= Request["video"] %></h1> 
	</div><!-- /header --> 
 
	<div data-role="content"> 
	
		<asp:Repeater id="TranscriptTextRepeater" runat="Server" >
			
			<ItemTemplate>
				<p><img src="//my.dts.edu/playerfiles/<%= Request["Course"] %>/Slides/<%= language %>/<%# ((XmlNode) Container.DataItem).Attributes["slideFileName"].Value %>" /></p>
			</ItemTemplate>
							
		</asp:Repeater>
		
	</div><!-- /content --> 
	
</div><!-- /page --> 
 
</asp:Content>