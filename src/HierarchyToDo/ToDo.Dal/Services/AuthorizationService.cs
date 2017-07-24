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

namespace ToDo.Dal.Repositories
{
	public class AuthorizationService : BaseToDoService
	{
		private static readonly MD5 md5 = MD5.Create();

		public AuthorizationService(ILoggerFactory loggerFactory, ElasticSettings settings,
			ElasticMapping<ElasticSettings> mapping, RequestContainer<ElasticSettings> container, UserName user)
			: base(loggerFactory, settings, mapping, container, user)
		{
		}

		public UserWithRoles TryLogin(string email, string password)
			=> Search<Models.User, UserWithRoles>(
					Query<Models.User>.Term(x => x.Email, email) &&
					Query<Models.User>.Term(x => x.Deny, false) &&
					Query<Models.User>.Term(x => x.Password, Base64UrlTextEncoder.Encode(md5.ComputeHash(Encoding.UTF8.GetBytes(password)))), null, 1)
				.FirstOrDefault();
	}
}
