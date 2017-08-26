using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Expo3.Model;
using Expo3.Model.Models;
using Expo3.TestsApp.Projections;
using Expo3.TestsApp.Services;

namespace Expo3.TestsApp
{
	public static class BuilderExtensions
	{
		public static ServiceRegistration<Expo3ElasticConnection> AddExpo3TestsApp(this ServiceRegistration<Expo3ElasticConnection> serviceRegistration)
		{
			return serviceRegistration
				.AddService<TestsUserService>()
				.AddService<TestsEventService>()
                .AddService<TestsCaterogyService>();
		}

		public static IElasticProjections<Expo3ElasticConnection> UseExpo3TestsApp(this IElasticProjections<Expo3ElasticConnection> services)
		{
			return services
				.AddProjection<NewUser, User>()
				.AddProjection<NewEvent, Event>()
				.AddProjection<NewCategory, Category>();
		}
	}
}
