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
    public class TestService : BaseService<ElasticConnection>
    {
        public TestService(ILoggerFactory loggerFactory, ElasticConnection settings, ElasticScopeFactory<ElasticConnection> factory) : 
			base(loggerFactory, settings, factory)
        {
        }

        public void Clean() => _client.DeleteIndex("*");
        
        public new Task<IReadOnlyCollection<T>> SearchAsync<T>(QueryContainer query, int take = 0,
            int skip = 0, bool load = true)
            where T : class, IProjection, ISearchProjection
            => base.SearchAsync<T>(query, take, skip, load);

	    public new IReadOnlyCollection<TProjection> Search<T, TProjection>(QueryContainer query,
		    Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, int take = 0, int skip = 0, bool load = true)
		    where TProjection : class, IProjection<T>, ISearchProjection
			where T : class, IModel
		    => base.Search<T, TProjection>(query, sort, take, skip, load);
    
        public new IReadOnlyCollection<TProjection> Search<T, TProjection>(
            Func<QueryContainerDescriptor<T>, QueryContainer> query,
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, int page = 0, int take = 0, bool load = true)
            where TProjection : class, IProjection<T>, ISearchProjection
			where T : class, IModel
            => base.Search<T, TProjection>(query, sort, page, take, load);

        public new T Get<T>(string id, bool load = true)
            where T : class, IProjection, IGetProjection
            => base.Get<T>(id, load);

	    public new T Get<T, TParent>(string id, string parent, bool load = true)
		    where T : class, IProjection, IGetProjection, IWithParent<TParent>
		    where TParent : class, IProjection
		    => base.Get<T, TParent>(id, parent, load);

        public new Task<T> GetAsync<T>(string id, bool load = true)
            where T : class, IProjection, IGetProjection
			=> base.GetAsync<T>(id, load);

        public new int Count<T>(QueryContainer query) where T : class, IProjection
            => base.Count<T>(query);

        public new Task<int> CountAsync<T>(QueryContainer query) where T : class, IProjection
			=> base.CountAsync<T>(query);

        public new bool Insert<T>(T entity, bool refresh) where T : BaseEntity, IProjection, IInsertProjection
			=> base.Insert<T>(entity, refresh);

        public new bool Insert<T, TParent>(T entity, bool refresh)
            where T : BaseEntity, IWithParent<TParent>, IProjection, IInsertProjection
			where TParent : IProjection
            => base.Insert<T, TParent>(entity, refresh);

        public new Task<bool> InsertAsync<T>(T entity, bool refresh = true) where T : BaseEntity, IProjection, IInsertProjection
            => base.InsertAsync<T>(entity, refresh);

        public new bool Update<T>(T entity, bool refresh) where T : BaseEntityWithVersion, IProjection, IWithVersion, IUpdateProjection
			=> base.Update<T>(entity, refresh);

        public new Task<bool> UpdateAsync<T>(T entity, bool refresh = true) where T : BaseEntityWithVersion, IProjection, IWithVersion, IUpdateProjection
			=> base.UpdateAsync<T>(entity, refresh);

        public new int Update<T>(QueryContainer query, UpdateByQueryBuilder<T> update, bool refresh = true)
            where T : class, IEntity, IWithVersion, IProjection, IUpdateProjection
			=> base.Update<T>(query, update, refresh);

        public new Task<(int updated, int total)> UpdateAsync<T>(QueryContainer query, UpdateByQueryBuilder<T> update,
            bool refresh = true)
            where T : class, IEntity, IWithVersion, IProjection, IUpdateProjection
            => base.UpdateAsync<T>(query, update, refresh);

        public new bool Remove<T>(T entity) 
			where T : class, IProjection, IWithVersion, IRemoveProjection
			=> base.Remove<T>(entity);

        public new Task<bool> RemoveAsync<T>(T entity) where T : class, IProjection, IWithVersion, IRemoveProjection
			=> base.RemoveAsync<T>(entity);

        public new int Remove<T>(QueryContainer query) where T : class, IProjection, IRemoveProjection
			=> base.Remove<T>(query);

        public new Task<int> RemoveAsync<T>(QueryContainer query) where T : class, IProjection, IRemoveProjection
			=> base.RemoveAsync<T>(query);

        public new Pager<TProjection> SearchPager<T, TProjection>(QueryContainer query, int page, int take,
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, bool load = true)
            where TProjection : class, IProjection<T>, ISearchProjection
			where T : class, IModel
            => base.SearchPager<T, TProjection>(query, page, take, sort, load);

        public new Pager<TProjection> SearchPager<T, TProjection>(
            Func<QueryContainerDescriptor<T>, QueryContainer> query, int page, int take,
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, bool load = true)
            where TProjection : class, IProjection<T>, ISearchProjection
			where T : class, IModel
            => base.SearchPager<T, TProjection>(query, page, take, sort, load);
    }
}