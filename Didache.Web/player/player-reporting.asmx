<%@ WebService Language="C#" Class="PlayerReporting" %>

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
//using System.Web.Mail;
using System.Net.Mail;
//using System.Collections.Generic;

using DTS.My;

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
    //public string ReportProblem(string data)
    {
    /*
				string type = "";
				string course = "";
				string time = "";
				string slide = "";
				string transcript = "";
				string text = "";
				*/
    
				MyDtsUser user = UserSecurity.GetLoggedOnUser();
				
    
        string emailText = @"
Problem Reporting

" + ((user != null) ? user.ForumsUser.Username + "\n" + user.ForumsUser.Email : "(guest user)") + @"
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


				MailMessage mailMessage = new MailMessage();
				mailMessage.To.Add("babegg@dts.edu");
				mailMessage.To.Add("mmckee@dts.edu");
				mailMessage.To.Add("jdyer@dts.edu");
				mailMessage.From = new MailAddress( ((user != null) ? user.ForumsUser.Email : "noreply@dts.edu") );
				mailMessage.Subject = "Player Reporting: " + type;
				mailMessage.IsBodyHtml = false;
				mailMessage.Body = emailText;
				
				SmtpClient client = new SmtpClient("65.17.245.174");
				client.Send(mailMessage);

			
				/*
				MailMessage mailMessage = new MailMessage();
				mailMessage.To = "babegg@dts.edu, mmckee@dts.edu, jdyer@dts.edu";
				mailMessage.From =  ((user != null) ? user.ForumsUser.Email : "noreply@dts.edu");
				mailMessage.Subject = "Player Reporting: " + type;
				//mailMessage.BodyFormat = MailFormat.Html;
				mailMessage.Body = emailText;

				SmtpMail.SmtpServer = "65.17.245.174";
				SmtpMail.Send(mailMessage);
				*/


        return "true";
    }   
}
