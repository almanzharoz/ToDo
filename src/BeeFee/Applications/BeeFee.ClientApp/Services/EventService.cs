using System;
using System.Collections.Generic;
using System.Linq;
using Core.ElasticSearch;
using BeeFee.ClientApp.Projections;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace BeeFee.ClientApp.Services
{
    public class EventService : BaseBeefeeService
    {
        public EventService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings,
            ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory, user)
        {
        }

		public EventProjection GetEventByUrl(string url)
			=> Filter<EventProjection>(f => f.Term(p => p.Url, url.HasNotNullArg("event url"))).SingleOrDefault();

		//public IReadOnlyCollection<EventSearchProjection> SearchByName(string query)
		//    => Search<Event, EventSearchProjection>(q => q
		//        .Match(m => m
		//            .Field(x => x.Name)
		//            .Query(query)));

		//public IReadOnlyCollection<EventSearchProjection> FilterEventsByNameAndCity(string query, string city)
		//    => Search<Event, EventSearchProjection>(q => q
		//        .Bool(b => b
		//            .Must(Query<Event>.Match(m => m.Field(x => x.Name).Query(query)) &&
		//                  Query<Event>.Match(m => m.Field(x => x.Address.City).Query(city)))));

		public Pager<EventCellProjection> SearchEvents(string query=null, string city=null, List<string> categories=null, List<EEventType> types=null, DateTime? startDateTime=null, DateTime? endDateTime=null, decimal? maxPrice=null, int pageSize = 9999, int pageIndex=0)
        {
            List<QueryContainer> qc = new List<QueryContainer>();
            if (!string.IsNullOrEmpty(query))
            {
                qc.Add(Query<Event>.Bool(d => d.Should(r => r.Match(m => m.Field(x => x.Name).Query(query)),
                                        r => r.Match(m => m.Field(x => x.Page.Html).Query(query)))));
            }
            List<QueryContainer> qf = new List<QueryContainer>();
            if (categories != null && categories.Any())
            {
                qf.Add(Query<Event>.Terms(m => m.Field(x => x.Category).Terms(categories)));
            }
            if (types != null && types.Any())
            {
                qf.Add(Query<Event>.Terms(m => m.Field(x => x.Type).Terms(types)));
            }
            if (!string.IsNullOrEmpty(city))
            {
                qf.Add(Query<Event>.Match(m => m.Field(x => x.Address.City).Query(city)));
            }
            if (startDateTime.HasValue)
            {
                qf.Add(Query<Event>.DateRange(m => m.Field(x => x.DateTime.Start).GreaterThanOrEquals(startDateTime.Value)));
            }
            if (endDateTime.HasValue)
            {
                qf.Add(Query<Event>.DateRange(m => m.Field(x => x.DateTime.Finish).LessThanOrEquals(endDateTime.Value)));
            }
            if (maxPrice.HasValue)
            {
                qf.Add(Query<Event>.Range(m => m.Field(x => x.Prices.First().Price).LessThanOrEquals((double)maxPrice.Value)));
            }
            return SearchPager<Event, EventCellProjection>(q => q
                .Bool(b => b
                    .Must(qc.ToArray()).Filter(qf.ToArray())), pageIndex, pageSize, null, false);
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
		// скорее всего сделаем справочник городов
        public IReadOnlyCollection<string> GetAllCities()
            => Filter<Event, EventAddressProjections>(q => q).Select(a => a.Address.City).Distinct().OrderBy(c => c).ToArray();

    }
}