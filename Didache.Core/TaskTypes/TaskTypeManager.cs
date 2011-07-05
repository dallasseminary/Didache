using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Didache.TaskTypes {
	public class TaskTypeManager {
		public static List<TaskTypeInfo> GetTaskTypes() {

			// TODO: Caching

			List<TaskTypeInfo> taskTypes = new List<TaskTypeInfo>();

			Type iTaskType = typeof(ITaskType);

			List<Type> types = Assembly.GetExecutingAssembly().GetTypes().Where(t => iTaskType.IsAssignableFrom(t) && t != iTaskType).ToList();

			foreach (Type type in types) {
				TaskTypeInfo info = new TaskTypeInfo() {
					TaskType = type,
					FullClassName = type.FullName,
					ClassName = type.Name,
					FriendlyName = System.Text.RegularExpressions.Regex.Replace(type.Name, @"\B[A-Z]", " $0"),
					TaskInstance = (ITaskType) Activator.CreateInstance(type)
				};

				taskTypes.Add(info);

			}

			return taskTypes;
		}

		public static TaskTypeResult ProcessFormCollection(string taskClassName, int taskID, int userID, FormCollection collection, HttpRequestBase request) {

			List<TaskTypeInfo> taskTypes = GetTaskTypes();
			TaskTypeInfo taskType = taskTypes.SingleOrDefault(i => i.ClassName == taskClassName);

			if (taskType != null) {
				return taskType.TaskInstance.ProcessFormCollection(taskID, userID, collection, request);
			} else {
				return null;
			}
		}
	}
}
