using Newtonsoft.Json;

namespace Core.ElasticSearch.Domain
{
	public class BaseEntity : IEntity
	{
		[JsonProperty]
		[JsonIgnore]
		public string Id { get; internal set; }

		protected void SetId(string id)
		{
			Id = id;
		}
	}

	public class BaseEntityWithVersion : BaseEntity, IWithVersion
	{
		[JsonProperty]
		[JsonIgnore]
		public int Version { get; internal set; }
	}
}