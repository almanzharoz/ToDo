using Core.ElasticSearch;
using Core.ElasticSearch.Domain;
using Expo3.Model;
using Expo3.Model.Interfaces;
using Expo3.Model.Models;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace Expo3.Model
{
	public abstract class BaseExpo3Service : BaseService<Expo3ElasticConnection>
	{
		protected UserName User;

		protected BaseExpo3Service(ILoggerFactory loggerFactory, Expo3ElasticConnection settings,
			ElasticScopeFactory<Expo3ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory)
		{
			User = user;
		}

		protected QueryContainer UserQuery<T>(QueryContainer query) where T : class, IWithOwner, ISearchProjection
			=> Query<T>.Term(p => p.Owner, User.HasNotNullArg(x => x.Id, "user").Id) && query;

		public UserName UseUserName(string userId)
			=> User = new UserName(userId);
	}
}