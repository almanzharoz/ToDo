using Core.ElasticSearch.Domain;
using Nest;

namespace Core.Tests.Models
{
	public class Product : BaseEntityWithParent<Models.Category, Category>, IWithVersion, IProjection<Product>
	{
		public string Name { get; set; }
		public int Version { get; set; }
		[Keyword]
		public Producer Producer { get; set; }
		public Projections.FullName FullName { get; set; }
	}
}