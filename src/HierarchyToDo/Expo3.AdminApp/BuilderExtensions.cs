using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Expo3.AdminApp.Projections;
using Expo3.AdminApp.Services;
using Expo3.Model;
using Expo3.Model.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Expo3.AdminApp
{
	public static class BuilderExtensions
	{
		public static ServiceRegistration<Expo3ElasticConnection> AddExpo3AdminApp(this ServiceRegistration<Expo3ElasticConnection> serviceRegistration)
		{
			return serviceRegistration
				.AddService<EventService>()
				.AddService<UserService>();
		}

		public static IElasticProjections<Expo3ElasticConnection> UseExpo3AdminApp(this IElasticProjections<Expo3ElasticConnection> services)
		{
			return services
				.AddProjection<EventProjection, Event>()
				
				.AddProjection<NewUserProjection, User>()
				.AddProjection<UserUpdateProjection, User>()
				.AddProjection<UserProjection, User>();
		}
	}
}