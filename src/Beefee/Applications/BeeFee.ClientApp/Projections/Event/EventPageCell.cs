using Newtonsoft.Json;

namespace BeeFee.ClientApp.Projections.Event
{
	public struct EventPageCell
	{
		[JsonProperty]
		public string Caption { get; private set; }
		[JsonProperty]
		public string Cover { get; private set; }

		[JsonProperty]
		public string Date { get; private set; }
		[JsonProperty]
		public string Category { get; private set; }
	}
}