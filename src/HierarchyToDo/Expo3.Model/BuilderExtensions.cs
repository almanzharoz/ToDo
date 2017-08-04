using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Expo3.Model
{
    public static class BuilderExtensions
    {
	    public static IServiceCollection AddExpo3Model(this IServiceCollection services, Uri connectionString)
			=> services.AddElastic(new Expo3ElasticConnection(connectionString));

	    public static IServiceProvider UseExpo3Model(this IServiceProvider services)
		    => services.UseElastic<Expo3ElasticConnection>(
			    m => m
				    // маппинг
				    .AddMapping<User>(x => x.UserIndexName)
				    .AddMapping<Event>(x => x.EventIndexName)
				    .AddMapping<EventPage>(x => x.EventIndexName)
				    .AddMapping<OwnerEvent>(x => x.EventIndexName)
					.AddMapping<Visitor>(x => x.EventIndexName)
				    // внутренние документы
				    .AddStruct<Price>()
				    .AddStruct<TicketPrice>()
				    .AddStruct<Address>()
				    // проекции
				    .AddProjection<BaseUserProjection, User>());

	    public static IServiceProvider UseExpo3Projections(this IServiceProvider services,
		    Action<IElasticProjections<Expo3ElasticConnection>> projectionsFactory)
		    => services.UseElasticProjections(projectionsFactory);
    }
}
