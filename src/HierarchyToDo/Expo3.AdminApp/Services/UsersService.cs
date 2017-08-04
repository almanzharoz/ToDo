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

namespace Expo3.AdminApp.Services
{
	public class UsersService : BaseExpo3Service
	{
		public UsersService(ILoggerFactory loggerFactory, Expo3ElasticConnection settings,
			ElasticScopeFactory<Expo3ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory,
			user)
		{
		}

		///<exception cref="EntityAlreadyExistsException"></exception>
		public bool AddUser(string email, string password, string nickname, EUserRole[] roles, bool refresh = false)
		{
			if (FilterCount<UserInsertProjection>(q => q.Term(x => x.Email.ToLowerInvariant(), email.ToLowerInvariant())) != 0)
				throw new EntityAlreadyExistsException();

			var salt = HashPasswordHelper.GenerateSalt();
			var hashedPassword = HashPasswordHelper.GetHash(password, salt);
			password = null;

			return Insert(new UserInsertProjection
			{
				Email = email,
				Nickname = nickname,
				Password = hashedPassword,
				Salt = Base64UrlTextEncoder.Encode(salt),
				Roles = roles
			}, refresh);
		}

		public bool EditUser(string id, string email, string password, string nickname, EUserRole[] roles)
		=> Update(Get<UserUpdateProjection>(id), u =>
			{
				u.Email = email;
				u.Nickname = nickname;
				u.Password = HashPasswordHelper.GetHash(password, Base64UrlTextEncoder.Decode(u.Salt));
				u.Roles = roles;
				return u;
			});

		public UserUpdateProjection GetUser(string id)
			=> Get<UserUpdateProjection>(id);

		public IReadOnlyCollection<UserSearchProjection> SearchUserByEmail(string query)
			//=> Search<User, UserSearchProjection>(q => q
			//	.Match(m => m
			//		.Field(x => x.Email)
			//		.Query(query)));
			=> Search<User, UserSearchProjection>(q => q
				.Match(m => m
					.Query(query)));

		public bool DeleteUser(string id)
			=> Remove(Get<UserRemoveProjection>(id));
	}
}