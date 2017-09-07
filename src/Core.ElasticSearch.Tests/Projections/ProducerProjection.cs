using Core.ElasticSearch.Domain;

namespace Core.ElasticSearch.Tests.Projections
{
	public class ProducerProjection : BaseEntity, IProjection<Models.Producer>
	{
		public string Name { get; set; }

		public ProducerProjection(string id) : base(id)
		{
		}
	}
}