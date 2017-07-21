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
		where TSettings : BaseElasticSettings
	{
		private readonly ConcurrentDictionary<Type, JsonConverter> _converters = new ConcurrentDictionary<Type, JsonConverter>();

		private readonly RequestContainer<TSettings> _container;
		private readonly ElasticMapping<TSettings> _mapping;
		private static readonly InnerValueJsonConverter _innerValueJsonConverter = new InnerValueJsonConverter();

		public ElasticSerializer(IConnectionSettingsValues settings, ElasticMapping<TSettings> mapping,
			RequestContainer<TSettings> container) : base(settings
				//,(serializerSettings, values) =>
				//serializerSettings.ContractResolver = new SisoJsonDefaultContractResolver(settings, new List<Func<Type, JsonConverter>>())
			)
		{
			_mapping = mapping;
			_container = container;
			ContractConverters.Add(GetJsonConverter);
		}

		private JsonConverter GetJsonConverter(Type x)
		{
			if (x == typeof(InnerValue))
				return _innerValueJsonConverter;
			if (x == typeof(IProjection))
				return _converters.GetOrAdd(x, z => new BaseClassJsonConverter<TSettings>(_container));
			if (x.GetInterfaces().Any(z => z.Name == "IProjection`1"))
				return _converters.GetOrAdd(x, z => _mapping.GetJsonConverter(z, _container));
			if (_mapping.TryGetResponseJsonConverter(x, out JsonConverter result))
				return result;
			return null;
		}

		protected sealed override IList<Func<Type, JsonConverter>> ContractConverters { get; } = new List<Func<Type, JsonConverter>>();
	}
}