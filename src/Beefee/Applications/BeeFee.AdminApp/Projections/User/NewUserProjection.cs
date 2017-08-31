using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
using Core.ElasticSearch.Domain;
using Microsoft.AspNetCore.WebUtilities;

namespace BeeFee.AdminApp.Projections.User
{
	internal class NewUserProjection : BaseNewEntity, IProjection<Model.Models.User>
	{
		public string Email { get; }
		public string Name { get; }
		public string Password { get; }
		public string Salt { get; }
		public EUserRole[] Roles { get; set; }

		public NewUserProjection() { } //Hack

		public NewUserProjection(string email, string name, string password)
		{
			Email = email.ToLowerInvariant();
			Name = name.Trim();
			var salt = HashPasswordHelper.GenerateSalt();
			Salt = Base64UrlTextEncoder.Encode(salt);
			Password = HashPasswordHelper.GetHash(password, salt);
		}
	}
}