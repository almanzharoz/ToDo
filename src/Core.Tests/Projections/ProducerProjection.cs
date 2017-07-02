using Core.ElasticSearch.Domain;

namespace Core.Tests.Projections
{
	public class ProducerProjection : BaseEntity, IProjection<ProducerProjection>
	{
		public string Name { get; set; }
	}
}