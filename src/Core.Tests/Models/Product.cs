using Core.ElasticSearch.Domain;
using Core.Tests.Projections;
using Nest;
using Newtonsoft.Json;

namespace Core.Tests.Models
{
	public class Product : BaseEntityWithParent<Models.Category, Models.Category>, IWithVersion, IProjection<Product>
	{
		public string Name { get; set; }
        [JsonIgnore]
        public int Version { get; set; }
		[Keyword]
		public Producer Producer { get; set; }
		public Projections.FullName FullName { get; set; }
	}
}