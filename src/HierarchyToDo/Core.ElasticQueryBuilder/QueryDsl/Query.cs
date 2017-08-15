using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.ElasticQueryBuilder.QueryDsl
{
	public class Query<T> : BaseCommandItem
	{
		public Query(Action<object> addParam) : base(HashExtensions.GetHash<T>("Query"), addParam)
		{
		}

		internal override void Build(JsonWriter writer)
			=> BuildHelper(writer, "query");

		public Query<T> Bool(Func<Bool<T>, Bool<T>> func)
		{
			AddItem(func(new Bool<T>()));
			return this;
		}

	}
}