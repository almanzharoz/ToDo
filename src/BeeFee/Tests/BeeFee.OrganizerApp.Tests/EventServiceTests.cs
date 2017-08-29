using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.OrganizerApp.Services;
using BeeFee.TestsApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.OrganizerApp.Tests
{
    [TestClass]
    public class EventServiceTests : BaseTestClass<EventService>
	{
		public EventServiceTests() 
			: base(BuilderExtensions.AddBeefeeOrganizerApp, BuilderExtensions.UseBeefeeOrganizerApp, new [] { EUserRole.Organizer })
		{
		}

		[TestInitialize]
		public override void Setup()
			=> base.Setup();
		
	}
}
