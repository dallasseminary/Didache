<%@ WebHandler Language="C#" Class="CoursesList" %>

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using Didache;

public class CoursesList : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {

		string courseCode = context.Request.QueryString["course"]  + "";
		string language = context.Request.QueryString["language"] + "";
		if (language == "")
			language = "en-US";
			
		if (context.Request.QueryString["admin"] + "" == "true" || context.Request.QueryString["alumni"] + "" == "true" 
			|| courseCode.ToLower() == "be547"
			) {
			context.Response.Redirect("/playerfiles/" + courseCode + "/titles/" + language + ".xml");
		}

		if (courseCode != null) {
						
			XmlDocument videosXmlDoc = DTS.Online.OnlineCourses.GetCourseUnitsXml(courseCode, language);
			
			
			context.Response.ContentType = "text/xml";
			// save the XML to the output stream
			videosXmlDoc.Save(context.Response.OutputStream);
		}
		else {
			//context.Response.Write("no course code defined");
		}

    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }


}