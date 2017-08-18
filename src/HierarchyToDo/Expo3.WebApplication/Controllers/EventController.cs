using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Expo3.Model.Embed;
using Expo3.ClientApp.Services;
using Expo3.WebApplication.Models.Event;
using Microsoft.AspNetCore.Mvc;

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
            return View(model);
        }

        [HttpGet]
        public IActionResult LoadEvents(LoadEventsRequest request)
        {
            List<EEventType> typies = new List<EEventType>();
            if (request.LoadConcert)
                typies.Add(EEventType.Concert);
            if (request.LoadExcursion)
                typies.Add(EEventType.Excursion);
            if (request.LoadExhibition)
                typies.Add(EEventType.Exhibition);
            var events = _service.FilterEvents(request.Text, request.City, typies, request.StartDate, request.EndDate, request.MaxPrice, request.PageSize, request.PageIndex).Select(e=>new EventListModel(){Title = e.Name, DateTimeString = e.});
            return Json(events);
        }
    }
}