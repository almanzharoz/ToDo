using System;
using Core.ElasticSearch.Domain;
using Expo3.Model.Embed;
using Expo3.Model.Helpers;
using Expo3.Model.Models;
using Microsoft.AspNetCore.WebUtilities;
using Nest;
using Newtonsoft.Json;
using SharpFuncExt;

namespace Expo3.AdminApp.Projections
{
	//TODO: Обратите внимание на модификаторы доступа

	internal class NewUserProjection : BaseNewEntity, IProjection<User>
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

	public class UserUpdateProjection : BaseEntity, IProjection<User>, IGetProjection, IUpdateProjection
	{
		[JsonProperty]
		public string Email { get; private set; }
		[JsonProperty]
		public string Name { get; private set; }
		[JsonProperty]
		public string Password { get; private set; }
		[JsonProperty]
		public EUserRole[] Roles { get; private set; }
		[JsonProperty]
		public string Salt { get; private set; }

		/// <summary>
		/// Метод не для админки. Добавил как пример использования. Надо его в LoginApp добавить.
		/// </summary>
		internal UserUpdateProjection ChangePassword(string oldPassword, string newPassword)
		{
			if (oldPassword != Password)
				throw new Exception("OldPassword");
			Password = HashPasswordHelper.GetHash(newPassword, Base64UrlTextEncoder.Decode(Salt));
			return this;
		}

		internal UserUpdateProjection ChangeUser(string email, string name, EUserRole[] roles)
		{
			Email = name.HasNotNullArg(nameof(name)).Trim().ToLowerInvariant();
			Roles = roles.HasNotNullArg(nameof(roles));
			Name = name.HasNotNullArg(nameof(name)).Trim();
			return this;
		}
	}

	public class UserProjection : BaseEntity, IProjection<User>, IGetProjection, ISearchProjection, IRemoveProjection
	{
		[JsonProperty]
		public string Email { get; private set; }
		[JsonProperty]
		public string Name { get; private set; }
		[JsonProperty]
		public EUserRole[] Roles { get; private set; }
	}
}