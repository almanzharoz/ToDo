using System;
using System.Linq;
using Expo3.Model;
using Expo3.AdminApp.Services;
using Expo3.Model.Embed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Expo3.AdminApp.Tests
{
	[TestClass]
	public class UsersServiceTests
	{
		private UsersService _service;
		[TestInitialize]
		public void Setup()
		{
			var serviceProvider = new ServiceCollection()
				.AddExpo3Model(new Uri("http://localhost:9200/"))
				.AddExpo3AdminApp(new Uri("http://localhost:9200/"))
				.AddLogging()
				.BuildServiceProvider();

			serviceProvider
				.UseExpo3Model()
				.UseExpo3AdminApp();

			_service = serviceProvider.GetService<UsersService>();
		}

		[TestMethod]
		public void AddUserTest()
		{
			var added = _service.AddUser("test@test", "123", "123", new[] {EUserRole.Admin, EUserRole.Organizer});
			Assert.AreEqual(true, added);

			var user = _service.SearchUserByName("test").ToList();
			Assert.AreEqual(1, user.Count);
			Assert.AreEqual("test@test", user[0].Email);
			Assert.AreEqual("123", user[0].Nickname);
			Assert.AreEqual(2, user[0].Roles.Length);

			var userById = _service.GetUser(user[0].Id);
			Assert.AreEqual("test@test", userById.Email);
			Assert.AreEqual("123", userById.Nickname);
			Assert.AreEqual(2, userById.Roles.Length);
		}
	}
}