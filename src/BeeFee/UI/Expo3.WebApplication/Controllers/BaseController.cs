using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Expo3.Model;

namespace Expo3.WebApplication.Controllers
{
    public abstract class BaseController<TService> : Controller where TService : BaseExpo3Service
	{
		protected readonly TService _service;

		protected BaseController(TService service)
		{
			_service = service;
		}
    }
}