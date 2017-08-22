using System;
using System.Collections.Generic;
using System.Linq;
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
    public class EventServiceTests
    {

        private EventService _service;
        private TestsEventService _testsEventService;
        private TestsCaterogyService _testsCaterogyService;

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
            _testsEventService = serviceProvider.GetService<TestsEventService>();
            _testsCaterogyService = serviceProvider.GetService<TestsCaterogyService>();

            serviceProvider.GetService<UserName>().SetId(_testsEventService.AddUser("user"));

        }

        private string AddEvent(string name)
            => _testsEventService.AddEvent(name, new EventDateTime(), new Address(), EEventType.Concert,
                _testsCaterogyService.AddCategory("Category 1"), 0);

        [TestMethod]
        public void GetEventPage()
        {
            var eventId = AddEvent("Event 1");

            var @event = _service.GetEventById(eventId);

            Assert.AreEqual(@event.Name, "Event 1");
            Assert.AreEqual(@event.Category.Name, "Category 1");
        }

        [TestMethod]
        public void SearchByDate()
        {
            var dateTimeNow = DateTime.UtcNow;
            _testsEventService.AddEvent("Event 1", new EventDateTime() { Start = dateTimeNow, Finish = dateTimeNow.AddDays(1) }, new Address() { City = "Yekaterinburg" }, EEventType.Concert,
                _testsCaterogyService.AddCategory("Category 1"), 0);

            var events = _service.SearchEvents(startDateTime: dateTimeNow.AddDays(-1), endDateTime: dateTimeNow.AddDays(1));

            Assert.IsTrue(events.Any());

            events = _service.SearchEvents(startDateTime: dateTimeNow.AddDays(1), endDateTime: dateTimeNow.AddDays(2));

            Assert.IsTrue(!events.Any());

            events = _service.SearchEvents(startDateTime: dateTimeNow.AddDays(-3), endDateTime: dateTimeNow.AddDays(-2));

            Assert.IsTrue(!events.Any());
        }

        [TestMethod]
        public void SearchByText()
        {
            var dateTimeNow = DateTime.UtcNow;
            _testsEventService.AddEvent("Event 1", new EventDateTime() { Start = dateTimeNow, Finish = dateTimeNow.AddDays(1) }, new Address() { City = "Yekaterinburg" }, EEventType.Concert,
                _testsCaterogyService.AddCategory("Category 1"), 0);

            var events = _service.SearchEvents("Event 1");

            Assert.IsTrue(events.Any());

            events = _service.SearchEvents("bla-bla");

            Assert.IsTrue(!events.Any());
        }

        [TestMethod]
        public void SearchByType()
        {
            var dateTimeNow = DateTime.UtcNow;
            _testsEventService.AddEvent("Event 1", new EventDateTime() { Start = dateTimeNow, Finish = dateTimeNow.AddDays(1) }, new Address() { City = "Yekaterinburg" }, EEventType.Concert,
                _testsCaterogyService.AddCategory("Category 1"), 0);

            var events = _service.SearchEvents(types: new List<EEventType>() { EEventType.Concert });

            Assert.IsTrue(events.Any());

            events = _service.SearchEvents(types: new List<EEventType>() { EEventType.Excursion });

            Assert.IsTrue(!events.Any());
        }

        [TestMethod]
        public void SearchByCity()
        {
            var dateTimeNow = DateTime.UtcNow;
            _testsEventService.AddEvent("Event 1", new EventDateTime() { Start = dateTimeNow, Finish = dateTimeNow.AddDays(1) }, new Address() { City = "Yekaterinburg" }, EEventType.Concert,
                _testsCaterogyService.AddCategory("Category 1"), 0);

            var events = _service.SearchEvents(city: "Yekaterinburg");

            Assert.IsTrue(events.Any());

            events = _service.SearchEvents(city: "Moscow");

            Assert.IsTrue(!events.Any());
        }

        [TestMethod]
        public void SearchByCategory()
        {
            var dateTimeNow = DateTime.UtcNow;
            var categoryId = _testsCaterogyService.AddCategory("Category 1");
            _testsEventService.AddEvent("Event 1", new EventDateTime() { Start = dateTimeNow, Finish = dateTimeNow.AddDays(1) }, new Address() { City = "Yekaterinburg" }, EEventType.Concert,
                categoryId, 0);

            var events = _service.SearchEvents(categories: new List<string>() { categoryId });

            Assert.IsTrue(events.Any());

            events = _service.SearchEvents(categories: new List<string>() { "bla-bla" });

            Assert.IsTrue(!events.Any());
        }

        //зачем разделили на рубли/копейки? и как теперь искать?
        [TestMethod]
        public void SearchByMaxPrice()
        {
            var dateTimeNow = DateTime.UtcNow;
            var categoryId = _testsCaterogyService.AddCategory("Category 1");
            _testsEventService.AddEvent("Event 1", new EventDateTime() { Start = dateTimeNow, Finish = dateTimeNow.AddDays(1) }, new Address() { City = "Yekaterinburg" }, EEventType.Concert,
                categoryId, 0);

            var events = _service.SearchEvents(maxPrice: 5);

            Assert.IsTrue(events.Any());

            events = _service.SearchEvents(maxPrice: -2);

            Assert.IsTrue(!events.Any());
        }

        [TestMethod]
        public void GetAllCities()
        {
            var dateTimeNow = DateTime.UtcNow;
            var categoryId = _testsCaterogyService.AddCategory("Category 1");
            _testsEventService.AddEvent("Event 1", new EventDateTime() { Start = dateTimeNow, Finish = dateTimeNow.AddDays(1) }, new Address() { City = "Yekaterinburg" }, EEventType.Concert,
                categoryId, 0);
            _testsEventService.AddEvent("Event 2", new EventDateTime() { Start = dateTimeNow, Finish = dateTimeNow.AddDays(1) }, new Address() { City = "Moscow" }, EEventType.Concert,
                categoryId, 0);

            var cities = _service.GetAllCities().Where(c=>!string.IsNullOrEmpty(c)).ToList();

            Assert.AreEqual(cities.Count, 2);

            Assert.IsTrue(cities.Any(c=>c.Equals("Yekaterinburg", StringComparison.OrdinalIgnoreCase)));
        }
    }
}
