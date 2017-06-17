
namespace Core.ElasticSearch.Domain
{
	public interface IProjection<T> : IEntity where T : class, IEntity
	{
		
	}
}