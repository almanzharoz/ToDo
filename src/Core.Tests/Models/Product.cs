using Core.ElasticSearch.Domain;
using Nest;

namespace Core.Tests.Models
{
	public class Product : BaseEntityWithParent<Models.Category, Projections.Category>, IWithVersion
	{
		public string Name { get; set; }
		public int Version { get; set; }
		[Keyword]
		public Projections.Category Category { get; set; }
		[Keyword]
		public Projections.Product Producer { get; set; }
		public Projections.FullName FullName { get; set; }
	}
}