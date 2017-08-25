using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Expo3.Model;
using Expo3.Model.Models;
using Expo3.OrganizersApp.Projections;
using Expo3.OrganizersApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Expo3.OrganizersApp
{
	public static class BuilderExtensions
	{
		public static ServiceRegistration<Expo3ElasticConnection> AddExpo3OrganizerApp(this ServiceRegistration<Expo3ElasticConnection> serviceRegistration)
		{
			return serviceRegistration
				.AddService<EventService>();
		}

		public static IElasticProjections<Expo3ElasticConnection> UseExpo3OrganizerApp(this IElasticProjections<Expo3ElasticConnection> services)
		{
			return services
				.AddProjection<EventProjection, Event>()
				.AddProjection<EventRemoveProjection, Event>();
		}
	}
}