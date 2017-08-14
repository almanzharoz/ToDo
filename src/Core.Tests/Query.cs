using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Exporters.Xml;
using Newtonsoft.Json;

namespace Core.Tests
{
	public interface IQuery
	{
		void Build();
	}
	public abstract class BaseQuery<T> : IQuery
	{
		protected readonly Query<T> _query;
		protected readonly List<IQuery> _queries = new List<IQuery>();

		protected BaseQuery(Query<T> query)
		{
			_query = query;
		}

		protected TQuery AddQuery<TQuery, TValue>(TQuery query) where TQuery : BaseQuery<TValue>
		{
			_queries.Add(query);
			return _query.AddQuery<TQuery, TValue>(query);
		}

		public void AddParam(object param)
			=> _query.AddParam(param);

		public abstract void Build();
	}

	public static class QueryFactory
	{
		private static ConcurrentDictionary<string, IQuery> _queries = new ConcurrentDictionary<string, IQuery>();

		public static int Count => _queries.Count;

		public static string GetOrAdd<T>(Func<Query<T>, Query<T>> query)
		{
			var q = query(new Query<T>());
			return ((Query<T>) _queries.GetOrAdd(q.Hash, k =>
			{
				q.Build();
				return q;
			})).CopyParams(q).GetJson();
		}
	}

	public class Query<T> : IDisposable, IQuery
	{
		public string Hash { get; private set; }
		public readonly JsonTextWriter _jsonWriter;
		private readonly TextWriter _writer;
		private readonly MemoryStream _stream;
		private string _result;
		private MatchCollection _matches;
		private Regex _rx = new Regex("\"param\"", RegexOptions.Compiled);

		protected List<object> _params = new List<object>();
		protected List<IQuery> _queries = new List<IQuery>();

		public Query()
		{
			_jsonWriter = new JsonTextWriter(_writer = new StreamWriter(_stream = new MemoryStream(), Encoding.UTF8));
		}

		internal TQuery AddQuery<TQuery, TValue>(TQuery query) where TQuery : BaseQuery<TValue>
		{
			Hash += " " + typeof(TQuery).Name + String.Concat("_", typeof(TValue).Name);
			return query;
		}

		internal TQuery AddQuery<TQuery, TValue, TFieldValue>(TQuery query, Expression<Func<T, TFieldValue>> fieldExpression) where TQuery : BaseQuery<TValue>
		{
			var s = fieldExpression.Body.ToString();
			s = s.Substring(s.IndexOf(".") + 1);
			Hash += " " + typeof(TQuery).Name + String.Concat("_", typeof(TValue).Name, "_", s);
			return query;
		}

		internal void AddParam(object param)
			=> _params.Add(param);

		public Query<T> Bool(Func<BoolQuery<T>, BoolQuery<T>> query)
		{
			var q = AddQuery<BoolQuery<T>, T>(query(new BoolQuery<T>(this)));
			_queries.Add(q);
			return this;
		}

		public void Build()
		{
			_jsonWriter.WriteStartObject();
			foreach (var query in _queries)
				query.Build();
			_jsonWriter.WriteEndObject();

			_jsonWriter.Flush();
			_stream.Position = 0;
			using (var reader = new StreamReader(_stream))
				_result = reader.ReadToEnd();
			_matches = _rx.Matches(_result);
		}

		public string GetJson()
		{
			var i = 0;
			var s = _result;
			foreach (Match match in _matches)
				s = s.Remove(match.Index, match.Length).Insert(match.Index, _params[i++].ToString());
			return s;
		}

		public void Dispose()
		{
			_writer.Dispose();
			_stream.Dispose();
			//_jsonWriter.Dispose();
		}

		public Query<T> CopyParams(Query<T> query)
		{
			_params = query._params;
			return this;
		}
	}

	public class BoolQuery<T> : BaseQuery<T>
	{
		public BoolQuery<T> Filter(Func<FilterQuery<T>, FilterQuery<T>> query)
		{
			AddQuery<FilterQuery<T>, T>(query(new FilterQuery<T>(_query)));
			return this;
		}


		public BoolQuery(Query<T> query) : base(query)
		{
		}

		public override void Build()
		{
			_query._jsonWriter.WritePropertyName("Bool");
			_query._jsonWriter.WriteStartObject();
			foreach (var query in _queries)
				query.Build();
			_query._jsonWriter.WriteEndObject();
		}
	}

	public class FilterQuery<T> : BaseQuery<T>
	{

		public FilterQuery<T> Term<TValue>(Expression<Func<T, TValue>> fieldExpression, TValue value)
		{
			var q = new TermQuery<T, TValue>(_query, fieldExpression, value);
			_queries.Add(q);
			_query.AddQuery<TermQuery<T, TValue>, T>(q);
			return this;
		}

		public FilterQuery(Query<T> query) : base(query)
		{
		}

		public override void Build()
		{
			_query._jsonWriter.WritePropertyName("Filter");
			_query._jsonWriter.WriteStartObject();
			foreach (var query in _queries)
				query.Build();
			_query._jsonWriter.WriteEndObject();
		}
	}

	public class TermQuery<T, TValue> : BaseQuery<T>
	{
		internal TermQuery(Query<T> query, Expression<Func<T, TValue>> fieldExpression, TValue value) : base(query)
		{
			var q = new FieldQuery<T, TValue>(query, fieldExpression, value);
			_query.AddQuery<FieldQuery<T, TValue>, T, TValue>(q, fieldExpression);
			_queries.Add(q);
		}

		public override void Build()
		{
			_query._jsonWriter.WritePropertyName("Term");
			_query._jsonWriter.WriteStartObject();
			foreach (var query in _queries)
				query.Build();
			_query._jsonWriter.WriteEndObject();
		}
	}

	public class FieldQuery<T, TValue> : BaseQuery<T>
	{
		private readonly Expression<Func<T, TValue>> _field;
		public FieldQuery(Query<T> query, Expression<Func<T, TValue>> fieldExpression, TValue value) : base(query)
		{
			_field = fieldExpression;
			AddParam(value);
		}

		public override void Build()
		{
			var s = _field.Body.ToString();
			s = s.Substring(s.IndexOf(".") + 1);
			_query._jsonWriter.WritePropertyName(s);
			_query._jsonWriter.WriteValue("param");
		}
	}
}