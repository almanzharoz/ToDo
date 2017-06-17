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
				.AddScoped<RequestContainer<TSettings>>();
			return services;
		}

		public static IServiceCollection AddRepository<T, TSettings>(this IServiceCollection services)
			where T : BaseRepository<TSettings>
			where TSettings : BaseElasticSettings
		{
			services.AddScoped<T>();
			return services;
		}

		public static IApplicationBuilder UseElastic<TSettings>(this IApplicationBuilder app, Action<ElasticMapping<TSettings>> mappingFactory)
			where TSettings : BaseElasticSettings
		{
			var mapping = app.ApplicationServices.GetService<ElasticMapping<TSettings>>();
			mappingFactory(mapping);
			mapping.Build();
			return app;
		}
	}
}