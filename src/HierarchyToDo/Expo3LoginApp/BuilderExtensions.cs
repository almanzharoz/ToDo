using System;
using Core.ElasticSearch;
using Expo3.LoginApp.Projections;
using Expo3.LoginApp.Services;
using Expo3.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Expo3.LoginApp
{
    public static class BuilderExtensions
	{
		public static IServiceCollection AddExpo3LoginApp(this IServiceCollection services, Uri connectionString)
			=> services
				.AddService<AuthorizationService, Expo3ElasticConnection>();

		public static IApplicationBuilder UseExpo3LoginApp(this IApplicationBuilder app)
			=> app.UseExpo3Projections(p => p
				.AddProjection<UserProjection, User>()
				.AddProjection<RegisterUserProjection, User>());
	}
}
