using System;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Elasticsearch.Net;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public abstract partial class BaseRepository<TSettings>
	{
		protected bool Insert<T>(T entity, bool refresh) where T : class, IEntity
			=> Try(
				c => c.Index(entity, refresh.If(new Func<IndexDescriptor<T>, IIndexRequest>(x => x.Refresh(Refresh.True)), null)).Fluent(x => entity.Id = x.Id),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected bool Insert<T>(T entity) where T : class, IEntity => Insert(entity, true);

		protected Task<bool> InsertAsync<T>(T entity, bool refresh = true) where T : class, IEntity
			=> TryAsync(
				c => c.IndexAsync(entity, refresh.If(new Func<IndexDescriptor<T>, IIndexRequest>(x => x.Refresh(Refresh.True)), null)),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected bool Update<T>(T entity, bool refresh) where T : class, IEntity, IWithVersion
			=> Try(
				c => c.Update(
					DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, x => x.Version, nameof(entity))),
					d => d.Version(entity.Version).Doc(entity).If(refresh, x => x.Refresh(Refresh.True))),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity?.Id})");

		protected bool Update<T>(T entity) where T : class, IEntity, IWithVersion => Update(entity, true);

		protected Task<bool> UpdateAsync<T>(T entity, bool refresh = true) where T : class, IEntity, IWithVersion
			=> TryAsync(
				c => c.UpdateAsync(
					DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, x => x.Version, nameof(entity))),
					d => d.Version(entity.Version).Doc(entity).If(refresh, x => x.Refresh(Refresh.True))),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity?.Id})");

		protected int Update<T>(QueryContainer query, UpdateByQueryBuilder<T> update, bool refresh = true) where T : class, IEntity, IWithVersion
			=> Try(
				c => c.UpdateByQuery<T>(x => x
					.Query(q => q.Bool(b => b.Filter(query)))
					.Version()
					.If(refresh, y => y.Refresh())
					.Script(s => s.Inline(update).Params(update.GetParams))),
				r => (int)r.Updated,
				RepositoryLoggingEvents.ES_UPDATEBYQUERY);

		protected Task<(int updated, int total)> UpdateAsync<T>(QueryContainer query, UpdateByQueryBuilder<T> update, bool refresh = true)
			where T : class, IEntity, IWithVersion
			=> TryAsync(
				c => c.UpdateByQueryAsync<T>(x => x
					.Query(q => q.Bool(b => b.Filter(query)))
					.Version()
					.If(refresh, y => y.Refresh())
					.Script(s => s.Inline(update).Params(update.GetParams))),
				r => ((int)r.Updated, (int)r.Total),
				RepositoryLoggingEvents.ES_UPDATEBYQUERY);

		protected bool Remove<T>(T entity) where T : class, IEntity, IWithVersion
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity))), x => x.Version(entity.Version)),
				r => r.Found,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {entity.Id})");

		protected Task<bool> RemoveAsync<T>(T entity) where T : class, IEntity, IWithVersion
			=> TryAsync(
				c => c.DeleteAsync(DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity))), x => x.Version(entity.Version)),
				r => r.Found,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {entity.Id})");

		protected int Remove<T>(QueryContainer query) where T : class, IEntity
			=> Try(
				c => c.DeleteByQuery<T>(d => d.Query(q => q.Bool(b => b.Filter(query)))),
				r => (int)r.Deleted,
				RepositoryLoggingEvents.ES_REMOVEBYQUERY);

		protected Task<int> RemoveAsync<T>(QueryContainer query) where T : class, IEntity
			=> TryAsync(
				c => c.DeleteByQueryAsync<T>(d => d.Query(q => q.Bool(b => b.Filter(query)))),
				r => (int)r.Deleted,
				RepositoryLoggingEvents.ES_REMOVEBYQUERY);
	}
}