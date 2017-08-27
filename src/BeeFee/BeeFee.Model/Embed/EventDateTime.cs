using System;

namespace BeeFee.Model.Embed
{
	public struct EventDateTime
	{
		public DateTime Start { get; set; }
		public DateTime Finish { get; set; }
		public string Timezone { get; set; }

		public EventDateTime(DateTime start, DateTime finish)
		{
			Start = start;
			Finish = finish;
			Timezone = null;
		}

		public override string ToString()
			=> String.Concat(Start.ToString("g"), " - ", Finish.ToString("g"));
	}
}