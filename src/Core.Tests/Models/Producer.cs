using Core.ElasticSearch.Domain;
using Newtonsoft.Json;

namespace Core.Tests.Models
{
	public class Producer : BaseEntityWithVersion, IModel, IProjection<Producer>, IGetProjection, IInsertProjection
	{
		public string Name { get; set; }
	}
}