using System;
using System.Collections.Generic;
using System.Linq;
using Core.ElasticSearch;
using Expo3.ClientApp.Projections;
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

        public Pager<EventSearchProjection> FilterEvents(string query, string city, List<EEventType> typies, DateTime? startDateTime, DateTime? endDateTime, decimal? maxPrice, int pageSize, int pageIndex)
        {
            QueryContainer qc = (new QueryContainer());
            if (!string.IsNullOrEmpty(query))
            {
                qc = qc && Query<Event>.Match(m => m.Field(x => x.Name).Query(query)) && Query<Event>.Match(m => m.Field(x => x.Page.Html).Query(query));
            }
            if (typies != null && typies.Any())
            {
                qc = qc && Query<Event>.Terms(m => m.Field(x => x.Type).Terms(typies));
            }
            if (!string.IsNullOrEmpty(city))
            {
                qc = qc && Query<Event>.Match(m => m.Field(x => x.Address.City).Query(city));
            }
            if (startDateTime.HasValue)
            {
                qc = qc && Query<Event>.DateRange(m => m.Field(x => x.DateTime).GreaterThanOrEquals(startDateTime.Value));
            }
            if (endDateTime.HasValue)
            {
                qc = qc && Query<Event>.DateRange(m => m.Field(x => x.DateTime).LessThanOrEquals(endDateTime.Value));
            }
            if (maxPrice.HasValue)
            {
                qc = qc && Query<Event>.Range(m => m.Field(x => x.Prices.).LessThanOrEquals(maxPrice.Value));
            }
            return SearchPager<Event, EventSearchProjection>(q => q
                .Bool(b => b
                    .Must(qc)), pageIndex, pageSize, null, false);
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

        public IReadOnlyCollection<string> GetAllCities()
            => Search<Event, OnlyAddressEventProjections>(q => q).Select(a => a.Address.City).Distinct().OrderBy(c => c).ToArray();
    }
}