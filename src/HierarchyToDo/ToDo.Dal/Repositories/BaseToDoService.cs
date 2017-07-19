using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Mapping;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;
using ToDo.Dal.Interfaces;
using ToDo.Dal.Models;
using ToDo.Dal.Projections;

namespace ToDo.Dal.Repositories
{
	public abstract class BaseToDoService : BaseService<ElasticSettings>
	{
		protected readonly UserName _user;

		protected BaseToDoService(ILoggerFactory loggerFactory, ElasticSettings settings, ElasticMapping<ElasticSettings> mapping, RequestContainer<ElasticSettings> container, UserName user) 
			: base(loggerFactory, settings, mapping, container)
		{
			_user = user;
		}

		/// <summary>
		/// Контроль доступа по пользователю
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <returns></returns>
		protected QueryContainer UserQuery<T>(QueryContainer query) where T : class, IWithUser
			=> Query<T>.Term(p => p.User, _user.HasNotNullArg("user").Id) && query;

		protected QueryContainer TaskQuery(QueryContainer query)
			=> (Query<Models.Task>.Term(p => p.User, _user.HasNotNullArg("user").Id) ||
			    Query<Models.Task>.HasParent<Project>(
				    x => x.Query(q => q.Bool(b => b.Filter(f => f.Term(p => p.Users, _user.Id))))))
			   && query;

		protected QueryContainer TaskQuery(QueryContainer query, string project)
			=> Query<Models.Task>.HasParent<Project>(
				    x => x.Query(q => q.Bool(b => b.Filter(f => f.Term(p => p.Users, _user.Id) &&
				                                                f.Ids(id => id.Values(project.HasNotNullArg(nameof(project))))))))
			   && query;

		public bool Save<T>(string id, Func<string, T> getEntity, Func<T, T> update)
			where T : class, IEntity, IWithUser, IWithVersion, new()
			=> id.IfNull(
				() => Insert(update(new T() { User = _user })),
				a => Update(update(getEntity(a).HasNotNullArg("entity"))
					.ThrowIf(x => x.Id != id, x => new Exception("Not equals ids"))));
	}
}