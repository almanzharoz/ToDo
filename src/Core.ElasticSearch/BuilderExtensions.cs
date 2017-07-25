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

		public static IApplicationBuilder UseElastic<TConnection, TService>(this IApplicationBuilder app, Action<IElasticMapping<TConnection>> mappingFactory, Action<TService> initFunc)
			where TConnection : BaseElasticConnection
			where TService : BaseService<TConnection>

		{
			var mapping = app.ApplicationServices.GetService<ElasticMapping<TConnection>>();
			mappingFactory(mapping);
			mapping.Build(initFunc, app.ApplicationServices.GetService<TService>());
			return app;
		}

		public static void UseElasticForTests<TConnection>(this IServiceProvider services, Action<IElasticMapping<TConnection>> mappingFactory)
			where TConnection : BaseElasticConnection

		{
			var mapping = services.GetService<ElasticMapping<TConnection>>();
			mappingFactory(mapping);
			mapping.Drop();
			mapping.Build(null);
		}

		public static void UseElastic<TConnection>(this IServiceProvider services, Action<IElasticMapping<TConnection>> mappingFactory)
			where TConnection : BaseElasticConnection

		{
			var mapping = services.GetService<ElasticMapping<TConnection>>();
			mappingFactory(mapping);
			mapping.Build(null);
		}

		public static void UseElastic<TConnection, TService>(this IServiceProvider services, Action<IElasticMapping<TConnection>> mappingFactory, Action<TService> initFunc)
			where TConnection : BaseElasticConnection
			where TService : BaseService<TConnection>

		{
			var mapping = services.GetService<ElasticMapping<TConnection>>();
			mappingFactory(mapping);
			mapping.Build(initFunc, services.GetService<TService>());
		}
	}
}