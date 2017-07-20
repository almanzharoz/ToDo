using Core.ElasticSearch.Domain;

namespace Core.Tests.Projections
{
	public class ProducerProjection : BaseEntity, IProjection<Models.Producer>
	{
		public string Name { get; set; }
	}
}