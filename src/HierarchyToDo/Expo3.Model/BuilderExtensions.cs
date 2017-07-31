using System;
using System.Collections.Generic;
using System.Text;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Expo3.Model.Embed;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Expo3.Model
{
    public static class BuilderExtensions
    {
	    public static IServiceCollection AddExpo3Model(this IServiceCollection services, Uri connectionString)
			=> services.AddElastic(new Expo3ElasticConnection(connectionString));

	    public static IApplicationBuilder UseExpo3Model(this IApplicationBuilder app)
		    => app.UseElastic<Expo3ElasticConnection>(
			    m => m
				    // маппинг
				    .AddMapping<User>(x => x.UserIndexName)
				    .AddMapping<Event>(x => x.EventIndexName)
				    // внутренние документы
				    .AddStruct<Price>()
				    .AddStruct<Address>()
				    // проекции
				    .AddProjection<BaseUserProjection, User>());

	    public static IApplicationBuilder UseExpo3Projections(this IApplicationBuilder app,
		    Action<IElasticProjections<Expo3ElasticConnection>> projectionsFactory)
		    => app.UseElasticProjections(projectionsFactory);
    }
}
