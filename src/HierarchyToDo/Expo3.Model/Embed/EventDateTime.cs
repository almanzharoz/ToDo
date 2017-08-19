using System;

namespace Expo3.Model.Embed
{
	public struct EventDateTime
	{
		public DateTime Start { get; set; }
		public DateTime Finish { get; set; }
		public string Timezone { get; set; }

		public override string ToString()
			=> String.Concat(Start.ToString("g"), " - ", Finish.ToString("g"));
	}
}