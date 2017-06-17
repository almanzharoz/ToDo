using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	public abstract partial class BaseRepository<TSettings>
	{
		protected Pager<T, TProjection> SearchPager<T, TProjection>(QueryContainer query, int page, int take,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, bool load = true)
			where TProjection : class, IProjection<T>
			where T : class, IEntity
			=> _mapping.GetProjectionItem<TProjection>()
				.Convert(
					projection => Try(
						c => c.Search<T, TProjection>(
							x => x.Type(projection.MappingItem.TypeName)
								.Source(s => s.Includes(f => f.Fields(projection.Fields)))
								.Query(q => q.Bool(b => b.Filter(query)))
								.IfNotNull(sort, y => y.Sort(sort))
								.If(y => typeof(TProjection).GetInterfaces().Any(z => z == typeof(IWithVersion)), y => y.Version())
								.IfNotNull(take, y => y.Take(take).Skip(page*take))),
						r => new Pager<T, TProjection>(page, take, (int) r.Total, r.Documents.If(load, Load)),
						RepositoryLoggingEvents.ES_SEARCH));

		protected Pager<T, TProjection> SearchPager<T, TProjection>(Func<QueryContainerDescriptor<T>, QueryContainer> query, int page, int take,
			Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, bool load = true)
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
								.IfNotNull(take, y => y.Take(take).Skip(page*take))),
						r => new Pager<T, TProjection>(page, take, (int)r.Total, r.Documents.If(load, Load)),
						RepositoryLoggingEvents.ES_SEARCH));
	}
}