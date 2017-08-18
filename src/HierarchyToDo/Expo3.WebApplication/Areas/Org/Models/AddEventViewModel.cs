using System;
using System.ComponentModel.DataAnnotations;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;

namespace Expo3.WebApplication.Areas.Org.Models
{
	public class AddEventViewModel
	{
		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }
		public string Category { get; set; }
		[Required(ErrorMessage = "Date is required")]
		public DateTime StartDateTime { get; set; }
		[Required(ErrorMessage = "Date is required")]
		public DateTime FinishDateTime { get; set; }
		[Required(ErrorMessage = "Address is required")]
		public string Address { get; set; }
		[Required]
		public EEventType Type { get; set; }
		//public TicketPrice[] Prices { get; set; }
		//public EventPage Page { get; set; }
		public string Page { get; set; }
}
}