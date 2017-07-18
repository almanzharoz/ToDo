using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;
using ToDo.Dal.Models;
using ToDo.Dal.Projections;

namespace ToDo.Dal.Repositories
{
	public class ProjectService : BaseToDoService
	{
		public ProjectService(ILoggerFactory loggerFactory, ElasticSettings settings,
			ElasticMapping<ElasticSettings> mapping, RequestContainer<ElasticSettings> container, UserName user)
			: base(loggerFactory, settings, mapping, container, user)
		{
		}

		public IReadOnlyCollection<Project> GetMyProjects() =>
			Search<Project, Project>(UserQuery<Project>(null));

		public IReadOnlyCollection<KeyValuePair<Project, int>> GetMyProjectsWithCount() =>
			_client.Search<Project>(s => s.Query(
					q => Query<Project>.HasChild<Models.Task>(c => c.ScoreMode(ChildScoreMode.Sum).Query(cq => cq.MatchAll())) ||
						q.Bool(b => b.Filter(UserQuery<Project>(null)))))
				.Fluent(Load)
				.Hits.Select(x => new KeyValuePair<Project, int>(x.Source, (int) x.Score))
				.ToArray();

		public Project GetProject(string id) =>
			Search<Project, Project>(UserQuery<Project>(Query<Project>.Ids(x => x.Values(id))))
				.FirstOrDefault();

		public bool SaveProject(string id, Func<Project, Project> update) =>
			id.IfNull(
				() => Insert(update(new Project() {User = _user})),
				a => Update(update(GetProject(a).HasNotNullArg("project"))
					.ThrowIf(x => x.Id != id, x => new Exception("Not equals ids"))));

		public bool Delete(string id, int version) =>
			Remove(
				GetProject(id)
					.HasNotNullArg("project")
					.ThrowIf(x => x.Version != version, x => new Exception("version not equals")));

		public bool AddUser(string id, string userId) =>
			Update(UserQuery<Project>(Query<Project>.Ids(x => x.Values(id.HasNotNullArg("project")))),
				new UpdateByQueryBuilder<Project>().Add(p => p.Users, userId.HasNotNullArg("user"))) > 0;

		public bool DeleteUser(string id, string userId) =>
			Update(UserQuery<Project>(Query<Project>.Ids(x => x.Values(id.HasNotNullArg("project")))),
				new UpdateByQueryBuilder<Project>().Remove(p => p.Users, userId.HasNotNullArg("user"))) > 0;

		public IEnumerable<Projections.User> GetUsersNames(string id, string s) =>
			Search<Models.User, Projections.User>(q => q.Bool(b => b
						.Must(Query<Models.User>.Match(m => m.Field(p => p.Nick).Query(s)))
						.Filter(GetProject(id.HasNotNullArg("project"))
							.Users.IfNotNull(u => !Query<Models.User>.Ids(f => f.Values(u.Select(x => x.Id))),
								() => new QueryContainer()))),
					sort => sort.Ascending(p => p.Nick), 0, 10);
	}
}