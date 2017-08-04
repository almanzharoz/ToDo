using System;
using System.Linq;
using Core.ElasticSearch;
using Expo3.LoginApp.Projections;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Exceptions;
using Expo3.Model.Helpers;
using Expo3.Model.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace Expo3.LoginApp.Services
{
	public class AuthorizationService : BaseExpo3Service
	{
		public AuthorizationService(ILoggerFactory loggerFactory, Expo3ElasticConnection settings,
			ElasticScopeFactory<Expo3ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory,
			user)
		{
		}

		public LoginUserProjection TryLogin(string email, string password)
			=> Filter<User, UserProjection>(q => q.Term(x => x.Email, email), null, 1)
				.FirstOrDefault()
				.If(user => user != null && HashPasswordHelper.GetHash(password, Base64UrlTextEncoder.Decode(user.Salt)) == user.Password,
					user => new LoginUserProjection(user), user => null);

		/// <exception cref="EntityAlreadyExistsException"></exception>
		public bool Register(string email, string nickname, string password, EUserRole[] roles)
		{
			if (FilterCount<UserProjection>(q => q.Term(x => x.Email.ToLowerInvariant(), email.ToLowerInvariant())) > 0)
				throw new EntityAlreadyExistsException();

			var salt = HashPasswordHelper.GenerateSalt();
			var hashedPassword = HashPasswordHelper.GetHash(password, salt);
			password = null;

			return Insert(new RegisterUserProjection
			{
				Email = email,
				Nickname = nickname,
				Password = hashedPassword,
				Salt =  Base64UrlTextEncoder.Encode(salt),
				Roles = roles
			});
		}

		public bool ChangePassword(string email, string oldPassword, string newPassword)
		{
			var user = TryLogin(email, oldPassword);
			oldPassword = null;
			if (user == null) return false;

			var salt = HashPasswordHelper.GenerateSalt();
			var hashedPassword = HashPasswordHelper.GetHash(newPassword, salt);
			newPassword = null;

			var userUpdate = Get<UpdatePasswordProjection>(user.Id);
			userUpdate.Password = hashedPassword;
			userUpdate.Salt = Base64UrlTextEncoder.Encode(salt);

			return Update(userUpdate);
		}
	}
}