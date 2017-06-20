using System;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.ElasticSearch.Serialization
{
	internal class InsertJsonConverter<T> : JsonConverter where T : IEntity
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			foreach (var property in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (property.Name == "Id" || property.Name == "Version" || property.Name == "Parent")
					continue;
				var v = property.GetValue(value);
				if (v == null)
					continue;
				writer.WritePropertyName(property.Name.ToLower());
				var o = JToken.FromObject(new InnerValue(v), serializer);
				o.WriteTo(writer);
			}
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}

	internal class InsertObjectJsonConverter<T> : JsonConverter where T : struct
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			foreach (var property in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				var v = property.GetValue(value);
				if (v == null)
					continue;
				writer.WritePropertyName(property.Name.ToLower());
				var o = JToken.FromObject(new InnerValue(v), serializer);
				o.WriteTo(writer);
			}
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jsonObject = JObject.Load(reader);
			var target = existingValue ?? new T();
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