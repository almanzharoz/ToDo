using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace ToDo.Dal.Repositories
{
	public class AdminService : BaseToDoService
	{
		private static readonly MD5 md5 = MD5.Create();

		public AdminService(ILoggerFactory loggerFactory, ElasticSettings settings, ElasticMapping<ElasticSettings> mapping, RequestContainer<ElasticSettings> container, Projections.UserName user) 
			: base(loggerFactory, settings, mapping, container, user)
		{
		}

		public bool AddUser(string email, string name, string password, string[] roles)
			=> Count<Models.User>(Query<Models.User>.Term(p => p.Email, email.ToLower()))
				.If(p => p == 0, x =>
					Insert(new Models.User()
					{
						Email = email.ToLower(),
						Nick = name ?? email,
						Password = Base64UrlTextEncoder.Encode(md5.ComputeHash(Encoding.UTF8.GetBytes(password))),
						Roles = roles
					}), x => false);

		public IReadOnlyCollection<Projections.UserWithRoles> GetUsers() => Search<Models.User, Projections.UserWithRoles>(new QueryContainer());

		public bool DeleteRole(string id, string role)
			=> Update(Query<Models.User>.Ids(x => x.Values(id)),
					new UpdateByQueryBuilder<Models.User>().Remove(x => x.Roles, role)) > 0;

		public bool AddRole(string id, string role)
			=> Update(Query<Models.User>.Ids(x => x.Values(id)),
					new UpdateByQueryBuilder<Models.User>().Add(x => x.Roles, role)) > 0;

		public bool DenyUser(string id, bool deny)
			=> Update(Query<Models.User>.Ids(x => x.Values(id)),
					new UpdateByQueryBuilder<Models.User>().Set(x => x.Deny, deny)) > 0;

		public bool DeleteUser(string id) => Remove<Models.User>(Query<Models.User>.Ids(x => x.Values(id))) > 0;

		public ISearchResponse<Projections.User> GetUsersNames(string s) =>
			_client.Search<Models.User, Projections.User>(x => x.Query(q => q.Match(m => m.Field(p => p.Nick).Query(s))));
	}
}