using System;
using Core.ElasticSearch;
using Expo3.LoginApp.Projections;
using Expo3.LoginApp.Services;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Exceptions;
using Expo3.Model.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Expo3.LoginApp.Tests
{
    [TestClass]
    public class AuthorizationServiceTests
    {
	    private AuthorizationService _service;

        [TestInitialize]
        public void Setup()
        {
	        var serviceProvider = new ServiceCollection()
		        .AddElastic(new Expo3ElasticConnection(new Uri("http://localhost:9200/")))
					.AddService<AuthorizationService, Expo3ElasticConnection>()
					.AddSingleton<ILoggerFactory, LoggerFactory>()
					.AddSingleton(new UserName("1"))
		        .BuildServiceProvider();

	        serviceProvider.UseElasticForTests<Expo3ElasticConnection>(map => map
		        .AddMapping<User>(x => x.UserIndexName)
		        .AddMapping<Event>(x => x.EventIndexName)

		        .AddProjection<LoginUserProjection, User>()
		        .AddProjection<RegisterUserProjection, User>()
		        .AddProjection<UserProjection, User>()
				.AddProjection<BaseUserProjection, User>());

	        _service = serviceProvider.GetService<AuthorizationService>();
        }

	    [TestMethod]
	    public void RegisterAndGetUser()
	    {
		    var registered = _service.Register("test@test", "iam", "123", new[] {EUserRole.User});
		    Assert.AreEqual(true, registered);

		    var user = _service.TryLogin("test@test", "123");
		    Assert.AreEqual("iam", user.Nickname);
			Assert.AreEqual(1, user.Roles.Length);
			Assert.AreEqual(EUserRole.User, user.Roles[0]);
	    }

	    [TestMethod]
	    public void TryToGetNotRegisteredUser()
	    {
		    var user = _service.TryLogin("123", "123");
			Assert.AreEqual(null, user);
	    }

	    [TestMethod]
		[ExpectedException(typeof(EntityAlreadyExistsException))]
	    public void TryToRegisterExistingUser()
	    {
		    var registered = _service.Register("test@test", "123", "123", new[] {EUserRole.User});
		    Assert.AreEqual(true, registered);

		    _service.Register("test@test", "234", "234", new[] {EUserRole.User});
	    }
    }
}
