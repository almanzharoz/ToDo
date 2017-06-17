using Newtonsoft.Json;

namespace Core.ElasticSearch.Domain
{
	public class BaseEntity : IEntity
	{
		[JsonIgnore]
		public string Id { get; set; }
	}
}