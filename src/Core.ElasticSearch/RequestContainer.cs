using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Mapping;
using SharpFuncExt;

namespace Core.ElasticSearch
{
	internal interface IRequestContainer
	{
		T GetOrAdd<T>(string key, bool load) where T : BaseEntity, new();
		IEntity Get(string key);
		IEnumerable<(string index, IEnumerable<string> types, IEnumerable<string> fields, IEnumerable<string> ids)> PopEntitiesForLoad();
	}
	/// <summary>
	/// Контейнер загруженных объектов и объектов, готовых для загрузки.
	/// </summary>
	/// <typeparam name="TSettings"></typeparam>
	internal class RequestContainer<TSettings> : IRequestContainer 
		where TSettings : BaseElasticSettings
	{
		private readonly ConcurrentDictionary<string, IList<KeyValuePair<IEntity, bool>>> _cache =
			new ConcurrentDictionary<string, IList<KeyValuePair<IEntity, bool>>>();

		private ConcurrentBag<IEntity> _loadBag = new ConcurrentBag<IEntity>();

		private object _locker = new object();
		private readonly ElasticMapping<TSettings> _mapping;

		public RequestContainer(ElasticMapping<TSettings> mapping)
		{
			_mapping = mapping;
		}

		public T GetOrAdd<T>(string key, bool load) where T : BaseEntity, new()
		{
			var type = typeof(T);
			return (T) _cache.AddOrUpdate(key,
					x =>
					{
						var result = new T {Id = key};
						if (load)
							lock (_locker)
								_loadBag.Add(result);
						var list = new List<KeyValuePair<IEntity, bool>>();
						list.Add(new KeyValuePair<IEntity, bool>(result, load));
						return list;
					},
					(k, e) =>
					{
						if (e.All(x => x.Key.GetType() != type))
						{
							var result = new T {Id = key};
							if (load)
								lock (_locker)
									_loadBag.Add(result);
							e.Add(new KeyValuePair<IEntity, bool>(result, load));
						}
						return e;
					})
				.First(x => x.Key.GetType() == type)
				.Key;
		}

		public IEntity Get(string key)
		{
			// Нужная проекция определяется из порядка запроса
			if (_cache.TryGetValue(key, out IList<KeyValuePair<IEntity, bool>> result))
			{
				var res = result.FirstOrDefault(x => x.Value);
				if (res.NotNull())
				{
					result[result.IndexOf(res)] = new KeyValuePair<IEntity, bool>(res.Key, false);
					return res.Key;
				}
			}
			return null;
		}

		public IEnumerable<(string index, IEnumerable<string> types, IEnumerable<string> fields, IEnumerable<string> ids)> PopEntitiesForLoad()
		{
			IEntity[] items;
			lock (_locker)
			{
				items = _loadBag.ToArray();
				_loadBag = new ConcurrentBag<IEntity>();
			}
			// группировать по MappingItem
			var result =
				new List<(string index, IEnumerable<string> types, IEnumerable<string> fields, IEnumerable<string> ids)>();
			foreach (var item in items.GroupBy(x => x.GetType())
				.Select(x => (projection: _mapping.GetProjectionItem(x.Key), ids: x.Select(y => y.Id).ToArray())))
			{
				result.Add((
					index: item.projection.MappingItem.IndexName,
					types: new[] {item.projection.MappingItem.TypeName},
					fields: item.projection.Fields,
					ids: item.ids));
			}
			return result;
		}
	}
}
