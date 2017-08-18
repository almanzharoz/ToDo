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

	    public static IServiceProvider UseExpo3Model(this IServiceProvider services, bool forTest)
		    => services.UseElastic<Expo3ElasticConnection>(AddMapping, forTest);

	    public static IServiceProvider UseExpo3Model<TService>(this IServiceProvider services, bool forTest, Action<TService> init)
			where TService : BaseService<Expo3ElasticConnection>
		    => services.UseElastic<Expo3ElasticConnection, TService>(AddMapping, forTest, init);

		public static IServiceProvider UseExpo3Projections(this IServiceProvider services,
		    Action<IElasticProjections<Expo3ElasticConnection>> projectionsFactory)
		    => services.UseElasticProjections(projectionsFactory);

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
