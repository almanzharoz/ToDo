using Newtonsoft.Json;

namespace Core.Tests.Projections
{
	public class DeserializeProjection
	{
		[JsonProperty]
		public string Name { get; private set; }
		[JsonProperty]
		public int Value { get; private set; }

		public DeserializeProjection(string name, int value)
		{
			Name = name;
			Value = value;
		}
	}
}