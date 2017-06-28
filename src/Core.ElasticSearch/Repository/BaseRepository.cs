using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Mapping;
using Core.ElasticSearch.Serialization;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public abstract partial class BaseRepository<TSettings> 
		where TSettings : BaseElasticSettings
	{
		protected readonly ILogger _logger;
		protected readonly ElasticClient _client;
		protected readonly RequestContainer<TSettings> _container;
		private readonly TSettings _settings;
		private readonly ElasticMapping<TSettings> _mapping;

		protected BaseRepository(ILoggerFactory loggerFactory, TSettings settings, ElasticMapping<TSettings> mapping, RequestContainer<TSettings> container)
		{
			_container = container;
			_settings = settings;
			_mapping = mapping;
			var connectionPool = new StaticConnectionPool(new[] { _settings.Url });
			var connectionSettings = new ConnectionSettings(connectionPool, new HttpConnection(), new SerializerFactory(values => new ElasticSerializer<TSettings>(values, _mapping, container)));

			connectionSettings.DefaultFieldNameInferrer(x => x.ToLower());
			connectionSettings.DisablePing();
			connectionSettings.DisableAutomaticProxyDetection();
			//connectionSettings.EnableDebugMode(x => { x.AuditTrail.Clear(); });
#if DEBUG
			connectionSettings.PrettyJson();
			connectionSettings.DisableDirectStreaming();
#endif
			connectionSettings.DefaultIndex(_settings.IndexName);

			_client = new ElasticClient(connectionSettings);
			_logger = loggerFactory.CreateLogger<BaseRepository<TSettings>>();
		}

		protected async Task LoadAsync()
		{
			// TODO: Проблема: могут запрашиваться поля не входящие в проекцию, т.к. одинаково имя поля из другой проекции
			var entitiesToLoad = _container.PopEntitiesForLoad();
			do
			{
				foreach (var item in entitiesToLoad)
				{
#if DEBUG
					_logger.LogDebug($"Loading data: {Newtonsoft.Json.JsonConvert.SerializeObject(item)}");
					_logger.LogDebug((await _client.SearchAsync<IEntity>(x => x
						.Type(Types.Type(item.types))
						.Source(s => s.Includes(f => f.Fields(item.fields.ToArray())))
						.Query(q => q.Ids(id => id.Values(item.ids))))).DebugInformation);
#else
				await _client.SearchAsync<IEntity>(x => x
					.Type(Types.Type(item.types))
					.Source(s => s.Includes(f => f.Fields(item.fields.ToArray())))
					.Query(q => q.Ids(id => id.Values(item.ids))));
#endif
				}
			} while ((entitiesToLoad = _container.PopEntitiesForLoad()).Any());
		}

		protected void Load()
		{
			var sw = new Stopwatch();
			sw.Start();
			// TODO: Проблема: могут запрашиваться поля не входящие в проекцию, т.к. одинаково имя из другой проекции
			var entitiesToLoad = _container.PopEntitiesForLoad();
			do
			{
				foreach (var item in entitiesToLoad)
				{
#if DEBUG
					_logger.LogDebug($"Loading data: {Newtonsoft.Json.JsonConvert.SerializeObject(item)}");
					_logger.LogDebug((_client.Search<IEntity>(x => x
						.Type(Types.Type(item.types))
						.Source(s => s.Includes(f => f.Fields(item.fields.ToArray())))
						.Query(q => q.Ids(id => id.Values(item.ids))))).DebugInformation);
#else
				_client.Search<IEntity>(x => x
					.Type(Types.Type(item.types))
					.Source(s => s.Includes(f => f.Fields(item.fields.ToArray())))
					.Query(q => q.Ids(id => id.Values(item.ids))));
#endif
				}
			} while ((entitiesToLoad = _container.PopEntitiesForLoad()).Any());
			sw.Stop();
			Debug.WriteLine("Load: " + sw.ElapsedMilliseconds);
		}

		#region Try
		protected TResult Try<TResponse, TResult>(Func<ElasticClient, TResponse> func, Func<TResponse, TResult> result, EventId eventId, string operationText = null)
			where TResponse : IResponse
		{
			try
			{
				var sw = new Stopwatch();
				return result(_client.Stopwatch(sw, x=> func(_client)).Fluent(x => Debug.WriteLine("Try: "+sw.ElapsedMilliseconds))
					.LogDebug(_logger, operationText ?? eventId.Name)
					.LogError(_logger, operationText ?? eventId.Name));
			}
			catch (Exception e)
			{
				_logger.LogCritical(eventId, e, operationText ?? eventId.Name);
				throw;
			}
		}

		protected async Task<TResult> TryAsync<TResponse, TResult>(Func<ElasticClient, Task<TResponse>> func, Func<TResponse, TResult> result, EventId eventId, string operationText = null)
			where TResponse : IResponse
		{
			try
			{
				return result((await func(_client))
					.LogDebug(_logger, operationText ?? eventId.Name)
					.LogError(_logger, operationText ?? eventId.Name));
			}
			catch (Exception e)
			{
				_logger.LogCritical(eventId, e, operationText ?? eventId.Name);
				throw;
			}
		}

		protected async Task<TResult> TryAsync<TResponse, TResult>(Func<ElasticClient, Task<TResponse>> func, Func<TResponse, Task<TResult>> result, EventId eventId, string operationText = null)
			where TResponse : IResponse
		{
			try
			{
				return await result((await func(_client))
					.LogDebug(_logger, operationText ?? eventId.Name)
					.LogError(_logger, operationText ?? eventId.Name));
			}
			catch (Exception e)
			{
				_logger.LogCritical(eventId, e, operationText ?? eventId.Name);
				throw;
			}
		}
		#endregion Try
	}

	public static class RepositoryLoggingEvents
	{
		public static EventId ES_SEARCH = new EventId(1000, "Search");
		public static EventId ES_GET = new EventId(1001, "Get");
		public static EventId ES_INSERT = new EventId(1002, "Insert");
		public static EventId ES_UPDATE = new EventId(1003, "Update");
		public static EventId ES_UPDATEBYQUERY = new EventId(1004, "UpdateByQuery");
		public static EventId ES_REMOVE = new EventId(1005, "Remove");
		public static EventId ES_REMOVEBYQUERY = new EventId(1006, "RemoveByQuery");
		public static EventId ES_COUNT = new EventId(1007, "Count");

	}

	internal static class ElasticExtensions
	{
		public static T LogError<T>(this T arg, ILogger logger, string text) where T : IResponse
			=> arg.IfNot(x => x.IsValid, x => logger
				.Fluent(z => z.LogError($"{text}: {x.ServerError.ToString()}\r\n{x.DebugInformation}"))
				.Throw(z => new Exception("Query error")));

		public static T LogDebug<T>(this T arg, ILogger logger, string text = null) where T : IResponse
		{
#if DEBUG
			if (arg.IsValid)
				logger.LogDebug(!String.IsNullOrWhiteSpace(text) ? $"{text}: {arg.DebugInformation}" : arg.DebugInformation);
#endif
			return arg;
		}

	}
}
