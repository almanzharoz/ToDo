using Core.ElasticSearch.Domain;

namespace Core.Tests.Models
{
	public class Producer : BaseEntity, IProjection<Producer>, IWithVersion
	{
		public string Name { get; set; }
	    public int Version { get; set; }
	}
}