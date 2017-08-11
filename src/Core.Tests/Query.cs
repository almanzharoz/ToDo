using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Core.Tests
{
	/// <summary>
	/// Через Fluent-интерфейс строим JSON с определенными параметрами. При дальнейшем использовании запроса, JSON уже не строиться, а заполняются лишь параметры.
	/// Параметры заполняются через тот же Fluent, что и строится при первом использовании JSON
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Query<T>
	{
		private Regex _rx = new Regex("\"param\"", RegexOptions.Compiled);
		private string _result;
		private MemoryStream _stream = new MemoryStream();
		private readonly JsonTextWriter _writer;
		private readonly List<object> _parameters = new List<object>();
		private int _counter=0;
		private MatchCollection _matches;
		private readonly Guid _hash;

		public JsonWriter Json => _writer;

		public bool IsBuild { get; private set; }

		public Query()
		{
			_hash = Guid.NewGuid();
			_writer = new JsonTextWriter(new StreamWriter(_stream));
			_writer.WriteStartObject();
		}

		public void SetNextParam(object value)
		{
			if (_parameters.Count-1 < _counter)
				_parameters.Add(value);
			else
				_parameters[_counter] = value;
			_counter++;
		}

		public Query<T> Bool(Func<BoolQuery<T>, BoolQuery<T>> query)
		{
			_counter = 0;
			query(new BoolQuery<T>(this));
			return this;
		}

		public void Build()
		{
			_writer.WriteEndObject();
			_writer.Flush();
			_stream.Position = 0;
			_result = new StreamReader(_stream).ReadToEnd();
			_matches = _rx.Matches(_result);
			IsBuild = true;
		}

		public string GetJson()
		{
			var i = 0;
			var s = _result;
			foreach (Match match in _matches)
			{
				s = s.Remove(match.Index, match.Length).Insert(match.Index, _parameters[i++].ToString());
			}
			return s;
		}
	}

	public class BoolQuery<T>
	{
		private readonly Query<T> _query;
		internal BoolQuery(Query<T> query)
		{
			_query = query;
		}

		public BoolQuery<T> Must(string field, int value)
		{
			if (!_query.IsBuild)
			{
				_query.Json.WritePropertyName("must");
				_query.Json.WriteStartObject();
				_query.Json.WritePropertyName(field);
				_query.Json.WriteValue("param");
				_query.Json.WriteEndObject();
			}
			_query.SetNextParam(value);
			return this;
		}
	}
}