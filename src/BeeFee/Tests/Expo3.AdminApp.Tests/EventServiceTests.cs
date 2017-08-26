using System;
using System.Linq;
using Expo3.AdminApp.Services;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Expo3.TestsApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Expo3.AdminApp.Tests
{
	[TestClass]
	public class EventServiceTests : BaseTestClass<EventService>
	{
		public EventServiceTests()
			: base(BuilderExtensions.AddExpo3AdminApp, BuilderExtensions.UseExpo3AdminApp, new[] { EUserRole.Admin })
		{
		}

		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void RemoveEventTestTest()
		{
			var eventId = AddEvent(AddCategory("Category 1"), "Event 1");

			Assert.IsNotNull(eventId);

			Assert.IsTrue(Service.RemoveEvent(eventId, 1));
		}

		[TestMethod]
		public void SetCategoryTest()
		{
			var eventId = AddEvent(AddCategory("Category 1"), "Event 1");
			Assert.IsNotNull(eventId);
			var categoryId = AddCategory("Category 2");
			Assert.IsNotNull(categoryId);

			Assert.IsTrue(Service.SetCategoryToEvent(eventId, categoryId, 1));
		}

		[TestMethod]
		public void SerachEvent()
		{
			var eventId = AddEvent(AddCategory("Category 1"), "Event 1");
			Assert.IsNotNull(eventId);

			var searched = Service.SearchByName("Event 1").SingleOrDefault();

			Assert.IsNotNull(searched);
			Assert.AreEqual(searched.Id, eventId);
		}

	}
}

