using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Core.ElasticQueryBuilder
{
	public static class HashExtensions
	{
		public static string GetHash<T>(string baseHash)
			=> String.Concat(baseHash, "<", GetTypeName<T>(), ">");

		private static string GetTypeName<T>()
			=> GetTypeName(typeof(T));

		private static string GetTypeName(Type type)
		{
			var typeInfo = type.GetTypeInfo();
			var result = new StringBuilder(type.Name);
			if (typeInfo.IsGenericType)
			{
				result.Append("<");
				bool isFirst = true;
				foreach (var arg in type.GenericTypeArguments)
				{
					if (!isFirst)
						result.Append(",");
					else
						isFirst = false;
					result.Append(GetTypeName(arg));
				}
				result.Append(">");
			}
			return result.ToString();
		}

		public static void FieldJson<T, TValue>(this JsonWriter writer, Expression<Func<T, TValue>> fieldExpression)
		{
			var s = fieldExpression.Body.ToString();
			s = s.Substring(s.IndexOf(".") + 1);
			writer.WritePropertyName(s);
			writer.WriteValue("param");
		}
	}
}