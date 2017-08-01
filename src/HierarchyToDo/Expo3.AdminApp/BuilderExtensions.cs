using System;
using Core.ElasticSearch;
using Expo3.AdminApp.Projections;
using Expo3.AdminApp.Services;
using Expo3.Model;
using Expo3.Model.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Expo3.AdminApp
{
	public static class BuilderExtensions
	{
		public static IServiceCollection AddExpo3AdminApp(this IServiceCollection services, Uri connectionString)
		{
			return services
				.AddService<EventService, Expo3ElasticConnection>()
				.AddService<UsersService, Expo3ElasticConnection>();
		}

		public static IApplicationBuilder UseExpo3AdminApp(this IApplicationBuilder app)
		{
			return app.UseExpo3Projections(p => p
				.AddProjection<EventProjection, Event>()
				.AddProjection<EventRemoveProjection, Event>());
		}
	}
}