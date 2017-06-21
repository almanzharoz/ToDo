using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Serialization;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;
using SharpFuncExt;

namespace Core.ElasticSearch.Mapping
{
	public class ElasticMapping<TSettings> where TSettings : BaseElasticSettings
	{
		private readonly ConcurrentDictionary<Type, IMappingItem> _mapping = new ConcurrentDictionary<Type, IMappingItem>();
		private readonly ConcurrentDictionary<Type, IProjectionItem> _projection = new ConcurrentDictionary<Type, IProjectionItem>();
		private static readonly ConcurrentDictionary<Type, JsonConverter> _converters = new ConcurrentDictionary<Type, JsonConverter>();
		private readonly TSettings _settings;
		private ILogger<ElasticMapping<TSettings>> _logger;

		public ElasticMapping(TSettings settings, ILoggerFactory loggerFactory)
		{
			_settings = settings;
			_logger = loggerFactory.CreateLogger<ElasticMapping<TSettings>>();
			_converters.TryAdd(typeof(GetResponse<IEntity>), new GetJsonConverter<IEntity>());
			_converters.TryAdd(typeof(SearchResponse<IEntity>), new SearchJsonConverter<IEntity>());
		}

		public ElasticMapping<TSettings> AddMapping<T>() where T : class, IEntity
		{
			_mapping.AddOrUpdate(typeof(T), x => new MappingItem<T>(), (t, m) => throw new Exception($"Mapping for type \"{typeof(T).Name}\" already exists."));
			_converters.TryAdd(typeof(T), new InsertJsonConverter<T>());
			return this;
		}

		public ElasticMapping<TSettings> AddStruct<T>() where T : struct
		{
			_converters.TryAdd(typeof(T), new ObjectJsonConverter<T>());
			return this;
		}

		public ElasticMapping<TSettings> AddProjection<T, TMapping>()
			where T : class, IProjection<TMapping>, new()
			where TMapping : class, IEntity
		{
			_projection.AddOrUpdate(typeof(T), x =>
				{
					var result = new ProjectionItem<T, TMapping>((MappingItem<TMapping>) _mapping.GetOrAdd(typeof(TMapping),
						y => throw new Exception($"Not found mapping for type \"{y.Name}\"")));
					_converters.TryAdd(typeof(GetResponse<T>), new GetJsonConverter<T>());
					_converters.TryAdd(typeof(SearchResponse<T>), new SearchJsonConverter<T>());
					return result;
				},
				(t, m) => throw new Exception($"Projection for type \"{typeof(T).Name}\" already exists."));
			return this;
		}

		public ElasticMapping<TSettings> AddProjection<T, TMapping, TParent, TParentMapping>()
			where T : class, IProjection<TMapping>, IWithParent<TParentMapping, TParent>, new()
			where TMapping : class, IEntity, IWithParent<TParentMapping, TParent>
			where TParent : class, IProjection<TParentMapping>, new()
			where TParentMapping : class, IEntity, new()
		{
			_projection.AddOrUpdate(typeof(T), x =>
				{
					var result = new ProjectionWithParentItem<T, TMapping, TParentMapping, TParent>((MappingItem<TMapping>)_mapping.GetOrAdd(typeof(TMapping),
						y => throw new Exception($"Not found mapping for type \"{y.Name}\"")));
					_converters.TryAdd(typeof(GetResponse<T>), new GetJsonConverter<T>());
					_converters.TryAdd(typeof(SearchResponse<T>), new SearchJsonConverter<T>());
					return result;
				},
				(t, m) => throw new Exception($"Projection for type \"{typeof(T).Name}\" already exists."));
			return this;
		}

		/// <summary>
		/// Проверяем индекс и маппинг
		/// </summary>
		internal void Build(Action initAction)
		{
			var connectionPool = new StaticConnectionPool(new[] {_settings.Url});
			var connectionSettings = new ConnectionSettings(connectionPool, new HttpConnection());

			connectionSettings.DefaultFieldNameInferrer(x => x.ToLower());
#if DEBUG
			connectionSettings.PrettyJson();
			connectionSettings.DisableDirectStreaming();
#endif
			connectionSettings.DefaultIndex(_settings.IndexName);

			var client = new ElasticClient(connectionSettings);
			
			if (!client.IndexExists(_settings.IndexName).Exists)
				// https://github.com/elastic/elasticsearch-dsl-py/issues/535
				client.CreateIndex(_settings.IndexName, x => x
						.Settings(s => s
							.Analysis(a => a
								.Analyzers(an => an.Custom("autocomplete", t => t.Tokenizer("autocomplete_token").Filters("lowercase")))
								.Tokenizers(t => t.EdgeNGram("autocomplete_token",
									e => e.MinGram(1).MaxGram(10).TokenChars(TokenChar.Letter, TokenChar.Digit)))))
						.Mappings(z => z.Each(_mapping, m => m.Value.Map(z))))
					.IfNot(x => x.IsValid, x => x
						.LogError(_logger, "Mapping error:\r\n" + x.DebugInformation)
						.Throw(t => new Exception("Create index error")), x => initAction.IfNotNull(f => f()));
			else
				_mapping.Each(m => m.Value.Map(client)
					.IfNot(x => x.IsValid, x => x
						.LogError(_logger, "Mapping error:\r\n" + x.DebugInformation)
						.Throw(t => new Exception("Mapping error"))));
		}

		internal void Build<TRepository>(Action<TRepository> initFunc, TRepository repository)
			where TRepository : BaseRepository<TSettings>
			=> Build(() => initFunc.IfNotNull(f => f(repository)));

		internal void Drop()
		{
			var connectionPool = new StaticConnectionPool(new[] { _settings.Url });
			var connectionSettings = new ConnectionSettings(connectionPool, new HttpConnection());

			connectionSettings.DefaultFieldNameInferrer(x => x.ToLower());
#if DEBUG
			connectionSettings.PrettyJson();
			connectionSettings.DisableDirectStreaming();
#endif
			connectionSettings.DefaultIndex(_settings.IndexName);

			var client = new ElasticClient(connectionSettings);

			if (client.IndexExists(_settings.IndexName).Exists)
				client.DeleteIndex(_settings.IndexName);
		}

		internal bool TryGetResponseJsonConverter(Type type, out JsonConverter result)
			=> _converters.TryGetValue(type, out result);

		internal JsonConverter GetJsonConverter(Type type, IRequestContainer container)
			=> _projection[type].GetJsonConverter(container);

		internal IProjectionItem GetProjectionItem(Type type)
			=> _projection[type];

		internal IProjectionItem GetProjectionItem<T>()
			=> _projection[typeof(T)];
	}
}