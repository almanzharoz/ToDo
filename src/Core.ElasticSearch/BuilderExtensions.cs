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
		public static IServiceCollection AddElastic<TSettings>(this IServiceCollection services, TSettings settings)
			where TSettings : BaseElasticSettings
		{
			services.AddSingleton<TSettings>(settings)
				.AddSingleton<ElasticMapping<TSettings>>()
				.AddScoped<RequestContainer<TSettings>>()
				.AddScoped<ElasticClient<TSettings>>();
			return services;
		}

		public static IServiceCollection AddElastic<TSettings>(this IServiceCollection services)
			where TSettings : BaseElasticSettings, new()
			=> AddElastic<TSettings>(services, new TSettings());
		

		public static IServiceCollection AddService<T, TSettings>(this IServiceCollection services)
			where T : BaseService<TSettings>
			where TSettings : BaseElasticSettings
		{
			services.AddScoped<T>();
			return services;
		}

		public static IApplicationBuilder UseElastic<TSettings, TService>(this IApplicationBuilder app, Action<ElasticMapping<TSettings>> mappingFactory, Action<TService> initFunc)
			where TSettings : BaseElasticSettings
			where TService : BaseService<TSettings>

		{
			var mapping = app.ApplicationServices.GetService<ElasticMapping<TSettings>>();
			mappingFactory(mapping);
			mapping.Build(initFunc, app.ApplicationServices.GetService<TService>());
			return app;
		}

		public static void UseElasticForTests<TSettings>(this IServiceProvider services, Action<ElasticMapping<TSettings>> mappingFactory)
			where TSettings : BaseElasticSettings

		{
			var mapping = services.GetService<ElasticMapping<TSettings>>();
			mappingFactory(mapping);
			mapping.Drop();
			mapping.Build(null);
		}

		public static void UseElastic<TSettings>(this IServiceProvider services, Action<ElasticMapping<TSettings>> mappingFactory)
			where TSettings : BaseElasticSettings

		{
			var mapping = services.GetService<ElasticMapping<TSettings>>();
			mappingFactory(mapping);
			mapping.Build(null);
		}

		public static void UseElastic<TSettings, TService>(this IServiceProvider services, Action<ElasticMapping<TSettings>> mappingFactory, Action<TService> initFunc)
			where TSettings : BaseElasticSettings
			where TService : BaseService<TSettings>

		{
			var mapping = services.GetService<ElasticMapping<TSettings>>();
			mappingFactory(mapping);
			mapping.Build(initFunc, services.GetService<TService>());
		}
	}
}