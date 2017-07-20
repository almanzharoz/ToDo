
namespace Core.ElasticSearch.Domain
{
	public interface IProjection : IEntity
	{
		
	}

	public interface IProjection<T> : IProjection where T : class, IModel
	{
		
	}
}