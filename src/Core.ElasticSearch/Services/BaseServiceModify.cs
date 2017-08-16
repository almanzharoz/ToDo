using System;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Elasticsearch.Net;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public abstract partial class BaseService<TConnection>
	{
		protected bool Insert<T>(T entity, bool refresh) where T : BaseEntity, IProjection, IInsertProjection
			=> Try(
				c => c.Index(entity, s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>()))
						//.If(refresh, x => x.Refresh(Refresh.True)))
					.Fluent(x => entity
						.Is<T, BaseEntityWithVersion>(s => (s as BaseEntityWithVersion).Version = (int) x.Version)
						.Id = x.Id),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected bool Insert<T>(T entity) where T : BaseEntity, IProjection, IInsertProjection => Insert(entity, true);

		protected bool Insert<T, TParent>(T entity, bool refresh)
			where T : BaseEntity, IProjection, IInsertProjection, IWithParent<TParent>
			where TParent : IProjection
			=> Try(
				c => c.Index(entity, s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.If(refresh, a => a.Refresh(Refresh.True))
						.IfNotNull(entity.Parent, a => a.Parent(entity.Parent.Id)))
					.Fluent(x => entity
						.Is<T, BaseEntityWithVersion>(s => (s as BaseEntityWithVersion).Version = (int) x.Version)
						.Id = x.Id),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected bool Insert<T, TParent>(T entity)
			where T : BaseEntity, IProjection, IInsertProjection, IWithParent<TParent>
			where TParent : IProjection 
			=> Insert<T, TParent>(entity, true);

		protected Task<bool> InsertAsync<T>(T entity, bool refresh = true)
			where T : BaseEntity, IProjection, IInsertProjection
			=> TryAsync(
				c => c.IndexAsync(entity, s => s
						.Index(_mapping.GetIndexName<T>())
						.Type(_mapping.GetTypeName<T>())
						.If(refresh, x => x.Refresh(Refresh.True)))
					.Fluent(x => entity
						.Is<T, BaseEntityWithVersion>(s => (s as BaseEntityWithVersion).Version = (int) x.Version)
						.Id = x.Id),
				r => r.Created,
				RepositoryLoggingEvents.ES_INSERT);

		protected bool Update<T>(T entity, bool refresh) where T : BaseEntityWithVersion, IProjection, IUpdateProjection
			=> Try(
				c => c.Update(
						DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, x => x.Version, nameof(entity))), d => d
							.Index(_mapping.GetIndexName<T>())
							.Type(_mapping.GetTypeName<T>())
							.Version(entity.Version)
							.Doc(entity)
							.If(refresh, x => x.Refresh(Refresh.True)))
					.Fluent(r => entity.Version = (int) r.Version),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity?.Id})");

		protected bool Update<T>(T entity) where T : BaseEntityWithVersion, IProjection, IUpdateProjection => Update(entity, true);

		protected bool Update<T>(T entity, Func<T, T> setter, bool refresh) where T : BaseEntityWithVersion, IProjection, IUpdateProjection
			=> Try(
				c => c.Update(
						DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, x => x.Version, nameof(entity))), d => d
							.Index(_mapping.GetIndexName<T>())
							.Type(_mapping.GetTypeName<T>())
							.Version(entity.Version)
							.Doc(setter(entity))
							.If(refresh, x => x.Refresh(Refresh.True)))
					.Fluent(r => entity.Version = (int)r.Version),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity?.Id})");

		protected bool Update<T>(T entity, Func<T, T> setter) where T : BaseEntityWithVersion, IProjection, IUpdateProjection => Update(entity, setter, true);

		protected Task<bool> UpdateAsync<T>(T entity, bool refresh = true)
			where T : BaseEntityWithVersion, IProjection, IUpdateProjection, IWithVersion
			=> TryAsync(
				c => c.UpdateAsync(
						DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, x => x.Version, nameof(entity))), d => d
							.Index(_mapping.GetIndexName<T>())
							.Type(_mapping.GetTypeName<T>())
							.Version(entity.Version)
							.Doc(entity)
							.If(refresh, x => x.Refresh(Refresh.True)))
					.Fluent(r => entity.Version = (int) r.Version),
				r => r.Result == Result.Updated,
				RepositoryLoggingEvents.ES_UPDATE,
				$"Update (Id: {entity?.Id})");

		protected int Update<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query, Func<UpdateByQueryBuilder<T>, UpdateByQueryBuilder<T>> update, bool refresh = true)
			where T : class, IProjection, IUpdateProjection, IWithVersion
			=> Try(
				c => c.UpdateByQuery<T>(x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Query(q => q.Bool(b => b.Filter(query)))
					.Version()
					.If(refresh, y => y.Refresh())
					.Script(s => s.Inject(new UpdateByQueryBuilder<T>(), update, (s1, u) => s1.Inline(u).Params(u.GetParams)))),
				r => (int) r.Updated,
				RepositoryLoggingEvents.ES_UPDATEBYQUERY);

		protected Task<(int updated, int total)> UpdateAsync<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query, Func<UpdateByQueryBuilder<T>, UpdateByQueryBuilder<T>> update,
			bool refresh = true)
			where T : class, IProjection, IUpdateProjection, IWithVersion
			=> TryAsync(
				c => c.UpdateByQueryAsync<T>(x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Query(q => q.Bool(b => b.Filter(query)))
					.Version()
					.If(refresh, y => y.Refresh())
					.Script(s => s.Inject(new UpdateByQueryBuilder<T>(), update, (s1, u) => s1.Inline(u).Params(u.GetParams)))),
				r => ((int) r.Updated, (int) r.Total),
				RepositoryLoggingEvents.ES_UPDATEBYQUERY);

		protected bool Remove<T>(T entity)
			where T : class, IWithVersion, IProjection, IRemoveProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Version(entity.Version.HasNotNullArg("version"))
					.Refresh(Refresh.True)),
				r => r.Found,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {entity.Id})");

		protected bool Remove<T>(T entity, int version)
			where T : class, IWithVersion, IProjection, IRemoveProjection
			=> Try(
				c => c.Delete(DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Version(version.HasNotNullArg("version"))
					.Refresh(Refresh.True)),
				r => r.Found,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {entity.Id})");

		protected Task<bool> RemoveAsync<T>(T entity) where T : class, IProjection, IRemoveProjection, IWithVersion
			=> TryAsync(
				c => c.DeleteAsync(DocumentPath<T>.Id(entity.HasNotNullArg(x => x.Id, nameof(entity))), x => x
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Version(entity.Version)
					.Refresh(Refresh.True)),
				r => r.Found,
				RepositoryLoggingEvents.ES_REMOVE,
				$"Remove (Id: {entity.Id})");

		protected int Remove<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query) where T : class, IProjection, IRemoveProjection
			=> Try(
				c => c.DeleteByQuery<T>(d => d.Query(q => q.Bool(b => b.Filter(query)))
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Refresh()),
				r => (int) r.Deleted,
				RepositoryLoggingEvents.ES_REMOVEBYQUERY);

		protected Task<int> RemoveAsync<T>(Func<QueryContainerDescriptor<T>, QueryContainer> query) where T : class, IProjection, IRemoveProjection
			=> TryAsync(
				c => c.DeleteByQueryAsync<T>(d => d.Query(q => q.Bool(b => b.Filter(query)))
					.Index(_mapping.GetIndexName<T>())
					.Type(_mapping.GetTypeName<T>())
					.Refresh()),
				r => (int) r.Deleted,
				RepositoryLoggingEvents.ES_REMOVEBYQUERY);
	}
}