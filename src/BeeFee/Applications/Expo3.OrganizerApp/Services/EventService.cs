using System;
using System.Collections.Generic;
using System.Linq;
using Core.ElasticSearch;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Exceptions;
using Expo3.Model.Models;
using Expo3.OrganizerApp.Projections;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace Expo3.OrganizerApp.Services
{
	public class EventService : BaseExpo3Service
	{
		public EventService(ILoggerFactory loggerFactory, Expo3ElasticConnection settings,
			ElasticScopeFactory<Expo3ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory, user)
		{
		}

		public EventProjection GetEvent(string id)
			=> GetWithVersion<Event, EventProjection>(id, f => UserQuery<EventProjection>(null));

		/// <exception cref="AddEntityException"></exception>
		// TODO: добавить проверку пользователя
		public void AddEvent(string name, EventDateTime dateTime, Address address, EEventType type,
			Category category, TicketPrice[] prices, string html)
			=> Insert(new NewEvent(Get<BaseUserProjection>(User.Id).HasNotNullArg("owner"))
				{
					Name = name.Trim(),
					//DateTime = dateTime,
					//Address = address,
					//Type = type,
					//Category = category,
					//Prices = prices,
					//Page = new EventPage { Address = address, Caption = name.Trim(), Category = category.Name, Date = dateTime.ToString(), Title = name.Trim(), Html = html}
				}, true)
				.ThrowIfNot<AddEntityException>();

		///<exception cref="RemoveEntityException"></exception>
		public void RemoveEvent(string id, int version)
			// TODO: Добавить проверку пользователя
			=> Remove<EventProjection>(id, version, true);

		///<exception cref="UpdateEntityException"></exception>
		public void UpdateEvent(string id, string name, EventDateTime dateTime, Address address, EEventType type,
			string categoryId, TicketPrice[] prices, string html, int version)
			=> Update<EventProjection>(id, version, x => x.Change(name, dateTime, address, type, Get<BaseCategoryProjection>(categoryId.HasNotNullArg("category")), prices, html), true)
				.ThrowIfNot<UpdateEntityException>();

		public IReadOnlyCollection<EventProjection> GetMyEvents() 
			=> Filter<Event, EventProjection>(q => UserQuery<EventProjection>(null));
	}
}