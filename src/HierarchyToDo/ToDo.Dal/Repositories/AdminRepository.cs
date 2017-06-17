using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Nest;
using ToDo.Dal.Projections;
using User = ToDo.Dal.Models.User;

namespace ToDo.Dal.Repositories
{
	public class AdminRepository : BaseToDoRepository
	{
		private static readonly MD5 md5 = MD5.Create();

		public AdminRepository(ILoggerFactory loggerFactory, ElasticSettings settings, ElasticMapping<ElasticSettings> mapping, RequestContainer<ElasticSettings> container, UserName user) 
			: base(loggerFactory, settings, mapping, container, user)
		{
		}

		public bool AddUser(string email, string name, string password, string[] roles)
			=> Insert(new User(){Email = email, Nick = name ?? email, Password = Base64UrlTextEncoder.Encode(md5.ComputeHash(Encoding.UTF8.GetBytes(password))), Roles = roles });

		public IReadOnlyCollection<Projections.UserWithRoles> GetUsers() => Search<User, Projections.UserWithRoles>(new QueryContainer());

		public bool DeleteRole(string id, string role)
			=> Update(Query<User>.Ids(x => x.Values(new[] {id})),
					new UpdateByQueryBuilder<User>().Remove(x => x.Roles, role)) > 0;

		public bool AddRole(string id, string role)
			=> Update(Query<User>.Ids(x => x.Values(new[] { id })),
					new UpdateByQueryBuilder<User>().Add(x => x.Roles, role)) > 0;

		public bool DenyUser(string id, bool deny)
			=> Update(Query<User>.Ids(x => x.Values(new[] { id })),
					new UpdateByQueryBuilder<User>().Set(x => x.Deny, deny)) > 0;

		public bool DeleteUser(string id) => Remove<User>(Query<User>.Ids(x => x.Values(new[]{id}))) > 0;

		public ISearchResponse<Projections.User> GetUsersNames(string s)
		{
			return _client.Search<User, Projections.User>(x => x.Query(q => q.Match(m => m.Field(p => p.Nick).Query(s))));
			//return _client.Search<Projections.User>(x => x.Suggest(z => z.Term("my-term-suggest", t => t.Field(p => p.Name).Text(s))));
			//return _client.Search<Projections.User>(x => x.Suggest(z => z.Completion("my-term-suggest", t => t.Field(p => p.Name).Prefix(s))));
		}
	}
}