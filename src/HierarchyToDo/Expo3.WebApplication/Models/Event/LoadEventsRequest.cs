using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Expo3.WebApplication.Models.Event
{
    public class LoadEventsRequest
    {
        public string Text { get; set; }
        public DateTime StartDate{ get; set; }
        public DateTime EndDate { get; set; }
        public string City{ get; set; }
        public bool LoadConcert{ get; set; }
        public bool LoadExhibition { get; set; }
        public bool LoadExcursion { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
