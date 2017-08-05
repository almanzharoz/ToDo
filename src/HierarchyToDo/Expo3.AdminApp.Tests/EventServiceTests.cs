using System;
using System.Linq;
using Expo3.AdminApp.Services;
using Expo3.LoginApp;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AuthorizationService = Expo3.LoginApp.Services.AuthorizationService;

namespace Expo3.AdminApp.Tests
{
    [TestClass]
    public class EventServiceTests
    {
	    private AuthorizationService _authService;
	    private EventService _service;

		[TestInitialize]
		public void Setup()
	    {
		    var serviceProvider = new ServiceCollection()
			    .AddExpo3Model(new Uri("http://localhost:9200/"))
			    .AddExpo3LoginApp()
				.AddExpo3AdminApp()
			    .AddSingleton(new UserName("1")) // hack
			    .AddLogging()
			    .BuildServiceProvider();

		    serviceProvider
			    .UseExpo3Model(true)
			    .UseExpo3LoginApp()
				.UseExpo3AdminApp();

		    _authService = serviceProvider.GetService<AuthorizationService>();
			_service = serviceProvider.GetService<EventService>();
		}

		[TestMethod]
        public void AddSearchAndGetEventTest()
        {
	        _authService.Register("test@test", "test", "123", new[] {EUserRole.Admin, EUserRole.Organizer, EUserRole.User});
			var userId = _authService.TryLogin("test@test", "123").Id;

	        _service.UseUserName(userId); // Hack for test
			
	        _service.AddEvent("Test Event", "New", DateTime.MinValue, DateTime.MaxValue,
		        new Address {AddressString = "asd", City = "Ekaterinburg"},
		        EEventType.Concert);

	        var searchedEvent = _service.SearchByName("Test").ToList();
			Assert.AreEqual(1, searchedEvent.Count);
			Assert.AreEqual("Test Event", searchedEvent[0].Name);
			Assert.AreEqual("New", searchedEvent[0].Caption);
			Assert.AreEqual(DateTime.MinValue, searchedEvent[0].StartDateTime);
			Assert.AreEqual(DateTime.MaxValue, searchedEvent[0].FinishDateTime);
			Assert.AreEqual("asd", searchedEvent[0].Address.AddressString);
			Assert.AreEqual("Ekaterinburg", searchedEvent[0].Address.City);
			//Assert.AreEqual(new GeoCoordinate(12, 14), searchedEvent[0].Address.Coordinates);
			Assert.AreEqual(EEventType.Concert, searchedEvent[0].Type);

	        var gotEvent = _service.GetEvent(searchedEvent[0].Id);
	        Assert.AreEqual("Test Event", gotEvent.Name);
	        Assert.AreEqual("New", gotEvent.Caption);
	        Assert.AreEqual(DateTime.MinValue, gotEvent.StartDateTime);
	        Assert.AreEqual(DateTime.MaxValue, gotEvent.FinishDateTime);
	        Assert.AreEqual("asd", gotEvent.Address.AddressString);
	        Assert.AreEqual("Ekaterinburg", gotEvent.Address.City);
	        //Assert.AreEqual(new GeoCoordinate(12, 14), gotEvent.Address.Coordinates);
	        Assert.AreEqual(EEventType.Concert, gotEvent.Type);
		}

	    [TestMethod]
	    public void TestRegisterVisitorsToEvent()
	    {
		    _authService.Register("test@test", "test", "123", new[] {EUserRole.User});
		    var userId = _authService.TryLogin("test@test", "123").Id;

		    _service.UseUserName(userId);

		    _service.AddEvent("Test Event", "New", DateTime.MinValue, DateTime.MaxValue,
			    new Address {AddressString = "asd", City = "Ekaterinburg"},
			    EEventType.Concert, true);

		    var searchedEvent = _service.SearchByName("Test").ToArray();

		    _service.RegisterNewVisitorToEvent(searchedEvent[0].Id, "test@test.test", "123", "Groot");

		    var eventById = _service.GetEvent(searchedEvent[0].Id);
			Assert.AreEqual(1, eventById.Visitors.Length);
			Assert.AreEqual("test@test.test", eventById.Visitors[0].Email);
			Assert.AreEqual("123", eventById.Visitors[0].PhoneNumber);
			Assert.AreEqual("Groot", eventById.Visitors[0].Name);
	    }

	    [TestMethod]
	    public void RemoveTest()
	    {
			_authService.Register("test@test", "test", "123", new[] { EUserRole.User });
		    var userId = _authService.TryLogin("test@test", "123").Id;

		    _service.UseUserName(userId);

		    _service.AddEvent("Test Event", "New", DateTime.MinValue, DateTime.MaxValue,
			    new Address { AddressString = "asd", City = "Ekaterinburg" },
			    EEventType.Concert, true);

		    var searchedEvent = _service.SearchByName("Test").ToArray();
			Assert.AreEqual(1, searchedEvent.Length);

			_service.RemoveEvent(searchedEvent[0].Id, searchedEvent[0].Version);

		    var searched2 = _service.SearchByName("Test").ToArray();
			Assert.AreEqual(0, searched2.Length);
	    }
    }
}

