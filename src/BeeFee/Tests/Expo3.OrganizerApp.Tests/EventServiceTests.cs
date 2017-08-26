using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.OrganizerApp.Services;
using Expo3.TestsApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Expo3.OrganizerApp.Tests
{
    [TestClass]
    public class EventServiceTests : BaseTestClass<EventService>
	{
		public EventServiceTests() 
			: base(BuilderExtensions.AddExpo3OrganizerApp, BuilderExtensions.UseExpo3OrganizerApp, new [] { EUserRole.Organizer })
		{
		}

		[TestInitialize]
		public override void Setup()
			=> base.Setup();
		
	}
}
