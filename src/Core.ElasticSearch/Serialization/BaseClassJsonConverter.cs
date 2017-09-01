using System;
using System.Diagnostics;
using Core.ElasticSearch.Mapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.ElasticSearch.Serialization
{
	internal interface IWithContainer{}

	internal class BaseClassJsonConverter<TSettings> : JsonConverter, IWithContainer
		where TSettings : BaseElasticConnection
	{
		private readonly RequestContainer<TSettings> _entityContainer;
		public BaseClassJsonConverter(RequestContainer<TSettings> entityContainer)
		{
			_entityContainer = entityContainer;
			Debug.WriteLine("BaseClassJsonConverter: " + _entityContainer.GetHashCode());
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jsonObject = JObject.Load(reader);
			//jsonObject.Remove("_type"); // не десериализовать это поле
			var target = existingValue ?? _entityContainer.Get(jsonObject["id"].ToString());
			using (var r = jsonObject.CreateReader())
				serializer.Populate(r, target);
			return target;
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}