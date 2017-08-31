using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeeFee.WebApplication.Models.Event
{
    public class EventFilterViewModel
    {
        public List<string> Cities { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public DateTime StartDate{ get; set; }
        public DateTime EndDate { get; set; }
    }
}
