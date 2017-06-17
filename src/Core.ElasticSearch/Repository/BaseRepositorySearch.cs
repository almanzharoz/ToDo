﻿using System;
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
	public abstract partial class BaseRepository<TSettings>
	{
		protected Task<IReadOnlyCollection<TProjection>> SearchAsync<T, TProjection>(QueryContainer query, int take = 0,
			int skip = 0, bool load = true)
			where TProjection : class, IProjection<T>
			where T : class, IEntity
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => TryAsync(
						c => c.SearchAsync<TProjection>(
							x => x.Type(projection.MappingItem.TypeName)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))
								.Query(q => q.Bool(b => b.Filter(query)))
								.IfNotNull(take, y => y.Take(take).Skip(skip))),
						r => r.Documents.IfAsync(load, LoadAsync),
						RepositoryLoggingEvents.ES_SEARCH));

		private Stopwatch sw2 = new Stopwatch();
		protected IReadOnlyCollection<TProjection> Search<T, TProjection>(QueryContainer query,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, int take = 0, int skip = 0, bool load = true)
			where TProjection : class, IProjection<T>
			where T : class, IEntity
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Stopwatch(sw2, c1 => c1.Search<T, TProjection>(
							x => x.Type(projection.MappingItem.TypeName)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))
								.Query(q => q.Bool(b => b.Filter(query)))
								.IfNotNull(sort, y => y.Sort(sort))
								.If(y => typeof(TProjection).GetInterfaces().Any(z => z == typeof(IWithVersion)), y => y.Version())
								.IfNotNull(take, y => y.Take(take).Skip(skip)))
						).Fluent(() => Debug.WriteLine("Search: " + sw2.ElapsedMilliseconds)),
						r => r.Documents.If(load, Load),
						RepositoryLoggingEvents.ES_SEARCH));

		protected IReadOnlyCollection<TProjection> Search<T, TProjection>(Func<QueryContainerDescriptor<T>, QueryContainer> query,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, int page = 0, int take = 0, bool load = true)
			where TProjection : class, IProjection<T>
			where T : class, IEntity
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Search<T, TProjection>(
							x => x.Type(projection.MappingItem.TypeName)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))
								.Query(query)
								.IfNotNull(sort, y => y.Sort(sort))
								.If(y => typeof(TProjection).GetInterfaces().Any(z => z == typeof(IWithVersion)), y => y.Version())
								.IfNotNull(take, y => y.Take(take).Skip(page * take))),
						r => r.Documents.If(load, Load),
						RepositoryLoggingEvents.ES_SEARCH));

		protected TProjection Get<T, TProjection>(string id, bool load = true)
			where TProjection : class, IProjection<T>
			where T : class, IEntity
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Get(
							DocumentPath<TProjection>.Id(new Id(id.HasNotNullArg(nameof(id)))).Type(projection.MappingItem.TypeName),
							x => x.SourceInclude(projection.Fields)),
						r => r.Source.If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id})"));

		protected TProjection Get<T, TProjection, TParent, TParentProjection>(string id, string parent, bool load = true)
			where TProjection : class, IProjection<T>
			where T : class, IEntity, IWithParent<TParent, TParentProjection>
			where TParent : class, IEntity
			where TParentProjection : IProjection<TParent>
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Get(
							DocumentPath<TProjection>.Id(new Id(id.HasNotNullArg(nameof(id)))).Type(projection.MappingItem.TypeName),
							x => x.Parent(parent).SourceInclude(projection.Fields)),
						r => r.Source.If(load, Load),
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id})"));

		protected Task<TProjection> GetAsync<T, TProjection>(string id, bool load = true)
			where TProjection : class, IProjection<T>
			where T : class, IEntity
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => TryAsync(
						c => c.GetAsync(
							DocumentPath<TProjection>.Id(new Id(id.HasNotNullArg(nameof(id)))).Type(projection.MappingItem.TypeName),
							x => x.SourceInclude(projection.Fields)),
						r => r.Source // загруженный объект
							.IfAsync(load, LoadAsync), // загрузка полей
						RepositoryLoggingEvents.ES_GET,
						$"Get (Id: {id})"));

		protected int Count<T>(QueryContainer query) where T : class, IEntity
			=> Try(
				c => c.Count<T>(d => d.Query(q => q.Bool(b => b.Filter(query)))),
				r => (int)r.Count,
				RepositoryLoggingEvents.ES_COUNT);

		protected Task<int> CountAsync<T>(QueryContainer query) where T : class, IEntity
			=> TryAsync(
				c => c.CountAsync<T>(d => d.Query(q => q.Bool(b => b.Filter(query)))),
				r => (int)r.Count,
				RepositoryLoggingEvents.ES_COUNT);

	}
}