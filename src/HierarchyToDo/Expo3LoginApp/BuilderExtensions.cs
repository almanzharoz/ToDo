using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Expo3.LoginApp.Projections;
using Expo3.LoginApp.Services;
using Expo3.Model;
using Expo3.Model.Models;

namespace Expo3.LoginApp
{
    public static class BuilderExtensions
	{
		public static ServiceRegistration<Expo3ElasticConnection> AddExpo3LoginApp(this ServiceRegistration<Expo3ElasticConnection> serviceRegistration)
			=> serviceRegistration
				.AddService<AuthorizationService>();

		public static IElasticProjections<Expo3ElasticConnection> UseExpo3LoginApp(this IElasticProjections<Expo3ElasticConnection> services)
			=> services
				.AddProjection<UserProjection, User>()
				.AddProjection<RegisterUserProjection, User>();
	}
}
