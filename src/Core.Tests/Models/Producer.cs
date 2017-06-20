using Core.ElasticSearch.Domain;

namespace Core.Tests.Models
{
	public class Producer : BaseEntity, IProjection<Producer>
	{
		public string Name { get; set; }
	}
}