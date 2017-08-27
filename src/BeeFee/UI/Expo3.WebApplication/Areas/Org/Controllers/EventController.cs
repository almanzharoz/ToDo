using System;
using System.Linq;
using Expo3.Model.Embed;
using Expo3.OrganizerApp.Services;
using Expo3.WebApplication.Areas.Org.Models;
using Expo3.WebApplication.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Expo3.WebApplication.Areas.Org.Controllers
{
	[Area("Org")]
	[Authorize(Roles = "organizer")]
	public class EventController : BaseController<EventService>
    {
	    public EventController(EventService service) : base(service)
	    {
	    }

	    public IActionResult Index()
	    {
		    return View(_service.GetMyEvents());
	    }

		[HttpGet]
	    public IActionResult Add()
		{
			return View(new AddEventEditModel
			{
				StartDateTime = DateTime.Now,
				FinishDateTime = DateTime.Now.AddDays(1),
				LoadedCategories = _service.GetCategories().Select(c => new SelectListItem { Value = c.Id, Text = c.Name }).ToList()
			});
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
					addEventEditModel.Caption,
				    addEventEditModel.Html);
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
				    new EventDateTime {Start = addEventEditModel.StartDateTime, Finish = addEventEditModel.FinishDateTime},
				    new Address {City = addEventEditModel.City, AddressString = addEventEditModel.Address},
				    addEventEditModel.Type,
				    null, //TODO: работа с категориями
				    new[] {new TicketPrice {Price = new Price(addEventEditModel.Price)}},
					addEventEditModel.Caption,
				    addEventEditModel.Html, addEventEditModel.Version);
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