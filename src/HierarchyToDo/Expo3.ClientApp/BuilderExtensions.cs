using System;
using Core.ElasticSearch;
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
        public static IServiceCollection AddExpo3ClientApp(this IServiceCollection services)
        {
            return services
                .AddService<EventService, Expo3ElasticConnection>();
        }

        public static IServiceProvider UseExpo3ClientApp(this IServiceProvider services)
        {
            return services.UseExpo3Projections(x => x
                .AddProjection<EventProjection, Event>()
                .AddProjection<EventCellProjection, Event>()
                .AddProjection<EventAddressProjections, Event>()
                .AddProjection<EventPageProjection, Event>());

        }
    }
}