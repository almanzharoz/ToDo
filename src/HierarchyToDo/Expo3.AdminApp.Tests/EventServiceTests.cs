using System;
using System.Linq;
using Core.ElasticSearch;
using Expo3.AdminApp.Projections;
using Expo3.AdminApp.Services;
using Expo3.LoginApp.Services;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;

namespace Expo3.AdminApp.Tests
{
    [TestClass]
    public class EventServiceTests
    {
	    private IServiceProvider _serviceProvider;

		[TestInitialize]
		public void Setup()
		{
			_serviceProvider = new ServiceCollection()
				.AddExpo3Model(new Uri("http://localhost:9200/"))
				.AddExpo3AdminApp(new Uri("http://localhost:9200/"))
				.AddLogging()
				.BuildServiceProvider();

			_serviceProvider
				.UseExpo3Model()
				.UseExpo3AdminApp();
		}

        [TestMethod]
        public void AddSearchAndGetEventTest()
        {
	        var authorizationService = new AuthorizationService(_serviceProvider.GetService<ILoggerFactory>(),
		        _serviceProvider.GetService<Expo3ElasticConnection>(),
		        _serviceProvider.GetService<ElasticScopeFactory<Expo3ElasticConnection>>(), null);
			authorizationService.Register("test@test", "test", "123", new[] {EUserRole.Admin, EUserRole.Organizer, EUserRole.User});
			var userId = authorizationService.TryLogin("test@test", "123").Id;
			

			var service = new EventService(_serviceProvider.GetService<ILoggerFactory>(), _serviceProvider.GetService<Expo3ElasticConnection>(), 
				_serviceProvider.GetService<ElasticScopeFactory<Expo3ElasticConnection>>(), new UserName(userId));

	        service.AddEvent("TestEvent", "New", DateTime.MinValue, DateTime.MaxValue,
		        new Address {AddressString = "asd", City = "Ekaterinburg", Coordinates = new GeoCoordinate(12, 14)},
		        EEventType.Concert);

	        var searchedEvent = service.SearchByName("Test").ToList();
			Assert.AreEqual(1, searchedEvent.Count);
			Assert.AreEqual("TestEvent", searchedEvent[0].Name);
			Assert.AreEqual("New", searchedEvent[0].Caption);
			Assert.AreEqual(DateTime.MinValue, searchedEvent[0].StartDateTime);
			Assert.AreEqual(DateTime.MaxValue, searchedEvent[0].FinishDateTime);
			Assert.AreEqual("asd", searchedEvent[0].Address.AddressString);
			Assert.AreEqual("Ekaterinburg", searchedEvent[0].Address.City);
			Assert.AreEqual(new GeoCoordinate(12, 14), searchedEvent[0].Address.Coordinates);
			Assert.AreEqual(EEventType.Concert, searchedEvent[0].Type);

	        var gotEvent = service.GetEvent(searchedEvent[0].Id);
	        Assert.AreEqual("TestEvent", gotEvent.Name);
	        Assert.AreEqual("New", gotEvent.Caption);
	        Assert.AreEqual(DateTime.MinValue, gotEvent.StartDateTime);
	        Assert.AreEqual(DateTime.MaxValue, gotEvent.FinishDateTime);
	        Assert.AreEqual("asd", gotEvent.Address.AddressString);
	        Assert.AreEqual("Ekaterinburg", gotEvent.Address.City);
	        Assert.AreEqual(new GeoCoordinate(12, 14), gotEvent.Address.Coordinates);
	        Assert.AreEqual(EEventType.Concert, gotEvent.Type);
		}
    }
}

