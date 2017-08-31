using Core.ElasticSearch.Domain;
using Newtonsoft.Json;

namespace BeeFee.ClientApp.Projections.Category
{
	public class CategoryProjection : BaseEntity, IProjection<Model.Models.Category>, ISearchProjection
	{
		[JsonProperty]
		public string Url { get; private set; }

		[JsonProperty]
		public string Name { get; private set; }
	}
}