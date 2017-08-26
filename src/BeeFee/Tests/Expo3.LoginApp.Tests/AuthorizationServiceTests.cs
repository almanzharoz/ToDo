using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Expo3.LoginApp.Services;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Exceptions;
using Expo3.TestsApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Expo3.LoginApp.Tests
{
    [TestClass]
    public class AuthorizationServiceTests : BaseTestClass<AuthorizationService>
    {
		public AuthorizationServiceTests() 
			: base(BuilderExtensions.AddExpo3LoginApp, BuilderExtensions.UseExpo3LoginApp, new [] {EUserRole.User})
		{
		}

	    [TestInitialize]
	    public override void Setup() => base.Setup();

	    [TestMethod]
	    public void RegisterAndGetUser()
	    {
			Assert.AreEqual(Service.Register("test@test.ru", "iam", "123"), UserRegistrationResult.Ok);

		    var user = Service.TryLogin("test@test.ru", "123");
		    Assert.AreEqual("iam", user.Name);
	    }

	    [TestMethod]
	    public void TryToGetNotRegisteredUser()
	    {
		   Assert.IsNull(Service.TryLogin("123", "123"));
	    }

	    [TestMethod]
	    public void TryToRegisterExistingUser()
	    {
			Assert.AreEqual(Service.Register("test@test.ru", "iam", "123"), UserRegistrationResult.Ok);

		    Assert.AreEqual(Service.Register("test@test.ru", "iam2", "1234"), UserRegistrationResult.EmailAlreadyExists);
	    }

	    [TestMethod]
	    public void RegisterUserWrongEmail()
	    {
		    Assert.AreEqual(Service.Register("test@test", "iam", "123"), UserRegistrationResult.WrongEmail);
	    }
    }
}
