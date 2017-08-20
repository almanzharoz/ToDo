using System;
using System.Collections.Generic;
using System.Linq;
using Core.ElasticSearch;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Exceptions;
using Expo3.Model.Models;
using Expo3.OrganizersApp.Projections;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace Expo3.OrganizersApp.Services
{
	public class EventService : BaseExpo3Service
	{
		public EventService(ILoggerFactory loggerFactory, Expo3ElasticConnection settings,
			ElasticScopeFactory<Expo3ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory, user)
		{
		}

		public EventProjection GetEvent(string id)
			//=> Get<EventProjection>(id.HasNotNullArg("event id"), f => UserQuery<EventProjection>(null));
			=> Search<Event, EventProjection>(q => UserQuery<EventProjection>(q.Ids(x => x.Values(id))))
				.FirstOrDefault();

		/// <exception cref="AddEntityException"></exception>
		// TODO: добавить проверку пользователя
		public void AddEvent(string name, EventDateTime dateTime, Address address, EEventType type,
			Category category, TicketPrice[] prices, EventPage page, bool refresh = false)
			=> Insert(new EventProjection(Get<BaseUserProjection>(User.Id).HasNotNullArg("owner"))
				{
					Name = name,
					DateTime = dateTime,
					Address = address,
					Type = type,
					Category = category,
					Prices = prices,
					Page = page
				}, refresh)
				.ThrowIfNot<AddEntityException>();

		///<exception cref="RemoveEntityException"></exception>
		public void RemoveEvent(string id, int version)
			//=> Remove(
			//		Get<EventRemoveProjection>(id.HasNotNullArg("event id"), f => UserQuery<EventProjection>(null))
			//			.HasNotNullArg("event"), version)
			//	.ThrowIfNot<RemoveEntityException>();
			=> Remove(
				GetEvent(id)
					.HasNotNullArg("event")
					.ThrowIf(x => x.Version != version, x => new RemoveEntityException()), version);

		///<exception cref="UpdateEntityException"></exception>
		public void UpdateEvent(string id, string name, EventDateTime dateTime, Address address, EEventType type,
			Category category, TicketPrice[] prices, EventPage page, bool refresh = false)
			=> Update(GetEvent(id).HasNotNullArg("event"), x =>
				{
					x.Name = name;
					x.DateTime = dateTime;
					x.Address = address;
					x.Type = type;
					x.Category = category;
					x.Prices = prices;
					x.Page = page;
					return x;
				}, refresh)
				.ThrowIfNot<UpdateEntityException>();

		public IReadOnlyCollection<EventProjection> GetMyEvents() 
			=> Filter<Event, EventProjection>(q => UserQuery<EventProjection>(null));
	}
}