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
using ToDo.Dal.Models;

namespace ToDo.Dal.Repositories
{
	public class AdminService : BaseToDoService
	{
		private static readonly MD5 md5 = MD5.Create();

		public AdminService(ILoggerFactory loggerFactory, ElasticSettings settings, ElasticScopeFactory<ElasticSettings> factory, Projections.UserName user) 
			: base(loggerFactory, settings, factory, user)
		{
		}

		public bool AddUser(string email, string name, string password, EUserRole[] roles)
			=> Count<Projections.UserWithRoles>(Query<Projections.UserWithRoles>.Term(p => p.Email, email.ToLower()))
				.If(p => p == 0, x =>
					Insert(new Projections.NewUser()
					{
						Email = email.ToLower(),
						Nick = name ?? email,
						Password = Base64UrlTextEncoder.Encode(md5.ComputeHash(Encoding.UTF8.GetBytes(password))),
						Roles = roles
					}), x => false);

		public IReadOnlyCollection<Projections.UserWithRoles> GetUsers() => Search<Models.User, Projections.UserWithRoles>(new QueryContainer());

		public bool DeleteRole(string id, EUserRole role)
			=> Update(Query<Projections.UserWithRoles>.Ids(x => x.Values(id)),
					new UpdateByQueryBuilder<Projections.UserWithRoles>().Remove(x => x.Roles, role)) > 0;

		public bool AddRole(string id, EUserRole role)
			=> Update(Query<Projections.UserWithRoles>.Ids(x => x.Values(id)),
					new UpdateByQueryBuilder<Projections.UserWithRoles>().Add(x => x.Roles, role)) > 0;

		public bool DenyUser(string id, bool deny)
			=> Update(Query<Projections.UserWithRoles>.Ids(x => x.Values(id)),
					new UpdateByQueryBuilder<Projections.UserWithRoles>().Set(x => x.Deny, deny)) > 0;

		public bool DeleteUser(string id) => Remove<Projections.User>(GetUser(id));
		public Projections.UserWithRoles GetUser(string id) => Get<Projections.UserWithRoles>(id.HasNotNullArg("userId"));

		public IReadOnlyCollection<Projections.User> GetUsersNames(string s) =>
			Search<Models.User, Projections.User>(q => q.Match(m => m.Field(p => p.Nick).Query(s)));
	}
}