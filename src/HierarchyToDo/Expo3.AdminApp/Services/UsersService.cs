using System.Security.Cryptography.X509Certificates;
using Core.ElasticSearch;
using Expo3.Model;
using Microsoft.Extensions.Logging;

namespace Expo3.AdminApp.Services
{
	internal class UsersService : BaseExpo3Service
	{
		public UsersService(ILoggerFactory loggerFactory, Expo3ElasticConnection settings,
			ElasticScopeFactory<Expo3ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory,
			user)
		{
		}
	}
}