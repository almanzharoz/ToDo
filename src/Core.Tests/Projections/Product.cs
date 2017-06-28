using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;

namespace Core.Tests.Projections
{
	public struct FullName
	{
		public string Name { get; set; }
		public string Category { get; set; }
		public string Producer { get; set; }
	}

	public class Product : BaseEntityWithParent<Models.Category, Category>, IProjection<Models.Product>, IWithVersion
	{
		public int Version { get; set; }
		[JsonProperty]
		[Keyword]
		public FullName FullName { get; private set; }
	}
}