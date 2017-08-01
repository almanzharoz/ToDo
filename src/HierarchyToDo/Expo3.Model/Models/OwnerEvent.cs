using System;
using Core.ElasticSearch.Domain;
using Expo3.Model.Embed;
using Expo3.Model.Interfaces;
using Expo3.Model.Models;
using Nest;

namespace Expo3.Model
{
	/// <summary>
	/// Настройки мероприятия, доступные только создателю
	/// </summary>
	public class OwnerEvent : BaseEntityWithParentAndVersion<Event>, IModel, IWithCreated
	{
        public string Email { get; set; }
        public DateTime Created { get; set; }
	}
}