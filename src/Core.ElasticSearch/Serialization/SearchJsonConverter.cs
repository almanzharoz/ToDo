using System;
using System.Diagnostics;
using System.Linq;
using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.ElasticSearch.Serialization
{
	internal class SearchJsonConverter<T> : JsonConverter where T : class, IProjection
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var sw = new Stopwatch();
			var jsonObject = JObject.Load(reader);
			foreach (var j in jsonObject["hits"]["hits"].AsEnumerable())
			{
				if (j["_id"] != null)
					j["_source"]["id"] = j["_id"];
				if (j["_version"] != null)
					j["_source"]["version"] = j["_version"];
				j["_source"]["_type"] = j["_type"];
				if (j["_parent"] != null)
					j["_source"]["parent"] = j["_parent"];
			}
			var target = new SearchResponse<T>();
			sw.Start();
			using (var r = jsonObject.CreateReader())
				serializer.Populate(r, target);
			sw.Stop();
			Debug.WriteLine($"DeserializeSearch<{typeof(T).Name}>: " + sw.ElapsedMilliseconds);
			return target;
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}