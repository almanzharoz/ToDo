using Core.ElasticSearch.Domain;
using Expo3.Model.Models;
using Newtonsoft.Json;

namespace Expo3.ClientApp.Projections.Event
{
	public class CategoryProjection : BaseEntity, IProjection<Category>, ISearchProjection
	{
		[JsonProperty]
		public string Url { get; private set; }

		[JsonProperty]
		public string Name { get; private set; }
	}
}