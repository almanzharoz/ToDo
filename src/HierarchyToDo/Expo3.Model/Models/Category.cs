using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;

namespace Expo3.Model.Models
{
	public class Category : BaseEntityWithVersion, IModel
	{
		[Keyword]
		public string Name { get; set; }
	}

	public class BaseCategoryProjection : BaseEntity, IProjection<Category>, IGetProjection, ISearchProjection
	{
		[JsonProperty]
		public string Name { get; private set; }
	}
}