using System;
using Core.ElasticSearch;
using Expo3.AdminApp.Projections;
using Expo3.Model;
using Expo3.Model.Helpers;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace Expo3.AdminApp.Services
{
    public class EventService : BaseService
    {
        public EventService(ILoggerFactory loggerFactory, ElasticConnection settings,
            ElasticScopeFactory<ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory,
            user)
        {
            
        }

        public void AddEvent(string name, string caption, Price startPrice, Price finishPrice, DateTime start,
            DateTime finish, Address address, EventPage eventPage, EEventType type, string email) =>
            Insert(new Event
            {
                Name = name,
                Caption = caption,
                Created = DateTime.Now,
                User = Get<User>(User.Id),
                StartPrice = startPrice,
                FinishPrice = finishPrice,
                StartDateTime = start,
                FinishDateTime = finish,
                Address = address,
                EventPage = eventPage,
                Type = type,
                Email = email
            });
    }
}