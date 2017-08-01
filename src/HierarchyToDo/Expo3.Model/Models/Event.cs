using System;
using Core.ElasticSearch.Domain;
using Expo3.Model.Embed;
using Expo3.Model.Interfaces;
using Nest;

namespace Expo3.Model.Models
{
	/// <summary>
	/// Модель мероприятия для фильтрации и отображения ячеек
	/// </summary>
    public abstract class Event : BaseEntityWithVersion, IModel, IProjection, IWithName
    {
        public string Name { get; set; }
        public string Caption { get; set; }
        [Keyword]
        public User Owner { get; set; }
		[Keyword(Index = false)]
	    public string Cover { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime FinishDateTime { get; set; }
		public Address Address { get; set; }
        public EEventType Type { get; set; }
        public TicketPrice[] Prices { get; set; }
		public Visitor[] Visitors { get; set; }
    }
}