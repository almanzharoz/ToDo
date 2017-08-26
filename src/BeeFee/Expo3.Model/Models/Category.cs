using Core.ElasticSearch.Domain;
using Expo3.Model.Interfaces;
using Nest;
using Newtonsoft.Json;

namespace Expo3.Model.Models
{
	public class Category : BaseEntityWithVersion, IModel, IWithName, IWithUrl
	{
		[Keyword]
		public string Name { get; set; }
		[Keyword]
		public string Url { get; set; }
	}

	public class BaseCategoryProjection : BaseEntity, IProjection<Category>, IGetProjection, IWithName
	{
		[JsonProperty]
		public string Name { get; private set; }
	}
}