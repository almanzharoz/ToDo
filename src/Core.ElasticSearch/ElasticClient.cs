﻿using Core.ElasticSearch.Mapping;
using Core.ElasticSearch.Serialization;
using Elasticsearch.Net;
using Nest;

namespace Core.ElasticSearch
{
	internal class ElasticClient<TSettings>
		where TSettings : BaseElasticConnection
	{
		public ElasticClient Client { get; }

		public ElasticClient(TSettings settings, ElasticMapping<TSettings> mapping, RequestContainer<TSettings> container)
		{
			var connectionPool = new StaticConnectionPool(new[] { settings.Url });
			var connectionSettings = new ConnectionSettings(connectionPool, new HttpConnection(), new SerializerFactory(values => new ElasticSerializer<TSettings>(values, mapping, container)));

			connectionSettings.DefaultFieldNameInferrer(x => x.ToLower());
			connectionSettings.DisablePing();
			connectionSettings.DisableAutomaticProxyDetection();
#if DEBUG
			connectionSettings.PrettyJson();
			connectionSettings.DisableDirectStreaming();
#endif

			Client = new ElasticClient(connectionSettings);
		}
	}
}