using System.Collections.Generic;
using Core.ElasticSearch;
using Expo3.ClientApp.Projections;
using Expo3.Model;
using Expo3.Model.Models;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace Expo3.ClientApp.Services
{
	public class EventService : BaseService
	{
		public EventService(ILoggerFactory loggerFactory, Expo3ElasticConnection settings,
			ElasticScopeFactory<Expo3ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory, user)
		{
		}

		public EventProjection GetEventById(string id)
		{
			return Get<EventProjection>(id.HasNotNullArg("event id"));
		}

		public IReadOnlyCollection<EventSearchProjection> SearchByName(string query)
		{
			return Search<Event, EventSearchProjection>(q => q
				.Match(m => m
					.Field(x => x.Name)
					.Query(query)));
		}

		public IReadOnlyCollection<EventSearchProjection> FilterEventsByNameAndCity(string query, string city)
		{
			return Search<Event, EventSearchProjection>(q => q
				.Bool(b => b
					.Must(Query<Event>.Match(m => m.Field(x => x.Name).Query(query)) &&
					      Query<Event>.Match(m => m.Field(x => x.Address.City).Query(city)))));
		}


		public void RegisterToEvent(string id, string email, string name, string phoneNumber)
		{
			RegisterToEvent(id, new Visitor
			{
				Email = email,
				Name = name,
				PhoneNumber = phoneNumber
			});
		}

		public void RegisterToEvent(string id, Visitor visitor)
		{
			Update(Get<EventProjectionWithVisitors>(id).HasNotNullArg("event"),
				x =>
				{
					x.Visitors.Add(visitor);
					return x;
				}, false);
		}
	}
}