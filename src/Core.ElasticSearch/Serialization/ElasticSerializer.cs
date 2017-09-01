using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Mapping;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;

namespace Core.ElasticSearch.Serialization
{
	internal class ElasticSerializer<TSettings> : JsonNetSerializer 
		where TSettings : BaseElasticConnection
	{
		//private readonly ConcurrentDictionary<Type, JsonConverter> _converters = new ConcurrentDictionary<Type, JsonConverter>();

		private readonly RequestContainer<TSettings> _container;
		private readonly ElasticMapping<TSettings> _mapping;
		private static readonly InnerValueJsonConverter _innerValueJsonConverter = new InnerValueJsonConverter();

		public ElasticSerializer(IConnectionSettingsValues settings, ElasticMapping<TSettings> mapping,
			RequestContainer<TSettings> container) : base(settings)
		{
			_mapping = mapping;
			_container = container;
			var p = typeof(Nest.JsonNetSerializer).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).First(x => x.Name == "<ContractResolver>k__BackingField");
			p.SetValue(this, new CoreElasticContractResolver(settings, this.ContractConverters)); // Hack ContractResolver with private set
			ContractConverters.Add(GetJsonConverter);
			OverwriteDefaultSerializers((s, csv) => { });
		}

		private JsonConverter GetJsonConverter(Type x)
		{
			if (x == typeof(InnerValue))
				return _innerValueJsonConverter;
			if (x == typeof(IProjection))
				return new BaseClassJsonConverter<TSettings>(_container);
			if (typeof(IProjection).IsAssignableFrom(x))
				return _mapping.GetJsonConverter(x, _container);
			if (_mapping.TryGetResponseJsonConverter(x, out JsonConverter result))
				return result;
			return null;
		}

		protected sealed override IList<Func<Type, JsonConverter>> ContractConverters { get; } = new List<Func<Type, JsonConverter>>();

		public override T Deserialize<T>(Stream stream)
		{
			return base.Deserialize<T>(stream);
		}

		public override Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = new CancellationToken())
		{
			return base.DeserializeAsync<T>(stream, cancellationToken);
		}

		public override void Serialize(object data, Stream writableStream, SerializationFormatting formatting = SerializationFormatting.Indented)
		{
			base.Serialize(data, writableStream, formatting);
		}
	}
}