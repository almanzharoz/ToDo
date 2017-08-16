using System;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace Core.ElasticQueryBuilder.QueryDsl
{
	public class Bool<T> : BaseCommandItem
	{
		public Bool(Action<object> addParam) : base("Bool", addParam)
		{
		}

		internal override void Build(JsonWriter writer)
			=> BuildHelper(writer, "bool");

		public Bool<T> Term<TValue>(Expression<Func<T, TValue>> fieldExpression, TValue value)
		{
			AddItem(new Term<T, TValue>(fieldExpression, value, _addParam));
			
			return this;
		}
	}
}