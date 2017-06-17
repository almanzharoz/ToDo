using System;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.ElasticSearch.Serialization
{
	internal class ParentJsonConverter<T, TParentModel, TParentProjection> : JsonConverter 
		where T : IEntity, IWithParent<TParentModel, TParentProjection>, new()
		where TParentModel : class, IEntity, new()
		where TParentProjection : IProjection<TParentModel>, new()
	{
		private readonly IRequestContainer _entityContainer;
		public ParentJsonConverter(IRequestContainer entityContainer)
		{
			_entityContainer = entityContainer;
		}

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
			if (reader.Value is string)
				return _entityContainer.GetOrAdd<T>(reader.Value as string, true);
			var jsonObject = JObject.Load(reader);
			jsonObject.Remove("_type");
			T target = _entityContainer.GetOrAdd<T>(jsonObject["id"].ToString(), false);
			using (var r = jsonObject.CreateReader())
				serializer.Populate(r, target);
			if (jsonObject.TryGetValue("parent", out var v))
				target.Parent = _entityContainer.GetOrAdd<TParentProjection>(v.Value<string>(), true);
			return target;
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}