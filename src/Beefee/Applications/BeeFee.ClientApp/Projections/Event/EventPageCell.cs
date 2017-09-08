
namespace BeeFee.ClientApp.Projections.Event
{
	public struct EventPageCell
	{
		public string Caption { get; }
		public string Cover { get; }

		public string Date { get; }
		public string Category { get; }

		public EventPageCell(string caption, string cover, string date, string category)
		{
			Caption = caption;
			Cover = cover;
			Date = date;
			Category = category;
		}
	}
}