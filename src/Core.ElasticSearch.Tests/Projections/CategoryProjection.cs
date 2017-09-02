using Core.ElasticSearch.Domain;
using Nest;

namespace Core.ElasticSearch.Tests.Projections
{
	public class CategoryProjection : BaseEntity, IProjection<Models.Category>, IGetProjection, ISearchProjection
	{
		[Keyword]
		public Projections.CategoryProjection Top { get; private set; }
		public string Name { get; private set; }
	}
}