using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;
using ToDo.Dal.Models;
using ToDo.Dal.Projections;
using ToDo.Dal.Services.Internal;

namespace ToDo.Dal.Services
{
	public class ProjectService : BaseToDoService
	{
		private readonly UsersService _usersService;

		public ProjectService(ILoggerFactory loggerFactory, ElasticConnection settings,
			ElasticScopeFactory<ElasticConnection> factory, UserName user)
			: base(loggerFactory, settings, factory, user)
		{
			_usersService = factory.GetInternalService<UsersService>();
		}

		public IReadOnlyCollection<Project> GetMyProjects() =>
			Filter<Project, Project>(q => UserQuery<Project>(null));

		public IReadOnlyCollection<KeyValuePair<Project, int>> GetMyProjectsWithCount() =>
			SearchWithScore<Project, Project>(
				q => Query<Project>.HasChild<Models.Task>(c => c.ScoreMode(ChildScoreMode.Sum).Query(cq => cq.MatchAll())) ||
					q.Bool(b => b.Filter(UserQuery<Project>(null))));

		public Project GetProject(string id) =>
			Search<Project, Project>(q => UserQuery<Project>(q.Ids(x => x.Values(id))))
				.FirstOrDefault();

		public bool SaveProject(string id, Func<Project, Project> update) =>
			id.IfNull(
				() => Insert(update(new Project() {User = _user})),
				a => Update(update(GetProject(a).HasNotNullArg("project"))
					.ThrowIf(x => x.Id != id, x => new Exception("Not equals ids"))));

		public bool Delete(string id, int version) =>
			Remove<Project>(
				GetProject(id)
					.HasNotNullArg("project")
					.ThrowIf(x => x.Version != version, x => new Exception("version not equals")));

		public bool AddUser(string id, string userId) =>
			Update<Project>(q => UserQuery<Project>(q.Ids(x => x.Values(id.HasNotNullArg("project")))),
				u => u.Add(p => p.Users, userId.HasNotNullArg("user"))) > 0;

		public bool DeleteUser(string id, string userId) =>
			Update<Project>(q => UserQuery<Project>(q.Ids(x => x.Values(id.HasNotNullArg("project")))),
				u => u.Remove(p => p.Users, userId.HasNotNullArg("user"))) > 0;

		public IEnumerable<Projections.User> GetUsersNames(string id, string s) =>
			Search<Models.User, Projections.User>(q => q.Bool(b => b
					.Must(Query<Models.User>.Match(m => m.Field(p => p.Nick).Query(s)))
					.Filter(GetProject(id.HasNotNullArg("project"))
						.Users.IfNotNull(u => !Query<Models.User>.Ids(f => f.Values(u.Select(x => x.Id))),
							() => new QueryContainer()))),
				sort => sort.Ascending(p => p.Nick), 0, 10);


		public bool ProjectExists(string projectId, string name)
			=> FilterCount<Project>(q => !q.Ids(x => x.Values(projectId)) && q.Term(x => x.Field(f => f.Name).Value(name)))>0;
		public bool ProjectExists(string name)
			=> FilterCount<Project>(q => q.Term(x => x.Name, name)) > 0;
	}
}