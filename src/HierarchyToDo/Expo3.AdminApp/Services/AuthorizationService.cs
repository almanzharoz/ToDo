using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Core.ElasticSearch;
using Expo3.AdminApp.Projections;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Exceptions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;

namespace Expo3.AdminApp.Services
{
    public class AuthorizationService : BaseService
    {
        public AuthorizationService(ILoggerFactory loggerFactory, ElasticConnection settings,
            ElasticScopeFactory<ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory,
            user)
        {
        }

        public UserProjection TryLogin(string email, string password)
        {
            var user = Filter<User, UserProjection>(q =>
                q.Term(x => x.Email, email)).FirstOrDefault();
            var hash = GetHash(password, Encoding.UTF8.GetBytes(user.Salt));
            return hash == user.Password ? user : null;
        }

        /// <exception cref="EntityAlreadyExistsException"></exception>
        public User Register(string email, string nickname, string password)
        {
            if(Filter<User, UserProjection>(q => q.Term(x => x.Email, email)).FirstOrDefault() != null) throw new EntityAlreadyExistsException();

            var user = new User
            {
                Email = email,
                Nickname = nickname,
                Password = GetHash(password, GenerateSalt())
            };
            Insert(user);

            return user;
        }

        /// <summary>
        /// Return base64 of hash password with a salt
        /// </summary>
        /// <param name="password">Password as a string</param>
        /// <param name="salt">Salt as a byte array</param>
        /// <returns></returns>
        private static string GetHash(string password, byte[] salt)
        {
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA512,
                10000,
                64));
            return hashed;
        }

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