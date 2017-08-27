using Expo3.AdminApp.Services;
using Expo3.WebApplication.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Expo3.WebApplication.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "admin")]
	public class EventsController : BaseController<EventService>
	{
		public EventsController(EventService service) : base(service)
		{
		}
	}
}