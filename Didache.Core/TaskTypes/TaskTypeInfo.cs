using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Didache.TaskTypes {
	public class TaskTypeInfo {
		public string ClassName { get; set; }
		public string FriendlyName { get; set; }
		public string Description { get; set; }
		public Type TaskType { get; set; }
		public ITaskType TaskInstance { get; set; }
	}
}
