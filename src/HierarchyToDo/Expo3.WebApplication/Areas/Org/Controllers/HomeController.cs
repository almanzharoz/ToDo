using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Expo3.WebApplication.Areas.Org.Controllers
{
	[Area("Org")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}