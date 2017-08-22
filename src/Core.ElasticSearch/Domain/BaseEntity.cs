using Newtonsoft.Json;

namespace Core.ElasticSearch.Domain
{
	public abstract class BaseEntity : IEntity
	{
		[JsonProperty]
		[JsonIgnore]
		public string Id { get; internal set; }
	}

	public abstract class BaseEntityWithVersion : IEntity, IWithVersion
	{
		[JsonProperty]
		[JsonIgnore]
		public string Id { get; internal set; }

		[JsonProperty]
		[JsonIgnore]
		public int Version { get; internal set; }
	}

	public abstract class BaseNewEntity : IInsertProjection
	{
		[JsonProperty]
		[JsonIgnore]
		public string Id { get; internal set; }

		protected BaseNewEntity() { }

		protected BaseNewEntity(string id)
		{
			Id = id;
		}
	}

}