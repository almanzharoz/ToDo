using Core.ElasticSearch.Domain;
using Nest;

namespace Expo3.Model.Models
{
	public class Category : BaseEntity
	{
		[Keyword]
		public string Name { get; set; }
	}
}