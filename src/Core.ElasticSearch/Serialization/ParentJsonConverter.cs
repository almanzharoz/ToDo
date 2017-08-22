using System;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Mapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.ElasticSearch.Serialization
{
	internal class ParentJsonConverter<T, TParent> : JsonConverter 
		where T : class, IEntity, IProjection, IWithParent<TParent>, new()
		where TParent : class, IEntity, IProjection, new()
	{
		private readonly IRequestContainer _entityContainer;
		private readonly IProjectionItem _projection;

		public ParentJsonConverter(IProjectionItem projection, IRequestContainer entityContainer)
		{
			_entityContainer = entityContainer;
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
				var o = JToken.FromObject(new InnerValue(v), serializer);
				o.WriteTo(writer);
			}
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value is string)
				return _entityContainer.GetOrAdd<T>(reader.Value as string, true);
			var jsonObject = JObject.Load(reader);
			jsonObject.Remove("_type");
			T target = _entityContainer.GetOrAdd<T>(jsonObject["id"].ToString(), false);
			using (var r = jsonObject.CreateReader())
				serializer.Populate(r, target);
			if (jsonObject.TryGetValue("parent", out var v))
			{
				if (target is BaseEntityWithParent<TParent>)
					(target as BaseEntityWithParent<TParent>).Parent = _entityContainer.GetOrAdd<TParent>(v.Value<string>(), true);
				else
					(target as BaseEntityWithParentAndVersion<TParent>).Parent = _entityContainer.GetOrAdd<TParent>(v.Value<string>(), true);
			}
			return target;
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}