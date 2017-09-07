using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Serialization;
using Nest;
using Newtonsoft.Json;
using SharpFuncExt;

namespace Core.ElasticSearch.Mapping
{
	internal interface IProjectionItem
	{
		string[] Fields { get; }
		PropertyInfo[] Properties { get; }
		IMappingItem MappingItem { get; }
		JsonConverter GetJsonConverter();
	}

	internal abstract class BaseProjectionItem<T, TMapping, TSettings> : IProjectionItem
		where T : class, IProjection, IProjection<TMapping>
		where TMapping : class, IModel
		where TSettings : BaseElasticConnection
	{
		protected BaseProjectionItem(MappingItem<TMapping, TSettings> mappingItem)
		{
			MappingItem = mappingItem;
			Fields = typeof(T).GetFieldsNames();
			Properties = typeof(T).GetProperties().ThrowIf(x => x.Any(y => typeof(IProjection).IsAssignableFrom(y.PropertyType) && !typeof(IJoinProjection).IsAssignableFrom(y.PropertyType)), x => new Exception($"Field in \"{typeof(T)}\" not join"));
			var errorFields = mappingItem.CheckFields(Fields);
			if (errorFields.Any())
				throw new Exception($"Not found fields for \"{typeof(T).Name}\": {String.Join(", ", errorFields)}");
		}

		public string[] Fields { get; }
		public PropertyInfo[] Properties { get; }
		public IMappingItem MappingItem { get; }

		public abstract JsonConverter GetJsonConverter();
	}

	internal class ProjectionItem<T, TMapping, TSettings> : BaseProjectionItem<T, TMapping, TSettings>
		where T : class, IProjection, IProjection<TMapping>
		where TMapping : class, IModel
		where TSettings : BaseElasticConnection
	{
		public ProjectionItem(MappingItem<TMapping, TSettings> mappingItem) : base(mappingItem)
		{
		}

		public override JsonConverter GetJsonConverter()
			=> new ClassJsonConverter<T>(this);
	}

	internal class JoinProjectionItem<T, TMapping, TSettings> : BaseProjectionItem<T, TMapping, TSettings>
		where T : class, IProjection, IProjection<TMapping>, IJoinProjection
		where TMapping : class, IModel
		where TSettings : BaseElasticConnection
	{
		public JoinProjectionItem(MappingItem<TMapping, TSettings> mappingItem) : base(mappingItem)
		{
		}

		public override JsonConverter GetJsonConverter()
			=> new JoinConverter<T>();
	}

	//internal class ProjectionWithParentItem<T, TMapping, TParent, TSettings> : BaseProjectionItem<T, TMapping, TSettings>
	//	where T : class, IProjection, IProjection<TMapping>, IWithParent<TParent>
	//	where TMapping : class, IModel
	//	where TParent : class, IProjection
	//	where TSettings : BaseElasticConnection
	//{
	//	public ProjectionWithParentItem(MappingItem<TMapping, TSettings> mappingItem) : base(mappingItem)
	//	{
	//		// TODO: Сделать проверку типа парента
	//	}

	//	public override JsonConverter GetJsonConverter()
	//		=> new ParentJsonConverter<T, TParent>(this);
	//}
}