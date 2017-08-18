using System;
using Core.ElasticSearch.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public static class BuilderExtensions
	{
		public static IServiceCollection AddElastic<TConnection>(this IServiceCollection services, TConnection settings)
			where TConnection : BaseElasticConnection
		{
			services
				.AddSingleton<TConnection>(settings)
				.AddSingleton<ElasticMapping<TConnection>>()
				.AddScoped<ElasticScopeFactory<TConnection>>();
			return services;
		}

		public static IServiceCollection AddElastic<TConnection>(this IServiceCollection services)
			where TConnection : BaseElasticConnection, new()
			=> AddElastic<TConnection>(services, new TConnection());
		

		public static IServiceCollection AddService<T, TConnection>(this IServiceCollection services)
			where T : BaseService<TConnection>
			where TConnection : BaseElasticConnection
		{
			services.AddScoped<T>();
			return services;
		}

		public static IServiceProvider UseElastic<TConnection, TService>(this IServiceProvider services, Action<IElasticMapping<TConnection>> mappingFactory, Action<TService> initFunc)
			where TConnection : BaseElasticConnection
			where TService : BaseService<TConnection>

		{
			var mapping = services.GetService<ElasticMapping<TConnection>>();
			mappingFactory(mapping);
			mapping.Build(initFunc, services.GetService<TService>());
			return services;
		}

		public static IServiceProvider UseElasticForTests<TConnection>(this IServiceProvider services, Action<IElasticMapping<TConnection>> mappingFactory)
			where TConnection : BaseElasticConnection

		{
			var mapping = services.GetService<ElasticMapping<TConnection>>();
			mappingFactory(mapping);
			mapping.Drop();
			mapping.Build(null);
			return services;
		}

		public static IServiceProvider UseElastic<TConnection>(this IServiceProvider services, Action<IElasticMapping<TConnection>> mappingFactory, bool forTest)
			where TConnection : BaseElasticConnection
		{
			var mapping = services.GetService<ElasticMapping<TConnection>>();
			mappingFactory(mapping);
			if (forTest)
				mapping.Drop();
			mapping.Build(null);
			return services;
		}

		public static IServiceProvider UseElastic<TConnection, TService>(this IServiceProvider services, Action<IElasticMapping<TConnection>> mappingFactory, bool forTest, Action<TService> initFunc)
			where TConnection : BaseElasticConnection
			where TService : BaseService<TConnection>
		{
			var mapping = services.GetService<ElasticMapping<TConnection>>();
			mappingFactory(mapping);
			if (forTest)
				mapping.Drop();
			mapping.Build(initFunc, services.GetService<TService>());
			return services;
		}

		public static IServiceProvider UseElasticProjections<TConnection>(this IServiceProvider services, Action<IElasticProjections<TConnection>> projectionsFactory)
			where TConnection : BaseElasticConnection

		{
			var mapping = services.GetService<ElasticMapping<TConnection>>();
			projectionsFactory(mapping);
			return services;
		}
	}
}