using Core.ElasticSearch.Domain;
using Newtonsoft.Json;

namespace Core.Tests.Models
{
	public class Producer : BaseEntityWithVersion, IModel, IProjection<Producer>, IGetProjection
	{
		public string Name { get; set; }
	}

	public class NewProducer : BaseNewEntity, IProjection<Producer>
	{
		public string Name { get; set; }
	}

}