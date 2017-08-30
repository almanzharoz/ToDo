using System;
using Core.ElasticSearch.Mapping;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Core.ElasticSearch
{
	/// <summary>
	/// Обертка для internal классов
	/// </summary>
	/// <typeparam name="TConnection"></typeparam>
	public class ElasticScopeFactory<TConnection>
		where TConnection : BaseElasticConnection
	{
		private readonly IServiceProvider _service;

		internal ElasticClient<TConnection> Client => _client;
		
		internal static ElasticClient<TConnection> _client;
		internal static object _clientLocker = new object();
		internal RequestContainer<TConnection> Container { get; }
		internal ElasticMapping<TConnection> Mapping { get; }

		public ElasticScopeFactory(IServiceProvider service, TConnection settings)
		{
			_service = service;
			Mapping = service.GetService<ElasticMapping<TConnection>>();
			Container = new RequestContainer<TConnection>(Mapping);
			lock (_clientLocker)
				if (_client == null)
					_client = new ElasticClient<TConnection>(settings, Mapping, Container);
		}

		public T GetInternalService<T>() where T : BaseService<TConnection>
			=> _service.GetService<T>();
	}
}