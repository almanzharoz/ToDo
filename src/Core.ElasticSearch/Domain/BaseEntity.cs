using Newtonsoft.Json;

namespace Core.ElasticSearch.Domain
{
	public abstract class BaseEntity : IEntity
	{
		[JsonIgnore]
		public string Id { get; internal set; }
	}

	public abstract class BaseEntityWithVersion : IEntity, IWithVersion
	{
		[JsonIgnore]
		public string Id { get; internal set; }

		[JsonIgnore]
		public int Version { get; internal set; }
	}

	/// <summary>
	/// Используется для добавления новых документов. Такие объекты никогда не попадают в RequestContainer.
	/// </summary>
	public abstract class BaseNewEntity
	{
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