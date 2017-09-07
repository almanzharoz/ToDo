using System;
using System.Linq;
using Core.ElasticSearch.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpFuncExt;

namespace Core.ElasticSearch.Serialization
{
	internal class JoinConverter<T> : JsonConverter where T: class, IProjection, IJoinProjection
	{
		private readonly (ObjectActivator<T> func, string[] parameters) _creator;
		public JoinConverter()
		{
			_creator = typeof(T).GetConstructors().FirstOrDefault().IfNotNullOrDefault(ObjectActivator.GetActivator<T>);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (String.IsNullOrWhiteSpace(((T)value).Id))
				throw new ArgumentNullException("Id");
			writer.WriteValue(((T)value).Id);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value == null)
				return null;
			var container = ((CoreElasticContractResolver) serializer.ContractResolver).Container;
			if (reader.Value is string)
				return container.GetOrAdd<T>(_creator.func(reader.Value as string));
			throw new Exception("Not read join"); // TODO: косяк здесь
		}

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}
	}
}