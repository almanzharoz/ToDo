using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Core.ElasticSearch;
using Expo3.AdminApp.Projections;
using Expo3.Model;
using Microsoft.AspNetCore.WebUtilities;
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
            using (var sha512 = SHA512.Create())
            {
                var user = Filter<User, UserProjection>(q =>
                    q.Term(x => x.Email, email)).FirstOrDefault();
                var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(password + Base64UrlTextEncoder.Decode(user.Salt)));
                if (hash == Base64UrlTextEncoder.Decode(user.Password))
                    return user;
            }
            return null;
        }
    }
}