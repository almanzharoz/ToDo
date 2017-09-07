using System;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Mapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.ElasticSearch.Serialization
{
	internal class ParentJsonConverter<T, TParent> : JsonConverter
		where T : class, IEntity, IProjection, IWithParent<TParent>
		where TParent : class, IEntity, IProjection
	{
		private readonly IProjectionItem _projection;

		public ParentJsonConverter(IProjectionItem projection)
		{
			_projection = projection;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			foreach (var property in _projection.Properties)
			{
				var v = property.GetValue(value);
				if (v == null)
					continue;
				writer.WritePropertyName(property.Name.ToLower());
				var o = JToken.FromObject(v, serializer);
				o.WriteTo(writer);
			}
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
			//var container = ((CoreElasticContractResolver)serializer.ContractResolver).Container;
			//if (reader.Value is string)
			//	return container.GetOrAdd<T>(reader.Value as string, true);
			//var jsonObject = JObject.Load(reader);
			//jsonObject.Remove("_type");
			//T target = container.GetOrAdd<T>(jsonObject["id"].ToString(), false);
			//using (var r = jsonObject.CreateReader())
			//	serializer.Populate(r, target);
			//if (jsonObject.TryGetValue("parent", out var v))
			//{
			//	if (target is BaseEntityWithParent<TParent>)
			//		(target as BaseEntityWithParent<TParent>).Parent = container.GetOrAdd<TParent>(v.Value<string>(), true);
			//	else
			//		(target as BaseEntityWithParentAndVersion<TParent>).Parent = container.GetOrAdd<TParent>(v.Value<string>(), true);
			//}
			//return target;
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}