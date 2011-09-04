<%@ WebHandler Language="C#" Class="UserCourses" %>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using DTS.Online;
using Didache;

public class UserCourses : IHttpHandler {
    
	public void ProcessRequest (HttpContext context) {

		List<OnlineCourse> courses = DTS.Online.OnlineCourses.GetCourses();
		context.Response.ContentType = "text/xml";

		XmlTextWriter writer = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8);
		writer.Formatting = Formatting.Indented;

		writer.WriteStartDocument();
		writer.WriteStartElement("courses");

		foreach (OnlineCourse course in courses) {

			writer.WriteStartElement("course");
			writer.WriteAttributeString("code", course.CourseCode);

			foreach (string key in course.Languages.Keys) {
				writer.WriteStartElement("language");

				writer.WriteAttributeString("code", key);
				writer.WriteAttributeString("name", course.Languages[key].Title);

				writer.WriteEndElement();  // language
			}

			writer.WriteEndElement(); // course

		}

		// close list
		writer.WriteEndElement();
		writer.WriteEndDocument();
		writer.Close();	
		
	}
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}