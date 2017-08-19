using System;
using System.ComponentModel.DataAnnotations;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;

namespace Expo3.WebApplication.Areas.Org.Models
{
	public class AddEventEditModel
	{
		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }

		public string Category { get; set; }

		[Required(ErrorMessage = "Start date is required")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
		public DateTime StartDateTime { get; set; }

		[Required(ErrorMessage = "Finish date is required")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime FinishDateTime { get; set; }

		[Required(ErrorMessage = "City is required")]
		public string City { get; set; }

		[Required(ErrorMessage = "Address is required")]
		public string Address { get; set; }

		public EEventType Type { get; set; }

		[DataType(DataType.Currency)]
		public string Price { get; set; }

		//public TicketPrice[] Prices { get; set; }
		public EventPage Page { get; set; }
	}
}