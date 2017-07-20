using Core.ElasticSearch.Mapping;
using Core.ElasticSearch.Serialization;
using Elasticsearch.Net;
using Nest;

namespace Core.ElasticSearch
{
	public class ElasticClient<TSettings>
		where TSettings : BaseElasticSettings
	{
		public ElasticClient Client { get; }

		public ElasticClient(TSettings settings, ElasticMapping<TSettings> mapping, RequestContainer<TSettings> container)
		{
			var connectionPool = new StaticConnectionPool(new[] { settings.Url });
			var connectionSettings = new ConnectionSettings(connectionPool, new HttpConnection(), new SerializerFactory(values => new ElasticSerializer<TSettings>(values, mapping, container)));

			connectionSettings.DefaultFieldNameInferrer(x => x.ToLower());
			connectionSettings.DisablePing();
			connectionSettings.DisableAutomaticProxyDetection();
			//connectionSettings.EnableDebugMode(x => { x.AuditTrail.Clear(); });
#if DEBUG
			connectionSettings.PrettyJson();
			connectionSettings.DisableDirectStreaming();
#endif
			connectionSettings.DefaultIndex(settings.IndexName);

			Client = new ElasticClient(connectionSettings);
		}
	}
}