using Newtonsoft.Json;

namespace Core.ElasticSearch.Domain
{
	public class BaseEntity : IEntity
	{
		[JsonProperty]
		[JsonIgnore]
		public string Id { get; internal set; }
	}

	public class BaseEntityWithVersion : IEntity, IWithVersion
	{
		[JsonProperty]
		[JsonIgnore]
		public string Id { get; internal set; }
		[JsonProperty]
		[JsonIgnore]
		public int Version { get; internal set; }
	}
}