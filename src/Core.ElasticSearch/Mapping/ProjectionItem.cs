using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Serialization;
using Newtonsoft.Json;

namespace Core.ElasticSearch.Mapping
{
	internal interface IProjectionItem
	{
		string[] Fields { get; }
		IMappingItem MappingItem { get; }
		JsonConverter GetJsonConverter(IRequestContainer container);
	}

	internal class ProjectionItem<T, TMapping> : IProjectionItem 
		where T : class, IProjection<TMapping>, new() 
		where TMapping : class, IEntity
	{
		public ProjectionItem(MappingItem<TMapping> mappingItem)
		{
			MappingItem = mappingItem;
			Fields = typeof(T).GetFields();
			var errorFields = mappingItem.CheckFields(Fields);
			if (errorFields.Any())
				throw new Exception($"Not found fields for \"{typeof(T).Name}\": {String.Join(", ", errorFields)}");
		}

		public string[] Fields { get; }
		public IMappingItem MappingItem { get; }

		public JsonConverter GetJsonConverter(IRequestContainer container)
			=> new ClassJsonConverter<T>(container);
	}

	internal class ProjectionWithParentItem<T, TMapping, TParentModel, TParentProjection> : IProjectionItem
		where T : class, IProjection<TMapping>, IWithParent<TParentModel, TParentProjection>, new()
		where TMapping : class, IEntity
		where TParentModel : class, IEntity, new()
		where TParentProjection : IProjection<TParentModel>, new()
	{
		public ProjectionWithParentItem(MappingItem<TMapping> mappingItem)
		{
			MappingItem = mappingItem;
			Fields = typeof(T).GetFields();
			var errorFields = mappingItem.CheckFields(Fields);
			if (errorFields.Any())
				throw new Exception($"Not found fields for \"{typeof(T).Name}\": {String.Join(", ", errorFields)}");
		}

		public string[] Fields { get; }
		public IMappingItem MappingItem { get; }

		public JsonConverter GetJsonConverter(IRequestContainer container)
			=> new ParentJsonConverter<T, TParentModel, TParentProjection>(container);
	}
}