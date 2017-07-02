using Core.ElasticSearch.Domain;
using Nest;

namespace Core.Tests.Projections
{
	public class CategoryProjection : BaseEntity, IProjection<Models.Category>
	{
		[Keyword]
		public Projections.CategoryProjection Top { get; set; }
		public string Name { get; set; }
	}
}