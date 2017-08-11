using Core.ElasticSearch.Domain;
using Nest;

namespace Expo3.Model.Models
{
	public class Category : BaseEntityWithVersion
	{
		[Keyword]
		public string Name { get; set; }
	}
}