using System;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace Core.ElasticQueryBuilder.QueryDsl
{
	public class Term<T, TValue> : BaseCommandItem
	{
		private Expression<Func<T, TValue>> _field;
		public Term(Expression<Func<T, TValue>> fieldExpression, TValue value, Action<object> addParam) : base("Term", addParam)
		{
			_field = fieldExpression;
			addParam(value);
		}

		internal override void Build(JsonWriter writer)
			=> BuildHelper(writer, "term", w => w.FieldJson(_field));

	}
}