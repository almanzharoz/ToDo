using Expo3.AdminApp.Services;
using Expo3.WebApplication.Areas.Admin.Models;
using Expo3.WebApplication.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Expo3.WebApplication.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
    public class CategoriesController : BaseController<CategoryService>
    {
	    public CategoriesController(CategoryService service) : base(service)
	    {
	    }

		public IActionResult Index()
        {
            return View(new AddCategoryListModel(_service.GetAllCategories()));
        }

	    [HttpPost]
	    public IActionResult Add(string name)
	    {
		    _service.Add(name);
		    return RedirectToAction("Index");
	    }

	    [HttpPost]
	    public IActionResult Remove(string id)
	    {
		    _service.Remove(id);
		    return RedirectToAction("Index");
	    }
    }
}