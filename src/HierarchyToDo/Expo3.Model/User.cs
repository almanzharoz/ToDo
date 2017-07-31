using Core.ElasticSearch.Domain;
using Expo3.Model.Embed;
using Nest;
using Newtonsoft.Json;

namespace Expo3.Model
{
	public class BaseUserProjection : BaseEntity, IProjection, IGetProjection
	{
		[JsonProperty]
		public string Nickname { get; }
		[JsonProperty]
		public EUserRole[] Roles { get; }
	}

	public class User : BaseEntityWithVersion, IModel
    {
        [Keyword]
        public string Email { get; set; }
        [Keyword]
        public string Nickname { get; set; }
		[Keyword(Index = false, Store = true)]
        public string Password { get; set; }
		[Keyword(Index = false, Store = true)]
        public string Salt { get; set; }
        [Keyword]
        public EUserRole[] Roles { get; set; }
    }
}
