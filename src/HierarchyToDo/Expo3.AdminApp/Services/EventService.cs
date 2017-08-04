using System;
using System.Collections.Generic;
using Core.ElasticSearch;
using Expo3.AdminApp.Projections;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Exceptions;
using Expo3.Model.Models;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace Expo3.AdminApp.Services
{
	public class EventService : BaseExpo3Service
	{
		public EventService(ILoggerFactory loggerFactory, Expo3ElasticConnection settings,
			ElasticScopeFactory<Expo3ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory,
			user)
		{
		}

		public EventProjection GetEvent(string id)
			=> Get<EventProjection>(id.HasNotNullArg("event id"), f => UserQuery<EventProjection>(null));

		/// <exception cref="AddEntityException"></exception>
		public void AddEvent(string name, string caption, DateTime start, DateTime finish, Address address, EEventType type)
			=> Insert(new EventProjection(Get<BaseUserProjection>(User.Id).HasNotNullArg("owner"))
				{
					Name = name,
					Caption = caption,
					StartDateTime = start,
					FinishDateTime = finish,
					Address = address,
					Type = type
				}, true)
				.ThrowIfNot<AddEntityException>();

		///<exception cref="RemoveEntityException"></exception>
		public void RemoveEvent(string id, int version)
			=> Remove(
					Get<EventRemoveProjection>(id.HasNotNullArg("event id"), f => UserQuery<EventProjection>(null))
						.HasNotNullArg("event"), version)
				.ThrowIfNot<RemoveEntityException>();

		///<exception cref="UpdateEntityException"></exception>
		public void UpdateEvent(string id, string name, string caption,
			DateTime startDateTime, DateTime finishDateTime, Address address, EEventType type)
			=> Update(GetEvent(id).HasNotNullArg("event"), x =>
				{
					x.Name = name;
					x.Caption = caption;
					x.StartDateTime = startDateTime;
					x.FinishDateTime = finishDateTime;
					x.Address = address;
					x.Type = type;
					return x;
				}, true)
				.ThrowIfNot<UpdateEntityException>();
		// TODO: удалил UpdateEvent(event...), т.к. нарушение безопасности доступа к данным происходит - неясно откуда взялась обновляемая проекция

		public IReadOnlyCollection<EventProjection> SearchByName(string query)
			=> Search<Event, EventProjection>(q => q
				.Match(m => m
					.Field(x => x.Name)
					.Query(query)));
	}
}