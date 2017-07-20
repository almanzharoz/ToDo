using System;
using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.ElasticSearch.Serialization
{
	public class UpdateResultJsonConverter<T> : JsonConverter where T : class, IEntity
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jsonObject = JObject.Load(reader);
			var result = new UpdateResponse<T>();
			using (var r = jsonObject.CreateReader())
				serializer.Populate(r, result);
			// меняем версию
			return result;
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}