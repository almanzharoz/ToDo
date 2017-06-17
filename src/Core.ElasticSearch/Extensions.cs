using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Serialization;
using Nest;

namespace Core.ElasticSearch
{
	internal static class Extensions
	{
		public static bool HasBaseType<T>(this Type type)
		{
			return type.HasBaseType(typeof(T));
		}
		public static bool HasBaseType(this Type type, Type basetype)
		{
			if (basetype.GetTypeInfo().IsInterface)
				return type.GetInterfaces().Any(x => x == basetype);
			Type baseType = type.GetTypeInfo().BaseType;
			while (baseType != null)
			{
				if (baseType == basetype)
					return true;
				baseType = baseType.GetTypeInfo().BaseType;
			}
			return false;
		}

		public static string[] GetFields(this Type type, string lastName = null)
		{
			var fields = new List<string>();
			foreach (var property in type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name != "Id" && x.Name != "Version" && x.Name != "Parent"))
			{
				if (property.PropertyType.HasBaseType<IEntity>() && property.GetCustomAttribute<KeywordAttribute>() == null)
					fields.AddRange(GetFields(property.PropertyType, String.Join(".", new[] { lastName, property.Name.ToLower() }.Where(x => x != null))));
				else
					fields.Add(String.Join(".", new[] { lastName, property.Name.ToLower() }.Where(x => x != null)));
			}
			return fields.ToArray();
		}
	}
}