using System;
using Core.ElasticSearch;
using Expo3.Model;
using Expo3.Model.Models;
using Expo3.OrganizersApp.Projections;
using Expo3.OrganizersApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Expo3.OrganizersApp
{
	public static class BuilderExtensions
	{
		public static IServiceCollection AddExpo3OrganizerApp(this IServiceCollection services)
		{
			return services
				.AddService<EventService, Expo3ElasticConnection>();
		}

		public static IServiceProvider UseExpo3OrganizerApp(this IServiceProvider services)
		{
			return services.UseExpo3Projections(p => p
				.AddProjection<EventProjection, Event>());
		}
	}
}