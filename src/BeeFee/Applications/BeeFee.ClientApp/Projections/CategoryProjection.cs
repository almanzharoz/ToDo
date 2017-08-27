using Core.ElasticSearch.Domain;
using BeeFee.Model.Models;
using Newtonsoft.Json;

namespace BeeFee.ClientApp.Projections
{
	public class CategoryProjection : BaseEntity, IProjection<Category>, ISearchProjection
	{
		[JsonProperty]
		public string Url { get; private set; }

		[JsonProperty]
		public string Name { get; private set; }
	}
}