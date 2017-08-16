using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Mapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.ElasticSearch.Serialization
{
	internal class PropertyInfoComparer : IEqualityComparer<PropertyInfo>
	{
		public bool Equals(PropertyInfo x, PropertyInfo y)
		{
			return x.Name.Equals(y.Name);
		}

		public int GetHashCode(PropertyInfo obj)
		{
			return 0;
		}
	}

	internal class ClassJsonConverter<T> : JsonConverter where T : BaseEntity, new()
	{
		private readonly IRequestContainer _entityContainer;
		private readonly IProjectionItem _projectionItem;
		public ClassJsonConverter(IProjectionItem projectionItem, IRequestContainer entityContainer)
		{
			_entityContainer = entityContainer;
			_projectionItem = projectionItem;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			//var sw = new Stopwatch();
			//sw.Start();
			writer.WriteStartObject();
			foreach (var property in _projectionItem.Properties)
			{
				var v = property.GetValue(value);
				if (v == null)
					continue;
				writer.WritePropertyName(property.Name.ToLower());
				var o = JToken.FromObject(new InnerValue(v), serializer);
				o.WriteTo(writer);
			}
			writer.WriteEndObject();
			//sw.Stop();
			//Console.WriteLine("WriteJson: " + sw.ElapsedMilliseconds);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var sw = new Stopwatch();
			sw.Start();
			if (reader.Value is string)
			{
				sw.Stop();
				var r = _entityContainer.GetOrAdd<T>(reader.Value as string, true);
				Console.WriteLine($"DeserializeId<{typeof(T).Name}>: " + sw.ElapsedMilliseconds);
				return r;
			}
			var jsonObject = JObject.Load(reader);
			jsonObject.Remove("_type");
			var target = existingValue ?? _entityContainer.GetOrAdd<T>(jsonObject["id"].ToString(), false);
			using (var r = jsonObject.CreateReader())
				serializer.Populate(r, target);
			if (jsonObject.TryGetValue("parent", out var parent))
				_entityContainer.GetOrAdd<T>(parent.Value<string>(), true);
			if (target is IWithVersion && jsonObject.TryGetValue("version", out var v))
				((BaseEntityWithVersion) target).Version = (int)v;
			sw.Stop();
			//Console.WriteLine($"Deserialize<{typeof(T).Name}>: " + sw.ElapsedMilliseconds);
			return target;
		}

		public override bool CanConvert(Type objectType)
		{
			throw new System.NotImplementedException();
		}
	}
}