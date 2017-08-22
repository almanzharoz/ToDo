using System;
using System.Collections.Generic;
using System.Linq;
using Core.ElasticSearch;
using Expo3.ClientApp.Projections;
using Expo3.ClientApp.Projections.Event;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace Expo3.ClientApp.Services
{
    public class EventService : BaseExpo3Service
    {
        public EventService(ILoggerFactory loggerFactory, Expo3ElasticConnection settings,
            ElasticScopeFactory<Expo3ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory, user)
        {
        }

        public EventProjection GetEventById(string id)
            => Get<EventProjection>(id.HasNotNullArg("event id"));

        public IReadOnlyCollection<EventSearchProjection> SearchByName(string query)
            => Search<Event, EventSearchProjection>(q => q
                .Match(m => m
                    .Field(x => x.Name)
                    .Query(query)));

        public IReadOnlyCollection<EventSearchProjection> FilterEventsByNameAndCity(string query, string city)
            => Search<Event, EventSearchProjection>(q => q
                .Bool(b => b
                    .Must(Query<Event>.Match(m => m.Field(x => x.Name).Query(query)) &&
                          Query<Event>.Match(m => m.Field(x => x.Address.City).Query(city)))));

        public EventProjection GetEventPageById(string id)
            => Get<EventProjection>(id.HasNotNullArg("event id"));

        public Pager<EventSearchProjection> SearchEvents(string query=null, string city=null, List<string> categories=null, List<EEventType> types=null, DateTime? startDateTime=null, DateTime? endDateTime=null, decimal? maxPrice=null, int pageSize = 9999, int pageIndex=0)
        {
            List<QueryContainer> qs = new List<QueryContainer>();
            if (!string.IsNullOrEmpty(query))
            {
                qs.Add(Query<Event>.Bool(d => d.Should(r => r.Match(m => m.Field(x => x.Name).Query(query)),
                                        r => r.Match(m => m.Field(x => x.Page.Html).Query(query)))));
            }
            if (categories != null && categories.Any())
            {
                qs.Add(Query<Event>.Terms(m => m.Field(x => x.Category).Terms(categories)));
            }
            if (types != null && types.Any())
            {
                qs.Add(Query<Event>.Terms(m => m.Field(x => x.Type).Terms(types)));
            }
            if (!string.IsNullOrEmpty(city))
            {
                qs.Add(Query<Event>.Match(m => m.Field(x => x.Address.City).Query(city)));
            }
            if (startDateTime.HasValue)
            {
                qs.Add(Query<Event>.DateRange(m => m.Field(x => x.DateTime.Start).GreaterThanOrEquals(startDateTime.Value)));
            }
            if (endDateTime.HasValue)
            {
                qs.Add(Query<Event>.DateRange(m => m.Field(x => x.DateTime.Finish).LessThanOrEquals(endDateTime.Value)));
            }
            if (maxPrice.HasValue)
            {
                qs.Add(Query<Event>.Range(m => m.Field(x => x.Prices.First().Price).LessThanOrEquals((double)maxPrice.Value)));
            }
            return SearchPager<Event, EventSearchProjection>(q => q
                .Bool(b => b
                    .Must(qs.ToArray())), pageIndex, pageSize, null, false);
        }


        //public void RegisterToEvent(string id, string email, string name, string phoneNumber)
        //	=> RegisterToEvent(id, new Visitor
        //	{
        //		Email = email,
        //		Name = name,
        //		PhoneNumber = phoneNumber
        //	});

        //public void RegisterToEvent(string id, Visitor visitor)
        //	=> Update(Get<EventProjectionWithVisitors>(id).HasNotNullArg("event"),
        //		x =>
        //		{
        //			x.Visitors.Add(visitor);
        //			return x;
        //		}, false);

        //TODO сделать агргегацию посредством эластика+кеширование
        public IReadOnlyCollection<string> GetAllCities()
            => Search<Event, EventAddressProjections>(q => q).Select(a => a.Address.City).Distinct().OrderBy(c => c).ToArray();
    }
}