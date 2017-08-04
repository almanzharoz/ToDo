using Core.ElasticSearch.Domain;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Expo3.AdminApp.Projections
{
	public class UserInsertProjection : BaseEntityWithVersion, IProjection<User>, IInsertProjection
	{
		public string Email { get; set; }
		public string Nickname { get; set; }
		public string Password { get; set; }
		public string Salt { get; set; }
		public EUserRole[] Roles { get; set; }
	}

	public class UserUpdateProjection : BaseEntityWithVersion, IProjection<User>, IGetProjection, IUpdateProjection
	{
		public string Email { get; set; }
		public string Nickname { get; set; }
		public string Password { get; set; }
		public EUserRole[] Roles { get; set; }
		[JsonProperty]
		public string Salt { get; private set; }
	}

	public class UserSearchProjection : BaseEntityWithVersion, IProjection<User>, IGetProjection, ISearchProjection
	{
		[JsonProperty]
		public string Email { get; private set; }
		[JsonProperty]
		public string Nickname { get; private set; }
		[JsonProperty]
		public EUserRole[] Roles { get; private set; }
	}

	public class UserRemoveProjection : BaseEntityWithVersion, IProjection<User>, IGetProjection, IRemoveProjection
	{
	}
}