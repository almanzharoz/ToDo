using System;
using System.Linq;
using BeeFee.ClientApp.Services;
using BeeFee.Model.Projections;
using BeeFee.Model.Services;
using BeeFee.WebApplication.Models.Event;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeeFee.WebApplication.Controllers
{
    public class HomeController : BaseController<EventService>
    {
        public HomeController(EventService service, CategoryService categoryService) : base(service, categoryService)
        {
        }

        public IActionResult Index()
        {
            //return RedirectToAction("Events", "Event");
            var model = new EventFilterViewModel();
            var nowTime = DateTime.UtcNow;
            model.StartDate = new DateTime(nowTime.Year, nowTime.Month, 1);
            model.EndDate = new DateTime(nowTime.Year, nowTime.Month + 1, 1).AddDays(-1);
            model.Cities = _service.GetAllCities().ToList();
            model.Categories = _categoryService.GetAllCategories<CategoryProjection>().Select(c => new SelectListItem() { Value = c.Id, Text = c.Name }).ToList();
            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

	    public IActionResult FileTest()
	    {
		    return View();
	    }

    }
}
