using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using InstaSharp;
using InstaSharp.Endpoints.Users;
using InstaSharp.Model;
using InstaSharp.Model.Responses;
using Newtonsoft.Json.Linq;

namespace Core.Instagram
{
	/// <summary>
	/// Overrides InstaSharp User.Authenticated to implement MIN_ID parameter.
	/// </summary>
	public class UsersAuthenticated : Authenticated
	{
		public UsersAuthenticated(InstagramConfig config, AuthInfo authInfo)
			: base(config, authInfo)
		{
		}

		public MediasResponse RecentFeed(string user, string minId)
		{
			return (MediasResponse)Mapper.Map<MediasResponse>(this.FeedJson(user, minId, 0));
		}

		private string FeedJson(string user, string minId, int count)
		{
			string uri = string.Format(this.Uri + "self/feed?access_token={0}", (object)this.AuthInfo.Access_Token);
			if (!string.IsNullOrEmpty(minId))
				uri = uri + "&min_id=" + minId;
			if (count > 0)
				uri = uri + (object)"&count=" + (string)(object)count;
			return HttpClient.GET(uri);
		}

		internal class Mapper
		{
			public static object Map<T>(string json) where T : new()
			{
				JObject json1 = JObject.Parse(json);
				Type t = typeof(T);
				try
				{
					object obj = Mapper.Map(t, json1);
					if (obj != null)
					{
						PropertyInfo property = obj.GetType().GetProperty("Json");
						if (property != (PropertyInfo)null)
							property.SetValue(obj, (object)json, (object[])null);
					}
					return obj;
				}
				catch (Exception ex)
				{
					return (object)null;
				}
			}

			private static object Map(Type t, JObject json)
			{
				object instance = Activator.CreateInstance(t);
				Array.ForEach<PropertyInfo>(instance.GetType().GetProperties(), (Action<PropertyInfo>)(prop =>
				{
					object[] local_0 = prop.GetCustomAttributes(typeof(JsonMapping), false);
					if (local_0.Length <= 0)
						return;
					Type local_1 = prop.PropertyType;
					string local_2 = ((JsonMapping)local_0[0]).MapsTo;
					switch (((JsonMapping)local_0[0]).MapType)
					{
						case JsonMapping.MappingType.Class:
							if (json[local_2] == null || !json[local_2].HasValues)
								break;
							object local_4 = Mapper.Map(local_1, (JObject)json[local_2]);
							prop.SetValue(instance, local_4, (object[])null);
							break;
						case JsonMapping.MappingType.Collection:
							IList local_5 = Mapper.Map(local_1, (JArray)json[local_2]);
							prop.SetValue(instance, (object)local_5, (object[])null);
							break;
						default:
							if (json == null || json[local_2] == null)
								break;
							if (prop.PropertyType == typeof(DateTime))
							{
								prop.SetValue(instance, (object)Mapper.UnixTimeStampToDateTime(((object)json[local_2]).ToString()), (object[])null);
								break;
							}
							else
							{
								prop.SetValue(instance, Convert.ChangeType((object)((object)json[local_2]).ToString(), prop.PropertyType), (object[])null);
								break;
							}
					}
				}));
				return instance;
			}

			private static IList Map(Type t, JArray json)
			{
				Type t1 = t.GetGenericArguments()[0];
				IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[1]
				{
				t1
				}));
				if (json != null)
				{
					foreach (JToken jtoken in (IEnumerable<JToken>)json)
					{
						if (t1.Name == "String" || t1.Name == "Int32")
							list.Add((object)((object)jtoken).ToString());
						else
							list.Add(Mapper.Map(t1, (JObject)jtoken));
					}
				}
				return list;
			}

			private static DateTime UnixTimeStampToDateTime(string unixTimeStamp)
			{
				return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToDouble(unixTimeStamp)).ToLocalTime();
			}

			private static void SetPropertyValue(PropertyInfo prop, object instance, object value)
			{
				prop.SetValue(instance, Convert.ChangeType(value, prop.PropertyType), (object[])null);
			}
		}

	}
}