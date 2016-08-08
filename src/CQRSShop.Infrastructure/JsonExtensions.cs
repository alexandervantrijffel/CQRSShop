using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CQRSShop.Infrastructure
{

	public static class JsonExtensions
	{
		public static T ToObject<T>(this byte[] data, string typeName) where T:class
		{
			var jsonString = Encoding.UTF8.GetString(data);
			return JsonConvert.DeserializeObject(jsonString, Type.GetType(typeName)) as T;
		}

		public static byte[] AsJson(this object o)
		{
			var jsonObj = JsonConvert.SerializeObject(o);
			var data = Encoding.UTF8.GetBytes(jsonObj);
			return data;
		}
	}
}
