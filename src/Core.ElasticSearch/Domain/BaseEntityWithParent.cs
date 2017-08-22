using Newtonsoft.Json;

namespace Core.ElasticSearch.Domain
{
	public abstract class BaseEntityWithParent<T> : BaseEntity, IWithParent<T>
		where T : IProjection
	{
		[JsonIgnore]
		public T Parent { get; internal set; }
	}

	public abstract class BaseEntityWithParentAndVersion<T> : BaseEntityWithVersion, IWithParent<T>
		where T : IProjection
	{
		[JsonIgnore]
		public T Parent { get; internal set; }
	}

	public abstract class BaseNewEntityWithParent<T> : BaseEntity, IWithParent<T>, IProjection, IInsertProjection
		where T : IProjection
	{
		[JsonIgnore]
		public T Parent { get; }

		protected BaseNewEntityWithParent() { } // TODO: Заменить везде where new() на контракты

		protected BaseNewEntityWithParent(T parent)
		{
			Parent = parent;
		}
	}
}