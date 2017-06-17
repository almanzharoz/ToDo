using Newtonsoft.Json;

namespace Core.ElasticSearch.Domain
{
	public class BaseEntityWithParent<T, TProjection> : IEntity, IWithParent<T, TProjection>
		where T : class, IEntity
		where TProjection : IProjection<T>
	{
		[JsonIgnore]
		public string Id { get; set; }
		[JsonIgnore]
		public TProjection Parent { get; set; }
	}
}