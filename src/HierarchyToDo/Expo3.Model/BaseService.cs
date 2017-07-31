using Core.ElasticSearch;
using Core.ElasticSearch.Domain;
using Expo3.Model;
using Expo3.Model.Interfaces;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace Expo3.Model
{
    public abstract class BaseService : BaseService<Expo3ElasticConnection>
    {
        protected readonly UserName User;

        protected BaseService(ILoggerFactory loggerFactory, Expo3ElasticConnection settings,
            ElasticScopeFactory<Expo3ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory)
        {
            User = user;
        }

        protected QueryContainer UserQuery<T>(QueryContainer query) where T : class, IWithOwner, ISearchProjection
            => Query<T>.Term(p => p.Owner, User.HasNotNullArg(x => x.Id, "user").Id) && query;
    }
}