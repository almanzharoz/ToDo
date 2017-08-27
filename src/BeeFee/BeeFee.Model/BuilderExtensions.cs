﻿using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BeeFee.Model
{
    public static class BuilderExtensions
    {
	    public static IServiceCollection AddBeefeeModel(this IServiceCollection services, Uri connectionString, Action<ServiceRegistration<BeefeeElasticConnection>> servicesRegistration)
			=> services.AddElastic(new BeefeeElasticConnection(connectionString), servicesRegistration);

	    public static IServiceProvider UseBeefeeModel(this IServiceProvider services, Func<IElasticProjections<BeefeeElasticConnection>, IElasticProjections<BeefeeElasticConnection>> projectionsRegistration, bool forTest = false)
		    => services.UseElastic<BeefeeElasticConnection>(AddMapping, projectionsRegistration, forTest);

	    private static void AddMapping(IElasticMapping<BeefeeElasticConnection> m)
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
