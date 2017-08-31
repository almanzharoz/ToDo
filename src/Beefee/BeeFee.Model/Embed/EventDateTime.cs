using System;
using Newtonsoft.Json;

namespace BeeFee.Model.Embed
{
	public struct EventDateTime
	{
		[JsonProperty]
		public DateTime Start { get; private set; }
		[JsonProperty]
		public DateTime Finish { get; private set; }
		[JsonProperty]
		public string Timezone { get; private set; }

		public EventDateTime(DateTime start, DateTime finish, string timezone = null)
		{
			Start = start;
			Finish = finish;
			Timezone = timezone;
			if (start >= finish) throw new IndexOutOfRangeException("start >= finish");
		}

		public override string ToString()
			=> String.Concat(Start.ToString("g"), " - ", Finish.ToString("g"));
	}
}