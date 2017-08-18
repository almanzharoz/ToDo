using System;
using Expo3.ClientApp.Services;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Expo3.TestsApp;
using Expo3.TestsApp.Projections;
using Expo3.TestsApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Expo3.ClientApp.Tests
{
    [TestClass]
    public class EventTests
    {

	    private EventService _service;
	    private TestsEventService _testsService;

	    [TestInitialize]
	    public void Setup()
	    {
		    var serviceProvider = new ServiceCollection()
			    .AddExpo3Model(new Uri("http://localhost:9200/"))
			    .AddExpo3ClientApp()
				.AddExpo3TestsApp()
			    .AddSingleton(new UserName("1"))
				.AddLogging()
			    .BuildServiceProvider();

		    serviceProvider
			    .UseExpo3Model(true)
			    .UseExpo3TestsApp()
			    .UseExpo3ClientApp();

		    _service = serviceProvider.GetService<EventService>();
		    _testsService = serviceProvider.GetService<TestsEventService>();

		    serviceProvider.GetService<UserName>().SetId(_testsService.AddUser("user"));

	    }

	    private string AddEvent(string name)
		    => _testsService.AddEvent(name, new EventDateTime(), new Address(), EEventType.Concert,
			    _testsService.AddCategory("Category 1"));

		[TestMethod]
        public void GetEventPage()
		{
			var eventId = AddEvent("Event 1");

			var @event = _service.GetEventById(eventId);

			Assert.AreEqual(@event.Name, "Event 1");
			Assert.AreEqual(@event.Category.Name, "Category 1");
		}
    }
}
