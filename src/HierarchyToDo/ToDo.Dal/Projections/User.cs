using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;
using SharpFuncExt;
using ToDo.Dal.Interfaces;
using ToDo.Dal.Models;

namespace ToDo.Dal.Projections
{
	public class User : BaseEntityWithVersion, IProjection<Models.User>, ISearchProjection, IRemoveProjection
	{
		[JsonProperty]
		public string Nick { get; private set; }
		public bool Deny { get; set; }

		public static implicit operator User(UserName userName) => userName.IfNotNullOrDefault(x=>new User(x.Id));
		public static implicit operator User(string userName) => userName.IfNotNullOrDefault(x => new User(x));

		public static bool operator ==(User user, string userName) => user?.Id == null && userName == null || user?.Id != null && user.Id.Equals(userName);
		public static bool operator !=(User user, string userName) => user?.Id == null || !user.Id.Equals(userName);

		public User() { }
		internal User(string id) => SetId(id);

		protected bool Equals(User other)
		{
			return object.ReferenceEquals(this, other) || string.Equals(Id, other.Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((User)obj);
		}

		public override int GetHashCode()
		{
			return Id?.GetHashCode() ?? 0;
		}
	}

	public class UserWithRoles : User, IUpdateProjection, IGetProjection
	{
		[JsonProperty]
		public EUserRole[] Roles { get; private set; }
		[JsonProperty]
		public string Email { get; private set; }
	}

	public class NewUser : BaseEntity, IProjection<Models.User>, IInsertProjection
	{
		[JsonProperty]
		public string Nick { get; set; }
		[JsonProperty]
		public EUserRole[] Roles { get; set; }
		[JsonProperty]
		public string Email { get; set; }
		[Keyword]
		public string Password { get; set; }
	}

	public class UserName
	{
		public UserName(string id)
		{
			Id = id;
		}

		public UserName(User user)
		{
			Id = user.Id;
		}
		public string Id { get; }
	}
}