using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Didache {
	public class EntityObjectSerializer {
		
		private readonly Type[] _builtInTypes = new[]{
			typeof(bool),
			typeof(byte),
			typeof(sbyte),
			typeof(char),
			typeof(decimal),
			typeof(double),
			typeof(float),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(short),
			typeof(ushort),
			typeof(string),
			typeof(DateTime),
			typeof(DateTime?),
			typeof(Guid)
		};

		public object Serialize(object obj) {

			if (obj == null)
				return null;

			Type type = obj.GetType();

			if (obj is IEnumerable) {
				List<object> result = new List<object>();
				foreach (object o in (obj as IEnumerable)) {
					result.Add(SerializeToDictionary(o));
				}
				return result;
			} else {
				return SerializeToDictionary(obj);
			}
		}

		private Dictionary<string, object> SerializeToDictionary(object obj) {
			Type type = obj.GetType();

			// get the underlying type so that we can access the true [attributes] of all properties
			// virtual properties don't retain their attributes :(
			if (type.FullName.StartsWith("System.Data.Entity.DynamicProxies"))
				type = type.BaseType;
			
			// get all the System.object properties
			var properties = from p in type.GetProperties()
							 where /* p.CanWrite && */
								   p.CanRead &&
								   _builtInTypes.Contains(p.PropertyType)
							 select p;

			// add them to the dictionary
			var result = properties.ToDictionary(
						  property => property.Name,
						  property => (Object)(property.GetValue(obj, null)
									  == null
									  ? ""
									  : property.GetValue(obj, null).ToString().Trim())
						  );

			// grab objects (not int, string, etc.)
			var complexProperties = from p in type.GetProperties()
									where p.CanWrite &&
											p.CanRead &&
											p.GetCustomAttributes(typeof(ScriptIgnoreAttribute), true).Length == 0 &&
											!_builtInTypes.Contains(p.PropertyType)
									select p;

			// spin through the sub classes and make them
			foreach (var property in complexProperties) {
				//var js = new ObjectToDictionaryNew();
				result.Add(property.Name, Serialize(property.GetValue(obj, null)));
			}

			return result;
		}
	}
}
