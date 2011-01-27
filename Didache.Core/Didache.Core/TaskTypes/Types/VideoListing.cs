using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Didache.TaskTypes {

	//[Display(Name = "Video Listing", Description = "Creates automatic links to video from XML.")]
	public class VideoListing : ITaskType {
		
		public object ProcessFormCollection(int taskID, int userID, FormCollection collection, HttpRequestBase request) {

			// just complete it!
			return new SimpleCompletion().ProcessFormCollection(taskID, userID, collection, request);
		}
	}
}
