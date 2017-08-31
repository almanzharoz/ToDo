using BeeFee.Model.Embed;
using Core.ElasticSearch.Domain;
using Newtonsoft.Json;

namespace BeeFee.AdminApp.Projections.User
{
	public class UserProjection : BaseEntity, IProjection<Model.Models.User>, IGetProjection, ISearchProjection, IRemoveProjection
	{
		[JsonProperty]
		public string Email { get; private set; }
		[JsonProperty]
		public string Name { get; private set; }
		[JsonProperty]
		public EUserRole[] Roles { get; private set; }
	}
}