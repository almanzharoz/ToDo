using Core.ElasticSearch.Domain;
using BeeFee.Model.Interfaces;
using Nest;
using Newtonsoft.Json;

namespace BeeFee.Model.Models
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