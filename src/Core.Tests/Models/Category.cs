using Core.ElasticSearch.Domain;
using Nest;

namespace Core.Tests.Models
{
	public class Category : BaseEntity
	{
		[Keyword]
		public Projections.Category Top { get; set; }
		public string Name { get; set; }
	}
}