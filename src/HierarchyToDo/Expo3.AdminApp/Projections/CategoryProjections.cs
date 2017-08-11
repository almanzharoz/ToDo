using Core.ElasticSearch.Domain;

namespace Expo3.AdminApp.Projections
{
	public class CategoryProjection : BaseEntityWithVersion, IProjection, IInsertProjection, IRemoveProjection, IGetProjection, IUpdateProjection
	{
		public string Name { get; set; }
	}
}