using Core.ElasticSearch.Domain;
using BeeFee.Model.Interfaces;
using Nest;

namespace BeeFee.Model.Models
{
	public class Category : BaseEntityWithVersion, IModel, IWithName, IWithUrl
	{
		[Keyword]
		public string Name { get; set; }
		[Keyword]
		public string Url { get; set; }
	}
}