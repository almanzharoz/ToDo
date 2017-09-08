using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BeeFee.ClientApp.Services;
using BeeFee.Model.Embed;
using BeeFee.TestsApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.ClientApp.Tests
{
    [TestClass]
    public class EventServiceTests : BaseTestClass<EventService>
    {
        public EventServiceTests()
            : base(BuilderExtensions.AddBeefeeClientApp, BuilderExtensions.UseBeefeeClientApp, new[] { EUserRole.User })
        {
        }

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
        }

        [TestMethod]
        public void GetEventPage()
        {
            var dateTimeNow = DateTime.UtcNow;
            var eventId = AddEvent(AddCategory("Category 1"), "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)));

            Assert.IsNotNull(eventId);

			var @event = Service.GetEventByUrl("event-1").Result;

			Assert.AreEqual(@event.Name, "Event 1");
            Assert.AreEqual(@event.Category.Name, "Category 1");
        }

        [TestMethod]
        public void SearchByDate()
        {
            var dateTimeNow = DateTime.UtcNow;
            AddEvent(AddCategory("Category 1"), "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), new Address("Yekaterinburg", ""), EEventType.Concert, 0);

            var events = Service.SearchEvents(startDateTime: dateTimeNow.AddDays(-1), endDateTime: dateTimeNow.AddDays(1));

            Assert.IsTrue(events.Any());

            events = Service.SearchEvents(startDateTime: dateTimeNow.AddDays(1), endDateTime: dateTimeNow.AddDays(2));

            Assert.IsTrue(!events.Any());

            events = Service.SearchEvents(startDateTime: dateTimeNow.AddDays(-3), endDateTime: dateTimeNow.AddDays(-2));

            Assert.IsTrue(!events.Any());
        }

        [TestMethod]
        public void SearchByText()
        {
            var dateTimeNow = DateTime.UtcNow;
            AddEvent(AddCategory("Category 1"), "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), new Address("Yekaterinburg", ""), EEventType.Concert, 0);

            var events = Service.SearchEvents("Event 1");

            Assert.IsTrue(events.Any());

            events = Service.SearchEvents("bla-bla");

            Assert.IsTrue(!events.Any());
        }

        [TestMethod]
        public void SearchByType()
        {
            var dateTimeNow = DateTime.UtcNow;
            AddEvent(AddCategory("Category 1"), "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), new Address("Yekaterinburg", ""), EEventType.Concert, 0);

            var events = Service.SearchEvents(types: new List<EEventType>() { EEventType.Concert });

            Assert.IsTrue(events.Any());

            events = Service.SearchEvents(types: new List<EEventType>() { EEventType.Excursion });

            Assert.IsTrue(!events.Any());
        }

        [TestMethod]
        public void SearchByCity()
        {
            var dateTimeNow = DateTime.UtcNow;
            AddEvent(AddCategory("Category 1"), "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), new Address("Yekaterinburg", ""), EEventType.Concert, 0);

            var events = Service.SearchEvents(city: "Yekaterinburg");

            Assert.IsTrue(events.Any());

            events = Service.SearchEvents(city: "Moscow");

            Assert.IsTrue(!events.Any());
        }

        [TestMethod]
        public void SearchByCategory()
        {
            var dateTimeNow = DateTime.UtcNow;
            var categoryId = AddCategory("Category 1");
            AddEvent(categoryId, "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), new Address("Yekaterinburg", ""), EEventType.Concert, 0);

            var events = Service.SearchEvents(categories: new List<string>() { categoryId });

            Assert.IsTrue(events.Any());

            events = Service.SearchEvents(categories: new List<string>() { "bla-bla" });

            Assert.IsTrue(!events.Any());
        }

        [TestMethod]
        public void SearchByMaxPrice()
        {
            var dateTimeNow = DateTime.UtcNow;
            var categoryId = AddCategory("Category 1");
            AddEvent(categoryId, "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), new Address("Yekaterinburg", ""), EEventType.Concert, 1);

            var events = Service.SearchEvents(maxPrice: 5);

            Assert.IsTrue(events.Any());

            events = Service.SearchEvents(maxPrice: -2);

            Assert.IsTrue(!events.Any());
        }

		[TestMethod]
		public void SearchByMaxPriceOrFree()
		{
			var dateTimeNow = DateTime.UtcNow;
			var categoryId = AddCategory("Category 1");
			AddEvent(categoryId, "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), new Address("Yekaterinburg", ""), EEventType.Concert, 0);

			var events = Service.SearchEvents(maxPrice: 5);

			Assert.IsTrue(events.Any());

			events = Service.SearchEvents(maxPrice: -2);

			Assert.IsTrue(events.Any());
		}

		[TestMethod]
        public void GetAllCities()
        {
            var dateTimeNow = DateTime.UtcNow;
            var categoryId = AddCategory("Category 1");
            AddEvent(categoryId, "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), new Address("Yekaterinburg", ""), EEventType.Concert, 0);
            AddEvent(categoryId, "Event 2", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), new Address("Moscow", ""), EEventType.Concert,
                 0);

            var cities = Service.GetAllCities().Where(c => !string.IsNullOrEmpty(c)).ToList();

            Assert.AreEqual(cities.Count, 2);

            Assert.IsTrue(cities.Any(c => c.Equals("Yekaterinburg", StringComparison.OrdinalIgnoreCase)));
        }
    }
}
