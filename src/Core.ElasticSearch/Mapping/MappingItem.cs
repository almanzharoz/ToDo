using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch.Mapping
{
	internal interface IMappingItem
	{
		string TypeName { get; }
		IPutMappingResponse Map(ElasticClient client);
		MappingsDescriptor Map(MappingsDescriptor descriptor);
	}
	internal class MappingItem<T> : IMappingItem where T : class, IEntity
	{
		private readonly IEnumerable<string> _fields;
		public MappingItem()
		{
			_fields = typeof(T).GetFields();
			TypeName = typeof(T).GetTypeInfo().GetCustomAttribute<ElasticsearchTypeAttribute>()?.Name ?? typeof(T).Name.ToLower();
		}

		public IEnumerable<string> CheckFields(IEnumerable<string> fields)
			=> fields.Except(_fields).ToImmutableArray();

		public string TypeName { get; }

		public IPutMappingResponse Map(ElasticClient client) => client.Map<T>(x => x.AutoMap()
			.If(typeof(T).GetTypeInfo().GetInterfaces().Any(z => z.Name.IndexOf("IWithParent") == 0),
				z => z.Parent(typeof(T).GetTypeInfo()
					.GetInterfaces()
					.First(y => y.Name.IndexOf("IWithParent") == 0)
					.GenericTypeArguments.First()
					.Name.ToLower()))); //TODO: доставать имя типа из маппинга

		public MappingsDescriptor Map(MappingsDescriptor descriptor)
			=> descriptor.Map<T>(x => x.AutoMap()
				.If(typeof(T).GetTypeInfo().GetInterfaces().Any(z => z.Name.IndexOf("IWithParent") == 0),
					z => z.Parent(typeof(T).GetTypeInfo()
						.GetInterfaces()
						.First(y => y.Name.IndexOf("IWithParent") == 0)
						.GenericTypeArguments.First()
						.Name.ToLower()))); //TODO: доставать имя типа из маппинга
	}
}