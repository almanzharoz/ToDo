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

		/// <summary>
		/// Используется, если хотим использовать свой Id при вставке
		/// </summary>
		/// <param name="id"></param>
		protected BaseNewEntity(string id)
		{
			Id = id;
		}
	}

}