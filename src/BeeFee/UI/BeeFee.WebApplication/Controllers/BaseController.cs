using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BeeFee.Model;

namespace BeeFee.WebApplication.Controllers
{
    public abstract class BaseController<TService> : Controller where TService : BaseBeefeeService
	{
		protected readonly TService _service;

		protected BaseController(TService service)
		{
			_service = service;
		}
    }
}