using System;

namespace Expo3.Model.Embed
{
	public struct EventDateTime
	{
		public DateTime StartDateTime { get; set; }
		public DateTime FinishDateTime { get; set; }
		public string Timezone { get; set; }
	}
}