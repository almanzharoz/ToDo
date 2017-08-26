using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Expo3.ClientApp.Projections;
using Expo3.ClientApp.Projections.Event;
using Expo3.ClientApp.Services;
using Expo3.Model;
using Expo3.Model.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Expo3.ClientApp
{
    public static class BuilderExtensions
    {
        public static ServiceRegistration<Expo3ElasticConnection> AddExpo3ClientApp(this ServiceRegistration<Expo3ElasticConnection> serviceRegistration)
		{
            return serviceRegistration
				.AddService<EventService>();
        }

        public static IElasticProjections<Expo3ElasticConnection> UseExpo3ClientApp(this IElasticProjections<Expo3ElasticConnection> services)
        {
            return services
                .AddProjection<EventProjection, Event>()
                .AddProjection<EventCellProjection, Event>()
                .AddProjection<EventAddressProjections, Event>()
                .AddProjection<EventPageProjection, Event>();

        }
    }
}