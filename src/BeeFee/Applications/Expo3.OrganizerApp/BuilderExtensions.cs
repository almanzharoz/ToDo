using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Expo3.Model;
using Expo3.Model.Models;
using Expo3.OrganizerApp.Projections;
using Expo3.OrganizerApp.Services;

namespace Expo3.OrganizerApp
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
				.AddProjection<NewEvent, Event>()
				
				.AddProjection<CategoryProjection, Category>();
		}
	}
}