using BeeFee.Model.Interfaces;
using BeeFee.Model.Models;
using Core.ElasticSearch.Domain;
using Newtonsoft.Json;

namespace BeeFee.Model.Projections
{
	public class BaseCategoryProjection : BaseEntity, IProjection<Category>, IWithName, IGetProjection
	{
		[JsonProperty]
		public string Name { get; private set; }
	}

	public class CategoryProjection : BaseEntity, IProjection<Category>, IWithName, IWithUrl, ISearchProjection
	{
		[JsonProperty]
		public string Name { get; private set; }
		[JsonProperty]
		public string Url { get; private set; }
	}
}