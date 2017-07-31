using System;
using Core.ElasticSearch;
using Expo3.AdminApp.Projections;
using Expo3.AdminApp.Services;
using Expo3.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Expo3.AdminApp
{
	public static class BuilderExtensions
	{
		public static IServiceCollection AddExpo3AdminApp(this IServiceCollection services, Uri connectionString)
			=> services
			.AddService<EventService, Expo3ElasticConnection>()
			.AddService<UsersService, Expo3ElasticConnection>();

		public static IApplicationBuilder UseExpo3AdminApp(this IApplicationBuilder app)
			=> app.UseExpo3Projections(p => p
				.AddProjection<EventProjection, Event>()
				.AddProjection<EventRemoveProjection, Event>());
	}
}