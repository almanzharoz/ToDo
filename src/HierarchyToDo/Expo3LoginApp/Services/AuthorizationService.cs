using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Core.ElasticSearch;
using Expo3.LoginApp.Projections;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Exceptions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
				.If(user => user != null && GetHash(password, Base64UrlTextEncoder.Decode(user.Salt)) == user.Password,
					user => new LoginUserProjection(user), user => null);

		/// <exception cref="EntityAlreadyExistsException"></exception>
		public bool Register(string email, string nickname, string password, EUserRole[] roles)
		{
			email = email.ToLower();
			if (FilterCount<UserProjection>(q => q.Term(x => x.Email, email)) > 0)
				throw new EntityAlreadyExistsException();

			var salt = GenerateSalt();

			return Insert(new RegisterUserProjection
			{
				Email = email,
				Nickname = nickname,
				Password = GetHash(password, salt),
				Salt =  Base64UrlTextEncoder.Encode(salt),
				Roles = roles
			});
		}

		/// <summary>
		/// Return base64 of hash password with a salt
		/// </summary>
		/// <param name="password">Password as a string</param>
		/// <param name="salt">Salt as a byte array</param>
		/// <returns></returns>
		private static string GetHash(string password, byte[] salt)
			=> Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password,
				salt,
				KeyDerivationPrf.HMACSHA512,
				10000,
				64));

		private static byte[] GenerateSalt()
		{
			var salt = new byte[128 / 8];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(salt);
			}
			return salt;
		}
	}
}