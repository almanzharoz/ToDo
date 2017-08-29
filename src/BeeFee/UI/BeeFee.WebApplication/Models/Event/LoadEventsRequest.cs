using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeeFee.WebApplication.Models.Event
{
    public class LoadEventsRequest
    {
        public string Text { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate{ get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        public string City{ get; set; }
        public bool LoadConcert{ get; set; }
        public bool LoadExhibition { get; set; }
        public bool LoadExcursion { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<string> Categories { get; set; }
    }
}
