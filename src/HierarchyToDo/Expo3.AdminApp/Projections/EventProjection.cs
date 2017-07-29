using System;
using Core.ElasticSearch.Domain;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Interfaces;

namespace Expo3.AdminApp.Projections
{
    public class EventProjection : BaseEntityWithVersion, IProjection<Event>, IGetProjection, IUpdateProjection, ISearchProjection, IWithName
    {
        public string Name { get; set; }
        public string Caption { get; set; }
        public Price[] Prices { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime FinishDateTime { get; set; }
        public Address Address { get; set; }
        public EventPage EventPage { get; set; }
        public EEventType Type { get; set; }
        public string Email { get; set; }
    }

    public class EventRemoveProjection : BaseEntityWithVersion, IProjection<Event>, IRemoveProjection, IGetProjection
    {
    }
}