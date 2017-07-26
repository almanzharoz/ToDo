using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Nest;
using ToDo.Dal.Projections;

namespace ToDo.Dal.Services
{
	public class AuthorizationService : BaseToDoService
	{
		private static readonly MD5 md5 = MD5.Create();

		public AuthorizationService(ILoggerFactory loggerFactory, ElasticConnection settings,
			ElasticScopeFactory<ElasticConnection> factory, UserName user)
			: base(loggerFactory, settings, factory, user)
		{
		}

		public UserWithRoles TryLogin(string email, string password)
			=> Filter<Models.User, UserWithRoles>(q =>
					q.Term(x => x.Email, email) &&
					!q.Term(x => x.Deny, true) &&
					q.Term(x => x.Password, Base64UrlTextEncoder.Encode(md5.ComputeHash(Encoding.UTF8.GetBytes(password)))), null, 1)
				.FirstOrDefault();
	}
}
