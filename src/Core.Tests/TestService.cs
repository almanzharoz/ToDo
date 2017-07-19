using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.ElasticSearch;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Mapping;
using Core.Tests.Models;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace Core.Tests
{
    public class TestService : BaseService<ElasticSettings>
    {
        public TestService(ILoggerFactory loggerFactory, ElasticSettings settings, ElasticMapping<ElasticSettings> mapping, RequestContainer<ElasticSettings> container) : base(loggerFactory, settings, mapping, container)
        {
        }

        public void Clean() => _client.DeleteIndex("*");
        
        public new Task<IReadOnlyCollection<TProjection>> SearchAsync<T, TProjection>(QueryContainer query, int take = 0,
            int skip = 0, bool load = true)
            where TProjection : class, IProjection<T>
            where T : class, IEntity
            => base.SearchAsync<T, TProjection>(query, take, skip, load);

        public new IReadOnlyCollection<TProjection> Search<T, TProjection>(QueryContainer query,
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, int take = 0, int skip = 0, bool load = true)
            where TProjection : class, IProjection<T>
            where T : class, IEntity
        {
    //        Debug.WriteLine(base._client.Search<T, TProjection>(x => x.Type(typeof(TProjection).Name)
    //.Source(s => s.Includes(f => f.Fields(typeof(TProjection).GetFields().Select(fh => fh.Name).ToArray())))
    //.Query(q => q.Bool(b => b.Filter(query)))
    //.IfNotNull(sort, y => y.Sort(sort))
    //.If(y => typeof(TProjection).GetInterfaces().Any(z => z == typeof(IWithVersion)), y => y.Version())
    //.IfNotNull(take, y => y.Take(take).Skip(skip)))
    //            .DebugInformation);
            return base.Search<T, TProjection>(query, sort, take, skip, load);
        }

        public new IReadOnlyCollection<TProjection> Search<T, TProjection>(
            Func<QueryContainerDescriptor<T>, QueryContainer> query,
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, int page = 0, int take = 0, bool load = true)
            where TProjection : class, IProjection<T>
            where T : class, IEntity
            => base.Search<T, TProjection>(query, sort, page, take, load);

        public new TProjection Get<T, TProjection>(string id, bool load = true)
            where TProjection : class, IProjection<T>
            where T : class, IEntity
            => base.Get<T, TProjection>(id, load);

        public new TProjection Get<T, TProjection, TParent, TParentProjection>(string id, string parent, bool load = true)
            where TProjection : class, IProjection<T>
            where T : class, IEntity, IWithParent<TParent, TParentProjection>
            where TParent : class, IEntity
            where TParentProjection : IProjection<TParent>

        {
            return base.Get<T, TProjection, TParent, TParentProjection>(id, parent, load);
        }

        public new Task<TProjection> GetAsync<T, TProjection>(string id, bool load = true)
            where TProjection : class, IProjection<T>
            where T : class, IEntity
            => base.GetAsync<T, TProjection>(id, load);

        public new int Count<T>(QueryContainer query) where T : class, IEntity
            => base.Count<T>(query);

        public new Task<int> CountAsync<T>(QueryContainer query) where T : class, IEntity
            => base.CountAsync<T>(query);

        public new bool Insert<T>(T entity, bool refresh) where T : class, IEntity
            => base.Insert<T>(entity, refresh);

        public new bool Insert<T, TParentModel, TParentProjection>(T entity, bool refresh)
            where T : class, IEntity, IWithParent<TParentModel, TParentProjection>
            where TParentModel : class, IEntity
            where TParentProjection : IProjection<TParentModel>
            => base.Insert<T, TParentModel, TParentProjection>(entity, refresh);

        public new Task<bool> InsertAsync<T>(T entity, bool refresh = true) where T : class, IEntity
            => base.InsertAsync<T>(entity, refresh);

        public new bool Update<T>(T entity, bool refresh) where T : class, IEntity, IWithVersion
            => base.Update<T>(entity, refresh);

        public new Task<bool> UpdateAsync<T>(T entity, bool refresh = true) where T : class, IEntity, IWithVersion
            => base.UpdateAsync<T>(entity, refresh);

        public new int Update<T>(QueryContainer query, UpdateByQueryBuilder<T> update, bool refresh = true)
            where T : class, IEntity, IWithVersion
            => base.Update<T>(query, update, refresh);

        public new Task<(int updated, int total)> UpdateAsync<T>(QueryContainer query, UpdateByQueryBuilder<T> update,
            bool refresh = true)
            where T : class, IEntity, IWithVersion
            => base.UpdateAsync<T>(query, update, refresh);

        public new bool Remove<T>(T entity) where T : class, IEntity, IWithVersion
            => base.Remove<T>(entity);

        public new Task<bool> RemoveAsync<T>(T entity) where T : class, IEntity, IWithVersion
            => base.RemoveAsync<T>(entity);

        public new int Remove<T>(QueryContainer query) where T : class, IEntity
            => base.Remove<T>(query);

        public new Task<int> RemoveAsync<T>(QueryContainer query) where T : class, IEntity
            => base.RemoveAsync<T>(query);

        public new Pager<T, TProjection> SearchPager<T, TProjection>(QueryContainer query, int page, int take,
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, bool load = true)
            where TProjection : class, IProjection<T>
            where T : class, IEntity
            => base.SearchPager<T, TProjection>(query, page, take, sort, load);

        public new Pager<T, TProjection> SearchPager<T, TProjection>(
            Func<QueryContainerDescriptor<T>, QueryContainer> query, int page, int take,
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, bool load = true)
            where TProjection : class, IProjection<T>
            where T : class, IEntity
            => base.SearchPager<T, TProjection>(query, page, take, sort, load);
    }
}