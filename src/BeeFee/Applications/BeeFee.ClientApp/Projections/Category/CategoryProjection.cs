using Core.ElasticSearch.Domain;

namespace BeeFee.ClientApp.Projections.Category
{
	public class CategoryProjection : BaseEntity, IProjection<Model.Models.Category>, ISearchProjection
	{
		public string Url { get; private set; }

		public string Name { get; private set; }
	}
}