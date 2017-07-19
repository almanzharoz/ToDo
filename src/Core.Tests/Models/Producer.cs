using Core.ElasticSearch.Domain;
using Newtonsoft.Json;

namespace Core.Tests.Models
{
	public class Producer : BaseEntity, IProjection<Producer>, IWithVersion
	{
		public string Name { get; set; }
        [JsonIgnore]
        public int Version { get; set; }
	}
}