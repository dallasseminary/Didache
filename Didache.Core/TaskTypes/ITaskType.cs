using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Didache.TaskTypes {
	public interface ITaskType {
		TaskTypeResult ProcessFormCollection(int taskID, int userID, FormCollection collection, HttpRequestBase request);	
	}
}
