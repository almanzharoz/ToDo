using Core.ElasticSearch;
using Expo3.AdminApp.Projections;
using Microsoft.Extensions.Logging;

namespace Expo3.AdminApp.Services
{
    internal class UsersService : BaseService
    {
        public UsersService(ILoggerFactory loggerFactory, ElasticConnection settings,
            ElasticScopeFactory<ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory,
            user)
        {
        }
    }
}