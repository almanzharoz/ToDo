using System.Collections.Generic;
using System.Linq;
using Core.ElasticSearch;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;
using ToDo.Dal.Models;
using ToDo.Dal.Projections;

namespace ToDo.Dal.Services.Internal
{
	internal class UsersService : BaseToDoService
	{
		public UsersService(ILoggerFactory loggerFactory, ElasticConnection settings,
			ElasticScopeFactory<ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory, user)
		{
		}

		public Project GetProject(string id, bool required = false) =>
			Search<Project, Project>(
					Query<Project>.Ids(x => x.Values(id.HasNotNullArg("projectId"))) && (Query<Project>.Term(p => p.User, _user.HasNotNullArg("user").Id) || Query<Project>.Term(x => x.Users, _user.Id)))
				.FirstOrDefault().If(required, x => x.HasNotNullArg("project"));
	}
}