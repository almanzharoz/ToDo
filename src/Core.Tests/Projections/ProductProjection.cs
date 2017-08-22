using Core.ElasticSearch.Domain;
using Core.Tests.Models;
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

	public class ProductProjection : BaseEntityWithParent<Models.Category>, IProjection<Models.Product>, IGetProjection
	{
        public string Name { get; set; }
        [JsonProperty]
		public FullName FullName { get; private set; }
    }
}