using System;
using System.Collections.Generic;
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
			=> Get<EventProjection>(id.HasNotNullArg("event id"), f => UserQuery<EventProjection>(null));

		/// <exception cref="AddEntityException"></exception>
		// TODO: добавить проверку пользователя
		public void AddEvent(string name, EventDateTime dateTime, Address address, EEventType type,
			Category category, EventPage page, bool refresh = false)
			=> Insert(new EventProjection(Get<BaseUserProjection>(User.Id).HasNotNullArg("owner"))
				{
					Name = name,
					DateTime = dateTime,
					Address = address,
					Type = type,
					Category = category,
					Page = page
				}, refresh)
				.ThrowIfNot<AddEntityException>();
		
		///<exception cref="RemoveEntityException"></exception>
		public void RemoveEvent(string id, int version)
			=> Remove(
					Get<EventRemoveProjection>(id.HasNotNullArg("event id"), f => UserQuery<EventProjection>(null))
						.HasNotNullArg("event"), version)
				.ThrowIfNot<RemoveEntityException>();

		///<exception cref="UpdateEntityException"></exception>
		public void UpdateEvent(string id, string name, string caption,
			EventDateTime dateTime, Address address, EEventType type, bool refresh = false)
			=> Update(GetEvent(id).HasNotNullArg("event"), x =>
				{
					x.Name = name;
					x.DateTime = dateTime;
					x.Address = address;
					x.Type = type;
					return x;
				}, false)
				.ThrowIfNot<UpdateEntityException>();

		public IReadOnlyCollection<EventProjection> GetMyEvents() 
			=> Filter<Event, EventProjection>(q => UserQuery<EventProjection>(null));
	}
}