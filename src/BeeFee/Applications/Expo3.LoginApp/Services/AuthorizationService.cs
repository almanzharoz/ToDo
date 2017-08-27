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

	    public UserProjection TryLogin(string email, string password)
		    => Filter<User, UserProjection>(q => q.Term(x => x.Email, email), null, 1)
			    .FirstOrDefault(x => x.CheckPassword(password));

        public UserRegistrationResult Register(string email, string name, string password)
        {
            if (String.IsNullOrEmpty(email))
                return UserRegistrationResult.EmailIsEmpty;

            if (!CommonHelper.IsValidEmail(email))
                return UserRegistrationResult.WrongEmail;

            if (String.IsNullOrWhiteSpace(password))
                return UserRegistrationResult.PasswordIsEmpty;

            if (String.IsNullOrEmpty(name))
                return UserRegistrationResult.NameIsEmpty;

            if (FilterCount<UserProjection>(q => q.Term(x => x.Email, email.ToLowerInvariant())) > 0)
                return UserRegistrationResult.EmailAlreadyExists;

	        return Insert(new RegisterUserProjection(email, name, password, new[] {EUserRole.User, EUserRole.Organizer, EUserRole.Admin}), true)
		        ? UserRegistrationResult.Ok
		        : UserRegistrationResult.UnknownError;
        }

	    public bool ChangePassword(string email, string oldPassword, string newPassword)
		    => TryLogin(email, oldPassword)
			    .IfNotNullOrDefault(
				    user => Update<UpdatePasswordProjection>(user.Id, x => x.ChangePassword(/*oldPassword, */newPassword), true));

	}
}