using BeeFee.Model.Embed;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Models;
using Core.ElasticSearch.Domain;
using Newtonsoft.Json;

namespace BeeFee.Model.Projections
{
	public class BaseUserProjection : BaseEntity, IProjection<User>, IGetProjection, IWithName
	{
		[JsonProperty]
		public string Name { get; private set; }
		[JsonProperty]
		public EUserRole[] Roles { get; private set; }
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