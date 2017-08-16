using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Mapping;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.ElasticSearch.Serialization
{
	internal class InsertJsonConverter<T> : JsonConverter where T : IProjection
	{
		private readonly IProjectionItem _projection;

		public InsertJsonConverter(IProjectionItem projection)
		{
			_projection = projection;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var sw = new Stopwatch();
			sw.Start();
			writer.WriteStartObject();
			foreach (var property in _projection.Properties.Where(x => x.CanWrite))
			{
				var v = property.GetValue(value);
				if (v == null)
					continue;
				writer.WritePropertyName(property.Name.ToLower());
				var o = JToken.FromObject(new InnerValue(v), serializer);
				o.WriteTo(writer);
			}
			writer.WriteEndObject();
			sw.Stop();
			Console.WriteLine("Insert WriteJson: "+sw.ElapsedMilliseconds);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}

}