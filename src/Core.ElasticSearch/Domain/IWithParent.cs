namespace Core.ElasticSearch.Domain
{
	public interface IWithParent<T, TProjection>
		where T : class, IEntity
		where TProjection : IProjection<T>
	{
		TProjection Parent { get; set; }
	}
}