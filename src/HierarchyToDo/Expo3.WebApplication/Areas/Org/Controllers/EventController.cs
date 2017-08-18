using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Expo3.OrganizersApp.Services;
using Expo3.WebApplication.Areas.Org.Models;
using Expo3.WebApplication.Controllers;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;

namespace Expo3.WebApplication.Areas.Org.Controllers
{
	[Area("Org")]
    public class EventController : BaseController<EventService>
    {
	    public EventController(EventService service) : base(service)
	    {
	    }

		//public IActionResult Index()
		//{
		//    return View();
		//}

	    public IActionResult Index()
	    {
		    return View(_service.GetMyEvents());
	    }

		[HttpGet]
	    public IActionResult Add()
	    {
		    return View(new AddEventViewModel());
	    }

	    [HttpPost]
	    public IActionResult Add(AddEventViewModel addEventViewModel)
	    {
		    if (ModelState.IsValid)
		    {
			    _service.AddEvent(
				    addEventViewModel.Name,
				    new EventDateTime() /* {StartDateTime = model.StartDateTime, FinishDateTime = model.FinishDateTime}*/,
				    new Address {AddressString = addEventViewModel.Address},
				    addEventViewModel.Type,
				    null,
				    addEventViewModel.Page);
			    return RedirectToAction("Index");
		    }

		    return View(addEventViewModel);
	    }
    }
}