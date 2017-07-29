using System;
using Core.ElasticSearch;
using Expo3.AdminApp.Projections;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Exceptions;
using Microsoft.Extensions.Logging;

namespace Expo3.AdminApp.Services
{
    public class EventService : BaseService
    {
        public EventService(ILoggerFactory loggerFactory, ElasticConnection settings,
            ElasticScopeFactory<ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory,
            user)
        {
            
        }

        /// <exception cref="AddEntityException"></exception>
        public void AddEvent(string name, string caption, Price[] prices, DateTime start,
            DateTime finish, Address address, EventPage eventPage, EEventType type, string email)
        {
            var result = Insert(new Event
            {
                Name = name,
                Caption = caption,
                Created = DateTime.Now,
                User = Get<User>(User.Id),
                Prices = prices,
                StartDateTime = start,
                FinishDateTime = finish,
                Address = address,
                EventPage = eventPage,
                Type = type,
                Email = email
            }, false);

            if(!result) throw new AddEntityException();
        }

        ///<exception cref="RemoveEntityException"></exception>
        public void RemoveEvent(EventRemoveProjection eventRemoveProjection)
        {
            var result = Remove(eventRemoveProjection);
            if(!result) throw new RemoveEntityException();
        }

        ///<exception cref="RemoveEntityException"></exception>
        public void RemoveEvent(string id)
        {
            RemoveEvent(Get<EventRemoveProjection>(id));
        }

        ///<exception cref="UpdateEntityException"></exception>
        public void UpdateEvent(EventProjection eventUpdateProjection, string name, string caption, Price[] prices,
            DateTime startDateTime, DateTime finishDateTime, Address address, EventPage eventPage, EEventType type,
            string email)
        {
            eventUpdateProjection.Name = name;
            eventUpdateProjection.Caption = caption;
            eventUpdateProjection.Prices = prices;
            eventUpdateProjection.StartDateTime = startDateTime;
            eventUpdateProjection.FinishDateTime = finishDateTime;
            eventUpdateProjection.Address = address;
            eventUpdateProjection.EventPage = eventPage;
            eventUpdateProjection.Type = type;
            eventUpdateProjection.Email = email;

            var result = Update(eventUpdateProjection, false);
            if (!result) throw new UpdateEntityException();
        }

        ///<exception cref="UpdateEntityException"></exception>
        public void UpdateEvent(string id, string name, string caption, Price[] prices,
            DateTime startDateTime, DateTime finishDateTime, Address address, EventPage eventPage, EEventType type,
            string email)
        {
            UpdateEvent(Get<EventProjection>(id), name, caption, prices, startDateTime, finishDateTime, address, eventPage, type, email);
        }
    }
}