using System;
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

		private readonly Func<IServiceCollection, IServiceCollection> _addAppFunc;
		private readonly Func<IServiceProvider, IServiceProvider> _useAppFunc;
		private readonly EUserRole[] _roles;
		private TestsEventService _eventService;
		private TestsCaterogyService _categoryService;

		protected BaseTestClass(Func<IServiceCollection, IServiceCollection> addAppFunc,
			Func<IServiceProvider, IServiceProvider> useAppFunc, EUserRole[] roles)
		{
			_addAppFunc = addAppFunc;
			_useAppFunc = useAppFunc;
			_roles = roles;
		}

		public virtual void Setup()
		{
			var serviceProvider = new ServiceCollection()
				.AddExpo3Model(new Uri("http://localhost:9200/"))
				.AddExpo3TestsApp()
				.AddExpo3TestedApp(_addAppFunc)
				.AddSingleton(x => new UserName(x.GetService<TestsUserService>().AddUser("user@mail.ru", "123", "user", _roles)))
				.AddLogging()
				.BuildServiceProvider();

			serviceProvider
				.UseExpo3Model(true)
				.UseExpo3TestsApp()
				.UseExpo3TestedApp(_useAppFunc);

			Service = serviceProvider.GetService<TService>();

			_eventService = serviceProvider.GetService<TestsEventService>();
			_categoryService = serviceProvider.GetService<TestsCaterogyService>();

		}

		protected string AddEvent(string categoryId, string name)
			=> _eventService.AddEvent(name, new EventDateTime(), new Address(), EEventType.Concert, categoryId, 0);

		protected string AddCategory(string name)
			=> _categoryService.Add(name);
	}
}