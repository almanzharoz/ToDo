using Core.ElasticSearch.Domain;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Newtonsoft.Json;

namespace Expo3.LoginApp.Projections
{
    public class UserProjection : BaseEntity, IProjection<User>, IGetProjection, ISearchProjection
    {
        [JsonProperty]
        public string Email { get; private set; }
        [JsonProperty]
        public string Nickname { get; private set; }
        [JsonProperty]
        public string Password { get; private set; }
        [JsonProperty]
        public string Salt { get; private set; }
        [JsonProperty]
        public EUserRole[] Roles { get; private set; }
    }

	public class RegisterUserProjection : BaseEntity, IProjection<User>, IInsertProjection
	{
		public string Email { get; set; }
		public string Nickname { get; set; }
		public string Password { get; set; }
		public string Salt { get; set; }
		public EUserRole[] Roles { get; set; }
	}

	public class LoginUserProjection : BaseEntity, IProjection<User>, ISearchProjection
	{
        [JsonProperty]
		public string Nickname { get; private set; }
        [JsonProperty]
		public EUserRole[] Roles { get; private set; }

		public LoginUserProjection(){} // for serializer

		internal LoginUserProjection(UserProjection user) // for auth service (without RequestContainer)
		{			
			SetId(user.Id);
			Nickname = user.Nickname;
			Roles = user.Roles;
		}
	}

	public class UpdatePasswordProjection : BaseEntityWithVersion, IProjection<User>, IUpdateProjection, IGetProjection
	{
		public string Password { get; set; }
		public string Salt { get; set; }
	}
}