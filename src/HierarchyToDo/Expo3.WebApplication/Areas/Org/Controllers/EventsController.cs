using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Expo3.OrganizersApp.Services;
using Expo3.WebApplication.Areas.Org.Models;
using Expo3.WebApplication.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;

namespace Expo3.WebApplication.Areas.Org.Controllers
{
	[Area("Org")]
	[Authorize(Roles = "organizer")]
	public class EventsController : BaseController<EventService>
    {
	    public EventsController(EventService service) : base(service)
	    {
	    }

	    public IActionResult Index()
	    {
		    return View(_service.GetMyEvents());
	    }

		[HttpGet]
	    public IActionResult Add()
	    {
		    return View(new AddEventEditModel() {StartDateTime = DateTime.Now, FinishDateTime = DateTime.Now.AddDays(1)});
	    }

	    [HttpPost]
	    public IActionResult Add(AddEventEditModel addEventEditModel)
	    {
		    if (ModelState.IsValid)
		    {
			    _service.AddEvent(
				    addEventEditModel.Name,
				    new EventDateTime {Start = addEventEditModel.StartDateTime, Finish = addEventEditModel.FinishDateTime},
				    new Address {City = addEventEditModel.City, AddressString = addEventEditModel.Address},
				    addEventEditModel.Type,
					null, //TODO: работа с категориями
					new[] {new TicketPrice {Price = new Price(addEventEditModel.Price)} }, 
				    addEventEditModel.Page);
			    return RedirectToAction("Index");
		    }

		    return View(addEventEditModel);
	    }

	    [HttpGet]
	    public IActionResult Edit(string id)
	    {
		    return View(new AddEventEditModel(_service.GetEvent(id)));
	    }

	    [HttpPost]
	    public IActionResult Edit(AddEventEditModel addEventEditModel)
	    {
		    if (ModelState.IsValid)
		    {
			    _service.UpdateEvent(
					addEventEditModel.Id,
					addEventEditModel.Name,
					new EventDateTime() { Start = addEventEditModel.StartDateTime, Finish = addEventEditModel.FinishDateTime},
					new Address() { City = addEventEditModel.City, AddressString = addEventEditModel.Address},
					addEventEditModel.Type,
					null, //TODO: работа с категориями
					new[] {new TicketPrice() {Price = new Price(addEventEditModel.Price)}},
					addEventEditModel.Page
					);
			    return RedirectToAction("Index");
		    }
		    return View(addEventEditModel);
	    }

	    public IActionResult Remove(string id, int version)
	    {
			_service.RemoveEvent(id, version);
		    return RedirectToAction("Index");
	    }
    }
}