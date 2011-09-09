<%@ Page Language="C#" MasterPageFile="Mobile.Master" EnableViewState="False" %>

<%@ Import Namespace="DTS.Online" %>
<%@ Import Namespace="System.Xml" %>

<script runat="server">
void Page_Load() {

	string language = (String.IsNullOrEmpty(Request["language"]+"")) ? "en-US" : Request["language"]+"";
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
		xmlDocument.Load(Didache.Settings.PlayerFilesLocation + course.CourseCode + "/transcripts/" + language + "/" + video.CourseCode + "_u" + video.UnitNumber.ToString().PadLeft(3, '0') + "_v" + video.VideoNumber.ToString().PadLeft(3, '0') + "_transcript.xml");
		
		TranscriptTextRepeater.DataSource = xmlDocument.SelectSingleNode("transcript").ChildNodes;
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
		<h1><%= Request["Course"] %> Transcript <%= Request["unit"] %>-<%= Request["video"] %></h1> 
	</div><!-- /header --> 
 
	<div data-role="content"> 
	
		<asp:Repeater id="TranscriptTextRepeater" runat="Server" >
			
			<HeaderTemplate><p></HeaderTemplate>
			
			<ItemTemplate>
					<%# ((XmlNode) Container.DataItem).Attributes["text"].Value %><%# ( ((XmlNode) Container.DataItem).Attributes["breakAfter"] != null) ? "</p><p>" : "" %>
			</ItemTemplate>
				
			<FooterTemplate><p></FooterTemplate>
		</asp:Repeater>
		
	</div><!-- /content --> 
	
</div><!-- /page --> 
 
</asp:Content>