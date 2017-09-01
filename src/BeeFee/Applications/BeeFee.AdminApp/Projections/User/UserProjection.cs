using BeeFee.Model.Embed;
using Core.ElasticSearch.Domain;

namespace BeeFee.AdminApp.Projections.User
{
	public class UserProjection : BaseEntity, IProjection<Model.Models.User>, IGetProjection, ISearchProjection, IRemoveProjection
	{
		public string Email { get; private set; }
		public string Name { get; private set; }
		public EUserRole[] Roles { get; private set; }
	}
}