using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using BeeFee.ClientApp.Projections;
using BeeFee.ClientApp.Services;
using BeeFee.Model;
using BeeFee.Model.Models;

namespace BeeFee.ClientApp
{
    public static class BuilderExtensions
    {
        public static ServiceRegistration<BeefeeElasticConnection> AddBeefeeClientApp(this ServiceRegistration<BeefeeElasticConnection> serviceRegistration)
		{
            return serviceRegistration
				.AddService<EventService>();
        }

        public static IElasticProjections<BeefeeElasticConnection> UseBeefeeClientApp(this IElasticProjections<BeefeeElasticConnection> services)
        {
            return services
                .AddProjection<EventProjection, Event>()
                .AddProjection<EventCellProjection, Event>()
                .AddProjection<EventAddressProjections, Event>()
				
				.AddProjection<CategoryProjection, Category>();

        }
    }
}