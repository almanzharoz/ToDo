using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;

namespace Core.Tests.Projections
{
	public class CategoryProjection : BaseEntity, IProjection<Models.Category>
	{
		[JsonProperty]
		[Keyword]
		public Projections.CategoryProjection Top { get; private set; }
		[JsonProperty]
		public string Name { get; private set; }
	}
}