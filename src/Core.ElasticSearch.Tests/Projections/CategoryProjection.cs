using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;

namespace Core.ElasticSearch.Tests.Projections
{
	public class CategoryProjection : BaseEntity, IProjection<Models.Category>, IGetProjection, ISearchProjection
	{
        [JsonProperty]
		[Keyword]
		public Projections.CategoryProjection Top { get; private set; }
		[JsonProperty]
		public string Name { get; private set; }
	}
}