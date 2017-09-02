﻿using System;
using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
using Core.ElasticSearch.Domain;
using Microsoft.AspNetCore.WebUtilities;
using SharpFuncExt;

namespace BeeFee.AdminApp.Projections.User
{
	internal class UserUpdateProjection : BaseEntity, IProjection<Model.Models.User>, IGetProjection, IUpdateProjection
	{
		public string Email { get; private set; }
		public string Name { get; private set; }
		public string Password { get; private set; }
		public EUserRole[] Roles { get; private set; }
		public string Salt { get; private set; }

		/// <summary>
		/// Метод не для админки. Добавил как пример использования. Надо его в LoginApp добавить.
		/// </summary>
		public UserUpdateProjection ChangePassword(string oldPassword, string newPassword)
		{
			if (oldPassword != Password)
				throw new Exception("OldPassword");
			Password = HashPasswordHelper.GetHash(newPassword, Base64UrlTextEncoder.Decode(Salt));
			return this;
		}

		public UserUpdateProjection ChangeUser(string email, string name, EUserRole[] roles)
		{
			Email = email.HasNotNullArg(nameof(name)).Trim().ToLowerInvariant();
			Roles = roles.HasNotNullArg(nameof(roles));
			Name = name.HasNotNullArg(nameof(name)).Trim();
			return this;
		}
	}
}