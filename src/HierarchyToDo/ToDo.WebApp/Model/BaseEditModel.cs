using System;
using Core.ElasticSearch.Domain;

namespace ToDo.WebApp.Model
{
	public abstract class BaseEditModel<T> where T : IEntity, IWithVersion
	{
		public string Id { get; set; }
		public int Version { get; set; }

		protected BaseEditModel() { }

		protected BaseEditModel(T entity)
		{
			Id = entity.Id;
			Version = entity.Version;
		}

		/// <summary>
		/// Обновляет нужные поля.
		/// </summary>
		/// <param name="entity">Загруженная модель перед обновлением</param>
		/// <returns></returns>
		public abstract T Update(T entity);
	}
}