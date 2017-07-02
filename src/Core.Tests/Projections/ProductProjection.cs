using Core.ElasticSearch.Domain;
using Nest;

namespace Core.Tests.Projections
{
	public struct FullName
	{
		public string Name { get; set; }
		public string Category { get; set; }
		public string Producer { get; set; }
	}

	public class ProductProjection : BaseEntityWithParent<Models.Category, CategoryProjection>, IProjection<Models.Product>, IWithVersion
	{
		public int Version { get; set; }
		[Keyword]
		public FullName FullName { get; set; }
	}
}