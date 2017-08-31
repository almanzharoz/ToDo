using System;
using System.Collections.Generic;
using System.Reflection;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
	}
}