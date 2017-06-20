using Core.ElasticSearch.Domain;
using Nest;

namespace Core.Tests.Projections
{
	public class Category : BaseEntity, IProjection<Models.Category>
	{
		[Keyword]
		public Projections.Category Top { get; set; }
		public string Name { get; set; }
	}
}