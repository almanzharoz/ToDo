using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Core.ElasticSearch.Domain;

namespace Core.ElasticSearch
{
    public class Pager<TModel, T> : IReadOnlyCollection<T>
		where T : IProjection<TModel>
		where TModel : class, IEntity
    {
	    private readonly IReadOnlyCollection<T> _items;
		public int Page { get; }
		public int Limit { get; }
	    public int Count { get; }

		public Pager(int page, int limit, int count, IReadOnlyCollection<T> items)
		{
			Page = page < 1 ? 1 : page;
		    Limit = limit;
		    Count = count;
		    _items = items;
	    }

	    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

	    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
