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
	    public static IServiceCollection AddExpo3Model(this IServiceCollection services, Uri connectionString, Action<ServiceRegistration<Expo3ElasticConnection>> servicesRegistration)
			=> services.AddElastic(new Expo3ElasticConnection(connectionString), servicesRegistration);

	    public static IServiceProvider UseExpo3Model(this IServiceProvider services, Func<IElasticProjections<Expo3ElasticConnection>, IElasticProjections<Expo3ElasticConnection>> projectionsRegistration, bool forTest = false)
		    => services.UseElastic<Expo3ElasticConnection>(AddMapping, projectionsRegistration, forTest);

	    private static void AddMapping(IElasticMapping<Expo3ElasticConnection> m)
		    => m
			    // маппинг
			    .AddMapping<User>(x => x.UserIndexName)
			    .AddMapping<Event>(x => x.EventIndexName)
			    .AddMapping<OwnerEvent>(x => x.EventIndexName)
			    .AddMapping<Category>(x => x.EventIndexName)
			    // внутренние документы
			    .AddStruct<Price>()
			    .AddStruct<TicketPrice>()
			    .AddStruct<Address>()
			    .AddStruct<EventDateTime>()
			    .AddStruct<EventPage>()
				// проекции
				.AddProjection<BaseUserProjection, User>()
			    .AddProjection<BaseCategoryProjection, Category>();
    }
}
