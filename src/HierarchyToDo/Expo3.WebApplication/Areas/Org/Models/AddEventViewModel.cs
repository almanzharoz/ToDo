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

		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime StartDateTime { get; set; }

		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime FinishDateTime { get; set; }

		[Required(ErrorMessage = "Address is required")]
		public string Address { get; set; }

		public EEventType Type { get; set; }

		//public TicketPrice[] Prices { get; set; }
		public EventPage Page { get; set; }
	}
}