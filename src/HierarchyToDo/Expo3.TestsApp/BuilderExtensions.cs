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
				.AddService<TestsEventService, Expo3ElasticConnection>()
                .AddService<TestsCaterogyService, Expo3ElasticConnection>();
		}

		public static IServiceProvider UseExpo3TestsApp(this IServiceProvider services)
		{
			return services.UseExpo3Projections(p => p
				.AddProjection<NewUserProjection, User>()
				.AddProjection<NewEventProjection, Event>()
				.AddProjection<NewCategoryProjection, Category>());
		}
	}
}
