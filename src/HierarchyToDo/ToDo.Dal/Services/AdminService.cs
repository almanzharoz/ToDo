﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;
using ToDo.Dal.Models;

namespace ToDo.Dal.Services
{
	public class AdminService : BaseToDoService
	{
		private static readonly MD5 md5 = MD5.Create();

		public AdminService(ILoggerFactory loggerFactory, ElasticConnection settings, ElasticScopeFactory<ElasticConnection> factory, Projections.UserName user) 
			: base(loggerFactory, settings, factory, user)
		{
		}

		public bool AddUser(string email, string name, string password, EUserRole[] roles)
			=> FilterCount<Projections.UserWithRoles>(q => q.Term(p => p.Email, email.ToLower()))
				.If(p => p == 0, x =>
					Insert(new Projections.NewUser()
					{
						Email = email.ToLower(),
						Nick = name ?? email,
						Password = Base64UrlTextEncoder.Encode(md5.ComputeHash(Encoding.UTF8.GetBytes(password))),
						Roles = roles
					}), x => false);

		public IReadOnlyCollection<Projections.UserWithRoles> GetUsers() => Filter<Models.User, Projections.UserWithRoles>(q => null);

		public bool DeleteRole(string id, EUserRole role)
			=> Update<Projections.UserWithRoles>(q => q.Ids(x => x.Values(id)),
					u => u.Remove(x => x.Roles, role)) > 0;

		public bool AddRole(string id, EUserRole role)
			=> Update<Projections.UserWithRoles>(q => q.Ids(x => x.Values(id)),
					u => u.Add(x => x.Roles, role)) > 0;

		public bool DenyUser(string id, bool deny)
			=> Update<Projections.UserWithRoles>(q => q.Ids(x => x.Values(id)),
					u => u.Set(x => x.Deny, deny)) > 0;

		public bool DeleteUser(string id) => Remove<Projections.User>(GetUser(id));
		public Projections.UserWithRoles GetUser(string id) => Get<Projections.UserWithRoles>(id.HasNotNullArg("userId"));

		public IReadOnlyCollection<Projections.User> GetUsersNames(string s) =>
			Search<Models.User, Projections.User>(q => q.Match(m => m.Field(p => p.Nick).Query(s)));
	}
}