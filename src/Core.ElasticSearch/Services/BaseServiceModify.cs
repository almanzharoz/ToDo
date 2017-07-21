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
		protected bool Insert<T>(T entity, bool refresh) where T : BaseEntity, IModel
			=> Try(
				c => c.Index(entity, refresh.If(new Func<IndexDescriptor<T>, IIndexRequest>(x => x.Refresh(Refresh.True)), null))
					.Fluent(x => entity
						.Is<T, BaseEntityWithVersion>(s => (s as BaseEntityWithVersion).Version = (int) x.Version)
						.Id = x.Id),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected bool Insert<T>(T entity) where T : BaseEntity, IModel => Insert(entity, true);

		protected bool Insert<T, TParent>(T entity, bool refresh)
			where T : BaseEntity, IModel, IWithParent<TParent>
			where TParent : IProjection
			=> Try(
				c => c.Index(entity, s => s.If(refresh, a => a.Refresh(Refresh.True)).IfNotNull(entity.Parent, a => a.Parent(entity.Parent.Id)))
					.Fluent(x => entity
						.Is<T, BaseEntityWithVersion>(s => (s as BaseEntityWithVersion).Version = (int)x.Version)
						.Id = x.Id),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected bool Insert<T, TParent>(T entity)
			where T : BaseEntity, IModel, IWithParent<TParent>
			where TParent : IProjection 
			=> Insert<T, TParent>(entity, true);

		protected Task<bool> InsertAsync<T>(T entity, bool refresh = true) where T : BaseEntity, IModel
			=> TryAsync(
				c => c.IndexAsync(entity,
						refresh.If(new Func<IndexDescriptor<T>, IIndexRequest>(x => x.Refresh(Refresh.True)), null))
					.Fluent(x => entity
						.Is<T, BaseEntityWithVersion>(s => (s as BaseEntityWithVersion).Version = (int) x.Version)
						.Id = x.Id),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected bool Update<T>(T entity, bool refresh) where T : BaseEntityWithVersion, IProjection
			=> Try(
				c => c.Update(
						DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, x => x.Version, nameof(entity))),
						d => d.Version(entity.Version).Doc(entity).If(refresh, x => x.Refresh(Refresh.True)))
					.Fluent(r => entity.Version = (int) r.Version),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity?.Id})");

		protected bool Update<T>(T entity) where T : BaseEntityWithVersion, IProjection => Update(entity, true);

		protected Task<bool> UpdateAsync<T>(T entity, bool refresh = true) where T : BaseEntityWithVersion, IProjection, IWithVersion
			=> TryAsync(
				c => c.UpdateAsync(
						DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, x => x.Version, nameof(entity))),
						d => d.Version(entity.Version).Doc(entity).If(refresh, x => x.Refresh(Refresh.True)))
					.Fluent(r => entity.Version = (int)r.Version),
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

		protected bool Remove<T>(T entity)
			where T : class, IWithVersion, IProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity))), x => x.Version(entity.Version.HasNotNullArg("version")).Refresh(Refresh.True)),
				r => r.Found,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {entity.Id})");

		protected Task<bool> RemoveAsync<T>(T entity) where T : class, IProjection, IWithVersion
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