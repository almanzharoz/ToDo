using System;
using System.Collections.Generic;
using Core.ElasticSearch;
using Expo3.AdminApp.Projections;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Exceptions;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace Expo3.AdminApp.Services
{
    public class EventService : BaseService
    {
        public EventService(ILoggerFactory loggerFactory, Expo3ElasticConnection settings,
            ElasticScopeFactory<Expo3ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory,
            user)
        {
        }

	    /// <exception cref="AddEntityException"></exception>
	    public void AddEvent(string name, string caption, Price[] prices, DateTime start,
		    DateTime finish, Address address, EventPage eventPage, EEventType type, string email)
		    => Insert(new Event
			    {
				    Name = name,
				    Caption = caption,
				    Created = DateTime.Now,
				    User = Get<BaseUserProjection>(User.Id),
				    Prices = prices,
				    StartDateTime = start,
				    FinishDateTime = finish,
				    Address = address,
				    EventPage = eventPage,
				    Type = type,
				    Email = email
			    }, false)
			    .ThrowIfNot<AddEntityException>();

        ///<exception cref="RemoveEntityException"></exception>
        public void RemoveEvent(EventRemoveProjection eventRemoveProjection)
			=> Remove(eventRemoveProjection.HasNotNullArg("event")).ThrowIfNot<RemoveEntityException>();

        ///<exception cref="RemoveEntityException"></exception>
        public void RemoveEvent(string id)
			=> RemoveEvent(Get<EventRemoveProjection>(id));

	    ///<exception cref="UpdateEntityException"></exception>
	    public void UpdateEvent(EventProjection eventUpdateProjection, string name, string caption, Price[] prices,
		    DateTime startDateTime, DateTime finishDateTime, Address address, EventPage eventPage, EEventType type,
		    string email)
		    => Update(eventUpdateProjection.HasNotNullArg("event"), x =>
			    {
				    x.Name = name;
				    x.Caption = caption;
				    x.Prices = prices;
				    x.StartDateTime = startDateTime;
				    x.FinishDateTime = finishDateTime;
				    x.Address = address;
				    x.EventPage = eventPage;
				    x.Type = type;
				    x.Email = email;
				    return x;
			    }, false)
			    .ThrowIfNot<UpdateEntityException>();

        ///<exception cref="UpdateEntityException"></exception>
        public void UpdateEvent(string id, string name, string caption, Price[] prices,
            DateTime startDateTime, DateTime finishDateTime, Address address, EventPage eventPage, EEventType type,
            string email)
			=> UpdateEvent(Get<EventProjection>(id), name, caption, prices, startDateTime, finishDateTime, address, eventPage, type, email);

        public IReadOnlyCollection<EventProjection> SearchByName(string query) => 
            Search<Event, EventProjection>(q => q
                .Match(m => m
                    .Field(x => x.Name)
                    .Query(query)));
    }
}