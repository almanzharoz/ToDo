using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Core.ElasticSearch.Serialization
{
	// TODO: Попытка сдлетать readonly поля без JsonPropertyAttribute
	public class SisoJsonDefaultContractResolver
		: ElasticContractResolver
	{
		//protected override JsonProperty CreateProperty(
		//	MemberInfo member,
		//	MemberSerialization memberSerialization)
		//{
		//	var prop = base.CreateProperty(member, memberSerialization);

		//	if (!prop.Writable)
		//	{
		//		var property = member as PropertyInfo;
		//		if (property != null)
		//		{
		//			var hasPrivateSetter = property.GetSetMethod(true) != null;
		//			prop.Writable = hasPrivateSetter;
		//		}
		//	}

		//	return prop;
		//}


		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var method = typeof(DefaultContractResolver).GetMethod("CreateProperty", BindingFlags.NonPublic | BindingFlags.Instance);
			//var baseSay = (Action)Activator.CreateInstance(typeof(Action), this, ptr);
			//baseSay();

			JsonProperty property = (JsonProperty)method.Invoke(new DefaultContractResolver(), new object [] {member, memberSerialization});

			if (!property.Writable)
			{
				var p = member as PropertyInfo;
				if (p != null)
				{
					var hasPrivateSetter = p.GetSetMethod(true) != null;
					property.Writable = hasPrivateSetter;
				}
			}

			if (property.PropertyType == typeof(QueryContainer))
				property.ShouldSerialize = o => ElasticContractResolver.ShouldSerializeQueryContainer(o, property);
			else if (property.PropertyType == typeof(IEnumerable<QueryContainer>))
				property.ShouldSerialize = o => ElasticContractResolver.ShouldSerializeQueryContainers(o, property);

			// Skip serialization of empty collections that have DefaultValueHandling set to Ignore.
			else if (property.DefaultValueHandling.HasValue
					&& property.DefaultValueHandling.Value == DefaultValueHandling.Ignore
					&& !typeof(string).IsAssignableFrom(property.PropertyType)
					&& typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
			{
				Predicate<object> shouldSerialize = obj =>
				{
					var collection = property.ValueProvider.GetValue(obj) as ICollection;
					if (collection == null)
					{
						return true;
					}
					return collection.Count != 0 && collection.Cast<object>().Any(item => item != null);
				};
				property.ShouldSerialize = property.ShouldSerialize == null ? shouldSerialize : (o => property.ShouldSerialize(o) && shouldSerialize(o));
			}

			IPropertyMapping propertyMapping = null;
			if (!this.ConnectionSettings.PropertyMappings.TryGetValue(member, out propertyMapping))
				propertyMapping = ElasticsearchPropertyAttributeBase.From(member);

			var serializerMapping = this.ConnectionSettings.Serializer?.CreatePropertyMapping(member);

			var nameOverride = propertyMapping?.Name ?? serializerMapping?.Name;
			if (!String.IsNullOrEmpty(nameOverride)) property.PropertyName = nameOverride;

			var overrideIgnore = propertyMapping?.Ignore ?? serializerMapping?.Ignore;
			if (overrideIgnore.HasValue)
				property.Ignored = overrideIgnore.Value;

			return property;
		}

		public SisoJsonDefaultContractResolver(IConnectionSettingsValues connectionSettings, IList<Func<Type, JsonConverter>> contractConverters) : base(connectionSettings, contractConverters)
		{
		}
	}
}