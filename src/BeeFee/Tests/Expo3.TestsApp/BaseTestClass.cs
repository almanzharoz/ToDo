using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Expo3.TestsApp.Projections;
using Expo3.TestsApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Expo3.TestsApp
{
	public abstract class BaseTestClass<TService> where TService : BaseExpo3Service
	{
		protected TService Service { get; private set; }

		private readonly Func<ServiceRegistration<Expo3ElasticConnection>, ServiceRegistration<Expo3ElasticConnection>> _servicesRegistration;
		private readonly Func<IElasticProjections<Expo3ElasticConnection>, IElasticProjections<Expo3ElasticConnection>> _useAppFunc;
		private readonly EUserRole[] _roles;
		private TestsEventService _eventService;
		private TestsCaterogyService _categoryService;

		protected BaseTestClass(
			Func<ServiceRegistration<Expo3ElasticConnection>, ServiceRegistration<Expo3ElasticConnection>> servicesRegistration,
			Func<IElasticProjections<Expo3ElasticConnection>, IElasticProjections<Expo3ElasticConnection>> useAppFunc,
			EUserRole[] roles)
		{
			_servicesRegistration = servicesRegistration;
			_useAppFunc = useAppFunc;
			_roles = roles;
		}

		public virtual void Setup()
		{
			var serviceProvider = new ServiceCollection()
				.AddExpo3Model(new Uri("http://localhost:9200/"), s => _servicesRegistration(s
					.AddExpo3TestsApp()))
				.AddSingleton(x => new UserName(x.GetService<TestsUserService>().AddUser("user@mail.ru", "123", "user", _roles)))
				.AddLogging()
				.BuildServiceProvider();

			serviceProvider
				.UseExpo3Model(s => _useAppFunc(s.UseExpo3TestsApp()), true);

			Service = serviceProvider.GetService<TService>();

			_eventService = serviceProvider.GetService<TestsEventService>();
			_categoryService = serviceProvider.GetService<TestsCaterogyService>();

		}

		protected string AddEvent(string categoryId, string name, EventDateTime date=default(EventDateTime), Address address=default(Address), EEventType type= EEventType.None, decimal price=0)
			=> _eventService.AddEvent(name, date, address, type, categoryId, price);

		protected string AddCategory(string name)
			=> _categoryService.Add(name);
	}
}