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
			
			// TODO: add caching

			int courseID = -1;
			List<Unit> courseUnits = null;
			User user = USers.GetLoggedInUser();
			
			if (user != null) {
				// get the courseID by looking at all the user's classes and matching the courseID
				List<course> usersCourses = Courses.GetUsersCurrentlyRunningCourses(user.UserID, CourseUserRole.Student); //(int) UserRole.Student);
				courseID = MatchCourseCode(courseCode, usersCourses);
				
				// get the list of units for this course

				if (courseID > -1)
					courseUnits = Courses.GetUnitsInCourse(courseID);
			}
			
			string path = context.Request.PhysicalPath;
			path = path.Substring(0, path.LastIndexOf("\\")+1);
			
			string videoXmlPath = context.Server.MapPath("~/playerfiles/" + courseCode + "/titles/" + language + ".xml");


			XmlDocument videosXmlDoc = new XmlDocument();
			
			
			// load XML
			if (System.IO.File.Exists(videoXmlPath)) {
				
				// load local file

				XmlTextReader xmlTextReader = new XmlTextReader(videoXmlPath);
				videosXmlDoc.Load(xmlTextReader);
			} else {
				throw new Exception("can't find " + videoXmlPath);
			}

			
			// check for active units

			XmlNode rootNode = videosXmlDoc.SelectSingleNode("course");
			if (courseID > 0) {
				XmlAttribute idAttribute = videosXmlDoc.CreateAttribute("courseID");
				idAttribute.Value = courseID.ToString();
				rootNode.Attributes.Append(idAttribute);
			}
			
			ArrayList nodesToRemove = new ArrayList();
			
			// find all the inactive units
			foreach (XmlNode unitNode in rootNode) {
				
				int unitNumber = Convert.ToInt32(unitNode.Attributes["number"].Value);
				
				// if the user is not registered for this class, then remove everything but the first 2 units (preview)
				if (courseUnits == null) {
					if (unitNumber > 2)
						nodesToRemove.Add(unitNode);
				} else {
					// if the user is registered for the course, then remove ones not yet active in my.dts.edu
					if (!UnitIsActive(context, unitNumber, courseUnits))
						nodesToRemove.Add(unitNode);
				}
			}
			
			// remove all the inactive units
			foreach (XmlNode nodeToRemove in nodesToRemove) {
				rootNode.RemoveChild(nodeToRemove);
			}
			
			//return;

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

	public int MatchCourseCode(string courseCode, ArrayList coursesList) {

		ArrayList coursesThatMightMatch = new ArrayList();

		foreach (Course course in coursesList) {
			string generateCode = (course.CourseMaster.DepartmentCode.ToUpper() + course.CourseMaster.Number.ToString() + ((course.VersionNumber > 1) ? "v" + course.VersionNumber.ToString() : "") ).Trim();
		
			if ( generateCode.ToUpper().Trim() == courseCode.ToUpper().Trim())
				coursesThatMightMatch.Add(course);
		}
		
		// if there are no courses, return -1
		if (coursesThatMightMatch.Count == 0) {
			return -1;
			
		// if there's just one, then return that one
		} else if (coursesThatMightMatch.Count == 1) {
			return ((Course) coursesThatMightMatch[0]).CourseID;
		
		// there's more than one, then try to find the OL one
		} else if (coursesThatMightMatch.Count > 1) {
			foreach (Course course in coursesThatMightMatch) {
				if (course.Section == "OL")
					return course.CourseID;
			
			}
		
			// if none are "OL", just try the first one
			return ((Course) coursesThatMightMatch[0]).CourseID;
		
		}
		
		return -1;	
	}

	public bool UnitIsActive(HttpContext context, int unitNumber, ArrayList unitList) {
		if (null == unitList) 
			return true;		
		
		foreach (CourseUnit unit in unitList) {
			if (unit.UnitNumber == unitNumber && unit.Active) {
				//context.Response.Write("&nbsp;- " + unit.UnitNumber + " - " + unit.Name + "<br />");
				return true;
			}
		}
		return false;
	}

}