using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public abstract partial class BaseService<TConnection>
	{
		protected Task<IReadOnlyCollection<T>> SearchAsync<T>(QueryContainer query, int take = 0,
			int skip = 0, bool load = true)
			where T : class, IProjection, ISearchProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => TryAsync(
						c => c.SearchAsync<T>(
							x => x
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))
								.Query(q => q.Bool(b => b.Filter(query)))
								.IfNotNull(take, y => y.Take(take).Skip(skip))),
						r => r.Documents.IfAsync(load, LoadAsync),
						RepositoryLoggingEvents.ES_SEARCH));

		private Stopwatch sw2 = new Stopwatch();

		protected IReadOnlyCollection<TProjection> Search<T, TProjection>(QueryContainer query,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, int take = 0, int skip = 0, bool load = true)
			where TProjection : class, IProjection<T>, ISearchProjection
			where T : class, IModel
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Stopwatch(sw2, c1 => c1.Search<T, TProjection>(
								x => x
									.Index(projection.MappingItem.IndexName)
									.Type(projection.MappingItem.TypeName)
									.Source(s => s.Includes(f => f.Fields(projection.Fields)))
									.Query(q => q.Bool(b => b.Filter(query)))
									.IfNotNull(sort, y => y.Sort(sort))
									.If(y => typeof(IWithVersion).IsAssignableFrom(typeof(TProjection)), y => y.Version())
									.IfNotNull(take, y => y.Take(take).Skip(skip)))
							)
							.Fluent(() => Debug.WriteLine("Search: " + sw2.ElapsedMilliseconds)),
						r => r.Documents.If(load, Load),
						RepositoryLoggingEvents.ES_SEARCH));

		protected IReadOnlyCollection<TProjection> Search<T, TProjection>(
			Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, int page = 0, int take = 0, bool load = true)
			where TProjection : class, IProjection<T>, ISearchProjection
			where T : class, IModel
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Search<T, TProjection>(
							x => x
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))
								.Query(query)
								.IfNotNull(sort, y => y.Sort(sort))
								.If(y => typeof(IWithVersion).IsAssignableFrom(typeof(TProjection)), y => y.Version())
								.IfNotNull(take, y => y.Take(take).Skip(page * take))),
						r => r.Documents.If(load, Load),
						RepositoryLoggingEvents.ES_SEARCH));

		protected IReadOnlyCollection<KeyValuePair<TProjection, int>> SearchWithScore<T, TProjection>(
			Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, int page = 0, int take = 0, bool load = true)
			where TProjection : class, IProjection<T>, ISearchProjection
			where T : class, IModel
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Search<T, TProjection>(
							x => x
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))
								.Query(query)
								.IfNotNull(sort, y => y.Sort(sort))
								.If(y => typeof(IWithVersion).IsAssignableFrom(typeof(TProjection)), y => y.Version())
								.IfNotNull(take, y => y.Take(take).Skip(page * take))),
						r => r.Hits.Select(x => new KeyValuePair<TProjection, int>(x.Source, (int)x.Score)).ToArray().If(load, Load),
						RepositoryLoggingEvents.ES_SEARCH));

		protected T Get<T>(string id, bool load = true)
			where T : class, IProjection, IGetProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Get(
							DocumentPath<T>.Id(new Id(id.HasNotNullArg(nameof(id))))
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName),
							x => x.SourceInclude(projection.Fields)),
						r => r.Source.If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id})"));

		protected T Get<T, TParent>(string id, string parent, bool load = true)
			where T : class, IProjection, IGetProjection, IWithParent<TParent>
			where TParent : class, IProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Get(
							DocumentPath<T>.Id(new Id(id.HasNotNullArg(nameof(id))))
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName),
							x => x.Parent(parent).SourceInclude(projection.Fields)),
						r => r.Source.If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id})"));

		protected T Get<T>(string id, QueryContainer query, bool load = true)
			where T : class, IProjection, IGetProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Search<T>(x => x
							.Index(projection.MappingItem.IndexName)
							.Type(projection.MappingItem.TypeName)
							.Query(q => q.Bool(b => b.Filter(Query<T>.Ids(i => i.Values(id.HasNotNullArg("id"))) && query)))
							.Take(1)
							.If(y => typeof(IWithVersion).IsAssignableFrom(typeof(T)), y => y.Version())
							.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
						r => r.Documents.FirstOrDefault().If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get with query (Id: {id})"));

		protected T Get<T, TParent>(string id, string parent, QueryContainer query, bool load = true)
			where T : class, IProjection, IGetProjection, IWithParent<TParent>
			where TParent : class, IProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Search<T>(x => x
							.Index(projection.MappingItem.IndexName)
							.Type(projection.MappingItem.TypeName)
							.Query(q => q.Bool(b => b.Filter(Query<T>.Ids(i => i.Values(id.HasNotNullArg("id"))) &&
															Query<T>.ParentId(p => p.Id(parent.HasNotNullArg("parent"))) && query)))
							.Take(1)
							.If(y => typeof(IWithVersion).IsAssignableFrom(typeof(T)), y => y.Version())
							.Source(s => s.Includes(f => f.Fields(projection.Fields)))),
						r => r.Documents.FirstOrDefault().If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get with query (Id: {id}, Parent: {parent})"));

		protected Task<T> GetAsync<T>(string id, bool load = true)
			where T : class, IProjection, IGetProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => TryAsync(
						c => c.GetAsync(
							DocumentPath<T>.Id(new Id(id.HasNotNullArg(nameof(id))))
								.Index(projection.MappingItem.IndexName)
								.Type(projection.MappingItem.TypeName),
							x => x.SourceInclude(projection.Fields)),
						r => r.Source // загруженный объект
							.IfAsync(load, LoadAsync), // загрузка полей
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id})"));

		protected int Count<T>(QueryContainer query) where T : class, IProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => Try(
						c => c.Count<T>(d => d.Query(q => q.Bool(b => b.Filter(query)))
							.Index(projection.MappingItem.IndexName)
							.Type(projection.MappingItem.TypeName)),
						r => (int) r.Count,
						RepositoryLoggingEvents.ES_COUNT));

		protected Task<int> CountAsync<T>(QueryContainer query) where T : class, IProjection
			=> _mapping.GetProjectionItem<T>()
				.Convert(
					projection => TryAsync(
						c => c.CountAsync<T>(d => d.Query(q => q.Bool(b => b.Filter(query)))
							.Index(projection.MappingItem.IndexName)
							.Type(projection.MappingItem.TypeName)),
						r => (int) r.Count,
						RepositoryLoggingEvents.ES_COUNT));

	}
}