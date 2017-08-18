using System;
using Expo3.ClientApp.Services;
using Expo3.Model;
using Expo3.Model.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToDo.Dal.Services;

namespace Expo3.ClientApp.Tests
{
    [TestClass]
    public class EventTests
    {

	    private EventService _service;

	    [TestInitialize]
	    public void Setup()
	    {
		    var serviceProvider = new ServiceCollection()
			    .AddExpo3Model(new Uri("http://localhost:9200/"))
			    .AddExpo3ClientApp()
			    .AddSingleton(new UserName("1"))
			    .BuildServiceProvider();

		    serviceProvider
			    .UseExpo3Model(true)
			    .UseExpo3ClientApp();

		    _service = serviceProvider.GetService<EventService>();
	    }


		[TestMethod]
        public void AddEvent()
        {
        }
    }
}
