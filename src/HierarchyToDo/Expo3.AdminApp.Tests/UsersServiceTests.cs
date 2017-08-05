using System;
using System.Linq;
using Expo3.Model;
using Expo3.AdminApp.Services;
using Expo3.LoginApp;
using Expo3.Model.Embed;
using Expo3.Model.Models;
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
				.AddExpo3LoginApp()
				.AddExpo3AdminApp()
				.AddSingleton(new UserName("1"))
				.AddLogging()
				.BuildServiceProvider();

			serviceProvider
				.UseExpo3Model(true)
				.UseExpo3LoginApp()
				.UseExpo3AdminApp();

			_service = serviceProvider.GetService<UsersService>();
		}

		[TestMethod]
		public void AddUserTest()
		{
			var added = _service.AddUser("test@test", "123", "123", new[] {EUserRole.Admin, EUserRole.Organizer}, true);
			Assert.AreEqual(true, added);

			var user = _service.SearchUsersByEmail("test").ToList();
			Assert.AreEqual(1, user.Count);
			Assert.AreEqual("test@test", user[0].Email);
			Assert.AreEqual("123", user[0].Nickname);
			Assert.AreEqual(2, user[0].Roles.Length);

			var userById = _service.GetUser(user[0].Id);
			Assert.AreEqual("test@test", userById.Email);
			Assert.AreEqual("123", userById.Nickname);
			Assert.AreEqual(2, userById.Roles.Length);
		}

		[TestMethod]
		public void DeleteUserTest()
		{
			var added = _service.AddUser("test@test", "123", "123", new[] {EUserRole.Admin, EUserRole.User}, true);
			Assert.AreEqual(true, added);

			var userId = _service.SearchUsersByEmail("test").ToList()[0].Id;

			var userById = _service.GetUser(userId);
			Assert.IsNotNull(userById);

			var deleted = _service.DeleteUser(userId);
			Assert.AreEqual(true, deleted);

			var userById2 = _service.GetUser(userId);
			Assert.IsNull(userById2);
		}

		[TestMethod]
		public void SearchUserTest()
		{
			var added = _service.AddUser("TeST@mail.RU", "132", "123", new[] {EUserRole.User}, true);
			Assert.AreEqual(true, added);

			var searched1 = _service.SearchUsersByEmail("test");
			Assert.AreEqual(1, searched1.Count);

			var searched2 = _service.SearchUsersByEmail("TEST@MAIL.RU");
			Assert.AreEqual(1, searched2.Count);

			var searched3 = _service.SearchUsersByEmail("@");
			Assert.AreEqual(1, searched3.Count);

			var searched4 = _service.SearchUsersByEmail("132");
			Assert.AreEqual(0, searched4.Count);
		}

		[TestMethod]
		public void EditUserText()
		{
			var added = _service.AddUser("test@test.test", "", "", new[] {EUserRole.User}, true);
			Assert.AreEqual(true, added);

			var searched = _service.SearchUsersByEmail("test").ToArray();
			Assert.AreEqual(1, searched.Length);

			var edited = _service.EditUser(searched[0].Id, searched[0].Email, "", "123", searched[0].Roles);
			Assert.AreEqual(true, edited);

			var searched2 = _service.SearchUsersByEmail("test").ToArray();
			Assert.AreEqual(1, searched2.Length);

			Assert.AreEqual(searched[0].Id, searched2[0].Id);
			Assert.AreEqual("test@test.test", searched2[0].Email);
			Assert.AreEqual(searched[0].Email, searched2[0].Email);
			Assert.AreEqual("123", searched2[0].Nickname);
			Assert.AreEqual(1, searched2[0].Roles.Length);
			Assert.AreEqual(EUserRole.User, searched2[0].Roles[0]);
		}
	}
}