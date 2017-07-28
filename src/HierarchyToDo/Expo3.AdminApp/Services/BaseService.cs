using Core.ElasticSearch;
using Core.ElasticSearch.Domain;
using Expo3.AdminApp.Projections;
using Expo3.Model.Interfaces;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace Expo3.AdminApp.Services
{
    public abstract class BaseService : BaseService<ElasticConnection>
    {
        protected readonly UserName User;

        protected BaseService(ILoggerFactory loggerFactory, ElasticConnection settings,
            ElasticScopeFactory<ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory)
        {
            User = user;
        }

        protected QueryContainer UserQuery<T>(QueryContainer query) where T : class, IWithUser, ISearchProjection
            => Query<T>.Term(p => p.User, User.HasNotNullArg("user").Id) && query;
    }
}