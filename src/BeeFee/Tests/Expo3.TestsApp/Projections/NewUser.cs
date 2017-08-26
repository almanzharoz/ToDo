using Core.ElasticSearch.Domain;
using Expo3.Model.Embed;
using Expo3.Model.Helpers;
using Expo3.Model.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace Expo3.TestsApp.Projections
{
	internal class NewUser : BaseNewEntity, IProjection<User>
	{
		public string Email { get; }
		public string Name { get; }
		public string Password { get; }
		public string Salt { get; }
		public EUserRole[] Roles { get; set; }

		public NewUser() { } //Hack

		public NewUser(string email, string name, string password)
		{
			Email = email.ToLowerInvariant();
			Name = name.Trim();
			var salt = HashPasswordHelper.GenerateSalt();
			Salt = Base64UrlTextEncoder.Encode(salt);
			Password = HashPasswordHelper.GetHash(password, salt);
		}
	}
}