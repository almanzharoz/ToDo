using Newtonsoft.Json;

namespace Core.ElasticSearch.Domain
{
	public class BaseEntityWithParent<T> : BaseEntity, IWithParent<T>
		where T : IProjection
	{
		[JsonIgnore]
		public T Parent { get; set; }
	}

	public class BaseEntityWithParentAndVersion<T> : BaseEntityWithVersion, IWithParent<T>
		where T : IProjection
	{
		[JsonIgnore]
		public T Parent { get; set; }
	}
}