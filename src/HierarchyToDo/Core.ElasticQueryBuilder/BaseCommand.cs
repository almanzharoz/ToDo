using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Core.ElasticQueryBuilder
{
	public abstract class BaseCommand
	{
		private static Regex _rx = new Regex("\"param\"", RegexOptions.Compiled);
		private string _result;

		protected List<string> _params = new List<string>();  //json serialized values

		private readonly StringBuilder _hashBuilder = new StringBuilder();

		public string Hash => _hashBuilder.ToString();

		protected List<BaseCommandItem> _items = new List<BaseCommandItem>();


		protected BaseCommand(string baseHash)
		{
			if (String.IsNullOrWhiteSpace(baseHash))
				throw new ArgumentNullException(nameof(baseHash));
			_hashBuilder.Append(baseHash);
		}

		internal void AddParam(object value) => _params.Add(value.ToString()); // add json serialize

		protected void AddItem<TItem>(TItem item) where TItem : BaseCommandItem
		{
			_hashBuilder.Append(item.Hash);
			_items.Add(item);
		}

		internal void Build()
		{
			using (var stream = new MemoryStream())
			using (var writer = new StreamWriter(stream, Encoding.UTF8))
			using (var jsonWriter = new JsonTextWriter(writer))
			{
				jsonWriter.WriteStartObject();
				foreach (var query in _items)
					query.Build(jsonWriter);
				jsonWriter.WriteEndObject();

				jsonWriter.Flush();
				stream.Position = 0;
				using (var reader = new StreamReader(stream))
					_result = reader.ReadToEnd();
			}
		}

		internal string GetJson(BaseCommand command)
		{
			if (Hash != command.Hash)
				throw new Exception("Commands hashes not equals");
			var i = 0;
			var s = _result;
			var p = command.GetParams();
			_rx.Replace(_result, match => p[i++]);
			return s;
		}

		internal string[] GetParams()
		{
			var result = _params.ToArray();
			_params = null;
			return result;
		}

	}
}