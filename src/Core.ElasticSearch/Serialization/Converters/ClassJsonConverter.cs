using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Mapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpFuncExt;

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

	internal class ClassJsonConverter<T> : JsonConverter where T : class, IProjection
	{
		private readonly IProjectionItem _projectionItem;
		private readonly (ObjectActivator<T> func, string[] parameters) _creator;

		public ClassJsonConverter(IProjectionItem projectionItem)
		{
			_projectionItem = projectionItem;
			_creator = typeof(T).GetConstructors().FirstOrDefault().IfNotNullOrDefault(ObjectActivator.GetActivator<T>);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			foreach (var property in _projectionItem.Properties)
			{
				var v = property.GetValue(value);
				if (v == null)
					continue;
				var o = JToken.FromObject(v, serializer);
				writer.WritePropertyName(property.Name.ToLower());
				o.WriteTo(writer);
			}
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (_creator.parameters != null)
			{
				var parameters = new Dictionary<string, object>(
					_creator.parameters.Select(x => KeyValuePair.Create<string, object>(x, null)));
				while (reader.Read() && reader.TokenType != JsonToken.EndObject)
				{
					if (reader.TokenType != JsonToken.PropertyName || reader.Value == null)
						throw new Exception("TokenType != JsonToken.PropertyName");

					reader.Value.As<string>()
						.IfNotNull(
							x => x.If(p => reader.Read() && parameters.ContainsKey(p),
								p => parameters[p] = serializer.Deserialize(reader, reader.ValueType)));
				}
				return _creator.func(_creator.parameters.Select(x => parameters[x]).ToArray());
			}
			throw new Exception($"Not create {typeof(T).Name}");
			//var jsonObject = JObject.Load(reader);
			//jsonObject.Remove("_type");
			//var target = jsonObject.TryGetValue("parent", out var parent) ? _creator.func(jsonObject["id"].ToString(), parent) : _creator.func(jsonObject["id"].ToString());
			//using (var r = jsonObject.CreateReader())
			//	serializer.Populate(r, target);
			//if (target is IWithVersion && jsonObject.TryGetValue("version", out var v))
			//	((BaseEntityWithVersion) target).Version = (int)v;
			//return target;
		}

		public override bool CanConvert(Type objectType)
		{
			throw new System.NotImplementedException();
		}
	}
}