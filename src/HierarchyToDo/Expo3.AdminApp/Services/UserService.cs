using System.Collections.Generic;
using Core.ElasticSearch;
using Expo3.AdminApp.Projections;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Exceptions;
using Expo3.Model.Helpers;
using Expo3.Model.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace Expo3.AdminApp.Services
{
	public class UserService : BaseExpo3Service
	{
		public UserService(ILoggerFactory loggerFactory, Expo3ElasticConnection settings,
			ElasticScopeFactory<Expo3ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory,
			user)
		{
		}

		///<exception cref="EntityAlreadyExistsException"></exception>
		public string AddUser(string email, string password, string name, EUserRole[] roles)
		{
			if (FilterCount<UserProjection>(q => q.Term(x => x.Email.ToLowerInvariant(), email.ToLowerInvariant())) != 0)
				throw new EntityAlreadyExistsException();

			return new NewUserProjection(email, name, password) { Roles = roles }.Fluent(x => Insert(x, true)).Id;
		}

		public bool EditUser(string id, string email, string name, EUserRole[] roles)
			=> Update<UserUpdateProjection>(id, u => u.ChangeUser(email, name, roles), true);

		public UserProjection GetUser(string id)
			=> Get<UserProjection>(id);

		public IReadOnlyCollection<UserProjection> SearchUsersByEmail(string query)
			=> Search<User, UserProjection>(q => q
				.Wildcard(w => w
					.Field(x => x.Email)
					.Value($"*{query.ToLowerInvariant()}*")));

		public bool DeleteUser(string id) => Remove<UserProjection>(id, true);
	}
}