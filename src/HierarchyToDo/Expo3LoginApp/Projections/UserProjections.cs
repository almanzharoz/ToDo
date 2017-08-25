using System;
using Core.ElasticSearch.Domain;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Helpers;
using Expo3.Model.Models;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using SharpFuncExt;

namespace Expo3.LoginApp.Projections
{
    public class UserProjection : BaseEntity, IProjection<User>, IGetProjection, ISearchProjection
    {
        [JsonProperty]
        public string Email { get; private set; }
        [JsonProperty]
        public string Name { get; private set; }
        [JsonProperty]
        private string Password { get; set; }
        [JsonProperty]
        private string Salt { get; set; }
        [JsonProperty]
        public EUserRole[] Roles { get; private set; }

	    internal bool CheckPassword(string password)
		    => HashPasswordHelper.GetHash(password, Base64UrlTextEncoder.Decode(Salt)) == Password;
    }

	internal class RegisterUserProjection : BaseNewEntity, IProjection<User>
	{
		public string Email { get; }
		public string Name { get; }
		public string Password { get; }
		public string Salt { get; }
		public EUserRole[] Roles { get; }

		public RegisterUserProjection() { }
		public RegisterUserProjection(string email, string name, string password, EUserRole[] roles)
		{
			Roles = roles.HasNotNullArg(nameof(roles));
			Email = email.ToLowerInvariant();
			Name = name.Trim();
			var salt = HashPasswordHelper.GenerateSalt();
			Salt = Base64UrlTextEncoder.Encode(salt);
			Password = HashPasswordHelper.GetHash(password, salt);
		}
	}

	internal class UpdatePasswordProjection : BaseEntity, IProjection<User>, IUpdateProjection, IGetProjection
	{
		[JsonProperty]
		public string Password { get; private set; }
		[JsonProperty]
		public string Salt { get; private set; }
		[JsonProperty]
		public string Name { get; private set; }
		[JsonProperty]
		public EUserRole[] Roles { get; private set; }

		internal UpdatePasswordProjection ChangePassword(/*string oldPassword, */string newPassword)
		{
			//if (HashPasswordHelper.GetHash(oldPassword, Base64UrlTextEncoder.Decode(Salt)) != Password)
			//	throw new Exception("OldPassword");
			Password = HashPasswordHelper.GetHash(newPassword, Base64UrlTextEncoder.Decode(Salt));
			return this;
		}

	}
}