using Core.ElasticSearch.Domain;
using ToDo.Dal.Interfaces;

namespace ToDo.WebApp.Model
{
	public struct ModelWithName<T> where T : IEntity, IWithName
	{
		public string Id { get; set; }
		public string Name { get; set; }

		public ModelWithName(T entity)
		{
			Id = entity.Id;
			Name = entity.Name;
		}
	}
}