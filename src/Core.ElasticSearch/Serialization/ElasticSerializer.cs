using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Mapping;
using Nest;
using Newtonsoft.Json;

namespace Core.ElasticSearch.Serialization
{
	internal class ElasticSerializer<TSettings> : JsonNetSerializer 
		where TSettings : BaseElasticConnection
	{
		private readonly ConcurrentDictionary<Type, JsonConverter> _converters = new ConcurrentDictionary<Type, JsonConverter>();

		private readonly RequestContainer<TSettings> _container;
		private readonly ElasticMapping<TSettings> _mapping;
		private static readonly InnerValueJsonConverter _innerValueJsonConverter = new InnerValueJsonConverter();

		public ElasticSerializer(IConnectionSettingsValues settings, ElasticMapping<TSettings> mapping,
			RequestContainer<TSettings> container) : base(settings
				//, (serializerSettings, values) =>
				// serializerSettings.ContractResolver = new CoreElasticContractResolver(settings, null)
			)
		{
			_mapping = mapping;
			_container = container;
			ContractConverters.Add(GetJsonConverter);
			//var t = typeof(Nest.JsonNetSerializer);
			//var p = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)[3];
			//var p = t.GetProperty("ContractResolver", BindingFlags.CreateInstance | BindingFlags.NonPublic);
			//p.SetValue(this, new CoreElasticContractResolver(settings, null));
		}

		private JsonConverter GetJsonConverter(Type x)
		{
			if (x == typeof(InnerValue))
				return _innerValueJsonConverter;
			if (x == typeof(IProjection))
				return _converters.GetOrAdd(x, z => new BaseClassJsonConverter<TSettings>(_container));
			if (typeof(IProjection).IsAssignableFrom(x))
				return _converters.GetOrAdd(x, z => _mapping.GetJsonConverter(z, _container));
			if (_mapping.TryGetResponseJsonConverter(x, out JsonConverter result))
				return result;
			return null;
		}

		protected sealed override IList<Func<Type, JsonConverter>> ContractConverters { get; } = new List<Func<Type, JsonConverter>>();
	}
}