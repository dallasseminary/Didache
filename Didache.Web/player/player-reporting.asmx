<%@ WebService Language="C#" Class="PlayerReporting" %>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
//using System.Web.Mail;
using System.Net.Mail;
//using System.Collections.Generic;
using Didache;

[WebService(Namespace = "http://tempuri.org/")]
//[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
//[System.Web.Script.Services.ScriptService]
public class PlayerReporting : System.Web.Services.WebService 
{
    public PlayerReporting()
    {
        //
        // TODO: Add any constructor code required
        //
    }

    [WebMethod]
    public string ReportProblem(string type, string course, string time, string slide, string transcript, string text)
    {

		User user = Users.GetLoggedInUser();
				
    
        string emailText = @"
Problem Reporting

" + ((user != null) ? user.Username + "\n" + user.Email : "(guest user)") + @"
Type: " + type + @"
Course: " + course + @"
Time: " + time + @"
Slide: " + slide + @"
Transcript: " + transcript + @"
Description: 
" + text + @"

User Agent: " + HttpContext.Current.Request.UserAgent + @"
IP: " + HttpContext.Current.Request.UserHostAddress + @"
";


		Emails.SendEmail(
					(user != null) ? user.Email : "noreply@dts.edu",
					new List<string>() { "babegg@dts.edu", "mmckee@dts.edu", "jdyer@dts.edu" },
					 "Player Reporting: " + type,
					 emailText);
	

        return "true";
    }   
}
