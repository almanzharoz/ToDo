using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Core.ElasticQueryBuilder
{
	public abstract class BaseCommandItem
	{
		private readonly StringBuilder _hashBuilder = new StringBuilder();
		protected List<BaseCommandItem> _items = new List<BaseCommandItem>();

		public string Hash => _hashBuilder.ToString().Trim();
		protected readonly Action<object> _addParam;

		protected BaseCommandItem(string baseHash, Action<object> addParam)
		{
			if (String.IsNullOrWhiteSpace(baseHash))
				throw new ArgumentNullException(nameof(baseHash));
			_hashBuilder.Append(baseHash);
			_addParam = addParam;
		}

		protected void AddItem<TItem>(TItem item) where TItem : BaseCommandItem
		{
			_hashBuilder.Append(item.Hash);
			_items.Add(item);
		}

		internal abstract void Build(JsonWriter writer);

		protected void BuildHelper(JsonWriter writer, string propertyName)
		{
			writer.WritePropertyName(propertyName);
			writer.WriteStartObject();
			foreach (var query in _items)
				query.Build(writer);
			writer.WriteEndObject();
		}

		protected void BuildHelper(JsonWriter writer, string propertyName, Action<JsonWriter> buildAction)
		{
			writer.WritePropertyName(propertyName);
			writer.WriteStartObject();
			if (_items.Any())
				throw new Exception("Items must empty");
			buildAction(writer);
			writer.WriteEndObject();
		}
	}
}