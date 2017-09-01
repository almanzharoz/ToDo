using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SharpFuncExt;

namespace Core.ElasticSearch.Serialization
{
	public class CoreElasticContractResolver : ElasticContractResolver
	{
		public CoreElasticContractResolver(IConnectionSettingsValues connectionSettings, IList<Func<Type, JsonConverter>> contractConverters) : base(connectionSettings, contractConverters)
		{
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var p = base.CreateProperty(member, memberSerialization);
			if (!p.Writable)
			{
				var property = member as PropertyInfo;
				if (property != null)
				{
					var hasPrivateSetter = property.GetSetMethod(true) != null;
					p.Writable = hasPrivateSetter;
				}
			}
			return p;
		}

		private static readonly ConcurrentDictionary<Type, JsonContract> Cache = new ConcurrentDictionary<Type, JsonContract>();

		public override JsonContract ResolveContract(Type type)
		{
			var t = typeof(IEntity).IsAssignableFrom(type);
			var t2 = type.IsGenericType && typeof(IEntity).IsAssignableFrom(type.GenericTypeArguments.First());
			// TODO: do refactoring
			return t || t2
				? CreateContract(type)
				: Cache.GetOrAdd(type.HasNotNullArg(nameof(type)), CreateContract);
		}
	}
}