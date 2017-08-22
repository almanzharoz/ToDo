using Core.ElasticSearch.Domain;
using Expo3.Model.Models;

namespace Expo3.AdminApp.Projections
{
	public class CategoryProjection : BaseEntityWithVersion, IProjection<Category>, IInsertProjection, IRemoveProjection,
		IGetProjection, IUpdateProjection, ISearchProjection
	{
		public string Name { get; set; }
	}
}