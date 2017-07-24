 using System;
using System.Collections.Concurrent;
using System.Diagnostics;
 using System.Linq;
 using System.Reflection;
 using System.Threading;
 using BenchmarkDotNet.Extensions;
 using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Serialization;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;
using SharpFuncExt;

namespace Core.ElasticSearch.Mapping
{
	public interface IElasticMapping<TSettings> where TSettings : BaseElasticSettings
	{
		IElasticMapping<TSettings> AddMapping<T>(Func<TSettings, string> indexName) where T : class, IModel;
		IElasticMapping<TSettings> AddStruct<T>() where T : struct;

		IElasticMapping<TSettings> AddProjection<T, TMapping>()
			where T : BaseEntity, IProjection<TMapping>, new()
			where TMapping : class, IModel;

		IElasticMapping<TSettings> AddProjection<T, TMapping, TParent>()
			where T : BaseEntity, IProjection<TMapping>, IWithParent<TParent>, new()
			where TMapping : class, IModel, IWithParent<TParent>
			where TParent : BaseEntity, IProjection, new();

		string GetIndexName<T>();
	}

	internal class ElasticMapping<TSettings> : IElasticMapping<TSettings> where TSettings : BaseElasticSettings
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
			_converters.TryAdd(typeof(GetResponse<IProjection>), new GetJsonConverter<IProjection>());
			_converters.TryAdd(typeof(SearchResponse<IProjection>), new SearchJsonConverter<IProjection>());
		}

		/// <summary>
		/// Регистрирует маппинг
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IElasticMapping<TSettings> AddMapping<T>(Func<TSettings, string> indexName) where T : class, IModel
		{
			_mapping.AddOrUpdate(typeof(T), x => new MappingItem<T, TSettings>(_settings, indexName),
				(t, m) => throw new Exception($"Mapping for type \"{typeof(T).Name}\" already exists."));
			//_converters.TryAdd(typeof(T), new InsertJsonConverter<T>()); // объекты этого типа доступны только для вставки
			return this;
		}

		public string GetIndexName<T>() => _mapping[typeof(T)].IndexName;


		/// <summary>
		/// Регистрирует внутренний документ
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IElasticMapping<TSettings> AddStruct<T>() where T : struct
		{
			_converters.TryAdd(typeof(T), new ObjectJsonConverter<T>());
			return this;
		}

		/// <summary>
		/// Регистрирует проекцию
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TMapping"></typeparam>
		/// <returns></returns>
		public IElasticMapping<TSettings> AddProjection<T, TMapping>()
			where T : BaseEntity, IProjection<TMapping>, new()
			where TMapping : class, IModel
		{
			_projection.AddOrUpdate(typeof(T), x =>
				{
					var result = new ProjectionItem<T, TMapping, TSettings>((MappingItem<TMapping, TSettings>) _mapping.GetOrAdd(typeof(TMapping),
						y => throw new Exception($"Not found mapping for type \"{y.Name}\"")));
					if (typeof(IInsertProjection).IsAssignableFrom(x))
						_converters.TryAdd(typeof(T), new InsertJsonConverter<T>(result));
					if (typeof(IGetProjection).IsAssignableFrom(x))
						_converters.TryAdd(typeof(GetResponse<T>), new GetJsonConverter<T>());
					if (typeof(ISearchProjection).IsAssignableFrom(x))
						_converters.TryAdd(typeof(SearchResponse<T>), new SearchJsonConverter<T>());
					//_converters.TryAdd(typeof(UpdateResponse<T>), new UpdateResultJsonConverter<T>());
					return result;
				},
				(t, m) => throw new Exception($"Projection for type \"{typeof(T).Name}\" already exists."));
			return this;
		}

		/// <summary>
		/// Регистрирует проекцию типа с парентом
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TMapping"></typeparam>
		/// <typeparam name="TParent"></typeparam>
		/// <returns></returns>
		public IElasticMapping<TSettings> AddProjection<T, TMapping, TParent>()
			where T : BaseEntity, IProjection<TMapping>, IWithParent<TParent>, new()
			where TMapping : class, IModel, IWithParent<TParent>
			where TParent : BaseEntity, IProjection, new()
		{
			_projection.AddOrUpdate(typeof(T), x =>
				{
					var result = new ProjectionWithParentItem<T, TMapping, TParent, TSettings>((MappingItem<TMapping, TSettings>)_mapping.GetOrAdd(typeof(TMapping),
						y => throw new Exception($"Not found mapping for type \"{y.Name}\"")));
					if (typeof(IInsertProjection).IsAssignableFrom(x))
						_converters.TryAdd(typeof(T), new InsertJsonConverter<T>(result));
					if (typeof(IGetProjection).IsAssignableFrom(x))
						_converters.TryAdd(typeof(GetResponse<T>), new GetJsonConverter<T>());
					if (typeof(ISearchProjection).IsAssignableFrom(x))
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

			var client = new ElasticClient(connectionSettings);

			foreach (var mapping in _mapping.GroupBy(s => s.Value.IndexName))
			{
				if (!client.IndexExists(mapping.Key).Exists)
					// https://github.com/elastic/elasticsearch-dsl-py/issues/535
					client.CreateIndex(mapping.Key, x => x
							.Settings(s => s
								.Analysis(a => a
									.Analyzers(an => an.Custom("autocomplete", t => t.Tokenizer("autocomplete_token").Filters("lowercase")))
									.Tokenizers(t => t.EdgeNGram("autocomplete_token",
										e => e.MinGram(1).MaxGram(10).TokenChars(TokenChar.Letter, TokenChar.Digit)))))
							.Mappings(z => z.Each(mapping, m => m.Value.Map(z, _mapping.GetValueOrDefault))))
						.IfNot(x => x.IsValid, x => x
							.LogError(_logger, "Mapping error:\r\n" + x.DebugInformation)
							.Throw(t => new Exception("Create index error")), x => initAction.IfNotNull(f => f()));
				else
					mapping.Each(m => m.Value.Map(client, _mapping.GetValueOrDefault)
						.IfNot(x => x.IsValid, x => x
							.LogError(_logger, "Mapping error:\r\n" + x.DebugInformation)
							.Throw(t => new Exception("Mapping error"))));
			}
		}

		internal void Build<TRepository>(Action<TRepository> initFunc, TRepository repository)
			where TRepository : BaseService<TSettings>
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

			var client = new ElasticClient(connectionSettings);

			foreach (var indexName in _mapping.Select(s => s.Value.IndexName).Distinct())
				if (client.IndexExists(indexName).Exists)
					client.DeleteIndex(indexName);
		}

		internal bool TryGetResponseJsonConverter(Type type, out JsonConverter result)
			=> _converters.TryGetValue(type, out result);

		internal JsonConverter GetJsonConverter(Type type, IRequestContainer container)
			=> _projection[type].GetJsonConverter(container);

		internal IProjectionItem GetProjectionItem(Type type)
			=> _projection[type];

		internal IProjectionItem GetProjectionItem<T>() where T : IProjection
			=> _projection[typeof(T)];
	}
}