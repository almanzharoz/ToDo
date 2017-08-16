using Core.ElasticSearch.Domain;
using Expo3.Model.Embed;
using Nest;
using Newtonsoft.Json;

namespace Expo3.Model.Models

{
	public class BaseUserProjection : BaseEntity, IProjection<User>, IGetProjection
	{
		[JsonProperty]
		public string Name { get; private set; }
		[JsonProperty]
		public EUserRole[] Roles { get; private set; }
	}

	public class User : BaseEntityWithVersion, IModel
    {
        [Keyword]
        public string Email { get; set; }
        [Keyword]
        public string Name { get; set; }
		[Keyword(Index = false, Store = true)]
        public string Password { get; set; }
		[Keyword(Index = false, Store = true)]
        public string Salt { get; set; }
        [Keyword]
        public EUserRole[] Roles { get; set; }
    }

	public class UserName
	{
		public string Id { get; }

		public UserName(string id)
		{
			Id = id;
		}

		public UserName(BaseUserProjection user)
		{
			Id = user.Id;
		}
	}
}
