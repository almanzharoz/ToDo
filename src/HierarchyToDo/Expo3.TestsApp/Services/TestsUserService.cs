using Core.ElasticSearch;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Exceptions;
using Expo3.Model.Models;
using Expo3.TestsApp.Projections;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace Expo3.TestsApp.Services
{
	public class TestsUserService : BaseExpo3Service
	{
		public TestsUserService(ILoggerFactory loggerFactory, Expo3ElasticConnection settings,
			ElasticScopeFactory<Expo3ElasticConnection> factory) : base(loggerFactory, settings, factory,
			null)
		{
		}

		public string AddUser(string email, string password, string name, EUserRole[] roles)
			=> new NewUser(email, name, password) {Roles = roles}.Fluent(x => Insert(x, true)).Id;
	}
}