using System;
using Core.ElasticSearch;
using Expo3.Model;
using Expo3.Model.Models;
using Expo3.TestsApp.Projections;
using Expo3.TestsApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Expo3.TestsApp
{
	public static class BuilderExtensions
	{
		public static IServiceCollection AddExpo3TestsApp(this IServiceCollection services)
		{
			return services
				.AddService<TestsUserService, Expo3ElasticConnection>()
				.AddService<TestsEventService, Expo3ElasticConnection>()
                .AddService<TestsCaterogyService, Expo3ElasticConnection>();
		}

		internal static IServiceCollection AddExpo3TestedApp(this IServiceCollection services, Func<IServiceCollection, IServiceCollection> addAppFunc)
			=> addAppFunc(services);

		public static IServiceProvider UseExpo3TestsApp(this IServiceProvider services)
		{
			return services.UseExpo3Projections(p => p
				.AddProjection<NewUser, User>()
				.AddProjection<NewEvent, Event>()
				.AddProjection<NewCategory, Category>());
		}

		internal static IServiceProvider UseExpo3TestedApp(this IServiceProvider services,
			Func<IServiceProvider, IServiceProvider> useAppFunc)
			=> useAppFunc(services);
	}
}
