using System;
using Core.ElasticSearch;
using Expo3.AdminApp.Projections;
using Expo3.AdminApp.Services;
using Expo3.Model;
using Expo3.Model.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Expo3.AdminApp
{
	public static class BuilderExtensions
	{
		public static IServiceCollection AddExpo3AdminApp(this IServiceCollection services)
		{
			return services
				.AddService<EventService, Expo3ElasticConnection>()
				.AddService<UserService, Expo3ElasticConnection>();
		}

		public static IServiceProvider UseExpo3AdminApp(this IServiceProvider services)
		{
			return services.UseExpo3Projections(p => p
				.AddProjection<EventProjection, Event>()
				
				.AddProjection<NewUserProjection, User>()
				.AddProjection<UserUpdateProjection, User>()
				.AddProjection<UserProjection, User>());
		}
	}
}