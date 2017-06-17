using Core.ElasticSearch.Domain;
using SharpFuncExt;
using ToDo.Dal.Interfaces;
using ToDo.Dal.Models;

namespace ToDo.Dal.Projections
{
	public class User : IProjection<Models.User>
	{
		public string Id { get; set; }
		public string Nick { get; set; }
		public bool Deny { get; set; }

		public static implicit operator User(UserName userName) => userName.IfNotNullOrDefault(x=>new User { Id = x.Id });
		public static implicit operator User(string userName) => userName.IfNotNullOrDefault(x => new User { Id = x });

		public static bool operator ==(User user, string userName) => user?.Id == null && userName == null || user?.Id != null && user.Id.Equals(userName);
		public static bool operator !=(User user, string userName) => user?.Id == null || !user.Id.Equals(userName);

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

	public class UserWithRoles : User
	{
		public string[] Roles { get; set; }
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