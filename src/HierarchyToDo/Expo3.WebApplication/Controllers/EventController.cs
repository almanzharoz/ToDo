using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Expo3.Model.Embed;
using Expo3.ClientApp.Services;
using Expo3.WebApplication.Models.Event;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharpFuncExt;

namespace Expo3.WebApplication.Controllers
{
    public class EventController : BaseController<EventService>
    {

        public EventController(EventService service) : base(service)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Events()
        {
            var model = new EventFilterViewModel();
            var nowTime = DateTime.UtcNow;
            model.StartDate = new DateTime(nowTime.Year, nowTime.Month, 1);
            model.EndDate = new DateTime(nowTime.Year, nowTime.Month + 1, 1).AddDays(-1);
            model.Cities = _service.GetAllCities().ToList();
            model.Categories = _service.GetCategories().Select(c => new SelectListItem() { Value = c.Id, Text = c.Name }).ToList();
            return View(model);
        }

        public IActionResult Event(string id)
        {
	        var model = _service.GetEventByUrl(id);
				//.IfNotNullOrDefault(m => new EventPageModel() { Caption = m.Name, Date = m.DateTime.ToString(), Html = m.Page.Html, Title = m.Page.Title });
				// здесь не нужна ViewMode, т.к. EventPage итак выполняет эту роль
            if (model == null)
                return NotFound();
            return View(model);
        }

        [HttpGet]
        public IActionResult LoadEvents(LoadEventsRequest request)
        {
            List<EEventType> types = new List<EEventType>();
            if (request.LoadConcert)
                types.Add(EEventType.Concert);
            if (request.LoadExcursion)
                types.Add(EEventType.Excursion);
            if (request.LoadExhibition)
                types.Add(EEventType.Exhibition);
            var events = _service.SearchEvents(request.Text, request.City, request.Categories, types, request.StartDate, request.EndDate, request.MaxPrice, request.PageSize, request.PageIndex).Select(e => new EventListModel() { Title = e.Name, DateTimeString = e.DateTime.ToString(), Id = e.Id, ImageUrl = "" }).ToList();
            return Json(new { events = events });
        }
    }
}