using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SharpFuncExt;

namespace Core.ElasticSearch.Serialization
{
	internal class CoreElasticContractResolver : ElasticContractResolver
	{
		public IRequestContainer Container { get; }
		public CoreElasticContractResolver(IConnectionSettingsValues connectionSettings, IList<Func<Type, JsonConverter>> contractConverters, IRequestContainer container) : base(connectionSettings, contractConverters)
		{
			Container = container;
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
			//return Cache.GetOrAdd(type.HasNotNullArg(nameof(type)), CreateContract);
			
			var t = typeof(IEntity).IsAssignableFrom(type);
			var t2 = type.IsGenericType && typeof(IEntity).IsAssignableFrom(type.GenericTypeArguments.First());
			var result = Cache.GetOrAdd(type.HasNotNullArg(nameof(type)), CreateContract);
			return result;
			if (t || t2)
			{
				var c = (JsonObjectContract) result;
				var newResult = new JsonObjectContract(type);
				newResult.CreatedType = result.CreatedType;
				newResult.ExtensionDataGetter = c.ExtensionDataGetter;
				newResult.ExtensionDataNameResolver = c.ExtensionDataNameResolver;
				newResult.ExtensionDataSetter = c.ExtensionDataSetter;
				newResult.ExtensionDataValueType = c.ExtensionDataValueType;
				newResult.ItemRequired = c.ItemRequired;
				newResult.MemberSerialization = c.MemberSerialization;
				newResult.OverrideCreator = c.OverrideCreator;
				newResult.DefaultCreator = c.DefaultCreator;
				newResult.DefaultCreatorNonPublic = c.DefaultCreatorNonPublic;
				newResult.IsReference = c.IsReference;
				newResult.ItemConverter = c.ItemConverter;
				newResult.ItemRequired = c.ItemRequired;
				newResult.ItemIsReference = c.ItemIsReference;
				newResult.ItemReferenceLoopHandling = c.ItemReferenceLoopHandling;
				newResult.ItemTypeNameHandling = c.ItemTypeNameHandling;
				foreach (var p in c.CreatorParameters)
					newResult.CreatorParameters.Add(p);
				foreach (var p in c.Properties)
					newResult.Properties.Add(p);
				newResult.Converter = result.Converter;
				return newResult;
			}
			return result;
			// TODO: do refactoring
			return t || t2
				? CreateContract(type)
				: Cache.GetOrAdd(type.HasNotNullArg(nameof(type)), CreateContract);
		}

		protected override JsonConverter ResolveContractConverter(Type objectType)
		{
			var sw = new Stopwatch();
			sw.Start();
			var result = base.ResolveContractConverter(objectType);
			sw.Stop();
			Debug.WriteLine($"ResolveContractConverter<{objectType.Name}> " + sw.ElapsedMilliseconds);
			return result;
		}

		protected override JsonObjectContract CreateObjectContract(Type objectType)
		{
			var sw = new Stopwatch();
			sw.Start();
			var result = base.CreateObjectContract(objectType);
			sw.Stop();
			Debug.WriteLine($"CreateObjectContract<{objectType.Name}> "+sw.ElapsedMilliseconds);
			return result;
		}
	}
}