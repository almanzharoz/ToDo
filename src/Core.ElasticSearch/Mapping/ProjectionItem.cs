using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Serialization;
using Nest;
using Newtonsoft.Json;

namespace Core.ElasticSearch.Mapping
{
	internal interface IProjectionItem
	{
		string[] Fields { get; }
		PropertyInfo[] Properties { get; }
		IMappingItem MappingItem { get; }
		JsonConverter GetJsonConverter(IRequestContainer container);
	}

	internal abstract class BaseProjectionItem<T, TMapping, TSettings> : IProjectionItem
		where T : BaseEntity, IProjection<TMapping>, new()
		where TMapping : class, IModel
		where TSettings : BaseElasticSettings
	{
		protected BaseProjectionItem(MappingItem<TMapping, TSettings> mappingItem)
		{
			MappingItem = mappingItem;
			Fields = typeof(T).GetFields();
			Properties = typeof(T).GetProperties();
			var errorFields = mappingItem.CheckFields(Fields);
			if (errorFields.Any())
				throw new Exception($"Not found fields for \"{typeof(T).Name}\": {String.Join(", ", errorFields)}");
		}

		public string[] Fields { get; }
		public PropertyInfo[] Properties { get; }
		public IMappingItem MappingItem { get; }

		public abstract JsonConverter GetJsonConverter(IRequestContainer container);
	}

	internal class ProjectionItem<T, TMapping, TSettings> : BaseProjectionItem<T, TMapping, TSettings>
		where T : BaseEntity, IProjection<TMapping>, new() 
		where TMapping : class, IModel
		where TSettings : BaseElasticSettings
	{
		public ProjectionItem(MappingItem<TMapping, TSettings> mappingItem) : base(mappingItem)
		{
		}

		public override JsonConverter GetJsonConverter(IRequestContainer container)
			=> new ClassJsonConverter<T>(this, container);
	}

	internal class ProjectionWithParentItem<T, TMapping, TParent, TSettings> : BaseProjectionItem<T, TMapping, TSettings>
		where T : BaseEntity, IProjection<TMapping>, IWithParent<TParent>, new()
		where TMapping : class, IModel
		where TParent : BaseEntity, IProjection, new()
		where TSettings : BaseElasticSettings
	{
		public ProjectionWithParentItem(MappingItem<TMapping, TSettings> mappingItem) : base(mappingItem)
		{
			// TODO: Сделать проверку типа парента
		}

		public override JsonConverter GetJsonConverter(IRequestContainer container)
			=> new ParentJsonConverter<T, TParent>(container);
	}
}