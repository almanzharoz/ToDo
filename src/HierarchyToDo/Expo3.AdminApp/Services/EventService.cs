﻿using System;
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

		public EventProjection GetEvent(string id) => GetWithVersion<EventProjection>(id);

		public BaseCategoryProjection GetCategory(string id) => Get<BaseCategoryProjection>(id);

		///<exception cref="RemoveEntityException"></exception>
		public void RemoveEvent(string id, int version)
			=> Remove<EventProjection>(id, version, true)
				.ThrowIfNot<RemoveEntityException>();

		public IReadOnlyCollection<EventProjection> SearchByName(string query)
			=> Search<Event, EventProjection>(q => q
				.Match(m => m
					.Field(x => x.Name)
					.Query(query)));

		public bool SetCategoryToEvent(string eventId, string categoryId, int version)
			=> Update<EventProjection>(eventId, version,
				u => u.ChangeCategory(
					GetCategory(categoryId.HasNotNullArg("new category id")).HasNotNullArg("new category")),
				true);
	}
}