using System;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Elasticsearch.Net;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public abstract partial class BaseService<TSettings>
	{
		protected bool Insert<T>(T entity, bool refresh) where T : class, IEntity
			=> Try(
				c => c.Index(entity, refresh.If(new Func<IndexDescriptor<T>, IIndexRequest>(x => x.Refresh(Refresh.True)), null))
					.Fluent(x => entity
						.Set(e => e.Id, x.Id)
						.Is<T, IWithVersion>(s => s.Set(e => ((IWithVersion) e).Version, (int) x.Version))),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected bool Insert<T>(T entity) where T : class, IEntity => Insert(entity, true);

		protected bool Insert<T, TParentModel, TParentProjection>(T entity, bool refresh)
			where T : class, IEntity, IWithParent<TParentModel, TParentProjection>
			where TParentModel : class, IEntity
			where TParentProjection : IProjection<TParentModel>
			=> Try(
				c => c.Index(entity, s => s.If(refresh, a => a.Refresh(Refresh.True)).IfNotNull(entity.Parent, a => a.Parent(entity.Parent.Id)))
					.Fluent(x => entity
						.Set(p => p.Id, x.Id)
						.Is<T, IWithVersion>(s => s.Set(e => ((IWithVersion) e).Version, (int) x.Version))),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected bool Insert<T, TParentModel, TParentProjection>(T entity)
			where T : class, IEntity, IWithParent<TParentModel, TParentProjection>
			where TParentModel : class, IEntity
			where TParentProjection : IProjection<TParentModel> 
			=> Insert<T, TParentModel, TParentProjection>(entity, true);

		protected Task<bool> InsertAsync<T>(T entity, bool refresh = true) where T : class, IEntity
			=> TryAsync(
				c => c.IndexAsync(entity,
						refresh.If(new Func<IndexDescriptor<T>, IIndexRequest>(x => x.Refresh(Refresh.True)), null))
					.Fluent(x => entity.Set(p => p.Id, x.Id)
						.Is<T, IWithVersion>(s => s.Set(e => ((IWithVersion) e).Version, (int) x.Version))),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected bool Update<T>(T entity, bool refresh) where T : class, IEntity, IWithVersion
			=> Try(
				c => c.Update(
						DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, x => x.Version, nameof(entity))),
						d => d.Version(entity.Version).Doc(entity).If(refresh, x => x.Refresh(Refresh.True)))
					.Fluent(r => entity.Set(p => p.Version, (int) r.Version)),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity?.Id})");

		protected bool Update<T>(T entity) where T : class, IEntity, IWithVersion => Update(entity, true);

		protected Task<bool> UpdateAsync<T>(T entity, bool refresh = true) where T : class, IEntity, IWithVersion
			=> TryAsync(
				c => c.UpdateAsync(
						DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, x => x.Version, nameof(entity))),
						d => d.Version(entity.Version).Doc(entity).If(refresh, x => x.Refresh(Refresh.True)))
					.Fluent(r => entity.Set(p => p.Version, (int) r.Version)),
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

		protected bool Remove<T, TModel>(T entity)
			where T : class, IEntity, IWithVersion, IProjection<TModel>
			where TModel : class, IEntity
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity))), x => x.Version(entity.Version).Refresh(Refresh.True)),
				r => r.Found,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {entity.Id})");

		protected Task<bool> RemoveAsync<T>(T entity) where T : class, IEntity, IWithVersion
			=> TryAsync(
				c => c.DeleteAsync(DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity))), x => x.Version(entity.Version).Refresh(Refresh.True)),
				r => r.Found,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {entity.Id})");

		protected int Remove<T>(QueryContainer query) where T : class, IEntity
			=> Try(
				c => c.DeleteByQuery<T>(d => d.Query(q => q.Bool(b => b.Filter(query))).Refresh()),
				r => (int)r.Deleted,
				RepositoryLoggingEvents.ES_REMOVEBYQUERY);

		protected Task<int> RemoveAsync<T>(QueryContainer query) where T : class, IEntity
			=> TryAsync(
				c => c.DeleteByQueryAsync<T>(d => d.Query(q => q.Bool(b => b.Filter(query))).Refresh()),
				r => (int)r.Deleted,
				RepositoryLoggingEvents.ES_REMOVEBYQUERY);
	}
}